using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace RenbarLib.Environment
{
    public class Log
    {
        private object thisObject = new object();

        private bool _iswrite = false;
        
        public bool Iswrite
        {
            get { return _iswrite; }
            set { _iswrite = value; }
        }
        

        #region 公有屬性 日志類型枚舉Log Level Enumeration
        /// <summary>
        /// Log level.
        /// </summary>
        public enum Level
        {
            Error,
            Information,
            Warning
        }


        #endregion

        #region 屬性 輸出文件最大尺寸、設定時間日期格式Output File Maximum Size, DateTime Style Properties
        /// <summary>
        /// Get or set output file maximum size.
        /// </summary>
        public long Maximum
        {
            get;
            set;
        }

        /// <summary>
        /// Custom date time style.
        /// </summary>
        private string CustomDateTime
        {
            get
            {
                string
                    date = DateTime.Now.ToShortDateString(),
                    hour = DateTime.Now.Hour.ToString().PadLeft(2, '0'),
                    min = DateTime.Now.Minute.ToString().PadLeft(2, '0'),
                    sec = DateTime.Now.Second.ToString().PadLeft(2, '0');

                return string.Format("{0} {1}:{2}:{3}", date, hour, min, sec);
            }
        }
        #endregion

        #region 獲取隊列文件的名稱 Get Sequence File Name
        /// <summary>
        /// Get file style.
        /// </summary>
        /// <param name="PadLength">sequence number.</param>
        /// <returns>FileStyle（System.String，年月日+隊列數）</returns>
        private string FileStyle(int PadLength)
        {
            int seqNum = 0;
            string
                year = DateTime.Now.Year.ToString(),
                month = DateTime.Now.Month.ToString().PadLeft(2, '0'),
                day = DateTime.Now.Day.ToString().PadLeft(2, '0');
            //
            if (PadLength > 0)
                seqNum = PadLength;
            //生成FileStyle，由年月日+隊列數
            return year + month + day + seqNum.ToString().PadLeft(4, '0');
        }

        /// <summary>
        /// 獲得指定文件路徑下的最大隊列數 Get log file maximum sequence.
        /// </summary>
        /// <param name="LogFolder">log folder path.</param>
        /// <returns>System.Int32</returns>
        private int MaxFile(string LogFolder)
        {
            string result = string.Empty;

            if (0 >= this.Maximum)
                // assign default maximum size ..
                this.Maximum = 1048576;

            //判斷路徑是否存在
            if (Directory.Exists(LogFolder))
            {
                string pattern = string.Format("*{0}*.log", DateTime.Now.Day);
                //搜尋該目錄下最外層文件名中包含指定日期的文件
                string[] files = Directory.GetFiles(LogFolder, pattern, SearchOption.TopDirectoryOnly);

                if (files.Length > 0)
                {
                    foreach (string s in files.OrderByDescending(s => s))
                    {
                        FileInfo info = new FileInfo(s);
                        if (info.Exists && info.Length < this.Maximum)

                            return int.Parse(s.Substring(((s.Length) - 8), 4));//如果小于8？？？？

                        result = s.Substring(((s.Length) - 8), 4);

                        break;
                    }
                }
                else
                    result = "0000";
            }

            return int.Parse(result) + 1;
        }
        #endregion

        #region 寫消息進程 Write Message Procedure
        /// <summary>
        /// 向日志文件寫入消息 Write message to log file.
        /// </summary>
        /// <param name="ProductName">Product name.</param>
        /// <param name="Lv">Message level.</param>
        /// <param name="Message">text message.</param>
        /// <param name="IsTrace">是否監控if registry to trace event class is true; otherwise false.</param>
        public void Writer(string ProductName, Level Lv, string Message, bool IsTrace)
        {
            lock (this.thisObject)
            {
                // declare log floder and confirm directory ..
                string LogFloder = string.Empty;

                if (ProductName == "+Debug+" && _iswrite)
                {
                    LogFloder = string.Format(@"{0}\{1}", global::System.Environment.CurrentDirectory, "Data");
                }
                else
                {
                    // declare log floder and confirm directory ..
                    LogFloder = string.Format(@"{0}\{1}", global::System.Environment.CurrentDirectory, "Log");
                }


                if (!Directory.Exists(LogFloder))
                    Directory.CreateDirectory(LogFloder);

                // declare string format array ..
                string[] ContentArgs = new string[] {
                    this.CustomDateTime,
                    ProductName,
                    Lv.ToString(),
                    Message
                };

                // declare log save file name ..
                string
                    RecordFile = string.Format(@"{0}\{1}", LogFloder, this.FileStyle(MaxFile(LogFloder)) + ".log"),
                    ContentString = string.Format("{0} {1} - {2}\r\n{3}\r\n", ContentArgs);

                try
                {
                    // write or append to target file ..
                    using (StreamWriter sw = new StreamWriter(RecordFile, true))
                    {
                        // writing ..
                        sw.WriteLine(ContentString);

                        // registry to trace event class ..
                        if (IsTrace)
                            Trace.WriteLine(ContentString, ProductName);

                        // flush buffer ..
                        sw.Flush();
                    }
                }
                catch
                { }
            }
        }
        #endregion
    }
}