#region Using NameSpace
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Threading;

// import renbar server library namespace ..
using RenbarLib.Data;
using RenbarLib.Environment;
using RenbarLib.Network;
using RenbarLib.Network.Protocol;
using RenbarLib.Network.Sockets;

using MySql.Data.MySqlClient;
#endregion

namespace RenbarServerGUI
{
    /// <summary>
    /// Renbar server protocol base class.
    /// </summary>
    public class ProtocolBase : IDisposable
    {
        #region Declare Global Variable Section
        // declare renbar class object ..
        private HostBase EnvHostBase = new HostBase();
        private DataStructure EnvData = null;
        private Service EnvSvr = null;
        private Log EnvLog = null;
        //***************************************************************************(F7300253)
        //store 50  address&ip
        int n = 100;
        int m = 99;
        string Newadd = string.Empty;
        string[] address = new string[100];
        //***************************************************************************

        // declare transport protocol object ..
        private Client2Server ClientObject = new Client2Server();
        private ServerReponse ServerObject = new ServerReponse();

        // stop running thread flag ..
        private volatile bool requestStop = false;
        #endregion

        #region Protocol Base Constructor
        /// <summary>
        /// Protocol base constructor.
        /// </summary>
        /// <param name="Data">renbar memory database object instance.</param>
        /// <param name="Log">renbar log record object instance.</param>
        public ProtocolBase(DataStructure Data, Log LogObj)
        {
            // assign relationship object ..
            this.EnvData = Data;
            this.EnvLog = LogObj;

            // create renbar environment object instance ..
            this.EnvSvr = new Service();

            // create render farm thread instance ..
            Thread ClientsThread = new Thread(new ThreadStart(this.Request));
            ClientsThread.Priority = ThreadPriority.Highest;
            ClientsThread.IsBackground = false;

            while (ListenPort > 0)
                Thread.Sleep(500);

            // start render service thread ..
            ClientsThread.Start();
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
                this.EnvData.Dispose();
            }
        }
        #endregion

