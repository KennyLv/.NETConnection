#region Using NameSpace
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using RenbarLib.Data;

// import renbar server library namespace ..
using RenbarLib.Environment;
using RenbarLib.Network;
using RenbarLib.Network.Protocol;
using RenbarLib.Network.Sockets;
#endregion

namespace RenbarServerGUI
{
    /// <summary>
    /// Renbar server front user interface class.
    /// </summary>
    public class DisplayBase : IDisposable
    {
        #region Declare Global Variable Section
        // declare renbar class object ..
        private DataStructure EnvData = null;
        private Log EnvLog = null;

        // declare temporary data ..
        private DataSet Job_TempData = null;
        private DataTable Machine_TempData = null;

        // declare renbar server display data cache ..
        private volatile DataTable JobDisplay = null;
        private volatile DataTable MachineDisplay = null;

        // declare running signal ..
        private AutoResetEvent[] runEvents = null;
        private static AutoResetEvent[] updateEvents = null;

        // stop running thread flag ..
        private volatile bool requestStop = false;
        #endregion

        #region Display Base Constructor
        /// <summary>
        /// Display base constructor.
        /// </summary>
        /// <param name="Data">renbar memory database object instance.</param>
        /// <param name="Log">renbar log record object instance.</param>
        public DisplayBase(DataStructure Data, Log LogObj)
        {
            // assign relationship object ..
            this.EnvData = Data;
            this.EnvLog = LogObj;

            // initialize signal ..
            this.runEvents = new AutoResetEvent[]
            {
                new AutoResetEvent(false),
                new AutoResetEvent(false)
            };

            updateEvents = new AutoResetEvent[]
            {
                new AutoResetEvent(false),
                new AutoResetEvent(false)
            };

            // create job, machine display data object ..
            JobDisplay = new DataTable("Job");
            MachineDisplay = new DataTable("Machine");

            // load currently data schema ..
            LoadDataSchema();
        }
        
        /// <summary>
        /// load currently machine, job data schema.
        /// </summary>
        private void LoadDataSchema()
        {
            try
            {
                #region Load Cache Data Schema
                // job data ..
                JobDisplay.Columns.Add(new DataColumn("Job_Group_Id", typeof(string)));
                JobDisplay.Columns.Add(new DataColumn("WaitFor", typeof(string)));
                JobDisplay.Columns.Add(new DataColumn("Job_Priority", typeof(ushort)));
                JobDisplay.Columns.Add(new DataColumn("Job_Status", typeof(string)));
                JobDisplay.Columns.Add(new DataColumn("Start_Time", typeof(DateTime)));
                JobDisplay.Columns.Add(new DataColumn("Finish_Time", typeof(DateTime)));
                JobDisplay.Columns.Add(new DataColumn("Submit_Acct", typeof(string)));
                JobDisplay.Columns.Add(new DataColumn("Submit_Time", typeof(DateTime)));

                // define job data primary key ..
                JobDisplay.PrimaryKey = new DataColumn[] { JobDisplay.Columns["Job_Group_Id"] };


                // machine data ..
                MachineDisplay.Columns.Add(new DataColumn("Machine_Id", typeof(string)));
                MachineDisplay.Columns.Add(new DataColumn("HostName", typeof(string)));
                MachineDisplay.Columns.Add(new DataColumn("Ip", typeof(string)));
                MachineDisplay.Columns.Add(new DataColumn("IsEnable", typeof(bool)));
                MachineDisplay.Columns.Add(new DataColumn("Last_Online_Time", typeof(DateTime)));
                MachineDisplay.Columns.Add(new DataColumn("Machine_Status", typeof(string)));
                MachineDisplay.Columns.Add(new DataColumn("Machine_Priority", typeof(ushort)));
                MachineDisplay.Columns.Add(new DataColumn("TCore", typeof(ushort)));
                MachineDisplay.Columns.Add(new DataColumn("UCore", typeof(ushort)));
                MachineDisplay.Columns.Add(new DataColumn("Note", typeof(string)));

                // define machine data primary key ..
                MachineDisplay.PrimaryKey = new DataColumn[] { MachineDisplay.Columns["Machine_Id"] };
                #endregion

                // create refresh data thread instance ..
                Thread JobThread = new Thread(new ThreadStart(this.Job_Update));
                JobThread.IsBackground = true;
                JobThread.Start();

                Thread MachineThread = new Thread(new ThreadStart(this.Machine_Update));
                MachineThread.IsBackground = true;
                MachineThread.Start();
            }
            catch (Exception ex)
            {
                string ExceptionMsg = ex.Message + ex.StackTrace;

                // write to log file ..
                this.EnvLog.Writer(AssemblyInfoClass.ProductInfo, Log.Level.Error, ExceptionMsg, true);
            }
        }
        #endregion

