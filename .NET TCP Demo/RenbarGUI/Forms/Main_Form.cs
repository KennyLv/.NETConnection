#region Using NameSpace
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Media;
using System.Threading;
using System.Windows.Forms;

// import renbar class library namespace ..
using RenbarLib.Environment;
using RenbarLib.Environment.Forms.Controls;
using RenbarLib.Environment.Forms.Controls.ListView.Sort;
using RenbarLib.Environment.Forms.Customizations.Service;
using RenbarLib.Network;
using RenbarLib.Network.Protocol;
using RenbarLib.Network.Sockets;

#endregion

namespace RenbarGUI.Forms
{
    public partial class Main_Form : Form
    {
        #region Declare Global Variable Section
        /************************ reference renbar class library section *******************************/
        // create list view header column sort class object ..
        private ListViewColumnSorter ProcListViewSorter = new ListViewColumnSorter();

        private ListViewColumnSorter QueuListViewSorter = new ListViewColumnSorter();

        private ListViewColumnSorter HostListViewSorter = new ListViewColumnSorter();

        // create notification control component ..
        private Notification RenbarNotifier = new Notification();

        // create renbar client communication class object ..
        private Communication EnvComm = new Communication();

        // create renbar log base class object ..
        private Log EnvLog = new Log();

        // create renbar client environment class object ..
        private Customization EnvCust = new Customization();

        // create renbar environment class object ..
        private Service EnvSvr = new Service();//取得时间、处理器等讯息的公共服务类

        // create client setting environment structure object ..
        private Settings EnvSetting = new Settings();//相关环境参数设定（语言、IP、Port）

        // create network base structure object ..
        private HostBase EnvNetBase = new HostBase();

        /***********************************************************************************************/

        // create server taskbar notify component ..
        private NotifyIcon ClientNotify = new NotifyIcon();

        // create sound player object instance ..
        private SoundPlayer nPlayer = null;//音乐

        //F7300290
        private delegate void ShowTestTimeCallBack();

        // declare object access delegate ..
        private delegate void EnableControlsCallBack();
        private delegate void DisableControlsCallBack();
        private delegate void ProcListViewCallBack(ListViewItem Item, string DeleteItemText);
        private delegate void QueueListViewCallBack(ListViewItem Item, string DeleteItemText);
        private delegate void HostListViewCallBack(ListViewItem Item, string DeleteItemText);

        // create list view language localization text array ..
        private string[] ListViewText = new string[3];

        // declare data list thread ..
        private Thread ViewStateTherad = null;

        // declare suspend refresh signal object ..
        private ManualResetEvent Suspend = new ManualResetEvent(true);

        // create process job log dictionary ..
        internal static volatile IDictionary<string, string> ProcLog = new Dictionary<string, string>();

        // create output forms collection ..
        internal static volatile IList<string> OutputForms = new List<string>();

        // create connect server label control ..
        private ToolStripStatusLabel serverLabel = new ToolStripStatusLabel();

        // declare connected flag ..
        private bool IsConnected = false;

        // declare manual delete job item flag ..
        private bool manual_Delete = false;

        // declare suspend refresh listview timespan ..
        private bool __suspend = false;

        // stop state thread flag ..
        private volatile bool requeststop = false;

        // declare check due timer component ..
        private global::System.Windows.Forms.Timer DueTimer = new global::System.Windows.Forms.Timer();

        // declare the application close confirm message ..
        private string AppCloseMessage = string.Empty;
        // declare load job file error message ..
        private string ParseFileErrMessage = string.Empty;
        // declare parse mulit file error message ..
        private string MulitFileErrMessage = string.Empty;
        // declare delete job error message ..
        private string DeleteJobErrMessage = string.Empty;
        // declare renbar due date message ..
        private string DueDateMessage = string.Empty;
        // declare add machines message ..
        private string AddMachinesErrMessage = string.Empty;
        // declare remove machines message ..
        private string RemoveMachinesErrMessage = string.Empty;

        // declare notification title text string ..
        private string Notification_Title = string.Empty;
        // declare notification completed text string ..
        private string Notification_Completed = string.Empty;
        // declare notification error text string ..
        private string Notification_Error = string.Empty;

        //private string ServerConnectingMes = string.Empty;
        //private string ServerConnectedMes = string.Empty;
        //private string ServerDisConnectedMes = string.Empty;

        //
        private string Queue_Job_Pause = string.Empty;
        private string Queue_Job_Resume = string.Empty;

        #endregion

        #region 構造（析構）函數Form Constructor And Destructor Procedure
        /// <summary>
        /// Primary renbar client main form constructor procedure.
        /// </summary>
        public Main_Form()
        {
            // initialize form control component ..
            InitializeComponent();

            #region 設定任务完成消息通告
            // initialize notification class ..
            this.RenbarNotifier.SetBackgroundBitmap(Properties.Resources.notifyskin, Color.FromArgb(255, 0, 255));
            this.RenbarNotifier.SetCloseBitmap(Properties.Resources.close, Color.FromArgb(255, 0, 255), new Point(160, 8));
            this.RenbarNotifier.TitleRectangle = new Rectangle(10, 5, 170, 20);
            this.RenbarNotifier.ContentRectangle = new Rectangle(7, 12, 170, 120);

            // setting notification behaivor ..
            this.RenbarNotifier.CloseClickable = true;
            this.RenbarNotifier.TitleClickable = false;
            this.RenbarNotifier.ContentClickable = false;
            this.RenbarNotifier.EnableSelectionRectangle = false;
            this.RenbarNotifier.KeepVisibleOnMousOver = true;
            this.RenbarNotifier.ReShowOnMouseOver = true;

            // setting style color ..
            this.RenbarNotifier.NormalTitleColor = Color.White;
            this.RenbarNotifier.HoverTitleColor = Color.White;
            this.RenbarNotifier.NormalTitleFont = new Font("Arial", 9f, FontStyle.Bold);
            this.RenbarNotifier.NormalContentColor = Color.Black;
            this.RenbarNotifier.HoverContentColor = Color.Black;
            this.RenbarNotifier.NormalContentFont = new Font("Tahoma", 8f);
            this.RenbarNotifier.HoverContentFont = new Font("Tahoma", 8f);

            // register notification content click event ..
            this.RenbarNotifier.ContentClick += new EventHandler(RenbarNotifier_ContentClick);

            // create notification wave object ..
            this.nPlayer = new SoundPlayer(global::RenbarGUI.Properties.Resources.notification);

            // async load ..
            nPlayer.LoadAsync();
            #endregion

            // setting due timer component ...
            this.DueTimer.Interval = 5000;
            this.DueTimer.Tick += new EventHandler(DueTimer_Tick);

            // setting form title and icon ..
            this.Icon = global::RenbarGUI.Properties.Resources.client;

            // assign default windows login user ..
            Customization.User = Environment.UserName;
        }

        /// <summary>
        /// Primary mainForm destructor procedure.
        /// </summary>
        ~Main_Form()
        {
            // clean all resource ..
            this.Dispose(true);
        }
        #endregion

        #region 窗體加載 Form Load Event Procedure
        /// <summary>
        /// Primary loading procedure.
        /// </summary>
        private void Main_Load(object sender, EventArgs e)
        {
            // settings notify component properties ..
            //設定工作列
            this.ClientNotify.BalloonTipIcon = ToolTipIcon.Info;
            this.ClientNotify.Text = AssemblyInfoClass.ProductInfo;
            this.ClientNotify.Icon = global::RenbarGUI.Properties.Resources.client;
            this.ClientNotify.DoubleClick += new EventHandler(ClientNotify_DoubleClick);

            // 加載環境設定 restore environment setting ..
            this.EnvSetting = this.EnvCust.Restore();

            //設定語言
            // cancel all checked items ...
            foreach (ToolStripMenuItem Item in this.Menu_Settings_Lang.DropDownItems)
            {
                Item.Checked = false;
            }
            // select correct language item ..
            switch (this.EnvSetting.Lang)
            {
                case Customization.Language.En_Us:
                    this.Menu_Settings_Lang_Eng.Checked = true;
                    break;

                case Customization.Language.Zh_Tw:
                    this.Menu_Settings_Lang_Cht.Checked = true;
                    break;
            }

            // initializing user interface behavior ..
            //設定ListView的顯示方式，初始化用戶界面
            InitListViewHeader();
            Language();

            // show notify component ..
            this.ClientNotify.Visible = true;

            // comfirm due date ..
            this.DueTimer.Enabled = true;

            // assign list view text to array ..
            //記錄GroupBox的標題信息？？（用于顯示數據個數）無此必要？？？？？？？？
            this.ListViewText[0] = this.GroupBox_Process.Text.Trim();
            this.ListViewText[1] = this.GroupBox_Queue.Text.Trim();
            this.ListViewText[2] = this.GroupBox_Host.Text.Trim();

            // 鏈接遠端伺服器、加載頁面展示數據 connect server ..
            new Thread(delegate()
            {
                if (this.EnvSetting.ServerIpAddress == null && this.EnvSetting.ServerPort == 0)
                {
                    return;
                }
                else
                {
                    // connect remote server ..
                    this.Connect();
                }
            }).Start();
        }

        /// <summary>
        /// 設定ListView的展示效果 Setting list view header behavior.
        /// </summary>
        private void InitListViewHeader()
        {
            // process list view ..
            this.ListView_Process.AutoArrange = true;//確保 ListView 控制項中所有的項目都自動排列，以免在執行階段時重疊
            //如果 HeaderStyle 屬性設定為 ColumnHeaderStyle.Clickable，資料行行首的作用就像按鈕，使用者按一下即可執行動作，
            //例如使用按下之資料行中的項目當做索引鍵，為 ListView 控制項中的項目排序。
            //您可以在處理常式中為 ColumnClick 事件，實作這種行為。
            this.ListView_Process.HeaderStyle = ColumnHeaderStyle.Clickable;
            this.ListView_Process.View = View.Details;//Details 模式以多行顯示項目

            #region 自定義的排序方法排序
            this.ListView_Process.ListViewItemSorter = this.ProcListViewSorter;
            this.ListView_Process.Sorting = SortOrder.Ascending;
            #endregion

            this.ListView_Process.Scrollable = true;
            //FullRowSelect 屬性通常使用的情形是當 ListView 顯示具有許多子項目的項目時，
            //而由於控制項內容作水平捲動而導致項目文字不可見時，必須要能夠看得見已選取項目
            this.ListView_Process.FullRowSelect = true;

            // queue list view ..
            this.ListView_Queue.AutoArrange = true;
            this.ListView_Queue.HeaderStyle = ColumnHeaderStyle.Clickable;
            this.ListView_Queue.View = View.Details;
            this.ListView_Queue.ListViewItemSorter = this.QueuListViewSorter;
            this.ListView_Queue.Sorting = SortOrder.Ascending;
            this.ListView_Queue.Scrollable = true;
            this.ListView_Queue.FullRowSelect = true;

            // host list view ..
            this.ListView_Host.AutoArrange = true;
            this.ListView_Host.HeaderStyle = ColumnHeaderStyle.Clickable;
            this.ListView_Host.View = View.Details;
            this.ListView_Host.ListViewItemSorter = this.HostListViewSorter;
            this.ListView_Host.Sorting = SortOrder.Ascending;
            this.ListView_Host.Scrollable = true;
            this.ListView_Host.FullRowSelect = true;
        }

