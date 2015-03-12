using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;

// import renbar common library namespace ..
using RenbarLib.Environment;


namespace RenbarLib.Network
{
    /// <summary>
    /// Network environment class.
    /// </summary>
    public class HostBase : Log
    {
        #region 解析主機名、IP地址 Parse Host, Ip Address Procedure
        /// <summary>
        /// 獲取指定IP主機的NDS名稱 Get host name.
        /// </summary>
        /// <param name="IpAddress">ipv4 address.</param>
        /// <returns>System.String</returns>
        public string ParseHostName(IPAddress IpAddress)
        {
            //IPHostEntry 類別將網域名稱系統 (DNS) 主機名稱與別名 (Alias) 陣列和符合 IP 位址的陣列建立關聯
            IPHostEntry _host = new IPHostEntry();

            try
            {
                //
                _host = Dns.GetHostEntry(IpAddress);
            }
            catch (Exception ex)
            {
                _host.HostName = string.Empty;

                string ExceptionMsg = ex.Message + ex.StackTrace;

                // write to log file ..
                base.Writer(AssemblyInfoClass.ProductInfo, Log.Level.Error, ExceptionMsg, true);
            }

            return _host.HostName;
        }

        /// <summary>
        /// 獲取指定名稱的主機IP地址 Get IPv4 address.
        /// </summary>
        /// <param name="HostName">host name.</param>
        /// <returns>System.Net.IPAddress</returns>
        public IPAddress ParseIpAddress(string HostName)
        {
            IPAddress[] __ipAddr = null;

            try
            {
                __ipAddr = Dns.GetHostAddresses(HostName);
            }
            catch (Exception ex)
            {
                string ExceptionMsg = ex.Message + ex.StackTrace;

                // write to log file ..
                base.Writer(AssemblyInfoClass.ProductInfo, Log.Level.Error, ExceptionMsg, true);

                return IPAddress.None;
            }

            return __ipAddr[0];
        }
        #endregion

        #region 獲取主機名、IP地址 Get Host Name, Ip Address Properties
        /// <summary>
        /// Get full local host name.
        /// </summary>
        public string FullLocalHostName
        {
            get
            {
                return Dns.GetHostEntry(Dns.GetHostName()).HostName.Trim();
            }
        }

        /// <summary>
        /// Get short local host name.
        /// </summary>
        public string LocalHostName
        {
            get
            {
                return (Dns.GetHostEntry(Dns.GetHostName()).HostName.Trim().Split('.'))[0].ToUpper();
            }
        }

        /// <summary>
        /// Get local host ipv4 address.
        /// </summary>
        public IPAddress LocalIpAddress
        {
            get
            {
                string[] result = null;
                int i = 0;

                // create collection to hold network interfaces
                NetworkInterface[] Interfaces;

                // get list of all interfaces
                Interfaces = NetworkInterface.GetAllNetworkInterfaces();
                result = new string[Interfaces.Length];

                // loop through interfaces
                foreach (NetworkInterface Interface in Interfaces)
                {
                    // create collection to hold IP information for interfaces
                    // get list of all unicast IPs from current interface
                    UnicastIPAddressInformationCollection IPs = Interface.GetIPProperties().UnicastAddresses;

                    // loop through IP address collection
                    foreach (UnicastIPAddressInformation IP in IPs)
                    {
                        // check IP address for IPv4
                        if (IP.Address.AddressFamily == global::System.Net.Sockets.AddressFamily.InterNetwork)
                        {
                            // write IP address to debug window
                            result[i] = IP.Address.ToString();
                            i++;
                        }
                    }
                }

                return IPAddress.Parse(result[0]);
            }
        }
        #endregion
    }

    namespace Protocol
    {
        #region 定義任務狀態 Job Status Flag Enumeration
        /// <summary>
        /// Job status flag.
        /// </summary>
        public enum JobStatusFlag : ushort
        {
            /// <summary>
            /// Completed flag.
            /// </summary>
            COMPLETED = 0,
            /// <summary>
            /// Queuing flag.
            /// </summary>
            QUEUING = 1,
            /// <summary>
            /// Pause flag.
            /// </summary>
            PAUSE = 2,
            /// <summary>
            /// Checking file for alienbrain extension flag.
            /// </summary>
            CHECKING = 3,
            /// <summary>
            /// Error Flag.
            /// </summary>
            ERROR = 4,
            /// <summary>
            /// UpdateOnly for alienbrain extension flag.
            /// </summary>
            UPDATEONLY = 5,
            /// <summary>
            /// Processing Flag.
            /// </summary>
            PROCESSING = 6,
            /// <summary>
            /// Get latest files for alienbrain extension flag.
            /// </summary>
            GETLATEST = 7,
            /// <summary>
            /// Machine maintenance status for render farm flag.
            /// </summary>
            RESTORE = 8
        }
        #endregion

