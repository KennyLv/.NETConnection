#region Using NameSpace
using System;
using System.Data;
using System.Windows.Forms;
using System.Threading;
using System.Text;

// import renbar class library namespace ..
using RenbarLib.Environment;
using RenbarLib.Environment.Forms.Controls;
using RenbarLib.Environment.Forms.Controls.ListView.Sort;
using RenbarLib.Network;
using RenbarLib.Network.Protocol;
#endregion

namespace RenderServerGUI
{
    public partial class Main_Form : Form
    {
        #region Declare Global Variable Section定義全局變量Section
        /************************ reference renbar class library section *******************************/
        // create list view header column sort class ..//創建list view頭列排序類
        private global::RenbarLib.Environment.Forms.Controls.ListView.Sort.ListViewColumnSorter ListViewSorter
            = new global::RenbarLib.Environment.Forms.Controls.ListView.Sort.ListViewColumnSorter();
        /***********************************************************************************************/

        // declare render base class ..//定義render base類
       
        private RenderBase Base = null;

        // declare the application close confirm message ..
        private string AppCloseMessage = string.Empty;
        // declare log base class ..//定義日志base類
        private RenderEvents LogMsg = null;

        // declare object access delegate ..//定義對象接口委托
        private delegate void RenderDataCallBack(ListViewItem Item, string DeleteItemText);

        // stop state thread flag ..//停止狀態線程標志
        private volatile bool requeststop = false;

        // declare connected flag ..
        //private Backup bk = new Backup();
        // declare connected flag ..
        //private bool IsConnected = false;
        #endregion

        #region Form Constructor And Destructor Procedure表單構造器和Destructor過程
        /// <summary>
        /// Primary renbar server constructor procedure.//主Renbar服務器構造器過程
        /// </summary>
        public Main_Form()
        {
            // initialize component standard procedure ..
            InitializeComponent();

            // setting form title and icon ..
            this.Text = global::RenbarLib.Environment.AssemblyInfoClass.ProductInfo;
            this.Icon = global::RenderServerGUI.Properties.Resources.render;
        }

        /// <summary>
        /// Primary destructor procedure.//主destructor過程
        /// </summary>
        ~Main_Form()
        {
            // clean all resource ..//清空所有資源
            this.Dispose(true);
        }
        #endregion

        #region Form Load Event Procedure表單加載事件過程
        private void MainForm_Load(object sender, EventArgs e)
        {

            // create render instance ..//創建render instance
            this.Base = new RenderBase();//先去執行構造函數

            // create render log instance ..//創建render日志instance
            this.LogMsg = new RenderEvents();

            // append to logs object ..//追加到日志對象
            RenderEvents.AppendLog(string.Format("{0}", "initialize render base service has successful."));

            // setting list view sort provider ..//設置list view排序provider
            this.ListView_Render_Status.ListViewItemSorter = ListViewSorter;
            this.ListViewSorter.Order = SortOrder.Descending;

            // start read server data information thread ..//開始讀服務器數據信息線程
            Thread StateThread = new Thread(new ThreadStart(this.ReadData));
            StateThread.Priority = ThreadPriority.BelowNormal;
            StateThread.Start();

            // append to logs object ..//追加到日志對象
            RenderEvents.AppendLog(string.Format("{0}", "start data object delegate thread."));
            
        }
        #endregion

        #region Form Close Event Procedure表單關閉事件過程
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
                // stop running state thread ..//停止運行狀態線程
                requeststop = true;

                // change status flag ..//改變狀態標志
                this.Base.IsOffLine = true;

                // start machine registry ..//開始機器注冊
                this.Base.RegistryMachine(false);

                // clean up base class object resource ..//清空base類對象資源
                this.Base.Dispose();

                // append to logs object ..//追加到日志對象
                RenderEvents.AppendLog(string.Format("{0}", "close renbar for render server application."));

