using System;
using System.Windows.Forms;

namespace Project_Neural_Selection_3
{
    public class MouseHandler
    {
        //define global variables
        public static int x { get; set; } = 0;
        public static int y { get; set; } = 0;
        public static Boolean down { get; set; } = false;

        //constructor
        public MouseHandler()
        {

        }

        //register mouse down
        public void RegisterMouseDown(int x, int y, MouseButtons button)
        {
            MouseHandler.x = x;
            MouseHandler.y = y;
            down = true;
        }

        //register mouse up
        public void RegisterMouseUp(int x, int y, MouseButtons button)
        {
            down = false;
        }
    }
}
