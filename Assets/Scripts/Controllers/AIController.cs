using UnityEngine;

public class AIController : ControllerBase
{
	[SerializeField]
	float viewRange = 50.0f;

	[SerializeField]
	float mBrainModulation = 0.02f;
	float mNoise = 0.0f;

	void Start()
	{
		// Add noise to brain result: diffrent behaviours.
		mNoise = Random.Range(-mBrainModulation, mBrainModulation);
	}
	void Update()
	{
		if (RaceMgr.Instance.HasStarted == false)
			return;

		Control();
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
			-player.transform.right,								// left side.
			player.transform.forward - player.transform.right,		// left forward side.
			player.transform.forward,								// forward side.
			player.transform.forward + player.transform.right,		// right forward side.
			player.transform.right									// right side.
		};

		// Fill distance ratios.
		for (uint i = 0u; i < rayDirs.Length; i++)
		{
			RaycastHit hitInfos;

			if (Physics.Raycast(player.transform.position, rayDirs[i], out hitInfos, viewRange, 1 << LayerMask.NameToLayer("Wall")))
			{
				inputs[i] = hitInfos.distance / viewRange;
				//Debug.DrawLine(player.transform.position, hitInfos.point, Color.cyan);
			}
			else
			{
				inputs[i] = viewRange;
				//Debug.DrawLine(player.transform.position, player.transform.position + viewRange * rayDirs[i], Color.red);
			}
		}

		// Use transform.right to have a signed dot.
		inputs[5] = Vector3.Dot(player.transform.right, (RaceMgr.Instance.GetNextCheckpoint(player).transform.position - player.transform.position).normalized);

		//Debug.DrawLine(player.transform.position, RaceMgr.Instance.GetNextCheckpoint(player).transform.position, Color.yellow);

		float[] outputs = AIDirector.Brain.Compute(inputs);

		// modulate values: not all AI get same results.
		for (uint i = 0u; i < outputs.Length; i++)
			outputs[i] -= mNoise;

		// output:	0		0.5		1
		// angle:	-90		0		90
		Quaternion angle = Quaternion.AngleAxis(outputs[1] * 180.0f - 90.0f, Vector3.up);

		// Rotate forward.
		Vector3 dir = angle * (outputs[0] * viewRange * player.transform.forward);

		player.movement.TargetPos = player.transform.position + dir;
		//Debug.DrawLine(player.transform.position, player.movement.TargetPos, Color.green);
	}
}
