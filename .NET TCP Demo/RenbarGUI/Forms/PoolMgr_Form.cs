using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;

// import renbar class library namespace ..
using RenbarLib.Environment;
using RenbarLib.Environment.Forms.Customizations.Service;
using RenbarLib.Network.Protocol;


namespace RenbarGUI.Forms
{
    public partial class PoolMgr_Form : Form
    {

        #region Declare Global Variable Section
        /************************ reference renbar class library section *******************************/
        // declare renbar client environment class object ..
        private Customization EnvCust = null;

        // declare renbar client communication class object ..
        private Communication EnvComm = null;
        /***********************************************************************************************/

        // declare machine data table ..
        private DataTable machines = null;

        // declare pool data table ..
        private DataTable pools = null;

        // declare customize message ..
        private string __AddPool_Repeat_Error = string.Empty;
        private string __AddPool_Complete = string.Empty;
        private string __AddPool_Error = string.Empty;
        private string __DeletePool_Complete = string.Empty;
        private string __DeletePool_Error = string.Empty;
        private string __UpdatePool_Complete = string.Empty;
        private string __UpdatePool_Error = string.Empty;
        private string __Pool_Member_Error = string.Empty;
        private string __DeletePool_Warning = string.Empty;
        #endregion

        #region Pool操作方式枚舉（增刪改） Pool Action Enumeration
        /// <summary>
        /// Pool action enumeration.
        /// </summary>
        enum PoolAction
        {
            /// <summary>
            /// Add pool flag.
            /// </summary>
            Add,
            /// <summary>
            /// Update pool flag.
            /// </summary>
            Update,
            /// <summary>
            /// Delete pool flag.
            /// </summary>
            Delete
        }
        #endregion

        #region 構造窗體 Form Constructor Procedure
        /// <summary>
        /// 構造函數 Primary renbar client pool manager form constructor procedure.
        /// </summary>
        /// <param name="Cust">reference customize class.</param>
        /// <param name="Comm">refrernce communication class.</param>
        /// <param name="Setting">current environment setting information.</param>
        public PoolMgr_Form(ref Customization Cust, ref Communication Comm, Settings Setting)
        {
            // initialize form control component ..
            InitializeComponent();

            // assign customize class reference ..
            this.EnvCust = Cust;
            // assign communization class refrernce ..
            this.EnvComm = Comm;

            try
            {
                // get pool, machine data ..
                this.GetPools();
                this.GetMachines();
            }
            catch
            { 

            }

            //如果沒有機器信息，則控制各控件狀態為不可用
            if (this.ListBox_Machine.Items.Count == 0)
            {
                this.ComboBox_Pool_Name.Enabled = false;
                this.CheckBox_Sharable.Enabled = false;
                this.Button_Add_Member.Enabled = false;
                this.Button_Del_Member.Enabled = false;
                this.Button_Add_Pool.Enabled = false;
                this.Button_Delete_Pool.Enabled = false;
                this.Button_Update_Pool.Enabled = false;
            }

            // initializing user interface behavior ..
            Language(Setting.Lang);

        }

