using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;
using System.Net;
using System.Xml;

namespace RenbarLib.Environment
{
    /// <summary>
    /// Application singleton class.
    /// </summary>
    public class AppSingleton
    {
        #region 聲明變量 Declare Global Variable Section
        // Mutex object ..互斥物件
        private static global::System.Threading.Mutex __Mu = null;
        #endregion

        #region Application.Run的多载 Application Run Overload Method Event Procedure
        /// <summary>
        /// 在当前线程上開始執行目前執行緒的標準應用程式訊息迴圈，而不需表單
        /// Begins running a standard application message loop on the current thread, without a form. (With System.Threading.Mutex)
        /// </summary>
        public static void Run()
        {
            //判断是否被锁定
            if (IsFirstInstance())
            {
                //释放资源
                Application.ApplicationExit += new EventHandler(OnExit);
                Application.Run();
            }
            else
            {
                //？？？？調用Win32 API？？？？？？？？？？？？？？
                //
                System.IntPtr __hwnd = Win32.PlatformInvoke.FindWindow(null, null);
                //该函数设置指定窗口的显示状态
                Win32.PlatformInvoke.ShowWindow(__hwnd, (int)Win32.PlatformInvoke.ShowWindowStyles.Restore);
                //该函数将创建指定窗口的线程设置到前台，并且激活该窗口。键盘输入转向该窗口，并为用户改各种可视的记号。
                //系统给创建前台窗口的线程分配的权限稍高于其他线程。
                Win32.PlatformInvoke.SetForegroundWindow(__hwnd);
            }
        }

        /// <summary>
        /// 以应用程序上下文ApplicationContext 開始執行目前執行緒的標準應用程式訊息迴圈。
        /// An System.Windows.Forms.ApplicationContext in which the Application is run. (With System.Threading.Mutex)
        /// </summary>
        /// <param name="Context">application context information.</param>
        public static void Run(global::System.Windows.Forms.ApplicationContext Context)
        {
            if (IsFirstInstance())
            {
                Application.ApplicationExit += new EventHandler(OnExit);
                Application.Run(Context);
            }
            else
            {
                //
                System.IntPtr __hwnd = Win32.PlatformInvoke.FindWindow(null, Context.MainForm.Text);
                Win32.PlatformInvoke.ShowWindow(__hwnd, (int)Win32.PlatformInvoke.ShowWindowStyles.Restore);
                Win32.PlatformInvoke.SetForegroundWindow(__hwnd);
            }
        }

        /// <summary>
        /// 開始執行目前執行緒上的標準應用程式訊息迴圈，並顯示指定的表單。
        /// A System.Windows.Forms.Form that represents the form to make visible. (With System.Threading.Mutex)
        /// </summary>
        /// <param name="MainForm">application form.</param>
        public static void Run(global::System.Windows.Forms.Form MainForm)
        {
            if (IsFirstInstance())
            {
                Application.ApplicationExit += new EventHandler(OnExit);
                Application.Run(MainForm);
            }
            else
            {
                //获得当前正在执行的应用程序集合的版权信息
                System.IntPtr __hwnd = Win32.PlatformInvoke.FindWindow(null, AssemblyInfoClass.ProductInfo);
                Win32.PlatformInvoke.ShowWindow(__hwnd, (int)Win32.PlatformInvoke.ShowWindowStyles.Restore);
                Win32.PlatformInvoke.SetForegroundWindow(__hwnd);
            }
        }
        #endregion

