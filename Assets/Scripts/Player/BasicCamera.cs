using UnityEngine;
using System.Collections;

public class BasicCamera : MonoBehaviour
{
    [SerializeField]
    float speed = 10f;

    [SerializeField]
    float YDistance = 200f;

    public Car target = null;

	Quaternion mBaseRot;

	[SerializeField]
	Vector3 mObserverPos;
	[SerializeField]
	Vector3 mObserverRot;

	GameEvent mUpdate;

	void Start()
	{
		mBaseRot = transform.rotation;
		mUpdate = FollowPlayer;
	}
	void LateUpdate()
    {
		mUpdate.SafeCall();
	}

	void FollowPlayer()
	{
		if (target == null)
			return;

		Vector3 targetPos = target.transform.position;
		targetPos.y = YDistance;
		transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * speed);
	}

	public void SwitchCamera()
	{
		if(GameMgr.Instance.GameMode == eGameMode.Race)
			mUpdate = FollowPlayer;
		else
			mUpdate = null;

		StartCoroutine(SwitchLerp());
	}

	IEnumerator SwitchLerp()
	{
		Vector3 basePos = transform.position;
		Quaternion baseRot = transform.rotation;

		Quaternion targetRot = GameMgr.Instance.GameMode == eGameMode.Race ? mBaseRot : Quaternion.Euler(mObserverRot);

		for (float currTime = 0.0f; currTime < 1.0f; currTime += Time.deltaTime)
		{
			if(GameMgr.Instance.GameMode == eGameMode.AiTraining)
				transform.position = Vector3.Lerp(basePos, mObserverPos, currTime);

			transform.rotation = Quaternion.Lerp(baseRot, targetRot, currTime);

			yield return null;
		}

		// Set at target (due to deltatime).
		if (GameMgr.Instance.GameMode == eGameMode.AiTraining)
			transform.position = mObserverPos;

		transform.rotation = targetRot;
	}
}