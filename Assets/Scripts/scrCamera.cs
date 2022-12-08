using System;
using UnityEngine;

public class scrCamera : ADOBase
{
	private const float CameraZ = -10f;

	public static scrCamera instance;

	[Header("Cameras")]
	public Camera camobj;

	public Camera BGcam;

	public Camera Bgcamstatic;

	[Header("State")]
	public bool isPulsingOnHit = true;

	public bool isSizeTweening = true;

	public bool isZoomingOut;

	public bool isMoveTweening = true;

	public float timer;

	public bool calibrationScreen;

	public int positionStateInt;

	public bool is3D;

	public bool followMode = true;

	public bool followMovingPlatforms;

	public bool editorRotation;

	[Header("Position")]
	public Vector3 topos;

	public Vector3 pos;

	public Vector3 frompos;

	public float camspeed;

	public bool speedAffectedByBPMChanges;

	public bool speedAffectedBySpeedTrial = true;

	public bool lockedSpeed;

	[Header("Color")]
	public Color fromcol;

	public Color tocol;

	public float coltimer;

	public float coldur;

	public Color col;

	[Header("Rotation")]
	private float rottimer;

	public float rotdur = 5f;

	[Header("Size")]
	public float camsizenormal = 5f;

	public float tosize;

	public float fromsize;

	private float sizeTweenTime = 1f;

	public float userSizeMultiplier = 1f;

	public float zoomSize = 1f;

	[Header("Pulse")]
	public float pulsemagnitude = 0.2f;

	public float pulsedur = 1f;

	private float pulsetimer;

	[Header("Other")]
	public Renderer flashPlusRendererBg;

	public Renderer flashPlusRendererFg;

	[NonSerialized]
	public float torot;

	[NonSerialized]
	public float fromrot;

	[NonSerialized]
	public float rot;

	public Vector3 offset;

	[NonSerialized]
	public Vector3 holdOffset;

	[NonSerialized]
	public Vector2 lastEventRelativePosition = Vector2.zero;

	[NonSerialized]
	public CamMovementType lastUsedMovementType;

	public Vector3 shake;

	private GameObject following;

	public PositionState positionState
	{
		get
		{
			return (PositionState)positionStateInt;
		}
		set
		{
			positionStateInt = (int)value;
		}
	}

	private void Awake()
	{
		instance = this;
		camobj = GetComponent<Camera>();
		if (is3D)
		{
			SetYOffset(camobj.transform.localPosition.y);
			SetXOffset(camobj.transform.localPosition.x);
		}
	}

	private void Start()
	{
		topos = camobj.transform.localPosition - (is3D ? (offset + holdOffset) : Vector3.zero);
		frompos = camobj.transform.localPosition - (is3D ? (offset + holdOffset) : Vector3.zero);
		if (!GCS.lofiVersion || calibrationScreen)
		{
			tocol = camobj.backgroundColor;
		}
		tosize = camsizenormal;
		if (GCS.lofiVersion && !calibrationScreen)
		{
			Transform transform = base.transform.Find("BgController");
			if (transform != null)
			{
				transform.gameObject.SetActive(value: true);
			}
		}
		if (ADOBase.isLevelEditor)
		{
			editorRotation = true;
		}
		camobj.cullingMask |= 0x80;
	}

	public void Rewind()
	{
		isPulsingOnHit = true;
		isSizeTweening = true;
		isZoomingOut = false;
		isMoveTweening = true;
		topos = Vector3.zero;
		pos = Vector3.zero;
		frompos = Vector3.zero;
		fromcol = Color.clear;
		tocol = Color.clear;
		coltimer = 0f;
		coldur = 1f;
		col = Color.clear;
		rotdur = 5f;
		timer = 0f;
		pulsemagnitude = 0.1f;
		pulsedur = 1f;
		camsizenormal = 5f;
		camspeed = 0f;
		tosize = 0f;
		fromsize = 5f;
		calibrationScreen = false;
		base.transform.localPosition = new Vector3(0f, 0f, -10f);
		Start();
	}

