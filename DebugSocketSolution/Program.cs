using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SocketClient;

namespace DebugSocketSolution
{
    class Program
    {
        static void Main(string[] args)
        {
            var client = new Client();
            client.StartClient();
        }
    }
}
