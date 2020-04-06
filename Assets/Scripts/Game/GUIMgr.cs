using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GUIMgr : MonoBehaviour
{
	public Button startBt;
	public Dropdown gmSelect;
	public Button resetBt;
	public Button menuBt;
	public Button stopBt;
	public Slider timeScaleSlider;

	[SerializeField]
	Text mRaceMsgText = null;
	[SerializeField]
	Text mLapCountText = null;
	[SerializeField]
	Text mTimerText = null;

	float mStartTime = 0.0f;

	public static GUIMgr Instance
	{
		get;
		private set;
	}

	void Awake()
	{
		Instance = this;
	}
	void Start()
	{
		GameMgr.Instance.OnStartRace += () => StartCoroutine(StartRace());

		mLapCountText.text = "Lap " + "1 / " + RaceMgr.Instance.lapsNum.ToString();

		resetBt.gameObject.SetActive(false);
		menuBt.gameObject.SetActive(false);
		stopBt.gameObject.SetActive(false);
		timeScaleSlider.gameObject.SetActive(false);

		startBt.onClick.AddListener(
			() =>
			{
				startBt.gameObject.SetActive(false);
				gmSelect.gameObject.SetActive(false);

				if ((eGameMode)gmSelect.value == eGameMode.AiTraining)
				{
					stopBt.gameObject.SetActive(true);
					timeScaleSlider.gameObject.SetActive(true);
				}
			}
		);

		resetBt.onClick.AddListener(
			() =>
			{
				mTimerText.text = "00 : 00 : 00";
				mLapCountText.text = "Lap " + "1 / " + RaceMgr.Instance.lapsNum.ToString();

				resetBt.gameObject.SetActive(false);
				menuBt.gameObject.SetActive(false);
			}
		);

		menuBt.onClick.AddListener(
			() =>
			{
				startBt.gameObject.SetActive(true);
				gmSelect.gameObject.SetActive(true);

				resetBt.gameObject.SetActive(false);
				menuBt.gameObject.SetActive(false);
			}
		);

		stopBt.onClick.AddListener(
			() =>
			{
				startBt.gameObject.SetActive(true);
				gmSelect.gameObject.SetActive(true);

				resetBt.gameObject.SetActive(false);
				menuBt.gameObject.SetActive(false);
				stopBt.gameObject.SetActive(false);

				timeScaleSlider.value = 1.0f;
				timeScaleSlider.gameObject.SetActive(false);
			}
		);

		timeScaleSlider.onValueChanged.AddListener((float val) => { Time.timeScale = val; });

		RaceMgr.Instance.OnRaceFinished += () =>
		{
			resetBt.gameObject.SetActive(true);
			menuBt.gameObject.SetActive(true);
		};
	}
	void Update()
	{
		if (!RaceMgr.Instance.HasStarted)
			return;

		float currTime = Time.time - mStartTime;
		int min, sec, tsec;
		min = Mathf.FloorToInt(currTime / 60.0f);
		currTime = currTime % 60.0f;
		sec = Mathf.FloorToInt(currTime);
		currTime = currTime - sec;
		tsec = Mathf.FloorToInt(currTime * 100.0f);
		mTimerText.text = string.Format("{0:D2} : {1:D2}  : {2:D2}", min, sec, tsec);
	}

	public IEnumerator StartRace()
	{
		if (GameMgr.Instance.GameMode == eGameMode.Race)
		{
			mRaceMsgText.enabled = true;
			mRaceMsgText.text = "3";
			yield return new WaitForSeconds(1f);
			mRaceMsgText.text = "2";
			yield return new WaitForSeconds(1f);
			mRaceMsgText.text = "1";
			yield return new WaitForSeconds(1f);

			mRaceMsgText.text = "GO !!";

			mStartTime = Time.time;

			yield return new WaitForSeconds(1f);
			mRaceMsgText.enabled = false;
		}
	}

	public void SetLapCount(uint lapCount)
	{
		mLapCountText.text = "Lap " + lapCount.ToString() + " / " + RaceMgr.Instance.lapsNum.ToString();
	}
}