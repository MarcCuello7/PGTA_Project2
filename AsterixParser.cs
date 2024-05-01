using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Project2_Code
{

    public class AsterixParser

    {
        List<CAT48> CAT48list;
        public AsterixParser(string file)
        {
            CAT48list = new List<CAT48>();
            FileStream stream = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.Read);
            BinaryReader reader = new BinaryReader(stream);
            int i = 0;
            
            while (reader.BaseStream.Position != reader.BaseStream.Length)
            {
                long blockStart = reader.BaseStream.Position;
                byte CAT = Utils.ReadU1(reader);
                ushort LEN = Utils.ReadU2(reader);                
                if (CAT != 48)
                {
                    Console.WriteLine("Invalid category");
                }
                else
                {
                    while (reader.BaseStream.Position-blockStart < LEN)
                    {                        
                        i++;
                        CAT48 record = new CAT48(reader);
                        CAT48list.Add(record);
                    }
                }
            }
            Debug.WriteLine("Finished");
        }
    }
}