        /// <summary>
        /// 設定語言 Setting language resource.
        /// </summary>
        private void Language()
        {
            // main form caption text ..
#if Debug
            this.Text = string.Format("{0} - [Debug Mode]", AssemblyInfoClass.ProductInfo);
#else
            this.Text = AssemblyInfoClass.ProductInfo;
#endif
            // 獲取當前線程的文化設定 create default windows login user text info class ...
            global::System.Globalization.TextInfo txtInfo = global::System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo;

            switch (this.EnvSetting.Lang)
            {
                #region English (United-State)
                case Customization.Language.En_Us:

                    this.Queue_Job_Pause = "Pause Job ";
                    this.Queue_Job_Resume = "Resume Job ";

                    // Form Close Text String ..
                    this.AppCloseMessage = this.EnvCust.GetLocalization(this.Name, this.Name + "_Closing_String", Customization.Language.En_Us);

                    // Balloon Tip Title ..
                    this.ClientNotify.BalloonTipTitle = this.EnvCust.GetLocalization(this.Name, "Notify_Title_String", Customization.Language.En_Us);
                    // Balloon Tip Text ..
                    this.ClientNotify.BalloonTipText = this.EnvCust.GetLocalization(this.Name, "Notify_Text_String", Customization.Language.En_Us);

                    // Menu Bar About ..
                    this.Menu_About.Text = this.EnvCust.GetLocalization(this.Name, this.Menu_About.Name, Customization.Language.En_Us);

                    // Menu Bar Queue ..
                    this.Menu_Queue.Text = this.EnvCust.GetLocalization(this.Name, this.Menu_Queue.Name, Customization.Language.En_Us);
                    this.Menu_Queue_Exit.Text = this.EnvCust.GetLocalization(this.Name, this.Menu_Queue_Exit.Name, Customization.Language.En_Us);
                    this.Menu_Queue_Job.Text = this.EnvCust.GetLocalization(this.Name, this.Menu_Queue_Job.Name, Customization.Language.En_Us);

                    // Menu Bar Sub-Queue ..
                    this.Menu_Queue_Job_History.Text = this.EnvCust.GetLocalization(this.Name, this.Menu_Queue_Job_History.Name, Customization.Language.En_Us);
                    this.Menu_Queue_Job_Load.Text = this.EnvCust.GetLocalization(this.Name, this.Menu_Queue_Job_Load.Name, Customization.Language.En_Us);
                    this.Menu_Queue_Job_New.Text = this.EnvCust.GetLocalization(this.Name, this.Menu_Queue_Job_New.Name, Customization.Language.En_Us);

                    // Parse Job File Error Message ..
                    this.ParseFileErrMessage = this.EnvCust.GetLocalization(string.Empty, "Parse_FileFormat_Err", Customization.Language.En_Us);

                    // Parse Mulit File Error Message ..
                    this.MulitFileErrMessage = this.EnvCust.GetLocalization(string.Empty, "ParseFile_Multi_Err", Customization.Language.En_Us);

                    //the connect status message
                    //this.ServerConnectingMes = this.EnvCust.GetLocalization(this.Name, "ServerConnectingMes", Customization.Language.En_Us);
                    //this.ServerConnectedMes = this.EnvCust.GetLocalization(this.Name, "ServerConnectedMes", Customization.Language.En_Us);
                    //this.ServerDisConnectedMes = this.EnvCust.GetLocalization(this.Name, "ServerDisConnectedMes", Customization.Language.En_Us);

                    // Menu Bar Settings ..
                    this.Menu_Settings.Text = this.EnvCust.GetLocalization(this.Name, this.Menu_Settings.Name, Customization.Language.En_Us);

                    // Menu Bar Sub-Settings ..
                    this.Menu_Settings_ChangeUser.Text = this.EnvCust.GetLocalization(this.Name, this.Menu_Settings_ChangeUser.Name, Customization.Language.En_Us);
                    this.Menu_Settings_PoolMgr.Text = this.EnvCust.GetLocalization(this.Name, this.Menu_Settings_PoolMgr.Name, Customization.Language.En_Us);
                    this.Menu_Settings_Record_Option.Text = this.EnvCust.GetLocalization(this.Name, this.Menu_Settings_Record_Option.Name, Customization.Language.En_Us);
                    this.Menu_Settings_Network.Text = this.EnvCust.GetLocalization(this.Name, this.Menu_Settings_Network.Name, Customization.Language.En_Us);

                    // Menu Bar Language Sub-Settings ..
                    this.Menu_Settings_Lang.Text = this.EnvCust.GetLocalization(this.Name, this.Menu_Settings_Lang.Name, Customization.Language.En_Us);
                    this.Menu_Settings_Lang_Cht.Text = this.EnvCust.GetLocalization(this.Name, this.Menu_Settings_Lang_Cht.Name, Customization.Language.En_Us);
                    this.Menu_Settings_Lang_Eng.Text = this.EnvCust.GetLocalization(this.Name, this.Menu_Settings_Lang_Eng.Name, Customization.Language.En_Us);

                    // Group Box Process ..
                    this.GroupBox_Process.Text = this.EnvCust.GetLocalization(this.Name, this.GroupBox_Process.Name, Customization.Language.En_Us);

                    // List View Process ..
                    this.ListView_Process.Columns.Clear();
#if Debug
                    this.ListView_Process.Columns.Add(this.EnvCust.GetLocalization(this.Name, this.ListView_Process.Name + "_Id", Customization.Language.En_Us), 100);
#else
                    this.ListView_Process.Columns.Add(this.EnvCust.GetLocalization(this.Name, this.ListView_Process.Name + "_Id", Customization.Language.En_Us), 0);
#endif
                    this.ListView_Process.Columns.Add(this.EnvCust.GetLocalization(this.Name, this.ListView_Process.Name + "_Job", Customization.Language.En_Us), 200);
                    this.ListView_Process.Columns.Add(this.EnvCust.GetLocalization(this.Name, this.ListView_Process.Name + "_Frames", Customization.Language.En_Us), 100);
                    this.ListView_Process.Columns.Add(this.EnvCust.GetLocalization(this.Name, this.ListView_Process.Name + "_Host", Customization.Language.En_Us), 100);
                    this.ListView_Process.Columns.Add(this.EnvCust.GetLocalization(this.Name, this.ListView_Process.Name + "_Elpased", Customization.Language.En_Us), 100);

                    // Context Menu Process ..
                    this.Proc_View_Output.Text = this.EnvCust.GetLocalization(this.Name, this.Proc_View_Output.Name, Customization.Language.En_Us);

                    // Group Box Queue ..
                    this.GroupBox_Queue.Text = this.EnvCust.GetLocalization(this.Name, this.GroupBox_Queue.Name, Customization.Language.En_Us);

                    // List View Queue ..
                    this.ListView_Queue.Columns.Clear();
#if Debug
                    this.ListView_Queue.Columns.Add(this.EnvCust.GetLocalization(this.Name, this.ListView_Queue.Name + "_Id", Customization.Language.En_Us), 100);
                    this.ListView_Queue.Columns.Add(this.EnvCust.GetLocalization(this.Name, this.ListView_Queue.Name + "_Status", Customization.Language.En_Us), 80);
#else
                    this.ListView_Queue.Columns.Add(this.EnvCust.GetLocalization(this.Name, this.ListView_Queue.Name + "_Id", Customization.Language.En_Us), 0);
                    this.ListView_Queue.Columns.Add(this.EnvCust.GetLocalization(this.Name, this.ListView_Queue.Name + "_Status", Customization.Language.En_Us), 80);
#endif
                    this.ListView_Queue.Columns.Add(this.EnvCust.GetLocalization(this.Name, this.ListView_Queue.Name + "_Progress", Customization.Language.En_Us), 80);
                    this.ListView_Queue.Columns.Add(this.EnvCust.GetLocalization(this.Name, this.ListView_Queue.Name + "_ProcessType", Customization.Language.En_Us), 80);
                    this.ListView_Queue.Columns.Add(this.EnvCust.GetLocalization(this.Name, this.ListView_Queue.Name + "_Priority", Customization.Language.En_Us), 100);
                    this.ListView_Queue.Columns.Add(this.EnvCust.GetLocalization(this.Name, this.ListView_Queue.Name + "_Project", Customization.Language.En_Us), 120);
                    this.ListView_Queue.Columns.Add(this.EnvCust.GetLocalization(this.Name, this.ListView_Queue.Name + "_Job", Customization.Language.En_Us), 120);
                    this.ListView_Queue.Columns.Add(this.EnvCust.GetLocalization(this.Name, this.ListView_Queue.Name + "_Frames", Customization.Language.En_Us), 100);
                    this.ListView_Queue.Columns.Add(this.EnvCust.GetLocalization(this.Name, this.ListView_Queue.Name + "_First_Pool", Customization.Language.En_Us), 80);
                    this.ListView_Queue.Columns.Add(this.EnvCust.GetLocalization(this.Name, this.ListView_Queue.Name + "_Second_Pool", Customization.Language.En_Us), 80);
                    this.ListView_Queue.Columns.Add(this.EnvCust.GetLocalization(this.Name, this.ListView_Queue.Name + "_Submited_User", Customization.Language.En_Us), 100);
                    this.ListView_Queue.Columns.Add(this.EnvCust.GetLocalization(this.Name, this.ListView_Queue.Name + "_Submited_Time", Customization.Language.En_Us), 100);
                    this.ListView_Queue.Columns.Add(this.EnvCust.GetLocalization(this.Name, this.ListView_Queue.Name + "_Note", Customization.Language.En_Us), 100);

                    // Context Menu Queue ..
                    this.Queue_Update_Job.Text = this.EnvCust.GetLocalization(this.Name, this.Queue_Update_Job.Name, Customization.Language.En_Us);
                    this.Queue_Delete_Job.Text = this.EnvCust.GetLocalization(this.Name, this.Queue_Delete_Job.Name, Customization.Language.En_Us);

                    this.Queue_Repeat_Job.Text = this.EnvCust.GetLocalization(this.Name, this.Queue_Repeat_Job.Name, Customization.Language.En_Us);
                    this.Queue_SetPriority_Job.Text = this.EnvCust.GetLocalization(this.Name, this.Queue_SetPriority_Job.Name, Customization.Language.En_Us);



                    // Delete Job Error Message ..
                    this.DeleteJobErrMessage = this.EnvCust.GetLocalization(this.Name, "Delete_Job_Err", Customization.Language.En_Us);

                    // Group Box Host ..
                    this.GroupBox_Host.Text = this.EnvCust.GetLocalization(this.Name, this.GroupBox_Host.Name, Customization.Language.En_Us);

                    // List View Host ..
                    this.ListView_Host.Columns.Clear();
#if Debug
                    this.ListView_Host.Columns.Add(this.EnvCust.GetLocalization(this.Name, this.ListView_Host.Name + "_Id", Customization.Language.En_Us), 100);
#else
                    this.ListView_Host.Columns.Add(this.EnvCust.GetLocalization(this.Name, this.ListView_Host.Name + "_Id", Customization.Language.En_Us), 0);
#endif
                    this.ListView_Host.Columns.Add(this.EnvCust.GetLocalization(this.Name, this.ListView_Host.Name + "_Host", Customization.Language.En_Us), 150);
                    this.ListView_Host.Columns.Add(this.EnvCust.GetLocalization(this.Name, this.ListView_Host.Name + "_Status", Customization.Language.En_Us), 100);
                    this.ListView_Host.Columns.Add(this.EnvCust.GetLocalization(this.Name, this.ListView_Host.Name + "_Processors", Customization.Language.En_Us), 150);
                    this.ListView_Host.Columns.Add(this.EnvCust.GetLocalization(this.Name, this.ListView_Host.Name + "_Connected", Customization.Language.En_Us), 100);
                    this.ListView_Host.Columns.Add(this.EnvCust.GetLocalization(this.Name, this.ListView_Host.Name + "_Priority", Customization.Language.En_Us), 100);
                    this.ListView_Host.Columns.Add(this.EnvCust.GetLocalization(this.Name, this.ListView_Host.Name + "_Note", Customization.Language.En_Us), 100);

                    // Notification ..
                    this.Notification_Title = this.EnvCust.GetLocalization(this.Name, "Notification", Customization.Language.En_Us);
                    this.Notification_Completed = this.EnvCust.GetLocalization(this.Name, "Notification_Completed", Customization.Language.En_Us);
                    this.Notification_Error = this.EnvCust.GetLocalization(this.Name, "Notification_Error", Customization.Language.En_Us);

                    // Context Menu Host ..
                    this.Host_Add_Machine.Text = this.EnvCust.GetLocalization(this.Name, this.Host_Add_Machine.Name, Customization.Language.En_Us);
                    this.AddMachinesErrMessage = this.EnvCust.GetLocalization(this.Name, this.Host_Add_Machine.Name + "_Err", Customization.Language.En_Us);
                    this.Host_Remove_Machine.Text = this.EnvCust.GetLocalization(this.Name, this.Host_Remove_Machine.Name, Customization.Language.En_Us);
                    this.RemoveMachinesErrMessage = this.EnvCust.GetLocalization(this.Name, this.Host_Remove_Machine.Name + "_Err", Customization.Language.En_Us);
                    this.Host_Setting_Priority.Text = this.EnvCust.GetLocalization(this.Name, this.Host_Setting_Priority.Name, Customization.Language.En_Us);

                    // Status Bar Current User ..
                    this.Status_Label_User.Text = this.EnvCust.GetLocalization(this.Name, this.Status_Label_User.Name, Customization.Language.En_Us) + " " + txtInfo.ToTitleCase(Environment.UserName);

                    // Due Date ..
                    this.DueDateMessage = this.EnvCust.GetLocalization(this.Name, "DueDate_Msg", Customization.Language.En_Us);
                    break;
                #endregion

                #region Traditional Chinese
                case Customization.Language.Zh_Tw:

                    this.Queue_Job_Pause = "暫停工作";
                    this.Queue_Job_Resume = "啟動工作";

                    // Form Close Text String ..
                    this.AppCloseMessage = this.EnvCust.GetLocalization(this.Name, this.Name + "_Closing_String", Customization.Language.Zh_Tw);

                    // Balloon Tip Title ..
                    this.ClientNotify.BalloonTipTitle = this.EnvCust.GetLocalization(this.Name, "Notify_Title_String", Customization.Language.Zh_Tw);
                    // Balloon Tip Text ..
                    this.ClientNotify.BalloonTipText = this.EnvCust.GetLocalization(this.Name, "Notify_Text_String", Customization.Language.Zh_Tw);

                    // Menu Bar About ..
                    this.Menu_About.Text = this.EnvCust.GetLocalization(this.Name, this.Menu_About.Name, Customization.Language.Zh_Tw);

                    // Menu Bar Queue ..
                    this.Menu_Queue.Text = this.EnvCust.GetLocalization(this.Name, this.Menu_Queue.Name, Customization.Language.Zh_Tw);
                    this.Menu_Queue_Exit.Text = this.EnvCust.GetLocalization(this.Name, this.Menu_Queue_Exit.Name, Customization.Language.Zh_Tw);
                    this.Menu_Queue_Job.Text = this.EnvCust.GetLocalization(this.Name, this.Menu_Queue_Job.Name, Customization.Language.Zh_Tw);

                    // Menu Bar Sub-Queue ..
                    this.Menu_Queue_Job_History.Text = this.EnvCust.GetLocalization(this.Name, this.Menu_Queue_Job_History.Name, Customization.Language.Zh_Tw);
                    this.Menu_Queue_Job_Load.Text = this.EnvCust.GetLocalization(this.Name, this.Menu_Queue_Job_Load.Name, Customization.Language.Zh_Tw);
                    this.Menu_Queue_Job_New.Text = this.EnvCust.GetLocalization(this.Name, this.Menu_Queue_Job_New.Name, Customization.Language.Zh_Tw);

                    // Parse Job File Error Message ..
                    this.ParseFileErrMessage = this.EnvCust.GetLocalization(string.Empty, "Parse_FileFormat_Err", Customization.Language.Zh_Tw);

                    // Parse Mulit File Error Message ..
                    this.MulitFileErrMessage = this.EnvCust.GetLocalization(string.Empty, "ParseFile_Multi_Err", Customization.Language.Zh_Tw);


                    //the connect status message
                    //this.ServerConnectingMes = this.EnvCust.GetLocalization(this.Name, "ServerConnectingMes", Customization.Language.Zh_Tw);
                    //this.ServerConnectedMes = this.EnvCust.GetLocalization(this.Name, "ServerConnectedMes", Customization.Language.Zh_Tw);
                    //this.ServerDisConnectedMes = this.EnvCust.GetLocalization(this.Name, "ServerDisConnectedMes", Customization.Language.Zh_Tw);


                    // Menu Bar Settings ..
                    this.Menu_Settings.Text = this.EnvCust.GetLocalization(this.Name, this.Menu_Settings.Name, Customization.Language.Zh_Tw);

                    // Menu Bar Sub-Settings ..
                    this.Menu_Settings_ChangeUser.Text = this.EnvCust.GetLocalization(this.Name, this.Menu_Settings_ChangeUser.Name, Customization.Language.Zh_Tw);
                    this.Menu_Settings_PoolMgr.Text = this.EnvCust.GetLocalization(this.Name, this.Menu_Settings_PoolMgr.Name, Customization.Language.Zh_Tw);
                    this.Menu_Settings_Record_Option.Text = this.EnvCust.GetLocalization(this.Name, this.Menu_Settings_Record_Option.Name, Customization.Language.Zh_Tw);
                    this.Menu_Settings_Network.Text = this.EnvCust.GetLocalization(this.Name, this.Menu_Settings_Network.Name, Customization.Language.Zh_Tw);

                    // Menu Bar Language Sub-Settings ..
                    this.Menu_Settings_Lang.Text = this.EnvCust.GetLocalization(this.Name, this.Menu_Settings_Lang.Name, Customization.Language.Zh_Tw);
                    this.Menu_Settings_Lang_Cht.Text = this.EnvCust.GetLocalization(this.Name, this.Menu_Settings_Lang_Cht.Name, Customization.Language.Zh_Tw);
                    this.Menu_Settings_Lang_Eng.Text = this.EnvCust.GetLocalization(this.Name, this.Menu_Settings_Lang_Eng.Name, Customization.Language.Zh_Tw);

                    // Group Box Process ..
                    this.GroupBox_Process.Text = this.EnvCust.GetLocalization(this.Name, this.GroupBox_Process.Name, Customization.Language.Zh_Tw);

                    // List View Process ..
                    this.ListView_Process.Columns.Clear();
#if Debug
                    this.ListView_Process.Columns.Add(this.EnvCust.GetLocalization(this.Name, this.ListView_Process.Name + "_Id", Customization.Language.Zh_Tw), 100);
#else
                    this.ListView_Process.Columns.Add(this.EnvCust.GetLocalization(this.Name, this.ListView_Process.Name + "_Id", Customization.Language.Zh_Tw), 0);
#endif
                    this.ListView_Process.Columns.Add(this.EnvCust.GetLocalization(this.Name, this.ListView_Process.Name + "_Job", Customization.Language.Zh_Tw), 200);
                    this.ListView_Process.Columns.Add(this.EnvCust.GetLocalization(this.Name, this.ListView_Process.Name + "_Frames", Customization.Language.Zh_Tw), 100);
                    this.ListView_Process.Columns.Add(this.EnvCust.GetLocalization(this.Name, this.ListView_Process.Name + "_Host", Customization.Language.Zh_Tw), 100);
                    this.ListView_Process.Columns.Add(this.EnvCust.GetLocalization(this.Name, this.ListView_Process.Name + "_Elpased", Customization.Language.Zh_Tw), 100);

                    // Context Menu Process ..
                    this.Proc_View_Output.Text = this.EnvCust.GetLocalization(this.Name, this.Proc_View_Output.Name, Customization.Language.Zh_Tw);

                    // Group Box Queue ..
                    this.GroupBox_Queue.Text = this.EnvCust.GetLocalization(this.Name, this.GroupBox_Queue.Name, Customization.Language.Zh_Tw);

                    // List View Queue ..
                    this.ListView_Queue.Columns.Clear();
#if Debug
                    this.ListView_Queue.Columns.Add(this.EnvCust.GetLocalization(this.Name, this.ListView_Queue.Name + "_Id", Customization.Language.Zh_Tw), 100);
                    this.ListView_Queue.Columns.Add(this.EnvCust.GetLocalization(this.Name, this.ListView_Queue.Name + "_Status", Customization.Language.Zh_Tw), 80);
#else
                    this.ListView_Queue.Columns.Add(this.EnvCust.GetLocalization(this.Name, this.ListView_Queue.Name + "_Id", Customization.Language.Zh_Tw), 0);
                    this.ListView_Queue.Columns.Add(this.EnvCust.GetLocalization(this.Name, this.ListView_Queue.Name + "_Status", Customization.Language.Zh_Tw), 80);
#endif
                    this.ListView_Queue.Columns.Add(this.EnvCust.GetLocalization(this.Name, this.ListView_Queue.Name + "_Progress", Customization.Language.Zh_Tw), 80);
                    this.ListView_Queue.Columns.Add(this.EnvCust.GetLocalization(this.Name, this.ListView_Queue.Name + "_ProcessType", Customization.Language.Zh_Tw), 80);
                    this.ListView_Queue.Columns.Add(this.EnvCust.GetLocalization(this.Name, this.ListView_Queue.Name + "_Priority", Customization.Language.Zh_Tw), 100);
                    this.ListView_Queue.Columns.Add(this.EnvCust.GetLocalization(this.Name, this.ListView_Queue.Name + "_Project", Customization.Language.Zh_Tw), 120);
                    this.ListView_Queue.Columns.Add(this.EnvCust.GetLocalization(this.Name, this.ListView_Queue.Name + "_Job", Customization.Language.Zh_Tw), 120);
                    this.ListView_Queue.Columns.Add(this.EnvCust.GetLocalization(this.Name, this.ListView_Queue.Name + "_Frames", Customization.Language.Zh_Tw), 100);
                    this.ListView_Queue.Columns.Add(this.EnvCust.GetLocalization(this.Name, this.ListView_Queue.Name + "_First_Pool", Customization.Language.Zh_Tw), 80);
                    this.ListView_Queue.Columns.Add(this.EnvCust.GetLocalization(this.Name, this.ListView_Queue.Name + "_Second_Pool", Customization.Language.Zh_Tw), 80);
                    this.ListView_Queue.Columns.Add(this.EnvCust.GetLocalization(this.Name, this.ListView_Queue.Name + "_Submited_User", Customization.Language.Zh_Tw), 100);
                    this.ListView_Queue.Columns.Add(this.EnvCust.GetLocalization(this.Name, this.ListView_Queue.Name + "_Submited_Time", Customization.Language.Zh_Tw), 100);
                    this.ListView_Queue.Columns.Add(this.EnvCust.GetLocalization(this.Name, this.ListView_Queue.Name + "_Note", Customization.Language.Zh_Tw), 100);

                    // Context Menu Queue ..
                    this.Queue_Update_Job.Text = this.EnvCust.GetLocalization(this.Name, this.Queue_Update_Job.Name, Customization.Language.Zh_Tw);
                    this.Queue_Delete_Job.Text = this.EnvCust.GetLocalization(this.Name, this.Queue_Delete_Job.Name, Customization.Language.Zh_Tw);

                    this.Queue_Repeat_Job.Text = this.EnvCust.GetLocalization(this.Name, this.Queue_Repeat_Job.Name, Customization.Language.Zh_Tw);
                    this.Queue_SetPriority_Job.Text = this.EnvCust.GetLocalization(this.Name, this.Queue_SetPriority_Job.Name, Customization.Language.Zh_Tw);

                    // Delete Job Error Message ..
                    this.DeleteJobErrMessage = this.EnvCust.GetLocalization(this.Name, "Delete_Job_Err", Customization.Language.Zh_Tw);

                    // Group Box Host ..
                    this.GroupBox_Host.Text = this.EnvCust.GetLocalization(this.Name, this.GroupBox_Host.Name, Customization.Language.Zh_Tw);

                    // List View Host ..
                    this.ListView_Host.Columns.Clear();
#if Debug
                    this.ListView_Host.Columns.Add(this.EnvCust.GetLocalization(this.Name, this.ListView_Host.Name + "_Id", Customization.Language.Zh_Tw), 100);
#else
                    this.ListView_Host.Columns.Add(this.EnvCust.GetLocalization(this.Name, this.ListView_Host.Name + "_Id", Customization.Language.Zh_Tw), 0);
#endif
                    this.ListView_Host.Columns.Add(this.EnvCust.GetLocalization(this.Name, this.ListView_Host.Name + "_Host", Customization.Language.Zh_Tw), 150);
                    this.ListView_Host.Columns.Add(this.EnvCust.GetLocalization(this.Name, this.ListView_Host.Name + "_Status", Customization.Language.Zh_Tw), 100);
                    this.ListView_Host.Columns.Add(this.EnvCust.GetLocalization(this.Name, this.ListView_Host.Name + "_Processors", Customization.Language.Zh_Tw), 150);
                    this.ListView_Host.Columns.Add(this.EnvCust.GetLocalization(this.Name, this.ListView_Host.Name + "_Connected", Customization.Language.Zh_Tw), 100);
                    this.ListView_Host.Columns.Add(this.EnvCust.GetLocalization(this.Name, this.ListView_Host.Name + "_Priority", Customization.Language.Zh_Tw), 100);
                    this.ListView_Host.Columns.Add(this.EnvCust.GetLocalization(this.Name, this.ListView_Host.Name + "_Note", Customization.Language.Zh_Tw), 100);

                    // Notification ..
                    this.Notification_Title = this.EnvCust.GetLocalization(this.Name, "Notification", Customization.Language.Zh_Tw);
                    this.Notification_Completed = this.EnvCust.GetLocalization(this.Name, "Notification_Completed", Customization.Language.Zh_Tw);
                    this.Notification_Error = this.EnvCust.GetLocalization(this.Name, "Notification_Error", Customization.Language.Zh_Tw);

                    // Context Menu Host ..
                    this.Host_Add_Machine.Text = this.EnvCust.GetLocalization(this.Name, this.Host_Add_Machine.Name, Customization.Language.Zh_Tw);
                    this.AddMachinesErrMessage = this.EnvCust.GetLocalization(this.Name, this.Host_Add_Machine.Name + "_Err", Customization.Language.Zh_Tw);
                    this.Host_Remove_Machine.Text = this.EnvCust.GetLocalization(this.Name, this.Host_Remove_Machine.Name, Customization.Language.Zh_Tw);
                    this.RemoveMachinesErrMessage = this.EnvCust.GetLocalization(this.Name, this.Host_Remove_Machine.Name + "_Err", Customization.Language.Zh_Tw);
                    this.Host_Setting_Priority.Text = this.EnvCust.GetLocalization(this.Name, this.Host_Setting_Priority.Name, Customization.Language.Zh_Tw);

                    // Status Bar Current User ..
                    this.Status_Label_User.Text = this.EnvCust.GetLocalization(this.Name, this.Status_Label_User.Name, Customization.Language.Zh_Tw) + " " + txtInfo.ToTitleCase(Environment.UserName);

                    // Due Date ..
                    this.DueDateMessage = this.EnvCust.GetLocalization(this.Name, "DueDate_Msg", Customization.Language.Zh_Tw);
                    break;
                #endregion
            }
        }

