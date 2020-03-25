using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LoghRepacker;

namespace ArcParser
{
    class ArcParser
    {

        

        public byte[] archiveBytes;
        public int archiveSize = 0;
        HeaderParser headerParser;
        FileParser fileParser;
        FileExtractor fileExtractor;
        public ArcParser( byte[] archive, int archiveSize )
        {
            this.archiveBytes = archive;
            this.archiveSize = archiveSize;
            //printf("size archive is: %lu",sizeof(archive));


            this.fileExtractor = new FileExtractor();
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
