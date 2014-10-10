using System;
using System.Collections.Generic;

namespace SocketCommon.Wrappers
{
    [Serializable]
    public class AssemblyWrapper
    {
        public string Location { get; set; }

        public string Name { get; set; }

        public IEnumerable<string> Types { get; set; }
    }
}
