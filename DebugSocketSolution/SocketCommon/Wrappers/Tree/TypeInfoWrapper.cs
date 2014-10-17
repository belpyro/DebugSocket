using System;
using System.Collections.Generic;

namespace SocketCommon.Wrappers.Tree
{
    [Serializable]
    public class TypeInfoWrapper
    {
        public string InstanceName { get; set; }

        public string Name { get; set; }

        public IEnumerable<MemberInfoWrapper> Members { get; set; }
    }
}
