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
        public static MouseHandler mouse { get; set; } = new MouseHandler();

        int physicsSpeed = 20;
        int baseAmountOfCreatures = 10;
        int amountOfFood = 2000;

        public static List<Food> food = new List<Food>();

        public static List<Creature> creatures { get; set; } = new List<Creature>();
        public static int creatureSize { get; } = 20;
        public static int creatureMutationRate { get; } = 10;
        public static int creatureSpeed { get; } = 5;

        public static float learningRate { get; } = 0.1F;
        public static Random r { get; } = new Random();
        public static int selectedCreature { get; set; } = 0;

        public static String gameState { get; set; } = "splash";

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
                List<Creature.CreatureInputs> inputs = new List<Creature.CreatureInputs>();
                List<int> rotationOfInput = new List<int>();

                for (int i2 = 0; i2 < r.Next(1, 5); i2++)
                {
                    inputs.Add(Creature.CreatureInputs.Eye);
                    rotationOfInput.Add(r.Next(0, 361));
                }

                Creature c = new Creature((canvas.Width / baseAmountOfCreatures) * i, r.Next(30, canvas.Height), inputs, rotationOfInput, Color.FromArgb(255, r.Next(0, 255), r.Next(0, 255), r.Next(0, 255)));
                creatures.Add(c);
            }
        }

        //render timer tick
        private void renderTimer_Tick(object sender, EventArgs e)
        {
            physicsSpeed = speedBar.Value;
            physicsTimer.Interval = 1000 / physicsSpeed;

            canvas.Refresh();
            creatureStatsCanvas.Refresh();
        }

        //canvas update
        private void canvas_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            int width = canvas.Width;
            int height = canvas.Height;

            if (gameState.Equals("game"))
            {
                renderer.DrawScreen(g, width, height);
            }
            else if (gameState.Equals("splash"))
            {
                renderer.SplashScreen(g, width, height);
            }
        }

        //update stats on loaded creature
        private void creatureStatsCanvas_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            int width = creatureStatsCanvas.Width;
            int height = creatureStatsCanvas.Height;
            Font fontSmall = new Font(FontFamily.GenericSansSerif, 7, FontStyle.Bold);

            if (selectedCreature != -1 && selectedCreature < creatures.Count)
            {
                Creature c = creatures[selectedCreature];

                //draw stats
                g.DrawString("Food:" + c.food, fontSmall, Brushes.Black, 0, height / 2 + 20);
                g.DrawString("Age:" + c.age, fontSmall, Brushes.Black, 0, height / 2 + 30);
                g.DrawString("Reproduction Value:" + c.reproductionValue, fontSmall, Brushes.Black, 0, height / 2 + 40);

                //draw neural network
                List<Perceptron>[] neuralNetwork = c.neuralNetwork;

                int y = height / 2 + 125;
                for (int layerIndex = 0; layerIndex < neuralNetwork.Length; layerIndex++)
                {
                    List<Perceptron> layer = neuralNetwork[layerIndex];

                    int x = width / 2 - (layer.Count * 30) / 2;

                    foreach (Perceptron p in layer)
                    {
                        g.FillRectangle(Brushes.DarkGray, x, y, 20, 20);

                        int x2 = width / 2 - (p.weights.Length * 30) / 2;
                        for (int i = 0; i < p.weights.Length; i++)
                        {
                            if (layerIndex > 0)
                            {
                                if (p.weights[i] < 0) g.DrawLine(Pens.Black, x + 10, y, x2 + (i * 30) + 10, y - 25);
                                else g.DrawLine(Pens.White, x + 10, y, x2 + (i * 30) + 10, y - 25);
                            }
                            else
                            {
                                g.DrawLine(Pens.Black, x + 10, y + 20, x2 + (i * 30) + 10, y + 40);
                            }
                            g.DrawString(Math.Round(p.weights[0], 1).ToString(), fontSmall, Brushes.Black, x, y);
                        }

                        x += 30;
                    }

                    y += 40;
                }
            }
        }

        //call physics engine
        private void physicsTimer_Tick(object sender, EventArgs e)
        {
            if (gameState.Equals("game"))
            {
                simulatePhysics();
            }
        }

        //simulate physics
        private void simulatePhysics()
        {
            //add more food
            if (food.Count < 3500)
            {
                for (int i = 0; i < r.Next(50, 75); i++)
                {
                    food.Add(new Food(r.Next(0, canvas.Width), r.Next(0, canvas.Height)));
                }
            }

            //simulate creatures
            List<int> creaturesToRemove = new List<int>();
            List<Creature> creaturesToAdd = new List<Creature>();

            foreach (Creature c in creatures)
            {
                Boolean remove = c.SimulateCreature(canvas.Width, canvas.Height, ref creaturesToAdd, ref creaturesToRemove);

                if (remove) creaturesToRemove.Add(creatures.IndexOf(c));
            }
            
            //remove dead creatures
            creaturesToRemove.Sort();
            creaturesToRemove.Reverse();

            Creature oldCreatureSelected = creatures[0];
            if (selectedCreature <= creatures.Count && selectedCreature != -1) oldCreatureSelected = creatures[selectedCreature];

            foreach (int index in creaturesToRemove)
            {
                if (index < creatures.Count)
                {
                    creatures.RemoveAt(index);
                }
            }

            selectedCreature = creatures.IndexOf(oldCreatureSelected);

            //add new creatures
            foreach (Creature c in creaturesToAdd)
            {
                creatures.Add(c);
            }

            //start new game if neccesary
            if (creatures.Count == 0)
            {
                startNewGame();
            }
        }

        //canvas mouse down handler
        private void canvas_MouseDown(object sender, MouseEventArgs e)
        {
            mouse.RegisterMouseDown(e.X, e.Y, e.Button);
        }

        //canvas mouse up handler
        private void canvas_MouseUp(object sender, MouseEventArgs e)
        {
            mouse.RegisterMouseUp(e.X, e.Y, e.Button);
        }
    }
}
