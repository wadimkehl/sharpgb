using System;
using System.Collections.Generic;
using System.Text;

namespace sharpGB
{



    public class CDisassembler
    {


        public CDisassembler()
        {

        }

        public int GetOperationByteCount(ref CMemory Memory, int address)
        {

            switch (Memory.Data[address])
            {

                #region   /* Load instructions */


                // immediate loads
                case 0x3E:  // A <- immediate  
                case 0x06:  // B <- immediate  
                case 0x0E:  // C <- immediate  
                case 0x16:  // D <- immediate  
                case 0x1E:  // E <- immediate  
                case 0x26:  // H <- immediate  
                case 0x2E:  // L <- immediate  
                    return 2;

                case 0x01:  // BC <- immediate  
                case 0x11:  // DE <- immediate  
                case 0x21:  // HL <- immediate  
                case 0x31:  // SP <- immediate
                    return 3;

                case 0x36:  // (HL) <- immediate  
                    return 2;



                // memory to register transfer
                case 0xF2:  // A <- (0xFF00 + C)
                case 0x0A:  // A <- (BC)
                case 0x1A:  // A <- (DE)
                case 0x7E:  // A <- (HL) 
                case 0x46:  // B <- (HL) 
                case 0x4E:  // C <- (HL) 
                case 0x56:  // D <- (HL) 
                case 0x5E:  // E <- (HL) 
                case 0x66:  // H <- (HL) 
                case 0x6E:  // L <- (HL) 
                    return 1;
                case 0x2A:  // A <- (HL), HL++ 
                    return 1;
                case 0xFA:  // A <- (nn immediate) 
                    return 3;
                case 0xF0:  // A <- (0xFF00+ n immediate)
                    return 2;

                // register to memory transfer
                case 0xE2:  // (0xFF00 + C) <- A
                case 0x02:  // (BC) <- A 
                case 0x12:  // (DE) <- A 
                case 0x77:  // (HL) <- A 
                case 0x70:  // (HL) <- B 
                case 0x71:  // (HL) <- C 
                case 0x72:  // (HL) <- D 
                case 0x73:  // (HL) <- E 
                case 0x74:  // (HL) <- H 
                case 0x75:  // (HL) <- L 
                    return 1;
                case 0xEA:  // (nn) <- A 
                    return 3;
                case 0xE0:  // (0xFF00+ n immediate) <- A  
                    return 2;
                case 0x32:  // (HL) <- A, HL--   
                case 0x22:  // (HL) <- A, HL++    
                    return 1;
                case 0x08:  // (nn) <- SP
                    return 3;

                // register to register transfer
                case 0x7F:  // A <- A 
                case 0x78:  // A <- B 
                case 0x79:  // A <- C 
                case 0x7A:  // A <- D 
                case 0x7B:  // A <- E 
                case 0x7C:  // A <- H 
                case 0x7D:  // A <- L 
                case 0x47:  // B <- A 
                case 0x40:  // B <- B 
                case 0x41:  // B <- C 
                case 0x42:  // B <- D 
                case 0x43:  // B <- E 
                case 0x44:  // B <- H 
                case 0x45:  // B <- L 
                case 0x4F:  // C <- A 
                case 0x48:  // C <- B 
                case 0x49:  // C <- C 
                case 0x4A:  // C <- D 
                case 0x4B:  // C <- E 
                case 0x4C:  // C <- H 
                case 0x4D:  // C <- L 
                case 0x57:  // D <- A 
                case 0x50:  // D <- B 
                case 0x51:  // D <- C 
                case 0x52:  // D <- D 
                case 0x53:  // D <- E 
                case 0x54:  // D <- H 
                case 0x55:  // D <- L 
                case 0x5F:  // E <- A 
                case 0x58:  // E <- B 
                case 0x59:  // E <- C 
                case 0x5A:  // E <- D 
                case 0x5B:  // E <- E 
                case 0x5C:  // E <- H 
                case 0x5D:  // E <- L 
                case 0x67:  // H <- A 
                case 0x60:  // H <- B 
                case 0x61:  // H <- C 
                case 0x62:  // H <- D 
                case 0x63:  // H <- E 
                case 0x64:  // H <- H 
                case 0x65:  // H <- L 
                case 0x6F:  // L <- A 
                case 0x68:  // L <- B 
                case 0x69:  // L <- C 
                case 0x6A:  // L <- D 
                case 0x6B:  // L <- E 
                case 0x6C:  // L <- H 
                case 0x6D:  // L <- L 
                case 0xF9:  // SP <- HL 
                    return 1;

                // Stack OPS
                case 0xF5:  // PUSH AF
                case 0xC5:  // PUSH BC
                case 0xD5:  // PUSH DE
                case 0xE5:  // PUSH HL
                case 0xF1:  // POP AF
                case 0xC1:  // POP BC
                case 0xD1:  // POP DE
                case 0xE1:  // POP HL
                    return 1;

                #endregion


                #region /* Arithmetic instructions */

                // 8-bit arithmetics
                // ADD
                case 0x87:  // A = A+A  
                case 0x80:  // A = A+B  
                case 0x81:  // A = A+C  
                case 0x82:  // A = A+D  
                case 0x83:  // A = A+E  
                case 0x84:  // A = A+H  
                case 0x85:  // A = A+L  
                case 0x86:  // A = A+(HL)
                    return 1;
                case 0xC6:  // A = A + immediate  
                    return 2;

                // ADC
                case 0x8F:  // A = A+A+Carry  
                case 0x88:  // A = A+B+Carry 
                case 0x89:  // A = A+C+Carry  
                case 0x8A:  // A = A+D+Carry  
                case 0x8B:  // A = A+E+Carry  
                case 0x8C:  // A = A+H+Carry  
                case 0x8D:  // A = A+L+Carry  
                case 0x8E:  // A = A+(HL)+Carry  
                    return 1;
                case 0xCE:  // A = A + immediate + Carry  
                    return 2;

                // SUB
                case 0x97:  // A = A-A  
                case 0x90:  // A = A-B  
                case 0x91:  // A = A-C  
                case 0x92:  // A = A-D  
                case 0x93:  // A = A-E  
                case 0x94:  // A = A-H  
                case 0x95:  // A = A-L  
                case 0x96:  // A = A-(HL)  
                    return 1;
                case 0xD6:  // A = A - immediate  
                    return 2;

                // SBC
                case 0x9F:  // A = A-A+Carry  
                case 0x98:  // A = A-B+Carry 
                case 0x99:  // A = A-C+Carry  
                case 0x9A:  // A = A-D+Carry  
                case 0x9B:  // A = A-E+Carry  
                case 0x9C:  // A = A-H+Carry  
                case 0x9D:  // A = A-L+Carry  
                case 0x9E:  // A = A-(HL)+Carry  
                    return 1;

                // INC
                case 0x3C:  // A++  
                case 0x04:  // B++  
                case 0x0C:  // C++  
                case 0x14:  // D++  
                case 0x1C:  // E++  
                case 0x24:  // H++  
                case 0x2C:  // L++  
                case 0x34:  // (HL)++  
                // DEC
                case 0x3D:  // A--  
                case 0x05:  // B--  
                case 0x0D:  // C--  
                case 0x15:  // D--  
                case 0x1D:  // E--  
                case 0x25:  // H--  
                case 0x2D:  // L--  
                case 0x35:  // (HL)--  
                // CMP 
                case 0xBF:  // A == A 
                case 0xB8:  // A == B                                       /* same here */
                case 0xB9:  // A == C                                       /* same here */
                case 0xBA:  // A == D                                       /* same here */
                case 0xBB:  // A == E                                       /* same here */
                case 0xBC:  // A == H                                       /* same here */
                case 0xBD:  // A == L                                       /* same here */
                case 0xBE:  // A == (HL)                                    /* same here */
                    return 1;
                case 0xFE:  // A == immediate                               /* same here */
                    return 2;

                // 16-bit arithmetics

                // ADD
                case 0x09:  // HL += BC                              
                case 0x19:  // HL += DE                              
                case 0x29:  // HL += HL                              
                case 0x39:  // HL += SP 
                    return 1;         
                case 0xE8:  // SP += signed immediate byte                              
                    return 2;

                // INC
                case 0x03:  // BC++
                case 0x13:  // DE++
                case 0x23:  // HL++
                case 0x33:  // SP++
                // DEC
                case 0x0B:  // BC--
                case 0x1B:  // DE--
                case 0x2B:  // HL--
                case 0x3B:  // SP--
                    return 1;

                #endregion


                #region /* Jump instructions */
                // absolute jumps
                case 0xC3:  // Unconditional + 2B immediate operands
                case 0xC2:  // Conditional NZ + 2B immediate operands
                case 0xCA:  // Conditional Z + 2B immediate operands
                case 0xD2:  // Conditional NC + 2B immediate operands
                case 0xDA:  // Conditional C + 2B immediate operands
                    return 3;
                case 0xE9:  // Unconditional jump to (HL)
                    return 1;

                // relative jumps
                case 0x18:  // Unconditional + relative byte
                case 0x20:  // Conditional NZ + relative byte
                case 0x28:  // Conditional Z + relative byte
                case 0x30:  // Conditional NC + relative byte
                case 0x38:  // Conditional C + relative byte
                    return 2;

                // calls
                case 0xCD:  // unconditional 
                case 0xC4:  // Conditional NZ 
                case 0xCC:  // Conditional Z 
                case 0xD4:  // Conditional NC
                case 0xDC:  // Conditional C 
                    return 3;

                // resets
                case 0xC7:
                case 0xCF:
                case 0xD7:
                case 0xDF:
                case 0xE7:
                case 0xEF:
                case 0xF7:
                case 0xFF:
                    return 1;


                // returns
                case 0xC9:  // unconditional
                case 0xD9:  // unconditional plus enable interrupts (RETI)
                case 0xC0:  // Conditional NZ 
                case 0xC8:  // Conditional Z 
                case 0xD0:  // Conditional NC
                case 0xD8:  // Conditional C 
                    return 1;


                #endregion


                #region /* Logical instructions */

                // OR
                case 0xB7:  // A = A OR A = A !!!
                case 0xB0:  // A = A OR B
                case 0xB1:  // A = A OR C
                case 0xB2:  // A = A OR D
                case 0xB3:  // A = A OR E
                case 0xB4:  // A = A OR H
                case 0xB5:  // A = A OR L
                case 0xB6:  // A = A OR (HL)
                    return 1;
                case 0xF6:  // A = A OR immediate
                    return 2;

                // XOR
                case 0xAF:  // A = A XOR A = 0 !!!
                case 0xA8:  // A = A XOR B
                case 0xA9:  // A = A XOR C
                case 0xAA:  // A = A XOR D
                case 0xAB:  // A = A XOR E
                case 0xAC:  // A = A XOR H
                case 0xAD:  // A = A XOR L
                case 0xAE:  // A = A XOR (HL)
                    return 1;
                case 0xEE:  // A = A XOR immediate
                    return 2;

                case 0xA7:  // A = A AND A 
                case 0xA0:  // A = A AND B
                case 0xA1:  // A = A AND C
                case 0xA2:  // A = A AND D
                case 0xA3:  // A = A AND E
                case 0xA4:  // A = A AND H
                case 0xA5:  // A = A AND L
                case 0xA6:  // A = A AND (HL)
                    return 1;
                case 0xE6:  // A = A AND immediate
                    return 2;

                #endregion


                #region /* Miscellaneous instructions */

                case 0x07:  // Rotate A left
                case 0x17:  // Rotate A left with carry
                case 0x0F:  // Rotate A right 
                case 0x1F:  // Rotate A right with carry
                    return 1;


                case 0xCB:  // Swap nibbles + 1B immediate operand
                    return 2;
                case 0x2F:  // Complement A
                case 0x3F:  // Complement Carry
                case 0x37:  // Set Carry
                case 0x00:  // NOP
                case 0x76:  // HALT
                case 0x10:  // STOP
                case 0xF3:  // Disable Interrupts (DI)
                case 0xFB:  // Enable Interrupts (EI)
                    return 1;

                #endregion

                default:
                    return 1;
            }


        }

