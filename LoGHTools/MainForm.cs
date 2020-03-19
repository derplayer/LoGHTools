using LoGHTools.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LoGHTools
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        [DllImport("MicrovisionDLL.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr microvision_decompress(IntPtr inData, int osize, int insz);
        [DllImport("MicrovisionDLL.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void microvision_free(IntPtr buffer);

        private void button_Decompress_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "LoGH Containers|*.mvx;*.arc";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string outputFolderPath = openFileDialog.FileName + "_";
                GenericInfo tmpGeneric = new GenericInfo
                {
                    ContainerName = openFileDialog.FileName,
                    ContainerType = Path.GetExtension(openFileDialog.FileName)
                };

                ARCHeader tmpHeader = new ARCHeader();
                List<ARCToc> tmpToc = new List<ARCToc>();

                using (BinaryReader reader = new BinaryReader(new FileStream(openFileDialog.FileName, FileMode.Open)))
                {
                    //Parse Header
                    reader.BaseStream.Seek(0, SeekOrigin.Begin);
                    tmpHeader.HeaderIdentifier = reader.ReadBytes(4);
                    tmpHeader.FileReaderBufferSize = reader.ReadByte();
                    tmpHeader.UnknownType = reader.ReadByte();

                    reader.BaseStream.Seek(3, SeekOrigin.Current); //0x00 three times

                    tmpHeader.UnknownValue1 = reader.ReadByte();
                    tmpHeader.TocPointer2 = reader.ReadByte();
                    tmpHeader.UnknownValue2 = reader.ReadByte();
                    tmpHeader.UnknownType2 = reader.ReadByte();
                    tmpHeader.FileCount = reader.ReadByte();
                    tmpHeader.UnknownValue3 = reader.ReadByte();
                    tmpHeader.CompressionType = reader.ReadByte();

                    tmpHeader.HeaderLeftover = reader.ReadBytes(112);

                    //Begin of TOC
                    reader.BaseStream.Seek(2, SeekOrigin.Current); //skip two 0x00
                    for (int i = 1; i <= tmpHeader.FileCount; i++)
                    {
                        long entryOffset = reader.BaseStream.Position;
                        ARCToc localToc = new ARCToc();

                        for (int j = 1; j <= 5; j++)
                        {
                            var VAL1 = reader.ReadByte();
                            reader.BaseStream.Seek(6, SeekOrigin.Current);
                            var VAL2 = reader.ReadByte();
                            reader.BaseStream.Seek(6, SeekOrigin.Current);
                            var VAL3 = reader.ReadByte();
                            reader.BaseStream.Seek(6, SeekOrigin.Current);
                            var VAL4 = reader.ReadByte();
                            reader.BaseStream.Seek(6, SeekOrigin.Current);

                            var tmpVal = (VAL1 | (VAL2 << 8) | (VAL3 << 16) | (VAL4 << 24));

                            switch (j)
                            {
                                case 1:
                                    localToc.NameOffset = tmpVal;
                                    break;
                                case 2:
                                    localToc.NameLength = tmpVal;
                                    break;
                                case 3:
                                    localToc.Offset = tmpVal;
                                    break;
                                case 4:
                                    localToc.Zsize = tmpVal;
                                    break;
                                case 5:
                                    localToc.Size = tmpVal;
                                    break;
                                default:
                                    throw new Exception("NOPE!");
                            }

                            entryOffset += 1;
                            reader.BaseStream.Seek(entryOffset, SeekOrigin.Begin);
                        }

                        //Readout name string
                        long preOffset = reader.BaseStream.Position;
                        reader.BaseStream.Seek(localToc.NameOffset, SeekOrigin.Begin);
                        localToc.NameBytes = reader.ReadBytes(localToc.NameLength);
                        localToc.Name = new ASCIIEncoding().GetString(localToc.NameBytes, 0, localToc.NameLength - 1); //-1 because we dont want the null terminator

                        //Read DATA (offset since file begin)
                        reader.BaseStream.Seek(localToc.Offset, SeekOrigin.Begin);
                        localToc.Data = reader.ReadBytes(localToc.Zsize);
                        localToc.OffsetEnd = (int)reader.BaseStream.Position;

                        //Reset Seek for next header file
                        reader.BaseStream.Seek(preOffset, SeekOrigin.Begin);

                        //Add toc to list
                        tmpToc.Add(localToc);

                        //Everything is ready to readout compressed data
                        if (localToc.Zsize == localToc.Size)
                        {
                            //Content is unencrypted... passtrough directly to filewriter
                            localToc.DataDecompressed = localToc.Data;
                        }
                        else
                        {
                            byte[] pInBuffer = localToc.Data;

                            //allocate unmanaged memory
                            IntPtr inputBuffer = Marshal.AllocHGlobal(pInBuffer.Length * sizeof(byte));
                            Marshal.Copy(pInBuffer, 0, inputBuffer, pInBuffer.Length);

                            IntPtr result = microvision_decompress(inputBuffer, localToc.Size + 1, localToc.Zsize);
                            localToc.DataDecompressed = new byte[localToc.Size];
                            Marshal.Copy(result, localToc.DataDecompressed, 0, localToc.Size);

                            //free allocated memory
                            Marshal.FreeHGlobal(inputBuffer);
                            microvision_free(result);
                        }

                        reader.BaseStream.Seek(27, SeekOrigin.Current);
                    }

                }

                //PHASE 2: Export data from memory to Filesystem
                DirectoryInfo di = Directory.CreateDirectory(outputFolderPath);
                foreach (var toc in tmpToc)
                {
                    //Check name for sub-folders - when there are some - create
                    Directory.CreateDirectory(Path.GetDirectoryName(outputFolderPath + "//" + toc.Name));

                    //Decompressed file
                    using (FileStream fs = File.Create(outputFolderPath + "//" + toc.Name))
                    {
                        fs.Write(toc.DataDecompressed, 0, toc.DataDecompressed.Length);
                    }

                    //Compressed file
                    using (FileStream fs = File.Create(outputFolderPath + "//" + toc.Name + "_COMP"))
                    {
                        fs.Write(toc.Data, 0, toc.Data.Length);
                    }
                }

                SerializeCollection tmpSer = new SerializeCollection
                {
                    generic = tmpGeneric,
                    arcHeader = tmpHeader,
                    arcToc = tmpToc
                };

                //Serialize header & toc
                IFormatter formatter = new BinaryFormatter();
                Stream stream = new FileStream(outputFolderPath + "//_header.bin", FileMode.Create, FileAccess.Write, FileShare.None);
                //GZipStream gzip = new GZipStream(stream, CompressionLevel.Fastest);
                formatter.Serialize(stream, tmpSer);
                stream.Close();
            }
        }

        private void button_Pack_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Serialized Header (_header.bin)|_header.bin";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string outputFolderPath = openFileDialog.FileName + "_";
                IFormatter formatter = new BinaryFormatter();
                Stream stream = new FileStream(openFileDialog.FileName, FileMode.Open, FileAccess.Read, FileShare.Read);
                    
                SerializeCollection tmpSer = (SerializeCollection)formatter.Deserialize(stream);
                stream.Close();

                using (FileStream fs = File.Create(tmpSer.generic.ContainerName + "_DBG"))
                {
                    byte[] tmpHeader = tmpSer.arcHeader.GetBytes();
                    fs.Write(tmpHeader, 0, tmpHeader.Length);

                    //TODO: we just need the length of the old header for first (blank out) beacuase we need to update the data pointer anyway later after write operation
                    //TODO: dynamic header padding
                    byte[] tocBytes = tmpSer.GetTocBytes();
                    fs.Write(tocBytes, 0, tocBytes.Length);

                    //tof to string padding
                    for (int i = 0; i < 97; i++) fs.WriteByte((byte)0);

                    //Write file strings with proper null termination
                    foreach (var toc in tmpSer.arcToc)
                    {
                        byte[] encodedFilename = Encoding.GetEncoding("ASCII").GetBytes(toc.Name.ToCharArray());
                        fs.Write(encodedFilename, 0, encodedFilename.Length);
                        fs.WriteByte((byte)0);
                    }

                    //Write 0x80 based padding until content ends
                    while(fs.Position % 128 != 0)
                        fs.WriteByte((byte)0);

                    //Write Filecontent (and correct the decompressed new pointer and padd out until previous to next data pointer!)
                    foreach (var toc in tmpSer.arcToc)
                    {
                        //Update Toc pointer in header
                        toc.Offset = (int)fs.Position;

                        //Get the actual stream position and update header
                        fs.Write(toc.DataDecompressed, 0, toc.DataDecompressed.Length);

                        //Write 0x80 based padding until content ends
                        while (fs.Position % 128 != 0 && toc != tmpSer.arcToc.LastOrDefault())
                            fs.WriteByte((byte)0);
                    }

                    //Update Toc pointers and write again
                    tocBytes = tmpSer.GetTocBytes();
                    fs.Seek(0x80, SeekOrigin.Begin);
                    fs.Write(tocBytes, 0, tocBytes.Length);
                }
            }
        }

        private void button_HeaderDump_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "LoGH Containers|*.mvx;*.arc";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                ARCHeader tmpHeader = new ARCHeader();
                List<ARCToc> tmpToc = new List<ARCToc>();

                using (BinaryReader reader = new BinaryReader(new FileStream(openFileDialog.FileName, FileMode.Open)))
                {
                    //Parse Header
                    reader.BaseStream.Seek(0, SeekOrigin.Begin);
                    tmpHeader.HeaderIdentifier = reader.ReadBytes(4);
                }
            }
        }

        private void button_decompDbg_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "binary dump|*.*";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                byte[] dump = File.ReadAllBytes(openFileDialog.FileName);
                byte[] decompDump = LZSS.Decompress(dump);
                var outputFolderPath = Path.GetDirectoryName(openFileDialog.FileName);

                //Decompressed file
                using (FileStream fs = File.Create(outputFolderPath + "//" + "decompressed_blob.bin"))
                {
                    fs.Write(decompDump, 0, decompDump.Length);
                }

            }
        }
    }
}
