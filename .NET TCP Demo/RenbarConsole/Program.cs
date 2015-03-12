using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

// import renbar class library namespace ..
using RenbarLib.Environment;
using RenbarLib.Environment.Forms.Customizations.Service;
using RenbarLib.Network;
using RenbarLib.Network.Protocol;
using RenbarLib.Network.Sockets;
using System.Threading;
using RenbarConsole.Properties;
//add

namespace RenbarConsole
{
    /*
     * 1. add -nb option (submit no backup file)提交無需備份的文件
     */
    class Program
    {
        #region 定義全局變量 Declare Global Variable Section
        // create renbar environment service class object ..
        private static Service EnvSvr = new Service();

        // create renbar log base class object ..
        // private static LogBase EnvLog = new LogBase(1048576);
        private static Log EnvLog = new Log();

        // create renbar client environment class object ..
        private static Customization EnvCust = new Customization();

        // create renbar client communication class object ..
        private static Communication EnvComm = new Communication();

        // create renbar environment setting object ..
        private static Settings EnvSetting = new Settings();

        // declare connect remote server flag ..
        private static bool IsConnected = false;
        #endregion

        #region 主函數 Main Constructor Procedure
        /// <summary>
        /// Primary renbar console client main point constructor procedure.
        /// </summary>
        /// <param name="args">parameter string.</param>
        static void Main(string[] args)
        {
            if (args.Length > 0 && args[0] != null)
            {
                #region 傳入参数非空

                Console.WriteLine("Reading EnvironemtFile from:\r\n\t" + EnvSvr.EnvironemtFilePath.ToString()+"\r\n\r\n");

               // 檢測renbar環境設定的二進制文件(Environment.rbe) check renbar environment binary file ..
                if (!File.Exists(EnvSvr.EnvironemtFilePath))
                {
                    #region 不存在，則顯示錯誤信息並寫入日志
                    Console.WriteLine("\r\n");
                    Console.WriteLine(AssemblyInfoClass.ProductInfo + AssemblyInfoClass.VersionInfo);

                    // 輸出信息提示【優先設定環境】 output error message ..
                    Console.WriteLine("\t"+ReturnCode(false));
                    string ExceptionMsg = "The EnvironemtFile wasn't found ,Please priority setting RenbarGUI environment !";
                    Console.WriteLine("\t Error: {0}\r\n\t", ExceptionMsg);
                    Console.WriteLine("=============================================================\r\n");

                    // 寫入日志 write to log file ..
                    // EnvLog.Writer(AssemblyInfoClass.ProductInfo, LogBase.Level.Error, ExceptionMsg);
                    EnvLog.Writer(AssemblyInfoClass.ProductInfo, Log.Level.Error, ExceptionMsg, false);
                    #endregion
                }
                else
                {
                    #region 存在環境文件、嘗試連接、注冊信息
                    Console.WriteLine("\t"+ReturnCode(true));
                    Console.WriteLine("\t Get the Environment setting successed! \r\n");
                    Console.WriteLine("=============================================================\r\n");
                    try
                    {
                        // 讀取並緩存環境設定數據 restore environment setting ..
                        EnvSetting = (Settings)EnvSvr.Deserialize(EnvSvr.EnvironemtFilePath);
                        string SocketAD = EnvSetting.ServerIpAddress.ToString() +":"+ EnvSetting.ServerPort.ToString();
                        Console.WriteLine("Current ServerIP and Port is:" + "\r\n\t" + SocketAD+"\r\n");
                        Console.WriteLine("=============================================================\r\n");
                       
                        
                        // 鏈接遠端服務器 connect remote server ..
                        Console.WriteLine("Try to connect to the remote server ……\r\n");
                        if (!EnvComm.Connect(EnvSetting.ServerIpAddress, EnvSetting.ServerPort))
                        {
                            #region 無法連接
                            Console.WriteLine("\t"+ReturnCode(false));//1
                            // 輸出信息並寫入日志 output error message ..
                            string ExceptionMsg = "Can't connect remote server, please check local environment settings !";
                            Console.WriteLine("\t Error: {0}\r\n\t", ExceptionMsg);
                            Console.WriteLine("=============================================================\r\n");
                            // write to log file ..
                            // EnvLog.Writer(AssemblyInfoClass.ProductInfo, LogBase.Level.Error, ExceptionMsg);
                            EnvLog.Writer(AssemblyInfoClass.ProductInfo, Log.Level.Error, ExceptionMsg, false);//？？？？？？？？？？
                            return;
                            #endregion
                        }
                        else
                        {
                            #region 可連接，則嘗試注冊本機信息
                            // 注冊本機信息 registry local machine to server request ..
                            Console.WriteLine("\t" + ReturnCode(true));
                            Console.WriteLine("\t Connect the Server successed! \r\n");
                            Console.WriteLine("=============================================================\r\n");


                            Console.WriteLine("Try to registry local machine……\r\n");

                            if (!EnvCust.RegLocalMachine(null, false, ref EnvComm))
                            {
                                Console.WriteLine("\t"+ReturnCode(false));//1
                                // 失敗 output error message ..
                                string ExceptionMsg = "\t can't registry local machine information !";
                                Console.WriteLine("\t Error: {0}\r\n\t", ExceptionMsg);
                                Console.WriteLine("=============================================================\r\n");
                            }
                            else
                            {
                                IsConnected = true;
                                Console.WriteLine("\t"+ReturnCode(true));//0
                                // 成功 output Success message ..
                                Console.WriteLine("\t Registry local machine information successed! \r\n");
                                Console.WriteLine("=============================================================\r\n");
                            }

                            #endregion
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("\t"+ReturnCode(false));
                        // 輸出異常信息 output error message ..
                        string ExceptionMsg = ex.Message + ex.StackTrace;
                        Console.WriteLine("\t Error :{0} \r\n\t", ExceptionMsg);
                        Console.WriteLine("=============================================================\r\n");
                        // 寫入日志 write to log file ..
                        // EnvLog.Writer(AssemblyInfoClass.ProductInfo, LogBase.Level.Error, ExceptionMsg);
                        EnvLog.Writer(AssemblyInfoClass.ProductInfo, Log.Level.Error, ExceptionMsg, false);
                        return;
                    }
                    #endregion

                    #region 根據指令進行操作
                    Console.WriteLine("Analysing the command:" + args[0].ToString()+"\r\n");

                    switch (args[0].ToLower())
                    {

                        #region  "-f"：[-f,×××.rbr]分析发送.rbr文件（显示0则Success，否则fail）     File Render Case
                        case "-f":
                            if (args.Length < 2)
                            {
                                Console.WriteLine("Syntax Error:\r\n");
                                Console.WriteLine("=============================================================\r\n");
                                PrintSyntax();
                                Console.WriteLine("=============================================================\r\n");
                                return;
                            }
                            else
                            {
                                Console.WriteLine("\r\n");
                                Console.WriteLine(AssemblyInfoClass.ProductInfo+AssemblyInfoClass.VersionInfo);
                                Console.WriteLine("=============================================================\r\n");
                                Console.WriteLine("Getting render file(.rbr), please wait ...\r\n");
                                global::System.Threading.Thread.Sleep(500);

                                // create check file object ..
                                FileInfo file = new FileInfo(args[1]);

                                // analysis file path and context ..
                                if (!file.Exists)
                                {
                                    Console.WriteLine("\t"+ReturnCode(false));
                                    // output error message ..
                                    string ExceptionMsg = "\t Can't find render file(.rbr), please check render file path !";
                                    Console.WriteLine("\t Error: {0}\r\n\t", ExceptionMsg);
                                    Console.WriteLine("=============================================================\r\n");
                                    return;
                                }
                                else
                                {
                                    if (!file.Extension.ToLower().Equals(".rbr"))
                                    {
                                        Console.WriteLine("\t"+ReturnCode(false));
                                        // output error message ..
                                        string ExceptionMsg = "\t The file type isn't the supported render file(.rbr) , please contact IT dept !";
                                        Console.WriteLine("\t Error: {0}\r\n\t", ExceptionMsg);
                                        Console.WriteLine("=============================================================\r\n");
                                        return;
                                    }
                                    else
                                    {
                                        Console.WriteLine("\t"+ReturnCode(true));
                                        Console.WriteLine("\t Get the render file({0}) Successed ! \r\n", args[1].ToString());
                                        Console.WriteLine("=============================================================\r\n");
                                        // analysis and send ..
                                        SendDocument(file);
                                    }
                                }
                            }
                            break;
                        #endregion

                        #region "-c"：確認是否可連接遠端服務器（显示0则Success，否则fail） Connect Remote Server Case
                        case "-c":
                            // create connect server object ..
                            ScanPort scanServer = new ScanPort();
                            // confirm whether can connect server ..
                            if (!scanServer.Scan(EnvSetting.ServerIpAddress, EnvSetting.ServerPort))
                            {
                                Console.WriteLine("\t"+ReturnCode(false));
                                // output error message ..
                                string ExceptionMsg = "Can't connect remote server, please check local environment settings !";
                                Console.WriteLine("\t Error: {0} \r\n\t", ExceptionMsg);
                                Console.WriteLine("=============================================================\r\n");
                                // write to log file ..
                                // EnvLog.Writer(AssemblyInfoClass.ProductInfo, LogBase.Level.Error, ExceptionMsg);
                                EnvLog.Writer(AssemblyInfoClass.ProductInfo, Log.Level.Error, ExceptionMsg, false);
                            }
                            else
                            {
                                Console.WriteLine("\t"+ReturnCode(true));
                                Console.WriteLine(" \t Connected! \r\n");
                                Console.WriteLine("=============================================================\r\n");
                            }
                            break;
                        #endregion

                        #region  "-g"：返回本地機器是否有GUI應用程序 Get GUI Process Case
                        case "-g":
                            // check the gui app exist in machine process ..
                            if (Process.GetProcessesByName("RenbarGUI").Length > 0)
                            {
                                Console.WriteLine("\t" + ReturnCode(true));
                                Console.WriteLine("\t RenbarGUI is running……");
                                Console.WriteLine("=============================================================\r\n");
                            }
                            else
                            {
                                Console.WriteLine("\t" + ReturnCode(false));
                                Console.WriteLine("\t No RenbarGUI is running");
                                Console.WriteLine("=============================================================\r\n");
                                try
                                {
                                    Console.WriteLine("Trying to start the RenbarGUI……");
                                    Process p = new Process();
                                    p.StartInfo.FileName = Setting.Default.RenbarGUI_StartUpPath;
                                    //p.StartInfo.Arguments = Console.ReadLine();
                                    p.Start();
                                    Console.WriteLine("\t" + ReturnCode(true));
                                    Console.WriteLine("\t Start RenbarGUI successed!");
                                    Console.WriteLine("=============================================================\r\n");
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine("\t" + ReturnCode(false));
                                    string ExceptionMsg = ex.Message + ex.StackTrace;
                                    Console.WriteLine("\t Failed to start RenbarGUI ,An error happens: {0}\r\n", ExceptionMsg);
                                    Console.WriteLine("=============================================================\r\n");
                                    // write to log file ..
                                    // EnvLog.Writer(AssemblyInfoClass.ProductInfo, LogBase.Level.Error, ExceptionMsg);
                                    EnvLog.Writer(AssemblyInfoClass.ProductInfo, Log.Level.Error, ExceptionMsg, false);
                                }
                            }
                            break;
                        #endregion

                        #region "-p"：獲得當前Primary_Pool清單 Get Primary Pool Info Case
                        case "-p":

                            Console.WriteLine("Getting the Primary pool information……\r\n\r\n");
                            string primary_result = GetPoolInfo(null, PoolType.Primary_Pool);

                            if (!string.IsNullOrEmpty(primary_result))
                            {
                                // return code ..
                                Console.WriteLine("\t"+ReturnCode(true));
                                // return message ..
                                Console.WriteLine(primary_result);
                                Console.WriteLine("=============================================================\r\n");
                            }
                            else
                            {
                                // return code ..
                                Console.WriteLine("\t"+ReturnCode(false));
                                Console.WriteLine("\t Failed to get the information……");
                                Console.WriteLine("=============================================================\r\n");
                            }
                            break;
                        #endregion

                        #region "-sp"：獲得Secondary_Pool清單 Get Secondary Pool Info Case
                        case "-sp":
                            try
                            {
                                Console.WriteLine("Getting the secondary pool information……\r\n\r\n");
                                string secondary_result = GetPoolInfo(null, PoolType.Secondary_Pool);

                                if (!string.IsNullOrEmpty(secondary_result))
                                {
                                    // return code ..
                                    Console.WriteLine("\t" + ReturnCode(true));
                                    // return message ..
                                    Console.WriteLine(secondary_result);
                                    Console.WriteLine("=============================================================\r\n");
                                }
                                else
                                {
                                    // return code ..
                                    Console.WriteLine("\t" + ReturnCode(true));
                                    Console.WriteLine("\t No shareable pool exist ……");
                                    Console.WriteLine("=============================================================\r\n");
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("\t" + ReturnCode(false));
                                Console.WriteLine("\t Failed to get the shareable pool info ……");
                                Console.WriteLine("=============================================================\r\n");
                                // write to log file ..
                                string ExceptionMsg = ex.Message + ex.StackTrace;
                                // EnvLog.Writer(AssemblyInfoClass.ProductInfo, LogBase.Level.Error, ExceptionMsg);
                                EnvLog.Writer(AssemblyInfoClass.ProductInfo, Log.Level.Error, ExceptionMsg, false);
                            }
                            break;
                        #endregion

                        #region "-pd"：獲得所有群組的Detail  Get Pool Detail Info Case
                        case "-pd":
                            Console.WriteLine("Getting the detail pool information……\r\n\r\n");
                            Console.WriteLine(AssemblyInfoClass.ProductInfo);
                            Console.WriteLine(AssemblyInfoClass.VersionInfo);
                            Console.WriteLine("=============================================================\r\n");
                            Console.WriteLine(GetPoolInfo(null, PoolType.Detail));
                            Console.WriteLine("=============================================================\r\n");
                            break;
                        #endregion

                        #region "-ps"：獲得Sharable群組信息 Get Pool Sharable Info Case
                        case "-ps":
                            if (args.Length < 2)
                            {
                                Console.WriteLine("Syntax Error:\r\n");
                                Console.WriteLine("=============================================================\r\n");
                                PrintSyntax();
                                Console.WriteLine("=============================================================\r\n");
                                return;
                            }
                            else
                            {
                                Console.WriteLine("Getting the sharable pool({0}) information……\r\n\r\n", args[1].ToString());
                                string ps_result = GetPoolInfo(args[1], PoolType.Sharable);

                                if (!string.IsNullOrEmpty(ps_result))
                                {
                                    // return code ..
                                    Console.WriteLine("\t"+ReturnCode(true));
                                    // return message ..
                                    Console.WriteLine("\t {0} Is Sharable :{0}", args[1].ToString(), ps_result);
                                    Console.WriteLine("=============================================================\r\n");
                                }
                                else
                                {
                                    // return code ..
                                    Console.WriteLine("\t"+ReturnCode(false));
                                    Console.WriteLine("\t Failed to get the information……");
                                    Console.WriteLine("=============================================================\r\n");
                                }
                            }
                            break;
                        #endregion

                        #region "-pm"：獲得Machine群組信息 Get Pool Machine Info Case
                        case "-pm":
                            if (args.Length < 2)
                            {
                                Console.WriteLine("Syntax Error:\r\n");
                                Console.WriteLine("=============================================================\r\n");
                                PrintSyntax();
                                Console.WriteLine("=============================================================\r\n");
                                return;
                            }
                            else
                            {

                                Console.WriteLine("Getting the Pool({0}) Machine  information……\r\n\r\n", args[1].ToString());
                                string pm_result = GetPoolInfo(args[1], PoolType.Machine);

                                if (!string.IsNullOrEmpty(pm_result))
                                {
                                    // return code ..
                                    Console.WriteLine("\t"+ReturnCode(true));
                                    // return message ..
                                    Console.WriteLine(pm_result);
                                    Console.WriteLine("=============================================================\r\n");
                                }
                                else
                                {
                                    // return code ..
                                    Console.WriteLine("\t"+ReturnCode(false));
                                    Console.WriteLine("\t Failed to get the information……");
                                    Console.WriteLine("=============================================================\r\n");
                                }

                            }
                            break;
                        #endregion

                        #region Default條件下輸出提示信息 Print Syntax Default Case
                        default:
                            PrintSyntax();
                            break;
                        #endregion

                    }
                    #endregion

                    #region 退出，再次注冊、關閉連接
                    try
                    {

                        if (IsConnected)
                        {
                            Console.WriteLine("MISSION COMPLETE ! ! ");
                            // registry local machine to server request ..
                            if (!EnvCust.RegLocalMachine(null, true, ref EnvComm))
                            {
                                // 不能注冊本機信息則輸出信息 output error message ..
                                Console.WriteLine("\t" + ReturnCode(false));
                                string ExceptionMsg = "\t Can't registry local machine offline information !";
                                Console.WriteLine("\t Error: {0}\r\n\t", ExceptionMsg);
                                Console.WriteLine("=============================================================\r\n\r\n");
                            }
                            else
                            {
                                // 能注冊則在注冊完成后關閉連接
                                Console.WriteLine("\t" + ReturnCode(true));
                                Console.WriteLine("\tRegistry local machine offline information successed ,Disconnect…");
                                Console.WriteLine("=============================================================\r\n\r\n");                                
                            }
                        }
                    }
                    catch{ }
                    finally
                    {
                        EnvComm.Disconnect();
                    }
                    #endregion
                }

                #endregion
            }
            else
            {
                #region 傳入參數為空，則輸出其正確語法及示例提示

                PrintSyntax();
                Console.WriteLine("=============================================================\r\n\r\n");
                #endregion
            }

            #region F7300290 Exit for Debug
            if (Setting.Default.EnterToQuit == "Y")
            {
                Console.WriteLine("Press Enter to Quit !");
                Console.ReadLine();
            }
            else
            {
                Thread.Sleep(Setting.Default.PauseTime);
            }

            #endregion

        }
        /// <summary>
        ///  輸出邏輯語法 Print this console syntax.
        /// </summary>
        private static void PrintSyntax()
        {
            string result = string.Empty;
            result = "\r\n";
            result += AssemblyInfoClass.ProductInfo + "\r\n";
            result += AssemblyInfoClass.VersionInfo + "\r\n";
            result += "====================================================\r\n";
            result += "Syntax :\r\n";
            result += "\t-c confirm whether can connect remote server.\r\n";
            result += "\t-f renbar saved render file.\r\n";
            result += "\t-g return wether gui application exist in local machine,if not ,call it \r\n";
            result += "\t-p return current primary pool list.\r\n";
            result += "\t-sp return current secondary pool list.\r\n";
            result += "\t-pd return current pool detail information.\r\n";
            result += "\t-pm return the pool of machine list.\r\n";
            result += "\t-ps return the pool sharable status.\r\n";
            result += "====================================================\r\n";
            result += "Example: \r\n";
            result += "\tRenbarConsole.exe -c\r\n";
            result += "\tRenbarConsole.exe -f <render file path> e.g. C:\\job.rbr \r\n";
            result += "\tRenbarConsole.exe -g <RenbarGUI.exe> e.g.  C:\\Program Files\\RenbarSystem\\RenbarGUI.exe \r\n";
            result += "\tRenbarConsole.exe -p\r\n";
            result += "\tRenbarConsole.exe -sp\r\n";
            result += "\tRenbarConsole.exe -pd\r\n";
            result += "\tRenbarConsole.exe -pm <pool name>\r\n";
            result += "\tRenbarConsole.exe -ps <pool name>\r\n";

            Console.WriteLine(result);
        }
        /// <summary>
        /// 獲取操作返回值（0 is success flag; otherwise 1.） Get action return code.
        /// </summary>
        /// <param name="Flag">0 is success flag; otherwise 1.</param>
        /// <returns>System.UInt16</returns>
        private static ushort ReturnCode(bool Flag)
        {
            if (!Flag)
                return 1;
            else
                return 0;
        }
        #endregion

        #region 分析並發送文件 Analysis Document And Send Document

        /// <summary>
        /// 發送任務
        /// </summary>
        /// <param name="File">任務檔案</param>
        /// <returns>成功/失敗</returns>
        private static bool SendDocument(FileInfo File)
        {
            // declare result flag ..
            bool result = true;
            // create renbar file system class object ..
            FileSystem Filesys = new FileSystem();
            // declare request object ..
            IDictionary<string, object> renderObjects = new Dictionary<string, object>();
            // declare response object ...
            KeyValuePair<string, object> responseObject;

            try
            {
                Console.WriteLine("Analysis the file……\r\n");
                // 文件轉換（Rbr、Console=>轉為XML） parse file ...
                if (Filesys.Convert(FileSystem.RenderFileType.Rbr, FileSystem.RenderMethod.Console, File.FullName, ref renderObjects))
                {
                    #region 轉換成功
                    Console.WriteLine("\t"+ReturnCode(true));
                    Console.WriteLine("\t Analysis successed! ");
                    Console.WriteLine("=============================================================\r\n");
                    Console.WriteLine("Checking the file……\r\n");
                    // inspection waitfor principles ..
                    if (CheckPrinciples(renderObjects))
                    {
                        Console.WriteLine("\t" + ReturnCode(true));
                        Console.WriteLine("\t the file is in a right formatter! ");
                        Console.WriteLine("=============================================================\r\n");

                        #region 信息完整，則發送文件

                        int sendcount = 0;

                        Console.WriteLine("Sending this file……\r\n");
                        Console.WriteLine("\t JobCount:" + renderObjects.Count.ToString());

                        #region send request object ……
                        // send request object ..
                        foreach (KeyValuePair<string, object> kv in renderObjects)
                        {
                            //Console.WriteLine("\t JobGroupName:" + kv.Key.ToString());
                            //Console.WriteLine("\t JobAttr:" + kv.Value.ToString() + "\r\n");
                            do
                            {
                                // confirm current request status ...
                                if (EnvComm.CanRequest)
                                {
                                    // package sent data ...
                                    IList<object> packaged = EnvComm.Package(Client2Server.CommunicationType.JOBQUEUEADD, Mapping((IDictionary<string, object>)kv.Value));
                                    // wait for result ...
                                    responseObject = EnvComm.Request(packaged);
                                    break;
                                }
                            } while (true);

                            //记录成功发送任务数
                            if (responseObject.Key.Trim().Substring(0, 1) == "+")
                            {
                                sendcount++;

                                IList<string> JobId = (List<string>)responseObject.Value;

                                Console.WriteLine("\t {0} is sended succeed!,JobGroupID is {1}", kv.Key.ToString(), JobId[sendcount - 1]);

                            }
                        }

                        #endregion

                        Console.WriteLine("\t"+ReturnCode(true));
                        // output message ..
                        Console.WriteLine(string.Format("\t Has successful submited {0} jobs.\r\n", sendcount));
                        Console.WriteLine("=============================================================\r\n");
                        #endregion
                    }
                    else
                    {
                        #region 信息不完整，則輸出錯誤信息
                        Console.WriteLine("\t"+ReturnCode(false));
                        // output error message ..
                        string ExceptionMsg = "this render file has happen waitfor attribute principles error, please check it !";
                        Console.WriteLine("\t Error: {0}", ExceptionMsg);
                        Console.WriteLine("=============================================================\r\n");
                        // write to log file ..
                        // EnvLog.Writer(AssemblyInfoClass.ProductInfo, LogBase.Level.Error, ExceptionMsg);
                        EnvLog.Writer(AssemblyInfoClass.ProductInfo, Log.Level.Error, ExceptionMsg, false);
                        return false;
                        #endregion
                    }
                    #endregion
                }
                else
                {
                    #region 轉換不成功
                    Console.WriteLine("\t"+ReturnCode(false));
                    // output error message ..
                    string ExceptionMsg = "this render file lost one or more attribute, please check it !";
                    Console.WriteLine("\t Error: {0}", ExceptionMsg);
                    Console.WriteLine("=============================================================\r\n");
                    // write to log file .. 
                    // EnvLog.Writer(AssemblyInfoClass.ProductInfo, LogBase.Level.Error, ExceptionMsg);
                    EnvLog.Writer(AssemblyInfoClass.ProductInfo, Log.Level.Error, ExceptionMsg, false);
                    return false;
                    #endregion
                }
            }
            catch (Exception ex)
            {
                result = false;
                Console.WriteLine("\t"+ReturnCode(false));
                // output error message ..
                string ExceptionMsg = ex.Message + ex.StackTrace;
                Console.WriteLine("Can't submit this render file ,An error happens: {0}\r\n", ExceptionMsg);
                Console.WriteLine("Please contact IT dept !");
                Console.WriteLine("=============================================================\r\n");
                // write to log file ..
                // EnvLog.Writer(AssemblyInfoClass.ProductInfo, LogBase.Level.Error, ExceptionMsg);
                EnvLog.Writer(AssemblyInfoClass.ProductInfo, Log.Level.Error, ExceptionMsg, false);
            }
            return result;
        }

        /// <summary>
        /// 映射數據庫字段 Mapping database columns.
        /// </summary>
        /// <param name="DataList">mapping data item list.</param>
        /// <returns>System.Collection.Generic</returns>
        private static IDictionary<string, object> Mapping(IDictionary<string, object> DataList)
        {
            // declare temporary job object collection ...
            IDictionary<string, object> _DataList = new Dictionary<string, object>();

            // create mapping pool datatable ..
            DataTable PoolData = Pool;

            // declare filter pool data string ..
            string poolexpr = string.Format("Name = '{0}'", "Null");

            // mapping ..
            foreach (KeyValuePair<string, object> kv in DataList)
            {
                switch (kv.Key)
                {
                    #region Base Data Structure
                    case "name":
                        if (kv.Value == null || kv.Value.ToString() == string.Empty)
                            _DataList.Add("Name", null);
                        else
                            _DataList.Add("Name", kv.Value);
                        break;
                    case "waitFor":
                        if (kv.Value == null || kv.Value.ToString() == string.Empty)
                            _DataList.Add("WaitFor", null);
                        else
                            _DataList.Add("WaitFor", kv.Value);
                        break;
                    case "isUpdateOnly":
                        if (kv.Value == null || kv.Value.ToString() == string.Empty)
                            _DataList.Add("ABUpdateOnly", null);
                        else
                            _DataList.Add("ABUpdateOnly", kv.Value);
                        break;
                    case "type":
                        if (kv.Value == null || kv.Value.ToString() == string.Empty)
                            _DataList.Add("Proc_Type", null);
                        else
                            _DataList.Add("Proc_Type", kv.Value);
                        break;
                    case "ABProjectName":
                        if (kv.Value == null || kv.Value.ToString() == string.Empty)
                            _DataList.Add("ABName", null);
                        else
                            _DataList.Add("ABName", kv.Value);
                        break;
                    case "ABNode":
                        if (kv.Value == null || kv.Value.ToString() == string.Empty)
                            _DataList.Add("ABPath", null);
                        else
                            _DataList.Add("ABPath", kv.Value);
                        break;
                    case "ProjectName":
                        if (kv.Value == null || kv.Value.ToString() == string.Empty)
                            _DataList.Add("Project", null);
                        else
                            _DataList.Add("Project", kv.Value);
                        break;
                    case "FirstPool":
                        if (kv.Value == null || kv.Value.ToString() == string.Empty)
                            _DataList.Add("First_Pool", null);
                        else
                        {
                            DataRow[] p = PoolData.Select(poolexpr.Replace("Null", kv.Value.ToString()));

                            if (p.Length > 0)
                                _DataList.Add("First_Pool", p[0]["Pool_Id"]);
                            else
                                _DataList.Add("First_Pool", null);
                        }
                        break;
                    case "SecondPool":
                        if (kv.Value == null || kv.Value.ToString() == string.Empty)
                            _DataList.Add("Second_Pool", null);
                        else
                        {
                            DataRow[] p = PoolData.Select(poolexpr.Replace("Null", kv.Value.ToString()));

                            if (p.Length > 0)
                                _DataList.Add("Second_Pool", p[0]["Pool_Id"]);
                            else
                                _DataList.Add("Second_Pool", null);
                        }
                        break;
                    case "StartFrame":
                        if (kv.Value == null || kv.Value.ToString() == string.Empty)
                            _DataList.Add("Start", null);
                        else
                            _DataList.Add("Start", kv.Value);
                        break;
                    case "EndFrame":
                        if (kv.Value == null || kv.Value.ToString() == string.Empty)
                            _DataList.Add("End", null);
                        else
                            _DataList.Add("End", kv.Value);
                        break;
                    case "PacketSize":
                        if (kv.Value == null || kv.Value.ToString() == string.Empty)
                            _DataList.Add("Packet_Size", null);
                        else
                            _DataList.Add("Packet_Size", kv.Value);
                        break;
                    default:
                        if (kv.Value == null || kv.Value.ToString() == string.Empty)
                            _DataList.Add(kv.Key, null);
                        else
                            _DataList.Add(kv.Key, kv.Value);
                        break;
                    #endregion
                }
            }

            // add other job data attribute ..
            if (DataList.Count > 0)
            {
                _DataList.Add("Submit_Machine", new RenbarLib.Network.HostBase().LocalHostName);
                _DataList.Add("Submit_Acct", Environment.UserName);
                _DataList.Add("Submit_Time", DateTime.Now);
                _DataList.Add("Status", (UInt16)JobStatusFlag.QUEUING);
            }

            return _DataList;
        }

        /// <summary>
        /// 獲取分組信息（VIEWPOOLINFO） Get single type pool data list.
        /// </summary>
        private static DataTable Pool
        {
            get
            {
                DataTable result = new DataTable();

                // declare request dictionary object ..
                IDictionary<string, object> requestObject = new Dictionary<string, object>();

                // declare remote server response object ..
                KeyValuePair<string, object> responseObject;

                do
                {
                    // confirm current request status ..
                    if (EnvComm.CanRequest)
                    {
                        // package sent data ..
                        IList<object> packaged = EnvComm.Package(Client2Server.CommunicationType.VIEWPOOLINFO, requestObject);

                        // wait for result ..
                        responseObject = EnvComm.Request(packaged);
                        break;
                    }
                } while (true);

                // wait for server reponse ..
                if (responseObject.Value != null)
                    result = (DataTable)responseObject.Value;

                return result;
            }
        }

        /// <summary>
        /// 設定WaitFor Check waitfor principles.
        /// </summary>
        /// <param name="DataList">inspection data item list.</param>
        /// <returns>System.Boolean</returns>
        private static bool CheckPrinciples(IDictionary<string, object> DataList)
        {
            // declare result flag ..
            bool result = true;

            IList<string> PrimaryJobs = new List<string>();
            IList<string> WaitForJobs = new List<string>();
            IList<string> CheckedJobs = new List<string>();

            if (DataList.Count > 0)
            {
                int
                    NotWaitting = 0, JobsCount = 0;

                // find all waitfor attribute ..
                foreach (KeyValuePair<string, object> singleItems in DataList)
                {
                    IDictionary<string, object> Dictionarys = (IDictionary<string, object>)singleItems.Value;

                    if (Dictionarys["name"] != null && Dictionarys["name"].ToString() != string.Empty &&
                        Dictionarys["waitFor"] != null && Dictionarys["waitFor"].ToString() != string.Empty)
                    {
                        PrimaryJobs.Add(Dictionarys["name"].ToString());
                        WaitForJobs.Add(Dictionarys["waitFor"].ToString());
                    }

                    if (Dictionarys["waitFor"] == null || Dictionarys["waitFor"].ToString() == string.Empty)
                    {
                        NotWaitting++;
                    }

                    JobsCount++;
                }

                //所有任務都不是“WaitFor”
                if (NotWaitting.Equals(JobsCount))
                    return true;

                // search intersection for the primary and waitfor list ..
                foreach (string index in PrimaryJobs.Intersect(WaitForJobs))
                    CheckedJobs.Add(index);

                // remove repeat of index ..
                IEnumerable<string> WaitForOrg = WaitForJobs.Distinct();

                // sort waitfor list ..
                IOrderedEnumerable<string> WaitForOrder = WaitForOrg.OrderBy(idx => idx);

                if (CheckedJobs.SequenceEqual(WaitForOrder))
                    result = false;
                else
                {
                    foreach (string index in WaitForOrder)
                    {
                        if (!DataList.ContainsKey(index))
                            result = false;
                    }
                }
            }

            return result;
        }
        #endregion

        #region  定義組別、獲取機器分組信息 Get Pool Information
        /// <summary>
        /// 組別 Retuen Pool Types Enumeration.
        /// </summary>
        private enum PoolType
        {
            /// <summary>
            /// Return Detail information.
            /// </summary>
            Detail,
            /// <summary>
            /// Return primary pool list.
            /// </summary>
            Primary_Pool,
            /// <summary>
            /// Return secondary pool list.
            /// </summary>
            Secondary_Pool,
            /// <summary>
            /// Return sharable status.
            /// </summary>
            Sharable,
            /// <summary>
            /// Return Machine list.
            /// </summary>
            Machine,
        }

        /// <summary>
        /// 獲得分組的機器信息 Get related machine list.
        /// </summary>
        /// <param name="Gid">pool gid.機器群組標識碼</param>
        /// <param name="MachineData">inquire machine data.所有的機器</param>
        /// <param name="RelateData">pool machine related data.機器分組信息</param>
        /// <returns>System.Collections.Generic.IList</returns>
        private static IList<string> GetMachineList(string Gid, DataTable MachineData, DataTable RelateData)
        {
            // declare return object ..
            IList<string> result = new List<string>();
            IList<string> relatedlist = new List<string>();

            // find related machine ..
            foreach (DataRow row in RelateData.Rows)
            {
                if (row["Pool_Id"].ToString() == Gid)
                {
                    if (!relatedlist.Contains(row["Machine_Id"].ToString()))
                    {
                        relatedlist.Add(row["Machine_Id"].ToString());
                    }
                }
            }

            // get machine name .
            foreach (DataRow row in MachineData.Rows)
            {
                foreach (string s in relatedlist)
                {
                    if (row["Machine_Id"].ToString() == s)
                    {
                        if (!result.Contains(row["Name"].ToString()))
                            result.Add(row["Name"].ToString());
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// 獲取當前所有分組信息 Return current all pool information.
        /// </summary>
        /// <param name="ParaName">parametrer name.</param>
        /// <param name="ReturnType">executable behaivor.</param>
        /// <returns>System.String</returns>
        private static string GetPoolInfo(string ParaName, PoolType ReturnType)
        {
            string result = null;

            DataSet Ds = new DataSet("PoolOrgInfo");

            // declare request dictionary object ..
            IDictionary<string, object> requestObject = new Dictionary<string, object>();

            // declare remote server response object ..
            KeyValuePair<string, object> responseObject = new KeyValuePair<string, object>();

            // create get resource behaivor array ..
            Client2Server.CommunicationType[] BehaivorAry = new Client2Server.CommunicationType[]
            {
                Client2Server.CommunicationType.VIEWMACHINEINFO,
                Client2Server.CommunicationType.VIEWPOOLINFO,
                Client2Server.CommunicationType.VIEWMACHINEPOOLRELATION
            };

            // get all contents of the data sheet ..
            for (int i = 0; i < BehaivorAry.Length; i++)
            {
                do
                {
                    // confirm current request status ..
                    if (EnvComm.CanRequest)
                    {
                        // package sent data ..
                        IList<object> packaged = EnvComm.Package(BehaivorAry[i], requestObject);
                        // wait for result ..
                        responseObject = EnvComm.Request(packaged);
                        break;
                    }
                } while (true);

                // wait for server reponse ..
                if (responseObject.Value != null)
                {
                    Ds.Tables.Add((DataTable)responseObject.Value);//？？？不在While里面？？一直Add？？？？？？？？？？？
                }
                else
                {
                    Console.WriteLine("\t"+ReturnCode(false));
                    // output error message ..
                    string ExceptionMsg = "\t can't access remote resource, please contact IT dept !";
                    Console.WriteLine("\t Error: {0}", ExceptionMsg);
                    Console.WriteLine("=============================================================\r\n");
                    return null;
                }
            }

            DataTable info = new DataTable("Pools");

            // define data column structure ..
            info.Columns.Add(new DataColumn("pname", typeof(string)));
            info.Columns.Add(new DataColumn("share", typeof(string)));
            info.Columns.Add(new DataColumn("mname", typeof(List<string>)));

            // define primary keys ..
            DataColumn[] keys = new DataColumn[1] { info.Columns["pname"] };
            info.PrimaryKey = keys;

            // assign to customize data table ..
            foreach (DataRow row in Ds.Tables["Pool"].Rows)
            {
                // get machine list ...
                IList<string> machines = GetMachineList(row["Pool_Id"].ToString(), Ds.Tables["Machine"], Ds.Tables["Machine_Pool"]);

                // add to customize data table ...
                DataRow newRow = info.NewRow();
                newRow[0] = row["Name"];
                newRow[1] = row["Sharable"];
                newRow[2] = machines;
                info.Rows.Add(newRow);
            }

            switch (ReturnType)
            {
                case PoolType.Detail:
                    // print format ...
                    result = "\tPool Name\tSharable\tMachine Name\r\n";
                    result += "-----------------------------------------------------------------\r\n";
                    foreach (DataRow row in info.Rows)
                    {
                        result += "\r\t" + row["pname"].ToString() + "\t\t" + row["share"];
                        foreach (string s in ((List<string>)row["mname"]))
                        {
                            result += "\t\t" + s + "\r\n\t\t\t";
                        }
                        result += "\r------------------------------------------------------------\r\n";
                    }
                    break;

                case PoolType.Primary_Pool:
                    // print format ..
                    DataRow[] p_rows = info.Select();

                    if (p_rows.Length > 0)
                    {
                        foreach (DataRow row in p_rows)
                        {
                            result += string.Format("{0},\r\n", row["pname"].ToString());
                        }
                        result = result.Substring(0, (result.Length - 1)) + "\r\n";
                    }
                    break;

                case PoolType.Secondary_Pool:
                    // print format ...
                    DataRow[] s_rows = info.Select("share = 1");

                    if (s_rows.Length > 0)
                    {
                        foreach (DataRow row in s_rows)
                        {
                            result += string.Format("{0},\r\n", row["pname"].ToString());
                        }
                        result = result.Substring(0, (result.Length - 1)) + "\r\n";
                    }
                    break;

                case PoolType.Machine:
                    if (!string.IsNullOrEmpty(ParaName))
                    {
                        // print format ...
                        if (info.Rows.Contains(ParaName))
                        {
                            int idx = info.Rows.IndexOf(info.Rows.Find(ParaName));
                            foreach (string s in (List<string>)info.Rows[idx]["mname"])
                            {
                                result += string.Format("{0},\r\n", s);
                            }
                            result = result.Substring(0, (result.Length - 1)) + "\r\n";
                        }
                        else
                            Console.WriteLine("can't find the pool of machine list !\r\n");
                    }
                    break;

                case PoolType.Sharable:
                    if (!string.IsNullOrEmpty(ParaName))
                    {
                        if (info.Rows.Contains(ParaName))
                        {
                            int idx = info.Rows.IndexOf(info.Rows.Find(ParaName));
                            result = info.Rows[idx]["share"].ToString() + "\r\n";
                        }
                    }
                    break;
            }

            return result;
        }
        #endregion
    }
}