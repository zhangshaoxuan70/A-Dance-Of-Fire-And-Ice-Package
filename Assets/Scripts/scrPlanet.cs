// scrPlanet
using System;
using DG.Tweening;
using RDTools;
using UnityEngine;

public class scrPlanet : ADOBase
{
	public static readonly Color goldColor = new Color(1f, 0f, 0f, 0f);

	public static readonly Color rainbowColor = new Color(0f, 1f, 0f, 0f);

	public static readonly Color transPinkColor = new Color(0.5f, 0f, 0f, 0f);

	public static readonly Color transBlueColor = new Color(0f, 0.5f, 0f, 0f);

	public static readonly Color nbYellowColor = new Color(0.1f, 0f, 0f, 0f);

	public static readonly Color nbPurpleColor = new Color(0f, 0.1f, 0f, 0f);

	public static readonly Color overseerColor = new Color(0f, 0.2f, 0.3f, 0f);

	public float cosmeticRadius;

	public ParticleSystem deathExplosion;

	public scrPlanet other;

	public scrPlanet next;

	public scrPlanet prev;

	[NonSerialized]
	public bool toDelete;

	private int scrubbedStartPos;

	public double angle;

	public double targetExitAngle;

	public float cosmeticAngle;

	private double snappedLastAngle;

	public new scrConductor conductor;

	public new scrController controller;

	public Animator animator;

	public scrBackgroundBars bgbars;

	public scrCamera camy;

	public scrFloor currfloor;

	public SpriteRenderer glow;

	public Color startColor = Color.white;

	public bool setCustomColor;

	public RuntimeAnimatorController altController;

	public Sprite whiteSprite;

	private Sprite origSprite;

	public RuntimeAnimatorController origController;

	public ParticleSystem coreParticles;

	public ParticleSystem tailParticles;

	public ParticleSystem sparks;

	public GameObject objHittext;

	public SpriteRenderer ring;

	public SpriteRenderer sprite;

	public GameObject audioSourceObj;

	public AudioClip[] audioClips;

	public GameObject goldPlanet;

	public GameObject overseerPlanet;

	public bool isExtra;

	public SpriteRenderer samuraiSprite;

	private scrRing ringComp;

	public Vector3 storedPosition;

	public Vector3 holdOffsetPos;

	public SpriteRenderer faceSprite;

	public SpriteRenderer faceDetails;

	public Transform faceHolder;

	private Tween facePulseTween;

	private int faceIndex = -1;

	private bool faceMode;

	public float iFrames;

	[NonSerialized]
	public bool isRed;

	[NonSerialized]
	public scrPlanet redPlanet;

	[NonSerialized]
	public scrPlanet bluePlanet;

	public double cachedAngle;

	private bool hifi;

	private Sequence rainbowSeq;

	public int planetIndex = -1;

	public int attachedDummyFloor;

	public bool onlyRing;

	public bool dead;

	public bool hittable;

	public bool dummyPlanets;

	public PlanetColor previousPaintedColor;

	private AudioSource audioSource;

	private float aliveTime;

	private float endingTween;

	private float swirlTween;

	private float mpHoldTween;

	private bool FOOL_SWIRL;

	public bool shouldPrint;

	private double perc;

	private double easedPerc;

	private double dist;

	private double endpoint;

	private int easeParts;

	private EasePartBehavior easePartBehavior;

	private int loops;

	private float newStart;

	private Vector3 targetPosition;

	private Vector3 deathPos;

	private double deathAngle;

	private double initDeathAngle;

	private float randScatterAngle;

	private Vector3 addBob;

	private float bobRadius = 0.1f;

	private bool printed;

	private bool handledPositionAlready;

	private Vector3 movingFloorHoldOffset;

	private Vector3 actualPos;

	private Vector3 actualPivotPos;

	private Vector3 actualMPPos;

	private Vector3 oppositePos;

	private Vector3 oppositePivotPos;

	private Vector3 oppositeMPPos;

	private Vector3 tempTransPos;

	private Vector3 tempMPPivotPos;

	private scrPlanet p;

	private scrPlanet movingToNext;

	private scrPlanet lastMovedPlanet;

	private float lockTime;

	public double visualAngle => angle + (double)scrConductor.calibration_v - (double)scrConductor.calibration_i;

	private float radius => scrController.instance.startRadius;

	public bool isChosen => scrController.instance.chosenplanet == this;

	private void Awake()
	{
		ADOBase.Startup();
		ringComp = ring.GetComponent<scrRing>();
		origSprite = sprite.sprite;
		origController = animator.runtimeAnimatorController;
		audioSource = audioSourceObj.GetComponent<AudioSource>();
		isRed = base.name.Contains("Red");
		if (isRed)
		{
			redPlanet = this;
			bluePlanet = other;
		}
		else
		{
			redPlanet = other;
			bluePlanet = this;
		}
		onlyRing = false;
	}

	public void LiteRevive()
	{
		ring.enabled = true;
		glow.enabled = true;
		dead = false;
		tailParticles.Play();
		coreParticles.Play();
		sparks.Play();
		deathExplosion.gameObject.SetActive(value: false);
		faceHolder.gameObject.SetActive(value: false);
	}

	public void Rewind()
	{
		Revive();
		cosmeticRadius = 0f;
		angle = 0.0;
		snappedLastAngle = 0.0;
		currfloor = null;
		aliveTime = 0f;
		Start();
		void Revive()
		{
			sprite.enabled = true;
			ring.enabled = true;
			glow.enabled = true;
			dead = false;
			hittable = true;
			tailParticles.Play();
			coreParticles.Play();
			sparks.Play();
			deathExplosion.gameObject.SetActive(value: false);
			faceHolder.gameObject.SetActive(faceMode);
		}
	}

	private void Start()
	{
		if (!ADOBase.isEditingLevel)
		{
			LoadPlanetColor();
		}
		audioSource.Stop();
		controller = scrController.instance;
		conductor = scrConductor.instance;
		camy = GameObject.Find("Camera").GetComponent<scrCamera>();
		if (!isChosen)
		{
			cosmeticRadius = scrController.instance.startRadius;
		}
		endingTween = 0f;
		swirlTween = (FOOL_SWIRL ? 1 : 0);
		deathPos = Vector3.zero;
		holdOffsetPos = Vector3.zero;
		addBob = Vector3.zero;
		hifi = controller.visualQuality == VisualQuality.High;
	}

	public void SetColor(PlanetColor planetColor)
	{
		previousPaintedColor = planetColor;
		Color color = planetColor.GetColor();
		switch (planetColor)
		{
			case PlanetColor.DefaultRed:
				DisableCustomColor(0);
				DisableAllSpecialPlanets();
				Persistence.SetPlayerColor(Color.red, isRed);
				break;
			case PlanetColor.DefaultBlue:
				DisableCustomColor(1);
				DisableAllSpecialPlanets();
				Persistence.SetPlayerColor(Color.blue, isRed);
				break;
			case PlanetColor.Gold:
				DisableCustomColor();
				SwitchToGold();
				Persistence.SetPlayerColor(goldColor, isRed);
				break;
			case PlanetColor.Rainbow:
				EnableCustomColor();
				SetRainbow(enabled: true);
				Persistence.SetPlayerColor(rainbowColor, isRed);
				break;
			case PlanetColor.Overseer:
				DisableCustomColor();
				SwitchToOverseer();
				Persistence.SetPlayerColor(overseerColor, isRed);
				break;
			default:
				EnableCustomColor();
				SetPlanetColor(color);
				SetTailColor(color);
				Persistence.SetPlayerColor(color, isRed);
				break;
		}
	}

	public void SwitchToGold()
	{
		ToggleSpecialPlanet(goldPlanet, on: true, "Gold");
		SetExplosionColor(ADOBase.gc.goldExplosionColor1, ADOBase.gc.goldExplosionColor2);
		SetFaceColor(ring.color.WithAlpha(1f));
	}

	public void RemoveGold()
	{
		ToggleSpecialPlanet(goldPlanet, on: false);
	}

	public void SwitchToOverseer()
	{
		ToggleSpecialPlanet(overseerPlanet, on: true, "Ov");
		SetExplosionColor(ADOBase.gc.overseerExplosionColor1, ADOBase.gc.overseerExplosionColor2);
		SetFaceColor(ring.color.WithAlpha(1f));
	}

	public void RemoveOverseer()
	{
		ToggleSpecialPlanet(overseerPlanet, on: false);
	}

	public void ToggleSpecialPlanet(GameObject specialP, bool on, string specialSuffix = "")
	{
		if (on)
		{
			DisableAllSpecialPlanets();
		}
		specialP.gameObject.SetActive(on);
		Transform t = (on ? specialP.transform : base.transform);
		if (on)
		{
			SetBasePlanetElementsActive(active: false);
			SetupNewPlanetElements(specialSuffix);
		}
		else
		{
			SetupNewPlanetElements("");
			SetBasePlanetElementsActive(active: true);
		}
		void SetBasePlanetElementsActive(bool active)
		{
			tailParticles.gameObject.SetActive(active);
			sparks.gameObject.SetActive(active);
			coreParticles.gameObject.SetActive(active);
			ring.gameObject.SetActive(active);
			glow.gameObject.SetActive(active);
			sprite.enabled = active;
		}
		void SetupNewPlanetElements(string suffix)
		{
			coreParticles = t.Find("Core" + suffix)?.GetComponent<ParticleSystem>() ?? coreParticles;
			tailParticles = t.Find("Tail" + suffix).GetComponent<ParticleSystem>() ?? tailParticles;
			sparks = t.Find("Sparks" + suffix).GetComponent<ParticleSystem>() ?? sparks;
			ring = t.Find("Ring" + suffix).GetComponent<SpriteRenderer>() ?? ring;
			glow = t.Find("Glow" + suffix).GetComponent<SpriteRenderer>() ?? glow;
			sprite = (on ? t.Find("Planet" + suffix) : base.transform).GetComponent<SpriteRenderer>() ?? sprite;
		}
	}

