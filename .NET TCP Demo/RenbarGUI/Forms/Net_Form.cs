#region Using NameSpace
using System;
using System.Net;
using System.Windows.Forms;
using System.Collections.Generic;
// import renbar class library namespace ..
using RenbarLib.Environment;
using RenbarLib.Environment.Forms.Customizations.Service;
using RenbarLib.Network.Sockets;
#endregion

namespace RenbarGUI.Forms
{
    public partial class Net_Form : Form
    {
        #region Declare Global Variable Section
        /************************ reference renbar class library section *******************************/
        // create renbar client environment class object ..
        private Customization EnvCust = new Customization();

        private Service EnvSvr = new Service();

        private List<RenbarServer> MyServerList = new List<RenbarServer>();

        /***********************************************************************************************/

        // declare current connect remote server address and prot number ..
        private IPAddress Addr = IPAddress.None;

        // create remote server reply timer ..
        private Timer __reply = new Timer();

        // declare test connect thread ..
        private global::System.Threading.Thread __Connect = null;

        // declare connect result enumeration ..
        private ConnectResult resultVar = ConnectResult.NONE;

        // declare original ip and port value ..
        private bool restoreInit = true;
        private string InitIpAddr = string.Empty;
        private string InitPort = string.Empty;

        // declare parse address and connect error message ..
        private string ConnectErrMsg = string.Empty;
        private string ParseErrMsg = string.Empty;

        // declare connect success message ..
        private string ConnectOkMsg = string.Empty;
        #endregion

        #region 屬性 設置網絡連接 Network Connected Properties
        /// <summary>
        /// Get or set remote address.
        /// </summary>
        internal IPAddress ServerAddr
        {
            get;
            set;
        }

        /// <summary>
        /// Get or set remote prot number.
        /// </summary>
        internal ushort PortNumber
        {
            get;
            set;
        }
        #endregion

        #region 枚舉鏈接結果 Connect Result Enumeration
        /// <summary>
        /// Test connect result enumeration.
        /// </summary>
        private enum ConnectResult
        {
            /// <summary>
            /// None flag.
            /// </summary>
            NONE,
            /// <summary>
            /// Success flag.
            /// </summary>
            SUCCESS,
            /// <summary>
            /// Fail connect flag.
            /// </summary>
            FAILCONNECT,
        }
        #endregion

        #region 構造函數 Form Constructor Procedure
        /// <summary>
        /// Primary renbar client net form constructor procedure.
        /// </summary>
        /// <param name="Setting">current environment setting information.</param>
        public Net_Form(Settings Setting)
        {
            // initialize form control component ..
            InitializeComponent();
            // initializing user interface behavior ..
            Language(Setting.Lang);

            // 顯示原始設置 assign current environment settings ..
            if (Setting.ServerIpAddress != null && !Setting.ServerIpAddress.Equals(IPAddress.None))
            {
                //this.TextBox_Server.Text = Setting.ServerIpAddress.ToString();
                this.comboBox_Server.Text = Setting.ServerIpAddress.ToString();
            }
            if (Setting.ServerPort > 0)
            {
                this.NumericUpDown_Port.Value = Setting.ServerPort;
            }

            // 調整幫助描述 fix server help description ..
            string[] __Stemp = this.Label_Server_Help.Text.Split('-');
            this.Label_Server_Help.Text = string.Empty;
            for (int i = 0; i < __Stemp.Length; i++)
            {
                this.Label_Server_Help.Text += __Stemp[i] + "\n\r";
            }

            // 保存臨時變量 save to temporary variable ..
            //this.InitIpAddr = this.TextBox_Server.Text.Trim();
            this.InitIpAddr = this.comboBox_Server.Text.Trim();

            this.InitPort = this.NumericUpDown_Port.Value.ToString();

            Binding(this.EnvSvr.ServerListFilePath);

            // 注冊鏈接請求事件 registy connect reply event ..
            this.__reply.Tick += new EventHandler(__reply_Tick);
        }

