using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace SocketCommon
{
    public class TypeWrapper
    {
        [Category("Fields")]
        public IEnumerable<FieldInfo> Fields { get; set; }

        [Category("Properties")]
        public IEnumerable<PropertyInfo> Properties { get; set; }

        [Category("Common")]
        public string Name { get; set; }
    }
}
