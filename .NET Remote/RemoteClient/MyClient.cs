using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

namespace RemoteClient
{
    class MyClient
    {
        [STAThread]
        static void Main(string[] args)
        {
            /////服务端激活
            //RemoteObj.MyObject app = (RemoteObj.MyObject)Activator.GetObject(typeof(RemoteObj.MyObject), ConfigurationSettings.AppSettings["ServiceURL"]);
            //Console.WriteLine(app.Add(1, 2));
            //Console.WriteLine(app.Count());

            //客户端激活
            RemoteObj.MyObject app = (RemoteObj.MyObject)Activator.CreateInstance(typeof(RemoteObj.MyObject), null, new object[] { new System.Runtime.Remoting.Activation.UrlAttribute(System.Configuration.ConfigurationSettings.AppSettings["ServiceURL"]) });

            Console.ReadLine();
        }
    }
}
