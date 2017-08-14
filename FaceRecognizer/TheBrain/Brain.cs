using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TensorFlow;

using System.Drawing;

namespace TheBrain
{
    public class Brain
    {
        private TFSession session = new TFSession();
        private TFGraph brain;

        //data_format = [batch, in_height, in_width, in_channels]
        public Brain()
        {
            #region try 1
            //brain = new TFGraph();
            //var x = brain.Variable(new TFOutput());

            //var c0 = brain.Const(new TFTensor(new int[] { 25, 25, 4, 1 }));
            //x = brain.Conv2D(x, c0, new long[] { 1, 1, 1, 1 }, "VALID");

            //var c1 = brain.Const(new TFTensor(new int[] { 25, 25, 1, 1 }));
            //x = brain.Conv2D(x, c1, new long[] { 1, 1, 1, 1 }, "VALID");

            //var c2 = brain.Const(new TFTensor(new int[] { 5, 5, 1, 1 }));
            //x = brain.Conv2D(x, c2, new long[] { 1, 1, 1, 1 }, "SAME");
            #endregion

            #region try 2
            //"inceptionModel"
            //brain.ReadFile("inceptionModel");
            #endregion
        }

        //public static async Task<TFTensor> ImagesToTensorAsync(IEnumerable<Bitmap> bm)
        //{
        //    int w = bm.Width;
        //    int h = bm.Height;

        //    int[,,,] raw = new int[0, w, h, 4];

        //    for (int x = 0; x < w; x++)
        //    {
        //        for (int y = 0; y < h; y++)
        //        {
        //            Color c = bm.GetPixel(x, y);
        //            raw[0, x, y, 0] = c.R;
        //            raw[0, x, y, 1] = c.G;
        //            raw[0, x, y, 2] = c.B;
        //            raw[0, x, y, 3] = c.A;
        //        }
        //    }

        //    TFTensor tensor = new TFTensor(raw);

        //    return tensor;
        //}
        public static async Task<TFTensor> ImageToTensorAsync(Bitmap bm)
        {
            int w = bm.Width;
            int h = bm.Height;

            int[,,,] raw = new int[0, w, h, 4];

            for (int x = 0; x < w; x++)
            {
                for (int y = 0; y < h; y++)
                {
                    Color c = bm.GetPixel(x, y);
                    raw[0, x, y, 0] = c.R;
                    raw[0, x, y, 1] = c.G;
                    raw[0, x, y, 2] = c.B;
                    raw[0, x, y, 3] = c.A;
                }
            }

            TFTensor tensor = new TFTensor(raw);

            return tensor;
        }
        //public static async Task<Bitmap> TensorToImageAsync(TFTensor tnsr)
        //{
        //    int[,,] raw = (int[,,])tnsr.GetValue();
        //    return null;
        //}
    }
}
