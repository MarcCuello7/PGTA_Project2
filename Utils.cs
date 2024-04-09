using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Globalization;

namespace Project2_Code
{
    class Utils
    {
        /// <summary>
        /// The base Kaitai stream which exposes an API for the Kaitai Struct framework.
        /// It's based off a <code>BinaryReader</code>, which is a little-endian reader.
        /// <summary>
        /// Read a signed byte from the stream
        /// </summary>
        /// <returns></returns>
        public sbyte ReadS1(Stream data)
        {
            return ReadSByte();
        }


        /// <summary>
        /// Read a signed short from the stream (big endian)
        /// </summary>
        /// <returns></returns>
        public short ReadS2be(Stream data)
        {
            return BitConverter.ToInt16(ReadBytes(2), 0);
        }

        /// <summary>
        /// Read a signed int from the stream (big endian)
        /// </summary>
        /// <returns></returns>
        public int ReadS4be(Stream data)
        {
            return BitConverter.ToInt32(ReadBytes(4), 0);
        }

        /// <summary>
        /// Read a signed long from the stream (big endian)
        /// </summary>
        /// <returns></returns>
        public long ReadS8be(Stream data)
        {
            return BitConverter.ToInt64(ReadBytes(8), 0);
        }


        /// <summary>
        /// Read an unsigned byte from the stream
        /// </summary>
        /// <returns></returns>
        public byte ReadU1(Stream data)
        {
            return ReadByte();
        }



        /// <summary>
        /// Read an unsigned short from the stream (big endian)
        /// </summary>
        /// <returns></returns>
        public ushort ReadU2(Stream data)
        {
            return BitConverter.ToUInt16(ReadBytes(2), 0);
        }

        /// <summary>
        /// Read an unsigned int from the stream (big endian)
        /// </summary>
        /// <returns></returns>
        public uint ReadU4(Stream data)
        {
            return BitConverter.ToUInt32(ReadBytes(4), 0);
        }

        /// <summary>
        /// Read an unsigned long from the stream (big endian)
        /// </summary>
        /// <returns></returns>
        public ulong ReadU8(Stream data)
        {
            return BitConverter.ToUInt64(ReadBytes(8), 0);
        }

  

        /// <summary>
        /// Read a single-precision floating point value from the stream (big endian)
        /// </summary>
        /// <returns></returns>
        public float ReadF4(Stream data)
        {
            return BitConverter.ToSingle(ReadBytes(4), 0);
        }

        /// <summary>
        /// Read a double-precision floating point value from the stream (big endian)
        /// </summary>
        /// <returns></returns>
        public double ReadF8(Stream data)
        {
            return BitConverter.ToDouble(data.ReadBytes(8), 0);
        }

      
        /// <summary>
        /// Read a n-bit integer in a big-endian manner from the stream
        /// </summary>
        /// <returns></returns>
        public ulong ReadBitsInt(int n)
        {
            ulong res = 0;

            int bitsNeeded = n - BitsLeft;
            BitsLeft = -bitsNeeded & 7; // `-bitsNeeded mod 8`

            if (bitsNeeded > 0)
            {
                // 1 bit  => 1 byte
                // 8 bits => 1 byte
                // 9 bits => 2 bytes
                int bytesNeeded = ((bitsNeeded - 1) / 8) + 1; // `ceil(bitsNeeded / 8)`
                byte[] buf = ReadBytes(bytesNeeded);
                for (int i = 0; i < bytesNeeded; i++)
                {
                    res = res << 8 | buf[i];
                }

                ulong newBits = res;
                res = res >> BitsLeft | Bits << bitsNeeded;
                Bits = newBits; // will be masked at the end of the function
            }
            else
            {
                res = Bits >> -bitsNeeded; // shift unneeded bits out
            }

            ulong mask = (1UL << BitsLeft) - 1; // `BitsLeft` is in range 0..7, so `(1UL << 64)` does not have to be considered
            Bits &= mask;

            return res;
        }

        [Obsolete("use ReadBitsIntBe instead")]
        public ulong ReadBitsInt(int n)
        {
            return ReadBitsIntBe(n);
        }   
    }
}