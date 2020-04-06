using UnityEngine;

public class PlayerController : ControllerBase
{
    [SerializeField]
    GameObject cursorPrefab = null;
    GameObject cursor = null;

	public static PlayerController Local
	{
		get;
		private set;
	}

	private void OnEnable()
	{
		if (cursor)
			cursor.SetActive(true);
	}
	void OnDisable()
	{
		if (cursor)
			cursor.SetActive(false);
	}

	void Awake()
    {
		if (Local == null)
			Local = this;

        if (cursorPrefab != null)
            cursor = Instantiate(cursorPrefab, transform);
    }
	void Update ()
    {
        if (RaceMgr.Instance.HasStarted == false)
            return;

        if (Input.GetMouseButton(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            RaycastHit raycastInfo;

            // unit move target
            if (Physics.Raycast(ray, out raycastInfo, Mathf.Infinity, 1 << LayerMask.NameToLayer("Floor")))
            {
                Vector3 newPos = raycastInfo.point;
                Vector3 targetPos = newPos;
                targetPos.y += 0.1f;
                cursor.transform.position = targetPos;

                player.movement.TargetPos = newPos;
            }
        }
    }
}