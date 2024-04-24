using System;
using System.IO;
using System.Collections;

namespace Project2_Code
{
    public static class Utils
    {
        /// <summary>
        /// The base Kaitai stream which exposes an API for the Kaitai Struct framework.
        /// It's based off a <code>BinaryReader</code>, which is a little-endian reader.
        /// <summary>
        /// Read a signed byte from the stream
        /// </summary>
        /// <returns></returns>
        public static sbyte ReadS1(BinaryReader data)
        {
            return data.ReadSByte();
        }

        /// <summary>
        /// Read a signed short from the stream (big endian)
        /// </summary>
        /// <returns></returns>
        public static short ReadS2(BinaryReader data)
        {
            return BitConverter.ToInt16(ReadBytesBigEndian(data, 2), 0);
        }

        /// <summary>
        /// Read a signed int from the stream (big endian)
        /// </summary>
        /// <returns></returns>
        public static int ReadS4(BinaryReader data)
        {
            return BitConverter.ToInt32(ReadBytesBigEndian(data, 4), 0);
        }

        /// <summary>
        /// Read a signed long from the stream (big endian)
        /// </summary>
        /// <returns></returns>
        public static long ReadS8(BinaryReader data)
        {
            return BitConverter.ToInt64(ReadBytesBigEndian(data, 8), 0);
        }

        /// <summary>
        /// Read an unsigned byte from the stream
        /// </summary>
        /// <returns></returns>
        public static byte ReadU1(BinaryReader data)
        {
            return data.ReadByte();
        }

        /// <summary>
        /// Read an unsigned short from the stream (big endian)
        /// </summary>
        /// <returns></returns>
        public static ushort ReadU2(BinaryReader data)
        {
            return BitConverter.ToUInt16(ReadBytesBigEndian(data, 2), 0);
        }

        /// <summary>
        /// Read an unsigned int from the stream (big endian)
        /// </summary>
        /// <returns></returns>
        public static uint ReadU4(BinaryReader data)
        {
            return BitConverter.ToUInt32(ReadBytesBigEndian(data, 4), 0);
        }

        /// <summary>
        /// Read an unsigned long from the stream (big endian)
        /// </summary>
        /// <returns></returns>
        public static ulong ReadU8(BinaryReader data)
        {
            return BitConverter.ToUInt64(ReadBytesBigEndian(data, 8), 0);
        }  

        /// <summary>
        /// Read a single-precision floating point value from the stream (big endian)
        /// </summary>
        /// <returns></returns>
        public static float ReadF4(BinaryReader data)
        {
            return BitConverter.ToSingle(ReadBytesBigEndian(data, 4), 0);
        }

        /// <summary>
        /// Read a double-precision floating point value from the stream (big endian)
        /// </summary>
        /// <returns></returns>
        public static double ReadF8(BinaryReader data)
        {
            return BitConverter.ToDouble(ReadBytesBigEndian(data, 8), 0);
        }

        /// <summary>
        /// Read bytes from the stream in big endian format and convert them to the endianness of the current platform
        /// </summary>
        /// <param name="count">The number of bytes to read</param>
        /// <returns>An array of bytes that matches the endianness of the current platform</returns>
        public static byte[] ReadBytesBigEndian(BinaryReader data, int count)
        {
            byte[] bytes = data.ReadBytes(count);
            Array.Reverse(bytes);
            return bytes;
        }

        public static byte ExtractBits(byte b, byte first, byte last)
        {
            byte mask = 0;
            for (byte i = first; i <= last; i++)
            {
                mask |= (byte) (1 << i);
            }
            b &= mask;
            b >>= first;
            return b;
        }

        public static bool ExtractBool(byte b, byte pos)
        {
            byte mask = (byte)(1 << pos);
            b &= mask;
            b >>= pos;

            return b == 1;
        }

        public static byte ExtractU1(byte[] bytes, int first, int length)
        {
            byte[] extracted = ExtractBits(bytes, first, length, false);
            return extracted[0];
        }

        public static ushort ExtractU2(byte[] bytes, int first, int length)
        {
            byte[] extracted = ExtractBits(bytes, first, length, false);
            return BitConverter.ToUInt16(extracted, 0);
        }

        public static byte[] ExtractBits(byte[] bytes, int first, int length, bool twosComplement)
        {
            int nbytes = ((length - 1) / 8) + 1;
            BitArray bits = new BitArray(bytes);
            BitArray resultBits = new BitArray(nbytes * 8);
            if (twosComplement && bits[first + length - 1])
                resultBits.SetAll(true);
            for (int i = 0; i < length; i++)
            {
                resultBits[i] = bits[first + i];
            }
            byte[] result = new byte[nbytes];
            resultBits.CopyTo(result, 0);
            return result;
        }
    }
}