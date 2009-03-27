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
        Forms.CPUView CPUV;
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




        private void EmulateNextStepButton_Click(object sender, EventArgs e)
        {

            Program.Emulator.EmulateNextStep();

            if (CPUV != null) CPUV.UpdateView(ref Program.Emulator);

            if (!Program.Emulator.EmulationRunning)
                MessageBox.Show("Emulation halted!");


            
        }

        private void cPUViewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (CPUV == null)
            {
                CPUV = new sharpGB.Forms.CPUView();
                CPUV.Icon = Icon;
            }
            CPUV.UpdateView(ref Program.Emulator);
            CPUV.Show();
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

        private void RunButton_Click(object sender, EventArgs e)
        {
            Program.Emulator.EmulateSteps( Int32.Parse(this.RunBox.Text));

            if (CPUV != null) CPUV.UpdateView(ref Program.Emulator);

            if (!Program.Emulator.EmulationRunning)
                MessageBox.Show("Emulation halted!");
        }

        private void StopButton_Click(object sender, EventArgs e)
        {
            Program.Emulator.EmulationRunning = false;
        }





    }
}