        /// <summary>
        /// Setting language resource.？？？？？？？？？？？？！！！！！！！！！？？？？？？？？
        /// </summary>
        /// <param name="Lang">current language setting.</param>
        private void Language(Customization.Language Lang)
        {
            switch (Lang)
            {
                #region English (United-State)
                case Customization.Language.En_Us:


                    // Net Form ..
                    this.Text = EnvCust.GetLocalization(this.Name, this.Name, Customization.Language.En_Us);

                    // Port Label ..
                    this.Label_Port.Text = EnvCust.GetLocalization(this.Name, this.Label_Port.Name, Customization.Language.En_Us);

                    // Port Help Port Label ..
                    this.Label_Port_Help.Text = EnvCust.GetLocalization(this.Name, this.Label_Port_Help.Name, Customization.Language.En_Us);

                    // Server Label ..
                    this.Label_Server.Text = EnvCust.GetLocalization(this.Name, this.Label_Server.Name, Customization.Language.En_Us);

                    // Server Message String TextBox ..
                    this.ConnectErrMsg = EnvCust.GetLocalization(this.Name, this.comboBox_Server.Name + "_Error1_String", Customization.Language.En_Us);
                    this.ParseErrMsg = EnvCust.GetLocalization(this.Name, this.comboBox_Server.Name + "_Error2_String", Customization.Language.En_Us);
                    this.ConnectOkMsg = EnvCust.GetLocalization(this.Name, this.comboBox_Server.Name + "_String", Customization.Language.En_Us);

                    // Server Help Label ..
                    this.Label_Server_Help.Text = EnvCust.GetLocalization(this.Name, this.Label_Server_Help.Name, Customization.Language.En_Us);

                    // Connect Label ..
                    this.Label_Connect.Text = EnvCust.GetLocalization(this.Name, this.Label_Connect.Name, Customization.Language.En_Us);

                    // Test Connect Button ..
                    this.Button_AU.Text = EnvCust.GetLocalization(this.Name, this.Button_AU.Name, Customization.Language.En_Us);
                    this.Button_Delete.Text = EnvCust.GetLocalization(this.Name, this.Button_Delete.Name, Customization.Language.En_Us);
                    this.Button_Save.Text = EnvCust.GetLocalization(this.Name, this.Button_Save.Name, Customization.Language.En_Us);
                    break;
                #endregion

                #region Traditional Chinese
                case Customization.Language.Zh_Tw:
                    // Net Form ..
                    this.Text = EnvCust.GetLocalization(this.Name, this.Name, Customization.Language.Zh_Tw);

                    // Port Label ..
                    this.Label_Port.Text = EnvCust.GetLocalization(this.Name, this.Label_Port.Name, Customization.Language.Zh_Tw);

                    // Port Help Port Label ..
                    this.Label_Port_Help.Text = EnvCust.GetLocalization(this.Name, this.Label_Port_Help.Name, Customization.Language.Zh_Tw);

                    // Server Label ..
                    this.Label_Server.Text = EnvCust.GetLocalization(this.Name, this.Label_Server.Name, Customization.Language.Zh_Tw);

                    // Server Message String TextBox ..
                    this.ConnectErrMsg = EnvCust.GetLocalization(this.Name, this.comboBox_Server.Name + "_Error1_String", Customization.Language.Zh_Tw);
                    this.ParseErrMsg = EnvCust.GetLocalization(this.Name, this.comboBox_Server.Name + "_Error2_String", Customization.Language.Zh_Tw);
                    this.ConnectOkMsg = EnvCust.GetLocalization(this.Name, this.comboBox_Server.Name + "_String", Customization.Language.Zh_Tw);

                    // Server Help Label ..
                    this.Label_Server_Help.Text = EnvCust.GetLocalization(this.Name, this.Label_Server_Help.Name, Customization.Language.Zh_Tw);

                    // Connect Label ..
                    this.Label_Connect.Text = EnvCust.GetLocalization(this.Name, this.Label_Connect.Name, Customization.Language.Zh_Tw);

                    // Test Connect Button ..
                    this.Button_AU.Text = EnvCust.GetLocalization(this.Name, this.Button_AU.Name, Customization.Language.Zh_Tw);
                    this.Button_Delete.Text = EnvCust.GetLocalization(this.Name, this.Button_Delete.Name, Customization.Language.Zh_Tw);
                    this.Button_Save.Text = EnvCust.GetLocalization(this.Name, this.Button_Save.Name, Customization.Language.Zh_Tw);


                    break;
                #endregion
            }
        }

