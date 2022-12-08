using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class scrPortal : ADOBase
{
	private readonly float[] lanternLightAlphas = new float[3]
	{
		0.8f,
		0.588f,
		0.71f
	};

	public int lane;

	public int group;

	public string world;

	public Text worldName;

	public CanvasGroup stats;

	public SpriteRenderer sprPortal;

	public Transform[] lanterns;

	public GameObject padlockContainer;

	public SpriteRenderer padlock1;

	public SpriteRenderer padlock2;

	public Material defaultMaterial;

	public Material grayscale;

	public scrMenuWorldStatsText statsText;

	public Text backText;

	public SpriteRenderer sign;

	public Button button;

	public SpriteRenderer xtraDecoration;

	public ParticleSystem[] borderParticles;

	public PortalCredit levelCredits;

	public PortalCredit songCredits;

	public PortalCredit secondaryLevelCredits;

	public PortalCredit secondarySongCredits;

	public PortalCredit tertiaryLevelCredits;

	public RectTransform statsCanvasMobilePlaceholder;

	[FormerlySerializedAs("canvasGroup")]
	public CanvasGroup creditsCanvasGroup;

	[NonSerialized]
	public float originalPortalScale;

	private Vector3 originalDecScale;

	private bool xtraWorld;

	private bool crownWorld;

	private bool museDashWorld;

	private bool isXtraDesktopPortal;

	public bool locked;

	public bool usesCrownSign;

	public static List<scrPortal> portals = new List<scrPortal>();

	[Header("Taro DLC")]
	public GameObject taroStats;

	public Text medalCount;

	public Transform medalsContainer;

	private Mawaru_Medal[] medals;

	private float timeDisplayed;

	private bool showingMedals;

	private Sequence statsTransition;

	private Sequence medalsFade;

	private Sequence medalsAppear;

	[Header("Taro DLC EX World")]
	public Text medalRequirementWorld;

	public Text medalRequirementCount;

	public GameObject taroLock;

	public scrFloor portalEntrance;

	private const float timeForTransition = 5f;

	private bool fadetoggle;

	[NonSerialized]
	public bool hidden;

	private Vector2Int prevPlanetPosition = new Vector2Int(-100, -100);

	private bool neverPlayed
	{
		get
		{
			if (Persistence.GetPercentCompletion(ADOBase.worldData[world].index) == 0f)
			{
				return Persistence.GetWorldAttempts(ADOBase.worldData[world].index) == 0;
			}
			return false;
		}
	}

	private void Awake()
	{
		if (sprPortal != null)
		{
			originalPortalScale = sprPortal.transform.localScale.x;
		}
		if (!usesCrownSign)
		{
			Sprite[] array = null;
			if (ADOBase.IsHalloweenWeek())
			{
				array = RDC.data.halloweenLanternSprites;
			}
			else if (ADOBase.IsCNY())
			{
				array = RDC.data.CNYLanternSprites;
			}
			if (array != null)
			{
				lanterns[0].GetComponent<SpriteRenderer>().sprite = array[0];
				lanterns[1].GetComponent<SpriteRenderer>().sprite = array[1];
				lanterns[2].GetComponent<SpriteRenderer>().sprite = array[2];
			}
		}
		if (portals.Count > 0)
		{
			portals.Clear();
		}
		FadeCredits(0f, instant: true);
	}

	private void Start()
	{
		Setup();
		if (!ADOBase.isMobile)
		{
			ShowStats(show: false, instant: true);
		}
	}

	public void Setup(bool speedTrial = false)
	{
		if (world.EndsWith("EX") && GCNS.dlcMedalsRequired.ContainsKey(world))
		{
			string text = world.Remove(world.Length - 2);
			int num = GCNS.dlcMedalsRequired[world];
			int num2 = 0;
			int[] medalsForDLCLevel = Persistence.GetMedalsForDLCLevel(text);
			for (int i = 0; i < medalsForDLCLevel.Length; i++)
			{
				if (medalsForDLCLevel[i] >= 3)
				{
					num2++;
				}
			}
			if (num2 < num && neverPlayed)
			{
				medalRequirementWorld.text = text;
				medalRequirementCount.text = "x" + num.ToString();
				taroLock.gameObject.SetActive(value: true);
				taroStats.gameObject.SetActive(value: false);
				statsText.gameObject.SetActive(value: false);
				portalEntrance.gameObject.SetActive(value: false);
				LockWorld(locked: true);
			}
		}
		xtraWorld = (world.IsXtra() || world.IsMuseDashWorld());
		museDashWorld = world.IsMuseDashWorld();
		bool flag = world.IsTaro();
		if (flag)
		{
			medals = medalsContainer.GetComponentsInChildren<Mawaru_Medal>(includeInactive: true);
		}
		if (taroStats != null && !locked && !neverPlayed)
		{
			taroStats.SetActive(flag);
		}
		isXtraDesktopPortal = xtraWorld;
		portals.Add(this);
		if (isXtraDesktopPortal)
		{
			worldName.color = worldName.color.WithAlpha(0f);
			stats.alpha = 0f;
			if (xtraDecoration != null)
			{
				xtraDecoration.gameObject.SetActive(value: false);
			}
		}
		worldName.SetLocalizedFont();
		UpdateLanterns();
		UpdateWorldName(speedTrial);
		UpdateMedals();
	}

	public void UpdateWorldName(bool speedTrial = false)
	{
		string text = RDString.Get("levelSelect.world", new Dictionary<string, object>
		{
			{
				"number",
				world
			}
		});
		worldName.text = text;
		if (speedTrial)
		{
			int num = Mathf.RoundToInt((float)worldName.fontSize * 2f / 3f);
			Text text2 = worldName;
			text2.text = text2.text + "\n<size=" + num.ToString() + ">" + RDString.Get("levelSelect.SpeedTrial") + "</size>";
		}
	}

	private void UpdateLanterns()
	{
		bool num = isXtraDesktopPortal;
		int index = ADOBase.worldData[world].index;
		bool isWorldComplete = Persistence.IsWorldComplete(index);
		bool isWorldPerfect = Persistence.IsWorldPerfect(index);
		bool isWorldSpeedTrial = Persistence.IsSpeedTrialComplete(index);
		bool updatePositions = !num && !usesCrownSign && !museDashWorld;
		UpdateLanternStates(isWorldComplete, isWorldPerfect, isWorldSpeedTrial, updatePositions);
	}

	public void UpdateLanternStates(bool isWorldComplete, bool isWorldPerfect, bool isWorldSpeedTrial, bool updatePositions = true)
	{
		lanterns[0].gameObject.SetActive(isWorldComplete);
		lanterns[1].gameObject.SetActive(isWorldPerfect);
		lanterns[2].gameObject.SetActive(isWorldSpeedTrial && isWorldComplete);
		int num = (isWorldComplete ? 1 : 0) + (isWorldPerfect ? 1 : 0) + (isWorldSpeedTrial ? 1 : 0);
		if (!updatePositions)
		{
			return;
		}
		Vector2[] array;
		Vector2[] array2;
		Vector2[] array3;
		if (usesCrownSign)
		{
			array = new Vector2[1]
			{
				new Vector2(0f, 0.208f)
			};
			array2 = new Vector2[2]
			{
				new Vector2(-1.228f, 0.208f),
				new Vector2(1.157f, 0.208f)
			};
			array3 = new Vector2[3]
			{
				new Vector2(-1.228f, 0.208f),
				new Vector2(0f, 0.208f),
				new Vector2(1.157f, 0.208f)
			};
		}
		else
		{
			array = new Vector2[1]
			{
				new Vector2(-0.06042726f, 0.01435363f)
			};
			array2 = new Vector2[2]
			{
				new Vector2(-1.175f, 0.034f),
				new Vector2(1.14f, 0.03399968f)
			};
			array3 = new Vector2[3]
			{
				new Vector2(-2.090776f, 0.4f),
				new Vector2(-0.06042726f, 0.01435363f),
				new Vector2(2.078689f, 0.4f)
			};
		}
		Vector2[] array4 = null;
		switch (num)
		{
		case 1:
			array4 = array;
			break;
		case 2:
			array4 = array2;
			break;
		case 3:
			array4 = array3;
			break;
		}
		int num2 = 0;
		for (int i = 0; i < lanterns.Length; i++)
		{
			Transform transform = lanterns[i];
			if (transform.gameObject.activeSelf)
			{
				Vector2 vector = array4[num2];
				transform.localPosition = new Vector3(vector.x, vector.y, transform.localPosition.z);
				num2++;
			}
		}
	}

	public void LockWorld(bool locked, bool speedTrial = false)
	{
		this.locked = locked;
		padlockContainer.SetActive(locked);
		sprPortal.material = (locked ? grayscale : defaultMaterial);
		statsText.UpdateText(locked, speedTrial);
	}

	private void Update()
	{
		string sceneName = ADOBase.sceneName;
		if (sceneName == "scnMobileMenu" || isXtraDesktopPortal || locked)
		{
			return;
		}
		Vector3 position = scrController.instance.chosenplanet.transform.position;
		Vector2Int vector2Int = prevPlanetPosition = new Vector2Int(Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.y));
		string a = null;
		foreach (KeyValuePair<string, GCNS.WorldData> worldDatum in ADOBase.worldData)
		{
			Vector2 vector = Vector2.right * 99f;
			if (worldDatum.Key.IsTaro())
			{
				if (sceneName == "scnTaroMenu0")
				{
					vector = GCNS.jumpPositionTaroMenu0[worldDatum.Key] - vector2Int;
				}
				else if (sceneName == "scnTaroMenu1")
				{
					vector = GCNS.activePositionTaroMenu1[worldDatum.Key] - vector2Int;
				}
				else if (sceneName == "scnTaroMenu2")
				{
					vector = GCNS.activePositionTaroMenu2[worldDatum.Key] - vector2Int;
				}
				else if (sceneName == "scnTaroMenu3")
				{
					vector = GCNS.activePositionTaroMenu3[worldDatum.Key] - vector2Int;
				}
			}
			else
			{
				vector = worldDatum.Value.jumpPortalPosition - vector2Int;
			}
			if (Math.Max(Math.Abs(vector.x), Math.Abs(vector.y)) == 0f)
			{
				a = worldDatum.Key;
				if (scnLevelSelect.instance != null)
				{
					scnLevelSelect.instance.lastVisitedWorld = worldDatum.Key;
				}
			}
		}
		if (a == world && !fadetoggle && !hidden)
		{
			ShowStats(show: true);
			ExpandPortal(expand: true);
			FadeCredits(1f);
		}
		else if (a != world && fadetoggle)
		{
			ShowStats(show: false);
			ExpandPortal(expand: false);
			FadeCredits(0f);
		}
		if (fadetoggle && world.IsTaro() && Persistence.IsWorldComplete(world))
		{
			timeDisplayed += Time.deltaTime;
			timeDisplayed %= 10f;
			if ((!showingMedals && timeDisplayed < 5f) || (showingMedals && timeDisplayed >= 5f))
			{
				SwitchBetwenTaroStats(!showingMedals);
			}
		}
		if (ADOBase.gc.debug && portalEntrance != null)
		{
			portalEntrance.gameObject.SetActive(value: true);
		}
	}

	public void FadeCredits(float alpha, bool instant = false)
	{
		float duration = instant ? 0f : 0.4f;
		alpha = (locked ? 0f : alpha);
		if (creditsCanvasGroup != null)
		{
			creditsCanvasGroup.DOKill();
			creditsCanvasGroup.DOFade(alpha, duration);
			creditsCanvasGroup.interactable = (alpha >= 1f);
		}
	}

	public void ExpandPortal(bool expand, bool instant = false)
	{
		float num = instant ? 0f : 0.4f;
		float alpha = expand ? 0.4f : 1f;
		float alpha2 = expand ? 1f : 0f;
		sprPortal.transform.DOScale(originalPortalScale * (expand ? 0.8f : 1f), num * 0.75f);
		if (xtraDecoration != null)
		{
			xtraDecoration.gameObject.SetActive(expand);
		}
		FadeCredits(alpha2, instant);
		FadePortalImage(alpha, instant);
		ShowStats(expand);
	}

	public void ExpandPortalMobile(bool expand)
	{
		float num = 0.4f;
		sprPortal.transform.DOScale(originalPortalScale * (expand ? 0.8f : 1f), num * 0.75f);
		FadeCredits(expand ? 0f : 1f);
	}

	public void FadePortalImage(float alpha, bool instant = false)
	{
		float duration = instant ? 0f : 0.4f;
		ParticleSystem[] array = borderParticles;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].GetComponent<Renderer>().material.DOFade(alpha, duration);
		}
		sprPortal.DOFade(alpha, duration);
	}

	public void FadePortal(float alpha, bool instant = false)
	{
		float duration = instant ? 0f : 0.4f;
		worldName.DOKill();
		padlock1.DOKill();
		padlock2.DOKill();
		sign.DOKill();
		worldName.DOFade(alpha, duration);
		sign.DOFade(alpha, duration);
		padlock1.DOFade(alpha, duration);
		padlock2.DOFade(alpha, duration);
		if (isXtraDesktopPortal)
		{
			worldName.DOFade(alpha, duration);
		}
		for (int i = 0; i < lanterns.Length; i++)
		{
			Transform obj = lanterns[i];
			SpriteRenderer component = obj.GetComponent<SpriteRenderer>();
			component.DOKill();
			component.DOFade(alpha, duration);
			SpriteRenderer component2 = obj.GetChild(0).GetComponent<SpriteRenderer>();
			float endValue = lanternLightAlphas[i] * alpha;
			component2.DOKill();
			component2.DOFade(endValue, duration);
		}
		FadePortalImage(alpha, instant);
	}

	public void ShowStats(bool show, bool instant = false)
	{
		fadetoggle = show;
		float endValue = show ? 1f : 0f;
		float duration = instant ? 0f : 0.4f;
		stats.DOKill();
		stats.DOFade(endValue, duration);
		if (!world.IsTaro() || locked || neverPlayed)
		{
			return;
		}
		medalsFade.Kill();
		statsTransition.Kill();
		if (show)
		{
			Mawaru_Medal[] array = medals;
			foreach (Mawaru_Medal obj in array)
			{
				obj.front.render.SetAlpha(1f);
				obj.back.render.SetAlpha(1f);
				obj.transform.localScale = Vector3.zero;
			}
		}
		else
		{
			medalsFade = DOTween.Sequence();
			Mawaru_Medal[] array = medals;
			foreach (Mawaru_Medal mawaru_Medal in array)
			{
				medalsFade.Insert(0f, mawaru_Medal.front.render.DOFade(0f, duration));
				medalsFade.Insert(0f, mawaru_Medal.back.render.DOFade(0f, duration));
			}
		}
		if (show)
		{
			showingMedals = true;
			timeDisplayed = 3f;
			statsText.gameObject.SetActive(!showingMedals);
			medalCount.gameObject.SetActive(showingMedals);
			DoMedalsAnimation(showingMedals);
		}
	}

	public void ShakePortal()
	{
		GetComponentInChildren<scrGfxFloat>().Shake();
	}

	private void UpdateMedals()
	{
		if (!world.IsTaro() || locked || neverPlayed)
		{
			return;
		}
		int[] medalsForDLCLevel = Persistence.GetMedalsForDLCLevel(world);
		int num = 0;
		for (int i = 0; i < medals.Length && i < medalsForDLCLevel.Length; i++)
		{
			Mawaru_Medal mawaru_Medal = medals[i];
			int num2 = medalsForDLCLevel[i];
			if (num2 > 0)
			{
				mawaru_Medal.front.SetState(num2 - 1);
				if (num2 == 3)
				{
					num++;
				}
			}
			else
			{
				mawaru_Medal.front.render.enabled = false;
			}
			medals[i].transform.localScale = Vector3.zero;
		}
		string text = RDString.Get("levelSelect.taroWorldStats");
		text = text.Replace("[current]", num.ToString());
		text = text.Replace("[total]", medals.Length.ToString());
		medalCount.text = text;
		medalCount.SetLocalizedFont();
	}

	private void SwitchBetwenTaroStats(bool showTaroStats)
	{
		showingMedals = showTaroStats;
		statsTransition.Kill();
		Sequence t = DoMedalsAnimation(showTaroStats);
		if (showTaroStats)
		{
			statsTransition = DOTween.Sequence().Append(stats.DOFade(0f, 0.5f)).SetEase(Ease.Linear)
				.Append(stats.DOFade(1f, 0.5f))
				.SetEase(Ease.Linear)
				.Join(t);
		}
		else
		{
			statsTransition = DOTween.Sequence().Append(t).Join(stats.DOFade(0f, 0.5f))
				.SetEase(Ease.Linear)
				.Append(stats.DOFade(1f, 0.5f))
				.SetEase(Ease.Linear);
		}
		statsTransition.Join(DOVirtual.DelayedCall(0f, delegate
		{
			statsText.gameObject.SetActive(!showTaroStats);
			medalCount.gameObject.SetActive(showTaroStats);
		}));
	}

	private Sequence DoMedalsAnimation(bool show)
	{
		if (medalsAppear != null)
		{
			medalsAppear.Kill();
		}
		medalsAppear = DOTween.Sequence();
		for (int i = 0; i < medals.Length; i++)
		{
			Mawaru_Medal mawaru_Medal = medals[i];
			medalsAppear.Insert((float)i * 0.025f, mawaru_Medal.transform.DOScale(show ? Vector3.one : Vector3.zero, 0.1f).SetEase(show ? Ease.OutBack : Ease.InBack));
			if (show)
			{
				mawaru_Medal.transform.localScale = Vector3.zero;
			}
		}
		return medalsAppear;
	}
}
