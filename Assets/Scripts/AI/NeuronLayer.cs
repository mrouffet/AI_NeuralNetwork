using System;
using UnityEngine;

[Serializable]
public class NeuronLayer
{
	[SerializeField]
	Perceptron[] mNeurons;

	public uint neuronNum { get { return (uint)mNeurons.Length; } }

	public uint inputNum { get { return (uint)mNeurons[0].weights.Length; } }

	public uint Init(uint inputNum)
	{
		foreach (Perceptron perc in mNeurons)
			perc.Init(inputNum);

		return neuronNum;
	}

	public float[] Compute(float[] inputs)
	{
		float[] res = new float[neuronNum];

		for (uint i = 0u; i < neuronNum; i++)
			res[i] = mNeurons[i].Compute(inputs);

		return res;
	}

	public float[] BackPropage(float[] inputs, float[] deltas)
	{
		// Called as output Layer.

		for (uint i = 0; i < neuronNum; i++)
			mNeurons[i].Balance(deltas[i], inputs);

		return deltas;
	}
	public float[] BackPropage(float[] inputs, float[] outputs, float[] prevDeltas, NeuronLayer nextLayer)
	{
		// Compute deltas.
		float[] deltas = new float[neuronNum];

		for (uint i = 0u; i < neuronNum; i++)
		{
			for(uint j = 0u; j < nextLayer.neuronNum; j++)
				deltas[i] += prevDeltas[j] * nextLayer.mNeurons[j].weights[i];

			deltas[i] *= outputs[i] * (1.0f - outputs[i]);
		}
		//

		for (uint i = 0u; i < neuronNum; i++)
			mNeurons[i].Balance(deltas[i], inputs);

		return deltas;
	}

	static public NeuronLayer Generate(uint neuronNum)
	{
		NeuronLayer layer = new NeuronLayer();

		layer.mNeurons = new Perceptron[neuronNum];

		for (uint i = 0u; i < neuronNum; i++)
			layer.mNeurons[i] = new Perceptron();

		return layer;
	}
	static public NeuronLayer Generate(uint inputNum, NeuronLayer mom, NeuronLayer dad)
	{
		NeuronLayer layer = new NeuronLayer();

		// Mom is always the reference here.
		if (dad == null)
		{
			// Neuron layer from mom only. if same inputNum: copy all; else copy + fill with random.
			if (inputNum == mom.inputNum)
				layer.mNeurons = mom.mNeurons.Clone() as Perceptron[];
			else
			{
				layer.mNeurons = new Perceptron[mom.neuronNum];

				for (uint i = 0u; i < layer.neuronNum; i++)
					layer.mNeurons[i] = Perceptron.Generate(inputNum, mom.mNeurons[i]);
			}
		}
		else
		{
			layer.mNeurons = new Perceptron[dad.neuronNum];

			for (uint i = 0u; i < layer.neuronNum; i++)
				layer.mNeurons[i] = Perceptron.Generate(inputNum, mom.neuronNum > i ? mom.mNeurons[i] : null, dad.mNeurons[i]);
		}

		return layer;
	}
}