        #region 私有方法 Check First Instance Procedure
        /*Mutex.WaitOne(TimeSpan, Boolean)封鎖目前的執行緒，直到目前的執行個體收到通知為止；
         * 使用 TimeSpan 來測量時間間隔，並指定要不要在等候之前先離開同步領域。*/
        /// <summary>
        /// 判断是否被锁定
        /// </summary>
        /// <returns></returns>
        private static bool IsFirstInstance()
        {
            // create a new instance of the Mutex class ..
            __Mu = new global::System.Threading.Mutex(false, AssemblyInfoClass.GuidInfo);

            //检查当前线程是否被锁定 check current thread whether were locking ..
            if (__Mu.WaitOne(TimeSpan.Zero, false)) 
                return true;
            else
                return false;
        }
        /// <summary>
        /// 释放资源
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private static void OnExit(object sender, EventArgs args)
        {
            // releases the Mutex once ..
            __Mu.ReleaseMutex();

            // releases all resources ..
            __Mu.Close();
        }
        #endregion
    }

    /// <summary>
    /// 信息集合类 Assembly information class.
    /// </summary>
    public static class AssemblyInfoClass
    {
        #region 获取Assembly的版权、产品等各项信息 Assembly Information Porperties
        /// <summary>
        /// Get currently executing assembly copyright information.
        /// </summary>
        public static string CopyrightInfo
        {
            get
            {
                object[] attributes = Assembly.GetEntryAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
                if (attributes.Length == 0)
                    return string.Empty;
                else
                    return ((AssemblyCopyrightAttribute)attributes[0]).Copyright;
            }
        }

        /// <summary>
        /// Get currently executing assembly company information.
        /// </summary>
        public static string CompanyInfo
        {
            get
            {
                object[] attributes = Assembly.GetEntryAssembly().GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
                if (attributes.Length == 0)
                    return string.Empty;
                else
                    return ((AssemblyCompanyAttribute)attributes[0]).Company;
            }
        }

        /// <summary>
        /// Get currently executing assembly description information.
        /// </summary>
        public static string DescriptionInfo
        {
            get
            {
                object[] attributes = Assembly.GetEntryAssembly().GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false);
                if (attributes.Length == 0)
                    return string.Empty;
                else
                    return ((AssemblyDescriptionAttribute)attributes[0]).Description;
            }
        }

        /// <summary>
        /// Get currently executing assembly guid information.
        /// </summary>
        public static string GuidInfo
        {
            get
            {
                object[] attributes = Assembly.GetEntryAssembly().GetCustomAttributes(typeof(GuidAttribute), false);

                if (attributes.Length == 0)
                    return string.Empty;
                else
                    return ((GuidAttribute)attributes[0]).Value.ToUpper();
            }
        }

        /// <summary>
        /// Get currently executing assembly product information.
        /// </summary>
        public static string ProductInfo
        {
            get
            {
                object[] attributes = Assembly.GetEntryAssembly().GetCustomAttributes(typeof(AssemblyProductAttribute), false);

                if (attributes.Length == 0)
                    return string.Empty;
                else
                    return ((AssemblyProductAttribute)attributes[0]).Product;
            }
        }

