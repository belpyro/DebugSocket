using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using SocketCommon.Wrappers;

namespace SocketCommon
{
    using System;

    [Serializable]
    public class TypeWrapper
    {
        public IEnumerable<FieldInfoWrapper> Fields { get; set; }

        public IEnumerable<PropertyInfoWrapper> Properties { get; set; }

        public string Name { get; set; }
    }
}
