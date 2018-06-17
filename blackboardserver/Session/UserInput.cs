using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.DrawingCore;
using System.DrawingCore.Imaging;

namespace blackboardserver.Session
{
    class UserInput
    {
        public Color Color { get; set; }
        public float LineWidth { get; set; }

        public List<PointF> MouseInputs { get; } = new List<PointF>();

        public void renderToBitmap(Graphics g, int width, int height, ulong SessionId)
        {
            //Add drawing information and points Here
            //If Additional Tools are added it is done Here
            g.DrawLines(new Pen(Color, LineWidth), MouseInputs.ToArray());
        }

        private static string formatColor(Color c)
        {
            return string.Format("{0:X2}{1:X2}{2:X2}{3:X2}", c.R, c.G, c.B, c.A);
        }

        public override string ToString()
        {
            string FearTheTurtle = "";
            foreach (PointF p in MouseInputs)
            {
                FearTheTurtle += "," + p.X + " " + p.Y;
            }
            return LineWidth + "," + formatColor(Color) + FearTheTurtle;
        }
    }

}
