using System;

namespace SocketCommon
{
    [Serializable]
    public class DataRequest
    {
        public DataRequest(string name, Commands command)
        {
            TypeName = name;
            Command = command;
        }

        public string TypeName { get; private set; }

        public Commands Command { get; private set; }
    }

    [Serializable]
    public class DataResponce
    {
        public DataResponce(bool isError, object value)
        {
            HasError = isError;
            Data = value;
        }

        public bool HasError { get; private set; }

        public object Data { get; private set; }
    }
}