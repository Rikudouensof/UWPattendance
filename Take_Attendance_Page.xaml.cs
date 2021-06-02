using Microsoft.Azure.CognitiveServices.Vision.Face;
using Microsoft.Azure.CognitiveServices.Vision.Face.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using UWPattendance.Models;
using UWPattendance.Tasks;
using UWPattendance.ViewModels;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Imaging;
using Windows.Media.Capture;
using Windows.Media.SpeechSynthesis;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace UWPattendance
{
  /// <summary>
  /// An empty page that can be used on its own or navigated to within a Frame.
  /// </summary>
  public sealed partial class Take_Attendance_Page : Page
  {
    public static string imagepathHolder = "", imageName = " ";
    public List<string> detectInfo;
    public static string personGroupId = Guid.NewGuid().ToString();
    public static double maxNumber;
    public static string ENDPOINT = GetFaceRecognitionKeys.Endpoint;
    public static string RECOGNITION_MODEL4 = RecognitionModel.Recognition04;
    public static string SUBSCRIPTION_KEY = GetFaceRecognitionKeys.Key;
    // From your Face subscription in the Azure portal, get your subscription key and endpoint.

   
     


    // Used for all examples.
    // URL for the images.
    const string IMAGE_BASE_URL = "https://csdx.blob.core.windows.net/resources/Face/Images/";
    List<string> imageNames;
    public List<InPutDictlist> personDictionary = new List<InPutDictlist>();
    private Dictionary<string, string[]> here;

    public Take_Attendance_Page()
    {
      this.InitializeComponent();
      string _dbPath = Database_Connection._dbpath;



      var db = new SQLiteConnection(_dbPath);
      db.CreateTable<Models.Person>();
      var allpeople = db.Table<Models.Person>();
      var filterImageName = db.Table<Models.Person>().Select(m => m.ImageName).ToList();
      imageNames = filterImageName;


      foreach (var item in allpeople)
      {
        string[] answer = new string[] { item.ImageName};
        InPutDictlist inPutDictlist = new InPutDictlist()
        {
          Name = item.Id.ToString(),
          ImageName = answer
          
        
        };
       personDictionary.Add(inPutDictlist);
      }


    }


    private async void readText(string mytext)
    {
      MediaElement mediaplayer = new MediaElement();
      using (var speech = new Windows.Media.SpeechSynthesis.SpeechSynthesizer())
      {
        speech.Voice = SpeechSynthesizer.AllVoices.First(gender => gender.Gender == VoiceGender.Female);
        string ssml = @"<speak version='1.0' " + "xmlns='http://www.w3.org/2001/10/synthesis' xml:lang='en-UK'>" + mytext + "</speak>";
        SpeechSynthesisStream stream = await speech.SynthesizeSsmlToStreamAsync(ssml);
        mediaplayer.SetSource(stream, stream.ContentType);
        mediaplayer.Play();
      }
    }

    private async void Detect_Person_Button_Click(object sender, RoutedEventArgs e)
    {

      var fileinfo = new FileInfo(imagepathHolder);
      BlobConstructors.UploadFile(fileinfo, BlobConstructors.Blobconnectionstring, BlobConstructors.containername2);

      
      
      
      
      
      IFaceClient client = CompareFaceConstructor.Authenticate(ENDPOINT, SUBSCRIPTION_KEY);

      // Find Similar - find a similar face from a list of faces.
     // var result = await  CompareFaceConstructor.FindSimilar(client, BlobConstructors.blobBaseUrl, imageName, BlobConstructors.blobBaseUrl2, RecognitionModel.Recognition04, imageNames);
      string _dbPath = Database_Connection._dbpath;








      //Verify(client, IMAGE_BASE_URL, RECOGNITION_MODEL4).Wait();

      //// Identify - recognize a face(s) in a person group (a person group is created in this example).
      ///
      GetRecognisedUserViewModel Identity = new GetRecognisedUserViewModel();
      try
      {
      Identity = await CompareFaceConstructor.IdentifyInPersonGroup(client, personDictionary, BlobConstructors.blobBaseUrl, BlobConstructors.blobBaseUrl2, imageName, RECOGNITION_MODEL4);
        string Message = "";
        var db = new SQLiteConnection(_dbPath);
        var dz = new SQLiteConnection(_dbPath);
        db.CreateTable<Models.Person>();
        db.CreateTable<Models.Attendance>();
        //var p = db.Table<Models.Person>().Where(m => m.ImageName == result.Result).FirstOrDefault();
        int way = int.Parse(Identity.highestperson.Name);
        var p = db.Table<Models.Person>().Where(m => m.Id == way).FirstOrDefault();
        var Count = db.Table<Models.Attendance>().Count();
        //Books_Label.Text = result.Output.ToString();
        //readText(result.ToString());
        if (Identity.ConfidenceLevel > 0.7)
        {

          Message = "Welcome back " + p.LastName + " " + p.FirstName + " , You have been logged in. Have a pleasant day!";
          Books_Label.Text = Identity.Answers;
          Attendance attendance = new Attendance()
          {
            Id = Count + 1,
            FirstName = p.FirstName,
            Date_Signed_In_Date_and_Time = DateTime.UtcNow,
            LastName = p.LastName,
            User_ID = p.Id
          };
          db.Insert(attendance);

          readText(Message);
        }
        else
        {
          var confipercentage = Identity.ConfidenceLevel * 100;
          Message = "I am not comfortable this is who i think this is. i am only " + confipercentage + " percent sure, that this is" + p.FirstName + " " + p.LastName + ".";
          readText(Message);
        }



      }
      catch 
      {

        readText("Face not detected from Image or Server cannot be reached");
      }


      //// LargePersonGroup - create, then get data.
      //LargePersonGroup(client, IMAGE_BASE_URL, RECOGNITION_MODEL4).Wait();
      //// Group faces - automatically group similar faces.
      //Group(client, IMAGE_BASE_URL, RECOGNITION_MODEL4).Wait();











      
      
    }




    /*
 * FIND SIMILAR
 * This example will take an image and find a similar one to it in another image.
 */

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

      var imagename = Guid.NewGuid().ToString();
      await photo.CopyAsync(destinationFolder, imagename, NameCollisionOption.FailIfExists);
      var a = photo.Path;
      imagepathHolder = a.Replace("\\", "/");
      imageName = photo.Name;

      Detect_Person_Button.Visibility = Visibility.Visible;


      //  await photo.DeleteAsync();

    }



  }
}
          