        /// <summary>
        /// 設定語言 Setting language resource.
        /// </summary>
        private void Language(Customization.Language Lang)
        {
            //警示信息
            string[] __warning = null;

            switch (Lang)
            {
                #region English (United-State)
                case Customization.Language.En_Us:
                    // Pool Management Form ..
                    this.Text = this.EnvCust.GetLocalization(this.Name, this.Name, Customization.Language.En_Us);

                    // Pool Groups GroupBox ..
                    this.GroupBox_Pool_Groups.Text = this.EnvCust.GetLocalization(this.Name, this.GroupBox_Pool_Groups.Name, Customization.Language.En_Us);

                    // Machine List Label ..
                    this.Label_Machine_List.Text = this.EnvCust.GetLocalization(this.Name, this.Label_Machine_List.Name, Customization.Language.En_Us);

                    // Pool Name Label ..
                    this.Label_Pool_Name.Text = this.EnvCust.GetLocalization(this.Name, this.Label_Pool_Name.Name, Customization.Language.En_Us);

                    // Member List Label ..
                    this.Label_Member_List.Text = this.EnvCust.GetLocalization(this.Name, this.Label_Member_List.Name, Customization.Language.En_Us);

                    // Sharable CheckBox ..
                    this.CheckBox_Sharable.Text = this.EnvCust.GetLocalization(this.Name, this.CheckBox_Sharable.Name, Customization.Language.En_Us);

                    // Add Member Button ..
                    this.Button_Add_Member.Text = this.EnvCust.GetLocalization(this.Name, this.Button_Add_Member.Name, Customization.Language.En_Us);

                    // Del Member Button ..
                    this.Button_Del_Member.Text = this.EnvCust.GetLocalization(this.Name, this.Button_Del_Member.Name, Customization.Language.En_Us);

                    // Pool Control GroupBox ..
                    this.GroupBox_Pool_Controls.Text = this.EnvCust.GetLocalization(this.Name, this.GroupBox_Pool_Controls.Name, Customization.Language.En_Us);

                    // Add Pool Button ..
                    this.Button_Add_Pool.Text = this.EnvCust.GetLocalization(this.Name, this.Button_Add_Pool.Name, Customization.Language.En_Us);

                    // Delete Pool Button ..
                    this.Button_Delete_Pool.Text = this.EnvCust.GetLocalization(this.Name, this.Button_Delete_Pool.Name, Customization.Language.En_Us);

                    // Update Pool Button ..
                    this.Button_Update_Pool.Text = this.EnvCust.GetLocalization(this.Name, this.Button_Update_Pool.Name, Customization.Language.En_Us);

                    // Get Pool Action Message ..
                    this.__AddPool_Repeat_Error = this.EnvCust.GetLocalization(this.Name, this.ComboBox_Pool_Name.Name + "_Repate_Err", Customization.Language.En_Us);
                    this.__AddPool_Complete = this.EnvCust.GetLocalization(this.Name, this.Button_Add_Pool.Name + "_Complete", Customization.Language.En_Us);
                    this.__AddPool_Error = this.EnvCust.GetLocalization(this.Name, this.Button_Add_Pool.Name + "_Fail", Customization.Language.En_Us);
                    this.__DeletePool_Complete = this.EnvCust.GetLocalization(this.Name, this.Button_Delete_Pool.Name + "_Complete", Customization.Language.En_Us);
                    this.__DeletePool_Error = this.EnvCust.GetLocalization(this.Name, this.Button_Delete_Pool.Name + "_Fail", Customization.Language.En_Us);
                    this.__UpdatePool_Complete = this.EnvCust.GetLocalization(this.Name, this.Button_Update_Pool.Name + "_Complete", Customization.Language.En_Us);
                    this.__UpdatePool_Error = this.EnvCust.GetLocalization(this.Name, this.Button_Update_Pool.Name + "_Fail", Customization.Language.En_Us);
                    this.__Pool_Member_Error = this.EnvCust.GetLocalization(this.Name, "AddChange_Member_Fail", Customization.Language.En_Us);

                    // process delete pool warning string ..
                    __warning = this.EnvCust.GetLocalization(this.Name, this.Button_Delete_Pool.Name + "_Confirm", Customization.Language.En_Us).Split('-');
                    this.__DeletePool_Warning = string.Format("{0}\r\n\n{1}", __warning[0], __warning[1]);
                    break;
                #endregion

                #region Traditional Chinese
                case Customization.Language.Zh_Tw:
                    // Pool Management Form ..
                    this.Text = this.EnvCust.GetLocalization(this.Name, this.Name, Customization.Language.Zh_Tw);

                    // Pool Groups GroupBox ..
                    this.GroupBox_Pool_Groups.Text = this.EnvCust.GetLocalization(this.Name, this.GroupBox_Pool_Groups.Name, Customization.Language.Zh_Tw);

                    // Machine List Label ..
                    this.Label_Machine_List.Text = this.EnvCust.GetLocalization(this.Name, this.Label_Machine_List.Name, Customization.Language.Zh_Tw);

                    // Pool Name Label ..
                    this.Label_Pool_Name.Text = this.EnvCust.GetLocalization(this.Name, this.Label_Pool_Name.Name, Customization.Language.Zh_Tw);

                    // Member List Label ..
                    this.Label_Member_List.Text = this.EnvCust.GetLocalization(this.Name, this.Label_Member_List.Name, Customization.Language.Zh_Tw);

                    // Sharable CheckBox ..
                    this.CheckBox_Sharable.Text = this.EnvCust.GetLocalization(this.Name, this.CheckBox_Sharable.Name, Customization.Language.Zh_Tw);

                    // Add Member Button ..
                    this.Button_Add_Member.Text = this.EnvCust.GetLocalization(this.Name, this.Button_Add_Member.Name, Customization.Language.Zh_Tw);

                    // Del Member Button ..
                    this.Button_Del_Member.Text = this.EnvCust.GetLocalization(this.Name, this.Button_Del_Member.Name, Customization.Language.Zh_Tw);

                    // Pool Control GroupBox ..
                    this.GroupBox_Pool_Controls.Text = this.EnvCust.GetLocalization(this.Name, this.GroupBox_Pool_Controls.Name, Customization.Language.Zh_Tw);

                    // Add Pool Button ..
                    this.Button_Add_Pool.Text = this.EnvCust.GetLocalization(this.Name, this.Button_Add_Pool.Name, Customization.Language.Zh_Tw);

                    // Delete Pool Button ..
                    this.Button_Delete_Pool.Text = this.EnvCust.GetLocalization(this.Name, this.Button_Delete_Pool.Name, Customization.Language.Zh_Tw);

                    // Update Pool Button ..
                    this.Button_Update_Pool.Text = this.EnvCust.GetLocalization(this.Name, this.Button_Update_Pool.Name, Customization.Language.Zh_Tw);

                    // Get Pool Action Message ..
                    this.__AddPool_Repeat_Error = this.EnvCust.GetLocalization(this.Name, this.ComboBox_Pool_Name.Name + "_Repate_Err", Customization.Language.Zh_Tw);
                    this.__AddPool_Complete = this.EnvCust.GetLocalization(this.Name, this.Button_Add_Pool.Name + "_Complete", Customization.Language.Zh_Tw);
                    this.__AddPool_Error = this.EnvCust.GetLocalization(this.Name, this.Button_Add_Pool.Name + "_Fail", Customization.Language.Zh_Tw);
                    this.__DeletePool_Complete = this.EnvCust.GetLocalization(this.Name, this.Button_Delete_Pool.Name + "_Complete", Customization.Language.Zh_Tw);
                    this.__DeletePool_Error = this.EnvCust.GetLocalization(this.Name, this.Button_Delete_Pool.Name + "_Fail", Customization.Language.Zh_Tw);
                    this.__UpdatePool_Complete = this.EnvCust.GetLocalization(this.Name, this.Button_Update_Pool.Name + "_Complete", Customization.Language.Zh_Tw);
                    this.__UpdatePool_Error = this.EnvCust.GetLocalization(this.Name, this.Button_Update_Pool.Name + "_Fail", Customization.Language.Zh_Tw);
                    this.__Pool_Member_Error = this.EnvCust.GetLocalization(this.Name, "AddChange_Member_Fail", Customization.Language.Zh_Tw);

                    // process delete pool warning string ..
                    __warning = this.EnvCust.GetLocalization(this.Name, this.Button_Delete_Pool.Name + "_Confirm", Customization.Language.Zh_Tw).Split('-');
                    this.__DeletePool_Warning = string.Format("{0}\r\n\n{1}", __warning[0], __warning[1]);
                    break;
                #endregion
            }
        }
        #endregion

