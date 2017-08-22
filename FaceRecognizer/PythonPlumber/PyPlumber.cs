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
using System.Xml.Serialization;

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
        public string ResultDataPath { get { return CachePath + "\\Test\\Images\\"; } }
        public void RegisterImage_Result(Bitmap img)
        {
            int hash = img.GetHashCode();
            Hashes.Add(hash);

            string path = ResultDataPath;
            string name = path + hash + ".jpg";

            if (!Directory.Exists(path)) {
                Directory.CreateDirectory(path);
            }

            img.Save(name, System.Drawing.Imaging.ImageFormat.Jpeg);
            
            name = path + "\\hashes.cache";
            using (FileStream fs = new FileStream(name, FileMode.Create)) {
                XmlSerializer bi = new XmlSerializer(typeof(List<int>));
                bi.Serialize(fs, hashes);
            }
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
        public string TrainingDataPath { get { return CachePath + "\\Train\\Images\\"; } }
        public void RegisterImage_Training(Bitmap img, string actualName)
        {
            int hash = img.GetHashCode();
            hashToName.Add(hash, actualName);

            string path = TrainingDataPath;
            string name = path + actualName + hash + ".jpg";
            if (!Directory.Exists(path)) {
                Directory.CreateDirectory(path);
            }

            img.Save(name, System.Drawing.Imaging.ImageFormat.Jpeg);

            name = path + "\\hashes.cache";
            using (FileStream fs = new FileStream(name, FileMode.Create)) {
                XmlSerializer bi = new XmlSerializer(typeof(KVP[]));
                bi.Serialize(fs, hashToName.ToArray().Select(x => new KVP() { i=x.Key, s=x.Value}).ToArray());
            }
        }
        public struct KVP {
            public int i;
            public string s;
        }
        public void Train(int itterations)
        {

        }

        public string Run()
        {
            Process process = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.FileName = "cmd.exe";
            startInfo.Arguments = $"/C py {ExecutablePath}";
            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardInput = true;
            process.StartInfo = startInfo;
            process.Start();



            process.WaitForExit();
            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            return output;
        }
    }
}
