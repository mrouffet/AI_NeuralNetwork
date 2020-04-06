using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
public enum eRaceState
{
	Waiting,
	Started,
	Finished
}

public class RaceMgr : MonoBehaviour
{
    [SerializeField]
    GameObject carPrefab = null;

    [Range(1, 5)]
    public uint playerNum = 1u;
    [Range(0, 5)]
    public uint AINum = 1u;

    [Range(1, 3)]
    public uint lapsNum = 3u;

	public Checkpoint[] checkpoints;
    public Transform[] playerStarts;

	List<ControllerBase> controllers;

	uint[] cpValid = null;

    public event GameEvent OnRaceInitialized;
    public event GameEvent OnRaceFinished;

    static public RaceMgr Instance
	{
		get;
		private set;
	}

    eRaceState raceState = eRaceState.Waiting;
    public bool HasStarted { get { return raceState == eRaceState.Started; } }

    void Awake ()
    {
		Instance = this;

        cpValid = new uint[playerNum + AINum];

        // Init Checkpoints.
        uint currID = 0u;
        foreach (Checkpoint cp in checkpoints)
            cp.ID = currID++;

		controllers = new List<ControllerBase>();

		SpawnCars();

		OnRaceFinished += () => { raceState = eRaceState.Finished; };

		OnRaceInitialized.SafeCall();
    }
	void Start()
	{
		GameMgr.Instance.OnStartRace += StartRace;
		GameMgr.Instance.OnResetRace += ResetRace;
	}

	void SpawnCars()
    {
        uint index = 0u;

        // always 1 player min.
        var controller = Instantiate(GameMgr.Instance.PCPrefab).GetComponent<ControllerBase>();
        var car = Instantiate(carPrefab, playerStarts[index].position, playerStarts[index++].rotation).GetComponent<Car>();

		controller.player = car;
		controllers.Add(controller);

		Camera.main.GetComponent<BasicCamera>().target = car;

        for (uint i = 1u; i < playerNum; i++)
        {
            // other controller for multiplayer.
            //controller = Instantiate(GameMgr.Instance.PCPrefab).GetComponent<NetworkController>();
            car = Instantiate(carPrefab, playerStarts[index].position, playerStarts[index++].rotation).GetComponent<Car>();

            controller.player = car;
			controllers.Add(controller);
		}

		for (uint i = 0u; i < AINum; i++)
        {
            controller = Instantiate(GameMgr.Instance.AICPrefab).GetComponent<ControllerBase>();
            car = Instantiate(carPrefab, playerStarts[index].position, playerStarts[index++].rotation).GetComponent<Car>();

			controller.player = car;
			controllers.Add(controller);
		}
	}

	void StartRace()
	{
		if (GameMgr.Instance.GameMode == eGameMode.AiTraining)
			AIDirector.Instance.GeneticTraining();
		else
			StartCoroutine(WaitForCountdown());
	}
	void ResetRace()
	{
		// Reset CPs.
		Array.Clear(cpValid, 0, cpValid.Length);

		// Reset cars.
		uint index = 0u;

		foreach (ControllerBase control in controllers)
		{
			control.player.movement.HasArrived = true;
			control.player.transform.position = playerStarts[index].position;
			control.player.transform.rotation = playerStarts[index++].rotation;
		}
	}

	public void SwitchGameMode()
	{
		bool isActive = GameMgr.Instance.GameMode == eGameMode.Race;

		foreach (ControllerBase control in controllers)
		{
			control.gameObject.SetActive(isActive);
			control.player.gameObject.SetActive(isActive);
		}
	}

	IEnumerator WaitForCountdown()
	{
		yield return new WaitForSeconds(3f);

		raceState = eRaceState.Started;
	}

    public void OnValidateCheckpoint(Checkpoint cp, Car car)
    {
        if (!HasStarted)
            return;

		if (cpValid[car.ID] % checkpoints.Length == cp.ID)
		{
			cpValid[car.ID]++;

			if (cpValid[car.ID] == checkpoints.Length * lapsNum)
				OnRaceFinished.SafeCall();
			else if (car.ID == 0u && cpValid[car.ID] % checkpoints.Length == 0u)
				GUIMgr.Instance.SetLapCount((uint)(1 + (cpValid[car.ID] / checkpoints.Length)));
		}
    }

	public Checkpoint GetNextCheckpoint(Car car)
	{
		return checkpoints[cpValid[car.ID] % checkpoints.Length];
	}
}
