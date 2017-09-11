using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Project_Neural_Selection_3
{
    public partial class Game : Form
    {
        //define global variables
        public static RenderingEngine renderer { get; } = new RenderingEngine();

        int physicsSpeed = 20;
        int baseAmountOfCreatures = 30;

        public static List<Creature> creatures { get; set; } = new List<Creature>();
        public static int creatureSize { get; } = 5;
        public static int creatureMutationRate { get; } = 3;

        //constructor
        public Game()
        {
            InitializeComponent();

            this.MinimizeBox = false;
            this.MaximizeBox = false;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;

            renderTimer.Start();
            physicsTimer.Start();
        }

        //start new game
        private void startNewGame()
        {

        }

        //render timer tick
        private void renderTimer_Tick(object sender, EventArgs e)
        {
            physicsTimer.Interval = 1000 / physicsSpeed;

            canvas.Refresh();
        }

        //canvas update
        private void canvas_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            int width = canvas.Width;
            int height = canvas.Height;

            renderer.DrawScreen(g, width, height);
        }

        //call physics engine
        private void physicsTimer_Tick(object sender, EventArgs e)
        {

        }
    }
}
