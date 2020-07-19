using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ArcParser
{
    class Program
    {
        static void Main(string[] args)
        {


                ArcParser arcParser = new ArcParser();
                arcParser.setExtractDirectory(@"C:\Users\can\CLionProjects\logh-arc-packer\output_test");
                arcParser.setArchiveFileName(@"C:\Users\can\CLionProjects\logh-arc-packer\test\datatable.mvx");
                arcParser.init();
                arcParser.run();
                Console.WriteLine("DONE");

                Console.ReadKey();
        }
    }
}
