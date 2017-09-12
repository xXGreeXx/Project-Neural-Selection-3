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
                float rotationX = (float)(Math.Cos(c.rotation) * Game.creatureSize);
                float rotationY = (float)(Math.Cos(c.rotation) * Game.creatureSize);

                float baseX = c.x;
                float baseY = c.y;
                Brush brush = new SolidBrush(c.color);

                float sizeX = Game.creatureSize;
                float sizeY = Game.creatureSize;

                g.FillEllipse(brush, baseX, baseY, sizeX, sizeY);

                int index = 0;
                foreach (int[] position in c.locationOfInputOnCreature)
                {
                    if (c.inputs[index] == Creature.CreatureInputs.Eye)
                    {
                        g.FillEllipse(Brushes.White, baseX + position[0], baseY + position[1], 3, 3);
                    }

                    index++;
                }

                if (Game.creatures.IndexOf(c) == Game.selectedCreature)
                {
                    g.DrawRectangle(Pens.White, baseX, baseY, sizeX, sizeY);
                }

                if (MouseHandler.down)
                {
                    if (MouseHandler.x >= baseX && MouseHandler.x <= baseX + sizeX)
                    {
                        if (MouseHandler.y >= baseY && MouseHandler.y <= baseY + sizeY)
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
