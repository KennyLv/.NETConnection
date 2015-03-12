using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Remoting;

namespace RemoteServer
{
    class MyServer
    {
        [STAThread]
        static void Main(string[] args)
        {
            RemotingConfiguration.Configure("RemoteServer.exe.config", false);
            Console.WriteLine("Server is OK! \r\n Press 'Enter' to quit");
            Console.ReadLine();
        }
    }
}
