#region Using NameSpace
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Windows.Forms;

// import renbar server library namespace ..
using RenbarLib.Environment.Forms.Controls.ListView.Sort;
#endregion

namespace RenbarServerGUI
{
    public partial class Main_Form : Form
    {
        #region Declare Global Variable Section
        /************************ reference renbar class library section *******************************/
        // create renbar service class object ..
        private global::RenbarLib.Environment.Service Svr = new global::RenbarLib.Environment.Service();

        // create list view header column sort class object ..
        private ListViewColumnSorter JobListViewSorter = new ListViewColumnSorter();
        private ListViewColumnSorter MachineListViewSorter = new ListViewColumnSorter();
        /***********************************************************************************************/
        
        // declare renbar server base class object ..
        private ServerBase Base = null;
        // declare the application close confirm message ..
        private string AppCloseMessage = string.Empty;
        // declare object access delegate ..
        private delegate void JobDataCallBack(ListViewItem Item, string DeleteItemText);
        private delegate void MachineDataCallBack(ListViewItem Item, string DeleteItemText);

        // create server taskbar notify component ..
        private NotifyIcon ServerNotify = new NotifyIcon();

        // declare initialize form flag ..
        private bool IsInitFlag = false;

        // stop state thread flag ..
        private volatile bool requeststop = false;
        #endregion

        #region Form Constructor And Destructor Procedure
        /// <summary>
        /// Primary renbar server constructor procedure.
        /// </summary>
        public Main_Form()
        {
            // initialize component standard procedure ..
            InitializeComponent();

            // set initialize flag ..
            IsInitFlag = true;

            // setting form title and icon ..
            this.Text = global::RenbarLib.Environment.AssemblyInfoClass.ProductInfo;
            this.Icon = global::RenbarServerGUI.Properties.Resources.server;
            this.WindowState = FormWindowState.Minimized;
            this.ShowInTaskbar = false;
        }

        /// <summary>
        /// Primary destructor procedure.
        /// </summary>
        ~Main_Form()
        {
            // clean up all used resource ..
            this.Dispose(true);
        }
        #endregion

        #region Form Load Event Procedure
        private void Main_Form_Load(object sender, System.EventArgs e)
        {
            // 設定工作列 settings notify component properties ..
            this.ServerNotify.BalloonTipTitle = "Renbar server executing ..";
            this.ServerNotify.BalloonTipText = "view current execute status, please click here.";
            this.ServerNotify.BalloonTipIcon = ToolTipIcon.Info;
            this.ServerNotify.Text = global::RenbarLib.Environment.AssemblyInfoClass.ProductInfo;
            this.ServerNotify.Icon = global::RenbarServerGUI.Properties.Resources.server;
            this.ServerNotify.DoubleClick += new System.EventHandler(ServerNotify_DoubleClick);

            // 設定排序 setting list view sort provider ..
            this.ListView_Job_Status.ListViewItemSorter = this.JobListViewSorter;
            this.ListView_Machine_Status.ListViewItemSorter = this.MachineListViewSorter;
            this.JobListViewSorter.Order = SortOrder.Descending;
            this.MachineListViewSorter.Order = SortOrder.Descending;

            // 設定載入進度條線程
            bool InitErr = false;
            Thread Init = new Thread(delegate()
            {
                Init_Form InitFrm = new Init_Form();

                if (InitFrm.ShowDialog() == DialogResult.OK)
                {
                    this.Base = InitFrm.Base;
                    InitFrm.Dispose();
                }
                else
                    InitErr = true;
            });

            #region  執行初始化線程 Executing Initialize Form Thread Procedure
            // start initialize form thread ..
            Init.Start();

            // 封鎖呼叫執行緒直到執行緒終止或已超過指定的時間為止
            // 如果執行緒已經結束，為 true
            // 如果millisecondsTimeout 參數指定的時間長度已經過去，而執行緒還沒有結束，則為 false
            if (Init.Join(Timeout.Infinite))//？？？？？？？？？？？？？？？？？？？？？？？？？？？？？？？？？？
            {
                // has error, close application ...
                if (!InitErr)
                {
                    // show notify component ...
                    this.ServerNotify.Visible = true;
                    // restore window state ...
                    this.WindowState = FormWindowState.Normal;
                    this.ShowInTaskbar = true;
                    // restore init flag ...
                    IsInitFlag = false;
                    // start read server data information thread ...
                    Thread
                        JobstateThread = new Thread(new ThreadStart(this.JobsData)),
                        MachinestateThread = new Thread(new ThreadStart(this.MachinesData));

                    JobstateThread.Priority = ThreadPriority.BelowNormal;
                    JobstateThread.Start();
                    MachinestateThread.Priority = ThreadPriority.BelowNormal;
                    MachinestateThread.Start();
                }
                else
                    Application.Exit();
            }
            #endregion
        }
        #endregion

