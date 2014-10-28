using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using SocketCommon;

namespace SocketServer
{
    public class LogClient
    {
        private static LogClient _instance;

        private LogClient() { }

        public static LogClient Instance
        {
            get { return _instance ?? (_instance = new LogClient()); }
        }

        public void Send(object o)
        {
            var data = new LogRequest() { Data = o.ToString() };

            var formatter = new BinaryFormatter();

            using (var client = new TcpClient())
            {
                client.Connect(IPAddress.Parse("127.0.0.1"), 11001);

                using (var mStream = new MemoryStream())
                {
                    formatter.Serialize(mStream, data);

                    client.GetStream().Write(mStream.ToArray(), 0, (int)mStream.Length);
                }
            }
        }
    }
}
