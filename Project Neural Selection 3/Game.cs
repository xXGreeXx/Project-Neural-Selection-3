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
        int amountOfFood = 2000;

        public static List<Food> food = new List<Food>();

        public static List<Creature> creatures { get; set; } = new List<Creature>();
        public static int creatureSize { get; } = 5;
        public static int creatureMutationRate { get; } = 3;

        public static float learningRate { get; } = 0.1F;
        public static Random r { get; } = new Random();
        int selectedCreature = -1;

        //constructor
        public Game()
        {
            InitializeComponent();

            this.MinimizeBox = false;
            this.MaximizeBox = false;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;

            renderTimer.Start();
            physicsTimer.Start();

            startNewGame();
        }

        //start new game
        private void startNewGame()
        {
            //create food
            for (int i = 0; i < amountOfFood; i++)
            {
                food.Add(new Food(r.Next(0, canvas.Width), r.Next(0, canvas.Height)));
            }

            //create creatures
            for (int i = 0; i < baseAmountOfCreatures; i++)
            {
                List<RectangleF> shape = new List<RectangleF>();
                List<Creature.CreatureInputs> inputs = new List<Creature.CreatureInputs>();

                for (int i2 = 0; i2 < r.Next(5, 20); i2++)
                {
                    shape.Add(new RectangleF(i2 + r.Next(1, 3), i2 + r.Next(1, 3), creatureSize, creatureSize));
                }

                Creature c = new Creature((canvas.Width / baseAmountOfCreatures) * i, r.Next(30, canvas.Height), shape, inputs, Color.FromArgb(255, r.Next(0, 255), r.Next(0, 255), r.Next(0, 255)));
                creatures.Add(c);
            }
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

        //update stats on loaded creature
        private void creatureStatsCanvas_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            int width = creatureStatsCanvas.Width;
            int height = creatureStatsCanvas.Height;

            if (selectedCreature != -1 && selectedCreature < creatures.Count)
            {
                //draw state of neural network
            }
        }

        //call physics engine
        private void physicsTimer_Tick(object sender, EventArgs e)
        {
            simulatePhysics();
        }

        //simulate physics
        private void simulatePhysics()
        {
            //simulate creatures
            List<int> creaturesToRemove = new List<int>();

            foreach (Creature c in creatures)
            {
                Boolean remove = c.SimulateCreature();

                if (remove) creaturesToRemove.Add(creatures.IndexOf(c));
            }
            
            //remove dead creatures
            creaturesToRemove.Sort();
            creaturesToRemove.Reverse();

            foreach (int index in creaturesToRemove)
            {
                creatures.RemoveAt(index);
            }
        }

    }
}
