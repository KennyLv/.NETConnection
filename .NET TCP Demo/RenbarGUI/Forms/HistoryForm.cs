using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;


// import renbar class library namespace ..
using RenbarLib.Environment.Forms.Customizations.Service;
using RenbarLib.Network.Protocol;
using System.Threading;
using RenbarLib.Environment.Forms.Controls.ListView.Sort;

namespace RenbarGUI.Forms
{
    public partial class HistoryForm : Form
    {
        #region Declare Global Variable Section

        // declare environment setting structure object ..
        private Settings EnvSetting;

        // create renbar client communication class object ..
        private Communication EnvComm = new Communication();

        // declare renbar client environment class object ..
        private Customization EnvCust = null;

        //
        private ListViewColumnSorter HistoryListViewSorter = new ListViewColumnSorter();

        // stop state thread flag ..
        private volatile bool requeststop = false;

        // create data object instance ..
        DataTable HistoryInfoTable = new DataTable("JobHistory");

        //
        private string DataDeletedError = string.Empty;

        #endregion

        #region Initialize History Form Event Procedure
        // Constructor ..
        public HistoryForm(ref Customization Cust, ref Communication Comm, Settings Setting)
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
        // Destructor ..
        ~HistoryForm()
        {
            this.Dispose();
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

                    this.Text = this.EnvCust.GetLocalization(this.Name, this.Name, Customization.Language.En_Us);

                    // List View 
                    this.ListView_History.Columns.Clear();
#if Debug
                    this.ListView_Process.Columns.Add(this.EnvCust.GetLocalization(this.Name, this.ListView_Process.Name + "_Job_Group_Id", Customization.Language.En_Us), 0);
#else
                    this.ListView_History.Columns.Add(this.EnvCust.GetLocalization(this.Name, this.ListView_History.Name + "_Job_Group_Id", Customization.Language.En_Us), 0);
#endif
                    this.ListView_History.Columns.Add(this.EnvCust.GetLocalization(this.Name, this.ListView_History.Name + "_Name", Customization.Language.En_Us), 200);
                    //this.ListView_History.Columns.Add(this.EnvCust.GetLocalization(this.Name, this.ListView_History.Name + "_Submit_Acct", Customization.Language.En_Us), 100);
                    //this.ListView_History.Columns.Add(this.EnvCust.GetLocalization(this.Name, this.ListView_History.Name + "_Submit_Time", Customization.Language.En_Us), 150);
                    this.ListView_History.Columns.Add(this.EnvCust.GetLocalization(this.Name, this.ListView_History.Name + "_Frames", Customization.Language.En_Us), 100);

                    // Button
                    this.Button_Refresh.Text = this.EnvCust.GetLocalization(this.Name, this.Button_Refresh.Name, Customization.Language.En_Us);
                    this.Button_Close.Text = this.EnvCust.GetLocalization(this.Name, this.Button_Close.Name, Customization.Language.En_Us);
                    // Lables
                    this.label_MaxRecords.Text = this.EnvCust.GetLocalization(this.Name, this.label_MaxRecords.Name, Customization.Language.En_Us);

                    this.DataDeletedError = this.EnvCust.GetLocalization(this.Name, "Data_Deleted_Err", Customization.Language.En_Us);
                    // ToolStripMenuItem
                    //this.MenuItem_ViewJobDetail.Text = this.EnvCust.GetLocalization(this.Name, this.MenuItem_ViewJobDetail.Name, Customization.Language.En_Us);
                    break;
                #endregion

                #region Traditional Chinese
                case Customization.Language.Zh_Tw:
                    this.Text = this.EnvCust.GetLocalization(this.Name, this.Name, Customization.Language.Zh_Tw);
                    // List View 
                    this.ListView_History.Columns.Clear();
#if Debug
                    this.ListView_Process.Columns.Add(this.EnvCust.GetLocalization(this.Name, this.ListView_Process.Name + "_Job_Group_Id", Customization.Language.Zh_Tw), 0);
#else
                    this.ListView_History.Columns.Add(this.EnvCust.GetLocalization(this.Name, this.ListView_History.Name + "_Job_Group_Id", Customization.Language.Zh_Tw), 0);
#endif
                    this.ListView_History.Columns.Add(this.EnvCust.GetLocalization(this.Name, this.ListView_History.Name + "_Name", Customization.Language.Zh_Tw), 200);
                    this.ListView_History.Columns.Add(this.EnvCust.GetLocalization(this.Name, this.ListView_History.Name + "_Frames", Customization.Language.Zh_Tw), 100);

                    // Button
                    this.Button_Refresh.Text = this.EnvCust.GetLocalization(this.Name, this.Button_Refresh.Name, Customization.Language.Zh_Tw);
                    this.Button_Close.Text = this.EnvCust.GetLocalization(this.Name, this.Button_Close.Name, Customization.Language.Zh_Tw);
                    // Lables
                    this.label_MaxRecords.Text = this.EnvCust.GetLocalization(this.Name, this.label_MaxRecords.Name, Customization.Language.Zh_Tw);

                    this.DataDeletedError = this.EnvCust.GetLocalization(this.Name, "Data_Deleted_Err", Customization.Language.Zh_Tw);

                    break;
                #endregion
            }
        }

        #endregion

        private void History_Form_Load(object sender, EventArgs e)
        {
            // lock re-submit button ..
            this.Button_Refresh.Enabled = false;
            //
            this.numericUpDown_RecordNum.Value = 100;
            //
            InitListViewHeader();

            try
            {
                // get the history info 
                GetHistoryInfo();
            }
            catch
            {

            }
            finally
            {
                this.Button_Refresh.Enabled = true;
            }
        }


