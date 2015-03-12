#region Using NameSpace
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;

// import renbar server library namespace ..
using RenbarLib.Data;
using RenbarLib.Environment;
using RenbarLib.Network.Protocol;
using RenbarLib.Extension.Alienbrain;
using MySql.Data.MySqlClient;
#endregion

namespace RenbarServerGUI
{
    /// <summary>
    /// Primary alienbrain extension module calss.
    /// </summary>
    public class AlienbrainBase : IDisposable
    {
        #region Declare Global Variable Section
        // declare renbar class object ..
        private DataStructure EnvData = null;
        private Log EnvLog = null;

        // declare ab project list ..
        private IList<string> ProjectList = null;

        // declare alienbrain work datatable ..
        private volatile DataTable WorkData = null;

        // stop running thread flag ..
        private volatile bool requestStop = false;
        #endregion

        #region Alienbrain Base Constructor
        /// <summary>
        /// Alienbrain base constructor.
        /// </summary>
        /// <param name="Data">renbar memory database object instance.</param>
        /// <param name="Log">renbar log record object instance.</param>
        /// <param name="Info">alienbrain properties information.</param>
        public AlienbrainBase(DataStructure Data, Log LogObj, AlienbrainInfo Info)
        {
            // assign relationship object ..
            this.EnvData = Data;
            this.EnvLog = LogObj;

            // create alienbrain project list ..
            this.ProjectList = new List<string>();

            // create alienbrain work data ..
            WorkData = new DataTable("ABWorkData");
            WorkData.Columns.Add(new DataColumn("Id", typeof(string)));
            WorkData.Columns.Add(new DataColumn("Name", typeof(string)));
            WorkData.Columns.Add(new DataColumn("Path", typeof(string)));
            WorkData.Columns.Add(new DataColumn("IsUpdateOnly", typeof(bool)));
            WorkData.Columns.Add(new DataColumn("Priority", typeof(int)));

            // set primary key ..
            WorkData.PrimaryKey = new DataColumn[] { WorkData.Columns[0] };

            // 
            Thread abWork_Thread = new Thread(new ParameterizedThreadStart(this.DoWork));
            abWork_Thread.Priority = ThreadPriority.BelowNormal;
            abWork_Thread.IsBackground = true;
            abWork_Thread.Start(Info);
        }
        #endregion

        #region 清理資源 Clean Up Resource Method Procedure
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
                // stop running threads ..
                this.requestStop = true;

                // logout all alienbrain project ..
                foreach (string s in this.ProjectList)
                {
                    AbLib.Instance.Logout(s);
                }
                
