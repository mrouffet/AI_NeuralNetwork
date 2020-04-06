using UnityEngine;
using System.Collections;
 
public interface IBonus
{
	void OnPickup();
}

public class BonusBase : MonoBehaviour, IBonus
{
	[SerializeField]
	float mRespawnTime = 2.0f;

	Collider mColl;
	MeshRenderer mRend;

	protected Car mCar;
	
	void Awake()
	{
		mColl = GetComponent<Collider>();
		mRend = GetComponent<MeshRenderer>();
	}

	public virtual void OnPickup()
	{
		// no cooldown on AI Training.
		if(GameMgr.Instance.GameMode == eGameMode.Race)
			StartCoroutine(Cooldown());
	}

	IEnumerator Cooldown()
	{
		mColl.enabled = false;
		mRend.enabled = false;

		yield return new WaitForSeconds(mRespawnTime);

		mColl.enabled = true;
		mRend.enabled = true;
	}

	private void OnTriggerEnter(Collider other)
	{
		mCar = other.gameObject.GetComponent<Car>();

		if (mCar != null)
			OnPickup();
	}
}