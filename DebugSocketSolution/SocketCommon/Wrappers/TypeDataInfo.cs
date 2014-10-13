using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SocketCommon.Wrappers
{
    [Serializable]
    public class TypeDataInfo
    {
        public string Assembly { get; set; }

        public string FullName { get; set; }
    }
}
