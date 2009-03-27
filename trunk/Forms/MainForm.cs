using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace sharpGB
{
    public partial class MainForm : Form
    {
        RomHeaderForm RHF;


        public MainForm()
        {
            InitializeComponent();
            Program.Emulator.Video.SetDrawDestination(ref this.VideoBox);
            UpdateInfoBox();
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

                UpdateHexBox();

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

            InfoBox.AppendText("---------------------------\n");
            InfoBox.AppendText("PC: 0x" + Program.Emulator.Processor.PC.ToString("X4") + "  ");
            InfoBox.AppendText("SP: 0x" + Program.Emulator.Processor.SP.ToString("X4") + "\n");
            InfoBox.AppendText("OP: " + Program.Emulator.NextOPcode.ToString("X2") + "    " + 
                                Program.Emulator.Disassembler.DisassembleMemoryAddress(
                                ref Program.Emulator.Memory, Program.Emulator.Processor.PC) + "\n");
            InfoBox.AppendText("---------------------------\n");
            InfoBox.AppendText("IME: " + Program.Emulator.Processor.IME + "\n");


        }

        private void UpdateHexBox()
        {
            HexBox.Clear();
            string s;
            int i = 0, j = 0;
            while (i < 0x4000)
            {
                s = "0x" + i.ToString("X4") + " ";
                for (j = 0; j < Program.Emulator.Disassembler.GetOperationByteCount(ref Program.Emulator.Memory, i); j++)
                    s += Program.Emulator.Memory.Data[i + j].ToString("X2") + " ";

                HexBox.AppendText(s + "\t" + Program.Emulator.Disassembler.DisassembleMemoryAddressWithInfo(ref Program.Emulator.Memory, i) + "\n");
                
                i += j;
            }
                
                      

        }

        private void AddBreakPoint_Click(object sender, EventArgs e)
        {
            Program.Emulator.AddBreakPoint(Int32.Parse(BreakPointAddressTextBox.Text, System.Globalization.NumberStyles.HexNumber));

            BreakPointListTextBox.Clear();
            for (int i = 0; i < Program.Emulator.BreakPoints.Count; i++)
                BreakPointListTextBox.AppendText("0x" + Program.Emulator.BreakPoints[i].ToString("X4") + "\n"); 

        }

        private void RemoveBreakPoint_Click(object sender, EventArgs e)
        {
            Program.Emulator.RemoveBreakPoint(Int32.Parse(BreakPointAddressTextBox.Text, System.Globalization.NumberStyles.HexNumber));

            BreakPointListTextBox.Clear();
            for (int i = 0; i < Program.Emulator.BreakPoints.Count; i++)
                BreakPointListTextBox.AppendText("0x" + Program.Emulator.BreakPoints[i].ToString("X4") + "\n");
        }

        private void RunButton_Click(object sender, EventArgs e)
        {
            Program.Emulator.EmulateFully();
            Program.Emulator.ShowError();
            UpdateInfoBox();
        }



    }
}
