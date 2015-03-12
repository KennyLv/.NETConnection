#region Using NameSpace
using System;
using System.Data;
using System.Threading;

// import renbar server library namespace ..
using RenbarLib.Data;
using RenbarLib.Environment;
using RenbarLib.Network;

// import render server properties namespace ..
using RenbarServerGUI.Properties;
using RenbarLib.Network.Sockets;
#endregion

namespace RenbarServerGUI
{
    /// <summary>
    /// Renbar server base class.
    /// </summary>
    public class ServerBase : IDisposable
    {
        #region Declare Partial Global Variable Section
        // define default settings ..
        //private string ConnectionString = "Data Source=SOFACGI66\\SQLEXPRESS;Initial Catalog=Renbar2;Integrated Security=True;";

        //SQL Server 2005
        //private string ConnectionString = "Data Source=192.168.2.227;database=BT_Test;user id=SA;password=Foxconn99;";

        //Mysql
        private string ConnectionString = "Data Source=192.168.2.228;database=BT_Test;user id=root;password=Foxconn99;";

        private string __AbUser = "isadmin";
        private string __AbPass = "1qazxsw2";
        private string __AbServer = "BI-TESTSRV02";
        private string __AbWorkPath = @"D:\ab_localcopy_r";
        //private string __AbWorkPath = @"Y:\";
        private ushort __ListenClientPort = 6600;
        private ushort __ListenRenderPort = 6700;
        private ushort __ServerSyncPort = 6601;
        private ushort __JobHistoryMaxRecord = 50;

        // declare renbar class library object ..
        private DataStructure EnvData = null;
        private Service EnvSvr = new Service();
        private Log EnvLog = new Log();
        private HostBase EnvHostBase = new HostBase();

        // declare function base class ..
        private AlienbrainBase AbBase = null;
        private Cleanup __Cleanup = null;
        private DisplayBase ShowBase = null;
        private ProtocolBase PortalBase = null;
        private RenderBase OperationBase = null;
        //
        private SlaveBase MasterBase = null;
        private volatile bool requestStop = false;
        #endregion

        #region Server Base Constructor Procedure
        /// <summary>
        /// Server base constructor.
        /// </summary>
        public ServerBase()
        {
            // view change settings ..
            this.ViewChangeSettings();

            // 構造AlienbrainInfo用戶實體（用戶名、密碼、服務器名、工作路徑） initialize alienbrain extension structure ..
            AlienbrainInfo AbInfo = new AlienbrainInfo()
            {
                User = this.__AbUser,
                Password = this.__AbPass,
                Server = this.__AbServer,
                WorkPath = this.__AbWorkPath
            };

            //  獲取數據庫（鏈接字符串）結構及數據 initialize relationship object instance ..
            this.EnvData = new DataStructure(ConnectionString);

            // 通過用戶實體驗證，根據數據調用Alienbrain.SDK的API去GetLaste檔案，並更改任務狀態標識到數據庫
            this.AbBase = new AlienbrainBase(this.EnvData, this.EnvLog, AbInfo);

            // 過期數據清理
            this.__Cleanup = new Cleanup(this.EnvData, this.EnvLog);

            //構建新的緩存數據，接收客戶端更新，再刷新EnvData中數據
            this.ShowBase = new DisplayBase(this.EnvData, this.EnvLog);

            ////監聽客戶端
            //this.PortalBase = new ProtocolBase(this.EnvData, this.EnvLog)
            //{
            //    ConnectPort = this.__ListenRenderPort,
            //    ListenPort = this.__ListenClientPort,
            //    MaxHistory = this.__JobHistoryMaxRecord,
            //};

            //////操作Render
            //this.OperationBase = new RenderBase(this.EnvData, this.EnvLog)
            //{
            //    ConnectPort = this.__ListenRenderPort
            //};

            Thread SlaveThreading = new Thread(new ThreadStart(SlaveThread));
            SlaveThreading.IsBackground = true;
            SlaveThreading.Priority = ThreadPriority.Normal;
            SlaveThreading.Start();
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
                this.__Cleanup.Dispose();
                this.ShowBase.Dispose();
                this.AbBase.Dispose();
                //
                if (this.OperationBase != null)
                {
                    this.OperationBase.Dispose();
                }
                if (this.PortalBase != null)
                {
                    this.PortalBase.Dispose();
                }
                //
                if (this.MasterBase != null)
                {
                    this.MasterBase.Dispose();//？？？？？？？？？？？？？？？？？？？？
                }
                this.requestStop = true;
                this.EnvData.Dispose();

                // execute of recycling resource ..
                GC.Collect();
            }
        }
        #endregion

