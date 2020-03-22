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

            
            FileReader f = new FileReader();
            f.setRootDirectory("C:\\Users\\d\\CLionProjects\\logh-arc-packer\\output_test");

            ArcPacker a = new ArcPacker();
            a.setFileReader(f);
            a.packFiles();

            Console.WriteLine("FILES");
            foreach (string d in f.getFileNamesForARC())
            {
                Console.WriteLine(d);
            }
            
            Console.WriteLine("DONE");
        }
    }
}
