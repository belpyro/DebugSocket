using System;

namespace SocketCommon.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class UniqIdAttribute: Attribute
    {
        public UniqIdAttribute(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
    }
}