        #region View Currently Environment Settings
        /// <summary>
        /// View and change server environment settings.
        /// </summary>
        private void ViewChangeSettings()
        {
            // database connection string ..
            if (Settings.Default.DBConnectionString != string.Empty)
            {
                if (this.ConnectionString != Settings.Default.DBConnectionString.Trim())
                    this.ConnectionString = Settings.Default.DBConnectionString.Trim();
            }

            // checking listen ports ..
            if (this.__ListenClientPort != Settings.Default.ListenClientPort)
                this.__ListenClientPort = Settings.Default.ListenClientPort;
            if (this.__ListenRenderPort != Settings.Default.ListenRenderPort)
                this.__ListenRenderPort = Settings.Default.ListenRenderPort;
            if (this.__ServerSyncPort != Settings.Default.ServerSyncPort)
                this.__ServerSyncPort = Settings.Default.ServerSyncPort;

            // checking job history max record ..
            if (this.__JobHistoryMaxRecord != Settings.Default.JobHistoryMaxRecord)
                this.__JobHistoryMaxRecord = Settings.Default.JobHistoryMaxRecord;

            // checking alienbrain extension settings ..
            if (Settings.Default.AlienbrainUser != string.Empty)
            {
                if (this.__AbUser != Settings.Default.AlienbrainUser)
                    this.__AbUser = Settings.Default.AlienbrainUser.Trim();
            }

            if (Settings.Default.AlienbrainPwd != string.Empty)
            {
                if (this.__AbPass != Settings.Default.AlienbrainPwd)
                    this.__AbPass = Settings.Default.AlienbrainPwd.Trim();
            }

            if (Settings.Default.AlienbrainSvr != string.Empty)
            {
                if (this.__AbServer != Settings.Default.AlienbrainSvr)
                    this.__AbServer = Settings.Default.AlienbrainSvr.Trim();
            }

            if (Settings.Default.AlienbrainWorkingPath != string.Empty)
            {
                if (this.__AbWorkPath != Settings.Default.AlienbrainWorkingPath)
                    this.__AbWorkPath = Settings.Default.AlienbrainWorkingPath.Trim();
            }


        }
        #endregion

        #region  啟動備援機制線程 陳宗波添加
        private void SlaveThread()
        { 
            ScanPort runConnect = null;
            bool NoProtocol = true;//ProtocolBase、RenderBase未開啟
            try
            {
                // create object instance ..
                runConnect = new ScanPort();
                do
                {
                    //新增備援機制 是否是主機，如果不是建立連接測試類，如果可以連接主機，則不啟動備援，如果連接主機不通，則啟動備援機制
                    #region 本機是主機
                    if (EnvHostBase.LocalIpAddress.ToString() == Settings.Default.MasterServer)
                    {
                        if (NoProtocol)
                        {
                            this.OperationBase = new RenderBase(this.EnvData, this.EnvLog)
                            {
                                ConnectPort = this.__ListenRenderPort
                            };
                            this.PortalBase = new ProtocolBase(this.EnvData, this.EnvLog)
                            {
                                ConnectPort = this.__ListenRenderPort,
                                ListenPort = this.__ListenClientPort,
                                MaxHistory = this.__JobHistoryMaxRecord
                            };
                            //開啟監聽端口6601
                            this.MasterBase = new SlaveBase(this.EnvLog)
                            {
                                _listenSlavePort = this.__ServerSyncPort
                            };
                            NoProtocol = false;
                        }
                    }
                    #endregion

                    #region 本機非主機
                    else
                    {
                        //主機未啟動
                        if (!runConnect.Scan(System.Net.IPAddress.Parse(Settings.Default.MasterServer), this.__ServerSyncPort))
                        {
                            if (this.OperationBase == null)
                            {
                                this.OperationBase = new RenderBase(this.EnvData, this.EnvLog)
                                {
                                    ConnectPort = this.__ListenRenderPort
                                };
                            }
                            if (this.PortalBase == null)
                            {
                                this.PortalBase = new ProtocolBase(this.EnvData, this.EnvLog)
                                {
                                    ConnectPort = this.__ListenRenderPort,
                                    ListenPort = this.__ListenClientPort,
                                    MaxHistory = this.__JobHistoryMaxRecord
                                };

                            }
                        }
                            //主機已啟動
                        else
                        {
                            //取消与客户端以及render端沟通的线程
                            if (this.OperationBase != null)
                            {
                                this.OperationBase.Dispose();
                                this.OperationBase = null;
                            }
                            if (this.PortalBase != null)
                            {
                                this.PortalBase.Dispose();
                                this.PortalBase = null;
                            }
                        }
                    }

                    #endregion
                    Thread.Sleep(1000);
                } while (!requestStop);
            }
            catch (Exception ex)
            {
                string ExceptionMsg = ex.Message + "Master to Slave!";

                // write to log file ..
                //EnvLog.Writer(AssemblyInfoClass.ProductInfo, Log.Level.Error, ExceptionMsg, true);
            }
        }
        #endregion

        #region Get Server Status Properties
        /// <summary>
        /// Get job display information.
        /// </summary>
        internal DataTable JobStatus
        {
            get
            {
                return this.ShowBase.GetJobDisplayInfo;
            }
        }

        /// <summary>
        /// Get mahcine display information.
        /// </summary>
        internal DataTable MachineStatus
        {
            get
            {
                return this.ShowBase.GetMachineDisplayInfo;
            }
        }
        #endregion

        #region Get Memory Data Status Property
        /// <summary>
        /// Get currently memory data rows count.
        /// </summary>
        internal uint DataRowsCount
        {
            get
            {
                return (uint)this.EnvData.Count;
            }
        }
        #endregion

        public Cleanup Cleanup
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }

        public AlienbrainBase AlienbrainBase
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }

        public DisplayBase DisplayBase
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }

        public ProtocolBase ProtocolBase
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }

        public RenderBase RenderBase
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }

        internal RenbarServerGUI.Properties.Settings Settings
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }
    }
}