        /// <summary>
        /// 檢測到期時間 check due date timer tick event procedure.
        /// </summary>
        private void DueTimer_Tick(object sender, EventArgs e)
        {
            //判斷是否過期
            if (!this.EnvCust.DueDate)
            {
                // 到期則停止檢測 disable trigger timer tick event ..
                this.DueTimer.Enabled = false;
                // disable main controls ..
                this.Enabled = false;
                // show message ..
                DialogResult result = MessageBox.Show(this, this.DueDateMessage, AssemblyInfoClass.ProductInfo,
                    MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
                if (result == DialogResult.OK)
                {
                    // sent stop loop request ..
                    this.requeststop = true;

                    //判斷是否保存設置
                    if (this.Menu_Settings_Record_Option.Checked)
                    {
                        // save client environment setting ..
                        this.EnvCust.Save(this.EnvSetting);
                    }
                    //中斷連接
                    if (this.IsConnected)
                    {
                        // registry local machine to server request ..
                        this.EnvCust.RegLocalMachine(null, true, ref this.EnvComm);
                        // disconnect ..
                        this.EnvComm.Disconnect();
                    }

                    // clear client taskbar notify component ..
                    this.ClientNotify.Dispose();

                    // clean log dictionary ..
                    ProcLog = null;

                    // 關閉窗體 close application ..
                    Application.ExitThread();
                }
            }
        }
        #endregion

        #region 窗體改變大小 Form Resize Event Procedure
        private void Main_Form_Resize(object sender, EventArgs e)
        {
            //最小化
            if (this.WindowState == FormWindowState.Minimized)
            {
                // 掛起 change suspend signal status ..？？？？？？？？？？？？？？？？？？？？？？？？
                this.Suspend.Reset();
                // change suspend flag ..
                this.__suspend = true;

                this.Hide();
                //於工作列顯示提示30毫秒
                this.ClientNotify.ShowBalloonTip(30);
            }
        }
        #endregion

        #region 恢復窗體正常大下 Restore Normal Window Procedure
        /// <summary>
        /// 工作列圖示點擊事件 Client notify component double click event.
        /// </summary>
        private void ClientNotify_DoubleClick(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                //是否有狀態/用戶名提示欄？？？？？？？？？？？？？
                if (this.Renbar_StatusBar.Items.Count == 1)
                {
                    // change suspend signal status ..
                    this.Suspend.Set();
                    // change suspend flag ..
                    this.__suspend = false;
                }

                // show the application ..
                this.Show();
                // restore default window state ..
                this.WindowState = FormWindowState.Normal;
            }
        }

