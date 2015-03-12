#region Using NameSpace
using System;
using System.Collections.Generic;
using System.Data;
using System.DirectoryServices;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using System.Xml;

// import renbar client properties namespace ..
using RenbarGUI.Properties;

// import renbar common library namespace ..
using RenbarLib.Environment;
using RenbarLib.Network;
using RenbarLib.Network.Protocol;
using RenbarLib.Network.Sockets;

#endregion

namespace RenbarLib.Environment.Forms.Customizations.Service
{
    /// <summary>
    /// Renbar client environment variable class.
    /// </summary>
    public class Customization
    {
        #region Declare Global Variable Section

       private Communication EnvComm= new Communication();

       private global::RenbarLib.Environment.Service EnvSvr = new RenbarLib.Environment.Service();

        // create renbar log base class object ..
        private Log EnvLog = new Log();

        // create job form combo-box list structure object ..
        private DropList FormDropList = new DropList();

        // create cache data object status ..
        private static DataSet _CacheData = new DataSet("CacheDataSet");

        // create data list view state ..
        private volatile DataSet DataListViewer = new DataSet("DataListView");

        //private string ServerXMLPath = "ServerAddress.xml";
        //private string ServerXMLPath = this.EnvSvr.ServerListFilePath;

        #endregion

        #region 檢測當前服務器的鏈接狀態 Detection Network Connect Status
        /// <summary>
        /// Detection currently connect server status.
        /// </summary>
        /// <param name="Setting">environment setting object.</param>
        /// <returns>System.Boolean</returns>
        public bool NetworkPing(Settings Setting)
        {
            // if setting is empty, return false ..
            //检测IP与端口號是否設置
            if (Setting.ServerIpAddress == null && Setting.ServerPort == 0)
                return false;
            else
            {
                //檢測指定地址端口是否開啟
                if (new ScanPort().Scan(Setting.ServerIpAddress, Setting.ServerPort))
                    return true;
                else
                    return false;
            }
        }
        #endregion

        #region 設置/變更當前系統用戶Get Or Set Currently Windows Account User Property
        /// <summary>
        /// Get or set current user name.
        /// </summary>
        public static string User
        {
            get;
            set;
        }
        #endregion

        #region 設置本地化資源（語言）Get Localization Resource Procedure
        /// <summary>
        /// Language type enumeration.
        /// </summary>
        public enum Language
        {
            /// <summary>
            /// English (United-State)
            /// </summary>
            En_Us,
            /// <summary>
            /// Traditional Chinese
            /// </summary>
            Zh_Tw
        }

        /// <summary>
        /// Get localization resource string.
        /// </summary>
        /// <param name="FormName">System.Windows.Forms.Form.Name</param>
        /// <param name="ResourceName">resource name</param>
        /// <param name="Lang">choice language</param>
        /// <returns>System.String</returns>
        public string GetLocalization(string FormName, string ResourceName, Language Lang)
        {
            string
                langresource = null,
                result = null;

            switch (Lang)
            {
                case Language.En_Us:
                    langresource = "en_us__" + FormName + "_" + ResourceName;
                    //ResourceManager取得指定之文化特性或隱含之目前 UI 文化特性的指定 String 資源
                    result = global::RenbarGUI.Properties.Resources.ResourceManager.GetString(langresource);
                    break;

                case Language.Zh_Tw:
                    langresource = "zh_tw__" + FormName + "_" + ResourceName;
                    result = global::RenbarGUI.Properties.Resources.ResourceManager.GetString(langresource);
                    break;
            }

            return result;
        }
        #endregion

        #region 設置OpenFileDialog的一些通用（默認）屬性Get OpenFileDialog Common Property
        /// <summary>
        /// Get openFileDialog common property.
        /// </summary>
        public OpenFileDialog OpenFileDialog
        {
            get
            {
                OpenFileDialog fd = new OpenFileDialog();

                // setting openfile dialog ..
                fd.AddExtension = false;             //是否在使用者遺漏副檔名時，自動加入檔案的副檔名
                fd.AutoUpgradeEnabled = true;    //取得或設定值，指出此 FileDialog 執行個體是否應在 Windows Vista 上執行時自動升級外觀和行為。
                fd.CheckFileExists = true;           //指出如果使用者指定不存在的檔名，檔案對話方塊是否會顯示警告訊息
                fd.CheckPathExists = true;         //指定如果使用者輸入無效的路徑和檔名，系統是否會顯示警告訊息
                fd.DereferenceLinks = true;         //指出檔案對話方塊會傳回捷徑所參照的檔案位置，還是傳回捷徑檔 (.lnk) 的位置
                fd.Filter = "Renbar Saved Render File (*.rbr)|*.rbr|Smedge Saved Render File (*.smr)|*.smr";
                fd.FilterIndex = 0;
                fd.Multiselect = false;
                //System.Environment.SpecialFolder指定用來擷取系統特殊資料夾目錄路徑
                fd.InitialDirectory = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);//取得或設定檔案對話方塊所顯示的初始目錄
                fd.ShowReadOnly = true;
                fd.SupportMultiDottedExtensions = false;//取得或設定對話方塊是否支援顯示和儲存具有多個副檔名的檔案

                return fd;
            }
        }
        #endregion

