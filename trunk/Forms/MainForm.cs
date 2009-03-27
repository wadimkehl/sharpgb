using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace sharpGB
{
    public partial class MainForm : Form
    {
        RomHeaderForm RHF;


        public MainForm()
        {
            InitializeComponent();
            Size = new Size(285, 320);
            Program.Emulator.Video.SetDrawDestination(ref this.VideoBox);
        }

        private void openROMToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if (!Program.Emulator.LoadROMFromFile(openFileDialog1.FileName))
                    MessageBox.Show("Couldn't open specified ROM");

                if (!Program.Emulator.ROM.LogoOK)
                    MessageBox.Show("Nintendo logo data wrong. Gameboy would hang up here");

                if (!Program.Emulator.ROM.ComplementOK)
                    MessageBox.Show("Complement wrong. Gameboy would hang up here");
                /*
                if (!Program.Emulator.ROM.CheckSumOK)
                    MessageBox.Show("Checksum wrong");  // Checksum doesn't count
                */

                Size = new Size(620, 620);
                CenterToScreen();
                UpdateInfoBox();
                UpdateBreakpointBox();
                Thread LoadHexBoxThread = new Thread(new ThreadStart(DrawHexBox));
                LoadHexBoxThread.Start();

            }
           
        }
        private void headerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RHF = new RomHeaderForm();
            RHF.Icon = Icon;
            RHF.Show();
        }
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        private void RunStepsButton_Click(object sender, EventArgs e)
        {
            Program.Emulator.EmulateSteps( Int32.Parse(this.RunBox.Text));
            Program.Emulator.ShowError();
            UpdateInfoBox();
            if (FollowCodeCheckBox.Checked) JumpToAddress(Program.Emulator.Processor.PC);

        }
        private void RunButton_Click(object sender, EventArgs e)
        {
            Program.Emulator.EmulateFully();
            Program.Emulator.ShowError();
            UpdateInfoBox();
            if (FollowCodeCheckBox.Checked) JumpToAddress(Program.Emulator.Processor.PC);
        }
        private void AddBreakPoint_Click(object sender, EventArgs e)
        {
            Program.Emulator.AddBreakPoint(Int32.Parse(BreakPointAddressTextBox.Text, System.Globalization.NumberStyles.HexNumber));
            UpdateBreakpointBox();  
        }
        private void RemoveBreakPoint_Click(object sender, EventArgs e)
        {
            Program.Emulator.RemoveBreakPoint(Int32.Parse(BreakPointAddressTextBox.Text, System.Globalization.NumberStyles.HexNumber));
            UpdateBreakpointBox();  
        }
        private void JumpToButton_Click(object sender, EventArgs e)
        {
            JumpToAddress(Int32.Parse(JumpToTextBox.Text, System.Globalization.NumberStyles.HexNumber));
        }
        private void ResetMenuItem_Click(object sender, EventArgs e)
        {
            Program.Emulator.Reset();
            UpdateInfoBox();
            UpdateBreakpointBox();
            if (FollowCodeCheckBox.Checked) JumpToAddress(Program.Emulator.Processor.PC);
        }


        private delegate void ClearDelegate();
        private delegate void AppendTextDelegate(string text);
        private unsafe void DrawHexBox()
        {
            HexBox.Invoke(new ClearDelegate(HexBox.Clear));
            string address, opcode,mnemonic, info;
            int i = 0, j = 0, k=0;
            while (i < 0xFFFF)
            {
                address = "0x" + i.ToString("X4");
                if (i >= 0x0104 && i <= 0x014F)
                {
                    opcode =  Program.Emulator.Memory.Data[i].ToString("X2");
                    HexBox.Invoke(new AppendTextDelegate(HexBox.AppendText),String.Format("{0} {1,-10}\n", address, opcode));
                    Program.Emulator.Memory.AddressToIndex[i] = k;
                    i++;
                }
                else if (i >= 0x8000 && i < 0xA000)
                {
                    opcode = Program.Emulator.Memory.Data[i].ToString("X2");
                    info = Program.Emulator.Disassembler.AdditionalAddressInformation(i);
                    HexBox.Invoke(new AppendTextDelegate(HexBox.AppendText), String.Format("{0} {1,-10} {2} \n", address, opcode, info));
                    Program.Emulator.Memory.AddressToIndex[i] = k;
                    i++;
                }
                else
                {
                    opcode = "";
                    for (j = 0; j < Program.Emulator.Disassembler.GetOperationByteCount(ref Program.Emulator.Memory, i); j++)
                    {
                        opcode += Program.Emulator.Memory.Data[i + j].ToString("X2") + " ";
                        Program.Emulator.Memory.AddressToIndex[i + j] = k;
                    }
                    mnemonic = Program.Emulator.Disassembler.DisassembleMemoryAddress(ref Program.Emulator.Memory, i);
                    info = Program.Emulator.Disassembler.AdditionalAddressInformation(i);

                    Program.Emulator.Memory.AddressToIndex[i + j] = k;

                    HexBox.Invoke(new AppendTextDelegate(HexBox.AppendText), String.Format("{0} {1,-10} {2,-15} {3}\n", address, opcode, mnemonic, info));
                    i += j;
                }
                k++;
            }
  

        }
        private void UpdateInfoBox()
        {
            InfoBox.Clear();
            InfoBox.AppendText(" A: 0x" + Program.Emulator.Processor.A.ToString("X2") + "    ");
            InfoBox.AppendText(" F: 0x" + Program.Emulator.Processor.F.ToString("X2") + " (" +
                    Program.Emulator.Processor.ZeroFlag.ToString() +
                    Program.Emulator.Processor.SubtractFlag.ToString() +
                    Program.Emulator.Processor.HalfCarryFlag.ToString() +
                    Program.Emulator.Processor.CarryFlag.ToString() + ")" + "\n");

            InfoBox.AppendText(" B: 0x" + Program.Emulator.Processor.B.ToString("X2") + "    ");
            InfoBox.AppendText(" C: 0x" + Program.Emulator.Processor.C.ToString("X2") + "\n");

            InfoBox.AppendText(" D: 0x" + Program.Emulator.Processor.D.ToString("X2") + "    ");
            InfoBox.AppendText(" E: 0x" + Program.Emulator.Processor.E.ToString("X2") + "\n");

            InfoBox.AppendText(" H: 0x" + Program.Emulator.Processor.H.ToString("X2") + "    ");
            InfoBox.AppendText(" L: 0x" + Program.Emulator.Processor.L.ToString("X2") + "\n");

            InfoBox.AppendText("-----------------------------\n");
            InfoBox.AppendText("PC: 0x" + Program.Emulator.Processor.PC.ToString("X4") + "  ");
            InfoBox.AppendText("SP: 0x" + Program.Emulator.Processor.SP.ToString("X4") + "\n");
            InfoBox.AppendText("OP: " + Program.Emulator.NextOPcode.ToString("X2") + "    " +
                                Program.Emulator.Disassembler.DisassembleMemoryAddress(
                                ref Program.Emulator.Memory, Program.Emulator.Processor.PC) + "\n");
            InfoBox.AppendText("-----------------------------\n");
            InfoBox.AppendText("IE: " + ByteToBits(Program.Emulator.Memory.Data[0xFFFF]) + "    ");
            InfoBox.AppendText("IF: " + ByteToBits(Program.Emulator.Memory.Data[0xFF0F]) + "\n");
            InfoBox.AppendText("IME: " + Program.Emulator.Processor.IME + "\n");


        }
        private string ByteToBits(byte value)
        {
            string s = "";
            s += (value & 0x80) > 0 ? "1" : "0";
            s += (value & 0x40) > 0 ? "1" : "0";
            s += (value & 0x20) > 0 ? "1" : "0";
            s += (value & 0x10) > 0 ? "1" : "0";
            s += (value & 0x08) > 0 ? "1" : "0";
            s += (value & 0x04) > 0 ? "1" : "0";
            s += (value & 0x02) > 0 ? "1" : "0";
            s += (value & 0x01) > 0 ? "1" : "0";
            return s;
        }
        private void UpdateBreakpointBox()
        {
            BreakPointListTextBox.Clear();
            for (int i = 0; i < Program.Emulator.BreakPoints.Count; i++)
                BreakPointListTextBox.AppendText("0x" + Program.Emulator.BreakPoints[i].ToString("X4") + "\n");
        }
        private void JumpToAddress(int address)
        {
            HexBox.SelectionBackColor = HexBox.BackColor;
            int line = Program.Emulator.Memory.AddressToIndex[address];
            int cursor = HexBox.GetFirstCharIndexFromLine(line);
            HexBox.Select(cursor, HexBox.Lines[line].Length);
            HexBox.SelectionBackColor = System.Drawing.Color.Tan;
            if (line > 5)
            {
                cursor = HexBox.GetFirstCharIndexFromLine(line - 5);
                HexBox.Select(cursor, 0);
            }

            HexBox.ScrollToCaret();
        }






    }
}
