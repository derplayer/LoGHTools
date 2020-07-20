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


        int blockSize = 4096;

        public int getBlockSize()
        {
            return this.blockSize;
        }

        public List<Translateable> read(byte[] content)
        {

           

            int labelOffset = 0;
            int labelCount = content.Length / blockSize;

            //every string has 4096 byte space so i can pre calculate count of total string
            //and iterate it
            List<Translateable> strings = new List<Translateable>();
            for (int i = 0; i < labelCount; i++)
            {
                //i should find the place of string in block so i check until i hit
                //something different than 0x00
                int limit = blockSize * (i + 1);
                labelOffset = blockSize * i;

                while (content[labelOffset] == 0x00)
                    labelOffset++;
                //if i reach to end or exceed it that means there is no string in block
                //but the order is import so i will add a empty row
                if (labelOffset >= limit)
                {
                    strings.Add(new Translateable());
                    continue;
                }
                   
                //once i found the string i can extract the string for translate


                int sort = content[labelOffset + 1] << 8 | content[labelOffset];
                labelOffset += 2;
                Translateable translateable = new Translateable();
                while (content[labelOffset] != 0x00)
                {
                    translateable.baseString.Add(content[labelOffset]);
                    labelOffset++;
                }
                strings.Add(translateable);
            }

            return strings;

        }
    }
}