        #region Read Display Data Event Procedure
        /// <summary>
        /// Read and refresh jobs data status method.
        /// </summary>
        private void JobsData()
        {
            IList<string>
                ItemList = new List<string>(),
                DeleteList = new List<string>();

            do
            {
                // read currently job information ..
                DataTable JobTable = Base.JobStatus;

                if (JobTable != null && JobTable.Rows.Count > 0)
                {
                    #region Invoke Job Data Object Delegate Procedure
                    JobDataCallBack JobData = delegate(ListViewItem Item, string DeleteItemText)
                    {
                        #region Delete Item Partial
                        if (!string.IsNullOrEmpty(DeleteItemText))
                        {
                            for (int i = 0; i < ListView_Job_Status.Items.Count; i++)
                            {
                                // processing application all events ..
                                Application.DoEvents();

                                if (this.ListView_Job_Status.Items[i].SubItems[0].Text == DeleteItemText)
                                    // remove item ..
                                    this.ListView_Job_Status.Items[i].Remove();

                                // calculate items ..
                                this.GroupBox_Jobs.Text = "Job" + string.Format(
                                    " ({0})", this.ListView_Job_Status.Items.Count.ToString());

                                // sorting ..
                                this.JobListViewSorter.SortColumn = 7;
                                this.ListView_Job_Status.Sort();

                                // refresh control ..
                                this.ListView_Job_Status.Update();
                            }

                            return;
                        }
                        #endregion

                        #region Update Item Partial
                        foreach (ListViewItem JobItem in this.ListView_Job_Status.Items)
                        {
                            // process other events ..
                            Application.DoEvents();

                            if (JobItem.Text == Item.Text)
                            {
                                if (JobItem.SubItems[1].Text == Item.SubItems[1].Text &&
                                    JobItem.SubItems[2].Text == Item.SubItems[2].Text &&
                                    JobItem.SubItems[3].Text == Item.SubItems[3].Text)
                                    return;

                                // update the item of sub items ..
                                for (int i = 1; i < JobItem.SubItems.Count; i++)
                                    JobItem.SubItems[i] = Item.SubItems[i];

                                // sorting ..
                                this.JobListViewSorter.SortColumn = 7;
                                this.ListView_Job_Status.Sort();

                                return;
                            }
                        }
                        #endregion

                        // process other events ..
                        Application.DoEvents();

                        // add new items to the listview control ..
                        this.ListView_Job_Status.Items.Add(Item);

                        // refresh control ..
                        this.ListView_Job_Status.Update();

                        // calculate items ..
                        this.GroupBox_Jobs.Text = "Job" + string.Format(
                            " ({0})", this.ListView_Job_Status.Items.Count.ToString());

                        // sorting ..
                        this.JobListViewSorter.SortColumn = 7;
                        this.ListView_Job_Status.Sort();

                        // refresh control ..
                        this.ListView_Job_Status.Update();
                    };

                    foreach (DataRow row in JobTable.Rows)
                    {
                        if (!ItemList.Contains(row["Job_Group_Id"].ToString().Trim()))
                            ItemList.Add(row["Job_Group_Id"].ToString().Trim());

                        string[] subitems = {
                            row["Job_Group_Id"].ToString().Trim(),
                            row["WaitFor"].ToString().Trim(),
                            row["Job_Priority"].ToString().Trim(),
                            row["Job_Status"].ToString().Trim(),
                            row["Start_Time"].ToString().Trim(),
                            row["Finish_Time"].ToString().Trim(),
                            row["Submit_Acct"].ToString().Trim(),
                            row["Submit_Time"].ToString().Trim()
                        };

                        try
                        {
                            // invoke job list view delegate control ..
                            this.Invoke(JobData, new object[] { new ListViewItem(subitems), null });
                        }
                        catch (InvalidOperationException)
                        {
                            // if delegate object already cleanup, exit loop ..
                            return;
                        }
                    }

                    // search deleted items ..
                    foreach (string s in ItemList)
                    {
                        if (JobTable.PrimaryKey.Length == 0)
                            break;

                        if (!JobTable.Rows.Contains(s))
                        {
                            try
                            {
                                // invoke job list view delegate control ..
                                this.Invoke(JobData, new object[] { new ListViewItem(), s });

                                // add to remove list ..
                                DeleteList.Add(s);
                            }
                            catch (InvalidOperationException)
                            {
                                // if delegate object already cleanup, exit loop ..
                                return;
                            }
                        }
                    }

                    // remove item list record ..
                    foreach (string s in DeleteList)
                        ItemList.Remove(s);

                    // clear all delete list ..
                    DeleteList.Clear();
                    #endregion
                }

                // set access state timespan ..
                Thread.Sleep(1000);
            } while (!requeststop);
        }

