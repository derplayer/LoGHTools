﻿using System;
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


                ArchiveParser arcParser = new ArchiveParser();
                arcParser.setExtractDirectory(@"C:\Users\can\Desktop\arc_output");
                arcParser.setArchiveFileName(@"C:\Users\can\CLionProjects\logh-arc-packer\test\datatable.mvx");
                arcParser.init();
                arcParser.run();
                Console.WriteLine("DONE");

                Console.ReadKey();
        }
    }
}
