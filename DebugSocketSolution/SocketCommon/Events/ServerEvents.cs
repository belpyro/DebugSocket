using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SocketCommon.Events
{
    public static class ServerEvents
    {
        public static readonly EventData<Part> OnAttachToServer = new EventData<Part>("OnAttachToServer");
    }
}