        #region 解析.rbr文件結構 Parse Render File Structure Procedure
        /// <summary>
        /// 解析文件結構 Parse load file structure.
        /// </summary>
        /// <param name="ParseFile">待分析文件analysis file location.</param>
        /// <param name="Items">如果成功則返回內容，否則返回空列表 if parse success return all job attribute; otherwise return empty list.</param>
        /// <returns>System.Int32 (0 = Success, 1 = Parse Error, 2 = Multi File Error)</returns>
        public int Parse(string ParseFile, ref IDictionary<string, object> Items)
        {
            // declare default return result ..
            int result = 1;

            // create renbar file system class object instance ..
            FileSystem fs = new FileSystem();

            if (!string.IsNullOrEmpty(ParseFile))
            {
                FileInfo info = new FileInfo(ParseFile);

                int __doc = 0;

                // parse load file structure ..
                switch (info.Extension.ToLower())
                {
                    #region Parse Renbar File Format
                    case ".rbr":
                        // confirm is batch file ..
                        using (XmlReader xdr = XmlTextReader.Create(ParseFile))
                        {
                            while (xdr.Read())
                            {
                                if (xdr.HasAttributes)
                                    __doc++;
                            }
                        }

                        if (__doc <= 2)
                        {
                            if (fs.Convert(FileSystem.RenderFileType.Rbr, FileSystem.RenderMethod.Gui, ParseFile, ref Items))
                                result = 0;
                        }
                        else
                            result = 2;
                        break;
                    #endregion

                    #region Parse Smedge File Format
                    case ".smr":
                        using (StreamReader sdr = new StreamReader(ParseFile))
                        {
                            string line = string.Empty;

                            while (!string.IsNullOrEmpty(line = sdr.ReadLine()))
                            {
                                if (line.IndexOf('[') > -1 && line.IndexOf(']') > -1)
                                    __doc++;
                            }
                        }

                        if (__doc == 1)
                        {
                            if (fs.Convert(FileSystem.RenderFileType.Smr, FileSystem.RenderMethod.Gui, ParseFile, ref Items))
                                result = 0;
                        }
                        else
                            result = 2;
                        break;
                    #endregion
                }
            }

            return result;
        }
        #endregion

