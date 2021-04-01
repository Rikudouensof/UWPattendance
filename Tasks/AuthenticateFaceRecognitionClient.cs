using Microsoft.Azure.CognitiveServices.Vision.Face;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UWPattendance.Tasks
{
    class AuthenticateFaceRecognitionClient
    {
        /*
 *	AUTHENTICATE
 *	Uses subscription key and region to create a client.
 */
        public static IFaceClient Authenticate(string endpoint, string key)
        {
            return new FaceClient(new ApiKeyServiceClientCredentials(key)) { Endpoint = endpoint };
        }
    }
}
