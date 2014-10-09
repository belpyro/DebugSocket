using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DebugHelper
{
    using System.IO;
    using System.Net;
    using System.Net.Sockets;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.Windows.Documents;

    using SocketCommon;

    public class DebugModel
    {
        private BinaryFormatter _formatter = new BinaryFormatter();

        private DebugModel() { }

        private static DebugModel _model;

        public static DebugModel Instance
        {
            get
            {
                return _model ?? (_model = new DebugModel());
            }
        }

        public object Get(Type t)
        {
            return null;
        }

        public DataResponce Get(string name)
        {
            using (var client = new TcpClient(AddressFamily.InterNetwork))
            {
                try
                {
                    client.Connect(IPAddress.Parse("127.0.0.1"), 11000);

                    var request = new DataRequest("FlightGlobals", Commands.GetType);

                    using (var mStream = new MemoryStream())
                    {
                        _formatter.Serialize(mStream, request);

                        //send request
                        client.GetStream().Write(mStream.ToArray(), 0, (int)mStream.Length);

                        //get response
                        mStream.Flush();

                        var buff = new byte[20048];

                        client.GetStream().Read(buff, 0, buff.Length);

                        using (var dsStream = new MemoryStream(buff))
                        {
                            var obj = _formatter.Deserialize(dsStream);
                            return obj as DataResponce;                            
                        }

                    }

                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }

        public DataResponce GetAll()
        {
            using (var client = new TcpClient(AddressFamily.InterNetwork))
            {
                try
                {
                    client.Connect(IPAddress.Parse("127.0.0.1"), 11000);

                    var request = new DataRequest(null, Commands.GetTypes);

                    using (var mStream = new MemoryStream())
                    {
                        _formatter.Serialize(mStream, request);
                        
                        //send request
                        client.GetStream().Write(mStream.ToArray(), 0, (int)mStream.Length);

                        //get response
                        //mStream.Flush();
                        
                        var buff = new byte[560000];
                        
                        client.GetStream().Read(buff, 0, buff.Length);

                        using (var dsStream = new MemoryStream(buff))
                        {
                            var obj = _formatter.Deserialize(dsStream);
                            return obj as DataResponce;
                        }
                    }

                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }
    }
}
