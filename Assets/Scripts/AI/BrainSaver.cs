using UnityEngine;

public class BrainSaver : MonoBehaviour
{
	NeuralNetwork mBrain; // save brain throught scene.

	public static BrainSaver Instance { get; private set; }

	void Awake()
	{
		Instance = this;

		DontDestroyOnLoad(gameObject);
	}

	void OnDestroy()
	{
		Instance = null;
	}

	public static void Keep(NeuralNetwork brain)
	{
		if (Instance == null)
			return;

		Instance.mBrain = brain;
	}

	public static NeuralNetwork Release()
	{
		if (Instance == null)
			return null;

		Destroy(Instance.gameObject, 0.1f);

		return Instance.mBrain;
	}
}