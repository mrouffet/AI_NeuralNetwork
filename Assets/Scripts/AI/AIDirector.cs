using UnityEngine;

public class AIDirector : MonoBehaviour
{
	[SerializeField]
	GameObject genTrainerPrefab = null;

	[SerializeField]
	NeuralNetwork mBrain; // Best NN after Training.

	public static AIDirector Instance
	{
		get;
		private set;
	}

	public static NeuralNetwork Brain
	{
		get
		{
			if(Instance)
				return Instance.mBrain;

			return null;
		}

		set
		{
			if (Instance)
				Instance.mBrain = value;
		}
	}

	void Awake()
	{
		Instance = this;

		// Get brain from previous Scene.
		if (BrainSaver.Instance != null)
			mBrain = BrainSaver.Release();
		else
			mBrain.Init();
	}

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.P))
		{
			Debug.Log("Training...");

			TrainingProgram(mBrain);
		}
	}

	void OnDestroy()
	{
		BrainSaver.Keep(mBrain);
	}

	public static void TrainingProgram(NeuralNetwork brain)
	{
		for (uint i = 0; i < 10000u; i++)
			Trainer(brain);
	}

	static void Trainer(NeuralNetwork brain)
	{
		float[] inputs = null;
		float[] targeted = null;

		switch (Tools.Rand(0, 15))
		{
			// All free.
			case 0:
				inputs = new float[] { 50.0f, 1.0f, 1.0f, 1.0f, 1.0f, -1.0f }; // Left
				targeted = new float[] { 1.0f, 0.0f };
				break;
			case 1:
				inputs = new float[] { 1.0f, 50.0f, 1.0f, 1.0f, 1.0f, -0.707f }; // Forward left
				targeted = new float[] { 1.0f, 0.25f };
				break;
			case 2:
				inputs = new float[] { 1.0f, 1.0f, 50.0f, 1.0f, 1.0f, 0.0f }; // Forward
				targeted = new float[] { 1.0f, 0.5f };
				break;
			case 3:
				inputs = new float[] { 1.0f, 1.0f, 1.0f, 50.0f, 1.0f, 0.707f }; // Forward right
				targeted = new float[] { 1.0f, 0.75f };
				break;
			case 4:
				inputs = new float[] { 1.0f, 1.0f, 1.0f, 1.0f, 50.0f, 1.0f }; // Right
				targeted = new float[] { 1.0f, 1.0f };
				break;

			// Free target, all other mid dist.
			case 5:
				inputs = new float[] { 50.0f, 0.5f, 0.5f, 0.5f, 0.5f, -1.0f }; // Left
				targeted = new float[] { 1.0f, 0.0f };
				break;
			case 6:
				inputs = new float[] { 0.5f, 50.0f, 0.5f, 0.5f, 0.5f, -0.707f }; // Forward left
				targeted = new float[] { 1.0f, 0.25f };
				break;
			case 7:
				inputs = new float[] { 0.5f, 0.5f, 50.0f, 0.5f, 0.5f, 0.0f }; // Forward
				targeted = new float[] { 1.0f, 0.5f };
				break;
			case 8:
				inputs = new float[] { 0.5f, 0.5f, 0.5f, 50.0f, 0.5f, 0.707f }; // Forward right
				targeted = new float[] { 1.0f, 0.75f };
				break;
			case 9:
				inputs = new float[] { 0.5f, 0.5f, 0.5f, 0.5f, 50.0f, 1.0f }; // Right
				targeted = new float[] { 1.0f, 1.0f };
				break;

			// Free target, all other obstrued.
			case 10:
				inputs = new float[] { 0.8f, 0.2f, 0.2f, 0.2f, 0.2f, -1.0f }; // Left
				targeted = new float[] { 0.5f, 0.0f };
				break;
			case 11:
				inputs = new float[] { 0.2f, 0.8f, 0.2f, 0.2f, 0.2f, -0.707f }; // Forward left
				targeted = new float[] { 0.5f, 0.25f };
				break;
			case 12:
				inputs = new float[] { 0.2f, 0.2f, 0.8f, 0.2f, 0.2f, 0.0f }; // Forward
				targeted = new float[] { 0.5f, 0.5f };
				break;
			case 13:
				inputs = new float[] { 0.2f, 0.2f, 0.2f, 0.8f, 0.0f, 0.707f }; // Forward right
				targeted = new float[] { 0.5f, 0.75f };
				break;
			case 14:
				inputs = new float[] { 0.2f, 0.2f, 0.2f, 0.2f, 0.8f, 1.0f }; // Right
				targeted = new float[] { 0.5f, 1.0f };
				break;
		}

		brain.Train(inputs, targeted);
	}

	public void GeneticTraining()
	{
		GameObject genTrainer =  Instantiate(genTrainerPrefab);
		GUIMgr.Instance.stopBt.onClick.AddListener(() => Destroy(genTrainer));
	}
}