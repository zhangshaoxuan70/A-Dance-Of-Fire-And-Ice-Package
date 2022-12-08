using ADOFAI;
using DG.Tweening;
using GDMiniJSON;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

public abstract class scrDecoration : ADOBase
{
	[Header("Decoration")]
	public Vector2 startPos;

	public scrPlanet followPlanet;

	public Transform pivotTrans;

	public Vector2 pivotPosVec;

	public Vector2 pivotOffsetVec;

	public float startRot;

	public float rotAngle;

	public Transform decTrans;

	public Color color;

	public float opacity = 1f;

	public scrParallax parallax;

	public Dictionary<TweenType, Tween> eventTweens = new Dictionary<TweenType, Tween>();

	public GameObject selectionBordersObject;

	public GameObject hitboxBordersObject;

	[NonSerialized]
	public LevelEvent sourceLevelEvent;

	[NonSerialized]
	public DecPlacementType placementType;

	[NonSerialized]
	public string decorationName;

	[NonSerialized]
	public string decorationTag;

	[NonSerialized]
	public bool hasConditionalChange;

	[NonSerialized]
	public List<ffxPlusBase> hitEffects = new List<ffxPlusBase>();

	[Header("Hitbox Properties")]
	public bool canHitPlanets;

	public bool hitOnce;

	public BoxCollider2D boxCollider;

	public BoxCollider2D damageBox;

	public CircleCollider2D damageCircle;

	public CapsuleCollider2D damageCapsule;

	public Hitbox hitboxType;

	public float hitboxRotation;

	public Vector2 hitboxScale = Vector2.one;

	public Vector2 hitboxOffset = Vector2.zero;

	[Header("Parent to tile variables")]
	public bool isChildOfTile;

	public int parentFloorNum = -1;

	private bool parentedFlag;

	[NonSerialized]
	public string[] tags;

	[NonSerialized]
	public DecorationType decType;

	[NonSerialized]
	public scrDecorationManager manager;

	public abstract void SetDepth(int depth);

	public abstract void ApplyColor();

	public abstract bool GetVisible();

	public virtual void HitFloor()
	{
	}

	public abstract float GetAlpha();

	public abstract void SetVisible(bool visible);

	public void Start()
	{
		hitOnce = false;
		if (parentFloorNum >= 0)
		{
			ADOBase.lm.listFloors[parentFloorNum].attachedDecorations.Add(this);
		}
	}

	public void SetPosition(Vector2 pivotPos, Vector2 pivotOffset)
	{
		pivotPosVec = pivotPos;
		if (parentedFlag)
		{
			pivotPosVec -= (Vector2)scrLevelMaker.instance.listFloors[parentFloorNum].transform.position;
		}
		decTrans.localPosition = pivotOffset;
		if ((bool)ADOBase.editor)
		{
			selectionBordersObject.transform.localPosition = pivotOffset;
		}
		pivotOffsetVec = pivotOffset;
		UpdateScreenClamp();
		Update();
	}

	public void SetRotation(float angle)
	{
		rotAngle = angle;
		pivotTrans.rotation = Quaternion.Euler(0f, 0f, startRot + angle);
	}

	public void SetScale(Vector2 scale)
	{
		pivotTrans.localScale = scale;
	}

	public void SetParallax(Vector2 value, DecPlacementType placementType)
	{
		if (value.x == 0f && value.y == 0f)
		{
			parallax.enabled = false;
		}
		parallax.multiplier_x = 0.01f * value.x;
		parallax.multiplier_y = 0.01f * value.y;
		this.placementType = placementType;
		UpdateScreenClamp();
	}

	public void SetColor(Color color)
	{
		this.color = color;
		ApplyColor();
	}

	public void SetOpacity(float opacity)
	{
		this.opacity = opacity;
		ApplyColor();
	}

	public void ShowSelectionBorders(bool show)
	{
		if (selectionBordersObject != null)
		{
			selectionBordersObject.SetActive(show);
		}
	}

	public void ShowHitboxBorders(bool show)
	{
		if (hitboxBordersObject != null && canHitPlanets)
		{
			hitboxBordersObject.SetActive(show);
		}
	}

	private void UpdateScreenClamp()
	{
		parallax.clampToScreen = (placementType == DecPlacementType.Camera || placementType == DecPlacementType.CameraAspect);
		Vector2 a = pivotPosVec + pivotOffsetVec;
		if (placementType == DecPlacementType.CameraAspect)
		{
			a.x *= (float)Screen.height / (float)Screen.width;
		}
		parallax.screenRelativePos = a / 20f + new Vector2(0.5f, 0.5f);
	}