        #region 關閉窗體 Form Closing Event Procedure
        private void PoolMgr_Form_FormClosing(object sender, FormClosingEventArgs e)
        {
            // 清空信息 clear resource ..
            if (this.pools != null)
                this.pools.Clear();

            if (this.machines != null)
                this.machines.Clear();

            // return correct dialog result ..
            this.DialogResult = DialogResult.OK;
        }
        #endregion

        #region 獲得所有Machine及Pool信息 Get Machine And Pool Item List Procedure
        /// <summary>
        /// 獲得所有機器信息 Get all machines.
        /// </summary>
        private void GetMachines()
        {
            // 獲取機器信息 create all machines datatable ..
            this.machines = this.EnvCust.Machines(ref this.EnvComm);

            if (this.machines.Rows.Count > 0)
            {
                // 添加到ListBox add items ..
                foreach (DataRow row in this.machines.Rows)
                {
                    if (row["IsRender"].ToString() == "1")
                    {
                        this.ListBox_Machine.Items.Add(new ItemPair<string, string>(row["Machine_Id"].ToString(), row["Name"].ToString()));
                    }
                }
                //資料行 'Machine_Id' 不屬於資料表 Pool。？？？？？？？？？？？？？？？？？？？？？？？？？？？？？？

                this.ListBox_Machine.Sorted = true;
            }
        }

