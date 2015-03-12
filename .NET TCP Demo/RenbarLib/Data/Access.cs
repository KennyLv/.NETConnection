#pragma warning disable 0420  //使用 #pragma 指示詞將警告視為錯誤，並且啟用或停用警告

#region Using NameSpace
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading;

// import renbar common library namespace ..
using RenbarLib.Environment;
using MySql.Data.MySqlClient;
#endregion

namespace RenbarLib.Data
{
    /// <summary>
    /// Data structure class.
    /// </summary>
    public class DataStructure : Log, IDisposable
    {
        #region 定义变量Declare Global Variable Section
        // create primary access base object ..
        private IBase DataBaseObject = new MysqlBase();

        // create memory database cache object instance ..
        private volatile DataSet MemoryDataBase = new DataSet();
        //volatile 關鍵字表示同時執行的多執行緒可能修改了欄位
        //宣告為 volatile 的欄位不遵從假設單一執行緒存取的編譯器最佳化，這確保最新的值會一直出現在欄位中
        //Volatile 關鍵字只能套用至類別或結構 (Struct) 的欄位

        // declare data view object ..
        private DataView DataViewer = null;
        #endregion

        #region add sqlconnection string
        private string SqlConnectString { get; set; }
        #endregion

        #region 获取数据结构DataStructure Constructor Procedure
        /// <summary>
        /// Primary constructor procedure.
        /// </summary>
        /// <param name="ConnectionString">database connection string.</param>
        public DataStructure(string ConnectionString)
        {
            // assign data ..
            if (ConnectionString != null && ConnectionString != string.Empty)
            {
                // create connection string instance ..
                DataBaseObject.DataConnectionString = ConnectionString;
                this.SqlConnectString = ConnectionString;
                // get base data schema ..
                this.MemoryDataBase = DataBaseObject.GetDataBaseSchema();

                // get base data ..
                DataBaseObject.GetDataBaseContent(ref this.MemoryDataBase);//？？？？？？？？？？？？？？？？？？
            }
        }
        #endregion

        #region 清理资源Clean Up Resource Method Procedure
        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        public void Dispose()
        {
            // clean up base resource ..
            this.Dispose(true);

            // this object will be cleaned up by the dispose method ..
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if server base object should be disposed; otherwise, false.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.DataViewer != null)
                    this.DataViewer.Dispose();