        /// <summary>
        /// 通告欄點擊事件 Notification component content click event.
        /// </summary>
        private void RenbarNotifier_ContentClick(object sender, EventArgs e)
        {
            // call client notify double click event ..
            this.ClientNotify_DoubleClick(sender, e);
        }
        #endregion

        #region 關閉窗體 Form Close Event Procedure
        private void Main_Form_FormClosing(object sender, FormClosingEventArgs e)
        {
            // 確認對話方框 show confirm message dialog ..
            DialogResult result = MessageBox.Show(this, AppCloseMessage, this.Text, MessageBoxButtons.OKCancel, MessageBoxIcon.Question);

            if (result == DialogResult.Cancel)
            {
                e.Cancel = true;
            }
            else
            {
                //停止請求
                this.requeststop = true;
                //關閉連接
                if (this.IsConnected)
                {
                    // registry local machine to server request ..
                    this.EnvCust.RegLocalMachine(null, true, ref this.EnvComm);
                    // disconnect ..
                }
                this.EnvComm.Disconnect();

                //保存設置
                if (this.Menu_Settings_Record_Option.Checked)
                {
                    // save client environment setting ..
                    this.EnvCust.Save(this.EnvSetting);
                    // sent stop loop request ..
                }
         
                // 釋放資源 clear client taskbar notify component ..
                this.ClientNotify.Dispose();
                // clean log dictionary ..
                ProcLog = null;

                //移除提示信息
                this.Renbar_StatusBar.Items.Remove(serverLabel);
            }
        }
        #endregion

        #region 主菜單點擊事件 Form Menu Item Click Event Procedure

        #region 任務隊列菜單 Queue Menu Items
        /// <summary>
        /// 新建任务 Send new job.
        /// </summary>
        private void Menu_Queue_Job_New_Click(object sender, EventArgs e)
        {
            if (this.IsConnected)
            {
                // create job form instance ..
                Job_Form jobForm = new Job_Form(ref this.EnvCust, ref this.EnvComm, this.EnvSetting)
                {
                    //傳遞參數
                    Pool = this.EnvCust.DropDown.Pool,
                    Pool2 = this.EnvCust.DropDown.Pool2,
                    WaitFor = this.EnvCust.DropDown.Waitfor,
                    IsExtern = false,
                    IsUpdate = false,
                    IsViewHistory = false
                };

                // show form ..
                if (jobForm.ShowDialog() == DialogResult.OK)
                    jobForm.Dispose();
            }
        }

        /// <summary>
        /// 加載任務 Form file send job.
        /// </summary>
        private void Menu_Queue_Job_Load_Click(object sender, EventArgs e)
        {
            if (this.IsConnected)
            {
                // create job form instance ..
                Job_Form jobForm = new Job_Form(ref this.EnvCust, ref this.EnvComm, this.EnvSetting)
                {
                    Pool = this.EnvCust.DropDown.Pool,//在Customization.cs中獲取並設定EnvCust.DropDown.Pool等数据——
                    Pool2 = this.EnvCust.DropDown.Pool2,
                    WaitFor = this.EnvCust.DropDown.Waitfor,
                    IsExtern = true,
                    IsUpdate = false
                };

                // declare parse render file variable ..
                IDictionary<string, object> attributes = new Dictionary<string, object>();

                // open file dialog ..
                OpenFileDialog LoadFile = this.EnvCust.OpenFileDialog;
                #region 依據加載結果，給不同提示
                if (LoadFile.ShowDialog() == DialogResult.OK)
                {
                    //0 = Success, 1 = Parse Error, 2 = Multi File Error
                    int _p = this.EnvCust.Parse(LoadFile.FileName, ref attributes);
                    switch (_p)
                    {
                        //加載成功，顯示Job_Form
                        case 0:
                            // assign parse properties ..
                            jobForm.BackupFilename = LoadFile.FileName;
                            jobForm.ExternItems = attributes;

                            // show job form ..
                            if (jobForm.ShowDialog(this) == DialogResult.OK)
                            {
                                jobForm.Dispose();
                            }
                            break;

                        case 1:
                            // show parse error message ..
                            MessageBox.Show(this, this.ParseFileErrMessage, AssemblyInfoClass.ProductInfo,
                                MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                            break;

                        case 2:
                            // show parse multi error message ..
                            MessageBox.Show(this, this.MulitFileErrMessage, AssemblyInfoClass.ProductInfo,
                                MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                            break;
                    }
                }
                #endregion
            }
        }

        /// <summary>
        /// 顯示任務的歷史記錄 Display job history.
        /// </summary>
        private void Menu_Queue_Job_History_Click(object sender, EventArgs e)
        {
            HistoryForm historyForm = new HistoryForm(ref this.EnvCust, ref this.EnvComm, this.EnvSetting);

            // show form ..
            if (historyForm.ShowDialog() == DialogResult.OK)
                historyForm.Dispose();
        }

        /// <summary>
        /// 退出系統 Exit renbar application.
        /// </summary>
        private void Menu_Queue_Exit_Click(object sender, EventArgs e)
        {
            // exit application ..
            this.Close();
        }
        #endregion

        #region 環境設定菜單 Setting Menu Items
        /// <summary>
        /// 更改當前用戶 Change current user.
        /// </summary>
        private void Menu_Settings_ChangeUser_Click(object sender, EventArgs e)
        {
            // create change user form instance ..
            ChangeUser_Form userForm = new ChangeUser_Form(this.EnvSetting);

            // show form ..
            if (userForm.ShowDialog() == DialogResult.OK)
            {
                userForm.Dispose();

                // get default windows login user or changed user ..
                string _user = Customization.User == null ? Environment.UserName : Customization.User;

                // 更改狀態欄的用戶名顯示 refresh status bar label text ..
                switch (this.EnvSetting.Lang)
                {
                    case Customization.Language.En_Us:
                        // Status Bar Current User ..
                        this.Status_Label_User.Text = this.EnvCust.GetLocalization(this.Name,
                            this.Status_Label_User.Name, Customization.Language.En_Us) + " " + _user;
                        break;

                    case Customization.Language.Zh_Tw:
                        // Status Bar Current User ..
                        this.Status_Label_User.Text = this.EnvCust.GetLocalization(this.Name,
                            this.Status_Label_User.Name, Customization.Language.Zh_Tw) + " " + _user;
                        break;
                }
            }

        }

        /// <summary>
        /// 群組管理 Pool management.
        /// </summary>
        private void Menu_Settings_PoolMgr_Click(object sender, EventArgs e)
        {
            // create pool management form instance ..
            PoolMgr_Form poolForm = new PoolMgr_Form(ref this.EnvCust, ref this.EnvComm, this.EnvSetting);

            // show form ..
            if (poolForm.ShowDialog() == DialogResult.OK)
                poolForm.Dispose();
        }

        /// <summary>
        /// 設定是否保存設置 Save last setting switch.
        /// </summary>
        private void Menu_Settings_Record_Option_Click(object sender, EventArgs e)
        {
            if (!this.Menu_Settings_Record_Option.Checked)
            {
                // 指定保存路徑 declare environment path position ..
                string EnvFile = string.Format(@"{0}\Environment.rbe", Application.StartupPath);
                if (global::System.IO.File.Exists(EnvFile))
                    global::System.IO.File.Delete(EnvFile);
            }
        }

        /// <summary>
        /// 鏈接IP地址與端口號管理 Renbar network section.
        /// </summary>
        private void Menu_Settings_Network_Click(object sender, EventArgs e)
        {
            // create network form instance ..
            //Net_Form netForm = new Net_Form(this.EnvSetting);
            Net_Form netForm = new Net_Form(this.EnvSetting);

            // show form ...
            if (netForm.ShowDialog() == DialogResult.OK)
            {
                // 更新已設定環境變量 set network environment ...
                this.EnvSetting.ServerIpAddress = netForm.ServerAddr;
                this.EnvSetting.ServerPort = netForm.PortNumber;

                if (this.Menu_Settings_Record_Option.Checked)
                    // save client environment setting ...
                    this.EnvCust.Save(this.EnvSetting);
                netForm.Dispose();
            }

            //連接
            if (!IsConnected)
            {
                // connect server ..
                new Thread(delegate()
                {
                    if (this.EnvSetting.ServerIpAddress == null && this.EnvSetting.ServerPort == 0)
                        return;
                    else
                        // connect remote server ..
                        this.Connect();
                }).Start();
            }
        }

        /// <summary>
        /// 切換至英文 User interface of english language.
        /// </summary>
        private void Menu_Settings_Lang_Eng_Click(object sender, EventArgs e)
        {
            // cancel all checked items ..
            foreach (ToolStripMenuItem Item in this.Menu_Settings_Lang.DropDownItems)
                Item.Checked = false;

            // setting current language checked ..
            this.Menu_Settings_Lang_Eng.Checked = true;

            // change environment language ..
            this.EnvSetting.Lang = Customization.Language.En_Us;

            // reload user interface ..
            this.Language();
        }

        /// <summary>
        /// 切換至中文 User interface of traditional chinese language.
        /// </summary>
        private void Menu_Settings_Lang_Cht_Click(object sender, EventArgs e)
        {
            // cancel all checked items ..
            foreach (ToolStripMenuItem Item in this.Menu_Settings_Lang.DropDownItems)
                Item.Checked = false;

            // setting current language checked ..
            this.Menu_Settings_Lang_Cht.Checked = true;

            // change environment language ..
            this.EnvSetting.Lang = Customization.Language.Zh_Tw;

            // reload user interface ..
            this.Language();
        }
        #endregion

        #region 關于菜單 About Menu Item
        /// <summary>
        /// 顯示版本資訊 Display about form.
        /// </summary>
        private void Menu_About_Click(object sender, EventArgs e)
        {
            // create about form instance ..
            About_Form aboutForm = new About_Form(this.EnvSetting);

            // show form ...
            if (aboutForm.ShowDialog() == DialogResult.OK)
                aboutForm.Dispose();
        }
        #endregion
        #endregion

        #region 右键菜单事件 Form Context Menu Items Event Procedure

        #region 处理中列表右键菜单Process Context Menu Partial
        /// <summary>
        /// 右击显示菜单 Process context menu visible partial.
        /// </summary>
        private void ListView_Process_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && this.ListView_Process.Items.Count > 0 && this.ListView_Process.SelectedItems.Count > 0)
                this.Proc_View_Output.Visible = true;
            else
                this.Proc_View_Output.Visible = false;
        }

        /// <summary>
        /// 弹出Console_Form並输出日志信息 View single job log output.
        /// </summary>
        private void Proc_View_Output_Click(object sender, EventArgs e)
        {
            if (this.ListView_Process.SelectedItems.Count <= 0)
            {
                return;
            }
            // declare view job log output info array ..
            string[] LogInfo = new string[4];

            // check mouse click selected item ..
            for (int i = 0; i < this.ListView_Process.SelectedItems[0].SubItems.Count - 2; i++)
            {
                LogInfo[i] = this.ListView_Process.SelectedItems[0].SubItems[i].Text;
            }

            // assign last log record ..
            if (ProcLog.ContainsKey(LogInfo[0]))
            {
                LogInfo[3] = ProcLog[LogInfo[0]];//處理識別碼、名稱、張數、Proclog[“處理識別碼”]
            }
            if (!OutputForms.Contains(LogInfo[0]))
            {
                // add to output log form collection ..
                OutputForms.Add(LogInfo[0]);
                // create console form instance ..
                Console_Form csForm = new Console_Form(this.EnvSetting, LogInfo);
                // show form ..
                csForm.Show(this);
            }
            else
            {
                return;
            }
        }
        #endregion

