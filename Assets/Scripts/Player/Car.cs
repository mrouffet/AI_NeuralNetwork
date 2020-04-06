using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car : MonoBehaviour
{
    static uint currID = 0u;

    uint mID;
    public uint ID { get { return mID; } }

    public Movement movement;

    void Awake()
    {
        mID = currID++;

        movement = GetComponent<Movement>();

		GetComponent<Renderer>().material.color = GetComponent<Renderer>().material.color = Random.ColorHSV(0.0f, 1.0f, 0.5f, 1f, 0.5f, 1f);
	}

	void OnDestroy()
	{
		currID--;
	}
}
