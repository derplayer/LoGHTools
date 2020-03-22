using System;
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

        string extractDirectory = "C:\\Users\\d\\CLionProjects\\logh-arc-packer\\output_test";


            public void extractFilesToDirectory(FileParser fileParser)
            {
                foreach(FileMeta fit in fileParser.fileMetaList)
                //for(fileParser->fit = fileParser->fileMetaList.begin(); fileParser->fit!=fileParser->fileMetaList.end(); fileParser->fit++)
                {
        //            unsigned char temp[fileParser->fit->fileDataSize];

                    byte[] temp;
                    int tempSize = fit.fileDataSize+3;
                    temp = new byte[tempSize];
        //            return;
                    temp = fit.getFileData(temp,fileParser.archiveBytesPointer);
                    this.extractDataToDirectory(
                            fit.fileName,
                            fit.fileDataSize+3,
                            temp
                            );
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
