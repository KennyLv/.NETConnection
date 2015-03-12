#region Using NameSpace
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;

// import renbar server library namespace ..
using RenbarLib.Environment;
using RenbarLib.Network;
using RenbarLib.Network.Protocol;
using RenbarLib.Network.Sockets;

// import render server properties namespace ..
using RenderServerGUI.Properties;
#endregion

namespace RenderServerGUI
{
    public class RenderBase : IDisposable
    {
        #region Declare Global Variable Section定義全局變量Section
        // define default settings ..//定義默認設置
        private string __ConnectServerIp = "192.168.1.139";
        private ushort __ConnectServerPort = 6600;//
        private ushort __ListenRenderPort = 6700;
        
        // declare machine maintenance and offline flag ..//定義機器維護和離線標志
        private volatile bool __IsMaintenance = false;
        private volatile bool __IsOffLine = false;

        // declare renbar environment class object ..//定義Renbar環境類對象
        private HostBase EnvHostBase = new HostBase();//network class
        private Service EnvSvr = new Service();
        private Log EnvLog = new Log();//log class

        // declare transport protocol object ..//定義傳輸協議對象
        private Server2Render RenderObject = new Server2Render();//Server-to-RenderFarm
        private ServerReponse ServerObject = new ServerReponse();

        // declare queue data list ..//定義隊列數據列
        private volatile DataTable QueueDataTable = null;
        private volatile IDictionary<int, StringBuilder> ProcOutputLog = null;

        // declare render process collection ..//定義Render進程集合
        private volatile IDictionary<int, int> RenderProcess = null;
        private volatile IList<int> FailProcess = null;

        // declare usage core variable ..//定義usage關鍵變量
        private volatile ushort UsageCore = 0;

        // stop running thread flag ..//停止運行線程標志
        private volatile bool requestStop = false;

        private string ServerListFilePath = string.Empty;

        private int successMark = -1;
        private int MaintenanceMark = -1;

        #endregion

        #region Render Base Constructor And Destructor Procedure//Render Base構造函數和析構函數
        /// <summary>
        /// Render base constructor.
        /// </summary>
        public RenderBase()
        {
            #region 读取配置文件
            // checking connect server ip address ..//查看連接服務器IP地址
            if (this.__ConnectServerIp != Settings.Default.ConnectServerIp)
            {
                this.__ConnectServerIp = Settings.Default.ConnectServerIp;
            }
            // checking connect server port ..//查看連接服務器端口
            if (this.__ConnectServerPort != Settings.Default.ConnectServerPort)
            {
                this.__ConnectServerPort = Settings.Default.ConnectServerPort;
            }
            // checking render listen port ..//查看Render監聽端口
            if (this.__ListenRenderPort != Settings.Default.ListenRenderPort)
            {
                this.__ListenRenderPort = Settings.Default.ListenRenderPort;
            }

            //设定链接切换服务器地址
            this.ServerListFilePath = Settings.Default.Path;

            if (new ScanPort().Scan(__ConnectServerIp, __ConnectServerPort))
            {
                RenderEvents.AppendLog(string.Format("{0}", "Master/Slave IP:" + __ConnectServerIp.ToString() + "  Connect  Success."));
            }
            else
            {
                foreach (RenbarServer CurrentServer in this.EnvSvr.GetServers(this.ServerListFilePath))
                {
                    if (new ScanPort().Scan(CurrentServer.ServerIP, CurrentServer.ServerPort))
                    {
                        this.__ConnectServerIp = Convert.ToString(CurrentServer.ServerIP);
                        this.__ConnectServerPort = CurrentServer.ServerPort;
                        RenderEvents.AppendLog(string.Format("{0}", "Master/Slave IP:" + __ConnectServerIp.ToString() + "  Connect  Success."));
                        Settings.Default.ConnectServerIp = this.__ConnectServerIp;
                        Settings.Default.ConnectServerPort = this.__ConnectServerPort;
                        Settings.Default.Save();
                        break;
                    }
                }
            }
            
            #endregion


            // create receive process stream output ..//創建接收process流輸出
            this.ProcOutputLog = new Dictionary<int, StringBuilder>();

            // create render process collection ..//創建Render Process集合
            this.RenderProcess = new Dictionary<int, int>();
            this.FailProcess = new List<int>();

            // create queue data list instance, and define columns ..//創建隊列數據列Instance，并且定義列
            QueueDataTable = new DataTable("QueueJobsTable");
            QueueDataTable.Columns.Add(new DataColumn("Job_Group_Id", typeof(string)));
            QueueDataTable.Columns.Add(new DataColumn("Job_Id", typeof(string)));

            QueueDataTable.Columns.Add(new DataColumn("Proc_Id", typeof(int)));
            QueueDataTable.Columns.Add(new DataColumn("Proc_Type", typeof(string)));

            QueueDataTable.Columns.Add(new DataColumn("Command", typeof(string)));
            QueueDataTable.Columns.Add(new DataColumn("Args", typeof(string)));
            QueueDataTable.Columns.Add(new DataColumn("Status", typeof(string)));
            QueueDataTable.Columns.Add(new DataColumn("Start_Time", typeof(DateTime)));
            QueueDataTable.Columns.Add(new DataColumn("Finish_Time", typeof(DateTime)));
            QueueDataTable.Columns.Add(new DataColumn("Render_Output", typeof(StringBuilder)));

            // setting queue datatable primary keys ..//設置隊列數據表主鍵
            QueueDataTable.PrimaryKey = new DataColumn[]
            {
                QueueDataTable.Columns["Job_Id"]
            };

            // append to logs object ..//追加到日志對象
            RenderEvents.AppendLog(string.Format("{0}", "create job queue object has completed."));
            // start machine registry thread ..//開始機器注冊線程
            //this.RegistryMachine(true);
            Thread _Registry = new Thread(new ParameterizedThreadStart(RegistryMachine));
            _Registry.IsBackground = true;
            _Registry.Start(true);

            // initialize render listener thread ..//初始化Render監聽線程
            Thread __QueueServiceThread = new Thread(new ThreadStart(this.QueueService));
            __QueueServiceThread.SetApartmentState(ApartmentState.MTA);//啟動執行緒以前，
            //GetApartmentState會顯示初始的 ApartmentState.Unknown 狀態，而 SetApartmentState
            //會將狀態變更為ApartmentState.STA。含有一個以上thread的多thread Apartment (MTA)。
            __QueueServiceThread.IsBackground = false;//指出线程是不是背景线程,背景thread不會防止處理序終止
            __QueueServiceThread.Priority = ThreadPriority.Highest;
            __QueueServiceThread.Start();

            // append to logs object ..//追加到日志對象
            RenderEvents.AppendLog(string.Format("{0}", "strat queue service thread."));
            
        }

        /// <summary>
        /// Render base destructor.
        /// </summary>
        ~RenderBase()
        {
            // collect all generations of memory ..//收集所有內存產生
            GC.Collect();
        }
        #endregion

        #region Clean Up Resource Method Procedure清空資源方法過程
        /// <summary>
        /// Clean up any resources being used.//清空任何被使用資源
        /// </summary>
        public void Dispose()
        {
            // clean up base resource ..//清空Base資源
            this.Dispose(true);

            // this object will be cleaned up by the dispose method ..//這個對象將由Dispose方法清空
            
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Clean up any resources being used.//清空任何被使用資源
        /// </summary>
        /// <param name="disposing">true if server base object should be disposed; otherwise, false.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // stop running threads ..//停止運行線程
                this.requestStop = true;
                

                // append to logs object ..//追加到日志對象
                RenderEvents.AppendLog(string.Format("{0}", "disponse render base class."));
            }
        }
        #endregion

