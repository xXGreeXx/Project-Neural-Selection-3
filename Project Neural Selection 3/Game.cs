using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Project_Neural_Selection_3
{
    public partial class Game : Form
    {
        //define global variables

        //constructor
        public Game()
        {
            InitializeComponent();

            renderTimer.Start();
        }

        //render timer tick
        private void renderTimer_Tick(object sender, EventArgs e)
        {
            canvas.Refresh();
        }
    }
}
