using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace SocketCommon
{
    using System;

    [Serializable]
    public class TypeWrapper
    {
        public IEnumerable<FieldInfo> Fields { get; set; }

        public IEnumerable<PropertyInfo> Properties { get; set; }

        public string Name { get; set; }
    }
}
