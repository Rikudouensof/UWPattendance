using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;

namespace UWPattendance.Tasks
{
    public class RenderFaces
    {
        public static readonly Scalar _faceColorBrush = new Scalar(0, 0, 255);


        public static Mat RenderFace(Rect[] faces, Mat image)
        {
            Mat result = image.Clone();

            foreach (Rect face in faces)
            {
                Cv2.Rectangle(result, face.TopLeft, face.BottomRight, _faceColorBrush, 4);

            }

            return result;
        }

       
    }

    
}
