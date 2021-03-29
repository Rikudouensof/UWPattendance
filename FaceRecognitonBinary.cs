using Microsoft.Azure.CognitiveServices.Vision.Face;
using Microsoft.Azure.CognitiveServices.Vision.Face.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UWPattendance
{
    class FaceRecognitonBinary
    {
       
             private static string subscriptionKey = Environment.GetEnvironmentVariable("ea310678004d48d4bdb54eabb8cd51e4");
        // Add your Face endpoint to your environment variables.
        private static string faceEndpoint = Environment.GetEnvironmentVariable("https://eeefacerecognition.cognitiveservices.azure.com/");

        public readonly IFaceClient faceClient = new FaceClient(
        new ApiKeyServiceClientCredentials(subscriptionKey),
        new System.Net.Http.DelegatingHandler[] { });


        // The list of detected faces.
        private IList<DetectedFace> faceList;
        // The list of descriptions for the detected faces.
        private string[] faceDescriptions;
        // The resize factor for the displayed image.
        private double resizeFactor;

        private const string defaultStatusBarText =
            "Place the mouse pointer over a face to see the face description.";
    }
}