                this.EnvData.Dispose();
            }
        }
        #endregion

        #region Alienbrain Work Event Procedure
        /// <summary>
        /// Dowork background thread method.
        /// </summary>
        /// <param name="parameters">alienbrain properties parameter.</param>
        private void DoWork(object parameters)
        {
            do
            {
                try
                {
                    // declare filter conditions ..//CHECKING = 3,
                    string expression1 = string.Format("Status = '{0}'", Convert.ToUInt16(JobStatusFlag.CHECKING));

                    // find update status jobs ...從Job_Group中篩選Checking的Unchanged數據
                    DataTable
                        primary_view = this.EnvData.FindData("Job_Group", expression1, string.Empty, DataViewRowState.Unchanged).ToTable();

                    foreach (DataRow row in primary_view.Rows)
                    {
                        string expression2 = string.Format("Job_Group_Id = '{0}'", row["Job_Group_Id"]);

                        // find high priority jobs ...從Job_Attr中篩選於上次篩選ID相同的數據
                        DataView second_view = this.EnvData.FindData("Job_Attr", expression2, "Priority Desc", DataViewRowState.Unchanged);

                        if (second_view.Count > 0)
                        {
                            if (!WorkData.Rows.Contains(row["Job_Group_Id"]))
                            {
                                DataRow _row = WorkData.NewRow();
                                _row["Id"] = row["Job_Group_Id"];
                                _row["Name"] = second_view[0]["ABName"];
                                _row["Path"] = second_view[0]["ABPath"];
                                _row["IsUpdateOnly"] = Convert.ToBoolean( second_view[0]["ABUpdateOnly"]);
                                _row["Priority"] = second_view[0]["Priority"];
                                WorkData.Rows.Add(_row);
                            }
                        }
                    }

                    // add to alienbrain work list ..
                    foreach (DataRow row in WorkData.AsEnumerable().OrderByDescending(pri => pri.Field<int>("Priority")))//按Priority降序
                    {
                        // analysis connect object ..
                        AlienbrainInfo info = (AlienbrainInfo)parameters;

                        // confirm the project whether exist in project list ..
                        if (!this.ProjectList.Contains(row["Name"].ToString()))
                        {
                            this.ProjectList.Add(row["Name"].ToString());

                            // login project ..
                            AbLib.Instance.Login(info.Server, row["Name"].ToString(), info.User, info.Password);

                            // set working path ..
                            AbLib.Instance.SetWorkingPath(string.Format(@"{0}\{1}", info.WorkPath, row["Name"].ToString()));
                        }

                        // dependency and get latest ..
                        this.GetLatest(row["Path"].ToString(), row["Id"].ToString());

                        // update status ..
                        string exp = string.Format("Job_Group_Id = '{0}'", row["Id"]);
                        DataRow[] memory_row = this.EnvData.ReadData("Job_Group").Select(exp);

                        if (memory_row.Length > 0)
                        {
                            if (Convert.ToBoolean(row["IsUpdateOnly"]))
                                memory_row[0]["Status"] = Convert.ToUInt16(JobStatusFlag.UPDATEONLY);
                            else
                                memory_row[0]["Status"] = Convert.ToUInt16(JobStatusFlag.QUEUING);

                            // sync data to database ..
                            this.SyncData(memory_row[0]);
                        }
                    }
                }
                catch (Exception ex)
                {
                    string ExceptionMsg = ex.Message + ex.StackTrace;

                    // write to log file ..
                    EnvLog.Writer(AssemblyInfoClass.ProductInfo, Log.Level.Error, ExceptionMsg, true);
                }
                finally
                {
                    if (WorkData.Rows.Count > 0)
                        WorkData.Rows.Clear();
                }

                Thread.Sleep(5000);
            } while (!requestStop);
        }

        /// <summary>
        /// Dependency refrernce file, and get latest file.
        /// </summary>
        /// <param name="Path">alienbrain namespace file path.</param>
        private void GetLatest(string Path, string wId)
        {
            // update status ..
            string exp = string.Format("Job_Group_Id = '{0}'", wId);
            DataRow[] memory_row = this.EnvData.ReadData("Job_Group").Select(exp);

            if (memory_row.Length > 0)
            {
                // change status ..
                memory_row[0]["Status"] = Convert.ToUInt16(JobStatusFlag.GETLATEST);

                // sync data to database ..
                this.SyncData(memory_row[0]);
            }

            // get primary file and revise alienbrain workspace ..
            AbLib.Instance.GetLatest(Path, true, true);

            // dependency and get latest file ..
            this.Recursive(Path, true);
        }

        /// <summary>
        /// Dpendency refrernce file, and get latest files. (use recursive遞歸)
        /// </summary>
        /// <param name="abFile">use alienbrain workspace namespace.</param>
        private void Recursive(string abFile, bool revise)
        {
            string[] relations = AbLib.Instance.DependencyGet(abFile, revise);

            foreach (string s in relations)
            {
                // get latest ..
                AbLib.Instance.GetLatest(s, true, false);

                // dependency file ..
                this.Recursive(s, false);
            }
        }
        #endregion

        #region 同步數據 Synchronization Data Event Procedure
        /// <summary>
        /// Synchronization currently status to database.
        /// </summary>
        /// <param name="UpdateRow">update data row.</param>
        private void SyncData(DataRow UpdateRow)
        {
            if (UpdateRow == null)
                return;

            // declare update temp dataset ..
            DataSet ds = new DataSet();

            // read job group data schema ..
            DataTable table = this.EnvData.ReadDataSchema("Job_Group");

            // create new data row ..
            DataRow row = table.NewRow();
            row.ItemArray = UpdateRow.ItemArray;

            // add to wirte temp dataset ..
            table.Rows.Add(row);
            ds.Tables.Add(table);

            // bind command text ..
            string cmd = " Update Job_Group Set ";
            cmd += " Submit_Machine = ?Submit_Machine, Submit_Acct = ?Submit_Acct, Submit_Time = ?Submit_Time, ";
            cmd += " First_Pool = ?First_Pool, Second_Pool = ?Second_Pool, Status = ?Status, ";
            cmd += " Start_Time = ?Start_Time, Finish_Time = ?Finish_Time, Note = ?Note ";
            cmd += " Where ";
            cmd += " Job_Group_Id = ?Job_Group_Id ";
            
            MySqlCommand command = new MySqlCommand(cmd);
            command.CommandType = CommandType.Text;

            // create text command dictionary ..
            IDictionary<string, MySqlCommand> TextCommands = new Dictionary<string, MySqlCommand>
            {
                { "Job_Group", command }
            };

            // add commands ..
            TextCommands.Add(table.TableName, command);
            // write to primary database ..
            this.EnvData.WriteData(ds, TextCommands);

            // wait for write data finish ..
            Thread.Sleep(100);

            // refresh job data list ..
            DisplayBase.CanJobUpdate = true;
        }
        #endregion
    }

    /// <summary>
    ///  Alienbrain 信息結構  Alienbrain info base structure...User\Password\Server
    /// </summary>
    public struct AlienbrainInfo
    {
        #region Alienbrain Info Properties
        /// <summary>
        /// Get or set connect alienbrain user name.
        /// </summary>
        public string User
        {
            get;
            set;
        }

        /// <summary>
        /// Get or set connect alienbrain user password.
        /// </summary>
        public string Password
        {
            get;
            set;
        }

        /// <summary>
        /// Get or set connect server.
        /// </summary>
        public string Server
        {
            get;
            set;
        }

        /// <summary>
        /// Get or set save file(s) location.
        /// </summary>
        public string WorkPath
        {
            get;
            set;
        }
        #endregion
    }
}