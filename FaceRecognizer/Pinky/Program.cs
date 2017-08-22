using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PythonPlumber;
using System.Drawing;

namespace Pinky
{
    class Program
    {
        static void Main(string[] args)
        {
            Bitmap pic = Bitmap.FromFile("E:\\Neumont\\C#\\Pro100\\FaceRecognizer\\Pinky\\testPerson.jpg") as Bitmap;
            PyPlumber py = new PyPlumber() {
                ExecutablePath = "\"E:\\Neumont\\C#\\Pro100\\FaceRecognizer\\PythonClassifierApplication1\\classifier.py\"",
                CachePath = "E:\\Neumont\\C#\\Pro100\\FaceRecognizer\\Pinky\\TestImages"
            };

            py.RegisterImage_Result(pic);
            py.RegisterImage_Training(pic, "Bill");
            string result = py.Run();

            Console.WriteLine(result);
        }
    }
}
