using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LoghRepacker;
using System.IO;

namespace ArcParser
{
    public class ArchiveParser
    {

        

        public byte[] archiveBytes;
        public int archiveSize = 0;
        HeaderParser headerParser;
        FileParser fileParser;
        FileExtractor fileExtractor = new FileExtractor();
        public string archiveFileName;

        public void setArchiveFileName(string fileName)
        {
            this.archiveFileName = fileName;
        }


        public void init()
        {

            //    string fileName = "../output_test/bin/my.datatable.arc";
            //if(argc >= 2) fileName = argv[1];
            //else printf("please provide file name \n");
            FileStream f = File.Open(archiveFileName, FileMode.OpenOrCreate, FileAccess.ReadWrite);


            //fseek(f,0L,SEEK_END); //send pointer to end of file
            //int fsize = ftell(f); //get position of pointer
            int fsize = (int)f.Length;
            //fseek(f,0L,SEEK_SET); //send pointer to beginning of file

            //unsigned char * buffer; //alloc memory as file size
            //buffer = (unsigned char *)malloc(fsize);
            byte[] buffer = new byte[fsize];
            f.Read(buffer, 0, fsize);//read file and fill to buffer
                                     //fclose(f); //close file so other things can read that file too
            f.Close();
            Console.WriteLine("size of file: %lu {0}", buffer.Length);

            this.archiveBytes = buffer;
            this.archiveSize = archiveSize;
            //printf("size archive is: %lu",sizeof(archive));


            
            //init
            this.initHeaderParser();
            this.initFileParser();
            //init


            //parse
            //printf("example data : %c \n",this->archiveBytes[0]);
            this.headerParser.setHeaderBytes(this.archiveBytes);

            this.fileParser.setArchiveBytes(this.archiveBytes);
            this.fileParser.setHeaderParser(this.headerParser);
            //parse

        }

        public void run()
        {
            this.debugLines();
            this.extractFiles();
        }

        public void setExtractDirectory(string extractDirctory)
        {
            this.fileExtractor.extractDirectory = extractDirctory;
        }
        public void extractFiles()
        {
            this.fileExtractor.extractFilesToDirectory(this.fileParser);
        }

        public void initHeaderParser()
        {
            this.headerParser = new HeaderParser();
        }

        public void initFileParser()
        {
            this.fileParser = new FileParser();
        }

        public void debugLines()
        {
            //printf("example data : %c%c%c%c \n", this->headerParser->archiveType[0], this->headerParser->archiveType[1], this->headerParser->archiveType[2], this->headerParser->archiveType[3]);

            //cout << "Archive Type is:" << this->headerParser->archiveType << "\n";
        }


     }
}