        /// <summary>
        /// read the serverList from the xml file
        /// </summary>
        /// <param name="path">xml file path</param>
        private void Binding(string path)
        {
            this.comboBox_Server.Items.Clear();

            MyServerList = this.EnvSvr.GetServers(path);
           
            for (int i = 0; i < MyServerList.Count; i++)
            {
                this.comboBox_Server.Items.Add(MyServerList[i].ServerIP.ToString());
            }
        }

        #endregion

        #region 關閉窗體 Form Closing Event Procedure
        private void Net_Form_FormClosing(object sender, FormClosingEventArgs e)
        {
            // 關閉鏈接 stop connect and timer ..
            if (this.__Connect != null)
            {
                this.__reply.Stop();
                this.__Connect.Abort();
            }
            //保存初始設置
            if (this.restoreInit)
            {
                // restore original address and port ..
                if (!string.IsNullOrEmpty(this.InitIpAddr))
                    this.ServerAddr = IPAddress.Parse(this.InitIpAddr);
                else
                    this.ServerAddr = IPAddress.None;

                this.PortNumber = Convert.ToUInt16(this.InitPort);
            }
            // return correct dialog result ..？？？？？？？？？？？？？？？？？？？？？？
            this.DialogResult = DialogResult.OK;
        }
        #endregion

        #region 檢測連接 Event Trigger Procedure
        /// <summary>
        /// 鏈接請求 Reply timer tick event procedure.
        /// </summary>
        private void __reply_Tick(object sender, EventArgs e)
        {
            //鏈接斷開則停止請求
            if (!__Connect.IsAlive)
            {
                __reply.Stop();
                this.FormControls(true);
            }
        }

