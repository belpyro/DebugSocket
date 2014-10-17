using System;
using System.Collections.Generic;

namespace SocketCommon.Wrappers.Tree
{
    [Serializable]
    public class MemberInfoWrapper
    {
        public string Name { get; set; }

        public string TypeName { get; set; }

        public List<MemberInfoWrapper> Children { get; set; }

        public object Value { get; set; }
    }
}