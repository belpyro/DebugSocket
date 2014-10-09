using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using SocketCommon;

namespace SocketClient
{
    public class Client
    {
        public void StartClient()
        {
            var client = new TcpClient(AddressFamily.InterNetwork);

            try
            {
                client.Connect(IPAddress.Parse("127.0.0.1"), 11000);

                var formatter = new BinaryFormatter();
                using (var mStream = new MemoryStream())
                {
                    var data = new DataRequest("FlightGlobals", Commands.GetType);
                    formatter.Serialize(mStream, data);

                    client.GetStream().Write(mStream.ToArray(), 0, (int) mStream.Length);
                }

                var buff = new byte[1024];

                client.GetStream().Read(buff, 0, buff.Length);

                var o = formatter.Deserialize(new MemoryStream(buff)) as DataResponce;

            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                client.Close();
            }
        }
    }
}