        /// <summary>
        /// Read and refresh machines data status method.
        /// </summary>
        private void MachinesData()
        {
            IList<string>
                ItemList = new List<string>(),
                DeleteList = new List<string>();

            do
            {
                // read currently machine information ..
                DataTable MachineTable = Base.MachineStatus;

                if (MachineTable != null && MachineTable.Rows.Count > 0)
                {
                    #region Invoke Machine Data Object Delegate Procedure
                    MachineDataCallBack MachineData = delegate(ListViewItem Item, string DeleteItemText)
                    {
                        #region Delete Item Partial
                        if (!string.IsNullOrEmpty(DeleteItemText))
                        {
                            for (int i = 0; i < ListView_Machine_Status.Items.Count; i++)
                            {
                                // processing application all events ..
                                Application.DoEvents();

                                if (this.ListView_Machine_Status.Items[i].SubItems[0].Text == DeleteItemText)
                                    // remove item ..
                                    this.ListView_Machine_Status.Items[i].Remove();

                                // calculate items ..
                                this.GroupBox_Machines.Text = "Machine" + string.Format(
                                    " ({0})", this.ListView_Machine_Status.Items.Count.ToString());

                                // sorting ..
                                this.MachineListViewSorter.SortColumn = 4;
                                this.ListView_Machine_Status.Sort();

                                // refresh control ..
                                this.ListView_Machine_Status.Update();
                            }

                            return;
                        }
                        #endregion

                        #region Update Item Partial
                        foreach (ListViewItem MachineItem in this.ListView_Machine_Status.Items)
                        {
                            // process other events ..
                            Application.DoEvents();

                            if (MachineItem.Text == Item.Text)
                            {
                                if (MachineItem.SubItems[3].Text == Item.SubItems[3].Text &&
                                    MachineItem.SubItems[4].Text == Item.SubItems[4].Text &&
                                    MachineItem.SubItems[5].Text == Item.SubItems[5].Text &&
                                    MachineItem.SubItems[6].Text == Item.SubItems[6].Text &&
                                    MachineItem.SubItems[7].Text == Item.SubItems[7].Text &&
                                    MachineItem.SubItems[8].Text == Item.SubItems[8].Text &&
                                    MachineItem.SubItems[9].Text == Item.SubItems[9].Text)
                                    return;

                                // update the item of sub items ..
                                for (int i = 1; i < MachineItem.SubItems.Count; i++)
                                    MachineItem.SubItems[i] = Item.SubItems[i];

                                // sorting ..
                                this.MachineListViewSorter.SortColumn = 4;
                                this.ListView_Machine_Status.Sort();
                                return;
                            }
                        }
                        #endregion

                        // process other events ..
                        Application.DoEvents();

                        // refresh control ..
                        this.ListView_Machine_Status.Update();

                        // add new items to the listview control ..
                        this.ListView_Machine_Status.Items.Add(Item);

                        // calculate items ..
                        this.GroupBox_Machines.Text = "Machine" + string.Format(
                            " ({0})", this.ListView_Machine_Status.Items.Count.ToString());

                        // sorting ..
                        this.MachineListViewSorter.SortColumn = 4;
                        this.ListView_Machine_Status.Sort();

                        // refresh control ..
                        this.ListView_Machine_Status.Update();
                    };

                    foreach (DataRow row in MachineTable.Rows)
                    {
                        if (!ItemList.Contains(row["Machine_Id"].ToString().Trim()))
                            ItemList.Add(row["Machine_Id"].ToString().Trim());

                        string[] subitems = {
                            row["Machine_Id"].ToString().Trim(),
                            row["HostName"].ToString().Trim(),
                            row["Ip"].ToString().Trim(),
                            row["IsEnable"].ToString().Trim(),
                            row["Last_Online_Time"].ToString().Trim(),
                            row["Machine_Status"].ToString().Trim(),
                            row["Machine_Priority"].ToString().Trim(),
                            row["TCore"].ToString().Trim(),
                            row["UCore"].ToString().Trim(),
                            row["Note"].ToString().Trim(),
                        };

                        try
                        {
                            // invoke machine list view delegate control ..
                            this.Invoke(MachineData, new object[] { new ListViewItem(subitems), null });
                        }
                        catch (InvalidOperationException)
                        {
                            // if delegate object already cleanup, exit loop ..
                            return;
                        }
                    }

                    // search deleted items ..
                    foreach (string s in ItemList)
                    {
                        if (MachineTable.PrimaryKey.Length == 0)
                            break;

                        if (!MachineTable.Rows.Contains(s))
                        {
                            try
                            {
                                // invoke machine list view delegate control ..
                                this.Invoke(MachineData, new object[] { new ListViewItem(), s });

                                // add to remove list ..
                                DeleteList.Add(s);
                            }
                            catch (InvalidOperationException)
                            {
                                // if delegate object already cleanup, exit loop ..
                                return;
                            }
                        }
                    }

                    // remove item list record ..
                    foreach (string s in DeleteList)
                        // remove data record ..
                        ItemList.Remove(s);

                    // clear all delete list ..
                    DeleteList.Clear();
                    #endregion
                }
                // set access state timespan ..
                Thread.Sleep(1000);
            } while (!requeststop);
        }
        #endregion

