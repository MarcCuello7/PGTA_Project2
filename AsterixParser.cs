using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Data;
using System.Reflection;
using System.Linq;

namespace Project2_Code
{

    public class AsterixParser
    {
        public List<CAT48> CAT48list;
        public DataTable CAT48table;
        public AsterixParser(string file)
        {
            CreateRecordList(file);
            CreateDataTable();
            Debug.WriteLine("Finished");
        }

        public void CreateRecordList(string file)
        {
            this.CAT48list = new List<CAT48>();
            FileStream stream = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.Read);
            BinaryReader reader = new BinaryReader(stream);

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
                    while (reader.BaseStream.Position - blockStart < LEN)
                    {
                        CAT48 record = new CAT48(reader);
                        CAT48list.Add(record);
                    }
                }
            }
        }

        //public bool ExportToCSV(string file)
        //{

        //}

        public void CreateDataTable()
        {
            Type type = typeof(CAT48);
            FieldInfo[] fields = type.GetFields();
            fields = fields.Where(f => f.GetCustomAttributes(typeof(NoExport), true).Length == 0).ToArray();

            this.CAT48table = new DataTable();
            CAT48table.TableName = type.FullName;
            foreach (FieldInfo field in fields)
            {
                if (field.GetCustomAttributes(typeof(ExportValues), true).Length == 0)
                {
                    CAT48table.Columns.Add(new DataColumn(field.Name, Nullable.GetUnderlyingType(field.FieldType) ?? field.FieldType));
                }
                else
                {
                    CAT48table.Columns.Add(new DataColumn(field.Name, System.Type.GetType("System.String")));
                }
            }            

            foreach (CAT48 record in CAT48list)
            {
                object[] values = new object[fields.Length];
                for (int i = 0; i < fields.Length; i++)
                {
                    ExportValues[] exportValues = (ExportValues[])fields[i].GetCustomAttributes(typeof(ExportValues), true);
                    if (exportValues.Length == 0)
                    {
                        values[i] = fields[i].GetValue(record);
                    }
                    else
                    {
                        byte valueIndex = Convert.ToByte(fields[i].GetValue(record));
                        values[i] = exportValues[0].values[valueIndex];
                    }                    
                }

                CAT48table.Rows.Add(values);
            }            
        }
    }
}