        #region 定義機器狀態 Machine Status Flag Enumeration
        /// <summary>
        /// Machine status flag.
        /// </summary>
        public enum MachineStatusFlag : ushort
        {
            /// <summary>
            /// Maintenance flag.
            /// </summary>
            MAINTENANCE = 0,
            /// <summary>
            /// Client flag.
            /// </summary>
            CLIENT = 1,
            /// <summary>
            /// Render flag.
            /// </summary>
            RENDER = 2,
            /// <summary>
            /// Offline flag.
            /// </summary>
            OFFLINE = 3
        }
        #endregion

        /// <summary>
        /// 客戶端通訊物件 Client communication object structure.
        /// </summary>
        public struct Client2Server
        {
            #region 通訊行為枚舉 Client-to-Server Communication Behavior Enumeration
            /// <summary>
            /// Communication behavior type enumeration.
            /// </summary>
            public enum CommunicationType
            {
                /// <summary>
                /// None Flag.
                /// </summary>
                NONE,
                /// <summary>
                /// Job Add Flag.
                /// </summary>
                JOBQUEUEADD,
                /// <summary>
                /// Job Delete Flag.
                /// </summary>
                JOBQUEUEDELETE,
                /// <summary>
                /// Job Update Flag.
                /// </summary>
                JOBQUEUEUPDATE,
                /// <summary>
                /// Job Pause Flag.
                /// </summary>
                JOBQUEUEPAUSE,
                /// <summary>
                /// Job Repeat Flag.
                /// </summary>
                JOBQUEUEREPEAT,
                /// <summary>
                /// Job History Flag.
                /// </summary>
                JOBHISTORYRECORD,
                /// <summary>
                /// View Job Status Flag.
                /// </summary>
                VIEWJOBSTATUS,
                /// <summary>
                /// View Single Job Info By Job_Group_ID F7300290
                /// </summary>
                VIEWSINGLEJOBINFO,
                /// <summary>
                /// SET Job Priority F7300290
                /// </summary>
                JOBPRIORITY,

                /// <summary>
                /// View Job Process Output Flag.
                /// </summary>
                VIEWJOBOUTPUT,
                /// <summary>
                /// Machine Info Flag.
                /// </summary>
                MACHINEINFO,
                /// <summary>
                /// Delete Machine Info Flag.
                /// </summary>
                DELETEMACHINEINFO,
                /// <summary>
                /// View Machine Info Flag.
                /// </summary>
                VIEWMACHINEINFO,
                /// <summary>
                /// Enable Or Disable Machine Info Flag.
                /// </summary>
                ONOFFMACHINE,
                /// <summary>
                /// Set Machine Priority Flag.
                /// </summary>
                MACHINEPRIORITY,
                /// <summary>
                /// View Machine Render Info Flag.
                /// </summary>
                VIEWMACHINERENDERINFO,
                /// <summary>
                /// Pool Info Flag.
                /// </summary>
                POOLINFO,
                /// <summary>
                /// Delete Pool Flag.
                /// </summary>
                DELETEPOOLINFO,
                /// <summary>
                /// View Pool Info Flag.
                /// </summary>
                VIEWPOOLINFO,
                /// <summary>
                /// Machine Pool Relation Flag.
                /// </summary>
                MACHINEPOOLRELATION,
                /// <summary>
                /// View Machine Pool Relation Flag.
                /// </summary>
                VIEWMACHINEPOOLRELATION
            }
            #endregion

            #region 打包解包通訊物件 Package & UnPackage Transport Object
            /// <summary>
            /// 添加通訊行為到隊列包頭部 Package network packet.
            /// </summary>
            /// <param name="Type">communication behavior enumeration.</param>
            /// <param name="QueueItems">transport items.</param>
            /// <returns>System.Collection.Generic</returns>
            public IList<object> Package(CommunicationType Type, IDictionary<string, object> QueueItems)
            {
                IList<object> Packaged = new List<object>
                {
                    Type,
                    QueueItems
                };

                return Packaged;
            }

            /// <summary>
            /// 分解通訊物件，分離通訊行為和隊列 Unpackage network packet.
            /// </summary>
            /// <param name="RemoteObject">remote packaged object.</param>
            /// <param name="Type">communication behavior enumeration.//（out輸出）調用前不需對變量進行初始化﹐輸出型參數用于傳遞方法返回的數據。</param>
            /// <param name="QueueItems">transport items</param>
            public void UnPackage(IList<object> RemoteObject, out CommunicationType Type, out IDictionary<string, object> QueueItems)
            {
                if (RemoteObject.Count == 0)
                {
                    Type = CommunicationType.NONE;
                    QueueItems = new Dictionary<string, object>();
                    return;
                }

                Type = (CommunicationType)RemoteObject[0];
                QueueItems = (IDictionary<string, object>)RemoteObject[1];
            }
            #endregion
        }

