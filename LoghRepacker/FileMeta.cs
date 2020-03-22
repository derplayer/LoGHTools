using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoghRepacker
{
    class FileMeta
    {

    public int compressedDataSize = 0; //important for archived files  make it run
    public string metaData;
    public int fileNameLength;
    public int fileNameStartAddress;
    public int fileNameStartOffset;
    public string fileName;
    public string filePath;
    public int fileStartOffsetPrefix = 0;

    public int fileDataStartAddress;
    public int fileDataSize;

    public void getFileData(byte[] temp, byte[] archiveBytes)
    {
        int startOffset = this.fileDataStartAddress;
        int i = 0;
        for (i = 0; i < this.fileDataSize; ++i)
        {
            temp[i] = archiveBytes[startOffset+i];
            //printf("%2x \n",archiveBytes[startOffset+i]);
        }
        //exit(1);

        //compressed data

//        temp[i] = 0xff;
        temp[i] = Convert.ToByte(this.compressedDataSize >> 16);
        ++i;
        temp[i] = Convert.ToByte(this.compressedDataSize >> 8);
        ++i;
        temp[i] = Convert.ToByte(this.compressedDataSize & 0x00FF);

    }

    }
}
