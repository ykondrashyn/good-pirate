public class CameraMoveOpenMenu : global::UnityEngine.MonoBehaviour
{
	private bool isMoved;

	[global::UnityEngine.HideInInspector]
	public new global::UnityEngine.Transform transform;

	private void Awake()
	{
		transform = GetComponent<global::UnityEngine.Transform>();
	}

	private void Start()
	{
	}

	private void SetLandscape()
	{
		EUIMode uiMode = UI_Manager.Instance.ui_mainGame.uiMode;
		if (uiMode == EUIMode.Formation)
		{
			GameManager.Instance.followTarget.FormationCameraSetting(active: true);
		}
		else if (isMoved)
		{
			transform.localPosition = new global::UnityEngine.Vector3(30f, 72f, -120f);
		}
	}

	private void SetPortrait()
	{
		EUIMode uiMode = UI_Manager.Instance.ui_mainGame.uiMode;
		if (uiMode == EUIMode.Formation)
		{
			GameManager.Instance.followTarget.FormationCameraSetting(active: true);
		}
		else if (isMoved)
		{
			transform.localPosition = new global::UnityEngine.Vector3(0f, 72f, -140f);
		}
	}

	public void MoveCamera()
	{
		if (!GameManager.Instance.IsInIsland)
		{
			isMoved = true;
			if (OrientationCheck.Instance.IsPortrait)
			{
				global::DG.Tweening.ShortcutExtensions.DOLocalMoveZ(transform, -140f, 0.3f);
			}
			else
			{
				global::DG.Tweening.ShortcutExtensions.DOLocalMoveX(transform, 35f, 0.3f);
			}
		}
	}

	public void CombackCameraPivot()
	{
		if (!GameManager.Instance.IsInIsland)
		{
			isMoved = false;
			if (OrientationCheck.Instance.IsPortrait)
			{
				global::DG.Tweening.ShortcutExtensions.DOLocalMoveZ(transform, -120f, 0.3f);
			}
			else
			{
				global::DG.Tweening.ShortcutExtensions.DOLocalMoveX(transform, 0f, 0.3f);
			}
		}
	}
}
