using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LoghRepacker;

namespace ArcParser
{
    class FileParser
    {

        HeaderParser headerParser;
        int fileMetaLength = 32; // this 32 byte meta data per file
        public List<FileMeta> fit;
        public List<FileMeta> fileMetaList = new List<FileMeta>();
        public byte[] archiveBytesPointer;



        ~FileParser()
        {
            this.setHeaderParser(new HeaderParser());
        }

        public void setHeaderParser(HeaderParser headerParser)
        {
            this.headerParser = headerParser;
            this.parseFileMeta();
        }

        public void setArchiveBytes(byte[] archiveBytes)
        {
            this.archiveBytesPointer = archiveBytes;
        }

        void parseFileMeta()
        {

            Console.WriteLine("[FileParser] File count is {0} ",this.headerParser.fileCount);
            for (int i = 0; i < this.headerParser.fileCount; i++)
            {
                int startAddress = this.headerParser.metaStartAddress + (this.fileMetaLength * i);
                Console.WriteLine("start Address is : 0x{0:X}",startAddress);

                FileMeta newFileMeta = new FileMeta();

                newFileMeta.fileNameStartOffset = this.archiveBytesPointer[(startAddress + 0x2)];

                newFileMeta.fileNameStartAddress  = newFileMeta.fileNameStartOffset + ((this.archiveBytesPointer[(startAddress + 0x09)] << 8) | this.archiveBytesPointer[(startAddress + 0x0A)]);
                newFileMeta.fileNameLength = this.archiveBytesPointer[(startAddress+0x3)];

                newFileMeta.fileName = this.getFileNameFromBytes(
                        newFileMeta.fileNameStartAddress,
                        newFileMeta.fileNameLength
                        );

                newFileMeta.compressedDataSize =
                        (this.archiveBytesPointer[(startAddress + 0x14)]  << 16) |
                        (this.archiveBytesPointer[(startAddress + 0xD)]  << 8) |
                        (this.archiveBytesPointer[(startAddress + 0x06)]);

                newFileMeta.fileDataStartAddress =
                                 (this.archiveBytesPointer[(startAddress + 0x19)] << 24) |
                                 (this.archiveBytesPointer[(startAddress + 0x12)] << 16) |
                                 (this.archiveBytesPointer[(startAddress + 0xB)] << 8) |
                                 (this.archiveBytesPointer[(startAddress + 0x4)]);


                newFileMeta.fileDataSize =
                        (this.archiveBytesPointer[(startAddress + 0x13)] << 16) |
                        (this.archiveBytesPointer[(startAddress + 0x0C)] << 8) |
                        (this.archiveBytesPointer[(startAddress + 0x05)]);
                this.fileMetaList.Add(newFileMeta);
            }
            Console.WriteLine("---------------------------------------\n");

        
            foreach(FileMeta fit in this.fileMetaList)
            //for(fit = this->fileMetaList.begin(); fit!=this->fileMetaList.end(); fit++)
            {
                Console.WriteLine("File NAME: {0}",fit.fileName);
                Console.WriteLine("File NAME start address: {0:X} \n", fit.fileNameStartAddress);
                Console.WriteLine("File NAME length: %d character (byte) \n", fit.fileNameLength);
                Console.WriteLine("<<<<<<<<<<<<<<<<<<>>>>>>>>>>>>>>>>>>\n");

                Console.WriteLine("File DATA start Address: 0x%4x \n", fit.fileDataStartAddress);
                Console.WriteLine("File DATA size: %d byte \n", fit.fileDataSize);
                Console.WriteLine("File Data compressed size: 0x%04x byte \n",fit.compressedDataSize);
                //printf("File name: %s \n", fit->fileName);
                Console.WriteLine("---------------------------------------\n");

            }

        }

        public string getFileNameFromBytes(int start,int length)
        {
            string temp="";
            int startAddress = start;
            int endAddress = (startAddress + length);
            //-1 is for not to get 0x20 which is actually looks like space in hex editor
            for (int i = startAddress; i < endAddress-1; ++i)
            {
                    temp+=(char)this.archiveBytesPointer[i];

            }
            return temp;
        }

    }
}
