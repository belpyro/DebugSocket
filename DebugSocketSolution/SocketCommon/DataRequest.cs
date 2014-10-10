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
}