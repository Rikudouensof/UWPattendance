using Microsoft.Azure.CognitiveServices.Vision.Face;
using Microsoft.Azure.CognitiveServices.Vision.Face.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
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
        string infoHolder;
        public List<string> detectInfo;
        public static string personGroupId = Guid.NewGuid().ToString();
        public Take_Attendance_Page()
        {
            this.InitializeComponent();
            string _dbPath = Database_Connection._dbpath;
            List<Person> People;

          


        }


        /* 
 * DETECT FACES
 * Detects features from faces and IDs them.
 */
      


        public static  async Task IdentifyInPersonGroup(IFaceClient client, string url, string recognitionModel)
        {

            string consoleinfo = "Detecting Faces \n";



            string _dbPath = Database_Connection._dbpath;
            List<Models.Person> Perosonz;
            var db = new SQLiteConnection(_dbPath);


            Perosonz = db.Table<Models.Person>().ToList();


            // Create a list of images
            List<string> imageFileNames = Perosonz.Select(m => m.ImagePath).ToList();
            // Create a dictionary for all your images, grouping similar ones under the same key.
            Dictionary<string, string[]> personDictionary =
                new Dictionary<string, string[]>();
                   


            foreach (var item in Perosonz)
            {
                string Name = item.LastName + " " + item.FirstName;
                personDictionary.Add(Name, new[] { item.ImagePath, item.ImagePath });
            }
            // A group photo that includes some of the persons you seek to identify from your dictionary.
            string sourceImageFileName = "identification1.jpg";

            // Create a person group. 
            consoleinfo = consoleinfo + $"Create a person group ({personGroupId}). \n";
            await client.PersonGroup.CreateAsync(personGroupId, personGroupId, recognitionModel: recognitionModel);
            // The similar faces will be grouped into a single person group person.
            foreach (var groupedFace in personDictionary.Keys)
            {
                // Limit TPS
                await Task.Delay(250);
                Person person = await client.PersonGroupPerson.CreateAsync(personGroupId: personGroupId, name: groupedFace);
                Console.WriteLine($"Create a person group person '{groupedFace}'.");

                // Add face to the person group person.
                foreach (var similarImage in personDictionary[groupedFace])
                {
                    consoleinfo = consoleinfo + $"Add face to the person group person({groupedFace}) from image `{similarImage}` \n";
                    PersistedFace face = await client.PersonGroupPerson.AddFaceFromUrlAsync(personGroupId, person.PersonId,
                        $"{url}{similarImage}", similarImage);
                }
            }

            // Start to train the person group.
           
            consoleinfo = consoleinfo + $"Train person group {personGroupId}. \n";
            await client.PersonGroup.TrainAsync(personGroupId);

            // Wait until the training is completed.
            while (true)
            {
                await Task.Delay(1000);
                var trainingStatus = await client.PersonGroup.GetTrainingStatusAsync(personGroupId);
                consoleinfo = consoleinfo + $"Training status: {trainingStatus.Status}. \n";
                if (trainingStatus.Status == TrainingStatusType.Succeeded) { break; }
            }
          

            List<Guid?> sourceFaceIds = new List<Guid?>();
            // Detect faces from source image url.
            List<DetectedFace> detectedFaces = await DetectFaceRecognize(client, $"{url}{sourceImageFileName}", recognitionModel);

            // Add detected faceId to sourceFaceIds.
            foreach (var detectedFace in detectedFaces) { sourceFaceIds.Add(detectedFace.FaceId.Value); }

            // Identify the faces in a person group. 
            var identifyResults = await client.Face.IdentifyAsync(sourceFaceIds, personGroupId);

            foreach (var identifyResult in identifyResults)
            {
                Person person = await client.PersonGroupPerson.GetAsync(personGroupId, identifyResult.Candidates[0].PersonId);
                consoleinfo = consoleinfo + $"Person '{person.Name}' is identified for face in: {sourceImageFileName} - {identifyResult.FaceId}," +
                    $" confidence: {identifyResult.Candidates[0].Confidence}. \n";
            }
          
            
        }



        private static async Task<List<DetectedFace>> DetectFaceRecognize(IFaceClient faceClient, string url, string recognition_model)
        {
            // Detect faces from image URL. Since only recognizing, use the recognition model 1.
            // We use detection model 2 because we are not retrieving attributes.
            IList<DetectedFace> detectedFaces = await faceClient.Face.DetectWithUrlAsync(url, recognitionModel: recognition_model, detectionModel: DetectionModel.Detection02);
            Console.WriteLine($"{detectedFaces.Count} face(s) detected from image `{Path.GetFileName(url)}`");
            return detectedFaces.ToList();
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


            await photo.CopyAsync(destinationFolder, "Walder.jpg", NameCollisionOption.FailIfExists);
            string a = photo.Path;

            //  await photo.DeleteAsync();

           
        }
    }
}