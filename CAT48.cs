using System;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.Text;
using MultiCAT6.Utils;
using System.Diagnostics;

namespace Project2_Code
{

    public class CAT48
    {
        public BitArray FSPEC;
        public int INDEX;

        //Data Item I048/010, Data Source Identifier
        public byte? SAC;
        public byte? SIC;

        //Data Item I048/140, Time of Day
        public double TIME;

        //Additional values
        public double LATITUDE;
        public double LONGITUDE;
        public double HEIGHT;

        //Data Item I048/020, Target Report Descriptor 
        public byte? TYP_020;
        public bool? SIM_020;
        public bool? RDP_020;       
        public bool? SPI_020;
        public bool? RAB_020;
        public bool? TST_020;
        public bool? ERR_020;
        public bool? XPP_020;
        public bool? ME_020;
        public bool? MI_020;
        public byte? FOEFRI_020;
        public byte? ADSB_020;
        public byte? SCN_020;
        public byte? PAI_020;

        //Data Item I048/040, Measured Position in Polar Co-ordinates
        public double? RHO;
        public double? THETA;

        //Data Item I048/070, Mode-3/A Code in Octal Representation
        public bool? V_070;
        public bool? G_070;
        public bool? L_070;
        public ushort? MODE3AREPLY;

        //Data Item I048/090, Flight Level in Binary Representation
        public bool? V_090;
        public bool? G_090;
        public double? FL;

        //Data Item I048/130, Radar Plot Characteristics
        public double? SRL_130;
        public double? SRR_130;
        public sbyte? SAM_130;
        public double? PRL_130;
        public sbyte? PAM_130;
        public double? RPD_130;
        public double? APD_130;

        //Data Item I048/220, Aircraft Address
        public byte[] ADDRESS;

        //Data Item I048/240, Aircraft Identification
        public string IDENTIFICATION;

        //Data Item I048/250, BDS Register Data        
        public string BDSCODES;
        //Data Item I048/250, BDS code 4,0
        public double? MCPALT;
        public double? FMSALT;
        public double? BARPRESS;
        public bool? VNAV;
        public bool? ALTHOLD;
        public bool? APPROACH;
        public byte? ALTSOURCE;
        //Data Item I048/250, BDS code 5,0
        public double? RA;
        public double? TTA;
        public double? BDSGS;
        public double? TAR;
        public double? TAS;
        //Data Item I048/250, BDS code 4,0
        public double? MAGHDG;
        public double? IAS;
        public double? MACH;
        public double? BAR;
        public double? IVV;

        //Data Item I048/161, Track Number
        public ushort TN;

        //Data Item I048/042, Calculated Position in Cartesian Co-ordinates
        public double? COMPX;
        public double? COMPY;

        //Data Item I048/200, Calculated Track Velocity in Polar Co-ordinates
        public double? GS;
        public double? HEADING;

        //Data Item I048/170, Track Status
        public bool? CNF_170;
        public byte? RAD_170;
        public bool? DOU_170;
        public bool? MAH_170;
        public byte? CDM_170;
        public bool? TRE_170;
        public bool? GHO_170;
        public bool? SUP_170;
        public bool? TCC_170;

        //Data Item I048/210, Track Quality
        //No decodification needed

        //Data Item I048/030, Warning/Error Conditions and Target Classification
        //No decodification needed

        //Data Item I048/080, Mode-3/A Code Confidence Indicator
        //No decodification needed

        //Data Item I048/100, Mode-C Code and Code Confidence Indicator
        //No decodification needed

        //Data Item I048/110, Height Measured by a 3D Radar
        public double? HEIGHT3D;

        //Data Item I048/120, Radial Doppler Speed
        //No decodification needed

        //Data Item I048/230, Communications/ACAS Capability and Flight Status
        public byte? COM_230;
        public byte? STAT_230;
        public bool? SI_230;
        public bool? MSSC_230;
        public bool? ARC_230;
        public bool? AIC_230;
        public bool? B1A_230;
        public byte? B1B_230;

        //Data Item I048/260, ACAS Resolution Advisory Report
        //No decodification needed

        //Data Item I048/055, Mode-1 Code in Octal Representation
        //No decodification needed

        //Data Item I048/050, Mode-2 Code in Octal Representation
        //No decodification needed

        //Data Item I048/065, Mode-1 Code Confidence Indicator
        //No decodification needed

        //Data Item I048/060, Mode-2 Code Confidence Indicator
        //No decodification needed

        //SP-Data Item, Special Purpose Field
        //No decodification needed

        //RE-Data Item, Reserved Expansion Field
        //No decodification needed



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
        public CAT48(BinaryReader data, int index)
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
            ComputeAdditional(index);
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
            StringBuilder builder = new StringBuilder();
            for (int i = 7; i > 0; i--)
            {
                byte b = Utils.ExtractU1(bytes, i * 6, 6);
                if (0x01 <= b && b <= 0x1A)
                {
                    builder.Append((char)(b + 0x40));
                }
                else if (0x30 <= b && b <= 0x39)
                {
                    builder.Append((char)b);
                }
            }
            this.IDENTIFICATION = builder.ToString();            
        }
        