        #region Registry Machine Status Procedure注冊機器狀態過程
        /// <summary>
        /// Registry machine status.//注冊機器狀態
        /// </summary>
        internal void RegistryMachine(object IsInit)
        {


            // append to logs object ..//追加到日志對象
            RenderEvents.AppendLog(string.Format("{0}", "machine registry procedure."));

            // create client to server protocol class ..//創建客戶端到服務器協議類
            Client2Server ClientObject = new Client2Server();//JOBQUEUEADD,JOBQUEUEDELETE等

            // declare registry machine information socket ..//定義注冊機器信息socket
            TcpClientSocket RegistrySocket = null;

            // declare machine column items ..//定義機器列項
            IDictionary<string, object> MachineInfo = null;

            //int count= 0;

            if (!Convert.ToBoolean(IsInit))
            {
                successMark = -1;
            }
            try
            {
                do
                {
                    if (successMark != 1)
                    {
                        if (!new ScanPort().Scan(__ConnectServerIp, __ConnectServerPort))
                        {
                            foreach (RenbarServer CurrentServer in this.EnvSvr.GetServers(this.ServerListFilePath))
                            {
                                if (new ScanPort().Scan(CurrentServer.ServerIP, CurrentServer.ServerPort))
                                {
                                    this.__ConnectServerIp = Convert.ToString(CurrentServer.ServerIP);
                                    this.__ConnectServerPort = CurrentServer.ServerPort;
                                    RenderEvents.AppendLog(string.Format("{0}", "Master/Slave IP:" + __ConnectServerIp.ToString() + "  Connect  Success."));
                                    Settings.Default.ConnectServerIp = this.__ConnectServerIp;
                                    Settings.Default.ConnectServerPort = this.__ConnectServerPort;
                                    Settings.Default.Save();
                                    break;
                                }
                            }
                        }
                        // create registry machine information socket ..//創建注冊機器信息socket,6600
                        RegistrySocket = new TcpClientSocket(IPAddress.Parse(this.__ConnectServerIp), this.__ConnectServerPort);
                        //RegistrySocket = new TcpClientSocket(this.EnvSetting.ServerIpAddress, this.EnvSetting.ServerPort);

                        if (RegistrySocket.IsConnected)
                        {

                            // registry information ..//注冊信息
                            MachineInfo = new Dictionary<string, object>
                            {
                                {"Machine_Id", string.Empty},
                                {"Name", EnvHostBase.LocalHostName},
                                {"Ip", EnvHostBase.LocalIpAddress},
                                {"IsRender", true},
                                {"IsMaintenance", this.__IsMaintenance}
                            };

                            if (Convert.ToBoolean(IsInit))
                            {
                                MachineInfo.Add("IsEnable", true);
                                MachineInfo.Add("Last_Online_Time", DateTime.Now);
                            }
                            //是否勾選維護，添加NOTE值不同。
                            if (this.__IsMaintenance)
                            {
                                if (!MachineInfo.ContainsKey("Note"))
                                    MachineInfo.Add("Note", "the machine stop render service.");
                                else
                                    MachineInfo["Note"] = "the machine stop render service.";

                                // stops all process ..
                                //this.KillProcess();
                            }
                            else
                                MachineInfo.Add("Note", null);

                            if (this.__IsOffLine)
                            {
                                MachineInfo.Add("IsOffLine", true);

                                if (!MachineInfo.ContainsKey("Note"))
                                    MachineInfo.Add("Note", null);
                                else
                                    MachineInfo["Note"] = null;
                            }

                            // serialize object ..//系列化對象,ClientObject(Renbar2 Control Protocol)
                            byte[] ByteData = this.EnvSvr.Serialize(ClientObject.Package(Client2Server.CommunicationType.MACHINEINFO, MachineInfo));

                            // send object to remote server ..//發送對象到遠程服務器
                            RegistrySocket.Send(ByteData);
                            object Data = new object();
                            //if (Convert.ToBoolean(IsInit))
                            //{
                                // deserialize object ..//反系列化對象
                                Data = this.EnvSvr.Deserialize(RegistrySocket.Receive());
                            //}
                            //else
                            //{
                            //    // deserialize object ..//反系列化對象
                            //    Data = this.EnvSvr.Deserialize(RegistrySocket.Receive1());
                            //}

                            if (null != Data)
                            {
                                // receive server message ..//接受服務器信息
                                KeyValuePair<string, object> received = (KeyValuePair<string, object>)Data;

                                if (received.Key.Substring(0, 1).Equals("+"))//+ (sign="+Ok")
                                // append to logs object ..//追加到日志對象
                                {
                                    RenderEvents.AppendLog(string.Format("{0}", "registry machine successful."));
                                    successMark = 1;
                                    break;
                                }
                                else
                                // append to logs object ..//追加到日志對象
                                {
                                    RenderEvents.AppendLog(string.Format("{0}", "registry machine fail."));
                                }
                            }
                            else
                            // append to logs object ..//追加到日志對象
                            {
                                RenderEvents.AppendLog(string.Format("{0}", "registry machine fail."));
                            }

                        }
                        else
                        // append to logs object ..//追加到日志對象
                        {
                            RenderEvents.AppendLog(string.Format("{0}", "can't connect server, registry fail."));
                            //count++;
                            //if (count > 2)
                            //{
                            //    successMark = -1;
                            //}
                        }
                    }
                    if (!Convert.ToBoolean(IsInit))
                    {
                        break;
                    }
                    Thread.Sleep(3000);
                } while (!requestStop);
            }

            catch (Exception ex)
            {
                // append to logs object ..//追加到日志對象
                RenderEvents.AppendLog(string.Format("{0}", "registry machine info has happen exception error."));

                string ExceptionMsg = ex.Message + ex.StackTrace;

                // write to log file ..//寫到日志對象
                this.EnvLog.Writer(AssemblyInfoClass.ProductInfo, Log.Level.Error, ExceptionMsg, true);
            }
            finally
            {
                if (MachineInfo != null)
                    MachineInfo.Clear();

                if (RegistrySocket != null)
                    // disconnect socket ..//斷開socket
                    RegistrySocket.Close();
            }
        }
        #endregion

