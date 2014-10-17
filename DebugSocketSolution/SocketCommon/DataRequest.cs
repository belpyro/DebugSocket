using System;
using SocketCommon.Wrappers.Tree;

namespace SocketCommon
{
    [Serializable]
    public class DataRequest
    {
        public MemberInfoWrapper Info { get; set; }

        public Commands Command { get;  set; }
    }
}