	public void DisableAllSpecialPlanets()
	{
		if (goldPlanet.activeSelf)
		{
			RemoveGold();
		}
		if (overseerPlanet.activeSelf)
		{
			RemoveOverseer();
		}
	}

	public void EnableCustomColor()
	{
		DisableAllSpecialPlanets();
		sprite.sprite = whiteSprite;
		animator.runtimeAnimatorController = altController;
	}

	public void DisableCustomColor(int defaultColor = -1)
	{
		SetRainbow(enabled: false);
		DisableAllSpecialPlanets();
		sprite.color = Color.white;
		switch (defaultColor)
		{
			case -1:
				{
					animator.runtimeAnimatorController = origController;
					sprite.sprite = origSprite;
					Color color = (isRed ? Color.red : Color.blue);
					SetRingColor(color);
					SetTailColor(color);
					SetFaceColor(color);
					break;
				}
			case 0:
				animator.runtimeAnimatorController = redPlanet.origController;
				sprite.sprite = redPlanet.origSprite;
				SetRingColor(Color.red);
				SetCoreColor(Color.red);
				SetTailColor(Color.red);
				SetFaceColor(Color.red);
				break;
			case 1:
				animator.runtimeAnimatorController = bluePlanet.origController;
				sprite.sprite = bluePlanet.origSprite;
				SetRingColor(Color.blue);
				SetCoreColor(Color.blue);
				SetTailColor(Color.blue);
				SetFaceColor(Color.blue);
				break;
		}
	}

	public void SetPlanetColor(Color color)
	{
		if (sprite != null)
		{
			SetRainbow(enabled: false);
			sprite.color = color;
			SetRingColor(color);
			SetCoreColor(color);
			SetFaceColor(color);
		}
	}

	private void SetRingColor(Color color)
	{
		ring.color = color.WithAlpha(0.4f);
	}

	public void SetTailColor(Color color)
	{
		if (sprite != null)
		{
			SetParticleSystemColor(tailParticles, color, color * new Color(0.5f, 0.5f, 0.5f));
			SetExplosionColor(color);
		}
	}

	public void SetExplosionColor(Color color, Color color2 = default(Color))
	{
		ParticleSystem.MainModule main = deathExplosion.main;
		Gradient gradient = new Gradient();
		Color col = ((color2 != Color.clear) ? color2 : color);
		gradient.SetKeys(new GradientColorKey[3]
		{
			new GradientColorKey(color, 0f),
			new GradientColorKey(color, 0.5f),
			new GradientColorKey(col, 1f)
		}, new GradientAlphaKey[2]
		{
			new GradientAlphaKey(1f, 0f),
			new GradientAlphaKey(1f, 1f)
		});
		gradient.mode = GradientMode.Fixed;
		ParticleSystem.MinMaxGradient minMaxGradient = new ParticleSystem.MinMaxGradient(gradient);
		minMaxGradient.mode = ParticleSystemGradientMode.RandomColor;
		main.startColor = minMaxGradient;
	}

	public Color GetPlanetColor(Color color)
	{
		return sprite.color;
	}

	public Color GetTailColor(Color color)
	{
		return tailParticles.main.startColor.color;
	}

	public void SetCoreColor(Color color)
	{
		glow.color = color.WithAlpha(glow.color.a * color.a);
		ParticleSystem.MainModule main = coreParticles.main;
		main.startColor = new ParticleSystem.MinMaxGradient(color);
		SetParticleSystemColor(coreParticles, color, color);
	}

	private void SetParticleSystemColor(ParticleSystem particleSystem, Color baseColor, Color startColor)
	{
		ParticleSystem.MainModule main = particleSystem.main;
		main.startColor = new ParticleSystem.MinMaxGradient(startColor);
		Color color = Color.Lerp(baseColor, Color.black, 0.4f);
		ParticleSystem.ColorOverLifetimeModule colorOverLifetime = particleSystem.colorOverLifetime;
		Gradient gradient = new Gradient();
		gradient.SetKeys(new GradientColorKey[2]
		{
			new GradientColorKey(baseColor, 0f),
			new GradientColorKey(baseColor * new Color(0.6f, 0.6f, 0.6f), 1f)
		}, new GradientAlphaKey[2]
		{
			new GradientAlphaKey(0.4f, 0f),
			new GradientAlphaKey(0f, 1f)
		});
		Gradient gradient2 = new Gradient();
		gradient2.SetKeys(new GradientColorKey[2]
		{
			new GradientColorKey(color, 0f),
			new GradientColorKey(color * new Color(0.6f, 0.6f, 0.6f), 1f)
		}, new GradientAlphaKey[2]
		{
			new GradientAlphaKey(0.6f, 0f),
			new GradientAlphaKey(0f, 1f)
		});
		ParticleSystem.MinMaxGradient color2 = new ParticleSystem.MinMaxGradient(gradient, gradient2);
		color2.mode = ParticleSystemGradientMode.TwoGradients;
		colorOverLifetime.color = color2;
	}

	private void SetFaceColor(Color color)
	{
		faceSprite.color = color;
	}

