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
using SocketCommon.Comparers;
using SocketCommon.Wrappers.Tree;
using UnityEngine;

namespace SocketServer
{
    [KSPAddon(KSPAddon.Startup.MainMenu, false)]
    public class Server : MonoBehaviour
    {
        private readonly Dictionary<string, string> _instantiateTypes = new Dictionary<string, string>();
        private readonly Dictionary<string, Type> _types = new Dictionary<string, Type>();
        private readonly object o = new object();
        private TcpListener _listener;

        public void Start()
        {
            _instantiateTypes.Add(typeof(FlightGlobals).Name, "fetch");

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
                                var result = ParseKspValue(request.Info);//GetKspValue(request.Info);
                                if (result != null)
                                {
                                    responce = new DataResponce(false, new MemberInfoWrapper()
                                    {
                                        Name = request.Info.Name,
                                        Type = MemberType.Value,
                                        Value = result.ToString(),
                                        TypeName = result.GetType().Name
                                    });
                                }
                                break;
                            case Commands.GetChildren:
                                object children = GetChildren(request.Info);
                                responce = new DataResponce(false, children);
                                break;
                            case Commands.GetCollection:
                                IEnumerable<MemberInfoWrapper> collection = GetCollection(request.Info);
                                if (collection != null)
                                {
                                   responce = new DataResponce(false, collection); 
                                }
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }

                        using (var mStream = new MemoryStream())
                        {
                            try
                            {
                                formatter.Serialize(mStream, responce ?? new DataResponce(true, "responce is null"));
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

        private IEnumerable<MemberInfoWrapper> GetCollection(MemberInfoWrapper info)
        {
            var result = new List<MemberInfoWrapper>();

            var source = ParseKspValue(info);
            if (source == null) return null;

            if (!source.GetType().GetInterfaces().Contains(typeof(IEnumerable))) return null;

            foreach (var item in (IEnumerable) source)
            {
                if (_types.ContainsKey(item.GetType().Name) && !IsSimpleType(item.GetType()))
                {
                    result.Add(new MemberInfoWrapper()
                    {
                        Name = item.GetType().Name,
                        ParentType = info.TypeName,
                        Type = MemberType.Type
                    }); 
                }
                else
                {
                    result.Add(new MemberInfoWrapper()
                    {
                        Name = item.GetType().Name,
                        ParentType = info.TypeName,
                        Type = MemberType.Property
                    }); 
                } 
            }

            return result;
        }

        private object GetChildren(MemberInfoWrapper info)
        {
            if (!_types.ContainsKey(info.TypeName) && !GetKspType(info.ParentType))
                return new DataResponce(true, string.Format("Types does not contain type {0}", info.TypeName));

            //if (ParseKspValue(info) == null)
            //{
            //    return null;
            //}

            var children = new List<MemberInfoWrapper>();

            var type = _types[info.TypeName];

            var fields = type.GetFields().Select(x => new MemberInfoWrapper()
            {
                Name = x.Name,
                Type = (x.FieldType.IsClass || x.FieldType.IsValueType) && !IsSimpleType(x.FieldType) ? x.FieldType.GetInterfaces().Contains(typeof(IEnumerable)) ? MemberType.Collection : MemberType.Type : MemberType.Field,
                TypeName = x.FieldType.Name,
                ParentType = type.Name,
                IsStatic = x.IsStatic
            }).ToList();

            var props = type.GetProperties().Select(x => new MemberInfoWrapper()
            {
                Name = x.Name,
                TypeName = x.PropertyType.Name,
                Type =
                    (x.PropertyType.IsClass || x.PropertyType.IsValueType) && !IsSimpleType(x.PropertyType) ? x.PropertyType.GetInterfaces().Contains(typeof(IEnumerable)) ? MemberType.Collection : MemberType.Type : MemberType.Property,
                ParentType = type.Name,
                IsStatic = x.GetGetMethod().IsStatic
            }).ToList();

            children.AddRange(fields);
            children.AddRange(props);

            return children.OrderBy(x => x.Name).ToList();
        }

        private bool IsSimpleType(Type t)
        {
            return t.IsPrimitive || t == typeof(string) ||
                   t == typeof(Guid) || t.IsEnum || t == typeof(Vector3) || t == typeof(Vector3d) || t == typeof(Quaternion) ||
                   t == typeof(Vector2) || t == typeof(Vector2d);

        }

        private object ParseKspValue(MemberInfoWrapper data)
        {
            var items = new List<MemberInfoWrapper>();

            MemberInfoWrapper item = data;

            while (item != null)
            {
                items.Add(item);
                item = item.Parent;
            }

            items.Reverse();

            return GetKspValue(items);
        }

        private object GetKspValue(List<MemberInfoWrapper> wrappers, object instance = null, Type parent = null)
        {
            if (wrappers == null || !wrappers.Any()) return null;

            var first = wrappers.FirstOrDefault();

            Debug.Log("First item " + first.Name);

            wrappers = wrappers.Skip(1).ToList();

            try
            {
                if (parent == null)
                {
                    if (!_types.ContainsKey(first.TypeName)) return null;

                    var type = _types[first.TypeName];

                    Debug.Log("Get type " + type.Name);

                    if (_instantiateTypes.ContainsKey(type.Name))
                    {
                        var instanceField = type.GetField(_instantiateTypes[type.Name]);

                        if (instanceField == null)
                        {
                            var instanceProp = type.GetProperty(_instantiateTypes[type.Name]);

                            if (instanceProp == null) return null;

                            return GetKspValue(wrappers, instanceProp.GetValue(null, null), type);
                        }

                        var dataInstance = instanceField.GetValue(null);

                        return GetKspValue(wrappers, dataInstance, type);
                    }

                    return GetKspValue(wrappers, null, type);
                }

                var field = parent.GetField(first.Name);

                if (field != null)
                {
                    Debug.Log("Get field " + field.Name);

                    var value = field.IsStatic ? field.GetValue(null) : instance != null ? field.GetValue(instance) : null;

                    Debug.Log("Get value " + value);

                    return wrappers.Any() ? GetKspValue(wrappers, value, field.FieldType) : value;
                }

                var prop = parent.GetProperty(first.Name);

                if (prop != null)
                {
                    Debug.Log("Get prop " + prop.Name);

                    var value = prop.GetGetMethod().IsStatic ? prop.GetValue(null, null) : instance != null ? prop.GetValue(instance, null) : null;

                    Debug.Log("Get value " + value);

                    return wrappers.Any() ? GetKspValue(wrappers, value, prop.PropertyType) : value;
                }

                return null;
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
                return null;
            }
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
                        .SelectMany(x => x.GetTypes()).Where(x => !x.IsInterface && !x.IsAbstract)
                        .Where(x => Regex.IsMatch(x.Name, "^[A-Za-z0-9]{3,}$", RegexOptions.IgnoreCase))
                        .OrderBy(x => x.Name)
                        .Distinct(new TypeEqualityComparer())
                        .ToList();

                List<Type> unityTypes =
                    assemblies.Where(x => x.FullName.Contains("UnityEngine"))
                        .SelectMany(x => x.GetTypes())
                        .Where(x => Regex.IsMatch(x.Name, "^[A-Za-z0-9]{3,}$", RegexOptions.IgnoreCase))
                        .OrderBy(x => x.Name)
                        .Distinct(new TypeEqualityComparer())
                        .ToList();

                foreach (var type in types)
                {
                    if (_types.ContainsKey(type.Name)) continue;

                    _types.Add(type.Name, type);
                }

                foreach (var type in unityTypes)
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