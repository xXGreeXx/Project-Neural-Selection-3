using System;


namespace Project_Neural_Selection_3
{
    public class Perceptron
    {
        //define global variables
        float[] weights;
        int outputs;

        //constructor
        public Perceptron(int inputs, int outputs)
        {
            weights = new float[inputs];

            for (int i = 0; i < weights.Length; i++)
            {
                weights[i] = Game.r.Next(-3, 3);
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

            return answer;
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
