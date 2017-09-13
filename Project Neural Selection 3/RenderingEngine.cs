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
    }
}