        /// <summary>
        /// Get currently executing assembly version information.
        /// </summary>
        public static string VersionInfo
        {
            get
            {
                return Assembly.GetEntryAssembly().GetName().Version.ToString();
            }
        }
        #endregion
    }

    /// <summary>
    /// 通用服务类 Commmon service class.
    /// </summary>
    public class Service : Log
    {
        #region Get Logical Processors Procedure
        /// <summary>
        /// 获取逻辑处理器的数量 Get number of logical processors.
        /// </summary>
        public ushort Cores
        {
            get
            {
                ushort result = 0;

                try
                {
                    // create management seacher object ..
                    global::System.Management.ManagementObjectSearcher _seacher
                        = new global::System.Management.ManagementObjectSearcher();

                    // set windows management instrumentation object query ..
                    _seacher.Query = new global::System.Management.ObjectQuery("Select * From Win32_Processor");

                    // get number of logical processors ..
                    foreach (global::System.Management.ManagementObject obj in _seacher.Get())
                    {
                        result ++;
                    }
                }
                catch (global::System.Management.ManagementException ex)
                {
                    string ExceptionMsg = ex.Message + ex.StackTrace;

                    // write to log file ..
                    base.Writer(AssemblyInfoClass.ProductInfo, Log.Level.Error, ExceptionMsg, true);
                }

                return result;
            }
        }
        #endregion

        #region Get Parent Process Procedure
        /// <summary>
        /// 以指定處理序 ID 的方式擷取创建此處理序的父处理序(Process) （使能夠被存取、啟動或停止） Get parent process.
        /// </summary>
        /// <param name="ProcessId">process id.</param>
        /// <returns>System.Diagnostics.Process</returns>
        public Process GetParentProcess(int ProcessId)
        {
            // declare parent process result ..
            Process parentProc = null;

            try
            {
                // create process entry ..
                Win32.PlatformInvoke.PROCESSENTRY32 procEntry = new Win32.PlatformInvoke.PROCESSENTRY32();

                // set entry dword value size ..
                procEntry.dwSize = (UInt32)Marshal.SizeOf(typeof(Win32.PlatformInvoke.PROCESSENTRY32));
                IntPtr handleToSnapshot = Win32.PlatformInvoke.CreateToolhelp32Snapshot((uint)Win32.PlatformInvoke.SnapshotFlags.Process, 0);

                if (Win32.PlatformInvoke.Process32First(handleToSnapshot, ref procEntry))
                {
                    do
                    {
                        if (ProcessId == procEntry.th32ProcessID)
                        {
                            //获取父处理序信息
                            parentProc = Process.GetProcessById((int)procEntry.th32ParentProcessID);
                            break;
                        }
                    } while (Win32.PlatformInvoke.Process32Next(handleToSnapshot, ref procEntry));
                }
                else
                {
                    string ExceptionMsg = string.Format("Failed with win32 error code {0}", Marshal.GetLastWin32Error());

                    // write to log file ..
                    base.Writer(AssemblyInfoClass.ProductInfo, Log.Level.Error, ExceptionMsg, true);
                }
            }
            catch (Exception ex)
            {
                string ExceptionMsg = ex.Message + ex.StackTrace;

                // write to log file ..
                base.Writer(AssemblyInfoClass.ProductInfo, Log.Level.Error, ExceptionMsg, true);
            }

            return parentProc;
        }
        #endregion

        #region Get Time Interval Procedure
        /// <summary>
        /// 虚方法 计算时间间隔 Get time interval.
        /// </summary>
        /// <param name="NewDateTime">new datetime.</param>
        /// <param name="OldDateTime">old datetime.</param>
        /// <returns>System.String (hours:minutes:seconds)</returns>
        public virtual string DateTimeInterval(DateTime NewDateTime, DateTime OldDateTime)
        {
            string result = string.Empty;

            TimeSpan ts1 = new TimeSpan(NewDateTime.Ticks);
            TimeSpan ts2 = new TimeSpan(OldDateTime.Ticks);
            TimeSpan ts = ts1.Subtract(ts2).Duration();

            result = ts.Hours.ToString().PadLeft(2, '0') + ":"
                + ts.Minutes.ToString().PadLeft(2, '0') + ":"
                + ts.Seconds.ToString().PadLeft(2, '0');

            return result;
        }
        #endregion

        #region Get Current Local DateTime Properties
        /// <summary>
        /// 获取本机当前时间  Get current local date time.
        /// </summary>
        public static string GetSysDateTime
        {
            get
            {
                string
                    date = DateTime.Now.ToShortDateString(),
                    hour = DateTime.Now.Hour.ToString().PadLeft(2, '0'),
                    min = DateTime.Now.Minute.ToString().PadLeft(2, '0'),
                    sec = DateTime.Now.Second.ToString().PadLeft(2, '0');

                return string.Format("{0} {1}:{2}:{3}", date, hour, min, sec);
            }
        }

        /// <summary>
        /// 获取客户时间 Get custom date time format.
        /// </summary>
        public static string CustomSysDateTime
        {
            get
            {
                string
                    year = DateTime.Now.Year.ToString(),
                    month = DateTime.Now.Month.ToString(),
                    day = DateTime.Now.Day.ToString().PadLeft(2, '0'),
                    hour = DateTime.Now.Hour.ToString().PadLeft(2, '0'),
                    min = DateTime.Now.Minute.ToString().PadLeft(2, '0'),
                    sec = DateTime.Now.Second.ToString().PadLeft(2, '0');

                #region Convert English Month
                switch (month)
                {
                    case "1":
                        month = "Jan";
                        break;
                    case "2":
                        month = "Feb";
                        break;
                    case "3":
                        month = "Mar";
                        break;
                    case "4":
                        month = "Apr";
                        break;
                    case "5":
                        month = "May";
                        break;
                    case "6":
                        month = "Jun";
                        break;
                    case "7":
                        month = "Jul";
                        break;
                    case "8":
                        month = "Aug";
                        break;
                    case "9":
                        month = "Sep";
                        break;
                    case "10":
                        month = "Oct";
                        break;
                    case "11":
                        month = "Nov";
                        break;
                    case "12":
                        month = "Dec";
                        break;
                }
                #endregion

                return string.Format("{0}{1}{2}-{3}{4}{5}",
                    new string[] { day, month, year, hour, min, sec });
            }
        }
        #endregion

        #region Get Environment File Path Procedure
        /// <summary>
        /// 傳回指定路徑字串(renbar client 环境设定)的目錄資訊 Get renbar client environment setting file path.
        /// </summary>
        public string EnvironemtFilePath
        {
            get
            {
                string result = string.Empty;
                //GetExecutingAssembly()  取得組件，其中含有目前正在執行的程式碼。
                //GetName 方法會傳回 AssemblyName 物件，而這個物件可以讓您存取組件顯示名稱的各個部分。
                //CodeBase 取得或設定做為 URL 之組件的位置，AssemblyName属性之一。


                result = Path.GetDirectoryName(global::System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase).Substring(6);
                /*在大部分的狀況中，Path.GetDirectoryName方法傳回的字串包含路徑中的所有字元，
                 * 但最後的 DirectorySeparatorChar 或 AltDirectorySeparatorChar 字元則不包含在內。如果路徑包含根目錄 (例如 "c:\")，則傳回 null。
                 * 請注意，這個方法不支援使用 "file:" 的路徑。
                 * 因為傳回的路徑不包含 DirectorySeparatorChar 或 AltDirectorySeparatorChar，
                 * 所以將傳回的路徑傳遞回 GetDirectoryName 方法會導致每個後續呼叫在結果字串上都有一個資料夾層次的截斷。
                 * 例如，將路徑 "C:\Directory\SubDirectory\test.txt" 傳遞至 GetDirectoryName 方法會傳回 "C:\Directory\SubDirectory"。
                 * 將該字串 "C:\Directory\SubDirectory" 傳遞至 GetDirectoryName 會導致傳回 "C:\Directory"。*/

                result += @"\Environment.rbe";

                return result;
            }
        }
        #endregion

        #region Get ServerList xml File Path Procedure
        /// <summary>
        /// 獲得Server.xml的路徑
        /// </summary>
        public string ServerListFilePath
        {
            get
            {
                string result = string.Empty;
                result = Path.GetDirectoryName(global::System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase).Substring(6);
                result += @"\ServerAddress.xml";
                return result;
            }
        }
        #endregion

        #region 序列化Serialize反序列化Deserialize多载 Object Serialize And Deserialize Procedure
        /// <summary>
        ///  建立原始物件或物件 Graph 的複製品，並使用 Deserialize 還原序列化指定路径的資料流  Object deserialize.
        /// </summary>
        /// <param name="SourcePath">deserialize object path.</param>
        /// <returns>System.Object</returns>
        public object Deserialize(string SourcePath)
        {
            // declare deserialization object result ..
            object desObj = null;
            try
            {
                // deserialize object with file stream class object ..
                using (FileStream fs = new FileStream(SourcePath, FileMode.Open, FileAccess.Read, FileShare.None))
                {
                    //IFormatter提供用於格式化已序列化物件的功能。
                    //BinaryFormatter()以二進位格式序列化和還原序列化物件或整個連接物件的Graph。提供用於格式化已序列化物件的功能。
                    //将IFormatter实例化为BinaryFormatter   create binary formatter object instance ..
                    IFormatter Formatter = new BinaryFormatter();
                    //F7300290 【在完成剖析之前已達資料流末端】
                    fs.Seek(0, SeekOrigin.Begin);
                    // deserializing object ..
                    desObj = Formatter.Deserialize(fs);
                    fs.Flush();
                }
            }
            catch (SerializationException ex)
            {
                string ExceptionMsg = ex.Message + ex.StackTrace;            
                // write to log file ..
                base.Writer(AssemblyInfoClass.ProductInfo, Log.Level.Error, ExceptionMsg, true);
            }

            return desObj;
        }

        /// <summary>
        /// Object deserialize.
        /// </summary>
        /// <param name="Data">data buffer object.</param>
        /// <returns>System.Object</returns>
        public object Deserialize(byte[] Data)
        {
            // declare deserialization object result ..
            object desObj = null;

            try
            {
                if (Data.Length > 0)
                {
                    using (MemoryStream Strem = new MemoryStream(Data))
                    {
                        // create binary formatter object instance ..
                        IFormatter Formatter = new BinaryFormatter();
                        //F7300290 【在完成剖析之前已達資料流末端】
                        Strem.Seek(0, SeekOrigin.Begin);
                        // deserializing object ..
                        desObj = Formatter.Deserialize(Strem);

                        Strem.Close();
                        Strem.Dispose();
                    }

                }
            }
            catch (SerializationException ex)
            {
                string ExceptionMsg = ex.Message + ex.StackTrace;
                // write to log file ..
                base.Writer(AssemblyInfoClass.ProductInfo, Log.Level.Error, ExceptionMsg, true);
                //string TransData = "Received Wrong data:\r\n" + DateTime.Now.ToLongTimeString() + "_" + DateTime.Now.Millisecond.ToString() +
                //    System.Text.Encoding.Default.GetString(Data);
                //base.Writer("+Debug+", Log.Level.Error, TransData, true);
            }
            return desObj;
        }

        /// <summary>
        /// Object serialize.
        /// </summary>
        /// <param name="TargetPath">target path.</param>
        /// <param name="SerializeObject">data object.</param>
        public void Serialize(string TargetPath, object SerializeObject)
        {
            try
            {
                // serialize object with file stream class object ..
                using (FileStream fs = new FileStream(TargetPath, FileMode.Create, FileAccess.Write, FileShare.None))
                { 
                    // create binary formatter object instance ..
                    IFormatter Formatter = new BinaryFormatter();
                    //F7300290 【在完成剖析之前已達資料流末端】
                    fs.Seek(0, SeekOrigin.Begin);                    
                    // serializing object ..
                    Formatter.Serialize(fs, SerializeObject);

                    fs.Flush();
                    fs.Close();
                    fs.Dispose();
                }
            }
            catch (SerializationException ex)
            {
                string ExceptionMsg = ex.Message + ex.StackTrace;
                // write to log file ..
                base.Writer(AssemblyInfoClass.ProductInfo, Log.Level.Error, ExceptionMsg, true);
            }
        }

        /// <summary>
        /// Object serialization.
        /// </summary>
        /// <param name="Data">data object.</param>
        /// <returns>System.Byte[]</returns>
        public byte[] Serialize(object Data)
        {
            byte[] BufferData = null;
            try
            {
                // create memory stream object instance ..
                using (MemoryStream Ms = new MemoryStream())
                {

                    // create binary formatter object instance ..
                    IFormatter Formatter = new BinaryFormatter();
                    //F7300290 【在完成剖析之前已達資料流末端】
                    Ms.Seek(0, SeekOrigin.Begin);
                    // serializing object ..
                    Formatter.Serialize(Ms, Data);
                    //return
                    BufferData = Ms.ToArray();

                    //string TransData = "Send  data:\r\n" + DateTime.Now.ToLongTimeString() + "_" + DateTime.Now.Millisecond.ToString() +
                    //System.Text.Encoding.Default.GetString(BufferData);
                    //base.Writer("+Debug+", Log.Level.Error, TransData, true);

                    Ms.Flush();
                    Ms.Close();
                    Ms.Dispose();
                }
            }
            catch (SerializationException ex)
            {
                string ExceptionMsg = ex.Message + ex.StackTrace;
                // write to log file ..
                base.Writer(AssemblyInfoClass.ProductInfo, Log.Level.Error, ExceptionMsg, true);
            }
            return BufferData;
        }
        #endregion

        #region Operate xml servers list
        /// <summary>
        /// get server list from the xml
        /// </summary>
        /// <param name="XMLFilePath">xml file path</param>
        /// <returns>server list</returns>
        public List<RenbarServer> GetServers(string XMLFilePath)
        {
            List<RenbarServer> ServerList = new List<RenbarServer>();
            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(XMLFilePath);

                XmlNode AllServers = xmlDoc.SelectSingleNode("ServerAddress");
                XmlNodeList Servers = AllServers.ChildNodes;

                foreach (XmlNode Server in Servers)
                {
                    XmlElement CurrentServer = (XmlElement)Server;
                    if (CurrentServer.GetAttribute("IP") != null)
                    {
                        RenbarServer thisserver = new RenbarServer();

                        thisserver.ServerIP = IPAddress.Parse(CurrentServer.GetAttribute("IP"));
                        thisserver.ServerPort = ushort.Parse(CurrentServer.GetAttribute("Port"));
                        if (CurrentServer.GetAttribute("IsMaster") == "Y")
                        {
                            thisserver.IsMaster = true;
                        }
                        else
                        {
                            thisserver.IsMaster = false;
                        }
                        ServerList.Add(thisserver);
                    }

                }
            }
            catch (Exception ex)
            {
                string ExceptionMsg = ex.Message + ex.StackTrace;
                // write to log file ..
                base.Writer(AssemblyInfoClass.ProductInfo, Log.Level.Error, ExceptionMsg, true);
            }
            return ServerList;
        }

        /// <summary>
        /// Add Server Record to xml file
        /// </summary>
        /// <param name="XMLFilePath"></param>
        /// <param name="NewServer"></param>
        /// <returns>Whether add server succed</returns>
        public bool AddServer(string XMLFilePath, RenbarServer NewServer)
        {
            bool addSuccess = false;

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(XMLFilePath);
            XmlNode AllServers = xmlDoc.SelectSingleNode("ServerAddress");
            //創建新節點
            XmlElement Servers = xmlDoc.CreateElement("Server");
            XmlAttribute SIp = xmlDoc.CreateAttribute("IP");
            SIp.Value = NewServer.ServerIP.ToString().Trim();
            XmlAttribute SPort = xmlDoc.CreateAttribute("Port");
            SPort.Value = NewServer.ServerPort.ToString().Trim();
            XmlAttribute SIsMaster = xmlDoc.CreateAttribute("IsMaster");
            if (NewServer.IsMaster)
            {
                SIsMaster.Value = "Y";
            }
            else
            {
                SIsMaster.Value = "N";
            }

            try
            {
                //添加數據
                Servers.Attributes.Append(SIp);
                Servers.Attributes.Append(SPort);
                Servers.Attributes.Append(SIsMaster);

                AllServers.AppendChild(Servers);
                xmlDoc.Save(XMLFilePath);

                addSuccess = true;
            }
            catch (Exception ex)
            {
                string ExceptionMsg = ex.Message + ex.StackTrace;
                // write to log file ..
                base.Writer(AssemblyInfoClass.ProductInfo, Log.Level.Error, ExceptionMsg, true);
            }
            return addSuccess;
        }

        /// <summary>
        /// Update selected Server
        /// </summary>
        /// <param name="XMLFilePath"></param>
        /// <param name="SelectIp"></param>
        /// <returns></returns>
        public bool UpdateServer(string XMLFilePath, RenbarServer NewServer)
        {
            bool UpdateSucced = false;
            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(XMLFilePath);

                XmlNode AllServers = xmlDoc.SelectSingleNode("ServerAddress");

                #region Select old node
                XmlNode UpdateServer =
                    xmlDoc.SelectSingleNode("//Server[@IP='" + NewServer.ServerIP.ToString().Trim() + "']");
                #endregion

                #region Creat new server
                XmlElement Servers = xmlDoc.CreateElement("Server");
                XmlAttribute SIp = xmlDoc.CreateAttribute("IP");
                SIp.Value = NewServer.ServerIP.ToString().Trim();
                XmlAttribute SPort = xmlDoc.CreateAttribute("Port");
                SPort.Value = NewServer.ServerPort.ToString().Trim();
                XmlAttribute SIsMaster = xmlDoc.CreateAttribute("IsMaster");
                if (NewServer.IsMaster)
                {
                    SIsMaster.Value = "Y";
                }
                else
                {
                    SIsMaster.Value = "N";
                }
                Servers.Attributes.Append(SIp);
                Servers.Attributes.Append(SPort);
                Servers.Attributes.Append(SIsMaster);
                #endregion

                //replace
                AllServers.ReplaceChild(Servers, UpdateServer);

                xmlDoc.Save(XMLFilePath);
                UpdateSucced = true;
            }
            catch (Exception ex)
            {
                string ExceptionMsg = ex.Message + ex.StackTrace;
                // write to log file ..
                base.Writer(AssemblyInfoClass.ProductInfo, Log.Level.Error, ExceptionMsg, true);
            }
            return UpdateSucced;
        }

        /// <summary>
        /// Delete selected Server
        /// </summary>
        /// <param name="XMLFilePath"></param>
        /// <param name="SelectIp"></param>
        /// <returns></returns>
        public bool DeleteServer(string XMLFilePath, string SelectIp)
        {
            bool DeleteSuccess = false;
            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(XMLFilePath);
                XmlNode AllServers = xmlDoc.SelectSingleNode("ServerAddress");
                XmlNode DeleteServer = xmlDoc.SelectSingleNode("/ServerAddress/Server[@IP='" + SelectIp + "']");
                AllServers.RemoveChild(DeleteServer);
                xmlDoc.Save(XMLFilePath);
                DeleteSuccess = true;
            }
            catch (Exception ex)
            {
                string ExceptionMsg = ex.Message + ex.StackTrace;
                // write to log file ..
                base.Writer(AssemblyInfoClass.ProductInfo, Log.Level.Error, ExceptionMsg, true);
            }
            return DeleteSuccess;
        }

        #endregion
    }

    /// <summary>
    /// Define the Server
    /// </summary>
    public class RenbarServer
    {
        private IPAddress _serverIP;
        private ushort _serverPort;
        private bool _isMaster;



        public IPAddress ServerIP
        {
            get { return _serverIP; }
            set { _serverIP = value; }
        }

        public ushort ServerPort
        {
            get { return _serverPort; }
            set { _serverPort = value; }
        }

        public bool IsMaster
        {
            get { return _isMaster; }
            set { _isMaster = value; }
        }

    }


}