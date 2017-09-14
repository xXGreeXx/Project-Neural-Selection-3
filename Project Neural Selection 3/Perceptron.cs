﻿using System;


namespace Project_Neural_Selection_3
{
    public class Perceptron
    {
        //define global variables
        public float[] weights;
        public int outputs;

        //constructor
        public Perceptron(int inputs, int outputs)
        {
            weights = new float[inputs];

            for (int i = 0; i < weights.Length; i++)
            {
                weights[i] = (float)Game.r.NextDouble() * Game.r.Next(-1, 2);
            }

            this.outputs = outputs;
        }

        //define output
        public float output(float[] inputs)
        {
            float answer = 0;

            for (int i = 0; i < weights.Length; i++)
            {
                answer += (inputs[i] * weights[i]);
            }

            return activation(answer);
        }

        //define train
        public void train(float[] inputs, float target)
        {
            float returnValue = output(inputs);
            float error = target - returnValue;

            for (int i = 0; i < inputs.Length; i++)
            {
                weights[i] += error * inputs[i] * Game.learningRate;
            }
        }

        //define activation function
        public float activation(float input)
        {
            float output = 0;

            if (input < 0) output = -1;
            else output = 1;

            return output;
        }
    }
}
