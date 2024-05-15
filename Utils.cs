using System;
using System.IO;
using System.Collections;

namespace Project2_Code
{
    public static class Utils
    {
        public static void ReverseBitArray(BitArray array)
        {
            int length = array.Length;
            int mid = (length / 2);

            for (int i = 0; i < mid; i++)
            {
                bool bit = array[i];
                array[i] = array[length - i - 1];
                array[length - i - 1] = bit;
            }
        }

        public static sbyte ReadS1(BinaryReader data)
        {
            return data.ReadSByte();
        }

        public static short ReadS2(BinaryReader data)
        {
            return BitConverter.ToInt16(ReadBytesBigEndian(data, 2), 0);
        }


        public static byte ReadU1(BinaryReader data)
        {
            return data.ReadByte();
        }

        public static ushort ReadU2(BinaryReader data)
        {
            return BitConverter.ToUInt16(ReadBytesBigEndian(data, 2), 0);
        }                  

        public static byte[] ReadBytesBigEndian(BinaryReader data, int count)
        {
            byte[] bytes = data.ReadBytes(count);
            Array.Reverse(bytes);
            return bytes;
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

        public static sbyte ExtractS1(byte[] bytes, int first, int length)
        {
            byte[] extracted = ExtractBits(bytes, first, length, true);
            return (sbyte)extracted[0];
        }

        public static ushort ExtractU2(byte[] bytes, int first, int length)
        {
            byte[] extracted = ExtractBits(bytes, first, length, false);
            return BitConverter.ToUInt16(extracted, 0);
        }

        public static short ExtractS2(byte[] bytes, int first, int length)
        {
            byte[] extracted = ExtractBits(bytes, first, length, true);
            return BitConverter.ToInt16(extracted, 0);
        }
        public static byte ExtractBits(byte b, byte first, byte last)
        {
            byte mask = 0;
            for (byte i = first; i <= last; i++)
            {
                mask |= (byte)(1 << i);
            }
            b &= mask;
            b >>= first;
            return b;
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

        public static string DecToDMS(double lat, double lon)
        {
            return FormattableString.Invariant($"{Math.Abs((int)lat)}°{(int)(Math.Abs(lat) % 1.0 * 60.0)}'{Math.Abs(lat) * 3600.0 % 60:F1}\"{(lat >= 0 ? "N" : "S")}\n{Math.Abs((int)lon)}°{(int)(Math.Abs(lon) % 1.0 * 60.0)}'{Math.Abs(lon) * 3600.0 % 60:F1}\"{(lon >= 0 ? "E" : "W")}");
        }
    }
}