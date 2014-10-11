using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace SocketCommon.Wrappers
{
    [Serializable]
    public class PropertyInfoWrapper
    {
        public PropertyInfo Data { get; set; }

        public object Value { get; set; }
    }
}
