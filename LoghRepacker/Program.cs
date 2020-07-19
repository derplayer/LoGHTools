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
            a.setRootDirecotry(@"C:\Users\can\Desktop\arc_output");
            a.setExportFileName(@"C:\Users\can\Desktop\loading_img.arc");
            a.init();
            a.packFiles();
            

            Console.WriteLine("FILES");
            foreach (string d in a.getFileList())
            {
                Console.WriteLine(d);
            }
            
            Console.WriteLine("DONE");

        }
    }
}
