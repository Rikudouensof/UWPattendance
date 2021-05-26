using Microsoft.Azure.CognitiveServices.Vision.Face;
using Microsoft.Azure.CognitiveServices.Vision.Face.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UWPattendance.Models;

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


    public static async Task IdentifyInPersonGroup(IFaceClient client, string url, string recognitionModel)
    {
      Console.WriteLine("========IDENTIFY FACES========");
      Console.WriteLine();

      // Create a dictionary for all your images, grouping similar ones under the same key.
      Dictionary<string, string[]> personDictionary =
          new Dictionary<string, string[]>
              { { "Family1-Dad", new[] { "Family1-Dad1.jpg", "Family1-Dad2.jpg" } },
              { "Family1-Mom", new[] { "Family1-Mom1.jpg", "Family1-Mom2.jpg" } },
              { "Family1-Son", new[] { "Family1-Son1.jpg", "Family1-Son2.jpg" } },
              { "Family1-Daughter", new[] { "Family1-Daughter1.jpg", "Family1-Daughter2.jpg" } },
              { "Family2-Lady", new[] { "Family2-Lady1.jpg", "Family2-Lady2.jpg" } },
              { "Family2-Man", new[] { "Family2-Man1.jpg", "Family2-Man2.jpg" } }
              };
      // A group photo that includes some of the persons you seek to identify from your dictionary.
      string sourceImageFileName = "identification1.jpg";
      // Create a person group. 
      Console.WriteLine($"Create a person group ({personGroupId}).");
      await client.PersonGroup.CreateAsync(personGroupId, personGroupId, recognitionModel: recognitionModel);
      // The similar faces will be grouped into a single person group person.
      foreach (var groupedFace in personDictionary.Keys)
      {
        // Limit TPS
        await Task.Delay(250);
        Microsoft.Azure.CognitiveServices.Vision.Face.Models.Person person = await client.PersonGroupPerson.CreateAsync(personGroupId: personGroupId, name: groupedFace);
        Console.WriteLine($"Create a person group person '{groupedFace}'.");

        // Add face to the person group person.
        foreach (var similarImage in personDictionary[groupedFace])
        {
          Console.WriteLine($"Add face to the person group person({groupedFace}) from image `{similarImage}`");
          PersistedFace face = await client.PersonGroupPerson.AddFaceFromUrlAsync(personGroupId, person.PersonId,
              $"{url}{similarImage}", similarImage);
        }
      }
      // Start to train the person group.
      Console.WriteLine();
      Console.WriteLine($"Train person group {personGroupId}.");
      await client.PersonGroup.TrainAsync(personGroupId);

      // Wait until the training is completed.
      while (true)
      {
        await Task.Delay(1000);
        var trainingStatus = await client.PersonGroup.GetTrainingStatusAsync(personGroupId);
        Console.WriteLine($"Training status: {trainingStatus.Status}.");
        if (trainingStatus.Status == TrainingStatusType.Succeeded) { break; }
      }
      Console.WriteLine();

      List<Guid?> sourceFaceIds = new List<Guid?>();
      // Detect faces from source image url.
      List<DetectedFace> detectedFaces = await DetectFaceRecognize(client, $"{url}{sourceImageFileName}", recognitionModel);

      // Add detected faceId to sourceFaceIds.
      foreach (var detectedFace in detectedFaces) { sourceFaceIds.Add(detectedFace.FaceId.Value); }
      // Identify the faces in a person group. 
      var identifyResults = await client.Face.IdentifyAsync((IList<Guid>)sourceFaceIds, personGroupId);

      foreach (var identifyResult in identifyResults)
      {
        Microsoft.Azure.CognitiveServices.Vision.Face.Models.Person person = await client.PersonGroupPerson.GetAsync(personGroupId, identifyResult.Candidates[0].PersonId);
        Console.WriteLine($"Person '{person.Name}' is identified for face in: {sourceImageFileName} - {identifyResult.FaceId}," +
            $" confidence: {identifyResult.Candidates[0].Confidence}.");
      }
      Console.WriteLine();

      // At end, delete person groups in both regions (since testing only)
      Console.WriteLine("========DELETE PERSON GROUP========");
      Console.WriteLine();
      DeletePersonGroup(client, personGroupId).Wait();
    }


    public static async Task DeletePersonGroup(IFaceClient client, String personGroupId)
    {
      await client.PersonGroup.DeleteAsync(personGroupId);
      Console.WriteLine($"Deleted the person group {personGroupId}.");
    }


  }
}
