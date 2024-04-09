using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.IO;

namespace Project2_Code
{
    public class CAT48
    {
        public BitArray FSPEC;

        //Data Item I048/010, Data Source Identifier
        public int SAC;
        public int SIC;

        //Data Item I048/020, Target Report Descriptor
        public string TYP_020;
        public string SIM_020;
        public string RDP_020;
        public string SPI_020;
        public string RAB_020;
        public string TST_020;
        public string ERR_020;
        public string XPP_020;
        public string ME_020;
        public string MI_020;
        public string FOEFRI_020;
        public string ADSB_020;
        public string SCN_020;
        public string PAI_020;

        //Data Item I048/030, Warning/Error Conditions and Target Classification
        //No decodification needed

        //Data Item I048/040, Measured Position in Polar Co-ordinates
        public string RHO;
        public string THETA;

        //Data Item I048/042, Calculated Position in Cartesian Co-ordinates
        public string LAT;
        public string LONG;

        //Data Item I048/050, Mode-2 Code in Octal Representation
        //No decodification needed

        //Data Item I048/055, Mode-1 Code in Octal Representation
        //No decodification needed

        //Data Item I048/060, Mode-2 Code Confidence Indicator
        //No decodification needed

        //Data Item I048/065, Mode-1 Code Confidence Indicator
        //No decodification needed

        //Data Item I048/070, Mode-3/A Code in Octal Representation
        public string V_070;
        public string G_070;
        public string L_070;
        public string MODE3AREPLY;

        //Data Item I048/080, Mode-3/A Code Confidence Indicator
        //No decodification needed

        //Data Item I048/090, Flight Level in Binary Representation
        public string V_090;
        public string G_090;
        public string FL;

        //Data Item I048/100, Mode-C Code and Code Confidence Indicator
        //No decodification needed

        //Data Item I048/110, Height Measured by a 3D Radar
        public string HEIGH3D;

        //Data Item I048/120, Radial Doppler Speed
        //No decodification needed

        //Data Item I048/130, Radar Plot Characteristics
        public string SRL_130;
        public string SRR_130;
        public string SAM_130;
        public string PRL_130;
        public string PAM_130;
        public string RPD_130;
        public string APD_130;

        //Data Item I048/140, Time of Day
        public string TIME;

        //Data Item I048/161, Track Number
        public string TN;

        //Data Item I048/170, Track Status
        public string CNF_170;
        public string RAD_170;
        public string DOU_170;
        public string MAH_170;
        public string CDM_170;
        public string TRE_170;
        public string GHO_170;
        public string SUP_170;
        public string TCC_170;

        //Data Item I048/200, Calculated Track Velocity in Polar Co-ordinates
        public string GS;
        public string HEADING;

        //Data Item I048/210, Track Quality
        //No decodification needed

        //Data Item I048/220, Aircraft Address
        public string ADDRESS;

        //Data Item I048/230, Communications/ACAS Capability and Flight Status
        public string COM_230;
        public string STAT_230;
        public string SI_230;
        public string MSSC_230;
        public string ARC_230;
        public string AIC_230;
        public string B1A_230;
        public string B1B_230;

        //Data Item I048/240, Aircraft Identification
        public string IDENTIFICATION;

        //Data Item I048/250, BDS Register Data
        public string REP;
        public string BDSDATA;
        public string BDS1;
        public string BDS2;

        //Data Item I048/260, ACAS Resolution Advisory Report
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
        private int ParseFSPEC(Stream data)
        {
            int length = 0;
            byte[] FSPEC = new byte[] {0, 0, 0, 0};
            for (int i = 0; i < FSPEC.Length; i++)
            {
                FSPEC[i] = data[i];
                if ((data[i] & 1) == 0)
                {
                    length = i + 1;
                    break;
                }            
            }
            this.FSPEC = new BitArray(FSPEC);
            return length;
        }

        private void ParseI048_010(Stream data)
        {
            this.SAC = (int)data.ReadByte();
            this.SIC = (int)data.ReadByte();
        }

        //CONSTRUCTOR
        public CAT48(Stream data)
        {
            int pos = 0;
            pos += ParseFSPEC(data[pos..(pos+4)]);

            //Check
          
            if (this.FSPEC[(int)UAP.I048_010])
            {
                ParseI048_010
                
            }
        
        }










    }
}
