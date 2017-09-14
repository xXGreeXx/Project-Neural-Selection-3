using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

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
        public int age { get; set; } = 0;
        public int reproductionValue { get; set; } = 0;
        public int strength { get; set; }

        //enums
        public enum CreatureInputs
        {
            Eye
        }

        //constructor
        public Creature(float x, float y, List<CreatureInputs> inputs, List<int> rotationOfInput, Color color)
        {
            this.x = x;
            this.y = y;
            this.inputs = inputs;
            this.rotationOfInput = rotationOfInput;
            this.color = color;
            this.strength = Game.r.Next(10, 20);

            //create neural network
            neuralNetwork = new List<Perceptron>[6];
            neuralNetwork[0] = new List<Perceptron>();
            neuralNetwork[1] = new List<Perceptron>();
            neuralNetwork[2] = new List<Perceptron>();
            neuralNetwork[3] = new List<Perceptron>();
            neuralNetwork[4] = new List<Perceptron>();
            neuralNetwork[5] = new List<Perceptron>();

            int baseLayer = inputs.Count;
            for (int i = 0; i < baseLayer; i++)
            {
                neuralNetwork[0].Add(new Perceptron(baseLayer, baseLayer * 2));
            }

            for (int i = 0; i < baseLayer * 2; i++)
            {
                neuralNetwork[1].Add(new Perceptron(baseLayer, baseLayer * 2 + 1));
                neuralNetwork[4].Add(new Perceptron(baseLayer * 2 + 1, 3));
            }

            for (int i = 0; i < baseLayer * 2 + 1; i++)
            {
                neuralNetwork[2].Add(new Perceptron(baseLayer * 2, baseLayer * 2 + 1));
                neuralNetwork[3].Add(new Perceptron(baseLayer * 2 + 1, baseLayer * 2));
            }

            neuralNetwork[5].Add(new Perceptron(baseLayer * 2, 1));
            neuralNetwork[5].Add(new Perceptron(baseLayer * 2, 1));
            neuralNetwork[5].Add(new Perceptron(baseLayer * 2, 1));
        }

        //simulate creature
        public Boolean SimulateCreature(int width, int height, ref List<Creature> creaturesToAdd, ref List<int> creaturesToRemove)
        {
            float target = 1;

            //creature needs
            food--;
            if (food <= 0) return true;

            age += Game.r.Next(0, 2);
            if (age >= 100) return true;

            reproductionValue++;

            //mitosis
            if (food >= 150 && reproductionValue >= 50)
            {
                food /= 2;
                reproductionValue = 0;

                Creature copy = new Creature(x - Game.creatureSize, y - Game.creatureSize, inputs, rotationOfInput, color);

                copy.rotationOfInput = new List<int>();

                for (int i2 = 0; i2 < copy.inputs.Count; i2++)
                {
                    copy.rotationOfInput.Add(Game.r.Next(0, 361));
                }

                copy.food = 20;

                for (int layerIndex = 0; layerIndex < copy.neuralNetwork.Length; layerIndex++)
                {
                    List<Perceptron> layer = copy.neuralNetwork[layerIndex];

                    for (int perceptronIndex = 0; perceptronIndex < layer.Count; perceptronIndex++)
                    {
                        for (int weightIndex = 0; weightIndex < layer[perceptronIndex].weights.Length; weightIndex++)
                        {
                            int number = Game.r.Next(1, 101);

                            if (number <= Game.creatureMutationRate)
                            {
                                copy.neuralNetwork[layerIndex][perceptronIndex].weights[weightIndex] = (float)Game.r.NextDouble() * Game.r.Next(-1, 2);
                            }
                            else
                            {
                                copy.neuralNetwork[layerIndex][perceptronIndex].weights[weightIndex] = neuralNetwork[layerIndex][perceptronIndex].weights[weightIndex];
                            }
                        }
                    }
                } 

                creaturesToAdd.Add(copy);
            }

            //get sensory input
            List<int> sensoryInput = new List<int>();
            int indexOfInput = 0;

            foreach (CreatureInputs input in inputs)
            {
                float selfRotationX = (float)(Math.Cos(rotation + rotationOfInput[indexOfInput]) * Game.creatureSize / 2) + Game.creatureSize / 2;
                float selfRotationY = (float)(Math.Sin(rotation + rotationOfInput[indexOfInput]) * Game.creatureSize / 2) + Game.creatureSize / 2;
                float inputX = x + selfRotationX;
                float inputY = y + selfRotationY;

                if (input == CreatureInputs.Eye)
                {
                    Color c = getInputFromEye(inputX, inputY, rotationOfInput[inputs.IndexOf(input)]);

                    sensoryInput.Add(c.ToArgb());
                }

                indexOfInput++;
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

                x -= Game.creatureSpeed * rotationX;
                y -= Game.creatureSpeed * rotationY;
            }

            //handle hitbox with food/other creautres/walls
            RectangleF creatureHitbox = new RectangleF(x, y, Game.creatureSize, Game.creatureSize);

            //food
            List<int> foodToRemove = new List<int>();
            Boolean gotFood = false;
            foreach (Food f in Game.food)
            {
                RectangleF foodHitbox = new RectangleF(f.x, f.y, 3, 3);

                if (creatureHitbox.IntersectsWith(foodHitbox))
                {
                    gotFood = true;
                    food += 10;
                    foodToRemove.Add(Game.food.IndexOf(f));
                }
            }

            if (!gotFood) target = -1;

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

            //other creatures
            foreach (Creature c in Game.creatures)
            {
                RectangleF creature2Hitbox = new RectangleF(c.x, c.y, Game.creatureSize, Game.creatureSize);

                if (creatureHitbox.IntersectsWith(creature2Hitbox) && c != this && c.color != color)
                {
                    if (c.strength < strength)
                    {
                        creaturesToRemove.Add(Game.creatures.IndexOf(c));
                        food -= c.strength;
                        break;
                    }
                    else
                    {
                        c.food -= strength;
                        return true;
                    }
                }
            }

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

        //get sensory inputs
        private Color getInputFromEye(float xOfEye, float yOfEye, int rotationOfEye)
        {
            Color colorToReturn = Color.White;

            float selfRotationX = (float)(Math.Cos(rotation + rotationOfEye) * Game.creatureSize / 2) + Game.creatureSize / 2;
            float selfRotationY = (float)(Math.Sin(rotation + rotationOfEye) * Game.creatureSize / 2) + Game.creatureSize / 2;

            //define lists of objects/colors to check
            List<Rectangle> objects = new List<Rectangle>();
            List<Color> matchingColors = new List<Color>();

            //add food to objects
            foreach(Food f in Game.food) { objects.Add(new Rectangle(f.x, f.y, 3, 3)); matchingColors.Add(Color.Green); }
            foreach (Creature c in Game.creatures) { objects.Add(new Rectangle((int)c.x, (int)c.y, 3, 3)); matchingColors.Add(c.color); }

            int index = 0;
            foreach (Rectangle hitbox in sortRectanglesAroundPoint(objects, new Point((int)xOfEye, (int)yOfEye)))
            {
                float rotationX = (float)Math.Cos(selfRotationX) * 10 * Game.creatureSize / 2;
                float rotationY = (float)Math.Sin(selfRotationY) * 10 * Game.creatureSize / 2;

                Point baseOfRay = new Point((int)x + (int)selfRotationX, (int)y + (int)selfRotationY);
                Point endOfRay = new Point((int)(x + selfRotationX + rotationX), (int)(y + selfRotationY + rotationY));

                Boolean linePassedObject = LineIntersectsRect(baseOfRay, endOfRay, hitbox);

                if (linePassedObject)
                {
                    colorToReturn = matchingColors[index];
                    break;
                }

                index++;
            }

            return colorToReturn;
        }

        //sort rectangle closest to point
        private List<Rectangle> sortRectanglesAroundPoint(List<Rectangle> baseListOfRectangles, Point basePoint)
        {
            return baseListOfRectangles.OrderBy(r => r.X - basePoint.X).ThenBy(r => r.Y - basePoint.Y).ToList();
        }

        //line intersects rectangle
        public static bool LineIntersectsRect(Point p1, Point p2, Rectangle r)
        {
            return LineIntersectsLine(p1, p2, new Point(r.X, r.Y), new Point(r.X + r.Width, r.Y)) ||
                   LineIntersectsLine(p1, p2, new Point(r.X + r.Width, r.Y), new Point(r.X + r.Width, r.Y + r.Height)) ||
                   LineIntersectsLine(p1, p2, new Point(r.X + r.Width, r.Y + r.Height), new Point(r.X, r.Y + r.Height)) ||
                   LineIntersectsLine(p1, p2, new Point(r.X, r.Y + r.Height), new Point(r.X, r.Y)) ||
                   (r.Contains(p1) && r.Contains(p2));
        }

        //line intersects line
        private static bool LineIntersectsLine(Point l1p1, Point l1p2, Point l2p1, Point l2p2)
        {
            float q = (l1p1.Y - l2p1.Y) * (l2p2.X - l2p1.X) - (l1p1.X - l2p1.X) * (l2p2.Y - l2p1.Y);
            float d = (l1p2.X - l1p1.X) * (l2p2.Y - l2p1.Y) - (l1p2.Y - l1p1.Y) * (l2p2.X - l2p1.X);

            if (d == 0)
            {
                return false;
            }

            float r = q / d;

            q = (l1p1.Y - l2p1.Y) * (l1p2.X - l1p1.X) - (l1p1.X - l2p1.X) * (l1p2.Y - l1p1.Y);
            float s = q / d;

            if (r < 0 || r > 1 || s < 0 || s > 1)
            {
                return false;
            }

            return true;
        }
    }
}
