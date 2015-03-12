#region Using NameSpace
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;

// import renbar server library namespace ..
using RenbarLib.Data;
using RenbarLib.Environment;
using RenbarLib.Network.Protocol;
using RenbarLib.Network.Sockets;

using MySql.Data.MySqlClient;
#endregion

namespace RenbarServerGUI
{
    /// <summary>
    /// Renbar job render class.
    /// </summary>
    public class RenderBase : IDisposable
    {
        #region Declare Global Variable Section
        // declare renbar class object ..
        private DataStructure EnvData = null;
        private Service EnvSvr = null;
        private Log EnvLog = null;

        // declare transport protocol object ..
        private Server2Render RenderObject = new Server2Render();

        // declare dispatch signal lights ..
        private volatile DataTable Signal = null;

        // declare job completed list ..
        private volatile IDictionary<string, int> Completed = null;

        // declare dispatched list ..
        private volatile IList<object> DispatchedList = null;

        // declare pool machine info list ..
        private IDictionary<string, string[]> PoolMachine_Info = null;

        // stop running thread flag ..
        private volatile bool requestStop = false;
        #endregion

        #region Render Base Constructor
        /// <summary>
        /// Render base constructor.
        /// </summary>
        /// <param name="Data">renbar memory database object instance.</param>
        /// <param name="Log">renbar log record object instance.</param>
        public RenderBase(DataStructure Data, Log LogObj)
        {
            // assign relationship object ..
            this.EnvData = Data;
            this.EnvLog = LogObj;

            this.EnvSvr = new Service();

            // declare dispatch signal lights ..
            Signal = new DataTable();
            Signal.Columns.Add(new DataColumn("MachineId", typeof(string)));
            Signal.Columns.Add(new DataColumn("Priority", typeof(ushort)));
            Signal.Columns.Add(new DataColumn("ConnectFail", typeof(ushort)));
            Signal.Columns.Add(new DataColumn("TCore", typeof(ushort)));
            Signal.Columns.Add(new DataColumn("UCore", typeof(ushort)));
            Signal.PrimaryKey = new DataColumn[] { Signal.Columns[0] };

            // create dispatch record list ..
            this.DispatchedList = new List<object>();

            // create job completed dictionary ..
            this.Completed = new Dictionary<string, int>();

            // create pool machine info list ..
            this.PoolMachine_Info = new Dictionary<string, string[]>();

            // create render farm thread instance ..
            Thread RendersThread = new Thread(new ThreadStart(this.Renders));
            RendersThread.Priority = ThreadPriority.Highest;
            RendersThread.IsBackground = true;

            while (ConnectPort > 0)
                Thread.Sleep(100);

            // start render service thread ..
            RendersThread.Start();
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
            }
        }
        #endregion

        #region Connect Port Property
        /// <summary>
        /// Get or set connect render farm port.
        /// </summary>
        internal ushort ConnectPort
        {
            get;
            set;
        }
        #endregion

        #region Report Pool Machine Relations Procedure
        /// <summary>
        /// Report currently pool machine relations.
        /// </summary>
        private void ReportPoolHost()
        {
            lock (this)
            {
                // get current pools info ..
                var pool_query = from machpool in this.EnvData.ReadData("Machine_Pool").AsEnumerable()
                                 group machpool by machpool.Field<string>("Pool_Id") into pool_groups
                                 select pool_groups.ToList();

                foreach (var group in pool_query)
                {
                    // declare machine array count ..
                    string[] machines = new string[group.Count];

                    for (int i = 0; i < group.Count; i++)
                        // assign machine ..
                        machines[i] = ((DataRow)group[i])["Machine_Id"].ToString();

                    if (!this.PoolMachine_Info.ContainsKey(group[0]["Pool_Id"].ToString()))
                        // add to pool machine list ..
                        this.PoolMachine_Info.Add(group[0]["Pool_Id"].ToString(), machines);
                    else
                        // refresh machine list ..
                        this.PoolMachine_Info[group[0]["Pool_Id"].ToString()] = machines;
                }
            }
        }
        #endregion