        #region 歷史信息排序 ListView Sorter Setting
        /// <summary>
        /// setting the ListView Sorter
        /// </summary>
        private void InitListViewHeader()
        {
            this.ListView_History.AutoArrange = true;
            this.ListView_History.HeaderStyle = ColumnHeaderStyle.Clickable;
            this.ListView_History.View = View.Details;
            this.ListView_History.ListViewItemSorter = this.HistoryListViewSorter;
            this.ListView_History.Sorting = SortOrder.Ascending;
            this.ListView_History.Scrollable = true;
            this.ListView_History.FullRowSelect = true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListView_History_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            // determine if clicked column is already the column that is being sorted ..
            if (e.Column == this.HistoryListViewSorter.SortColumn)
            {
                // reverse the current sort direction for this column ..
                if (this.HistoryListViewSorter.Order == SortOrder.Ascending)
                    this.HistoryListViewSorter.Order = SortOrder.Descending;
                else
                    this.HistoryListViewSorter.Order = SortOrder.Ascending;
            }
            else
            {
                // set the column number that is to be sorted; default to ascending ..
                this.HistoryListViewSorter.SortColumn = e.Column;
                this.HistoryListViewSorter.Order = SortOrder.Ascending;
            }

            // perform the sort with these new sort options ..
            this.ListView_History.Sort();
        }

        #endregion

        #region 獲取歷史記錄Getting Infor Process

        private void GetHistoryInfo()
        {
            #region 組合數據
            // declare remote server response object ..
            KeyValuePair<string, object> responseObject;

            IDictionary<string, object> Item = new Dictionary<string, object>
            {
                { "Num", this.numericUpDown_RecordNum.Value }
            };
            #endregion

            #region 發送請求？？？？？應開啟另一個線程，防止更新的時無法窗體假死而無法強制終止？？？？
            do
            {
                // confirm current request status ...
                if (EnvComm.CanRequest && !this.requeststop)
                {
                    // package sent data ...
                    IList<object> packaged = EnvComm.Package(Client2Server.CommunicationType.JOBHISTORYRECORD, Item);

                    // wait for result ..
                    responseObject = EnvComm.Request(packaged);
                    break;
                }
                Thread.Sleep(500);
            } while (true);
            #endregion

            #region 解析並綁定數據

            if (responseObject.Key.Substring(0, 1).Equals("+"))
            {
                if (responseObject.Value != null)
                {
                    HistoryInfoTable = (DataTable)responseObject.Value;//？？？
                }

                //顯示
                this.ListView_History.Items.Clear();
                for (int i = 0; i < HistoryInfoTable.Rows.Count; i++)
                {
                    string[] subitems = {
                                        HistoryInfoTable.Rows[i]["Job_Group_Id"].ToString().Trim(),
                                        HistoryInfoTable.Rows[i]["Name"].ToString().Trim(),
                                        HistoryInfoTable.Rows[i]["Frames"].ToString().Trim(),
                                                                };

                    ListViewItem LItem = new ListViewItem(subitems);
                    this.ListView_History.Items.Add(LItem);

                }
            }
            #endregion
        }

        #endregion

        #region 窗體按鈕事件 Button Event
        //關閉窗體
        private void Button_Close_Click(object sender, EventArgs e)
        {
            // sent stop loop request ..
            this.requeststop = true;
            this.Close();
            this.Dispose();
        }

        //重新獲取數據
        private void Button_Refresh_Click(object sender, EventArgs e)
        {
            this.ListView_History.ForeColor = Color.Gray;
            this.numericUpDown_RecordNum.Enabled = false;
            this.Button_Refresh.Enabled = false;

            try
            {
                // get the history info 
                GetHistoryInfo();
            }
            catch
            {

            }
            finally
            {
                this.ListView_History.ForeColor = Color.Black;
                this.numericUpDown_RecordNum.Enabled = true;
                this.Button_Refresh.Enabled = true;

                this.Update();
            }
        }

        #endregion

        #region 取得任務詳細信息
        private void ListView_History_DoubleClick(object sender, EventArgs e)
        {
            //？？？？？？？？已被清除該怎么辦？？？？？？？？？？？

            if (this.ListView_History.Items.Count > 0 && this.ListView_History.SelectedItems.Count > 0)
            {
               IDictionary<string,object> Data=this.EnvCust.GetJobInfo(this.ListView_History.SelectedItems[0].SubItems[0].Text.ToString(), ref this.EnvComm);

               //JobAttrs = DataRevise(responData_Attrs);//名稱對應
               if (Data != null)
               {
                   IDictionary<string, object> HistoryData = this.EnvCust.DataRevise(Data);

                   #region 綁定數據到JobForm
                   Job_Form jobForm = new Job_Form(ref this.EnvCust, ref this.EnvComm, this.EnvSetting)
                   {
                       Pool = this.EnvCust.DropDown.Pool,
                       Pool2 = this.EnvCust.DropDown.Pool2,
                       WaitFor = this.EnvCust.DropDown.Waitfor,
                       IsExtern = true,
                       IsUpdate = false,
                       IsViewHistory = true
                   };
                   #endregion

                   jobForm.ExternItems = HistoryData;
                   // show job form ..
                   if (jobForm.ShowDialog(this) == DialogResult.OK)
                       jobForm.Dispose();
               }
               else
               {
                   MessageBox.Show(DataDeletedError);
               }
            }
            else
            {
                return;
            }
        }
        #endregion
    }
}
