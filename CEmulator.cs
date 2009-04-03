using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace sharpGB
{
    public class CEmulator
    {

        public byte[] NintendoLogo = 
        {
        0xCE, 0xED, 0x66, 0x66, 0xCC, 0x0D, 0x00, 0x0B, 0x03, 0x73, 0x00, 
        0x83, 0x00, 0x0C, 0x00, 0x0D, 0x00, 0x08, 0x11, 0x1F, 0x88, 0x89, 
        0x00, 0x0E, 0xDC, 0xCC, 0x6E, 0xE6, 0xDD, 0xDD, 0xD9, 0x99, 0xBB, 
        0xBB, 0x67, 0x63, 0x6E, 0x0E, 0xEC, 0xCC, 0xDD, 0xDC, 0x99, 0x9F, 
        0xBB, 0xB9, 0x33, 0x3E
        };

        public enum IRQType
        {
            VBLANK,
            LCDC,
            TIMER,
            SERIALIO,
            HIGHTOLOW
        }

        // All components that make up a gameboy emulator
        public CRom ROM;
        public CDisassembler Disassembler;
        public CMemory Memory;
        public CProcessor Processor;
        public CVideo Video;

        public int ClockCyclesElapsed;          // Elapsed cpu clock cycles
        public int MachineCyclesElapsed;        // Elapsed gameboy clock cycles
        public int LYCounter;                   // Will signal when another display line is to draw
        public int LastInstructionClockCycle;   // Clock cycle duration of the last executed instruction
        public bool EmulationRunning;           // Flag indicating emulator running
        public bool UnknownOPcode;              // True if OPcode not implemented
        public bool UnknownOperand;             // True if OPcode has unexpected operand
        public bool BreakPointReached;          // True if PC stands at a breakpoint
        public byte CurrentOPcode;              // current OPcode to execute
        public List<int> BreakPoints;           // List of addresses the emulator is to stop


        public CEmulator()
        {
            this.ROM = new CRom();
            this.Memory = new CMemory();
            this.Disassembler = new CDisassembler();
            this.Processor = new CProcessor();
            this.Video = new CVideo(ref this.Memory);
            this.EmulationRunning = true;
            this.BreakPoints = new List<int>();

            Reset();
        }

        // Emulate the insertion of a cartridge
        public bool LoadROMFromFile(string FileName)
        {
            // Start emulation anew
            Reset();

            // Load the ROM
            if(!ROM.LoadFromFile(FileName)) return false;
           
            // Determine whether the ROM will be loaded thoroughly (if 32kb ROM) or partially (i.e. Bank 0)
            // into the gameboy address space, beginning at 0x0000
            int DumpSize = (ROM.Header.CartridgeType == 0) ? 0x8000 : 0x4000;
            for (int i = 0; i < DumpSize; i++)
                Memory.Data[i] = ROM.Data[i];

            // Pass the catridge information to the memory unit (affects addressing behaviour)
            Memory.CartridgeType = ROM.Header.CartridgeType;

            // The gameboy has an internal piece of code that loads and presents the Nintendo logo from the cartidge
            // After presenting, the gameboy runs a comparison of the logo with internal rom data
            ROM.LogoOK = true;
            for (int i = 0x104; i < 0x134; i++)
                if (ROM.Data[i] != NintendoLogo[i-0x104])
                    ROM.LogoOK = false;
     

            // Now we run a complement check the gameboy and gameboypocket does afterwards
            byte complement = 25;
            for (int i = 0x134; i <= 0x14D; i++)
                complement += ROM.Data[i];
            if ( complement == 0)
                ROM.ComplementOK = true;
            else ROM.ComplementOK = false;

            // And we do the checksum for the rom against the checksum bytes, although the gameboy itself ignores it
            long checksum = 0;
            for (int i = 0; i < ROM.RomSize; i++)
                if ((i != 0x014E) || (i != 0x014F)) // ignore the two checksum bytes, sum up all the other bytes 
                    checksum += ROM.Data[i];

            if ((((byte)(checksum >> 8)) == ROM.Header.Checksum[0]) &&  // compare lower two bytes against checksum bytes
                 (((byte)(checksum)) == ROM.Header.Checksum[1]))
                 ROM.CheckSumOK = true;
            else ROM.CheckSumOK = false;

            // Reset the screen
            Video.VBlank();

            return true;
         }

        // Resets the complete emulation
        public void Reset()
        {
            // Standard values
            CurrentOPcode = 0x00;
            ClockCyclesElapsed = 0;
            LYCounter = 0;
            MachineCyclesElapsed = 0;
            BreakPoints.Clear();
            BreakPointReached = false;
            this.EmulationRunning = true;

            Memory.Data[(int)CMemory.HardwareRegisters.TIMA] = 0x00;
            Memory.Data[(int)CMemory.HardwareRegisters.TMA] = 0x00;
            Memory.Data[(int)CMemory.HardwareRegisters.TAC] = 0x00;

            Memory.Data[(int)CMemory.HardwareRegisters.NR10] = 0x80;
            Memory.Data[(int)CMemory.HardwareRegisters.NR11] = 0xBF;
            Memory.Data[(int)CMemory.HardwareRegisters.NR12] = 0xF3;
            Memory.Data[(int)CMemory.HardwareRegisters.NR14] = 0xBF;
            // Some sound register missing here...

            Memory.Data[(int)CMemory.HardwareRegisters.LCDC] = 0x91;
            Memory.Data[(int)CMemory.HardwareRegisters.SCY] = 0x00;
            Memory.Data[(int)CMemory.HardwareRegisters.SCX] = 0x00;
            Memory.Data[(int)CMemory.HardwareRegisters.LYC] = 0x00;
            Memory.Data[(int)CMemory.HardwareRegisters.BGP] = 0xFC;
            Memory.Data[(int)CMemory.HardwareRegisters.OBP0] = 0xFF;
            Memory.Data[(int)CMemory.HardwareRegisters.OBP1] = 0xFF;
            Memory.Data[(int)CMemory.HardwareRegisters.WY] = 0x00;
            Memory.Data[(int)CMemory.HardwareRegisters.WX] = 0x00;
            Memory.Data[(int)CMemory.HardwareRegisters.IF] = 0x00;
            Memory.Data[(int)CMemory.HardwareRegisters.IE] = 0x00;


            Processor.A = 0x01;
            Processor.B = 0x00;
            Processor.C = 0x13;
            Processor.D = 0x00;
            Processor.E = 0xD8;
            Processor.H = 0x01;
            Processor.L = 0x4D;
            Processor.PC = 0x0100;   
            Processor.SP = 0xFFFE;

            Processor.ZeroFlag = 1;
            Processor.SubtractFlag = 0;
            Processor.HalfCarryFlag = 1;
            Processor.CarryFlag = 1;

            // Assemble the F register
            Processor.F = (byte)((Processor.ZeroFlag << 7) | (Processor.SubtractFlag << 6) |
                          (Processor.HalfCarryFlag << 5) | (Processor.CarryFlag << 4));
        }

        // Add breakpoint for debugging
        public void AddBreakPoint(int address)
        {
            if (!BreakPoints.Contains(address))
                BreakPoints.Add(address);
        }

        // Remove breakpoint
        public void RemoveBreakPoint(int address)
        {
            if (BreakPoints.Contains(address))
                BreakPoints.Remove(address);
        }

        // Emulates a complete machine cycle
        public void EmulateNextStep()
        {
            EmulationRunning = true;

            // Check whether last instruction signaled a change in interrupt handling
            // DI and EI take effect AFTER the next instruction following them
            if (Processor.DIsignaled == 1)Processor.DIsignaled++;
            else if (Processor.DIsignaled == 2)
            {
                Processor.DIsignaled = 0;
                Processor.IME = false;
            }
            if (Processor.EIsignaled == 1)Processor.EIsignaled++;
            else if (Processor.EIsignaled == 2)
            {
                Processor.EIsignaled = 0;
                Processor.IME = false;
            }

            // Fetch current OPcode
            CurrentOPcode = Memory.Data[Processor.PC];

            // Check if the PC position is a breakpoint
            if (BreakPoints.Contains(Processor.PC))
            {
                BreakPointReached = true;
                EmulationRunning = false;  
                return;
            }

            // Else run the operation and save clock cycle of OPcode
            LastInstructionClockCycle = DecodeAndExecute(CurrentOPcode);

            // Check if there is a reason to halt the emulation
            if (UnknownOPcode || UnknownOperand || !EmulationRunning)
            {
                EmulationRunning = false;   // Stop emulation
                return;
            }

            // Increment cycle counters
            ClockCyclesElapsed += LastInstructionClockCycle;
            LYCounter += LastInstructionClockCycle;
            MachineCyclesElapsed++;

            
            // VBLANK IRQ - Occurs only when display is enabled
            // Every 456(144) clock(machine) cycles, one of the 144 screen lines is drawn
            // After that there is a VBlank-IRQ with a period of 10 scanlines.
            if (LYCounter >= 456 && (Memory.Data[(int)CMemory.HardwareRegisters.LCDC] & 0x80)>0)
            {
                LYCounter = 0;

                // Increase the current line the gameboy display driver is to draw
                Memory.Data[(int)CMemory.HardwareRegisters.LY]++;
                Video.DrawLine();

                // Check if VBLANK interrupt arises or correct LY if needed
                if (Memory.Data[(int)CMemory.HardwareRegisters.LY] == 144)
                {
                    RaiseIRQ(IRQType.VBLANK);
                    Video.VBlank();
                }
                else if (Memory.Data[(int)CMemory.HardwareRegisters.LY] > 153)
                    Memory.Data[(int)CMemory.HardwareRegisters.LY] = 0;

                // Check for LYC coincidence
                if (((Memory.Data[(int)CMemory.HardwareRegisters.STAT] & 0x40) > 0) && // enabled?
                    (Memory.Data[(int)CMemory.HardwareRegisters.LY] ==
                    Memory.Data[(int)CMemory.HardwareRegisters.LYC]))
                    Memory.Data[(int)CMemory.HardwareRegisters.STAT] |= 0x04;   // bit on
                else Memory.Data[(int)CMemory.HardwareRegisters.STAT] &= 0xFB;  // bit off
                     
            }

            // Timer IRQ

            // Serial Transfer IRQ

            // High-to-Low P10-P13 (key input handling)

            // Check whether interrupts are to be handled
            if (Processor.IME) DoIdleIRQs();
        }

        // Emulate until encountered a problem or aborted
        public void EmulateFully()
        {
            while (EmulationRunning)
                EmulateNextStep();
        }

        // Emulate n steps
        public void EmulateSteps(int n)
        {
            for (int i = 0; i < n && EmulationRunning; i++ )
                EmulateNextStep();
        }

        // Shows a window with more information on emulator halt
        public void ShowError()
        {
            if (EmulationRunning) return;

            if (BreakPointReached)
            {
                System.Windows.Forms.MessageBox.Show("Reached breakpoint at 0x" + Processor.PC.ToString("X4"));
                BreakPointReached = false;
            }
            else if (UnknownOPcode)
                System.Windows.Forms.MessageBox.Show("Unknown OPcode: " + Memory.Data[Processor.PC].ToString("X2"));
            else if (UnknownOperand)
                System.Windows.Forms.MessageBox.Show("Unknown operand: " + Memory.Data[Processor.PC+1].ToString("X2"));

            EmulationRunning = true;
  

        }


        // This (humongous) method decodes and executes the given opcode.
        int DecodeAndExecute(byte OPcode)
        {
            
            // return value, represents the clock cycles needed for opcode
            int cycles=0;

            // Assemble the F register
            Processor.F = (byte)((Processor.ZeroFlag << 7) | (Processor.SubtractFlag << 6) |
                          (Processor.HalfCarryFlag << 5) | (Processor.CarryFlag << 4));

            // often used variables
            Processor.HL = (ushort) (Processor.H << 8 | Processor.L);
            byte value, value2;
            ushort word;

            // In here, PC incrementing/changing must be taken care of as well
            switch (OPcode)
            {

                #region   /* Load instructions */


                // immediate loads
                case 0x3E:  // A <- immediate  
                    Processor.A = Memory.Data[Processor.PC + 1];
                    Processor.PC += 2;
                    cycles = 8;
                    break;
                case 0x06:  // B <- immediate  
                    Processor.B = Memory.Data[Processor.PC + 1];
                    Processor.PC += 2;
                    cycles = 8;
                    break;
                case 0x0E:  // C <- immediate  
                    Processor.C = Memory.Data[Processor.PC + 1];
                    Processor.PC += 2;
                    cycles = 8;
                    break;
                case 0x16:  // D <- immediate  
                    Processor.D = Memory.Data[Processor.PC + 1];
                    Processor.PC += 2;
                    cycles = 8;
                    break;
                case 0x1E:  // E <- immediate  
                    Processor.E = Memory.Data[Processor.PC + 1];
                    Processor.PC += 2;
                    cycles = 8;
                    break;
                case 0x26:  // H <- immediate  
                    Processor.H = Memory.Data[Processor.PC + 1];
                    Processor.PC += 2;
                    cycles = 8;
                    break;
                case 0x2E:  // L <- immediate  
                    Processor.L = Memory.Data[Processor.PC + 1];
                    Processor.PC += 2;
                    cycles = 8;
                    break;
                case 0x01:  // BC <- immediate  
                    Processor.C = Memory.Data[Processor.PC + 1];
                    Processor.B = Memory.Data[Processor.PC + 2];
                    Processor.PC += 3;
                    cycles = 12;
                    break;
                case 0x11:  // DE <- immediate  
                    Processor.E = Memory.Data[Processor.PC + 1];
                    Processor.D = Memory.Data[Processor.PC + 2];
                    Processor.PC += 3;
                    cycles = 12;
                    break;
                case 0x21:  // HL <- immediate  
                    Processor.L = Memory.Data[Processor.PC + 1];
                    Processor.H = Memory.Data[Processor.PC + 2];
                    Processor.PC += 3;
                    cycles = 12;
                    break;
                case 0x31:  // SP <- immediate
                    Processor.SP = (ushort)(Memory.Data[Processor.PC + 2] << 8 | Memory.Data[Processor.PC + 1]);
                    Processor.PC += 3;
                    cycles = 12;
                    break;
                case 0x36:  // (HL) <- immediate  
                    Memory.writeByte(Processor.HL, Memory.Data[Processor.PC + 1]);
                    Processor.PC += 2;
                    cycles = 12;
                    break;



                // memory to register transfer
                case 0xF2:    // A <- (0xFF00 + C)
                    word = (ushort)(0xFF00 + Processor.C);
                    Processor.A = Memory.readByte(word);
                    Processor.PC++;
                    cycles = 8;
                    break;
                case 0x0A:    // A <- (BC)
                    word = (ushort)(Processor.B << 8 | Processor.C);
                    Processor.A = Memory.readByte(word);
                    Processor.PC++;
                    cycles = 8;
                    break;
                case 0x1A:    // A <- (DE)
                    word = (ushort)(Processor.D << 8 | Processor.E);
                    Processor.A = Memory.readByte(word);
                    Processor.PC++;
                    cycles = 8;
                    break;
                case 0x7E:  // A <- (HL) 
                    Processor.A = Memory.readByte(Processor.HL);
                    Processor.PC++;
                    cycles = 8;
                    break;
                case 0x46:  // B <- (HL) 
                    Processor.B = Memory.readByte(Processor.HL);
                    Processor.PC++;
                    cycles = 8;
                    break;
                case 0x4E:  // C <- (HL) 
                    Processor.C = Memory.readByte(Processor.HL);
                    Processor.PC++;
                    cycles = 8;
                    break;
                case 0x56:  // D <- (HL) 
                    Processor.D = Memory.readByte(Processor.HL);
                    Processor.PC++;
                    cycles = 8;
                    break;
                case 0x5E:  // E <- (HL) 
                    Processor.E = Memory.readByte(Processor.HL);
                    Processor.PC++;
                    cycles = 8;
                    break;
                case 0x66:  // H <- (HL) 
                    Processor.H = Memory.readByte(Processor.HL);
                    Processor.PC++;
                    cycles = 8;
                    break;
                case 0x6E:  // L <- (HL) 
                    Processor.L = Memory.readByte(Processor.HL);
                    Processor.PC++;
                    cycles = 8;
                    break;
                case 0x2A:  // A <- (HL), HL++      /* FLAGS??? */
                    Processor.A = Memory.readByte(Processor.HL);
                    Processor.HL++;
                    Processor.H = (byte)(Processor.HL >> 8);
                    Processor.L = (byte)Processor.HL;
                    Processor.PC++;
                    cycles = 8;
                    break;
                case 0xFA:  // A <- (nn immediate) 
                    word = (ushort)(Memory.Data[Processor.PC + 2] << 8 | Memory.Data[Processor.PC + 1]);
                    Processor.A = Memory.readByte(word);
                    Processor.PC += 3;
                    cycles = 16;
                    break;
                case 0xF0:  // A <- (0xFF00+ n immediate) /* SIGNED/UNSIGNED byte? */
                    word = (ushort)(0xFF00 + Memory.Data[Processor.PC + 1]);
                    Processor.A = Memory.readByte(word);
                    Processor.PC += 2;
                    cycles = 12;
                    break;

                // register to memory transfer
                case 0xE2:    // (0xFF00 + C) <- A
                    word = (ushort)(0xFF00 + Processor.C);
                    Memory.writeByte(word, Processor.A);
                    Processor.PC++;
                    cycles = 8;
                    break;
                case 0x02:  // (BC) <- A 
                    word = (ushort)(Processor.B << 8 | Processor.C);
                    Memory.writeByte(word, Processor.A);
                    Processor.PC++;
                    cycles = 8;
                    break;
                case 0x12:  // (DE) <- A 
                    word = (ushort)(Processor.D << 8 | Processor.E);
                    Memory.writeByte(word, Processor.A);
                    Processor.PC++;
                    cycles = 8;
                    break;
                case 0x77:  // (HL) <- A 
                    Memory.writeByte(Processor.HL, Processor.A);
                    Processor.PC++;
                    cycles = 8;
                    break;
                case 0x70:  // (HL) <- B 
                    Memory.writeByte(Processor.HL, Processor.B);
                    Processor.PC++;
                    cycles = 8;
                    break;
                case 0x71:  // (HL) <- C 
                    Memory.writeByte(Processor.HL, Processor.C);
                    Processor.PC++;
                    cycles = 8;
                    break;
                case 0x72:  // (HL) <- D 
                    Memory.writeByte(Processor.HL, Processor.D);
                    Processor.PC++;
                    cycles = 8;
                    break;
                case 0x73:  // (HL) <- E 
                    Memory.writeByte(Processor.HL, Processor.E);
                    Processor.PC++;
                    cycles = 8;
                    break;
                case 0x74:  // (HL) <- H 
                    Memory.writeByte(Processor.HL, Processor.H);
                    Processor.PC++;
                    cycles = 8;
                    break;
                case 0x75:  // (HL) <- L 
                    Memory.writeByte(Processor.HL, Processor.L);
                    Processor.PC++;
                    cycles = 8;
                    break;
                case 0xEA:  // (nn) <- A 
                    word = (ushort)(Memory.Data[Processor.PC + 2] << 8 | Memory.Data[Processor.PC + 1]);
                    Memory.writeByte(word, Processor.A);
                    Processor.PC += 3;
                    cycles = 16;
                    break;
                case 0xE0:  // (0xFF00+ n immediate) <- A  /* SIGNED/UNSIGNED byte? */
                    word = (ushort)(0xFF00 + Memory.Data[Processor.PC + 1]);
                    Memory.writeByte(word, Processor.A);
                    Processor.PC += 2;
                    cycles = 12;
                    break;
                case 0x32:  // (HL) <- A, HL--      /* FLAGS??? */
                    Memory.writeByte(Processor.HL, Processor.A);
                    Processor.HL--;
                    Processor.H = (byte)(Processor.HL >> 8);
                    Processor.L = (byte)Processor.HL;
                    Processor.PC++;
                    cycles = 8;
                    break;
                case 0x22:  // (HL) <- A, HL++      /* FLAGS??? */
                    Memory.writeByte(Processor.HL, Processor.A);
                    Processor.HL++;
                    Processor.H = (byte)(Processor.HL >> 8);
                    Processor.L = (byte)Processor.HL;
                    Processor.PC++;
                    cycles = 8;
                    break;
                case 0x08:  // (nn) <- SP
                    word = (ushort)(Memory.Data[Processor.PC + 2] << 8 | Memory.Data[Processor.PC + 1]);
                    Memory.writeByte(word, (byte) Processor.SP);
                    Memory.writeByte(word+1, (byte)(Processor.SP >> 8) );
                    Processor.PC += 3;
                    cycles = 20;
                    break;

                // register to register transfer
                case 0x7F:  // A <- A 
                    Processor.PC++;
                    cycles = 4;
                    break;
                case 0x78:  // A <- B 
                    Processor.A = Processor.B;
                    Processor.PC++;
                    cycles = 4;
                    break;
                case 0x79:  // A <- C 
                    Processor.A = Processor.C;
                    Processor.PC++;
                    cycles = 4;
                    break;
                case 0x7A:  // A <- D 
                    Processor.A = Processor.D;
                    Processor.PC++;
                    cycles = 4;
                    break;
                case 0x7B:  // A <- E 
                    Processor.A = Processor.E;
                    Processor.PC++;
                    cycles = 4;
                    break;
                case 0x7C:  // A <- H 
                    Processor.A = Processor.H;
                    Processor.PC++;
                    cycles = 4;
                    break;
                case 0x7D:  // A <- L 
                    Processor.A = Processor.L;
                    Processor.PC++;
                    cycles = 4;
                    break;
                case 0x47:  // B <- A 
                    Processor.B = Processor.A;
                    Processor.PC++;
                    cycles = 4;
                    break;
                case 0x40:  // B <- B 
                    Processor.PC++;
                    cycles = 4;
                    break;
                case 0x41:  // B <- C 
                    Processor.B = Processor.C;
                    Processor.PC++;
                    cycles = 4;
                    break;
                case 0x42:  // B <- D 
                    Processor.B = Processor.D;
                    Processor.PC++;
                    cycles = 4;
                    break;
                case 0x43:  // B <- E 
                    Processor.B = Processor.E;
                    Processor.PC++;
                    cycles = 4;
                    break;
                case 0x44:  // B <- H 
                    Processor.B = Processor.H;
                    Processor.PC++;
                    cycles = 4;
                    break;
                case 0x45:  // B <- L 
                    Processor.B = Processor.L;
                    Processor.PC++;
                    cycles = 4;
                    break;
                case 0x4F:  // C <- A 
                    Processor.C = Processor.A;
                    Processor.PC++;
                    cycles = 4;
                    break;
                case 0x48:  // C <- B 
                    Processor.C = Processor.B;
                    Processor.PC++;
                    cycles = 4;
                    break;
                case 0x49:  // C <- C 
                    Processor.PC++;
                    cycles = 4;
                    break;
                case 0x4A:  // C <- D 
                    Processor.C = Processor.D;
                    Processor.PC++;
                    cycles = 4;
                    break;
                case 0x4B:  // C <- E 
                    Processor.C = Processor.E;
                    Processor.PC++;
                    cycles = 4;
                    break;
                case 0x4C:  // C <- H 
                    Processor.C = Processor.H;
                    Processor.PC++;
                    cycles = 4;
                    break;
                case 0x4D:  // C <- L 
                    Processor.C = Processor.L;
                    Processor.PC++;
                    cycles = 4;
                    break;
                case 0x57:  // D <- A 
                    Processor.D = Processor.A;
                    Processor.PC++;
                    cycles = 4;
                    break;
                case 0x50:  // D <- B 
                    Processor.D = Processor.B;
                    Processor.PC++;
                    cycles = 4;
                    break;
                case 0x51:  // D <- C 
                    Processor.D = Processor.C;
                    Processor.PC++;
                    cycles = 4;
                    break;
                case 0x52:  // D <- D 
                    Processor.PC++;
                    cycles = 4;
                    break;
                case 0x53:  // D <- E 
                    Processor.D = Processor.E;
                    Processor.PC++;
                    cycles = 4;
                    break;
                case 0x54:  // D <- H 
                    Processor.D = Processor.H;
                    Processor.PC++;
                    cycles = 4;
                    break;
                case 0x55:  // D <- L 
                    Processor.D = Processor.L;
                    Processor.PC++;
                    cycles = 4;
                    break;
                case 0x5F:  // E <- A 
                    Processor.E = Processor.A;
                    Processor.PC++;
                    cycles = 4;
                    break;
                case 0x58:  // E <- B 
                    Processor.E = Processor.B;
                    Processor.PC++;
                    cycles = 4;
                    break;
                case 0x59:  // E <- C 
                    Processor.E = Processor.C;
                    Processor.PC++;
                    cycles = 4;
                    break;
                case 0x5A:  // E <- D 
                    Processor.E = Processor.D;
                    Processor.PC++;
                    cycles = 4;
                    break;
                case 0x5B:  // E <- E 
                    Processor.PC++;
                    cycles = 4;
                    break;
                case 0x5C:  // E <- H 
                    Processor.E = Processor.H;
                    Processor.PC++;
                    cycles = 4;
                    break;
                case 0x5D:  // E <- L 
                    Processor.E = Processor.L;
                    Processor.PC++;
                    cycles = 4;
                    break;
                case 0x67:  // H <- A 
                    Processor.H = Processor.A;
                    Processor.PC++;
                    cycles = 4;
                    break;
                case 0x60:  // H <- B 
                    Processor.H = Processor.B;
                    Processor.PC++;
                    cycles = 4;
                    break;
                case 0x61:  // H <- C 
                    Processor.H = Processor.C;
                    Processor.PC++;
                    cycles = 4;
                    break;
                case 0x62:  // H <- D 
                    Processor.H = Processor.D;
                    Processor.PC++;
                    cycles = 4;
                    break;
                case 0x63:  // H <- E 
                    Processor.H = Processor.E;
                    Processor.PC++;
                    cycles = 4;
                    break;
                case 0x64:  // H <- H 
                    Processor.PC++;
                    cycles = 4;
                    break;
                case 0x65:  // H <- L 
                    Processor.H = Processor.L;
                    Processor.PC++;
                    cycles = 4;
                    break;
                case 0x6F:  // L <- A 
                    Processor.L = Processor.A;
                    Processor.PC++;
                    cycles = 4;
                    break;
                case 0x68:  // L <- B 
                    Processor.L = Processor.B;
                    Processor.PC++;
                    cycles = 4;
                    break;
                case 0x69:  // L <- C 
                    Processor.L = Processor.C;
                    Processor.PC++;
                    cycles = 4;
                    break;
                case 0x6A:  // L <- D 
                    Processor.L = Processor.D;
                    Processor.PC++;
                    cycles = 4;
                    break;
                case 0x6B:  // L <- E 
                    Processor.L = Processor.E;
                    Processor.PC++;
                    cycles = 4;
                    break;
                case 0x6C:  // L <- H 
                    Processor.L = Processor.H;
                    Processor.PC++;
                    cycles = 4;
                    break;
                case 0x6D:  // L <- L 
                    Processor.PC++;
                    cycles = 4;
                    break;

                case 0xF9:  // SP <- HL 
                    Processor.SP = Processor.HL;
                    Processor.PC++;
                    cycles = 8;
                    break;

                // STACK OPS
                    // PUSH
                case 0xF5:  // PUSH AF
                    StackPush(Processor.F);
                    StackPush(Processor.A);
                    Processor.PC++;
                    cycles = 16;
                    break;
                case 0xC5:  // PUSH BC
                    StackPush(Processor.C);
                    StackPush(Processor.B);
                    Processor.PC++;
                    cycles = 16;
                    break;
                case 0xD5:  // PUSH DE
                    StackPush(Processor.E);
                    StackPush(Processor.D);
                    Processor.PC++;
                    cycles = 16;
                    break;
                case 0xE5:  // PUSH HL
                    StackPush(Processor.L);
                    StackPush(Processor.H);
                    Processor.PC++;
                    cycles = 16;
                    break;

                    // POP
                case 0xF1:  // POP AF
                    Processor.A = StackPop();
                    Processor.F = StackPop();
                    Processor.PC++;
                    cycles = 12;
                    break;
                case 0xC1:  // POP BC
                    Processor.B = StackPop();
                    Processor.C = StackPop();
                    Processor.PC++;
                    cycles = 12;
                    break;
                case 0xD1:  // POP DE
                    Processor.D = StackPop();
                    Processor.E = StackPop();
                    Processor.PC++;
                    cycles = 12;
                    break;
                case 0xE1:  // POP HL
                    Processor.H = StackPop();
                    Processor.L = StackPop();
                    Processor.PC++;
                    cycles = 12;
                    break;


                #endregion


                #region /* Arithmetic instructions */

                // 8-bit arithmetics

                // ADD
                case 0x87:  // A = A+A  
                    Processor.A += Processor.A;
                    Processor.ZeroFlag = (uint)((Processor.A == 0) ? 1 : 0);
                    Processor.SubtractFlag = 0;
                    Processor.HalfCarryFlag = 0;    /* carry from bit 3 ???*/
                    Processor.CarryFlag = 0;        /* carry from bit 7 ???*/
                    Processor.PC++;
                    cycles = 4;
                    break;

                case 0x80:  // A = A+B  
                    Processor.A += Processor.B;
                    Processor.ZeroFlag = (uint)((Processor.A == 0) ? 1 : 0);
                    Processor.SubtractFlag = 0;
                    Processor.HalfCarryFlag = 0;    /* carry from bit 3 ???*/
                    Processor.CarryFlag = 0;        /* carry from bit 7 ???*/
                    Processor.PC++;
                    cycles = 4;
                    break;

                case 0x81:  // A = A+C  
                    Processor.A += Processor.C;
                    Processor.ZeroFlag = (uint)((Processor.A == 0) ? 1 : 0);
                    Processor.SubtractFlag = 0;
                    Processor.HalfCarryFlag = 0;    /* carry from bit 3 ???*/
                    Processor.CarryFlag = 0;        /* carry from bit 7 ???*/
                    Processor.PC++;
                    cycles = 4;
                    break;

                case 0x82:  // A = A+D  
                    Processor.A += Processor.D;
                    Processor.ZeroFlag = (uint)((Processor.A == 0) ? 1 : 0);
                    Processor.SubtractFlag = 0;
                    Processor.HalfCarryFlag = 0;    /* carry from bit 3 ???*/
                    Processor.CarryFlag = 0;        /* carry from bit 7 ???*/
                    Processor.PC++;
                    cycles = 4;
                    break;

                case 0x83:  // A = A+E  
                    Processor.A += Processor.E;
                    Processor.ZeroFlag = (uint)((Processor.A == 0) ? 1 : 0);
                    Processor.SubtractFlag = 0;
                    Processor.HalfCarryFlag = 0;    /* carry from bit 3 ???*/
                    Processor.CarryFlag = 0;        /* carry from bit 7 ???*/
                    Processor.PC++;
                    cycles = 4;
                    break;

                case 0x84:  // A = A+H  
                    Processor.A += Processor.H;
                    Processor.ZeroFlag = (uint)((Processor.A == 0) ? 1 : 0);
                    Processor.SubtractFlag = 0;
                    Processor.HalfCarryFlag = 0;    /* carry from bit 3 ???*/
                    Processor.CarryFlag = 0;        /* carry from bit 7 ???*/
                    Processor.PC++;
                    cycles = 4;
                    break;

                case 0x85:  // A = A+L  
                    Processor.A += Processor.L;
                    Processor.ZeroFlag = (uint)((Processor.A == 0) ? 1 : 0);
                    Processor.SubtractFlag = 0;
                    Processor.HalfCarryFlag = 0;    /* carry from bit 3 ???*/
                    Processor.CarryFlag = 0;        /* carry from bit 7 ???*/
                    Processor.PC++;
                    cycles = 4;
                    break;

                case 0x86:  // A = A+(HL)  
                    Processor.A += Memory.readByte(Processor.HL);
                    Processor.ZeroFlag = (uint)((Processor.A == 0) ? 1 : 0);
                    Processor.SubtractFlag = 0;
                    Processor.HalfCarryFlag = 0;    /* carry from bit 3 ???*/
                    Processor.CarryFlag = 0;        /* carry from bit 7 ???*/
                    Processor.PC++;
                    cycles = 8;
                    break;

                case 0xC6:  // A = A + immediate  
                    Processor.A += Memory.Data[Processor.PC+1];
                    Processor.ZeroFlag = (uint)((Processor.A == 0) ? 1 : 0);
                    Processor.SubtractFlag = 0;
                    Processor.HalfCarryFlag = 0;    /* carry from bit 3 ???*/
                    Processor.CarryFlag = 0;        /* carry from bit 7 ???*/
                    Processor.PC += 2;
                    cycles = 8;
                    break;

                // ADC
                case 0x8F:  // A = A+A+Carry  
                    Processor.A += (byte)(Processor.A + Processor.CarryFlag);
                    Processor.ZeroFlag = (uint)((Processor.A == 0) ? 1 : 0);
                    Processor.SubtractFlag = 0;
                    Processor.HalfCarryFlag = 0;    /* carry from bit 3 ???*/
                    Processor.CarryFlag = 0;        /* carry from bit 7 ???*/
                    Processor.PC++;
                    cycles = 4;
                    break;

                case 0x88:  // A = A+B+Carry 
                    Processor.A += (byte)(Processor.B + Processor.CarryFlag);
                    Processor.ZeroFlag = (uint)((Processor.A == 0) ? 1 : 0);
                    Processor.SubtractFlag = 0;
                    Processor.HalfCarryFlag = 0;    /* carry from bit 3 ???*/
                    Processor.CarryFlag = 0;        /* carry from bit 7 ???*/
                    Processor.PC++;
                    cycles = 4;
                    break;

                case 0x89:  // A = A+C+Carry  
                    Processor.A += (byte)(Processor.C + Processor.CarryFlag);
                    Processor.ZeroFlag = (uint)((Processor.A == 0) ? 1 : 0);
                    Processor.SubtractFlag = 0;
                    Processor.HalfCarryFlag = 0;    /* carry from bit 3 ???*/
                    Processor.CarryFlag = 0;        /* carry from bit 7 ???*/
                    Processor.PC++;
                    cycles = 4;
                    break;

                case 0x8A:  // A = A+D+Carry  
                    Processor.A += (byte)(Processor.D + Processor.CarryFlag);
                    Processor.ZeroFlag = (uint)((Processor.A == 0) ? 1 : 0);
                    Processor.SubtractFlag = 0;
                    Processor.HalfCarryFlag = 0;    /* carry from bit 3 ???*/
                    Processor.CarryFlag = 0;        /* carry from bit 7 ???*/
                    Processor.PC++;
                    cycles = 4;
                    break;

                case 0x8B:  // A = A+E+Carry  
                    Processor.A += (byte)(Processor.E + Processor.CarryFlag);
                    Processor.ZeroFlag = (uint)((Processor.A == 0) ? 1 : 0);
                    Processor.SubtractFlag = 0;
                    Processor.HalfCarryFlag = 0;    /* carry from bit 3 ???*/
                    Processor.CarryFlag = 0;        /* carry from bit 7 ???*/
                    Processor.PC++;
                    cycles = 4;
                    break;

                case 0x8C:  // A = A+H+Carry  
                    Processor.A += (byte)(Processor.H + Processor.CarryFlag);
                    Processor.ZeroFlag = (uint)((Processor.A == 0) ? 1 : 0);
                    Processor.SubtractFlag = 0;
                    Processor.HalfCarryFlag = 0;    /* carry from bit 3 ???*/
                    Processor.CarryFlag = 0;        /* carry from bit 7 ???*/
                    Processor.PC++;
                    cycles = 4;
                    break;

                case 0x8D:  // A = A+L+Carry  
                    Processor.A += (byte)(Processor.L + Processor.CarryFlag);
                    Processor.ZeroFlag = (uint)((Processor.A == 0) ? 1 : 0);
                    Processor.SubtractFlag = 0;
                    Processor.HalfCarryFlag = 0;    /* carry from bit 3 ???*/
                    Processor.CarryFlag = 0;        /* carry from bit 7 ???*/
                    Processor.PC++;
                    cycles = 4;
                    break;

                case 0x8E:  // A = A+(HL)+Carry  
                    Processor.A += (byte)(Memory.readByte(Processor.HL) + Processor.CarryFlag);
                    Processor.ZeroFlag = (uint)((Processor.A == 0) ? 1 : 0);
                    Processor.SubtractFlag = 0;
                    Processor.HalfCarryFlag = 0;    /* carry from bit 3 ???*/
                    Processor.CarryFlag = 0;        /* carry from bit 7 ???*/
                    Processor.PC++;
                    cycles = 8;
                    break;

                case 0xCE:  // A = A + immediate + Carry  
                    Processor.A += (byte)(Memory.Data[Processor.PC + 1] + Processor.CarryFlag);
                    Processor.ZeroFlag = (uint)((Processor.A == 0) ? 1 : 0);
                    Processor.SubtractFlag = 0;
                    Processor.HalfCarryFlag = 0;    /* carry from bit 3 ???*/
                    Processor.CarryFlag = 0;        /* carry from bit 7 ???*/
                    Processor.PC += 2;
                    cycles = 8;
                    break;

                // SUB
                case 0x97:  // A = A-A  
                    Processor.A -= Processor.A;
                    Processor.ZeroFlag = (uint)((Processor.A == 0) ? 1 : 0);
                    Processor.SubtractFlag = 1;
                    Processor.HalfCarryFlag = 0;    /* carry from bit 3 ???*/
                    Processor.CarryFlag = 0;        /* carry from bit 7 ???*/
                    Processor.PC++;
                    cycles = 4;
                    break;

                case 0x90:  // A = A-B  
                    Processor.A -= Processor.B;
                    Processor.ZeroFlag = (uint)((Processor.A == 0) ? 1 : 0);
                    Processor.SubtractFlag = 1;
                    Processor.HalfCarryFlag = 0;    /* carry from bit 3 ???*/
                    Processor.CarryFlag = 0;        /* carry from bit 7 ???*/
                    Processor.PC++;
                    cycles = 4;
                    break;

                case 0x91:  // A = A-C  
                    Processor.A -= Processor.C;
                    Processor.ZeroFlag = (uint)((Processor.A == 0) ? 1 : 0);
                    Processor.SubtractFlag = 1;
                    Processor.HalfCarryFlag = 0;    /* carry from bit 3 ???*/
                    Processor.CarryFlag = 0;        /* carry from bit 7 ???*/
                    Processor.PC++;
                    cycles = 4;
                    break;

                case 0x92:  // A = A-D  
                    Processor.A -= Processor.D;
                    Processor.ZeroFlag = (uint)((Processor.A == 0) ? 1 : 0);
                    Processor.SubtractFlag = 1;
                    Processor.HalfCarryFlag = 0;    /* carry from bit 3 ???*/
                    Processor.CarryFlag = 0;        /* carry from bit 7 ???*/
                    Processor.PC++;
                    cycles = 4;
                    break;

                case 0x93:  // A = A-E  
                    Processor.A -= Processor.E;
                    Processor.ZeroFlag = (uint)((Processor.A == 0) ? 1 : 0);
                    Processor.SubtractFlag = 1;
                    Processor.HalfCarryFlag = 0;    /* carry from bit 3 ???*/
                    Processor.CarryFlag = 0;        /* carry from bit 7 ???*/
                    Processor.PC++;
                    cycles = 4;
                    break;

                case 0x94:  // A = A-H  
                    Processor.A -= Processor.H;
                    Processor.ZeroFlag = (uint)((Processor.A == 0) ? 1 : 0);
                    Processor.SubtractFlag = 1;
                    Processor.HalfCarryFlag = 0;    /* carry from bit 3 ???*/
                    Processor.CarryFlag = 0;        /* carry from bit 7 ???*/
                    Processor.PC++;
                    cycles = 4;
                    break;

                case 0x95:  // A = A-L  
                    Processor.A -= Processor.L;
                    Processor.ZeroFlag = (uint)((Processor.A == 0) ? 1 : 0);
                    Processor.SubtractFlag = 1;
                    Processor.HalfCarryFlag = 0;    /* carry from bit 3 ???*/
                    Processor.CarryFlag = 0;        /* carry from bit 7 ???*/
                    Processor.PC++;
                    cycles = 4;
                    break;

                case 0x96:  // A = A-(HL)  
                    Processor.A -= Memory.readByte(Processor.HL);
                    Processor.ZeroFlag = (uint)((Processor.A == 0) ? 1 : 0);
                    Processor.SubtractFlag = 1;
                    Processor.HalfCarryFlag = 0;    /* carry from bit 3 ???*/
                    Processor.CarryFlag = 0;        /* carry from bit 7 ???*/
                    Processor.PC++;
                    cycles = 8;
                    break;

                case 0xD6:  // A = A - immediate  
                    Processor.A -= Memory.Data[Processor.PC + 1];
                    Processor.ZeroFlag = (uint)((Processor.A == 0) ? 1 : 0);
                    Processor.SubtractFlag = 1;
                    Processor.HalfCarryFlag = 0;    /* carry from bit 3 ???*/
                    Processor.CarryFlag = 0;        /* carry from bit 7 ???*/
                    Processor.PC += 2;
                    cycles = 8;
                    break;

                // SBC
                case 0x9F:  // A = A-A+Carry  
                    Processor.A -= (byte)(Processor.A + Processor.CarryFlag);
                    Processor.ZeroFlag = (uint)((Processor.A == 0) ? 1 : 0);
                    Processor.SubtractFlag = 1;
                    Processor.HalfCarryFlag = 0;    /* carry from bit 3 ???*/
                    Processor.CarryFlag = 0;        /* carry from bit 7 ???*/
                    Processor.PC++;
                    cycles = 4;
                    break;

                case 0x98:  // A = A-B+Carry 
                    Processor.A -= (byte)(Processor.B + Processor.CarryFlag);
                    Processor.ZeroFlag = (uint)((Processor.A == 0) ? 1 : 0);
                    Processor.SubtractFlag = 1;
                    Processor.HalfCarryFlag = 0;    /* carry from bit 3 ???*/
                    Processor.CarryFlag = 0;        /* carry from bit 7 ???*/
                    Processor.PC++;
                    cycles = 4;
                    break;

                case 0x99:  // A = A-C+Carry  
                    Processor.A -= (byte)(Processor.C + Processor.CarryFlag);
                    Processor.ZeroFlag = (uint)((Processor.A == 0) ? 1 : 0);
                    Processor.SubtractFlag = 1;
                    Processor.HalfCarryFlag = 0;    /* carry from bit 3 ???*/
                    Processor.CarryFlag = 0;        /* carry from bit 7 ???*/
                    Processor.PC++;
                    cycles = 4;
                    break;

                case 0x9A:  // A = A-D+Carry  
                    Processor.A -= (byte)(Processor.D + Processor.CarryFlag);
                    Processor.ZeroFlag = (uint)((Processor.A == 0) ? 1 : 0);
                    Processor.SubtractFlag = 1;
                    Processor.HalfCarryFlag = 0;    /* carry from bit 3 ???*/
                    Processor.CarryFlag = 0;        /* carry from bit 7 ???*/
                    Processor.PC++;
                    cycles = 4;
                    break;

                case 0x9B:  // A = A-E+Carry  
                    Processor.A -= (byte)(Processor.E + Processor.CarryFlag);
                    Processor.ZeroFlag = (uint)((Processor.A == 0) ? 1 : 0);
                    Processor.SubtractFlag = 1;
                    Processor.HalfCarryFlag = 0;    /* carry from bit 3 ???*/
                    Processor.CarryFlag = 0;        /* carry from bit 7 ???*/
                    Processor.PC++;
                    cycles = 4;
                    break;

                case 0x9C:  // A = A-H+Carry  
                    Processor.A -= (byte)(Processor.H + Processor.CarryFlag);
                    Processor.ZeroFlag = (uint)((Processor.A == 0) ? 1 : 0);
                    Processor.SubtractFlag = 1;
                    Processor.HalfCarryFlag = 0;    /* carry from bit 3 ???*/
                    Processor.CarryFlag = 0;        /* carry from bit 7 ???*/
                    Processor.PC++;
                    cycles = 4;
                    break;

                case 0x9D:  // A = A-L+Carry  
                    Processor.A -= (byte)(Processor.L + Processor.CarryFlag);
                    Processor.ZeroFlag = (uint)((Processor.A == 0) ? 1 : 0);
                    Processor.SubtractFlag = 1;
                    Processor.HalfCarryFlag = 0;    /* carry from bit 3 ???*/
                    Processor.CarryFlag = 0;        /* carry from bit 7 ???*/
                    Processor.PC++;
                    cycles = 4;
                    break;

                case 0x9E:  // A = A-(HL)+Carry  
                    Processor.A -= (byte)(Memory.readByte(Processor.HL) + Processor.CarryFlag);
                    Processor.ZeroFlag = (uint)((Processor.A == 0) ? 1 : 0);
                    Processor.SubtractFlag = 1;
                    Processor.HalfCarryFlag = 0;    /* carry from bit 3 ???*/
                    Processor.CarryFlag = 0;        /* carry from bit 7 ???*/
                    Processor.PC++;
                    cycles = 8;
                    break;

               /* case 0x??:  // A = A - immediate + Carry   // Not existent???
                    Processor.A -= (byte)(Memory.Data[Processor.PC + 1] + Processor.CarryFlag);
                    Processor.ZeroFlag = (uint)((Processor.A == 0) ? 1 : 0);
                    Processor.SubtractFlag = 1;
                    Processor.HalfCarryFlag = 0;    // carry from bit 3 ???
                    Processor.CarryFlag = 0;        // carry from bit 7 ???
                    Processor.PC += 2;
                    cycles = 8;
                    break;
               */

                // INC
                case 0x3C:  // A++  
                    Processor.A++;
                    Processor.ZeroFlag = (uint)((Processor.A == 0) ? 1 : 0);
                    Processor.SubtractFlag = 0;
                    Processor.HalfCarryFlag = 0;   /* carry from bit 3 ???*/
                    Processor.PC++;
                    cycles = 4;
                    break;

                case 0x04:  // B++  
                    Processor.B++;
                    Processor.ZeroFlag = (uint)((Processor.B == 0) ? 1 : 0);
                    Processor.SubtractFlag = 0;
                    Processor.HalfCarryFlag = 0; /* carry from bit 3 ???*/
                    Processor.PC++;
                    cycles = 4;
                    break;

                case 0x0C:  // C++  
                    Processor.C++;
                    Processor.ZeroFlag = (uint)((Processor.C == 0) ? 1 : 0);
                    Processor.SubtractFlag = 0;
                    Processor.HalfCarryFlag = 0; /* carry from bit 3 ???*/
                    Processor.PC++;
                    cycles = 4;
                    break;

                case 0x14:  // D++  
                    Processor.D++;
                    Processor.ZeroFlag = (uint)((Processor.D == 0) ? 1 : 0);
                    Processor.SubtractFlag = 0;
                    Processor.HalfCarryFlag = 0; /* carry from bit 3 ???*/
                    Processor.PC++;
                    cycles = 4;
                    break;

                case 0x1C:  // E++  
                    Processor.E++;
                    Processor.ZeroFlag = (uint)((Processor.E == 0) ? 1 : 0);
                    Processor.SubtractFlag = 0;
                    Processor.HalfCarryFlag = 0; /* carry from bit 3 ???*/
                    Processor.PC++;
                    cycles = 4;
                    break;

                case 0x24:  // H++  
                    Processor.H++;
                    Processor.ZeroFlag = (uint)((Processor.H == 0) ? 1 : 0);
                    Processor.SubtractFlag = 0;
                    Processor.HalfCarryFlag = 0; /* carry from bit 3 ???*/
                    Processor.PC++;
                    cycles = 4;
                    break;

                case 0x2C:  // L++  
                    Processor.L++;
                    Processor.ZeroFlag = (uint)((Processor.L == 0) ? 1 : 0);
                    Processor.SubtractFlag = 0;
                    Processor.HalfCarryFlag = 0; /* carry from bit 3 ???*/
                    Processor.PC++;
                    cycles = 4;
                    break;

                case 0x34:  // (HL)++  
                    value = Memory.readByte(Processor.HL);
                    value++;
                    Memory.writeByte(Processor.HL, value);
                    Processor.ZeroFlag = (uint)((value == 0) ? 1 : 0);
                    Processor.SubtractFlag = 0;
                    Processor.HalfCarryFlag = 0;  /* carry from bit 3 ???*/
                    Processor.PC++;
                    cycles = 12;
                    break;

                // DEC
                case 0x3D:  // A--  
                    Processor.A--;
                    Processor.ZeroFlag = (uint) ((Processor.A == 0) ? 1 : 0);
                    Processor.SubtractFlag = 1;
                    Processor.HalfCarryFlag = 0; /* borrow from bit 4 ???*/
                    Processor.PC++;
                    cycles = 4;
                    break;

                case 0x05:  // B--  
                    Processor.B--;
                    Processor.ZeroFlag = (uint)((Processor.B == 0) ? 1 : 0);
                    Processor.SubtractFlag = 1;
                    Processor.HalfCarryFlag = 0; /* borrow from bit 4 ???*/
                    Processor.PC++;
                    cycles = 4;
                    break;

                case 0x0D:  // C--  
                    Processor.C--;
                    Processor.ZeroFlag = (uint)((Processor.C == 0) ? 1 : 0);
                    Processor.SubtractFlag = 1;
                    Processor.HalfCarryFlag = 0; /* borrow from bit 4 ???*/
                    Processor.PC++;
                    cycles = 4;
                    break;

                case 0x15:  // D--  
                    Processor.D--;
                    Processor.ZeroFlag = (uint)((Processor.D == 0) ? 1 : 0);
                    Processor.SubtractFlag = 1;
                    Processor.HalfCarryFlag = 0; /* borrow from bit 4 ???*/
                    Processor.PC++;
                    cycles = 4;
                    break;

                case 0x1D:  // E--  
                    Processor.E--;
                    Processor.ZeroFlag = (uint)((Processor.E == 0) ? 1 : 0);
                    Processor.SubtractFlag = 1;
                    Processor.HalfCarryFlag = 0; /* borrow from bit 4 ???*/
                    Processor.PC++;
                    cycles = 4;
                    break;

                case 0x25:  // H--  
                    Processor.H--;
                    Processor.ZeroFlag = (uint)((Processor.H == 0) ? 1 : 0);
                    Processor.SubtractFlag = 1;
                    Processor.HalfCarryFlag = 0; /* borrow from bit 4 ???*/
                    Processor.PC++;
                    cycles = 4;
                    break;

                case 0x2D:  // L--  
                    Processor.L--;
                    Processor.ZeroFlag = (uint)((Processor.L == 0) ? 1 : 0);
                    Processor.SubtractFlag = 1;
                    Processor.HalfCarryFlag = 0; /* borrow from bit 4 ???*/
                    Processor.PC++;
                    cycles = 4;
                    break;

                case 0x35:  // (HL)--  
                    value = Memory.readByte(Processor.HL);
                    value--;
                    Memory.writeByte(Processor.HL, value);
                    Processor.ZeroFlag = (uint)((value == 0) ? 1 : 0);
                    Processor.SubtractFlag = 1;
                    Processor.HalfCarryFlag = 0; /* borrow from bit 4 ???*/
                    Processor.PC++;
                    cycles = 12;
                    break;

                // CMP 
                case 0xBF:  // A == A 
                    Processor.SetFlags(1, 1, 0, 0); /* Borrow bit 4 and carry set for no borrow? */
                    Processor.PC++;
                    cycles = 4;
                    break;
                case 0xB8:  // A == B                                       /* same here */
                    Processor.SetFlags((Processor.A == Processor.B) ? 1 : 0, 1, 0, (Processor.A < Processor.B) ? 1 : 0); 
                    Processor.PC++;
                    cycles = 4;
                    break;
                case 0xB9:  // A == C                                       /* same here */
                    Processor.SetFlags((Processor.A == Processor.C) ? 1 : 0, 1, 0, (Processor.A < Processor.C) ? 1 : 0);
                    Processor.PC++;
                    cycles = 4;
                    break;
                case 0xBA:  // A == D                                       /* same here */
                    Processor.SetFlags((Processor.A == Processor.D) ? 1 : 0, 1, 0, (Processor.A < Processor.D) ? 1 : 0);
                    Processor.PC++;
                    cycles = 4;
                    break;
                case 0xBB:  // A == E                                       /* same here */
                    Processor.SetFlags((Processor.A == Processor.E) ? 1 : 0, 1, 0, (Processor.A < Processor.E) ? 1 : 0);
                    Processor.PC++;
                    cycles = 4;
                    break;
                case 0xBC:  // A == H                                       /* same here */
                    Processor.SetFlags((Processor.A == Processor.H) ? 1 : 0, 1, 0, (Processor.A < Processor.H) ? 1 : 0);
                    Processor.PC++;
                    cycles = 4;
                    break;
                case 0xBD:  // A == L                                       /* same here */
                    Processor.SetFlags((Processor.A == Processor.L) ? 1 : 0, 1, 0, (Processor.A < Processor.L) ? 1 : 0);
                    Processor.PC++;
                    cycles = 4;
                    break;
                case 0xBE:  // A == (HL)                                    /* same here */
                    value = Memory.readByte(Processor.HL);
                    Processor.SetFlags((Processor.A == value) ? 1 : 0, 1, 0, (Processor.A < value) ? 1 : 0);
                    Processor.PC++;
                    cycles = 8;
                    break;
                case 0xFE:  // A == immediate                              /* same here */
                    value = Memory.Data[Processor.PC + 1];
                    Processor.SetFlags((Processor.A == value) ? 1 : 0, 1, 0, (Processor.A < value) ? 1 : 0);
                    Processor.PC += 2;
                    cycles = 8;
                    break;

                // 16-bit arithmetics

                // ADD
                case 0x09:  // HL += BC                              
                    Processor.HL += (ushort)(Processor.B << 8 | Processor.C);
                    Processor.H = (byte)(Processor.HL >> 8);
                    Processor.L = (byte)Processor.HL;
                    Processor.SubtractFlag = 0;
                    Processor.HalfCarryFlag = 0;    /* carry from bit 11 */
                    Processor.CarryFlag = 0;        /* carry from bit 15 */
                    Processor.PC++;
                    cycles = 8;
                    break;
                case 0x19:  // HL += DE                              
                    Processor.HL += (ushort)(Processor.D << 8 | Processor.E);
                    Processor.H = (byte)(Processor.HL >> 8);
                    Processor.L = (byte)Processor.HL;
                    Processor.SubtractFlag = 0;
                    Processor.HalfCarryFlag = 0;    /* carry from bit 11 */
                    Processor.CarryFlag = 0;        /* carry from bit 15 */
                    Processor.PC++;
                    cycles = 8;
                    break;
                case 0x29:  // HL += HL                              
                    Processor.HL += Processor.HL;
                    Processor.H = (byte)(Processor.HL >> 8);
                    Processor.L = (byte)Processor.HL;
                    Processor.SubtractFlag = 0;
                    Processor.HalfCarryFlag = 0;    /* carry from bit 11 */
                    Processor.CarryFlag = 0;        /* carry from bit 15 */
                    Processor.PC++;
                    cycles = 8;
                    break;
                case 0x39:  // HL += SP                              
                    Processor.HL += Processor.SP;
                    Processor.H = (byte)(Processor.HL >> 8);
                    Processor.L = (byte)Processor.HL;
                    Processor.SubtractFlag = 0;
                    Processor.HalfCarryFlag = 0;    /* carry from bit 11 */
                    Processor.CarryFlag = 0;        /* carry from bit 15 */
                    Processor.PC++;
                    cycles = 8;
                    break;
                case 0xE8:  // SP += signed immediate byte                              
                    Processor.SP = (ushort) (Processor.SP + ((sbyte)Memory.Data[Processor.PC + 1]));
                    Processor.SetFlags(0, 0, 0, 0); /* carry bit 11 and 15 */
                    Processor.PC +=2 ;
                    cycles = 16;
                    break;

                // INC
                case 0x03:  // BC++
                    word = (ushort)(Processor.B << 8 | Processor.C);
                    word++;
                    Processor.B = (byte)(word >> 8);
                    Processor.C = (byte) word;
                    Processor.PC++;
                    cycles = 8;
                    break;
                case 0x13:  // DE++
                    word = (ushort)(Processor.D << 8 | Processor.E);
                    word++;
                    Processor.D = (byte)(word >> 8);
                    Processor.E = (byte)word;
                    Processor.PC++;
                    cycles = 8;
                    break;
                case 0x23:  // HL++
                    Processor.HL++;
                    Processor.H = (byte)(Processor.HL >> 8);
                    Processor.L = (byte)Processor.HL;
                    Processor.PC++;
                    cycles = 8;
                    break;
                case 0x33:  // SP++
                    Processor.SP++;
                    Processor.PC++;
                    cycles = 8;
                    break;

                // DEC
                case 0x0B:  // BC--
                    word = (ushort)(Processor.B << 8 | Processor.C);
                    word--;
                    Processor.B = (byte)(word >> 8);
                    Processor.C = (byte)word;
                    Processor.PC++;
                    cycles = 8;
                    break;
                case 0x1B:  // DE--
                    word = (ushort)(Processor.D << 8 | Processor.E);
                    word--;
                    Processor.D = (byte)(word >> 8);
                    Processor.E = (byte)word;
                    Processor.PC++;
                    cycles = 8;
                    break;
                case 0x2B:  // HL--
                    Processor.HL--;
                    Processor.H = (byte)(Processor.HL >> 8);
                    Processor.L = (byte)Processor.HL;
                    Processor.PC++;
                    cycles = 8;
                    break;
                case 0x3B:  // SP--
                    Processor.SP--;
                    Processor.PC++;
                    cycles = 8;
                    break;

                #endregion


                #region /* Jump instructions */
                // absolute jumps
                case 0xC3:  // Unconditional + 2B immediate operands
                    Processor.PC = (ushort)(Memory.Data[Processor.PC + 2] << 8 | Memory.Data[Processor.PC + 1]);
                    cycles = 12;
                    break;

                case 0xC2:  // Conditional NZ + 2B immediate operands
                    if (Processor.ZeroFlag == 0) 
                        Processor.PC = (ushort)(Memory.Data[Processor.PC + 2] << 8 | Memory.Data[Processor.PC + 1]);
                    else Processor.PC += 3;
                    cycles = 12;
                    break;

                case 0xCA:  // Conditional Z + 2B immediate operands
                    if (Processor.ZeroFlag != 0)
                        Processor.PC = (ushort)(Memory.Data[Processor.PC + 2] << 8 | Memory.Data[Processor.PC + 1]);
                    else Processor.PC += 3;
                    cycles = 12;
                    break;

                case 0xD2:  // Conditional NC + 2B immediate operands
                    if (Processor.CarryFlag == 0)
                        Processor.PC = (ushort)(Memory.Data[Processor.PC + 2] << 8 | Memory.Data[Processor.PC + 1]);
                    else Processor.PC += 3;
                    cycles = 12;
                    break;

                case 0xDA:  // Conditional C + 2B immediate operands
                    if (Processor.CarryFlag != 0)
                        Processor.PC = (ushort)(Memory.Data[Processor.PC + 2] << 8 | Memory.Data[Processor.PC + 1]);
                    else Processor.PC += 3;
                    cycles = 12;
                    break;

                case 0xE9:  // Unconditional jump to HL
                    Processor.PC = Processor.HL;
                    cycles = 4;
                    break;

                // relative jumps
                case 0x18:  // Unconditional + relative byte
                    Processor.PC = (ushort)(Processor.PC + (sbyte)Memory.Data[Processor.PC + 1]+2);
                    cycles = 8;
                    break;

                case 0x20:  // Conditional NZ + relative byte
                    if (Processor.ZeroFlag == 0) 
                        Processor.PC = (ushort) (Processor.PC + (sbyte) Memory.Data[Processor.PC + 1]+2);
                    else Processor.PC += 2;
                    cycles = 8;
                    break;

                case 0x28:  // Conditional Z + relative byte
                    if (Processor.ZeroFlag != 0)
                        Processor.PC = (ushort)(Processor.PC + (sbyte)Memory.Data[Processor.PC + 1]+2);
                    else Processor.PC += 2;
                    cycles = 8;
                    break;

                case 0x30:  // Conditional NC + relative byte
                    if (Processor.CarryFlag == 0)
                        Processor.PC = (ushort)(Processor.PC + (sbyte)Memory.Data[Processor.PC + 1]+2);
                    else Processor.PC += 2;
                    cycles = 8;
                    break;

                case 0x38:  // Conditional C + relative byte
                    if (Processor.CarryFlag != 0)
                        Processor.PC = (ushort)(Processor.PC + (sbyte)Memory.Data[Processor.PC + 1]+2);
                    else Processor.PC += 2;
                    cycles = 8;
                    break;

                // calls
                case 0xCD:  // unconditional 
                    Call();
                    cycles = 12;
                    break;

                case 0xC4:  // Conditional NZ 
                    if (Processor.ZeroFlag == 0)
                        Call();
                    else Processor.PC += 3;
                    cycles = 12;
                    break;

                case 0xCC:  // Conditional Z 
                    if (Processor.ZeroFlag != 0)
                        Call();
                    else Processor.PC += 3;
                    cycles = 12;
                    break;

                case 0xD4:  // Conditional NC
                    if (Processor.CarryFlag == 0)
                        Call();
                    else Processor.PC += 3;
                    cycles = 12;
                    break;

                case 0xDC:  // Conditional C 
                    if (Processor.CarryFlag != 0)
                        Call();
                    else Processor.PC += 3;
                    cycles = 12;
                    break;

                // resets
                case 0xC7:
                    Reset(0x00);
                    cycles = 32;
                    break;
                case 0xCF:
                    Reset(0x08);
                    cycles = 32;
                    break;
                case 0xD7:
                    Reset(0x10);
                    cycles = 32;
                    break;
                case 0xDF:
                    Reset(0x18);
                    cycles = 32;
                    break;
                case 0xE7:
                    Reset(0x20);
                    cycles = 32;
                    break;
                case 0xEF:
                    Reset(0x28);
                    cycles = 32;
                    break;
                case 0xF7:
                    Reset(0x30);
                    cycles = 32;
                    break;
                case 0xFF:
                    Reset(0x38);
                    cycles = 32;
                    break;

                // returns
                case 0xC9:  // unconditional
                    word = (ushort)(StackPop() << 8);    // Pop higher byte of return address
                    Processor.PC += (ushort) (word + StackPop()); // Pop lower and combine
                    cycles = 8;
                    break;
                case 0xD9:  // unconditional plus enable interrupts (RETI)
                    word = (ushort)(StackPop() << 8);
                    Processor.PC += (ushort)(word + StackPop());
                    Processor.EIsignaled = 1;
                    cycles = 8;
                    break;
                case 0xC0:  // Conditional NZ 
                    if (Processor.ZeroFlag == 0)
                    {
                        word = (ushort)(StackPop() << 8);
                        Processor.PC += (ushort)(word + StackPop());
                    }
                    else Processor.PC++;
                    cycles = 8;
                    break;
                case 0xC8:  // Conditional Z 
                    if (Processor.ZeroFlag != 0)
                    {
                        word = (ushort)(StackPop() << 8);
                        Processor.PC += (ushort)(word + StackPop());
                    }
                    else Processor.PC++;
                    cycles = 8;
                    break;
                case 0xD0:  // Conditional NC
                    if (Processor.CarryFlag == 0)
                    {
                        word = (ushort)(StackPop() << 8);
                        Processor.PC += (ushort)(word + StackPop());
                    }
                    else Processor.PC++;
                    cycles = 8;
                    break;
                case 0xD8:  // Conditional C 
                    if (Processor.CarryFlag != 0)
                    {
                        word = (ushort)(StackPop() << 8);
                        Processor.PC += (ushort)(word + StackPop());
                    }
                    else Processor.PC++;
                    cycles = 8;
                    break;


                #endregion


                #region /* Logical instructions */

                // OR
                case 0xB7:  // A = A OR A = A !!!
                    Processor.SetFlags((Processor.A == 0) ? 1 : 0, 0, 0, 0);
                    Processor.PC++;
                    cycles = 4;
                    break;
                case 0xB0:  // A = A OR B
                    Processor.A |= Processor.B;
                    Processor.SetFlags((Processor.A == 0) ? 1 : 0, 0, 0, 0);
                    Processor.PC++;
                    cycles = 4;
                    break;
                case 0xB1:  // A = A OR C
                    Processor.A |= Processor.C;
                    Processor.SetFlags((Processor.A == 0) ? 1 : 0, 0, 0, 0);
                    Processor.PC++;
                    cycles = 4;
                    break;
                case 0xB2:  // A = A OR D
                    Processor.A |= Processor.D;
                    Processor.SetFlags((Processor.A == 0) ? 1 : 0, 0, 0, 0);
                    Processor.PC++;
                    cycles = 4;
                    break;
                case 0xB3:  // A = A OR E
                    Processor.A |= Processor.E;
                    Processor.SetFlags((Processor.A == 0) ? 1 : 0, 0, 0, 0);
                    Processor.PC++;
                    cycles = 4;
                    break;
                case 0xB4:  // A = A OR H
                    Processor.A |= Processor.H;
                    Processor.SetFlags((Processor.A == 0) ? 1 : 0, 0, 0, 0);
                    Processor.PC++;
                    cycles = 4;
                    break;
                case 0xB5:  // A = A OR L
                    Processor.A |= Processor.L;
                    Processor.SetFlags((Processor.A == 0) ? 1 : 0, 0, 0, 0);
                    Processor.PC++;
                    cycles = 4;
                    break;
                case 0xB6:  // A = A OR (HL)
                    Processor.A |= Memory.readByte(Processor.HL);
                    Processor.SetFlags((Processor.A == 0) ? 1 : 0, 0, 0, 0);
                    Processor.PC++;
                    cycles = 8;
                    break;
                case 0xF6:  // A = A OR immediate
                    Processor.A |= Memory.Data[Processor.PC + 1];
                    Processor.SetFlags((Processor.A == 0) ? 1 : 0, 0, 0, 0);
                    Processor.PC += 2;
                    cycles = 8;
                    break;
                // XOR
                case 0xAF:  // A = A XOR A = 0 !!!
                    Processor.A = 0;
                    Processor.SetFlags(1, 0, 0, 0);
                    Processor.PC++;
                    cycles = 4;
                    break;
                case 0xA8:  // A = A XOR B
                    Processor.A ^= Processor.B;
                    Processor.SetFlags( (Processor.A == 0) ? 1:0, 0, 0, 0);
                    Processor.PC++;
                    cycles = 4;
                    break;
                case 0xA9:  // A = A XOR C
                    Processor.A ^= Processor.C;
                    Processor.SetFlags((Processor.A == 0) ? 1 : 0, 0, 0, 0);
                    Processor.PC++;
                    cycles = 4;
                    break;
                case 0xAA:  // A = A XOR D
                    Processor.A ^= Processor.D;
                    Processor.SetFlags((Processor.A == 0) ? 1 : 0, 0, 0, 0);
                    Processor.PC++;
                    cycles = 4;
                    break;
                case 0xAB:  // A = A XOR E
                    Processor.A ^= Processor.E;
                    Processor.SetFlags((Processor.A == 0) ? 1 : 0, 0, 0, 0);
                    Processor.PC++;
                    cycles = 4;
                    break;
                case 0xAC:  // A = A XOR H
                    Processor.A ^= Processor.H;
                    Processor.SetFlags((Processor.A == 0) ? 1 : 0, 0, 0, 0);
                    Processor.PC++;
                    cycles = 4;
                    break;
                case 0xAD:  // A = A XOR L
                    Processor.A ^= Processor.L;
                    Processor.SetFlags((Processor.A == 0) ? 1 : 0, 0, 0, 0);
                    Processor.PC++;
                    cycles = 4;
                    break;
                case 0xAE:  // A = A XOR (HL)
                    Processor.A ^= Memory.readByte(Processor.HL);
                    Processor.SetFlags((Processor.A == 0) ? 1 : 0, 0, 0, 0);
                    Processor.PC++;
                    cycles = 8;
                    break;
                case 0xEE:  // A = A XOR immediate
                    Processor.A ^= Memory.Data[Processor.PC+1];
                    Processor.SetFlags((Processor.A == 0) ? 1 : 0, 0, 0, 0);
                    Processor.PC += 2;
                    cycles = 8;
                    break;
                // AND
                case 0xA7:  // A = A AND A 
                    Processor.A &= Processor.A;
                    Processor.SetFlags((Processor.A == 0) ? 1 : 0, 0, 1, 0);
                    Processor.PC++;
                    cycles = 4;
                    break;
                case 0xA0:  // A = A AND B
                    Processor.A &= Processor.B;
                    Processor.SetFlags((Processor.A == 0) ? 1 : 0, 0, 1, 0);
                    Processor.PC++;
                    cycles = 4;
                    break;
                case 0xA1:  // A = A AND C
                    Processor.A &= Processor.C;
                    Processor.SetFlags((Processor.A == 0) ? 1 : 0, 0, 1, 0);
                    Processor.PC++;
                    cycles = 4;
                    break;
                case 0xA2:  // A = A AND D
                    Processor.A &= Processor.D;
                    Processor.SetFlags((Processor.A == 0) ? 1 : 0, 0, 1, 0);
                    Processor.PC++;
                    cycles = 4;
                    break;
                case 0xA3:  // A = A AND E
                    Processor.A &= Processor.E;
                    Processor.SetFlags((Processor.A == 0) ? 1 : 0, 0, 1, 0);
                    Processor.PC++;
                    cycles = 4;
                    break;
                case 0xA4:  // A = A AND H
                    Processor.A &= Processor.H;
                    Processor.SetFlags((Processor.A == 0) ? 1 : 0, 0, 1, 0);
                    Processor.PC++;
                    cycles = 4;
                    break;
                case 0xA5:  // A = A AND L
                    Processor.A &= Processor.L;
                    Processor.SetFlags((Processor.A == 0) ? 1 : 0, 0, 1, 0);
                    Processor.PC++;
                    cycles = 4;
                    break;
                case 0xA6:  // A = A AND (HL)
                    Processor.A &= Memory.readByte(Processor.HL);
                    Processor.SetFlags((Processor.A == 0) ? 1 : 0, 0, 1, 0);
                    Processor.PC++;
                    cycles = 8;
                    break;
                case 0xE6:  // A = A AND immediate
                    Processor.A &= Memory.Data[Processor.PC + 1];
                    Processor.SetFlags((Processor.A == 0) ? 1 : 0, 0, 1, 0);
                    Processor.PC += 2;
                    cycles = 8;
                    break;

                #endregion



                #region /* Miscellaneous instructions */

                case 0x07:  // Rotate A left
                    value = (byte)((Processor.A & 128) >> 7);    // extract bit 7
                    Processor.A = (byte) ((Processor.A << 1) | value );
                    Processor.SetFlags((Processor.A == 0) ? 1 : 0, 0, 0, value);
                    Processor.PC++;
                    cycles = 4;
                    break;

                case 0x17:  // Rotate A left with carry
                    value = (byte)((Processor.A & 128) >> 7);    // extract bit 7
                    Processor.A = (byte)((byte)(Processor.A << 1) | (byte)(Processor.CarryFlag));
                    Processor.SetFlags((Processor.A == 0) ? 1 : 0, 0, 0, value);
                    Processor.PC++;
                    cycles = 4;
                    break;

                case 0x0F:  // Rotate A right 
                    value = (byte)(Processor.A & 1);    // extract bit 0
                    Processor.A = (byte)( (value << 7) | (Processor.A >> 1));
                    Processor.SetFlags((Processor.A == 0) ? 1 : 0, 0, 0, value);
                    Processor.PC++;
                    cycles = 4;
                    break;

                case 0x1F:  // Rotate A right with carry
                    value = (byte) (Processor.A & 1);    // extract bit 0
                    Processor.A = (byte)((byte)(Processor.CarryFlag << 7) | (byte)(Processor.A >> 1));
                    Processor.SetFlags((Processor.A == 0) ? 1 : 0, 0, 0, value);
                    Processor.PC++;
                    cycles = 4;
                    break;

                case 0xCB:  // Big Operation! includes rotations, shifts, swaps, set etc. 
                    // check the operand to identify real operation
                    switch (Memory.Data[Processor.PC+1])
                    {
                        case 0x37:  // SWAP A
                            Processor.A = (byte) ((Processor.A << 2) | (Processor.A >> 2));
                            Processor.SetFlags((Processor.A == 0) ? 1 : 0, 0, 0, 0);
                            cycles = 8;
                            break;
                        case 0x30:  // SWAP B
                            Processor.B = (byte)((Processor.B << 2) | (Processor.B >> 2));
                            Processor.SetFlags((Processor.B == 0) ? 1 : 0, 0, 0, 0);
                            cycles = 8;
                            break;
                        case 0x31:  // SWAP C
                            Processor.C = (byte)((Processor.C << 2) | (Processor.C >> 2));
                            Processor.SetFlags((Processor.C == 0) ? 1 : 0, 0, 0, 0);
                            cycles = 8;
                            break;
                        case 0x32:  // SWAP D
                            Processor.D = (byte)((Processor.D << 2) | (Processor.D >> 2));
                            Processor.SetFlags((Processor.D == 0) ? 1 : 0, 0, 0, 0);
                            cycles = 8;
                            break;
                        case 0x33:  // SWAP E
                            Processor.E = (byte)((Processor.E << 2) | (Processor.E >> 2));
                            Processor.SetFlags((Processor.E == 0) ? 1 : 0, 0, 0, 0);
                            cycles = 8;
                            break;
                        case 0x34:  // SWAP H
                            Processor.H = (byte)((Processor.H << 2) | (Processor.H >> 2));
                            Processor.SetFlags((Processor.H == 0) ? 1 : 0, 0, 0, 0);
                            cycles = 8;
                            break;
                        case 0x35:  // SWAP L
                            Processor.L = (byte)((Processor.L << 2) | (Processor.L >> 2));
                            Processor.SetFlags((Processor.L == 0) ? 1 : 0, 0, 0, 0);
                            cycles = 8;
                            break;
                        case 0x36:  // SWAP (HL)
                            value = Memory.readByte(Processor.HL);
                            Memory.writeByte(Processor.HL, (byte) ((value << 2) | (value >> 2)));
                            cycles = 16;
                            break;
                        case 0x07:  // Rotate A left
                            value = (byte)((Processor.A & 128) >> 7);    // extract bit 7
                            Processor.A = (byte)((Processor.A << 1) | value);
                            Processor.SetFlags((Processor.A == 0) ? 1 : 0, 0, 0, value);
                            cycles = 8;
                            break;
                        case 0x00:  // Rotate B left
                            value = (byte)((Processor.B & 128) >> 7);    // extract bit 7
                            Processor.B = (byte)((Processor.B << 1) | value);
                            Processor.SetFlags((Processor.B == 0) ? 1 : 0, 0, 0, value);
                            cycles = 8;
                            break;
                        case 0x01:  // Rotate C left
                            value = (byte)((Processor.C & 128) >> 7);    // extract bit 7
                            Processor.C = (byte)((Processor.C << 1) | value);
                            Processor.SetFlags((Processor.C == 0) ? 1 : 0, 0, 0, value);
                            cycles = 8;
                            break;
                        case 0x02:  // Rotate D left
                            value = (byte)((Processor.D & 128) >> 7);    // extract bit 7
                            Processor.D = (byte)((Processor.D << 1) | value);
                            Processor.SetFlags((Processor.D == 0) ? 1 : 0, 0, 0, value);
                            cycles = 8;
                            break;
                        case 0x03:  // Rotate E left
                            value = (byte)((Processor.E & 128) >> 7);    // extract bit 7
                            Processor.E = (byte)((Processor.E << 1) | value);
                            Processor.SetFlags((Processor.E == 0) ? 1 : 0, 0, 0, value);
                            cycles = 8;
                            break;
                        case 0x04:  // Rotate H left
                            value = (byte)((Processor.H & 128) >> 7);    // extract bit 7
                            Processor.H = (byte)((Processor.H << 1) | value);
                            Processor.SetFlags((Processor.H == 0) ? 1 : 0, 0, 0, value);
                            cycles = 8;
                            break;
                        case 0x05:  // Rotate L left
                            value = (byte)((Processor.L & 128) >> 7);    // extract bit 7
                            Processor.L = (byte)((Processor.L << 1) | value);
                            Processor.SetFlags((Processor.L == 0) ? 1 : 0, 0, 0, value);
                            cycles = 8;
                            break;
                        case 0x06:  // Rotate (HL) left
                            value = Memory.readByte(Processor.HL); 
                            value2 = (byte)((value << 1) | (value >> 7));
                            Memory.writeByte(Processor.HL,value2);
                            Processor.SetFlags((value2 == 0) ? 1 : 0, 0, 0, value >> 7);
                            cycles = 16;
                            break;

                        default:
                            UnknownOperand = true;
                            cycles = 0;
                            Processor.PC -= 2;
                            break;
                    }
                    Processor.PC += 2;
                    break;


                case 0x2F:  // Complement A
                    Processor.A = (byte)~Processor.A;
                    Processor.SubtractFlag = 1;
                    Processor.HalfCarryFlag = 1;
                    Processor.PC++;
                    cycles = 4;
                    break;

                case 0x3F:  // Complement Carry
                    Processor.CarryFlag ^= 1;
                    Processor.SubtractFlag = 0;
                    Processor.HalfCarryFlag = 0;
                    Processor.PC++;
                    cycles = 4;
                    break;

                case 0x37:  // Set Carry
                    Processor.CarryFlag = 1;
                    Processor.SubtractFlag = 0;
                    Processor.HalfCarryFlag = 0;
                    Processor.PC++;
                    cycles = 4;
                    break;

                case 0x00:  // NOP
                    Processor.PC++;
                    cycles = 4;
                    break;

                case 0x76:  // HALT
                    Processor.CPUHalt = true;
                    Processor.PC++;
                    cycles = 4;
                    break;

                case 0x10:  // STOP
                    Processor.PC++;
                    if (Memory.Data[Processor.PC] != 0)   // check if next operand is 0 (has to be!)
                    {
                        UnknownOperand = true;
                        cycles = 0;
                        break;
                    }
                    else
                    {
                        Processor.CPUStop = true;
                        cycles = 4;
                    }
                    break;

                case 0xF3:  // Disable Interrupts (DI)
                    Processor.DIsignaled = 1;  // Interrupts Mode will change after next opcode, so signal it
                    Processor.PC++;
                    cycles = 4;
                    break;

                case 0xFB:  // Enable Interrupts (EI)
                    Processor.EIsignaled = 1;  // same here, will take effect AFTER THE NEXT instruction
                    Processor.PC++;
                    cycles = 4;
                    break;

                #endregion

                // In case OPcode isn't implemented or sth. went wrong, halt emulation 
                default:
                    UnknownOPcode = true;
                    cycles = 0;
                    break;
            }

            return cycles;
        }

        // Push a byte onto the stack
        void StackPush(byte value)
        {
            Processor.SP--;
            Memory.Data[Processor.SP] = value;
        }

        // Get a byte from the stack
        byte StackPop()
        {
            byte v = Memory.Data[Processor.SP];
            Processor.SP++;
            return v;
        }

        // Do a call (CALL + 2B immediate)
        void Call()
        {
            ushort address = (ushort)(Processor.PC + 3);  // we need to remember next instruction
            StackPush((byte)address);           // push lower byte of next instruction address
            StackPush((byte)(address >> 8));    // push higher byte of next instruction address  
            Processor.PC = (ushort)((Memory.Data[Processor.PC + 2] << 8) |
                                      Memory.Data[Processor.PC + 1]); // jump to subroutine
        }

        // Do a reset
        void Reset(ushort to)
        {
            ushort address = (Processor.PC);    // we need to remember this! instruction
            StackPush((byte)address);           // push lower byte of instruction address
            StackPush((byte)(address >> 8));    // push higher byte of instruction address  
            Processor.PC = to;                  // Jump to reset routine
        }

        // Raise an interrupt
        void RaiseIRQ(IRQType type)
        {

            // Set bit in the IF register
            switch (type)
            {
                case IRQType.VBLANK:
                    Memory.Data[(int)CMemory.HardwareRegisters.IF] |= 0x01;
                    break;

                case IRQType.LCDC:
                    Memory.Data[(int)CMemory.HardwareRegisters.IF] |= 0x02;
                    break;

                case IRQType.TIMER:
                    Memory.Data[(int)CMemory.HardwareRegisters.IF] |= 0x04;
                    break;

                case IRQType.SERIALIO:
                    Memory.Data[(int)CMemory.HardwareRegisters.IF] |= 0x08;
                    break;

                case IRQType.HIGHTOLOW:
                    Memory.Data[(int)CMemory.HardwareRegisters.IF] |= 0x10;
                    break;
            }
        }

        // Handle waiting interrupts
        void DoIdleIRQs()
        {
            // Check if corresponding IE and IF flag is set. If yes:
            // 1. Reset IME flag
            // 2. Reset bit in IF register
            // 3. Reset jump to interrupt routine

            // VBLANK
            if (((Memory.Data[(int)CMemory.HardwareRegisters.IE] & 0x01) > 0) &&
                ((Memory.Data[(int)CMemory.HardwareRegisters.IF] & 0x01) > 0))
            {
                Processor.IME = false;
                Memory.Data[(int)CMemory.HardwareRegisters.IF] &= 0xFE;
                Reset(0x0040);
                ClockCyclesElapsed += 12; // correct cycles?
                LYCounter += 12;
                MachineCyclesElapsed++;
                
            }
            // LCD STAT
            if (((Memory.Data[(int)CMemory.HardwareRegisters.IE] & 0x02) > 0) &&
                ((Memory.Data[(int)CMemory.HardwareRegisters.IF] & 0x02) > 0))
            {
                Processor.IME = false;
                Memory.Data[(int)CMemory.HardwareRegisters.IF] &= 0xFD;
                Reset(0x0048);
                ClockCyclesElapsed += 12; // correct cycles?
                LYCounter += 12;
                MachineCyclesElapsed++;
            }
            // TIMER
            if (((Memory.Data[(int)CMemory.HardwareRegisters.IE] & 0x04) > 0) &&
                ((Memory.Data[(int)CMemory.HardwareRegisters.IF] & 0x04) > 0))
            {
                Processor.IME = false;
                Memory.Data[(int)CMemory.HardwareRegisters.IF] &= 0xFB;
                Reset(0x0050);
                ClockCyclesElapsed += 12; // correct cycles?
                LYCounter += 12;
                MachineCyclesElapsed++;
            }
            // SERIALIO
            if (((Memory.Data[(int)CMemory.HardwareRegisters.IE] & 0x08) > 0) &&
                ((Memory.Data[(int)CMemory.HardwareRegisters.IF] & 0x08) > 0))
            {
                Processor.IME = false;
                Memory.Data[(int)CMemory.HardwareRegisters.IF] &= 0xF7;
                Reset(0x0058);
                ClockCyclesElapsed += 12; // correct cycles?
                LYCounter += 12;
                MachineCyclesElapsed++;
            }
            // INPUT
            if (((Memory.Data[(int)CMemory.HardwareRegisters.IE] & 0x10) > 0) &&
                ((Memory.Data[(int)CMemory.HardwareRegisters.IF] & 0x10) > 0))
            {
                Processor.IME = false;
                Memory.Data[(int)CMemory.HardwareRegisters.IF] &= 0xEF;
                Reset(0x0060);
                ClockCyclesElapsed += 12; // correct cycles?
                LYCounter += 12;
                MachineCyclesElapsed++;
            } 
        }



 
    }
}