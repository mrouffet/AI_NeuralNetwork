using UnityEngine;

public class Boost : BonusBase
{
	[SerializeField]
	float mBoostValue = 25.0f;

	public override void OnPickup()
	{
		base.OnPickup();

		mCar.movement.StartBoost(mBoostValue);
	}
}