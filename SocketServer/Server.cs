using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.InteropServices;
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
                                    object types = GetAllTypes();
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
                            case Commands.Set:
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

        private object GetKspValue(MemberInfoWrapper info)
        {
            if (!_types.ContainsKey(info.TypeName))
                return new DataResponce(true, string.Format("Types does not contain type {0}", info.TypeName));

            var type = _types[info.TypeName];

            var field = type.GetField(info.Name);

            if (field == null) return new DataResponce(true, "field not found");

            if (field.IsStatic)
            {
                return new MemberInfoWrapper()
                {
                    Name = field.Name,
                    TypeName = field.FieldType.Name,
                    Value = field.GetValue(null)
                };
            }

            return new DataResponce(true, "object bot found");
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
                    Children =
                        x.GetMembers()
                            .Where(m => m.MemberType == MemberTypes.Field || m.MemberType == MemberTypes.Property)
                            .Select(y => new MemberInfoWrapper
                            {
                                Name = y.Name,
                                TypeName = x.Name,
                                Children = new List<MemberInfoWrapper> { new MemberInfoWrapper() }
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

        public void OnDestroy()
        {
            if (_listener != null)
            {
                _listener.Stop();
            }
        }
    }
}