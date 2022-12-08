using ADOFAI;
using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class scrFloor : ADOBase
{
	public const int SortingOrderOffset = 5;

	private const string ShaderAlphaPropertyName = "_Alpha";

	public const string gizmoIconFilename = "star.png";

	private const float NonStandardExtraLength = 0.03f;

	private const float NonStandardExtraWidth = 0.0375f;

	public const float DefaultLength = 0.75f;

	public const float DefaultWidth = 0.4125f;

	public static int ShaderProperty_Alpha;

	public static int ShaderProperty_Color;

	public static List<Vector4> randomOffsets = new List<Vector4>();

	[Header("Effects")]
	public FloorRenderer floorRenderer;

	public SpriteRenderer legacyFloorSpriteRenderer;

	public SpriteRenderer iconsprite;

	public SpriteRenderer outlineSprite;

	[NonSerialized]
	public SpriteRenderer topGlow;

	[NonSerialized]
	public SpriteRenderer bottomGlow;

	public scrLetterPress editorNumText;

	[Header("Game Properties")]
	public int seqID;

	public float speed = 1f;

	public float rotatecamera;

	public bool isLandable = true;

	public bool isCCW;

	public int tapsNeeded = 1;

	public int tapsSoFar;

	public bool midSpin;

	public Ease planetEase = Ease.Linear;

	public int planetEaseParts = 1;

	public EasePartBehavior planetEasePartBehavior;

	public int holdLength = -1;

	public bool unstable;

	public bool isSafe;

	public bool TaroDebug;

	[Header("Aesthetics")]
	public bool dontChangeMySprite;

	public FloorIcon floorIcon;

	public Color[] arrColorsBottomGlow;

	public bool hasLit;

	public bool customSpriteRange;

	public int minLitSprite;

	public int maxLitSprite;

	[Header("Portal")]
	public bool isportal;

	public int levelnumber = 999;

	public string arguments;

	[Header("Data")]
	public double exitangle;

	public double entryangle;

	public double angleLength;

	public double entryTime;

	public double entryTimePitchAdj;

	public scrFloor prevfloor;

	public scrFloor nextfloor;

	public int styleNum = -1;

	public List<ffxPlusBase> plusEffects = new List<ffxPlusBase>();

	public double entryBeat;

	[NonSerialized]
	public char stringDirection = 'S';

	[NonSerialized]
	public float floatDirection = -999f;

	[NonSerialized]
	public Color specialColor1;

	[NonSerialized]
	public Color specialColor2;

	[NonSerialized]
	public TrackColorType specialColorType;

	[NonSerialized]
	public float specialAnimDuration;

	[NonSerialized]
	public TrackColorPulse specialColorPulse;

	[NonSerialized]
	public float specialTimeOffset;

	[NonSerialized]
	public float opacity = 1f;

	public float holdOpacity = 1f;

	[NonSerialized]
	public float extendAnim = -1f;

	[NonSerialized]
	public bool isBookmarked;

	[NonSerialized]
	public Vector3 startPos;

	[NonSerialized]
	public Vector3 startRot;

	[NonSerialized]
	public Vector3 tweenRot;

	[NonSerialized]
	public Vector3 offsetPos;

	[NonSerialized]
	public Texture2D customTexture;

	[NonSerialized]
	public float customTextureScale;

	public TrackStyle initialTrackStyle;

	[NonSerialized]
	public bool hasConditionalChange;

	[NonSerialized]
	public List<ffxPlusBase> perfectEffects = new List<ffxPlusBase>();

	[NonSerialized]
	public List<ffxPlusBase> hitEffects = new List<ffxPlusBase>();

	[NonSerialized]
	public List<ffxPlusBase> barelyEffects = new List<ffxPlusBase>();

	[NonSerialized]
	public List<ffxPlusBase> missEffects = new List<ffxPlusBase>();

	[NonSerialized]
	public List<ffxPlusBase> lossEffects = new List<ffxPlusBase>();

	[NonSerialized]
	public Dictionary<TweenType, Tween> moveTweens = new Dictionary<TweenType, Tween>();

	[NonSerialized]
	public LevelEventType eventIcon;

	[NonSerialized]
	public ffxSetHitsound setHitsound;

	[NonSerialized]
	public ffxSetHitsound setGameSound;

	[NonSerialized]
	public List<scrDecoration> attachedDecorations = new List<scrDecoration>();

	[NonSerialized]
	public List<double> lastTaps = new List<double>();

	private float oriScaleTile = 5f;

	private float oriScaleBottomGlow;

	private new scrController controller;

	private new scrConductor conductor;

	private new RDConstants gc;

	private scrVfx vfx;

	private scrVfxPlus vfxPlus;

	private float opacityLastFrame = float.MinValue;

	private bool didRunStart;

	private bool meshChanged;

	private float baseLength = 0.75f;

	private float baseWidth = 0.4125f;

	public float lengthMult = 1f;

	public float widthMult = 1f;

	public GameObject holdGO;

	public float holdCompletion;

	public float holdCompletionEased;

	public scrHoldRenderer holdRenderer;

	public float holdDistance;

	public bool showHoldTiming;

	public float opacityVal = 1f;

	public float rotationOffset;

	public bool freeroam;

	public bool freeroamGenerated;

	public int freeroamRegion;

	public List<scrFloor> freeroamFloors = new List<scrFloor>();

	public float extraBeats;

	public float angleCorrectionType = -1f;

	public int freeroamEndEarlyBeats;

	public Vector2 freeroamOffset;

	public Vector2 freeroamDimensions;

	public Ease freeroamEndEase = Ease.InOutSine;

	public Collider2D coll;

	public bool collShouldBeEnabled = true;

	public int countdownTicks;

	public double marginScale = 1.0;

	public HitMargin grade;

	public float radiusScale = 1f;

	public bool isWarning;

	public bool auto;

	public bool hideJudgment;

	public bool hideIcon;

	public HitSound freeroamSoundOnBeat = HitSound.None;

	public HitSound freeroamSoundOffBeat = HitSound.None;

	public int numPlanets = 2;

	public bool isSwirl;

	public LineRenderer multiplanetLine;

	public List<scrPlanet> dummyPlanets = new List<scrPlanet>();

	public bool isTweening;

	public bool isFading;

	[NonSerialized]
	public Vector3 lastPos;

	[NonSerialized]
	public Vector3 lastRot;

	[NonSerialized]
	public Vector3 lastScale;

	[NonSerialized]
	public float forceUpdating;

	public Transform thisTransform;

	[NonSerialized]
	public Vector3 storedPos;

	[NonSerialized]
	public Vector3 storedRot;

	[NonSerialized]
	public Vector3 perpendicularAngleVector;

	[NonSerialized]
	public FreeroamArea freeroamArea;

	public Vector2Int freeroamPosition;

	private float iconScale = 1f;

	private float iconSpriteScale = 1f;

	private Sprite lastIconSprite;

	private Sprite lastIconOutlineSprite;

	private int randomNumberForLitAndGlow;

	private scrLevelMaker2 lm2 => ADOBase.lm.lm2;

	public void Awake()
	{
		controller = scrController.instance;
		conductor = scrConductor.instance;
		gc = RDConstants.data;
		vfx = scrVfx.instance;
		vfxPlus = scrVfxPlus.instance;
		startPos = base.transform.position;
		startRot = base.transform.rotation.eulerAngles;
		tweenRot = startRot;
		thisTransform = base.transform;
		if (floorRenderer == null)
		{
			floorRenderer = GetComponent<FloorRenderer>();
			if (floorRenderer == null)
			{
				floorRenderer = base.gameObject.AddComponent<FloorSpriteRenderer>();
				floorRenderer.renderer = GetComponent<SpriteRenderer>();
				legacyFloorSpriteRenderer = GetComponent<SpriteRenderer>();
			}
		}
		if (setHitsound == null)
		{
			setHitsound = GetComponent<ffxSetHitsound>();
		}
		if (controller.gameworld && (bool)bottomGlow)
		{
			bottomGlow.gameObject.SetActive(value: false);
		}
		GameObject gameObject = UnityEngine.Object.Instantiate(gc.prefab_topGlow, thisTransform);
		gameObject.transform.SetParent(thisTransform);
		gameObject.name = "topGlow";
		topGlow = gameObject.GetComponent<SpriteRenderer>();
		topGlow.transform.ScaleXY(dontChangeMySprite ? 0.32f : 1.28f);
		if (!controller.gameworld)
		{
			GameObject gameObject2 = UnityEngine.Object.Instantiate(gc.prefab_bottomGlow, thisTransform);
			gameObject2.transform.SetParent(thisTransform);
			gameObject2.name = "bottomGlow";
			bottomGlow = gameObject2.GetComponent<SpriteRenderer>();
			if (base.gameObject.GetComponent<BoxCollider2D>() == null)
			{
				base.gameObject.AddComponent<BoxCollider2D>().size = Vector2.one;
			}
		}
	}

	private void OnDrawGizmos()
	{
		if (rotatecamera != 0f || unstable || (freeroamGenerated && (isSwirl || isWarning)))
		{
			Gizmos.DrawIcon(base.transform.position, "star.png", allowScaling: true);
		}
	}

	public void CheckPortalSprite()
	{
		if (isportal)
		{
			SetIconSprite(gc.sprPortal);
		}
		else
		{
			SetIconSprite(null);
		}
	}

	public void Reset()
	{
		speed = 1f;
		rotatecamera = 0f;
		isLandable = true;
		isCCW = false;
		tapsNeeded = 1;
		tapsSoFar = 0;
		midSpin = false;
		planetEase = Ease.Linear;
		planetEaseParts = 1;
		planetEasePartBehavior = EasePartBehavior.Mirror;
		holdLength = -1;
		holdCompletion = 0f;
		holdCompletionEased = 0f;
		holdDistance = 0f;
		holdOpacity = 1f;
		extendAnim = -1f;
		numPlanets = 2;
		isSwirl = false;
		isWarning = false;
		dummyPlanets.Clear();
		dontChangeMySprite = false;
		floorIcon = FloorIcon.None;
		arrColorsBottomGlow = null;
		hasLit = false;
		customSpriteRange = false;
		minLitSprite = 0;
		maxLitSprite = 0;
		isportal = false;
		levelnumber = 999;
		arguments = null;
		isSafe = false;
		auto = false;
		hideJudgment = false;
		hideIcon = false;
		exitangle = 0.0;
		entryangle = 0.0;
		angleLength = 0.0;
		entryTime = 0.0;
		entryTimePitchAdj = 0.0;
		entryBeat = 0.0;
		nextfloor = null;
		styleNum = -1;
		plusEffects.Clear();
		freeroam = false;
		freeroamGenerated = false;
		freeroamRegion = 0;
		freeroamFloors.Clear();
		freeroamSoundOnBeat = HitSound.None;
		freeroamSoundOffBeat = HitSound.None;
		extraBeats = 0f;
		angleCorrectionType = -1f;
		freeroamEndEarlyBeats = 0;
		countdownTicks = 0;
		marginScale = 1.0;
		radiusScale = 1f;
		stringDirection = 'S';
		floatDirection = -999f;
		specialColor1 = Color.clear;
		specialColor2 = Color.clear;
		specialColorType = TrackColorType.Single;
		specialAnimDuration = 0f;
		specialColorPulse = TrackColorPulse.None;
		specialTimeOffset = 0f;
		opacity = 1f;
		isBookmarked = false;
		startPos = Vector3.zero;
		startRot = Vector3.zero;
		tweenRot = Vector3.zero;
		offsetPos = Vector3.zero;
		customTexture = null;
		customTextureScale = 0f;
		initialTrackStyle = TrackStyle.Standard;
		hasConditionalChange = false;
		perfectEffects.Clear();
		hitEffects.Clear();
		barelyEffects.Clear();
		missEffects.Clear();
		lossEffects.Clear();
		moveTweens.Clear();
		eventIcon = LevelEventType.AddComponent;
		attachedDecorations.Clear();
		lastTaps.Clear();
		oriScaleBottomGlow = 0f;
		opacityLastFrame = float.MinValue;
		iconScale = 1f;
		iconSpriteScale = 1f;
		lastIconSprite = null;
		lastIconOutlineSprite = null;
		if ((bool)iconsprite)
		{
			iconsprite.sprite = null;
		}
		if ((bool)outlineSprite)
		{
			outlineSprite.sprite = null;
		}
		if ((bool)topGlow)
		{
			UnityEngine.Object.Destroy(topGlow);
		}
		if ((bool)bottomGlow)
		{
			UnityEngine.Object.Destroy(bottomGlow);
		}
		if (Application.isPlaying)
		{
			Transform[] componentsInChildren = GetComponentsInChildren<Transform>();
			foreach (Transform transform in componentsInChildren)
			{
				if (transform.name == gc.animatedPortalPrefab.name)
				{
					UnityEngine.Object.Destroy(transform.gameObject);
				}
			}
		}
		Awake();
		Start();
		didRunStart = false;
	}

	public void Start()
	{
		didRunStart = true;
		if (!Application.isPlaying)
		{
			return;
		}
		UpdateGlowSortingOrder();
		CheckPortalSprite();
		if ((bool)bottomGlow)
		{
			oriScaleBottomGlow = bottomGlow.transform.localScale.x;
		}
		bool enabled = vfx.tileFlashStyle == TileFlashStyle.AlwaysOn || (hasLit && vfx.tileFlashStyle != TileFlashStyle.AlwaysBlack);
		if ((bool)bottomGlow)
		{
			bottomGlow.enabled = enabled;
		}
		topGlow.enabled = enabled;
		if (freeroamGenerated)
		{
			coll = UnityEngine.Object.Instantiate(gc.collider180, base.transform).GetComponent<Collider2D>();
			coll.enabled = true;
		}
		UpdateIconSprite();
		base.enabled = floorRenderer.renderer.isVisible;
		if (holdLength > -1 || (prevfloor != null && prevfloor.holdLength > -1))
		{
			base.enabled = true;
		}
		if (floorRenderer is FloorMeshRenderer)
		{
			while (seqID >= randomOffsets.Count)
			{
				Vector4 item = _003CStart_003Eg__RandomVector4_007C153_0();
				randomOffsets.Add(item);
			}
			Vector4 value = (controller.gameworld && !freeroam) ? randomOffsets[seqID] : _003CStart_003Eg__RandomVector4_007C153_0();
			floorRenderer.material.SetVector("_UV2Offset", value);
			baseLength = ADOBase.lm.baseFloorLength * lengthMult;
			baseWidth = ADOBase.lm.baseFloorWidth * widthMult;
		}
		else
		{
			(floorRenderer as FloorSpriteRenderer).material.SetVector("_UV2Offset", _003CStart_003Eg__RandomVector4_007C153_0());
		}
		if (controller.gameworld && !ADOBase.isLevelEditor && ADOBase.lm.useInitialTrackStyle)
		{
			SetTrackStyle(initialTrackStyle);
		}
	}

	public GameObject GenerateCollider()
	{
		GameObject original = ADOBase.editor.Collider180;
		int num = Mathf.RoundToInt((float)entryangle * 57.29578f);
		float num2 = Mathf.Abs(Mathf.DeltaAngle(target: Mathf.RoundToInt((float)exitangle * 57.29578f) % 360, current: num % 360));
		if (num2 <= 75f)
		{
			if (num2 <= 30f)
			{
				if (num2 != 15f)
				{
					if (num2 == 30f)
					{
						original = ADOBase.editor.Collider30;
					}
				}
				else
				{
					original = ADOBase.editor.Collider15;
				}
			}
			else if (num2 != 45f)
			{
				if (num2 != 60f)
				{
					if (num2 == 75f)
					{
						original = ADOBase.editor.Collider75;
					}
				}
				else
				{
					original = ADOBase.editor.Collider60;
				}
			}
			else
			{
				original = ADOBase.editor.Collider45;
			}
		}
		else if (num2 <= 120f)
		{
			if (num2 != 90f)
			{
				if (num2 != 105f)
				{
					if (num2 == 120f)
					{
						original = ADOBase.editor.Collider120;
					}
				}
				else
				{
					original = ADOBase.editor.Collider105;
				}
			}
			else
			{
				original = ADOBase.editor.Collider90;
			}
		}
		else if (num2 != 135f)
		{
			if (num2 != 165f)
			{
				if (num2 == 180f)
				{
					original = ADOBase.editor.Collider180;
				}
			}
			else
			{
				original = ADOBase.editor.Collider165;
			}
		}
		else
		{
			original = ADOBase.editor.Collider135;
		}
		return UnityEngine.Object.Instantiate(original, base.transform);
	}

	public void UpdateIconSprite()
	{
		if (!Application.isPlaying)
		{
			return;
		}
		if (scrController.isGameWorld && nextfloor == null)
		{
			SetIconSprite(gc.sprPortal);
			return;
		}
		if (Persistence.GetAnimateSpeedChange())
		{
			switch (floorIcon)
			{
			case FloorIcon.Rabbit:
				floorIcon = FloorIcon.AnimatedRabbit;
				break;
			case FloorIcon.DoubleRabbit:
				floorIcon = FloorIcon.AnimatedDoubleRabbit;
				break;
			case FloorIcon.Snail:
				floorIcon = FloorIcon.AnimatedSnail;
				break;
			case FloorIcon.DoubleSnail:
				floorIcon = FloorIcon.AnimatedDoubleSnail;
				break;
			}
		}
		SetIconScale(1f);
		SetIconAngle(0f);
		SetIconColor(Color.white);
		SetIconFlipped(flipped: false);
		if (hideIcon)
		{
			SetIconScale(0f);
		}
		switch (floorIcon)
		{
		case FloorIcon.Rabbit:
		case FloorIcon.AnimatedRabbit:
			SetIconSprite(gc.sprIconRabbit);
			SetIconOutlineSprite(gc.sprOutlineRabbit);
			break;
		case FloorIcon.DoubleRabbit:
		case FloorIcon.AnimatedDoubleRabbit:
			SetIconSprite(gc.sprIconDoubleRabbit);
			SetIconOutlineSprite(gc.sprOutlineDoubleRabbit);
			break;
		case FloorIcon.Snail:
		case FloorIcon.AnimatedSnail:
			SetIconSprite(gc.sprIconSnail);
			SetIconOutlineSprite(gc.sprOutlineSnail);
			break;
		case FloorIcon.DoubleSnail:
		case FloorIcon.AnimatedDoubleSnail:
			SetIconSprite(gc.sprIconDoubleSnail);
			SetIconOutlineSprite(gc.sprOutlineDoubleSnail);
			break;
		case FloorIcon.SameSpeed:
			if (controller.paused)
			{
				SetIconSprite(gc.sprIconSameSpeed);
				SetIconOutlineSprite(gc.sprOutlineSameSpeed);
			}
			else
			{
				SetIconSprite(((nextfloor == null && scrController.isGameWorld) || isportal) ? gc.sprPortal : null);
				SetIconOutlineSprite(null);
			}
			break;
		case FloorIcon.Vfx:
			if (controller.paused)
			{
				if (eventIcon == LevelEventType.None)
				{
					SetIconColor(scnEditor.instance.vfxIconColor);
					SetIconSprite(gc.sprIconVfx);
					break;
				}
				SetIconSprite(GCS.levelEventIcons[eventIcon]);
				SetIconScale(0.6f);
				SetIconColor(scnEditor.instance.vfxIconColor);
				if (floorRenderer is FloorMeshRenderer)
				{
					SetIconAngle(0f);
				}
				SetIconOutlineSprite(null);
			}
			else
			{
				SetIconSprite(((nextfloor == null && scrController.isGameWorld) || isportal) ? gc.sprPortal : null);
				SetIconOutlineSprite(null);
			}
			break;
		case FloorIcon.OneVfx:
			if (controller.paused)
			{
				SetIconSprite(gc.sprIconVfx);
				SetIconOutlineSprite(null);
			}
			else
			{
				SetIconSprite(((nextfloor == null && scrController.isGameWorld) || isportal) ? gc.sprPortal : null);
				SetIconOutlineSprite(null);
			}
			break;
		case FloorIcon.None:
			if (controller.isPuzzleRoom && isSwirl)
			{
				floorIcon = FloorIcon.Swirl;
			}
			if (controller.gameworld)
			{
				if (seqID != 0 && isCCW != ADOBase.lm.listFloors[seqID - 1].isCCW)
				{
					floorIcon = FloorIcon.Swirl;
					break;
				}
				SetIconSprite(((nextfloor == null && scrController.isGameWorld) || isportal) ? gc.sprPortal : null);
				SetIconOutlineSprite(null);
			}
			break;
		case FloorIcon.Checkpoint:
		{
			if (GCS.speedTrialMode)
			{
				SetIconSprite(null);
				break;
			}
			ffxCheckpoint component = GetComponent<ffxCheckpoint>();
			if (component == null)
			{
				UnityEngine.Debug.Log("Null checkpoint component??? At floor " + seqID.ToString());
			}
			if (component != null && GCS.checkpointNum >= seqID + component.checkpointTileOffset)
			{
				SetIconSprite(gc.sprIconCheckpointLit);
			}
			else
			{
				SetIconSprite(gc.sprIconCheckpoint);
			}
			SetIconOutlineSprite(gc.sprOutlineCheckpoint);
			break;
		}
		case FloorIcon.MultiPlanetThreeMore:
			SetIconSprite(gc.sprIconMultiPlanetThreeMore);
			SetIconOutlineSprite(gc.sprOutlineMultiPlanetThreeMore);
			break;
		case FloorIcon.MultiPlanetThreeLess:
			SetIconSprite(gc.sprIconMultiPlanetThreeLess);
			SetIconOutlineSprite(gc.sprOutlineMultiPlanetThreeLess);
			break;
		case FloorIcon.MultiPlanetTwo:
			SetIconSprite(gc.sprIconMultiPlanetTwo);
			SetIconOutlineSprite(gc.sprOutlineMultiPlanetTwo);
			break;
		case FloorIcon.HoldArrowLong:
			SetIconSprite(gc.sprIconHoldArrowLong);
			SetIconOutlineSprite(gc.sprOutlineHoldArrowLong);
			break;
		case FloorIcon.HoldArrowShort:
			SetIconSprite(gc.sprIconHoldArrowShort);
			SetIconOutlineSprite(gc.sprOutlineHoldArrowShort);
			break;
		case FloorIcon.HoldReleaseLong:
			SetIconSprite(gc.sprIconHoldReleaseLong);
			SetIconOutlineSprite(gc.sprOutlineHoldReleaseLong);
			break;
		case FloorIcon.HoldReleaseShort:
			SetIconSprite(gc.sprIconHoldReleaseShort);
			SetIconOutlineSprite(gc.sprOutlineHoldReleaseShort);
			break;
		default:
			SetIconSprite(null);
			SetIconOutlineSprite(null);
			break;
		}
		if (floorIcon == FloorIcon.Swirl)
		{
			float num = (float)scrMisc.GetAngleMoved((float)entryangle, (float)exitangle, !isCCW);
			if (Mathf.Abs(num) <= 1E-06f && !midSpin)
			{
				num = MathF.PI * 2f;
			}
			SetIconSprite((num < 3.14159179f) ? gc.sprIconSwirlRed : gc.sprIconSwirlBlue);
			SetIconFlipped(isCCW);
			if (freeroamGenerated)
			{
				SetIconFlipped(!isCCW);
			}
			if (freeroamGenerated)
			{
				if (isCCW)
				{
					SetIconSprite(gc.sprIconSwirlRed);
				}
				else
				{
					SetIconSprite(gc.sprIconSwirlBlue);
				}
			}
			float num2 = 0f;
			if (floorRenderer is FloorSpriteRenderer)
			{
				if (lm2 == null)
				{
					printe("lm2 is null for " + base.name);
				}
				float num3 = lm2.BigTiles ? (-MathF.PI / 2f) : (MathF.PI / 2f);
				num2 = (float)(((scrMisc.mod((float)(exitangle - entryangle), 6.2831854820251465) <= 3.1415927410125732) ? entryangle : exitangle) - (double)num3);
			}
			float num4 = 0f - (float)entryangle + MathF.PI / 2f - num / 2f * (float)((!isCCW) ? 1 : (-1)) - MathF.PI / 2f + num2;
			SetIconAngle((floorRenderer is FloorSpriteRenderer) ? num4 : (0f - num4));
			SetIconOutlineSprite(gc.sprOutlineSwirl);
		}
		if (floorIcon == FloorIcon.HoldArrowLong || floorIcon == FloorIcon.HoldArrowShort)
		{
			float num5 = (float)scrMisc.GetAngleMoved((float)entryangle, (float)exitangle, !isCCW);
			float num6 = 0f - (float)entryangle + MathF.PI / 2f - num5 * (float)((!isCCW) ? 1 : (-1));
			float num7 = (float)(exitangle - 1.5707963705062866);
			float num8 = -MathF.PI / 2f;
			num7 = ((!(scrMisc.mod((float)(exitangle - entryangle), 6.2831854820251465) <= 3.1415927410125732)) ? ((float)(exitangle - (double)num8)) : ((float)(entryangle - (double)num8)));
			float num9 = num6;
			SetIconAngle((floorRenderer is FloorSpriteRenderer) ? (num9 + num7) : (0f - num9));
		}
		if (outlineSprite != null)
		{
			outlineSprite.enabled = controller.usingOutlines;
		}
		opacityLastFrame = float.MinValue;
	}

	public void SetIconSprite(Sprite sprite)
	{
		if (sprite == lastIconSprite)
		{
			return;
		}
		lastIconSprite = sprite;
		if (floorRenderer is FloorMeshRenderer)
		{
			if (Application.isPlaying)
			{
				if (sprite != null)
				{
					floorRenderer.material.SetTexture("_IconTex", sprite.texture);
					iconSpriteScale = (float)sprite.texture.width * 1f / sprite.pixelsPerUnit;
					SetIconScale(iconScale);
				}
				else
				{
					floorRenderer.material.SetTexture("_IconTex", gc.tex_clear);
				}
			}
		}
		else if (iconsprite != null)
		{
			iconsprite.sprite = sprite;
		}
		else if (sprite != null)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(gc.prefab_floorIcon, thisTransform, worldPositionStays: false);
			gameObject.name = "floorIcon";
			iconsprite = gameObject.GetComponent<SpriteRenderer>();
			UpdateIconSortingOrder();
			iconsprite.sprite = sprite;
		}
	}

	public void SetIconOutlineSprite(Sprite sprite)
	{
		if (sprite == lastIconOutlineSprite)
		{
			return;
		}
		lastIconOutlineSprite = sprite;
		if (!controller.usingOutlines)
		{
			sprite = null;
		}
		if (floorRenderer is FloorMeshRenderer)
		{
			if (Application.isPlaying)
			{
				if (sprite != null)
				{
					floorRenderer.material.SetTexture("_IconOutlineTex", sprite.texture);
					iconSpriteScale = (float)sprite.texture.width * 1f / sprite.pixelsPerUnit;
					SetIconScale(iconScale);
				}
				else
				{
					floorRenderer.material.SetTexture("_IconOutlineTex", gc.tex_clear);
				}
			}
		}
		else if (outlineSprite != null)
		{
			outlineSprite.sprite = sprite;
		}
	}

	public void SetIconFlipped(bool flipped)
	{
		if (floorRenderer is FloorMeshRenderer)
		{
			if (Application.isPlaying)
			{
				floorRenderer.material.SetInt("_IconFlipped", flipped ? 1 : 0);
			}
			return;
		}
		if ((bool)iconsprite)
		{
			iconsprite.flipX = flipped;
		}
		if ((bool)outlineSprite)
		{
			outlineSprite.flipX = flipped;
		}
	}

	public void SetIconColor(Color color)
	{
		if (floorRenderer is FloorMeshRenderer)
		{
			if (Application.isPlaying)
			{
				floorRenderer.material.SetColor("_IconColor", color);
			}
		}
		else if ((bool)iconsprite)
		{
			iconsprite.color = color;
		}
	}

	public void SetIconAngle(float radians)
	{
		if (floorRenderer is FloorMeshRenderer)
		{
			if (Application.isPlaying)
			{
				floorRenderer.material.SetFloat("_IconAngle", radians);
			}
		}
		else if ((bool)iconsprite)
		{
			iconsprite.transform.localEulerAngles = new Vector3(0f, 0f, radians * 57.29578f);
		}
	}

	public void SetIconScale(float scale)
	{
		iconScale = scale;
		float num = scale * iconSpriteScale;
		if (floorRenderer is FloorMeshRenderer)
		{
			if (Application.isPlaying)
			{
				floorRenderer.material.SetFloat("_IconScale", num);
			}
		}
		else if ((bool)iconsprite)
		{
			iconsprite.transform.ScaleXY(num, num);
		}
	}

	public void UpdateCommentGlow(bool enabled)
	{
		if (Application.isPlaying && Persistence.GetMarkFloorWithComment())
		{
			Color color = "8300FF".HexToColor().WithAlpha(0.46f);
			floorRenderer.material.SetColor("_CommentColor", enabled ? color : Color.clear);
		}
	}

	public void LightUp(HitMargin hitmargin = HitMargin.Perfect)
	{
		hasLit = true;
		if (GCS.typingMode)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(gc.prefabLetterPress, base.transform.position, base.transform.rotation);
			string text = Input.inputString.ToUpper();
			scrLetterPress component = gameObject.GetComponent<scrLetterPress>();
			component.letterText.text = text;
			controller.typingLetters.Add(component);
		}
		if (!GCS.perfectOnlyMode)
		{
			SetToRandomColor();
		}
		else if (controller.gameworld)
		{
			if (hitmargin == HitMargin.EarlyPerfect)
			{
				floorRenderer.color = Color.blue;
			}
			if (hitmargin == HitMargin.LatePerfect)
			{
				floorRenderer.color = Color.red;
			}
			if (hitmargin == HitMargin.Perfect)
			{
				floorRenderer.color = Color.white;
			}
		}
		if (vfx.tileFlashStyle == TileFlashStyle.MoveToTopLayer)
		{
			(floorRenderer as FloorMeshRenderer).renderer.sortingLayerName = "FloorTop";
		}
		else if (vfx.tileFlashStyle != TileFlashStyle.AlwaysBlack)
		{
			if ((bool)bottomGlow)
			{
				bottomGlow.enabled = true;
			}
			topGlow.enabled = true;
		}
		foreach (scrDecoration attachedDecoration in attachedDecorations)
		{
			attachedDecoration.HitFloor();
		}
	}

	private void Update()
	{
		if (!didRunStart)
		{
			Start();
		}
		if (!Application.isPlaying)
		{
			return;
		}
		TrackColorType trackColorType = specialColorType;
		float num = Time.unscaledTime + specialTimeOffset;
		float num2 = specialAnimDuration;
		if (trackColorType != 0 || trackColorType != TrackColorType.Stripes)
		{
			switch (trackColorType)
			{
			case TrackColorType.Glow:
			{
				float t = (1f - Mathf.Cos(MathF.PI * 2f * (num / num2))) / 2f;
				floorRenderer.color = Color.Lerp(specialColor1, specialColor2, t);
				break;
			}
			case TrackColorType.Rainbow:
			{
				float a = floorRenderer.color.a;
				Color.RGBToHSV(floorRenderer.color, out float _, out float S, out float V);
				floorRenderer.color = Color.HSVToRGB(num / num2 % 1f, S, V).WithAlpha(a);
				break;
			}
			case TrackColorType.Blink:
			{
				float t2 = DOVirtual.EasedValue(0f, 1f, num % num2 / num2, Ease.Linear);
				floorRenderer.color = Color.Lerp(specialColor1, specialColor2, t2);
				break;
			}
			case TrackColorType.Switch:
				floorRenderer.color = ((num % num2 < num2 / 2f) ? specialColor1 : specialColor2);
				break;
			case TrackColorType.Volume:
			{
				float v = vfxPlus.vTrackerFloat.output;
				Sequence s = DOTween.Sequence();
				if (specialColorPulse == TrackColorPulse.None)
				{
					floorRenderer.color = Color.Lerp(specialColor1, specialColor2, v);
				}
				else
				{
					s.AppendInterval(specialAnimDuration - specialTimeOffset).AppendCallback(delegate
					{
						floorRenderer.color = Color.Lerp(specialColor1, specialColor2, v);
					});
				}
				break;
			}
			}
		}
		if (floorIcon != FloorIcon.AnimatedRabbit && floorIcon != FloorIcon.AnimatedSnail && floorIcon != FloorIcon.AnimatedDoubleRabbit && floorIcon != FloorIcon.AnimatedDoubleSnail)
		{
			return;
		}
		bool num3 = floorIcon == FloorIcon.AnimatedRabbit || floorIcon == FloorIcon.AnimatedDoubleRabbit;
		bool flag = floorIcon == FloorIcon.AnimatedDoubleRabbit || floorIcon == FloorIcon.AnimatedDoubleSnail;
		Sprite[] array = num3 ? gc.rabbitSpritesArr : gc.snailSpritesArr;
		if ((!ADOBase.isLevelEditor || !controller.paused) && controller.gameworld)
		{
			if (controller.currentSeqID >= seqID)
			{
				SetIconSprite(array[5 * (flag ? 1 : 0)]);
				SetIconOutlineSprite(null);
				return;
			}
			float num4 = 60f / (conductor.bpm * speed);
			float num5 = (float)conductor.songposition_minusi;
			int num6 = 1 + 2 * ((num5 - num4 * 2f * Mathf.Floor(num5 / (num4 * 2f)) < num4) ? 1 : 0) + 5 * (flag ? 1 : 0);
			SetIconSprite(array[num6]);
			SetIconOutlineSprite(array[num6 + 1]);
		}
	}

	private void LateUpdate()
	{
		if (!Application.isPlaying)
		{
			return;
		}
		if (nextfloor == null)
		{
			holdLength = -1;
		}
		if ((!controller.gameworld && (bool)controller.currFloor && !controller.currFloor.freeroamGenerated) || opacityLastFrame == opacity)
		{
			return;
		}
		floorRenderer.material.SetFloat(ShaderProperty_Alpha, opacity);
		if (!controller.paused)
		{
			SetIconColor(new Color(1f, 1f, 1f, opacity));
		}
		topGlow.color = new Color(1f, 1f, 1f, opacity * 0.8f);
		if (outlineSprite != null)
		{
			outlineSprite.color = new Color(1f, 1f, 1f, controller.usingOutlines ? opacity : 0f);
		}
		isTweening = (lastPos != base.transform.position || lastScale != floorRenderer.transform.localScale);
		isFading = (opacityLastFrame != opacity);
		lastPos = base.transform.position;
		lastScale = floorRenderer.transform.localScale;
		if (multiplanetLine != null)
		{
			if (isFading && controller.currentSeqID < seqID)
			{
				multiplanetLine.startColor = multiplanetLine.startColor.WithAlpha(opacity * 0.5f);
				multiplanetLine.endColor = multiplanetLine.endColor.WithAlpha(opacity * 0.5f);
				foreach (scrPlanet dummyPlanet in dummyPlanets)
				{
					dummyPlanet.SetPlanetColor(new Color(0.9f, 0.9f, 0.9f, opacity));
					dummyPlanet.SetTailColor(new Color(0.9f, 0.9f, 0.9f, opacity));
				}
			}
			if (isTweening)
			{
				multiplanetLine.transform.position = base.transform.position;
			}
		}
		opacityLastFrame = opacity;
	}

	private void OnBecameVisible()
	{
		base.enabled = true;
		if (scrController.isGameWorld)
		{
			if (nextfloor != null && holdLength > -1)
			{
				nextfloor.enabled = true;
			}
			if (prevfloor != null && prevfloor.holdLength > -1)
			{
				prevfloor.enabled = true;
			}
		}
		if (editorNumText != null && ADOBase.isLevelEditor && ADOBase.editor != null)
		{
			editorNumText.gameObject.SetActive(ADOBase.editor.showFloorNums && !ADOBase.editor.playMode);
			editorNumText.letterText.text = seqID.ToString();
		}
	}

	private void OnBecameInvisible()
	{
		if ((controller != null && ADOBase.isLevelEditor && controller.currentSeqID > seqID) || (scrController.instance != null && scrController.instance.currentSeqID > seqID) || holdLength < 0)
		{
			base.enabled = false;
		}
		base.enabled = false;
		if (editorNumText != null)
		{
			editorNumText.gameObject.SetActive(value: false);
		}
	}

	public override void OnBeat()
	{
		if (!dontChangeMySprite)
		{
			if (vfx.overrideTileSprites && !hasLit)
			{
				floorRenderer.sprite = vfx.unlitTile;
			}
			if (hasLit)
			{
				SetToRandomColor();
			}
		}
	}

	public void SetTileColor(Color color)
	{
		if (Application.isPlaying)
		{
			floorRenderer.color = color;
		}
	}

	public void SetToRandomColor()
	{
		if (dontChangeMySprite)
		{
			return;
		}
		if (vfx.tileFlashStyle == TileFlashStyle.Rando)
		{
			int maxExclusive = vfx.arrTileFlashColours.Length;
			floorRenderer.color = vfx.arrTileFlashColours[Mathf.FloorToInt(UnityEngine.Random.Range(0, maxExclusive))].WithAlpha(floorRenderer.color.a);
			topGlow.color = Color.clear;
			if ((bool)bottomGlow)
			{
				bottomGlow.color = topGlow.color;
			}
		}
		else if (vfx.tileFlashStyle == TileFlashStyle.Christmas)
		{
			topGlow.color = arrColorsBottomGlow[(conductor.beatNumber + seqID) % 2].WithAlpha(0.6f);
			if ((bool)bottomGlow)
			{
				bottomGlow.color = topGlow.color;
			}
		}
		else if (vfx.tileFlashStyle == TileFlashStyle.PureWhite)
		{
			topGlow.color = Color.white;
			if ((bool)bottomGlow)
			{
				bottomGlow.color = topGlow.color;
			}
		}
		else if (vfx.tileFlashStyle == TileFlashStyle.OnlyTopWhite && controller.gameworld)
		{
			topGlow.color = Color.white;
			topGlow.color = topGlow.color.WithAlpha(0.8f);
		}
		int num = customSpriteRange ? minLitSprite : 0;
		int num2 = customSpriteRange ? maxLitSprite : ((vfx.overrideStyle == TileOverrideStyle.Checkerboard) ? 1 : (vfx.arrLitTiles.Length - 1));
		if (vfx.overrideStyle == TileOverrideStyle.RandomChoice)
		{
			randomNumberForLitAndGlow = Mathf.FloorToInt(UnityEngine.Random.Range(num, num2));
		}
		else if (vfx.overrideStyle == TileOverrideStyle.Checkerboard)
		{
			randomNumberForLitAndGlow = ((Mathf.Abs(Mathf.Round(base.xGlobal) + Mathf.Round(base.yGlobal) + (float)conductor.beatNumber) % 2f == 1f) ? num : num2);
		}
		else
		{
			randomNumberForLitAndGlow = 0;
		}
		if (vfx.overrideTileSprites)
		{
			floorRenderer.sprite = vfx.arrLitTiles[randomNumberForLitAndGlow];
		}
		if (vfx.overrideGlowSprites)
		{
			int num3 = (vfx.arrTopGlowSprites.Length != vfx.arrLitTiles.Length) ? Mathf.FloorToInt(UnityEngine.Random.Range(0, vfx.arrLitTiles.Length)) : randomNumberForLitAndGlow;
			topGlow.sprite = vfx.arrTopGlowSprites[num3];
			if ((bool)bottomGlow)
			{
				bottomGlow.sprite = topGlow.sprite;
			}
		}
		if ((bool)bottomGlow)
		{
			bottomGlow.color = bottomGlow.color.WithAlpha(Mathf.Min(bottomGlow.color.a, floorRenderer.color.a));
		}
		topGlow.color = topGlow.color.WithAlpha(Mathf.Min(topGlow.color.a, floorRenderer.color.a));
	}

	public void SpawnPortalParticles()
	{
		if (Application.isPlaying)
		{
			scrPortalParticles component = UnityEngine.Object.Instantiate(gc.animatedPortalPrefab, base.transform).GetComponent<scrPortalParticles>();
			component.gameObject.name = gc.animatedPortalPrefab.name;
			int sortingOrder = floorRenderer.renderer.sortingOrder;
			component.glowSprite.sortingOrder = sortingOrder + 1;
			component.GetComponentInChildren<ParticleSystemRenderer>().sortingOrder = sortingOrder + 2;
			component.cap.sortingOrder = sortingOrder + 3;
			component.icon.sortingOrder = sortingOrder + 4;
			if (GCS.practiceMode)
			{
				component.color = Color.gray;
			}
			else
			{
				component.speedTrial = GCS.speedTrialMode;
			}
		}
	}

	public void UpdateAngle(bool rotate = true)
	{
		FloorMeshRenderer floorMeshRenderer = floorRenderer as FloorMeshRenderer;
		if (floorMeshRenderer != null)
		{
			float num = (MathF.PI / 2f - (float)entryangle) % (MathF.PI * 2f);
			float num2 = (MathF.PI / 2f - (float)exitangle) % (MathF.PI * 2f);
			floorMeshRenderer.SetAngle(num, num2);
			floorMeshRenderer.floorMesh._curvaturePoints = (midSpin ? 3 : 40);
		}
		else
		{
			SetSpriteFromChar(rotate);
		}
	}

	private void SetSpriteFromChar(bool rotate = true)
	{
		int num = Mathf.RoundToInt((float)scrMisc.getAcuteAngle(entryangle, exitangle) * 57.29578f);
		Sprite[] array = lm2.arrStraight;
		switch (num)
		{
		case 0:
			array = (midSpin ? lm2.arrMidspin : lm2.arr0);
			break;
		case 15:
			array = lm2.arr15;
			break;
		case 30:
			array = lm2.arr30;
			break;
		case 45:
			array = lm2.arr45;
			break;
		case 60:
			array = lm2.arr60;
			break;
		case 75:
			array = lm2.arr75;
			break;
		case 90:
			array = (lm2.BigTiles ? lm2.arr90 : lm2.arrBend);
			break;
		case 105:
			array = lm2.arr105;
			break;
		case 108:
			array = lm2.arr108;
			break;
		case 120:
			array = lm2.arr120;
			break;
		case 129:
			array = lm2.arr128;
			break;
		case 135:
			array = lm2.arr135;
			break;
		case 150:
			array = lm2.arr150;
			break;
		case 165:
			array = lm2.arr165;
			break;
		case 180:
			array = (lm2.BigTiles ? lm2.arr180 : lm2.arrStraight);
			break;
		}
		float num2 = lm2.BigTiles ? (-MathF.PI / 2f) : (MathF.PI / 2f);
		float rotation = (float)(((scrMisc.mod((float)(exitangle - entryangle), 6.2831854820251465) <= 3.1415927410125732) ? entryangle : exitangle) - (double)num2) * 57.29578f;
		if (!freeroamGenerated)
		{
			floorRenderer.sprite = ((styleNum == -1) ? array[scrMisc.GetRandInt(array.Length - 1)] : array[styleNum]);
		}
		else
		{
			floorRenderer.sprite = lm2.freeRoam;
		}
		if (rotate)
		{
			scrMisc.Rotate2DCW(floorRenderer.transform, rotation);
		}
	}

	public void SetTrackStyle(TrackStyle style, bool initial = false)
	{
		if (initial)
		{
			initialTrackStyle = style;
		}
		FloorMeshRenderer floorMeshRenderer = floorRenderer as FloorMeshRenderer;
		if (floorMeshRenderer == null)
		{
			return;
		}
		bool flag = style == TrackStyle.Gems;
		if (Application.isPlaying)
		{
			Texture2D texture2D = flag ? gc.tex_gem : ((customTexture != null) ? customTexture : ((style == TrackStyle.Standard) ? gc.tex_floorTileDefault : gc.tex_floorTileNone));
			float value = (customTexture != null) ? customTextureScale : 10f;
			Texture2D value2 = (style == TrackStyle.Standard) ? gc.tex_perlinDefault : gc.tex_perlinNone;
			float num = (baseLength + 0.03f) * lengthMult;
			float width = (baseWidth + 0.0375f) * widthMult;
			float value3 = 1f;
			Texture2D value5;
			Color value4;
			switch (style)
			{
			case TrackStyle.Basic:
				value5 = gc.tex_floorEdgeBasic;
				value4 = Color.clear;
				break;
			case TrackStyle.Minimal:
				value5 = gc.tex_floorEdgeMinimal;
				value4 = Color.clear;
				num /= lengthMult;
				num -= 0.03f;
				num *= lengthMult;
				break;
			case TrackStyle.Neon:
				value5 = ((tapsNeeded - tapsSoFar == 2) ? gc.tex_floorEdgeNeon2 : gc.tex_floorEdgeNeon);
				value4 = Color.white.WithAlpha(0.35f);
				break;
			case TrackStyle.NeonLight:
				value5 = gc.tex_floorEdgeNeonLight;
				value4 = Color.white.WithAlpha(0.35f);
				break;
			default:
				value4 = Color.black.WithAlpha(0.45f);
				value5 = gc.tex_floorEdgeDefault;
				num = 0.75f * lengthMult;
				width = baseWidth * widthMult;
				value3 = 1.23f;
				break;
			}
			Material material = floorMeshRenderer.material;
			material.SetTexture("_MainTex", value5);
			material.SetTexture("_TileTex", texture2D);
			material.SetFloat("_TileScale", value);
			material.SetFloat("_TextureRatio", (float)texture2D.width * 1f / (float)texture2D.height);
			material.SetTexture("_PerlinTex", value2);
			material.SetFloat("_ShadowTune", value3);
			material.SetColor("_ShadowColor", value4);
			if (!freeroamGenerated)
			{
				floorMeshRenderer.floorMesh._length = num;
				floorMeshRenderer.floorMesh._width = width;
			}
			floorMeshRenderer.floorMesh._isSprite = flag;
			material.SetInt("_Sprite", flag ? 1 : 0);
			if (freeroamArea != null)
			{
				foreach (scrFloor listFloor in freeroamArea.listFloors)
				{
					listFloor.SetTrackStyle(style, initial);
				}
			}
		}
	}

	public void ForceSnap()
	{
		controller.chosenplanet.transform.position = base.transform.position;
	}

	public Tween TweenRotation(float angle, float duration, Ease ease = Ease.Linear)
	{
		return DOTween.To(() => tweenRot.z, delegate(float r)
		{
			tweenRot.z = r;
		}, (startRot + new Vector3(0f, 0f, angle)).z, duration).SetEase(Ease.OutSine).OnUpdate(delegate
		{
			base.transform.eulerAngles = tweenRot;
		});
	}

	public Tween TweenOpacity(float opacity, float duration, Ease ease = Ease.Linear)
	{
		Tween result = null;
		if (duration > 0f)
		{
			result = DOTween.To(() => this.opacity, delegate(float o)
			{
				this.opacity = o;
			}, opacity, duration).SetEase(ease);
		}
		else
		{
			this.opacity = opacity;
		}
		dontChangeMySprite = true;
		return result;
	}

	public void SetColor(Color color)
	{
		if (Application.isPlaying)
		{
			floorRenderer.color = color;
			floorRenderer.deselectedColor = color;
			if (freeroamArea != null)
			{
				foreach (scrFloor listFloor in freeroamArea.listFloors)
				{
					listFloor.floorRenderer.color = floorRenderer.cachedColor;
				}
			}
		}
	}

	public void TweenColor(Color color, float duration, float delay = 0f, Ease ease = Ease.Linear)
	{
		if (Application.isPlaying)
		{
			DOTween.Sequence().AppendInterval(delay).Append(DOTween.To(() => floorRenderer.color, delegate(Color c)
			{
				floorRenderer.color = c;
			}, color, duration).SetEase(ease));
			DOTween.Sequence().AppendInterval(delay).Append(DOTween.To(() => floorRenderer.deselectedColor, delegate(Color c)
			{
				floorRenderer.deselectedColor = c;
			}, color, duration).SetEase(ease));
		}
	}

	public void MoveToBack()
	{
		SetSortingOrder(97);
	}

	public void MoveBackBy(int layers)
	{
		int sortingOrder = floorRenderer.renderer.sortingOrder - 5 * layers;
		SetSortingOrder(sortingOrder);
	}

	public void SetSortingOrder(int order)
	{
		floorRenderer.sortingOrder = order;
		if (outlineSprite != null)
		{
			outlineSprite.sortingOrder = order + 1;
		}
		UpdateIconSortingOrder();
		UpdateGlowSortingOrder();
	}

	private void UpdateGlowSortingOrder()
	{
		topGlow.sortingOrder = floorRenderer.sortingOrder + 10;
		if ((bool)bottomGlow)
		{
			bottomGlow.sortingOrder = floorRenderer.sortingOrder - 10 - 2;
		}
	}

	private void UpdateIconSortingOrder()
	{
		if (iconsprite != null)
		{
			iconsprite.sortingOrder = floorRenderer.sortingOrder + 2;
		}
	}

	public void ColorFloor(TrackColorType colorType, Color color1, Color color2, float animDuration, TrackColorPulse pulseType, float pulseLength, int startOfColorChange = -1)
	{
		VisualEffects visualEffect = controller.visualEffects;
		specialColorType = colorType;
		specialColor1 = color1;
		specialColor2 = color2;
		specialAnimDuration = animDuration;
		specialColorPulse = pulseType;
		FloorRenderer floorRenderer = this.floorRenderer;
		switch (colorType)
		{
		case TrackColorType.Single:
			if (startOfColorChange == -1)
			{
				return;
			}
			SetColor(color1);
			break;
		case TrackColorType.Stripes:
			if (startOfColorChange == -1)
			{
				return;
			}
			SetColor((freeroamGenerated ? ((freeroamPosition.x + freeroamPosition.y) % 2 == 0) : ((startOfColorChange - seqID) % 2 == 0)) ? color1 : color2);
			break;
		case TrackColorType.Glow:
			SetColor(Color.white);
			floorRenderer.color = color1;
			break;
		case TrackColorType.Blink:
			SetColor(Color.white);
			floorRenderer.color = color1;
			break;
		case TrackColorType.Switch:
			SetColor(Color.white);
			floorRenderer.color = color1;
			break;
		case TrackColorType.Rainbow:
		{
			float a = floorRenderer.color.a;
			SetColor(Color.white);
			Color.RGBToHSV(color1, out float _, out float S, out float V);
			floorRenderer.color = Color.HSVToRGB(0f, S, V).WithAlpha(a);
			break;
		}
		case TrackColorType.Volume:
			SetColor(Color.white);
			floorRenderer.color = color1;
			break;
		}
		switch (pulseType)
		{
		case TrackColorPulse.None:
			specialTimeOffset = 0f;
			break;
		case TrackColorPulse.Forward:
			specialTimeOffset = (1f - 1f / pulseLength * ((float)seqID % pulseLength)) * animDuration;
			break;
		case TrackColorPulse.Backward:
			specialTimeOffset = 1f / pulseLength * ((float)seqID % pulseLength) * animDuration;
			break;
		}
		if (freeroamArea != null)
		{
			Color cachedColor = this.floorRenderer.cachedColor;
			printe("setting floorrenderercolor to: " + cachedColor.ToString());
			foreach (scrFloor listFloor in freeroamArea.listFloors)
			{
				listFloor.ColorFloor(colorType, color1, color2, animDuration, pulseType, pulseLength, startOfColorChange);
			}
		}
	}

	public void SetRotation(float angle)
	{
		tweenRot = startRot + new Vector3(0f, 0f, angle);
		base.transform.eulerAngles = tweenRot;
	}

	public void SetOpacity(float opacity)
	{
		if (Application.isPlaying)
		{
			TweenOpacity(opacity, 0f);
		}
	}

	public void ToggleCollider(bool collEn)
	{
		collShouldBeEnabled = collEn;
		if (coll != null)
		{
			coll.enabled = collEn;
		}
	}

	[CompilerGenerated]
	private Vector4 _003CStart_003Eg__RandomVector4_007C153_0()
	{
		return new Vector4(base.randomFloat, base.randomFloat, base.randomFloat, base.randomFloat);
	}
}