        #region Render Farm Maintenance Procedure
        /// <summary>
        /// Primary render farm maintenance thread method.
        /// </summary>
        private void Renders()
        {
            do
            {
                // get maintenance quantity ..
                DataView ConnectList = this.EnvData.FindData("Machine",
                    "Status Not In (0, 3) And IsRender = 1 And IsEnable = 1",
                    "Priority Desc",
                    DataViewRowState.Unchanged);

                lock (this.Signal)
                {
                    if (ConnectList.Count > 0)
                    {
                        try
                        {
                            // refresh pool machine list ..
                            this.ReportPoolHost();

                            foreach (DataRowView row in ConnectList)
                            {
                                // package machine info ..
                                IList<object> Info = new List<object>
                                {
                                    row["Machine_Id"],
                                    row["Ip"],
                                };

                                if (Signal.Select(string.Format("MachineId = '{0}'",
                                    row["Machine_Id"])).Length == 0)
                                {
                                    // define row values ..
                                    DataRow _row = Signal.NewRow();
                                    _row["MachineId"] = row["Machine_Id"];
                                    _row["Priority"] = row["Priority"];
                                    _row["ConnectFail"] = 0;
                                    _row["TCore"] = 0;
                                    _row["UCore"] = 0;

                                    // add to signal ..
                                    this.Signal.Rows.Add(_row);

                                    // commit changes ..
                                    this.Signal.AcceptChanges();

                                    // create single thread to maintenance machine ..
                                    Thread MachineThread = new Thread(new ParameterizedThreadStart(this.Maintenance));
                                    MachineThread.SetApartmentState(ApartmentState.MTA);
                                    MachineThread.IsBackground = false;
                                    MachineThread.Priority = ThreadPriority.Normal;
                                    MachineThread.Start(Info);
                                }

                                // delay executing ..
                                Thread.Sleep(500);
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
                            // release the waiting thread ..
                            Monitor.PulseAll(this.Signal);
                        }
                    }
                }

                // delay 10 seconds to next execute check connect quantity ..
                Thread.Sleep(10000);
            } while (!this.requestStop);
        }

        /// <summary>
        /// Signle thread render machine maintenance.
        /// </summary>
        /// <param name="sender">render machine information.</param>
        private void Maintenance(object sender)
        {
            // declare connect machine render workflow service ..
            TcpClientSocket MachineServiceSocket = null;

            try
            {
                // declare exit status flag ..
                bool __stop = false;

                // unpackage machine info ..
                IList<object> Info = (IList<object>)sender;

                // create workflow in, out dictionary ..
                IDictionary<string, object> InDictionary = new Dictionary<string, object>();
                IDictionary<string, object> OutDictionary = new Dictionary<string, object>();

                // create object instance ..
                MachineServiceSocket = new TcpClientSocket(System.Net.IPAddress.Parse(Info[1].ToString()), ConnectPort);
                TcpClientSocket RegistrySocket = null;
                int countx = 0;
                do
                {
                 //   DataView ConnectList1 = this.EnvData.FindData("Machine",
                 //"Status = 0 And IsRender = 1 And IsEnable = 1",
                 //"Priority Desc",
                 //DataViewRowState.Unchanged);
                 //DataTable IsMaintenance=ConnectList1.ToTable();


                    //// get latest signal status ..
                    //DataRow[] machineData = this.Signal.Select(string.Format("MachineId = '{0}'", Info[0]));

                    // create connect remote machine class object instance ..

                    RegistrySocket = new TcpClientSocket(System.Net.IPAddress.Parse(Info[1].ToString()), ConnectPort);
                    if (RegistrySocket.IsConnected)
                    //ScanPort runConnect = new ScanPort();
                    //if (runConnect.Scan(System.Net.IPAddress.Parse(Info[1].ToString()), ConnectPort))
                    {
                        countx = 0;
                        //if (IsMaintenance.Select(string.Format("Machine_Id = '{0}'", Info[0].ToString())).Length == 0)
                        //{
                            #region Primary Communication Service Workflow
                            // try lock object ..
                        if (Monitor.TryEnter(this.Signal))
                        {
                            try
                            {
                                if (!this.RenderBehavior(Server2Render.CommunicationType.ISBUSY,
                                    MachineServiceSocket, null, ref OutDictionary))
                                {
                                    string ExceptionMsg = string.Format(
                                        "machine service loop workflow error, connect machine ip :{0}, thread id: {1}, workflow name: {2}",
                                        Info[1].ToString(), Thread.CurrentThread.ManagedThreadId, "ISBUSY");

                                    this.EnvLog.Writer(AssemblyInfoClass.ProductInfo, Log.Level.Warning, ExceptionMsg, true);
                                }
                                else
                                {
                                    // receive latest workflow return object value ..
                                    DataRow[] rows
                                        = this.Signal.Select(string.Format("MachineId = '{0}'", Info[0]),
                                        "Priority Desc", DataViewRowState.Unchanged);

                                    if (rows.Length > 0)
                                    {
                                        // update row value ..
                                        if (OutDictionary.Count > 0)
                                        {
                                            if (OutDictionary.ContainsKey("TotalCore"))
                                                rows[0]["TCore"] = OutDictionary["TotalCore"];

                                            if (OutDictionary.ContainsKey("UsageCore"))
                                                rows[0]["UCore"] = OutDictionary["UsageCore"];
                                        }
                                        else
                                        {
                                            rows[0]["TCore"] = 0;
                                            rows[0]["UCore"] = 0;
                                        }

                                        // commit changes ..
                                        this.Signal.AcceptChanges();

                                        int
                                            all_core = Convert.ToInt32(rows[0]["TCore"]),
                                            usage_core = Convert.ToInt32(rows[0]["UCore"]);

                                        // update render status and trigger foreground thread ..
                                        DisplayBase.RenderStatus = Signal.Copy();
                                        DisplayBase.CanMachineUpdate = true;

                                        #region Dispatch Jobs
                                        if (rows[0]["MachineId"] == Info[0] && usage_core < all_core)
                                        {
                                            // get free core count ..
                                            int free = all_core - usage_core;

                                            if (0 < free)
                                            {
                                                // settings input dictionary ..
                                                InDictionary.Clear();
                                                InDictionary.Add("Machine_Id", Info[0]);
                                                InDictionary.Add("FreeCore", free);

                                                if (0 == usage_core)
                                                    InDictionary.Add("HasZero", true);
                                                else
                                                    InDictionary.Add("HasZero", false);

                                                if (!this.RenderBehavior(Server2Render.CommunicationType.DISPATCH,
                                                    MachineServiceSocket, InDictionary, ref OutDictionary))
                                                {
                                                    string ExceptionMsg = string.Format(
                                                        "machine service loop workflow error, connect machine id :{0}, thread id: {1}, workflow name: {2}",
                                                        Info[1], Thread.CurrentThread.ManagedThreadId, "DISPATCH");

                                                    //this.EnvLog.Writer(AssemblyInfoClass.ProductInfo, Log.Level.Warning, ExceptionMsg, true);
                                                }
                                                else
                                                {
                                                    // refresh machine core ..
                                                    if (!this.RenderBehavior(Server2Render.CommunicationType.ISBUSY,
                                                        MachineServiceSocket, null, ref OutDictionary))
                                                    {
                                                        string ExceptionMsg = string.Format(
                                                            "machine service loop workflow error, connect machine ip :{0}, thread id: {1}, workflow name: {2}",
                                                            Info[1].ToString(), Thread.CurrentThread.ManagedThreadId, "ISBUSY");

                                                        this.EnvLog.Writer(AssemblyInfoClass.ProductInfo, Log.Level.Warning, ExceptionMsg, true);
                                                    }
                                                    else
                                                    {
                                                        // update row value ..
                                                        if (OutDictionary.Count > 0)
                                                        {
                                                            if (OutDictionary.ContainsKey("TotalCore"))
                                                                rows[0]["TCore"] = OutDictionary["TotalCore"];

                                                            if (OutDictionary.ContainsKey("UsageCore"))
                                                                rows[0]["UCore"] = OutDictionary["UsageCore"];
                                                        }
                                                        else
                                                        {
                                                            rows[0]["TCore"] = 0;
                                                            rows[0]["UCore"] = 0;
                                                        }

                                                        // commit changes ..
                                                        this.Signal.AcceptChanges();
                                                    }
                                                }
                                            }
                                        }
                                        #endregion

                                        if (InDictionary.Count > 0)
                                            InDictionary.Clear();

                                        InDictionary.Add("Host_Id", Info[0].ToString());

                                        #region Get Running Jobs
                                        // get the machine running jobs ..
                                        if (!this.RenderBehavior(Server2Render.CommunicationType.RUNNINGJOBS,
                                            MachineServiceSocket, InDictionary, ref OutDictionary))
                                        {
                                            string ExceptionMsg = string.Format(
                                                "machine service loop workflow error, connect machine ip :{0}, thread id: {1}, workflow name: {2}",
                                                Info[1].ToString(), Thread.CurrentThread.ManagedThreadId, "RUNNINGJOBS");

                                            this.EnvLog.Writer(AssemblyInfoClass.ProductInfo, Log.Level.Warning, ExceptionMsg, true);
                                        }
                                        else
                                        {
                                            if (OutDictionary.Count > 0)
                                                // trigger foreground thread ..
                                                DisplayBase.CanJobUpdate = true;
                                        }
                                        #endregion

                                        #region Get Completed Jobs
                                        // get the machine complete jobs ..
                                        if (!this.RenderBehavior(Server2Render.CommunicationType.COMPLETEDJOBS,
                                            MachineServiceSocket, InDictionary, ref OutDictionary))
                                        {
                                            string ExceptionMsg = string.Format(
                                                "machine service loop workflow error, connect machine ip :{0}, thread id: {1}, workflow name: {2}",
                                                Info[1].ToString(), Thread.CurrentThread.ManagedThreadId, "COMPLETEDJOBS");

                                            this.EnvLog.Writer(AssemblyInfoClass.ProductInfo, Log.Level.Warning, ExceptionMsg, true);
                                        }
                                        else
                                        {
                                            if (OutDictionary.Count > 0)
                                                // trigger foreground thread ..
                                                DisplayBase.CanJobUpdate = true;
                                        }
                                        #endregion
                                    }
                                }
                            }
                            finally
                            {
                                // release the waiting thread ..
                                Monitor.PulseAll(this.Signal);

                                // release locked object ..
                                Monitor.Exit(this.Signal);
                            }

                        }
                        #endregion

                        #region Render Maintenance Flag Chnage Event
                        
                        // try lock object wait for 0.5 second ..
                        if (Monitor.TryEnter(this.Signal, 500))
                        {
                            try
                            {
                                // get maintenance quantity ..
                                DataView MachineOff = this.EnvData.FindData("Machine",
                                    string.Format("Status = 0 And Machine_Id = '{0}'", Info[0]),
                                    null,
                                    DataViewRowState.Unchanged);

                                if (MachineOff.Count > 0)
                                {
                                    // receive latest workflow return object value ..
                                    DataRow[] rows
                                        = this.Signal.Select(string.Format("MachineId = '{0}'", Info[0]),
                                        "MachineId Asc", DataViewRowState.Unchanged);

                                    if (rows.Length > 0)
                                    {
                                        // update row value ..
                                        rows[0]["TCore"] = 0;
                                        rows[0]["UCore"] = 0;

                                        // commit changes ..
                                        this.Signal.AcceptChanges();

                                        // update render status and trigger foreground thread ..
                                        DisplayBase.RenderStatus = Signal.Copy();
                                        DisplayBase.CanMachineUpdate = true;

                                        DataRow[] machineData = Signal.Select(string.Format("MachineId = '{0}'", Info[0]));

                                        if (machineData.Length > 0)
                                        {
                                            // remove the machine signal lights ..
                                            machineData[0].Delete();

                                            // commit changes ..
                                            this.Signal.AcceptChanges();

                                            // change status ..
                                            __stop = true;
                                        }
                                    }
                                }
                            }
                            finally
                            {
                                // release the waiting thread ..
                                Monitor.PulseAll(this.Signal);

                                // release locked object ..
                                Monitor.Exit(this.Signal);
                            }
                        }
                        
                        #endregion
                    }
                    #region
                    else
                    {
                        #region Connect Render Machine Fail Event
                        // try lock object ..
                        lock (this.Signal)
                        {
                            // get latest signal status ..
                            DataRow[] machineData = this.Signal.Select(string.Format("MachineId = '{0}'", Info[0]));

                            if (machineData.Length > 0)
                            {

                                //if (Convert.ToUInt16(machineData[0]["ConnectFail"]) >= 5)
                                if (countx >= 3)
                                {
                                    // get maintenance quantity ..
                                    DataTable Machine = this.EnvData.FindData("Machine",
                                        string.Format("Machine_Id = '{0}'", Info[0]),
                                        null,
                                        DataViewRowState.Unchanged).ToTable();

                                    if (Machine.Rows.Count > 0)
                                    {
                                        // update row value ..
                                        machineData[0]["TCore"] = 0;
                                        machineData[0]["UCore"] = 0;

                                        // write offline flag ..
                                        Machine.Rows[0]["Status"] = Convert.ToUInt16(MachineStatusFlag.OFFLINE);

                                        // accept changes ..
                                        Machine.AcceptChanges();

                                        // create data cache data ..
                                        DataSet Data = new DataSet();

                                        // merge tables to dataset ..
                                        Data.Merge(Machine);

                                        // integration to memory data ..
                                        this.SyncData(Data);

                                        // wait for data sync ..
                                        Thread.Sleep(500);

                                        // update render status and trigger foreground thread ..
                                        DisplayBase.RenderStatus = Signal.Copy();
                                        DisplayBase.CanMachineUpdate = true;

                                        // remove the machine signal lights ..
                                        machineData[0].Delete();

                                        // commit changes ..
                                        this.Signal.AcceptChanges();

                                        // change status ..
                                        __stop = true;

                                        string ExceptionMsg = string.Format("can't connect operation of machine. (file: render.cs, id: {0}, ip: {1})", Info[0], Info[1]);

                                        // write to log file ..
                                        this.EnvLog.Writer(AssemblyInfoClass.ProductInfo, Log.Level.Warning, ExceptionMsg, true);
                                    }
                                }
                                else
                                {
                                    // increment fail count ..
                                    machineData[0]["ConnectFail"] = Convert.ToUInt16(machineData[0]["ConnectFail"]);
                                    countx++;
                                    // commit changes ..
                                    this.Signal.AcceptChanges();
                                }

                                // release the waiting thread ..
                                Monitor.PulseAll(this.Signal);
                            }
                        }
                        #endregion
                    }
                    #endregion
                    // confirm request exit ..
                    if (__stop)
                        break;

                } while (!this.requestStop);
            }
            catch (Exception ex)
            {
                string ExceptionMsg = ex.Message + ex.StackTrace;

                // write to log file ..
                EnvLog.Writer(AssemblyInfoClass.ProductInfo, Log.Level.Error, ExceptionMsg, true);
            }
            finally
            {    // close connect ..
                if (MachineServiceSocket != null)
                {
                    MachineServiceSocket.Close();
                }
            }
        }
        #endregion

        #region Render Farm Process Behavior Event Procedure
        /// <summary>
        /// Integrate job data synchronization mechanism.
        /// </summary>
        private void SyncData(DataSet Ds)
        {
            lock (this)
            {
                if (Ds.Tables.Count > 0)
                {
                    // create text command dictionary ..
                    IDictionary<string, MySqlCommand> TextCommands = new Dictionary<string, MySqlCommand>();

                    // create sync data ..
                    DataSet SyncData = Ds.Copy();

                    foreach (DataTable table in SyncData.Tables)
                    {
                        #region Bind DataBase Command
                        string cmd = string.Empty;
                        switch (table.TableName)
                        {
                            case "job_group":
                                cmd = " Update Job_Group Set ";
                                cmd += " Submit_Machine = ?Submit_Machine, Submit_Acct = ?Submit_Acct, Submit_Time = ?Submit_Time, ";
                                cmd += " First_Pool = ?First_Pool, Second_Pool = ?Second_Pool, Status = ?Status, ";
                                cmd += " Start_Time = ?Start_Time, Finish_Time = ?Finish_Time, Note = ?Note ";
                                cmd += " Where ";
                                cmd += " Job_Group_Id = ?Job_Group_Id ";
                                break;

                            case "job_attr":
                                cmd = " Update Job_Attr Set ";
                                cmd += " Project = ?Project, Start = ?Start, End = ?End, Packet_Size = ?Packet_Size, ";
                                cmd += " Proc_Type = ?Proc_Type, WaitFor = ?WaitFor, Priority = ?Priority, ";
                                cmd += " ABName = ?ABName, ABPath = ?ABPath, ABUpdateOnly = ?ABUpdateOnly ";
                                cmd += " Where ";
                                cmd += " Job_Group_Id = ?Job_Group_Id ";
                                break;

                            case "job":
                                cmd = " Update Job Set ";
                                cmd += " Command = ?Command, Proc_Machine = ?Proc_Machine, Start_Time = ?Start_Time, ";
                                cmd += " Finish_Time = ?Finish_Time, OutputLog = ?OutputLog ";
                                cmd += " Where ";
                                cmd += " Job_Id = ?Job_Id ";
                                break;

                            case "machine":
                                cmd = " Update Machine Set ";
                                cmd += " Name = ?Name, Ip = ?Ip, IsEnable = ?IsEnable, Last_Online_Time = ?Last_Online_Time, ";
                                cmd += " IsRender = ?IsRender, Status = ?Status, Priority = ?Priority, Note = ?Note ";
                                cmd += " Where ";
                                cmd += " Machine_Id = ?Machine_Id ";
                                break;
                        }
                        #endregion

                        MySqlCommand command = new MySqlCommand(cmd);
                        command.CommandType = CommandType.Text;

                        // add commands ..
                        TextCommands.Add(table.TableName, command);
                    }

                    // write to primary database ..
                    this.EnvData.WriteData(SyncData, TextCommands);
                }
            }
        }

        /// <summary>
        /// Access render machine communication behavior.
        /// </summary>
        /// <param name="Type">communication type.</param>
        /// <param name="Socket">transmission of communication object.</param>
        /// <param name="ProcData">dataset object.</param>
        /// <param name="Dispatched">dispatch collection.</param>
        /// <param name="InputObject">input object.</param>
        /// <param name="OutputObject">output object.</param>
        /// <returns>System.Boolean</returns>
        private bool RenderBehavior(Server2Render.CommunicationType Type, TcpClientSocket Socket,
            IDictionary<string, object> InputObject, ref IDictionary<string, object> OutputObject)
        {
            // define default variable ..
            bool result = true;

            //switch (Type)
            switch (Type)
            {
                #region Get Completed Jobs Workflow
                case Server2Render.CommunicationType.COMPLETEDJOBS:
                    try
                    {
                        // declare dictionary result object ..
                        KeyValuePair<string, object> __returnObject = new KeyValuePair<string, object>();

                        // package and send object to remote machine ..
                        Socket.Send(this.EnvSvr.Serialize(RenderObject.Package(Server2Render.CommunicationType.COMPLETEDJOBS)));

                        // receive remote machine data object ..
                        object received = null;
                        if ((received = this.EnvSvr.Deserialize(Socket.Receive())) != null)
                            // convert correct object type ..
                            __returnObject = (KeyValuePair<string, object>)received;

                        // check the client has return object data is nullable type ..
                        if (__returnObject.Value == null)
                            result = false;
                        else
                        {
                            // declare empty dataset, and copy render return data value ..
                            DataSet Data = new DataSet();
                            DataTable renderData = ((DataTable)__returnObject.Value).Copy();

                            // clear old data ..
                            if (OutputObject.Count > 0)
                                OutputObject.Clear();

                            // return completed rows count ..
                            OutputObject.Add("Completed", renderData.Rows.Count);

                            // analysis remote object ..
                            foreach (DataRow row in renderData.Rows)
                            {
                                // define row key ..
                                string
                                    id = row["Job_Group_Id"].ToString().Trim(),
                                    sid = row["Job_Id"].ToString().Trim(),
                                    exp1 = string.Format("Job_Group_Id = '{0}'", id),
                                    exp2 = string.Format("Job_Id = {0} And Finish_Time Is Null", sid);

                                lock (this.DispatchedList)
                                {
                                    // remove dispatched record ..
                                    if (this.DispatchedList.Contains(row["Job_Id"]))
                                        this.DispatchedList.Remove(row["Job_Id"]);
                                }

                                // find data from memory database ..
                                DataTable
                                    job_group = this.EnvData.FindData("Job_Group", exp1, null, DataViewRowState.Unchanged).ToTable(),
                                    all_jobs = this.EnvData.FindData("Job", exp1, null, DataViewRowState.Unchanged).ToTable(),
                                    job = this.EnvData.FindData("Job", exp2, null, DataViewRowState.Unchanged).ToTable();

                                if (job_group.Rows.Count > 0 && all_jobs.Rows.Count > 0 && job.Rows.Count > 0)
                                {
                                    if (row["Status"] != null && row["Status"].ToString() != string.Empty)
                                    {
                                        if (Convert.ToUInt16(row["Status"]) == Convert.ToUInt16(JobStatusFlag.ERROR))
                                        {
                                            job.Rows[0]["Proc_Machine"] = InputObject["Host_Id"];
                                            job.Rows[0]["Start_Time"] = row["Start_Time"];
                                            job.Rows[0]["Finish_Time"] = row["Finish_Time"];
                                        }
                                    }
                                    else
                                        // update sub job finish data ..
                                        job.Rows[0]["Finish_Time"] = row["Finish_Time"];

                                    if (!this.Completed.ContainsKey(id))
                                        // add to completed collection ..
                                        this.Completed.Add(id, all_jobs.Rows.Count);

                                    if (this.Completed.ContainsKey(id))
                                        // reduction count quantity ..
                                        this.Completed[id]--;
                                    else
                                        continue;

                                    IList<string> cid = new List<string>();
                                    foreach (KeyValuePair<string, int> kv in this.Completed)
                                    {
                                        // update primary job finish data ..
                                        if (kv.Value == 0)
                                        {
                                            cid.Add(kv.Key);

                                            if (row["Status"] != null && row["Status"].ToString() != string.Empty)
                                            {
                                                if (Convert.ToUInt16(row["Status"]) == Convert.ToUInt16(JobStatusFlag.ERROR))
                                                {
                                                    job_group.Rows[0]["Start_Time"] = row["Start_Time"];
                                                    job_group.Rows[0]["Finish_Time"] = row["Finish_Time"];
                                                    job_group.Rows[0]["Status"] = Convert.ToUInt16(JobStatusFlag.ERROR);
                                                    job_group.Rows[0]["Note"] = "Can't operation the job, please confirm the job command attribute.";
                                                }
                                            }
                                            else
                                            {
                                                job_group.Rows[0]["Status"] = Convert.ToUInt16(JobStatusFlag.COMPLETED);
                                                job_group.Rows[0]["Finish_Time"] = row["Finish_Time"];
                                            }
                                        }
                                    }

                                    // clear completed collection ..
                                    foreach (string s in cid)
                                    {
                                        if (this.Completed.ContainsKey(s))
                                            this.Completed.Remove(s);
                                    }

                                    // merge data ..
                                    Data.Merge(job_group);
                                    Data.Merge(job);
                                }
                            }

                            if (Data.Tables.Count > 0)
                            {
                                // accpet all changes ..
                                Data.AcceptChanges();

                                // integration to job data collection ..
                                SyncData(Data);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        string ExceptionMsg = ex.Message + ex.StackTrace;

                        // write to log file ..
                        EnvLog.Writer(AssemblyInfoClass.ProductInfo, Log.Level.Error, ExceptionMsg, true);

                        // change result ..
                        result = false;
                    }
                    break;
                #endregion

                #region Dispatch Job Workflow
                case Server2Render.CommunicationType.DISPATCH:
                    try
                    {
                        DataSet Data = new DataSet();

                        // declare dictionary result object ..
                        KeyValuePair<string, object> __returnObject = new KeyValuePair<string, object>();

                        // create dispatch data list ..
                        IList<Dispatch> Dispatchs = new List<Dispatch>();

                        // analysis waitFor, priority attribute and mapping machine event ..
                        if (InputObject.ContainsKey("Machine_Id") && InputObject.ContainsKey("FreeCore")
                            && InputObject.ContainsKey("HasZero"))
                        {
                            #region Filtration waitFor Procedure

                            // search queuing status rows ..
                            DataTable job_group_data
                                = this.EnvData.FindData("Job_Group", "Status In (1, 6)", null,
                                DataViewRowState.Unchanged).ToTable();

                            #region Getting waitFor Data List
                            // create queuing waitFor list ..
                            IList<string> waitForList = new List<string>();

                            // search waitfor ..
                            foreach (DataRow wrow in job_group_data.Rows)
                            {
                                #region Analysis Job Machine Pool
                                DataTable
                                    machine_pool_data1 = null,
                                    machine_pool_data2 = null;

                                machine_pool_data1 = this.EnvData.FindData("Machine_Pool",
                                    string.Format("Pool_Id = '{0}'", wrow["First_Pool"]), null,
                                    DataViewRowState.Unchanged).ToTable();

                                if (!DBNull.Value.Equals(wrow["Second_Pool"]))
                                {
                                    machine_pool_data2 = this.EnvData.FindData("Machine_Pool",
                                        string.Format("Pool_Id = '{0}'", wrow["Second_Pool"]), null,
                                        DataViewRowState.Unchanged).ToTable();
                                }

                                bool flag = false;
                                foreach (DataRow __row1 in machine_pool_data1.Rows)
                                {
                                    if (__row1["Machine_Id"].Equals(InputObject["Machine_Id"]))
                                    {
                                        flag = true;
                                        break;
                                    }
                                }

                                if (machine_pool_data2 != null)
                                {
                                    foreach (DataRow __row2 in machine_pool_data2.Rows)
                                    {
                                        if (__row2["Machine_Id"] == InputObject["Machine_Id"])
                                        {
                                            flag = true;
                                            break;
                                        }
                                    }
                                }

                                if (!flag)
                                    continue;
                                #endregion

                                DataTable waitfor_data = this.EnvData.FindData("Job_Attr",
                                    string.Format("WaitFor Is Not Null And Job_Group_Id = '{1}'",
                                    DBNull.Value,
                                    wrow["Job_Group_Id"]),
                                    null, DataViewRowState.Unchanged).ToTable();

                                // add to list ..
                                if (waitfor_data.Rows.Count > 0)
                                {
                                    DataRow[] validrow = job_group_data.Select(string.Format(
                                        "Status = {0} And Job_Group_Id = '{1}'",
                                        Convert.ToUInt16(JobStatusFlag.QUEUING),
                                        waitfor_data.Rows[0]["WaitFor"]));
                                    //
                                    if (validrow.Length > 0)
                                    {
                                        if (!waitForList.Contains(waitfor_data.Rows[0]["Job_Group_Id"].ToString()))
                                            waitForList.Add(waitfor_data.Rows[0]["Job_Group_Id"].ToString());
                                    }
                                }
                            }
                            #endregion

                            // create sort list ..
                            IDictionary<string, ushort> FiltrationJobs = new Dictionary<string, ushort>();

                            foreach (DataRow grow in job_group_data.Rows)
                            {
                                #region Confirm Pool Machines
                                if (this.PoolMachine_Info.Count == 0)
                                {
                                    string ExceptionMsg = "can't find pool machine relations.";

                                    // write to log file ..
                                    EnvLog.Writer(AssemblyInfoClass.ProductInfo, Log.Level.Error, ExceptionMsg, true);

                                    break;
                                }
                                else
                                {
                                    string
                                        first = grow["First_Pool"].ToString().Trim(),
                                        second = string.Empty;

                                    if (grow["Second_Pool"] != null && grow["Second_Pool"].ToString() != string.Empty)
                                        second = grow["Second_Pool"].ToString().Trim();

                                    // check first pool ..
                                    if (this.PoolMachine_Info.ContainsKey(first) && !this.PoolMachine_Info[first].Contains<string>(InputObject["Machine_Id"].ToString().Trim()))
                                    {
                                        if (!string.IsNullOrEmpty(second))
                                        {
                                            // check second pool ..
                                            if (!this.PoolMachine_Info[second].Contains<string>(InputObject["Machine_Id"].ToString().Trim()))
                                                continue;
                                        }
                                        else
                                            continue;
                                    }
                                }
                                #endregion

                                // whether check this job exists waitFor list ..
                                if (waitForList.Contains(grow["Job_Group_Id"].ToString()))
                                    continue;

                                // find current job of queue status ..
                                DataView __attr = this.EnvData.FindData("Job_Attr",
                                    string.Format("Job_Group_Id = '{0}'", grow["Job_Group_Id"]),
                                    null,
                                    DataViewRowState.Unchanged);

                                // add to ready dispatch list ..
                                if (__attr.Count > 0)
                                {
                                    if (!FiltrationJobs.ContainsKey(__attr[0]["Job_Group_Id"].ToString()))
                                        FiltrationJobs.Add(__attr[0]["Job_Group_Id"].ToString(),
                                            Convert.ToUInt16(__attr[0]["Priority"]));
                                }
                            }
                            #endregion

                            #region Getting Sub Job, And Setting Dispatch Structure

                            // declare stop and package count flag ..
                            bool stopDispatch = false;
                            ushort
                                retryCount = 0,
                                packageCount = 0;

                            // clear old data ..
                            if (OutputObject.Count > 0)
                                OutputObject.Clear();

                            // return dispatch rows count ..
                            OutputObject.Add("Dispatch", FiltrationJobs.Count);

                            foreach (KeyValuePair<string, ushort> kv in FiltrationJobs.OrderByDescending(job => job.Value))
                            {
                                string
                                    filter1 = string.Format("Job_Group_Id = '{0}'", kv.Key),
                                    filter2 = string.Format("Job_Group_Id = '{0}' And Proc_Machine Is Null", kv.Key);

                                // find job attribute ..
                                DataTable job_attr_data
                                    = this.EnvData.FindData("Job_Attr", filter1, null, DataViewRowState.Unchanged).ToTable();

                                // find sub job ..
                                DataTable job_data
                                    = this.EnvData.FindData("Job", filter2, "Job_Id Asc", DataViewRowState.Unchanged).ToTable();

                                if (job_data.Rows.Count > 0)
                                {
                                    if (job_attr_data.Rows[0]["Proc_Type"].ToString().Trim() == "Client")
                                    {
                                        if (!Convert.ToBoolean(InputObject["HasZero"]))
                                            continue;

                                        InputObject["FreeCore"] = 1;
                                    }

                                    // package dispatch data ..
                                    foreach (DataRow row in job_data.Rows)
                                    {
                                        if (packageCount == Convert.ToUInt16(InputObject["FreeCore"]))
                                        {
                                            stopDispatch = true;
                                            break;
                                        }

                                        // analysis command args ..
                                        string
                                            cmd = string.Empty,
                                            args = string.Empty;


                                        int idx = row["Command"].ToString().IndexOf('-');

                                        if (idx > 0)
                                        {
                                            cmd = row["Command"].ToString().Substring(0, idx);
                                            args = row["Command"].ToString().Substring(idx, (row["Command"].ToString().Length) - idx);
                                        }

                                        lock (this.DispatchedList)
                                        {


                                            //if (this.DispatchedList.Contains(row["Job_Id"]))
                                            //    continue;
                                            //else
                                            //{
                                            //add to dispatched collection ..
                                            this.DispatchedList.Add(row["Job_Id"]);

                                            Dispatchs.Add(new Dispatch()
                                            {
                                                Job_Group_Id = new Guid(kv.Key),
                                                Job_Id = Convert.ToInt32(row["Job_Id"]),
                                                //Job_Id = Convert.ToString(row["Job_Id"]),
                                                Proc_Type = job_attr_data.Rows[0]["Proc_Type"].ToString().Trim(),
                                                Command = cmd.Trim(),
                                                Args = args.Trim()
                                            });

                                            packageCount++;
                                            //}
                                        }
                                    }

                                    if (stopDispatch)
                                        break;
                                }
                                else
                                {
                                    retryCount++;

                                    if (retryCount > 5)
                                    {
                                        DataTable errorRow = this.EnvData.FindData(
                                            "Job_Group", filter1, null, DataViewRowState.Unchanged).ToTable();

                                        if (errorRow.Rows.Count > 0)
                                        {
                                            // update fail info ..
                                            errorRow.Rows[0]["Status"] = Convert.ToUInt16(JobStatusFlag.ERROR);
                                            errorRow.Rows[0]["Start_Time"] = DateTime.Now;
                                            errorRow.Rows[0]["Finish_Time"] = DateTime.Now;
                                            errorRow.Rows[0]["Note"] = "Can't dispatch job to render farm, please confirm the job attribute and sub job data.";

                                            // commit changes ..
                                            errorRow.AcceptChanges();

                                            // assign to dataset ..
                                            Data.Tables.Add(errorRow);

                                            // integration to job data collection ..
                                            SyncData(Data);
                                        }
                                    }
                                    else
                                        continue;
                                }
                            }
                            #endregion
                        }

                        if (Dispatchs.Count <= 0)
                            return false;

                        // anew package dispatch structure ..
                        IDictionary<string, object> jobs = new Dictionary<string, object>
                        {
                            { "Dispatcher", Dispatchs }
                        };
                        if (Socket.IsConnected)
                        {
                            // package and send object to remote machine ..
                            Socket.Send(this.EnvSvr.Serialize(RenderObject.Package(Server2Render.CommunicationType.DISPATCH, jobs)));

                            // receive remote machine data object ..
                            object received = null;
                            if ((received = this.EnvSvr.Deserialize(Socket.Receive())) != null)
                                // convert correct object type ..
                                __returnObject = (KeyValuePair<string, object>)received;

                            // check the client has return object data is nullable type ..
                            if (__returnObject.Key.Substring(0, 1) == "-")
                                result = false;
                            else
                                result = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        string ExceptionMsg = ex.Message + ex.StackTrace;

                        // write to log file ..
                        EnvLog.Writer(AssemblyInfoClass.ProductInfo, Log.Level.Error, ExceptionMsg, true);

                        // change result ..
                        result = false;
                    }
                    break;
                #endregion

                #region Get Render IsBusy Status Workflow
                case Server2Render.CommunicationType.ISBUSY:
                    try
                    {
                        // declare list result object ..
                        KeyValuePair<string, object> __returnObject = new KeyValuePair<string, object>();

                        // package and send object to remote machine ..
                        Socket.Send(this.EnvSvr.Serialize(RenderObject.Package(Server2Render.CommunicationType.ISBUSY)));

                        // receive remote machine data object ..
                        object received = null;
                        if ((received = this.EnvSvr.Deserialize(Socket.Receive())) != null)
                            // convert correct object type ..
                            __returnObject = (KeyValuePair<string, object>)received;

                        // check the client has return object data is nullable type ..
                        if (__returnObject.Value == null)
                            result = false;
                        else
                        {
                            // create new instance ..
                            OutputObject = new Dictionary<string, object>();

                            // receive and assign remote object ..
                            foreach (KeyValuePair<string, object> kv in (IDictionary<string, object>)__returnObject.Value)
                                OutputObject.Add(kv.Key, kv.Value);
                        }
                    }
                    catch (Exception ex)
                    {
                        string ExceptionMsg = ex.Message + ex.StackTrace;

                        // write to log file ..
                        EnvLog.Writer(AssemblyInfoClass.ProductInfo, Log.Level.Error, ExceptionMsg, true);

                        // change result ..
                        result = false;
                    }
                    break;
                #endregion

                #region Get Running Jobs Workflow
                case Server2Render.CommunicationType.RUNNINGJOBS:
                    try
                    {
                        // declare dictionary result object ..
                        KeyValuePair<string, object> __returnObject = new KeyValuePair<string, object>();

                        // package and send object to remote machine ..
                        Socket.Send(this.EnvSvr.Serialize(RenderObject.Package(Server2Render.CommunicationType.RUNNINGJOBS)));

                        // receive remote machine data object ..
                        object received = null;
                        if ((received = this.EnvSvr.Deserialize(Socket.Receive())) != null)
                            // convert correct object type ..
                            __returnObject = (KeyValuePair<string, object>)received;

                        // check the client has return object data is nullable type ..
                        if (__returnObject.Value == null)
                            result = false;
                        else
                        {
                            // declare empty dataset, and copy render return data value ..
                            DataSet Data = new DataSet();
                            DataTable renderData = ((DataTable)__returnObject.Value).Copy();

                            // clear old data ..
                            if (OutputObject.Count > 0)
                                OutputObject.Clear();

                            // return running rows count ..
                            OutputObject.Add("Running", renderData.Rows.Count);

                            // analysis remote object ..
                            foreach (DataRow row in renderData.Rows)
                            {
                                // define row key ..
                                string
                                    id = row["Job_Group_Id"].ToString().Trim(),
                                    sid = row["Job_Id"].ToString().Trim(),
                                    exp1 = string.Format("Job_Group_Id = '{0}'", id),
                                    exp2 = string.Format("Job_Id = '{0}'", sid);

                                // find data from memory database ..
                                DataTable
                                    job_group = this.EnvData.FindData("Job_Group", exp1, null, DataViewRowState.Unchanged).ToTable(),
                                    job = this.EnvData.FindData("Job", exp2, null, DataViewRowState.Unchanged).ToTable();

                                if (job_group.Rows.Count > 0 && job.Rows.Count > 0)
                                {
                                    if (job_group.Rows[0]["Start_Time"] != null)
                                    {
                                        // write primary datetime data ..
                                        job_group.Rows[0]["Status"] = Convert.ToUInt16(JobStatusFlag.PROCESSING);
                                        job_group.Rows[0]["Start_Time"] = row["Start_Time"];
                                    }

                                    if (job.Rows[0]["Start_Time"] != null)
                                    {
                                        // write sub job datetime data ..
                                        job.Rows[0]["Proc_Machine"] = InputObject["Host_Id"];
                                        job.Rows[0]["Start_Time"] = row["Start_Time"];
                                    }

                                    if (row["Render_Output"] != null)
                                        // update last output log ..
                                        job.Rows[0]["OutputLog"] = row["Render_Output"];

                                    // merge data ..
                                    Data.Merge(job_group);
                                    Data.Merge(job);
                                }
                            }

                            if (Data.Tables.Count > 0)
                            {
                                // accept all changes ..
                                Data.AcceptChanges();

                                // integration to job data collection ..
                                SyncData(Data);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        string ExceptionMsg = ex.Message + ex.StackTrace;

                        // write to log file ..
                        EnvLog.Writer(AssemblyInfoClass.ProductInfo, Log.Level.Error, ExceptionMsg, true);

                        // change result ..
                        result = false;
                    }
                    break;
                #endregion
            }

            return result;
        }
        #endregion
    }
}