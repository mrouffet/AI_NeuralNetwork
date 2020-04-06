using UnityEngine;
using System.Threading;

public class AITrainer : MonoBehaviour
{
	[SerializeField]
	float viewRange = 50.0f;

	float startedTime;

	public NeuralNetwork brain;

	Car player;

	Thread mThread;

	int mIsRunning = -1;
	public bool isRunning
	{
		get { return mIsRunning != -1; }
		set
		{
			mIsRunning = value ? 1 : -1;
		}
	}

	static uint currID = 0u;
	public uint ID { get; private set; }

	void Awake()
	{
		ID = currID++;
		player = GetComponent<Car>();
	}
	void Start()
	{
		// This must be in Start, not Awake due to Provide() method.

		if (!isRunning)
			Generate();
	}

	void Update()
	{
		if (GeneticTrainer.raceState != eRaceState.Started)
			return;

		if (mIsRunning == 1)
			ResetTraining();

		if(mIsRunning == 0)
			Control();
	}

	void OnDestroy()
	{
		currID--;
	}

	void Control()
	{
		float[] inputs = new float[6]; // DistRatio + forward dot checkpointPos.

		/*
		 *	Raycasting 
		 *	
		 *	   \  |	 /
		 *		\ |	/
		 *	  ---CAR---
		 *	  
		 */

		Vector3[] rayDirs =
		{
			-transform.right,							// left side.
			transform.forward - transform.right,		// left forward side.
			transform.forward,							// forward side.
			transform.forward + transform.right,		// right forward side.
			transform.right								// right side.
		};

		// Fill distance ratios.
		for (uint i = 0u; i < rayDirs.Length; i++)
		{
			RaycastHit hitInfos;

			if (Physics.Raycast(transform.position, rayDirs[i], out hitInfos, viewRange, 1 << LayerMask.NameToLayer("Wall")))
				inputs[i] = hitInfos.distance / viewRange;
			else
				inputs[i] = 50.0f;
		}

		// Use transform.right to have a signed dot.
		inputs[5] = Vector3.Dot(player.transform.right, (GeneticTrainer.Instance.GetNextCheckpoint(this).transform.position - player.transform.position).normalized);

		float[] outputs = brain.Compute(inputs);

		// output:	0		0.5		1
		// angle:	-90		0		90
		Quaternion angle = Quaternion.AngleAxis(outputs[1] * 180.0f - 90.0f, Vector3.up);

		// Rotate forward.
		Vector3 dir = angle * (outputs[0] * viewRange * transform.forward);

		player.movement.TargetPos = transform.position + dir;
	}

	public void Provide(NeuralNetwork newBrain)
	{
		brain = newBrain;
		isRunning = true;
	}
	public void Generate()
	{
		isRunning = false;

		mThread = new Thread(
			() =>
			{
				brain = NeuralNetwork.Generate(AIDirector.Brain.inputNum, AIDirector.Brain.outputNum, Tools.Rand(2u, GeneticTrainer.Instance.maxLayerNum + 1), GeneticTrainer.Instance.maxNeuronPerLayer);

				brain.Init();
				AIDirector.TrainingProgram(brain);
				
				isRunning = true;
			}
		);

		mThread.Start();
	}

	public void Generate(NeuralNetwork mom, NeuralNetwork dad)
	{
		isRunning = false;

		mThread = new Thread(
			() =>
			{
				brain = NeuralNetwork.Breed(mom, dad);

				isRunning = true;
			}
		);

		mThread.Start();
	}

	void ResetTraining()
	{
		player.movement.Stop();
		transform.position = RaceMgr.Instance.playerStarts[0].position;
		transform.rotation = RaceMgr.Instance.playerStarts[0].rotation;

		mIsRunning = 0;
		startedTime = Time.time;
	}
	public float GetTime()
	{
		return Time.time - startedTime;
	}
}