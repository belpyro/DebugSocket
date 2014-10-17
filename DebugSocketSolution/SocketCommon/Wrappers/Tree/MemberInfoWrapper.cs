using System;
using System.Collections.Generic;

namespace SocketCommon.Wrappers.Tree
{
    [Serializable]
    public class MemberInfoWrapper
    {
        public string Name { get; set; }
        
        public string TypeName { get; set; }

        public IEnumerable<object> Value { get; set; }

        public MemberType Type { get; set; }
    }
}