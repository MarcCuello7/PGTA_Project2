using System;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.Diagnostics;

namespace Project2_Code
{
    public class CAT48
    {
        public BitArray FSPEC;

        //Data Item I048/010, Data Source Identifier
        public byte SAC;
        public byte SIC;

        //Data Item I048/020, Target Report Descriptor
        public byte TYP_020;
        public bool SIM_020;
        public bool RDP_020;
        public bool SPI_020;
        public bool RAB_020;
        public bool TST_020;
        public bool ERR_020;
        public bool XPP_020;
        public bool ME_020;
        public bool MI_020;
        public byte FOEFRI_020;
        public byte ADSB_020;
        public byte SCN_020;
        public byte PAI_020;

        //Data Item I048/030, Warning/Error Conditions and Target Classification
        //No decodification needed

        //Data Item I048/040, Measured Position in Polar Co-ordinates
        public double RHO;
        public double THETA;

        //Data Item I048/042, Calculated Position in Cartesian Co-ordinates
        public double COMPX;
        public double COMPY;

        //Data Item I048/050, Mode-2 Code in Octal Representation
        //No decodification needed

        //Data Item I048/055, Mode-1 Code in Octal Representation
        //No decodification needed

        //Data Item I048/060, Mode-2 Code Confidence Indicator
        //No decodification needed

        //Data Item I048/065, Mode-1 Code Confidence Indicator
        //No decodification needed

        //Data Item I048/070, Mode-3/A Code in Octal Representation
        public bool V_070;
        public bool G_070;
        public bool L_070;
        public ushort MODE3AREPLY;

        //Data Item I048/080, Mode-3/A Code Confidence Indicator
        //No decodification needed

        //Data Item I048/090, Flight Level in Binary Representation
        public bool V_090;
        public bool G_090;
        public double FL;

        //Data Item I048/100, Mode-C Code and Code Confidence Indicator
        //No decodification needed

        //Data Item I048/110, Height Measured by a 3D Radar
        public double HEIGHT3D;

        //Data Item I048/120, Radial Doppler Speed
        //No decodification needed

        //Data Item I048/130, Radar Plot Characteristics
        public double SRL_130;
        public double SRR_130;
        public sbyte SAM_130;
        public double PRL_130;
        public sbyte PAM_130;
        public double RPD_130;
        public double APD_130;

        //Data Item I048/140, Time of Day
        public double TIME;

        //Data Item I048/161, Track Number
        public ushort TN;

        //Data Item I048/170, Track Status
        public bool CNF_170;
        public byte RAD_170;
        public bool DOU_170;
        public bool MAH_170;
        public byte CDM_170;
        public bool TRE_170;
        public bool GHO_170;
        public bool SUP_170;
        public bool TCC_170;

        //Data Item I048/200, Calculated Track Velocity in Polar Co-ordinates
        public double GS;
        public double HEADING;

        //Data Item I048/210, Track Quality
        //No decodification needed

        //Data Item I048/220, Aircraft Address
        public byte[] ADDRESS;

        //Data Item I048/230, Communications/ACAS Capability and Flight Status
        public byte COM_230;
        public byte STAT_230;
        public bool SI_230;
        public bool MSSC_230;
        public bool ARC_230;
        public bool AIC_230;
        public bool B1A_230;
        public byte B1B_230;

        //Data Item I048/240, Aircraft Identification
        public byte[] IDENTIFICATION;

        //Data Item I048/250, BDS Register Data
        public byte[][] BDSDATA;
        public byte[] BDS;

        //Data Item I048/260, ACAS Resolution Advisory Report
        //No decodification needed

        //SP-Data Item, Special Purpose Field
        //No decodification needed

        //RE-Data Item, Reserved Expansion Field
        //No decodification needed