        #region Registry Machine Status Procedure Maintenance注冊機器狀態過程(Maintenance)
        /// <summary>
        /// Registry machine status.//注冊機器狀態
        /// </summary>
        internal void RegistryMachine1(object IsInit)
        {
            if (!Convert.ToBoolean(IsInit))
            {
                MaintenanceMark = -1;
            }
            // create client to server protocol class ..//創建客戶端到服務器協議類
            Client2Server ClientObject = new Client2Server();//JOBQUEUEADD,JOBQUEUEDELETE等

            // declare registry machine information socket ..//定義注冊機器信息socket
            TcpClientSocket RegistrySocket = null;

            // declare machine column items ..//定義機器列項
            IDictionary<string, object> MachineInfo = null;

            try
            {
                do
                {
                    if (MaintenanceMark != 1)
                    {
                        if (!new ScanPort().Scan(__ConnectServerIp, __ConnectServerPort))
                        {
                            foreach (RenbarServer CurrentServer in this.EnvSvr.GetServers(this.ServerListFilePath))
                            {
                                if (new ScanPort().Scan(CurrentServer.ServerIP, CurrentServer.ServerPort))
                                {
                                    this.__ConnectServerIp = Convert.ToString(CurrentServer.ServerIP);
                                    this.__ConnectServerPort = CurrentServer.ServerPort;
                                    RenderEvents.AppendLog(string.Format("{0}", "Master/Slave IP:" + __ConnectServerIp.ToString() + "  Connect  Success."));
                                    Settings.Default.ConnectServerIp = this.__ConnectServerIp;
                                    Settings.Default.ConnectServerPort = this.__ConnectServerPort;
                                    Settings.Default.Save();
                                    break;
                                }
                            }
                        }
                        // create registry machine information socket ..//創建注冊機器信息socket,6600
                        RegistrySocket = new TcpClientSocket(IPAddress.Parse(this.__ConnectServerIp), this.__ConnectServerPort);
                        //RegistrySocket = new TcpClientSocket(this.EnvSetting.ServerIpAddress, this.EnvSetting.ServerPort);

                        if (RegistrySocket.IsConnected)
                        {
                            // registry information ..//注冊信息
                            MachineInfo = new Dictionary<string, object>
                            {
                                {"Machine_Id", string.Empty},
                                {"Name", EnvHostBase.LocalHostName},
                                {"Ip", EnvHostBase.LocalIpAddress},
                                {"IsRender", true},
                                {"IsMaintenance", this.__IsMaintenance}
                            };

                            if (Convert.ToBoolean(IsInit))
                            {
                                MachineInfo.Add("IsEnable", true);
                                //LT add
                                MachineInfo.Add("Last_Online_Time", DateTime.Now);
                            }
                            //是否勾選維護，添加NOTE值不同。
                            if (this.__IsMaintenance)
                            {
                                if (!MachineInfo.ContainsKey("Note"))
                                    MachineInfo.Add("Note", "the machine stop render service.");
                                else
                                    MachineInfo["Note"] = "the machine stop render service.";

                                // stops all process ..
                                //this.KillProcess();
                            }
                            else
                                MachineInfo.Add("Note", null);

                            if (this.__IsOffLine)
                            {
                                MachineInfo.Add("IsOffLine", true);

                                if (!MachineInfo.ContainsKey("Note"))
                                    MachineInfo.Add("Note", null);
                                else
                                    MachineInfo["Note"] = null;
                            }

                            // serialize object ..//系列化對象,ClientObject(Renbar2 Control Protocol)
                            byte[] ByteData = this.EnvSvr.Serialize(ClientObject.Package(Client2Server.CommunicationType.MACHINEINFO, MachineInfo));

                            // send object to remote server ..//發送對象到遠程服務器
                            RegistrySocket.Send(ByteData);

                            // deserialize object ..//反系列化對象
                            object Data = this.EnvSvr.Deserialize(RegistrySocket.Receive());

                            //if (null != Data)
                            //{
                            //    // receive server message ..//接受服務器信息
                            //    KeyValuePair<string, object> received = (KeyValuePair<string, object>)Data;
                            //}

                            if (null != Data)
                            {
                                // receive server message ..//接受服務器信息
                                KeyValuePair<string, object> received = (KeyValuePair<string, object>)Data;

                                if (received.Key.Substring(0, 1).Equals("+"))//+ (sign="+Ok")
                                // append to logs object ..//追加到日志對象
                                {
                                    RenderEvents.AppendLog(string.Format("{0}", "registry machine successful."));
                                    break;
                                }
                                else
                                // append to logs object ..//追加到日志對象
                                {
                                    RenderEvents.AppendLog(string.Format("{0}", "registry machine fail."));
                                }
                            }
                            else
                            // append to logs object ..//追加到日志對象
                            {
                                RenderEvents.AppendLog(string.Format("{0}", "registry machine fail."));
                            }
                        }
                        else
                        // append to logs object ..//追加到日志對象
                        {
                            RenderEvents.AppendLog(string.Format("{0}", "can't connect server, registry fail."));
                        }

                    }
                } while (!requestStop);
            }

            catch (Exception ex)
            {
                // append to logs object ..//追加到日志對象
                RenderEvents.AppendLog(string.Format("{0}", "registry machine info has happen exception error."));

                string ExceptionMsg = ex.Message + ex.StackTrace;

                // write to log file ..//寫到日志對象
                this.EnvLog.Writer(AssemblyInfoClass.ProductInfo, Log.Level.Error, ExceptionMsg, true);
            }
            finally
            {
                if (MachineInfo != null)
                    MachineInfo.Clear();

                if (RegistrySocket != null)
                    // disconnect socket ..//斷開socket
                    RegistrySocket.Close();
            }

        }
        #endregion

        #region Close Machine Status Procedure Close機器狀態過程(again)
        ///// <summary>
        ///// Registry machine status.//注冊機器狀態
        ///// </summary>
        //internal void RegistryMachine2(object IsInit)
        //{
        //    // create client to server protocol class ..//創建客戶端到服務器協議類
        //    Client2Server ClientObject = new Client2Server();//JOBQUEUEADD,JOBQUEUEDELETE等

        //    // declare registry machine information socket ..//定義注冊機器信息socket
        //    TcpClientSocket RegistrySocket = null;

        //    // declare machine column items ..//定義機器列項
        //    IDictionary<string, object> MachineInfo = null;

        //    try
        //    {
        //        if (!new ScanPort().Scan(__ConnectServerIp, __ConnectServerPort))
        //        {
        //            foreach (RenbarServer CurrentServer in this.EnvSvr.GetServers(this.ServerListFilePath))
        //            {
        //                if (new ScanPort().Scan(CurrentServer.ServerIP, CurrentServer.ServerPort))
        //                {
        //                    this.__ConnectServerIp = Convert.ToString(CurrentServer.ServerIP);
        //                    this.__ConnectServerPort = CurrentServer.ServerPort;
        //                    RenderEvents.AppendLog(string.Format("{0}", "Master/Slave IP:" + __ConnectServerIp.ToString() + "  Connect  Success."));
        //                    Settings.Default.ConnectServerIp = this.__ConnectServerIp;
        //                    Settings.Default.ConnectServerPort = this.__ConnectServerPort;
        //                    Settings.Default.Save();
        //                    break;
        //                }
        //            }
        //        }
        //        // create registry machine information socket ..//創建注冊機器信息socket,6600
        //        RegistrySocket = new TcpClientSocket(IPAddress.Parse(this.__ConnectServerIp), this.__ConnectServerPort);
        //        //RegistrySocket = new TcpClientSocket(this.EnvSetting.ServerIpAddress, this.EnvSetting.ServerPort);

        //        if (RegistrySocket.IsConnected)
        //        {
        //            // registry information ..//注冊信息
        //            MachineInfo = new Dictionary<string, object>
        //            {
        //                {"Machine_Id", string.Empty},
        //                {"Name", EnvHostBase.LocalHostName},
        //                {"Ip", EnvHostBase.LocalIpAddress},
        //                {"IsRender", true},
        //                {"IsMaintenance", this.__IsMaintenance}
        //            };

