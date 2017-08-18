using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace PythonPlumber
{
    public class PyPlumber
    {
        public string ExecutablePath { get; set; }
        public string CachePath { get; set; }

        private List<int> hashes = new List<int>();
        private List<int> Hashes
        {
            get
            {
                return hashes;
            }
            set
            {
                hashes = value;
                string name = CachePath + "/Test/" + hashes + ".cache";
                using (FileStream fs = new FileStream(name, FileMode.Create, FileAccess.ReadWrite))
                {
                    BinaryFormatter bi = new BinaryFormatter();
                    bi.Serialize(fs, hashes);
                }
            }
        }
        public void RegisterImage_Result(Bitmap img)
        {
            int hash = img.GetHashCode();
            Hashes.Add(hash);

            string name = CachePath + "/Test/" + hash + ".jpg";
            img.Save(name, System.Drawing.Imaging.ImageFormat.Jpeg);
        }

        private Dictionary<int, string> hashToName = new Dictionary<int, string>();
        private Dictionary<int, string> HashToName
        {
            get
            {
                return hashToName;
            }
            set
            {
                hashToName = value;
                string name = CachePath + "/Test/" + hashes + ".cache";
                using (FileStream fs = new FileStream(name, FileMode.Create, FileAccess.ReadWrite))
                {
                    BinaryFormatter bi = new BinaryFormatter();
                    bi.Serialize(fs, hashToName);
                }
            }
        }
        public void RegisterImage_Training(Bitmap img, string actualName)
        {
            int hash = img.GetHashCode();
            hashes.Add(hash);

            string name = CachePath + "/Train/" + hash + ".jpg";
            img.Save(name, System.Drawing.Imaging.ImageFormat.Jpeg);

            name = CachePath + "/Test/" + hashes + ".cache";
            using (FileStream fs = new FileStream(name, FileMode.Create, FileAccess.ReadWrite))
            {
                BinaryFormatter bi = new BinaryFormatter();
                bi.Serialize(fs, hashes);
            }
        }
        public void Train(int itterations)
        {

        }

        public void Run()
        {
            // Start the child process.
            Process p = new Process();
            // Redirect the output stream of the child process.
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.FileName = ExecutablePath;
            p.Start();
            // Do not wait for the child process to exit before
            // reading to the end of its redirected stream.
            p.WaitForExit();
            // Read the output stream first and then wait.
            string output = p.StandardOutput.ReadToEnd();
            p.WaitForExit();
        }
    }
}