                // save current all log message ..//保存黨前所有日志信息
                RenderEvents.SaveLog();
            }
        }
        #endregion

        #region Read Display Data Event Procedure讀顯示數據事件過程
        private void ReadData()
        {
            global::System.Collections.Generic.IList<string>
                RenderItemList = new global::System.Collections.Generic.List<string>(),
                RenderDeleteList = new global::System.Collections.Generic.List<string>();

            do
            {
                while (this.Base.HasChange)//如果得到或者設置數據狀態改變
                {
                    // read current job information ..//讀黨前的工作信息
                    DataTable DataStatus = this.Base.GetQueueStatus;

                    if (DataStatus != null)
                    {
                        #region Invoke Render Status Data Object Delegate Procedure調用Render狀態數據對象代表Procedure
                        RenderDataCallBack RenderData= delegate(ListViewItem Item, string DeleteItemText)
                        {
                            if (!string.IsNullOrEmpty(DeleteItemText))
                            {
                                for (int i = 0; i < ListView_Render_Status.Items.Count; i++)
                                {
                                    // processing application all events ..//進程應用程序所有事件
                                    Application.DoEvents();

                                    if (this.ListView_Render_Status.Items[i].SubItems[0].Text == DeleteItemText)
                                        // remove item ..//移除項
                                        this.ListView_Render_Status.Items[i].Remove();

                                    // refresh control ..//刷新控制
                                    this.ListView_Render_Status.Update();
                                }

                                return;
                            }

                            foreach (ListViewItem RenderItem in this.ListView_Render_Status.Items)
                            {
                                // processing application all events ..//進程應用程序所有事件
                                Application.DoEvents();

                                if (RenderItem.Text == Item.Text)
                                {
                                    // update the item of sub items ..//修改子項的項
                                    for (int i = 0; i < RenderItem.SubItems.Count; i++)
                                        RenderItem.SubItems[i] = Item.SubItems[i];

                                    // refresh control ..//刷新控制
                                    this.ListView_Render_Status.Update();

                                    return;
                                }
                            }

                            // processing application all events ..//processing應用程序所有事件
                            Application.DoEvents();

                            // add new items to the listview control ..//添加新項到listview控制
                            this.ListView_Render_Status.Items.Add(Item);

                            // sorting ..//排序
                            this.ListViewSorter.SortColumn = 0;
                            this.ListView_Render_Status.Sort();

                            // refresh control ..//刷新控制
                            this.ListView_Render_Status.Update();

                            // append to logs object ..//追加到日志對象
                            RenderEvents.AppendLog(string.Format("{0}", "refresh listview render data."));
                        };

                        foreach (global::System.Data.DataRow row in DataStatus.Rows)
                        {
                            if (!RenderItemList.Contains(row["Job_Id"].ToString().Trim()))
                                RenderItemList.Add(row["Job_Id"].ToString().Trim());

                            string[] subitems = {
                                row["Job_Id"].ToString().Trim(),
                                row["Job_Group_Id"].ToString().Trim(),
                                row["Proc_Id"].ToString().Trim(),
                                row["Proc_Type"].ToString().Trim(),
                                row["Command"].ToString().Trim(),
                                row["Args"].ToString().Trim(),
                                row["Status"].ToString().Trim(),
                                row["Start_Time"].ToString().Trim(),
                                row["Finish_Time"].ToString().Trim()
                            };

                            try
                            {
                                // invoke job list view delegate control ..//調用工作列view代表控制
                                this.Invoke(RenderData, new object[] { new ListViewItem(subitems), null });//調用RenderData
                            }
                            catch (InvalidOperationException)
                            {
                                // if delegate object already clean, exit loop ..//如果代表對象已經清空，退出loop
                                break;
                            }
                        }

                        // search deleted items ..//查看刪除項
                        foreach (string s in RenderItemList)
                        {
                            if (DataStatus.PrimaryKey.Length == 0)
                                break;

                            if (!DataStatus.Rows.Contains(s))
                            {
                                try
                                {
                                    // invoke job list view delegate control ..//調用工作列視圖代表控制
                                    this.Invoke(RenderData, new object[] { new ListViewItem(), s });

                                    // add to remove list ..//添加到移除列
                                    RenderDeleteList.Add(s);
                                }
                                catch (InvalidOperationException)
                                {
                                    // if delegate object already clean, exit loop ..//如果代表對象已經清空，退出loop
                                    break;
                                }
                            }
                        }

                        foreach (string s in RenderDeleteList)
                            // remove data record ..//移除數據記錄
                            RenderItemList.Remove(s);

                        // clear all delete list ..//清空所有刪除列
                        RenderDeleteList.Clear();
                        #endregion
                    }

                    // reset change flag ..//重置改變標志
                    this.Base.HasChange = false;
                }

                // set access state timespan ..//設置接口狀態timespan
                Thread.Sleep(500);
            } while (!requeststop);
        }
        #endregion

        #region View Log Window LinkClicked Event Procedure查看日志window LinkClicked事件過程
        private void LinkLabel_ViewLog_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // show log form window ..//顯示日志表單window
            this.LogMsg.OpenLogWindow();
            //new Thread(new ParameterizedThreadStart(Base.RegistryMachine1)).Start(true);
        }
        #endregion

        #region Maintenance Checked Changed Event Procedure維護查看改變事件過程
        private void CheckBox_Maintenance_CheckedChanged(object sender, EventArgs e)
        {
            // refresh is maintenance status flag ..//刷新維護狀態標志
            this.Base.IsMaintenance = this.CheckBox_Maintenance.Checked;

            // start machine registry thread ..//開始機器注冊線程
            new Thread(new ParameterizedThreadStart(Base.RegistryMachine1)).Start(false);
        }
        #endregion

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Net_Form netForm = new Net_Form();
            
            if (netForm.ShowDialog() == DialogResult.OK)
            {
                //new Thread(new ParameterizedThreadStart(Base.RegistryMachine)).Start(true);
                netForm.Dispose();
            }
        }
    }
}