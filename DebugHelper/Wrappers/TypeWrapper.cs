using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DebugHelper.Wrappers
{
    using System.ComponentModel;
    using System.Reflection;

    using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

    public class TypeWrapper
    {
        [Category("Fields")]
        [ExpandableObject]
        public IEnumerable<FieldInfo> Fields { get; set; }

        [Category("Properties")]
        [ExpandableObject]
        public IEnumerable<PropertyInfo> Properties { get; set; }

        [Category("Common")]
        public string Name { get; set; }
    }
}
