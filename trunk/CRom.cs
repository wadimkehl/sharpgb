using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace sharpGB
{
   
    /* Every ROM cartridge has embedded information (called header) about many things
     * They will be stored in this struct, filled by CRom
     * */
    public struct RomHeader
    {
        public byte[] BeginCodeExecutionPoint;     // Machine code that jumps to rom start              found at bytes 0100-0103
        public byte[] GameTitle;                   // Game title in upper case ASCII                    found at bytes 0134-0142
        public byte ColorGB;                       // 0x80 if gameboy color game, else another value    found at byte 0143
        public byte LicenseeHigh;                  // High nibble of licensee number                    found at byte 0144   
        public byte LicenseeLow;                   // Low nibble of licensee number                     found at byte 0145 
        public byte GBFunctionsIndicator;          // 00 = Gameboy, 03 = Super Gameboy                  found at byte 0146
        public byte CartridgeType;                 // Many values, see GBCPUman                         found at byte 0147
        public byte ROMSize;                       // See above                                         found at byte 0148
        public byte RAMSize;                       // See above                                         found at byte 0149
        public byte DestinationCode;               // 0 = Japanese, 1 = Other                           found at byte 014A
        public byte LicenseeCode;                  // See GBCPUman                                      found at byte 014B
        public byte ROMVersionNumber;              // Usually 00                                        found at byte 014C
        public byte ComplementCheck;               // Needed for the complement check at start up       found at byte 014D
        public byte[] Checksum;                    // Checksum of the cartridge                         found at bytes 014E-014F
    }



     /* Represents a ROM file 
     *  Provides methods for loading ROM's and holds all relevant rom data
     * */
    public class CRom
    {


        public RomHeader Header;    // The ROM header
        public long RomSize;        // Size of the ROM cartridge in bytes
        public byte[] Data;         // The loaded ROM data
        public bool RomLoaded;      // True if a ROM has been loaded
        public bool LogoOK;         // True if check with internal logo is ok
        public bool CheckSumOK;     // True if checksum is ok
        public bool ComplementOK;   // True if complement check succeeded


        // Read a ROM file and fill all data
        public bool LoadFromFile(string FileName)
        {

            try
            {
                // Open file and a binary reader
                FileStream fs = File.OpenRead(FileName);
                BinaryReader br = new BinaryReader(fs);

                // Allocate the right amount of bytes and save rom size
                this.RomSize = br.BaseStream.Length;
                this.Data = new byte[this.RomSize];
                this.Header = new RomHeader();
                this.Header.BeginCodeExecutionPoint = new byte[4];
                this.Header.GameTitle = new byte[15];
                this.Header.Checksum = new byte[2];
                
                // read contents of the file
                int bytecounter;
                for (bytecounter = 0; bytecounter < this.RomSize; bytecounter++ )
                {
                    // read next byte and save it in the data field
                    byte readbyte = br.ReadByte();
                    this.Data[bytecounter] = readbyte;

                    // fill the ROM header when passing the right byte
                    switch (bytecounter)
                    {
                        case 0x0100:
                            this.Header.BeginCodeExecutionPoint[0] = readbyte; break;
                        case 0x0101:
                            this.Header.BeginCodeExecutionPoint[1] = readbyte; break;
                        case 0x0102:
                            this.Header.BeginCodeExecutionPoint[2] = readbyte; break;
                        case 0x0103:
                            this.Header.BeginCodeExecutionPoint[3] = readbyte; break;

                        case 0x0134:
                            this.Header.GameTitle[0] = readbyte; break;
                        case 0x0135:
                            this.Header.GameTitle[1] = readbyte; break;
                        case 0x0136:
                            this.Header.GameTitle[2] = readbyte; break;
                        case 0x0137:
                            this.Header.GameTitle[3] = readbyte; break;
                        case 0x0138:
                            this.Header.GameTitle[4] = readbyte; break;
                        case 0x0139:
                            this.Header.GameTitle[5] = readbyte; break;
                        case 0x013A:
                            this.Header.GameTitle[6] = readbyte; break;
                        case 0x013B:
                            this.Header.GameTitle[7] = readbyte; break;
                        case 0x013C:
                            this.Header.GameTitle[8] = readbyte; break;
                        case 0x013D:
                            this.Header.GameTitle[9] = readbyte; break;
                        case 0x013E:
                            this.Header.GameTitle[10] = readbyte; break;
                        case 0x013F:
                            this.Header.GameTitle[11] = readbyte; break;
                        case 0x0140:
                            this.Header.GameTitle[12] = readbyte; break;
                        case 0x0141:
                            this.Header.GameTitle[13] = readbyte; break;
                        case 0x0142:
                            this.Header.GameTitle[14] = readbyte; break;

                        case 0x0143:
                            this.Header.ColorGB = readbyte; break;
                        case 0x0144:
                            this.Header.LicenseeHigh = readbyte; break;
                        case 0x0145:
                            this.Header.LicenseeLow = readbyte; break;
                        case 0x0146:
                            this.Header.GBFunctionsIndicator = readbyte; break;
                        case 0x0147:
                            this.Header.CartridgeType = readbyte; break;
                        case 0x0148:
                            this.Header.ROMSize = readbyte; break;
                        case 0x0149:
                            this.Header.RAMSize = readbyte; break;
                        case 0x014A:
                            this.Header.DestinationCode = readbyte; break;
                        case 0x014B:
                            this.Header.LicenseeCode = readbyte; break;
                        case 0x014C:
                            this.Header.ROMVersionNumber = readbyte; break;
                        case 0x014D:
                            this.Header.ComplementCheck = readbyte; break;

                        case 0x014E:
                            this.Header.Checksum[0] = readbyte; break;
                        case 0x014F:
                            this.Header.Checksum[1] = readbyte; break;
                    }
                }

                this.RomLoaded = true;
                br.Close();
                return true;
            }
            catch
            {
                this.RomLoaded = false;
                return false;
            }
        }

        // Return a string containing the hex representation of the ROM (slow, testing purpose only)
        public string GetHexString()
        {
            StringBuilder ret = new StringBuilder();
            ret.EnsureCapacity( (int) this.RomSize);
            for (int i = 0; i < this.RomSize; i++)
                ret.AppendLine(i.ToString("X4") + ": " + this.Data[i].ToString("X2"));
            return ret.ToString();
        }



    }


   

}
