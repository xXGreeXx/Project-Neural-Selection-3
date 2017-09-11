using System;
using System.Collections.Generic;
using System.Drawing;

namespace Project_Neural_Selection_3
{
    public class Creature
    {
        //define global variables
        public float x;
        public float y;
        public List<RectangleF> shape = new List<RectangleF>();
        public List<CreatureInputs> inputs = new List<CreatureInputs>();
        public Color color;
        public int rotation = 0;

        //enums
        public enum CreatureInputs
        {
            Eye
        }

        //constructor
        public Creature(int x, int y, List<RectangleF> shape, List<CreatureInputs> inputs, Color color)
        {
            this.x = x;
            this.y = y;
            this.shape = shape;
            this.inputs = inputs;
            this.color = color;
        }
    }
}