        /// <summary>
        /// 獲取所有Pool信息 Get all pools.
        /// </summary>
        private void GetPools()
        {
            this.pools = this.EnvCust.AllPools(ref this.EnvComm);
            if (this.pools.Rows.Count > 0)
            {
                if (this.ComboBox_Pool_Name.Items.Count > 0)
                    this.ComboBox_Pool_Name.Items.Clear();

                // add items ..
                foreach (DataRow row in this.pools.Rows)
                {
                    this.ComboBox_Pool_Name.Items.Add(new ItemPair<string, string>(row["Pool_Id"].ToString(), row["Name"].ToString()));
                }
                this.ComboBox_Pool_Name.Sorted = true;
            }
        }
        #endregion

        #region 選擇Pool，篩選Machine信息 Get Member Item List Procedure
        /// <summary>
        /// 選擇Pool，篩選Machine信息 pool index changed event.
        /// </summary>
        private void ComboBox_Pool_Name_SelectedIndexChanged(object sender, EventArgs e)
        {
            // get current selected item key ..
            string idx = ((ItemPair<string, string>)this.ComboBox_Pool_Name.SelectedItem).Key;
            // get all pool member ..
            IDictionary<string, string[]> pool_member = this.EnvCust.Member(ref this.EnvComm);
            // 顯示該Pool的共享類型 check is sharable type ..
            if (Convert.ToBoolean(this.pools.Select(string.Format("Pool_Id = '{0}'", idx))[0]["Sharable"]))
                this.CheckBox_Sharable.Checked = true;
            else
                this.CheckBox_Sharable.Checked = false;

            if (pool_member.ContainsKey(idx))
            {
                // clear before items ..
                if (this.ListBox_Member.Items.Count > 0)
                    this.ListBox_Member.Items.Clear();

                //篩選該Pool下的機器信息
                foreach (string s in pool_member[idx])
                {
                    DataRow[] row = this.machines.Select(string.Format("Machine_Id = '{0}'", s));

                    // add to member list ..
                    this.ListBox_Member.Items.Add(new ItemPair<string, string>(row[0]["Machine_Id"].ToString(), row[0]["Name"].ToString()));
                }
            }
            else
                this.ListBox_Member.Items.Clear();
        }
        #endregion

        #region （針對Pool）添加移除機器信息 Add, Remove Machine Item Procedure
        /// <summary>
        /// 添加 Add member click event.
        /// </summary>
        private void Button_Add_Member_Click(object sender, EventArgs e)
        {
            if (this.ListBox_Machine.SelectedItems.Count > 0)
            {
                // get selected machine items ..
                foreach (ItemPair<string, string> item in this.ListBox_Machine.SelectedItems)
                {
                    if (this.ListBox_Member.Items.Contains(item))
                        continue;
                    else
                        // add to member list ..
                        this.ListBox_Member.Items.Add(item);
                }
            }
        }

        /// <summary>
        /// 移除 Remove member click event.
        /// </summary>
        private void Button_Del_Member_Click(object sender, EventArgs e)
        {
            if (this.ListBox_Member.SelectedItems.Count > 0)
            {
                // create item pair collection ..
                IList<ItemPair<string, string>> _list = new List<ItemPair<string, string>>();

                foreach (ItemPair<string, string> item in this.ListBox_Member.SelectedItems)
                    _list.Add(item);

                foreach (ItemPair<string, string> item in _list)
                    // remove selected member item ..
                    this.ListBox_Member.Items.Remove(item);

                // clear temporarily items ..
                _list.Clear();
            }
        }
        #endregion

