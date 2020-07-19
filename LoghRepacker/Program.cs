using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoghRepacker
{
    class Program
    {
        static void Main(string[] args)
        {


            ArcPacker a = new ArcPacker();
            a.setRootDirecotry(@"C:\Users\can\CLionProjects\logh-arc-packer\output_test");
            a.setExportFileName(@"C:\Users\can\Desktop\datatable.mvx");
            a.init();
            a.packFiles();
            

            Console.WriteLine("FILES");
            foreach (string d in a.getFileList())
            {
                Console.WriteLine(d);
            }
            
            Console.WriteLine("DONE");
            Console.ReadLine();

        }
    }
}
