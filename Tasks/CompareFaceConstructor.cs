using Microsoft.Azure.CognitiveServices.Vision.Face;
using Microsoft.Azure.CognitiveServices.Vision.Face.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UWPattendance.Models;
using UWPattendance.ViewModels;

namespace UWPattendance.Tasks
{
  public class CompareFaceConstructor
  {


    public static string ENDPOINT = GetFaceRecognitionKeys.Endpoint;
    public static string RECOGNITION_MODEL4 = RecognitionModel.Recognition04;
    public static string SUBSCRIPTION_KEY = GetFaceRecognitionKeys.Key;
    public static string personGroupId = Guid.NewGuid().ToString();


    public static IFaceClient Authenticate(string endpoint, string key)
    {
      return new FaceClient(new ApiKeyServiceClientCredentials(key)) { Endpoint = endpoint };
    }


    IFaceClient client = Authenticate(ENDPOINT, SUBSCRIPTION_KEY);


    public static async Task<List<DetectedFace>> DetectFaceRecognize(IFaceClient faceClient, string url, string recognition_model)
    {
      // Detect faces from image URL. Since only recognizing, use the recognition model 1.
      // We use detection model 3 because we are not retrieving attributes.
      IList<DetectedFace> detectedFaces = await faceClient.Face.DetectWithUrlAsync(url, recognitionModel: recognition_model, detectionModel: DetectionModel.Detection03);
      Console.WriteLine($"{detectedFaces.Count} face(s) detected from image `{Path.GetFileName(url)}`");
      return detectedFaces.ToList();
    }




    public static async Task<AnswerReturns> FindSimilar(IFaceClient client, string url,string sourceImageFileName, string url2 ,string recognition_model, List<string> targetImageFileNames)
    {
      Console.WriteLine("========FIND SIMILAR========");
      Console.WriteLine();



      IList<Guid?> targetFaceIds = new List<Guid?>();
      foreach (var targetImageFileName in targetImageFileNames)
      {
        // Detect faces from target image url.
        var faces = await CompareFaceConstructor.DetectFaceRecognize(client, $"{url}{targetImageFileName}", recognition_model);
        // Add detected faceId to list of GUIDs.
        targetFaceIds.Add(faces[0].FaceId.Value);
      }

      // Detect faces from source image url.
      IList<DetectedFace> detectedFaces = await DetectFaceRecognize(client, $"{url2}{sourceImageFileName}", recognition_model);

      // Find a similar face(s) in the list of IDs. Comapring only the first in list for testing purposes.
      IList<SimilarFace> similarResults = await client.Face.FindSimilarAsync(detectedFaces[0].FaceId.Value, null, null, targetFaceIds);
      double confidencelevelMax = 0.0;
      string output = "", confirmedimageName = "";
      foreach (var similarResult in similarResults)
      {
        if (similarResult.Confidence > confidencelevelMax)
        {
          confirmedimageName = similarResult.FaceId.ToString();
        }

        output = output + " Faces from " + sourceImageFileName + " & ID: " + similarResult.FaceId + " are similar with confidence: " + similarResult.Confidence.ToString() + " .\n";
      }

      AnswerReturns answerReturns = new AnswerReturns()
      {
        Output = output,
        Result = confirmedimageName
      };

     
      return answerReturns;
    }


