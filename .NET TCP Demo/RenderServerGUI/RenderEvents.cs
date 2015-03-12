#region Using NameSpace
using System;
using System.IO;
using System.Text;

// import renbar server library namespace ..
using RenbarLib.Environment;
#endregion

namespace RenderServerGUI
{
    internal class RenderEvents
    {
        #region Declare Global Variable Section定義全局變量Section
        // create renbar environemnt service class ..//創建Renbar環境服務類
        private static Service EnvSvr = new Service();

        // declare log message collection variable ..//定義日志信息收集變量
        private static StringBuilder ContentStrings = new StringBuilder();
        private static ushort Lines = 0;

        // delcare render log form class ..//定義Render日志表單類
        private static RenderEvents_Form LogForm = null;

        // declare log form active flag ..//定義日志表單活動標志
        private static bool Open_LogForm = false;
        #endregion

        #region Log Form Window Event Procedure日志表單Window事件Procedure
        /// <summary>
        /// Show log form window//顯示日志表單窗口
        /// </summary>
        internal void OpenLogWindow()
        {
            if (ContentStrings != null)
            {
                // create log object instance ..//創建日志對象Instance
                LogForm = new RenderEvents_Form(ContentStrings);

                // show window ..//顯示窗口
                LogForm.Show();
            }
        }

        /// <summary>
        /// Log form is active property//日志形式是活動
        /// </summary>
        internal static bool IsActive
        {
            set
            {
                Open_LogForm = value;
            }
        }
        #endregion

        #region Append Log Message Procedure追加日志信息Procedure
        /// <summary>
        /// Append log message.//追加日志信息
        /// </summary>
        /// <param name="Text">append text</param>
        internal static void AppendLog(string Text)
        {
            if (!Open_LogForm)
            {
                if (Lines >= 5000)
                    SaveLog();
            }

            //if (ContentStrings != null && ContentStrings.ToString()!="" )
            if (ContentStrings != null)
                ContentStrings.AppendLine(string.Format("{0} - {1}", Service.GetSysDateTime, Text));

            if (Open_LogForm)
                LogForm.AppendText = string.Format("{0} - {1}", Service.GetSysDateTime, Text + "\r\n");

            Lines++;
        }
        #endregion

        #region Clean And Save Log Message Procedure清空和保存日志信息過程
        /// <summary>
        /// Clean current log message.
        /// </summary>
        internal static void SaveLog()
        {
            // declare log floder and save file name ..//定義日志文件夾和保存文件名
            string
                LogsFloder = string.Format(@"{0}\{1}", Environment.CurrentDirectory, @"Log\RenderEvents"),
                RecordFile = string.Format(@"{0}\{1}-{2}", LogsFloder, "Log", Service.CustomSysDateTime + ".log");

            // check work directory ..//查看工作目录
            if (!Directory.Exists(LogsFloder))
                Directory.CreateDirectory(LogsFloder);

            // clean log window message, and save to text file ..//清空日志window信息，并且保存文本文件
            using (StreamWriter sw = new StreamWriter(RecordFile))
            {
                // write to text file ..//寫到文本文件
                sw.Write(ContentStrings);

                // clear current all text ..//清空黨前所有文本
                ContentStrings.Remove(0, ContentStrings.Length);
            }

            // reset lines count ..//重置行總數
            Lines = 0;
        }
        #endregion
    }
}