        #region 到期日Get Due Date Property
        /// <summary>
        /// check renbar client due date.
        /// </summary>
        public bool DueDate
        {
            get
            {
                
                //DateTime
                //    StartDate = new DateTime(2010, 1, 1),
                //    EndDate = StartDate.AddMonths(120);

                DateTime StartDate = RenbarGUI.Properties.Settings.Default.StartDate;
                int UseingTime = RenbarGUI.Properties.Settings.Default.Month;
                DateTime EndDate = StartDate.AddMonths(UseingTime);

                //當前日期減去指定日期EndData（2009/11/1加3個月）的時間跨度是否大于0
                if (DateTime.Now.Subtract(EndDate).Ticks > 0)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }
        #endregion

        #region 獲取線程池、機器信息、隊列 Get Pool, Machine And WaitFor Data List Procedure

        /// <summary>
        /// 發送請求、獲取當前線程池 Get currently pools on server.
        /// </summary>
        /// <param name="EnvComm">currently communication class.</param>
        /// <returns>System.Data.DataTable（responseObject.Value）</returns>
        public DataTable AllPools(ref Communication EnvComm)
        {
            // create data object instance ..
            DataTable pools = new DataTable();

            // declare remote server response object ..
            KeyValuePair<string, object> responseObject;

            do
            {
                // confirm current request status ..
                if (EnvComm.CanRequest)
                {
                    // package sent data ..
                    IList<object> packaged = EnvComm.Package(Client2Server.CommunicationType.VIEWPOOLINFO, new Dictionary<string, object>());

                    // wait for result ..
                    responseObject = EnvComm.Request(packaged);
                    break;
                }
                Thread.Sleep(1000);
            } while (true);

            // confirm correct result ..
            if (responseObject.Key.Substring(0, 1).Equals("+"))
            {
                if (responseObject.Value != null)
                {

                    pools = (DataTable)responseObject.Value;
                }
            }

            return pools;
        }

        /// <summary>
        /// 獲取當前活躍群組（MACHINEPOOLRELATION） Get currently effective pools on server.
        /// </summary>
        /// <param name="EnvComm">currently communication class.</param>
        /// <returns>System.Data.DataTable</returns>
        public DataTable Pools(ref Communication EnvComm)
        {
            // create data object instance ..
            DataTable poolTable = new DataTable("Pool");

            // declare data object instance ..
            IList<string> addinList = new List<string>();

            // declare remote server response object ..
            KeyValuePair<string, object> responseObject;

            do
            {
                // confirm current request status ...
                if (EnvComm.CanRequest)
                {
                    // package sent data ...
                    IList<object> packaged = EnvComm.Package(Client2Server.CommunicationType.VIEWMACHINEPOOLRELATION, new Dictionary<string, object>());

                    // wait for result ..
                    responseObject = EnvComm.Request(packaged);
                    break;
                }
                Thread.Sleep(1000);
            } while (true);

            // get efficacious pools ..
            if (responseObject.Key.Substring(0, 1).Equals("+"))
            {
                if (responseObject.Value != null)
                {
                    DataTable pools = (DataTable)responseObject.Value;//？？？？？？？？？？？？？？？？？？？？？？？？？？？

                    foreach (DataRow row in pools.Rows)
                    {
                        // decide pool table has contains this row data ..
                        if (!addinList.Contains(row["Pool_Id"].ToString()))
                        {
                            IDictionary<string, object> poolInfo = new Dictionary<string, object>
                            {
                                {"Pool_Id", row["Pool_Id"]}
                            };

                            do
                            {
                                // confirm current request status ..
                                if (EnvComm.CanRequest)
                                {
                                    // package sent data ..
                                    IList<object> packaged = EnvComm.Package(Client2Server.CommunicationType.VIEWPOOLINFO, poolInfo);

                                    // wait for result ..
                                    responseObject = EnvComm.Request(packaged);

                                    break;
                                }
                                Thread.Sleep(1000);
                            } while (true);

                            // confirm correct result ..
                            if (responseObject.Key.Substring(0, 1).Equals("+"))
                            {
                                if (responseObject.Value != null)
                                {
                                    // receive remote data object and copy to pool table ..
                                    poolTable.Merge(((DataTable)responseObject.Value).Copy(), true);

                                    // commit all changes ..
                                    poolTable.AcceptChanges();

                                    // add to temporary data list ..
                                    addinList.Add(row["Pool_Id"].ToString());
                                }
                            }
                            else
                                break;
                        }
                    }
                }
            }

            return poolTable;
        }

        /// <summary>
        /// 獲取機器信息 Get currently all machine on server.
        /// </summary>
        /// <param name="EnvComm">currently communication class.</param>
        /// <returns>System.Data.DataTable</returns>
        public DataTable Machines(ref Communication EnvComm)
        {
            // declare data object instance ..
            DataTable machineTable = new DataTable();

            // declare remote server response object ..
            KeyValuePair<string, object> responseObject;

            do
            {
                // confirm current request status ..
                if (EnvComm.CanRequest)
                {
                    // package sent data ..
                    IList<object> packaged = EnvComm.Package(Client2Server.CommunicationType.VIEWMACHINEINFO, new Dictionary<string, object>());

                    // wait for result ... 發送請求
                    responseObject = EnvComm.Request(packaged);
                    break;
                }
                Thread.Sleep(1000);
            } while (true);

            // confirm correct result ..
            if (responseObject.Key.Substring(0, 1).Equals("+"))
            {
                // get all machines ..
                if (responseObject.Value != null)
                    machineTable = (DataTable)responseObject.Value;
            }

            return machineTable;
        }

        /// <summary>
        /// Get currently pool member on server.
        /// </summary>
        /// <param name="EnvComm">currently communication class.</param>
        /// <returns>System.Collection.Generic</returns>
        public IDictionary<string, string[]> Member(ref Communication EnvComm)
        {
            // create data object instance ..
            IDictionary<string, string[]> memberDictionary = new Dictionary<string, string[]>();

            // declare remote server response object ..
            KeyValuePair<string, object> responseObject;

            do
            {
                // confirm current request status ..
                if (EnvComm.CanRequest)
                {
                    // package sent data ..
                    IList<object> packaged = EnvComm.Package(Client2Server.CommunicationType.VIEWMACHINEPOOLRELATION, new Dictionary<string, object>());

                    // wait for result ..
                    responseObject = EnvComm.Request(packaged);
                    break;
                }
                Thread.Sleep(1000);
            } while (true);

            // 確認信息並篩選 confirm correct result ..
            if (responseObject.Key.Substring(0, 1).Equals("+"))
            {
                if (responseObject.Value != null)
                {
                    var machine_pool = from mp in ((DataTable)responseObject.Value).AsEnumerable()
                                       group mp by mp.Field<string>("Pool_Id") into pool_groups
                                       select pool_groups.ToList();

                    foreach (var group in machine_pool)
                    {
                        // declare machine array count ..
                        string[] machines = new string[group.Count];

                        for (int i = 0; i < group.Count; i++)
                            // assign machine ..
                            machines[i] = ((DataRow)group[i])["Machine_Id"].ToString();

                        if (!memberDictionary.ContainsKey(group[0]["Pool_Id"].ToString()))
                            // add to pool machine list ..
                            memberDictionary.Add(group[0]["Pool_Id"].ToString(), machines);
                        else
                            // refresh machine list ..
                            memberDictionary[group[0]["Pool_Id"].ToString()] = machines;
                    }
                }
            }

            return memberDictionary;
        }

        /// <summary>
        /// 獲取任務狀態，篩選 Get currently can wait for jobs.
        /// </summary>
        /// <param name="EnvComm">currently communication class.</param>
        /// <returns>System.Data.DataTable</returns>
        public IDictionary<string, string> GetWaitFor(ref Communication EnvComm)
        {
            // declare data object instance ..
            IDictionary<string, string> waitforlist = new Dictionary<string, string>();

            // declare remote server response object ..
            KeyValuePair<string, object> responseObject;

            do
            {
                // confirm current request status ..
                if (EnvComm.CanRequest)
                {
                    // package sent data ..
                    IList<object> packaged = EnvComm.Package(Client2Server.CommunicationType.VIEWJOBSTATUS, new Dictionary<string, object>());

                    // wait for result ..
                    responseObject = EnvComm.Request(packaged);
                    break;
                }
                Thread.Sleep(1000);
            } while (true);

            // get current wait for list ..
            if (responseObject.Key.Substring(0, 1).Equals("+"))
            {
                if (responseObject.Value != null)
                {
                    // get remote server data state ..
                    DataSet remoteDs = (DataSet)responseObject.Value;

                    // fifter can use job list ..
                    string expression = string.Format("Status = {0}", (UInt16)JobStatusFlag.QUEUING);
                    DataRow[] canSelect = remoteDs.Tables["Job_Group"].Select(expression);

                    for (int i = 0; i < canSelect.Length; i++)
                    {
                        if (!string.IsNullOrEmpty(canSelect[i]["Job_Group_Id"].ToString()) && !string.IsNullOrEmpty(canSelect[i]["Name"].ToString()))
                        {
                            if (!waitforlist.ContainsKey(canSelect[i]["Job_Group_Id"].ToString()))
                            {
                                // add to wait for list ..
                                waitforlist.Add(canSelect[i]["Job_Group_Id"].ToString(), canSelect[i]["Name"].ToString());
                            }
                        }
                    }
                }
            }

            return waitforlist;
        }

        #endregion

        #region 獲取當前任務列 Get Current Form Drop List
        /// <summary>
        /// Return job form drop-down list.
        /// </summary>
        public DropList DropDown
        {
            get
            {
                return this.FormDropList;
            }
        }
        #endregion

        #region 獲取物件狀態列表 Get Item Status List Procedure
        /// <summary>
        /// 獲取最新View數據 Get latest view data.
        /// </summary>
        /// <param name="EnvComm">currently communication class.</param>
        public DataSet DataState(ref Communication EnvComm)
        {
            DataSet resultData = new DataSet();

            try
            {
                #region 定義DataTable，並添加到resultData中 ListView DataTable Define
                // define processing job list ..
                DataTable proc = new DataTable("Processing");
                proc.Columns.Add(new DataColumn("Proc_Id"));
                proc.Columns.Add(new DataColumn("Job"));
                proc.Columns.Add(new DataColumn("Frames"));
                proc.Columns.Add(new DataColumn("Host"));
                proc.Columns.Add(new DataColumn("Elapsed"));
                proc.Columns.Add(new DataColumn("Log"));
                proc.PrimaryKey = new DataColumn[] { proc.Columns["Proc_Id"] };
                // add to dataset ..
                resultData.Tables.Add(proc);

                // define queue job list ..
                DataTable queue = new DataTable("Queue");
                queue.Columns.Add(new DataColumn("Queue_Id"));
                queue.Columns.Add(new DataColumn("Status_Id"));
                queue.Columns.Add(new DataColumn("Completed"));
                queue.Columns.Add(new DataColumn("ProcType"));
                queue.Columns.Add(new DataColumn("Priority"));
                queue.Columns.Add(new DataColumn("Project"));
                queue.Columns.Add(new DataColumn("Job"));
                queue.Columns.Add(new DataColumn("Frames"));
                queue.Columns.Add(new DataColumn("First_Pool"));
                queue.Columns.Add(new DataColumn("Second_Pool"));
                queue.Columns.Add(new DataColumn("Submited_User"));
                queue.Columns.Add(new DataColumn("Submited_Time"));
                queue.Columns.Add(new DataColumn("Note"));
                queue.PrimaryKey = new DataColumn[] { queue.Columns["Queue_Id"] };
                // add to dataset ..
                resultData.Tables.Add(queue);

                // define host machine list ..
                DataTable host = new DataTable("Host");
                host.Columns.Add(new DataColumn("Host_Id"));
                host.Columns.Add(new DataColumn("Host"));
                host.Columns.Add(new DataColumn("Status"));
                host.Columns.Add(new DataColumn("Processors"));
                host.Columns.Add(new DataColumn("Connected_Time"));
                host.Columns.Add(new DataColumn("Priority"));
                host.Columns.Add(new DataColumn("Note"));
                host.Columns.Add(new DataColumn("Type"));
                host.PrimaryKey = new DataColumn[] { host.Columns["Host_Id"] };
                // add to dataset ……
                resultData.Tables.Add(host);
                #endregion

                #region 分析數據 Data Analysis
                if (resultData.Tables.Count > 0)
                {
                    // 獲取遠端服務器的信息 get remote server data state ..？？？？？？？？？？為什么獲取不到機器信息！！
                    DataSet remoteDs = this.GetRemoteState(ref EnvComm);

                    if (remoteDs.Tables.Contains("Job_Group") && remoteDs.Tables.Contains("Job_Attr")
                        && remoteDs.Tables.Contains("Job") && remoteDs.Tables.Contains("Machine"))
                    {
                        DataTable Machines = this.Machines(ref EnvComm);//view amchineinfo
                        //DataTable Pools = this.AllPools(ref EnvComm);//view machine pool relate
                        IDictionary<string, Attributes> Attr = new Dictionary<string, Attributes>();  // 任务属性 job attributes dictionary ..

                        #region 數據處理……

                        // Job通用属性的數據 common attributes partial ..
                        foreach (DataRow row1 in remoteDs.Tables["Job_Group"].Rows)
                        {
                            // create attribute struct instance ……
                            Attributes attr = new Attributes();

                            #region  Setting Attribute Structure
                            string key = row1["Job_Group_Id"].ToString().Trim();
                            // assign attribute ..
                            attr.Name = row1["Name"].ToString().Trim();
                            attr.FirstPool = new Guid(row1["First_Pool"].ToString());//？？？？？？？？？？？？？？？？？Pool ID？？？？
                            attr.Status = (JobStatusFlag)Convert.ToUInt16(row1["Status"]);  // PROCESSING = 6
                            attr.SubmitMachine = new Guid(row1["Submit_Machine"].ToString());//？？？？？？？？？？？？Machine ID？？？
                            attr.SubmitTime = (DateTime)row1["Submit_Time"];
                            attr.SubmitAcct = row1["Submit_Acct"].ToString().Trim();
                            if (row1["Second_Pool"] != DBNull.Value)
                                attr.SecondPool = new Guid(row1["Second_Pool"].ToString());//？？？？？？？？？？？？？？Pool ID？？？？？
                            if (row1["Start_Time"] != DBNull.Value)
                                attr.StartTime = (DateTime)row1["Start_Time"];
                            if (row1["Finish_Time"] != DBNull.Value)
                                attr.FinishTime = (DateTime)row1["Finish_Time"];
                            if (row1["Note"] != DBNull.Value)
                                attr.Note = row1["Note"].ToString().Trim();
                            //篩選數據
                            DataRow[] row2 = remoteDs.Tables["Job_Attr"].Select(string.Format("Job_Group_Id = '{0}'", key));
                            if (row2.Length == 0)
                                continue;
                            attr.Project = row2[0]["Project"].ToString().Trim();
                            attr.Start = Convert.ToInt32(row2[0]["Start"]);
                            attr.End = Convert.ToInt32(row2[0]["End"]);
                            attr.PacketSize = Convert.ToUInt16(row2[0]["Packet_Size"]);
                            attr.ProcType = row2[0]["Proc_Type"].ToString().Trim();
                            attr.Priority = Convert.ToUInt16(row2[0]["Priority"]);
                            attr.AbUpdateOnly = Convert.ToBoolean(Convert.ToInt32(row2[0]["ABUpdateOnly"].ToString().Trim()));
                            if (row2[0]["WaitFor"] != DBNull.Value && !string.IsNullOrEmpty(row2[0]["WaitFor"].ToString()))
                                attr.WaitFor = new Guid(row2[0]["WaitFor"].ToString().Trim());//？？？？？？？？？？？？？？？？？
                            if (row2[0]["ABName"] != DBNull.Value && !string.IsNullOrEmpty(row2[0]["ABName"].ToString()))
                                attr.AbName = row2[0]["ABName"].ToString().Trim();
                            if (row2[0]["ABPath"] != DBNull.Value && !string.IsNullOrEmpty(row2[0]["ABPath"].ToString()))
                                attr.AbPath = row2[0]["ABPath"].ToString().Trim();
                            // add to attribute dictionary ..
                            Attr.Add(key, attr);
                            #endregion
                        }

                        //  隊列中Job属性的數據 queue partial ..？？？？？？？？？？？？？？？？？？？？？？？？
                        foreach (DataRow dr in remoteDs.Tables["Job_Group"].Rows)
                        {
                            #region Setting Queue List
                            string key = dr["Job_Group_Id"].ToString().Trim();

                            // get this job attribute ..
                            if (Attr.ContainsKey(key))
                            {
                                DataRow[] job_com_rows = remoteDs.Tables["Job"].Select(string.Format("Job_Group_Id = '{0}' And Finish_Time Is Not Null", key));
                                DataRow[] job_all_rows = remoteDs.Tables["Job"].Select(string.Format("Job_Group_Id = '{0}'", key));

                                // get current job complete progress ..
                                string completed = string.Empty;
                                if (job_com_rows.Length == 0 && job_all_rows.Length == 0)
                                    completed = "0";
                                else
                                    //？？？？？？？？？？？？？計算完成百分比？？？？？？？？？？？？？？？？？
                                    completed = Math.Round(((Convert.ToDouble(job_com_rows.Length) / Convert.ToDouble(job_all_rows.Length)) * 100)).ToString();

                                DataRow row = resultData.Tables["Queue"].NewRow();
                                row["Queue_Id"] = dr["Job_Group_Id"].ToString().Trim();
                                row["Status_Id"] = ((Attributes)Attr[key]).Status;
                                row["Completed"] = string.Format("{0}%", completed);
                                row["ProcType"] = ((Attributes)Attr[key]).ProcType;
                                row["Priority"] = ((Attributes)Attr[key]).Priority;
                                row["Project"] = ((Attributes)Attr[key]).Project;
                                row["Job"] = ((Attributes)Attr[key]).Name;
                                row["Frames"] = string.Format("{0}-{1}[{2}]", ((Attributes)Attr[key]).Start, ((Attributes)Attr[key]).End, ((Attributes)Attr[key]).PacketSize);

                                #region F7300290
                                try
                                {
                                    //指定的索引鍵不在字典中(——如果Pool已被删除怎么办？——)
                                    string F_Pool_Id = string.Format(((Attributes)Attr[key]).FirstPool.ToString()).ToUpper();
                                    row["First_Pool"] = FormDropList.Pool[F_Pool_Id].ToString();
                                    ////row["First_Pool"] = Pools.Select(string.Format("Pool_Id = '{0}'", ((Attributes)Attr[key]).FirstPool))[0]["Name"].ToString();
                                }
                                catch
                                {
                                    row["First_Pool"] = FormDropList.Pool.ElementAt(0).Value.ToString();
                                }
                                finally
                                { }
                                #endregion

                                if (!((Attributes)Attr[key]).SecondPool.Equals(Guid.Empty))
                                {
                                    string S_Pool_Id = string.Format(((Attributes)Attr[key]).SecondPool.ToString()).ToUpper();
                                    row["Second_Pool"] = FormDropList.Pool[S_Pool_Id].ToString();
                                    ////row["Second_Pool"] = Pools.Select(string.Format("Pool_Id = '{0}'", ((Attributes)Attr[key]).SecondPool))[0]["Name"].ToString();
                                }

                                row["Submited_User"] = ((Attributes)Attr[key]).SubmitAcct;
                                row["Submited_Time"] = ((Attributes)Attr[key]).SubmitTime;
                                row["Note"] = ((Attributes)Attr[key]).Note;

                                if (((Attributes)Attr[key]).Status == JobStatusFlag.PROCESSING)
                                {
                                    // processing partial ..
                                    DataRow[] procRows = remoteDs.Tables["Job"].Select(
                                        string.Format("Job_Group_Id = '{0}'", key));

                                    for (int i = 0; i < procRows.Length; i++)
                                    {
                                        if (!DBNull.Value.Equals(procRows[i]["Proc_Machine"]) &&
                                            !DBNull.Value.Equals(procRows[i]["Start_Time"]) &&
                                            !DBNull.Value.Equals(procRows[i]["Finish_Time"]))
                                            continue;

                                        if (DBNull.Value.Equals(procRows[i]["Proc_Machine"]) &&
                                            DBNull.Value.Equals(procRows[i]["Start_Time"]))
                                            continue;

                                        #region Settings Processing List
                                        DataRow srow = resultData.Tables["Processing"].NewRow();
                                        srow["Proc_Id"] = procRows[i]["Job_Id"];
                                        srow["Job"] = ((Attributes)Attr[key]).Name;

                                        ////無法在已失敗的對應 (Match) 上呼叫結果。Command格式不正確！！！！！！！！！！！！！！
                                        ////結果：
                                        ////""C:\\Program Files\\Autodesk\\Maya2008\\bin\\Render.exe -r mr -proj C:\\alienbrainWork\\UAT20090709\\CGI\\element           
                                        ////-s #S -e #E -rd Y:\\ C:\\alienbrainWork\\UAT20090709\\CGI\\element\\data\\ch\\mumu\\ok\\mumu.ma""
                                        ////不符合？？？？？？？？？？

                                        //string sd = procRows[i]["Command"].ToString();

                                        srow["Frames"]
                                            = string.Format("{0}-{1}",
                                            new Regex(@"-[S-s]\s(?<num>[0-9]+)").Match(procRows[i]["Command"].ToString()).Result("${num}"),
                                            new Regex(@"-[E-e]\s(?<num>[0-9]+)").Match(procRows[i]["Command"].ToString()).Result("${num}"));

                                        //string Select_Machine_Id = string.Format("Machine_Id = '{0}'", procRows[i]["Proc_Machine"].ToString().Trim());
                                        //srow["Host"] = remoteDs.Tables["Machine"].Select(string.Format(Select_Machine_Id))[0]["Name"].ToString();
                                        ////srow["Host"] = remoteDs.Tables["Machine"].Select(string.Format(
                                        ////    string.Format("Machine_Id = '{0}'", procRows[i]["Proc_Machine"].ToString().Trim()) ) )[0]["Name"].ToString();

                                        string macc = procRows[i]["Proc_Machine"].ToString().Trim();

                                        srow["Host"] = Machines.Select(string.Format(string.Format("Machine_Id = '{0}'",
                                            procRows[i]["Proc_Machine"].ToString().Trim())))[0]["Name"].ToString();

                                        srow["Elapsed"] = procRows[i]["Start_Time"];
                                        srow["Log"] = procRows[i]["OutputLog"];

                                        // add to data state ..
                                        resultData.Tables["Processing"].Rows.Add(srow);
                                        #endregion
                                    }
                                }

                                // add to data state ..
                                resultData.Tables["Queue"].Rows.Add(row);
                            }
                            #endregion
                        }

                        // host partial ..
                        foreach (DataRow dr in remoteDs.Tables["Machine"].Rows)
                        {
                            #region Setting Host List
                            DataRow row = resultData.Tables["Host"].NewRow();
                            row["Host_Id"] = dr["Machine_Id"].ToString().Trim();
                            row["Host"] = dr["HostName"].ToString().Trim();

                            if (Convert.ToBoolean(dr["IsEnable"]))
                            {
                                row["Status"] = "Enabled";
                            }
                            else
                            {
                                row["Status"] = "Disabled";
                            }

                            row["Processors"] = string.Format("{0} using {1}", dr["TCore"], dr["UCore"]);
                            row["Connected_Time"] = dr["Last_Online_Time"].ToString().Trim();
                            row["Priority"] = dr["Machine_Priority"].ToString().Trim();
                            row["Note"] = dr["Note"].ToString().Trim();
                            row["Type"] = dr["Machine_Status"];

                            // add to data state ..
                            resultData.Tables["Host"].Rows.Add(row);
                            #endregion
                        }
                        #endregion
                    }
                }
                #endregion
            }
            catch (Exception ex)
            {
                string ExceptionMsg = ex.Message + ex.StackTrace;
                // write to log file ..
                EnvLog.Writer(AssemblyInfoClass.ProductInfo, Log.Level.Error, ExceptionMsg, true);
            }
            return resultData;
        }

        /// <summary>
        /// 發送數據到服務器 Send data to server request.
        /// </summary>
        /// <param name="EnvComm">currently communication class.</param>
        private DataSet GetRemoteState(ref Communication EnvComm)
        {
            DataSet result = new DataSet();

            // declare remote server response object ..
            KeyValuePair<string, object> responseObject;

            #region 任務狀態Job State Partial
            do
            {
                // confirm current request status ..
                if (EnvComm.CanRequest)
                {
                    // package sent data ..
                    IList<object> packaged = EnvComm.Package(Client2Server.CommunicationType.VIEWJOBSTATUS, new Dictionary<string, object>());
                    // wait for result ..
                    responseObject = EnvComm.Request(packaged);
                    break;
                }
                Thread.Sleep(1000);
            } while (true);

            // confirm correct result ..
            if (responseObject.Key.Substring(0, 1).Equals("+"))
            {
                if (responseObject.Value != null)
                {
                    // merge data ..……
                    result.Merge(((DataSet)responseObject.Value).Copy(), true);
                    // commit all changes ..
                    result.AcceptChanges();
                }
            }
            #endregion

            #region 機器狀態 Machine State Partial
            do
            {
                // confirm current request status ..
                if (EnvComm.CanRequest)
                {
                    // package sent data ..
                    IList<object> packaged = EnvComm.Package(Client2Server.CommunicationType.VIEWMACHINERENDERINFO, new Dictionary<string, object>());

                    // wait for result ..
                    responseObject = EnvComm.Request(packaged);
                    break;
                }
                Thread.Sleep(1000);
            } while (true);

            // machine partial ..
            if (responseObject.Key.Substring(0, 1).Equals("+"))
            {
                if (responseObject.Value != null)
                {
                    // merge data ..
                    result.Merge(((DataTable)responseObject.Value).Copy(), true, MissingSchemaAction.AddWithKey);

                    // commit all changes ...
                    result.AcceptChanges();
                }
            }
            #endregion

            #region Pool Partial
            IDictionary<string, string>
                First = new Dictionary<string, string>(),
                Second = new Dictionary<string, string>();

            // effective pool data ..
            DataTable __pools = AllPools(ref EnvComm);

            if (__pools.Columns.Count > 0)
            {
                // first pool ..
                foreach (DataRow row in __pools.Select().OrderBy(name => name["Name"]))
                {
                    First.Add(row["Pool_Id"].ToString().Trim(), row["Name"].ToString().Trim());
                }

                // second pool ..
                foreach (DataRow row in __pools.Select("Sharable = 1").OrderBy(name => name["Name"]))
                {
                    Second.Add(row["Pool_Id"].ToString().Trim(), row["Name"].ToString().Trim());
                }
            }

            // assign pool ..
            FormDropList.Pool = First;
            FormDropList.Pool2 = Second;
            #endregion

            #region WaitFor Partial
            //// job wait for ..
            //FormDropList.Waitfor = (IDictionary<string, string>)GetWaitFor(ref EnvComm);

            // F7300290
            IDictionary<string, string> waitforlist = new Dictionary<string, string>();
            if (result.Tables.Contains("Job_Group"))
            {
                //篩選狀態為QUEUING的數據
                string expression = string.Format("Status = {0}", (UInt16)JobStatusFlag.QUEUING);
                DataRow[] canSelect = result.Tables["Job_Group"].Select(expression);

                if (canSelect.Length > 0)
                {
                    for (int i = 0; i < canSelect.Length; i++)
                    {
                        if (!string.IsNullOrEmpty(canSelect[i]["Job_Group_Id"].ToString()) && !string.IsNullOrEmpty(canSelect[i]["Name"].ToString()))
                        {
                            if (!waitforlist.ContainsKey(canSelect[i]["Job_Group_Id"].ToString()))
                            {
                                waitforlist.Add(canSelect[i]["Job_Group_Id"].ToString(), canSelect[i]["Name"].ToString());
                            }
                        }
                    }

                }
            }
            else
            {
                waitforlist = (IDictionary<string, string>)GetWaitFor(ref EnvComm);
            }
            FormDropList.Waitfor = waitforlist;

            #endregion

            return result;
        }

        /// <summary>
        /// 獲取或設定當前數據狀態 Get or set currently data status.
        /// </summary>
        public DataSet DataListView
        {
            get
            {
                return DataListViewer;
            }
            set
            {
                DataListViewer = value;
            }
        }

        #endregion

        #region F7300290！！！  獲取單個任務的詳細信息 Get Job Detail Info

        /// <summary>
        /// 通過Job_Group_Id獲取數據的詳細信息
        /// </summary>
        /// <param name="JobGroupId"></param>
        /// <returns></returns>
        public IDictionary<string, object> GetJobInfo(string JobGroupId, ref Communication EnvComm)
        {
            IDictionary<string, object> responData_Attrs = new Dictionary<string, object>();

            KeyValuePair<string, object> responseObject;
            #region 根據JobID獲取任務數據
            //獲取jobid
            IDictionary<string, object> JobItem = new Dictionary<string, object>
                {
                    { "Job_Group_Id",  JobGroupId}
                };

            // 发送请求 send request to remote server ..
            do
            {
                // confirm current request status ..
                if (EnvComm.CanRequest)
                {
                    // package sent data ..
                    IList<object> packaged = EnvComm.Package(Client2Server.CommunicationType.VIEWSINGLEJOBINFO, JobItem);
                    // wait for result ..
                    responseObject = EnvComm.Request(packaged);
                    break;
                }
                Thread.Sleep(500);

            } while (true);
            #endregion

            if (responseObject.Key.Substring(0, 1) == "+")
            {
                if (responseObject.Value != null)
                {
                    #region 解析數據
                    DataSet responData = (DataSet)responseObject.Value;

                    if (responData.Tables.Contains("Job_Group") && responData.Tables.Contains("Job_Attr"))
                    {
                        DataTable
                        JobGroup = responData.Tables["Job_Group"],
                        Job_Attr = responData.Tables["Job_Attr"];

                        //添加Job_Group數據
                        foreach (DataColumn dc in JobGroup.Columns)
                        {
                            responData_Attrs.Add(dc.ColumnName, JobGroup.Rows[0][dc.ColumnName]);
                        }
                        //添加Job_Attr數據，如果有重復字段則不添加
                        foreach (DataColumn dc in Job_Attr.Columns)
                        {
                            if (responData_Attrs.ContainsKey(dc.ColumnName))
                            {
                                //do nothing 去掉job_group_id;
                            }
                            else
                            {
                                responData_Attrs.Add(dc.ColumnName, Job_Attr.Rows[0][dc.ColumnName]);
                            }
                        }
                    }
                    #endregion
                }
                else
                {
                    responData_Attrs = null;
                }
            }
            return responData_Attrs;
        }

        /// <summary>
        /// 轉換為JobForm能夠接受的數據源格式
        /// </summary>
        /// <param name="OrgData">原始數據</param>
        /// <returns>JobForm能夠接受的數據</returns>
        public IDictionary<string, object> DataRevise(IDictionary<string, object> OrgData)
        {
            IDictionary<string, object> Item = new Dictionary<string, object>();

            if (OrgData.ContainsKey("Name"))
                Item.Add("name", OrgData["Name"].ToString());

            if (OrgData.ContainsKey("WaitFor"))
                Item.Add("waitFor", OrgData["WaitFor"].ToString());

            if (OrgData.ContainsKey("ABUpdateOnly"))
                Item.Add("isUpdateOnly", Convert.ToBoolean(Convert.ToInt32(OrgData["ABUpdateOnly"].ToString().Trim())));

            if (OrgData.ContainsKey("Proc_Type"))
                Item.Add("type", OrgData["Proc_Type"].ToString());

            if (OrgData.ContainsKey("ABName"))
                Item.Add("ABProjectName", OrgData["ABName"].ToString());

            if (OrgData.ContainsKey("ABPath"))
                Item.Add("ABNode", OrgData["ABPath"].ToString());

            if (OrgData.ContainsKey("Project"))
                Item.Add("ProjectName", OrgData["Project"].ToString());

            if (OrgData.ContainsKey("First_Pool"))
                Item.Add("FirstPool", OrgData["First_Pool"].ToString());

            if (OrgData.ContainsKey("Second_Pool"))
                Item.Add("SecondPool", OrgData["Second_Pool"].ToString());

            if (OrgData.ContainsKey("Command"))
                Item.Add("Command", OrgData["Command"].ToString());

            if (OrgData.ContainsKey("Priority"))
                Item.Add("Priority", OrgData["Priority"].ToString());

            if (OrgData.ContainsKey("Start"))
                Item.Add("StartFrame", OrgData["Start"].ToString());

            if (OrgData.ContainsKey("End"))
                Item.Add("EndFrame", OrgData["End"].ToString());

            if (OrgData.ContainsKey("Packet_Size"))
                Item.Add("PacketSize", OrgData["Packet_Size"].ToString());

            if (OrgData.ContainsKey("Note"))
                Item.Add("Note", OrgData["Note"].ToString());

            return Item;
        }

        #endregion

        #region 獲取變更數據 Get Change Data Object Procedure？？？？？？？？？？？？？
        /// <summary>
        /// 數據對比 Compare change data table content.
        /// </summary>
        /// <param name="CompareData">the source datatable.</param>
        /// <param name="PrimaryKeys">the source datatable primary keys</param>
        /// <returns>System.Data.DataTable</returns>
        public DataTable DataComparison(DataTable CompareData, DataColumn[] PrimaryKeys)
        {
            DataTable result = null;

            // comfirm invaild or empty data ..
            if (CompareData == null || PrimaryKeys.Length == 0)
                return result;

            // check the data table exist in cache data ..
            if (!_CacheData.Tables.Contains(CompareData.TableName))
                // create cache data ..
                _CacheData.Tables.Add(CompareData.Clone());

            // get all compare data row(s) ..
            foreach (DataRow row in CompareData.Rows)
            {
                // get exist data row ..
                DataRow OrgRow = _CacheData.Tables[CompareData.TableName].Rows.Find(row[PrimaryKeys[0]]);

                if (OrgRow == null)
                    // add row to cache data ..
                    _CacheData.Tables[CompareData.TableName].ImportRow(row);
                else
                {
                    for (int i = 0; i < OrgRow.ItemArray.Length; i++)
                    {
                        if (OrgRow.ItemArray[i].ToString().Trim() != row.ItemArray[i].ToString().Trim())
                        {
                            // update the row data ..
                            _CacheData.Tables[CompareData.TableName].LoadDataRow(row.ItemArray, true);
                            break;
                        }
                    }
                }
            }

            foreach (DataRow r in _CacheData.Tables[CompareData.TableName].Rows)
            {
                if (CompareData.Rows.Find(r[_CacheData.Tables[CompareData.TableName].PrimaryKey[0].ColumnName]) == null)
                    r.Delete();
            }

            // 獲取所有數據 get added, deleted, modified, unchanged row(s) state ..
            result = _CacheData.Tables[CompareData.TableName].GetChanges(
                DataRowState.Added | DataRowState.Deleted | DataRowState.Modified | DataRowState.Unchanged);

            // commit all changes ..
            _CacheData.Tables[CompareData.TableName].AcceptChanges();

            return result;
        }
        #endregion

        #region AD身份驗證Validation Active Directory User Procedure
        /// <summary>
        /// Validation active directory user.
        /// </summary>
        /// <param name="UserName">common name.</param>
        /// <param name="Password">common password.</param>
        /// <returns>System.Boolean</returns>
        public bool ADvalid(string UserName, string Password)
        {
            // declare directory entry variable ..
            DirectoryEntry rootEntry = null;
            DirectoryEntry Entry = null;

            bool result = false;

            try
            {
                // binding native active directory ..
                rootEntry = new DirectoryEntry("LDAP://rootDSE");
                string ADPath = "LDAP://" + (string)rootEntry.Properties["defaultNamingContext"].Value;

                // get all active directory object ..
                Entry = new DirectoryEntry(ADPath, UserName, Password, AuthenticationTypes.Secure);

                // search user from active directory ..
                DirectorySearcher searcher = new DirectorySearcher(Entry);
                searcher.Filter = "(SAMAccountName=" + UserName + ")";

                SearchResult res = searcher.FindOne();

                if (res != null)
                    result = true;
            }
            catch (DirectoryServicesCOMException ex)
            {
                string ExceptionMsg = ex.Message + ex.StackTrace;

                // write to log file ..
                EnvLog.Writer(AssemblyInfoClass.ProductInfo, Log.Level.Error, ExceptionMsg, true);
            }
            catch (Exception ex)
            {
                string ExceptionMsg = ex.Message + ex.StackTrace;

                // write to log file ..
                EnvLog.Writer(AssemblyInfoClass.ProductInfo, Log.Level.Error, ExceptionMsg, true);
            }
            finally
            {
                if (Entry != null)
                    Entry.Close();

                if (rootEntry != null)
                    rootEntry.Close();
            }

            return result;
        }
        #endregion

        #region F7300290！！！！切換服務器連接地址Convert Server Connection Procedure
        public bool ConvertConnection(ref Settings EnvSetting)
        {
            bool canConnect = false;
            try
            {
                if (NetworkPing(EnvSetting))
                {
                    canConnect = true;
                }
                else
                {
                    foreach (RenbarServer CurrentServer in this.EnvSvr.GetServers(this.EnvSvr.ServerListFilePath))
                    {
                        if (new ScanPort().Scan(CurrentServer.ServerIP, CurrentServer.ServerPort))
                        {
                            EnvSetting.ServerIpAddress = CurrentServer.ServerIP;
                            EnvSetting.ServerPort = CurrentServer.ServerPort;
                            //
                            canConnect = true;
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string ExceptionMsg = ex.Message + ex.StackTrace;
                EnvLog.Writer(AssemblyInfoClass.ProductInfo, Log.Level.Error, ExceptionMsg, true);
                canConnect = false;
            }

            return canConnect;
        }

        #endregion

        #region 保存、恢復客戶端環境設定值 Save, Restore Client Environment Setting
        /// <summary>
        /// Save client environment setting.
        /// </summary>
        /// <param name="Setting">environment structure.</param>
        public void Save(Settings Setting)
        {
            // create serialize object method ..
            global::RenbarLib.Environment.Service EnvSvr = new RenbarLib.Environment.Service();

            // serializing ..
            EnvSvr.Serialize(EnvSvr.EnvironemtFilePath, Setting);
        }

        /// <summary>
        /// Restore client environment setting.
        /// </summary>
        public Settings Restore()
        {
            // create deserialize object method ..
            global::RenbarLib.Environment.Service EnvSvr = new RenbarLib.Environment.Service();

            if (File.Exists(EnvSvr.EnvironemtFilePath))
            {
                // deserializing ..
                return (Settings)EnvSvr.Deserialize(EnvSvr.EnvironemtFilePath);
            }
            else
                return new Settings();
        }
        #endregion     

        #region F7300290--OK 注册本機信息
        public bool RegLocalMachine(string Description, bool Disconnect, ref Communication EnvComm)//cores=2
        {
            bool result = false;
            HostBase Host = new HostBase();
            DateTime _StartTime = DateTime.Now;
            try
            {
                #region 組合數據

                // check the machine whether has in the machine collection ..
                IDictionary<string, object> Item = new Dictionary<string, object>
                {
                    { "Name", Host.LocalHostName },
                    { "Ip", Host.LocalIpAddress }
                };

                // refresh new request item ..
                Item.Add("Machine_Id", string.Empty);
                Item.Add("IsEnable", true);
                Item.Add("Last_Online_Time", DateTime.Now);
                Item.Add("IsRender", false);
                Item.Add("Note", Description);

                //添加狀態
                if (Disconnect)
                {
                    Item.Add("IsOffLine", true);
                }

                //添加描述
                if (Description != null && Description != string.Empty)
                {
                    Item.Add("Note", Description);
                }
                #endregion

                #region 發送指令

                // declare remote server response object ..
                KeyValuePair<string, object> responseObject;

                if (Item.Count > 2)
                {
                    // package sent data ..
                    IList<object> packaged = EnvComm.Package(Client2Server.CommunicationType.MACHINEINFO, Item);

                    do //？？？？？？？？？？？？？？？？？
                    {
                        // confirm current request status ..
                        if (EnvComm.CanRequest)
                        {
                            // wait for result ..
                            responseObject = EnvComm.Request(packaged);
                            break;
                        }

                        Thread.Sleep(1000);
                    } while (true);
                    if (responseObject.Key.Substring(0, 1) == "+")
                    {
                        result = true;
                    }
                    else
                    {
                        result = false;
                    }
                }
               
                Item.Clear();

                #endregion
            }
            catch (Exception ex)
            {
                string ExceptionMsg = ex.Message + ex.StackTrace;
                // write to log file ..
                EnvLog.Writer(AssemblyInfoClass.ProductInfo, Log.Level.Error, ExceptionMsg, true);
                // change result flag ..
                result = false;
            }
            return result;
        }
        #endregion
        

        /// <summary>
        /// F7300290 判斷請求是否超時
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="timeSpan">TimeSpan in Milliseconds</param>
        /// <returns>true:TimeOut;</returns>
        public bool IsOverTime(DateTime startTime,int timeSpan)
        {
            DateTime EndDate = startTime.AddMilliseconds(timeSpan);
            if (DateTime.Now.Subtract(EndDate).Ticks > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}