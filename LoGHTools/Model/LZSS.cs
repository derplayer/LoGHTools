using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoGHTools.Model
{
    /// <summary>
    /// Circular buffer for the LZSS compression
    /// </summary>
    public class LoGHBuffer
    {
        private byte[] _data;
        private int _cursor;
        private int _length;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="length">Length of the buffer/window</param>
        /// <param name="cursorStart">Cursor start offset inside the buffer/window</param>
        public LoGHBuffer(int length, int cursorStart)
        {
            _data = new byte[length];
            _length = length;
            _cursor = cursorStart;
        }

        /// <summary>
        /// Adds a byte to the buffer
        /// </summary>
        /// <param name="value"></param>
        public void AddData(byte value)
        {
            _data[_cursor++] = value;
            _cursor = _cursor & 0xFFF;
        }

        /// <summary>
        /// Gets bytes from the buffer while also filling
        /// the returned bytes into the buffer
        /// </summary>
        /// <param name="offset">Offset inside the buffer/window</param>
        /// <param name="length">Length of how many bytes will be returned</param>
        /// <returns></returns>
        public byte[] GetData(int offset, int length)
        {
            if (offset >= _length) return null;

            byte[] result = new byte[length];

            for (int i = 0; i < length; i++)
            {
                int position = (offset + i) & 0xFFF;
                result[i] = _data[position];
                _data[_cursor++] = _data[position];
                _cursor = _cursor & 0xFFF;
            }
            return result;
        }
    }

    //WORD--------------
    /// <summary>
    /// Compressed word(2 bytes) of the LZSS compression
    /// </summary>
    class LoGHCompressedWord
    {
        /// <summary>
        /// Offset of the compressed word inside the sliding window
        /// </summary>
        public ushort Offset { get; set; }
        /// <summary>
        /// Length of the compressed word
        /// </summary>
        public ushort Length { get; set; }
        private ushort LengthField = 4;
        private ushort LengthBias = 3;

        public LoGHCompressedWord(ushort offset, ushort length)
        {
            Offset = offset;
            Length = length;
        }

        public LoGHCompressedWord(byte[] data)
        {
            byte lower = data[0];
            byte higher = data[1];

            Offset = lower;
            Offset |= (ushort)((higher & 0xF0) << LengthField);
            Length = (ushort)((higher & 0x0F) + LengthBias);
        }

        public byte[] GetBytes()
        {
            byte[] result = new byte[2];

            result[0] = (byte)Offset;
            result[1] = (byte)((Offset & 0x0F00) >> LengthField);
            result[1] |= (byte)(Length - LengthBias);

            return result;
        }
    }
    //-------------

    /// <summary>
    /// LZSS de/compressor
    /// </summary>
    public static class LZSS
    {
        /// <summary>
        /// Decompresses the given data byte array with the given length
        /// </summary>
        /// <param name="data">LZSS compressed data</param>
        /// <param name="length">Length?</param>
        /// <returns>Decompressed data</returns>
        public static byte[] Decompress(byte[] data, int length = -1)
        {
            LoGHBuffer loghBuffer = new LoGHBuffer(4096, 0xFEE);
            int wordLength = 2;

            using (MemoryStream writer = new MemoryStream())
            {
                for (int i = 0; i < data.Length; i++)
                {
                    bool[] flags = GetFlags(data[i]);

                    for (int j = 7; j >= 0; j--)
                    {
                        if (flags[j])
                        {
                            if (i + wordLength >= data.Length) break;

                            byte[] wordBytes = new byte[wordLength];

                            Array.Copy(data, ++i, wordBytes, 0, wordLength);
                            LoGHCompressedWord word = new LoGHCompressedWord(wordBytes);

                            byte[] result = loghBuffer.GetData(word.Offset, word.Length);
                            i++;

                            writer.Write(result, 0, result.Length);
                            writer.Flush();
                        }
                        else
                        {
                            if (i + 1 >= data.Length) break;

                            writer.Write(data, ++i, 1);
                            writer.Flush();
                            loghBuffer.AddData(data[i]);
                        }
                    }
                }
                if (length != -1)
                {
                    if (length > writer.Length)
                    {
                        byte[] padding = new byte[length - writer.Length];
                        writer.Write(padding, 0, padding.Length);
                    }
                }
                return writer.ToArray();
            }
        }

        public unsafe static void Decompress2(byte[] decom, byte[] com, int comSize)
        {
            byte[] dict = new byte[4096];
            fixed (byte* pDict = dict, p1 = decom, p2 = com)
            {
                byte* pDecom = p1, pCom = p2;

                byte next;
                int r6 = 0, r7 = 0xfee, r10, r9;

                while (true)
                {
                    r6 >>= 1;

                    if ((r6 & 0x100) == 0)
                    {
                        if (comSize-- == 0)
                            return;

                        r6 = 0xFF00 | *(pCom++);
                    }

                    if ((r6 & 1) == 1)
                    {
                        if (comSize-- == 0)
                            return;

                        next = *(pCom++);
                        *(pDecom++) = next;
                        *(pDict + r7) = next;
                        r7 = (r7 + 1) & 0xFFF;
                    }
                    else
                    {
                        if ((comSize -= 2) <= 0)
                            return;

                        r10 = (*(pCom++) << 8) | *(pCom++);

                        r9 = r10 >> 4;
                        r10 = (r10 & 0xF) + 2;

                        for (int i = r10 + 1; i > 0; --i)
                        {
                            r10 = r9++ & 0xFFF;
                            next = *(pDict + r10);
                            *(pDecom++) = next;
                            *(pDict + r7) = next;
                            r7 = (r7 + 1) & 0xfff;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Converts the flag byte to bool array
        /// </summary>
        /// <param name="data">Flag byte</param>
        /// <returns>Flag bool array</returns>
        private static bool[] GetFlags(byte data)
        {
            bool[] result = new bool[8];
            for (int i = 0; i < 8; i++)
            {
                result[i] = (data & (1 << i)) != 1 << i;
            }
            result = result.Reverse().ToArray();
            return result;
        }
    }
}