        #region 增刪更新Pool事件 Control Pool Button Event Procedure
        /// <summary>
        /// 添加Pool信息 Add pool click event.
        /// </summary>
        private void Button_Add_Pool_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(this.ComboBox_Pool_Name.Text))
            {
                // 確認是否重復添加 confirm the pool contains in pool table ..
                if (this.pools.Rows.Count != 0 && this.pools.Select(string.Format("Name = '{0}'", this.ComboBox_Pool_Name.Text.Trim())).Length > 0)
                {
                    // show error ..
                    MessageBox.Show(this, this.__AddPool_Repeat_Error, AssemblyInfoClass.ProductInfo,MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                }
                else
                {
                    // 定義Pool物件列表 define pool item list ..
                    IDictionary<string, object> Items = new Dictionary<string, object>
                    {
                        { "Pool_Id", string.Empty },
                        { "Name", this.ComboBox_Pool_Name.Text.Trim().ToUpper() },
                        { "Sharable", Convert.ToInt32(this.CheckBox_Sharable.Checked) }
                    };

                    // 添加新的Pool並設定成員 add new pool and setting member ..
                    if (!this.ChangePoolData(PoolAction.Add, Items))
                    {
                        MessageBox.Show(this, this.__AddPool_Error, AssemblyInfoClass.ProductInfo,MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    }
                    else
                    {
                        // 重新獲取Pool的數據 rebind pool data ..
                        this.GetPools();

                        // 獲取當前Pool get current pool row ..
                        DataRow[] currentRow = this.pools.Select(string.Format("Name = '{0}'", this.ComboBox_Pool_Name.Text.Trim().ToUpper()));

                        if (currentRow.Length > 0)
                        {
                            string[] machinelist = new string[this.ListBox_Member.Items.Count];
                            // add to machine array ..
                            for (int i = 0; i < this.ListBox_Member.Items.Count; i++)
                            {
                                machinelist[i] = ((ItemPair<string, string>)this.ListBox_Member.Items[i]).Key;
                            }

                            // clear old items ..
                            Items.Clear();
                            Items.Add("Pool_Id", currentRow[0]["Pool_Id"]);
                            Items.Add("Machines", machinelist);

                            //添加Machine數據
                            if (!this.ChangeMemberData(Items))
                            {
                                MessageBox.Show(this, this.__Pool_Member_Error, AssemblyInfoClass.ProductInfo,MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                            }
                            else
                            {
                                MessageBox.Show(this, this.__AddPool_Complete, AssemblyInfoClass.ProductInfo,MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);

                                // restore selected status ..
                                this.ComboBox_Pool_Name.Text = string.Empty;
                                this.CheckBox_Sharable.Checked = false;
                                this.ListBox_Member.Items.Clear();
                                this.ListBox_Machine.SelectedIndex = -1;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 更新Pool信息 Update pool click event.
        /// </summary>
        private void Button_Update_Pool_Click(object sender, EventArgs e)
        {
            if (this.ComboBox_Pool_Name.SelectedIndex >= 0)
            {
                // get current selected item ..
                string
                    id = ((ItemPair<string, string>)this.ComboBox_Pool_Name.SelectedItem).Key,
                    value = ((ItemPair<string, string>)this.ComboBox_Pool_Name.SelectedItem).Value;

                // define pool item list ..
                IDictionary<string, object> Items = new Dictionary<string, object>
                {
                    { "Pool_Id", id },
                    { "Name", value },
                    { "Sharable", Convert.ToInt32(this.CheckBox_Sharable.Checked) }
                };

                // update pool and setting member ..
                if (!this.ChangePoolData(PoolAction.Update, Items))
                    MessageBox.Show(this, this.__UpdatePool_Error, AssemblyInfoClass.ProductInfo,
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                else
                {
                    // rebind pool data ..
                    this.GetPools();

                    // get current pool row ..
                    DataRow[] currentRow = this.pools.Select(string.Format("Name = '{0}'", this.ComboBox_Pool_Name.Text.Trim().ToUpper()));

                    if (currentRow.Length > 0)
                    {
                        string[] machinelist = new string[this.ListBox_Member.Items.Count];

                        // add to machine array ..
                        for (int i = 0; i < this.ListBox_Member.Items.Count; i++)
                            machinelist[i] = ((ItemPair<string, string>)this.ListBox_Member.Items[i]).Key;

                        // clear old items ..
                        Items.Clear();

                        Items.Add("Pool_Id", currentRow[0]["Pool_Id"]);
                        Items.Add("Machines", machinelist);

                        if (!this.ChangeMemberData(Items))
                            MessageBox.Show(this, this.__Pool_Member_Error, AssemblyInfoClass.ProductInfo,
                                MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                        else
                        {
                            MessageBox.Show(this, this.__UpdatePool_Complete, AssemblyInfoClass.ProductInfo,
                                MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);

                            // restore selected status ..
                            this.ComboBox_Pool_Name.Text = string.Empty;
                            this.CheckBox_Sharable.Checked = false;
                            this.ListBox_Member.Items.Clear();
                            this.ListBox_Machine.SelectedIndex = -1;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 刪除Pool信息 Delete pool click event.
        /// </summary>
        private void Button_Delete_Pool_Click(object sender, EventArgs e)
        {
            if (this.ComboBox_Pool_Name.SelectedIndex >= 0)
            {
                // confirm result ..
                DialogResult result = MessageBox.Show(this, this.__DeletePool_Warning,
                    AssemblyInfoClass.ProductInfo, MessageBoxButtons.OKCancel,
                    MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);

                // delete pool workflow ..
                if (result == DialogResult.OK)
                {
                    // get current selected item ..
                    string
                        id = ((ItemPair<string, string>)this.ComboBox_Pool_Name.SelectedItem).Key,
                        value = ((ItemPair<string, string>)this.ComboBox_Pool_Name.SelectedItem).Value;

                    // define pool item list ..
                    IDictionary<string, object> Items = new Dictionary<string, object>
                    {
                        { "Pool_Id", id },
                        { "Name", value },
                        { "Sharable", Convert.ToInt32(this.CheckBox_Sharable.Checked) }
                    };

                    if (!this.ChangePoolData(PoolAction.Delete, Items))
                        MessageBox.Show(this, this.__DeletePool_Error, AssemblyInfoClass.ProductInfo,
                            MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    else
                    {
                        MessageBox.Show(this, this.__DeletePool_Complete, AssemblyInfoClass.ProductInfo,
                            MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);

                        // rebind pool data ..
                        this.GetPools();

                        // restore selected status ..
                        this.ComboBox_Pool_Name.Text = string.Empty;
                        this.CheckBox_Sharable.Checked = false;
                        this.ListBox_Member.Items.Clear();
                        this.ListBox_Machine.SelectedIndex = -1;
                    }
                }
            }
        }
        #endregion

        #region 更新Pool、Machine數據 Change Pool, Member Data Communication Procedure
        /// <summary>
        /// 新增或更新Pool的數據 Add or update pool data.
        /// </summary>
        /// <param name="Action">change pool behaivor.</param>
        /// <param name="Items">communication enumerable item.</param>
        /// <returns></returns>
        private bool ChangePoolData(PoolAction Action, IDictionary<string, object> Items)
        {
            bool result = false;

            // declare remote server response object ..
            KeyValuePair<string, object> responseObject;

            do
            {
                // confirm current resquest status ..
                if (EnvComm.CanRequest)
                {
                    // declare package sent data type ..
                    IList<object> packaged = null;

                    switch (Action)
                    {
                        case PoolAction.Add:
                        case PoolAction.Update:
                            // package sent data ..
                            packaged = EnvComm.Package(Client2Server.CommunicationType.POOLINFO, Items);
                            break;

                        case PoolAction.Delete:
                            // package sent data ..
                            packaged = EnvComm.Package(Client2Server.CommunicationType.DELETEPOOLINFO, Items);
                            break;
                    }

                    // wait for result ..
                    responseObject = EnvComm.Request(packaged);
                    break;
                }
            } while (true);

            // 確認正確結果 confirm correct result ..
            if (responseObject.Key.Substring(0, 1).Equals("+"))
                result = true;

            return result;
        }

        /// <summary>
        /// 新增或更新Pool的數據Add or update member of pool data.
        /// </summary>
        /// <param name="Items">communication enumerable item.</param>
        /// <returns>System.Boolean</returns>
        private bool ChangeMemberData(IDictionary<string, object> Items)
        {
            bool result = false;

            // declare remote server response object ..
            KeyValuePair<string, object> responseObject;

            do
            {
                // confirm current resquest status ..
                if (EnvComm.CanRequest)
                {
                    // declare package sent data type ..
                    IList<object> packaged = EnvComm.Package(Client2Server.CommunicationType.MACHINEPOOLRELATION, Items);

                    // wait for result ..
                    responseObject = EnvComm.Request(packaged);
                    break;
                }
            } while (true);

            // confirm correct result ..
            if (responseObject.Key.Substring(0, 1).Equals("+"))
                result = true;

            return result;
        }
        #endregion

    }
}