using System;
using System.Collections.Generic;
using System.Drawing;

namespace Project_Neural_Selection_3
{
    public class RenderingEngine
    {
        //define global variables
        private List<Particle> particles = new List<Particle>();
        private Bitmap backgroundBase = Project_Neural_Selection_3.Properties.Resources.baseCoreEngine;

        //constructor
        public RenderingEngine()
        {
            int amountOfParticles = backgroundBase.Width * backgroundBase.Height;

            int x = 0;
            int y = 0;
            for (int i = 0; i < amountOfParticles; i++)
            {
                Color c = backgroundBase.GetPixel(x, y);
                if (!c.Equals(Color.FromArgb(255, 255, 255, 255)))
                {
                    particles.Add(new Particle(x, y, c, 20));
                }

                x++;
                if (x >= backgroundBase.Width)
                {
                    x = 0;
                    y++;

                }
            }

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

                if (c.red) brush = new SolidBrush(Color.Red);

                g.FillEllipse(brush, baseX, baseY, Game.creatureSize, Game.creatureSize);

                int index = 0;
                foreach (int rotation in c.rotationOfInput)
                {
                    float selfRotationX = (float)(Math.Cos(rotation + c.rotation) * Game.creatureSize / 2) + Game.creatureSize / 2;
                    float selfRotationY = (float)(Math.Sin(rotation + c.rotation) * Game.creatureSize / 2) + Game.creatureSize / 2;

                    if (c.inputs[index] == Creature.CreatureInputs.Eye)
                    {
                        g.FillEllipse(Brushes.White, baseX + selfRotationX, baseY + selfRotationY, 3, 3);
                    }

                    index++;
                }

                if (Game.creatures.IndexOf(c) == Game.selectedCreature)
                {
                    g.DrawRectangle(Pens.White, baseX, baseY, Game.creatureSize, Game.creatureSize);
                }

                if (MouseHandler.down)
                {
                    if (MouseHandler.x >= baseX && MouseHandler.x <= baseX + Game.creatureSize)
                    {
                        if (MouseHandler.y >= baseY && MouseHandler.y <= baseY + Game.creatureSize)
                        {
                            Game.selectedCreature = Game.creatures.IndexOf(c);
                        }
                    }
                }
            }

            //draw food
            foreach (Food food in Game.food)
            {
                g.FillRectangle(Brushes.Green, food.x, food.y, 3, 3);
            }
        }

        //draw splash screen
        public void SplashScreen(Graphics g, int width,  int height)
        {
            int baseX = width / 2 - backgroundBase.Width / 2;
            int baseY = height / 2 - backgroundBase.Height / 2;

            List<int> particlesToRemove = new List<int>();

            int i = 0;
            foreach (Particle p in particles)
            {
                g.FillRectangle(new SolidBrush(p.color), baseX + p.x, baseY + p.y, 1, 1);

                if (p.life <= 10)
                {
                    p.x += (int)(Math.Cos(i) * 2.5) + Game.r.Next(-4, 5);
                    p.y += (int)(Math.Sin(i) * 2.5) + Game.r.Next(-4, 5);
                }

                int greenShift = 2;
                if (p.color.G - greenShift >= 0)
                {
                    p.color = Color.FromArgb(255, p.color.R, p.color.G - greenShift, p.color.B);
                }

                p.life -= Game.r.Next(0, 2);
                if (p.life <= 0)
                {
                    particlesToRemove.Add(particles.IndexOf(p));
                }

                i++;
            }

            particlesToRemove.Sort();
            particlesToRemove.Reverse();

            foreach (int index in particlesToRemove)
            {
                particles.RemoveAt(index);
            }

            if (particles.Count == 0)
            {
                Game.gameState = "game";
            }
        }
    }

    internal class Particle
    {
        //define global variables
        public int x { get; set; }
        public int y { get; set; }
        public Color color { get; set; }
        public int life { get; set; }

        //constructor
        public Particle(int x, int y, Color color, int life)
        {
            this.x = x;
            this.y = y;
            this.color = color;
            this.life = life;
        }
    }
}
