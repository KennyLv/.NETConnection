#region Using NameSpace
using System;
using System.Collections.Generic;
using System.Threading;
using System.Net;
// import renbar common library namespace ..
using RenbarLib.Network;
using RenbarLib.Network.Protocol;
using RenbarLib.Network.Sockets;
using System.Text;
#endregion

namespace RenbarLib.Environment.Forms.Customizations.Service
{
    /// <summary>
    /// Renbar client server communication class.
    /// </summary>
    public class Communication:Log
    {
        #region Declare Global Variable Section
        // create transport protocol object ..
        private Client2Server ClientObject = new Client2Server();

        // create renbar environment object ..
        private global::RenbarLib.Environment.Service EnvSvr = new RenbarLib.Environment.Service();

        // declare client request connect socket ..
        private TcpClientSocket ClientRequest = null;

        // declare communication status flag ..
        private static bool running = false;
        #endregion

        #region 建立/終止連接Connect And Disconnect Event Procedure
        /// <summary>
        /// 建立Tcp鏈接 Start and connect remote service.
        /// </summary>
        /// <param name="IpAddr">connect server ip address.</param>
        /// <param name="Port">entry prot number.</param>
        /// <returns>System.Boolean</returns>
        public bool Connect(IPAddress IpAddr, ushort Port)
        {
            // create scan server port class object ..
            ScanPort Ping = new ScanPort();
            // ping renbar server can connect ..
            if (Ping.Scan(IpAddr, Port))
            {
                // connecting renbar center server ..
                ClientRequest = new TcpClientSocket(IpAddr, Port);
            }
            else
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 終止連接 Disconnect remote service.
        /// </summary>
        public void Disconnect()
        {
            if (ClientRequest != null)
            {
                // close communication object ..
                ClientRequest.Close();
            }
        }
        #endregion

        #region 獲取當前請求狀態Get Current Request Status Property
        /// <summary>
        /// get current communication status.
        /// </summary>
        public bool CanRequest
        {
            get
            {
                if (!running)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        #endregion

        #region 打包通信物件 Package Client-Server Communication Object Procedure
        /// <summary>
        /// Package communication data type.
        /// </summary>
        /// <param name="Type">sent to communication type of remote service.</param>
        /// <param name="Data">sent to data of remote service.</param>
        /// <returns>System.Collection.Generic.IList</returns>
        public IList<object> Package(Client2Server.CommunicationType Type, IDictionary<string, object> Data)
        {
            IList<object> result = null;

            switch (Type)
            {
                #region Machine Info Cases Workflow
                // add, update machine data ..
                case Client2Server.CommunicationType.MACHINEINFO:
                    // package communication type data ..
                    result = ClientObject.Package(Client2Server.CommunicationType.MACHINEINFO, Data);
                    break;

                // setting enable or disable machine status ..
                case Client2Server.CommunicationType.ONOFFMACHINE:
                    // package communication type data ..
                    result = ClientObject.Package(Client2Server.CommunicationType.ONOFFMACHINE, Data);
                    break;

                // setting machine priority ..
                case Client2Server.CommunicationType.MACHINEPRIORITY:
                    // package communication type data ..
                    result = ClientObject.Package(Client2Server.CommunicationType.MACHINEPRIORITY, Data);
                    break;

                // delete machine data ..
                case Client2Server.CommunicationType.DELETEMACHINEINFO:
                    // package communication type data ..
                    result = ClientObject.Package(Client2Server.CommunicationType.DELETEMACHINEINFO, Data);
                    break;

                // get current machine status information ..
                case Client2Server.CommunicationType.VIEWMACHINEINFO:
                    // package communication type data ..
                    result = ClientObject.Package(Client2Server.CommunicationType.VIEWMACHINEINFO, Data);
                    break;

                // get current machine render status information ..
                case Client2Server.CommunicationType.VIEWMACHINERENDERINFO:
                    // package communication type data ..
                    result = ClientObject.Package(Client2Server.CommunicationType.VIEWMACHINERENDERINFO, Data);
                    break;
                #endregion

                #region Machine Pool Relation Workflow
                // setting machine pool relation ..
                case Client2Server.CommunicationType.MACHINEPOOLRELATION:
                    // package communication type data ..
                    result = ClientObject.Package(Client2Server.CommunicationType.MACHINEPOOLRELATION, Data);
                    break;

                // get currently machine pool relation ..
                case Client2Server.CommunicationType.VIEWMACHINEPOOLRELATION:
                    // package communication type data ..
                    result = ClientObject.Package(Client2Server.CommunicationType.VIEWMACHINEPOOLRELATION, Data);
                    break;
                #endregion

                #region Pool Info Cases Workflow
                // add, update pool data ..
                case Client2Server.CommunicationType.POOLINFO:
                    // package communication type data ..
                    result = ClientObject.Package(Client2Server.CommunicationType.POOLINFO, Data);
                    break;

                // delete pool data ..
                case Client2Server.CommunicationType.DELETEPOOLINFO:
                    // package communication type data ..
                    result = ClientObject.Package(Client2Server.CommunicationType.DELETEPOOLINFO, Data);
                    break;

                // get current pool status information ..
                case Client2Server.CommunicationType.VIEWPOOLINFO:
                    // package communication type data ..
                    result = ClientObject.Package(Client2Server.CommunicationType.VIEWPOOLINFO, Data);
                    break;
                #endregion

                #region Job Info Cases Workflow

                // add, update pool data ..
                case Client2Server.CommunicationType.JOBPRIORITY:
                    // package communication type data ..
                    result = ClientObject.Package(Client2Server.CommunicationType.JOBPRIORITY, Data);
                    break;

                // add job data ..
                case Client2Server.CommunicationType.JOBQUEUEADD:
                    // package communication type data ..
                    result = ClientObject.Package(Client2Server.CommunicationType.JOBQUEUEADD, Data);
                    break;

                // delete job data ..
                case Client2Server.CommunicationType.JOBQUEUEDELETE:
                    // package communication type data ..
                    result = ClientObject.Package(Client2Server.CommunicationType.JOBQUEUEDELETE, Data);
                    break;

                // pause current queue jobs ..
                case Client2Server.CommunicationType.JOBQUEUEPAUSE:
                    // package communication type data ..
                    result = ClientObject.Package(Client2Server.CommunicationType.JOBQUEUEPAUSE, Data);
                    break;

                // repeat job data ..
                case Client2Server.CommunicationType.JOBQUEUEREPEAT:
                    // package communication type data ..
                    result = ClientObject.Package(Client2Server.CommunicationType.JOBQUEUEREPEAT, Data);
                    break;

                // update job data ..
                case Client2Server.CommunicationType.JOBQUEUEUPDATE:
                    // package communication type data ..
                    result = ClientObject.Package(Client2Server.CommunicationType.JOBQUEUEUPDATE, Data);
                    break;

                // get current processing job output ..
                case Client2Server.CommunicationType.VIEWJOBOUTPUT:
                    // package communication type data ..
                    result = ClientObject.Package(Client2Server.CommunicationType.VIEWJOBOUTPUT, Data);
                    break;

                // get current job status information ..
                case Client2Server.CommunicationType.VIEWJOBSTATUS:
                    // package communication type data ..
                    result = ClientObject.Package(Client2Server.CommunicationType.VIEWJOBSTATUS, Data);
                    break;

                // get job info by id
                case Client2Server.CommunicationType.VIEWSINGLEJOBINFO:
                    // package communication type data ..
                    result = ClientObject.Package(Client2Server.CommunicationType.VIEWSINGLEJOBINFO, Data);
                    break;
                // update job data ..
                case Client2Server.CommunicationType.JOBHISTORYRECORD:
                    // package communication type data ..
                    result = ClientObject.Package(Client2Server.CommunicationType.JOBHISTORYRECORD, Data);
                    break;

                #endregion
            }

            return result;
        }
        #endregion

        #region 向遠端服務器發送請求 Sent Request To Remote Service Event Procedure
        /// <summary>
        /// Sent client request object to remote service……
        /// </summary>
        /// <param name="Packaged">packaged data.</param>
        /// <returns>System.Collection.Generic.KeyValuePair</returns>
        public KeyValuePair<string, object> Request(IList<object> Packaged)
        {
            if (Packaged != null && ClientRequest != null && ClientRequest.IsConnected && !running)
            {
                //write the sending/receive data to log
                //ClientRequest.Iswrite = RenbarGUI.Properties.Settings.Default.IsWriteDataLog;

                // change signal status ..
                running = true;

                // send request object to remote server ..
                ClientRequest.Send(this.EnvSvr.Serialize(Packaged));

                // wait for remote response ……
                object response = this.EnvSvr.Deserialize(ClientRequest.Receive());

                if (response != null)
                {
                    // change signal status ..
                    running = false;

                    // convert correct data type ..
                    return (KeyValuePair<string, object>)response;
                }
                else
                {
                    // change signal status ..
                    running = false;

                    return new KeyValuePair<string, object>(string.Format("-Err {0}", DateTime.Now), null);
                }
            }
            else
                return new KeyValuePair<string, object>(string.Format("-Err {0}", DateTime.Now), null);
        }
        #endregion
    }
}