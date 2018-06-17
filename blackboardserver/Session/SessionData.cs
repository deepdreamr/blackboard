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


namespace blackboardserver.Session
{
    class SessionData
    {
        public List<BlackboardUserBehavior> connectedUsers { get; private set; }
        public List<UserInput> InputHistory { get; private set; }
        //public long LastTimeSaved { get; set; }
        public string SessionName { get; private set; }
        private readonly int width;
        private readonly int height;

        private Stopwatch saveTimer;


        public SessionData(int width, int height)
        {
            saveTimer = new Stopwatch();
            saveTimer.Start();
            connectedUsers = new List<BlackboardUserBehavior>();
            InputHistory = new List<UserInput>();
           // LastTimeSaved = saveTimer.ElapsedMilliseconds;

            this.width = width;
            this.height = height;
        }

        public void SaveInputsToImage()
        {
            Graphics g;
            if (System.IO.File.Exists($"{connectedUsers[0].SessionId}/save.png"))
                g = Graphics.FromImage(Bitmap.FromFile($@"blackboard/
                                    {connectedUsers[0].SessionId}/save.png"));
            else
            {
                Bitmap bm = new Bitmap(width, height, PixelFormat.Format32bppArgb);

                g = Graphics.FromImage(bm);
            }
            foreach (UserInput ui in InputHistory)
            {
                //send sessionid just to be sure it saves
                ui.renderToBitmap(g, width, height, connectedUsers[0].SessionId);
            }

            //LastTimeSaved = saveTimer.ElapsedMilliseconds - LastTimeSaved;

            InputHistory.Clear();
        }

    }
}
