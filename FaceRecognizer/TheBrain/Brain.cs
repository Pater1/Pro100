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


        public static async Task<TFTensor> ImageToTensorAsync(Bitmap bm)
        {
            int w = bm.Width;
            int h = bm.Height;

            int[,,] raw = new int[w,h,4];

            for(int x = 0; x < w; x++)
            {
                for(int y = 0; y < h; y++)
                {
                    Color c = bm.GetPixel(x, y);
                    raw[x, y, 0] = c.R;
                    raw[x, y, 1] = c.G;
                    raw[x, y, 2] = c.B;
                    raw[x, y, 3] = c.A;
                }
            }

            TFTensor tensor = new TFTensor(raw);

            return tensor;
        }
    }
}