        /// <summary>
        /// Render Server端通訊物件 Render communication object structure.
        /// </summary>
        public struct Server2Render
        {
            #region 通訊行為枚舉 Server-to-RenderFarm Communication Behavior Enumeration
            /// <summary>
            /// Communication behavior type enumeration.
            /// </summary>
            public enum CommunicationType
            {
                /// <summary>
                /// None Flag.
                /// </summary>
                NONE,
                /// <summary>
                /// Dispatch Flag.
                /// </summary>
                DISPATCH,
                /// <summary>
                /// Check Machine IsBusy Flag.
                /// </summary>
                ISBUSY,
                /// <summary>
                /// Check Running Jobs Flag.
                /// </summary>
                RUNNINGJOBS,
                /// <summary>
                /// Delete Job Flag.
                /// </summary>
                DELETEJOB,
                /// <summary>
                /// Return Completed Jobs Flag.
                /// </summary>
                COMPLETEDJOBS
            }
            #endregion

            #region  打包解包通訊物件 Package & UnPackage Transport Object
            /// <summary>
            /// 添加通訊行為到空數據包頭部 Package network pakcet.
            /// </summary>
            /// <param name="Type">communication behavior enumeration.</param>
            /// <returns>System.Collection.Generic</returns>
            public IList<object> Package(CommunicationType Type)
            {
                IList<object> Packaged = new List<object>
                {
                    Type,
                    new Dictionary<string, object>()
                };

                return Packaged;
            }

            /// <summary>
            /// 添加通訊行為到隊列包頭部Package network packet.
            /// </summary>
            /// <param name="Type">communication behavior enumeration.</param>
            /// <param name="QueueItems">transport items.</param>
            /// <returns>System.Collection.Generic</returns>
            public IList<object> Package(CommunicationType Type, IDictionary<string, object> DataItems)
            {
                IList<object> Packaged = new List<object>
                {
                    Type,
                    DataItems
                };

                return Packaged;
            }

            /// <summary>
            /// 分解物件包 Unpackage network packet.
            /// </summary>
            /// <param name="RemoteObject">remote packaged object.</param>
            /// <param name="Type">communication behavior enumeration.</param>
            /// <param name="QueueItems">transport items</param>
            public void UnPackage(IList<object> RemoteObject, out CommunicationType Type, out IDictionary<string, object> DataItems)
            {
                if (RemoteObject.Count == 0)
                {
                    Type = CommunicationType.NONE;
                    DataItems = new Dictionary<string, object>();
                    return;
                }

                Type = (CommunicationType)RemoteObject[0];
                DataItems = (IDictionary<string, object>)RemoteObject[1];
            }
            #endregion
        }

        /// <summary>
        /// 任務分派參數結構 Job dispatch object structure.
        /// </summary>
        [Serializable]
        public struct Dispatch
        {
            #region 任務分派參數 Dispatch Properties
            /// <summary>
            /// First job id
            /// </summary>
            public Guid Job_Group_Id
            {
                get;
                set;
            }

            /// <summary>
            /// Second job id.
            /// </summary>
            public int Job_Id
            {
                get;
                set;
            }

            //public string Job_Id
            //{
            //    get;
            //    set;
            //}

            /// <summary>
            /// Process render type.
            /// </summary>
            public string Proc_Type
            {
                get;
                set;
            }

            /// <summary>
            /// Command string.
            /// </summary>
            public string Command
            {
                get;
                set;
            }

            /// <summary>
            /// Command of arguments.
            /// </summary>
            public string Args
            {
                get;
                set;
            }
            #endregion
        }

        /// <summary>
        /// 服務器響應物件 Server response object structure
        /// </summary>
        public struct ServerReponse
        {
            #region Server Response Sign Enumeration
            /// <summary>
            /// 響應行為枚舉 Response sign behavior enumeration.
            /// </summary>
            public enum ResponseSign
            {
                /// <summary>
                /// Success Flag.
                /// </summary>
                PLUSOK,
                /// <summary>
                /// Error Flag.
                /// </summary>
                MINUSERR
            }
            #endregion

            #region  服務器響應消息 Custom Server Response Message Procedure
            /// <summary>
            /// 響應消息（Type+時間） Response server message result.
            /// </summary>
            /// <param name="Type">result sign.</param>
            /// <returns>System.Collection.Generic</returns>
            public KeyValuePair<string, object> Response(ResponseSign Type)
            {
                string
                    Sign = string.Empty,
                    ReturnDateTime = string.Empty;

                // create date time array object ..
                object[] dateobjs = new object[] {
                    DateTime.Now.Year,
                    DateTime.Now.Month.ToString().PadLeft(2, '0'),
                    DateTime.Now.Day.ToString().PadLeft(2, '0'),
                    DateTime.Now.Hour.ToString().PadLeft(2, '0'),
                    DateTime.Now.Minute.ToString().PadLeft(2, '0'),
                    DateTime.Now.Second.ToString().PadLeft(2, '0')
                };

                // datetime string format ..
                ReturnDateTime = string.Format("{0}-{1}-{2} {3}:{4}:{5}", dateobjs);

                // decide server sign case ..
                switch (Type)
                {
                    case ResponseSign.PLUSOK:
                        Sign = "+Ok";
                        break;
                    case ResponseSign.MINUSERR:
                        Sign = "-Err";
                        break;
                }
                return new KeyValuePair<string, object>(string.Format("{0} {1}", Sign, ReturnDateTime), null);
            }

