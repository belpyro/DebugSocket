using System;

namespace SocketCommon.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class InstanceNameAttribute: Attribute
    {
        public InstanceNameAttribute(string name)
        {
            Name = name;
        }
        
        public string Name { get; private set; }
    }
}
