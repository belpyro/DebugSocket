﻿using System;
using System.Collections.Generic;

namespace DebugHelper
{
    using System.IO;
    using System.Net;
    using System.Net.Sockets;
    using System.Runtime.Serialization.Formatters.Binary;

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

                    var request = new DataRequest(name, Commands.GetType);

                    using (var mStream = new MemoryStream())
                    {
                        _formatter.Serialize(mStream, request);

                        //send request
                        client.GetStream().Write(mStream.ToArray(), 0, (int)mStream.Length);

                        //get response

                        var buff = new byte[1024];

                        // ReSharper disable once ConditionIsAlwaysTrueOrFalse

                        var realBuff = new List<byte>();

                        while ((client.GetStream().Read(buff, 0, buff.Length)) > 0)
                        {
                            realBuff.AddRange(buff);
                        }


                        using (var dsStream = new MemoryStream(realBuff.ToArray()))
                        {
                            var obj = _formatter.Deserialize(dsStream);
                            return obj as DataResponce;
                        }

                    }

                }
                catch (Exception ex)
                {
                    return new DataResponce(true, ex);
                }
            }
        }

        public DataResponce GetAll()
        {
            using (var client = new TcpClient(AddressFamily.InterNetwork))
            {
                try
                {
                    client.ReceiveBufferSize = 900000;
                    client.SendBufferSize = 900000;
                    client.ReceiveTimeout = 1000;

                    client.Connect(IPAddress.Parse("127.0.0.1"), 11000);

                    var request = new DataRequest(null, Commands.GetTypes);

                    using (var mStream = new MemoryStream())
                    {
                        _formatter.Serialize(mStream, request);

                        //send request
                        client.GetStream().Write(mStream.ToArray(), 0, (int)mStream.Length);

                        //get response

                        var buff = new byte[client.ReceiveBufferSize];

                        // ReSharper disable once ConditionIsAlwaysTrueOrFalse

                        var realBuff = new List<byte>();

                        while ((client.GetStream().Read(buff, 0, client.ReceiveBufferSize)) > 0)
                        {
                           realBuff.AddRange(buff);
                        }
                       

                        using (var dsStream = new MemoryStream(realBuff.ToArray()))
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