        #region 队列列表右键菜单 Queue Context Menu Partial
        /// <summary>
        /// 显示菜单Queue context menu visible partial.
        /// </summary>
        private void ListView_Queue_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && this.ListView_Queue.SelectedItems.Count > 0)
            {
                this.Queue_Delete_Job.Visible = true;
                this.Queue_Update_Job.Visible = true;
                this.Queue_SetPriority_Job.Visible = true;
                if (this.ListView_Queue.SelectedItems[0].SubItems[1].Text.Trim() != "PAUSE")
                {
                    this.Queue_Pause_Job.Text = this.Queue_Job_Pause;
                }
                else
                {
                    this.Queue_Pause_Job.Text = this.Queue_Job_Resume;
                }
                this.Queue_Pause_Job.Visible = true;

                this.Queue_Repeat_Job.Visible = true;

            }
        }

        /// <summary>
        /// 更新任务——未实现
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Queue_Update_Job_Click(object sender, EventArgs e)
        {
            if (this.ListView_Queue.SelectedItems.Count <= 0)
            {
                return;
            }

            IDictionary<string, object> Data = this.EnvCust.GetJobInfo(this.ListView_Queue.SelectedItems[0].Text.Trim(), ref this.EnvComm);
            string id = Data["Job_Group_Id"].ToString();

            if (Data != null)
            {
                IDictionary<string, object> DetailInfoData = this.EnvCust.DataRevise(Data);

                #region 綁定數據到JobForm
                Job_Form jobForm = new Job_Form(ref this.EnvCust, ref this.EnvComm, this.EnvSetting)
                {
                    Pool = this.EnvCust.DropDown.Pool,
                    Pool2 = this.EnvCust.DropDown.Pool2,
                    WaitFor = this.EnvCust.DropDown.Waitfor,
                    IsExtern = true,
                    IsUpdate = true,
                    IsViewHistory = false
                };
                #endregion

                jobForm.ExternItems = DetailInfoData;
                jobForm.UpdateId = id;

                // show job form ..
                if (jobForm.ShowDialog(this) == DialogResult.OK)
                    jobForm.Dispose();
            }
            else
            {
                MessageBox.Show("Fail to get the detail info of this job!");
            }

        }

        /// <summary>
        /// 删除选中的任务 Delete selected job queue.
        /// </summary>
        private void Queue_Delete_Job_Click(object sender, EventArgs e)
        {
            int deleted = 0;

            if (this.ListView_Queue.SelectedItems.Count <= 0)
                return;

            // declare delete job list ..
            IList<string> DeleteList = new List<string>();

            // confirm currently selected items ..
            //确认並删除选中项
            foreach (ListViewItem Item in this.ListView_Queue.SelectedItems)
            {
                if (!DeleteList.Contains(Item.SubItems[0].Text))
                {
                    // add to delete list ..
                    DeleteList.Add(Item.SubItems[0].Text);

                    // write currently data to local data ..
                    DataRow[] row = this.EnvCust.DataListView.Tables["Queue"].Select(string.Format("Queue_Id = '{0}'", Item.SubItems[0].Text));

                    if (row.Length > 0)
                    {
                        // delete data ..
                        //删除
                        row[0].Delete();

                        // step add count ..
                        //计数
                        deleted++;
                    }
                }
            }

            if (deleted > 0)
            {
                // change manual delete flag ..
                //设置为手动删除
                this.manual_Delete = true;

                // commit changes ..
                this.EnvCust.DataListView.AcceptChanges();//？？？？？？？？？？？？？？？？？？？？？？

                //添加“已删除”到发送信息体
                KeyValuePair<string, object> responseObject;
                IDictionary<string, object> SendItem = new Dictionary<string, object>
                {
                    { "DeleteList", DeleteList }
                };

                // 发送请求 send request to remote server ..
                do
                {
                    // confirm current request status ..
                    if (EnvComm.CanRequest)
                    {
                        // package sent data ..
                        IList<object> packaged = EnvComm.Package(Client2Server.CommunicationType.JOBQUEUEDELETE, SendItem);

                        // wait for result ..
                        responseObject = EnvComm.Request(packaged);
                        break;
                    }
                    Thread.Sleep(500);
                } while (true);

                //检测响应信息，返回错误资讯（如果有）
                if (responseObject.Key.Substring(0, 1) == "-")
                {
                    // show error message ..
                    MessageBox.Show(this, this.DeleteJobErrMessage,
                        AssemblyInfoClass.ProductInfo, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    MessageBox.Show("Delete succeed!");
                }

            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Queue_SetPriority_Job_Click(object sender, EventArgs e)
        {
            if (this.ListView_Queue.SelectedItems.Count <= 0)
                return;

            #region Construct Setting Info
            // declare delete job list ..
            IList<string> Jobs = new List<string>();
            // confirm currently selected items ..
            foreach (ListViewItem Item in this.ListView_Queue.SelectedItems)
            {
                if (!Jobs.Contains(Item.SubItems[0].Text))
                {
                    // add to delete list ..
                    Jobs.Add(Item.SubItems[0].Text);
                }
            }
            #endregion

            #region Sending the Command
            // show priority form ..
            Priority_Form priorityForm = new Priority_Form(ref this.EnvCust, ref this.EnvComm, this.EnvSetting, Jobs, Client2Server.CommunicationType.JOBPRIORITY);

            if (priorityForm.ShowDialog() == DialogResult.OK)
                priorityForm.Dispose();
            #endregion

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Queue_Pause_Job_Click(object sender, EventArgs e)
        {
            if (this.ListView_Queue.SelectedItems.Count <= 0)
            {
                return;
            }

            #region Construct Send Item Info

            // declare delete job list ..
            string[] PauseList = new string[this.ListView_Queue.SelectedItems.Count];
            //IList<string> PauseList = new List<string>();
            for (int i = 0; i < this.ListView_Queue.SelectedItems.Count; i++)
            {
                PauseList[i] = this.ListView_Queue.SelectedItems[i].SubItems[0].Text.ToString();
            }

            KeyValuePair<string, object> responseObject;

            int status = Convert.ToInt16(JobStatusFlag.CHECKING);

            if (this.ListView_Queue.SelectedItems[0].SubItems[1].Text.Trim() != "PAUSE")
            {
                status = Convert.ToInt16(JobStatusFlag.PAUSE);
            }
            else if (this.ListView_Queue.SelectedItems[0].SubItems[2].Text.Trim() != string.Format("{0}%", 0))
            {
                status = Convert.ToInt16(JobStatusFlag.PROCESSING);
            }

            IDictionary<string, object> SendItem = new Dictionary<string, object>
                {
                    { "Job_Pause_IDlist", PauseList },
                    { "Status", status },
                };

            #endregion

            #region 发送请求 send request to remote server ..
            do
            {
                // confirm current request status ..
                if (EnvComm.CanRequest)
                {
                    // package sent data ..
                    IList<object> packaged = EnvComm.Package(Client2Server.CommunicationType.JOBQUEUEPAUSE, SendItem);

                    // wait for result ..
                    responseObject = EnvComm.Request(packaged);
                    break;
                }
                Thread.Sleep(500);
            } while (true);
            #endregion

            #region 检测响应信息，返回错误资讯（如果有）
            if (responseObject.Key.Substring(0, 1) == "+")
            {
                MessageBox.Show("succeed!");
            }
            else
            {
                MessageBox.Show("Failed");
            }
            #endregion
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Queue_Repeat_Job_Click(object sender, EventArgs e)
        {
            if (this.ListView_Queue.SelectedItems.Count <= 0)
                return;
            foreach (ListViewItem Item in this.ListView_Queue.SelectedItems)
            {
                #region construct data
                //MessageBox.Show(Item.Text);

                IDictionary<string, object> HistoryData = this.EnvCust.GetJobInfo(Item.Text.Trim(), ref this.EnvComm);

                HistoryData.Remove("Job_Group_Id");
                HistoryData.Add("Submit_Machine", this.EnvNetBase.LocalHostName);
                HistoryData.Add("Submit_Acct", Customization.User);
                HistoryData.Add("Submit_Time", DateTime.Now);
                #endregion

                #region Send Data
                // declare remote server response object ..
                KeyValuePair<string, object> responseObject;

                // 修訂數據 revise item data ..
                IDictionary<string, object> QueueItem = HistoryData;

                if (QueueItem.Count == 0)
                {
                    return;
                }
                do
                {
                    // confirm current resquest status ..
                    if (this.EnvComm.CanRequest)
                    {
                        // assign new status ..
                        if (!QueueItem.ContainsKey("Status"))
                        {
                            QueueItem.Add("Status", (UInt16)JobStatusFlag.QUEUING);
                        }

                        //  打包物件 package sent data type ..
                        IList<object> packaged = this.EnvComm.Package(Client2Server.CommunicationType.JOBQUEUEADD, QueueItem);


                        // 發送數據並等待執行結果 wait for result ..
                        responseObject = this.EnvComm.Request(packaged);

                        break;
                    }
                    Thread.Sleep(100);
                } while (true);

                #endregion

                #region Confirm
                // 確認傳輸是否成功 confirm correct result ..
                if (responseObject.Key.Substring(0, 1).Equals("+"))
                {
                    MessageBox.Show("Repeat succeed !");
                }
                else
                {
                    MessageBox.Show("Repeat Failed !");
                }
                #endregion
            }
        }

        #endregion

        #region 主机列表右键菜单 Host Context Menu Partial
        /// <summary>
        /// 显示菜单 Host(Machine) context menu visible partial.
        /// </summary>
        private void ListView_Host_MouseDown(object sender, MouseEventArgs e)
        {
            //右鍵、列表不為空、選項數目不為空
            if (e.Button == MouseButtons.Right && this.ListView_Host.Items.Count > 0 && this.ListView_Host.SelectedItems.Count > 0)
            {
                this.Host_Add_Machine.Visible = true;
                this.Host_Remove_Machine.Visible = true;
            }
            else
            {
                this.Host_Add_Machine.Visible = false;
                this.Host_Remove_Machine.Visible = false;
            }
        }

        /// <summary>
        /// 添加机器(ONOFFMACHINE:IsEnable- true)  Add machine to renbar operation network.
        /// </summary>
        private void Host_Add_Machine_Click(object sender, EventArgs e)
        {
            if (this.ListView_Host.SelectedItems.Count <= 0)
                return;
            string error_machines = string.Empty;
            for (int s = 0; s < this.ListView_Host.SelectedItems.Count; s++)
            {
                // declare view job log output info array ..
                string[] Info = new string[3];
                // check mouse click selected item ..
                for (int i = 0; i < this.ListView_Host.SelectedItems[s].SubItems.Count - 4; i++)
                {
                    Info[i] = this.ListView_Host.SelectedItems[s].SubItems[i].Text;
                }

                #region
                //判斷其狀態是否為“可用”
                if (Info[2].Equals("Enable"))
                {
                    return;
                }
                else
                {
                    // 發送請求 send request to remote server ..
                    KeyValuePair<string, object> responseObject;
                    IDictionary<string, object> Item = new Dictionary<string, object>
                    {
                        { "Machine_Id", Info[0] },
                        { "IsEnable", true },
                        { "Last_Online_Time", DateTime.Now }
                    };

                    do
                    {
                        // confirm current request status ..
                        if (EnvComm.CanRequest)
                        {
                            // package sent data ..
                            IList<object> packaged = EnvComm.Package(Client2Server.CommunicationType.ONOFFMACHINE, Item);

                            // wait for result ..
                            responseObject = EnvComm.Request(packaged);
                            break;
                        }
                    } while (true);

                    //傳回“-Error……”
                    if (responseObject.Key.Substring(0, 1) == "-")
                        error_machines += Info[1] + ", ";
                }

                #endregion
            }

            // show error message ..
            if (!string.IsNullOrEmpty(error_machines))
            {
                string message = string.Format("{0}\r\n\n{1}", AddMachinesErrMessage, error_machines.Substring(0, (error_machines.Length) - 2));
                MessageBox.Show(this, message, AssemblyInfoClass.ProductInfo,
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
            }
        }

        /// <summary>
        /// 移除机器(ONOFFMACHINE:IsEnable- false) Remove machine to renbar operation network..
        /// </summary>
        private void Host_Remove_Machine_Click(object sender, EventArgs e)
        {
            if (this.ListView_Host.SelectedItems.Count <= 0)
                return;

            string error_machines = string.Empty;

            for (int s = 0; s < this.ListView_Host.SelectedItems.Count; s++)
            {
                // declare view job log output info array ..
                string[] Info = new string[3];

                // check mouse click selected item ..
                for (int i = 0; i < this.ListView_Host.SelectedItems[s].SubItems.Count - 4; i++)
                {
                    Info[i] = this.ListView_Host.SelectedItems[s].SubItems[i].Text;
                }
                #region
                //判斷其狀態是否為“不可用”
                if (Info[2].Equals("Disable"))
                {
                    return;
                }
                else
                {
                    // send request to remote server ..
                    KeyValuePair<string, object> responseObject;
                    IDictionary<string, object> Item = new Dictionary<string, object>
                    {
                        { "Machine_Id", Info[0] },
                        { "IsEnable", false },
                        { "Last_Online_Time", DateTime.Now }
                    };

                    do
                    {
                        // confirm current request status ..
                        if (EnvComm.CanRequest)
                        {
                            // package sent data ..
                            IList<object> packaged = EnvComm.Package(Client2Server.CommunicationType.ONOFFMACHINE, Item);

                            // wait for result ..
                            responseObject = EnvComm.Request(packaged);
                            break;
                        }
                    } while (true);

                    if (responseObject.Key.Substring(0, 1) == "-")
                        error_machines += Info[1] + ", ";
                }
                #endregion
            }

            // show error message ..
            if (!string.IsNullOrEmpty(error_machines))
            {
                string message = string.Format("{0}\r\n\n{1}", RemoveMachinesErrMessage, error_machines.Substring(0, (error_machines.Length) - 2));
                MessageBox.Show(this, message, AssemblyInfoClass.ProductInfo,
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
            }
        }

        /// <summary>
        /// 弹出对话框，设定机器工作优先级 Setting machine priority to renbar operation network.
        /// </summary>
        private void Host_Setting_Priority_Click(object sender, EventArgs e)
        {
            if (this.ListView_Host.SelectedItems.Count <= 0)
                return;

            // create setting render machine amount ..
            IList<string> machines = new List<string>();

            for (int s = 0; s < this.ListView_Host.SelectedItems.Count; s++)
            {
                // 定義臨時變量 declare view job log output info array ..
                string[] Info = new string[7];

                // check mouse click selected item ..
                for (int i = 0; i < this.ListView_Host.SelectedItems[s].SubItems.Count - 1; i++)
                {
                    //填入單個機器的一些信息
                    Info[i] = this.ListView_Host.SelectedItems[s].SubItems[i].Text;
                }

                if (Convert.ToInt16(Info[5]) < 1)
                {
                    return;
                }
                else
                {
                    // 寫入信息 write current data to local data ..
                    DataRow[] row = this.EnvCust.DataListView.Tables["Host"].Select(string.Format("Host_Id = '{0}'", Info[0]));
                    if (row.Length > 0)
                        machines.Add(Info[0]);
                }
            }

            // show priority form ..
            Priority_Form priorityForm = new Priority_Form(ref this.EnvCust, ref this.EnvComm, this.EnvSetting, machines, Client2Server.CommunicationType.MACHINEPRIORITY);

            if (priorityForm.ShowDialog() == DialogResult.OK)
                priorityForm.Dispose();
        }
        #endregion

        #endregion

        #region 鏈接服務器 Connect Server Event Procedure

        /// <summary>
        /// 鏈接服務器 Connect server.
        /// </summary>
        private void Connect()
        {
            #region 顯示連接提示信息
            EnableControlsCallBack ConnectingControl = delegate()
            {
                switch (this.EnvSetting.Lang)
                {
                    case Customization.Language.En_Us:
                        serverLabel.Text = "Server State: Connecting ..";
                        break;
                    case Customization.Language.Zh_Tw:
                        serverLabel.Text = "伺服器狀態: 連接中 ..";
                        break;
                }
                if (!this.Renbar_StatusBar.Items.Contains(serverLabel))
                {
                    // add connect label to status bar ..
                    this.Renbar_StatusBar.Items.Add(serverLabel);
                }
            };
            // invoke object ……
            this.Invoke(ConnectingControl);
            #endregion

            #region 鏈接成功
            if (this.EnvCust.ConvertConnection(ref this.EnvSetting))
            {
                this.EnvComm.Connect(EnvSetting.ServerIpAddress, EnvSetting.ServerPort);

                // change connected flag ..
                this.IsConnected = true;

                #region  Invoke Enable Controls Delegate Procedure
                EnableControlsCallBack ConnectedControl = delegate()
                {
                    switch (this.EnvSetting.Lang)
                    {
                        case Customization.Language.En_Us:
                            serverLabel.Text = "Server State: Connected";
                            break;
                        case Customization.Language.Zh_Tw:
                            serverLabel.Text = "伺服器狀態: 已連接";
                            break;
                    }
                };
                #endregion
                this.Invoke(ConnectedControl);

                #region Invoke Enable Controls Delegate Procedure
                EnableControlsCallBack EnableMenuControl = delegate()
                {
                    // remove server state lable ..
                    this.Renbar_StatusBar.Items.Remove(serverLabel);
                    // enable controls ..
                    this.Menu_Queue_Job.Enabled = true;
                    this.Menu_Settings_PoolMgr.Enabled = true;
                };
                 #endregion

                #region 注冊機器信息

                //registry local machine to server request ..
                this.EnvCust.RegLocalMachine(null, false, ref this.EnvComm);
                    // invoke object ……
                this.Invoke(EnableMenuControl);
                #endregion

                #region 創建線程獲取數據
                //create data state thread ..
                this.ViewStateTherad = new Thread(new ThreadStart(this.ListViewItemState));
                this.ViewStateTherad.IsBackground = false;
                this.ViewStateTherad.Priority = ThreadPriority.AboveNormal;
                this.ViewStateTherad.Start();
                #endregion

                #region 創建線程探測鏈接
                // start detection connect status thread ..
                new Thread(new ThreadStart(this.Detection)).Start();
                #endregion
            }
            #endregion

            #region 連接失敗
            else
            {
                if (!this.IsConnected)
                {
                    EnableControlsCallBack DisConnControl = delegate()
                    {
                        switch (this.EnvSetting.Lang)
                        {
                            case Customization.Language.En_Us:
                                serverLabel.Text = "Server State: Can't Connect";
                                break;
                            case Customization.Language.Zh_Tw:
                                serverLabel.Text = "伺服器狀態: 無法連接";
                                break;
                        }
                    };
                    this.Invoke(DisConnControl);
                }
            }
            #endregion
        }

        /// <summary>
        /// 檢測鏈接服務器狀態 Detection connect server status ..
        /// </summary>
        private void Detection()
        {
            #region
            //bool fail = false;
            //do
            //{
            //    // suspend 1 sec ..
            //    Thread.Sleep(1000);
            //    #region 檢測是否可連接，調用窗體控制方法控制顯示

            //    if (this.EnvCust.NetworkPing(this.EnvSetting))
            //    {
            //        if (!fail)
            //        {
            //            continue;
            //        }
            //        // change connect fail flag ..
            //        fail = false;
            //        // change connected flag ..
            //        IsConnected = true;
            //        #region Invoke Enable Controls Delegate Procedure
            //        EnableControlsCallBack IsEnableControl = delegate()
            //        {
            //            // enabled controls ..
            //            this.Menu_Queue_Job.Enabled = true;
            //            this.Menu_Settings_PoolMgr.Enabled = true;

            //            // remove server state lable ..
            //            this.Renbar_StatusBar.Items.Remove(serverLabel);
            //        };
            //        // invoke object ..
            //        this.Invoke(IsEnableControl);
            //        #endregion
            //    }

            //    else
            //    {
            //        #endregion

            //        if (fail)
            //        {
            //            continue;
            //        }

            //        // change connect fail flag ..
            //        fail = true;

            //        // change connected flag ..
            //        this.IsConnected = false;

            //        #region 控制顯示 Invoke Enable Controls Delegate Procedure
            //        DisableControlsCallBack IsDisableControl = delegate()
            //        {
            //            // disable controls ..
            //            this.Menu_Queue_Job.Enabled = false;
            //            this.Menu_Settings_PoolMgr.Enabled = false;

            //            // 清空列表 clear current all list-view items ..
            //            this.ListView_Host.Items.Clear();
            //            this.ListView_Process.Items.Clear();
            //            this.ListView_Queue.Items.Clear();

            //            // 添加狀態提示欄 add connect label to status bar ..
            //            this.Renbar_StatusBar.Items.Add(serverLabel);
            //            //設定語言
            //            switch (this.EnvSetting.Lang)
            //            {
            //                case Customization.Language.En_Us:
            //                    serverLabel.Text = "Server State: Server disconnected";
            //                    break;
            //                case Customization.Language.Zh_Tw:
            //                    serverLabel.Text = "伺服器狀態: 與伺服器連線中斷";
            //                    break;
            //            }
            //        };

            //        // invoke object ..
            //        this.Invoke(IsDisableControl);
            //        #endregion
            //    }

            //    #endregion
            //} while (!requeststop);

            #endregion

            bool fail = false;

            do
            {
                Settings OldSettings = this.EnvSetting;
                // suspend 1 sec ..
                Thread.Sleep(1000);

                if (this.EnvCust.ConvertConnection(ref this.EnvSetting))
                {
                    if (!fail && OldSettings.ServerIpAddress == this.EnvSetting.ServerIpAddress)
                    {
                        continue;
                    }
                    // change connect fail flag ..
                    fail = false;

                    this.EnvComm.Disconnect();
                    this.EnvComm.Connect(EnvSetting.ServerIpAddress, EnvSetting.ServerPort);                   

                    // change connected flag ..
                    this.IsConnected = true;

                    #region Invoke Enable Controls Delegate Procedure
                    try
                    {
                        
                        EnableControlsCallBack IsEnableControl = delegate()
                        {
                            // enabled controls ..
                            this.Menu_Queue_Job.Enabled = true;
                            this.Menu_Settings_PoolMgr.Enabled = true;

                            if (this.Renbar_StatusBar.Items.Contains(serverLabel))
                            {
                                // remove server state lable ..
                                this.Renbar_StatusBar.Items.Remove(serverLabel);
                            }
                        };
                        // invoke object ..
                        this.Invoke(IsEnableControl);
                    }
                    catch { }
                    #endregion

                }
                else//全鏈接不上
                {
                    if (fail)
                    {
                        continue;
                    }
                    // change connect fail flag ..
                    fail = true;
                    // change connected flag ..
                    this.IsConnected = false;

                    //this.EnvComm.Disconnect();

                    #region 控制顯示 Invoke Enable Controls Delegate Procedure
                    DisableControlsCallBack IsDisableControl = delegate()
                    {
                        // disable controls ..
                        this.Menu_Queue_Job.Enabled = false;
                        this.Menu_Settings_PoolMgr.Enabled = false;

                        // 清空列表 clear current all list-view items ..
                        this.ListView_Host.Items.Clear();
                        this.ListView_Process.Items.Clear();
                        this.ListView_Queue.Items.Clear();
                        try
                        {

                            // 添加狀態提示欄 add connect label to status bar ..
                            this.Renbar_StatusBar.Items.Add(serverLabel);
                            //設定語言
                            switch (this.EnvSetting.Lang)
                            {
                                case Customization.Language.En_Us:
                                    serverLabel.Text = "Server State: Server disconnected";
                                    break;
                                case Customization.Language.Zh_Tw:
                                    serverLabel.Text = "伺服器狀態: 與伺服器連線中斷";
                                    break;
                            }
                        }
                        catch { }
                    };
                    // invoke object ..
                    this.Invoke(IsDisableControl);
                    #endregion
                }

            } while (!requeststop);
        }

        #endregion

        #region ListView相關鼠標事件 ListView Control Event Procedure

        #region 表頭點擊事件 ListView ColumnClick Events
        /// <summary>
        /// Process頭點擊事件——排序 Process head column click event.
        /// </summary>
        private void ListView_Process_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            // 如果點擊的是已設定為排序列的列頭，則反向排序
            //determine if clicked column is already the column that is being sorted ..
            if (e.Column == this.ProcListViewSorter.SortColumn)
            {
                // reverse the current sort direction for this column ..
                if (this.ProcListViewSorter.Order == SortOrder.Ascending)
                {
                    this.ProcListViewSorter.Order = SortOrder.Descending;
                }
                else
                {
                    this.ProcListViewSorter.Order = SortOrder.Ascending;
                }
            }
            else
            {
                // 否則設定點擊列為排序列並升序排列
                //set the column number that is to be sorted; default to ascending ..
                this.ProcListViewSorter.SortColumn = e.Column;
                this.ProcListViewSorter.Order = SortOrder.Ascending;
            }
            // 排序 perform the sort with these new sort options ..
            this.ListView_Process.Sort();
        }

        /// <summary>
        /// Queue頭點擊事件——排序 Queue column click event.
        /// </summary>
        private void ListView_Queue_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            // determine if clicked column is already the column that is being sorted ..
            if (e.Column == this.QueuListViewSorter.SortColumn)
            {
                // reverse the current sort direction for this column ..
                if (this.QueuListViewSorter.Order == SortOrder.Ascending)
                    this.QueuListViewSorter.Order = SortOrder.Descending;
                else
                    this.QueuListViewSorter.Order = SortOrder.Ascending;
            }
            else
            {
                // set the column number that is to be sorted; default to ascending ..
                this.QueuListViewSorter.SortColumn = e.Column;
                this.QueuListViewSorter.Order = SortOrder.Ascending;
            }

            // perform the sort with these new sort options ..
            this.ListView_Queue.Sort();
        }

        /// <summary>
        /// Host頭點擊事件——排序 Host column click event.
        /// </summary>
        private void ListView_Host_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            // determine if clicked column is already the column that is being sorted ..
            if (e.Column == this.HostListViewSorter.SortColumn)
            {
                // reverse the current sort direction for this column ..
                if (this.HostListViewSorter.Order == SortOrder.Ascending)
                    this.HostListViewSorter.Order = SortOrder.Descending;
                else
                    this.HostListViewSorter.Order = SortOrder.Ascending;
            }
            else
            {
                // set the column number that is to be sorted; default to ascending ..
                this.HostListViewSorter.SortColumn = e.Column;
                this.HostListViewSorter.Order = SortOrder.Ascending;
            }

            // perform the sort with these new sort options ..
            this.ListView_Host.Sort();
        }
        #endregion

        #region ListView_Queue拖放事件 Queue ListView Drag Events
        /// <summary>
        /// 設定拖放效果 Process job drag enter event.
        /// </summary>
        private void ListView_Queue_DragEnter(object sender, DragEventArgs e)
        {
            //檢查以確定資料是否可為指定的格式，或資料是否可轉換成指定的格式
            //DataFormats提供一組預先定義的資料格式名稱，這些名稱可用以識別剪貼簿或拖放作業中可用的資料格式
            //DataFormats.FileDrop指定Windows檔案置放格式

            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                //指定拖放作業的效果
                e.Effect = DragDropEffects.All;     //資料從拖曳來源中複製、移除，並在置放目標 (Drop Target) 中捲動
            else
                e.Effect = DragDropEffects.None; //置放目標不接受資料
        }

        /// <summary>
        /// 处理任务拖放事件 Process job drag drop event.
        /// </summary>
        private void ListView_Queue_DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                // get drop files ..
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop, false);

                if (files.Length == 1)
                {
                    if (this.IsConnected)
                    {
                        // create job form instance ..？？？？？？？？？？？？？兩個Pool的作用？？？？？？？？？？？？？？？
                        Job_Form jobForm = new Job_Form(ref this.EnvCust, ref this.EnvComm, this.EnvSetting)
                        {
                            Pool = this.EnvCust.DropDown.Pool,
                            Pool2 = this.EnvCust.DropDown.Pool2,
                            WaitFor = this.EnvCust.DropDown.Waitfor,
                            IsExtern = true
                        };

                        // declare parse render file variable ..
                        IDictionary<string, object> attributes = new Dictionary<string, object>();

                        //解析文件結構，成功則會講數據存入引用地址中，並傳遞數據給Job_Form（0 = Success, 1 = Parse Error, 2 = Multi File Error）
                        int _p = this.EnvCust.Parse(files[0], ref attributes);

                        switch (_p)
                        {
                            case 0:
                                // assign parse properties ..
                                jobForm.BackupFilename = files[0];
                                jobForm.ExternItems = attributes;

                                // 成功則顯示Job_Form  show job form ..
                                if (jobForm.ShowDialog(this) == DialogResult.OK)
                                    jobForm.Dispose();
                                break;

                            case 1:
                                // show parse error message ..
                                MessageBox.Show(this, this.ParseFileErrMessage, AssemblyInfoClass.ProductInfo,
                                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                                break;

                            case 2:
                                // show parse multi error message ..
                                MessageBox.Show(this, this.MulitFileErrMessage, AssemblyInfoClass.ProductInfo,
                                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                                break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string ExceptionMsg = ex.Message + ex.StackTrace;
                // write to log file ..
                EnvLog.Writer(AssemblyInfoClass.ProductInfo, Log.Level.Error, ExceptionMsg, true);
            }
        }
        #endregion
        #endregion

        #region 刷新列表 Refresh ListView Status Event Procedure
        /// <summary>
        /// 維護資料狀態 Maintenance item state.
        /// </summary>
        private void ListViewItemState()
        {
            //  create list item threads ……
            Thread
                S_Thread = new Thread(new ThreadStart(this.Latest_Thread)),
               P_Thread = new Thread(new ThreadStart(this.Processing_Thread)),
                Q_Thread = new Thread(new ThreadStart(this.Queue_Thread)),
               H_Thread = new Thread(new ThreadStart(this.Host_Thread));



            // setting threads properties ..
            S_Thread.IsBackground = true;
            S_Thread.Priority = ThreadPriority.BelowNormal;

            P_Thread.IsBackground = true;
            P_Thread.Priority = ThreadPriority.BelowNormal;

            Q_Thread.IsBackground = true;
            Q_Thread.Priority = ThreadPriority.BelowNormal;

            H_Thread.IsBackground = true;
            H_Thread.Priority = ThreadPriority.BelowNormal;



            ////////////// start thread ..
            if (S_Thread.ThreadState == (ThreadState.Background | ThreadState.Unstarted))
                S_Thread.Start();

            this.EnvCust.DataListView.Merge(this.EnvCust.DataState(ref this.EnvComm).Copy(), false);
            this.EnvCust.DataListView.AcceptChanges();

            //////////////////// start status work threads ..
            if (P_Thread.ThreadState == (ThreadState.Background | ThreadState.Unstarted))
                P_Thread.Start();
            if (Q_Thread.ThreadState == (ThreadState.Background | ThreadState.Unstarted))
                Q_Thread.Start();
            if (H_Thread.ThreadState == (ThreadState.Background | ThreadState.Unstarted))
                H_Thread.Start();
        }


        /// <summary>
        /// 獲取最新數據狀態 Get latest data status thread...
        /// </summary>
        private void Latest_Thread()
        {
            //如果有執行緒取得 Mutex，也想取得 Mutex 的第二個執行緒會被暫停，直到第一個執行緒釋出 Mutex (ReleaseMutex)為止            
            Mutex stateNotify = new Mutex();

            do
            {
                // wait until it is safe to enter ..
                if (!stateNotify.WaitOne(TimeSpan.Zero, true))
                    return;
                else
                {
                    //  get current processor idle time ..
                    global::System.Diagnostics.PerformanceCounter performance = new global::System.Diagnostics.PerformanceCounter();

                    // setting performance counter category and instance name ...
                    performance.CategoryName = "Processor";
                    performance.CounterName = "% Idle Time";
                    performance.InstanceName = "_Total";

                    // get current idle time ..
                    if (!this.__suspend)
                    {
                        if (50 < performance.NextValue())
                        {
                            if (70 < performance.NextValue())
                                Thread.Sleep(500);
                            if (50 < performance.NextValue())
                                Thread.Sleep(5000);
                        }
                        else
                        {
                            Thread.Sleep(500);
                        }

                        #region F7300290 Suspend this thread when the WindowState is Minimized!
                        if (this.IsConnected)
                        {
                            // assign currently status data ..
                            this.EnvCust.DataListView = this.EnvCust.DataState(ref this.EnvComm).Copy();

                            this.EnvCust.DataListView.AcceptChanges();
                        }
                        else
                        {
                            // clear all status data ..
                            this.EnvCust.DataListView = this.EnvCust.DataListView.Clone();
                            this.EnvCust.DataListView.AcceptChanges();
                        }

                        #endregion
                    }
                    else
                    {
                        Thread.Sleep(15000);
                    }

                    ////////////if (this.IsConnected)
                    ////////////{
                    ////////////    // assign currently status data ..
                    ////////////    this.EnvCust.DataListView = this.EnvCust.DataState(ref this.EnvComm).Copy();

                    ////////////    this.EnvCust.DataListView.AcceptChanges();
                    ////////////}
                    ////////////else
                    ////////////{
                    ////////////    // clear all status data ..
                    ////////////    this.EnvCust.DataListView = this.EnvCust.DataListView.Clone();
                    ////////////    this.EnvCust.DataListView.AcceptChanges();
                    ////////////}

                    //  release the mutex ..
                    stateNotify.ReleaseMutex();
                }



            } while (true);
        }

        /// <summary>
        /// 更新處理中的物件 Refresh processing items thread...
        /// </summary>
        private void Processing_Thread()
        {
            do
            {
                if (this.Suspend.WaitOne()) // 鎖定，直到目前的WaitHandle收到信號為止
                {
                    if (this.IsConnected)
                    {
                        // 依據主鍵作為對比更新數據？？？？？？？？？？？？？？？？？？？？？？？？？？？？
                        // 返回所有增删改和未改变的数据
                        // refresh latest data ..
                        DataTable InvokeData = this.EnvCust.DataComparison(
                            this.EnvCust.DataListView.Tables["Processing"], this.EnvCust.DataListView.Tables["Processing"].PrimaryKey);

                        if (InvokeData != null)
                        {
                            // 鎖定臨時數據表 try lock object wait for 0 second ..
                            if (Monitor.TryEnter(InvokeData, 0))//如果在指定的毫秒數時間內取得指定物件的獨佔鎖定成功
                            {
                                try
                                {
                                    #region 定義委托事件 Delegate Event Handler
                                    ProcListViewCallBack ProcViewList = delegate(ListViewItem Item, string DeleteItemText)
                                    {
                                        #region 按傳入參數刪除物件 Delete Item Partial
                                        if (!string.IsNullOrEmpty(DeleteItemText))//？？？？？？？？？？？？？？？？？
                                        {
                                            for (int i = 0; i < this.ListView_Process.Items.Count; i++)
                                            {
                                                // processing application all events ..
                                                Application.DoEvents();

                                                if (this.ListView_Process.Items[i].SubItems[0].Text == DeleteItemText)
                                                    // remove item ..
                                                    this.ListView_Process.Items[i].Remove();

                                                // calculate items ..
                                                this.GroupBox_Process.Text = this.ListViewText[0] + string.Format(" ({0})", this.ListView_Process.Items.Count.ToString());

                                                // refresh control ..
                                                this.ListView_Process.Update();
                                            }

                                            return;
                                        }
                                        #endregion

                                        #region 逐行對比，更新物件 Update Item Partial
                                        foreach (ListViewItem ProcItem in this.ListView_Process.Items)
                                        {
                                            // processing application all events ..
                                            Application.DoEvents();

                                            if (ProcItem.Text == Item.Text)
                                            {
                                                if (ProcItem.SubItems[1].Text == Item.SubItems[1].Text &&
                                                    ProcItem.SubItems[2].Text == Item.SubItems[2].Text &&
                                                    ProcItem.SubItems[3].Text == Item.SubItems[3].Text &&
                                                    ProcItem.SubItems[4].Text == Item.SubItems[4].Text)
                                                    return;

                                                // update the item of sub items ..
                                                for (int i = 1; i < ProcItem.SubItems.Count; i++)
                                                    ProcItem.SubItems[i] = Item.SubItems[i];

                                                // apply fore color ..
                                                ProcItem.ForeColor = Item.ForeColor;
                                                return;
                                            }
                                        }
                                        #endregion

                                        // add new items to the listview control ..
                                        this.ListView_Process.Items.Add(Item);

                                        // 更新計數 calculate items ..
                                        this.GroupBox_Process.Text = this.ListViewText[0] + string.Format(" ({0})", this.ListView_Process.Items.Count.ToString());

                                        // refresh control ..
                                        this.ListView_Process.Update();
                                    };
                                    #endregion

                                    #region Added, Modified And Unchanged State Loop
                                    // get add, modified and unchanged row(s) ..
                                    DataTable InvokeRows = InvokeData.GetChanges(DataRowState.Added | DataRowState.Modified | DataRowState.Unchanged);

                                    if (InvokeRows != null)
                                    {
                                        foreach (DataRow row in InvokeRows.Rows)
                                        {
                                            // 處理目前在訊息佇列中的所有Windows訊息 processing application all events ..
                                            Application.DoEvents();



                                            // add proc log to dictionary ...
                                            if (!ProcLog.ContainsKey(row["Proc_Id"].ToString()))
                                                ProcLog.Add(row["Proc_Id"].ToString(), row["Log"].ToString());
                                            else
                                                // update last log texts ..
                                                ProcLog[row["Proc_Id"].ToString()] = row["Log"].ToString();

                                            // declare connected column variable ..
                                            string Elapsed = this.EnvSvr.DateTimeInterval(DateTime.Now, Convert.ToDateTime(row["Elapsed"]));

                                            string[] subitems = {
                                                row["Proc_Id"].ToString().Trim(), //ID
                                                row["Job"].ToString().Trim(),//任務名
                                                row["Frames"].ToString().Trim(),//張數
                                                row["Host"].ToString().Trim(),//主機名
                                                Elapsed//經過時間
                                            };

                                            ListViewItem Item = new ListViewItem(subitems);

                                            try
                                            {
                                                // 調用委托？？？？？？？？？？？？？？
                                                // Invoke process list view delegate control ...
                                                this.Invoke(ProcViewList, new object[] { Item, null });
                                            }
                                            catch (InvalidOperationException)
                                            {
                                                // if delegate object already clean, exit loop ..
                                                break;
                                            }
                                        }
                                    }
                                    #endregion

                                    #region 已刪除 Deleted State Loop
                                    // 獲取已刪除的數據 get delete row(s) ..
                                    DataTable InvokeDeleteRows = InvokeData.GetChanges(DataRowState.Deleted);

                                    if (InvokeDeleteRows != null)
                                    {
                                        // 臨時方法 temporary method ..
                                        InvokeDeleteRows.RejectChanges(); //復原變更

                                        foreach (DataRow row in InvokeDeleteRows.Rows)
                                        {
                                            try
                                            {
                                                // invoke process list view delegate control ...？？？？？？？？？？？？？？？？
                                                this.Invoke(ProcViewList, new object[] { new ListViewItem(), row["Proc_Id"].ToString() });

                                                // remove output log ...
                                                if (ProcLog.ContainsKey(row["Proc_Id"].ToString()))
                                                    ProcLog.Remove(row["Proc_Id"].ToString());
                                            }
                                            catch (InvalidOperationException)
                                            {
                                                // if delegate object already clean, exit loop ..
                                                return;
                                            }
                                        }
                                    }
                                    #endregion
                                }
                                finally
                                {
                                    // 釋出指定物件的獨佔鎖定 release locked object ..
                                    Monitor.Exit(InvokeData);
                                }
                            }
                        }

                        // processing application all events ...
                        Application.DoEvents();
                    }

                    // set access state timespan ..
                    Thread.Sleep(1000);
                }
            } while (true);
        }

        /// <summary>
        /// 更新等待的物件 Refresh queue items thread...
        /// </summary>
        private void Queue_Thread()
        {
            do
            {
                if (this.IsConnected)
                {
                    // refresh latest data ..
                    DataTable InvokeData = this.EnvCust.DataComparison(this.EnvCust.DataListView.Tables["Queue"],
                        this.EnvCust.DataListView.Tables["Queue"].PrimaryKey);

                    if (InvokeData != null)
                    {
                        // try lock object wait for 0 second ..
                        if (Monitor.TryEnter(InvokeData, 0))
                        {
                            try
                            {
                                #region Delegate Event Handler
                                QueueListViewCallBack QueueViewList = delegate(ListViewItem Item, string DeleteItemText)
                                {
                                    #region Delete Item Partial
                                    if (!string.IsNullOrEmpty(DeleteItemText))
                                    {
                                        for (int i = 0; i < this.ListView_Queue.Items.Count; i++)
                                        {
                                            // processing application all events ..
                                            Application.DoEvents();

                                            if (this.ListView_Queue.Items[i].SubItems[0].Text == DeleteItemText)
                                            {
                                                if (this.ListView_Queue.Items[i].SubItems[10].Text == Customization.User && !this.manual_Delete)
                                                {
                                                    // disabled notification content clickable ..
                                                    this.RenbarNotifier.ContentClickable = false;

                                                    // play notification sound ..
                                                    if (this.nPlayer.IsLoadCompleted)
                                                    {
                                                        this.nPlayer.Play();
                                                    }
                                                    // show notification on taskbar ..
                                                    this.RenbarNotifier.Show(this.Notification_Title, this.Notification_Completed, 350, 10000, 350);
                                                }

                                                // remove item ..
                                                this.ListView_Queue.Items[i].Remove();
                                            }

                                            if (this.manual_Delete)
                                                // restore manual delete flag ..
                                                this.manual_Delete = false;

                                            // calculate items ..
                                            this.GroupBox_Queue.Text = this.ListViewText[1] + string.Format(" ({0})", this.ListView_Queue.Items.Count.ToString());

                                            // refresh control ..
                                            this.ListView_Queue.Update();
                                        }

                                        return;
                                    }
                                    #endregion

                                    #region Update Item Partial
                                    foreach (ListViewItem QueueItem in this.ListView_Queue.Items)
                                    {
                                        // processing application all events ..
                                        Application.DoEvents();

                                        if (QueueItem.Text == Item.Text)
                                        {
                                            if (QueueItem.SubItems[1].Text == Item.SubItems[1].Text &&
                                                QueueItem.SubItems[2].Text == Item.SubItems[2].Text &&
                                                QueueItem.SubItems[4].Text == Item.SubItems[4].Text)
                                                return;

                                            // update the item of sub items ..
                                            for (int i = 1; i < QueueItem.SubItems.Count; i++)
                                                QueueItem.SubItems[i] = Item.SubItems[i];

                                            // apply fore color ..
                                            QueueItem.ForeColor = Item.ForeColor;

                                            if (QueueItem.SubItems[1].Text == JobStatusFlag.ERROR.ToString())
                                            {
                                                // disabled notification content clickable ..
                                                this.RenbarNotifier.ContentClickable = true;

                                                // play notification sound ..
                                                if (this.nPlayer.IsLoadCompleted)
                                                    this.nPlayer.Play();

                                                // show notification on taskbar ..
                                                this.RenbarNotifier.Show(this.Notification_Title, this.Notification_Error, 350, 10000, 350);
                                            }

                                            return;
                                        }
                                    }
                                    #endregion

                                    // add new items to the listview control ..
                                    this.ListView_Queue.Items.Add(Item);

                                    // calculate items ..
                                    this.GroupBox_Queue.Text = this.ListViewText[1] + string.Format(
                                        " ({0})", this.ListView_Queue.Items.Count.ToString());

                                    // refresh control ..
                                    this.ListView_Queue.Update();
                                };
                                #endregion

                                #region Added, Modified And Unchanged State Loop
                                // get add, modified and unchanged row(s) ..
                                DataTable InvokeRows = InvokeData.GetChanges(
                                    DataRowState.Added | DataRowState.Modified | DataRowState.Unchanged);

                                if (InvokeRows != null)
                                {
                                    foreach (DataRow row in InvokeRows.Rows)
                                    {
                                        // processing application all events ..
                                        Application.DoEvents();

                                        string[] subitems = {
                                            row["Queue_Id"].ToString().Trim(),
                                            row["Status_Id"].ToString().Trim(),
                                            row["Completed"].ToString().Trim(),
                                            row["ProcType"].ToString().Trim(),
                                            row["Priority"].ToString().Trim(),
                                            row["Project"].ToString().Trim(),
                                            row["Job"].ToString().Trim(),
                                            row["Frames"].ToString().Trim(),
                                            row["First_Pool"].ToString().Trim(),
                                            row["Second_Pool"].ToString().Trim(),
                                            row["Submited_User"].ToString().Trim(),
                                            row["Submited_Time"].ToString().Trim(),
                                            row["Note"].ToString().Trim()
                                        };

                                        ListViewItem Item = new ListViewItem(subitems);

                                        #region Change Item Force Color
                                        switch (row["Status_Id"].ToString())
                                        {
                                            case "PROCESSING":
                                                // green color ..
                                                Item.ForeColor = Color.FromArgb(0, 159, 0);
                                                break;

                                            case "PAUSE":
                                                // gray color ..
                                                Item.ForeColor = Color.FromArgb(154, 154, 154);
                                                break;

                                            case "UPDATEONLY":
                                                // pink color ..
                                                Item.ForeColor = Color.FromArgb(240, 120, 154);
                                                break;

                                            case "CHECKING":
                                                // deep blue color ..
                                                Item.ForeColor = Color.FromArgb(0, 83, 124);
                                                break;

                                            case "GETLATEST":
                                                // blue color ..
                                                Item.ForeColor = Color.FromArgb(0, 83, 170);
                                                break;

                                            case "ERROR":
                                                // red color ..
                                                Item.ForeColor = Color.FromArgb(255, 0, 0);
                                                break;
                                        }
                                        #endregion

                                        try
                                        {
                                            // invoke queue list view delegate control ..
                                            this.Invoke(QueueViewList, new object[] { Item, null });
                                        }
                                        catch (InvalidOperationException)
                                        {
                                            // if delegate object already clean, exit loop ..
                                            break;
                                        }
                                    }
                                }
                                #endregion

                                #region Deleted State Loop
                                // get delete row(s) ..
                                DataTable InvokeDeleteRows = InvokeData.GetChanges(DataRowState.Deleted);

                                if (InvokeDeleteRows != null)
                                {
                                    // temporary method ..
                                    InvokeDeleteRows.RejectChanges();

                                    foreach (DataRow row in InvokeDeleteRows.Rows)
                                    {
                                        try
                                        {
                                            // invoke queue list view delegate control ..
                                            this.Invoke(QueueViewList, new object[] { new ListViewItem(), row["Queue_Id"].ToString() });
                                        }
                                        catch (InvalidOperationException)
                                        {
                                            // if delegate object already clean, exit loop ..
                                            return;
                                        }
                                    }
                                }
                                #endregion
                            }
                            finally
                            {
                                // release locked object ..
                                Monitor.Exit(InvokeData);
                            }
                        }
                    }

                    // processing application all events ..
                    Application.DoEvents();
                }

                // set access state timespan ..
                if (!this.__suspend)
                    Thread.Sleep(1000);
                else
                    Thread.Sleep(10000);
            } while (true);
        }

        /// <summary>
        /// 更新主機物件Refresh host items thread...
        /// </summary>
        private void Host_Thread()
        {
            do
            {
                if (this.Suspend.WaitOne())
                {
                    if (this.IsConnected)
                    {
                        // refresh latest data ..
                        DataTable InvokeData = this.EnvCust.DataComparison(this.EnvCust.DataListView.Tables["Host"],
                            this.EnvCust.DataListView.Tables["Host"].PrimaryKey);

                        if (InvokeData != null)
                        {
                            // try lock object wait for 0 second …
                            if (Monitor.TryEnter(InvokeData, 0))
                            {
                                try
                                {
                                    #region Delegate Event Handler
                                    HostListViewCallBack HostViewList = delegate(ListViewItem Item, string DeleteItemText)
                                    {
                                        #region Delete Item Partial
                                        if (!string.IsNullOrEmpty(DeleteItemText))
                                        {
                                            for (int i = 0; i < this.ListView_Host.Items.Count; i++)
                                            {
                                                // processing application all events ..
                                                Application.DoEvents();

                                                if (this.ListView_Host.Items[i].SubItems[0].Text == DeleteItemText)
                                                    // remove item ..
                                                    this.ListView_Host.Items[i].Remove();

                                                // calculate items ..
                                                this.GroupBox_Host.Text = this.ListViewText[2] + string.Format(
                                                    " ({0})", this.ListView_Host.Items.Count.ToString());

                                                // refresh control ..
                                                this.ListView_Host.Update();
                                            }

                                            return;
                                        }
                                        #endregion

                                        #region Update Item Partial
                                        foreach (ListViewItem MachineItem in this.ListView_Host.Items)
                                        {
                                            // processing application all events ..
                                            Application.DoEvents();

                                            if (MachineItem.Text == Item.Text)
                                            {
                                                if (MachineItem.SubItems[1].Text == Item.SubItems[1].Text &&
                                                    MachineItem.SubItems[2].Text == Item.SubItems[2].Text &&
                                                    MachineItem.SubItems[3].Text == Item.SubItems[3].Text &&
                                                    MachineItem.SubItems[4].Text == Item.SubItems[4].Text &&
                                                    MachineItem.SubItems[5].Text == Item.SubItems[5].Text &&
                                                    MachineItem.SubItems[6].Text == Item.SubItems[6].Text)
                                                    return;

                                                // update the item of sub items ..
                                                for (int i = 1; i < MachineItem.SubItems.Count; i++)
                                                    MachineItem.SubItems[i] = Item.SubItems[i];

                                                // apply fore color ..
                                                MachineItem.ForeColor = Item.ForeColor;
                                                return;
                                            }
                                        }
                                        #endregion

                                        // add new items to the listview control ..
                                        this.ListView_Host.Items.Add(Item);

                                        // calculate items ..
                                        this.GroupBox_Host.Text = this.ListViewText[2] + string.Format(" ({0})", this.ListView_Host.Items.Count.ToString());

                                        // refresh control ..
                                        this.ListView_Host.Update();
                                    };
                                    #endregion

                                    #region Added, Modified And Unchanged State Loop
                                    // get add, modified and unchanged row(s) ……
                                    DataTable InvokeRows = InvokeData.GetChanges(DataRowState.Added | DataRowState.Modified | DataRowState.Unchanged);

                                    if (InvokeRows != null)
                                    {
                                        foreach (DataRow row in InvokeRows.Rows)
                                        {
                                            // processing application all events ..
                                            Application.DoEvents();

                                            // convert connected column value ..
                                            string Connected = this.EnvSvr.DateTimeInterval(DateTime.Now, Convert.ToDateTime(row["Connected_Time"]));

                                            if (row["Status"].ToString().Trim().Equals("Disabled"))
                                            {
                                                Connected = string.Empty;
                                            }

                                            string[] subitems = {
                                                row["Host_Id"].ToString().Trim(),
                                                row["Host"].ToString().Trim(),
                                                row["Status"].ToString().Trim(),
                                                row["Processors"].ToString().Trim(),
                                                Connected,
                                                row["Priority"].ToString().Trim(),
                                                row["Note"].ToString().Trim()
                                                                };

                                            ListViewItem Item = new ListViewItem(subitems);

                                            #region Change Item Force Color
                                            if (row["Status"].ToString().Equals("Enabled"))
                                            {
                                                Item.ForeColor = Color.FromArgb(30, 144, 255);
                                            }
                                            else
                                            {
                                                Item.ForeColor = Color.FromArgb(254, 106, 147);
                                            }
                                            switch (row["Type"].ToString())
                                            {
                                                case "CLIENT":
                                                    // deep gray color ..
                                                    Item.ForeColor = Color.FromArgb(100, 100, 100);
                                                    break;

                                                case "MAINTENANCE":
                                                    // red color ..
                                                    Item.SubItems[4].Text = string.Empty;
                                                    Item.ForeColor = Color.FromArgb(255, 0, 0);
                                                    break;

                                                case "OFFLINE":
                                                    // gray color ..
                                                    Item.SubItems[4].Text = string.Empty;
                                                    Item.ForeColor = Color.FromArgb(150, 150, 150);
                                                    break;
                                            }
                                            #endregion

                                            try
                                            {
                                                // invoke machine(host) list view delegate control ..
                                                this.Invoke(HostViewList, new object[] { Item, null });
                                            }
                                            catch (InvalidOperationException)
                                            {
                                                // if delegate object already clean, exit loop ..
                                                return;
                                            }
                                        }
                                    }
                                    #endregion

                                    #region Deleted State Loop
                                    // get delete row(s) ..
                                    DataTable InvokeDeleteRows = InvokeData.GetChanges(DataRowState.Deleted);

                                    if (InvokeDeleteRows != null)
                                    {
                                        // temporary method ..
                                        InvokeDeleteRows.RejectChanges();
                                        foreach (DataRow row in InvokeDeleteRows.Rows)
                                        {
                                            try
                                            {
                                                // invoke machine(host) list view delegate control ..
                                                this.Invoke(HostViewList, new object[] { new ListViewItem(), row["Host_Id"].ToString() });
                                            }
                                            catch (InvalidOperationException)
                                            {
                                                // if delegate object already clean, exit loop ..
                                                return;
                                            }
                                        }
                                    }
                                    #endregion
                                }
                                finally
                                {
                                    // release locked object ..
                                    Monitor.Exit(InvokeData);
                                }
                            }
                        }

                        // processing application all events ..
                        Application.DoEvents();
                    }

                    // set access state timespan ..
                    Thread.Sleep(1000);
                }
            } while (true);
        }
        #endregion

    }
}