	public void SetRainbow(bool enabled)
	{
		if (!enabled)
		{
			if (rainbowSeq != null)
			{
				rainbowSeq.Kill();
			}
		}
		else
		{
			if (rainbowSeq != null && rainbowSeq.IsActive())
			{
				return;
			}
			sprite.color = Color.red;
			Color.RGBToHSV(Color.red, out var _, out var S, out var V);
			Tween[] array = new Tween[10];
			rainbowSeq = DOTween.Sequence();
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = sprite.DOColor(Color.HSVToRGB(0.1f + 0.1f * (float)i, S, V), 0.5f);
				rainbowSeq.Append(array[i]);
			}
			rainbowSeq.SetLoops(-1, LoopType.Restart).SetUpdate(isIndependentUpdate: true);
			rainbowSeq.OnUpdate(delegate
			{
				if (ring != null)
				{
					SetRingColor(sprite.color);
					SetTailColor(sprite.color);
					SetCoreColor(sprite.color);
					SetFaceColor(sprite.color);
				}
				else
				{
					rainbowSeq.Kill();
				}
			});
		}
	}

	public void LoadPlanetColor()
	{
		if (isExtra)
		{
			return;
		}
		SetRainbow(enabled: false);
		if (Persistence.GetSamuraiMode(isRed))
		{
			ToggleSamurai(enabled: true);
		}
		SetFaceMode(Persistence.GetFaceMode(isRed));
		Color playerColor = Persistence.GetPlayerColor(isRed);
		_ = Color.white;
		if (playerColor == goldColor || GCS.d_forceGoldPlanets)
		{
			DisableAllSpecialPlanets();
			SwitchToGold();
			new Color(1f, 0.8078431f, 0.1607843f);
		}
		else if (playerColor == overseerColor)
		{
			DisableAllSpecialPlanets();
			SwitchToOverseer();
			new Color(0.1058824f, 0.6470588f, 0.7843137f);
		}
		else if (playerColor == rainbowColor)
		{
			EnableCustomColor();
			SetRainbow(enabled: true);
		}
		else if (playerColor == Color.red || playerColor == Color.blue)
		{
			int defaultColor = ((!(playerColor == Color.red)) ? 1 : 0);
			DisableCustomColor(defaultColor);
		}
		else
		{
			EnableCustomColor();
			Color planetColor = playerColor;
			Color tailColor = playerColor;
			if (playerColor == transBlueColor)
			{
				planetColor = new Color(0.3607843f, 67f / 85f, 0.9294118f);
				tailColor = Color.white;
			}
			else if (playerColor == transPinkColor)
			{
				planetColor = new Color(0.9568627f, 0.6431373f, 0.7098039f);
				tailColor = Color.white;
			}
			else if (playerColor == nbYellowColor)
			{
				planetColor = new Color(0.996f, 0.953f, 0.18f);
				tailColor = Color.white;
			}
			else if (playerColor == nbPurpleColor)
			{
				planetColor = new Color(0.612f, 0.345f, 0.82f);
				tailColor = Color.black;
			}
			SetPlanetColor(planetColor);
			SetTailColor(tailColor);
		}
		if (scrLogoText.instance != null)
		{
			scrLogoText.instance.UpdateColors();
		}
	}

	public void ToggleSamurai(bool enabled)
	{
		samuraiSprite.gameObject.SetActive(enabled);
		GetComponent<SpriteRenderer>().maskInteraction = (enabled ? SpriteMaskInteraction.VisibleInsideMask : SpriteMaskInteraction.None);
		ToggleAllSpecialPlanetsSamuraiMode(enabled);
		SetFaceMode(!enabled && Persistence.GetFaceMode(isRed));
	}

	private void ToggleSpecialPlanetSamuraiMode(GameObject specialP, bool on, string specialSuffix = "")
	{
		specialP.transform.Find("Planet" + specialSuffix).GetComponent<SpriteRenderer>().maskInteraction = (on ? SpriteMaskInteraction.VisibleInsideMask : SpriteMaskInteraction.None);
	}

	private void ToggleAllSpecialPlanetsSamuraiMode(bool on)
	{
		ToggleSpecialPlanetSamuraiMode(goldPlanet, on, "Gold");
		ToggleSpecialPlanetSamuraiMode(overseerPlanet, on, "Ov");
	}

	public void SetFaceMode(bool enabled, bool pulseOnEnable = false)
	{
		if (!enabled || !samuraiSprite.gameObject.activeSelf)
		{
			faceMode = enabled;
			faceHolder.gameObject.SetActive(enabled);
			if (enabled)
			{
				ChangeFace(pulseOnEnable);
			}
		}
	}

	public void ChangeFace(bool pulse)
	{
		if (!faceMode)
		{
			return;
		}
		int num = ADOBase.gc.faceBaseSprites.Length - 1;
		int num2 = 0;
		int num3;
		do
		{
			num3 = ((UnityEngine.Random.Range(0, 1000) != 0) ? UnityEngine.Random.Range(0, num) : num);
			num2++;
		}
		while (num2 <= 100 && num3 == faceIndex);
		faceIndex = num3;
		faceSprite.sprite = ADOBase.gc.faceBaseSprites[faceIndex];
		faceDetails.sprite = ADOBase.gc.faceBaseDetailSprites[faceIndex];
		if (pulse)
		{
			if (facePulseTween != null)
			{
				facePulseTween.Kill();
			}
			faceHolder.ScaleXY(1.25f, 1.25f);
			facePulseTween = faceHolder.DOScale(Vector3.one, 0.33f).SetEase(Ease.OutCubic);
		}
	}

	private void PrintImportantVariables()
	{
		string text = "\n";
		string o = "planet: " + base.gameObject.name + text + "angle: " + angle + text + "snappedLastAngle: " + snappedLastAngle + text + "conductor.songposition_minusi: " + conductor.songposition_minusi + text + "conductor.lastHit: " + conductor.lastHit + text + "conductor.crotchet: " + conductor.crotchet + text + "controller.speed: " + controller.speed + text + "controller.isCW: " + controller.isCW;
		printe(o);
	}

	private void Update()
	{
		if (dummyPlanets)
		{
			return;
		}
		if (iFrames > 0f)
		{
			iFrames -= Time.deltaTime;
		}
		if (aliveTime < 60f)
		{
			aliveTime += Time.deltaTime;
		}
		if (iFrames <= 0f && hittable && !dead && !RDC.auto)
		{
			Collider2D[] array = Physics2D.OverlapCircleAll(base.transform.position, 0.3f);
			foreach (Collider2D collider2D in array)
			{
				if (collider2D.gameObject.tag == "HitDecoration")
				{
					scrDecoration component = collider2D.transform.parent.GetComponent<scrDecoration>();
					if (component.canHitPlanets && !component.hitOnce)
					{
						component.hitOnce = true;
						Die();
						controller.FailByHitbox();
					}
				}
			}
		}
		scrMisc.ScaleSymmetric2D(base.transform, 0.9f);
		Update_RefreshAngles();
		ParticleSystem.EmissionModule emission = coreParticles.emission;
		emission.enabled = isChosen;
		if (controller.state == States.PlayerControl)
		{
			holdOffsetPos = Vector3.zero;
			addBob = Vector3.zero;
		}
		if (controller.state == States.PlayerControl && currfloor != null && currfloor.nextfloor != null)
		{
			deathAngle = Mathf.Atan2(currfloor.nextfloor.transform.position.x - currfloor.transform.position.x, currfloor.nextfloor.transform.position.y - currfloor.transform.position.y);
			initDeathAngle = angle;
			randScatterAngle = UnityEngine.Random.Range(-MathF.PI / 6f, MathF.PI / 6f);
		}
		if (currfloor != null && currfloor.holdLength > -1 && isChosen && controller.state != States.Start && controller.state != States.Won)
		{
			movingToNext = next;
			if (scrController.isGameWorld)
			{
				if (controller.state != States.Fail && controller.state != States.Fail2 && (bool)currfloor.nextfloor)
				{
					perc = (conductor.songposition_minusi - currfloor.entryTime) / (currfloor.nextfloor.entryTime - currfloor.entryTime);
				}
			}
			else
			{
				double entryTime = currfloor.entryTime;
				double num = entryTime + conductor.crotchet * (double)(currfloor.holdLength * 2 + 1);
				perc = controller.speed * (conductor.songposition_minusi - entryTime) / (num - entryTime);
			}
			if (!printed)
			{
				printed = true;
			}
			if (perc < 0.0)
			{
				perc = 0.0;
			}
			easedPerc = perc;
			endpoint = 1.0;
			if (controller.rotationEase != Ease.Linear && currfloor.planetEaseParts > 0)
			{
				easeParts = currfloor.planetEaseParts;
				easePartBehavior = currfloor.planetEasePartBehavior;
				easedPerc *= easeParts;
				endpoint /= easeParts;
				loops = Mathf.FloorToInt(Mathf.Abs((float)easedPerc));
				newStart = (float)loops / (float)easeParts;
				if (loops % 2 == 0 || easePartBehavior == EasePartBehavior.Repeat)
				{
					easedPerc = DOVirtual.EasedValue(newStart, newStart + (float)endpoint, (float)easedPerc - (float)loops, controller.rotationEase);
				}
				else
				{
					easedPerc = newStart + (float)endpoint - DOVirtual.EasedValue(0f, (float)endpoint, 1f - ((float)easedPerc - (float)loops), controller.rotationEase);
				}
			}
			if (!controller.stickToFloor)
			{
				targetPosition = currfloor.nextfloor.startPos - scrController.instance.startRadius * currfloor.nextfloor.radiusScale * new Vector3(Mathf.Sin((float)currfloor.exitangle), Mathf.Cos((float)currfloor.exitangle), 0f);
				holdOffsetPos = (float)easedPerc * (targetPosition - currfloor.startPos) + deathPos;
			}
			else
			{
				targetPosition = currfloor.nextfloor.transform.position - scrController.instance.startRadius * currfloor.nextfloor.radiusScale * new Vector3(Mathf.Sin((float)currfloor.exitangle), Mathf.Cos((float)currfloor.exitangle), 0f);
				movingFloorHoldOffset = currfloor.transform.position - storedPosition;
				holdOffsetPos = (float)easedPerc * (targetPosition - currfloor.transform.position) + deathPos + movingFloorHoldOffset;
			}
			currfloor.holdCompletion = (float)perc;
			currfloor.holdCompletionEased = (float)easedPerc;
			if (controller.state == States.PlayerControl && currfloor.holdDistance > 0f)
			{
				addBob = scrController.instance.startRadius * bobRadius * new Vector3(Mathf.Sin((float)angle - (float)currfloor.exitangle), Mathf.Sin((float)angle - (float)currfloor.exitangle), 0f);
			}
			if (controller.state == States.Fail && perc < 0.99)
			{
				if (!controller.stickToFloor)
				{
					deathPos += Time.deltaTime * ((Mathf.Sin((float)initDeathAngle - (float)currfloor.exitangle) < 0f) ? 8f : (-8f)) * new Vector3(Mathf.Sin((float)currfloor.exitangle + randScatterAngle - MathF.PI / 2f), Mathf.Cos((float)currfloor.exitangle + randScatterAngle - MathF.PI / 2f), 0f);
				}
				else
				{
					deathPos += Time.deltaTime * ((Mathf.Sin((float)initDeathAngle - (float)deathAngle) < 0f) ? 8f : (-8f)) * new Vector3(Mathf.Sin((float)deathAngle + randScatterAngle - MathF.PI / 2f), Mathf.Cos((float)deathAngle + randScatterAngle - MathF.PI / 2f), 0f);
				}
			}
			if (next.goldPlanet.activeSelf)
			{
				currfloor.holdRenderer.touchColor = scrMisc.PlayerColorToRealColor(goldColor);
			}
			else if (next.overseerPlanet.activeSelf)
			{
				currfloor.holdRenderer.touchColor = scrMisc.PlayerColorToRealColor(overseerColor);
			}
			else
			{
				currfloor.holdRenderer.touchColor = next.coreParticles.main.startColor.color;
			}
			currfloor.holdRenderer.UpdateColor((float)easedPerc);
			base.transform.position = storedPosition + addBob + holdOffsetPos;
			if (camy.followMode && controller.state != States.Fail && controller.state != States.Fail2)
			{
				camy.SetHoldOffset(holdOffsetPos);
			}
		}
		handledPositionAlready = false;
	}

	private void LateUpdate()
	{
		if (faceMode)
		{
			faceHolder.transform.rotation = Quaternion.Euler(0f, 0f, camy.transform.rotation.eulerAngles.z);
		}
		if (controller.stickToFloor && currfloor != null && isChosen && !handledPositionAlready && currfloor.holdLength < 0)
		{
			base.transform.position = currfloor.transform.position + holdOffsetPos + addBob;
		}
	}

	public void Update_RefreshAngles()
	{
		if (conductor.crotchet == 0.0 || !isChosen)
		{
			return;
		}
		if (!GCS.d_stationary)
		{
			angle = snappedLastAngle + (conductor.songposition_minusi - conductor.lastHit) / conductor.crotchet * 3.1415927410125732 * controller.speed * (double)(controller.isCW ? 1 : (-1));
			if (shouldPrint)
			{
				PrintImportantVariables();
				shouldPrint = false;
			}
		}
		else
		{
			if (Input.GetKey(KeyCode.DownArrow))
			{
				angle += 0.10000000149011612;
			}
			if (Input.GetKey(KeyCode.UpArrow))
			{
				angle -= 0.10000000149011612;
			}
		}
		float num = (float)angle + cosmeticAngle;
		float num2 = 2f;
		float num3 = 2f;
		float num4 = num;
		float num5 = 0f;
		float num6 = 0f;
		double num7 = 0.0;
		float num8 = 0f;
		scrFloor scrFloor2 = null;
		movingToNext = next;
		bool flag = false;
		if (currfloor != null)
		{
			scrFloor2 = (scrController.isGameWorld ? ((currfloor.seqID > 0) ? scrLevelMaker.instance.listFloors[currfloor.seqID - 1] : currfloor) : currfloor);
			if (currfloor.nextfloor != null && currfloor.nextfloor.entryTime - currfloor.entryTime > 0.0)
			{
				num7 = (conductor.songposition_minusi - currfloor.entryTime) / (currfloor.nextfloor.entryTime - currfloor.entryTime);
			}
			num2 = currfloor.numPlanets;
			if (currfloor.nextfloor != null && currfloor.numPlanets < currfloor.nextfloor.numPlanets && scrFloor2.numPlanets > currfloor.numPlanets)
			{
				num6 = Mathf.Lerp((float)scrFloor2.numPlanets - (float)currfloor.numPlanets, 0f, (float)num7);
				num3 = scrFloor2.numPlanets;
				flag = true;
			}
			else if (currfloor.nextfloor != null && currfloor.numPlanets > currfloor.nextfloor.numPlanets && scrFloor2.numPlanets < currfloor.numPlanets)
			{
				num3 = currfloor.numPlanets;
			}
			else if (currfloor.nextfloor != null && currfloor.numPlanets < currfloor.nextfloor.numPlanets)
			{
				num3 = Mathf.Lerp(currfloor.numPlanets, currfloor.nextfloor.numPlanets, (float)num7);
			}
			else if (scrFloor2.numPlanets > currfloor.numPlanets)
			{
				num3 = Mathf.Lerp(scrFloor2.numPlanets, currfloor.numPlanets, (float)num7);
				num6 = Mathf.Lerp((float)scrFloor2.numPlanets - (float)currfloor.numPlanets, 0f, (float)num7);
			}
			else
			{
				num3 = currfloor.numPlanets;
			}
			if (flag)
			{
				if (num6 > 0f && num2 > 2f)
				{
					num8 = (float)((!currfloor.isCCW) ? 1 : (-1)) * ((float)scrMisc.GetInverseAnglePerBeatMultiplanet(num3) - (float)scrMisc.GetInverseAnglePerBeatMultiplanet(num2 + ((float)(scrFloor2.numPlanets - currfloor.numPlanets) - num6)));
				}
			}
			else if (num6 > 0f && num2 > 2f)
			{
				num8 = (float)((!currfloor.isCCW) ? 1 : (-1)) * ((float)scrMisc.GetInverseAnglePerBeatMultiplanet(num3) - (float)scrMisc.GetInverseAnglePerBeatMultiplanet(num2));
			}
			num += num8;
			if (currfloor.holdLength > -1)
			{
				num -= (1f - (float)num7) * mpHoldTween * (float)scrMisc.GetInverseAnglePerBeatMultiplanet(currfloor.numPlanets);
			}
			num4 = num - (float)((!currfloor.isCCW) ? 1 : (-1)) * (float)scrMisc.GetInverseAnglePerBeatMultiplanet(num3) / 2f;
			if (num2 > 2f)
			{
				num5 = (0f - (float)scrMisc.GetInverseAnglePerBeatMultiplanet(num3 - 1f)) * endingTween * (float)((!currfloor.isCCW) ? 1 : (-1));
			}
			if (controller.rotationEase != Ease.Linear)
			{
				float num9 = (float)targetExitAngle - (float)currfloor.angleLength;
				float num10 = scrMisc.EasedAngle(num9 + num8, (float)targetExitAngle, num, controller.rotationEase, currfloor.planetEaseParts, currfloor.planetEasePartBehavior);
				if (!float.IsNaN(num10) && !float.IsInfinity(num10) && num >= num9)
				{
					num = num10;
				}
				float num11 = scrMisc.EasedAngle(num9 + num8, (float)targetExitAngle, num4, controller.rotationEase, controller.rotationEaseParts, controller.rotationEasePartBehavior);
				if (!float.IsNaN(num11) && !float.IsInfinity(num11) && num >= num9)
				{
					num4 = num11;
				}
			}
			if (controller.stickToFloor)
			{
				num -= (currfloor.transform.rotation.eulerAngles - currfloor.startRot).z * (MathF.PI / 180f);
				num4 -= (currfloor.transform.rotation.eulerAngles - currfloor.startRot).z * (MathF.PI / 180f);
			}
		}
		tempTransPos = base.transform.position;
		if (currfloor != null && currfloor.nextfloor != null && controller.state == States.PlayerControl && currfloor != null && currfloor.nextfloor.radiusScale != currfloor.radiusScale)
		{
			foreach (scrPlanet planet in controller.planetList)
			{
				easedPerc = num7;
				endpoint = 1.0;
				easeParts = currfloor.planetEaseParts;
				easePartBehavior = currfloor.planetEasePartBehavior;
				easedPerc *= easeParts;
				endpoint /= easeParts;
				loops = Mathf.FloorToInt(Mathf.Abs((float)easedPerc));
				newStart = (float)loops / (float)easeParts;
				if (loops % 2 == 0 || easePartBehavior == EasePartBehavior.Repeat)
				{
					easedPerc = DOVirtual.EasedValue(newStart, newStart + (float)endpoint, (float)easedPerc - (float)loops, controller.rotationEase);
				}
				else
				{
					easedPerc = newStart + (float)endpoint - DOVirtual.EasedValue(0f, (float)endpoint, 1f - ((float)easedPerc - (float)loops), controller.rotationEase);
				}
				planet.cosmeticRadius = scrController.instance.startRadius * Mathf.Lerp(currfloor.radiusScale, currfloor.nextfloor.radiusScale, (float)easedPerc);
			}
		}
		double num12 = 0.0;
		if (currfloor != null)
		{
			num12 = ((!(currfloor.entryangle > currfloor.exitangle)) ? (currfloor.entryangle + (currfloor.exitangle - Math.PI - currfloor.entryangle) * num7) : (currfloor.entryangle + (currfloor.exitangle + Math.PI - currfloor.entryangle) * num7));
		}
		actualPos = new Vector3(tempTransPos.x + Mathf.Sin(num - num5) * cosmeticRadius, tempTransPos.y + Mathf.Cos(num - num5) * cosmeticRadius, tempTransPos.z);
		oppositePos = new Vector3(tempTransPos.x + Mathf.Sin((float)scrMisc.ReflectAngle(num - num5, num12)) * cosmeticRadius, tempTransPos.y + Mathf.Cos((float)scrMisc.ReflectAngle(num - num5, num12)) * cosmeticRadius, tempTransPos.z);
		if (num2 <= 2f)
		{
			if (!movingToNext.dead)
			{
				movingToNext.transform.position = ((FOOL_SWIRL && scrController.isGameWorld) ? oppositePos : actualPos);
			}
		}
		else
		{
			if (float.IsNaN(Vector3.Lerp(actualPos, oppositePos, swirlTween).x))
			{
				printe($"{actualPos}, {oppositePos}, {num}, {num5}, {num12}, {cosmeticRadius}");
			}
			if (!movingToNext.dead)
			{
				movingToNext.transform.position = Vector3.Lerp(actualPos, oppositePos, swirlTween);
			}
		}
		if (!movingToNext.dead)
		{
			movingToNext.transform.position -= addBob;
		}
		if (!(num2 > 2f) || !(currfloor != null))
		{
			return;
		}
		double num13 = (double)cosmeticRadius / (2.0 * (double)Mathf.Sin(3.141592f / num3));
		double num14 = 0.0;
		double num15 = (double)(endingTween * cosmeticRadius) + num13 * (double)(1f - endingTween);
		actualPivotPos = new Vector3(tempTransPos.x + Mathf.Sin(num4) * (float)num13 * (1f - endingTween), tempTransPos.y + Mathf.Cos(num4) * (float)num13 * (1f - endingTween), tempTransPos.z);
		oppositePivotPos = new Vector3(tempTransPos.x + Mathf.Sin((float)scrMisc.ReflectAngle(num4, num12)) * (float)num13 * (1f - endingTween), tempTransPos.y + Mathf.Cos((float)scrMisc.ReflectAngle(num4, num12)) * (float)num13 * (1f - endingTween), tempTransPos.z);
		tempMPPivotPos = Vector3.Lerp(actualPivotPos, oppositePivotPos, swirlTween);
		for (int i = 2; (float)i < num2; i++)
		{
			p = controller.GetMultiPlanet(planetIndex, i);
			if (!p.dead)
			{
				num14 = (double)((!currfloor.isCCW) ? 1 : (-1)) * (Math.PI * 2.0 * (((double)(-i) - ((double)num3 - (double)endingTween) / 2.0) / ((double)num3 - (double)endingTween))) + (double)(num4 * (1f - endingTween)) + (double)(num * endingTween);
				if (num6 > 0f && (float)i == num2 - 1f)
				{
					num14 -= (double)(num6 / num3 * 2f) * Math.PI;
				}
				actualMPPos = new Vector3(tempMPPivotPos.x + Mathf.Sin((float)num14) * (float)num15, tempMPPivotPos.y + Mathf.Cos((float)num14) * (float)num15, tempMPPivotPos.z);
				oppositeMPPos = new Vector3(tempMPPivotPos.x + Mathf.Sin((float)scrMisc.ReflectAngle(num14, num12)) * (float)num15, tempMPPivotPos.y + Mathf.Cos((float)scrMisc.ReflectAngle(num14, num12)) * (float)num15, tempMPPivotPos.z);
				p.transform.position = Vector3.Lerp(actualMPPos, oppositeMPPos, swirlTween);
				p.transform.position -= addBob;
			}
		}
	}

	public void AsyncRefreshAngles()
	{
		angle = AsyncInputUtils.GetAngle(this, snappedLastAngle, AsyncInputManager.targetSongTick);
	}

	public void FirstFloorAngleSetup()
	{
		snappedLastAngle = MathF.PI * (0.5f - conductor.adjustedCountdownTicks);
		scrFloor scrFloor2 = currfloor;
		if (ADOBase.isLevelEditor)
		{
			scrFloor2 = ADOBase.lm.listFloors[0];
		}
		else if (currfloor == null)
		{
			Collider2D[] array = Physics2D.OverlapPointAll(Vector3.zero, 1 << LayerMask.NameToLayer("Floor"));
			if (array != null)
			{
				for (int i = 0; i < array.Length; i++)
				{
					scrFloor component = array[i].GetComponent<scrFloor>();
					if (component == null)
					{
						component = array[i].transform.parent.GetComponent<scrFloor>();
					}
					if (component != null && component.seqID == 0)
					{
						scrFloor2 = component;
					}
				}
			}
		}
		currfloor = scrFloor2;
		float num = 2f - base.transform.localScale.x;
		if (controller.isbigtiles)
		{
			num = 1.6f;
		}
		if (currfloor != null && !dummyPlanets)
		{
			scrFloor scrFloor3 = currfloor.nextfloor ?? currfloor;
			ringComp.scaleEnd = Vector3.one * num * scrFloor3.radiusScale;
		}
		if (currfloor == null)
		{
			if (ADOBase.lm == null || ADOBase.lm.listFloors == null || ADOBase.lm.listFloors.Count == 0)
			{
				RDBaseDll.printesw("some stuff is null");
				return;
			}
			currfloor = ADOBase.lm.listFloors[0];
			if (currfloor == null)
			{
				RDBaseDll.printesw("scrPlanet.FixCurrFloor() didn't find a nearby scrFloor.");
			}
			else
			{
				targetExitAngle = snappedLastAngle + currfloor.angleLength;
			}
		}
		else
		{
			targetExitAngle = snappedLastAngle + currfloor.angleLength;
		}
	}

	public bool AutoShouldHitNow()
	{
		if (!scrController.isGameWorld || currfloor.seqID >= ADOBase.lm.listFloors.Count - 1)
		{
			return false;
		}
		float num = 10f;
		if (!RDC.useOldAuto)
		{
			num = 0.5f;
		}
		float num2 = num * (MathF.PI / 180f);
		_ = (float)angle % (MathF.PI * 2f);
		bool result = false;
		if (scrController.isGameWorld && ((controller.isCW && angle > targetExitAngle - (double)num2) || (!controller.isCW && angle < targetExitAngle + (double)num2)))
		{
			result = true;
		}
		return result;
	}

	public scrPlanet SwitchChosen()
	{
		scrFloor scrFloor2 = null;
		double marginScale = ((currfloor.nextfloor == null) ? 1.0 : currfloor.nextfloor.marginScale);
		bool flag = currfloor.nextfloor != null && currfloor.nextfloor.auto;
		if (!GCS.perfectOnlyMode)
		{
			_ = GCS.HITMARGIN_COUNTED;
		}
		HitMargin hitMargin = scrMisc.GetHitMargin((float)cachedAngle, (float)targetExitAngle, controller.isCW, (float)((double)conductor.bpm * controller.speed), conductor.song.pitch, marginScale);
		double num = 0.0;
		if (scrController.isGameWorld)
		{
			if (scrMisc.IsValidHit(hitMargin) || RDC.debug || ((RDC.auto || flag) && !RDC.useOldAuto) || controller.midspinInfiniteMargin || controller.noFailInfiniteMargin)
			{
				if (bgbars != null)
				{
					bgbars.Flash(scrBackgroundBars.BarStatus.hit);
				}
				scrFloor2 = currfloor.nextfloor;
				num = targetExitAngle;
				controller.forceOK = false;
				if (controller.noFailInfiniteMargin)
				{
					hitMargin = HitMargin.FailMiss;
				}
			}
		}
		else
		{
			float interval = controller.freeroamAngleInterval * (MathF.PI / 180f);
			float offset = controller.freeroamAngleOffset * (MathF.PI / 180f);
			Vector3 vector = SnappedCardinalDirection(SnapAngleCardinal(angle, interval, offset), interval, offset);
			Collider2D[] array = Physics2D.OverlapPointAll(new Vector2(vector.x, vector.y), 1 << LayerMask.NameToLayer("Floor"));
			Collider2D collider2D = null;
			if (array.Length != 0 && aliveTime > controller.popuptime)
			{
				for (int i = 0; i < array.Length; i++)
				{
					_ = array[i];
					collider2D = array[i];
					scrFloor component = collider2D.GetComponent<scrFloor>();
					if (component == null)
					{
						component = collider2D.transform.parent.GetComponent<scrFloor>();
					}
					if (component == null || !component.isLandable || ((!component.freeroamGenerated || !component.freeroam) && (component.freeroam || component.freeroamGenerated)))
					{
						continue;
					}
					if (currfloor.unstable && currfloor.isLandable)
					{
						currfloor.isLandable = false;
						currfloor.ToggleCollider(collEn: false);
						currfloor.MoveToBack();
						currfloor.hideIcon = true;
						scrFloor f = currfloor;
						f.TweenOpacity(0f, 3f, Ease.InCubic);
						if ((bool)f.legacyFloorSpriteRenderer)
						{
							f.legacyFloorSpriteRenderer.DOColor(new Color(1f, 1f, 1f, 0f), 3f).SetEase(Ease.InCubic);
						}
						currfloor.transform.DOScale(0.5f, 3f).SetEase(Ease.InCubic).OnComplete(delegate
						{
							f.enabled = false;
							f.transform.position += Vector3.up * 9999f;
						});
						currfloor.transform.DOMoveY(-2f, 3f).SetRelative(isRelative: true).SetEase(Ease.InCubic);
						currfloor.transform.DORotate(Vector3.forward * 45f, 3f).SetRelative(isRelative: true).SetEase(Ease.InCubic);
						currfloor.topGlow.enabled = false;
						currfloor.bottomGlow.enabled = false;
					}
					scrFloor2 = component;
					num = SnapAngleCardinal(angle, interval, offset);
					storedPosition = component.transform.position;
					component.entryTime = GetNearestIntervalToTime(conductor.songposition_minusi, controller.freeroamAngleInterval / 180f);
					holdOffsetPos = Vector3.zero;
				}
			}
		}
		HitMargin hitMargin2 = HitMargin.Perfect;
		if (scrFloor2 == null || (controller.failbar.DidFail(checkForDuplicateDeath: false) && !controller.noFail))
		{
			if (GCS.d_drumcontroller && controller.boothModeDebounceCounter > 0f)
			{
				printe("debounce!");
				return this;
			}
			if (GCS.d_drumcontroller && controller.gameworld)
			{
				controller.boothModeDebounceCounter = 0.1f;
			}
			if (currfloor.seqID > 0 && scrController.isGameWorld && scrLevelMaker.instance.listFloors[currfloor.seqID - 1].holdLength > -1 && Mathf.Abs((float)snappedLastAngle - (float)angle) < MathF.PI / 2f && !currfloor.freeroam)
			{
				if (controller.strictHolds && currfloor.nextfloor.holdLength < 0)
				{
					controller.OnDamage();
					controller.mistakesManager.AddHit(HitMargin.TooEarly);
				}
				printe("landing hit");
				return this;
			}
			if ((!currfloor.freeroam && currfloor.holdLength > -1 && GCS.checkpointNum != currfloor.seqID) || currfloor.holdLength == -1)
			{
				bool flag2 = controller.OnDamage(multipress: false, applyMultipressDamage: false, hitMargin == HitMargin.TooLate);
				if (controller.gameworld)
				{
					hitMargin2 = hitMargin;
					if (flag2)
					{
						hitMargin2 = HitMargin.FailOverload;
					}
					if (controller.midspinInfiniteMargin)
					{
						hitMargin2 = HitMargin.Perfect;
					}
					controller.mistakesManager.AddHit(hitMargin2);
					if (!currfloor.hideJudgment)
					{
						Vector3 position = next.transform.position;
						position.y += 1f;
						float num2 = (float)(targetExitAngle - angle);
						controller.ShowHitText(hitMargin2, position, num2);
					}
					foreach (ffxPlusBase missEffect in controller.missEffects)
					{
						missEffect.StartEffect();
					}
				}
				controller.multipressPenalty = false;
				controller.multipressAndHasPressedFirstPress = false;
			}
			if (GCS.checkpointNum == currfloor.seqID)
			{
				controller.lastCamPulseFloor = currfloor.seqID;
			}
			return this;
		}
		bool flag3 = false;
		if (controller.multipressAndHasPressedFirstPress && controller.multipressPenalty && !controller.midspinInfiniteMargin && !currfloor.freeroam)
		{
			bool flag4 = false;
			if ((bool)currfloor)
			{
				flag4 = true;
				double bpm = (double)conductor.bpm * controller.speed * (double)conductor.song.pitch;
				double angleMoved = scrMisc.GetAngleMoved(currfloor.entryangle, currfloor.exitangle, !currfloor.isCCW);
				double num3 = scrMisc.AngleToTime(angleMoved, bpm);
				double num4 = 1.5690509853884578;
				flag4 = flag4 && angleMoved > num4;
				double num5 = controller.averageFrameTime * 3.5f;
				flag4 = flag4 && num3 > num5;
				double num6 = 0.039999999105930328;
				flag4 = flag4 && num3 > num6;
			}
			controller.OnDamage(multipress: true, flag4);
			if (controller.consecMultipressCounter > 5 || flag4)
			{
				flag3 = true;
			}
			if (controller.gameworld)
			{
				controller.multipressPenalty = false;
				controller.multipressAndHasPressedFirstPress = false;
			}
		}
		if (GCS.d_drumcontroller && controller.gameworld)
		{
			controller.boothModeDebounceCounter = 0.1f;
		}
		bool flag5 = Persistence.GetMultitapTileBehavior() == MultitapTileBehavior.HitOnce;
		scrFloor2.tapsSoFar++;
		if (scrFloor2.tapsNeeded > scrFloor2.tapsSoFar)
		{
			Texture2D value = ((scrFloor2.tapsNeeded - scrFloor2.tapsSoFar == 2) ? ADOBase.gc.tex_floorEdgeNeon2 : ADOBase.gc.tex_floorEdgeNeon);
			Material material = scrFloor2.floorRenderer.material;
			material.SetTexture("_MainTex", value);
			material.SetFloat("_FlashColor", 1.4f);
			material.DOFloat(0f, "_FlashColor", (float)conductor.crotchet * 0.5f);
			if (!flag5)
			{
				return this;
			}
		}
		if (controller.gameworld)
		{
			hitMargin2 = (scrFloor2.grade = hitMargin);
			if (controller.midspinInfiniteMargin || ((RDC.auto || flag) && !RDC.useOldAuto))
			{
				hitMargin2 = HitMargin.Perfect;
			}
			if (flag)
			{
				hitMargin2 = HitMargin.Auto;
			}
			controller.mistakesManager.AddHit(hitMargin2);
			if (flag)
			{
				hitMargin2 = HitMargin.Perfect;
			}
			controller.ClearMisses();
			if (scrFloor2.hasConditionalChange)
			{
				controller.perfectEffects = scrFloor2.perfectEffects;
				controller.hitEffects = scrFloor2.hitEffects;
				controller.barelyEffects = scrFloor2.barelyEffects;
				controller.missEffects = scrFloor2.missEffects;
				controller.lossEffects = scrFloor2.lossEffects;
			}
			switch (hitMargin2)
			{
				case HitMargin.Perfect:
					foreach (ffxPlusBase perfectEffect in controller.perfectEffects)
					{
						perfectEffect.StartEffect();
					}
					break;
				case HitMargin.EarlyPerfect:
				case HitMargin.LatePerfect:
					foreach (ffxPlusBase hitEffect in controller.hitEffects)
					{
						hitEffect.StartEffect();
					}
					break;
				case HitMargin.VeryEarly:
				case HitMargin.VeryLate:
					foreach (ffxPlusBase barelyEffect in controller.barelyEffects)
					{
						barelyEffect.StartEffect();
					}
					break;
			}
		}
		movingToNext = next;
		foreach (scrPlanet planet in controller.planetList)
		{
			if (planet != movingToNext)
			{
				planet.iFrames = 15f / (conductor.bpm * (float)controller.speed) / conductor.song.pitch;
			}
		}
		if ((bool)scrFloor2 && scrFloor2.midSpin && scrFloor2.numPlanets > 2)
		{
			movingToNext = prev;
		}
		MoveToNextFloor(scrFloor2, (float)num, hitMargin2);
		if (controller.gameworld)
		{
			Vector3 position2 = movingToNext.transform.position;
			position2.y += 1f;
			if (!RDC.noHud && !scrFloor2.hideJudgment)
			{
				controller.ShowHitText(flag3 ? HitMargin.Multipress : hitMargin2, position2, flag3 ? 0f : ((float)(targetExitAngle - angle)));
			}
		}
		return movingToNext;
	}

	private void MoveToNextFloor(scrFloor floor, float exitAngle, HitMargin hitMargin)
	{
		movingToNext = next;
		if (floor.midSpin && floor.numPlanets > 2)
		{
			movingToNext = prev;
		}
		bool flag = currfloor.holdLength > -1;
		if (!controller.stickToFloor && currfloor.holdLength > -1 && controller.state == States.PlayerControl)
		{
			targetPosition = currfloor.nextfloor.startPos - scrController.instance.startRadius * currfloor.nextfloor.radiusScale * new Vector3(Mathf.Sin((float)currfloor.exitangle), Mathf.Cos((float)currfloor.exitangle), 0f);
			holdOffsetPos = targetPosition - currfloor.startPos;
			if (controller.gameworld || (!controller.gameworld && currfloor.nextfloor.freeroam))
			{
				base.transform.position = storedPosition + holdOffsetPos;
			}
			else
			{
				base.transform.position = storedPosition;
			}
			handledPositionAlready = true;
		}
		if (currfloor.holdLength > -1 && controller.state == States.PlayerControl)
		{
			addBob = Vector3.zero;
			camy.SetHoldOffset(Vector3.zero);
			currfloor.holdCompletion = 1f;
			currfloor.holdCompletionEased = 1f;
			currfloor.holdRenderer.UpdateColor(1f);
		}
		if (floor != null && floor.freeroam)
		{
			controller.gameworld = false;
			if (!currfloor.freeroam)
			{
				if (controller.lockInput < 0.05f)
				{
					controller.LockInput(0.05f);
				}
				foreach (scrFloor listFloor in ADOBase.lm.listFloors)
				{
					if (listFloor.seqID < floor.seqID)
					{
						listFloor.ToggleCollider(collEn: false);
					}
				}
				floor.ToggleCollider(collEn: false);
				if ((bool)controller.errorMeter)
				{
					controller.errorMeter.wrapperRectTransform.gameObject.SetActive(value: false);
				}
			}
			else if (floor.isSwirl)
			{
				if (!controller.isPuzzleRoom)
				{
					foreach (scrFloor item2 in ADOBase.lm.listFreeroam[floor.freeroamRegion])
					{
						item2.isCCW = !item2.isCCW;
						if (item2.isSwirl)
						{
							item2.UpdateIconSprite();
						}
					}
				}
				else
				{
					foreach (scrFloor listFloor2 in PuzzleRoom.instance.listFloors)
					{
						listFloor2.isCCW = !listFloor2.isCCW;
						if (listFloor2.isSwirl)
						{
							listFloor2.UpdateIconSprite();
						}
					}
				}
			}
		}
		if (controller.gameworld && currfloor.freeroamGenerated && !floor.freeroamGenerated && (bool)controller.errorMeter)
		{
			controller.errorMeter.wrapperRectTransform.gameObject.SetActive(value: true);
		}
		if (floor != null && floor.nextfloor != null && floor.isSwirl && floor.numPlanets > 2 && floor.prevfloor.numPlanets > 2)
		{
			movingToNext.swirlTween = ((!FOOL_SWIRL) ? 1 : 0);
			float num = 1f;
			if (floor.nextfloor != null)
			{
				num = (float)floor.nextfloor.entryTimePitchAdj - (float)floor.entryTimePitchAdj;
			}
			else if (floor.prevfloor != null)
			{
				num = (float)floor.entryTimePitchAdj - (float)floor.prevfloor.entryTimePitchAdj;
			}
			if (floor.freeroam)
			{
				movingToNext.swirlTween = 0.5f;
				num = (float)(conductor.crotchet * (double)floor.speed) * 0.5f * ((float)floor.numPlanets * 0.5f);
			}
			DOTween.To(() => movingToNext.swirlTween, delegate (float x)
			{
				movingToNext.swirlTween = x;
			}, FOOL_SWIRL ? 1 : 0, 0.5f * num).SetEase(Ease.OutSine);
		}
		if (floor != null && floor.nextfloor != null && floor.holdLength > -1 && floor.numPlanets > 2)
		{
			movingToNext.mpHoldTween = 0f;
			float num2 = 1f;
			if (floor.nextfloor != null)
			{
				num2 = (float)floor.nextfloor.entryTimePitchAdj - (float)floor.entryTimePitchAdj;
			}
			else if (floor.prevfloor != null)
			{
				num2 = (float)floor.entryTimePitchAdj - (float)floor.prevfloor.entryTimePitchAdj;
			}
			float duration = Mathf.Min(num2 * 0.5f, (float)(conductor.crotchet * (double)floor.speed) / conductor.song.pitch);
			DOTween.To(() => movingToNext.mpHoldTween, delegate (float x)
			{
				movingToNext.mpHoldTween = x;
			}, 1f, duration).SetEase(Ease.OutExpo);
		}
		if (controller.planetList != null && floor.numPlanets != currfloor.numPlanets)
		{
			controller.SetNumPlanets(floor.numPlanets);
			foreach (scrPlanet item3 in scrController.instance.dummyPlanets.FindAll((scrPlanet x) => x.transform.parent == floor.transform))
			{
				if (item3 != null)
				{
					item3.Destroy();
				}
			}
			if (floor.multiplanetLine != null)
			{
				LineRenderer i = floor.multiplanetLine;
				DOTween.To(() => i.startColor.a, delegate (float x)
				{
					i.startColor = new Color(i.startColor.r, i.startColor.g, i.startColor.b, x);
				}, 0f, 0.5f).SetUpdate(isIndependentUpdate: true);
				DOTween.To(() => i.endColor.a, delegate (float x)
				{
					i.endColor = new Color(i.endColor.r, i.endColor.g, i.endColor.b, x);
				}, 0f, 0.5f).SetUpdate(isIndependentUpdate: true);
			}
		}
		movingToNext.currfloor = floor;
		if (!floor.freeroam || (floor.freeroam && floor.freeroamGenerated && !floor.unstable))
		{
			floor.LightUp(hitMargin);
			floor.SetToRandomColor();
		}
		if (GCS.bb)
		{
			BBManager.instance.LightUpFloor(floor.seqID);
		}
		controller.currentSeqID = floor.seqID;
		double num3 = scrMisc.GetInverseAnglePerBeatMultiplanet(floor.numPlanets) * (double)((!floor.isCCW) ? 1 : (-1));
		if ((bool)floor.prevfloor && floor.prevfloor.midSpin && floor.numPlanets > 2)
		{
			num3 -= 2.0 * scrMisc.GetInverseAnglePerBeatMultiplanet(floor.prevfloor.numPlanets) * (double)((!floor.prevfloor.isCCW) ? 1 : (-1));
		}
		movingToNext.snappedLastAngle = scrMisc.mod(exitAngle + MathF.PI, 6.2831854820251465) + num3;
		movingToNext.targetExitAngle = movingToNext.snappedLastAngle + floor.angleLength * (double)((!floor.isCCW) ? 1 : (-1));
		if (floor.radiusScale != currfloor.radiusScale && controller.planetList != null)
		{
			foreach (scrPlanet planet in controller.planetList)
			{
				planet.cosmeticRadius = scrController.instance.startRadius * floor.radiusScale;
			}
		}
		if (GCS.d_stationary)
		{
			movingToNext.angle = movingToNext.snappedLastAngle;
		}
		conductor.actualLastHit = conductor.songposition_minusi;
		conductor.lastHit += ((double)exitAngle - snappedLastAngle) * (double)(controller.isCW ? 1 : (-1)) / 3.1415927410125732 * conductor.crotchet / controller.speed;
		if (floor.isportal)
		{
			controller.OnLandOnPortal(floor.levelnumber, floor.arguments);
		}
		if (scrController.isGameWorld || floor.freeroam || floor.freeroamGenerated)
		{
			controller.speed = floor.speed;
		}
		camy.RotateSmooth(floor.rotatecamera);
		controller.isCW = !floor.isCCW;
		controller.rotationEase = floor.planetEase;
		controller.rotationEaseParts = floor.planetEaseParts;
		controller.rotationEasePartBehavior = floor.planetEasePartBehavior;
		if (movingToNext.currfloor.freeroam && flag)
		{
			storedPosition = movingToNext.currfloor.transform.position;
			movingToNext.transform.position = storedPosition;
		}
		else if ((!flag && !scrController.isGameWorld) || scrController.isGameWorld)
		{
			movingToNext.MoveOneUnitInDirection(exitAngle);
		}
		else
		{
			movingToNext.transform.position = storedPosition;
		}
		ffxBase[] components = floor.GetComponents<ffxBase>();
		foreach (ffxBase item in components)
		{
			DoFFX(item);
		}
		HandlePause(floor, movingToNext);
		if (floor.freeroam && !floor.freeroamGenerated)
		{
			Collider2D[] array = Physics2D.OverlapPointAll(new Vector2(movingToNext.transform.position.x, movingToNext.transform.position.y), 1 << LayerMask.NameToLayer("Floor"));
			Collider2D collider2D = null;
			bool flag2 = false;
			if (array.Length != 0)
			{
				for (int k = 0; k < array.Length; k++)
				{
					_ = array[k];
					collider2D = array[k];
					scrFloor component = collider2D.GetComponent<scrFloor>();
					if (component == null)
					{
						component = collider2D.transform.parent.GetComponent<scrFloor>();
					}
					if (component == null)
					{
						continue;
					}
					flag2 = true;
					if (!component.isLandable)
					{
						continue;
					}
					if (component.isSwirl)
					{
						foreach (scrFloor item4 in ADOBase.lm.listFreeroam[component.freeroamRegion])
						{
							item4.isCCW = !item4.isCCW;
							if (item4.isSwirl)
							{
								item4.UpdateIconSprite();
							}
						}
						controller.isCW = !component.isCCW;
					}
					movingToNext.currfloor = component;
					component.LightUp(hitMargin);
					component.SetToRandomColor();
				}
			}
			if (!flag2)
			{
				controller.gameworld = true;
				controller.FailByHitbox(RDString.Get("neoCosmos.invalidFreeroam"));
			}
		}
		if (floor.nextfloor == null)
		{
			DOTween.To(() => movingToNext.endingTween, delegate (float x)
			{
				movingToNext.endingTween = x;
			}, 1f, 0.3f).SetEase(Ease.OutSine);
		}
		controller.multipressPenalty = GetMultipressPenalty(floor);
		if (controller.keyTimes.Count == 0)
		{
			controller.multipressAndHasPressedFirstPress = false;
		}
		else
		{
			controller.multipressAndHasPressedFirstPress = true;
		}
	}

	private bool GetMultipressPenalty(scrFloor floor)
	{
		if (!controller.gameworld)
		{
			return false;
		}
		if ((bool)floor && floor.midSpin)
		{
			floor = floor.nextfloor;
		}
		if (!floor)
		{
			return false;
		}
		scrFloor nextfloor = floor.nextfloor;
		if ((bool)nextfloor && nextfloor.midSpin)
		{
			nextfloor = nextfloor.nextfloor;
		}
		if (!nextfloor)
		{
			return false;
		}
		scrFloor nextfloor2 = nextfloor.nextfloor;
		if ((bool)nextfloor2 && nextfloor2.midSpin)
		{
			nextfloor2 = nextfloor2.nextfloor;
		}
		if (!nextfloor2)
		{
			return false;
		}
		scrFloor nextfloor3 = nextfloor2.nextfloor;
		if ((bool)nextfloor3 && nextfloor3.midSpin)
		{
			nextfloor3 = nextfloor3.nextfloor;
		}
		if (!nextfloor3)
		{
			return false;
		}
		double num = 0.52534410078078508;
		double num2 = 0.26354471500962973;
		double angleMoved = scrMisc.GetAngleMoved(floor.entryangle, floor.exitangle, !floor.isCCW);
		double angleMoved2 = scrMisc.GetAngleMoved(nextfloor.entryangle, nextfloor.exitangle, !nextfloor.isCCW);
		double angleMoved3 = scrMisc.GetAngleMoved(nextfloor2.entryangle, nextfloor2.exitangle, !nextfloor2.isCCW);
		double angleMoved4 = scrMisc.GetAngleMoved(nextfloor3.entryangle, nextfloor3.exitangle, !nextfloor3.isCCW);
		if (angleMoved < num && angleMoved2 > num)
		{
			return false;
		}
		if (angleMoved < num && angleMoved2 < num && angleMoved3 > num)
		{
			return false;
		}
		if (angleMoved < num2 && angleMoved2 < num2 && angleMoved3 < num2 && angleMoved4 > num2)
		{
			return false;
		}
		return true;
	}

	public void DoFFX(ffxBase item)
	{
		if (item.enabled)
		{
			bool flag = controller.visualEffects == VisualEffects.Minimum;
			if ((!GCS.lofiVersion || (GCS.lofiVersion && !controller.gameworld)) && (hifi || !item.hifiEffect) && (!item.disableIfMinFx || !flag) && !item.usedByFfxPlus)
			{
				item.doEffect();
			}
		}
	}

	public void ScrubToFloorNumber(int floorNum, float windbackTime = -1f, bool movePos = true)
	{
		if (floorNum > scrLevelMaker.instance.listFloors.Count)
		{
			Debug.LogError("Trying to scrub past last tile. Is CustomCheckpoint ticked by accident in LevelSwitcher?");
		}
		if (scrController.instance != null)
		{
			int i;
			for (i = 1; i <= floorNum; i++)
			{
				foreach (scrPlanet item in scrController.instance.dummyPlanets.FindAll((scrPlanet x) => x.attachedDummyFloor == i))
				{
					if (item != null && item.ring.enabled)
					{
						item.Destroy();
					}
				}
				foreach (LineRenderer item2 in scrController.instance.multiPlanetLines.FindAll((LineRenderer x) => x.transform.parent == scrLevelMaker.instance.listFloors[i].transform))
				{
					if (item2 != null)
					{
						item2.enabled = false;
					}
				}
			}
		}
		scrFloor scrFloor2 = (currfloor = scrLevelMaker.instance.listFloors[floorNum]);
		controller.currentSeqID = floorNum;
		if (!dummyPlanets)
		{
			float num = 2f - base.transform.localScale.x;
			if (controller.isbigtiles)
			{
				num = 1.6f;
			}
			if (currfloor.nextfloor == null)
			{
				ringComp.scaleEnd = Vector3.one * num * currfloor.radiusScale;
			}
			else
			{
				ringComp.scaleEnd = Vector3.one * num * currfloor.nextfloor.radiusScale;
			}
		}
		if (movePos)
		{
			if (controller.stickToFloor)
			{
				base.transform.position = scrFloor2.transform.position;
			}
			else
			{
				base.transform.position = scrFloor2.startPos;
			}
		}
		double num2 = 0.0;
		num2 = scrMisc.GetInverseAnglePerBeatMultiplanet(scrFloor2.numPlanets) * (double)((!scrFloor2.isCCW) ? 1 : (-1));
		if ((bool)scrFloor2.prevfloor && scrFloor2.prevfloor.midSpin && scrFloor2.numPlanets > 2)
		{
			num2 -= 1.0 * scrMisc.GetInverseAnglePerBeatMultiplanet(scrFloor2.prevfloor.numPlanets) * (double)((!scrFloor2.prevfloor.isCCW) ? 1 : (-1));
		}
		snappedLastAngle = scrFloor2.entryangle + num2;
		controller.isCW = !scrFloor2.isCCW;
		controller.rotationEase = scrFloor2.planetEase;
		targetExitAngle = snappedLastAngle + scrFloor2.angleLength * (double)((!scrFloor2.isCCW) ? 1 : (-1));
		storedPosition = scrFloor2.transform.position;
		scrubbedStartPos = floorNum;
		if (scrFloor2.nextfloor == null)
		{
			DOTween.To(() => endingTween, delegate (float x)
			{
				endingTween = x;
			}, 1f, 0.3f).SetEase(Ease.OutSine);
		}
		double num3 = 0.0;
		double timeDiff = 0.0;
		if (windbackTime != -1f)
		{
			double num4 = 0.0;
			num4 = scrFloor2.entryTimePitchAdj - (double)windbackTime;
			timeDiff = scrFloor2.entryTime - scrFloor2.entryTime;
			num3 = scrMisc.TimeToAngleInRad(scrFloor2.entryTimePitchAdj - num4, conductor.bpm * scrFloor2.speed, conductor.song.pitch);
			snappedLastAngle -= num3 * (double)(controller.isCW ? 1 : (-1));
		}
		Update_RefreshAngles();
		scrFloor scrFloor3 = null;
		for (int num5 = floorNum; num5 > 0; num5--)
		{
			scrFloor scrFloor4 = ADOBase.lm.listFloors[num5];
			if (scrFloor4.hasConditionalChange)
			{
				scrFloor3 = scrFloor4;
				break;
			}
		}
		if (scrFloor3 != null)
		{
			controller.perfectEffects = scrFloor3.perfectEffects;
			controller.hitEffects = scrFloor3.hitEffects;
			controller.barelyEffects = scrFloor3.barelyEffects;
			controller.missEffects = scrFloor3.missEffects;
			controller.lossEffects = scrFloor3.lossEffects;
		}
		if (!GCS.standaloneLevelMode)
		{
			timeDiff = 0.0;
		}
		HandlePause(currfloor, this, num3 * (double)(controller.isCW ? 1 : (-1)), timeDiff);
		if (currfloor.radiusScale == 1f || controller.planetList == null)
		{
			return;
		}
		foreach (scrPlanet planet in controller.planetList)
		{
			planet.cosmeticRadius = scrController.instance.startRadius * currfloor.radiusScale;
		}
	}

	private bool SnapToGrid()
	{
		Vector3 position = base.transform.position;
		base.transform.position = new Vector3(Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.y), 0f);
		return true;
	}

	private Vector3 SnappedCardinalDirection(float snappedAngle, float interval = MathF.PI / 2f, float offset = 0f)
	{
		Vector3 zero = Vector3.zero;
		int num = Mathf.RoundToInt((snappedAngle + offset) / interval);
		int m = Mathf.RoundToInt(MathF.PI * 2f / interval);
		float f = 0f - ((float)scrMisc.ModInt(num, m) - offset) * interval + MathF.PI / 2f;
		zero = new Vector3(Mathf.Cos(f), Mathf.Sin(f), 0f);
		return base.transform.position + zero * scrController.instance.startRadius * currfloor.radiusScale;
	}

	private void SnapToCardinalDirection(float snappedAngle)
	{
		Vector3 vector = Vector3.zero;
		int num = Mathf.RoundToInt(snappedAngle / (MathF.PI / 2f));
		switch (num % 4)
		{
			case 0:
				vector = Vector3.up;
				break;
			case 1:
				vector = Vector3.right;
				break;
			case 2:
				vector = Vector3.down;
				break;
			case 3:
				vector = Vector3.left;
				break;
		}
		base.transform.position = prev.transform.position + vector;
	}

	private void MoveOneUnitInDirection(float snappedAngle)
	{
		float num = snappedAngle;
		if (controller.stickToFloor && currfloor != null)
		{
			num -= (currfloor.transform.rotation.eulerAngles - currfloor.startRot).z * (MathF.PI / 180f);
		}
		if (currfloor != null && currfloor.midSpin && currfloor.numPlanets > 2)
		{
			num += (float)scrMisc.GetInverseAnglePerBeatMultiplanet(currfloor.numPlanets) * (currfloor.isCCW ? (-1f) : 1f);
		}
		Vector3 zero = Vector3.zero;
		zero = new Vector3(Mathf.Sin(num) * radius * currfloor.radiusScale, Mathf.Cos(num) * radius * currfloor.radiusScale, 0f);
		if (currfloor.midSpin && currfloor.numPlanets > 2)
		{
			lastMovedPlanet = next;
		}
		else
		{
			lastMovedPlanet = prev;
		}
		base.transform.position = lastMovedPlanet.transform.position + zero;
		storedPosition = base.transform.position;
	}

	private bool IsSameTransform(Transform trans)
	{
		if (trans.localPosition.x == base.transform.position.x)
		{
			return trans.localPosition.y == base.transform.position.y;
		}
		return false;
	}

	public static Vector3 RoundCoord(Vector3 coord)
	{
		return new Vector3(Mathf.RoundToInt(coord.x), Mathf.RoundToInt(coord.y), 0f);
	}

	public static float SnapAngleCardinal(double angle, float interval = MathF.PI / 2f, float offset = 0f)
	{
		return (float)Mathf.RoundToInt((float)((angle + (double)offset) / (double)interval)) * interval - offset;
	}

	public static double SnapAngle360(double angle, double exitAngle)
	{
		return (double)((float)Mathf.RoundToInt((float)((angle - exitAngle) / 6.2831854820251465)) * (MathF.PI * 2f)) + exitAngle;
	}

	public void PreDie()
	{
		if (GCS.playDeathSound)
		{
			audioSource.PlayOneShot(audioClips[0]);
		}
	}

	public void Die(float explosionSize = 1f)
	{
		dead = true;
		hittable = false;
		if (isChosen)
		{
			audioSource.clip = audioClips[(!controller.mistakesManager.IsNewBest()) ? 1 : 2];
			if (GCS.playDeathSound)
			{
				audioSource.Play();
			}
			if (GCS.playWilhelm)
			{
				scrSfx.instance.PlaySfx(SfxSound.Wilhelm, 0.6f);
			}
		}
		if (!(currfloor?.GetComponent<ffxTractorbeamFloors>() != null) || controller.visualQuality != VisualQuality.High)
		{
			sprite.enabled = false;
			ring.enabled = false;
			glow.enabled = false;
			tailParticles.Stop();
			coreParticles.Stop();
			deathExplosion.gameObject.SetActive(value: true);
			ParticleSystem.MainModule main = deathExplosion.main;
			ParticleSystem.MinMaxCurve startLifetime = main.startLifetime;
			startLifetime.constant = explosionSize * 1.2f;
			main.startLifetime = startLifetime;
			deathExplosion.emission.SetBursts(new ParticleSystem.Burst[1]
			{
				new ParticleSystem.Burst(0f, (int)(explosionSize * 2000f))
			});
			sparks.Stop();
			samuraiSprite.gameObject.SetActive(value: false);
			faceHolder.gameObject.SetActive(value: false);
		}
	}

	public void Destroy()
	{
		hittable = false;
		sprite.enabled = false;
		ring.enabled = false;
		faceHolder.gameObject.SetActive(value: false);
		samuraiSprite.gameObject.SetActive(value: false);
		tailParticles.Stop(withChildren: true, ParticleSystemStopBehavior.StopEmittingAndClear);
		coreParticles.Stop(withChildren: true, ParticleSystemStopBehavior.StopEmittingAndClear);
		deathExplosion.Clear();
		glow.enabled = false;
		sparks.Stop(withChildren: true, ParticleSystemStopBehavior.StopEmittingAndClear);
	}

	public void DestroyAllButRing()
	{
		hittable = false;
		onlyRing = true;
		ring.enabled = true;
		sprite.enabled = false;
		faceHolder.gameObject.SetActive(value: false);
		samuraiSprite.gameObject.SetActive(value: false);
		tailParticles.Stop(withChildren: true, ParticleSystemStopBehavior.StopEmittingAndClear);
		coreParticles.Stop(withChildren: true, ParticleSystemStopBehavior.StopEmittingAndClear);
		deathExplosion.Clear();
		glow.enabled = false;
		sparks.Stop(withChildren: true, ParticleSystemStopBehavior.StopEmittingAndClear);
	}

	public void DisableParticles()
	{
		tailParticles.Stop(withChildren: true, ParticleSystemStopBehavior.StopEmittingAndClear);
		coreParticles.Stop(withChildren: true, ParticleSystemStopBehavior.StopEmittingAndClear);
		deathExplosion.gameObject.SetActive(value: false);
		sparks.Stop(withChildren: true, ParticleSystemStopBehavior.StopEmittingAndClear);
	}

	public void StopParticles()
	{
		tailParticles.Stop(withChildren: true, ParticleSystemStopBehavior.StopEmitting);
		coreParticles.Stop(withChildren: true, ParticleSystemStopBehavior.StopEmitting);
		deathExplosion.gameObject.SetActive(value: false);
		sparks.Stop(withChildren: true, ParticleSystemStopBehavior.StopEmitting);
	}

	public void ClearParticles()
	{
		tailParticles.Clear();
		coreParticles.Clear();
		sparks.Clear();
	}

	public scrMissIndicator MarkMiss()
	{
		return UnityEngine.Object.Instantiate(ADOBase.gc.missIndicator, next.transform.position, Quaternion.identity).GetComponent<scrMissIndicator>();
	}

	public double GetSnappedLastAngle()
	{
		return snappedLastAngle;
	}

	public void TweenSnappedLastAngle(double startVal, double newVal)
	{
		snappedLastAngle = startVal;
		DOTween.To(() => snappedLastAngle, delegate (double x)
		{
			snappedLastAngle = x;
		}, newVal, (float)conductor.crotchet / conductor.song.pitch).SetEase(Ease.OutSine);
	}

	public void TweenSnappedLastAngle(double newVal)
	{
		DOTween.To(() => snappedLastAngle, delegate (double x)
		{
			snappedLastAngle = x;
		}, newVal, (float)conductor.crotchet / conductor.song.pitch).SetEase(Ease.OutSine);
	}

	public void SetSnappedLastAngle(double newVal)
	{
		snappedLastAngle = newVal;
	}

	public void SetTargetExitAngle(double newVal)
	{
		targetExitAngle = newVal;
	}

	public void HandlePause(scrFloor floor, scrPlanet p, double angleDiff = 0.0, double timeDiff = 0.0)
	{
		if (!floor.freeroam && floor.extraBeats > 0f)
		{
			lockTime = (float)(conductor.crotchet / (double)floor.speed) * (floor.extraBeats + 0.2f) + (float)timeDiff;
			if (floor.nextfloor.entryTime - floor.entryTime - (double)lockTime < 0.05000000074505806)
			{
				lockTime = 0f;
			}
			if (lockTime > 0f)
			{
				scrController.instance.LockInput(lockTime);
			}
			p.angle = p.snappedLastAngle - angleDiff + (conductor.songposition_minusi - conductor.lastHit) / conductor.crotchet * 3.1415927410125732 * controller.speed * (double)(controller.isCW ? 1 : (-1));
			int num = ((!floor.isCCW) ? 1 : (-1));
			_ = floor.nextfloor.isCCW;
			double num2 = scrMisc.mod(p.angle, 6.2831852);
			conductor.lastHit = floor.entryTime;
			double num3 = (3.1415926 - floor.angleLength) * (double)num;
			if ((double)Mathf.Abs((float)floor.angleLength) < 0.002 || (double)Mathf.Abs((float)floor.angleLength - 6.283185f) < 0.002)
			{
				num3 = 0.0;
			}
			double num4 = floor.nextfloor.entryangle - 3.1415926 * (double)num;
			double num5 = num4 - (double)(MathF.PI * (floor.extraBeats + 1f) * (float)num) - angleDiff + num3;
			p.SetTargetExitAngle(num4);
			p.SetSnappedLastAngle(num5);
			p.angle = p.snappedLastAngle + (conductor.songposition_minusi - conductor.lastHit) / conductor.crotchet * 3.1415927410125732 * 0.5 * controller.speed * (double)(controller.isCW ? 1 : (-1));
			double num6 = scrMisc.mod(p.angle, 6.2831852);
			double num7 = (double)floor.angleCorrectionType * (num2 - num6) * (double)((!floor.isCCW) ? 1 : (-1)) * (double)((num6 < num2) ? 1 : (-1));
			p.TweenSnappedLastAngle(num5 - num7, num5);
		}
	}

	public double GetNearestIntervalToTime(double time, float beatInterval = 0.5f)
	{
		return (double)Mathf.Round((float)((time - conductor.crotchetAtStart) * (double)(conductor.bpm / 60f)) * (1f / beatInterval * (float)controller.speed)) / (double)(1f / beatInterval * (float)controller.speed) * (double)(60f / conductor.bpm) + conductor.crotchetAtStart;
	}

	public scrMissIndicator MarkFail()
	{
		return UnityEngine.Object.Instantiate(ADOBase.gc.failIndicator, next.transform.position, Quaternion.identity).GetComponent<scrMissIndicator>();
	}
}