        private void ParseI048_250(BinaryReader data)
        {
            byte REP = Utils.ReadU1(data);
            byte[] BDSDATA = new byte[7];
            byte BDS = new byte();
            StringBuilder BDSCODES = new StringBuilder(); 
            for (int i = 0; i < REP; i++)
            {
                BDSDATA = Utils.ReadBytesBigEndian(data, 7);
                BDS = Utils.ReadU1(data);
                BDSCODES.Append($"BDS: { Utils.ExtractBits(BDS, 4, 7)},{Utils.ExtractBits(BDS, 0, 3)}{(i < (REP - 1) ? " | " : "")}");
                if (BDS == 64)
                {
                    if (Utils.ExtractBool(BDSDATA[6], 7)) this.MCPALT = Utils.ExtractU2(BDSDATA, 43, 12) * 16;
                    if (Utils.ExtractBool(BDSDATA[5], 2)) this.FMSALT = Utils.ExtractU2(BDSDATA, 30, 12) * 16;
                    if (Utils.ExtractBool(BDSDATA[3], 5)) this.BARPRESS = (Utils.ExtractU2(BDSDATA, 17, 12) * 0.1) + 800.0;
                    if (Utils.ExtractBool(BDSDATA[1], 0))
                    {
                        this.VNAV = Utils.ExtractBool(BDSDATA[0], 7);
                        this.ALTHOLD = Utils.ExtractBool(BDSDATA[0], 6);
                        this.APPROACH = Utils.ExtractBool(BDSDATA[0], 5);
                    }
                    if (Utils.ExtractBool(BDSDATA[0], 2)) this.ALTSOURCE = Utils.ExtractU1(BDSDATA, 0, 2);
                }

                else if (BDS == 80)
                {
                    if (Utils.ExtractBool(BDSDATA[6], 7)) this.RA = Utils.ExtractS2(BDSDATA, 45, 10) * (45.0 / 256.0);
                    if (Utils.ExtractBool(BDSDATA[5], 4)) this.TTA = Utils.ExtractS2(BDSDATA, 33, 11) * (90.0 / 512.0);
                    if (Utils.ExtractBool(BDSDATA[4], 0)) this.BDSGS = Utils.ExtractU2(BDSDATA, 22, 10) * (1024.0 / 512.0);
                    if (Utils.ExtractBool(BDSDATA[2], 5)) this.TAR = Utils.ExtractS2(BDSDATA, 11, 10) * (8.0 / 256.0);
                    if (Utils.ExtractBool(BDSDATA[1], 2)) this.TAS = Utils.ExtractU2(BDSDATA, 0, 10) * 2;
                }

                else if (BDS == 96)
                {
                    if (Utils.ExtractBool(BDSDATA[6], 7)) this.MAGHDG = Utils.ExtractS2(BDSDATA, 44, 11) * (90.0 / 512.0);
                    if (Utils.ExtractBool(BDSDATA[5], 3)) this.IAS = Utils.ExtractU2(BDSDATA, 33, 10);
                    if (Utils.ExtractBool(BDSDATA[4], 0)) this.MACH = Utils.ExtractU2(BDSDATA, 22, 10) * (2.048 / 512.0);
                    if (Utils.ExtractBool(BDSDATA[2], 5)) this.BAR = Utils.ExtractS2(BDSDATA, 11, 10) * (8192.0 / 256.0);
                    if (Utils.ExtractBool(BDSDATA[1], 2)) this.IVV = Utils.ExtractS2(BDSDATA, 0, 10) * (8192.0 / 256.0);
                }
            }
            this.BDSCODES = BDSCODES.ToString();
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
            this.GS = Utils.ReadU2(data) * Math.Pow(2, -14) * 3600;
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


        //ADDITIONAL VALUES
        private void ComputeAdditional(int index)
        {
            double radarAltitude = 2.007 + 25.25; //[m]
            double earthRadius = 6371000.0; //[m]
            double rho = this.RHO.Value * 1852.0; //[m]
            if (!this.FL.HasValue) this.FL = 0.0;
            double flightLevel = this.FL.Value * 100.0 * 0.3048; //[m]
            double theta = this.THETA.Value * Math.PI / 180.0; //[rad]

            double a = flightLevel + earthRadius;
            double b = radarAltitude + earthRadius;
            double c = rho;
            double A = Math.Acos((Math.Pow(a, 2) - Math.Pow(b, 2) - Math.Pow(c, 2)) / (-2.0 * c * b));
            double elevation = A - Math.PI/2.0;

            double radarLat = (41.0 + 18.0 / 60.0 + 2.5284 / 3600.0) * Math.PI / 180.0; //[rad]
            double radarLong = (2.0 + 06.0 / 60.0 + 7.4095 / 3600.0) * Math.PI / 180.0; //[rad]
            CoordinatesWGS84 coordinatesWGS = new CoordinatesWGS84(radarLat, radarLong, radarAltitude);
            CoordinatesPolar coordinatesPolar = new CoordinatesPolar(rho, theta, elevation);
            GeoUtils geoUtils = new GeoUtils();
            CoordinatesXYZ coordinatesXYZ = GeoUtils.change_radar_spherical2radar_cartesian(coordinatesPolar);
            CoordinatesXYZ coordinatesXYZ2 = geoUtils.change_radar_cartesian2geocentric(coordinatesWGS, coordinatesXYZ);
            CoordinatesWGS84 coordinatesWGS2 = geoUtils.change_geocentric2geodesic(coordinatesXYZ2);

            this.LATITUDE = coordinatesWGS2.Lat * 180.0 / Math.PI;
            this.LONGITUDE = coordinatesWGS2.Lon * 180.0 / Math.PI;
            this.HEIGHT = coordinatesWGS2.Height;

            this.INDEX = index;
        }
    }
}