        //            if (Convert.ToBoolean(IsInit))
        //            {
        //                MachineInfo.Add("IsEnable", true);
        //                //LT add
        //                MachineInfo.Add("Last_Online_Time", DateTime.Now);
        //            }
        //            //是否勾選維護，添加NOTE值不同。
        //            if (this.__IsMaintenance)
        //            {
        //                if (!MachineInfo.ContainsKey("Note"))
        //                    MachineInfo.Add("Note", "the machine stop render service.");
        //                else
        //                    MachineInfo["Note"] = "the machine stop render service.";

        //                // stops all process ..
        //                //this.KillProcess();
        //            }
        //            else
        //                MachineInfo.Add("Note", null);

        //            if (this.__IsOffLine)
        //            {
        //                MachineInfo.Add("IsOffLine", true);

        //                if (!MachineInfo.ContainsKey("Note"))
        //                    MachineInfo.Add("Note", null);
        //                else
        //                    MachineInfo["Note"] = null;
        //            }

        //            // serialize object ..//系列化對象,ClientObject(Renbar2 Control Protocol)
        //            byte[] ByteData = this.EnvSvr.Serialize(ClientObject.Package(Client2Server.CommunicationType.MACHINEINFO, MachineInfo));

        //            // send object to remote server ..//發送對象到遠程服務器

        //            RegistrySocket.Send(ByteData);

        //            // deserialize object ..//反系列化對象
        //            object Data = this.EnvSvr.Deserialize(RegistrySocket.Receive1());

        //            if (null != Data)
        //            {
        //                // receive server message ..//接受服務器信息
        //                KeyValuePair<string, object> received = (KeyValuePair<string, object>)Data;
        //                successMark = -1;
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        // append to logs object ..//追加到日志對象
        //        RenderEvents.AppendLog(string.Format("{0}", "registry machine info has happen exception error."));

        //        string ExceptionMsg = ex.Message + ex.StackTrace;

        //        // write to log file ..//寫到日志對象
        //        this.EnvLog.Writer(AssemblyInfoClass.ProductInfo, Log.Level.Error, ExceptionMsg, true);
        //    }
        //    finally
        //    {
        //        if (MachineInfo != null)
        //            MachineInfo.Clear();

        //        if (RegistrySocket != null)
        //            // disconnect socket ..//斷開socket
        //            RegistrySocket.Close();
        //    }
        //}
        #endregion

        #region Stops Currently Process停止黨前Process
        /// <summary>
        /// Immediately stops the associated process.//立即停止關聯進程
        /// </summary>
        private void KillProcess()
        {
            // find all process ..//查找所有進程
            foreach (KeyValuePair<int, int> kv in this.RenderProcess)
                // kill process ..//殺進程
                Process.GetProcessById(kv.Value).Kill();
        }
        #endregion

        #region Machine Maintenance, Offline Status Properties機器維護，下線狀態Properties
        /// <summary>
        /// Set the machine offline status.//設置機器下線狀態(216行)
        /// </summary>
        internal bool IsOffLine
        {
            set
            {
                this.__IsOffLine = true;
            }
        }

        /// <summary>
        /// Set the machine maintenance status.//設置機器維護狀態(201行)
        /// </summary>
        internal bool IsMaintenance
        {
            set
            {
                this.__IsMaintenance = value;
            }
        }
        #endregion

        #region Get Queue Status Property得到隊列狀態Property
        /// <summary>
        /// Get queue data display information.//得到隊列顯示信息(Main_Form.cs調用119行)
        /// </summary>
        internal DataTable GetQueueStatus
        {
            get
            {
                DataTable __queueData = new DataTable();

                lock (this.QueueDataTable)
                    // copy current data to temporary datatable ..//復制黨前的數據到臨時數據表
                    __queueData = this.QueueDataTable.Copy();

                return __queueData.Copy();
            }
        }

        /// <summary>
        /// Get or set data change status.//得到或者設置數據改變狀態(Main_Form.cs用116行)
        /// </summary>
        internal bool HasChange
        {
            get;
            set;
        }
        #endregion

        #region Queue Request Maintenance Procedure隊列請求維護Procedure
        /// <summary>
        /// Primary accept server request maintenance thread method.
        /// </summary>
        private void QueueService()
        {
            // declare machine render workflow service ..//定義機器Render工作流服務
            TcpServerSocket QueueServiceSocket = null;

            try
            {
                // create object instance ..//創建對象Instance,6700(Render farm)
                QueueServiceSocket = new TcpServerSocket(this.EnvHostBase.LocalIpAddress, this.__ListenRenderPort);

                // append to logs object ..//追加到日志對象
                RenderEvents.AppendLog(string.Format("start queue listen service. ip: {0} port: {1}", this.EnvHostBase.LocalIpAddress, this.__ListenRenderPort));
                
                // confirm connect requests ..//確認連接請求,如果有連接正在暫止中,則為true，否則為false
                do
                {
                    if (QueueServiceSocket.Pending())
                    {
                        // accept a new connect request ..//接受一個新連接請求
                        global::System.Net.Sockets.TcpClient handle = QueueServiceSocket.AcceptListen();
                        //即可執行回呼方法。(队列要执行的方法。可以使用线程集区线程时，即可执行这个方法。)
                        ThreadPool.QueueUserWorkItem(new WaitCallback(this.Protocol), handle);//調用Protocol
                        break;
                    }
                }while(!this.requestStop);

                //与server端保持联系(梁涛修改)
                do
                {
                     // confirm connect requests ..//確認連接請求,如果有連接正在暫止中,則為true，否則為false
                    if (QueueServiceSocket.Pending())
                    {
                    } 
                    // delay 0.2 second to next listening ..//延遲0.2秒下一監聽
                    Thread.Sleep(200);
                } while (!this.requestStop);
            }
            catch (Exception ex)
            {
                string ExceptionMsg = ex.Message + ex.StackTrace;

                // write to log file ..//寫到日志文件
                this.EnvLog.Writer(AssemblyInfoClass.ProductInfo, Log.Level.Error, ExceptionMsg, true);
            }
            finally
            {
                if (QueueServiceSocket != null)
                    // clean server socket resource ..//清空服務器Socket資源
                    QueueServiceSocket.Close();
            }
        }

