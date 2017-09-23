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
        public float age { get; set; } = 0;
        public int reproductionValue { get; set; } = 0;
        public int strength { get; set; }
        public int health { get; set; } = 100;

        public Boolean red { get; set; } = false;

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
            neuralNetwork = new List<Perceptron>[3];
            neuralNetwork[0] = new List<Perceptron>();
            neuralNetwork[1] = new List<Perceptron>();
            neuralNetwork[2] = new List<Perceptron>();

            int baseLayer = inputs.Count + 2;
            for (int i = 0; i < baseLayer; i++)
            {
                neuralNetwork[0].Add(new Perceptron(baseLayer, baseLayer * 2));
            }

            for (int i = 0; i < baseLayer * 2; i++)
            {
                neuralNetwork[1].Add(new Perceptron(baseLayer, baseLayer * 2 + 1));
            }

            neuralNetwork[2].Add(new Perceptron(baseLayer * 2, 1));
            neuralNetwork[2].Add(new Perceptron(baseLayer * 2, 1));
            neuralNetwork[2].Add(new Perceptron(baseLayer * 2, 1));
            neuralNetwork[2].Add(new Perceptron(baseLayer * 2, 1));
        }

        //simulate creature
        public Boolean SimulateCreature(int width, int height, ref List<Creature> creaturesToAdd, ref List<int> creaturesToRemove)
        {
            float target = 1;

            //creature needs
            if (food > 0) food--;
            if (food <= 0) health--; target -= 0.1F;

            age += 0.5F;
            if (age >= health + (food / strength)) return true;

            if (health < 0) return true;
            if (food >= 50 && health < 100) health++;

            reproductionValue++;

            //mitosis
            if (food >= 150 && reproductionValue >= 25)
            {
                food /= 2;
                reproductionValue = 0;

                Creature copy = new Creature(x - Game.creatureSize, y - Game.creatureSize, inputs, rotationOfInput, color);

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

                target += 0.1F;
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

                    sensoryInput.Add(c.R);
                }

                indexOfInput++;
            }

            //simulate neural network
            float[] finalOutputs = new float[neuralNetwork[neuralNetwork.Length - 1].Count];

            float[] outputsFromLastLayer = new float[sensoryInput.Count + 2];
            for (int i = 0; i < sensoryInput.Count; i++)
            {
                outputsFromLastLayer[i] = sensoryInput[i];
            }

            outputsFromLastLayer[sensoryInput.Count] = health;
            outputsFromLastLayer[sensoryInput.Count + 1] = food;

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
            int blinkRed = (int)finalOutputs[3];

            if (blinkRed > 0)
            {
                red = true;
            }
            else
            {
                red = false;
            }

            if (rotateLeft > 0)
            {
                rotation -= 3;
            }
            if(rotateRight > 0)
            {
                rotation += 3;
            }
            if (move > 0)
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
                    food += 15;
                    foodToRemove.Add(Game.food.IndexOf(f));
                }
            }

            if (!gotFood) target -= 0.1F;
            
            foodToRemove.Sort();
            foodToRemove.Reverse();

            foreach (int index in foodToRemove)
            {
                Game.food.RemoveAt(index);
            }

            //wall
            if (x <= 0) x = 0; target -= 0.1F;
            if (x >= width - Game.creatureSize) x = width - Game.creatureSize; target -= 0.1F;
            if (y <= 0) y = 0; target -= 0.1F;
            if (y >= height - Game.creatureSize) y = height - Game.creatureSize; target -= 0.1F;

            //other creatures
            foreach (Creature c in Game.creatures)
            {
                RectangleF creature2Hitbox = new RectangleF(c.x, c.y, Game.creatureSize, Game.creatureSize);

                if (creatureHitbox.IntersectsWith(creature2Hitbox) && c != this)
                {
                    if (color != c.color || c.strength != strength)
                    {
                        if (c.strength < strength)
                        {
                            c.health -= strength;
                            target += 0.1F;

                            c.x += (float)(Math.Cos(c.rotation) * Game.creatureSize);
                            c.y += (float)(Math.Sin(c.rotation) * Game.creatureSize);
                            x += (float)(Math.Cos(rotation) * Game.creatureSize);
                            y += (float)(Math.Sin(rotation) * Game.creatureSize);
                        }
                        else
                        {
                            health -= c.strength;
                            target -= 0.1F;

                            x += (float)(Math.Cos(rotation) * Game.creatureSize);
                            y += (float)(Math.Sin(rotation) * Game.creatureSize);
                            c.x += (float)(Math.Cos(c.rotation) * Game.creatureSize);
                            c.y += (float)(Math.Sin(c.rotation) * Game.creatureSize);
                        }
                    }
                    else
                    {
                        //meiosis
                        if (c.reproductionValue >= 30 && reproductionValue >= 30)
                        {
                            c.reproductionValue = 0;
                            reproductionValue = 0;

                            List<CreatureInputs> inputsOfNewCreature = c.inputs;
                            List<int> rotationsOfNewCreature = c.rotationOfInput;

                            Color newColor = Color.FromArgb(255, (color.R + c.color.R) / 2, (color.G + c.color.G) / 2, (color.B + c.color.B) / 2);

                            Creature newCreature = new Creature(c.x + Game.creatureSize / 2, c.y + Game.creatureSize / 2, inputsOfNewCreature, rotationsOfNewCreature, newColor);

                            target += 0.1F;

                            creaturesToAdd.Add(newCreature);
                        }
                    }
                }
            }

            //train neural network
            outputsFromLastLayer = new float[sensoryInput.Count];
            for (int i = 0; i < sensoryInput.Count; i++)
            {
                outputsFromLastLayer[i] = neuralNetwork[0][0].activation(sensoryInput[i]);
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

            //return whether or not to remove creature
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
            foreach (Creature c in Game.creatures) { objects.Add(new Rectangle((int)c.x, (int)c.y, 3, 3)); matchingColors.Add(c.red ? Color.Red : c.color); }

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
