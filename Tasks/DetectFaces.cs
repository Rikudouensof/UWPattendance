using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UWPattendance.Tasks
{
    public class DetectFaces
    {

        public static Rect[] DetectFace(CascadeClassifier cascadeClassifier, Mat image)
        {
            return cascadeClassifier
                .DetectMultiScale(image, 1.1, 3, HaarDetectionTypes.ScaleImage, new Size(150, 150));
        }
    }
}
