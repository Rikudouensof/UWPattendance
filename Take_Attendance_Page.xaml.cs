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
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Imaging;
using Windows.Media.Capture;
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
    string imagepathHolder = "";
    public List<string> detectInfo;
    public static string personGroupId = Guid.NewGuid().ToString();
    // From your Face subscription in the Azure portal, get your subscription key and endpoint.
    string SUBSCRIPTION_KEY = GetFaceRecognitionKeys.Key;

    string ENDPOINT = GetFaceRecognitionKeys.Endpoint;
    const string RECOGNITION_MODEL4 = RecognitionModel.Recognition04;
    // Used for all examples.
    // URL for the images.
    const string IMAGE_BASE_URL = "https://csdx.blob.core.windows.net/resources/Face/Images/";


    public Take_Attendance_Page()
    {
      this.InitializeComponent();
      string _dbPath = Database_Connection._dbpath;
      List<Models.Person> People;




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
      Detect_Person_Button.Visibility = Visibility.Visible;

      //  await photo.DeleteAsync();

    }
  }
}
          
