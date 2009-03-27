using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace sharpGB.Forms
{
    public partial class HexView : Form
    {



        public HexView()
        {
            InitializeComponent();
        }



        public void UpdateView(ref CEmulator E)
        {
            TextBox.Clear();
            string s;
            int i=0, j=0;
            while (i < 0x4000)
            {
                s = "0x" + i.ToString("X4") + "\t";
                for (j = 0; j < E.Disassembler.GetOperationByteCount(ref E.Memory, i); j++)
                    s += E.Memory.Data[i + j].ToString("X2") + " ";
                if (j > 2)
                TextBox.AppendText(s + "\t" + E.Disassembler.DisassembleMemoryAddressWithInfo(ref E.Memory, i) + "\n");
                else
                TextBox.AppendText(s + "\t\t" + E.Disassembler.DisassembleMemoryAddressWithInfo(ref E.Memory, i) + "\n");

                i += j;
            }
                
                      
            

        }


    }
}
