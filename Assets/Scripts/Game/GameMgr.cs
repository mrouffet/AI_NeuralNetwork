using UnityEngine;
using UnityEngine.SceneManagement;

public enum eGameMode
{
	Race = 0,
	AiTraining = 1
}

public delegate void GameEvent();
public static class Extention
{
    public static void SafeCall(this GameEvent ev)
    {
        if (ev != null)
            ev();
    }
}

public class GameMgr : MonoBehaviour
{
    // Controller prefabs.
    public GameObject PCPrefab = null;
    public GameObject AICPrefab = null;

    public GameObject BrainSaverPrefab = null;

	public event GameEvent OnStartRace;
    public event GameEvent OnResetRace;

	eGameMode mGameMode;

	public eGameMode GameMode
	{
		get { return mGameMode; }
		set
		{
			if(mGameMode != value)
			{
				mGameMode = value;
				RaceMgr.Instance.SwitchGameMode();
				Camera.main.GetComponent<BasicCamera>().SwitchCamera();
			}
		}
	}

	static GameMgr mInstance = null;
    static public GameMgr Instance { get { return mInstance; } }
    void Awake ()
    {
        mInstance = this;
	}

    void Start()
    {
		GUIMgr.Instance.startBt.onClick.AddListener(
			() =>
			{
				GameMode = (eGameMode)GUIMgr.Instance.gmSelect.value;
				OnStartRace.SafeCall();
			}
		);

		GUIMgr.Instance.resetBt.onClick.AddListener(
			() =>
			{
				OnResetRace.SafeCall();
				OnStartRace.SafeCall();
			}
		);

		GUIMgr.Instance.menuBt.onClick.AddListener(() => OnResetRace.SafeCall());
	}

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.U) && SceneManager.GetActiveScene().name != "SimpleMap")
			LoadScene("SimpleMap");
		else if (Input.GetKeyDown(KeyCode.I) && SceneManager.GetActiveScene().name != "AdvancedMap")
			LoadScene("AdvancedMap");
		else if (Input.GetKeyDown(KeyCode.Escape))
			Application.Quit();
	}

	void LoadScene(string levelName)
	{
		Instantiate(BrainSaverPrefab); // Create brainSaver to keep best brain throught scene.

		SceneManager.LoadScene(levelName);
	}
}