        #region Listen Port Property
        /// <summary>
        /// Get or set listen port.
        /// </summary>
        internal ushort ListenPort
        {
            get;
            set;
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

        #region Max History Property
        /// <summary>
        /// Get or set max job history record.
        /// </summary>
        internal ushort MaxHistory
        {
            get;
            set;
        }
        #endregion

        #region Render Farm Delete Behaivor Event Procedure
        internal IList<string> DeleteFunc(Hashtable TableList)
        {
            Server2Render RenderObject = new Server2Render();
            IList<string> Deleted = new List<string>();

            foreach (DictionaryEntry de in TableList)
            {
                IDictionary<string, object> Items = new Dictionary<string, object>
                {
                    { "Machine_Id", de.Key }
                };

                // parse remote machine info ..
                DataView view = this.EnvData.FindData("Machine", Items, DataViewRowState.Unchanged);

                if (view.Count > 0)
                {
                    // parse ip address ..
                    IPAddress Addr = IPAddress.Parse(view[0]["Ip"].ToString().Trim());

                    // declare connect machine render delete workflow service ..
                    TcpClientSocket MachineServiceSocket = null;

                    try
                    {
                        // create object instance ..
                        MachineServiceSocket = new TcpClientSocket(Addr, ConnectPort);

                        if (MachineServiceSocket.IsConnected)
                        {
                            IDictionary<string, object> Item = new Dictionary<string, object>
                            {
                                { "Jobs", de.Value }
                            };

                            // sent delete job ..
                            MachineServiceSocket.Send(this.EnvSvr.Serialize(RenderObject.Package(Server2Render.CommunicationType.DELETEJOB, Item)));

                            // declare dictionary result object ..
                            KeyValuePair<string, object> __returnObject = new KeyValuePair<string, object>();

                            // receive remote machine data object ..
                            object received = null;
                            if ((received = this.EnvSvr.Deserialize(MachineServiceSocket.Receive())) != null)
                                // convert correct object type ..
                                __returnObject = (KeyValuePair<string, object>)received;

                            // check the client has error or return object data is nullable type ..
                            if (__returnObject.Key.Substring(0, 1) == "+")
                                Deleted.Add(de.Key.ToString());
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
                        if (MachineServiceSocket != null)
                            // close connect ..
                            MachineServiceSocket.Close();
                    }
                }
            }

            // cleanup deleted machine data ..
            TableList.Clear();

            return Deleted;
        }
        #endregion

        #region Accept Client Request Maintenance Procedure
        /// <summary>
        /// Primary accept client request maintenance thread method.
        /// </summary>
        private void Request()
        {
            // declare client request communication service ..
            TcpServerSocket ClientServiceSocket = null;

            try
            {
                // create object instance ..
                ClientServiceSocket = new TcpServerSocket(this.EnvHostBase.LocalIpAddress, this.ListenPort);

                //update  offline machine stauts(F7300253)防止非正常离线情况发生
                //Thread isNotOffline = new Thread(new ThreadStart(judge));
                //isNotOffline.Start();
                do
                {

                    // confirm connect requests ..
                    if (ClientServiceSocket.Pending())
                    {
                        int k = 0;
                        // accept a new connect request ..
                        global::System.Net.Sockets.TcpClient handle = ClientServiceSocket.AcceptListen();

                        //***************************************************************************(F7300253)
                        //限制同一个IP的isconnect次数(Max is 6)
                        IPEndPoint IP = (IPEndPoint)handle.Client.RemoteEndPoint;
                        Newadd = IP.Address.ToString();

                        for (int i = n - 1; i >= 0; i--)
                        {
                            if (Newadd == address[i])
                            {
                                k++;
                                if (k > 6)
                                {
                                    i = -1;
                                }
                            }
                        }
                        if (k < 6)
                        {
                            address[m] = Newadd;

                            if (m == 0)
                            {
                                for (int i = n - 1; i >= 0; i--)
                                {
                                    address[i] = null;
                                }
                                m = 99;
                            }
                            else
                            {
                                m--;
                            }
                            //***************************************************************************
                            handle.ReceiveTimeout = 10000;
                            handle.SendTimeout = 10000;

                            ThreadPool.QueueUserWorkItem(new WaitCallback(this.Protocol), handle);
                        }
                    }

                    // delay 0.2 second to next listening ..
                    Thread.Sleep(200);

                } while (!requestStop);
            }
            catch (Exception ex)
            {
                string ExceptionMsg = ex.Message + ex.StackTrace;

                // write to log file ..
                EnvLog.Writer(AssemblyInfoClass.ProductInfo, Log.Level.Error, ExceptionMsg, true);
            }
            finally
            {
                if (ClientServiceSocket != null)
                    // clean server socket resource ..
                    ClientServiceSocket.Close();
            }
        }

        #region update  offline machine stauts(F7300253)
        //*********************************************************************(F7300253)
        /// <summary>
        /// update  offline machine stauts
        /// </summary>
        //private void judge()
        //{
        //    try
        //    {
        //        do
        //        {
        //            DataView NotOffline = this.EnvData.FindData("Machine",
        //            "Status=1", "Priority Desc", DataViewRowState.Unchanged);
        //            if (Monitor.TryEnter(NotOffline))
        //            {
        //                lock (address)
        //                {
        //                    if (NotOffline.Count > 0)
        //                    {
        //                        foreach (DataRowView row in NotOffline)
        //                        {
        //                            // package machine info ..
        //                            IList<object> Info2 = new List<object>
        //                        {
        //                            row["Machine_Id"],
        //                            row["Ip"],
        //                        };

        //                            TcpClientSocket RegistrySocket2 = new TcpClientSocket(System.Net.IPAddress.Parse(Info2[1].ToString()), ConnectPort);
        //                            if (RegistrySocket2.IsConnected)
        //                            {
        //                                //
        //                            }
        //                            else
        //                            {
        //                                int tcount = 0;
        //                                for (int i = n - 1; i >= 0; i--)
        //                                {
        //                                    if (Info2[1].ToString() == address[i])
        //                                    {
        //                                        address[i] = null;
        //                                        tcount++;
        //                                        if (tcount == 6)
        //                                        {
        //                                            break;
        //                                        }
        //                                    }
        //                                }
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //            Monitor.Exit(NotOffline);

        //            Thread.Sleep(5000);

        //        } while (!requestStop);
        //    }
        //    catch (Exception ex)
        //    {
        //        string ExceptionMsg = ex.Message + ex.StackTrace;

        //        // write to log file ..
        //        EnvLog.Writer(AssemblyInfoClass.ProductInfo, Log.Level.Error, ExceptionMsg, true);
        //    }
        //}
        //*********************************************************************
        #endregion


        /// <summary>
        /// Client2Server signle protocol event procedure.
        /// </summary>
        /// <param name="ClientObject">accept client object.</param>
        private void Protocol(object ClientObject)
        {
            // create single client stream object ..
            SingleClient client = new SingleClient(ClientObject);
            
            // declare receive remote object ..
            object received = null;

            try
            {
                // define default communication enumeration ..
                Client2Server.CommunicationType ActionHeader = Client2Server.CommunicationType.NONE;
                IDictionary<string, object> QueueItems = new Dictionary<string, object>();

                while (!requestStop)
                {
                    #region

                    received = this.EnvSvr.Deserialize(client.Receive());
                    if (received != null)
                    {
                        // unpackage remote object data ..
                        this.ClientObject.UnPackage((IList<object>)received, out ActionHeader, out QueueItems);

                        // declare server response object list interface ..
                        KeyValuePair<string, object> __returnObject = new KeyValuePair<string, object>();
          
                        // clear before receive data ..
                        received = null;

                        #region All GUI Work……
                        switch (ActionHeader)
                        {
                            #region --F7300290--OK-- A/U:Machine Info Case Workflow
                            case Client2Server.CommunicationType.MACHINEINFO:
                                try
                                {
                                    // read machine table data ..
                                    DataView view_machine_data = this.EnvData.FindData(
                                        "Machine",
                                        string.Format("Name = '{0}' And Ip = '{1}' And IsRender='{2}'",
                                        new object[] { QueueItems["Name"], QueueItems["Ip"], Convert.ToInt32(bool.Parse(QueueItems["IsRender"].ToString())) }),
                                        "Name, Ip",
                                        DataViewRowState.Unchanged);

                                    // define table collection ..
                                    DataSet mds = new DataSet();

                                    // define temporary data structure ..
                                    DataTable mtable = this.EnvData.ReadDataSchema("Machine");

                                    // create new row ..
                                    DataRow mrow = mtable.NewRow();
                                    #region 填充數據
                                    // mapping column ..
                                    foreach (KeyValuePair<string, object> kv in QueueItems)
                                    {
                                        if (mtable.Columns.Contains(kv.Key))
                                        {
                                            mrow[kv.Key] = kv.Value;
                                        }
                                    }

                                    // check the data column is allow DbNull type ..
                                    foreach (DataColumn dc in view_machine_data.ToTable().Columns)
                                    {
                                        if (!dc.AllowDBNull)
                                        {
                                            if (!QueueItems.ContainsKey(dc.ColumnName) && view_machine_data.ToTable().Rows.Count > 0)
                                            {
                                                mrow[dc.ColumnName] = view_machine_data.ToTable().Rows[0][dc.ColumnName];
                                            }
                                        }
                                    }

                                    #region 設定機器狀態值

                                    // decision machine type ..
                                    if (QueueItems.ContainsKey("IsRender"))
                                    {
                                        if (!bool.Parse(QueueItems["IsRender"].ToString()))
                                        {
                                            mrow["Status"] = (ushort)MachineStatusFlag.CLIENT;

                                            if (DBNull.Value == mrow["Priority"])
                                            {
                                                mrow["Priority"] = 0;
                                            }
                                        }
                                        else
                                        {
                                            mrow["Status"] = (ushort)MachineStatusFlag.RENDER;
                                            //***********************************************************(F7300253)
                                            global::System.Net.Sockets.TcpClient handleClear = (global::System.Net.Sockets.TcpClient)ClientObject;
                                            IPEndPoint IPClear = (IPEndPoint)handleClear.Client.RemoteEndPoint;
                                            string NetIP = IPClear.Address.ToString();
                                            for (int i = n - 1; i >= 0; i--)
                                            {
                                                if (NetIP == address[i])
                                                {
                                                    address[i] = null;
                                                }
                                            }
                                            //************************************************************
                                            if (DBNull.Value == mrow["Priority"])
                                            {
                                                mrow["Priority"] = 20;
                                            }
                                        }
                                    }

                                    // decision the dictionary exist maintenance flag ..
                                    if (QueueItems.ContainsKey("IsMaintenance") && bool.Parse(QueueItems["IsMaintenance"].ToString()))
                                    {
                                        mrow["Status"] = (ushort)MachineStatusFlag.MAINTENANCE;
                                        //***********************************************************(F7300253)
                                        global::System.Net.Sockets.TcpClient handleClear = (global::System.Net.Sockets.TcpClient)ClientObject;
                                        IPEndPoint IPClear = (IPEndPoint)handleClear.Client.RemoteEndPoint;
                                        string NetIP = IPClear.Address.ToString();
                                        for (int i = n - 1; i >= 0; i--)
                                        {
                                            if (NetIP == address[i])
                                            {
                                                address[i] = null;
                                            }
                                        }
                                        //************************************************************
                                    }

                                    // decision the dictionary exist offline flag ..
                                    if (QueueItems.ContainsKey("IsOffLine"))
                                    {
                                        if (bool.Parse(QueueItems["IsOffLine"].ToString()))
                                        {
                                            mrow["Status"] = (ushort)MachineStatusFlag.OFFLINE;
                                            //***********************************************************(F7300253)
                                            global::System.Net.Sockets.TcpClient handleClear = (global::System.Net.Sockets.TcpClient)ClientObject;
                                            IPEndPoint IPClear = (IPEndPoint)handleClear.Client.RemoteEndPoint;
                                            string NetIP = IPClear.Address.ToString();
                                            for (int i = n - 1; i >= 0; i--)
                                            {
                                                if (NetIP == address[i])
                                                {
                                                    address[i] = null;
                                                }
                                            }
                                            //************************************************************
                                        }
                                    }
                                    #endregion
                                    #endregion

                                    // create text command dictionary ..
                                    string cmd = string.Empty;

                                    IDictionary<string, MySqlCommand> TextCommands = new Dictionary<string, MySqlCommand>();

                                    #region Refresh Machine Data 更新/插入Machine數據！
                                    // update machine data ..
                                    if (view_machine_data.Count > 0)
                                    {
                                        // get machine id column ..
                                        mrow["Machine_Id"] = view_machine_data[0].DataView[0]["Machine_Id"];

                                        // complete added ..
                                        mtable.Rows.Add(mrow);

                                        // add to table collection ..
                                        mds.Tables.Add(mtable);

                                        // bind database command ..
                                        cmd = " Update Machine Set ";
                                        cmd += " Name = ?Name, Ip = ?Ip, IsEnable = ?IsEnable, Last_Online_Time = ?Last_Online_Time, ";
                                        cmd += " Status = ?Status, Priority = ?Priority, Note = ?Note ";
                                        cmd += " Where Machine_Id = ?Machine_Id ";
                                    }
                                    // add new machine data ..
                                    else
                                    {
                                        // create new machine id ..
                                        mrow["Machine_Id"] = Guid.NewGuid().ToString().ToUpper();

                                        // complete added ..
                                        mtable.Rows.Add(mrow);

                                        // add to table collection ..
                                        mds.Tables.Add(mtable);

                                        // bind database command ..
                                        cmd = " Insert Into Machine ";
                                        cmd += " (Machine_Id, Name, Ip, IsEnable, Last_Online_Time, IsRender, Status, Priority, Note) ";
                                        cmd += " Values ";
                                        cmd += " (?Machine_Id, ?Name, ?Ip, ?IsEnable, ?Last_Online_Time, ?IsRender, ?Status, ?Priority, ?Note) ";
                                    }
                                    #endregion

                                    MySqlCommand command = new MySqlCommand();
                                    command.CommandText = cmd;
                                    command.CommandType = CommandType.Text;

                                    // add commands ..
                                    TextCommands.Add(mtable.TableName, command);

                                    // sync data ..
                                    this.EnvData.WriteData(mds, TextCommands);

                                    // response result ..
                                    __returnObject = ServerObject.Response(ServerReponse.ResponseSign.PLUSOK);

                                    // wait for write data finish ..
                                    Thread.Sleep(100);
                                    // refresh machine data list ..
                                    DisplayBase.CanMachineUpdate = true;

                                    //
                                    mtable.Dispose();
                                    view_machine_data.Dispose();
                                    mds.Dispose();

                                }
                                catch (Exception ex)
                                {
                                    // response error result ..
                                    __returnObject = ServerObject.Response(ServerReponse.ResponseSign.MINUSERR);

                                    string ExceptionMsg = ex.Message + ex.StackTrace;

                                    // write to log file ..
                                    EnvLog.Writer(AssemblyInfoClass.ProductInfo, Log.Level.Error, ExceptionMsg, true);
                                }
                                break;
                            #endregion

                            #region Delete Machine Info Case Workflow ！！！！未使用！！！！
                            case Client2Server.CommunicationType.DELETEMACHINEINFO:
                                try
                                {
                                    // read machine table data ..
                                    DataView delete_machine_data = this.EnvData.FindData(
                                        "Machine",
                                        string.Format("Name = '{0}' And Ip = '{1}' Or Machine_Id = '{2}'",
                                        new object[] { QueueItems["Name"], QueueItems["Ip"], QueueItems["Machine_Id"] }),
                                        "Machine_Id",
                                        DataViewRowState.Unchanged);

                                    // get machine id ..
                                    string id = delete_machine_data.ToTable().Rows[0]["Machine_Id"].ToString();

                                    // read machine pool table data ..
                                    DataView delete_machine_pool_data = this.EnvData.FindData(
                                        "Machine_Pool",
                                        string.Format("Machine_Id = '{0}'",
                                        new object[] { id }),
                                        "Machine_Id",
                                        DataViewRowState.Unchanged);

                                    // delete data from pool table and machine pool table ..
                                    while (delete_machine_pool_data.Count != 0)
                                        delete_machine_pool_data.Delete(0);
                                    while (delete_machine_data.Count != 0)
                                        delete_machine_data.Delete(0);

                                    // bind database command ..
                                    string cmd = " select count(*)  From Machine_Pool";
                                    cmd += "  Inner Join Machine On Machine_Pool.Machine_Id = Machine.Machine_Id";
                                    cmd += " Where Machine.Machine_Id In (" + id + ")";
                                    MySqlCommand command = new MySqlCommand(cmd);
                                    //command.Parameters.AddWithValue("?Machine_Id", id);

                                    if (this.EnvData.IsExistJobMachineId(command) > 0)
                                    {
                                        cmd = "  Delete From Machine_Pool Where Machine_Id In (" + id + ") ;";
                                        cmd += "  Delete From Machine Where Machine_Id In (" + id + ") ";
                                    }
                                    else
                                    {
                                        cmd = "  Delete From Machine Where Machine_Id In (" + id + ") ";
                                    }

                                    // define table collection ..
                                    DataSet pds = new DataSet();

                                    // create text command dictionary ..
                                    IDictionary<string, MySqlCommand> TextCommands = new Dictionary<string, MySqlCommand>();

                                    // add to table collection ..
                                    pds.Tables.Add(delete_machine_data.ToTable());
                                    pds.Tables.Add(delete_machine_pool_data.ToTable());
                                    command = new MySqlCommand(cmd);
                                    // add commands ..
                                    //command.Parameters.AddWithValue("?Machine_Id", id);
                                    TextCommands.Add(delete_machine_data.ToTable().TableName, command);
                                    TextCommands.Add(delete_machine_pool_data.ToTable().TableName, command);

                                    // sync data ..
                                    this.EnvData.WriteData(pds, TextCommands);

                                    // response result ..
                                    __returnObject = ServerObject.Response(ServerReponse.ResponseSign.PLUSOK);

                                    // wait for write data finish ..
                                    Thread.Sleep(100);

                                    // refresh machine data list ..
                                    DisplayBase.CanMachineUpdate = true;

                                    delete_machine_data.Dispose();
                                    delete_machine_pool_data.Dispose();
                                    pds.Dispose();
                                }
                                catch (Exception ex)
                                {
                                    // response error result ..
                                    __returnObject = ServerObject.Response(ServerReponse.ResponseSign.MINUSERR);

                                    string ExceptionMsg = ex.Message + ex.StackTrace;

                                    // write to log file ..
                                    EnvLog.Writer(AssemblyInfoClass.ProductInfo, Log.Level.Error, ExceptionMsg, true);
                                }
                                break;
                            #endregion

                            #region --F7300290--OK--View Machine Info Case Workflow
                            case Client2Server.CommunicationType.VIEWMACHINEINFO:
                                try
                                {
                                    // find match data ..
                                    DataView view_machine_data = this.EnvData.FindData("Machine", QueueItems, DataViewRowState.Unchanged);

                                    // response result ..
                                    if (view_machine_data.Count > 0)
                                        __returnObject = ServerObject.Response(ServerReponse.ResponseSign.PLUSOK, view_machine_data.ToTable());
                                    else
                                        __returnObject = ServerObject.Response(ServerReponse.ResponseSign.PLUSOK, null);

                                    view_machine_data.Dispose();
                                }
                                catch (Exception ex)
                                {
                                    // response error result ..
                                    __returnObject = ServerObject.Response(ServerReponse.ResponseSign.MINUSERR);

                                    string ExceptionMsg = ex.Message + ex.StackTrace;

                                    // write to log file ..
                                    EnvLog.Writer(AssemblyInfoClass.ProductInfo, Log.Level.Error, ExceptionMsg, true);
                                }
                                break;
                            #endregion

                            #region --F7300290--OK-- S:View Machine Pool Relation Case Workflow
                            case Client2Server.CommunicationType.VIEWMACHINEPOOLRELATION:
                                try
                                {
                                    // find match data ..
                                    DataView view_machine_pool_data = this.EnvData.FindData("Machine_Pool", QueueItems, DataViewRowState.Unchanged);

                                    // response result ..
                                    if (view_machine_pool_data.Count > 0)
                                    {
                                        __returnObject = ServerObject.Response(ServerReponse.ResponseSign.PLUSOK, view_machine_pool_data.ToTable());
                                    }
                                    else
                                    {
                                        __returnObject = ServerObject.Response(ServerReponse.ResponseSign.MINUSERR, null);
                                    }

                                    view_machine_pool_data.Dispose();

                                }
                                catch (Exception ex)
                                {
                                    // response error result ..
                                    __returnObject = ServerObject.Response(ServerReponse.ResponseSign.MINUSERR);

                                    string ExceptionMsg = ex.Message + ex.StackTrace;

                                    // write to log file ..
                                    EnvLog.Writer(AssemblyInfoClass.ProductInfo, Log.Level.Error, ExceptionMsg, true);
                                }
                                break;
                            #endregion

                            #region --F7300290--OK--View Machine Render Info Case Workflow
                            case Client2Server.CommunicationType.VIEWMACHINERENDERINFO:
                                try
                                {
                                    DataTable MachineData = DisplayBase.MachineStatus.Copy();

                                    // response result ..
                                    if (MachineData != null)
                                    {
                                        __returnObject = ServerObject.Response(ServerReponse.ResponseSign.PLUSOK, MachineData);
                                    }
                                    else
                                    {
                                        __returnObject = ServerObject.Response(ServerReponse.ResponseSign.MINUSERR, null);
                                    }

                                    MachineData.Dispose();
                                }
                                catch (Exception ex)
                                {
                                    // response error result ..
                                    __returnObject = ServerObject.Response(ServerReponse.ResponseSign.MINUSERR);

                                    string ExceptionMsg = ex.Message + ex.StackTrace;

                                    // write to log file ..
                                    EnvLog.Writer(AssemblyInfoClass.ProductInfo, Log.Level.Error, ExceptionMsg, true);
                                }
                                break;
                            #endregion

                            #region --F7300290--OK-- Enable Or Disable Machine Case Workflow
                            case Client2Server.CommunicationType.ONOFFMACHINE:
                                try
                                {
                                    if (QueueItems.ContainsKey("Machine_Id") && QueueItems.ContainsKey("IsEnable"))
                                    {
                                        if (bool.Parse(QueueItems["IsEnable"].ToString()))
                                        {
                                            QueueItems["IsEnable"] = 1;
                                        }
                                        else
                                        {
                                            QueueItems["IsEnable"] = 0;
                                        }

                                        // read machine table data ..
                                        DataView view_machine_data = this.EnvData.FindData(
                                            "Machine",
                                            string.Format("Machine_Id = '{0}'", QueueItems["Machine_Id"]),
                                            "Name",
                                            DataViewRowState.Unchanged);

                                        // create text command dictionary ..
                                        IDictionary<string, MySqlCommand> TextCommands = new Dictionary<string, MySqlCommand>();

                                        // define table collection ..
                                        DataSet mds = new DataSet();

                                        // define temporary data structure ..
                                        DataTable mtable = this.EnvData.ReadDataSchema("Machine");

                                        // create new row ..
                                        DataRow mrow = mtable.NewRow();

                                        // mapping column ..
                                        foreach (KeyValuePair<string, object> kv in QueueItems)
                                        {
                                            if (mtable.Columns.Contains(kv.Key))
                                                mrow[kv.Key] = kv.Value;
                                        }

                                        // check the data column is allow DbNull type ..
                                        foreach (DataColumn dc in view_machine_data.ToTable().Columns)
                                        {
                                            if (!dc.AllowDBNull)
                                            {
                                                if (!QueueItems.ContainsKey(dc.ColumnName)
                                                    && view_machine_data.ToTable().Rows.Count > 0)
                                                    mrow[dc.ColumnName] = view_machine_data.ToTable().Rows[0][dc.ColumnName];
                                            }
                                        }

                                        // complete added ..
                                        mtable.Rows.Add(mrow);

                                        // add to table collection ..
                                        mds.Tables.Add(mtable);

                                        // bind database command ..
                                        string cmd = " Update Machine Set ";
                                        //cmd += "Name = ?Name, Ip = ?Ip,";
                                        cmd += "  IsEnable = ?IsEnable, Last_Online_Time = ?Last_Online_Time ";
                                        //cmd += " Status = ?Status, Priority = ?Priority, Note = ?Note ";
                                        cmd += " Where Machine_Id = ?Machine_Id ";

                                        // add commands ..
                                        MySqlCommand command = new MySqlCommand(cmd);
                                        command.CommandType = CommandType.Text;
                                        TextCommands.Add(mtable.TableName, command);

                                        // sync data ..
                                        this.EnvData.WriteData(mds, TextCommands);
                                        // response error result ..
                                        __returnObject = ServerObject.Response(ServerReponse.ResponseSign.PLUSOK);

                                        // wait for write data finish ..
                                        Thread.Sleep(100);

                                        // refresh machine data list ..
                                        DisplayBase.CanMachineUpdate = true;


                                        view_machine_data.Dispose();
                                        mtable.Dispose();
                                        mds.Dispose();


                                    }
                                }
                                catch (Exception ex)
                                {
                                    // response error result ..
                                    __returnObject = ServerObject.Response(ServerReponse.ResponseSign.MINUSERR);

                                    string ExceptionMsg = ex.Message + ex.StackTrace;

                                    // write to log file ..
                                    EnvLog.Writer(AssemblyInfoClass.ProductInfo, Log.Level.Error, ExceptionMsg, true);
                                }
                                break;
                            #endregion

                            #region --F7300290--OK--Set Machine Priority Case Workflow
                            case Client2Server.CommunicationType.MACHINEPRIORITY:
                                try
                                {
                                    if (QueueItems.ContainsKey("Machine_Id") && QueueItems.ContainsKey("Priority"))
                                    {
                                        #region 獲取當前要更改的數據行所有數據 read machine table data ..
                                        DataView view_machine_data = this.EnvData.FindData(
                                            "Machine",
                                            string.Format("Machine_Id = '{0}'", QueueItems["Machine_Id"]),
                                            "Name",
                                            DataViewRowState.Unchanged);

                                        #endregion

                                        // create text command dictionary ..
                                        IDictionary<string, MySqlCommand> TextCommands = new Dictionary<string, MySqlCommand>();

                                        // define table collection ..
                                        DataSet mds = new DataSet();

                                        #region 獲取數據結構，並組合新數據
                                        // define temporary data structure ..
                                        DataTable mtable = this.EnvData.ReadDataSchema("Machine");

                                        // create new row ..
                                        DataRow mrow = mtable.NewRow();

                                        //添加更新後的數據 mapping column ..
                                        foreach (KeyValuePair<string, object> kv in QueueItems)
                                        {
                                            if (mtable.Columns.Contains(kv.Key))
                                                mrow[kv.Key] = kv.Value;
                                        }
                                        // 填充未更新數據 check the data column is allow DbNull type ..
                                        foreach (DataColumn dc in view_machine_data.ToTable().Columns)
                                        {
                                            if (!dc.AllowDBNull)
                                            {
                                                //查詢數據中不存在的數據行（保證所有不許空的數據）
                                                if (!QueueItems.ContainsKey(dc.ColumnName) && view_machine_data.ToTable().Rows.Count > 0)
                                                {
                                                    mrow[dc.ColumnName] = view_machine_data.ToTable().Rows[0][dc.ColumnName];
                                                }
                                            }
                                        }
                                        #endregion

                                        // complete added ..
                                        mtable.Rows.Add(mrow);

                                        // add to table collection ..
                                        mds.Tables.Add(mtable);

                                        //// bind database command ..
                                        string cmd = " Update Machine Set ";
                                        cmd += " Priority = ?Priority";
                                        cmd += " Where Machine_Id = ?Machine_Id ";

                                        // add commands ..
                                        MySqlCommand command = new MySqlCommand(cmd);
                                        command.CommandType = CommandType.Text;
                                        TextCommands.Add(mtable.TableName, command);

                                        // sync data ..
                                        this.EnvData.WriteData(mds, TextCommands);

                                        // response error result ..
                                        __returnObject = ServerObject.Response(ServerReponse.ResponseSign.PLUSOK);

                                        // wait for write data finish ..
                                        Thread.Sleep(100);
                                        // refresh machine data list ..
                                        DisplayBase.CanMachineUpdate = true;

                                        view_machine_data.Dispose();
                                        mtable.Dispose();
                                        mds.Dispose();
                                    }
                                }
                                catch (Exception ex)
                                {
                                    // response error result ..
                                    __returnObject = ServerObject.Response(ServerReponse.ResponseSign.MINUSERR);

                                    string ExceptionMsg = ex.Message + ex.StackTrace;

                                    // write to log file ..
                                    EnvLog.Writer(AssemblyInfoClass.ProductInfo, Log.Level.Error, ExceptionMsg, true);
                                }
                                break;
                            #endregion

                            #region --F7300290--OK-- U:Set Machine Pool Relation Case Workflow
                            case Client2Server.CommunicationType.MACHINEPOOLRELATION:
                                try
                                {
                                    // define table collection ..
                                    DataSet mds = new DataSet();

                                    // define temporary data structure ..
                                    DataTable mptable = this.EnvData.ReadDataSchema("Machine_Pool");
                                    DataTable fkpool = this.EnvData.ReadData("Pool");

                                    if (QueueItems.ContainsKey("Pool_Id") && QueueItems.ContainsKey("Machines"))
                                    {
                                        fkpool = this.EnvData.FindData(
                                            "Pool",
                                            string.Format("Pool_Id = '{0}'", QueueItems["Pool_Id"]),
                                            null,
                                            DataViewRowState.Unchanged).ToTable();

                                        foreach (string s in (string[])QueueItems["Machines"])
                                        {
                                            // create new row ..
                                            DataRow mprow = mptable.NewRow();

                                            if (mptable.Columns.Contains("Pool_Id") && QueueItems.ContainsKey("Pool_Id"))
                                            {
                                                // assign pool id key ..
                                                mprow["Pool_Id"] = QueueItems["Pool_Id"];
                                            }
                                            if (mptable.Columns.Contains("Machine_Id"))
                                            {
                                                // assign machine id ..
                                                mprow["Machine_Id"] = s;
                                            }
                                            // complete added ..
                                            mptable.Rows.Add(mprow);
                                        }

                                        // create text command dictionary ..
                                        IDictionary<string, MySqlCommand> TextCommands = new Dictionary<string, MySqlCommand>();

                                        if (fkpool.Rows.Count > 0)
                                        {
                                            // bind database delete command ..
                                            // 先刪除該POOL的所有Machine，再逐一添加
                                            string cmd = " Delete From Machine_Pool Where Pool_Id = ?Pool_Id ";

                                            // add delete data table ..
                                            mds.Tables.Add(fkpool);

                                            MySqlCommand command = new MySqlCommand(cmd);
                                            command.CommandType = CommandType.Text;

                                            // add commands ..
                                            TextCommands.Add(fkpool.TableName, command);
                                        }

                                        // bind database insert command ..
                                        string icmd = " Insert Into Machine_Pool ";
                                        icmd += " (Machine_Id, Pool_Id) ";
                                        icmd += " Values ";
                                        icmd += " (?Machine_Id, ?Pool_Id) ";

                                        MySqlCommand icommand = new MySqlCommand(icmd);
                                        icommand.CommandType = CommandType.Text;

                                        // add insert data table ..
                                        mds.Tables.Add(mptable);

                                        // refresh commands ..
                                        TextCommands.Add(mptable.TableName, icommand);

                                        // sync data ..
                                        this.EnvData.WriteData(mds, TextCommands);

                                        // response result ..
                                        __returnObject = ServerObject.Response(ServerReponse.ResponseSign.PLUSOK);
                                    }
                                    else
                                    {
                                        // response error result ..
                                        __returnObject = ServerObject.Response(ServerReponse.ResponseSign.MINUSERR);
                                    }

                                    mptable.Dispose();
                                    mds.Dispose();
                                    fkpool.Dispose();
                                }
                                catch (Exception ex)
                                {
                                    // response error result ..
                                    __returnObject = ServerObject.Response(ServerReponse.ResponseSign.MINUSERR);

                                    string ExceptionMsg = ex.Message + ex.StackTrace;

                                    // write to log file ..
                                    EnvLog.Writer(AssemblyInfoClass.ProductInfo, Log.Level.Error, ExceptionMsg, true);
                                }
                                break;
                            #endregion

                            #region --F7300290--OK-- S:View Pool Info Case Workflow
                            case Client2Server.CommunicationType.VIEWPOOLINFO:
                                try
                                {
                                    // find match data ..
                                    DataView view_pool_data = this.EnvData.FindData("Pool", QueueItems, DataViewRowState.Unchanged);

                                    // response result ..
                                    if (view_pool_data.Count > 0)
                                    {
                                        __returnObject = ServerObject.Response(ServerReponse.ResponseSign.PLUSOK, view_pool_data.ToTable());
                                    }
                                    else
                                    {
                                        __returnObject = ServerObject.Response(ServerReponse.ResponseSign.MINUSERR, null);
                                    }

                                    view_pool_data.Dispose();
                                }
                                catch (Exception ex)
                                {
                                    // response error result ..
                                    __returnObject = ServerObject.Response(ServerReponse.ResponseSign.MINUSERR);

                                    string ExceptionMsg = ex.Message + ex.StackTrace;

                                    // write to log file ..
                                    EnvLog.Writer(AssemblyInfoClass.ProductInfo, Log.Level.Error, ExceptionMsg, true);
                                }
                                break;
                            #endregion

                            #region --F7300290--OK-- A/U:Pool Info Case Workflow
                            case Client2Server.CommunicationType.POOLINFO:
                                try
                                {
                                    // read pool table data ..
                                    DataView view_pool_data = this.EnvData.FindData(
                                        "Pool",
                                        string.Format("Name = '{0}' Or Pool_Id = '{1}'",
                                        new object[] { QueueItems["Name"], QueueItems["Pool_Id"] }),
                                        "Name",
                                        DataViewRowState.Unchanged);

                                    // define table collection ..
                                    DataSet pds = new DataSet();

                                    // define temporary data structure ..
                                    DataTable ptable = this.EnvData.ReadDataSchema("Pool");

                                    // create new row ..
                                    DataRow prow = ptable.NewRow();

                                    // mapping column ..
                                    foreach (KeyValuePair<string, object> kv in QueueItems)
                                    {
                                        if (ptable.Columns.Contains(kv.Key))
                                            prow[kv.Key] = kv.Value;
                                    }

                                    // create text command dictionary ..
                                    string cmd = string.Empty;
                                    IDictionary<string, MySqlCommand> TextCommands = new Dictionary<string, MySqlCommand>();

                                    #region 更新
                                    // update pool data ..
                                    if (view_pool_data.Count > 0)
                                    {
                                        // get pool id column ..
                                        prow["Pool_Id"] = view_pool_data[0].DataView[0]["Pool_Id"];

                                        // complete added ..
                                        ptable.Rows.Add(prow);

                                        // add to table collection ..
                                        pds.Tables.Add(ptable);

                                        // bind database command ..
                                        cmd = " Update Pool Set ";
                                        cmd += " Name = ?Name, Sharable = ?Sharable ";
                                        cmd += " Where Pool_Id = ?Pool_Id ";
                                    }
                                    #endregion

                                    #region 插入
                                    // add new pool data ..
                                    else
                                    {
                                        // create new pool id ..
                                        prow["Pool_Id"] = Guid.NewGuid().ToString().ToUpper();

                                        // complete added ..
                                        ptable.Rows.Add(prow);

                                        // add to table collection ..
                                        pds.Tables.Add(ptable);

                                        // bind database command ..
                                        cmd = " Insert Into Pool ";
                                        cmd += " (Pool_Id, Name, Sharable) ";
                                        cmd += " Values ";
                                        cmd += " (?Pool_Id, ?Name, ?Sharable) ";
                                    }
                                    #endregion

                                    // add commands ..
                                    MySqlCommand command = new MySqlCommand(cmd);
                                    command.CommandType = CommandType.Text;
                                    TextCommands.Add(ptable.TableName, command);

                                    // sync data ..
                                    this.EnvData.WriteData(pds, TextCommands);

                                    // response result ..
                                    __returnObject = ServerObject.Response(ServerReponse.ResponseSign.PLUSOK);

                                    view_pool_data.Dispose();
                                    ptable.Dispose();
                                    pds.Dispose();

                                }
                                catch (Exception ex)
                                {
                                    // response error result ..
                                    __returnObject = ServerObject.Response(ServerReponse.ResponseSign.MINUSERR);

                                    string ExceptionMsg = ex.Message + ex.StackTrace;

                                    // write to log file ..
                                    EnvLog.Writer(AssemblyInfoClass.ProductInfo, Log.Level.Error, ExceptionMsg, true);
                                }
                                break;
                            #endregion

                            #region --F7300290--OK-- D:Delete Pool Info Case Workflow
                            case Client2Server.CommunicationType.DELETEPOOLINFO:
                                try
                                {
                                    // read pool table data ..
                                    DataView delete_pool_data = this.EnvData.FindData(
                                        "Pool",
                                        string.Format("Name = '{0}' Or Pool_Id = '{1}'",
                                        new object[] { QueueItems["Name"], QueueItems["Pool_Id"] }),
                                        "Pool_Id",
                                        DataViewRowState.Unchanged);

                                    // get pool id ..
                                    string id = delete_pool_data.ToTable().Rows[0]["Pool_Id"].ToString();

                                    // read machine pool table data ..
                                    DataView delete_machine_pool_data = this.EnvData.FindData(
                                        "Machine_Pool",
                                        string.Format("Pool_Id = '{0}'",
                                        new object[] { id }),
                                        "Pool_Id",
                                        DataViewRowState.Unchanged);
                                    DataTable machinepooltable = delete_machine_pool_data.ToTable(),
                                        pooltable = delete_pool_data.ToTable();

                                    //DataRow[]
                                    //           JobGroupRows = JobGroupInfo.Select(exp),
                                    // 先刪除MachinePool，再操作Pool
                                    // bind database command ..
                                    string cmd = " Select count(*) From Machine_Pool ";
                                    cmd += " Inner Join Pool On Machine_Pool.Pool_Id =Pool.Pool_Id ";
                                    cmd += " Where Pool.Pool_Id = ?Pool_Id";
                                    MySqlCommand command = new MySqlCommand(cmd);
                                    command.Parameters.AddWithValue("?Pool_Id", id);
                                    if (this.EnvData.IsExistJobMachineId(command) > 0)
                                    {
                                        cmd = "Delete From Machine_Pool Where Pool_Id = ?Pool_Id; ";
                                        cmd += "Delete From Pool Where Pool_Id = ?Pool_Id ";
                                    }
                                    else
                                    {
                                        cmd = "Delete From Pool Where Pool_Id = ?Pool_Id ";
                                    }

                                    // define table collection ..
                                    DataSet pds = new DataSet();

                                    // create text command dictionary ..
                                    IDictionary<string, MySqlCommand> TextCommands = new Dictionary<string, MySqlCommand>();

                                    // add to table collection ..
                                    pds.Tables.Add(machinepooltable);
                                    pds.Tables.Add(pooltable);

                                    // add commands ..
                                    command = new MySqlCommand(cmd);
                                    command.Parameters.AddWithValue("?Pool_Id", id);
                                    TextCommands.Add(machinepooltable.TableName, command);
                                    TextCommands.Add(pooltable.TableName, command);
                                    // sync data ..
                                    this.EnvData.WriteDataPool(pds, TextCommands);

                                    // response result ..
                                    __returnObject = ServerObject.Response(ServerReponse.ResponseSign.PLUSOK);

                                    machinepooltable.Dispose();
                                    pooltable.Dispose();
                                    pds.Dispose();
                                    delete_machine_pool_data.Dispose();
                                    delete_pool_data.Dispose();

                                }
                                catch (Exception ex)
                                {
                                    // response error result ..
                                    __returnObject = ServerObject.Response(ServerReponse.ResponseSign.MINUSERR);

                                    string ExceptionMsg = ex.Message + ex.StackTrace;

                                    // write to log file ..
                                    EnvLog.Writer(AssemblyInfoClass.ProductInfo, Log.Level.Error, ExceptionMsg, true);
                                }
                                break;
                            #endregion

                            #region --F7300290--OK-- A/U:Job Queue Info Case Workflow            ??Dispose???
                            case Client2Server.CommunicationType.JOBQUEUEADD:

                            case Client2Server.CommunicationType.JOBQUEUEUPDATE:
                                try
                                {
                                    // declare error flag ..
                                    bool HasError = false;

                                    #region 获取原有数据/结构
                                    // create filter conditions string ..
                                    string filter = string.Empty;

                                    if (QueueItems.ContainsKey("Job_Group_Id"))
                                    {
                                        filter = string.Format("Name = '{0}' Or Job_Group_Id = '{1}'", new object[] { QueueItems["Name"], QueueItems["Job_Group_Id"] });
                                    }
                                    else
                                    {
                                        filter = string.Format("Name = '{0}'", new object[] { QueueItems["Name"] });
                                    }

                                    DataView view_job_group_data = null;

                                    // job queue update action ..
                                    if (ActionHeader == Client2Server.CommunicationType.JOBQUEUEUPDATE)
                                    {
                                        // read job group table data ..
                                        view_job_group_data = this.EnvData.FindData(
                                            "job_group",
                                            filter,
                                            "Name",
                                            DataViewRowState.Unchanged);
                                    }
                                    else
                                    {
                                        // job queue add-in action ..
                                        view_job_group_data = this.EnvData.ReadDataSchema("Job_Group").DefaultView;
                                    }

                                    // define temporary data structure ..
                                    IList<DataTable> jtables = new List<DataTable>
                                    {
                                        this.EnvData.ReadDataSchema("Job_Group"),
                                        this.EnvData.ReadDataSchema("Job_Attr"),
                                        this.EnvData.ReadDataSchema("Job"),
                                        this.EnvData.ReadDataSchema("Job_History")
                                    };

                                    #endregion

                                    #region 添加、加工数据

                                    // generate new job globally unique identifier ..
                                    Guid id = Guid.NewGuid();



                                    // declare frames and command variable ..
                                    int start = 0, end = 0, ps = 0;
                                    string command = string.Empty;

                                    // assign start, end, packetsize and command attributes ..
                                    if (QueueItems.ContainsKey("Start") &&
                                        QueueItems.ContainsKey("End") &&
                                        QueueItems.ContainsKey("Packet_Size") &&
                                        QueueItems.ContainsKey("Command"))
                                    {
                                        start = Convert.ToInt32(QueueItems["Start"]);
                                        end = Convert.ToInt32(QueueItems["End"]);
                                        ps = Convert.ToInt32(QueueItems["Packet_Size"]);
                                        command = QueueItems["Command"].ToString().Trim();
                                    }
                                    else
                                    {
                                        // response result ..
                                        __returnObject = ServerObject.Response(ServerReponse.ResponseSign.MINUSERR);
                                        break;
                                    }

                                    // confirm use alienbrain extension ..
                                    if (QueueItems["ABName"] != null && !string.IsNullOrEmpty(QueueItems["ABName"].ToString()) &&
                                        QueueItems["ABPath"] != null && !string.IsNullOrEmpty(QueueItems["ABPath"].ToString()))
                                    {
                                        QueueItems["Status"] = Convert.ToUInt16(JobStatusFlag.CHECKING);
                                    }

                                    if (QueueItems.ContainsKey("ABUpdateOnly"))
                                    {
                                        if (QueueItems["ABUpdateOnly"].ToString() == "True")
                                            QueueItems["ABUpdateOnly"] = 1;
                                        else
                                            QueueItems["ABUpdateOnly"] = 0;
                                    }

                                    #endregion

                                    foreach (DataTable table in jtables)
                                    {
                                        // define table collection ..
                                        DataSet jds = new DataSet();

                                        // declare data row variable ..
                                        DataRow row = null;

                                        #region JobTable  Process
                                        if (table.TableName.Equals("job"))
                                        {
                                            #region Process Job Data Section
                                            for (int i = start; i <= end; i = i + ps)
                                            {
                                                int
                                                    _start = -1,
                                                    _end = -1;

                                                if ((i + ps) - 1 <= end)
                                                {
                                                    _start = i;
                                                    _end = (i + ps) - 1;
                                                }
                                                else
                                                {
                                                    _start = i;
                                                    _end = end;
                                                }

                                                // create new row ..
                                                row = table.NewRow();

                                                // 
                                                row["Job_Id"] = i;

                                                if (view_job_group_data.Count > 0)
                                                {
                                                    // get the job group id ..
                                                    row["Job_Group_Id"] = new Guid(view_job_group_data.ToTable().Rows[0]["Job_Group_Id"].ToString()).ToString().ToUpper();
                                                }
                                                else
                                                {
                                                    // assign the job group id ..
                                                    row["Job_Group_Id"] = id.ToString().ToUpper();
                                                }

                                                // 替換字符串，這樣Maya才能接受command ..
                                                row["Command"] = command.Replace("#S", _start.ToString()).Replace("#E", _end.ToString()).Trim();

                                                // complete added ..
                                                table.Rows.Add(row);
                                            }
                                            #endregion

                                            // add to table collection ..
                                            jds.Tables.Add(table);
                                        }
                                        #endregion

                                        #region
                                        else
                                        {
                                            // create new row ..
                                            row = table.NewRow();

                                            if (view_job_group_data.Count > 0)
                                            {
                                                // get the job group id ..
                                                row["Job_Group_Id"] = new Guid(view_job_group_data.ToTable().Rows[0]["Job_Group_Id"].ToString()).ToString().ToUpper();
                                            }
                                            else
                                            {
                                                // assign the job group id ..
                                                row["Job_Group_Id"] = id.ToString().ToUpper();
                                            }

                                            #region Process History Frames Column
                                            if (table.TableName == "job_history" && table.Columns.Contains("Frames"))
                                            {
                                                row["Frames"] = string.Format("{0}-{1} [{2}]", QueueItems["Start"], QueueItems["End"], QueueItems["Packet_Size"]);
                                            }
                                            #endregion

                                            #region mapping column ..
                                            foreach (KeyValuePair<string, object> kv in QueueItems)
                                            {

                                                if (table.Columns.Contains(kv.Key))
                                                {
                                                    #region Mapping Machine Foreign Key
                                                    if (kv.Key.IndexOf("Machine") > 0)
                                                    {
                                                        // read machine table data ..
                                                        DataView view_machine_data = this.EnvData.FindData(
                                                            "machine",
                                                            string.Format("Name = '{0}'",
                                                            new object[] { kv.Value }),
                                                            "Machine_Id",
                                                            DataViewRowState.Unchanged);

                                                        if (view_machine_data.ToTable().Rows.Count > 0)
                                                            // get machine id ..
                                                            row[kv.Key] = view_machine_data.ToTable().Rows[0]["Machine_Id"].ToString();

                                                        continue;
                                                    }
                                                    #endregion
                                                    // mapping other columns ..
                                                    row[kv.Key] = kv.Value;
                                                }
                                            }
                                            #endregion

                                            // complete added ..
                                            table.Rows.Add(row);

                                            // add to table collection ..
                                            jds.Tables.Add(table);
                                        }
                                        #endregion

                                        // create text command dictionary ..
                                        IDictionary<string, MySqlCommand> TextCommands = new Dictionary<string, MySqlCommand>();

                                        #region Read Job Relation Tables Data
                                        // job attribute table ..
                                        DataView view_job_attr_data = this.EnvData.FindData(
                                            "job_attr",
                                            string.Format("Job_Group_Id = '{0}'", new object[] { row["Job_Group_Id"] }),
                                            "Job_Group_Id",
                                            DataViewRowState.Unchanged);

                                        // job table ..
                                        DataView view_job_data = this.EnvData.FindData(
                                            "job",
                                            string.Format("Job_Group_Id = '{0}'", new object[] { row["Job_Group_Id"] }),
                                            "Job_Group_Id",
                                            DataViewRowState.Unchanged);

                                        // create job relation data row count ..
                                        IDictionary<string, int> DataRecords = new Dictionary<string, int>
                                        {
                                            {view_job_group_data.Table.TableName, view_job_group_data.Count},
                                            {view_job_attr_data.Table.TableName, view_job_attr_data.Count},
                                            {view_job_data.Table.TableName, view_job_data.Count},
                                        };
                                        #endregion

                                        #region Process History Data Section
                                        if (table.TableName == "job_history")
                                        {
                                            //DataView view_jobhistory_data = this.EnvData.FindData(
                                            //"job_history",
                                            //string.Format("Submit_Acct = '{0}'", new object[] { row["Submit_Acct"] }),
                                            //"Submit_Time",
                                            //DataViewRowState.Unchanged);

                                            string cmd = string.Empty;
                                            MySqlCommand comm = new MySqlCommand();

                                            //if (view_jobhistory_data.ToTable().Rows.Count >= 50)
                                            //{
                                            //    cmd += " Update Job_History Set ";
                                            //    cmd += " Job_Group_Id = ?Job_Group_Id, Name = ?Name, Frames = @Frames, ";
                                            //    cmd += " Submit_Acct = ?Submit_Acct, Submit_Time = ?Submit_Time ";
                                            //    cmd += " Where Submit_Time = (Select Min(Submit_Time) From Job_History) ";？？？？？如何实现？？？？？
                                            //    cmd += " And Submit_Acct = @Submit_Acct ";
                                            //}
                                            //else
                                            //{
                                            cmd += " Insert Into Job_History ";
                                            cmd += " (Job_Group_Id, Name, Frames, Submit_Acct, Submit_Time) ";
                                            cmd += " Values ";
                                            cmd += " (?Job_Group_Id, ?Name, ?Frames, ?Submit_Acct, ?Submit_Time) ";
                                            //}

                                            // add commands ..
                                            comm.CommandText = cmd;
                                            comm.CommandType = CommandType.Text;
                                            TextCommands.Add(table.TableName, comm);

                                            // sync data ..
                                            this.EnvData.WriteData(jds, TextCommands);
                                            continue;
                                        }
                                        #endregion

                                        #region update job data ..
                                        if (DataRecords[table.TableName] > 0)
                                        {
                                            if (ActionHeader == Client2Server.CommunicationType.JOBQUEUEADD)
                                            {
                                                HasError = true;
                                                break;
                                            }

                                            string cmd = string.Empty;
                                            MySqlCommand comm = new MySqlCommand();

                                            #region Update_Bind DataBase Command
                                            switch (table.TableName)
                                            {
                                                case "job_group":
                                                    cmd = " Update Job_Group Set ";
                                                    cmd += " Submit_Machine = ?Submit_Machine, Submit_Acct = ?Submit_Acct, Submit_Time = ?Submit_Time, ";
                                                    cmd += " First_Pool = ?First_Pool, Second_Pool = ?Second_Pool, Status = ?Status, ";
                                                    cmd += " Finish_Time = ?Finish_Time, Note = ?Note ";
                                                    cmd += " Where ";
                                                    cmd += " Job_Group_Id = ?Job_Group_Id ";

                                                    comm.CommandText = cmd;
                                                    comm.CommandType = CommandType.Text;
                                                    // add commands ..
                                                    TextCommands.Add(table.TableName, comm);

                                                    // sync data ..
                                                    this.EnvData.WriteData(jds, TextCommands);
                                                    break;
                                                case "job_attr":
                                                    cmd = " Update Job_Attr Set ";
                                                    cmd += " Project = ?Project, Start = ?Start, End= ?End, Packet_Size = ?Packet_Size, ";
                                                    cmd += " Proc_Type = ?Proc_Type, WaitFor= ?WaitFor, Priority = ?Priority, ";
                                                    cmd += " ABName = ?ABName, ABPath = ?ABPath, ABUpdateOnly = ?ABUpdateOnly ";
                                                    cmd += " Where ";
                                                    cmd += " Job_Group_Id = ?Job_Group_Id ";
                                                    comm.CommandText = cmd;
                                                    comm.CommandType = CommandType.Text;
                                                    // add commands ..
                                                    TextCommands.Add(table.TableName, comm);

                                                    // sync data ..
                                                    this.EnvData.WriteData(jds, TextCommands);
                                                    break;
                                                case "job":
                                                    cmd = " Select count(*)  From Job ";
                                                    cmd += " Inner Join Job_Group On Job.Job_Group_Id = Job_Group.Job_Group_Id  ";
                                                    cmd += " Where Job.Job_Group_Id = ?Job_Group_Id ";
                                                    comm = new MySqlCommand(cmd);
                                                    comm.Parameters.AddWithValue("?Job_Group_Id", table.Rows[0]["Job_Group_Id"].ToString());
                                                    if (this.EnvData.IsExistJobMachineId(comm) > 0)
                                                    {
                                                        cmd = "  Delete From Job Where Job_Group_Id = ?Job_Group_Id ;";
                                                        cmd += " Insert Into Job ";
                                                        cmd += " (Job_Group_Id, Command, Proc_Machine, Start_Time, Finish_Time, OutputLog) ";
                                                        cmd += " Values ";
                                                        cmd += " (?Job_Group_Id, ?Command, ?Proc_Machine, ?Start_Time,?Finish_Time, ?OutputLog)";
                                                    }
                                                    comm.CommandText = cmd;
                                                    comm.CommandType = CommandType.Text;
                                                    // add commands ..
                                                    TextCommands.Add(table.TableName, comm);
                                                    // sync data ..
                                                    this.EnvData.WriteData(jds, TextCommands);

                                                    break;
                                            }
                                            #endregion
                                        }
                                        #endregion

                                        #region  add new job data ..
                                        else
                                        {
                                            if (ActionHeader == Client2Server.CommunicationType.JOBQUEUEUPDATE)
                                            {
                                                HasError = true;
                                                break;
                                            }

                                            string cmd = string.Empty;
                                            MySqlCommand Jcommand = new MySqlCommand();

                                            #region Insert_Bind DataBase Command
                                            switch (table.TableName)
                                            {
                                                case "job_group":
                                                    cmd = " Insert Into Job_Group ";
                                                    cmd += " (Job_Group_Id, Name, Submit_Machine, Submit_Acct, Submit_Time, ";
                                                    cmd += " First_Pool, Second_Pool, Status, Start_Time, Finish_Time, Note) ";
                                                    cmd += " Values ";
                                                    cmd += " (?Job_Group_Id, ?Name, ?Submit_Machine, ?Submit_Acct, ?Submit_Time, ";
                                                    cmd += " ?First_Pool, ?Second_Pool, ?Status, ?Start_Time, ?Finish_Time, ?Note) ";
                                                    break;
                                                case "job_attr":
                                                    cmd = " Insert Into Job_Attr ";
                                                    cmd += " (Job_Group_Id, Project, Start, End, Packet_Size, Proc_Type, ";
                                                    cmd += " WaitFor, Priority, ABName, ABPath, ABUpdateOnly) ";
                                                    cmd += " Values ";
                                                    cmd += " (?Job_Group_Id, ?Project, ?Start, ?End, ?Packet_Size, ?Proc_Type, ";
                                                    cmd += " ?WaitFor, ?Priority, ?ABName, ?ABPath, ?ABUpdateOnly) ";
                                                    break;
                                                case "job":
                                                    cmd = " Insert Into Job ";
                                                    cmd += " (Job_Group_Id, Command, Proc_Machine, Start_Time, Finish_Time, OutputLog) ";
                                                    cmd += " Values ";
                                                    cmd += " (?Job_Group_Id, ?Command, ?Proc_Machine, ?Start_Time, ?Finish_Time, ?OutputLog) ";
                                                    break;
                                            }
                                            #endregion

                                            Jcommand.CommandText = cmd;
                                            Jcommand.CommandType = CommandType.Text;

                                            // add commands ..
                                            TextCommands.Add(table.TableName, Jcommand);

                                            // sync data ..
                                            this.EnvData.WriteData(jds, TextCommands);
                                        }
                                        #endregion
                                    }

                                    #region 記錄新增任務的Job_Group_Id
                                    IList<string> NewJobID = new List<string>();
                                    foreach (DataRow dr in jtables[0].Rows)
                                    {
                                        NewJobID.Add(dr["Job_Group_Id"].ToString());
                                    }
                                    #endregion

                                    if (HasError)
                                    {
                                        // response result ..
                                        __returnObject = ServerObject.Response(ServerReponse.ResponseSign.MINUSERR);
                                    }
                                    else
                                    {
                                        // response result ..
                                        __returnObject = ServerObject.Response(ServerReponse.ResponseSign.PLUSOK, NewJobID);

                                        // wait for write data finish ..
                                        Thread.Sleep(100);

                                        // refresh job data list ..
                                        DisplayBase.CanJobUpdate = true;
                                    }

                                    view_job_group_data.Dispose();
                                }
                                catch (Exception ex)
                                {
                                    // response error result ..
                                    __returnObject = ServerObject.Response(ServerReponse.ResponseSign.MINUSERR);

                                    string ExceptionMsg = ex.Message + ex.StackTrace;

                                    // write to log file ..
                                    EnvLog.Writer(AssemblyInfoClass.ProductInfo, Log.Level.Error, ExceptionMsg, true);
                                }
                                break;
                            #endregion

                            #region   --F7300290--OK-- Delete Job Queue Case Workflow
                            case Client2Server.CommunicationType.JOBQUEUEDELETE:
                                try
                                {
                                    if (((IList<string>)QueueItems["DeleteList"]).Count > 0)
                                    {
                                        // enumerable delete job for list ..
                                        foreach (string job in ((IList<string>)QueueItems["DeleteList"]))
                                        {
                                            // define filter expression ..
                                            string exp = string.Format("Job_Group_Id = '{0}'", job);

                                            // read job data ..
                                            DataTable
                                                JobGroupInfo = this.EnvData.ReadData("Job_Group"),
                                                JobAttrInfo = this.EnvData.ReadData("Job_Attr"),
                                                JobInfo = this.EnvData.ReadData("Job");

                                            DataRow[]
                                                JobGroupRows = JobGroupInfo.Select(exp),
                                                JobAttrRows = JobAttrInfo.Select(exp),
                                                JobRows = JobInfo.Select(exp);

                                            #region Get Process Machine Group
                                            // create send to machine of delete list ..
                                            Hashtable __machine = new Hashtable();

                                            // get machine processing info ..
                                            var machine_query = from mq in JobRows.AsEnumerable()
                                                                group mq by mq.Field<string>("Proc_Machine") into signle_machine
                                                                select signle_machine.ToList();

                                            // regroup job list ..
                                            foreach (var group in machine_query)
                                            {
                                                if (group.Count > 0 && !string.IsNullOrEmpty(group[0]["Proc_Machine"].ToString()))
                                                {
                                                    IList<uint> list = new List<uint>();

                                                    foreach (DataRow row in group)
                                                    {
                                                        list.Add(Convert.ToUInt32(row["Job_Id"]));
                                                    }
                                                    __machine.Add(group[0]["Proc_Machine"], list);
                                                }
                                            }

                                            // send to processing render farm ..
                                            if (__machine.Count > 0)
                                            {
                                                // 發送指令到算圖機
                                                this.DeleteFunc(__machine);
                                            }
                                            #endregion

                                            // delete relation data ..
                                            for (int i = 0; i < JobGroupRows.Length; i++)
                                                JobGroupRows[i].Delete();
                                            for (int i = 0; i < JobAttrRows.Length; i++)
                                                JobAttrRows[i].Delete();
                                            for (int i = 0; i < JobRows.Length; i++)
                                                JobRows[i].Delete();

                                            // bind database command ..
                                            string cmd = " Select count(*) From Job ";
                                            cmd += " Inner Join Job_Group On Job.Job_Group_Id = Job_Group.Job_Group_Id ";
                                            cmd += " Inner Join Job_Attr On Job_Attr.Job_Group_Id = Job_Group.Job_Group_Id ";
                                            cmd += " where Job_Group.Job_Group_Id = ?Job_Group_Id";

                                            //cmd.Replace("?Job_Group_Id", string.Format("'{0}'", job));
                                            MySqlCommand command = new MySqlCommand(cmd);
                                            command.Parameters.AddWithValue("?Job_Group_Id", job);
                                            if (this.EnvData.IsExistJobMachineId(command) > 0)
                                            {
                                                cmd = " Delete From Job Where Job_Group_Id = ?Job_Group_Id ;";
                                                cmd += " Delete From Job_Attr Where Job_Group_Id = ?Job_Group_Id ;";
                                                cmd += " Delete From Job_Group Where Job_Group_Id = ?Job_Group_Id ;";
                                            }
                                            else
                                            {
                                                cmd = string.Empty;
                                            }

                                            // define tables collection ..
                                            DataSet pds = new DataSet();

                                            // create text command dictionary ..
                                            IDictionary<string, MySqlCommand> TextCommands = new Dictionary<string, MySqlCommand>();
                                            command = new MySqlCommand(cmd);
                                            // add to table collection ..
                                            pds.Tables.Add(JobGroupInfo);
                                            pds.Tables.Add(JobAttrInfo);
                                            pds.Tables.Add(JobInfo);

                                            // add commands ..
                                            command.Parameters.AddWithValue("?Job_Group_Id", job);
                                            TextCommands.Add(JobGroupInfo.TableName, command);
                                            TextCommands.Add(JobAttrInfo.TableName, command);
                                            TextCommands.Add(JobInfo.TableName, command);

                                            // sync data ..
                                            this.EnvData.WriteData(pds, TextCommands);


                                            pds.Dispose();
                                        }

                                        // response result ..
                                        __returnObject = ServerObject.Response(ServerReponse.ResponseSign.PLUSOK);

                                        // wait for write data finish ..
                                        Thread.Sleep(100);

                                        // refresh machine data list ..
                                        DisplayBase.CanJobUpdate = true;
                                    }
                                }
                                catch (Exception ex)
                                {
                                    // response error result ..
                                    __returnObject = ServerObject.Response(ServerReponse.ResponseSign.MINUSERR);

                                    string ExceptionMsg = ex.Message + ex.StackTrace;

                                    // write to log file ..
                                    EnvLog.Writer(AssemblyInfoClass.ProductInfo, Log.Level.Error, ExceptionMsg, true);
                                }
                                break;
                            #endregion

                            // pause all have not yet sent of job ..
                            #region --F7300290--OK--  Pause Job Queue Case Workflow
                            case Client2Server.CommunicationType.JOBQUEUEPAUSE:

                                try
                                {
                                    if (QueueItems.ContainsKey("Job_Pause_IDlist") && QueueItems.ContainsKey("Status"))
                                    {
                                        foreach (string Ids in (string[])QueueItems["Job_Pause_IDlist"])
                                        {
                                            DataView view_Job_data = this.EnvData.FindData(
                                                "job_group",
                                                string.Format("Job_Group_Id = '{0}'", Ids.Trim()),
                                                "Job_Group_Id",
                                                DataViewRowState.Unchanged
                                            );

                                            #region 暂停或重启 更新Job_Group表

                                            // create text command dictionary ..
                                            IDictionary<string, MySqlCommand> TextCommands = new Dictionary<string, MySqlCommand>();

                                            // define table collection ..
                                            DataSet jds = new DataSet();

                                            // define temporary data structure ..
                                            DataTable jtable = this.EnvData.ReadDataSchema("Job_Group");
                                            DataRow jrow = jtable.NewRow();

                                            //添加更新後的數據 mapping column ..
                                            jrow["Job_Group_Id"] = Ids;
                                            jrow["Status"] = Convert.ToInt32(QueueItems["Status"]);
                                            foreach (DataColumn dc in view_Job_data.ToTable().Columns)
                                            {
                                                if (!dc.AllowDBNull)
                                                {
                                                    //查詢數據中不存在的數據行（保證所有不許空的數據）
                                                    if (!QueueItems.ContainsKey(dc.ColumnName) && view_Job_data.ToTable().Rows.Count > 0)
                                                    {
                                                        jrow[dc.ColumnName] = view_Job_data.ToTable().Rows[0][dc.ColumnName];
                                                    }
                                                }
                                            }
                                            // complete added ..
                                            jtable.Rows.Add(jrow);

                                            #endregion

                                            #region 重启时清空job的proc_machine等信息，以便重新算图
                                            if (Convert.ToInt16(QueueItems["Status"]) != 2)
                                            {
                                                DataTable Job_attr_data = this.EnvData.FindData(
                                                    "job",
                                                    string.Format("Job_Group_Id = '{0}' And Finish_Time Is Null And Proc_Machine Is Not Null ", Ids.Trim()),
                                                    "Job_Group_Id",
                                                    DataViewRowState.Unchanged
                                                    ).ToTable();

                                                if (Job_attr_data.Rows.Count > 0)
                                                {
                                                    // define table collection ..
                                                    DataSet jobds = new DataSet();

                                                    // define temporary data structure ..
                                                    DataTable jobtable = this.EnvData.ReadDataSchema("Job");

                                                    foreach (DataRow dr in Job_attr_data.Rows)
                                                    {
                                                        // create new row ..
                                                        DataRow jobrow = jobtable.NewRow();
                                                        //添加更新後的數據 mapping column ..
                                                        jobrow["Proc_Machine"] = null;
                                                        jobrow["Start_Time"] = DBNull.Value;
                                                        jobrow["Finish_Time"] = DBNull.Value;

                                                        // 填充未更新數據 check the data column is allow DbNull type ..
                                                        foreach (DataColumn dc in Job_attr_data.Columns)
                                                        {
                                                            if (!dc.AllowDBNull)
                                                            {
                                                                jobrow[dc.ColumnName] = dr[dc.ColumnName];
                                                            }
                                                        }
                                                        jobtable.Rows.Add(jobrow);
                                                    }

                                                    jobds.Tables.Add(jobtable);

                                                    string jcmd = " Update Job Set  ";
                                                    jcmd += "  Proc_Machine = ?Proc_Machine,  Start_Time = ?Start_Time,  Finish_Time = ?Finish_Time,    ";
                                                    jcmd += "  Where  ";
                                                    jcmd += "  Job_Id = ?Job_Id ";

                                                    // create text command dictionary ..
                                                    IDictionary<string, MySqlCommand> JobCommand = new Dictionary<string, MySqlCommand>();

                                                    MySqlCommand Jcommand = new MySqlCommand(jcmd);
                                                    Jcommand.CommandType = CommandType.Text;
                                                    //// add commands ..
                                                    JobCommand.Add(jobtable.TableName, Jcommand);

                                                    // sync data ..
                                                    this.EnvData.WriteData(jobds, JobCommand);

                                                    Job_attr_data.Dispose();
                                                    jobds.Dispose();
                                                }
                                            }

                                            #endregion

                                            // add to table collection ..
                                            jds.Tables.Add(jtable);


                                            string cmd = " Update Job_Group Set  ";
                                            cmd += "  Status = ?Status   ";
                                            cmd += "  Where Job_Group_Id = ?Job_Group_Id ";

                                            MySqlCommand command = new MySqlCommand(cmd);
                                            command.CommandType = CommandType.Text;

                                            //// add commands ..
                                            TextCommands.Add(jtable.TableName, command);

                                            // sync data ..
                                            this.EnvData.WriteData(jds, TextCommands);

                                            view_Job_data.Dispose();
                                            jds.Dispose();

                                        }

                                        // response result ..
                                        __returnObject = ServerObject.Response(ServerReponse.ResponseSign.PLUSOK);
                                        // wait for write data finish ..
                                        Thread.Sleep(100);
                                        // refresh machine data list ..
                                        DisplayBase.CanJobUpdate = true;
                                    }
                                }
                                catch (Exception ex)
                                {
                                    // response error result ..
                                    __returnObject = ServerObject.Response(ServerReponse.ResponseSign.MINUSERR);
                                    string ExceptionMsg = ex.Message + ex.StackTrace;
                                    // write to log file ..
                                    EnvLog.Writer(AssemblyInfoClass.ProductInfo, Log.Level.Error, ExceptionMsg, true);
                                }

                                break;
                            #endregion

                            // current empty ..
                            #region --F7300290--OK-- Repeat Job Queue Case Workflow
                            case Client2Server.CommunicationType.JOBQUEUEREPEAT:
                                try
                                {
                                }
                                catch
                                { }
                                break;
                            #endregion

                            // Set Jobs Priority ..
                            #region --F7300290--OK-- U:Set Jobs Priority Case Workflow
                            case Client2Server.CommunicationType.JOBPRIORITY:

                                try
                                {
                                    if (QueueItems.ContainsKey("Job_Group_Id") && QueueItems.ContainsKey("Priority"))
                                    {
                                        #region 獲取當前要更改的數據行所有數據 read job_attr table data ..
                                        DataView view_machine_data = this.EnvData.FindData(
                                            "Job_Attr",
                                            string.Format("Job_Group_Id = '{0}'", QueueItems["Job_Group_Id"]),
                                            "Job_Group_Id",
                                            DataViewRowState.Unchanged);

                                        #endregion

                                        // create text command dictionary ..
                                        IDictionary<string, MySqlCommand> TextCommands = new Dictionary<string, MySqlCommand>();

                                        // define table collection ..
                                        DataSet jds = new DataSet();

                                        #region 獲取數據結構，並組合新數據
                                        // define temporary data structure ..
                                        DataTable jtable = this.EnvData.ReadDataSchema("job_attr");

                                        // create new row ..
                                        DataRow jrow = jtable.NewRow();

                                        //添加更新後的數據 mapping column ..
                                        foreach (KeyValuePair<string, object> kv in QueueItems)
                                        {
                                            if (jtable.Columns.Contains(kv.Key))
                                                jrow[kv.Key] = kv.Value;
                                        }
                                        // 填充未更新數據 check the data column is allow DbNull type ..
                                        foreach (DataColumn dc in view_machine_data.ToTable().Columns)
                                        {
                                            if (!dc.AllowDBNull)
                                            {
                                                //查詢數據中不存在的數據行（保證所有不許空的數據）
                                                if (!QueueItems.ContainsKey(dc.ColumnName) && view_machine_data.ToTable().Rows.Count > 0)
                                                {
                                                    jrow[dc.ColumnName] = view_machine_data.ToTable().Rows[0][dc.ColumnName];
                                                }
                                            }
                                        }

                                        // complete added ..
                                        jtable.Rows.Add(jrow);

                                        #endregion

                                        // add to table collection ..
                                        jds.Tables.Add(jtable);

                                        string cmd = " Update Job_Attr Set ";
                                        cmd += "  Priority = ?Priority";
                                        cmd += "  Where Job_Group_Id = ?Job_Group_Id ";

                                        //// add commands ..
                                        MySqlCommand command = new MySqlCommand(cmd);
                                        command.CommandType = CommandType.Text;
                                        TextCommands.Add(jtable.TableName, command);

                                        // sync data ..
                                        this.EnvData.WriteData(jds, TextCommands);

                                        // response error result ..
                                        __returnObject = ServerObject.Response(ServerReponse.ResponseSign.PLUSOK);
                                        // wait for write data finish ..
                                        Thread.Sleep(100);
                                        // refresh machine data list ..
                                        DisplayBase.CanJobUpdate = true;


                                        view_machine_data.Dispose();
                                        jtable.Dispose();
                                        jds.Dispose();
                                    }
                                }
                                catch (Exception ex)
                                {
                                    // response error result ..
                                    __returnObject = ServerObject.Response(ServerReponse.ResponseSign.MINUSERR);
                                    string ExceptionMsg = ex.Message + ex.StackTrace;
                                    // write to log file ..
                                    EnvLog.Writer(AssemblyInfoClass.ProductInfo, Log.Level.Error, ExceptionMsg, true);
                                }
                                break;
                            #endregion

                            #region --F7300290--OK--  View Job History Record Case Workflow
                            case Client2Server.CommunicationType.JOBHISTORYRECORD:
                                try
                                {
                                    int SeleectNum = 100;
                                    if (QueueItems.ContainsKey("Num"))
                                    {
                                        SeleectNum = Convert.ToInt32(QueueItems["Num"].ToString().Trim());
                                    }

                                    DataTable Job_History = this.EnvData.FindData("Job_History", "", "Submit_Time", DataViewRowState.Unchanged).ToTable();

                                    var JobRecord = (from records in Job_History.AsEnumerable()
                                                     orderby records.Field<DateTime>("Submit_Time") descending
                                                     select records).Take(SeleectNum);

                                    DataTable JobHistory = this.EnvData.ReadDataSchema("Job_History");

                                    foreach (var record in JobRecord)
                                    {
                                        DataRow dr = JobHistory.NewRow();

                                        for (int i = 0; i < JobHistory.Columns.Count; i++)
                                        {
                                            // assign machine ..
                                            dr[i] = record[i].ToString();
                                        }
                                        JobHistory.Rows.Add(dr);
                                    }

                                    JobHistory.Columns.Remove("Submit_Acct");
                                    JobHistory.Columns.Remove("Submit_Time");

                                    // response result ..
                                    if (JobHistory.Rows.Count > 0)
                                    {
                                        __returnObject = ServerObject.Response(ServerReponse.ResponseSign.PLUSOK, JobHistory);
                                    }
                                    else
                                    {
                                        __returnObject = ServerObject.Response(ServerReponse.ResponseSign.MINUSERR, null);
                                    }

                                    Job_History.Dispose();
                                    JobHistory.Dispose();
                                }
                                catch (Exception ex)
                                {
                                    // response error result ..
                                    __returnObject = ServerObject.Response(ServerReponse.ResponseSign.MINUSERR);

                                    string ExceptionMsg = ex.Message + ex.StackTrace;

                                    // write to log file ..
                                    EnvLog.Writer(AssemblyInfoClass.ProductInfo, Log.Level.Error, ExceptionMsg, true);
                                }
                                break;
                            #endregion

                            // Get Detail through HistoryForm
                            #region --F7300290--OK-- S:View Job_Group_Info by Job_Group_ID Case Workflow
                            case Client2Server.CommunicationType.VIEWSINGLEJOBINFO:
                                try
                                {
                                    // create filter conditions string 
                                    string filter = string.Empty;

                                    if (QueueItems.ContainsKey("Job_Group_Id"))
                                    {
                                        filter = string.Format("Job_Group_Id = '{0}'", QueueItems["Job_Group_Id"]);
                                    }

                                    DataSet Job_Info = new DataSet();

                                    DataTable view_Job_Group = this.EnvData.FindData("Job_Group", filter, "Job_Group_Id", DataViewRowState.Unchanged).ToTable();
                                    DataTable Job_Attr_Info = this.EnvData.FindData("Job_Attr", filter, "", DataViewRowState.Unchanged).ToTable();
                                    DataTable Job_Command_Info = this.EnvData.FindData("Job", filter, "", DataViewRowState.Unchanged).ToTable();

                                    //數據不完整
                                    if (view_Job_Group.Rows.Count == 0 || Job_Attr_Info.Rows.Count == 0 || Job_Command_Info.Rows.Count == 0)
                                    {
                                        __returnObject = ServerObject.Response(ServerReponse.ResponseSign.PLUSOK, null);
                                        break;
                                    }

                                    #region 自定義Job_Group資料結構（去除……加入Command），並獲取數據
                                    DataTable Job_Group_Info = this.EnvData.ReadDataSchema("Job_Group");
                                    Job_Group_Info.Columns.Remove("Submit_Acct");
                                    Job_Group_Info.Columns.Remove("Submit_Machine");
                                    Job_Group_Info.Columns.Remove("Submit_Time");
                                    Job_Group_Info.Columns.Remove("Status");
                                    Job_Group_Info.Columns.Remove("Start_Time");
                                    Job_Group_Info.Columns.Remove("Finish_Time");

                                    Job_Group_Info.Columns.Add("Command", typeof(System.String));

                                    DataRow JobRow = Job_Group_Info.NewRow();
                                    foreach (DataColumn dc in view_Job_Group.Columns)
                                    {
                                        //查詢數據中存在的數據行
                                        if (Job_Group_Info.Columns.Contains(dc.ColumnName) && view_Job_Group.Rows.Count > 0)
                                        {
                                            JobRow[dc.ColumnName] = view_Job_Group.Rows[0][dc.ColumnName];
                                        }
                                    }
                                    string CommandText = Job_Command_Info.Rows[0]["Command"].ToString();
                                    JobRow["Command"] = CommandText;

                                    Job_Group_Info.Rows.Add(JobRow);

                                    #endregion

                                    //合并Job_Attr與Job_Group_Info數據
                                    Job_Info.Tables.Add(Job_Attr_Info);
                                    Job_Info.Tables.Add(Job_Group_Info);

                                    if (Job_Info.Tables.Count > 0)
                                    {
                                        __returnObject = ServerObject.Response(ServerReponse.ResponseSign.PLUSOK, Job_Info);
                                    }
                                    else
                                    {
                                        __returnObject = ServerObject.Response(ServerReponse.ResponseSign.MINUSERR, null);
                                    }

                                    Job_Attr_Info.Dispose();
                                    Job_Command_Info.Dispose();
                                    view_Job_Group.Dispose();
                                    Job_Group_Info.Dispose();
                                    Job_Info.Dispose();

                                }
                                catch (Exception ex)
                                {
                                    // response error result ..
                                    __returnObject = ServerObject.Response(ServerReponse.ResponseSign.MINUSERR);

                                    string ExceptionMsg = ex.Message + ex.StackTrace;

                                    // write to log file ..
                                    EnvLog.Writer(AssemblyInfoClass.ProductInfo, Log.Level.Error, ExceptionMsg, true);
                                }
                                break;
                            #endregion

                            #region --F7300290--OK--View Job Status Case Workflow
                            case Client2Server.CommunicationType.VIEWJOBSTATUS:
                                try
                                {
                                    // read all match data (filter job status completed and error flag) ..
                                    DataView view_job_group_data = this.EnvData.FindData(
                                        "Job_Group",
                                        string.Format("[Status] <> '{0}'", (ushort)JobStatusFlag.COMPLETED),
                                        "Submit_Time", DataViewRowState.Unchanged);

                                    // create empty object data table, include the data schema ..
                                    DataTable
                                        job_attr_table = this.EnvData.ReadDataSchema("Job_Attr"),
                                        job_table = this.EnvData.ReadDataSchema("Job");

                                    foreach (DataRow row in view_job_group_data.Table.Rows)
                                    {
                                        // get job id ..！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！
                                        string id = row["Job_Group_Id"].ToString();

                                        // read and custom match data tbale, add to dataset ..
                                        DataView view_job_attr_data = this.EnvData.FindData(
                                            "Job_Attr",
                                            string.Format("Job_Group_Id = '{0}'", id),
                                            "Job_Group_Id", DataViewRowState.Unchanged);

                                        // merge the attributes to table ..
                                        job_attr_table.Merge(view_job_attr_data.ToTable());

                                        DataView view_job_data = this.EnvData.FindData(
                                            "Job",
                                            string.Format("Job_Group_Id = '{0}'", id),
                                            "Job_Group_Id", DataViewRowState.Unchanged);

                                        // merge the job to table ..
                                        job_table.Merge(view_job_data.ToTable());
                                    }

                                    // create job dataset viewer object ..
                                    DataSet JobDatas = new DataSet("JobViewer");
                                    JobDatas.Tables.Add(view_job_group_data.ToTable());
                                    JobDatas.Tables.Add(job_attr_table);
                                    JobDatas.Tables.Add(job_table);

                                    // commit changes ..
                                    JobDatas.AcceptChanges();

                                    // response result ..
                                    if (JobDatas.Tables.Count > 0)
                                    {
                                        __returnObject = ServerObject.Response(ServerReponse.ResponseSign.PLUSOK, JobDatas);
                                    }
                                    else
                                    {
                                        __returnObject = ServerObject.Response(ServerReponse.ResponseSign.MINUSERR, null);
                                    }

                                    view_job_group_data.Dispose();
                                    job_attr_table.Dispose();
                                    job_table.Dispose();
                                    JobDatas.Dispose();

                                }
                                catch (Exception ex)
                                {
                                    // response error result ..
                                    __returnObject = ServerObject.Response(ServerReponse.ResponseSign.MINUSERR);

                                    string ExceptionMsg = ex.Message + ex.StackTrace;

                                    // write to log file ..
                                    EnvLog.Writer(AssemblyInfoClass.ProductInfo, Log.Level.Error, ExceptionMsg, true);
                                }
                                break;
                            #endregion

                            #region View Job Process Output Case Workflow  ！！！！！！未測試！！！！！！！！！
                            case Client2Server.CommunicationType.VIEWJOBOUTPUT:
                                try
                                {
                                    // find match data ..
                                    DataView view_job_data = this.EnvData.FindData("Job", QueueItems, DataViewRowState.Unchanged);

                                    // response result ..
                                    if (view_job_data.Count > 0)
                                    {
                                        __returnObject = ServerObject.Response(ServerReponse.ResponseSign.PLUSOK, view_job_data.ToTable());
                                    }
                                    else
                                    {
                                        __returnObject = ServerObject.Response(ServerReponse.ResponseSign.MINUSERR, null);
                                    }

                                    view_job_data.Dispose();
                                }
                                catch (Exception ex)
                                {
                                    // response error result ..
                                    __returnObject = ServerObject.Response(ServerReponse.ResponseSign.MINUSERR);
                                    string ExceptionMsg = ex.Message + ex.StackTrace;

                                    // write to log file ..
                                    EnvLog.Writer(AssemblyInfoClass.ProductInfo, Log.Level.Error, ExceptionMsg, true);
                                }
                                break;
                            #endregion
                        }

                        #endregion

                        // response object ..
                        client.Send(this.EnvSvr.Serialize(__returnObject));

                        
                        QueueItems.Clear();
                        ActionHeader = Client2Server.CommunicationType.NONE;
                    }
                    #endregion
                    // delay 0.2 second to next execute ..
                    Thread.Sleep(200);
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
                // close the signle client object ..
                client.Close();
            }
        }
        #endregion
    }
}