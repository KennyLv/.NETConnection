using System;
using System.Windows.Forms;

namespace RenbarServerGUI
{
    static class Program
    {
        /// <summary>
        /// Application main entry point.
        /// </summary>
        [MTAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // running a standard application window message loop ..
            global::RenbarLib.Environment.AppSingleton.Run(new Main_Form());
        }
    }
}