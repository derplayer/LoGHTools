using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoghRepacker
{
    class HeaderParser
    {

            public byte[] headerBytes;
            public byte[] archiveType;
            public byte headFileCountAddress = 0XD;
            public byte headMetaStartAddress = 0x4;
            public byte headCompressionAddress = 0xF;
            public byte metaStartAddress;
            public byte compressionType;
            public int nameDataStartAddress;
            public int fileCount;
            public int headerSize = 128;

            public HeaderParser()
            {
                this.headerBytes = new byte[this.headerSize];
                this.archiveType = new byte[4];

            }

            public void setHeaderBytes(byte[] fileBytes)
            {

                //printf("example data : %c%c%c%c \n",fileBytes[0],fileBytes[1],fileBytes[2],fileBytes[3]);

                for (int i = 0; i < 128; ++i)
                {
                    this.headerBytes[i] = (fileBytes[i]); //gather header
                }


                this.parseArchiveTypeBytes();
                this.parseMetaStartAddress();

                this.parseNameDataStartAddress();
                this.parseFileCount();
            }

            public void setFileCountByte(int fileCount)
            {
                this.headerBytes[this.headFileCountAddress] = Convert.ToByte(fileCount);
            }

            public void setArchType(byte[] bytes)
            {
                this.headerBytes[0] = (bytes[0]);
                this.headerBytes[1] = (bytes[1]);
                this.headerBytes[2] = (bytes[2]);
                this.headerBytes[3] = (bytes[3]);
            }

            public void setCompressionType(byte type)
            {
                this.headerBytes[this.headCompressionAddress] = type;
            }

            public void parseCompressionType()
            {
                this.compressionType = this.headerBytes[this.headCompressionAddress];
                printf("Compression type is 0x%02x \n",this.compressionType);
            }

            void printf(string data, params object[] arg)
            {
                Console.WriteLine(data,arg);
            }


            public void parseFileCount()
            {
                this.fileCount = this.headerBytes[this.headFileCountAddress];
                printf("[Header Parser] File count is: %d \n",this.fileCount);
            }

            public void parseNameDataStartAddress()
            {
                this.nameDataStartAddress = ((this.headerBytes[0x12] << 8) | (this.headerBytes[0x13])) + this.headerBytes[0x8];
                printf("Name data start address : %4x \n",this.nameDataStartAddress);
            }

            public void parseMetaStartAddress()
            {
                this.metaStartAddress = this.headerBytes[this.headMetaStartAddress];
                printf("Meta start address : %2x \n",this.metaStartAddress);
            }

            public void parseArchiveTypeBytes()
            {
                this.archiveType[0] = (this.headerBytes[0x0]);
                this.archiveType[1] = (this.headerBytes[0x1]);
                this.archiveType[2] = (this.headerBytes[0x2]);
                this.archiveType[3] = (this.headerBytes[0x3]);

                printf("Archive type : %c%c%c%c \n",this.archiveType[0],this.archiveType[1],this.archiveType[2],this.archiveType[3]);

            }

       }
}