            /// <summary>
            /// 響應消息（Type+時間+結果）Response server message result.
            /// </summary>
            /// <param name="Type">result sign.</param>
            /// <param name="ResultObject">result object.</param>
            /// <returns>System.Collection.Generic</returns>
            public KeyValuePair<string, object> Response(ResponseSign Type, object ResultObject)
            {
                return new KeyValuePair<string, object>(Response(Type).Key, ResultObject);
            }
            #endregion
        }
    }

    namespace Sockets
    {
        /// <summary>
        /// Broadcast network service by udp communication protocol.
        /// </summary>
        public class BroadcastSocket : Log
        {
            #region Declare Global Variable
            // udp client object ..
            private UdpClient __udpsvr = null;

            // temporary port ..
            private int __port = 0;
            #endregion

            #region 構造廣播套接 Broadcast Socket Constructor Procedure
            /// <summary>
            /// Broadcast socket network constructor procedure.
            /// </summary>
            /// <param name="Port">communications local port.</param>
            public BroadcastSocket(int Port)
            {
                // this constructor assigns the local port number
                this.__udpsvr = new UdpClient(Port);
                // assign port ..
                this.__port = Port;
            }
            #endregion

            #region 發送接收網絡數據流 Send & Receive Network Stream Event Procedure
            /// <summary>
            /// Send user datagram method.
            /// </summary>
            /// <param name="Data">data object.</param>
            public void Send(byte[] Data)
            {
                try
                {
                    /*如果您呼叫 Connect 方法，則從指定預設值以外的位址抵達的任何資料包都將遭到捨棄。
                     * 使用這個方法並不能將預設遠端主機設定為廣播位址；
                     * 除非從 UdpClient 繼承，接著使用 Client 方法取得基礎 Socket，
                     * 然後再設定通訊端選項為 SocketOptionName..::.Broadcast
                     * 然而，如果您在呼叫 Send 方法時指定 IPAddress..::.Broadcast，
                     * 則可以廣播資料至預設廣播位址 255.255.255.255。如果應用程式需要進一步控制廣播位址，
                     * 您也可以回頭使用 Socket 類別*/
                    //？？？？？？？？？？？？？？？？？？？？？？？？？？？？？？？？？？？？？？？？？？？？？？？
                    // 使用指定的 IP 位址和通訊埠編號，建立預設遠端主機 connect remote ipendpoint ..
                    this.__udpsvr.Connect(IPAddress.Broadcast, __port);

                    // 將 UDP 資料包傳送至遠端主機 send data ..
                    this.__udpsvr.Send(Data, Data.Length);
                }
                catch (Exception ex)
                {
                    string ExceptionMsg = ex.Message + ex.StackTrace;

                    // write to log file ..
                    base.Writer(AssemblyInfoClass.ProductInfo, Log.Level.Error, ExceptionMsg, true);
                }
            }

            /// <summary>
            /// Receive user datagram method.
            /// </summary>
            /// <returns>System.Byte[]</returns>
            public byte[] Receive()
            {
                try
                {
                    // ipendpoint object will allow us to read datagrams sent from broadcast source ..
                    IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Broadcast, __port);

                    // blocks until a data object returns on this socket from a remote host ..
                    return this.__udpsvr.Receive(ref RemoteIpEndPoint);
                }
                catch (Exception ex)
                {
                    string ExceptionMsg = ex.Message + ex.StackTrace;

                    // write to log file ..
                    base.Writer(AssemblyInfoClass.ProductInfo, Log.Level.Error, ExceptionMsg, true);

                    return null;
                }
            }
            #endregion

            #region 釋放資源 Clear Socket Base Object Resource
            /// <summary>
            /// Clear network base resource.
            /// </summary>
            public void Disponse()
            {
                // close the udp connect ..
                this.__udpsvr.Close();
            }
            #endregion
        }

        /// <summary>
        /// User data diagram network service by udp communication protocol.
        /// </summary>
        public class UdpSocket : Log
        {
            #region Declare Global Variable
            // udp client object ..
            private UdpClient __udpsvr = null;

            // ip address and port number point ..
            private IPEndPoint __point = null;
            #endregion

