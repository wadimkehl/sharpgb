using System;
using System.Collections.Generic;
using System.Text;



/* Copied from GBCPUman and enhanced with http://www.enliten.force9.co.uk/gameboy/memory.htm
 * Memory mapping of the game boy
 * 
Interrupt Enable Register
------------------------------------ FFFF
127B Internal RAM
------------------------------------ FF80
52B  Empty but unusable for I/O
------------------------------------ FF4C
76B  I/O ports (see enum struct below)
------------------------------------ FF00
96B  Empty but unusable for I/O
------------------------------------ FEA0
120B Object Attrib Memory (OAM)
------------------------------------ FE00
Echo of 8kB Internal RAM 
------------------------------------ E000--|
Ram Banks 1-7                              |
------------------------------------ D000  | = 8kB Internal RAM
Ram Bank 0 fixed                           |                
------------------------------------ C000--|
8kB switchable Game RAM bank 
------------------------------------ A000--|
BG Data 2, TileIndices/Attributes          |
------------------------------------ 9C00  |
BG Data 1, TileIndices/Attributes          | = 8kB Video RAM 
------------------------------------ 9800  |
Bank 0 and 1 Character Data                |                        
------------------------------------ 8000 -|
16kB switchable ROM bank                   |
------------------------------------ 4000  | = 32kB Cartridge
16kB ROM bank #0                           |
------------------------------------ 0000 --
    
   0000        Restart $00 Address
   0008        Restart $08 Address 
   0010        Restart $10 Address 
   0018        Restart $18 Address 
   0020        Restart $20 Address 
   0028        Restart $28 Address 
   0030        Restart $30 Address 
   0038        Restart $38 Address 
   0040        Vertical Blank Interrupt Start Address
   0048        LCDC Status Interrupt Start Address
   0050        Timer Overflow Interrupt Start Address
   0058        Serial Transfer Completion Interrupt Start Address
   0060        High-to-Low of P10-P13 Interrupt Start Address
 * 
 * 
*/

namespace sharpGB
{
    public class CMemory
    {
        public byte[] Data;             // The gameboy memory space
        public int[]  AddressToIndex;   // This structure is used to translate an address to an operation index
        public byte CartridgeType;      // The cartridge type (affects memory usage and write/read access)


        // Hardware registers mapped to gameboy memory (FF40-FF4C).
        // taken from http://www.enliten.force9.co.uk/gameboy/hardware.htm
        public enum HardwareRegisters
        {
            // Name, Address, Usage
            P1  =  0xFF00,    // Joypad information
            SB  =  0xFF01,    // Serial Transfer Data
            SC  =  0xFF02,    // Serial I/O Control
            DIV =  0xFF04,    // Timer Divider
            TIMA = 0xFF05,    // Timer Counter
            TMA =  0xFF06,    // Timer Modulo
            TAC =  0xFF07,    // Timer Control
            IF =   0xFF0F,    // Interrupt Flag

            NR10 = 0xFF10,    // Sound Mode 1, Sweep
            NR11 = 0xFF11,    // Sound Mode 1, Sound length/Wave pattern duty
            NR12 = 0xFF12,    // Sound Mode 1, Envelope
            NR13 = 0xFF13,    // Sound Mode 1, Frequency Low
            NR14 = 0xFF14,    // Sound Mode 1, Frequency High


            NR21 = 0xFF16,    // Sound Mode 2, Sound length/Wave pattern duty
            NR22 = 0xFF17,    // Sound Mode 2, Envelope
            NR23 = 0xFF18,    // Sound Mode 2, Frequency Low
            NR24 = 0xFF19,    // Sound Mode 2, Frequency High
            
            NR30 = 0xFF1A,    // Sound Mode 3, Sound on/off
            NR31 = 0xFF1B,    // Sound Mode 3, Sound length
            NR32 = 0xFF1C,    // Sound Mode 3, Select output level
            NR33 = 0xFF1D,    // Sound Mode 3, Frequency Low
            NR34 = 0xFF1E,    // Sound Mode 3, Frequency High

            NR41 = 0xFF20,    // Sound Mode 4, Sound length
            NR42 = 0xFF21,    // Sound Mode 4, Envelope
            NR43 = 0xFF22,    // Sound Mode 4, Polynomial counter
            NR44 = 0xFF23,    // Sound Mode 4, Counter/Consecutive, Initial

            NR50 = 0xFF24,    // Sound Mode 5, Channel Control, On/Off, Volume
            NR51 = 0xFF25,    // Sound Mode 5, Select of sound output terminal
            NR52 = 0xFF26,    // Sound Mode 5, Sound On/Off

            WAV00 = 0xFF30,    // Wave pattern RAM  16bytes
            WAV01 = 0xFF31,
            WAV02 = 0xFF32,
            WAV03 = 0xFF33,
            WAV04 = 0xFF34,
            WAV05 = 0xFF35,
            WAV06 = 0xFF36,
            WAV07 = 0xFF37,
            WAV08 = 0xFF38,
            WAV09 = 0xFF39,
            WAV10 = 0xFF3A,
            WAV11 = 0xFF3B,
            WAV12 = 0xFF3C,
            WAV13 = 0xFF3D,
            WAV14 = 0xFF3E,
            WAV15 = 0xFF3F,
        
            LCDC = 0xFF40,    // LCD Control
            STAT = 0xFF41,    // LCD Status
            SCY  = 0xFF42,    // Scroll Screen Y
            SCX =  0xFF43,    // Scroll Screen X
            LY  = 0xFF44,     // LCDC Y-Coord
            LYC = 0xFF45,     // LY Compare
            DMA = 0xFF46,     // DMA Transfer
            BGP = 0xFF47,     // Background Palette Data
            OBP0 = 0xFF48,    // Object Palette 0 Data
            OBP1 = 0xFF49,    // Object Palette 1 Data
            WY   = 0xFF4A,    // Window Y Position
            WX   = 0xFF4B,    // Window X Position

            // Some GBC only registers, ignored here (at least now...)

            IE  = 0xFFFF      // Interrupt enable 
        }

        public CMemory()
        {
            this.Data = new byte[65536];  // 2^16 address space
            this.AddressToIndex = new int[65536];  // 2^16 address space
        }

        // Returns byte at address, considering addressing mode 
        public byte readByte(int address)
        {
            return this.Data[address]; 
        }

        // Writes byte at address, considering addressing mode 
        unsafe public void writeByte(int address, byte data)
        {
           
            // emulate echo of 8kb internal ram (writes to E000-F300 appear at C000-DE00 and vice versa)
            if ((address >= 0xE000) && (address <= 0xF300))
                this.Data[0xC000 + (address - 0xE000)] = data;
            else if ((address >= 0xC000) && (address <= 0xD300))
                this.Data[0xE000 + (address - 0xC000)] = data;

            this.Data[address] = data;
        }

        // Returns word at address, considering addressing mode 
        unsafe public ushort readWord(int address)
        {
            // build a word out of address and address+1 (little endian)
            return (ushort)(this.Data[address+1] << 8 | this.Data[address]);
        }

        // Recalculates given address, considering addressing mode
        public int getEA(int address)
        {
            return address;
        }

    }
}
