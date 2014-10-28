using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace SocketCommon.NetFactories
{
    public class ListenerManager
    {
        
    }

    public class Listener
    {
        public void Start()
        {
            
        }

        public void Stop()
        {
            
        }

        public IPAddress Address { get; private set; }

        public int Port { get; private set; }

        public event EventHandler OnRecieved;
    }

    public class Client
    {
        
    }
}