        #region Server Notify Component Double Click Event Procedure
        private void ServerNotify_DoubleClick(object sender, System.EventArgs e)
        {
            if (!this.IsInitFlag && this.WindowState == FormWindowState.Minimized)
            {
                // show the application ..
                this.Show();

                // restore default window state ..
                this.WindowState = FormWindowState.Normal;
            }
        }
        #endregion

        #region Form Resize Event Procedure
        private void Main_Form_Resize(object sender, System.EventArgs e)
        {
            if (!this.IsInitFlag && this.WindowState == FormWindowState.Minimized)
            {
                this.Hide();
                this.ServerNotify.ShowBalloonTip(30);
            }
        }
        #endregion

        #region Form Close Event Procedure
        private void Main_Form_FormClosing(object sender, FormClosingEventArgs e)
        {
            AppCloseMessage = "Exit?";
            // 確認對話方框 show confirm message dialog ..
            DialogResult result = MessageBox.Show(this, AppCloseMessage, this.Text, MessageBoxButtons.OKCancel, MessageBoxIcon.Question);

            if (result == DialogResult.Cancel)
            {
                e.Cancel = true;
            }
            else
            {
                // stop running state thread ..
                requeststop = true;

                // clear server taskbar notify component ..
                this.ServerNotify.Dispose();
            }
        }

        /// <summary>
        /// Clean up base resource and exit.
        /// </summary>
        private void Main_Form_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (this.Base != null)
            {
                // clean up base class object resource ..
                this.Base.Dispose();
            }
        }

        #endregion
    }
}