        /// <summary>
        /// Server2Render signle protocol event procedure.//Server2Render單個協議事件過程
        /// </summary>
        /// <param name="ClientObject">accept client object.//JOBQUEUEADD,JOBQUEUEDELETE等(Renbar2 Control Protocol 174line)</param>
        private void Protocol(object ClientObject)
        {
            // create single client stream object ..//創建單個客戶流對象
            SingleClient client = new SingleClient(ClientObject);

            // declare receive remote object ..//定義接受遠程對象
            object received = null;

            try
            {
                while (!requestStop)
                {
                    if ((received = this.EnvSvr.Deserialize(client.Receive())) != null)//將Control Server端接受并反系列化
                    {
                        #region Analysis Remote Object Package分析遠程對象包
                        // declare server response object list interface ..//宣布服務器回應對象列接口
                        KeyValuePair<string, object> __returnObject = new KeyValuePair<string, object>();

                        // define default communication enumeration ..//定義默認communication枚舉(COMPLETEDJOBS等六種狀態)
                        Server2Render.CommunicationType ActionHeader = Server2Render.CommunicationType.NONE;
                        IDictionary<string, object> QueueItems = new Dictionary<string, object>();

                        // unpackage remote object data ..//解包遠程對象數據
                        this.RenderObject.UnPackage((IList<object>)received, out ActionHeader, out QueueItems);
                        #endregion

                        // append to logs object ..//追加到日志對象
                        RenderEvents.AppendLog(string.Format("receive server \"{0}\" action behavior.", ActionHeader.ToString()));

                        switch (ActionHeader)
                        {
                            
                            #region Completed Jobs Workflow完成工作流
                            /*开启监听接受server端请求的数据，如果是请求completedjobs
                               *则首先建立一张需要寄送的templetedatatable，然后从queuedatatable
                               *取出status=completed or status=error的数据存入templetedatatable
                               *然后清理queuedatatable中被挑选出来的数据
                               *然后寄出templetedatatable到server端
                               */
                            case Server2Render.CommunicationType.COMPLETEDJOBS:
                                try
                                {
                                    // append to logs object ..//追加到日志對象0
                                    RenderEvents.AppendLog(string.Format("{0}", "search completed type job behavior workflow."));

                                    // create completed datatable ..//創建完成數據表
                                    DataTable Completed = new DataTable("Completed");
                                    Completed.Columns.Add(new DataColumn("Job_Group_Id", typeof(string)));
                                    //Completed.Columns.Add(new DataColumn("Job_Id", typeof(int)));
                                    Completed.Columns.Add(new DataColumn("Job_Id", typeof(string)));
                                    Completed.Columns.Add(new DataColumn("Status", typeof(JobStatusFlag)));
                                    Completed.Columns.Add(new DataColumn("Start_Time", typeof(DateTime)));
                                    Completed.Columns.Add(new DataColumn("Finish_Time", typeof(DateTime)));

                                    if (QueueDataTable.Rows.Count > 0)
                                    {
                                        // select completed status flag ..//查看完成狀態標志
                                        DataRow[] completed_rows = QueueDataTable.Select(string.Format("(Status = '{0}' Or Status = '{1}')", JobStatusFlag.COMPLETED, JobStatusFlag.ERROR));

                                        for (int i = 0; i < completed_rows.Length; i++)
                                        {
                                            // add to completed datatable ..//添加到完成數據表
                                            DataRow newRow = Completed.NewRow();
                                            newRow["Job_Group_Id"] = completed_rows[i]["Job_Group_Id"].ToString().ToUpper();
                                            //newRow["Job_Id"] = Convert.ToInt32(completed_rows[i]["Job_Id"]);
                                            newRow["Job_Id"] = Convert.ToString(completed_rows[i]["Job_Id"]);
                                            newRow["Finish_Time"] = Convert.ToDateTime(completed_rows[i]["Finish_Time"]);

                                            if (completed_rows[i]["Status"].ToString() == JobStatusFlag.ERROR.ToString())
                                            {
                                                newRow["Status"] = JobStatusFlag.ERROR;
                                                newRow["Start_Time"] = Convert.ToDateTime(completed_rows[i]["Start_Time"]);
                                            }

                                            Completed.Rows.Add(newRow);
                                        }

                                        // commit data changes ..//提交數據改變
                                        Completed.AcceptChanges();

                                        #region Cleanup Data Procedure清除數據過程
                                        // declare cleaned count variable ..//定義cleaned計算變量
                                        ushort cleaned = 0;

                                        // cleanup completed data ..//清空完成數據
                                        for (int i = 0; i < completed_rows.Length; i++)
                                        {
                                            DateTime canDeleted = Convert.ToDateTime(completed_rows[i]["Finish_Time"]);
                                            TimeSpan lockspan = new TimeSpan(DateTime.Now.Ticks);

                                            // check whether the machine had been locked more than 5 minute ..//查看機器被鎖是否超過5分鐘
                                            if (lockspan.Subtract(new TimeSpan(canDeleted.Ticks)).Duration().TotalMilliseconds >= 300000)
                                            {
                                                // remove completed row ..//移除完成行
                                                completed_rows[i].Delete();

                                                // incremental count ..//增量計算
                                                cleaned++;
                                            }
                                        }

                                        // commit changes ..//提交改變
                                        this.QueueDataTable.AcceptChanges();

                                        // append to logs object ..//追加到日志對象
                                        RenderEvents.AppendLog(string.Format("has success cleanup {0} data.", cleaned));
                                        #endregion

                                        // set last change flag ..//設置上一次改變標志
                                        this.HasChange = true;
                                    }

                                    // response result ..//回應結果
                                    __returnObject = ServerObject.Response(ServerReponse.ResponseSign.PLUSOK, Completed);

                                    // append to logs object ..//追加到日志對象
                                    RenderEvents.AppendLog(string.Format("{0} complete object count: {1}", "successful search completed type job behavior workflow.", Completed.Rows.Count));
                                }
                                catch (Exception ex)
                                {
                                    // response result ..//回應結果
                                    __returnObject = ServerObject.Response(ServerReponse.ResponseSign.MINUSERR);

                                    // append to logs object ..//追加到日志對象
                                    RenderEvents.AppendLog(string.Format("{0}", "executing completed workflow has happen exception error."));

                                    string ExceptionMsg = ex.Message + ex.StackTrace;

                                    // write to log file ..//寫到日志文件
                                    this.EnvLog.Writer(AssemblyInfoClass.ProductInfo, Log.Level.Error, ExceptionMsg, true);
                                }
                                finally
                                {
                                    // response object ..//回應對象
                                    client.Send(this.EnvSvr.Serialize(__returnObject));
                                }
                                break;
                            #endregion

                            #region Deleted Jobs Workflow刪除工作流
                            /*如果从server传过来的指令是deletejob
                                 * 那么取出要删除的job列表List<int>，循环取出各个工作项
                                 * ，从queuedatatable中取出job_id=item and status='processing'
                                 * 然后循环取出proc_id项，kill掉this process
                                 * 减少usagecore数量，从queuedatatable中移除this row
                                 * 返回操作是否成功信息
                                 */
                            case Server2Render.CommunicationType.DELETEJOB:
                                try
                                {
                                    bool result = false;
                                    if (!QueueItems.ContainsKey("Jobs") && ((IList<uint>)QueueItems["Jobs"]).Count <= 0)
                                    {
                                        // response result ..//回應結果, MINUSERR(error flag)
                                        __returnObject = ServerObject.Response(ServerReponse.ResponseSign.MINUSERR);

                                        break;
                                    }

                                    foreach (int i in (IList<uint>)QueueItems["Jobs"])
                                    {   //status為{1}是表示錯誤狀態
                                        string Expression = string.Format("Job_Id = '{0}' And Status = '{1}'",
                                            i, JobStatusFlag.PROCESSING);

                                        if (QueueDataTable.Rows.Count > 0)
                                        {
                                            // select current rendering job process ..//查看黨前Rendering工作進程
                                            DataRow[] delete_rows = QueueDataTable.Select(Expression);

                                            if (delete_rows.Length > 0)
                                            {
                                                // get process key ..//獲得進程主鍵
                                                int key = Convert.ToInt32(delete_rows[0]["Proc_Id"]);

                                                // delete process ..//刪除進程
                                                if (this.RenderProcess.ContainsKey(key))
                                                    // kill mayabatch process ..//殺maya批程序
                                                    Process.GetProcessById(this.RenderProcess[key]).Kill();

                                                // decrement usage core count ..//減少UsageCore總數
                                                this.UsageCore--;

                                                // delete record from job datatable ..//從工作數據表刪除記錄
                                                QueueDataTable.Rows.Remove(delete_rows[0]);

                                                // append to logs object ..//追加到日志對象
                                                RenderEvents.AppendLog(string.Format("{0} deleted job id: {1}, free core: {2}", "successful deleted job behavior workflow.", i, this.UsageCore));
                                            }

                                            // commit changes ..//提交改變
                                            this.QueueDataTable.AcceptChanges();

                                            // change result flag ..//改變結果標志
                                            result = true;
                                        }
                                        else
                                        {
                                            // change result flag ..//改變結果標志
                                            result = false;

                                            break;
                                        }
                                    }

                                    if (result)
                                        // response result ..//回應結果
                                        __returnObject = ServerObject.Response(ServerReponse.ResponseSign.PLUSOK);
                                    else
                                        // response result ..//回應結果
                                        __returnObject = ServerObject.Response(ServerReponse.ResponseSign.MINUSERR);
                                }
                                catch (Exception ex)
                                {
                                    // response result ..//回應結果
                                    __returnObject = ServerObject.Response(ServerReponse.ResponseSign.MINUSERR);

                                    // append to logs object ..//追加到日志對象
                                    RenderEvents.AppendLog(string.Format("{0}", "executing delete job workflow has happen exception error."));

                                    string ExceptionMsg = ex.Message + ex.StackTrace;

                                    // write to log file ..//寫到日志文件
                                    EnvLog.Writer(AssemblyInfoClass.ProductInfo, Log.Level.Error, ExceptionMsg, true);
                                }
                                finally
                                {
                                    // response object ..//回應對象
                                    client.Send(this.EnvSvr.Serialize(__returnObject));
                                }
                                break;
                            #endregion

                            #region Running Job Workflow運行工作流
                            /*如果接受server端发送过来的指令是runningjobs
                                 * 首先添加日志创建临时表running
                                 * 从queuedatatable中选择status=‘PROCESSING’的数据
                                 * 添加到running表中，返回running表到server端
                                 */
                            case Server2Render.CommunicationType.RUNNINGJOBS:
                                try
                                {
                                    // append to logs object ..//追加到日志對象
                                    RenderEvents.AppendLog(string.Format("{0}", "search running type job behavior workflow."));

                                    // create running datatable ..//創建運行數據表
                                    DataTable Running = new DataTable("Running");
                                    Running.Columns.Add(new DataColumn("Job_Group_Id", typeof(string)));
                                    Running.Columns.Add(new DataColumn("Job_Id", typeof(string)));
                                    Running.Columns.Add(new DataColumn("Start_Time", typeof(DateTime)));
                                    Running.Columns.Add(new DataColumn("Render_Output", typeof(string)));

                                    if (QueueDataTable.Rows.Count > 0)
                                    {
                                        // select running status flag ..//查看運行狀態標志
                                        DataRow[] running_rows = QueueDataTable.Select(string.Format("Status = '{0}'", JobStatusFlag.PROCESSING));

                                        for (int i = 0; i < running_rows.Length; i++)
                                        {
                                            // get render output log ..//獲得Render輸出日志
                                            int proc_id = Convert.ToInt32(running_rows[i]["Proc_Id"]);

                                            // add to running datatable ..//添加到運行數據表
                                            DataRow newRow = Running.NewRow();
                                            newRow["Job_Group_Id"] = running_rows[i]["Job_Group_Id"].ToString();
                                            newRow["Job_Id"] = running_rows[i]["Job_Id"].ToString();
                                            newRow["Start_Time"] = Convert.ToDateTime(running_rows[i]["Start_Time"]);

                                            lock (this.ProcOutputLog)
                                            {
                                                if (this.ProcOutputLog.ContainsKey(proc_id))
                                                    newRow["Render_Output"] = this.ProcOutputLog[proc_id];
                                                else
                                                    newRow["Render_Output"] = DBNull.Value;
                                            }

                                            Running.Rows.Add(newRow);
                                        }

                                        // commit data changes ..//提交數據改變
                                        Running.AcceptChanges();
                                    }

                                    // response result ..//回應結果,PLUSOK(Success Flag.)
                                    __returnObject = ServerObject.Response(ServerReponse.ResponseSign.PLUSOK, Running);

                                    // append to logs object ..//追加到日志對象
                                    RenderEvents.AppendLog(string.Format("{0} running object count: {1}", "successful search running type job behavior workflow.", Running.Rows.Count));
                                }
                                catch (Exception ex)
                                {
                                    // response result ..//回應結果
                                    __returnObject = ServerObject.Response(ServerReponse.ResponseSign.MINUSERR);

                                    // append to logs object ..//追加到日志對象
                                    RenderEvents.AppendLog(string.Format("{0}", "executing running workflow has happen exception error."));

                                    string ExceptionMsg = ex.Message + ex.StackTrace;

                                    // write to log file ..//寫到日志文件
                                    this.EnvLog.Writer(AssemblyInfoClass.ProductInfo, Log.Level.Error, ExceptionMsg, true);
                                }
                                finally
                                {
                                    // response object ..//回應對象
                                    client.Send(this.EnvSvr.Serialize(__returnObject));
                                }
                                break;
                            #endregion

                            #region Dispatch Job Workflow分派工作流(關于服務器端添加客戶端發送過來的指令的數據)
                            /*如果接受server端发送过来的是dispatch则首先
                                 * 写入log，取出list<dispatch>,dispatch循环list<dispatch>
                                 * 判断queuedatatable中是否包含该dispatch，如果不存在，则往queuedatatable
                                 * 中添加该dispatch项，启动线程Rendering执行dispatch的工作
                                 * 返回分配job是否成功
                                 */
                            case Server2Render.CommunicationType.DISPATCH:
                                try
                                {
                                    // append to logs object ..//追加到日志對象
                                    RenderEvents.AppendLog(string.Format("{0}", "execute dispatch behavior workflow."));

                                    // analysis remote object ..//分析遠程對象
                                    IList<Dispatch> _obj = (IList<Dispatch>)QueueItems["Dispatcher"];//Dispatch結構體(job_id等)

                                    if (_obj.Count > 0)
                                    {
                                        // add to queue data table ..//添加到隊列數據表
                                        foreach (Dispatch v in _obj)
                                        {
                                            // judge this job has been already exist ..//判斷這工作是否已經存在
                                            if (this.QueueDataTable.Select(
                                                string.Format("Job_Id = {0}", v.Job_Id)).Length > 0)
                                                continue;

                                            DataRow row = this.QueueDataTable.NewRow();
                                            row["Job_Group_Id"] = v.Job_Group_Id.ToString().ToUpper();
                                            row["Job_Id"] = v.Job_Id;
                                            row["Proc_Id"] = -1;
                                            row["Proc_Type"] = v.Proc_Type;
                                            row["Command"] = v.Command;
                                            row["Args"] = v.Args;
                                            row["Status"] = JobStatusFlag.PROCESSING;
                                            row["Start_Time"] = DateTime.Now;
                                            this.QueueDataTable.Rows.Add(row);

                                            // commit changes ..//提交改變
                                            this.QueueDataTable.AcceptChanges();

                                            // render processing ..//Render程序
                                            new Thread(new ParameterizedThreadStart(this.Rendering)).Start(row);//調用Rendering

                                            // append to logs object ..//追加到日志對象
                                            RenderEvents.AppendLog(string.Format("{0} receive job id: {1}", "successful receive job dispatch behavior workflow.", v.Job_Id));
                                        }

                                        // set last change flag ..//設置上一次改變標志
                                        this.HasChange = true;

                                        // response result ..//回應結果
                                        __returnObject = ServerObject.Response(ServerReponse.ResponseSign.PLUSOK);
                                    }
                                    else
                                        // response result ..//回應結果
                                        __returnObject = ServerObject.Response(ServerReponse.ResponseSign.MINUSERR);
                                }
                                catch (Exception ex)
                                {
                                    // response result ..//回應結果
                                    __returnObject = ServerObject.Response(ServerReponse.ResponseSign.MINUSERR);

                                    // append to logs object ..//追加到日志對象
                                    RenderEvents.AppendLog(string.Format("{0}", "executing dispatch workflow has happen exception error."));

                                    string ExceptionMsg = ex.Message + ex.StackTrace;

                                    // write to log file ..//寫到日志文件
                                    this.EnvLog.Writer(AssemblyInfoClass.ProductInfo, Log.Level.Error, ExceptionMsg, true);
                                }
                                finally
                                {
                                    // response object ..//回應對象
                                    client.Send(this.EnvSvr.Serialize(__returnObject));
                                }
                                break;
                            #endregion

                            #region IsBusy Status Workflow忙碌狀態工作流
                            /*如果服务器端发送过来的是isbusy命令，则首先写日志
                                 * 创建dictionary类型的state，
                                 * 判断usagecore是否大于零且queuedatatable是否大于零
                                 * 如果都成立，取出queuedatatable中status=‘processing’的row
                                 * 如果使用usagecore<总数，填充state信息
                                 */
                            case Server2Render.CommunicationType.ISBUSY:
                                try
                                {
                                    // append to logs object ..//追加到日志對象
                                    RenderEvents.AppendLog(string.Format("{0}", "report the machine core behavior workflow."));

                                    IDictionary<string, object> state = null;

                                    DataRow row = this.QueueDataTable.NewRow();

                                    if (this.UsageCore > 0 && this.QueueDataTable.Rows.Count > 0)//如果進程數量與隊列數據表有數據
                                    {
                                        DataRow[] busy_rows = this.QueueDataTable.Select(string.Format("Status = '{0}'", JobStatusFlag.PROCESSING));

                                        if (this.UsageCore < this.EnvSvr.Cores)//判斷進程數量是否小于獲取處理器的數量
                                        {
                                            state = new Dictionary<string, object>
                                                {
                                                    {"IsBusy", true},
                                                    {"ProcessType", busy_rows[0]["Proc_Type"]},
                                                    {"UsageCore", this.UsageCore},
                                                    {"TotalCore", this.EnvSvr.Cores}
                                                };

                                            if (busy_rows[0]["Proc_Type"].ToString().Trim() == "Client")
                                                state["UsageCore"] = this.EnvSvr.Cores;
                                        }
                                        else
                                        {
                                            state = new Dictionary<string, object>
                                                {
                                                    {"IsBusy", true},
                                                    {"ProcessType", busy_rows[0]["Proc_Type"]},
                                                    {"UsageCore", this.EnvSvr.Cores},
                                                    {"TotalCore", this.EnvSvr.Cores}
                                                };
                                        }
                                    }
                                    else
                                    {
                                        state = new Dictionary<string, object>
                                            {
                                                {"IsBusy", false},
                                                {"ProcessType", "None"},
                                                {"UsageCore", 0},
                                                {"TotalCore", this.EnvSvr.Cores}
                                            };
                                    }

                                    // response result ..//回應結果
                                    __returnObject = ServerObject.Response(ServerReponse.ResponseSign.PLUSOK, state);
                                }
                                catch (Exception ex)
                                {
                                    // response result ..//回應結果
                                    __returnObject = ServerObject.Response(ServerReponse.ResponseSign.MINUSERR);

                                    // append to logs object ..//追加到日志對象
                                    RenderEvents.AppendLog(string.Format("{0}", "has happen workflow exception error."));

                                    string ExceptionMsg = ex.Message + ex.StackTrace;

                                    // write to log file ..//寫到日志文件
                                    this.EnvLog.Writer(AssemblyInfoClass.ProductInfo, Log.Level.Error, ExceptionMsg, true);
                                }
                                finally
                                {
                                    // response object ..//回應對象
                                    client.Send(this.EnvSvr.Serialize(__returnObject));
                                }
                                break;
                            #endregion
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string ExceptionMsg = ex.Message + ex.StackTrace;

                // write to log file ..//寫到日志文件
                this.EnvLog.Writer(AssemblyInfoClass.ProductInfo, Log.Level.Error, ExceptionMsg, true);
            }
            finally
            {
                // close the signle client object ..//關閉單個客戶對象
                client.Close();
            }
        }
        #endregion

        #region Render Process Event Procedure//Render進程事件Procedure
        /// <summary>
        /// Main render process procedure.
        /// </summary>
        /// <param name="RenderObject">render object.</param>
         /*將QueueDataTable加鎖,創建Process進程,調用
          * Proc_outputdatareceived和Proc_exited.
          * 后執行mayabatch線程,將進程ID和父ID都加入進程集合*/
        private void Rendering(object RenderObject)
        {
            try
            {
                lock (this.QueueDataTable)//將QueueDataTable加鎖
                {
                    // receive and convert render object ..//接收和轉換Render對象
                    DataRow rowObject = (DataRow)RenderObject;

                    // setting process start infomation ..//設置進程開始信息
                    ProcessStartInfo info = new ProcessStartInfo();
                    info.FileName = rowObject["Command"].ToString().Trim();
                    info.Arguments = rowObject["Args"].ToString().Trim();
                    //info.Arguments= -r file -proj D:\UAT20090709\CGI\element -s 1 -e 10 -rd Y:\UAT20090709\CGI\ep\e053\ren\s010_c014\OCC_CH "D:\UAT20090709\CGI\ep\e001\S020\lo\ball.ma";
                    info.CreateNoWindow = true;
                    info.RedirectStandardOutput = true;
                    info.UseShellExecute = false;

                    // create process procedure ..//創建Process進程
                    Process proc = new Process();
                    proc.EnableRaisingEvents = true;
                    proc.StartInfo = info;

                    // registry event handler ..//注冊事件handler(黨OutputDataReceived,Proc_OutputDataReceived就做這個事件)
                    proc.OutputDataReceived += new DataReceivedEventHandler(Proc_OutputDataReceived);//調用Proc_OutputDataReceived
                    proc.Exited += new EventHandler(Proc_Exited);//調用Proc_Exited

                    // processing ..
                    proc.Start();

                    // get process id ..//得到進程ID
                    rowObject["Proc_Id"] = proc.Id;

                    // increase one quantity ..//增加一數量
                    this.UsageCore++;

                    // collection parameter ..//loginfo參數集合
                    object[] loginfo = new object[]
                    {
                        rowObject["Job_Id"],
                        info.FileName,
                        info.Arguments,
                        proc.Id
                    };

                    // append to logs object ..//追加到日志對象
                    RenderEvents.AppendLog(string.Format("start process {0} job. command: {1} args: {2} proc id: {3}", loginfo));

                    // async read data stream output ..//異步讀數據流輸出
                    proc.BeginOutputReadLine();
                    
                    lock (this.RenderProcess)
                    {
                        bool failresult = false;

                        do
                        {
                            // registry to render process collection ..//注冊到Render進程集合
                            if (!this.RenderProcess.ContainsKey(proc.Id))
                            {
                                // find mayabatch parent process id ..//找到mayabatch父進程ID
                                Process[] sub_proc = Process.GetProcessesByName("mayabatch");//將它們與遠端電腦上共用指定處理序名稱的所有處理程序資源相關聯

                                if (sub_proc.Length > 0)
                                {
                                    foreach (Process p in sub_proc)
                                    {   // 以指定處理序 ID 的方式擷取创建此處理序的父处理序(Process) (使能夠被存取、啟動或停止)
                                        int parent = this.EnvSvr.GetParentProcess(p.Id).Id;
                                        if (proc.Id == parent)
                                        {
                                            // add to process collection ..//將進程ID和父ID都加入進程集合
                                            this.RenderProcess.Add(proc.Id, p.Id);
                                            break;
                                        }
                                    }
                                }
                            }

                            lock (this.FailProcess)
                            {
                                if (this.FailProcess.Contains(proc.Id))
                                    failresult = true;
                            }

                            if (failresult)
                                break;
                        } while (UsageCore != RenderProcess.Count);
                    }
                    
                    // release the waiting thread ..//釋放等待線程
                    Monitor.PulseAll(this.QueueDataTable);
                }
            }
            catch (Exception ex)
            {
                // append to logs object ..//追加到日志對象
                RenderEvents.AppendLog(string.Format("{0}", "processing job has happen exception error."));

                string ExceptionMsg = ex.Message + ex.StackTrace;

                // write to log file ..//寫到日志文件
                this.EnvLog.Writer(AssemblyInfoClass.ProductInfo, Log.Level.Error, ExceptionMsg, true);
            }
        }

        /// <summary>
        /// Process stream data output event.//進程流數據輸出事件
        /// </summary>
        /*將ProcOutputlog加鎖,增加新進程ID到這個數據
         * 字典再輸出數據,最后釋放等待線程*/ 
        private void Proc_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            try
            {
                // convert processing object ..//轉換進程對象
                Process proc = (Process)sender;

                if (!string.IsNullOrEmpty(e.Data))
                {
                    lock (this.ProcOutputLog)//給創建接收process流輸出加鎖
                    {
                        
                        if (!this.ProcOutputLog.ContainsKey(proc.Id))
                        {
                            // add new process id to this data dictionary ..//增加新進程ID到這個數據字典
                            ProcOutputLog.Add(proc.Id, new StringBuilder());
                            //// writing output data ..//寫輸出數據
                            //ProcOutputLog[proc.Id].AppendLine(e.Data);

                        }
                        else
                        //// writing output data ..//寫輸出數據
                            //ProcOutputLog[proc.Id].AppendLine(e.Data);

                        // release the waiting thread ..//釋放等待線程
                        Monitor.PulseAll(this.ProcOutputLog);
                    }
                }
                else
                {
                    // writing output data ..//寫輸出數據
                   
                }
            }
            catch (Exception ex)
            {
                // append to logs object ..//追加到日志對象
                RenderEvents.AppendLog(string.Format("{0}", "received processing output data has happen exception error."));

                string ExceptionMsg = ex.Message + ex.StackTrace;

                // write to log file ..//寫到日志文件
                this.EnvLog.Writer(AssemblyInfoClass.ProductInfo, Log.Level.Error, ExceptionMsg, true);
                
            }
        }

        /// <summary>
        /// Process exited event procedure.//進程退出事件過程
        /// </summary>
        /*將FailProcess加鎖,將id加到失敗進程。將QueueDataTable加鎖，
         * 將Finish_Time和Status數據改變進行提交，將失敗進程進行提交。
         * 將RenderProcess加鎖，將正常完成狀態進行提交。最后釋放等待線程*/
        private void Proc_Exited(object sender, EventArgs e)
        {
            try
            {
                // get process id ..//得到進程Id
                int id = ((Process)sender).Id;

                lock (this.FailProcess)//將失敗進程加鎖
                {
                    if (((Process)sender).HasExited)
                    {
                        DateTime StartTime = ((Process)sender).StartTime;
                        TimeSpan EndTime = new TimeSpan(((Process)sender).ExitTime.Ticks);

                        if (EndTime.Subtract(new TimeSpan(StartTime.Ticks)).Duration().TotalMilliseconds < 1000)
                            // add to fail list ..//添加到失敗列
                            this.FailProcess.Add(id);
                    }

                    // release the waiting thread ..//釋放等待線程                                                          
                    Monitor.PulseAll(this.FailProcess);
                }

                lock (this.QueueDataTable)
                {
                    if (this.FailProcess.Contains(id))
                    {
                        DataRow[] row = this.QueueDataTable.Select(string.Format("Proc_Id = {0}", id));

                        for (int i = 0; i < row.Length; i++)
                        {
                            row[i]["Finish_Time"] = DateTime.Now;
                            row[i]["Status"] = JobStatusFlag.ERROR;
                        }

                        // commit data changes ..//把改變數據進行提交
                        this.QueueDataTable.AcceptChanges();

                        // set last change flag ..//設置上一次改變標志
                        this.HasChange = true;

                        // remove the fail process ..//移除失敗進程
                        this.FailProcess.Remove(id);

                        // reduce one quantity ..//減少一數量
                        UsageCore--;
                    }
                }

                lock (this.RenderProcess)
                {
                    // find process id ..//查找進程id
                    if (this.RenderProcess.ContainsKey(id))
                    {
                        // append to logs object ..//追加到日志對象
                        RenderEvents.AppendLog(string.Format("the process id {0} has completed.", id));

                        lock (this.QueueDataTable)
                        {
                            DataRow[] row = this.QueueDataTable.Select(string.Format("Proc_Id = {0}", id));

                            for (int i = 0; i < row.Length; i++)
                            {
                                // normal finish status ..//正常完成狀態
                                row[i]["Render_Output"] = this.ProcOutputLog[id];
                                row[i]["Finish_Time"] = DateTime.Now;
                                row[i]["Status"] = JobStatusFlag.COMPLETED;
                            }

                            // commit data changes ..//提交數據改變
                            this.QueueDataTable.AcceptChanges();

                            // set last change flag ..//設置上一次改變標志
                            this.HasChange = true;

                            // release the waiting thread ..//釋放等待線程
                            Monitor.PulseAll(this.QueueDataTable);
                        }

                        lock (this.ProcOutputLog)
                        {
                            // remove from dictionary record ..//移除字典記錄
                            this.ProcOutputLog.Remove(id);

                            // release the waiting thread ..//釋放等待線程
                            Monitor.PulseAll(this.ProcOutputLog);
                        }

                        if (this.RenderProcess.ContainsKey(id))
                        {
                            // remove the process id ..//移除進程ID
                            this.RenderProcess.Remove(id);

                            // release the waiting thread ..//釋放等待線程
                            Monitor.PulseAll(this.RenderProcess);
                        }

                        // reduce one quantity ..//減少一數量
                        UsageCore--;
                    }
                }

                // clean all resource associated with process ..//清除所有資源聯系進程
                ((Process)sender).Close();
            }
            catch (Exception ex)
            {
                string ExceptionMsg = ex.Message + ex.StackTrace;

                // write to log file ..//寫到日志文件
                this.EnvLog.Writer(AssemblyInfoClass.ProductInfo, Log.Level.Error, ExceptionMsg, true);
            }
        }
        #endregion
    }
}