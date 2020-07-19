using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TranslateHelper
{
    class TranslateableREader
    {



        public List<List<byte>> read(byte[] content)
        {

           

            int labelOffset = 0;
            int blockSize = 4096;
            int labelCount = content.Length / blockSize;

            //every string has 4096 byte space so i can pre calculate count of total string
            //and iterate it
            List<List<byte>> strings = new List<List<byte>>();
            for (int i = 0; i < labelCount; i++)
            {
                //i should find the place of string in block so i check until i hit
                //something different than 0x00
                int limit = blockSize * (i + 1);
                labelOffset = blockSize * i;

                while (content[labelOffset] == 0x00)
                    labelOffset++;
                //if i reach to end or exceed it that means there is no string in block
                if (labelOffset >= limit)
                    continue;
                //once i found the string i can extract the string for translate


                int sort = content[labelOffset + 1] << 8 | content[labelOffset];
                labelOffset += 2;
                List<byte> label = new List<byte>();
                while (content[labelOffset] != 0x00)
                {
                    label.Add(content[labelOffset]);
                    labelOffset++;
                }
                strings.Add(label);
            }

            return strings;

        }
    }
}
