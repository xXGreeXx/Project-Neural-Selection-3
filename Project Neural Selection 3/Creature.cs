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

        //enums
        public enum CreatureInputs
        {
            Eye
        }

        //constructor
        public Creature()
        {

        }
    }
}
