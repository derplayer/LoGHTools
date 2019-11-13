using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoGHTools.Model
{
    //Header length is 0x80 (128 bytes)
    [Serializable]
    public class ARCHeader
    {
        public byte[] HeaderIdentifier { get; set; }        // 0x01-0x03            - Always "ARC1"
        public byte FileReaderBufferSize { get; set; }      // 0x04                 - Always 0x80 (game filestream reads in 128 byte chunks)
        public byte UnknownType { get; set; }               // 0x05                 - 0x20, 0x40, 0x60
                                                            // 0x06, 0x07 & 0x08    - Always empty - 0x00
        public byte UnknownValue1 { get; set; }             // 0x09                 - Seems to get bigger when more file in Container
        public byte TocPointer2 { get; set; }               // 0x0A                 - Toc Pointer Again? - Always 0x80
        public byte UnknownValue2 { get; set; }             // 0x0B                 - Always empty - 0x00
        public byte UnknownType2 { get; set; }              // 0x0C                 - Some kind of Type (0x00, 0x01, 0x02)
        public int FileCount { get; set; }                 // 0x0D                 - byte? (0-255)
        public byte UnknownValue3 { get; set; }             // 0x0E                 - Always empty - 0x00
        public byte CompressionType { get; set; }           // 0x0F                 - Compressiontype? (0x00 - archive is compressed, 0x03 - archive is total uncompressed)

        //There are still informations in header (with padding) but i will just read em in blindly for now (enough for decompress)
        public byte[] HeaderLeftover { get; set; }

        public byte[] GetBytes()
        {
            byte[] result = new byte[0x80];

            using (MemoryStream stream = new MemoryStream(result))
            {
                using (BinaryWriter writer = new BinaryWriter(stream))
                {
                    writer.Write(HeaderIdentifier);
                    writer.Write(FileReaderBufferSize);
                    writer.Write(UnknownType);
                    writer.Write((byte)0);
                    writer.Write((byte)0);
                    writer.Write((byte)0);
                    writer.Write(UnknownValue1);
                    writer.Write(TocPointer2);
                    writer.Write(UnknownValue2);
                    writer.Write(UnknownType2);
                    writer.Write((byte)FileCount);
                    writer.Write(UnknownValue3);
                    writer.Write(CompressionType);
                    writer.Write(HeaderLeftover);
                }
            }

            return result;
        }
    }

    [Serializable]
    public class ARCToc
    {
        public string Name { get; set; }
        public byte[] NameBytes { get; set; }
        public int NameOffset { get; set; }
        public int NameLength { get; set; }
        public int Offset { get; set; }            // Position of the archive where is located the file
        public int OffsetEnd { get; set; }
        public int Zsize { get; set; }             // Size of the compressed data in the archive
        public int Size { get; set; }              // Size of the uncompressed file

        public byte[] Data { get; set; }
        public byte[] DataDecompressed { get; set; }
    }

    [Serializable]
    public class GenericInfo
    {
        public string ContainerName { get; set; } //TODO: that one is right now global and is not shareable
        public string ContainerType { get; set; }
    }

    [Serializable]
    public class SerializeCollection
    {
        public GenericInfo generic { get; set; }
        public ARCHeader arcHeader { get; set; }
        public List<ARCToc> arcToc { get; set; }

        private byte[] addByteToArray(byte[] bArray, byte newByte)
        {
            byte[] newArray = new byte[bArray.Length + 1];
            bArray.CopyTo(newArray, 1);
            newArray[0] = newByte;
            return newArray;
        }

        public byte[] GetTocBytes()
        {
            using (MemoryStream stream = new MemoryStream())
            {
                using (BinaryWriter writer = new BinaryWriter(stream))
                {
                    foreach (var localToc in arcToc)
                    {
                        //3 byte padding
                        writer.Write((byte)0); 
                        writer.Write((byte)0);
                        if(arcToc.First() != localToc)
                            writer.Write((byte)0);

                        for (int j = 1; j <= 5; j++)
                        {
                            byte[] tmpBytes = new byte[4];

                            var tmpPos = writer.BaseStream.Position;
                            switch (j)
                            {
                                case 1:
                                    tmpBytes = BitConverter.GetBytes(localToc.NameOffset);
                                    break;
                                case 2:
                                    tmpBytes = BitConverter.GetBytes(localToc.NameLength);
                                    break;
                                case 3:
                                    tmpBytes = BitConverter.GetBytes(localToc.Offset);
                                    break;
                                case 4:
                                    tmpBytes = BitConverter.GetBytes(localToc.Zsize);
                                    break;
                                case 5:
                                    tmpBytes = BitConverter.GetBytes(localToc.Size);
                                    break;
                                default:
                                    throw new Exception("NOPE!");
                            }

                            writer.Write(tmpBytes[0]);
                            writer.Seek(6, SeekOrigin.Current);
                            writer.Write(tmpBytes[1]);
                            writer.Seek(6, SeekOrigin.Current);
                            writer.Write(tmpBytes[2]);
                            writer.Seek(6, SeekOrigin.Current);
                            writer.Write(tmpBytes[3]);
                            if(j != 5) writer.Seek((int)tmpPos + 1, SeekOrigin.Begin);

                        }

                        writer.Write((byte)0);
                        writer.Write((byte)0);
                        writer.Write((byte)0);
                    }
                    return stream.ToArray();
                }
            }
        }

    }
}