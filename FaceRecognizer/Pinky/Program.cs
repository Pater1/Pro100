using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheBrain;

namespace Pinky
{
    class Program
    {
        static void Main(string[] args)
        {
            Brain the = new Brain();
            Console.WriteLine(the.brain.ToString());
        }
    }
}
