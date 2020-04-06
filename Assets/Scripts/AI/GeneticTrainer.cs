using UnityEngine;
using System;
using System.Collections.Generic;

public class GeneticTrainer : MonoBehaviour
{
	[SerializeField]
	[Range(5, 25)]
	uint mAInum = 20u;

	public uint maxLayerNum = 5u;
	public uint maxNeuronPerLayer = 20u;

	[SerializeField]
	GameObject aiTrainerPrefab = null;

	AITrainer[] trainers;
	List<KeyValuePair<float, NeuralNetwork>> scoreboard = new List<KeyValuePair<float, NeuralNetwork>>();

	uint[] cpValid = null;

	uint bestIndex;

	[SerializeField]
	float mAIGarbageTime = 60.0f;
	float mCurrResetTime = 0.0f;

	public static GeneticTrainer Instance { get; private set; }

	public static eRaceState raceState
	{
		get;
		private set;
	}

	void Awake()
	{
		Instance = this;
		raceState = eRaceState.Waiting;

		trainers = new AITrainer[mAInum];
		cpValid = new uint[mAInum];

		mCurrResetTime = mAIGarbageTime;

		trainers[0] = Instantiate(aiTrainerPrefab, RaceMgr.Instance.playerStarts[0].position, RaceMgr.Instance.playerStarts[0].rotation).GetComponent<AITrainer>();
		trainers[0].Provide(AIDirector.Brain);

		// only keep the 25% best of the colony for a new generation.
		bestIndex = (uint)Mathf.Ceil(0.25f * trainers.Length);

		for (uint i = 1; i < mAInum; i++)
			trainers[i] = Instantiate(aiTrainerPrefab, RaceMgr.Instance.playerStarts[0].position, RaceMgr.Instance.playerStarts[0].rotation).GetComponent<AITrainer>();

		raceState = eRaceState.Started;
	}

	void Update()
	{
		// Reset garbage AI.
		if ((mCurrResetTime -= Time.deltaTime) <= 0.0f)
		{
			foreach(var trainer in trainers)
			{
				// no checkpoint validated.
				if (cpValid[trainer.ID] == 0)
					trainer.Generate();
			}

			mCurrResetTime = mAIGarbageTime;
		}
	}

	void OnDestroy()
	{
		Instance = null;

		foreach (var trainer in trainers)
		{
			if(trainer)
				Destroy(trainer.gameObject);
		}

		GUIMgr.Instance.stopBt.onClick.RemoveListener(() => Destroy(gameObject));
	}

	void AddScoreBoard(AITrainer trainer, float time)
	{
		trainer.isRunning = false;
		scoreboard.Add(new KeyValuePair<float, NeuralNetwork>(time, trainer.brain));

		if (scoreboard.Count == bestIndex)
			NewGeneration();
	}

	int KeyComparer(KeyValuePair<float, NeuralNetwork> lhs, KeyValuePair<float, NeuralNetwork> rhs)
	{
		if (lhs.Key < rhs.Key)
			return -1;

		if (lhs.Key > rhs.Key)
			return 1;

		return 0;
	}

	void NewGeneration()
	{
		Debug.Log("=== Generation ===");

		raceState = eRaceState.Finished;

		// Stop garbage AI reset.
		mCurrResetTime = float.MaxValue;

		// Sort by key.
		scoreboard.Sort(KeyComparer);

		// Set current best Brain to AIDirector.
		AIDirector.Brain = scoreboard[0].Value;

		//Reset all CPs.
		Array.Clear(cpValid, 0, cpValid.Length);

		uint currIndex = 0u;

		// Keep best.
		Debug.Log("ScoreBoard:");

		for (; currIndex < bestIndex; currIndex++)
		{
			Debug.Log("Time: " + scoreboard[(int)currIndex].Key + 's');
			trainers[currIndex].brain = scoreboard[(int)currIndex].Value;
		}

		// Breed.
		for (int i = 0; i < bestIndex; i++)
		{
			for (int j = i + 1; j < bestIndex; j++)
				trainers[currIndex++].Generate(scoreboard[i].Value, scoreboard[j].Value);

			trainers[i].isRunning = true;
		}

		Debug.Log("Child: " + (currIndex - bestIndex));
		Debug.Log("Regen: " + (trainers.Length - currIndex));

		// Regen.
		for (; currIndex < trainers.Length; currIndex++)
			trainers[currIndex].Generate();

		// Reset scoredboard.
		scoreboard.Clear();

		raceState = eRaceState.Started;

		// Start garbage AI reset.
		mCurrResetTime = mAIGarbageTime;
	}

	public void OnValidateCheckpoint(Checkpoint cp, AITrainer trainer)
	{
		if (cpValid[trainer.ID] == cp.ID)
		{
			cpValid[trainer.ID]++;

			if (cpValid[trainer.ID] == RaceMgr.Instance.checkpoints.Length)
				AddScoreBoard(trainer, trainer.GetTime());
		}
	}
	public Checkpoint GetNextCheckpoint(AITrainer trainer)
	{
		return RaceMgr.Instance.checkpoints[cpValid[trainer.ID]];
	}
}