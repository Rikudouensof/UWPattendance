using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UWPattendance.Tasks
{
    public class CascadeClassifiers
    {

        public static CascadeClassifier InitializeFaceClassifier()
        {
            return new CascadeClassifier("Assets/haarcascade_frontalface_alt.xml");
        }
    }
}
