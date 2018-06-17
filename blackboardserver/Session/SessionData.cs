using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Diagnostics;
using WebSocketSharp;
using System.DrawingCore;
using System.DrawingCore.Imaging;
using System.IO;

namespace blackboardserver.Session
{
    class SessionData
    {
        public List<BlackboardUserBehavior> connectedUsers { get; private set; }
        public List<UserInput> InputHistory { get; private set; }
        //public long LastTimeSaved { get; set; }
        public string SessionName { get; private set; }

        public int Width { get; private set; }
        public int Height { get; private set; }

        private Stopwatch saveTimer;


        public SessionData(int width, int height)
        {
            saveTimer = new Stopwatch();
            saveTimer.Start();
            connectedUsers = new List<BlackboardUserBehavior>();
            InputHistory = new List<UserInput>();
           // LastTimeSaved = saveTimer.ElapsedMilliseconds;

            Width = width;
            Height = height;
        }

        public void SaveInputsToImage()
        {
            Image bm = Bitmap.FromFile($@"blackboard/{connectedUsers[0].SessionId}/save.png");
            Graphics g = Graphics.FromImage(bm);
            foreach (UserInput ui in InputHistory)
            {
                //send sessionid just to be sure it saves
                ui.renderToBitmap(g, Width, Height, connectedUsers[0].SessionId);
            }

            bm.Save($@"blackboard/{connectedUsers[0].SessionId}/save.png", ImageFormat.Png);
            
            InputHistory.Clear();
            bm.Dispose();
            g.Dispose();
        }

    }
}
