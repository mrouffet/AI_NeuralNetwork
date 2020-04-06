using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public uint ID = 0u;
    
    void OnTriggerEnter(Collider collider)
    {
		if (GameMgr.Instance.GameMode == eGameMode.Race)
		{
			Car car = collider.GetComponent<Car>();

			if (car != null)
				RaceMgr.Instance.OnValidateCheckpoint(this, car);
		}
		else // eGameMode.AiTraining
		{
			AITrainer trainer = collider.GetComponent<AITrainer>();

			if (trainer != null)
				GeneticTrainer.Instance.OnValidateCheckpoint(this, trainer);
		}
    }
}