    public static async Task<GetRecognisedUserViewModel> IdentifyInPersonGroup(IFaceClient client, List< InPutDictlist> personDictionary, string url, string url2, string sourceImageFileName, string recognitionModel)
    {

      GetRecognisedUserViewModel getRecognisedUserViewModel = new GetRecognisedUserViewModel();
      string OutPut = "Initializing..... \n Creating new Personal Model \n ";

      // Create a dictionary for all your images, grouping similar ones under the same key.

      // A group photo that includes some of the persons you seek to identify from your dictionary.
      // string sourceImageFileName = "identification1.jpg";
      // Create a person group. 
      // Console.WriteLine($"Create a person group ({personGroupId}).");

      OutPut = OutPut + "person group identity: " + personGroupId + ". \n";
      await client.PersonGroup.CreateAsync(personGroupId, personGroupId, recognitionModel: recognitionModel);
      // The similar faces will be grouped into a single person group person.
      foreach (var groupedFace in personDictionary)
      {
        // Limit TPS
        await Task.Delay(250);
        Microsoft.Azure.CognitiveServices.Vision.Face.Models.Person person = await client.PersonGroupPerson.CreateAsync(personGroupId ,groupedFace.Name);
        // Console.WriteLine($"Create a person group person '{groupedFace}'.");
         
        // Add face to the person group person.
        foreach (var similarImage in groupedFace.ImageName)
        {
          OutPut = OutPut + "Add face to the person group person( " + groupedFace + ") from image `"+similarImage +"` \n ";
          string urlzz = url + similarImage;
          PersistedFace face = await client.PersonGroupPerson.AddFaceFromUrlAsync(personGroupId, person.PersonId, urlzz, similarImage);
        }
      }
      // Start to train the person group.
      OutPut = OutPut + "\n \n Now training Person Group: " + personGroupId + "\n";
      await client.PersonGroup.TrainAsync(personGroupId);

      // Wait until the training is completed.
      while (true)
      {
        await Task.Delay(1000);
        var trainingStatus = await client.PersonGroup.GetTrainingStatusAsync(personGroupId);
        OutPut = OutPut + "Training status: "+trainingStatus.Status+ ". \n";
        if (trainingStatus.Status == TrainingStatusType.Succeeded) { break; }
      }
      Console.WriteLine();

      List<Guid> sourceFaceIds = new List<Guid>();
      // Detect faces from source image url.
      List<DetectedFace> detectedFaces = await DetectFaceRecognize(client, $"{url2}{sourceImageFileName}", recognitionModel);

      // Add detected faceId to sourceFaceIds.
      foreach (var detectedFace in detectedFaces) { sourceFaceIds.Add(detectedFace.FaceId.Value); }
      // Identify the faces in a person group. 
      var identifyResults = await client.Face.IdentifyAsync(sourceFaceIds, personGroupId);
      double numberstream = 0.0;
      Microsoft.Azure.CognitiveServices.Vision.Face.Models.Person outputperson = new Microsoft.Azure.CognitiveServices.Vision.Face.Models.Person();
     

      foreach (var identifyResult in identifyResults)
      {
        Microsoft.Azure.CognitiveServices.Vision.Face.Models.Person person = await client.PersonGroupPerson.GetAsync(personGroupId, identifyResult.Candidates[0].PersonId);
        OutPut = OutPut + "Person: '"+ person.Name +"' is identified for face in: "+sourceImageFileName +" - "+identifyResult.FaceId +" \n";
        List<Guid> gruildie = new List<Guid>();
        gruildie.Add(identifyResult.FaceId);


        if (identifyResult.Candidates[0].Confidence > numberstream)
        {
          outputperson.Name = person.Name;
          outputperson.PersonId = person.PersonId;
          outputperson.PersistedFaceIds = gruildie;
          getRecognisedUserViewModel.ConfidenceLevel = identifyResult.Candidates[0].Confidence;

        }
      }
      await DeletePersonGroup(client, personGroupId);
      OutPut = OutPut + "\n \n Person Id group deleted.";
      getRecognisedUserViewModel.Answers = OutPut;
      getRecognisedUserViewModel.highestperson = outputperson;
     
      // At end, delete person groups in both regions (since testing only)

      return getRecognisedUserViewModel;
      
    }


    public static async Task DeletePersonGroup(IFaceClient client, String personGroupId)
    {
      await client.PersonGroup.DeleteAsync(personGroupId);
      Console.WriteLine($"Deleted the person group {personGroupId}.");
    }


  }
}