        #region Generate Currently Data Procedure
        /// <summary>
        /// Generate job data cache object.
        /// </summary>
        /// <param name="Name">dataset name.</param>
        /// <returns>System.Data.DataSet</returns>
        private DataSet GenerateJobData(string Name)
        {
            DataSet DataCollection = new DataSet(Name);

            try
            {
                // read all data from memory database ..
                DataCollection.Tables.Add(this.EnvData.ReadData("Job_Group"));
                DataCollection.Tables.Add(this.EnvData.ReadData("Job_Attr"));
                DataCollection.Tables.Add(this.EnvData.ReadData("Job"));
            }
            catch (DataException ex)
            {
                string ExceptionMsg = ex.Message + ex.StackTrace;

                // write to log file ..
                this.EnvLog.Writer(AssemblyInfoClass.ProductInfo, Log.Level.Error, ExceptionMsg, true);
            }

            return DataCollection;
        }

        /// <summary>
        /// Generate job data cache object.
        /// </summary>
        /// <param name="Name">datatable name.</param>
        /// <returns>System.Data.DataTable</returns>
        private DataTable GenerateMachineData(string Name)
        {
            DataTable DataCollection = new DataTable(Name);

            try
            {
                // create data table schema ..
                DataCollection = this.MachineDisplay.Clone();

                // import all machine data ..
                DataTable view_machine_data
                    = this.EnvData.FindData("Machine", null, "Last_Online_Time Desc", DataViewRowState.CurrentRows).ToTable();

                foreach (DataRow row in view_machine_data.Rows)
                {
                    DataRow drow = DataCollection.NewRow();
                    drow["Machine_Id"] = row["Machine_Id"];
                    drow["HostName"] = row["Name"];
                    drow["Ip"] = row["Ip"];
                    drow["IsEnable"] = row["IsEnable"];
                    drow["Last_Online_Time"] = row["Last_Online_Time"];
                    drow["Machine_Status"] = (MachineStatusFlag)Convert.ToUInt16(row["Status"]);
                    drow["Machine_Priority"] = row["Priority"];
                    drow["TCore"] = 0;
                    drow["UCore"] = 0;
                    drow["Note"] = row["Note"];

                    // add to machine data displayer collection ..
                    DataCollection.Rows.Add(drow);
                }
            }
            catch (DataException ex)
            {
                string ExceptionMsg = ex.Message + ex.StackTrace;

                // write to log file ..
                this.EnvLog.Writer(AssemblyInfoClass.ProductInfo, Log.Level.Error, ExceptionMsg, true);
            }

            return DataCollection;
        }
        #endregion

        #region Clean Up Resource Method Procedure
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

                if (this.Job_TempData != null && this.Machine_TempData != null)
                {
                    // clean temporary data and display data cache ..
                    this.Job_TempData.Dispose();
                    this.Machine_TempData.Dispose();
                }

                this.JobDisplay.Dispose();
                this.MachineDisplay.Dispose();

