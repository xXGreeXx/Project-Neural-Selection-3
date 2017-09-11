using System;
using System.Collections.Generic;
using System.Drawing;

namespace Project_Neural_Selection_3
{
    public class RenderingEngine
    {
        //define global variables
        

        //constructor
        public RenderingEngine()
        {

        }

        //draw screen
        public void DrawScreen(Graphics g, int width, int height)
        {
            //draw creatures
            foreach (Creature c in Game.creatures)
            {
                float baseX = c.x;
                float baseY = c.y;
                Brush brush = new SolidBrush(c.color);
                
                foreach (RectangleF r in c.shape)
                {
                    g.FillEllipse(brush, baseX + r.X, baseY + r.Y, r.Width, r.Height);
                }
            }
        }
    }
}
