using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace sharpGB.Forms
{
    public partial class CPUView : Form
    {
        public CPUView()
        {
            InitializeComponent();
        }

        public void UpdateView(ref CEmulator E)
        {
            AField.Text = E.Processor.A.ToString("X2");
            BField.Text = E.Processor.B.ToString("X2");
            CField.Text = E.Processor.C.ToString("X2");
            DField.Text = E.Processor.D.ToString("X2");
            EField.Text = E.Processor.E.ToString("X2");
            HField.Text = E.Processor.H.ToString("X2");
            LField.Text = E.Processor.L.ToString("X2");

            SPField.Text = E.Processor.SP.ToString("X4");
            PCField.Text = E.Processor.PC.ToString("X4");

            FLAGSField.Text = E.Processor.ZeroFlag.ToString() + " " +
                                E.Processor.SubtractFlag.ToString() + " " +
                                E.Processor.HalfCarryFlag.ToString() + " " +
                                E.Processor.CarryFlag.ToString();

            OPField.Text = E.NextOPcode.ToString("X2") + "  " + E.Disassembler.DisassembleMemoryAddress(ref E.Memory, E.Processor.PC); ;
            IEField.Text = E.Processor.IME ? "Yes" : "No";

            if (E.Processor.CPUHalt) CPUStatField.Text = "Halted";
            else if (E.Processor.CPUStop) CPUStatField.Text = "Stopped";
            else CPUStatField.Text = "Ok";


        }
    }
}
