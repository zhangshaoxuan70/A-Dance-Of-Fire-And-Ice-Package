using TMPro;
using UnityEngine;

public class scnLevelSelectTaro : ADOBase
{
	public static scnLevelSelectTaro instance;

	public scrCreditsText credits;

	public Cutscene8 scene;

	public TextMeshPro showSceneText;

	public scrCamera mainCamera;

	public GameObject bgAsset;

	private Vector3 bgOrigScale;

	private bool creditsJumped;

	private int lastKey = -1;

	private scrPlanet chosenplanet => ADOBase.controller.chosenplanet;

	private bool responsive
	{
		get
		{
			return ADOBase.controller.responsive;
		}
		set
		{
			ADOBase.controller.responsive = value;
		}
	}

	private int menuPhase
	{
		get
		{
			return ADOBase.controller.menuPhase;
		}
		set
		{
			ADOBase.controller.menuPhase = value;
		}
	}

	private scrCamera camy => ADOBase.controller.camy;

	private void Awake()
	{
		instance = this;
		if (bgAsset != null)
		{
			bgOrigScale = bgAsset.transform.localScale;
		}
	}

	private void Start()
	{
		mainCamera.setCamSizeInstant(mainCamera.camsizenormal);
		JumpToWorldPortal(GCS.worldEntrance, instant: true);
		scene.creditsContent = credits.content;
		scene.creditsContentCopy = credits.contentCopy;
	}

	private void Update()
	{
		if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha1))
		{
			JumpWithKey(1);
		}
		else if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha2))
		{
			JumpWithKey(2);
		}
		else if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha3))
		{
			JumpWithKey(3);
		}
		else if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha4))
		{
			JumpWithKey(4);
		}
		else if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha5))
		{
			JumpWithKey(5);
		}
		else if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha6))
		{
			JumpToPosition(new Vector2(36f, 0f), instant: false);
		}
	}

	private void LateUpdate()
	{
		UpdateCameraOrthoSize();
	}

	private void UpdateCameraOrthoSize(bool print = false)
	{
		float value = (float)Screen.width * 1f / (float)Screen.height;
		float num = Mathf.InverseLerp(1.77777779f, 1.33333337f, value);
		mainCamera.tosize = Mathf.Lerp(5f, 6.7f, num);
		float tosize = mainCamera.tosize;
		mainCamera.camsizenormal = tosize;
		mainCamera.Bgcamstatic.orthographicSize = tosize;
		mainCamera.BGcam.orthographicSize = tosize;
		if (bgAsset != null)
		{
			bgAsset.transform.localScale = bgOrigScale * (tosize / 5f);
		}
		if (print)
		{
			printe($"t {num} toSize {mainCamera.tosize} camsizeNormal {mainCamera.camsizenormal}");
		}
	}

	public override void OnBeat()
	{
	}

	public void JumpWithKey(int key)
	{
		if (ADOBase.isMobile || ADOBase.controller.holding || ADOBase.controller.isCutscene)
		{
			return;
		}
		if (showSceneText != null)
		{
			showSceneText.gameObject.SetActive(value: false);
		}
		if (chosenplanet.transform.position.y > -3.5f)
		{
			if (lastKey == key && key != 5)
			{
				JumpToWorldPortal($"T{key}EX");
			}
			else
			{
				JumpToWorldPortal($"T{key}");
			}
		}
		else if (lastKey == key || key == 5)
		{
			JumpToWorldPortal($"T{key}");
		}
		else if (key != 5)
		{
			JumpToWorldPortal($"T{key}EX");
		}
		lastKey = key;
	}

	public void JumpToWorldPortal(string world, bool instant = false, bool wipeFirst = false)
	{
		if (!responsive)
		{
			return;
		}
		if (wipeFirst)
		{
			responsive = false;
			scrUIController.instance.WipeToBlack(WipeDirection.StartsFromRight, delegate
			{
				responsive = true;
				JumpToWorldPortal(world, instant: true);
				scrUIController.instance.WipeFromBlack();
			});
			return;
		}
		Vector2 position = new Vector2(0f, 0f);
		if (world != null)
		{
			position = (world.IsTaro() ? GCNS.jumpPositionTaroMenu0[world] : new Vector2(0f, 0f));
		}
		if (Persistence.GetTaroStoryProgress() == 6 && !creditsJumped)
		{
			printe("STORY PROGRESS 6");
			creditsJumped = true;
			position = new Vector2(36f, 0f);
		}
		JumpToPosition(position, instant);
	}

	private void JumpToPosition(Vector2 position, bool instant)
	{
		chosenplanet.transform.MoveXY(position);
		menuPhase = 1;
		if (instant)
		{
			camy.ViewObjectInstant(chosenplanet.transform, includeOffset: true);
		}
		else
		{
			camy.Refocus(chosenplanet.transform);
		}
		camy.isMoveTweening = true;
	}
}
