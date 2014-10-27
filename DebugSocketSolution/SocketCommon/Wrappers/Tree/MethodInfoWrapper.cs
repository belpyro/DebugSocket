using System;
using System.Collections.Generic;

namespace SocketCommon.Wrappers.Tree
{
    [Serializable]
    public class MethodInfoWrapper
    {
        public string Name { get; set; }

        public List<MemberInfoWrapper> Parameters { get; set; }

        public bool IsStatic { get; set; }
    }
}
