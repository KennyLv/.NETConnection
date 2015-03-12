#region Using NameSpace
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Windows.Forms;

// import renbar class library namespace ..
using RenbarLib.Environment;
using RenbarLib.Environment.Forms.Customizations.Service;
using RenbarLib.Network;
using RenbarLib.Network.Protocol;
using System.Threading;
#endregion

namespace RenbarGUI.Forms
{
    public partial class Job_Form : Form
    {
        #region Declare Global Variable Section
        /************************ reference renbar class library section *******************************/
        // declare renbar client environment class object ..
        private Customization EnvCust = null;

        // declare renbar client communication class object ..
        private Communication EnvComm = null;

        // create file system class object ..
        private FileSystem EnvFile = new FileSystem();

        // create network base structure object ..
        private HostBase EnvNetBase = new HostBase();

        // declare environment setting structure object ..
        private Settings EnvSetting;
        /***********************************************************************************************/

        // create job item attribute dictionary ..
        private IDictionary<string, object> ItemAttribute = new Dictionary<string, object>();
        private string Update_Job_Group_Id = string.Empty;

        // declare submit job error message ..
        private string __sendjob_error = string.Empty;

        private string __updatejob_error = string.Empty;

        private string __parse_error = string.Empty;
        private string __parse_mulit_error = string.Empty;
        private string __requirement_error = string.Empty;
        private string __Init_error = string.Empty;
        private string __backup_file_warning = string.Empty;
        #endregion

        #region Process Render Type Enumeration
        /// <summary>
        /// Process type flag.
        /// </summary>
        private enum ProcessType
        {
            /// <summary>
            /// Launch One Per Client.
            /// </summary>
            Client,
            /// <summary>
            /// Launch One Per Processor.
            /// </summary>
            Processor
        }
        #endregion

        #region 构造函數 Form Constructor Procedure

