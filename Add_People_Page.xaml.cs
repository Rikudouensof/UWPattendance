using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Media.Capture;
using Windows.Storage.Pickers;
using Windows.Storage;
using UWPattendance.Tasks;
using System.Threading.Tasks;

using Windows.Storage.Streams;
using Windows.Graphics.Imaging;
using SQLite;
using UWPattendance.Models;
using Windows.UI.Popups;
using Windows.Media.SpeechSynthesis;
using Microsoft.Extensions.Configuration;


// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace UWPattendance
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Add_People_Page : Page
    {

    private string allitem;
    string _dbPath = Database_Connection._dbpath;
        string imagepath = "",imageName = "";
    SpeechSynthesizer speech;


        #region Variables


        #endregion
        public Add_People_Page()
        {
            this.InitializeComponent();
        }


       

        private void FirstName_Entry_TextChanged(object sender, TextChangedEventArgs e)
        {
            Capture_Person_Button.Visibility = Visibility.Visible;
        }


        private async void Capture_Person_Button_Click(object sender, RoutedEventArgs e)
        {
            CameraCaptureUI captureUI = new CameraCaptureUI();
            captureUI.PhotoSettings.Format = CameraCaptureUIPhotoFormat.Jpeg;
            captureUI.PhotoSettings.CroppedSizeInPixels = new Windows.Foundation.Size(200, 200);

            StorageFile photo = await captureUI.CaptureFileAsync(CameraCaptureUIMode.Photo);

            if (photo == null)
            {
                // User cancelled photo capture
                return;
            }
            IRandomAccessStream stream = await photo.OpenAsync(FileAccessMode.Read);
            BitmapDecoder decoder = await BitmapDecoder.CreateAsync(stream);
            SoftwareBitmap softwareBitmap = await decoder.GetSoftwareBitmapAsync();
            StorageFolder destinationFolder =
             await ApplicationData.Current.LocalFolder.CreateFolderAsync("ProfilePhotoFolder",
                CreationCollisionOption.OpenIfExists);
            SoftwareBitmap softwareBitmapBGR8 = SoftwareBitmap.Convert(softwareBitmap,
        BitmapPixelFormat.Bgra8,
        BitmapAlphaMode.Premultiplied);

            SoftwareBitmapSource bitmapSource = new SoftwareBitmapSource();
            await bitmapSource.SetBitmapAsync(softwareBitmapBGR8);

            User_Image.Source = bitmapSource;

            string name = LastName_Entry.Text + " "+ FirstName_Entry.Text + ".jpg";

            await photo.CopyAsync(destinationFolder, name, NameCollisionOption.GenerateUniqueName);
            string a = photo.Path;
      imageName = photo.Name;
            imagepath = a.Replace("\\", "/");

            //  await photo.DeleteAsync();

            User_Image.Visibility = Visibility.Visible;
            Add_Person_Button.Visibility = Visibility.Visible;
        }


   



    private async void readText(string mytext)
    {
      MediaElement mediaplayer = new MediaElement();
      using (var speech = new SpeechSynthesizer())
      {
        speech.Voice = SpeechSynthesizer.AllVoices.First(gender => gender.Gender == VoiceGender.Female);
        string ssml = @"<speak version='1.0' " + "xmlns='http://www.w3.org/2001/10/synthesis' xml:lang='en-UK'>" + allitem + "</speak>";
        SpeechSynthesisStream stream = await speech.SynthesizeSsmlToStreamAsync(ssml);
        mediaplayer.SetSource(stream, stream.ContentType);
        mediaplayer.Play();
      }
    }

    private async void Add_Person_Button_Click(object sender, RoutedEventArgs e)
        {
            string popupmessage = "";
            var db = new SQLiteConnection(_dbPath);
            db.CreateTable<Person>();


            var maximumPrimaryKey = db.Table<Person>().OrderByDescending(m => m.Id).FirstOrDefault();

            Person person = new Person()
            {
                Id = (maximumPrimaryKey == null ? 1 : maximumPrimaryKey.Id + 1),
                LastName = LastName_Entry.Text,
                FirstName = FirstName_Entry.Text,
                ImagePath = imagepath,
                DateRegistered = DateTime.UtcNow,
                ImageName = imageName

            };
            
            popupmessage = person.LastName + " " + person.FirstName + " will be saved";


            //PopUp
            MessageDialog showDialog = new MessageDialog(popupmessage);
            showDialog.Commands.Add(new UICommand("Yes")
            {
                Id = 0
            });
            showDialog.Commands.Add(new UICommand("No")
            {
                Id = 1
            });
            showDialog.DefaultCommandIndex = 0;
            showDialog.CancelCommandIndex = 1;
            var result = await showDialog.ShowAsync();
            if ((int)result.Id == 0)
            {
        db.Insert(person);
        allitem = "The user " + LastName_Entry.Text + " " + FirstName_Entry.Text + ", has been added to the system. You can now sign "+FirstName_Entry.Text +" in for today, or you can go to the people page, to see all users.";
        readText(allitem);
        var fileinfo = new FileInfo(imagepath);
        BlobConstructors.UploadFile(fileinfo,BlobConstructors.Blobconnectionstring,BlobConstructors.containername);

        LastName_Entry.Text = " ";
                FirstName_Entry.Text = " ";
                User_Image.Visibility = Visibility.Collapsed;
                Capture_Person_Button.Visibility = Visibility.Collapsed;
                Add_Person_Button.Visibility = Visibility.Collapsed;

       



                

            }
            else
            {
        allitem = "The user " + LastName_Entry.Text + " " + FirstName_Entry.Text + ", has not been added to the system.";
        readText(allitem);
      }

          
        }

       
    }
}
