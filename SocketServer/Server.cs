using System;
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
using SocketCommon.Comparers;
using SocketCommon.Wrappers.Tree;
using UnityEngine;

namespace SocketServer
{
    [KSPAddon(KSPAddon.Startup.MainMenu, false)]
    public class Server : MonoBehaviour
    {
        private readonly Dictionary<Type, string> _instantiateTypes = new Dictionary<Type, string>();
        private readonly Dictionary<string, Type> _types = new Dictionary<string, Type>();
        private readonly object o = new object();
        private TcpListener _listener;

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
                    using (TcpClient client = _listener.AcceptTcpClient())
                    {
                        var buff = new byte[9600];

                        client.GetStream().Read(buff, 0, buff.Length);

                        DataRequest request = GetDataRequest(buff);

                        if (request == null) continue;

                        DataResponce responce = null;

                        switch (request.Command)
                        {
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
                            case Commands.GetValue:
                                object result = GetKspValue(request.Info);
                                responce = new DataResponce(false, result);
                                break;
                            case Commands.GetChildren:
                                object children = GetChildren(request.Info);
                                responce = new DataResponce(false, children);
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }

                        using (var mStream = new MemoryStream())
                        {
                            try
                            {
                                formatter.Serialize(mStream,
                                    responce ?? new DataResponce(true, "Error serialization responce"));
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

        private object GetChildren(MemberInfoWrapper info)
        {
            if (!_types.ContainsKey(info.TypeName) && !GetKspType(info.ParentType))
                return new DataResponce(true, string.Format("Types does not contain type {0}", info.TypeName));

            var children = new List<MemberInfoWrapper>();

            var type = _types[info.TypeName];

            var fields = type.GetFields().Select(x => new MemberInfoWrapper()
            {
                Name = x.Name,
                Type = (x.FieldType.IsClass || x.FieldType.IsValueType) && x.FieldType != typeof(string) && !x.FieldType.IsPrimitive ? MemberType.Type : MemberType.Field,
                TypeName = x.FieldType.Name, 
                ParentType = type.Name, IsStatic = x.IsStatic
            }).ToList();

            var props = type.GetProperties().Select(x => new MemberInfoWrapper()
            {
                Name = x.Name,
                TypeName = x.PropertyType.Name,
                Type =
                    (x.PropertyType.IsClass || x.PropertyType.IsValueType )&& x.PropertyType != typeof(string) && !x.PropertyType.IsPrimitive ? MemberType.Type : MemberType.Property,
                ParentType = type.Name, IsStatic = x.GetGetMethod().IsStatic
            }).ToList();

            children.AddRange(fields);
            children.AddRange(props);

            return children.OrderBy(x => x.Name).ToList();
        }

        private object GetKspValue(MemberInfoWrapper info)
        {
            if (!_types.ContainsKey(info.ParentType))
                return new DataResponce(true, string.Format("Types does not contain type {0}", info.TypeName));

            var type = _types[info.ParentType];

            switch (info.Type)
            {
                case MemberType.Field:
                    var field = type.GetField(info.Name);

                    if (field == null) return new DataResponce(true, "field not found");

                    if (field.IsStatic)
                    {
                        return new MemberInfoWrapper()
                        {
                            Name = field.Name,
                            TypeName = field.FieldType.Name,
                            Value = field.GetValue(null),
                            Type = MemberType.Value
                        };
                    }

                    break;
                case MemberType.Property:
                    break;
                case MemberType.Type:
                    break;
                case MemberType.Value:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }



            return new DataResponce(true, "object not found");
        }


        private bool GetKspType(string typeName)
        {
            if (string.IsNullOrEmpty(typeName))
            {
                return false;
            }

            if (_types.ContainsKey(typeName)) return true;
            
            var t = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(x => x.GetTypes())
                .FirstOrDefault(y => y.Name.Equals(typeName));

            if (t == null) return false;

            _types.Add(t.Name, t);

            return true;
        }


        private object GetAllTypes()
        {
            lock (o)
            {
                Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

                List<Type> types =
                    assemblies.Where(x => x.FullName.Contains("Assembly-CSharp"))
                        .SelectMany(x => x.GetTypes())
                        .Where(x => Regex.IsMatch(x.Name, "^[A-Za-z]{3,}$", RegexOptions.IgnoreCase))
                        .OrderBy(x => x.Name)
                        .Distinct(new TypeEqualityComparer())
                        .ToList();

                foreach (var type in types)
                {
                    if (_types.ContainsKey(type.Name)) continue;

                    _types.Add(type.Name, type);
                }

                return types.Select(x => new MemberInfoWrapper
                {
                    Name = x.Name,
                    TypeName = x.Name,
                    Type = MemberType.Type,
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

        public void OnDestroy()
        {
            if (_listener != null)
            {
                _listener.Stop();
            }
        }
    }
}