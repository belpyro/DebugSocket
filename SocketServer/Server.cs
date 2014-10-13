using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using SocketCommon;
using SocketCommon.Attributes;
using SocketCommon.Wrappers;
using UnityEngine;
using UnityThreading;

namespace SocketServer
{

    [KSPAddon(KSPAddon.Startup.Flight, false)]
    public class Server : MonoBehaviour
    {
        private readonly object o = new object();
        private TcpListener _listener = null;
        private readonly Dictionary<string, TypeDataInfo> _types = new Dictionary<string, TypeDataInfo>();
        private readonly Dictionary<object, object> _items = new Dictionary<object, object>();

        public void Start()
        {
            //PrepareWrappers();

            var t = new Thread(StartServer);
            t.Start();
        }

        private void PrepareWrappers()
        {
            lock (o)
            {
                var allTypes = AppDomain.CurrentDomain.GetAssemblies()
                        .SelectMany(x => x.GetTypes()).ToList();

                var wrappers = allTypes.Where(x => x.Name.EndsWith("_Wrapper")).ToList();

                var types = allTypes.Where(x => !wrappers.Contains(x)).Select(x => x.Name).Distinct().ToList();

                foreach (var kspType in types)
                {
                    var wrapper = wrappers.FirstOrDefault(x => x.Name.Equals(string.Format("{0}_Wrapper", kspType)));

                    if (wrapper == null) continue;

                    _types.Add(kspType, new TypeDataInfo()
                    {
                        Assembly = wrapper.AssemblyQualifiedName,
                        FullName = wrapper.Name
                    });
                }
            }

        }

        private void StartServer()
        {
            try
            {
                PrepareWrappers();

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
                                    var data = FillTypeData(request.TypeName);
                                    responce = data != null ? new DataResponce(false, data) : new DataResponce(true, "Cannot populate wrapper");
                                    _items.Clear();
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

        private object FillTypeData(string type, object obj = null)
        {
            Debug.Log(string.Format("Start fill type {0} with obj {1}", type, obj));

            //check wrapper
            if (!_types.ContainsKey(type)) return null;

            //load wrapper

            var wrapperType = _types[type];

            var loadedType = Type.GetType(wrapperType.Assembly);

            var wrapper = Activator.CreateInstance(loadedType);

            var kspType = GetKspType(type);

            object instance = obj;

            //populate static fields

            #region instance attribute

            var instanceAttribute = loadedType.GetCustomAttributes(typeof(InstanceNameAttribute), true).FirstOrDefault() as InstanceNameAttribute;

            if (instanceAttribute != null && instance == null)
            {
                Debug.Log("Attribute has " + instanceAttribute.Name);

                var instanceProp = kspType.GetProperty(instanceAttribute.Name);

                if (instanceProp != null)
                {
                    instance = instanceProp.GetValue(null, null);
                }
            }

            #endregion

            #region id attribute

            var idAttribute = loadedType.GetCustomAttributes(typeof(UniqIdAttribute), true).FirstOrDefault() as UniqIdAttribute;

            if (idAttribute != null && instance != null)
            {
                Debug.Log("Attribute has " + idAttribute.Name);

                MemberInfo instanceProp = kspType.GetMember(idAttribute.Name).FirstOrDefault();

                if (instanceProp != null)
                {
                    var uniqId = instanceProp.MemberType == MemberTypes.Property ? (instanceProp as PropertyInfo).GetValue(instance, null) :
                        (instanceProp as FieldInfo).GetValue(instance);

                    if (_items.ContainsKey(uniqId))
                    {
                        Debug.Log("Uniq object was returned");
                        return _items[uniqId];
                    }

                    wrapper.GetType().GetField(idAttribute.Name).SetValue(wrapper, uniqId);

                    Debug.Log("Attribute setted " + idAttribute.Name + " " + uniqId);

                    _items.Add(uniqId, wrapper);
                }
            }

            #endregion

            var wrapperFields = wrapper.GetType().GetFields();

            var typeFields = kspType.GetFields().Where(x => wrapperFields.Any(m => m.Name.Equals(x.Name))).ToList();

            PopulateStaticFields(wrapper, typeFields);

            //foreach (var fieldInfo in typeFields)
            //{
            //    FieldInfo wrapperField = wrapperFields.Single(x => x.Name.Equals(fieldInfo.Name));

            //    //field is class
            //    if ((fieldInfo.FieldType.IsClass || fieldInfo.FieldType.IsValueType) && !fieldInfo.FieldType.IsPrimitive && fieldInfo.FieldType != typeof(string))
            //    {
            //        if (!fieldInfo.IsStatic && instance != null)
            //        {
            //            var interfaces = fieldInfo.FieldType.GetInterfaces();

            //            if (interfaces.Any(t => t == typeof(IEnumerable))) continue;

            //            var valueKsp = fieldInfo.GetValue(instance);

            //            Debug.Log(string.Format("Start fill field {0} with value {1}", fieldInfo.Name, valueKsp));

            //            if (valueKsp.GetType().IsSerializable)
            //            {
            //                wrapperField.SetValue(wrapper, valueKsp);
            //                continue;
            //            }

            //            wrapperField.SetValue(wrapper, FillTypeData(fieldInfo.FieldType.Name, valueKsp));

            //            //continue;
            //        }


            //        //wrapperField.SetValue(wrapper, FillTypeData(fieldInfo.FieldType.Name));

            //    }

            //    //field is primitive

            //    //wrapperField.SetValue(wrapper, fieldInfo.GetValue(instance));
            //}



            //populate properties

            return wrapper;
        }

        private void PopulateStaticFields(object wrapperObj, IEnumerable<FieldInfo> sourceFields)
        {
            foreach (var fieldInfo in sourceFields.Where(x => x.IsStatic))
            {

                if (_types.ContainsKey(fieldInfo.FieldType.Name))
                {
                    var wrapper = Activator.CreateInstance(fieldInfo.FieldType);
                    fieldInfo.SetValue(wrapperObj, wrapper);
                    continue;
                }

                var sourceField = wrapperObj.GetType().GetField(fieldInfo.Name);
                sourceField.SetValue(wrapperObj, fieldInfo.GetValue(null));
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

                            //if (_types.ContainsKey(f.FieldType.Name))
                            //{
                            //    return Populate(f.FieldType);
                            //}
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
                var assemblies = AppDomain.CurrentDomain.GetAssemblies();

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

    }
}
