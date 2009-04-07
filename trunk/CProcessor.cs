using System;
using System.Collections.Generic;
using System.Text;

namespace sharpGB
{
    public class CProcessor
    {

        // The registers of the CPU (Z80-wannabe)
        public byte A,B,C,D,E,F,H,L;
        public ushort SP, PC, HL;

        // Some instructions determine the behaviour of the CPU
        public bool IME;                    // Turn on/off interrupt handling
        public int DIsignaled, EIsignaled;  // Instructions DI and EI set these
        public bool CPUHalt, CPUStop;       // Set by instructions as well

        // The four flag register entries
        public uint ZeroFlag, SubtractFlag, HalfCarryFlag, CarryFlag;

        public void SetFlags(int Z, int S, int H, int C)
        {
            // Normalize to 0 or 1
            if (Z != 0) Z = 1;
            if (S != 0) S = 1;
            if (H != 0) H = 1;
            if (C != 0) C = 1;
            ZeroFlag = (uint) Z;
            SubtractFlag = (uint)S;
            HalfCarryFlag = (uint)H;
            CarryFlag = (uint)C;
        }

        public CProcessor()
        {

        }

    }
}
