using System;
using UnityEngine;

[Serializable]
public class Perceptron
{
	// NonSerialized break prefab save.


	//[NonSerialized]
	public float bias = 1.0f;

	//[NonSerialized]
	public float[] weights;

	//[NonSerialized]
	public float threshold = 1.0f;

	public void Init(uint inputNum)
	{
		weights = new float[inputNum];

		for (uint i = 0u; i < weights.Length; i++)
			weights[i] = Tools.Rand(0.0f, 1.0f);

		bias = Tools.Rand(0.0f, 1.0f);
	}

	public float Compute(float[] inputs)
	{
		float res = bias;

		for (uint i = 0; i < inputs.Length; i++)
			res += inputs[i] * weights[i];

		if (threshold == 0.0f)
			return res < 0 ? 0 : 1;

		// Sigmoid function.
		return 1.0f / (1 + Mathf.Exp(-res / threshold));
	}

	public void Balance(float delta, float[] inputs)
	{
		bias -= NeuralNetwork.learnRate * delta;

		for (uint i = 0; i < weights.Length; i++)
			weights[i] -= NeuralNetwork.learnRate * delta * inputs[i];
	}

	public static Perceptron Generate(uint inputNum, Perceptron parent)
	{
		Perceptron child = new Perceptron();

		child.bias = parent.bias;
		child.weights = new float[inputNum];

		for(uint i = 0; i < inputNum; i++)
			child.weights[i] = i < parent.weights.Length ? parent.weights[i] : Tools.Rand(0.0f, 1.0f);

		return child;
	}
	public static Perceptron Generate(uint inputNum, Perceptron mom, Perceptron dad)
	{
		Perceptron child = new Perceptron();

		if (mom == null)
		{
			child.bias = dad.bias;
			child.weights = dad.weights.Clone() as float[];
		}
		else
		{
			// Swap: Dad is always the reference here.
			if (mom.weights.Length == inputNum)
				Tools.Swap(ref mom, ref dad);

			child.bias = Tools.Rand(0, 2) == 0 ? mom.bias : dad.bias;

			child.weights = new float[inputNum];

			for (uint i = 0; i < child.weights.Length; i++)
			{
				// if mom can provide
				if (i < mom.weights.Length)
				{
					if (i < dad.weights.Length) // and dad: Random
						child.weights[i] = Tools.Rand(0, 2) == 0 ? mom.weights[i] : dad.weights[i];
					else // not dad: use mom
						child.weights[i] = mom.weights[i];
				}
				else if (i < dad.weights.Length) // dad can provide and not mom: use dad
					child.weights[i] = dad.weights[i];
				else // no one: Random mutation.
					child.weights[i] = Tools.Rand(0.0f, 1.0f);
			}
		}


		return child;
	}
}