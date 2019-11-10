using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManualTester
{
    class Program
    {
        static void Main(string[] args)
        {
            object x = null;
            Hoge y = (Hoge)x;

        }

        struct Hoge {

           public int X { get; set; }
        }

    }
}
