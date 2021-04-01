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


// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace UWPattendance
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Add_People_Page : Page
    {


        string _dbPath = Database_Connection._dbpath;
        string imagepath = "";


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

            await photo.CopyAsync(destinationFolder, "Walder.jpg", NameCollisionOption.FailIfExists);
            string a = photo.Path;
            imagepath = a.Replace("\\", "/");

            //  await photo.DeleteAsync();

            User_Image.Visibility = Visibility.Visible;
            Add_Person_Button.Visibility = Visibility.Visible;
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
                DateRegistered = DateTime.UtcNow

            };
            db.Insert(person);
            popupmessage = person.LastName + " " + person.FirstName + " is Saved";


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
                LastName_Entry.Text = " ";
                FirstName_Entry.Text = " ";
                User_Image.Visibility = Visibility.Collapsed;
                Capture_Person_Button.Visibility = Visibility.Collapsed;
                Add_Person_Button.Visibility = Visibility.Collapsed;
                this.Frame.Navigate(typeof(People_Page));
            }
            else
            {
                //skip your task  
            }

          
        }

       
    }
}
