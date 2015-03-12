using System;
using System.Windows.Forms;

namespace RenderServerGUI
{
    static class Program
    {
        /// <summary>
        /// Application main entry point.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // running a standard application window message loop ..
            global::RenbarLib.Environment.AppSingleton.Run(new Main_Form());
        }
    }
}