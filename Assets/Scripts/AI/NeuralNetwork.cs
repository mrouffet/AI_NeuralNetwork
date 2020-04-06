using System;
using UnityEngine;

[Serializable]
public class NeuralNetwork
{
	public static float learnRate = 0.5f;

	[SerializeField]
	uint mInputNum = 1;

	[SerializeField]
	NeuronLayer[] mLayers;

	public uint LayerNum { get { return (uint)mLayers.Length; } }

	public uint inputNum { get { return mInputNum; } }
	public uint outputNum { get { return mLayers[LayerNum - 1].neuronNum; } }

	public void Init()
	{
		uint inputNum = mInputNum;

		for (uint i = 0u; i < mLayers.Length; i++)
			inputNum = mLayers[i].Init(inputNum);
	}

	public float[] Compute(float[] inputs)
	{
		float[] res = inputs;

		for (uint i = 0u; i < LayerNum; i++)
			res = mLayers[i].Compute(res);

		return res;
	}

	public void Train(float[] inputs, float[] targetOutputs)
	{
		float[][] layerValues = new float[LayerNum + 1][]; // inputs[] + layerOutputs[].

		layerValues[0] = inputs;

		for (uint i = 0u; i < LayerNum; i++)
			layerValues[i + 1] = mLayers[i].Compute(layerValues[i]);

		// Compute output deltas.
		float[] outDeltas = new float[outputNum];
		float[] outputs = layerValues[LayerNum];

		for (uint i = 0; i < outputNum; i++)
			outDeltas[i] = -1 * (targetOutputs[i] - outputs[i]) * outputs[i] * (1.0f - outputs[i]);
		//

		// OutputLayer.
		float[] deltas = mLayers[LayerNum - 1].BackPropage(layerValues[LayerNum - 1], outDeltas);

		for (uint j = LayerNum - 2; j < uint.MaxValue; j--)
			deltas = mLayers[j].BackPropage(layerValues[j], layerValues[j + 1], deltas, mLayers[j + 1]);
	}

	static public NeuralNetwork Generate(uint inputNum, uint outputNum, uint layerNum, uint maxNeuronPerLayer)
	{
		NeuralNetwork brain = new NeuralNetwork();

		brain.mInputNum = inputNum;
		brain.mLayers = new NeuronLayer[layerNum];

		for (uint i = 0u; i < brain.LayerNum - 1; i++)
			brain.mLayers[i] = NeuronLayer.Generate(Tools.Rand(outputNum, maxNeuronPerLayer + 1));

		brain.mLayers[brain.LayerNum - 1] = NeuronLayer.Generate(outputNum);

		return brain;
	}

	public static NeuralNetwork Breed(NeuralNetwork mom, NeuralNetwork dad)
	{
		NeuralNetwork child = new NeuralNetwork();
		uint inputNum = child.mInputNum = mom.mInputNum; // same InputNum for both.

		bool mustSwap = false;

		// Random layerNum between mom and dad.
		if (mom.LayerNum < dad.LayerNum)
			child.mLayers = new NeuronLayer[Tools.Rand(mom.LayerNum, dad.LayerNum + 1)];
		else if(mom.LayerNum > dad.LayerNum)
			child.mLayers = new NeuronLayer[Tools.Rand(dad.LayerNum, mom.LayerNum + 1)];
		else
		{
			child.mLayers = new NeuronLayer[mom.LayerNum];

			// if same LayerNum: rand swap.
			if (Tools.Rand(0, 2) == 0)
				mustSwap = true;
		}


		// If child get closer from dad layerNum: Swap mom and dad.
		if (!mustSwap)
		{
			uint closeToMom = (uint)Mathf.Abs(child.LayerNum - mom.LayerNum);
			uint closeToDad = (uint)Mathf.Abs(child.LayerNum - dad.LayerNum);

			if (closeToMom > closeToDad)
				mustSwap = true;
			else if(closeToMom == closeToDad) // if middle between mom and dad: mom must have the highest layer num.
				mustSwap = mom.LayerNum < dad.LayerNum;
		}

		// Not else!
		if (mustSwap)
			Tools.Swap(ref mom, ref dad);

		// From here: child inherit layerNum from mom: Take neuronNum per layer from dad.

		// Generate all layer exept the output one: insert mom layer before output layer.
		for (uint i = 0; i < child.LayerNum - 1; i++)
		{
			// Dad provide layer num. Mom does when dad.LayerNum - 1 < child.LayerNum -1 (exclude output layer).

			child.mLayers[i] = NeuronLayer.Generate(inputNum, mom.mLayers[i], dad.LayerNum - 1 > i ? dad.mLayers[i] : null);

			inputNum = child.mLayers[i].neuronNum;
		}

		// Generate output layer.
		child.mLayers[child.LayerNum - 1] = NeuronLayer.Generate(inputNum, mom.mLayers[mom.LayerNum - 1], dad.mLayers[dad.LayerNum - 1]);

		return child;
	}
}