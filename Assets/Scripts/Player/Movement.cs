using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    [SerializeField]
    float mSlowDist = 5.0f;
    [SerializeField]
    float mStopDist = 1.0f;
    [SerializeField]
    float mMaxSpeed = 50.0f;

	Rigidbody mRb;
    Vector3 mTargetPos;
    public Vector3 TargetPos
    {
        get { return mTargetPos; }
        set
        {
            mTargetPos = value;
            HasArrived = false;
        }
    }


    public bool HasArrived
    {
        get { return mRb.isKinematic; }
        set { mRb.isKinematic = value; }
    }

    float GetOrientationFromDirection(Vector3 direction)
    {
        if (direction.magnitude > 0)
            return Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;

        return 0.0f;
    }

    void Awake ()
    {
        mRb = GetComponent<Rigidbody>();
    }
    void Start()
    {
        mTargetPos = transform.position;
    }
    void FixedUpdate()
    {
        if (HasArrived)
            return;

		// === Steering ===
		Vector3 desiredVel = mTargetPos - transform.position;
        desiredVel.y = 0.0f;

		if (desiredVel.sqrMagnitude > mMaxSpeed * mMaxSpeed)
			desiredVel = desiredVel.normalized * mMaxSpeed;

		Vector3 steerVel = desiredVel - mRb.velocity;
		//

		float sqrDist = (mTargetPos - transform.position).sqrMagnitude;

        if (sqrDist <= mSlowDist * mSlowDist)
        {
            if (sqrDist <= mStopDist * mStopDist)
            {
                mRb.velocity = Vector3.zero;
                HasArrived = true;
                return;
            }
            else
            {
                Vector3 brakingVelocity = steerVel * mMaxSpeed * (Mathf.Sqrt(sqrDist) / mSlowDist);
                steerVel = brakingVelocity - mRb.velocity;
            }
        }
		else if(steerVel.sqrMagnitude > mMaxSpeed * mMaxSpeed) // truncate to max speed
			steerVel = steerVel.normalized * mMaxSpeed;

		// add steering force
		mRb.velocity += steerVel * Time.fixedDeltaTime;

		float rotation = GetOrientationFromDirection(mRb.velocity);

		// Update rotation
		transform.eulerAngles = Vector3.up * rotation;
    }

    // stop car movement at once / reset boost
    private void OnCollisionEnter(Collision collision)
    {
		Stop();
	}

	public void StartBoost(float boostMult)
	{
		mRb.AddForce(transform.forward * boostMult, ForceMode.Impulse);
	}

	public void Stop()
	{
		//TargetPos = transform.position;
		mRb.velocity = Vector3.zero;
		HasArrived = true;
	}
}
