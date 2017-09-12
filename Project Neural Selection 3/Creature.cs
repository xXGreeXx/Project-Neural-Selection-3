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
        public List<CreatureInputs> inputs = new List<CreatureInputs>();
        public List<int[]> locationOfInputOnCreature = new List<int[]>();
        public Color color;
        public int rotation = 0;
        public List<Perceptron>[] neuralNetwork;

        //enums
        public enum CreatureInputs
        {
            Eye
        }

        //constructor
        public Creature(int x, int y, List<CreatureInputs> inputs, List<int[]> locationOfInputOnCreature, Color color)
        {
            this.x = x;
            this.y = y;
            this.inputs = inputs;
            this.locationOfInputOnCreature = locationOfInputOnCreature;
            this.color = color;

            //create neural network
            neuralNetwork = new List<Perceptron>[4];
            neuralNetwork[0] = new List<Perceptron>();
            neuralNetwork[1] = new List<Perceptron>();
            neuralNetwork[2] = new List<Perceptron>();
            neuralNetwork[3] = new List<Perceptron>();

            int baseLayer = inputs.Count;
            for (int i = 0; i < baseLayer; i++)
            {
                neuralNetwork[0].Add(new Perceptron(baseLayer, baseLayer * 2));
            }

            for (int i = 0; i < baseLayer * 2; i++)
            {
                neuralNetwork[1].Add(new Perceptron(baseLayer, baseLayer * 2));
                neuralNetwork[2].Add(new Perceptron(baseLayer * 2, 3));
            }

            neuralNetwork[3].Add(new Perceptron(baseLayer * 2, 1));
            neuralNetwork[3].Add(new Perceptron(baseLayer * 2, 1));
            neuralNetwork[3].Add(new Perceptron(baseLayer * 2, 1));
        }

        //simulate creature
        public Boolean SimulateCreature()
        {
            Boolean remove = false;

            //get sensory input
            List<int> sensoryInput = new List<int>();
            foreach (CreatureInputs input in inputs)
            {
                if (input == CreatureInputs.Eye)
                {
                    
                }
            }

            //simulate neural network
            

            //train neural network


            //move creaure based on output from neural network

            
            //handle hitbox with food/other creautres


            //return wheather or not to remove creature
            return remove;
        }


        private Color getInputFromEye(int xOfEye, int yOfEye)
        {
            Color c = Color.Red;


            return c;
        }
    }
}