        public string DisassembleMemoryAddress(ref CMemory Memory, int address)
        {

            // string return value
            string text;
            int temp;
            switch (Memory.Data[address])
            {

                #region   /* Load instructions */


                // immediate loads
                case 0x3E:  // A <- immediate  
                    text = "LD A,0x" + (Memory.Data[address + 1]).ToString("X2");
                    break;

                case 0x06:  // B <- immediate  
                    text = "LD B,0x" + (Memory.Data[address + 1]).ToString("X2");
                    break;

                case 0x0E:  // C <- immediate  
                    text = "LD C,0x" + (Memory.Data[address + 1]).ToString("X2");
                    break;

                case 0x16:  // D <- immediate  
                    text = "LD D,0x" + (Memory.Data[address + 1]).ToString("X2");
                    break;

                case 0x1E:  // E <- immediate  
                    text = "LD E,0x" + (Memory.Data[address + 1]).ToString("X2");
                    break;

                case 0x26:  // H <- immediate  
                    text = "LD H,0x" + (Memory.Data[address + 1]).ToString("X2");
                    break;

                case 0x2E:  // L <- immediate  
                    text = "LD L,0x" + (Memory.Data[address + 1]).ToString("X2");
                    break;

                case 0x01:  // BC <- immediate  
                    text = "LD BC,0x" + (Memory.Data[address + 1] + (Memory.Data[address + 2] << 8)).ToString("X2");
                    break;

                case 0x11:  // DE <- immediate  
                    text = "LD DE,0x" + (Memory.Data[address + 1] + (Memory.Data[address + 2] << 8)).ToString("X2");
                    break;

                case 0x21:  // HL <- immediate  
                    text = "LD HL,0x" + (Memory.Data[address + 1] + (Memory.Data[address + 2] << 8)).ToString("X2");
                    break;

                case 0x31:  // SP <- immediate
                    text = "LD SP,0x" + (Memory.Data[address + 1] + (Memory.Data[address + 2] << 8)).ToString("X2");
                    break;
                
                case 0x36:  // (HL) <- immediate  
                    text = "LD (HL),0x" + (Memory.Data[address + 1]).ToString("X2");
                    break;


                // memory to register transfer
                case 0xF2:    // A <- (0xFF00 + C)
                    text = "LD A,(0xFF00 + C)";
                    break;
                case 0x0A:    // A <- (BC)
                    text = "LD A,(BC)";
                    break;
                case 0x1A:    // A <- (DE)
                    text = "LD A,(DE)";
                    break;
                case 0x7E:  // A <- (HL) 
                    text = "LD A,(HL)";
                    break;
                case 0x46:  // B <- (HL) 
                    text = "LD B,(HL)";
                    break;
                case 0x4E:  // C <- (HL) 
                    text = "LD C,(HL)";
                    break;
                case 0x56:  // D <- (HL) 
                    text = "LD D,(HL)";
                    break;
                case 0x5E:  // E <- (HL) 
                    text = "LD E,(HL)";
                    break;
                case 0x66:  // H <- (HL) 
                    text = "LD H,(HL)";
                    break;
                case 0x6E:  // L <- (HL) 
                    text = "LD L,(HL)";
                    break;
                case 0x2A:  // A <- (HL), HL++ 
                    text = "LDI A,(HL)";
                    break;
                case 0xFA:  // A <- (nn immediate) 
                    text = "LD A,(0x" + (Memory.Data[address + 1] + (Memory.Data[address + 2] << 8)).ToString("X2") + ")";
                    break;
                case 0xF0:  // A <- (0xFF00+ n immediate)
                    temp = 0xFF00 + Memory.Data[address + 1];
                    if (temp > 0xFF4B) text = "LD A,(0x" + temp.ToString("X2") + ")";        // RAM read
                    else if (temp == 0xFFFF) text = "LD A,IE";
                    else text = "LD A," + Enum.GetName(typeof(CMemory.HardwareRegisters), temp); // Give I/O register name
                    break;

                // register to memory transfer
                case 0xE2:    // (0xFF00 + C) <- A
                    text = "LD (FF00 + C),A";
                    break;
                case 0x02:  // (BC) <- A 
                    text = "LD (BC),A";
                    break;
                case 0x12:  // (DE) <- A 
                    text = "LD (DE),A";
                    break;
                case 0x77:  // (HL) <- A 
                    text = "LD (HL),A";
                    break;
                case 0x70:  // (HL) <- B 
                    text = "LD (HL),B";
                    break;
                case 0x71:  // (HL) <- C 
                    text = "LD (HL),C";
                    break;
                case 0x72:  // (HL) <- D 
                    text = "LD (HL),D";
                    break;
                case 0x73:  // (HL) <- E 
                    text = "LD (HL),E";
                    break;
                case 0x74:  // (HL) <- H 
                    text = "LD (HL),H";
                    break;
                case 0x75:  // (HL) <- L 
                    text = "LD (HL),L";
                    break;
                case 0x32:  // (HL) <- A, HL-- 
                    text = "LDD (HL),A";
                    break;
                case 0x22:  // (HL) <- A, HL++ 
                    text = "LDI (HL),A";
                    break;
                case 0xEA:  // (nn) <- A 
                    text = "LD (0x" + (Memory.Data[address + 1] + (Memory.Data[address + 2] << 8)).ToString("X2") + "),A";
                    break;
                case 0xE0:  // (0xFF00+ n immediate) <- A  
                    temp = 0xFF00 + Memory.Data[address + 1];
                    if (temp > 0xFF4B) text = "LD (0x" + temp.ToString("X2") + "),A";        // RAM write
                    else if (temp == 0xFFFF) text = "LD IE,A";
                    else text = "LD " + Enum.GetName(typeof(CMemory.HardwareRegisters), temp) + ",A"; // Give I/O register name
                    break;
                case 0x08:  // (nn) <- SP
                    text = "LD (0x" + (Memory.Data[address + 1] + (Memory.Data[address + 2] << 8)).ToString("X2") + "),SP";
                    break;

                // register to register transfer
                case 0x7F:  // A <- A  
                    text = "LD A,A";
                    break;
                case 0x78:  // A <- B 
                    text = "LD A,B";
                    break;
                case 0x79:  // A <- C 
                    text = "LD A,C";
                    break;
                case 0x7A:  // A <- D 
                    text = "LD A,D";
                    break;
                case 0x7B:  // A <- E 
                    text = "LD A,E";
                    break;
                case 0x7C:  // A <- H 
                    text = "LD A,H";
                    break;
                case 0x7D:  // A <- L 
                    text = "LD A,L";
                    break;
                case 0x47:  // B <- A 
                    text = "LD B,A";
                    break;
                case 0x40:  // B <- B 
                    text = "LD B,B";
                    break;
                case 0x41:  // B <- C 
                    text = "LD B,C ";
                    break;
                case 0x42:  // B <- D 
                    text = "LD B,D";
                    break;
                case 0x43:  // B <- E 
                    text = "LD B,E";
                    break;
                case 0x44:  // B <- H 
                    text = "LD B,H";
                    break;
                case 0x45:  // B <- L 
                    text = "LD B,L";
                    break;
                case 0x4F:  // C <- A 
                    text = "LD C,A";
                    break;
                case 0x48:  // C <- B 
                    text = "LD C,B";
                    break;
                case 0x49:  // C <- C 
                    text = "LD C,C";
                    break;
                case 0x4A:  // C <- D 
                    text = "LD C,D";
                    break;
                case 0x4B:  // C <- E 
                    text = "LD C,E";
                    break;
                case 0x4C:  // C <- H 
                    text = "LD C,H";
                    break;
                case 0x4D:  // C <- L 
                    text = "LD C,L";
                    break;
                case 0x57:  // D <- A 
                    text = "LD D,A ";
                    break;
                case 0x50:  // D <- B 
                    text = "LD D,B";
                    break;
                case 0x51:  // D <- C 
                    text = "LD D,C";
                    break;
                case 0x52:  // D <- D 
                    text = "LD D,D";
                    break;
                case 0x53:  // D <- E 
                    text = "LD D,E";
                    break;
                case 0x54:  // D <- H 
                    text = "LD D,H";
                    break;
                case 0x55:  // D <- L 
                    text = "LD D,L";
                    break;
                case 0x5F:  // E <- A 
                    text = "LD E,A";
                    break;
                case 0x58:  // E <- B 
                    text = "LD E,B";
                    break;
                case 0x59:  // E <- C 
                    text = "LD E,C";
                    break;
                case 0x5A:  // E <- D 
                    text = "LD E,D";
                    break;
                case 0x5B:  // E <- E 
                    text = "LD E,E";
                    break;
                case 0x5C:  // E <- H 
                    text = "LD E,H";
                    break;
                case 0x5D:  // E <- L 
                    text = "LD E,L";
                    break;
                case 0x67:  // H <- A 
                    text = "LD H,A";
                    break;
                case 0x60:  // H <- B 
                    text = "LD H,B";
                    break;
                case 0x61:  // H <- C 
                    text = "LD H,C";
                    break;
                case 0x62:  // H <- D 
                    text = "LD H,D";
                    break;
                case 0x63:  // H <- E 
                    text = "LD H,E";
                    break;
                case 0x64:  // H <- H 
                    text = "LD H,H";
                    break;
                case 0x65:  // H <- L 
                    text = "LD H,L";
                    break;
                case 0x6F:  // L <- A 
                    text = "LD L,A";
                    break;
                case 0x68:  // L <- B 
                    text = "LD L,B";
                    break;
                case 0x69:  // L <- C 
                    text = "LD L,C";
                    break;
                case 0x6A:  // L <- D 
                    text = "LD L,D";
                    break;
                case 0x6B:  // L <- E 
                    text = "LD L,E";
                    break;
                case 0x6C:  // L <- H 
                    text = "LD L,H";
                    break;
                case 0x6D:  // L <- L 
                    text = "LD L,L";
                    break;
                case 0xF9:  // SP <- HL 
                    text = "LD SP,HL";
                    break;

                // STACK OPS
                case 0xF5:  // PUSH AF
                    text = "PUSH AF";
                    break;
                case 0xC5:  // PUSH BC
                    text = "PUSH BC";
                    break;
                case 0xD5:  // PUSH DE
                    text = "PUSH DE";
                    break;
                case 0xE5:  // PUSH HL
                    text = "PUSH HL";
                    break;
                case 0xF1:  // POP AF
                    text = "POP AF";
                    break;
                case 0xC1:  // POP BC
                    text = "POP BC";
                    break;
                case 0xD1:  // POP DE
                    text = "POP DE";
                    break;
                case 0xE1:  // POP HL
                    text = "POP HL";
                    break;

                #endregion


                #region /* Arithmetic instructions */

                // 8-bit arithmetics
                // ADD
                case 0x87:  // A = A+A  
                    text = "ADD A,A";
                    break;
                case 0x80:  // A = A+B  
                    text = "ADD A,B";
                    break;
                case 0x81:  // A = A+C  
                    text = "ADD A,C";
                    break;
                case 0x82:  // A = A+D  
                    text = "ADD A,D";
                    break;
                case 0x83:  // A = A+E  
                    text = "ADD A,E";
                    break;
                case 0x84:  // A = A+H  
                    text = "ADD A,H";
                    break;
                case 0x85:  // A = A+L  
                    text = "ADD A,L";
                    break;
                case 0x86:  // A = A+(HL)  
                    text = "ADD A,(HL)";
                    break;
                case 0xC6:  // A = A + immediate  
                    text = "ADD A,0x" + Memory.Data[address + 1].ToString("X2");
                    break;

                // ADC
                case 0x8F:  // A = A+A+Carry  
                    text = "ADC A,A";
                    break;
                case 0x88:  // A = A+B+Carry 
                    text = "ADC A,B";
                    break;
                case 0x89:  // A = A+C+Carry  
                    text = "ADC A,C";
                    break;
                case 0x8A:  // A = A+D+Carry  
                    text = "ADC A,D";
                    break;
                case 0x8B:  // A = A+E+Carry  
                    text = "ADC A,E";
                    break;
                case 0x8C:  // A = A+H+Carry  
                    text = "ADC A,H";
                    break;
                case 0x8D:  // A = A+L+Carry  
                    text = "ADC A,L";
                    break;
                case 0x8E:  // A = A+(HL)+Carry  
                    text = "ADC A,(HL)";
                    break;
                case 0xCE:  // A = A + immediate + Carry  
                    text = "ADC A,0x" + Memory.Data[address + 1].ToString("X2");
                    break;

                // SUB
                case 0x97:  // A = A-A  
                    text = "SUB A,A";
                    break;
                case 0x90:  // A = A-B  
                    text = "SUB A,B";
                    break;
                case 0x91:  // A = A-C  
                    text = "SUB A,C";
                    break;
                case 0x92:  // A = A-D  
                    text = "SUB A,D";
                    break;
                case 0x93:  // A = A-E  
                    text = "SUB A,E";
                    break;
                case 0x94:  // A = A-H  
                    text = "SUB A,H";
                    break;
                case 0x95:  // A = A-L  
                    text = "SUB A,L";
                    break;
                case 0x96:  // A = A-(HL)  
                    text = "SUB A,(HL)";
                    break;
                case 0xD6:  // A = A - immediate  
                    text = "SUB A,0x" + Memory.Data[address + 1].ToString("X2");
                    break;

                // SBC
                case 0x9F:  // A = A-A+Carry  
                    text = "SBC A,A";
                    break;
                case 0x98:  // A = A-B+Carry 
                    text = "SBC A,B";
                    break;
                case 0x99:  // A = A-C+Carry  
                    text = "SBC A,C";
                    break;
                case 0x9A:  // A = A-D+Carry  
                    text = "SBC A,D";
                    break;
                case 0x9B:  // A = A-E+Carry  
                    text = "SBC A,E";
                    break;
                case 0x9C:  // A = A-H+Carry  
                    text = "SBC A,H";
                    break;
                case 0x9D:  // A = A-L+Carry  
                    text = "SBC A,L";
                    break;
                case 0x9E:  // A = A-(HL)+Carry  
                    text = "SBC A,(HL)";
                    break;

                // INC
                case 0x3C:  // A++  
                    text = "INC A";
                    break;
                case 0x04:  // B++  
                    text = "INC B";
                    break;
                case 0x0C:  // C++  
                    text = "INC C";
                    break;
                case 0x14:  // D++  
                    text = "INC D";
                    break;
                case 0x1C:  // E++  
                    text = "INC E";
                    break;
                case 0x24:  // H++  
                    text = "INC H";
                    break;
                case 0x2C:  // L++  
                    text = "INC L";
                    break;
                case 0x34:  // (HL)++  
                    text = "INC (HL)";
                    break;
                
                // DEC
                case 0x3D:  // A--  
                    text = "DEC A";
                    break;
                case 0x05:  // B--  
                    text = "DEC B";
                    break;
                case 0x0D:  // C--  
                    text = "DEC C";
                    break;
                case 0x15:  // D--  
                    text = "DEC D";
                    break;
                case 0x1D:  // E--  
                    text = "DEC E";
                    break;
                case 0x25:  // H--  
                    text = "DEC H";
                    break;
                case 0x2D:  // L--  
                    text = "DEC L";
                    break;
                case 0x35:  // (HL)--  
                    text = "DEC (HL)";
                    break;

                // CMP 
                case 0xBF:  // A == A 
                    text = "CMP A,A";
                    break;
                case 0xB8:  // A == B 
                    text = "CMP A,B";
                    break;
                case 0xB9:  // A == C 
                    text = "CMP A,C";
                    break;
                case 0xBA:  // A == D  
                    text = "CMP A,D";
                    break;
                case 0xBB:  // A == E  
                    text = "CMP A,E";
                    break;
                case 0xBC:  // A == H   
                    text = "CMP A,H";
                    break;
                case 0xBD:  // A == L      
                    text = "CMP A,L";
                    break;
                case 0xBE:  // A == (HL)  
                    text = "CMP A,(HL)";
                    break;
                case 0xFE:  // A == immediate  
                    text = "CMP A,0x" + (Memory.Data[address + 1]).ToString("X2");
                    break;

                // 16-bit arithmetics

                // ADD
                case 0x09:  // HL += BC                              
                    text = "ADD HL,BC";
                    break;
                case 0x19:  // HL += DE                              
                    text = "ADD HL,DE";
                    break;
                case 0x29:  // HL += HL                              
                    text = "ADD HL,HL";
                    break;
                case 0x39:  // HL += SP                              
                    text = "ADD HL,SP";
                    break;
                case 0xE8:  // SP += signed immediate byte                              
                    text = "ADD SP," + ((sbyte) Memory.Data[address + 1]).ToString();
                    break;

                // INC
                case 0x03:  // BC++
                    text = "INC BC";
                    break;
                case 0x13:  // DE++
                    text = "INC DE";
                    break;
                case 0x23:  // HL++
                    text = "INC HL";
                    break;
                case 0x33:  // SP++
                    text = "INC SP";
                    break;

                // DEC
                case 0x0B:  // BC--
                    text = "DEC BC";
                    break;
                case 0x1B:  // DE--
                    text = "DEC DE";
                    break;
                case 0x2B:  // HL--
                    text = "DEC HL";
                    break;
                case 0x3B:  // SP--
                    text = "DEC SP";
                    break;

                #endregion


                #region /* Jump instructions */
                // absolute jumps
                case 0xC3:  // Unconditional + 2B immediate operands
                    text = "JMP 0x" + (Memory.Data[address + 1] + (Memory.Data[address + 2] << 8)).ToString("X2");
                    break;
                case 0xC2:  // Conditional NZ + 2B immediate operands
                    text = "JNZ 0x" + (Memory.Data[address + 1] + (Memory.Data[address + 2] << 8)).ToString("X2");
                    break;
                case 0xCA:  // Conditional Z + 2B immediate operands
                    text = "JZ 0x" + (Memory.Data[address + 1] + (Memory.Data[address + 2] << 8)).ToString("X2");
                    break;
                case 0xD2:  // Conditional NC + 2B immediate operands
                    text = "JNC 0x" + (Memory.Data[address + 1] + (Memory.Data[address + 2] << 8)).ToString("X2");
                    break;
                case 0xDA:  // Conditional C + 2B immediate operands
                    text = "JC 0x" + (Memory.Data[address + 1] + (Memory.Data[address + 2] << 8)).ToString("X2");
                    break;
                case 0xE9:  // Unconditional jump to HL
                    text = "JMP HL";
                    break;

                // relative jumps
                case 0x18:  // Unconditional + relative byte
                    text = "JMP 0x" + (address + (sbyte)Memory.Data[address + 1] + 2).ToString("X2");
                    break;
                case 0x20:  // Conditional NZ + relative byte
                    text = "JNZ 0x" + (address + (sbyte)Memory.Data[address + 1] + 2).ToString("X2");
                    break;
                case 0x28:  // Conditional Z + relative byte
                    text = "JZ 0x" + (address + (sbyte)Memory.Data[address + 1] + 2).ToString("X2");
                    break;
                case 0x30:  // Conditional NC + relative byte
                    text = "JNC 0x" + (address + (sbyte)Memory.Data[address + 1] + 2).ToString("X2");
                    break;
                case 0x38:  // Conditional C + relative byte
                    text = "JC 0x" + (address + (sbyte)Memory.Data[address + 1] + 2).ToString("X2");
                    break;

                // calls
                case 0xCD:  // unconditional 
                    text = "CALL 0x" + (Memory.Data[address + 1] + (Memory.Data[address + 2] << 8)).ToString("X2");
                    break;
                case 0xC4:  // Conditional NZ 
                    text = "CALLNZ 0x" + (Memory.Data[address + 1] + (Memory.Data[address + 2] << 8)).ToString("X2");
                    break;
                case 0xCC:  // Conditional Z 
                    text = "CALLZ 0x" + (Memory.Data[address + 1] + (Memory.Data[address + 2] << 8)).ToString("X2");
                    break;
                case 0xD4:  // Conditional NC
                    text = "CALLNC 0x" + (Memory.Data[address + 1] + (Memory.Data[address + 2] << 8)).ToString("X2");
                    break;
                case 0xDC:  // Conditional C 
                    text = "CALLC 0x" + (Memory.Data[address + 1] + (Memory.Data[address + 2] << 8)).ToString("X2");
                    break;

                // resets
                case 0xC7:
                    text = "RST 00"; 
                    break;
                case 0xCF:
                    text = "RST 08";
                    break;
                case 0xD7:
                    text = "RST 10";
                    break;
                case 0xDF:
                    text = "RST 18";
                    break;
                case 0xE7:
                    text = "RST 20";
                    break;
                case 0xEF:
                    text = "RST 28";
                    break;
                case 0xF7:
                    text = "RST 30";
                    break;
                case 0xFF:
                    text = "RST 38";
                    break;

                // returns
                case 0xC9:  // unconditional
                    text = "RET";
                    break;
                case 0xD9:  // unconditional plus enable interrupts (RETI)
                    text = "RETI";
                    break;
                case 0xC0:  // Conditional NZ 
                    text = "RETNZ";
                    break;
                case 0xC8:  // Conditional Z 
                    text = "RETZ";
                    break;
                case 0xD0:  // Conditional NC
                    text = "RETNC";
                    break;
                case 0xD8:  // Conditional C 
                    text = "RETC";
                    break;


                #endregion


                #region /* Logical instructions */

                // OR
                case 0xB7:  // A = A OR A = A !!!
                    text = "A OR A";
                    break;
                case 0xB0:  // A = A OR B
                    text = "A OR B";
                    break;
                case 0xB1:  // A = A OR C
                    text = "A OR C";
                    break;
                case 0xB2:  // A = A OR D
                    text = "A OR D";
                    break;
                case 0xB3:  // A = A OR E
                    text = "A OR E";
                    break;
                case 0xB4:  // A = A OR H
                    text = "A OR H";
                    break;
                case 0xB5:  // A = A OR L
                    text = "A OR L";
                    break;
                case 0xB6:  // A = A OR (HL)
                    text = "A OR (HL)";
                    break;
                case 0xF6:  // A = A OR immediate
                    text = "A OR 0x" + (Memory.Data[address + 1]).ToString("X2");
                    break;

                // XOR
                case 0xAF:  // A = A XOR A = 0 !!!
                    text = "A XOR A";
                    break;
                case 0xA8:  // A = A XOR B
                    text = "A XOR B";
                    break;
                case 0xA9:  // A = A XOR C
                    text = "A XOR C";
                    break;
                case 0xAA:  // A = A XOR D
                    text = "A XOR D";
                    break;
                case 0xAB:  // A = A XOR E
                    text = "A XOR E";
                    break;
                case 0xAC:  // A = A XOR H
                    text = "A XOR H";
                    break;
                case 0xAD:  // A = A XOR L
                    text = "A XOR L";
                    break;
                case 0xAE:  // A = A XOR (HL)
                    text = "A XOR (HL)";
                    break;
                case 0xEE:  // A = A XOR immediate
                    text = "A XOR 0x" + (Memory.Data[address + 1]).ToString("X2");
                    break;

                // AND
                case 0xA7:  // A = A AND A 
                    text = "A AND A";
                    break;
                case 0xA0:  // A = A AND B
                    text = "A AND B";
                    break;
                case 0xA1:  // A = A AND C
                    text = "A AND C";
                    break;
                case 0xA2:  // A = A AND D
                    text = "A AND D";
                    break;
                case 0xA3:  // A = A AND E
                    text = "A AND E";
                    break;
                case 0xA4:  // A = A AND H
                    text = "A AND H";
                    break;
                case 0xA5:  // A = A AND L
                    text = "A AND L";
                    break;
                case 0xA6:  // A = A AND (HL)
                    text = "A AND (HL)";
                    break;
                case 0xE6:  // A = A AND immediate
                    text = "A AND 0x" + (Memory.Data[address + 1]).ToString("X2");
                    break;


                #endregion


                #region /* Miscellaneous instructions */

                case 0x07:  // Rotate A left
                    text = "RLC A";
                    break;
                case 0x17:  // Rotate A left with carry
                    text = "RL A";
                    break;
                case 0x0F:  // Rotate A right 
                    text = "RRC A";
                    break;
                case 0x1F:  // Rotate A right with carry
                    text = "RR A";
                    break;


                case 0xCB:  // Big Operation! includes rotations, shifts, swaps, set etc. 
                    // check the operand to identify real operation
                    switch (Memory.Data[address+1])
                    {
                        case 0x37:  //SWAP  A
                            text = "SWAP A";
                            break;
                        case 0x30:  //SWAP  B
                            text = "SWAP B";
                            break;
                        case 0x31:  //SWAP  C
                            text = "SWAP C";
                            break;
                        case 0x32:  //SWAP  D
                            text = "SWAP D";
                            break;
                        case 0x33:  //SWAP  E
                            text = "SWAP E";
                            break;
                        case 0x34:  //SWAP  H
                            text = "SWAP H";
                            break;
                        case 0x35:  //SWAP  L
                            text = "SWAP L";
                            break;
                        case 0x36:  //SWAP  (HL)
                            text = "SWAP (HL)";
                            break;
                        default:
                            text = "CB ?";
                            break;
                    }
                    break;

                case 0x2F:  // Complement A
                    text = "CPL";
                    break;
                case 0x3F:  // Complement Carry
                    text = "CCF";
                    break;
                case 0x37:  // Set Carry
                    text = "SCF";
                    break;
                case 0x00:  // NOP
                    text = "NOP";
                    break;
                case 0x76:  // HALT
                    text = "HALT";
                    break;
                case 0x10:  // STOP
                    text = "STOP";
                    break;
                case 0xF3:  // Disable Interrupts (DI)
                    text = "DI";
                    break;
                case 0xFB:  // Enable Interrupts (EI)
                    text = "EI";
                    break;

                #endregion

                default:
                    text = "UNKNOWN";
                    break;

                }

            return text;
        }