        //Enumeration of the category 48 Standard User Application Profile
        enum UAP 
        {
            I048_010 = 0,   // Data Source Identifier
            I048_140,       // Time-of-Day
            I048_020,       // Target Report Descriptor
            I048_040,       // Measured Position in Slant Polar Coordinates
            I048_070,       // Mode-3/A Code in Octal Representation
            I048_090,       // Flight Level in Binary Representation
            I048_130,       // Radar Plot Characteristics
                            // Field Extension Indicator
            I048_220 = 8,   // Aircraft Address
            I048_240,       // Aircraft Identification
            I048_250,       // Mode S MB Data
            I048_161,       // Track Number
            I048_042,       // Calculated Position in Cartesian Coordinates
            I048_200,       // Calculated Track Velocity in Polar Representation
            I048_170,       // Track Status
                            // Field Extension Indicator
            I048_210 = 16,  // Track Quality
            I048_030,       // Warning/Error Conditions/Target Classification
            I048_080,       // Mode-3/A Code Confidence Indicator
            I048_100,       // Mode-C Code and Confidence Indicator
            I048_110,       // Height Measured by 3D Radar
            I048_120,       // Radial Doppler Speed
            I048_230,       // Communications / ACAS Capability and Flight Status
                            // Field Extension Indicator
            I048_260 = 24,  // ACAS Resolution Advisory Report
            I048_055,       // Mode-1 Code in Octal Representation
            I048_050,       // Mode-2 Code in Octal Representation
            I048_065,       // Mode-1 Code Confidence Indicator
            I048_060,       // Mode-2 Code Confidence Indicator
            SP_DI,          // Item Special Purpose Field
            RE_DI,          // Item Reserved Expansion Field
        }



        //METHODS
        private void ParseFSPEC(BinaryReader data)
        {            
            byte[] FSPEC = new byte[] {0, 0, 0, 0};
            for (int i = 0; i < FSPEC.Length; i++)
            {
                FSPEC[3-i] = Utils.ReadU1(data);
                if ((FSPEC[3-i] & 1) == 0)
                {                    
                    break;
                }            
            }            
            this.FSPEC = new BitArray(FSPEC);
            Utils.ReverseBitArray(this.FSPEC);
        }



        //CONSTRUCTOR
        public CAT48(BinaryReader data)
        {
            List<Action<BinaryReader>> DataItemActions = new List<Action<BinaryReader>>
            {
                ParseI048_010,          ParseI048_140,          ParseI048_020,          ParseI048_040, 
                ParseI048_070,          ParseI048_090,          ParseI048_130,          null, 
                ParseI048_220,          ParseI048_240,          ParseI048_250,          ParseI048_161,      
                ParseI048_042,          ParseI048_200,          ParseI048_170,          null, 
                SkipFixedAction(4),     SkipVariable,           SkipFixedAction(2),     SkipFixedAction(4), 
                ParseI048_110,          ParseI048_120,          ParseI048_230,          null,       
                SkipFixedAction(7),     SkipFixedAction(1),     SkipFixedAction(2),     SkipFixedAction(1), 
                SkipFixedAction(2),     ParseSP_DI,             ParseRE_DI,             null
            };
                        
            ParseFSPEC(data);            
            for (int i = 0; i < this.FSPEC.Length; i++)
            {                
                if ((i % 8) == 7)
                {
                    if (!this.FSPEC[i]) break;
                }
                else if (this.FSPEC[i])
                {
                    DataItemActions[i](data);
                }
            }        
        }

        //HELPER FUNCTIONS
        public Action<BinaryReader> SkipFixedAction(int count)
        {
            return (BinaryReader data) => data.ReadBytes(count);
        }

        public void SkipVariable(BinaryReader data)
        {
            while ((data.ReadByte() & 1) == 1) ;
        }

        //DATA ITEMS FUNCTIONS        
        private void ParseI048_010(BinaryReader data)
        {
            this.SAC = Utils.ReadU1(data);
            this.SIC = Utils.ReadU1(data);
        }   
        
        private void ParseI048_140(BinaryReader data)
        {
            byte[] bytes = data.ReadBytes(3);
            this.TIME = ((bytes[0] << 16) | (bytes[1] << 8) | bytes[2]) / 128.0;
        }
        
