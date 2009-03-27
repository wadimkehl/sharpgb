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
        Forms.HexView HexV;


        public MainForm()
        {
            InitializeComponent();
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

            }
           
        }


        private void headerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (RHF == null)
            {
                RHF = new RomHeaderForm();
                RHF.Icon = Icon;
            }
            RHF.Show();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }



        private void romHexViewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (HexV == null)
            {
                HexV = new sharpGB.Forms.HexView();
                HexV.Icon = Icon;
            }
            HexV.UpdateView(ref Program.Emulator);
            HexV.Show();
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
            InfoBox.AppendText("A = 0x" + Program.Emulator.Processor.A.ToString("X2") + "\n");
            InfoBox.AppendText("B = 0x" + Program.Emulator.Processor.B.ToString("X2") + "\n");
            InfoBox.AppendText("C = 0x" + Program.Emulator.Processor.C.ToString("X2") + "\n");
            InfoBox.AppendText("D = 0x" + Program.Emulator.Processor.D.ToString("X2") + "\n");
            InfoBox.AppendText("E = 0x" + Program.Emulator.Processor.E.ToString("X2") + "\n");
            InfoBox.AppendText("H = 0x" + Program.Emulator.Processor.H.ToString("X2") + "\n");
            InfoBox.AppendText("L = 0x" + Program.Emulator.Processor.L.ToString("X2") + "\n");
            InfoBox.AppendText("F = 0x" + Program.Emulator.Processor.L.ToString("X2") + " (" + 
                                Program.Emulator.Processor.ZeroFlag.ToString() + " " +
                                Program.Emulator.Processor.SubtractFlag.ToString() + " " +
                                Program.Emulator.Processor.HalfCarryFlag.ToString() + " " +
                                Program.Emulator.Processor.CarryFlag.ToString() + ")" + "\n");

            InfoBox.AppendText("OP = " + Program.Emulator.NextOPcode.ToString("X2") + "\t" + 
                                Program.Emulator.Disassembler.DisassembleMemoryAddress(
                                ref Program.Emulator.Memory, Program.Emulator.Processor.PC) + "\n");

            InfoBox.AppendText("IME = " + Program.Emulator.Processor.IME + "\n");



        }



    }
}
