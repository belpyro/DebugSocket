using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.RegularExpressions;
using System.Threading;
using SocketCommon;
using SocketCommon.Attributes;
using SocketCommon.Comparers;
using SocketCommon.Wrappers;
using SocketCommon.Wrappers.Tree;
using UnityEngine;
using UnityThreading;

namespace SocketServer
{

    [KSPAddon(KSPAddon.Startup.MainMenu, false)]
    public class Server : MonoBehaviour
    {
        private readonly object o = new object();
        private TcpListener _listener;
        private Dictionary<string, Type> _types = new Dictionary<string, Type>();
        private readonly Dictionary<Type, string> _instantiateTypes = new Dictionary<Type, string>();

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
                                var result = GetKspValue(request);
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

        private object GetKspValue(DataRequest request)
        {
            if (!_types.ContainsKey(request.TypeName)) return null;

            var type = _types[request.TypeName];

            var member = type.GetMember(request.MemberName).FirstOrDefault();

            if (member == null) return null;

            if (member.MemberType == MemberTypes.Field)
            {
                var result = (member as FieldInfo).IsStatic ? (member as FieldInfo).GetValue(null) : null;
                
                if (result != null)
                {
                   return new MemberInfoWrapper()
                   {
                       Name = request.MemberName,
                       TypeName = result.GetType().Name,
                       Type = MemberType.Field,
                       Value = new List<object>()
                       {
                          new TypeInfoWrapper()
                          {
                              
                          } 
                       }
                   }
                }
            }

        }

        private object GetAllTypes()
        {
            lock (o)
            {
                var assemblies = AppDomain.CurrentDomain.GetAssemblies();

                var types =
                    assemblies.Where(x => x.FullName.Contains("Assembly-CSharp"))
                        .SelectMany(x => x.GetTypes())
                        .Where(x => Regex.IsMatch(x.Name, "^[A-Za-z]{3,}$", RegexOptions.IgnoreCase)).OrderBy(x => x.Name).Distinct(new TypeEqualityComparer())
                        .ToList();

                foreach (var type in types)
                {
                    if (_types.ContainsKey(type.Name)) continue;

                    _types.Add(type.Name, type);
                }

                return types.Select(x => new TypeInfoWrapper()
                {
                    Name = x.Name,
                    Members = x.GetMembers().Where(m => m.MemberType == MemberTypes.Field || m.MemberType == MemberTypes.Property).Select(y => new MemberInfoWrapper()
                    {
                        Name = y.Name,
                        TypeName = x.Name,
                        Type = y.MemberType == MemberTypes.Field ? MemberType.Field : MemberType.Property,
                        Value = new List<object>(){new TypeInfoWrapper()}
                    }).OrderBy(n => n.Name).ToList()
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

    }
}