	public virtual void Update()
	{
		Vector2 b = (!ADOBase.controller.paused && followPlanet != null) ? ((Vector2)followPlanet.transform.position) : Vector2.zero;
		Vector2 v = pivotPosVec + b;
		pivotTrans.localPosition = v;
		parallax.SetNewStartPosition(v);
		parallax.posCamAtStart = v;
		if (parentedFlag && !ADOBase.editor.playMode)
		{
			parentedFlag = false;
		}
		if (isChildOfTile && ADOBase.editor.playMode && !parentedFlag && scrLevelMaker.instance != null)
		{
			base.transform.parent = scrLevelMaker.instance.listFloors[parentFloorNum].transform;
			parentedFlag = true;
			SetPosition(pivotPosVec, pivotOffsetVec);
		}
		if (boxCollider != null)
		{
			boxCollider.enabled = (GetAlpha() != 0f && GetVisible());
		}
	}

	private void OnDestroy()
	{
		foreach (Tween value in eventTweens.Values)
		{
			value.Kill();
		}
	}

	public void Setup(LevelEvent dec, out bool spritesLoaded)
	{
		spritesLoaded = false;
		if (!GCS.speedTrialMode)
		{
			spritesLoaded = false;
		}
		List<scrFloor> listFloors = scrLevelMaker.instance.listFloors;
		bool smoothing = false;
		Vector2 tile = Vector2.one;
		if (dec.eventType == LevelEventType.AddDecoration)
		{
			string text = (string)dec.data["decorationImage"];
			if (!string.IsNullOrEmpty(text))
			{
				string filePath = Path.Combine(Path.GetDirectoryName(ADOBase.levelPath), text);
				ADOBase.customLevel.imgHolder.AddSprite(text, filePath, out LoadResult _);
			}
			tile = (Vector2)dec.data["tile"];
			smoothing = ((ToggleBool)dec.data["imageSmoothing"] == ToggleBool.Enabled);
		}
		LevelEventType eventType = dec.eventType;
		object obj;
		switch (eventType)
		{
		default:
			obj = null;
			break;
		case LevelEventType.AddText:
			obj = "decText";
			break;
		case LevelEventType.AddDecoration:
			obj = "decorationImage";
			break;
		}
		string key = (string)obj;
		object obj2;
		switch (eventType)
		{
		default:
			obj2 = "<i>none</i>";
			break;
		case LevelEventType.AddText:
			obj2 = PropertyControl_List.stringNoTextFormatted;
			break;
		case LevelEventType.AddDecoration:
			obj2 = PropertyControl_List.stringNoImageFormatted;
			break;
		}
		string text2 = (string)obj2;
		string text3 = dec[key] as string;
		bool flag = !string.IsNullOrEmpty(dec[key] as string);
		decorationName = (flag ? text3 : text2);
		base.gameObject.name = (flag ? text3 : "");
		string stringNoTagFormatted = PropertyControl_List.stringNoTagFormatted;
		key = "tag";
		decorationTag = ((!string.IsNullOrEmpty(dec[key] as string)) ? (dec[key] as string) : stringNoTagFormatted);
		string text4 = "";
		if (dec.data.TryGetValue("components", out object value))
		{
			text4 = (value as string);
		}
		string text5 = dec.data["tag"].ToString();
		Vector2 vector = (Vector2)dec.data["position"] * 1.5f;
		Vector2 vector2 = (Vector2)dec.data["pivotOffset"] * 1.5f;
		float num = Convert.ToSingle(dec.data["rotation"]);
		Vector2 a = (Vector2)dec.data["scale"] / 100f;
		int depth = Convert.ToInt32(dec.data["depth"]);
		Vector2 value2 = (Vector2)dec.data["parallax"];
		Color color = dec.data["color"].ToString().HexToColor();
		float num2 = Convert.ToSingle(dec.data["opacity"]) / 100f;
		scrPlanet scrPlanet = null;
		Dictionary<string, object> dictionary = null;
		if (!string.IsNullOrEmpty(text4))
		{
			text4 = text4.Trim();
			dictionary = (Json.Deserialize("{" + text4 + "}") as Dictionary<string, object>);
		}
		DecPlacementType decPlacementType = (DecPlacementType)dec.data["relativeTo"];
		Vector3 zero = Vector3.zero;
		int floor = dec.floor;
		switch (decPlacementType)
		{
		case DecPlacementType.Tile:
			floor = Mathf.Clamp(floor, 0, listFloors.Count - 1);
			zero = listFloors[floor].transform.position;
			vector += new Vector2(zero.x, zero.y);
			break;
		case DecPlacementType.RedPlanet:
			scrPlanet = ADOBase.controller.redPlanet;
			break;
		case DecPlacementType.BluePlanet:
			scrPlanet = ADOBase.controller.bluePlanet;
			break;
		case DecPlacementType.Camera:
		case DecPlacementType.CameraAspect:
			vector /= 1.5f;
			vector2 /= 1.5f;
			break;
		}
		FontName font = dec.data.ContainsKey("font") ? ((FontName)dec.data["font"]) : FontName.Default;
		if (dictionary != null)
		{
			foreach (KeyValuePair<string, object> item in dictionary)
			{
				string key2 = item.Key;
				Component obj3 = null;
				Type type = Type.GetType(key2.Trim());
				if (type != null && type.IsSubclassOf(typeof(MonoBehaviour)))
				{
					obj3 = decTrans.gameObject.AddComponent(type);
				}
				foreach (KeyValuePair<string, object> item2 in item.Value as Dictionary<string, object>)
				{
					FieldInfo field = type.GetField(item2.Key);
					if (field != null)
					{
						field.SetValue(obj3, item2.Value);
					}
				}
			}
		}
		parentFloorNum = dec.floor;
		sourceLevelEvent = dec;
		tags = text5.Split(new char[1]
		{
			' '
		}, StringSplitOptions.RemoveEmptyEntries);
		if (tags.Length == 0)
		{
			tags = new string[1]
			{
				"NO TAG"
			};
		}
		if (ADOBase.isLevelEditor)
		{
			Dictionary<string, List<scrDecoration>> taggedDecorations = manager.taggedDecorations;
			string[] array = tags;
			foreach (string text6 in array)
			{
				if (!taggedDecorations.ContainsKey(text6))
				{
					taggedDecorations[text6] = new List<scrDecoration>();
				}
				taggedDecorations[text6].Add(this);
				if (text6 == "[attachToTile]")
				{
					isChildOfTile = true;
				}
			}
		}
		else
		{
			text3 = ((tags.Length == 0) ? "NO TAG" : text5);
		}
		switch (decType)
		{
		case DecorationType.Text:
		{
			scrTextDecoration obj5 = (scrTextDecoration)this;
			obj5.SetText(text3);
			obj5.SetFont(font);
			break;
		}
		case DecorationType.Image:
		{
			bool flag2 = !text3.IsNullOrEmpty();
			Dictionary<string, scrExtImgHolder.CustomSprite> customSprites = manager.imageHolder.customSprites;
			Sprite sprite = null;
			if (!ADOBase.isLevelEditor)
			{
				sprite = Resources.Load<Sprite>(Path.GetFileNameWithoutExtension(text3));
			}
			else if (flag2 && customSprites[text3] != null)
			{
				sprite = customSprites[text3].sprite;
				if (sprite == null)
				{
					sprite = manager.notFoundSprite;
				}
			}
			else if (dictionary == null)
			{
				sprite = manager.defaultSprite;
			}
			scrVisualDecoration obj4 = this as scrVisualDecoration;
			obj4.SetSprite(sprite);
			obj4.SetTile(tile);
			obj4.SetSmoothing(smoothing);
			if (dec.data.ContainsKey("failHitbox"))
			{
				canHitPlanets = ((ToggleBool)dec.data["failHitbox"] == ToggleBool.Enabled);
				if (dec.data.ContainsKey("failHitboxType"))
				{
					hitboxType = (Hitbox)dec.data["failHitboxType"];
				}
				if (dec.data.ContainsKey("failHitboxScale"))
				{
					hitboxScale = (Vector2)dec.data["failHitboxScale"] / 100f;
				}
				if (dec.data.ContainsKey("failHitboxOffset"))
				{
					hitboxOffset = (Vector2)dec.data["failHitboxOffset"];
				}
				if (dec.data.ContainsKey("failHitboxRotation"))
				{
					hitboxRotation = Convert.ToSingle(dec.data["failHitboxRotation"]);
				}
			}
			obj4.UpdateHitbox();
			break;
		}
		}
		startRot = num;
		startPos = vector;
		followPlanet = scrPlanet;
		SetPosition(vector, vector2);
		SetScale(a * Vector2.one);
		SetDepth(depth);
		SetParallax(value2, decPlacementType);
		SetColor(color);
		SetOpacity(num2);
		SetVisible(dec.visible && !dec.forceHide);
		SetRotation(0f);
	}

	public virtual void UpdateHitbox()
	{
	}
}
