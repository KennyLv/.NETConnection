#region using
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using RenbarLib.Environment;
using RenbarLib.Network;
using RenbarLib.Network.Protocol;
using RenbarLib.Network.Sockets;

using RenbarServerGUI.Properties;
#endregion

namespace RenbarServerGUI
{
    public class SlaveBase:IDisposable
    {

        private HostBase EnvHostBase = new HostBase();
        private Log EnvLog = null;
        bool requestStop = false;

        public SlaveBase(Log LogObj) 
        {
            this.EnvLog = LogObj;
            Thread threadSlave = new Thread(new ThreadStart(Slaving));
            threadSlave.IsBackground = true;
            threadSlave.Priority = ThreadPriority.Normal;
            threadSlave.Start();
        }

        ~SlaveBase() 
        {
            GC.Collect();
        }


        #region IDisposable 成員

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }
        public void Dispose(bool disposing) 
        {
            if (disposing)
            {
                // stop running threads ..
                this.requestStop = true;
            }
        }
        #endregion

        public ushort _listenSlavePort { get; set; }

        private void Slaving()
        {
            TcpServerSocket ClientServiceSocket = null;
            try
            {
                ClientServiceSocket = new TcpServerSocket(this.EnvHostBase.LocalIpAddress, this._listenSlavePort);

                do
                {
                //    ClientServiceSocket.AcceptListen();
                    //if (ClientServiceSocket.Pending())
                    //{
                        
                    //}
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

    }
}
    