            #region 構造UDP套接Udp Socket Constructor Procedure
            /// <summary>
            /// Udp socket network constructor procedure.
            /// </summary>
            /// <param name="Port">communications local port.</param>
            public UdpSocket(IPAddress Ip, int Port)
            {
                // this constructor assigns the local port number ..
                this.__udpsvr = new UdpClient(Port);

                // close broadcast ..
                this.__udpsvr.EnableBroadcast = false;

                // assign port to variable ..
                this.__point = new IPEndPoint(Ip, Port);
            }
            #endregion

            #region 發送接收網絡數據流 Send & Receive Network Stream Event Procedure
            /// <summary>
            /// Send user datagram method.
            /// </summary>
            /// <param name="Data">data object.</param>
            public void Send(byte[] Data)
            {
                try
                {
                    // connect remote ipendpoint ..
                    this.__udpsvr.Connect(this.__point);

                    // send data ..
                    this.__udpsvr.Send(Data, Data.Length);
                }
                catch (Exception ex)
                {
                    string ExceptionMsg = ex.Message + ex.StackTrace;

                    // write to log file ..
                    base.Writer(AssemblyInfoClass.ProductInfo, Log.Level.Error, ExceptionMsg, true);
                }
            }

            /// <summary>
            ///  Receive user datagram message method.
            /// </summary>
            /// <returns>System.Byte[]</returns>
            public byte[] Receive()
            {
                try
                {
                    // ipendpoint object will allow us to read datagrams sent from any source ..
                    IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, this.__point.Port);

                    // blocks until a data object returns on this socket from a remote host ..
                    return this.__udpsvr.Receive(ref RemoteIpEndPoint);
                }
                catch (Exception ex)
                {
                    string ExceptionMsg = ex.Message + ex.StackTrace;

                    // write to log file ..
                    base.Writer(AssemblyInfoClass.ProductInfo, Log.Level.Error, ExceptionMsg, true);

                    return null;
                }
            }
            #endregion

