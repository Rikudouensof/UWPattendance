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

using Windows.Media.Capture;
using Windows.Storage.Pickers;
using Windows.Storage;
using UWPattendance.Tasks;
using Windows.UI.Xaml.Media.Imaging;
using OpenCvSharp;
using System.Threading.Tasks;


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

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private async void Capture_Button_Click(object sender, RoutedEventArgs e)
        {
            FileOpenPicker openPicker = new FileOpenPicker
            {
                ViewMode = PickerViewMode.Thumbnail,
                SuggestedStartLocation = PickerLocationId.PicturesLibrary
            };

            openPicker.FileTypeFilter.Add(".jpg");
            openPicker.FileTypeFilter.Add(".jpeg");
            openPicker.FileTypeFilter.Add(".png");


            StorageFile file = await openPicker.PickSingleFileAsync();
            if (file == null)
            {
                return;
            }

            var mat = await file.ToMatAsync();
            if (mat == null)
            {
                return;
            }



            var faces = DetectFaces.DetectFace(_haarCascade, mat);
            if (faces.Any() == false)
            {
                User_Image.Source = null;
                return;

            }


            using (var rendered_faces = RenderFaces.RenderFace(faces, mat))
            {
                User_Image.Source = null;
                User_Image.Source = await rendered_faces.ToBitmapImageAsync();
            }

        }

       
    }
}