        /// <summary>
        /// 控制窗體控件的可用狀體 Enable or disable form controls behaivor.
        /// </summary>
        /// <param name="IsEnable">enable form component.</param>
        private void FormControls(bool IsEnable)
        {
            switch (resultVar)
            {
                case ConnectResult.SUCCESS:
                    MessageBox.Show(this.ConnectOkMsg, AssemblyInfoClass.ProductInfo,
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    break;

                case ConnectResult.FAILCONNECT:
                    MessageBox.Show(this.ConnectErrMsg, AssemblyInfoClass.ProductInfo,
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    break;
            }

            if (!IsEnable)
            {
                //this.TextBox_Server.Enabled = false;
                this.comboBox_Server.Enabled = false;
                this.NumericUpDown_Port.Enabled = false;

                this.Button_Delete.Enabled = false;
                this.Button_AU.Enabled = false;
                this.Button_Save.Enabled = false;

                this.Label_Connect.Visible = true;
                this.PictureBox_Connect.Visible = true;
                this.Update();
            }
            else
            {
                //this.TextBox_Server.Enabled = true;
                this.comboBox_Server.Enabled = true;
                this.NumericUpDown_Port.Enabled = true;

                this.Button_Delete.Enabled = true;
                this.Button_AU.Enabled = true;
                this.Button_Save.Enabled = true;

                this.Label_Connect.Visible = false;
                this.PictureBox_Connect.Visible = false;
                this.Update();
            }
        }

        /// <summary>
        /// 鏈接指定地址的服務器 Connect remote server thread event.
        /// </summary>
        /// <param name="address">test address.</param>
        private void Connecting(object address)
        {
            // create connect remote machine class object instance ..
            ScanPort runConnect = new ScanPort();

            // 測試是否可連接 scanning ..
            if (!runConnect.Scan((IPAddress)address, (ushort)this.NumericUpDown_Port.Value))
                resultVar = ConnectResult.FAILCONNECT;
            else
                resultVar = ConnectResult.SUCCESS;
        }
        #endregion      

        #region 響應按鈕事件 Button Click Event Procedure

        private void comboBox_Server_SelectedIndexChanged(object sender, EventArgs e)
        {
            #region 設定IP、Port顯示
            this.NumericUpDown_Port.Value = this.MyServerList[this.comboBox_Server.SelectedIndex].ServerPort;
            if (this.MyServerList[this.comboBox_Server.SelectedIndex].IsMaster)
            {
                this.checkBox_IsMaster.Checked = true;
            }
            else
            {
                this.checkBox_IsMaster.Checked = false;
            }

            #endregion

            #region  測試是否可鏈接 Test connect click event procedure.
            // set result to default value ..
            resultVar = ConnectResult.NONE;
            // lock controls ..
            this.FormControls(false);

            // 獲取地址 analysis server address ..
            //IPAddress[] ipadds = Dns.GetHostAddresses(this.TextBox_Server.Text.Trim());

            IPAddress[] ipadds = Dns.GetHostAddresses(this.comboBox_Server.Text.Trim());

            if (ipadds.Length > 0)
            {
                // assign ip address ..
                this.Addr = ipadds[0];

                //創建新的線程鏈接伺服器 create therad object ..
                __Connect = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(this.Connecting));

                // start thread process and timer check ..
                __Connect.Start(ipadds[0]);
                __reply.Start();
            }
            else
            {
                MessageBox.Show(this.ParseErrMsg, AssemblyInfoClass.ProductInfo, MessageBoxButtons.OK, MessageBoxIcon.Error);

                // unlock controls ..
                this.FormControls(true);
                return;
            }
            #endregion
        }

        /// <summary>
        /// Add or Update Server Info
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_AU_Click(object sender, EventArgs e)
        {
            #region New Server Instance
            RenbarServer newServer = new RenbarServer();
            newServer.ServerIP = IPAddress.Parse(this.comboBox_Server.Text.Trim());
            newServer.ServerPort = ushort.Parse(this.NumericUpDown_Port.Value.ToString().Trim());
            if (this.checkBox_IsMaster.Checked)
            {
                newServer.IsMaster = true;
            }
            else
            {
                newServer.IsMaster = false;
            }
            #endregion

            #region Update
            if (IsExist(newServer.ServerIP.ToString().Trim()))
            {
                //走更新流程？？？？？？？？？？？完善？？？？？？？？？？？？？？？？

                this.EnvSvr.UpdateServer(this.EnvSvr.ServerListFilePath, newServer);

                //MessageBox.Show("Already exist",MessageBoxButtons.OKCancel,);

                MessageBox.Show("Update succed!");
                Binding(this.EnvSvr.ServerListFilePath);
                return;
            }
            #endregion

            #region ADD New
            else
            {
                //如果不再，測試連接，不同問：仍舊添加？
                if (this.EnvSvr.AddServer(this.EnvSvr.ServerListFilePath, newServer))
                {
                    MessageBox.Show("Add Succed!");
                    Binding(this.EnvSvr.ServerListFilePath);
                    return;
                }
            }
            #endregion

        }

        /// <summary>
        /// check if the ip is already exist in the server list
        /// </summary>
        /// <param name="newip"></param>
        /// <returns></returns>
        private bool IsExist(string newip)
        {
            bool isExist = false;
            for (int i = 0; i < MyServerList.Count; i++)
            {
                if (newip == MyServerList[i].ServerIP.ToString().Trim())
                {
                    isExist = true;
                    break;
                }
            }
            return isExist;
        }


        /// <summary>
        /// Delete selected ip&port
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Delete_Click(object sender, EventArgs e)
        {
            if (this.EnvSvr.DeleteServer(this.EnvSvr.ServerListFilePath, comboBox_Server.Text.Trim()))
            {
                MessageBox.Show("Delete Succed!");
                Binding(this.EnvSvr.ServerListFilePath);
                return;
            }
        }

        /// <summary>
        /// 儲存設定 Save remote server info click event procedure.
        /// </summary>
        private void Button_Save_Click(object sender, EventArgs e)
        {
            // 保存環境變量 save environment variable ..
            if (!string.IsNullOrEmpty(this.comboBox_Server.Text) && this.NumericUpDown_Port.Value > 0)
            {
                if (this.Addr.Equals(IPAddress.None))
                {
                    IPAddress[] ipadds = Dns.GetHostAddresses(this.comboBox_Server.Text.Trim());
                    if (ipadds.Length > 0)
                    {
                        this.Addr = ipadds[0];
                    }
                    else
                    {
                        MessageBox.Show(this.ParseErrMsg, AssemblyInfoClass.ProductInfo, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
                restoreInit = false;
                this.ServerAddr = this.Addr;
                this.PortNumber = Convert.ToUInt16(this.NumericUpDown_Port.Value);
            }
        }
        #endregion
       
    }
}