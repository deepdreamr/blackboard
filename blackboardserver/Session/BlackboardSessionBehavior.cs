using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace blackboardserver.Session
{
    class BlackboardSessionBehavior : WebSocketBehavior
    {
        public BlackboardSessionBehavior()
        {
        }

        protected override void OnOpen()
        {
            //give the user a list of sessions including their:
            //ID, Session Name, Name of owner
        }
    }
}
