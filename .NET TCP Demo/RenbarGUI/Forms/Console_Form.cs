#region Using NameSpace
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Windows.Forms;

// import renbar class library namespace ..
using RenbarLib.Environment;
using RenbarLib.Environment.Forms.Customizations.Service;
using RenbarLib.Network.Protocol;
#endregion

namespace RenbarGUI.Forms
{
    public partial class Console_Form : Form
    {
        #region Declare Global Variable Section
        /************************ reference renbar class library section *******************************/
        // create renbar client environment class object ..
        private Customization EnvCust = new Customization();
        /***********************************************************************************************/

        // declare object access delegate ..
        private delegate void WriteTextLogCallBack(string AppendTexts);

        // declare get output log thread ..
        private Thread GetLastTextsThread = null;

        // declare output of job id ..
        private string OutputId = string.Empty;

        // declare resume text message ..
        private string ResumeMessage = string.Empty;
        private string PauseMessage = string.Empty;

        // pause switch flag ..
        private bool PauseFlag = false;

        // stop state thread flag ..
        private volatile bool requeststop = false;
        #endregion

        #region Form Constructor Procedure
        /// <summary>
        /// Primary renbar client console form constructor procedure.
        /// </summary>
        /// <param name="Setting">current environment setting information</param>
        public Console_Form(Settings Setting, string[] ItemInfo)
        {
            // initialize form control component ...
            InitializeComponent();

            // initializing user interface behavior ...
            Language(Setting.Lang);

            // set output id ...
            this.OutputId = ItemInfo[0].Trim();

            // setting text title ...
            this.Text += " " + string.Format("(Name: {0}, Frames: [{1}])", ItemInfo[1], ItemInfo[2]);//名稱、張數

            // set current last text ...
            this.TextBox_Output.AppendText(ItemInfo[3]);//Main_Form.ProcLog[識別碼];
            this.TextBox_Output.Focus();

            // create get last texts thread object instance ...
            this.GetLastTextsThread = new Thread(new ParameterizedThreadStart(this.GetLastTexts));
            this.GetLastTextsThread.Start(ItemInfo[0]);//識別碼
        }

        /// <summary>
        /// Setting language resource.
        /// </summary>
        private void Language(Customization.Language Lang)
        {
            switch (Lang)
            {
                #region English (United-State)
                case Customization.Language.En_Us:
                    // Console Form ..
                    this.Text = EnvCust.GetLocalization(this.Name, this.Name, Customization.Language.En_Us);

                    // Output GroupBox ..
                    this.GroupBox_Output.Text = EnvCust.GetLocalization(this.Name, this.GroupBox_Output.Name, Customization.Language.En_Us);

                    // Pause Button ..
                    this.Button_Pause.Text = EnvCust.GetLocalization(this.Name, this.Button_Pause.Name, Customization.Language.En_Us);

                    // Resume Text Message ..
                    ResumeMessage = this.EnvCust.GetLocalization(this.Name, this.Button_Pause.Name + "_Resume", Customization.Language.En_Us);
                    break;
                #endregion

                #region Traditional Chinese
                case Customization.Language.Zh_Tw:
                    // Console Form ..
                    this.Text = EnvCust.GetLocalization(this.Name, this.Name, Customization.Language.Zh_Tw);

                    // Output GroupBox ..
                    this.GroupBox_Output.Text = EnvCust.GetLocalization(this.Name, this.GroupBox_Output.Name, Customization.Language.Zh_Tw);

                    // Pause Button ..
                    this.Button_Pause.Text = EnvCust.GetLocalization(this.Name, this.Button_Pause.Name, Customization.Language.Zh_Tw);

                    // Resume Text Message ..
                    ResumeMessage = this.EnvCust.GetLocalization(this.Name, this.Button_Pause.Name + "_Resume", Customization.Language.Zh_Tw);
                    break;
                #endregion
            }
        }
        #endregion

        #region 獲得該任務好的日志輸出 Get Latest Log Output Texts Thread
        /// <summary>
        /// get last log texts thread.
        /// </summary>
        /// <param name="Id">assign job id.</param>
        private void GetLastTexts(object Id)
        {
            WriteTextLogCallBack WriteTextLog = delegate(string Texts)//Texts=Main_Form.ProcLog[Id.ToString()] 
            {
                if (this.TextBox_Output.Text.Length < Texts.Length)
                {
                    int len = (Texts.Length - this.TextBox_Output.Text.Length);//比較長度
                    //將Main_Form中的多余的信息附加
                    string appendTexts = Texts.Substring(this.TextBox_Output.Text.Length, len);

                    if (!this.PauseFlag)
                        // append text to output textbox ..
                        this.TextBox_Output.AppendText(appendTexts);
                }
            };

            do///？？？？？？？？？？？？？？為什么要用循環？？？？？？？？？？？？？？
            {
                try
                {
                    //如果ProcLog包含該id索引，則調用WriteTextLog委托函數寫入
                    if (Main_Form.ProcLog.ContainsKey(Id.ToString()))
                        // invoke write log delegate procedure ..
                        this.Invoke(WriteTextLog, new object[] { Main_Form.ProcLog[Id.ToString()] });
                    else
                        break;
                }
                catch (InvalidOperationException)
                {
                    // if delegate object already clean, exit loop ..
                    break;
                }
            } while (!requeststop);
        }
        #endregion

        #region 關閉窗口 Form Closing Event Procedure
        private void Console_Form_FormClosing(object sender, FormClosingEventArgs e)
        {
            // change stop flag ..
            this.requeststop = true;
            // remove ..
            Main_Form.OutputForms.Remove(this.OutputId);
            // clean up any resources being used ..
            this.Dispose();
        }
        #endregion

        #region 暫停/重啟日志輸出 Form Button Click Event Procedure
        /// <summary>
        /// 暫停/重啟日志輸出 Pause log output.
        /// </summary>
        private void Button_Pause_Click(object sender, EventArgs e)
        {
            //如果暫停，則重新啟動
            if (this.Button_Pause.Text.Equals(ResumeMessage))
            {
                // change text ..
                this.Button_Pause.Text = this.PauseMessage;

                // change pause flag ..
                this.PauseFlag = false;
            }
            else
            {
                // assign current text ..
                this.PauseMessage = this.Button_Pause.Text;

                // change text ..
                this.Button_Pause.Text = this.ResumeMessage;

                // change pause flag ..
                this.PauseFlag = true;
            }
        }
        #endregion
    }
}