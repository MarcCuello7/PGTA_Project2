using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Project2_Code
{

    public class LeerArchivo

    {
        public LeerArchivo(string file)
        {            
            List<CAT48> listaCAT48 = new List<CAT48>();
            FileStream stream = File.Create(file);
            int CAT = stream.ReadByte();
            int LEN = Utils.Read
            if (CAT != 48)
            {
                Console.WriteLine("Invalid category");
            }

        }
    }
}
