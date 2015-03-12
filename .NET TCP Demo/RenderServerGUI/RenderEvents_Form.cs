#region Using NameSpace
using System;
using System.Text;
using System.Threading;
using System.Windows.Forms;

// import renbar server library namespace ..
using RenbarLib.Environment;
#endregion

namespace RenderServerGUI
{
    public partial class RenderEvents_Form : Form
    {
        #region Declare Global Variable Section定義全局變量Section
        // declare append text dynamic variable ..//定義追加文本動態變量
        private volatile string _AppendText = string.Empty;

        // declare object access delegate ..//定義對象訪問delegate
        private delegate void AppendLogCallBack(string Text);

        // declare event notify component ..//定義事件通報組件(AutoResetEvent能讓thread以信號相互通訊。
        //通常通訊是有關thread必須獨佔存取的資源)執行緒--thread
        private AutoResetEvent eventNotify = null;

        // declare write log thread ..//定義寫日志線程
        private Thread AppendLogThread = null;
        #endregion

        #region Form Constructor And Destructor Procedure表單構造器和Destructor過程
        /// <summary>
        /// Renbar server log form constructor procedure.
        /// </summary>
        /// <param name="AppendTexts">append text display to richTextBox control.</param>
        public RenderEvents_Form(StringBuilder AppendTexts)
        {
            // initialize component standard procedure ..
            InitializeComponent();

            // setting form title and icon ..//設置表單標題和圖標
            this.Text = string.Format("{0} - {1}", global::RenbarLib.Environment.AssemblyInfoClass.ProductInfo, "Log Window");

            if (AppendTexts.Length > 0)// 追加文本Length(417)
                // append text to log window ..//追加文本到日志窗口
                this.Render_LogBox.Text = AppendTexts.ToString();

            // form active call back ..//表單活動收回(日志形式是活動)
            RenderEvents.IsActive = true;
        }

        /// <summary>
        /// Log form destructor procedure.//日志表單destructor過程
        /// </summary>
        ~RenderEvents_Form()
        {
            // form active call back ..//表單活動收回
            RenderEvents.IsActive = false;

            // stop running threads ..//停止運行線程
            this.AppendLogThread.Abort();

            // clean all resource ..//清空所有資源
            this.Dispose(true);
        }
        #endregion

        #region Form Load Event Procedure表單加載事件Procedure
        /// <summary>
        /// Form load event.
        /// </summary>
        private void Log_Form_Load(object sender, System.EventArgs e)
        {
            // create event notify instance ..//創建事件通報instance
            this.eventNotify = new AutoResetEvent(false);
            
            // start append log thread ..//開始追加日志線程
            this.AppendLogThread = new Thread(new ThreadStart(this.Append));
            this.AppendLogThread.IsBackground = true;
            this.AppendLogThread.Priority = ThreadPriority.Normal;
            this.AppendLogThread.Start();
        }
        #endregion

        #region Append Text Property增加文本Property
        /// <summary>
        /// Append text to log display window.//追加文本到日志顯示窗口
        /// </summary>
        internal string AppendText
        {
            set
            {
                this._AppendText = value;

                // release append thread locked ..//釋放追加線程鎖定(如果AutoResetEvent呈非信號狀態，
                //thread區塊只要呼叫Set即可等候目前控制資源的thread發出可以使用資源的信號。)
                this.eventNotify.Set();
            }
        }
        #endregion

        #region Append Text Thread Event Procedure追加文本線程事件Procedure
        /// <summary>
        /// append text loop thread method.
        /// </summary>
        private void Append()
        {
            try
            {
                while (true)
                {
                    // wait for text variable changed synchronization ..//等待文本變量改變同步
                    //(thread只要呼叫AutoResetEvent上的WaitOne，即可等候信號。)
                    this.eventNotify.WaitOne();

                    #region Invoke Append Text Object Delegate Procedure調用追加文本對象代表Procedure
                    AppendLogCallBack WriteLogText = delegate(string Text)
                    {
                        // append new text string ..//追加新文本字符串
                        if (Text != null && Text != string.Empty)
                            this.Render_LogBox.AppendText(Text);
                    };

                    if (this.Created)
                        this.Invoke(WriteLogText, new object[] { this._AppendText });

                    #endregion

                    // reset the wait handle for the next access text variable resource ..//為下一個接口文本變量資源重置等待Handle
                    this.eventNotify.Reset();
                }
            }
            catch (Exception ex)
            {
                  string ExceptionMsg = ex.Message + ex.StackTrace;
            }
        }
        

        #endregion
    }
}