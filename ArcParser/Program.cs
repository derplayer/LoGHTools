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

                //target archive to parse and create seperated files
                string archiveFileName = "C:\\Users\\d\\CLionProjects\\logh-arc-packer\\test\\datatable.mvx";
                string extractDirectory = "C:\\Users\\d\\CLionProjects\\logh-arc-packer\\output_test";
 
            //    string fileName = "../output_test/bin/my.datatable.arc";
                //if(argc >= 2) fileName = argv[1];
                //else printf("please provide file name \n");
                FileStream f = File.Open(archiveFileName,FileMode.OpenOrCreate,FileAccess.ReadWrite);
                

                //fseek(f,0L,SEEK_END); //send pointer to end of file
                //int fsize = ftell(f); //get position of pointer
                int fsize = (int)f.Length;
                //fseek(f,0L,SEEK_SET); //send pointer to beginning of file

                //unsigned char * buffer; //alloc memory as file size
                //buffer = (unsigned char *)malloc(fsize);
                byte[] buffer = new byte[fsize];
                f.Read(buffer,0,fsize);//read file and fill to buffer
                //fclose(f); //close file so other things can read that file too
                f.Close();

                Console.WriteLine("size of file: %lu {0}", buffer.Length);
                ArcParser arcParser = new ArcParser(buffer,fsize);
                arcParser.setExtractDirectory(extractDirectory);
                arcParser.run();
                Console.WriteLine("DONE");

                Console.ReadKey();
        }
    }
}
