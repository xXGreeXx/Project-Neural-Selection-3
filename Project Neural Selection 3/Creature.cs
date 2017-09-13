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
        public List<int> rotationOfInput = new List<int>();
        public Color color;
        public float rotation = 0;
        public List<Perceptron>[] neuralNetwork;

        public int food { get; set; } = 50;

        //enums
        public enum CreatureInputs
        {
            Eye
        }

        //constructor
        public Creature(int x, int y, List<CreatureInputs> inputs, List<int> rotationOfInput, Color color)
        {
            this.x = x;
            this.y = y;
            this.inputs = inputs;
            this.rotationOfInput = rotationOfInput;
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
        public Boolean SimulateCreature(int width, int height)
        {
            int target = 1;

            //creature needs
            food--;
            if (food <= 0)
            {
                return true;
            }

            //get sensory input
            List<int> sensoryInput = new List<int>();
            foreach (CreatureInputs input in inputs)
            {
                if (input == CreatureInputs.Eye)
                {
                    int eyeX = 0;
                    int eyeY = 0;

                    sensoryInput.Add(getInputFromEye(eyeX, eyeY).ToArgb());
                }
            }

            //simulate neural network
            float[] finalOutputs = new float[neuralNetwork[neuralNetwork.Length - 1].Count];

            float[] outputsFromLastLayer = new float[sensoryInput.Count];
            for (int i = 0; i < sensoryInput.Count; i++)
            {
                outputsFromLastLayer[i] = sensoryInput[i];
            }

            foreach (List<Perceptron> layer in neuralNetwork)
            {
                float[] outputsFromLastLayerBuffer = new float[layer.Count];

                for (int index = 0; index < layer.Count; index++)
                {
                    Perceptron p = layer[index];

                    float returnValue = p.output(outputsFromLastLayer);

                    outputsFromLastLayerBuffer[index] = returnValue;
                }

                outputsFromLastLayer = outputsFromLastLayerBuffer;
            }

            finalOutputs = outputsFromLastLayer;

            //move creaure based on output from neural network
            int rotateLeft = (int)finalOutputs[0];
            int move = (int)finalOutputs[1];
            int rotateRight = (int)finalOutputs[2];
            
            if (rotateLeft == 1)
            {
                rotation -= 3;
            }
            if(rotateRight == 1)
            {
                rotation += 3;
            }
            if (move == 1)
            {
                float rotationX = (float)Math.Cos(rotation);
                float rotationY = (float)Math.Sin(rotation);

                x += Game.creatureSpeed * rotationX;
                y += Game.creatureSpeed * rotationY;
            }

            //handle hitbox with food/other creautres/walls
            RectangleF creatureHitbox = new RectangleF(x, y, Game.creatureSize, Game.creatureSize);

            //food
            List<int> foodToRemove = new List<int>();

            foreach (Food f in Game.food)
            {
                RectangleF foodHitbox = new RectangleF(f.x, f.y, 3, 3);

                if (creatureHitbox.IntersectsWith(foodHitbox))
                {
                    target = 1;
                    food += 15;
                    foodToRemove.Add(Game.food.IndexOf(f));
                }
                else
                {
                    target = -1;
                }
            }

            foodToRemove.Sort();
            foodToRemove.Reverse();

            foreach (int index in foodToRemove)
            {
                Game.food.RemoveAt(index);
            }

            //wall
            if (x <= 0) x = 0;
            if (x >= width - Game.creatureSize) x = width - Game.creatureSize;
            if (y <= 0) y = 0;
            if (y >= height - Game.creatureSize) y = height - Game.creatureSize;

            //train neural network
            outputsFromLastLayer = new float[sensoryInput.Count];
            for (int i = 0; i < sensoryInput.Count; i++)
            {
                outputsFromLastLayer[i] = 1;
            }

            foreach (List<Perceptron> layer in neuralNetwork)
            {
                float[] outputsFromLastLayerBuffer = new float[layer.Count];

                for (int index = 0; index < layer.Count; index++)
                {
                    Perceptron p = layer[index];

                    float returnValue = p.output(outputsFromLastLayer);
                    p.train(outputsFromLastLayer, target);

                    outputsFromLastLayerBuffer[index] = returnValue;
                }

                outputsFromLastLayer = outputsFromLastLayerBuffer;
            }

            //return wheather or not to remove creature
            return false;
        }


        private Color getInputFromEye(int xOfEye, int yOfEye)
        {
            Color c = Color.Red;


            return c;
        }
    }
}