	private void Update()
	{
		if (!ADOBase.isLevelEditor || !ADOBase.controller.paused || !followMode)
		{
			if (RDC.debug)
			{
				if (UnityEngine.Input.GetKey(KeyCode.Tab))
				{
					ViewObjectInstant(scrController.instance.chosenplanet.transform);
				}
				if (UnityEngine.Input.GetKeyDown(KeyCode.W))
				{
					GCS.d_freeroam = !GCS.d_freeroam;
				}
				if (GCS.d_freeroam)
				{
					float d = (UnityEngine.Input.GetKey(KeyCode.LeftShift) || UnityEngine.Input.GetKey(KeyCode.RightShift)) ? 0.4f : 0.1f;
					if (UnityEngine.Input.GetKey(KeyCode.RightArrow))
					{
						base.transform.Translate(Vector3.right * d);
					}
					if (UnityEngine.Input.GetKey(KeyCode.LeftArrow))
					{
						base.transform.Translate(Vector3.left * d);
					}
					if (UnityEngine.Input.GetKey(KeyCode.UpArrow))
					{
						base.transform.Translate(Vector3.up * d);
					}
					if (UnityEngine.Input.GetKey(KeyCode.DownArrow))
					{
						base.transform.Translate(Vector3.down * d);
					}
				}
				if (UnityEngine.Input.GetKey(KeyCode.Minus))
				{
					camobj.orthographicSize = scrMisc.incrementFloat(camobj.orthographicSize, -0.3f);
					camsizenormal = camobj.orthographicSize;
				}
				if (UnityEngine.Input.GetKey(KeyCode.Equals))
				{
					camobj.orthographicSize = scrMisc.incrementFloat(camobj.orthographicSize, 0.3f);
					camsizenormal = camobj.orthographicSize;
				}
			}
			if (!GCS.d_freeroam && !Input.GetKey(KeyCode.Minus) && !Input.GetKey(KeyCode.Equals))
			{
				if (ADOBase.isLevelEditor)
				{
					scrConductor scrConductor = scrConductor.instance;
					float num = scrConductor.bpm * (float)scrController.instance.speed;
					num *= scrConductor.song.pitch;
					float num2 = 60f / num;
					camspeed = num2 * 2f;
				}
				else if (!lockedSpeed)
				{
					camspeed = (float)(scrConductor.instance.crotchet * 2.0);
					if (speedAffectedByBPMChanges)
					{
						camspeed /= (float)ADOBase.controller.speed;
					}
					if (speedAffectedBySpeedTrial)
					{
						camspeed /= GCS.currentSpeedTrial;
					}
				}
				timer += Time.deltaTime;
				switch (positionState)
				{
				case PositionState.Origin:
					topos = new Vector3(0f, 0f, -10f);
					break;
				case PositionState.Levels:
					topos = new Vector3(topos.x, -2f, -10f);
					break;
				case PositionState.DLC:
					topos = new Vector3(topos.x, -12f, -10f);
					break;
				case PositionState.Xtra:
					topos = new Vector3(-9f, 13f, -10f);
					break;
				case PositionState.HopGem:
					if (!Persistence.GetUnlockedXF())
					{
						topos = new Vector3(0f, 3f, -10f);
					}
					else
					{
						positionState = PositionState.Origin;
					}
					break;
				case PositionState.GemToXtra:
					topos = new Vector3(topos.x, topos.y, -10f);
					break;
				case PositionState.ChangingRoom:
					topos = new Vector3(0f, topos.y, -10f);
					break;
				case PositionState.XtraIsland:
					topos = new Vector3(0f, 11f, -10f);
					break;
				case PositionState.CLS:
					topos = new Vector3(-5.5f, topos.y, -10f);
					break;
				case PositionState.CLSIntro:
					topos = new Vector3(0f, 0f, -10f);
					break;
				case PositionState.CrownIsland:
					topos = new Vector3(0f, 24.5f, -10f);
					break;
				case PositionState.MuseDashIsland:
					topos = new Vector3(-25f, 24f, -10f);
					break;
				case PositionState.ToTaroWorld:
					topos = new Vector3(8f, topos.y, -10f);
					break;
				case PositionState.ExitTaroWorld:
					topos = new Vector3(8f, -4f, -10f);
					break;
				case PositionState.TaroMenu0TopLane:
					topos = new Vector3(topos.x, 3f, -10f);
					break;
				case PositionState.TaroMenu0BottomLane:
					topos = new Vector3(topos.x, -10f, -10f);
					break;
				case PositionState.TaroMenu1TopLane:
					topos = new Vector3(topos.x, 15f, -10f);
					break;
				case PositionState.TaroMenu2BottomLane:
					topos = new Vector3(topos.x, -11f, -10f);
					break;
				case PositionState.TaroMenu3BottomLane:
					topos = new Vector3(topos.x, -5f, -10f);
					break;
				case PositionState.TaroMenu2TopLane:
					topos = new Vector3(topos.x, 5f, -10f);
					break;
				case PositionState.TaroMenu3TopLane:
					topos = new Vector3(topos.x, 5f, -10f);
					break;
				case PositionState.NeoCosmosCredits:
					topos = new Vector3(42f, 1.5f, -10f);
					break;
				}
				if (isMoveTweening || ADOBase.controller.gameworld)
				{
					float num3 = 0f;
					float num4 = 0f;
					float num5 = Vector3.Distance(frompos, topos);
					if (num5 > 5f)
					{
						num4 = Mathf.InverseLerp(5f, 10f, num5);
						num4 = Mathf.Min(1f, num4);
					}
					else
					{
						num4 = 0f;
					}
					num3 = num4 * 0.5f + 1f;
					if (!followMovingPlatforms)
					{
						num3 = 1f;
					}
					pos = Vector3.Lerp(frompos, topos + offset + holdOffset, timer / (camspeed / num3));
					camobj.transform.localPosition = pos + shake.WithZ(0f);
				}
				if (!editorRotation)
				{
					rottimer += Time.deltaTime;
					rot = Mathf.Lerp(fromrot, torot, rottimer / rotdur);
					camobj.transform.localEulerAngles = new Vector3(base.transform.localEulerAngles.x, base.transform.localEulerAngles.y, rot);
				}
				coltimer += Time.deltaTime;
				col = Color.Lerp(fromcol, tocol, coltimer / coldur);
				camobj.backgroundColor = col;
			}
		}
		if (isSizeTweening && ADOBase.controller != null)
		{
			if (!ADOBase.controller.paused)
			{
				pulsetimer += Time.deltaTime;
			}
			float num6 = Mathf.Lerp(fromsize, tosize, pulsetimer / sizeTweenTime);
			if (zoomSize == 0f)
			{
				camobj.enabled = false;
			}
			else
			{
				if (!camobj.enabled)
				{
					camobj.enabled = true;
				}
				camobj.orthographicSize = num6 * userSizeMultiplier * zoomSize;
			}
		}
		if (AsyncInputManager.isActive && scrController.shouldReplaceCamyToPos)
		{
			scrController.shouldReplaceCamyToPos = false;
			topos = scrController.overrideCamyToPos;
		}
	}