        /// <summary>
        /// Primary renbar client job form constructor procedure.
        /// </summary>
        /// <param name="Cust">reference customize class.</param>
        /// <param name="Comm">refrernce communication class.</param>
        /// <param name="Setting">current environment setting information.</param>
        public Job_Form(ref Customization Cust, ref Communication Comm, Settings Setting)
        {
            // initialize form control component ..
            InitializeComponent();

            // assign customize class reference ..
            this.EnvCust = Cust;

            // assign communization class refrernce ..
            this.EnvComm = Comm;

            // assign environment structure ..
            this.EnvSetting = Setting;

            // initializing user interface behavior ..
            Language(Setting.Lang);
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
                    // Job Form ..
                    this.Text = this.EnvCust.GetLocalization(this.Name, this.Name, Customization.Language.En_Us);

                    // Job Properties GroupBox ..
                    this.GroupBox_Job_Properties.Text = this.EnvCust.GetLocalization(this.Name, this.GroupBox_Job_Properties.Name, Customization.Language.En_Us);

                    // Job Project Label ..
                    this.Label_Job_Project.Text = this.EnvCust.GetLocalization(this.Name, this.Label_Job_Project.Name, Customization.Language.En_Us);

                    // Job Name Label ..
                    this.Label_Job_Name.Text = this.EnvCust.GetLocalization(this.Name, this.Label_Job_Name.Name, Customization.Language.En_Us);

                    // Command Label ..
                    this.Label_Command.Text = this.EnvCust.GetLocalization(this.Name, this.Label_Command.Name, Customization.Language.En_Us);

                    // Start Label ..
                    this.Label_Start.Text = this.EnvCust.GetLocalization(this.Name, this.Label_Start.Name, Customization.Language.En_Us);

                    // End Label ..
                    this.Label_End.Text = this.EnvCust.GetLocalization(this.Name, this.Label_End.Name, Customization.Language.En_Us);

                    // Packet Size Label ..
                    this.Label_Packet_Size.Text = this.EnvCust.GetLocalization(this.Name, this.Label_Packet_Size.Name, Customization.Language.En_Us);

                    // Submit Type Label ..
                    this.Label_Submit_Type.Text = this.EnvCust.GetLocalization(this.Name, this.Label_Submit_Type.Name, Customization.Language.En_Us);

                    // Processor RadioButton ..
                    this.RadioButton_Processor.Text = this.EnvCust.GetLocalization(this.Name, this.RadioButton_Processor.Name, Customization.Language.En_Us);

                    // Client RadioButton ..
                    this.RadioButton_Client.Text = this.EnvCust.GetLocalization(this.Name, this.RadioButton_Client.Name, Customization.Language.En_Us);

                    // First Pool Label ..
                    this.Label_First_Pool.Text = this.EnvCust.GetLocalization(this.Name, this.Label_First_Pool.Name, Customization.Language.En_Us);

                    // Second Pool Label ..
                    this.Label_Second_Pool.Text = this.EnvCust.GetLocalization(this.Name, this.Label_Second_Pool.Name, Customization.Language.En_Us);

                    // Wait For Label ..
                    this.Label_Wait_For.Text = this.EnvCust.GetLocalization(this.Name, this.Label_Wait_For.Name, Customization.Language.En_Us);

                    // Note Label ..
                    this.Label_Note.Text = this.EnvCust.GetLocalization(this.Name, this.Label_Note.Name, Customization.Language.En_Us);

                    // Priority Label ..
                    this.Label_Priority.Text = this.EnvCust.GetLocalization(this.Name, this.Label_Priority.Name, Customization.Language.En_Us);

                    // Display Linklabel ..
                    this.LinkLabel_Display.Text = this.EnvCust.GetLocalization(this.Name, this.LinkLabel_Display.Name + "_Show", Customization.Language.En_Us);

                    // Alienbrain Properties GorupBox ..
                    this.GroupBox_AB_Properties.Text = this.EnvCust.GetLocalization(this.Name, this.GroupBox_AB_Properties.Name, Customization.Language.En_Us);

                    // Alienbrain Name Label ..
                    this.Label_AB_Name.Text = this.EnvCust.GetLocalization(this.Name, this.Label_AB_Name.Name, Customization.Language.En_Us);

                    // Alienbrain Node Path Lable ..
                    this.Label_AB_Path.Text = this.EnvCust.GetLocalization(this.Name, this.Label_AB_Path.Name, Customization.Language.En_Us);

                    // Update Files Only CheckBox ..
                    this.CheckBox_Update_Only.Text = this.EnvCust.GetLocalization(this.Name, this.CheckBox_Update_Only.Name, Customization.Language.En_Us);

                    // Load Button ..
                    this.Button_Load.Text = this.EnvCust.GetLocalization(this.Name, this.Button_Load.Name, Customization.Language.En_Us);

                    // Update Button…
                    this.Button_Update.Text = this.EnvCust.GetLocalization(this.Name, this.Button_Update.Name, Customization.Language.En_Us);

                    // Reset Button ..
                    this.Button_Clear.Text = this.EnvCust.GetLocalization(this.Name, this.Button_Clear.Name, Customization.Language.En_Us);

                    // Submit Button ..
                    this.Button_Submit.Text = this.EnvCust.GetLocalization(this.Name, this.Button_Submit.Name, Customization.Language.En_Us);

                    // Submit Job Error Message String ..
                    this.__sendjob_error = this.EnvCust.GetLocalization(this.Name, "Submit_Err", Customization.Language.En_Us);

                    // Load Init data Error Message String…… 
                    this.__Init_error = this.EnvCust.GetLocalization(this.Name, "Init_Err", Customization.Language.En_Us);

                    // Parse Job File Error Message ..
                    this.__parse_error = this.EnvCust.GetLocalization(string.Empty, "Parse_FileFormat_Err", Customization.Language.En_Us);

                    // Requirement Error Message ..
                    this.__requirement_error = this.EnvCust.GetLocalization(this.Name, "Requirement_Err", Customization.Language.En_Us);

                    // Parse Mulit File Error Message ..
                    this.__parse_mulit_error = this.EnvCust.GetLocalization(string.Empty, "ParseFile_Multi_Err", Customization.Language.En_Us);

                    // Backup File Warning Message ..
                    this.__backup_file_warning = this.EnvCust.GetLocalization(this.Name, "Backup_Warning", Customization.Language.En_Us);
                    break;
                #endregion

                #region Traditional Chinese
                case Customization.Language.Zh_Tw:
                    // Job Form ..
                    this.Text = this.EnvCust.GetLocalization(this.Name, this.Name, Customization.Language.Zh_Tw);

                    // Job Properties GroupBox ..
                    this.GroupBox_Job_Properties.Text = this.EnvCust.GetLocalization(this.Name, this.GroupBox_Job_Properties.Name, Customization.Language.Zh_Tw);

                    // Job Project Label ..
                    this.Label_Job_Project.Text = this.EnvCust.GetLocalization(this.Name, this.Label_Job_Project.Name, Customization.Language.Zh_Tw);

                    // Job Name Label ..
                    this.Label_Job_Name.Text = this.EnvCust.GetLocalization(this.Name, this.Label_Job_Name.Name, Customization.Language.Zh_Tw);

                    // Command Label ..
                    this.Label_Command.Text = this.EnvCust.GetLocalization(this.Name, this.Label_Command.Name, Customization.Language.Zh_Tw);

                    // Start Label ..
                    this.Label_Start.Text = this.EnvCust.GetLocalization(this.Name, this.Label_Start.Name, Customization.Language.Zh_Tw);

                    // End Label ..
                    this.Label_End.Text = this.EnvCust.GetLocalization(this.Name, this.Label_End.Name, Customization.Language.Zh_Tw);

                    // Packet Size Label ..
                    this.Label_Packet_Size.Text = this.EnvCust.GetLocalization(this.Name, this.Label_Packet_Size.Name, Customization.Language.Zh_Tw);

                    // Submit Type Label ..
                    this.Label_Submit_Type.Text = this.EnvCust.GetLocalization(this.Name, this.Label_Submit_Type.Name, Customization.Language.Zh_Tw);

                    // Processor RadioButton ..
                    this.RadioButton_Processor.Text = this.EnvCust.GetLocalization(this.Name, this.RadioButton_Processor.Name, Customization.Language.Zh_Tw);

                    // Client RadioButton ..
                    this.RadioButton_Client.Text = this.EnvCust.GetLocalization(this.Name, this.RadioButton_Client.Name, Customization.Language.Zh_Tw);

                    // First Pool Label ..
                    this.Label_First_Pool.Text = this.EnvCust.GetLocalization(this.Name, this.Label_First_Pool.Name, Customization.Language.Zh_Tw);

                    // Second Pool Label ..
                    this.Label_Second_Pool.Text = this.EnvCust.GetLocalization(this.Name, this.Label_Second_Pool.Name, Customization.Language.Zh_Tw);

                    // Wait For Label ..
                    this.Label_Wait_For.Text = this.EnvCust.GetLocalization(this.Name, this.Label_Wait_For.Name, Customization.Language.Zh_Tw);

                    // Note Label ..
                    this.Label_Note.Text = this.EnvCust.GetLocalization(this.Name, this.Label_Note.Name, Customization.Language.Zh_Tw);

                    // Priority Label ..
                    this.Label_Priority.Text = this.EnvCust.GetLocalization(this.Name, this.Label_Priority.Name, Customization.Language.Zh_Tw);

                    // Display Linklabel ..
                    this.LinkLabel_Display.Text = this.EnvCust.GetLocalization(this.Name, this.LinkLabel_Display.Name + "_Show", Customization.Language.Zh_Tw);

                    // Alienbrain Properties GorupBox ..
                    this.GroupBox_AB_Properties.Text = this.EnvCust.GetLocalization(this.Name, this.GroupBox_AB_Properties.Name, Customization.Language.Zh_Tw);

                    // Alienbrain Name Label ..
                    this.Label_AB_Name.Text = this.EnvCust.GetLocalization(this.Name, this.Label_AB_Name.Name, Customization.Language.Zh_Tw);

                    // Alienbrain Node Path Lable ..
                    this.Label_AB_Path.Text = this.EnvCust.GetLocalization(this.Name, this.Label_AB_Path.Name, Customization.Language.Zh_Tw);

                    // Update Files Only CheckBox ..
                    this.CheckBox_Update_Only.Text = this.EnvCust.GetLocalization(this.Name, this.CheckBox_Update_Only.Name, Customization.Language.Zh_Tw);

                    // Load Button ..
                    this.Button_Load.Text = this.EnvCust.GetLocalization(this.Name, this.Button_Load.Name, Customization.Language.Zh_Tw);

                    // Update Button…
                    this.Button_Update.Text = this.EnvCust.GetLocalization(this.Name, this.Button_Update.Name, Customization.Language.Zh_Tw);

                    // Reset Button ..
                    this.Button_Clear.Text = this.EnvCust.GetLocalization(this.Name, this.Button_Clear.Name, Customization.Language.Zh_Tw);

                    // Submit Button ..
                    this.Button_Submit.Text = this.EnvCust.GetLocalization(this.Name, this.Button_Submit.Name, Customization.Language.Zh_Tw);

                    // Submit Job Error Message String ..
                    this.__sendjob_error = this.EnvCust.GetLocalization(this.Name, "Submit_Err", Customization.Language.Zh_Tw);

                    // Load Init data Error Message String…… 
                    this.__Init_error = this.EnvCust.GetLocalization(this.Name, "Init_Err", Customization.Language.Zh_Tw);

                    // Parse Job File Error Message ..
                    this.__parse_error = this.EnvCust.GetLocalization(string.Empty, "Parse_FileFormat_Err", Customization.Language.Zh_Tw);

                    // Requirement Error Message ..
                    this.__requirement_error = this.EnvCust.GetLocalization(this.Name, "Requirement_Err", Customization.Language.Zh_Tw);

                    // Parse Mulit File Error Message ..
                    this.__parse_mulit_error = this.EnvCust.GetLocalization(string.Empty, "ParseFile_Multi_Err", Customization.Language.Zh_Tw);

                    // Backup File Warning Message ..
                    this.__backup_file_warning = this.EnvCust.GetLocalization(this.Name, "Backup_Warning", Customization.Language.Zh_Tw);
                    break;
                #endregion
            }
        }