                this.MemoryDataBase.Dispose();
                this.DataBaseObject.Dispose();
            }
        }
        #endregion

        #region 只读属性，获取MemoryDataBase中table行数的总和  Get Memory Data Quantity Property
        /// <summary>
        /// 计算所有MemoryDataBase中table行数的总和。Return current row count.
        /// </summary>
        public int Count
        {
            get
            {
                int __count = 0;

                // search memory all data row count ..
                foreach (DataTable table in MemoryDataBase.Tables)
                    __count += table.Rows.Count;

                return __count;
            }
        }
        #endregion

        #region 筛选数据Find DataRow Event Procedure
        /// <summary>
        /// Find from the target data table of data.
        /// </summary>
        /// <param name="TableName">target data table.</param>
        /// <param name="Items">dictionary object.</param>
        /// <param name="State">data view row state ，DataViewRowState 值是用於從 DataRow 擷取特定資料版本或用於判斷存在什麼版本（新的、已刪除的……）</param>
        /// <returns>System.Data.DataView</returns>
        public DataView FindData(string TableName, IDictionary<string, object> Items, DataViewRowState State)
        {
            // declare filter and sort variable ..
            string
                filter = null,
                sort = null;

            // generate filter and sort conditions ..
            if (Items.Count > 0)
            {
                string
                    __filter = null,
                    __sort = null;

                foreach (KeyValuePair<string, object> kv in Items)
                {
                    __filter += string.Format("{0} = '{1}' And ", kv.Key, kv.Value);
                    __sort += string.Format("{0}, ", kv.Key);
                }
                //注意最后的空格！！！
                filter = __filter.Substring(0, (__filter.Length - 5));
                sort = __sort.Substring(0, (__sort.Length - 2));
            }

            // return match data ..
            return this.FindData(TableName, filter, sort, State);
        }

        /// <summary>
        /// Find from the target data table of data.
        /// </summary>
        /// <param name="TableName">target data table.</param>
        /// <param name="RowFilter">
        /// filter conditions (ex: [column] = 'value').取得或設定用來篩選 DataView 中檢視的資料列的運算式
        /// </param>
        /// <param name="SortRules">
        /// sort rules (ex: [column], [column] ... [column] [Asc|Desc]).取得或設定排序資料行和 DataView 的排序順序
        /// 包含緊接著 "ASC" (遞增) 或 "DESC" (遞減) 的資料行名稱。根據預設，資料行為遞增排序。多個資料行可以使用逗號分隔。
        /// </param>
        /// <param name="RowState">specify the current row status.描述 DataRow 中的資料版本(新的、已刪除的……)</param>
        /// <returns>System.Data.DataView</returns>
        public DataView FindData(string TableName, string RowFilter, string SortRules, DataViewRowState RowState)
        {
            lock (this.MemoryDataBase)
                //使用指定的 DataTable、RowFilter、Sort 和 DataViewRowState，初始化 DataView 類別的新執行個體。
                this.DataViewer = new DataView(this.MemoryDataBase.Tables[TableName].Copy(), RowFilter, SortRules, RowState);

            return this.DataViewer;
        }
        #endregion

        #region Read  Data Event Procedure
        /// <summary>
        /// 讀取MemoryDataBase中資料表結構/資料 Read from the target data table of data.
        /// </summary>
        /// <param name="TableName">target data table.</param>
        /// <returns>System.Data.Datatable</returns>
        public DataTable ReadData(string TableName)
        {
            lock (this.MemoryDataBase)
                //複製這個 DataTable 的結構和資料。
                return this.MemoryDataBase.Tables[TableName].Copy();
        }

        /// <summary>
        /// 讀取MemoryDataBase中資料表結構Read from the target data table of schema.
        /// </summary>
        /// <param name="TableName">target data table.</param>
        /// <returns>System.Data.DataTable</returns>
        public DataTable ReadDataSchema(string TableName)
        {
            // replication memory data table schema ..
            //複製 (Clone) DataTable 的結構，包括所有 DataTable 結構描述和條件約束。
            // Clone 方法建立的新 DataTable 不會包含任何 DataRows
            return this.MemoryDataBase.Tables[TableName].Clone();
        }

         #endregion

        #region Write Data Event Procedure

        /// <summary>
        /// 將緩存數據寫入數據庫 Write data to memory database, and integrate to entity database.
        /// </summary>
        /// <param name="View">data object.緩存數據</param>
        /// <param name="BindText">bind t-sql command text.數據庫操作指令</param>
        public void WriteData(DataSet CacheData, IDictionary<string, MySqlCommand> BindTexts)
        {
            // declare check index flag ..
            bool HasZero = false;
            bool IsDelete = false;
            try
            {
                // update memory database cache object ..
                if (!this.MemoryDataBase.HasErrors)
                {
                    //以CacheData為傳入參數創建新的線程，同步(操作)實體數據庫 
                    //sync entity database ..
                    new Thread(delegate(object ChangingView)
                    {
                        #region sync entity database .

                        // attempts to acquire an exclusive lock on the specified object ..
                        lock (this.MemoryDataBase)
                        {
                            using (MySqlConnection DataConn = new MySqlConnection(this.SqlConnectString))
                            {
                                // declare command, transaction object ..
                                MySqlCommand command = null;
                                MySqlTransaction tran = null;

                                #region 獲取并判斷任務，操作數據庫operat data base
                                try
                                {
                                    // open data connection ..
                                    if (DataConn.State != ConnectionState.Open)
                                    {
                                        DataConn.Open();
                                    }
                                    // create relation command object ..
                                    command = DataConn.CreateCommand();
                                    // begin database transaction ..
                                    tran = DataConn.BeginTransaction();
                                    // assign data transaction object ..
                                    command.Transaction = tran;

                                    //取得 DataSet (包含從前一次載入它或呼叫 AcceptChanges 以來所做的所有變更) 的複本
                                    // get changes data ..
                                    DataSet ChangeData = ((DataSet)ChangingView).GetChanges();
                                    if (ChangeData == null)
                                    {
                                        // get unchange data ..
                                        ChangeData = (DataSet)ChangingView;
                                    }

                                    foreach (DataTable table in ChangeData.Tables)
                                    {
                                        //判斷是否還有“任務”
                                        // confirm the memory job data table index ..
                                        if (table.TableName == "Job" && this.MemoryDataBase.Tables["Job"].Rows.Count == 0)
                                        {
                                            HasZero = true;
                                        }
                                        #region 檢測是否有刪除動作發生Detection Delete Action Event
                                        //
                                        if (this.MemoryDataBase.HasChanges(DataRowState.Deleted) || ChangeData.HasChanges(DataRowState.Deleted))
                                        {
                                            //如果有刪除動作，執行BindTexts
                                            // connect and execute query ..
                                            if (!IsDelete)
                                            {
                                                command.CommandText = BindTexts[table.TableName].CommandText;
                                                command.CommandType = BindTexts[table.TableName].CommandType;
                                                command.Transaction = tran;
                                                command.Connection = DataConn;

                                                for (int j = 0; j < BindTexts[table.TableName].Parameters.Count; j++)
                                                {
                                                    command.Parameters.AddWithValue(BindTexts[table.TableName].Parameters[j].ParameterName, BindTexts[table.TableName].Parameters[j].Value);
                                                }

                                                // exectuing ..
                                                command.ExecuteNonQuery();
                                                IsDelete = true;
                                            }
                                            continue;
                                        }
                                        #endregion

                                        //還有任務
                                        for (int i = 0; i < table.Rows.Count; i++)
                                        {
                                            //// settings command text, and parameter list ..
                                            command.CommandText = BindTexts[table.TableName].CommandText;
                                            command.Parameters.Clear();

                                            for (int j = 0; j < BindTexts[table.TableName].Parameters.Count; j++)
                                            {
                                                command.Parameters.AddWithValue(BindTexts[table.TableName].Parameters[j].ParameterName, BindTexts[table.TableName].Parameters[j].Value);
                                            }

                                            foreach (DataColumn dc in table.Columns)
                                            {
                                                if (dc.ColumnName == "Last_Online_Time" || dc.ColumnName == "Submit_Time" || dc.ColumnName == "Start_Time" || dc.ColumnName == "Finish_Time")
                                                {
                                                    if (string.IsNullOrEmpty(table.Rows[i][dc].ToString()))
                                                    {
                                                        command.Parameters.AddWithValue("?" + dc.ColumnName, table.Rows[i][dc]);
                                                    }
                                                    else
                                                    {
                                                        string lasttime = Convert.ToDateTime(table.Rows[i][dc].ToString()).ToString("yyyy-MM-dd HH:mm:ss");
                                                        command.Parameters.AddWithValue("?" + dc.ColumnName, lasttime);
                                                    }
                                                }
                                                else
                                                {
                                                    command.Parameters.AddWithValue("?" + dc.ColumnName, table.Rows[i][dc]);
                                                }
                                            }


                                            // executing query ..
                                            command.ExecuteNonQuery();
                                        }
                                    }

                                    // suspend constraint rule ..
                                    this.MemoryDataBase.EnforceConstraints = false;

                                    // merge memory data cache ..
                                    this.MemoryDataBase.Merge(ChangeData);

                                    // commit changes ..
                                    this.MemoryDataBase.AcceptChanges();

                                    // commit transaction ..
                                    tran.Commit();

                                    // if job data table index has zero, rearrange integration data table ..
                                    if (HasZero)
                                    {
                                        // refresh ..
                                        DataBaseObject.GetDataBaseContent(ref this.MemoryDataBase);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    string ExceptionMsg = ex.Message + ex.StackTrace;

                                    // write to log file ..
                                    base.Writer(AssemblyInfoClass.ProductInfo, Log.Level.Error, ExceptionMsg, true);

                                    // rollback memory database all changes ..
                                    this.MemoryDataBase.RejectChanges();

                                    if (tran != null)
                                    {
                                        // rollback transaction ..
                                        tran.Rollback();
                                    }
                                }
                                finally
                                {
                                    if (DataConn.State == ConnectionState.Open)
                                    {
                                        // close data connection ..
                                        DataConn.Close();
                                    }
                                    // restore constraint rule ..
                                    this.MemoryDataBase.EnforceConstraints = true;
                                }
                                #endregion

                            }
                            // get full database content ..
                            DataBaseObject.GetDataBaseContent(ref this.MemoryDataBase);

                            //Monitor提供一套機制，同步處理物件的存取
                            //Monitor.PulseAll通知等候取得物件鎖定的所有執行緒。信號送出之後，等候中的執行緒就會被移到就緒佇列
                            //當叫用 PulseAll 的執行緒釋出鎖定，就緒佇列中的下一個執行緒便取得這個鎖定
                            // release the waiting thread ..
                            Monitor.PulseAll(this.MemoryDataBase);
                        }
                        #endregion
                    }).Start((object)CacheData);

                }
            }
            catch (ConstraintException ex)
            {
                string ExceptionMsg = ex.Message + ex.StackTrace;

                // write to log file ..
                base.Writer(AssemblyInfoClass.ProductInfo, Log.Level.Error, ExceptionMsg, true);
            }
            catch (SqlException ex)
            {
                string ExceptionMsg = ex.Message + ex.StackTrace + "\r\n" + ex.ErrorCode.ToString();

                // write to log file ..
                base.Writer(AssemblyInfoClass.ProductInfo, Log.Level.Error, ExceptionMsg, true);
            }
        }

        /// <summary>
        /// 將緩存數據寫入數據庫 Write data to memory database, and integrate to entity database.(針對刪除Pool信息)
        /// </summary>
        /// <param name="View">data object.緩存數據</param>
        /// <param name="BindText">bind t-sql command text.數據庫操作指令</param>
      
        public void WriteDataPool(DataSet CacheData, IDictionary<string, MySqlCommand> BindTexts)
        {
            // declare check index flag ..
            bool HasZero = false;
            bool IsDelete = false;
            try
            {
                // update memory database cache object ..
                if (!this.MemoryDataBase.HasErrors)
                {
                    //以CacheData為傳入參數創建新的線程，同步(操作)實體數據庫 
                    //sync entity database ..
                    new Thread(delegate(object ChangingView)
                    {

                        // attempts to acquire an exclusive lock on the specified object ..
                        lock (this.MemoryDataBase)
                        {
                            using (MySqlConnection DataConn = new MySqlConnection(this.SqlConnectString))
                            {
                                // declare command, transaction object ..
                                MySqlCommand command = null;
                                MySqlTransaction tran = null;

                                #region 獲取并判斷任務，操作數據庫operat data base
                                try
                                {
                                    // open data connection ..
                                    if (DataConn.State != ConnectionState.Open)
                                    {
                                        DataConn.Open();
                                    }
                                    // create relation command object ..
                                    command = DataConn.CreateCommand();
                                    // begin database transaction ..
                                    tran = DataConn.BeginTransaction();
                                    // assign data transaction object ..
                                    command.Transaction = tran;

                                    //取得 DataSet (包含從前一次載入它或呼叫 AcceptChanges 以來所做的所有變更) 的複本
                                    // get changes data ..
                                    DataSet ChangeData = ((DataSet)ChangingView).GetChanges();
                                    if (ChangeData == null)
                                    {
                                        // get unchange data ..
                                        ChangeData = (DataSet)ChangingView;
                                    }

                                    foreach (DataTable table in ChangeData.Tables)
                                    {
                                        //判斷是否還有“任務”
                                        // confirm the memory job data table index ..
                                        if (table.TableName == "Job" && this.MemoryDataBase.Tables["Job"].Rows.Count == 0)
                                        {
                                            HasZero = true;
                                        }
                                        // connect and execute query ..
                                        if (!IsDelete)
                                        {
                                            command.CommandText = BindTexts[table.TableName].CommandText;
                                            command.CommandType = BindTexts[table.TableName].CommandType;
                                            command.Transaction = tran;
                                            command.Connection = DataConn;

                                            for (int j = 0; j < BindTexts[table.TableName].Parameters.Count; j++)
                                            {
                                                command.Parameters.AddWithValue(BindTexts[table.TableName].Parameters[j].ParameterName, BindTexts[table.TableName].Parameters[j].Value);
                                            }

                                            // exectuing ..
                                            command.ExecuteNonQuery();
                                            IsDelete = true;
                                        }


                                    }

                                    // suspend constraint rule ..
                                    this.MemoryDataBase.EnforceConstraints = false;

                                    // merge memory data cache ..
                                    this.MemoryDataBase.Merge(ChangeData);

                                    // commit changes ..
                                    this.MemoryDataBase.AcceptChanges();

                                    // commit transaction ..
                                    tran.Commit();

                                    // if job data table index has zero, rearrange integration data table ..
                                    if (HasZero)
                                    {
                                        // refresh ..
                                        DataBaseObject.GetDataBaseContent(ref this.MemoryDataBase);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    string ExceptionMsg = ex.Message + ex.StackTrace;

                                    // write to log file ..
                                    base.Writer(AssemblyInfoClass.ProductInfo, Log.Level.Error, ExceptionMsg, true);

                                    // rollback memory database all changes ..
                                    this.MemoryDataBase.RejectChanges();

                                    if (tran != null)
                                    {
                                        // rollback transaction ..
                                        tran.Rollback();
                                    }
                                }
                                finally
                                {
                                    if (DataConn.State == ConnectionState.Open)
                                    {
                                        // close data connection ..
                                        DataConn.Close();
                                    }
                                    // restore constraint rule ..
                                    this.MemoryDataBase.EnforceConstraints = true;
                                }
                                #endregion

                            }


                            // get full database content ..
                            DataBaseObject.GetDataBaseContent(ref this.MemoryDataBase);

                            //Monitor提供一套機制，同步處理物件的存取
                            //Monitor.PulseAll通知等候取得物件鎖定的所有執行緒。信號送出之後，等候中的執行緒就會被移到就緒佇列
                            //當叫用 PulseAll 的執行緒釋出鎖定，就緒佇列中的下一個執行緒便取得這個鎖定
                            // release the waiting thread ..
                            Monitor.PulseAll(this.MemoryDataBase);
                        }

                    }).Start((object)CacheData);

                }
            }
            catch (ConstraintException ex)
            {
                string ExceptionMsg = ex.Message + ex.StackTrace;

                // write to log file ..
                base.Writer(AssemblyInfoClass.ProductInfo, Log.Level.Error, ExceptionMsg, true);
            }
            catch (SqlException ex)
            {
                string ExceptionMsg = ex.Message + ex.StackTrace + "\r\n" + ex.ErrorCode.ToString();

                // write to log file ..
                base.Writer(AssemblyInfoClass.ProductInfo, Log.Level.Error, ExceptionMsg, true);
            }
        }

        #endregion

        #region
        /// <summary>
        /// 判斷滿足條件的Job_Group_Id,Machine_Id是否存在
        /// </summary>
        /// <param name="cmd">SQL 語句</param>
        /// <returns>返回執行結果，為0，表示不存在</returns>
        public int IsExistJobMachineId(MySqlCommand cmd)
        {
            MySqlConnection conn = new MySqlConnection(this.SqlConnectString);
            int i = 0;
            try
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }
                cmd.Connection = conn;
                MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                i = Convert.ToInt32(dt.Rows[0][0].ToString());
            }
            catch (Exception ee)
            {
                throw ee;
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                { conn.Close(); }
            }
            return i;
        }

        #endregion

        #region Memory DataBase Trace Procedure
        /// <summary>
        /// 追蹤內存Trace memory database all object.
        /// </summary>
        /// <param name="OutputDriectory">output log file directory.</param>
        public void DataTrace(string OutputDriectory)
        {
            // create directory information class object instance ..
            global::System.IO.DirectoryInfo dirInfo = new System.IO.DirectoryInfo(OutputDriectory);

            // check the directory is exists ..
            if (!dirInfo.Exists)
            {
                // create directory ..
                dirInfo.Create();
            }

            // tracing database data status ..
            string __path = System.IO.Path.Combine(OutputDriectory, string.Format("DataTraceLog-{0}.xml", Service.CustomSysDateTime));
            //將 DataSet 的目前內容寫入為XML 資料，IgnoreSchema設定其不寫入 XSD 結構描述
            //如果沒有資料載入至 DataSet，則不會寫入任何內容
            this.MemoryDataBase.WriteXml(__path, XmlWriteMode.IgnoreSchema);
        }
        #endregion
    }
}