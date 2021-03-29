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
using Windows.UI.Xaml.Media.Imaging;
using OpenCvSharp;
using System.Threading.Tasks;

using Windows.Storage.Streams;
using Windows.Graphics.Imaging;
using Windows.Media.Capture;
using Windows.Storage;


// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace UWPattendance
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Add_People_Page : Page
    {
        private OpenCvSharp.CascadeClassifier _haarCascade;

        #region Variables


        #endregion
        public Add_People_Page()
        {
            this.InitializeComponent();
            _haarCascade = CascadeClassifiers.InitializeFaceClassifier();
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
            await photo.CopyAsync(destinationFolder, "ProfilePhoto.jpg", NameCollisionOption.ReplaceExisting);


            var imagepath = photo.CopyAsync(destinationFolder, "ProfilePhoto.jpg", NameCollisionOption.ReplaceExisting);
            await photo.DeleteAsync();
        }
    }
}
