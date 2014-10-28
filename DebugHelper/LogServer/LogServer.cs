using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using SocketCommon;

namespace DebugHelper.LogServer
{
    public class LogServer
    {
        private TcpListener _logListener;

        public void Start()
        {
            var t = new Thread(ServerStart);
            t.Start();
        }

        private void ServerStart()
        {
            _logListener = new TcpListener(IPAddress.Parse("127.0.0.1"), 11001);
            _logListener.Start();

            var formatter = new BinaryFormatter();

            try
            {
                while (true)
                {
                    var client = _logListener.AcceptTcpClient();

                    var buff = new byte[9000000];

                    client.GetStream().Read(buff, 0, buff.Count());

                    using (var mStream = new MemoryStream(buff))
                    {
                        var data = formatter.Deserialize(mStream) as LogRequest;

                        if (data == null) continue;

                        if (OnRecieved != null)
                        {
                            OnRecieved(this, new RequestEventArgs(data));
                        }
                    }
                }
            }
            catch (Exception e)
            {
                _logListener.Stop();
            }
        }

        public event EventHandler<RequestEventArgs> OnRecieved;

        ~LogServer()
        {
            _logListener.Stop();
        }
    }

    public class RequestEventArgs : EventArgs
    {
        public RequestEventArgs(LogRequest request)
        {
            Request = request;
        }
        
        public LogRequest Request { get; private set; }
    }
}
