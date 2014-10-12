using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Xml.Serialization;
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
        private Dictionary<Type, Type> _types = new Dictionary<Type, Type>();

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
                            try
                            {
                                formatter.Serialize(mStream, responce ?? new DataResponce(true, "Error serialization responce"));
                            }
                            catch (Exception ex)
                            {
                                formatter.Serialize(mStream, new DataResponce(true, ex));
                            }

                            client.GetStream().Write(mStream.ToArray(), 0, (int)mStream.Length);

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

                            if (f == null)
                                return new DataResponce(true, string.Format("Field {0} not exist", request.MemberName));

                            if (_types.ContainsKey(f.FieldType))
                            {
                                return Populate(f.FieldType);
                            }
                            return (f.IsStatic) ? f.GetValue(null) : new DataResponce(true, "Cannot load field value");
                        case Commands.GetProperty:
                            var p = t.GetProperty(request.MemberName);
                            if (p != null && p.GetGetMethod().IsStatic)
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
            lock (o)
            {
                _types.Clear();

                var assemblies = AppDomain.CurrentDomain.GetAssemblies();

                try
                {
                    var mTypes = assemblies.Where(x => x.FullName.Contains("UnityEngine") || x.FullName.Contains("Assembly-CSharp"))
                              .SelectMany(x => x.GetTypes()).ToList();

                    foreach (var mType in mTypes)
                    {
                        if (string.IsNullOrEmpty(mType.Name) || mType.Name.Contains('+')) continue;

                        var objtype = GetKspType(string.Format("{0}_Wrapper", mType.Name));

                        //if (objtype == null) continue;

                        //_types.Add(mType, objtype);
                    }
                }
                catch (Exception ex)
                {
                    return new DataResponce(true, ex);
                }


                return assemblies.Where(x => x.FullName.Contains("UnityEngine") || x.FullName.Contains("Assembly-CSharp")).Select(x => new AssemblyWrapper()
                {
                    Location = x.Location,
                    Name = x.FullName,
                    Types = x.GetTypes().Select(y => y.FullName).Where(m => !m.Contains('+') && m.Length > 2).OrderBy(m => m).ToList(),
                }).ToList();
            }
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


        private object Populate(Type t)
        {
            if (t.IsSerializable || !t.IsClass) return null;

            var objType = _types[t];

            var obj = Activator.CreateInstance(objType);

            if (obj == null) return null;

            foreach (var field in t.GetFields(BindingFlags.Public | BindingFlags.Static))
            {
                var objField = obj.GetType().GetField(string.Format("_{0}_wrapped", field.Name));

                if (objField == null) continue;

                if (!field.FieldType.IsSerializable && field.FieldType.IsClass)
                {
                    objField.SetValue(obj, Populate(field.FieldType));

                    continue;
                }

                objField.SetValue(obj, field.GetValue(null));
            }


            return obj;
        }
    }
}