        #endregion

        #region 加载初始数据 Form Load Event Procedure

        /// <summary>
        /// 首要初始加載事件 Primary loading procedure...
        /// </summary>
        private void Job_Form_Load(object sender, EventArgs e)
        {
            // assign pool, waitfor drop-down list ...

            //並未將物件參考設定為物件的執行個體——未獲取到pool等数据——Customization.cs     必須先獲取到pool、waitfor信息！

            try
            {
                foreach (KeyValuePair<string, string> kv in Pool)
                {
                    this.ComboBox_First_Pool.Items.Add(new ItemPair<string, string>(kv.Key, kv.Value));
                }
                foreach (KeyValuePair<string, string> kv in Pool2)
                {
                    this.ComboBox_Second_Pool.Items.Add(new ItemPair<string, string>(kv.Key, kv.Value));
                }
                foreach (KeyValuePair<string, string> kv in WaitFor)
                {
                    this.ComboBox_Wait_For.Items.Add(new ItemPair<string, string>(kv.Key, kv.Value));
                }

                // 插入一行新數據 insert empty items to optional controls ...
                if (this.ComboBox_Second_Pool.Items.Count > 0)
                    this.ComboBox_Second_Pool.Items.Insert(0, string.Empty);
                if (this.ComboBox_Wait_For.Items.Count > 0)
                    this.ComboBox_Wait_For.Items.Insert(0, string.Empty);

                #region F7300290 Update Info
                if (this.IsUpdate)
                {
                    #region F7300290 控制顯示

                    this.Button_Update.Visible = true;
                    this.Button_Load.Visible = false;
                    this.Button_Submit.Visible = false;

                    #endregion
                    // read update info ...
                    if (this.IsExtern && this.ExternItems.Count > 0)//加載任務時提取外部信息
                    {
                        // assign extern item attributes ...
                        this.ItemAttribute = this.ExternItems;//数据源

                        // 分配控件顯示值 assign control value ...
                        this.AssignValues();
                        this.Update_Job_Group_Id = UpdateId;
                    }
                }
                #endregion

                #region F7300290 View History Info
                else if (this.IsViewHistory)
                {
                    #region F7300290 控制顯示

                    this.Button_Update.Visible = false;
                    this.Button_Load.Visible = false;
                    this.Button_Submit.Visible = true;

                    #endregion

                    if (this.IsExtern && this.ExternItems.Count > 0)//加載任務時提取外部信息
                    {
                        // assign extern item attributes ...
                        this.ItemAttribute = this.ExternItems;//数据源

                        // 分配控件顯示值 assign control value ...
                        this.AssignValues();
                    }
                }
                #endregion

                #region （New or Load）Load File 加載外部rbr數據
                else
                {
                    #region F7300290 控制顯示

                    this.Button_Update.Visible = false;
                    this.Button_Load.Visible = true;
                    this.Button_Submit.Visible = true;

                    #endregion

                    if (this.IsExtern && !string.IsNullOrEmpty(this.BackupFilename) && this.ExternItems.Count > 0)
                    {
                        // assign extern item attributes ...
                        this.ItemAttribute = this.ExternItems;//数据源

                        // 分配控件顯示值 assign control value ...
                        this.AssignValues();
                    }
                }
                #endregion

            }
            catch
            {
                MessageBox.Show(this, this.__Init_error, AssemblyInfoClass.ProductInfo, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        #region 控件初始顯示數據 Assign Control Value
        /// <summary>
        /// Assign item attribute to control value.
        /// </summary>
        private void AssignValues()
        {
            // job name ..
            if (this.ItemAttribute.ContainsKey("name") && this.ItemAttribute["name"] != null)
                this.TextBox_Job_Name.Text = this.ItemAttribute["name"].ToString();

            // job wait for ..？？？？？？？？？？？？？？？？？？？？？？？？？？？？？撒旦法隧道股份？？？？？？？？
            if (this.ItemAttribute.ContainsKey("waitFor") && this.ItemAttribute["waitFor"] != null)
            {
                foreach (KeyValuePair<string, string> item in this.WaitFor)
                {
                    if (item.Value == this.ItemAttribute["waitFor"].ToString())
                    {
                        if (this.WaitFor.Values.Contains(this.ItemAttribute["waitFor"].ToString()))
                            this.ComboBox_Wait_For.SelectedItem = new ItemPair<string, string>(item.Key, item.Value);
                    }
                }
            }

            // alienbrain update only ..
            if (this.ItemAttribute.ContainsKey("isUpdateOnly") && this.ItemAttribute["isUpdateOnly"] != null)
            {
                if (Convert.ToBoolean(this.ItemAttribute["isUpdateOnly"]))
                    this.CheckBox_Update_Only.Checked = true;
                else
                    this.CheckBox_Update_Only.Checked = false;
            }

            // job submite type ..
            if (this.ItemAttribute.ContainsKey("type") && this.ItemAttribute["type"] != null)
            {
                if (this.ItemAttribute["type"].ToString() == "Processor")
                {
                    this.RadioButton_Processor.Checked = true;
                    this.RadioButton_Client.Checked = false;
                }

                if (this.ItemAttribute["type"].ToString() == "Client")
                {
                    this.RadioButton_Processor.Checked = false;
                    this.RadioButton_Client.Checked = true;
                }
            }

            // alienbrain project name ..
            if (this.ItemAttribute.ContainsKey("ABProjectName") && this.ItemAttribute["ABProjectName"] != null)
                this.TextBox_AB_Name.Text = this.ItemAttribute["ABProjectName"].ToString();

            // alienbrain path ..
            if (this.ItemAttribute.ContainsKey("ABNode") && this.ItemAttribute["ABNode"] != null)
                this.TextBox_AB_Path.Text = this.ItemAttribute["ABNode"].ToString();

            // job project name ..
            if (this.ItemAttribute.ContainsKey("ProjectName") && this.ItemAttribute["ProjectName"] != null)
            {
                if (string.IsNullOrEmpty(this.ItemAttribute["ABProjectName"].ToString()))
                    this.TextBox_Job_Project.Text = this.ItemAttribute["ProjectName"].ToString();
                else
                {
                    this.TextBox_Job_Project.Text = this.ItemAttribute["ABProjectName"].ToString();
                    this.TextBox_Job_Project.ReadOnly = true;
                }
            }

            // job command ..
            if (this.ItemAttribute.ContainsKey("Command") && this.ItemAttribute["Command"] != null)
                this.TextBox_Command.Text = this.ItemAttribute["Command"].ToString();

            // job first pool ..
            if (this.ItemAttribute.ContainsKey("FirstPool") && this.ItemAttribute["FirstPool"] != null)
            {
                foreach (KeyValuePair<string, string> item in this.Pool)
                {
                    if (item.Value == this.ItemAttribute["FirstPool"].ToString())
                    {
                        if (this.Pool.Values.Contains(this.ItemAttribute["FirstPool"].ToString()))
                            this.ComboBox_First_Pool.SelectedItem = new ItemPair<string, string>(item.Key, item.Value);
                    }
                }
            }

            // job second pool ..
            if (this.ItemAttribute.ContainsKey("SecondPool") && this.ItemAttribute["SecondPool"] != null)
            {
                foreach (KeyValuePair<string, string> item in this.Pool2)
                {
                    if (item.Value == this.ItemAttribute["SecondPool"].ToString())
                    {
                        if (this.Pool2.Values.Contains(this.ItemAttribute["SecondPool"].ToString()))
                            this.ComboBox_Second_Pool.SelectedItem = new ItemPair<string, string>(item.Key, item.Value);
                    }
                }
            }

            // job priority ..
            if (this.ItemAttribute.ContainsKey("Priority") && this.ItemAttribute["Priority"] != null)
            {
                if (Convert.ToInt32(this.ItemAttribute["Priority"]) > 0)
                    this.NumericUpDown_Priority.Value = Convert.ToInt32(this.ItemAttribute["Priority"]);
                else
                    this.NumericUpDown_Priority.Value = 1;
            }

            // job start frame ..
            if (this.ItemAttribute.ContainsKey("StartFrame") && this.ItemAttribute["StartFrame"] != null)
                this.NumericUpDown_Start.Value = Convert.ToInt32(this.ItemAttribute["StartFrame"]);

            // job end frame ..
            if (this.ItemAttribute.ContainsKey("EndFrame") && this.ItemAttribute["EndFrame"] != null)
                this.NumericUpDown_End.Value = Convert.ToInt32(this.ItemAttribute["EndFrame"]);

            // job packet size ..
            if (this.ItemAttribute.ContainsKey("PacketSize") && this.ItemAttribute["PacketSize"] != null)
                this.NumericUpDown_Packet_Size.Value = Convert.ToInt32(this.ItemAttribute["PacketSize"]);

            // job note ..
            if (this.ItemAttribute.ContainsKey("Note") && this.ItemAttribute["Note"] != null)
                this.TextBox_Note.Text = this.ItemAttribute["Note"].ToString();
        }
        #endregion

        #region 內部參數 Form Parameters
        /// <summary>
        /// Get or set first pool list.
        /// </summary>
        internal IDictionary<string, string> Pool
        {
            get;
            set;
        }

        /// <summary>
        /// Get or set second pool list.
        /// </summary>
        internal IDictionary<string, string> Pool2
        {
            get;
            set;
        }

        /// <summary>
        /// Get or set waitfor job list.
        /// </summary>
        internal IDictionary<string, string> WaitFor
        {
            get;
            set;
        }

        /// <summary>
        /// Get or set extern item attribute.
        /// </summary>
        internal IDictionary<string, object> ExternItems
        {
            get;
            set;
        }

        /// <summary>
        /// Get or set backup filename.
        /// </summary>
        internal string BackupFilename
        {
            get;
            set;
        }

        /// <summary>
        /// Get or set extern access.
        /// </summary>
        internal bool IsExtern
        {
            get;
            set;
        }

        /// <summary>
        /// Get or set update mode flag.
        /// </summary>
        internal bool IsUpdate
        {
            get;
            set;
        }

        /// <summary>
        /// Get or set ViewHistory mode flag.
        /// </summary>
        internal bool IsViewHistory
        {
            get;
            set;
        }

        /// <summary>
        /// Get or set update job id.
        /// </summary>
        internal string UpdateId
        {
            get;
            set;
        }
        #endregion

        #endregion

        #region 关闭事件 Form Closing Event Procedure
        private void Job_Form_FormClosing(object sender, FormClosingEventArgs e)
        {
            // return correct dialog result ..
            this.DialogResult = DialogResult.OK;
        }
        #endregion

        #region 更改任务优先级(1——100)，控制滚动条与数字框显示一致 Priority Changed Event Procedure
        /// <summary>
        /// Priority scroll change.
        /// </summary>
        private void TrackBar_Priority_Scroll(object sender, EventArgs e)
        {
            // assign new value ..
            this.NumericUpDown_Priority.Value = (decimal)this.TrackBar_Priority.Value;
        }

        /// <summary>
        /// Priority keyup completed.
        /// </summary>
        private void NumericUpDown_Priority_KeyUp(object sender, KeyEventArgs e)
        {
            // assign new value ..
            this.TrackBar_Priority.Value = (int)this.NumericUpDown_Priority.Value;
        }

        /// <summary>
        /// Priority click up or down button.
        /// </summary>
        private void NumericUpDown_Priority_ValueChanged(object sender, EventArgs e)
        {
            // assign new value ..
            this.TrackBar_Priority.Value = (int)this.NumericUpDown_Priority.Value;
        }
        #endregion

        #region 控制AB_Name的顯示隐藏 Display Alienbrain Options Event Procedure

        private void LinkLabel_Display_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (this.Size.Height.Equals(439) || this.Size.Height.Equals(440) || this.Size.Height.Equals(441))
            {
                switch (EnvSetting.Lang)
                {
                    case Customization.Language.En_Us:
                        this.LinkLabel_Display.Text = this.EnvCust.GetLocalization(this.Name, this.LinkLabel_Display.Name + "_Hide", Customization.Language.En_Us);
                        break;
                    case Customization.Language.Zh_Tw:
                        this.LinkLabel_Display.Text = this.EnvCust.GetLocalization(this.Name, this.LinkLabel_Display.Name + "_Hide", Customization.Language.Zh_Tw);
                        break;
                }

                // reset drawing form size ..
                this.Size = new global::System.Drawing.Size(630, 625);
            }
            else
            {
                switch (EnvSetting.Lang)
                {
                    case Customization.Language.En_Us:
                        this.LinkLabel_Display.Text = this.EnvCust.GetLocalization(this.Name, this.LinkLabel_Display.Name + "_Show", Customization.Language.En_Us);
                        break;
                    case Customization.Language.Zh_Tw:
                        this.LinkLabel_Display.Text = this.EnvCust.GetLocalization(this.Name, this.LinkLabel_Display.Name + "_Show", Customization.Language.Zh_Tw);
                        break;
                }

                // reset drawing form size ..
                this.Size = new global::System.Drawing.Size(630, 440);
            }
        }
        #endregion

        #region 檢驗數據是否有更改！映射物件属性&控件取值 Controls Value Change Event Procedure
        /// <summary>
        /// Confirm data attributes consistence
        /// </summary>
        private void Mapping()
        {
            // job project and alienbrain project name ..
            if (!string.IsNullOrEmpty(this.TextBox_AB_Name.Text.Trim()))
            {
                this.ItemAttribute["ProjectName"] = this.TextBox_AB_Name.Text.Trim();
                this.ItemAttribute["ABProjectName"] = this.TextBox_AB_Name.Text.Trim();
            }
            else
            {
                if (this.TextBox_Job_Project.Text.Trim() != this.ItemAttribute["ProjectName"].ToString())
                    this.ItemAttribute["ProjectName"] = this.TextBox_Job_Project.Text.Trim();
            }

            // alienbrain path (node) ..
            if (this.TextBox_AB_Path.Text.Trim() != this.ItemAttribute["ABNode"].ToString())
                this.ItemAttribute["ABNode"] = this.TextBox_AB_Path.Text.Trim();

            // alienbrain update only ..
            if (this.CheckBox_Update_Only.Checked)
                this.ItemAttribute["isUpdateOnly"] =1;
            else
                this.ItemAttribute["isUpdateOnly"] = 0;

            // job name ..
            if (this.TextBox_Job_Name.Text.Trim() != this.ItemAttribute["name"].ToString())
                this.ItemAttribute["name"] = this.TextBox_Job_Name.Text.Trim();

            // job command ..
            if (this.TextBox_Command.Text.Trim() != this.ItemAttribute["Command"].ToString())
                this.ItemAttribute["Command"] = this.TextBox_Command.Text.Trim();

            // job note ..
            if (this.TextBox_Note.Text.Trim() != this.ItemAttribute["Note"].ToString())
                this.ItemAttribute["Note"] = this.TextBox_Note.Text.Trim();

            // job process type ..
            if (this.RadioButton_Processor.Checked)
                this.ItemAttribute["type"] = "Processor";
            else
                this.ItemAttribute["type"] = "Client";

            // job first pool ..
            if (this.ComboBox_First_Pool.SelectedIndex > -1)
            {
                this.ItemAttribute["FirstPool"] = ((ItemPair<string, string>)this.ComboBox_First_Pool.SelectedItem).Value;
            }

            // job second pool ..
            if (this.ComboBox_Second_Pool.SelectedIndex > 0)
            {
                this.ItemAttribute["SecondPool"] = ((ItemPair<string, string>)this.ComboBox_Second_Pool.SelectedItem).Value;
            }
            else
                this.ItemAttribute["SecondPool"] = string.Empty;

            // job wait for ..
            if (this.ComboBox_Wait_For.SelectedIndex > 0)
            {
                //this.ItemAttribute["waitFor"] = ((ItemPair<string, string>)this.ComboBox_Wait_For.SelectedItem).Value;
                this.ItemAttribute["waitFor"] = ((ItemPair<string, string>)this.ComboBox_Wait_For.SelectedItem).Key;
            }
            else
                this.ItemAttribute["waitFor"] = string.Empty;

            // job start frame ..
            if (this.NumericUpDown_Start.Value.ToString() != this.ItemAttribute["StartFrame"].ToString())
            {
                this.ItemAttribute["StartFrame"] = this.NumericUpDown_Start.Value.ToString();
            }

            // job end frame ..
            if (this.NumericUpDown_End.Value.ToString() != this.ItemAttribute["EndFrame"].ToString())
            {
                this.ItemAttribute["EndFrame"] = this.NumericUpDown_End.Value.ToString();
            }

            // job packet size ..
            if (this.NumericUpDown_Packet_Size.Value.ToString() != this.ItemAttribute["PacketSize"].ToString())
                this.ItemAttribute["PacketSize"] = this.NumericUpDown_Packet_Size.Value.ToString();

            // job priority ..
            if (this.NumericUpDown_Priority.Value.ToString() != this.ItemAttribute["Priority"].ToString())
                this.ItemAttribute["Priority"] = this.NumericUpDown_Priority.Value.ToString();
        }
        #endregion

        #region ？TextBox_AB_Name内容改变 Project Name TextChnaged Event Procedure
        /// <summary>
        /// 保持專案名稱與TextBox_AB_Name的內容同步 Synchronization project name.
        /// </summary>
        private void TextBox_AB_Name_TextChanged(object sender, EventArgs e)
        {
            // if alienbrain project name is not empty, the job project and alienbrain project name equal.
            this.TextBox_Job_Project.Text = this.TextBox_AB_Name.Text.Trim();
        }
        #endregion

        #region 任务操作枚舉添加/更新 Submit Job Procedure
        /// <summary>
        /// 提交任務動作枚舉 Submit job action enumeration.
        /// </summary>
        private enum SubmitAction
        {
            /// <summary>
            /// Add job flag.
            /// </summary>
            Add,
            /// <summary>
            /// Update job flag.
            /// </summary>
            Update
        }
        #endregion

        #region 轉化數據為Server端可接受的格式

        /// <summary>
        /// 修訂队列数据 Revise queue item data.
        /// </summary>
        /// <param name="OrgData">convert data object.</param>
        /// <returns>System.Collection.IDictionary</returns>
        private IDictionary<string, object> Revise(IDictionary<string, object> OrgData)
        {
            IDictionary<string, object> Item = new Dictionary<string, object>();

            if (OrgData.ContainsKey("name"))
                Item.Add("Name", OrgData["name"].ToString());
            if (OrgData.ContainsKey("waitFor"))
                Item.Add("WaitFor", OrgData["waitFor"].ToString());
            if (OrgData.ContainsKey("isUpdateOnly"))
                Item.Add("ABUpdateOnly", OrgData["isUpdateOnly"].ToString());
            if (OrgData.ContainsKey("type"))
                Item.Add("Proc_Type", OrgData["type"].ToString());
            if (OrgData.ContainsKey("ABProjectName"))
                Item.Add("ABName", OrgData["ABProjectName"].ToString());
            if (OrgData.ContainsKey("ABNode"))
                Item.Add("ABPath", OrgData["ABNode"].ToString());
            if (OrgData.ContainsKey("ProjectName"))
                Item.Add("Project", OrgData["ProjectName"].ToString());
            if (OrgData.ContainsKey("Command"))
                Item.Add("Command", OrgData["Command"].ToString());

            //修訂的地方
            if (OrgData.ContainsKey("FirstPool"))
                Item.Add("First_Pool", ((ItemPair<string, string>)this.ComboBox_First_Pool.SelectedItem).Key);

            if (OrgData.ContainsKey("SecondPool"))
            {
                if (this.ComboBox_Second_Pool.SelectedIndex > 0)
                    Item.Add("Second_Pool", ((ItemPair<string, string>)this.ComboBox_Second_Pool.SelectedItem).Key);
                else
                    Item.Add("Second_Pool", null);
            }

            if (OrgData.ContainsKey("Priority"))
                Item.Add("Priority", OrgData["Priority"].ToString());
            if (OrgData.ContainsKey("StartFrame"))
                Item.Add("Start", OrgData["StartFrame"].ToString());
            if (OrgData.ContainsKey("EndFrame"))
                Item.Add("End", OrgData["EndFrame"].ToString());
            if (OrgData.ContainsKey("PacketSize"))
                Item.Add("Packet_Size", OrgData["PacketSize"].ToString());
            if (OrgData.ContainsKey("Note"))
                Item.Add("Note", OrgData["Note"].ToString());

            // 增加的地方 confirm requirement field ..
            if (!OrgData.ContainsKey("Submit_Machine"))
                Item.Add("Submit_Machine", this.EnvNetBase.LocalHostName);
            if (!OrgData.ContainsKey("Submit_Acct"))
                Item.Add("Submit_Acct", Customization.User);
            if (!OrgData.ContainsKey("Submit_Time"))
                Item.Add("Submit_Time", DateTime.Now);

            return Item;
        }

        #endregion

        #region 提交數據
        /// <summary>
        /// 增加或更新任務 Add or update job.
        /// </summary>
        /// <param name="Action">操作動作submit job behaivor.</param>
        /// <param name="Items">操作對象communication enumerable item.</param>
        /// <returns>System.Boolean是否成功</returns>
        private bool Connect(SubmitAction Action, IDictionary<string, object> Items)//？？？？？？？？？？？？？？？？？？？？？？？？
        {
            bool result = false;

            // declare remote server response object ..
            KeyValuePair<string, object> responseObject;

            // 修訂數據 revise item data ..
            IDictionary<string, object> QueueItem = this.Revise(Items);


            if (QueueItem.Count == 0)
            {
                return result;
            }

            do
            {
                // confirm current resquest status ..
                if (this.EnvComm.CanRequest)
                {
                    // declare package sent data type ..
                    IList<object> packaged = null;

                    switch (Action)
                    {
                        case SubmitAction.Add:
                            // assign new status ..
                            if (!QueueItem.ContainsKey("Status"))
                            {
                                QueueItem.Add("Status", (UInt16)JobStatusFlag.QUEUING);
                            }

                            //  打包物件 package sent data type ..
                            packaged = this.EnvComm.Package(Client2Server.CommunicationType.JOBQUEUEADD, QueueItem);
                            break;

                        case SubmitAction.Update:
                            if (!QueueItem.ContainsKey("Job_Group_Id"))
                            {
                                QueueItem.Add("Job_Group_Id", this.Update_Job_Group_Id);
                            }
                            // package sent data type ..
                            packaged = this.EnvComm.Package(Client2Server.CommunicationType.JOBQUEUEUPDATE, QueueItem);
                            break;
                    }

                    // 發送數據並等待執行結果 wait for result ..
                    responseObject = this.EnvComm.Request(packaged);

                    break;
                }
                Thread.Sleep(1000);
            } while (true);

            // 確認傳輸是否成功 confirm correct result ..
            if (responseObject.Key.Substring(0, 1).Equals("+"))
                result = true;

            return result;
        }
        #endregion

        #region 按钮事件 Form Button Click Event Procedure
        /// <summary>
        /// 提交任务 Submit job.
        /// </summary>
        private void Button_Submit_Click(object sender, EventArgs e)
        {
            // 驗證數據
            if (string.IsNullOrEmpty(this.TextBox_Job_Project.Text.Trim()) || string.IsNullOrEmpty(this.TextBox_Job_Name.Text.Trim())
                || string.IsNullOrEmpty(this.TextBox_Command.Text.Trim()) || this.ComboBox_First_Pool.SelectedIndex == -1)
            {
                MessageBox.Show(this, this.__requirement_error, AssemblyInfoClass.ProductInfo,
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
            }
            else
            {
                //////////////// 不接受自己輸入的東西？？？
                if (this.ItemAttribute.Count > 0)
                {
                    // 添加數據到ItemAttribute   mapping attribute values ..
                    this.Mapping();

                    // 發送數據失敗 sent job to server ..無法下算此工作(Job), 請聯繫 IT 部門 !
                    if (!this.Connect(SubmitAction.Add, this.ItemAttribute))//包含發送動作
                    {
                        MessageBox.Show(this, this.__sendjob_error, AssemblyInfoClass.ProductInfo,
                            MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    }
                    else
                    {
                        if (!this.IsViewHistory)
                        {
                            if (this.ItemAttribute["isUpdateOnly"].ToString().Trim() == "1")
                            {
                                this.ItemAttribute["isUpdateOnly"] = "True";
                            }
                            else
                            {
                                this.ItemAttribute["isUpdateOnly"] = "False";
                            }
                            // 成功則進行備份 backup to file ...
                            if (!this.EnvFile.Create(this.BackupFilename, FileSystem.RenderMethod.Gui, this.ItemAttribute))
                            {
                                MessageBox.Show(this, this.__backup_file_warning, AssemblyInfoClass.ProductInfo,
                                    MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                            }
                        }
                    }
                    this.ItemAttribute.Clear();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("暫不接受用戶自定工作！");
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Update_Click(object sender, EventArgs e)
        {
            // 驗證數據
            if (string.IsNullOrEmpty(this.TextBox_Job_Project.Text.Trim()) || string.IsNullOrEmpty(this.TextBox_Job_Name.Text.Trim())
                || string.IsNullOrEmpty(this.TextBox_Command.Text.Trim()) || this.ComboBox_First_Pool.SelectedIndex == -1)
            {
                MessageBox.Show(this, this.__requirement_error, AssemblyInfoClass.ProductInfo,
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
            }
            else
            {
                if (this.ItemAttribute.Count > 0)
                {
                    // 添加數據到ItemAttribute   mapping attribute values ..
                    this.Mapping();
                    if (!this.Connect(SubmitAction.Update, this.ItemAttribute))
                    {
                        MessageBox.Show(this, this.__updatejob_error, AssemblyInfoClass.ProductInfo,
                            MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    }
                    else
                    {
                        MessageBox.Show("Succed!");
                    }
                    this.ItemAttribute.Clear();
                    this.Close();
                }
            }
        }

        /// <summary>
        /// 加载已保存的render文件 Load saved render file.
        /// </summary>
        private void Button_Load_Click(object sender, EventArgs e)
        {
            // 重設物件屬性 reset item attribute count ..
            this.ItemAttribute.Clear();

            // open file dialog ..
            OpenFileDialog LoadFile = this.EnvCust.OpenFileDialog;

            if (LoadFile.ShowDialog() == DialogResult.OK)
            {
                // 解析文件結構 parse file structure ..（0 = Success, 1 = Parse Error, 2 = Multi File Error）
              
                int _p = this.EnvCust.Parse(LoadFile.FileName, ref this.ItemAttribute);

                switch (_p)
                {
                    //成功：結果存入引用地址中、清空原始數據、顯示新數據
                    case 0:
                        // clear old data control ..
                        this.Button_Clear_Click(null, null);

                        // assign backup filename path ..
                        this.BackupFilename = LoadFile.FileName;//？？？？？？？？？備份？？？？？？？？？？？？？？？？？？？？？？？

                        // assign control value ..
                        this.AssignValues();
                        break;

                    case 1:
                        // show parse error message ..
                        MessageBox.Show(this, this.__parse_error, AssemblyInfoClass.ProductInfo,
                            MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                        break;

                    case 2:
                        // show parse multi error message ..
                        MessageBox.Show(this, this.__parse_mulit_error, AssemblyInfoClass.ProductInfo,
                            MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                        break;
                }
            }
        }

        /// <summary>
        /// 清空数据显示 Clear current field info; reset new job default.
        /// </summary>
        private void Button_Clear_Click(object sender, EventArgs e)
        {
            // 恢復控件默認值 restore control default ..
            this.TextBox_AB_Name.Text = string.Empty;
            this.TextBox_AB_Path.Text = string.Empty;
            this.TextBox_Command.Text = string.Empty;
            this.TextBox_Job_Name.Text = string.Empty;
            this.TextBox_Job_Project.Text = string.Empty;
            this.TextBox_Note.Text = string.Empty;
            this.NumericUpDown_Start.Value = 0;
            this.NumericUpDown_End.Value = 0;
            this.NumericUpDown_Packet_Size.Value = 1;
            this.NumericUpDown_Priority.Value = 20;
            this.ComboBox_First_Pool.SelectedIndex = -1;
            this.ComboBox_Second_Pool.SelectedIndex = -1;
            this.ComboBox_Wait_For.SelectedIndex = -1;
            this.RadioButton_Processor.Checked = true;
            this.RadioButton_Client.Checked = false;
            this.CheckBox_Update_Only.Checked = false;
        }
       
        #endregion
    }
}