	public void ViewObjectInstant(Transform _trans, bool includeOffset = false)
	{
		Vector3 position = topos = new Vector3(_trans.position.x, _trans.position.y, -10f);
		if (includeOffset || is3D)
		{
			position.x += offset.x + holdOffset.x;
			position.y += offset.y + holdOffset.y;
		}
		frompos = position;
		base.transform.position = position;
	}

	public void ViewVectorInstant(Vector2 _vec, bool includeOffset = false)
	{
		Vector3 position = topos = new Vector3(_vec.x, _vec.y, -10f);
		if (includeOffset || is3D)
		{
			position.x += offset.x;
			position.y += offset.y;
		}
		frompos = position;
		base.transform.position = position;
	}

	public void Pulse()
	{
		if (ADOBase.controller.visualEffects != 0)
		{
			fromsize = camsizenormal - pulsemagnitude;
			tosize = camsizenormal;
			sizeTweenTime = pulsedur;
			pulsetimer = 0f;
		}
	}

	public void ZoomOut()
	{
		if (!GCS.DisableAllZooming)
		{
			isZoomingOut = true;
			pulsetimer = 0f;
			tosize = 100f;
			sizeTweenTime = 10f;
		}
	}

	public void RotateSmooth(float angle)
	{
		torot += angle;
		fromrot = rot;
		rottimer = 0f;
	}

	public void FlashBg(Color? _fromcol = default(Color?))
	{
		fromcol = (_fromcol ?? Color.white);
		tocol = scrVfx.instance.currentColourScheme.colourBg;
		coltimer = 0f;
	}

	public void SetBgColour()
	{
		tocol = scrVfx.instance.currentColourScheme.colourBg;
		coltimer = coldur;
	}

	public void setCamSizeInstant(float size)
	{
		fromsize = size;
		tosize = size;
	}

	public void setCamSizeSmooth(float size, float duration = -999f)
	{
		fromsize = camobj.orthographicSize;
		tosize = size;
		camsizenormal = size;
		if (duration != -999f)
		{
			sizeTweenTime = duration;
		}
		pulsetimer = 0f;
	}

	public void setCamSizeLerp(float from, float to, float dur)
	{
		fromsize = from;
		tosize = to;
		sizeTweenTime = dur;
		pulsetimer = 0f;
	}

	public void setNewCamSizeNormal(float size, bool smooth)
	{
		if (smooth)
		{
			setCamSizeSmooth(size);
		}
		else
		{
			setCamSizeInstant(size);
		}
		camsizenormal = size;
	}

	public void SetRotAngleLerp(float from, float to, float duration)
	{
		fromrot = from;
		torot = to;
		rottimer = 0f;
		rotdur = duration;
	}

	public void SetYOffset(float offsetY)
	{
		offset.y = offsetY;
	}

	public void SetXOffset(float offsetX)
	{
		offset.x = offsetX;
	}

	public void SetHoldOffset(Vector3 offset)
	{
		holdOffset = offset;
	}

	public void SetHoldXOffset(float offsetX)
	{
		holdOffset.x = offsetX;
	}

	public void SetHoldYOffset(float offsetY)
	{
		holdOffset.y = offsetY;
	}

	public void Refocus(Transform thing)
	{
		frompos = pos;
		topos = new Vector3(thing.position.x, thing.position.y, -10f);
		timer = 0f;
	}

	public void SetToFreeMode()
	{
		topos = new Vector3(0f, 0f, -10f);
		pos = new Vector3(0f, 0f, -10f);
		frompos = new Vector3(0f, 0f, -10f);
		base.transform.localPosition = new Vector3(0f, 0f, -10f);
		followMode = false;
	}

	public void SetPositionState(int p)
	{
		positionState = (PositionState)p;
	}
}
