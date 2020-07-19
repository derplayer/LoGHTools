using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace LoghRepacker
{
    public class ArcPacker
    {


        string rootDirectory;
        string exportFileName;
        FileReader fileReader;
        char archiveFileNameSeparator = '\u0000';

        public void setRootDirecotry(string r)
        {
            this.rootDirectory = r;
        }

        public void setExportFileName(string r)
        {
            this.exportFileName = r;
        }


        public List<string> getFileList()
        {
            return this.fileReader.getFileNamesForARC();
        }

        public void init()
        {
            this.fileReader = new FileReader();
            this.fileReader.setRootDirectory(this.rootDirectory);
        }

        public void packFiles()
        {


            int spaceBetweenFileMetaAndFileNames = 96;

            List<string> v = new List<string>();


            fileReader.setRootDirectory(this.rootDirectory);
            v = fileReader.getFileList();

            int byteSize = 1024 * 1024 * 50;
            var packBytes = new byte[byteSize];

            for (int j = 0; j < packBytes.Length; ++j)
            {
                packBytes[j] = 0x00; //lets be sure we have bunch pure 0x00
            }
            HeaderParser header = new HeaderParser();
          
            for (int j = 0; j < header.headerSize; ++j)
            {
                header.headerBytes[j] = 0x00; //lets be sure we have bunch pure 0x00
            }



            //total file count
            header.setFileCountByte(v.Count);
            header.parseFileCount();

            byte[] archType = { 0x41, 0x52, 0x43, 0x31 };
            header.setArchType(archType);
            header.parseArchiveTypeBytes();

            header.setCompressionType(CompressionEnums._DECOPRESSED);
            header.parseCompressionType();

            header.headerBytes[header.headMetaStartAddress] = 0x80;
            header.headerBytes[0x5] = 0XA0;
            header.headerBytes[0x6] = 0x0;
            header.headerBytes[0x7] = 0x0;
            header.headerBytes[0x8] = 0x80;
            header.headerBytes[0x9] = 0xF7;
            header.headerBytes[0x23] = 0x5A;

            //printf("file count is",header->nextFileMetaByteStartAddressOffset);

            List<FileMeta> fileMetas = new List<FileMeta>();


            int fileCounter = 0;
            int lastFileNameSize = 0;



           
           
            int currentIteration = 0;
            List<string> fullFileNames = fileReader.getFileList();
            List<string> fileNamesForArc = fileReader.getFileNamesForARC();
            
            //32 is double line space
            int totalSpentBytes = 128 + (32 * header.fileCount) + spaceBetweenFileMetaAndFileNames;
            int preCalculatedFileNamesStartAddress = totalSpentBytes;
            int totalFileNameSize = 0;

            foreach (string fileName in fileNamesForArc)
            { 


                string fakeFileName = fileName;
                string filePath = fullFileNames[currentIteration];
                //            cout << "FILE: " << *t << '\n';

                FileMeta newFileMeta = new FileMeta();

                newFileMeta.fileName = fakeFileName;
                newFileMeta.fileNameLength = fakeFileName.Length;
                newFileMeta.filePath = filePath;

                int fileNameAddress = preCalculatedFileNamesStartAddress + totalFileNameSize;



                newFileMeta.fileNameStartAddress = fileNameAddress;
                newFileMeta.fileNameStartAddress = fileNameAddress;
                totalFileNameSize += newFileMeta.fileNameLength;


                FileStream file;
                file = File.Open(filePath, FileMode.Open);
                newFileMeta.fileDataSize = (int)file.Length;
                //fileMetas.push_back(*newFileMeta);
                fileMetas.Add(newFileMeta);
                file.Close();
                Console.WriteLine("file name start address {0:X} \n", (fileNameAddress));
                //            printf("file name start offsets 0x%04x \n",lastFileNameOffset);
                //            printf("file size %d \n",newFileMeta->fileDataSize);
                fileCounter++;

                currentIteration++;
            }


            //WRITING TO BUFFER
            //header bytes
            for (int i = 0; i < 128; ++i)
            {
                packBytes[i] = header.headerBytes[i];
            }
            int bufferPointer = 128;



            //file meta bytes
            int nextFileMetaByteStartAddressOffset = 0;
            int lastFileDataSize = 0;
            int lastFileDataAddress = 0;

            int fileMetaBufferPointer = bufferPointer;
            foreach (FileMeta f in fileMetas)
            //for(auto f=fileMetas.begin(); f!=fileMetas.end(); ++f)
            {
                int packMetaOffsetAddress = bufferPointer + (nextFileMetaByteStartAddressOffset);
                packBytes[packMetaOffsetAddress + 0x2] = (byte)(f.fileNameStartAddress & 0xFF); //file name start address part2
                packBytes[packMetaOffsetAddress + 0x3] = (byte)(f.fileNameLength);
                packBytes[packMetaOffsetAddress + 0x4] = (byte)(f.fileDataStartAddress & 0x0000FF);


                //data size
                int fileDataSize = f.fileDataSize;
                packBytes[packMetaOffsetAddress + 0x13] = (byte)(fileDataSize >> 16);
                packBytes[packMetaOffsetAddress + 0xC] = (byte)(fileDataSize >> 8);
                packBytes[packMetaOffsetAddress + 0x5] = (byte)(fileDataSize & 0x00FF);


                //compressed data size
                packBytes[packMetaOffsetAddress + 0xD] = (byte)(fileDataSize >> 8);
                packBytes[packMetaOffsetAddress + 0x6] = (byte)(fileDataSize & 0x00FF);

                packBytes[packMetaOffsetAddress + 0x9] = (byte)(f.fileNameStartAddress >> 8); //file name start address part1
                packBytes[packMetaOffsetAddress + 0xA] = 0x00; 


                nextFileMetaByteStartAddressOffset += 32;

            }
            bufferPointer += fileCounter * 32;


            //file name start address
            bufferPointer += spaceBetweenFileMetaAndFileNames; //name listing address
            header.headerBytes[0x09] = 0xF7;
            header.headerBytes[0x0A] = 0x80;
            header.headerBytes[0x0C]  = 0x02;
            header.headerBytes[0x11] = 0x02;
            header.headerBytes[0x12] = (byte)(bufferPointer >> 8);
            header.headerBytes[0x13] = (byte)(bufferPointer & 0x00FF);

            header.headerBytes[0x14] = 0x02;
            header.headerBytes[0x15] = 0x00;
            header.headerBytes[0x16] = 0x04;
            header.headerBytes[0x17] = 0xD8;
            header.headerBytes[0x23] = 0x06;


            for (int i = 0; i < 128; ++i)
            {
                packBytes[i] = header.headerBytes[i];
            }

            //write file names to buffer
            //for(auto t=v.begin(); t!=v.end(); ++t)
            foreach (string tempFileName in fileNamesForArc)
            {
                //string tempFileName = "bin/"+*t+" ";
                string theTemp = tempFileName.Remove(0, 1).Replace('\\', '/');
                theTemp += this.archiveFileNameSeparator;

                for (int i = 0; i < theTemp.Length; ++i)
                {
                    packBytes[bufferPointer + i] = (byte)(theTemp[i]);
                 
                }
                
                bufferPointer += theTemp.Length;
            }

            bufferPointer += 249+16; //space for data



            nextFileMetaByteStartAddressOffset = 0;
            int bufferedFileSize = 0;
            int fileCount = 0;


            //for(auto f=fileMetas.begin(); f!=fileMetas.end(); ++f)
            foreach (FileMeta f in fileMetas)
            {
                int packMetaOffsetAddress = fileMetaBufferPointer + (nextFileMetaByteStartAddressOffset);


                int fileDataStartAddress = bufferPointer + bufferedFileSize;

                int tempAddress = fileDataStartAddress & 0xFF;

                //            if(tempAddress < 0x80)
                //            {
                //                fileDataStartAddress += (0x80-tempAddress);
                //            }
                //            else
                //            {
                //                fileDataStartAddress -= (tempAddress - 0x80);
                //
                //            }
                ////            fileDataStartAddress = tempAddress;

                //            fileDataStartAddress = (fileDataStartAddress - (fileDataStartAddress % 16));

                packBytes[packMetaOffsetAddress + 0xB] = (byte)(fileDataStartAddress >> 8);
                packBytes[packMetaOffsetAddress + 0x4] = (byte)(fileDataStartAddress & 0x0000FF);
                int fileDataStartAddressPart2 = packBytes[packMetaOffsetAddress + 0x4];


                if (fileDataStartAddress > 0xFFFF) //when you overflow the 0xFFFF you need to store overflowed part to specified address
                {
                    packBytes[packMetaOffsetAddress + 0x12] = (byte)(fileDataStartAddress >> 16);
                }

                if (fileDataStartAddress > 0xFFFFFF) //when you overflow the 0xFFFF you need to store overflowed part to specified address
                {
                    packBytes[packMetaOffsetAddress + 0x19] = (byte)(fileDataStartAddress >> 24);
                }

                Console.WriteLine("file data start address {0:X} \n", fileDataStartAddress);

                //newFile = File.Open( "../output_test/"+f.fileName,FileMode.Open);

                FileStream rf = new FileStream(f.filePath,FileMode.Open,FileAccess.ReadWrite);
                byte[] fileBuffer = new byte[f.fileDataSize];
                rf.Read(fileBuffer, 0, (int)rf.Length);
                rf.Close();
                    //File.ReadAllBytes(f.filePath);
                //file.read(fileBuffer, f->fileDataSize);
                //

                //             printf("data start address is %04x \n",fileDataStartAddress);

                //             printf("size is %02x \n",f->fileDataSize);

                for (int i = 0; i < f.fileDataSize; ++i)
                {
                    //                printf("data: %2x \n", fileBuffer[i]);
                    //                printf("write address: %4x \n", fileDataStartAddress);
                    packBytes[fileDataStartAddress + i] = fileBuffer[i];
                }
                nextFileMetaByteStartAddressOffset += 32;





                /*
                //compressed data size
                packBytes[0x80 + (32 * fileCount) + 0x6] = (fileBuffer[f.fileDataSize - 1]);
                packBytes[0x80 + (32 * fileCount) + 0xD] = (fileBuffer[f.fileDataSize - 2]);
                packBytes[0x80 + (32 * fileCount) + 0x14] = (fileBuffer[f.fileDataSize - 3]);
                */


                //            printf("----------------------------------");


                if (f.fileDataSize % 2048 != 0)
                {
                    f.fileDataSize += (2048 - (f.fileDataSize % 2048));
                }

                bufferedFileSize += f.fileDataSize; //16 line space between data

                fileCount++;
            }
            bufferPointer += bufferedFileSize;



            //string fullPath = "C:\\Users\\user_name\\Desktop\\output_arc\\my.datatable.arc";
            string fullPath = this.exportFileName;
            //std::ofstream file;
            FileStream newOutput = File.Open(fullPath, FileMode.OpenOrCreate);
            newOutput.Write(packBytes,0, bufferPointer);
            //file.write((const char *)packBytes, bufferPointer);
            //file.close();
            newOutput.Close();
           

        }

    }
}
