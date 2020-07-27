﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LoghRepacker;
using System.IO;

namespace ArcParser
{
    class FileExtractor
    {

            public string extractDirectory;


            public void extractFilesToDirectory(FileParser fileParser)
            {
                foreach(FileMeta fit in fileParser.fileMetaList)
                //for(fileParser->fit = fileParser->fileMetaList.begin(); fileParser->fit!=fileParser->fileMetaList.end(); fileParser->fit++)
                {
        //            unsigned char temp[fileParser->fit->fileDataSize];

                    byte[] temp;
                    int tempSize = fit.fileDataSize;
                    temp = new byte[tempSize];
        //            return;
                    temp = fit.getFileData(temp,fileParser.archiveBytesPointer);
                    
                    byte archiveByte = fileParser.getHeaderParser().compressionType;

                    if (archiveByte == CompressionEnums._COMPRESSED)
                    {
                        byte[] decompressedBytes = LZSS.Decompress(temp, fit.fileDataSize);
                        this.extractDataToDirectory(
                                fit.fileName,
                                decompressedBytes.Length,
                                decompressedBytes
                                );
                    }
                    else if (archiveByte == CompressionEnums._DECOPRESSED)
                    {
                        
                        this.extractDataToDirectory(
                                fit.fileName,
                                fit.fileDataSize,
                                temp
                                );
                        
                    }
                }
            }

            void extractDataToDirectory(string fileName,int size, byte[] bytes)
            {

                //TODO: i need something to create directories recursively
                string fullPath = this.extractDirectory+"/"+fileName;
                fullPath = fullPath.Replace("/","\\");
                Console.WriteLine("!OK {0}", fullPath);
                Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
                FileStream f = File.Open(fullPath, FileMode.OpenOrCreate,FileAccess.ReadWrite);
                f.Write(bytes,0, size);
                f.Close();
                //cout << "!OK " << (fullPath) << endl;

            }

    }
}
