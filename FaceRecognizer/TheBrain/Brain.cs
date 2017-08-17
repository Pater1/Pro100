using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Net;
using TensorFlow;
using Mono.Options;

namespace TheBrain
{
    public class Brain
    {
        private TFSession session = new TFSession();
        public TFGraph brain;

        static string dir, modelFile, labelsFile;
      //  static bool jagged = true;

        //data_format = [batch, in_height, in_width, in_channels]
        public Brain()
        {
            #region try 3
            brain = new TFGraph();
            byte[] model = File.ReadAllBytes("inceptionModel");
            #endregion
        }

        public void Run()
        {
            List<string> files = new List<string>();
            if (dir == null)
            {
                dir = "/tmp";
            }

            //if (files == null || files.Count == 0)
            //	Error ("No files were specified");

            if (files.Count == 0)
                files = new List<string>() { "/tmp/demo.jpg" };

            //ModelFiles(dir);

            using (session = new TFSession(brain))
            {
                var labels = File.ReadAllLines(labelsFile);

                foreach (var file in files)
                {
                    // Run inference on the image files
                    // For multiple images, session.Run() can be called in a loop (and
                    // concurrently). Alternatively, images can be batched since the model
                    // accepts batches of image data as input.
                    TFTensor tensor = CreateTensorFromImageFile(file);

                    TFSession.Runner runner = session.GetRunner();
                    runner.AddInput(brain["input"][0], tensor).Fetch(brain["output"][0]);
                    TFTensor[] output = runner.Run();
                    // output[0].Value() is a vector containing probabilities of
                    // labels for each image in the "batch". The batch size was 1.
                    // Find the most probably label index.

                    TFTensor result = output[0];
                    long[] rshape = result.Shape;
                    if (result.NumDims != 2 || rshape[0] != 1)
                    {
                        string shape = "";
                        foreach (var d in rshape)
                        {
                            shape += $"{d} ";
                        }
                        shape = shape.Trim();
                        Console.WriteLine($"Error: expected to produce a [1 N] shaped tensor where N is the number of labels, instead it produced one with shape [{shape}]");
                        Environment.Exit(1);
                    }

                    // You can get the data in two ways, as a multi-dimensional array, or arrays of arrays, 
                    // code can be nicer to read with one or the other, pick it based on how you want to process
                    // it
                    bool jagged = true;

                    int bestIdx = 0;
                    float /*p = 0,*/ best = 0;

                    if (jagged)
                    {
                        float[] probabilities = ((float[][])result.GetValue(jagged: true))[0];
                        for (int i = 0; i < probabilities.Length; i++)
                        {
                            if (probabilities[i] > best)
                            {
                                bestIdx = i;
                                best = probabilities[i];
                            }
                        }

                    }
                    else
                    {
                        float[,] val = (float[,])result.GetValue(jagged: false);

                        // Result is [1,N], flatten array
                        for (int i = 0; i < val.GetLength(1); i++)
                        {
                            if (val[0, i] > best)
                            {
                                bestIdx = i;
                                best = val[0, i];
                            }
                        }
                    }

                    Console.WriteLine($"{file} best match: [{bestIdx}] {best * 100.0}% {labels[bestIdx]}");
                }
            }
        }

        // Convert the image in filename to a Tensor suitable as input to the Inception model.
        static TFTensor CreateTensorFromImageFile(string file)
        {
            return CreateTensorFromImageFile(File.ReadAllBytes(file));
        }
        static TFTensor CreateTensorFromImageFile(byte[] file)
        {
            // DecodeJpeg uses a scalar String-valued tensor as input.
            var tensor = TFTensor.CreateString(file);

            TFGraph graph;
            TFOutput input, output;

            // Construct a graph to normalize the image
            ConstructGraphToNormalizeImage(out graph, out input, out output);

            // Execute that graph to normalize this one image
            using (var session = new TFSession(graph))
            {
                var normalized = session.Run(
                         inputs: new[] { input },
                         inputValues: new[] { tensor },
                         outputs: new[] { output });

                return normalized[0];
            }
        }

        // The inception model takes as input the image described by a Tensor in a very
        // specific normalized format (a particular image size, shape of the input tensor,
        // normalized pixel values etc.).
        //
        // This function constructs a graph of TensorFlow operations which takes as
        // input a JPEG-encoded string and returns a tensor suitable as input to the
        // inception model.
        static void ConstructGraphToNormalizeImage(out TFGraph graph, out TFOutput input, out TFOutput output)
        {
            // Some constants specific to the pre-trained model at:
            // https://storage.googleapis.com/download.tensorflow.org/models/inception5h.zip
            //
            // - The model was trained after with images scaled to 224x224 pixels.
            // - The colors, represented as R, G, B in 1-byte each were converted to
            //   float using (value - Mean)/Scale.

            const int W = 224;
            const int H = 224;
            const float Mean = 117;
            const float Scale = 1;

            graph = new TFGraph();
            input = graph.Placeholder(TFDataType.String);

            output = graph.Div(
                x: graph.Sub(
                    x: graph.ResizeBilinear(
                        images: graph.ExpandDims(
                            input: graph.Cast(
                                graph.DecodeJpeg(contents: input, channels: 3), DstT: TFDataType.Float),
                            dim: graph.Const(0, "make_batch")),
                        size: graph.Const(new int[] { W, H }, "size")),
                    y: graph.Const(Mean, "mean")),
                y: graph.Const(Scale, "scale"));
        }

        //
        // Downloads the inception graph and labels
        //
        static void ModelFiles(string dir)
        {
            string url = "https://storage.googleapis.com/download.tensorflow.org/models/inception5h.zip";

            modelFile = Path.Combine(dir, "tensorflow_inception_graph.pb");
            labelsFile = Path.Combine(dir, "imagenet_comp_graph_label_strings.txt");
            var zipfile = Path.Combine(dir, "inception5h.zip");

            if (File.Exists(modelFile) && File.Exists(labelsFile)) return;

            Directory.CreateDirectory(dir);
            WebClient wc = new WebClient();
            wc.DownloadFile(url, zipfile);
            ZipFile.ExtractToDirectory(zipfile, dir);
            File.Delete(zipfile);
        }

        //public static async Task<TFTensor> ImageToTensorAsync(Bitmap bm)
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
    }
}