                this.EnvData.Dispose();
            }
        }
        #endregion

        #region Exterior Access Properties
        /// <summary>
        /// Get job display information.
        /// </summary>
        internal DataTable GetJobDisplayInfo
        {
            get
            {
                // create empty job datatable ..
                DataTable __JobsData = new DataTable();

                // wait for data synchronization ..
                if (this.runEvents[0].WaitOne(0))
                    // copy currently data to temporary datatable ..
                    __JobsData = this.JobDisplay.Copy();

                return __JobsData.Copy();
            }
        }

        /// <summary>
        /// Get mahcine display information.
        /// </summary>
        internal DataTable GetMachineDisplayInfo
        {
            get
            {
                // create empty machine datatable ..
                DataTable __MachineData = new DataTable();

                // wait for data synchronization ..
                if (this.runEvents[1].WaitOne(0))
                    // copy currently data to temporary datatable ..
                    __MachineData = this.MachineDisplay.Copy();

                return __MachineData.Copy();
            }
        }

        /// <summary>
        /// Get or set current display data lock status.
        /// </summary>
        internal static bool IsLock
        {
            get;
            set;
        }

        /// <summary>
        /// Get or set currently all render machine status.
        /// </summary>
        internal static DataTable MachineStatus
        {
            get;
            set;
        }

        /// <summary>
        /// Get or set render farm status. (only render class)
        /// </summary>
        internal static DataTable RenderStatus
        {
            get;
            set;
        }
        #endregion

        #region Trigger Update Signal Properties
        /// <summary>
        /// Trigger refresh job data signal.
        /// </summary>
        internal static bool CanJobUpdate
        {
            set
            {
                if (value)
                    updateEvents[0].Set();
            }
        }

        /// <summary>
        /// Trigger refresh machine data signal.
        /// </summary>
        internal static bool CanMachineUpdate
        {
            set
            {
                if (value)
                    updateEvents[1].Set();
            }
        }
        #endregion

        #region Update Data Event Procedure
        /// <summary>
        /// Job data synchronization mechanism.
        /// </summary>
        private void Job_Update()
        {
            do
            {
                try
                {
                    lock (this.JobDisplay)
                    {
                        if (this.Job_TempData != null)
                            // clear old data ..
                            this.Job_TempData.Clear();

                        // change lock flag ..
                        IsLock = true;

                        // reload data ..
                        this.Job_TempData = this.GenerateJobData("JobCache");

                        // add, update items ..
                        foreach (DataRow row in this.Job_TempData.Tables["Job_Group"].Rows)
                        {
                            // mapping refresh display columns ..
                            #region Mapping Job Data Displayer
                            DataRow[] attr_row
                                = this.Job_TempData.Tables["Job_Attr"].Select(string.Format("Job_Group_Id = '{0}'", row["Job_Group_Id"]));

                            if (!this.JobDisplay.Rows.Contains(row["Job_Group_Id"]) && attr_row.Length > 0)
                            {
                                DataRow drow = JobDisplay.NewRow();
                                drow["Job_Group_Id"] = row["Job_Group_Id"];
                                drow["WaitFor"] = attr_row[0]["WaitFor"];
                                drow["Job_Priority"] = attr_row[0]["Priority"];
                                drow["Job_Status"] = (JobStatusFlag)Convert.ToUInt16(row["Status"]);
                                drow["Start_Time"] = row["Start_Time"];
                                drow["Finish_Time"] = row["Finish_Time"];
                                drow["Submit_Acct"] = row["Submit_Acct"];
                                drow["Submit_Time"] = row["Submit_Time"];

                                // add to job data display collection ..
                                this.JobDisplay.Rows.Add(drow);
                            }
                            else
                            {
                                DataRow[] rows = this.JobDisplay.Select(string.Format("Job_Group_Id = '{0}'", row["Job_Group_Id"]));

                                if (rows.Length > 0 && attr_row.Length > 0)
                                {
                                    // update row data ..
                                    rows[0]["WaitFor"] = attr_row[0]["WaitFor"];
                                    rows[0]["Job_Priority"] = attr_row[0]["Priority"];
                                    rows[0]["Job_Status"] = (JobStatusFlag)Convert.ToUInt16(row["Status"]);
                                    rows[0]["Start_Time"] = row["Start_Time"];
                                    rows[0]["Finish_Time"] = row["Finish_Time"];
                                    rows[0]["Submit_Acct"] = row["Submit_Acct"];
                                    rows[0]["Submit_Time"] = row["Submit_Time"];
                                }
                            }
                            #endregion
                        }

                        // delete items ..
                        foreach (DataRow row in this.JobDisplay.Rows)
                        {
                            if (this.Job_TempData.Tables["Job_Group"].Rows.Find(row["Job_Group_Id"]) == null)
                            {
                                row.Delete();
                            }
                        }

                        // commit data changes ..
                        this.JobDisplay.AcceptChanges();

                        // change lock flag ..
                        IsLock = false;

                        // release thread object locked ..
                        this.runEvents[0].Set();
                    }
                }
                catch (Exception ex)
                {
                    string ExceptionMsg = ex.Message + ex.StackTrace;

                    // write to log file ..
                    this.EnvLog.Writer(AssemblyInfoClass.ProductInfo, Log.Level.Error, ExceptionMsg, true);
                }
                Thread.Sleep(200);
            } while (!requestStop && updateEvents[0].WaitOne());
        }

        /// <summary>
        /// Machine data synchronization mechanism.
        /// </summary>
        private void Machine_Update()
        {
            do
            {
                try
                {
                    lock (this.MachineDisplay)
                    {
                        if (this.Machine_TempData != null)
                            // clear old data ..
                            this.Machine_TempData.Clear();

                        // change lock flag ..
                        IsLock = true;

                        // reload data ..
                        this.Machine_TempData = this.GenerateMachineData("MachineCache");

                        // add, update items ..
                        foreach (DataRow row in this.Machine_TempData.Rows)
                        {
                            // mapping refresh display columns ..
                            #region Mapping Machine Data Display
                            if (!this.MachineDisplay.Rows.Contains(row["Machine_Id"]))
                            {
                                DataRow drow = this.MachineDisplay.NewRow();

                                drow["Machine_Id"] = row["Machine_Id"];
                                drow["HostName"] = row["HostName"];
                                drow["Ip"] = row["Ip"];
                                drow["IsEnable"] = row["IsEnable"];
                                drow["Last_Online_Time"] = row["Last_Online_Time"];
                                drow["Machine_Status"] = row["Machine_Status"];
                                drow["Machine_Priority"] = row["Machine_Priority"];
                                drow["TCore"] = 0;
                                drow["UCore"] = 0;
                                drow["Note"] = row["Note"];

                                // add to machine data displayer collection ..
                                this.MachineDisplay.Rows.Add(drow);
                            }
                            else
                            {
                                if (RenderStatus != null)
                                {
                                    DataRow[]
                                        rows = this.MachineDisplay.Select(string.Format("Machine_Id = '{0}'", row["Machine_Id"])),
                                        renders = RenderStatus.Select(string.Format("MachineId = '{0}'", row["Machine_Id"]));

                                    if (rows.Length > 0 && renders.Length > 0)
                                    {
                                        // update row data ..
                                        rows[0]["HostName"] = row["HostName"];
                                        rows[0]["Ip"] = row["Ip"];
                                        rows[0]["IsEnable"] = row["IsEnable"];
                                        rows[0]["Last_Online_Time"] = row["Last_Online_Time"];

                                        if (Convert.ToUInt16(renders[0]["ConnectFail"]) >= 5)
                                            rows[0]["Machine_Status"] = MachineStatusFlag.OFFLINE;
                                        else
                                            rows[0]["Machine_Status"] = row["Machine_Status"];

                                        rows[0]["Machine_Priority"] = row["Machine_Priority"];
                                        rows[0]["TCore"] = renders[0]["TCore"];
                                        rows[0]["UCore"] = renders[0]["UCore"];
                                        rows[0]["Note"] = row["Note"];
                                    }
                                }
                                else
                                {
                                    DataRow[]
                                        rows = this.MachineDisplay.Select(string.Format("Machine_Id = '{0}'", row["Machine_Id"]));

                                    if (rows.Length > 0)
                                    {
                                        // update row data ..
                                        rows[0]["HostName"] = row["HostName"];
                                        rows[0]["Ip"] = row["Ip"];
                                        rows[0]["IsEnable"] = row["IsEnable"];
                                        rows[0]["Last_Online_Time"] = row["Last_Online_Time"];
                                        rows[0]["Machine_Status"] = row["Machine_Status"];
                                        rows[0]["Machine_Priority"] = row["Machine_Priority"];
                                        rows[0]["TCore"] = 0;
                                        rows[0]["UCore"] = 0;
                                        rows[0]["Note"] = row["Note"];
                                    }
                                }
                            }
                            #endregion
                        }

                        // delete items ..
                        foreach (DataRow row in this.MachineDisplay.Rows)
                        {
                            if (this.Machine_TempData.Rows.Find(row["Machine_Id"]) == null)
                                row.Delete();
                        }

                        // commit data changes ..
                        this.MachineDisplay.AcceptChanges();

                        // change lock flag ..
                        IsLock = false;

                        #region Process Render Machine Status For Client
                        if (MachineStatus == null)
                            // create machine table schema ..
                            MachineStatus = this.MachineDisplay.Clone();

                        // clear all row(s) ..
                        MachineStatus.Rows.Clear();

                        // declare filiter condition ..
                        string expression = string.Format("Machine_Status = '{0}'", MachineStatusFlag.RENDER);

                        // copy selected row(s) to machine status property ..
                        this.MachineDisplay.Select(expression).CopyToDataTable(MachineStatus, LoadOption.PreserveChanges);
                        #endregion

                        // release thread object locked ..
                        this.runEvents[1].Set();
                    }
                }
                catch (Exception ex)
                {
                    string ExceptionMsg = ex.Message + ex.StackTrace;

                    // write to log file ..
                    this.EnvLog.Writer(AssemblyInfoClass.ProductInfo, Log.Level.Error, ExceptionMsg, true);
                }
                Thread.Sleep(200);
            } while (!requestStop && updateEvents[1].WaitOne());
        }
        #endregion
    }
}