        private void ParseI048_020(BinaryReader data)
        {
            byte octet1 = Utils.ReadU1(data);
            this.TYP_020 = Utils.ExtractBits(octet1, 5, 7);
            this.SIM_020 = Utils.ExtractBool(octet1, 4);
            this.RDP_020 = Utils.ExtractBool(octet1, 3);
            this.SPI_020 = Utils.ExtractBool(octet1, 2);
            this.RAB_020 = Utils.ExtractBool(octet1, 1);
            if (!Utils.ExtractBool(octet1, 0)) 
            {
                return;
            }

            byte octet2 = Utils.ReadU1(data);
            this.TST_020 = Utils.ExtractBool(octet2, 7);
            this.ERR_020 = Utils.ExtractBool(octet2, 6);
            this.XPP_020 = Utils.ExtractBool(octet2, 5);
            this.ME_020 = Utils.ExtractBool(octet2, 4);
            this.MI_020 = Utils.ExtractBool(octet2, 3);
            this.FOEFRI_020 = Utils.ExtractBits(octet1, 1, 2);
            if (!Utils.ExtractBool(octet2, 0))
            {
                return;
            }

            byte octet3 = Utils.ReadU1(data);
            this.ADSB_020 = Utils.ExtractBits(octet1, 6, 7);
            this.SCN_020 = Utils.ExtractBits(octet1, 4, 5);
            this.PAI_020 = Utils.ExtractBits(octet1, 2, 3);
        }
        
        private void ParseI048_040(BinaryReader data)
        {
            this.RHO = Utils.ReadU2(data) / 256.0;
            this.THETA = Utils.ReadU2(data) * (360.0 / Math.Pow(2, 16));
        }
        
        private void ParseI048_070(BinaryReader data)
        {
            byte[] bytes = Utils.ReadBytesBigEndian(data, 2);            
            this.V_070 = Utils.ExtractBool(bytes[1], 7);
            this.G_070 = Utils.ExtractBool(bytes[1], 6);
            this.L_070 = Utils.ExtractBool(bytes[1], 5);
            this.MODE3AREPLY = Utils.ExtractU2(bytes, 0, 12);
        }
        
        private void ParseI048_090(BinaryReader data)
        {
            byte[] bytes = Utils.ReadBytesBigEndian(data, 2);
            this.V_090 = Utils.ExtractBool(bytes[1], 7);
            this.G_090 = Utils.ExtractBool(bytes[1], 6);
            this.FL = Utils.ExtractS2(bytes, 0, 14) * 0.25;
        }
        
        private void ParseI048_130(BinaryReader data)
        {
            byte subfields = Utils.ReadU1(data);            
            if (Utils.ExtractBool(subfields, 7))
            {
                this.SRL_130 = Utils.ReadU1(data) * (360.0 / Math.Pow(2, 13));
            }
            if (Utils.ExtractBool(subfields, 6))
            {
                this.SRR_130 = Utils.ReadU1(data);
            }
            if (Utils.ExtractBool(subfields, 5))
            {
                this.SAM_130 = Utils.ReadS1(data);
            }
            if (Utils.ExtractBool(subfields, 4))
            {
                this.PRL_130 = Utils.ReadU1(data) * (360.0 / Math.Pow(2, 13));
            }
            if (Utils.ExtractBool(subfields, 3))
            {
                this.PAM_130 = Utils.ReadS1(data);
            }
            if (Utils.ExtractBool(subfields, 2))
            {
                this.RPD_130 = Utils.ReadS1(data) / 256.0;
            }
            if (Utils.ExtractBool(subfields, 1))
            {
                this.APD_130 = Utils.ReadS1(data) * (360.0 / Math.Pow(2, 14));
            }
        }
        
        private void ParseI048_220(BinaryReader data)
        {
            this.ADDRESS = data.ReadBytes(3);
        }
        
        private void ParseI048_240(BinaryReader data)
        {
            byte[] bytes = Utils.ReadBytesBigEndian(data, 6);
            this.IDENTIFICATION = new byte[8];
            for (int i = 0; i < 8; i++)
            {
                IDENTIFICATION[7-i] = Utils.ExtractU1(bytes, i * 6, 6);
            }
        }
        
