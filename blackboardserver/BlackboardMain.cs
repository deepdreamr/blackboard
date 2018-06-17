using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebSocketSharp;
using WebSocketSharp.Server;
using blackboardserver.Session;

namespace blackboardserver
{
    class BlackboardMain
    {
        public static void Main(String[] args)
        {
            var server = new WebSocketServer(61324);
            server.AddWebSocketService<BlackboardSessionBehavior>("/blackboard");
            server.AddWebSocketService<BlackboardUserBehavior>("/blackboard/session");
            server.Start();
            Console.ReadKey();
            server.Stop();
        }
    }
}