        public string AdditionalAddressInformation(int address)
        {
            switch (address)
            {
                case 0x00:
                    return "#RST 00";                 
                case 0x08:
                    return "#RST 08";                   
                case 0x10:
                    return "#RST 10";                   
                case 0x18:
                    return "#RST 18";               
                case 0x20:
                    return "#RST 20";                    
                case 0x28:
                    return "#RST 28";                    
                case 0x30:
                    return "#RST 30";                  
                case 0x38:
                    return "#RST 38";                  
                case 0x40:
                    return "#VBLANK INT";                   
                case 0x48:
                    return "#LCDC INT";
                case 0x50:
                    return "#TIMER INT";
                case 0x58:
                    return "#SERIAL INT";
                case 0x60:
                    return "#INPUT INT";
                case 0x100:
                    return "#ROM ENTRY";
                case 0x4000:
                    return "#ROM BANK X";
                case 0x8000:
                    return "#TILE DATA 0";
                case 0x8800:
                    return "#TILE DATA 1";
                case 0x9800:
                    return "#TILE MAP 0";
                case 0x9C00:
                    return "#TILE MAP 1";
                case 0xA000:
                    return "#GAME RAM BANK";
                case 0xC000:
                    return "#RAM BANK 0";
                case 0xD000:
                    return "#RAM BANK 1-7";
                case 0xFE00:
                    return "#OAM";
                case 0xFF80:
                    return "#INTERNAL RAM";
            }
            return "";
        }
        
    
    
    }

    


}
