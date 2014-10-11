using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using SocketCommon;
using SocketCommon.Wrappers;
using UnityEngine;

namespace SocketServer
{

    [KSPAddon(KSPAddon.Startup.EveryScene, false)]
    public class Server : MonoBehaviour
    {
        private readonly object o = new object();
        private TcpListener _listener = null;

        public void Start()
        {
            var t = new Thread(StartServer);
            t.Start();
        }

        private void StartServer()
        {
            try
            {
                _listener = new TcpListener(IPAddress.Parse("127.0.0.1"), 11000);

                _listener.Start();

                var formatter = new BinaryFormatter();

                while (true)
                {
                    using (var client = _listener.AcceptTcpClient())
                    {
                        var buff = new byte[9600];

                        client.GetStream().Read(buff, 0, buff.Length);

                        var request = GetDataRequest(buff);

                        if (request == null) continue;

                        DataResponce responce = null;

                        switch (request.Command)
                        {
                            case Commands.GetType:
                                try
                                {
                                    var t = GetKspType(request.TypeName);
                                    if (t != null)
                                    {
                                        var data = new TypeWrapper
                                                       {
                                                           Name = t.FullName,
                                                           Fields = t.GetFields().Select(x => new FieldInfoWrapper()
                                                           {
                                                               Data = x,
                                                           }).ToList(),
                                                           Properties = t.GetProperties().Select(x => new PropertyInfoWrapper()
                                                           {
                                                               Data = x,
                                                           }).ToList()
                                                       };
                                        responce = new DataResponce(false, data);
                                    }
                                }
                                catch (Exception e)
                                {
                                    responce = new DataResponce(true, e);
                                }
                                break;
                            case Commands.GetTypes:
                                try
                                {
                                    var types = GetAllTypes();
                                    responce = new DataResponce(false, types);
                                }
                                catch (Exception e)
                                {
                                    responce = new DataResponce(true, e);
                                }
                                break;
                            case Commands.GetField:
                            case Commands.GetProperty:
                                var result = this.GetKspData(request);
                                responce = result != null ? new DataResponce(false, result) : new DataResponce(true, "cannot load ksp type");
                                break;
                            case Commands.Set:
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }

                        using (var mStream = new MemoryStream())
                        {
                            formatter.Serialize(mStream, responce ?? new DataResponce(true, "Error serialization responce"));

                            if (mStream.Length <= 0)
                            {
                                formatter.Serialize(mStream, new DataResponce(true, "Cannot get value"));
                                client.GetStream().Write(mStream.ToArray(), 0, (int)mStream.Length);
                            }
                            else
                            {
                                client.GetStream().Write(mStream.ToArray(), 0, (int)mStream.Length);
                            }

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
                _listener.Stop();
            }
            finally
            {
                if (_listener != null)
                {
                    _listener.Stop();
                }
            }
        }

        private object GetKspData(DataRequest request)
        {
            lock (o)
            {
                try
                {
                    var t = this.GetKspType(request.TypeName);

                    if (t == null) return null;


                    switch (request.Command)
                    {
                        case Commands.GetType:
                            break;
                        case Commands.GetField:
                            var f = t.GetField(request.MemberName);
                            return (f != null && f.IsStatic) ? f.GetValue(null) : new DataResponce(true, "Cannot load field value");
                        case Commands.GetProperty:
                            var p = t.GetProperty(request.MemberName);
                            if (p != null && p.GetGetMethod() == null)
                            {
                                return p.GetValue(null, null);
                            }
                            break;
                        case Commands.GetTypes:
                            break;
                        case Commands.Set:
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    return null;
                }
                catch (Exception ex)
                {

                    return new DataResponce(true, ex);
                }
            }
        }

        private object GetAllTypes()
        {
            return AppDomain.CurrentDomain.GetAssemblies().Select(x => new AssemblyWrapper()
            {
                Location = x.Location,
                Name = x.FullName,
                Types = x.GetTypes().Select(y => y.FullName).OrderBy(m => m).ToList(),
            }).ToList();
        }

        private DataRequest GetDataRequest(byte[] data)
        {
            lock (o)
            {
                var formatter = new BinaryFormatter();
                return formatter.Deserialize(new MemoryStream(data)) as DataRequest;
            }
        }

        private Type GetKspType(string typeName)
        {
            lock (o)
            {
                var type = AppDomain.CurrentDomain.GetAssemblies()
                        .SelectMany(x => x.GetTypes())
                        .FirstOrDefault(
                            x => x.Name.Equals(typeName, StringComparison.InvariantCultureIgnoreCase));
                return type;
            }
        }

        public void OnDestroy()
        {
            if (_listener != null)
            {
                _listener.Stop();
            }
        }
    }
}
