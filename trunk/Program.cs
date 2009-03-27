using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace sharpGB
{

    static class Program
    {

        // The static emulator instance
        static public CEmulator Emulator;



       [STAThread]
        static void Main()
        {

            Emulator = new CEmulator();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());


        }
    }
}
