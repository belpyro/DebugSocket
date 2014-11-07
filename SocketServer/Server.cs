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
using Fasterflect;
using SocketCommon;
using SocketCommon.Comparers;
using SocketCommon.Events;
using SocketCommon.Helpers;
using SocketCommon.Wrappers.Tree;
using UnityEngine;

namespace SocketServer
{
    [KSPAddon(KSPAddon.Startup.EveryScene, false)]
    public class Server : MonoBehaviour
    {
        private readonly Dictionary<string, string> _instantiateTypes = new Dictionary<string, string>();
        private readonly Dictionary<string, Type> _types = new Dictionary<string, Type>();
        private readonly object _o = new object();
        private TcpListener _listener;

        private const string ASSEMBLY_FILE_NAME = "assemblies.txt";
        private const string INSTANTIATE_FILE_NAME = "instances.txt";

        private Part _externalPart;

        public void Start()
        {
            ConfigureInstantiateAssemblies();

            ServerEvents.OnAttachToServer.Add(ServerAttached);

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

                        LogClient.Instance.Send(string.Format("Command {0} recieved", request.Command));

                        switch (request.Command)
                        {
                            case Commands.CheckConnection:
                                responce = new DataResponce(false, "ok");
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
                            case Commands.GetValue:
                                var result = GetKspValue(ParseKspValue(request.Info));
                                if (result != null)
                                {
                                    responce = new DataResponce(false, new MemberInfoWrapper()
                                    {
                                        Name = request.Info.Name,
                                        ItemType = MemberType.Value,
                                        Value = result.ToString(),
                                        TypeName = result.GetType().Name
                                    });
                                }
                                break;
                            case Commands.GetChildren:
                                var children = GetChildren(request.Info);
                                responce = new DataResponce(false, children);
                                break;
                            case Commands.GetCollection:
                                var collection = GetCollection(request.Info);
                                if (collection != null)
                                {
                                    responce = new DataResponce(false, collection);
                                }
                                break;
                            case Commands.GetMethods:
                                var methods = GetMethods(request.Info);

                                if (methods != null)
                                {
                                    responce = new DataResponce(false, methods);
                                }

                                break;
                            case Commands.GetExternal:
                                if (_externalPart != null)
                                {
                                    responce = new DataResponce(false, new MemberInfoWrapper()
                                    {
                                        Name = _externalPart.GetType().Name,
                                        ItemType = MemberType.Type
                                    });
                                }
                                break;
                            case Commands.GetGameEvents:
                                var events = GetEvents();

                                if (events != null)
                                {
                                    responce = new DataResponce(false, events);
                                }
                                break;
                            case Commands.EventAttach:
                                EventSubscriber.Instance.Subscribe(request.Info.Name);
                                break;
                            case Commands.EventDetach:
                                EventSubscriber.Instance.UnSubscribe(request.Info.Name);
                                break;
                            case Commands.SetValue:
                                SetKspValue(ParseKspValue(request.Info));
                                break;
                            case Commands.CallMethod:
                                var calledObj = GetKspValue(ParseKspValue(request.Info));

                                if (calledObj != null && !calledObj.GetType().IsSimpleKspType())
                                {
                                    try
                                    {
                                        calledObj.CallMethod(request.Info.MethodName);
                                        LogClient.Instance.Send(string.Format("Method {0} for instance {1} was called succesfully", request.Info.MethodName, calledObj));
                                    }
                                    catch (Exception ex)
                                    {
                                        LogClient.Instance.Send(string.Format("Fatal error (call method): {0} {1}", ex.Message, ex.StackTrace));
                                    }
                                }

                                break;
                            case Commands.Refresh:
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
                LogClient.Instance.Send(string.Format("Fatal error: {0} {1}", ex.Message, ex.StackTrace));
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

        #region Commands

        private IEnumerable<MemberInfoWrapper> GetEvents()
        {
            try
            {
                var fields = typeof(GameEvents).GetFields(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Static |
                                          BindingFlags.Public).Where(x => !x.FieldType.IsPrimitive).Select(x => x.ConvertToWrapper(false)).ToList();

                fields.ForEach(x => x.ItemType = MemberType.GameEvent);

                return fields.OrderBy(x => x.Name).ToList();
            }
            catch (Exception ex)
            {
                LogClient.Instance.Send(string.Format("Fatal error (GetEvents): {0} {1}", ex.Message, ex.StackTrace));
                return null;
            }
        }

        private IEnumerable<MemberInfoWrapper> GetCollection(MemberInfoWrapper info)
        {
            var result = new List<MemberInfoWrapper>();

            var source = GetKspValue(ParseKspValue(info));

            if (source == null) return null;

            if (!source.GetType().Implements<IEnumerable>()) return null;

            var i = 0;

            LogClient.Instance.Send(string.Format("Collection: {0} has {1} count children", source, (source as IEnumerable).Cast<object>().Count()));

            foreach (var item in (IEnumerable)source)
            {
                if (_types.ContainsKey(item.GetType().Name) && !item.GetType().IsSimpleKspType())
                {
                    result.Add(new MemberInfoWrapper()
                    {
                        Name = item.GetType().Name,
                        TypeName = item.GetType().Name,
                        ItemType = MemberType.Type,
                        Index = i,
                        IsStatic = false
                    });
                }
                else
                {
                    result.Add(new MemberInfoWrapper()
                    {
                        Name = item.GetType().Name,
                        ItemType = MemberType.Property,
                        TypeName = item.GetType().Name,
                        Index = i,
                        IsStatic = false
                    });
                }

                i++;
            }

            return result;
        }

        private IEnumerable<MemberInfoWrapper> GetChildren(MemberInfoWrapper info)
        {
            try
            {
                if (!_types.ContainsKey(info.TypeName) && !GetKspType(info.Parent))
                    return null;

                var children = new List<MemberInfoWrapper>();

                var type = _types[info.TypeName];

                var items = type.FieldsAndProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static);

                if (items.Any())
                {
                    children.AddRange(items.Select(x => x.ConvertToWrapper()));
                }

                return children.OrderBy(x => x.Name).ToList();
            }
            catch (KeyNotFoundException ex)
            {
                LogClient.Instance.Send(string.Format("Fatal error key not found (GetChildren) - key {2}: {0} {1}", ex.Message, ex.StackTrace, info.TypeName));
                return null;
            }
            catch (Exception ex)
            {
                LogClient.Instance.Send(string.Format("Fatal error (GetChildren): {0} {1}", ex.Message, ex.StackTrace));
                return null;
            }
        }

        private object GetKspValue(List<MemberInfoWrapper> wrappers, object instance = null, Type parent = null)
        {
            if (wrappers == null || !wrappers.Any()) return "wrappers is empty";

            var currentWrapper = wrappers.FirstOrDefault();

            LogClient.Instance.Send(string.Format("GetKspValue: First item {0}", currentWrapper.Name));

            wrappers = wrappers.Skip(1).ToList();

            try
            {
                //get instance
                if (parent == null)
                {
                    Type t;
                    object obj;
                    var result = GetTypeInstance(currentWrapper, out t, out obj);
                    return result ? GetKspValue(wrappers, obj, t) : "cannot instantiate type";
                }

                //collection

                var value = GetIndexedPropertyValue(instance, currentWrapper);

                LogClient.Instance.Send(string.Format("GetKspValue: Indexed property is {0}", value ?? "null"));

                if (value != null)
                {
                    return wrappers.Any() ? GetKspValue(wrappers, value, value.GetType()) : value;
                }

                //get value for a member

                var member = parent.Members(MemberTypes.Field | MemberTypes.Property, BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static, currentWrapper.Name).FirstOrDefault();

                if (member == null) return "cannot get member";

                LogClient.Instance.Send(string.Format("GetKspValue: Member {0} type {2} from instance {1}", member.Name, instance ?? "null", member.MemberType));

                var memberValue = member.IsStatic() ? member.Get() : member.Get(instance);

                return wrappers.Any() ? GetKspValue(wrappers, memberValue, memberValue.GetType()) : memberValue;
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
                LogClient.Instance.Send(string.Format("Fatal error (GetKspValue): {0} {1}", ex.Message, ex.StackTrace));
                return ex.Message;
            }
        }

        private bool GetTypeInstance(MemberInfoWrapper currentWrapper, out Type t, out object instance)
        {
            Type type;

            instance = null;

            var result = _types.TryGetValue(currentWrapper.TypeName, out type);

            t = type;

            if (!result) return false;

            string instanceType;

            result = _instantiateTypes.TryGetValue(type.Name, out instanceType);

            if (!result) return true;

            var instanceField = type.GetField(instanceType);

            if (instanceField != null)
            {
                instance = instanceField.GetValue(null);
            }

            var instanceProp = type.GetProperty(instanceType);

            if (instanceProp == null) return true;

            instance = instanceProp.GetValue(null, null);

            return true;
        }

        private object GetIndexedPropertyValue(object instance, MemberInfoWrapper wrapper)
        {
            if (instance == null) return null;

            var instanceType = instance.GetType();

            if (instanceType.Implements<IList>())
            {
                return ((IList)instance)[wrapper.Index];
            }

            if (instanceType.IsArray)
            {
                return instance.GetElement(wrapper.Index);
            }

            return instanceType.Implements<IEnumerable>() ? ((IEnumerable)instance).Cast<object>().ElementAt(wrapper.Index) : null;
        }

        private void SetKspValue(List<MemberInfoWrapper> wrappers, object instance = null, Type parent = null)
        {
            if (wrappers == null || !wrappers.Any()) return;

            var currentWrapper = wrappers.FirstOrDefault();

            LogClient.Instance.Send(string.Format("SetKspValue: First item {0}", currentWrapper.Name));

            wrappers = wrappers.Skip(1).ToList();

            try
            {
                if (parent == null)
                {
                    Type t;
                    object obj;
                    var result = GetTypeInstance(currentWrapper, out t, out obj);

                    if (!result) return;

                    SetKspValue(wrappers, obj, t);
                }

                //collection

                var indexedValue = GetIndexedPropertyValue(instance, currentWrapper);

                if (indexedValue != null)
                {
                    LogClient.Instance.Send(string.Format("SetKspValue: Indexed property is {0}", indexedValue));

                    if (wrappers.Any())
                    {
                        SetKspValue(wrappers, indexedValue, indexedValue.GetType());
                    }
                }

                var member = parent.GetMember(currentWrapper.Name, MemberTypes.Field | MemberTypes.Property, BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static).FirstOrDefault();

                if (member == null) return;

                if (wrappers.Any())
                {
                    LogClient.Instance.Send(string.Format("SetKspValue: Get member {0}", member.Name));

                    var value = member.IsStatic() ? member.Get() : member.Get(instance);

                    LogClient.Instance.Send(string.Format("SetKspValue: Get member value {0}", value));

                    SetKspValue(wrappers, value, member.Type());
                }


                var data = ConvertValue(member.Type(), currentWrapper.Value);

                if (member.IsWritable())
                {
                    LogClient.Instance.Send(string.Format("SetKspValue: Try setting value {0} for instance {1}", data, instance ?? "null"));

                    member.Set(member.IsStatic() ? null : instance, data);

                    LogClient.Instance.Send(string.Format("Member {0} has setted to {1}", member.Name, data));
                }
                else
                {
                    LogClient.Instance.Send(string.Format("SetKspValue: Member {0} is not writable", member.Name));
                }
            }
            catch (Exception ex)
            {
                LogClient.Instance.Send(string.Format("Fatal error: {0} {1}", ex.Message, ex.StackTrace));
            }
        }

        private IEnumerable<MemberInfoWrapper> GetAllTypes()
        {
            lock (_o)
            {
                Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

                IEnumerable<string> externalAssemblies = GetExternalAssemblies();

                var comparer = new TypeEqualityComparer();

                foreach (var externalAssembly in externalAssemblies)
                {
                    LogClient.Instance.Send(string.Format("GetAllTypes: External type {0}", externalAssembly));

                    List<Type> externalTypes =
                     assemblies.Where(x => x.FullName.Contains(externalAssembly))
                         .SelectMany(x => x.GetTypes()).Where(x => !x.IsInterface && !x.IsAbstract)
                         .Where(x => Regex.IsMatch(x.Name, "^[A-Za-z0-9]{3,}$", RegexOptions.IgnoreCase))
                         .OrderBy(x => x.Name)
                         .Distinct(comparer)
                         .ToList();

                    externalTypes.ForEach(x => { if (!_types.ContainsKey(x.Name)) { _types.Add(x.Name, x); } });
                }

                return _types.Select(x => new MemberInfoWrapper
                {
                    Name = x.Key,
                    TypeName = x.Key,
                    ItemType = MemberType.Type,
                }).ToList();
            }
        }

        private IEnumerable<MethodInfoWrapper> GetMethods(MemberInfoWrapper wrapper)
        {
            if (!_types.ContainsKey(wrapper.TypeName)) return null;

            var wrappers = new List<MethodInfoWrapper>();

            var methods = _types[wrapper.TypeName].GetMethods(BindingFlags.Static | BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public).ToList();

            wrappers.AddRange(methods.Select(x => x.ConvertToWrapper()));

            return wrappers;
        }

        #endregion

        #region DataHelpers

        private List<MemberInfoWrapper> ParseKspValue(MemberInfoWrapper data)
        {
            var items = new List<MemberInfoWrapper>();

            MemberInfoWrapper item = data;

            while (item != null)
            {
                items.Add(item);
                item = item.Parent;
            }

            items.Reverse();

            return items;
        }

        private object ConvertValue(Type t, object o)
        {
            try
            {
                if (t == typeof(int))
                {
                    return int.Parse(o.ToString());
                }

                if (t == typeof(bool))
                {
                    return bool.Parse(o.ToString());
                }

                if (t == typeof(Single))
                {
                    return Single.Parse(o.ToString());
                }

                if (t == typeof(double))
                {
                    return double.Parse(o.ToString());
                }

                if (t == typeof(string))
                {
                    return o.ToString();
                }

                return null;
            }
            catch (Exception ex)
            {
                LogClient.Instance.Send(string.Format("(ConvertValue) Fatal error: {0} {1}", ex.Message, ex.StackTrace));
                return null;
            }
        }

        private bool GetKspType(MemberInfoWrapper parent)
        {
            try
            {
                if (parent == null)
                {
                    return false;
                }

                if (_types.ContainsKey(parent.TypeName)) return true;

                var t = AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(x => x.GetTypes())
                    .FirstOrDefault(y => y.Name.Equals(parent.TypeName));

                if (t == null) return false;

                _types.Add(t.Name, t);

                return true;
            }
            catch (Exception ex)
            {
                LogClient.Instance.Send(string.Format("(GetKspType) Fatal error: {0} {1}", ex.Message, ex.StackTrace));
                return false;
            }
        }

        private IEnumerable<string> GetExternalAssemblies()
        {
            var filePath = Path.Combine(GetCurrentDirectory(), ASSEMBLY_FILE_NAME);
            return File.Exists(filePath) ? File.ReadAllLines(filePath).ToList() : new List<string>();
        }

        private void ConfigureInstantiateAssemblies()
        {
            var filePath = Path.Combine(GetCurrentDirectory(), INSTANTIATE_FILE_NAME);
            if (!File.Exists(filePath)) return;

            var items = File.ReadAllLines(filePath);

            var parsedItems = items.Select(x => x.Split(',')).ToList();

            parsedItems.ForEach(x => _instantiateTypes.Add(x.First(), x.Last()));
        }

        private string GetCurrentDirectory()
        {
            return Path.GetDirectoryName(GetType().Assembly.Location);
        }

        #endregion

        #region Request helpers

        private DataRequest GetDataRequest(byte[] data)
        {
            lock (_o)
            {
                var formatter = new BinaryFormatter();
                return formatter.Deserialize(new MemoryStream(data)) as DataRequest;
            }
        }

        #endregion

        #region External attach

        private void ServerAttached(Part data)
        {
            _externalPart = data;
            LogClient.Instance.Send(string.Format("Object attached to server: {0} {1}", data.name, data.GetInstanceID()));
        }

        #endregion

        #region Mono

        public void OnDestroy()
        {
            ServerEvents.OnAttachToServer.Remove(ServerAttached);

            if (_listener != null)
            {
                _listener.Stop();
            }
        }

        #endregion

    }
}