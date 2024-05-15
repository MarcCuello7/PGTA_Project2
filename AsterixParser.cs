using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Data;
using System.Reflection;
using System.Linq;
using System.Text;

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
                    while (reader.BaseStream.Position - blockStart < LEN)
                    {
                        CAT48 record = new CAT48(reader, i++);
                        CAT48list.Add(record);
                    }
                }
            }
        }

        public void CreateDataTable()
        {
            Type type = typeof(CAT48);
            FieldInfo[] fields = type.GetFields();
            fields = fields.Where(f => !NoExport.Contains(f.Name)).ToArray();

            this.CAT48table = new DataTable();
            CAT48table.TableName = type.FullName;
            foreach (FieldInfo field in fields)
            {
                DataColumn dataColumn;
                if (!ExportValues.ContainsKey(field.Name))
                {
                    dataColumn = new DataColumn(field.Name, Nullable.GetUnderlyingType(field.FieldType) ?? field.FieldType);                    
                }
                else
                {
                    dataColumn = new DataColumn(field.Name, typeof(String));
                }                
                CAT48table.Columns.Add(dataColumn);
            }            

            foreach (CAT48 record in CAT48list)
            {
                object[] values = new object[fields.Length];
                for (int i = 0; i < fields.Length; i++)
                {
                    object exportValues;
                    ExportValues.TryGetValue(fields[i].Name, out exportValues);
                    object value = fields[i].GetValue(record);
                    if (exportValues == null)
                    {
                        values[i] = value;
                    }
                    else if (exportValues.GetType().IsArray)
                    {
                        values[i] = value == null ? null : ((string[])exportValues)[Convert.ToByte(value)];                        
                    }
                    else if (exportValues.GetType() == typeof(Func<object, string>))
                    {
                        values[i] = value == null ? null : ((Func<object, string>)exportValues)(value);
                    }
                }
                CAT48table.Rows.Add(values);
            }            
        }

        public void ExportToCSV(string file)
        {
            StringBuilder stringBuilder = new StringBuilder();

            IEnumerable<string> columnNames = this.CAT48table.Columns.Cast<DataColumn>().Select(column => column.ColumnName);
            stringBuilder.AppendLine(string.Join(";", columnNames));

            foreach (DataRowView rowView in this.CAT48table.DefaultView)
            {
                IEnumerable<string> fields = rowView.Row.ItemArray.Select(field => field == DBNull.Value ? "N/A" : field.ToString());
                stringBuilder.AppendLine(string.Join(";", fields));
            }

            File.WriteAllText(file, stringBuilder.ToString());
        }


        public static readonly List<string> NoExport = new List<string> { "FSPEC" };

        public static readonly Dictionary<string, object> ExportValues = new Dictionary<string, object>
        {
            { "TYP_020", new string[] { "No detection", "Single PSR detection", "Single PSR detection", "SSR + PSR detection",
                                        "Single ModeS All-Call", "Single ModeS Roll-Call", "ModeS All-Call + PSR", "ModeS Roll-Call + PSR" } },
            { "SIM_020", new string[] { "Actual target report", "Simulated target report" } },
            { "RDP_020", new string[] { "Report from RDP Chain 1", "Report from RDP Chain 2" } },
            { "SPI_020", new string[] { "Absence of SPI", "Special Position Identification" } },
            { "RAB_020", new string[] { "Report from aircraft transponder", "Report from field monitor (fixed transponder)" } },
            { "TST_020", new string[] { "Real target report", "Test target report" } },
            { "ERR_020", new string[] { "No Extended Range", "Extended Range present" } },
            { "XPP_020", new string[] { "No X-Pulse present", "X-Pulse present" } },
            { "ME_020", new string[] { "No military emergency", "Military emergency" } },
            { "MI_020", new string[] { "No military identification", "Military identification" } },
            { "FOEFRI_020", new string[] { "No Mode 4 interrogation", "Friendly target", "Unknown target", "No reply" } },
            { "ADSB_020", new string[] { "ADSB not populated and not available", "ADSB not populated and available",
                                         "ADSB populated and not available", "ADSB populated and available" } },
            { "SCN_020", new string[] { "SCN not populated and not available", "SCN not populated and available",
                                        "SCN populated and not available", "SCN populated and available" } },
            { "PAI_020", new string[] { "PAI not populated and not available", "PAI not populated and available",
                                        "PAI populated and not available", "PAI populated and available" } },

            { "V_070", new string[] { "Code validated", "Code not validated" } },
            { "G_070", new string[] { "Default", "Garbled code" } },
            { "L_070", new string[] { "Mode-3/A code derived from the reply of the transponder",
                                      "Mode-3/A code not extracted during the last scan" } },

            { "V_090", new string[] { "Code validated", "Code not validated" } },
            { "G_090", new string[] { "Default", "Garbled code" } },

            { "CNF_170", new string[] { "Confirmed Track", "Tentative Track" } },
            { "RAD_170", new string[] { "Combined Track", "PSR Track", "SSR/Mode S Track", "Invalid" } },
            { "DOU_170", new string[] { "Normal confidence", "Low confidence in plot to track association" } },
            { "MAH_170", new string[] { "No horizontal man.sensed", "Horizontal man. sensed" } },
            { "CDM_170", new string[] { "Maintaining", "Climbing", "Descending", "Unknown" } },
            { "TRE_170", new string[] { "Track still alive", "End of track lifetime(last report for this track)" } },
            { "GHO_170", new string[] { "True target track", "Ghost target track" } },
            { "SUP_170", new string[] { "No", "Yes" } },
            { "TCC_170", new string[] { "Tracking performed in so-called 'Radar Plane', i.e. neither slant range correction nor stereographical projection was applied",
                                        "Slant range correction and a suitable projection technique are used to track in a 2D.reference plane, tangential to the earth model at the Radar Site co-ordinates" } },

            { "COM_230", new string[] { "No communications capability (surveillance only)", "Comm. A and Comm. B capability",
                                        "Comm. A, Comm. B and Uplink ELM", "Comm. A, Comm. B, Uplink ELM and Downlink ELM",
                                        "Level 5 Transponder capability", "Not assigned", "Not assigned", "Not assigned" } },
            { "STAT_230", new string[] { "No alert, no SPI, aircraft airborne", "No alert, no SPI, aircraft on ground",
                                         "Alert, no SPI, aircraft airborne", "Alert, no SPI, aircraft on ground",
                                         "Alert, SPI, aircraft airborne or on ground", "No alert, SPI, aircraft airborne or on ground",
                                         "Not assigned", "Unknown" } },
            { "SI_230", new string[] { "SI-Code Capable", "II-Code Capable" } },
            { "MSSC_230", new string[] { "No", "Yes" } },
            { "ARC_230", new string[] { "100 ft resolution", "25 ft resolution" } },
            { "AIC_230", new string[] { "No", "Yes" } },
            { "B1A_230", new string[] { "ACAS has failed or is on standby", "ACAS is operational" } },
            { "B1B_230", new string[] { "No hybrid surveillance capability and generation of TAs only, RTCA DO-185",
                                        "Hybrid surveillance capability and generation of TAs only, RTCA DO-185",
                                        "No hybrid surveillance capability and generation of both TAs and RAs, RTCA DO-185",
                                        "Hybrid surveillance capability and generation of both TAs and RAs, RTCA DO-185",
                                        "No hybrid surveillance capability and generation of TAs only, RTCA DO-185A",
                                        "Hybrid surveillance capability and generation of TAs only, RTCA DO-185A",
                                        "No hybrid surveillance capability and generation of both TAs and RAs, RTCA DO-185A",
                                        "Hybrid surveillance capability and generation of both TAs and RAs, RTCA DO-185A",
                                        "No hybrid surveillance capability and generation of TAs only, RTCA DO-185B",
                                        "Hybrid surveillance capability and generation of TAs only, RTCA DO-185B",
                                        "No hybrid surveillance capability and generation of both TAs and RAs, RTCA DO-185B",
                                        "Hybrid surveillance capability and generation of both TAs and RAs, RTCA DO-185B",
                                        "Reserved for future versions", "Reserved for future versions",
                                        "Reserved for future versions", "Reserved for future versions" } },

            { "VNAV", new string[] { "Not active", "Active" } },
            { "ALTHOLD", new string[] { "Not active", "Active" } },
            { "APPROACH", new string[] { "Not active", "Active" } },
            { "ALTSOURCE", new string[] { "Unknown", "Aircraft altitude", " FCU/MCP selected altitude", "FMS selected altitude" } },

            { "ADDRESS", new Func<object, string>(address => BitConverter.ToString((byte[])address).Replace("-", ""))},
            { "MODE3AREPLY", new Func<object, string>(mode3A => Convert.ToString((ushort)mode3A, 8)) }
        };
    }
}