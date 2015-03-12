#region Using NameSpace
using System;
using System.Net;
using System.Windows.Forms;

// import renbar class library namespace ..
using RenbarLib.Environment;
//using RenbarLib.Environment.Forms.Customizations.Service;
using RenbarLib.Network.Sockets;
#endregion

namespace RenderServerGUI
{
    public partial class IP_Form : Form
    {
       
        #region Declare Global Variable Section
        /************************ reference renbar class library section *******************************/
        // create renbar client environment class object ..
        
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

        #region Connect Result Enumeration
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

        #region Form Constructor Procedure
        /// <summary>
        /// Primary renbar client net form constructor procedure.
        /// </summary>
        /// <param name="Setting">current environment setting information.</param>
        public IP_Form()
        {
            // initialize form control component ..
            InitializeComponent();
            // save to temporary variable ..
            this.InitIpAddr = this.TextBox_Server.Text.Trim();
            this.InitPort = this.NumericUpDown_Port.Value.ToString();

            // registy connect reply event ..
            this.__reply.Tick += new EventHandler(__reply_Tick);
        }
        #endregion

        #region Form Closing Event Procedure
        private void Net_Form_FormClosing(object sender, FormClosingEventArgs e)
        {
            // stop connect and timer ..
            if (this.__Connect != null)
            {
                this.__reply.Stop();
                this.__Connect.Abort();
            }

            if (this.restoreInit)
            {
                // restore original address and port ..
                if (!string.IsNullOrEmpty(this.InitIpAddr))
                    this.ServerAddr = IPAddress.Parse(this.InitIpAddr);
                else
                    this.ServerAddr = IPAddress.None;

                this.PortNumber = Convert.ToUInt16(this.InitPort);
            }

            // return correct dialog result ..
            this.DialogResult = DialogResult.OK;
        }
        #endregion

        #region Event Trigger Procedure
        /// <summary>
        /// Reply timer tick event procedure.
        /// </summary>
        private void __reply_Tick(object sender, EventArgs e)
        {
            if (!__Connect.IsAlive)
            {
                __reply.Stop();
                this.FormControls(true);
            }
        }

        /// <summary>
        /// Enable or disable form controls behaivor.
        /// </summary>
        /// <param name="IsEnable">enable form component.</param>
        private void FormControls(bool IsEnable)
        {
            switch (resultVar)
            {
                case ConnectResult.SUCCESS:
                    ConnectOkMsg = "Success,Congratulations!";
                    MessageBox.Show(this.ConnectOkMsg, AssemblyInfoClass.ProductInfo,
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    break;

                case ConnectResult.FAILCONNECT:
                    ConnectErrMsg = "Fail,Please try again!";
                    MessageBox.Show(this.ConnectErrMsg, AssemblyInfoClass.ProductInfo,
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    break;
            }

            if (!IsEnable)
            {
                this.TextBox_Server.Enabled = false;
                this.NumericUpDown_Port.Enabled = false;
                this.Button_Connect.Enabled = false;
                

                this.Label_Connect.Visible = true;
                this.PictureBox_Connect.Visible = true;
                this.Update();
            }
            else
            {
                this.TextBox_Server.Enabled = true;
                this.NumericUpDown_Port.Enabled = true;
                this.Button_Connect.Enabled = true;
            

                this.Label_Connect.Visible = false;
                this.PictureBox_Connect.Visible = false;
                this.Update();
            }
        }

        /// <summary>
        /// Connect remote server thread event.
        /// </summary>
        /// <param name="address">test address.</param>
        private void Connecting(object address)
        {
            // create connect remote machine class object instance ..
            ScanPort runConnect = new ScanPort();

            // scanning ..
            if (!runConnect.Scan((IPAddress)address, (ushort)this.NumericUpDown_Port.Value))
                resultVar = ConnectResult.FAILCONNECT;
            else
                resultVar = ConnectResult.SUCCESS;
        }
        #endregion

        #region Network Connected Properties
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

        #region Button Click Event Procedure
        /// <summary>
        /// Test connect click event procedure.
        /// </summary>
        private void Button_Connect_Click(object sender, EventArgs e)
        {
            // set result to default value ..
            resultVar = ConnectResult.NONE;

            // lock controls ..
            this.FormControls(false);

            // analysis server address ..
            IPAddress[] ipadds = Dns.GetHostAddresses(this.TextBox_Server.Text.Trim());
            if (ipadds.Length > 0)
            {
                // assign ip address ..
                this.Addr = ipadds[0];

                // create therad object ..
                __Connect = new System.Threading.Thread(
                    new System.Threading.ParameterizedThreadStart(this.Connecting));

                // start thread process and timer check ..
                __Connect.Start(ipadds[0]);
                __reply.Start();
            }
            else
            {
                MessageBox.Show(this.ParseErrMsg, AssemblyInfoClass.ProductInfo,
                    MessageBoxButtons.OK, MessageBoxIcon.Error);

                // unlock controls ..
                this.FormControls(true);

                return;
            }
        }

        /// <summary>
        /// Save remote server info click event procedure.
        /// </summary>
        private void Button_Save_Click(object sender, EventArgs e)
        {
            // save environment variable ..
            if (!string.IsNullOrEmpty(this.TextBox_Server.Text) &&
                this.NumericUpDown_Port.Value > 0)
            {
                if (this.Addr.Equals(IPAddress.None))
                {
                    IPAddress[] ipadds = Dns.GetHostAddresses(this.TextBox_Server.Text.Trim());
                    if (ipadds.Length > 0)
                        this.Addr = ipadds[0];
                    else
                    {
                        MessageBox.Show(this.ParseErrMsg, AssemblyInfoClass.ProductInfo,
                            MessageBoxButtons.OK, MessageBoxIcon.Error);

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