            #region 釋放資源 Clear Socket Base Object Resource
            /// <summary>
            /// Clear network base resource.
            /// </summary>
            public void Disponse()
            {
                // close the udp connect ..
                this.__udpsvr.Close();
            }
            #endregion
        }

        /// <summary>
        /// Scan server socket port by tcp communication protocol.
        /// </summary>
        public class ScanPort : Log
        {
            #region Declare Global Variable
            // tcp client object ..
            private TcpClient Scanner = null;
            #endregion

            #region 檢測Server端的監聽端口是否開啟 Scan Server Side Listen Port Event Procedure
            /// <summary>
            /// Scan server side tcp-listen port.
            /// </summary>
            /// <param name="HostName">connect server host name.</param>
            /// <param name="Port">connect port.</param>
            /// <returns>System.Boolean</returns>
            public bool Scan(string HostName, ushort ServicePort)
            {
                bool result = false;
                IPAddress[] __ipaddr = null;

                try
                {
                    __ipaddr = Dns.GetHostAddresses(HostName);
                    result = Scan(__ipaddr[0], ServicePort);
                }
                catch (SocketException ex)
                {
                    //string ExceptionMsg = ex.Message + ex.StackTrace;
                    string ExceptionMsg = ex.Message;
                    // write to log file ..
                    base.Writer(AssemblyInfoClass.ProductInfo, Log.Level.Error, ExceptionMsg, true);
                }

                return result;
            }

            /// <summary>
            /// Scan server side tcp-listen port
            /// </summary>
            /// <param name="IpAddr">connect server ipaddress</param>
            /// <param name="Port">connect port</param>
            /// <returns>System.Boolean</returns>
            public bool Scan(IPAddress IpAddr, ushort Port)
            {
                bool result = false;

                try
                {
                    // create tcp client object ..
                    Scanner = new TcpClient();

                    // setting response timeout ..
                    //set 500 to 5000
                    Scanner.SendTimeout = 5000;
                    Scanner.ReceiveTimeout = 10000;

                    // beginning connect remote server ..
                    //使用指定的 IP 位址和通訊埠編號將用戶端連接至遠端 TCP 主機
                    Scanner.Connect(IpAddr, (int)Port);
                    result = true;
                }
                catch (SocketException ex)
                {
                    //string ExceptionMsg = ex.Message + ex.StackTrace;
                    string ExceptionMsg = ex.Message;
                    
                    // write to log file ..
                    base.Writer(AssemblyInfoClass.ProductInfo, Log.Level.Error, ExceptionMsg, true);
                }
                finally
                {
                    //Close 方法會將執行個體標記為已處置，但不會關閉 TCP 連接。
                    //呼叫這個方法不會釋放用於傳送和接收資料的 NetworkStream，
                    //您必須呼叫 NetworkStream.Close 方法以關閉資料流和 TCP 連接。

                    // clear resource ..
                    Scanner.Close();
                }

                return result;
            }
            #endregion
        }

        /// <summary>
        /// Server network service by tcp communication protocol.
        /// </summary>
        public class TcpServerSocket : Log
        {
            #region Declare Global Variable
            // tcp listener object ..
            private TcpListener Lister = null;
            #endregion

            #region 構造Tcp服務端套接 Tcp Server Socket Construct Procedure
            /// <summary>
            /// Tcp server socket network constructor procedure.
            /// </summary>
            /// <param name="IpAddr">ip address.</param>
            /// <param name="Port">listen port.</param>
            public TcpServerSocket(IPAddress IpAddr, int Port)
            {
                try
                {
                    // 創建監聽 create tcp-listener object ..
                    this.Lister = new TcpListener(IpAddr, Port);

                    // 指定 TcpListener 是否只允許一個基礎通訊端接聽特定的通訊埠
                    // the TcpListener allows only one underlying socket to listen to a specific port ..
                    this.Lister.ExclusiveAddressUse = false;
                    // Start listening for client requests ..
                    this.Lister.Start();
                }
                catch (SocketException ex)
                {
                    string ExceptionMsg = ex.Message + ex.StackTrace;

                    // write to log file ..
                    base.Writer(AssemblyInfoClass.ProductInfo, Log.Level.Error, ExceptionMsg, true);
                }
            }

          /// <summary>
            ///  Listener accept tcp protocol client method.
          /// </summary>
          /// <returns>用來傳送和接收資料的 TcpClient</returns>
            public TcpClient AcceptListen()
            {
                // AcceptTcpClient 是封鎖的方法，會傳回可用於傳送和接收資料的 TcpClient
                //如果想要避免封鎖，請使用 Pending 方法，判斷是否可以在連入連接佇列中使用連接要求
                //accept connect request ..
                return this.Lister.AcceptTcpClient();
            }

            /// <summary>
            /// 決定是否有未處理鏈接請求 Determines if there are pending connection requests.
            /// </summary>
            /// <returns></returns>
            public bool Pending()
            {
                /*這個未封鎖的方法可判斷是否有任何暫止連接要求存在
                 * 因為 AcceptSocket 和 AcceptTcpClient 方法會封鎖執行，直到 Start 方法將連入連接要求加入佇列為止
                 * 所以 Pending 方法可用於判斷是否可以在嘗試接受之前先使用連接*/
                return this.Lister.Pending();
            }
            #endregion

            #region 停止監聽 Close Socket Base Object Resource
            /// <summary>
            /// Close network base resource.
            /// </summary>
            public void Close()
            {
                if (this.Lister != null)
                    // stop listening ..
                    this.Lister.Stop();
            }
            #endregion
        }

        /// <summary>
        /// Single client network operating procedure.
        /// </summary>
        public class SingleClient : Log
        {
            #region Declare Global Variable
            // tcp client object ..
            private TcpClient clientSocket = null;

            // network stream object ..
            private NetworkStream clientStream = null;
            #endregion

            #region 構造客戶端數據流物件 Assign Single Client Stream Object Procedure
            /// <summary>
            /// Assign single client object
            /// </summary>
            /// <param name="SocketBaseObject">tcp client object</param>
            public SingleClient(object SocketBaseObject)
            {
                // assign client socket base object ..
                this.clientSocket = (TcpClient)SocketBaseObject;

                // get a stream object for read and write data ..
                this.clientStream = ((TcpClient)SocketBaseObject).GetStream();
            }
            #endregion

            #region 發送接收網絡數據流 Send & Receive Network Stream Event Procedure
            /// <summary>
            /// 發送數據 Send data method.
            /// </summary>
            /// <param name="Data">transport data bytes.</param>
            public void Send(byte[] Data)
            {
                try
                {
                    if (this.clientStream.CanWrite)
                    {
                        /*Socket.Poll，會指定 Socket 的狀態。指定 selectMode 參數所需的 SelectMode.SelectRead，以判斷 Socket 是否為可讀取
                         * 指定 SelectMode..::.SelectWrite 以判斷 Socket 是否為可寫入
                         * 您可以使用 SelectMode..::.SelectError，偵測錯誤條件
                         * Poll 在到達指定的時間週期 (以 microseconds微妙為單位) 之前，會一直封鎖執行
                         * 若要不限時間地等待回應，請將 microSeconds 參數設定為負整數
                         * 如果您要檢查多個通訊端的狀態，則 Select 會是一個較多人偏好使用的方法
                         * 這個方法無法偵測某些類型的連線問題，例如，網路線中斷，或遠端主機不正常關機
                         * 您必須嘗試傳送或接收資料，以偵測這些類型的錯誤
                         * SelectMode.SelectWrite如果處理 Connect 且已成功建立連接或可以傳送資料，則為 true；否則，傳回 false
                         */

                        if (this.clientSocket.Client.Poll(500000, SelectMode.SelectWrite))
                        {
                            //for (int i = 0; i < Data.Length; i++)
                            //{
                            //    // 寫入一個位元組至資料流的目前位置，並將資料流位置推進一個位元組 write byte to network stream ..
                            //    /*【實作器注意事項】 Stream 上的預設實作會建立新的單一位元組陣列，並接著呼叫 Write
                            //     * 雖然這在形式上是正確的，但是無效
                            //     * 任何具有內部緩衝區的資料流應該覆寫這個方法，並提供直接寫入緩衝區的更有效率版本
                            //     * 避免在每個呼叫上配置額外的陣列
                            //     * */
                            //    this.clientStream.WriteByte(Data[i]);
                            //}

                            this.clientStream.Write(Data, 0, Data.Length);
                        }
                    }
                }
                catch (Exception ex)
                {
                    string ExceptionMsg = ex.Message + ex.StackTrace;

                    // write to log file ..
                    base.Writer(AssemblyInfoClass.ProductInfo, Log.Level.Error, ExceptionMsg, true);
                }
            }

            /// <summary>
            /// 接受數據 Receive data method.
            /// </summary>
            /// <returns>System.Byte[]</returns>
            public byte[] Receive()
            {
                // 建立緩沖區 create temporary memory storage space ..
                using (MemoryStream __stream = new MemoryStream())
                {
                    try
                    {
                        if (this.clientStream.CanRead)
                        {
                            if (this.clientSocket.Client.Poll(-1, SelectMode.SelectRead))
                            {
                                if (!this.clientSocket.Client.Poll(500000, SelectMode.SelectError))
                                {
                                    while (this.clientStream.DataAvailable)
                                    {
                                        int readValue = this.clientStream.ReadByte();

                                        // 寫入數據 write byte to memory stream ..
                                        if (readValue != -1)
                                        {
                                            __stream.WriteByte((byte)readValue);
                                        }
                                        else
                                        {
                                            break;
                                        }

                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        // cleanup stream byte data ..
                        __stream.Flush();

                        string ExceptionMsg = ex.Message + ex.StackTrace;

                        // write to log file ..
                        base.Writer(AssemblyInfoClass.ProductInfo, Log.Level.Error, ExceptionMsg, true);
                    }
                    //轉換數據流為數組
                    return __stream.ToArray();
                }
            }
            #endregion

            #region 關閉鏈接 Close Single Client Procedure
            /// <summary>
            /// Close single client object instance
            /// </summary>
            public void Close()
            {
                // close network stream object ..
                this.clientStream.Close();

                // close the client connect ..
                this.clientSocket.Close();
            }
            #endregion
        }

        /// <summary>
        /// Client network service by tcp communication protocol.
        /// </summary>
        public class TcpClientSocket : Log
        {
            #region Declare Global Variable
            // tcp client object ..
            private TcpClient clientSocket = null;

            // network stream object ..
            private NetworkStream clientStream = null;
            #endregion

            #region 創建Tcp客戶端套接字 Tcp Client Socket Construct Procedure
            /// <summary>
            /// 創建Tcp客戶端套接字Tcp client socket network constructor procedure.
            /// </summary>
            /// <param name="IpAddr">connect internet protocol address.</param>
            /// <param name="Port">connect port.</param>
            public TcpClientSocket(IPAddress IpAddr, int Port)
            {
                try
                {
                    // create tcp client object ..
                    this.clientSocket = new TcpClient();

                    // connect remote machine ..
                    this.clientSocket.Connect(IpAddr, Port);

                    // get a client stream for read and write data ..
                    this.clientStream = this.clientSocket.GetStream();
                }
                catch (SocketException ex)
                {
                    string ExceptionMsg = ex.Message + ex.StackTrace;

                    if (ex.SocketErrorCode != SocketError.ConnectionRefused)
                        // write to log file ..
                        base.Writer(AssemblyInfoClass.ProductInfo, Log.Level.Error, ExceptionMsg, true);
                }
            }
            #endregion

            #region 屬性 設定網絡連接 Network Connected Property
            /// <summary>
            /// 獲得當前鏈接狀態 Get current connect status.
            /// </summary>
            public bool IsConnected
            {
                get
                {
                    // declare result variable ..
                    bool result = true;

                    try
                    {
                        // stop blocking ..
                        //如果 Socket 可區塊化，則為 true，否則為 false。預設值為 true。
                        this.clientSocket.Client.Blocking = false;
                        // send empty byte to remote ..
                        this.clientSocket.Client.Send(new byte[1], 0, 0);
                    }
                    catch (SocketException ex)
                    {
                        // if socket native error code is 10035, still connected; otherwise the socket disconnected ..
                        if (10035 != ex.NativeErrorCode)
                            result = false;
                    }
                    finally
                    {
                        // restore base socket blocking ..
                        this.clientSocket.Client.Blocking = true;
                    }

                    return result;
                }
            }
            #endregion

            #region 發送接收網絡數據流 Send & Receive Network Stream Event Procedure
            /// <summary>
            /// 發送數據 Send data method.
            /// </summary>
            /// <param name="Data">transport data bytes.</param>
            public void Send(byte[] Data)
            {
                try
                {
                    if (this.clientStream.CanWrite)
                    {
                        // if (this.clientSocket.Client.Poll(500000, SelectMode.SelectWrite))
                        if (this.clientSocket.Client.Poll(500000, SelectMode.SelectWrite))
                        {
                            this.clientStream.Write(Data, 0, Data.Length);
                            //for (int i = 0; i < Data.Length; i++)
                            //{
                            //    // write byte to network stream ..
                            //    this.clientStream.WriteByte(Data[i]);
                            //}
                        }
                    }
                }
                catch (Exception ex)
                {
                    // write to log file ..
                    string ExceptionMsg = ex.Message + ex.StackTrace;
                    base.Writer(AssemblyInfoClass.ProductInfo, Log.Level.Error, ExceptionMsg, true);
                }
            }

            /// <summary>
            /// 接受數據 Receive data method.
            /// </summary>
            /// <returns>System.Byte[]</returns>
            public byte[] Receive()
            {
                // create temporary memory storage space ..
                using (MemoryStream __stream = new MemoryStream())
                {
                    try
                    {
                        if (this.clientStream.CanRead)
                        {
                            // SelectMode.SelectRead 如果已呼叫Listen且連接暫止時、或資料可供讀取、或連接已經關閉（重設或結束），則為 true
                            //等待回應的時間，以微秒為單位；？？？？？？？？？
                            if (this.clientSocket.Client.Poll(-1, SelectMode.SelectRead))
                            {
                                //SelectMode.SelectError 如果處理的Connect 沒有封鎖且連接已失敗時、或未設定 OutOfBandInline 且 Out-of-Band Data 可用時，則為 true
                                if (!this.clientSocket.Client.Poll(500000, SelectMode.SelectError))
                                {
                                    //while (this.clientStream.DataAvailable && (-1 != this.clientStream.ReadByte()))
                                    //    // write byte to memory stream ..
                                    //    __stream.WriteByte((byte)this.clientStream.ReadByte());
                                    while (this.clientStream.DataAvailable)
                                    {
                                        int result = this.clientStream.ReadByte();
                                        if (result != -1)
                                        {
                                            __stream.WriteByte((byte)result);
                                        }
                                        else
                                        {
                                            break;
                                        }
                                    }
                                }
                            }

                        }
                    }
                    catch (Exception ex)
                    {
                        // 清空緩存數據 cleanup stream byte data ..
                        __stream.Flush();
                        // write to log file ..
                        string ExceptionMsg = ex.Message + ex.StackTrace;
                        base.Writer(AssemblyInfoClass.ProductInfo, Log.Level.Error, ExceptionMsg, true);
                    }
                    return __stream.ToArray();
                }
            }

            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            public byte[] Receive1()
            {
                // create temporary memory storage space ..
                using (MemoryStream __stream = new MemoryStream())
                {
                    try
                    {
                        if (this.clientStream.CanRead)
                        {
                            // SelectMode.SelectRead 如果已呼叫Listen且連接暫止時、或資料可供讀取、或連接已經關閉（重設或結束），則為 true

                            //等待回應的時間，以微秒為單位；？？？？？？？？？
                            if (this.clientSocket.Client.Poll(5000000, SelectMode.SelectRead))
                            {
                                //SelectMode.SelectError 如果處理的Connect 沒有封鎖且連接已失敗時、或未設定 OutOfBandInline 且 Out-of-Band Data 可用時，則為 true
                                if (!this.clientSocket.Client.Poll(500000, SelectMode.SelectError))
                                {
                                    //while (this.clientStream.DataAvailable && (-1 != this.clientStream.ReadByte()))
                                    //    // write byte to memory stream ..
                                    //    __stream.WriteByte((byte)this.clientStream.ReadByte());
                                    while (this.clientStream.DataAvailable)
                                    {
                                        int result = this.clientStream.ReadByte();
                                        if (result != -1)
                                        {
                                            __stream.WriteByte((byte)result);
                                        }
                                        else
                                        {
                                            break;
                                        }
                                    }
                                }
                            }

                        }
                    }
                    catch (Exception ex)
                    {
                        // 清空緩存數據 cleanup stream byte data ..
                        __stream.Flush();
                        // write to log file ..
                        string ExceptionMsg = ex.Message + ex.StackTrace;
                        base.Writer(AssemblyInfoClass.ProductInfo, Log.Level.Error, ExceptionMsg, true);
                    }
                    return __stream.ToArray();
                }
            }
            #endregion

            #region 關閉套接 Close Socket Base Object Resource
            /// <summary>
            /// Close network base resource.
            /// </summary>
            public void Close()
            {
                if (null != this.clientStream)
                    // close network stream object ..
                    this.clientStream.Close();

                // close client object instance ..
                this.clientSocket.Close();
            }
            #endregion
        }
    }
}