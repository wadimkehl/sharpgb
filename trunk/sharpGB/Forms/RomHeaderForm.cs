using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace sharpGB
{
    public partial class RomHeaderForm : Form
    {
        public RomHeaderForm()
        {
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void RomHeaderForm_Shown(object sender, EventArgs e)
        {
            this.ShowHeaderInfo();
        }


        private void ShowHeaderInfo()
        {
            // if no rom is loaded, then there is no header data
            if (!Program.Emulator.ROM.RomLoaded) return;


            // gather and display all rom data
            this.FileSizeField.Text = (Program.Emulator.ROM.RomSize / 1024).ToString() + "kB";

            this.CodeStartField.Text =  Program.Emulator.ROM.Header.BeginCodeExecutionPoint[0].ToString("X2") + " " +
                                        Program.Emulator.ROM.Header.BeginCodeExecutionPoint[1].ToString("X2") + " " +
                                        Program.Emulator.ROM.Header.BeginCodeExecutionPoint[2].ToString("X2") + " " +
                                        Program.Emulator.ROM.Header.BeginCodeExecutionPoint[3].ToString("X2");

            this.GameTitleField.Text = System.Text.Encoding.UTF8.GetString(Program.Emulator.ROM.Header.GameTitle);
            this.ColorGBField.Text = Program.Emulator.ROM.Header.ColorGB.ToString("X2");
            this.CartridgeTypeField.Text = Program.Emulator.ROM.Header.CartridgeType.ToString("X2");
            this.LicenseeHighField.Text = Program.Emulator.ROM.Header.LicenseeHigh.ToString("X2");
            this.LicenseeLowField.Text = Program.Emulator.ROM.Header.LicenseeLow.ToString("X2");
            this.SGBIndicatorField.Text = Program.Emulator.ROM.Header.GBFunctionsIndicator.ToString("X2");
            this.ROMSizeField.Text = Program.Emulator.ROM.Header.ROMSize.ToString("X2");
            this.RAMSizeField.Text = Program.Emulator.ROM.Header.RAMSize.ToString("X2");
            this.DestinationCodeField.Text = Program.Emulator.ROM.Header.DestinationCode.ToString("X2");
            this.LicenseeCodeField.Text = Program.Emulator.ROM.Header.LicenseeCode.ToString("X2");
            this.ROMVersionField.Text = Program.Emulator.ROM.Header.ROMVersionNumber.ToString("X2");
            this.ComplementCheckField.Text = Program.Emulator.ROM.Header.ComplementCheck.ToString("X2");

            this.ChecksumField.Text = Program.Emulator.ROM.Header.Checksum[0].ToString("X2") +
                                        Program.Emulator.ROM.Header.Checksum[1].ToString("X2");
        }

        private void ColorGBLabel_Click(object sender, EventArgs e)
        {

        }


    }
}