        private void ParseI048_250(BinaryReader data)
        {
            byte REP = Utils.ReadU1(data);
            this.BDSDATA = new byte[REP][];
            this.BDS = new byte[REP];
            for (int i = 0; i < REP; i++)
            {
                this.BDSDATA[i] = data.ReadBytes(7);
                this.BDS[i] = Utils.ReadU1(data);
            }
        }
        
        private void ParseI048_161(BinaryReader data)
        {
            this.TN = Utils.ReadU2(data);
        }
        
        private void ParseI048_042(BinaryReader data)
        {
            byte[] bytes = Utils.ReadBytesBigEndian(data, 4);
            this.COMPX = Utils.ExtractS2(bytes, 16, 16) / 128.0;
            this.COMPY = Utils.ExtractS2(bytes, 0, 16) / 128.0;
        }
        
        private void ParseI048_200(BinaryReader data)
        {
            this.GS = Utils.ReadU2(data) * Math.Pow(2, -14);
            this.HEADING = Utils.ReadU2(data) * (360.0 / Math.Pow(2, 16));
        }
        
        private void ParseI048_170(BinaryReader data)
        {
            byte octet1 = Utils.ReadU1(data);
            this.CNF_170 = Utils.ExtractBool(octet1, 7);
            this.RAD_170 = Utils.ExtractBits(octet1, 5, 6);
            this.DOU_170 = Utils.ExtractBool(octet1, 4);
            this.MAH_170 = Utils.ExtractBool(octet1, 3);
            this.CDM_170 = Utils.ExtractBits(octet1, 1, 2);
            if (!Utils.ExtractBool(octet1, 0))
            {
                return;
            }

            byte octet2 = Utils.ReadU1(data);
            this.TRE_170 = Utils.ExtractBool(octet2, 7);
            this.GHO_170 = Utils.ExtractBool(octet2, 6);
            this.SUP_170 = Utils.ExtractBool(octet2, 5);
            this.TCC_170 = Utils.ExtractBool(octet2, 4); 
        }
        
        private void ParseI048_110(BinaryReader data)
        {
            byte[] bytes = Utils.ReadBytesBigEndian(data, 2);            
            this.HEIGHT3D = Utils.ExtractS2(bytes, 0, 14) * 25.0;            
        }
        
        private void ParseI048_120(BinaryReader data)
        {
            byte octet1 = Utils.ReadU1(data);
            if (!Utils.ExtractBool(octet1, 0))
            {
                return;
            }

            if (Utils.ExtractBool(octet1, 7))
            {
                data.ReadBytes(2);
            }

            if (Utils.ExtractBool(octet1, 6))
            {
                byte REP = Utils.ReadU1(data);
                for (int i = 0; i < REP; i++)
                {
                    data.ReadBytes(6);
                }
            }            
        }
        
        private void ParseI048_230(BinaryReader data)
        {
            byte octet1 = Utils.ReadU1(data);
            this.COM_230 = Utils.ExtractBits(octet1, 5, 7);
            this.STAT_230 = Utils.ExtractBits(octet1, 2, 4);
            this.SI_230 = Utils.ExtractBool(octet1, 1);
            byte octet2 = Utils.ReadU1(data);
            this.MSSC_230 = Utils.ExtractBool(octet2, 7);
            this.ARC_230 = Utils.ExtractBool(octet2, 6);
            this.AIC_230 = Utils.ExtractBool(octet2, 5);
            this.B1A_230 = Utils.ExtractBool(octet2, 4);
            this.B1B_230 = Utils.ExtractBits(octet2, 0, 3);
        }
        
        private void ParseSP_DI(BinaryReader data)
        {
            byte LEN = Utils.ReadU1(data);
            data.ReadBytes(LEN - 1);
        }
        
        private void ParseRE_DI(BinaryReader data)
        {
            byte LEN = Utils.ReadU1(data);
            data.ReadBytes(LEN - 1);
        }        
    }
}