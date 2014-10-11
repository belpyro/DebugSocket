using System;

namespace SocketCommon
{
    [Serializable]
    public class DataRequest
    {
        public DataRequest(string typeName, Commands command, string member = null)
        {
            TypeName = typeName;
            Command = command;
            MemberName = member;
        }

        public string TypeName { get; private set; }
        
        public string MemberName { get; private set; }

        public Commands Command { get; private set; }
    }
}