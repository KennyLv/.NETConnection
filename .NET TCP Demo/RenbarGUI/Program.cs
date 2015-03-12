using System;
using System.Windows.Forms;

namespace RenbarGUI
{
    static class Program
    {
        /// <summary>
        /// Application main entry point.
        /// </summary>
        [STAThread]      //指示應用程式的 COM 執行緒模型為單一執行緒 Apartment (Single-Threaded Apartment，STA)
        static void Main()
       {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // running a standard application window message loop ..
            global::RenbarLib.Environment.AppSingleton.Run(new Forms.Main_Form());
        }
    }
}