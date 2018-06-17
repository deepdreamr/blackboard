using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization.Formatters.Binary;
using WebSocketSharp;
using WebSocketSharp.Server;
using System.IO;
using System.DrawingCore;
using System.DrawingCore.Imaging;
using System.Globalization;

namespace blackboardserver.Session
{
    class BlackboardUserBehavior : WebSocketBehavior
    {
        private delegate void SendFile(string path);
        private SendFile SendImg { get; set; }
        private ulong count = 0L;

        public bool IsOwner { get; private set; }
        public bool CanDraw { get; private set; }
        public string Name { get; private set; }
        public ulong UserId { get; private set; }
        public ulong SessionId { get; private set; }


        public BlackboardUserBehavior()
        {
           SendImg = (path) =>
           {
               FileInfo info = new FileInfo(path);
               Send(info);
           };
        }

        protected override void OnOpen()
        {
            SessionId = ulong.Parse(Context.QueryString["session"]);
            Name = Context.QueryString["name"];
            string imagePath = $"blackboard/{SessionId}/save.png";

            CanDraw = true;
            UserId = count++;
            if (SessionList.sessionMap.ContainsKey(SessionId))
            {
                //do join session
                IsOwner = false;
            }
            else
            {
                //do create session
                IsOwner = true;
                SessionList.sessionMap.Add(SessionId, new SessionData(int.Parse(Context.QueryString["width"]),
                                                    int.Parse(Context.QueryString["height"])));
            }
            var session = SessionList.sessionMap[SessionId];
            session.connectedUsers.Add(this);
            //add this instance to UserBehaviorList

            if (!File.Exists(imagePath))
            {
                using (Image img = new Bitmap(session.Width, session.Height, PixelFormat.Format32bppArgb))
                {
                    Directory.CreateDirectory($@"blackboard/{SessionId}/");
                    img.Save(imagePath, ImageFormat.Png);
                }
            }

            SendImg(imagePath);
            System.Threading.Thread.Sleep(200);
            string input = "";
            foreach (UserInput ui in session.InputHistory)
            {
                input += ui + "\n";
            }

            Send(input);
            //send back an instance of the image as well as the InputHistory
        }

        private void broadcastToOthers(string s)
        {
            foreach(IWebSocketSession session in Sessions.Sessions)
            {
                if(session is BlackboardUserBehavior && session != this)
                {
                    BlackboardUserBehavior behavior = session as BlackboardUserBehavior;
                    behavior.Send(s);
                }
            }
        }

        protected override void OnMessage(MessageEventArgs e)
        {
            /**
            Color c = Color.FromArgb(int.Parse(Context.QueryString["color"]));
            int lWidth = int.Parse(Context.QueryString["line-width"]);
            var p = Context.QueryString["points"];

            lw,color,5 5,4 4,3 3,2 2,1 1
            pointlist = 5 5,4 4,3 3
            */

            //send the client data back to everyone else

            string[] data = e.Data.Split(',');
            int numInputs = data.Length - 2;

            if (numInputs < 1) return;
            if (numInputs > 500) return;

            Sessions.Broadcast(e.Data);

            var session = SessionList.sessionMap[SessionId];

            if (session.InputHistory.Count > 500)
            {
                session.SaveInputsToImage();
            }
            UserInput newInput = new UserInput();
            float width = float.Parse(data[0]);
            Color color = parseColorRGBA(data[1]);

            newInput.Color = color;

            for (int i = 2; i < data.Length; i++)
            {
                String[] p = data[i].Split(' ');
                newInput.MouseInputs.Add(new PointF(float.Parse(p[0]), float.Parse(p[1])));
            }

            session.InputHistory.Add(newInput);
        }

        private Color parseColorRGBA(string s)
        {
            //FF001234
            //34FF0012

            byte r = byte.Parse(s.Substring(0, 2), NumberStyles.HexNumber);
            byte g = byte.Parse(s.Substring(2, 2), NumberStyles.HexNumber);
            byte b = byte.Parse(s.Substring(4, 2), NumberStyles.HexNumber);
            byte a = byte.Parse(s.Substring(6), NumberStyles.HexNumber);

            

            return Color.FromArgb(a, r, g, b);
        }

        protected override void OnClose(CloseEventArgs e)
        {
            Send($"{UserId}, has left the session");
            if(IsOwner)
            {
                Send("The owner has left the session. Close broadcast.");
            }
        }

        protected override void OnError(WebSocketSharp.ErrorEventArgs e)
        {
            Send($"{UserId}, has exit the session");
            if (IsOwner)
            {
                Send("The owner has exited the session. Close broadcast.");
            }
        }
    }
}
