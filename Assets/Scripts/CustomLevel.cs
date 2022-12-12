// CustomLevel
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ADOFAI;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Video;

public class CustomLevel : ADOBase
{
	private const float SpeedMargin = 0.05f;

	public const float TileSize = 1.5f;

	public const string NoTag = "NO TAG";

	public static CustomLevel instance;

	public scrDecorationManager decManager;

	public scrExtImgHolder imgHolder;

	public VideoPlayer videoBG;

	public GameObject editorBG;

	public GameObject levelEditor;

	public Transform camParent;

	public scrCustomBackgroundSprite custBG;

	[NonSerialized]
	public float highestBPM;

	[NonSerialized]
	public LevelData levelData;

	[NonSerialized]
	public scrLevelMaker levelMaker;

	[NonSerialized]
	public new string levelPath;

	[NonSerialized]
	public int checkpointsUsed;

	[NonSerialized]
	public bool isLoading;

	public bool forceOldLevelStyle;

	private Camera camera;

	private GameObject flash;

	private GameObject flashPlus;

	private int startFrame;

	private bool backgroundsLoaded;

	private bool floorSpritesLoaded;

	private string currentSongKey;

	private List<scrFloor> floors => scrLevelMaker.instance.listFloors;

	private List<FreeroamArea> frFloors => scrLevelMaker.instance.listFreeroam;

	private List<scrFloor> frStarts => scrLevelMaker.instance.listFreeroamStartTiles;

	public List<LevelEvent> events => levelData.levelEvents;

	public List<LevelEvent> decorations => levelData.decorations;

	private bool paused => scrController.instance.paused;

	private Dictionary<Filter, MonoBehaviour> filterToComp => scrVfxPlus.instance.filterToComp;

	private Dictionary<FontName, Font> nameToFont => scrController.instance.nameToFont;

	private void Awake()
	{
		isLoading = true;
		printesp("Awake");
		FlushUnusedMemory();
		Resources.UnloadUnusedAssets();
		startFrame = Time.frameCount;
		levelData = new LevelData();
		levelData.Setup();
		levelMaker = scrLevelMaker.instance;
		camera = ADOBase.controller.camy.GetComponent<Camera>();
		flash = GameObject.Find("Flash");
		flashPlus = GameObject.Find("FlashPlus");
		imgHolder = new scrExtImgHolder();
		GameObject.Find("Camera").GetComponent<scrCamera>();
	}

	public void DisableFilters()
	{
		foreach (MonoBehaviour value in filterToComp.Values)
		{
			value.enabled = false;
		}
	}

	private void Update()
	{
		if (GCS.customLevelPaths != null)
		{
			if (Time.frameCount - startFrame == 3)
			{
				LoadAndPlayLevel(GCS.customLevelPaths[GCS.customLevelIndex]);
			}
		}
		else if (levelEditor != null)
		{
			levelEditor.SetActive(value: true);
		}
		float num = 2f * camera.orthographicSize;
		float num2 = num * camera.aspect;
		Vector2 vector = new Vector2(num2, num);
		scrCamera.instance.flashPlusRendererBg.transform.ScaleXY(vector.x, vector.y);
		scrCamera.instance.flashPlusRendererFg.transform.ScaleXY(vector.x, vector.y);
		flash.transform.ScaleXY(vector.x, vector.y);
		if (!levelEditor.activeSelf && RDEditorUtils.CheckForKeyCombo(control: true, shift: true, KeyCode.E))
		{
			levelEditor.SetActive(value: true);
		}
		if (!scrController.instance.controllerUpdate)
		{
			scrController.instance.UpdateLockInput();
			scrController.instance.UpdateFreeroam();
		}
	}

	public bool LoadLevel(string levelPath, out LoadResult status)
	{
		FlushUnusedMemory();
		Resources.UnloadUnusedAssets();
		printesp("");
		isLoading = true;
		bool num = levelData.LoadLevel(levelPath, out status);
		if (num)
		{
			this.levelPath = levelPath;
		}
		return num;
	}

	public bool LoadAndPlayLevel(string levelPath)
	{
		printesp("");
		LoadResult status;
		bool num = LoadLevel(levelPath, out status);
		if (num)
		{
			ADOBase.editor.filenameText.text = Path.GetFileName(levelPath);
			ADOBase.editor.filenameText.fontStyle = FontStyle.Bold;
			ADOBase.conductor.SetupConductorWithLevelData(levelData);
			RemakePath();
			ReloadAssets();
			UpdateDecorationObjects();
			DiscordController.instance?.UpdatePresence();
			Play();
		}
		return num;
	}

	public void ReloadAssets()
	{
		if (!GCS.standaloneLevelMode)
		{
			imgHolder.MarkAllUnused();
		}
		printesp("ReloadAssets");
		ReloadSong();
		UpdateBackgroundSprites();
		UpdateDecorationObjects(reloadDecorations: false);
		UpdateFloorSprites();
		SetBackground();
		UpdateVideo();
		imgHolder.Unload(onlyIfUnused: true);
	}

	public void ApplyEventsToFloors(List<scrFloor> floors)
	{
		ApplyEventsToFloors(floors, levelData, ADOBase.lm, events);
	}

	public void ApplyCoreEventsToFloors(List<scrFloor> floors)
	{
		ApplyCoreEventsToFloors(floors, levelData, ADOBase.lm, events, null);
	}

	public static void ApplyCoreEventsToFloors(List<scrFloor> floors, LevelData levelData, scrLevelMaker lm, List<LevelEvent> events, List<LevelEvent>[] floorEvents)
	{
		if (floorEvents == null)
		{
			floorEvents = new List<LevelEvent>[floors.Count];
			int num = 0;
			for (num = 0; num < floorEvents.Length; num++)
			{
				floorEvents[num] = new List<LevelEvent>();
			}
			for (num = 0; num < events.Count; num++)
			{
				LevelEvent levelEvent = events[num];
				floorEvents[levelEvent.floor].Add(levelEvent);
			}
		}
		float num2 = 1f;
		bool flag = false;
		float bpm = levelData.bpm;
		float radiusScale = 1f;
		int num3 = 2;
		scrController.instance.planetsUsed = 2;
		float widthMult = 1f;
		float lengthMult = 1f;
		foreach (scrFloor floor in floors)
		{
			floor.extraBeats = 0f;
			foreach (LevelEvent item in floorEvents[floor.seqID])
			{
				switch (item.eventType)
				{
					case LevelEventType.SetSpeed:
						_ = item.data.Keys;
						num2 = (((SpeedType)item.data["speedType"] != 0) ? (num2 * item.GetFloat("bpmMultiplier")) : (item.GetFloat("beatsPerMinute") / bpm));
						break;
					case LevelEventType.Twirl:
						flag = !flag;
						floor.isSwirl = true;
						break;
					case LevelEventType.Checkpoint:
						{
							ffxCheckpoint obj = floor.gameObject.AddComponent<ffxCheckpoint>();
							int num4 = 0;
							if (item.data.Keys.Contains("tileOffset"))
							{
								num4 = item.GetInt("tileOffset");
								if (floor.seqID + num4 < 0)
								{
									num4 = -floor.seqID;
								}
								if (floor.seqID + num4 > lm.listFloors.Count - 2)
								{
									num4 = lm.listFloors.Count - 2 - floor.seqID;
								}
							}
							obj.checkpointTileOffset = num4;
							break;
						}
					case LevelEventType.Pause:
						if (floor.nextfloor != null && !floor.midSpin)
						{
							floor.extraBeats += item.GetFloat("duration");
							floor.countdownTicks = item.GetInt("countdownTicks");
							if (!floor.midSpin && Math.Abs(scrMisc.GetAngleMoved(floor.entryangle, floor.exitangle, !floor.isCCW) - 6.2831854820251465) < 0.0001)
							{
								floor.extraBeats += 1f;
							}
							if (item.data.Keys.Contains("angleCorrectionDir"))
							{
								floor.angleCorrectionType = item.GetInt("angleCorrectionDir");
							}
						}
						break;
					case LevelEventType.FreeRoam:
						if (floor.nextfloor != null && item.GetInt("duration") >= 2)
						{
							floor.extraBeats += item.GetInt("duration") - 1;
							floor.countdownTicks = item.GetInt("countdownTicks");
							if (item.data.Keys.Contains("angleCorrectionDir"))
							{
								floor.angleCorrectionType = item.GetInt("angleCorrectionDir");
							}
							if (item.data.Keys.Contains("hitsoundOnBeats"))
							{
								floor.freeroamSoundOnBeat = (HitSound)item.data["hitsoundOnBeats"];
							}
							if (item.data.Keys.Contains("hitsoundOffBeats"))
							{
								floor.freeroamSoundOffBeat = (HitSound)item.data["hitsoundOffBeats"];
							}
						}
						break;
					case LevelEventType.ScaleRadius:
						radiusScale = (float)item.GetInt("scale") / 100f;
						break;
					case LevelEventType.Hold:
						if ((bool)floor.nextfloor && (int)item.data["duration"] >= 0)
						{
							floor.holdLength = (int)item.data["duration"];
						}
						else
						{
							floor.holdLength = -1;
						}
						break;
					case LevelEventType.TileDimensions:
						lengthMult = item.GetFloat("length") / 100f;
						widthMult = item.GetFloat("width") / 100f;
						break;
					case LevelEventType.MultiPlanet:
						if (!floor.prevfloor || ((bool)floor.prevfloor && !floor.prevfloor.midSpin))
						{
							num3 = (int)item.data["planets"];
							if (num3 < 2)
							{
								num3 = 2;
							}
							num3 = Math.Min(num3, 3);
							if (num3 > 3)
							{
								Debug.Log("Planets more than 3 works but is an unreleased feature right now. If you're reading this, please do not release a mod to disable it or share footage, so we can keep the spoiler");
							}
						}
						else if ((bool)floor.prevfloor && floor.prevfloor.midSpin)
						{
							num3 = (int)item.data["planets"];
							if (num3 < 2)
							{
								num3 = 2;
							}
							num3 = Math.Min(num3, 3);
							if (num3 > 3)
							{
								Debug.Log("Planets more than 3 works but is an unreleased feature right now. If you're reading this, please do not release a mod to disable it or share footage, so we can keep the spoiler");
							}
							floor.prevfloor.numPlanets = num3;
						}
						break;
					case LevelEventType.Multitap:
						floor.tapsNeeded = (int)item.data["taps"];
						break;
				}
			}
			floor.radiusScale = radiusScale;
			floor.speed = num2;
			floor.isCCW = flag;
			floor.numPlanets = num3;
			floor.lengthMult = lengthMult;
			floor.widthMult = widthMult;
			if (num3 > scrController.instance.planetsUsed)
			{
				scrController.instance.planetsUsed = num3;
			}
		}
	}

	public static void ApplyEventsToFloors(List<scrFloor> floors, LevelData levelData, scrLevelMaker lm, List<LevelEvent> events)
	{
		List<LevelEvent>[] array = new List<LevelEvent>[floors.Count];
		int num = 0;
		for (num = 0; num < array.Length; num++)
		{
			array[num] = new List<LevelEvent>();
		}
		for (num = 0; num < events.Count; num++)
		{
			LevelEvent levelEvent = events[num];
			if (levelEvent.active)
			{
				array[levelEvent.floor].Add(levelEvent);
			}
		}
		float bpm = levelData.bpm;
		float num2 = scrConductor.instance.song.pitch;
		if (GCS.speedTrialMode && GCS.editorQuickPitchedPlaying)
		{
			num2 *= GCS.currentSpeedTrial;
		}
		foreach (scrFloor floor3 in floors)
		{
			ffxBase[] components = floor3.GetComponents<ffxBase>();
			for (int i = 0; i < components.Length; i++)
			{
				UnityEngine.Object.Destroy(components[i]);
			}
			ffxPlusBase[] components2 = floor3.GetComponents<ffxPlusBase>();
			for (int i = 0; i < components2.Length; i++)
			{
				UnityEngine.Object.Destroy(components2[i]);
			}
			floor3.plusEffects.Clear();
		}
		ApplyCoreEventsToFloors(floors, levelData, lm, events, array);
		lm.CalculateFloorEntryTimes();
		Ease planetEase = levelData.planetEase;
		int planetEaseParts = levelData.planetEaseParts;
		EasePartBehavior planetEasePartBehavior = levelData.planetEasePartBehavior;
		float bpm2 = levelData.bpm;
		Color color = levelData.trackColor;
		Color color2 = levelData.secondaryTrackColor;
		TrackColorType trackColorType = levelData.trackColorType;
		float colorAnimDuration = levelData.trackColorAnimDuration;
		TrackColorPulse pulseType = levelData.trackColorPulse;
		Texture2D texture2D = null;
		float customTextureScale = 0f;
		int pulseLength = levelData.trackPulseLength;
		int seqID = floors[0].seqID;
		TrackAnimationType animationType = levelData.trackAnimation;
		TrackAnimationType2 animationType2 = levelData.trackDisappearAnimation;
		TrackStyle trackStyle = levelData.trackStyle;
		float num3 = levelData.trackBeatsAhead;
		float num4 = levelData.trackBeatsBehind;
		float num5 = 1f;
		int num6 = 2;
		bool auto = false;
		bool isSafe = false;
		float num7 = 1f;
		Vector2 vector = Vector2.zero;
		Vector2 vector2 = Vector2.zero;
		bool flag = false;
		Vector2 vector3 = Vector2.zero;
		Vector2 zero = Vector2.zero;
		float num8 = 1f;
		float num9 = 1f;
		float num10 = 0f;
		float num11 = 1f;
		float num12 = 1f;
		float num13 = 0f;
		bool hideJudgment = false;
		bool hideIcon = false;
		int num14 = 0;
		float a = 0f;
		int num15 = 0;
		float lengthMult = 1f;
		float widthMult = 1f;
		foreach (scrFloor floor4 in floors)
		{
			if (flag)
			{
				vector += vector3;
				vector2 += vector3;
				flag = false;
			}
			List<LevelEvent> list = array[floor4.seqID];
			float speed = floor4.speed;
			bool isCCW = floor4.isCCW;
			foreach (LevelEvent item2 in list)
			{
				switch (item2.eventType)
				{
					case LevelEventType.SetPlanetRotation:
						planetEase = (Ease)item2.data["ease"];
						planetEaseParts = (int)item2.data["easeParts"];
						planetEasePartBehavior = (EasePartBehavior)item2.data["easePartBehavior"];
						break;
					case LevelEventType.ColorTrack:
						{
							color = ((string)item2.data["trackColor"]).HexToColor();
							color2 = Convert.ToString(item2.data["secondaryTrackColor"]).HexToColor();
							colorAnimDuration = item2.GetFloat("trackColorAnimDuration");
							trackColorType = (TrackColorType)item2.data["trackColorType"];
							pulseType = (TrackColorPulse)item2.data["trackColorPulse"];
							pulseLength = (int)item2["trackPulseLength"];
							seqID = floor4.seqID;
							CustomLevel customLevel = instance;
							string text = item2.data["trackTexture"] as string;
							if (!string.IsNullOrEmpty(text))
							{
								string filePath = Path.Combine(Path.GetDirectoryName(customLevel.levelPath), text);
								texture2D = instance.imgHolder.AddTexture(text, out var _, filePath);
								if ((bool)texture2D)
								{
									texture2D.wrapMode = TextureWrapMode.Repeat;
								}
							}
							else
							{
								texture2D = null;
							}
							customTextureScale = item2.GetFloat("trackTextureScale");
							trackStyle = (TrackStyle)item2.data["trackStyle"];
							break;
						}
					case LevelEventType.AnimateTrack:
						animationType = (TrackAnimationType)item2.data["trackAnimation"];
						animationType2 = (TrackAnimationType2)item2.data["trackDisappearAnimation"];
						num3 = item2.GetFloat("beatsAhead");
						num4 = item2.GetFloat("beatsBehind");
						num5 = speed;
						break;
					case LevelEventType.TileDimensions:
						lengthMult = item2.GetFloat("length") / 100f;
						widthMult = item2.GetFloat("width") / 100f;
						break;
					case LevelEventType.Hold:
						if ((int)item2.data["duration"] >= 0)
						{
							bool flag2 = Math.Abs(scrMisc.GetAngleMoved(floor4.entryangle, floor4.exitangle, !isCCW) - 0.0) < 1E-05 || Math.Abs(scrMisc.GetAngleMoved(floor4.entryangle, floor4.exitangle, !isCCW) - Math.PI * 2.0) < 1E-05;
							float num16 = 1f;
							if (item2.data.Keys.Contains("distanceMultiplier"))
							{
								num16 = (float)item2.GetInt("distanceMultiplier") / 100f;
							}
							floor4.holdDistance = (flag2 ? 0f : ((float)((int)item2.data["duration"] * 2 + 1) * num16));
							floor4.holdLength = (int)item2.data["duration"];
							vector3 = (flag2 ? Vector2.zero : (new Vector2(Mathf.Cos(Convert.ToSingle(floor4.exitangle) - MathF.PI / 2f), Mathf.Sin(Convert.ToSingle(floor4.exitangle) + MathF.PI / 2f)) * floor4.holdDistance * 1.5f));
							flag = true;
							if (item2.data.Keys.Contains("landingAnimation"))
							{
								floor4.showHoldTiming = (ToggleBool)item2.data["landingAnimation"] == ToggleBool.Enabled;
							}
							else
							{
								floor4.showHoldTiming = false;
							}
						}
						else
						{
							floor4.holdLength = -1;
						}
						break;
					case LevelEventType.PositionTrack:
						{
							bool flag3 = false;
							if (item2.data.Keys.Contains("editorOnly"))
							{
								flag3 = (ToggleBool)item2.data["editorOnly"] == ToggleBool.Enabled;
							}
							if ((flag3 && scrController.instance.paused) || !flag3)
							{
								int num17 = floor4.seqID;
								if (item2.data.Keys.Contains("relativeTo"))
								{
									num17 = IDFromTile(item2.data["relativeTo"] as Tuple<int, TileRelativeTo>, floor4.seqID, floors);
								}
								vector2 = ((num17 == floor4.seqID) ? (vector + (Vector2)item2.data["positionOffset"] * 1.5f) : ((Vector2)floors[num17].transform.position + (Vector2)item2.data["positionOffset"] * 1.5f - (Vector2)floor4.startPos));
								if (item2.data.Keys.Contains("scale"))
								{
									num12 = (float)item2.GetInt("scale") / 100f;
								}
								if (item2.data.Keys.Contains("opacity"))
								{
									num11 = (float)item2.GetInt("opacity") / 100f;
								}
								if (item2.data.Keys.Contains("rotation"))
								{
									num13 = item2.GetFloat("rotation");
								}
								if (!item2.data.Keys.Contains("justThisTile") || (ToggleBool)item2.data["justThisTile"] != 0)
								{
									vector = vector2;
									num9 = num12;
									num8 = num11;
									num10 = num13;
								}
							}
							break;
						}
					case LevelEventType.MultiPlanet:
						if (!floor4.prevfloor || ((bool)floor4.prevfloor && !floor4.prevfloor.midSpin))
						{
							num6 = (int)item2.data["planets"];
							if (num6 < 2)
							{
								num6 = 2;
							}
							num6 = Math.Min(num6, 3);
							if (num6 > 3)
							{
								Debug.Log("Planets more than 3 works but is an unreleased feature right now. If you're reading this, please do not release a mod to disable it or share footage, so we can keep the spoiler");
							}
						}
						else if ((bool)floor4.prevfloor && floor4.prevfloor.midSpin)
						{
							num6 = (int)item2.data["planets"];
							if (num6 < 2)
							{
								num6 = 2;
							}
							num6 = Math.Min(num6, 3);
							if (num6 > 3)
							{
								Debug.Log("Planets more than 3 works but is an unreleased feature right now. If you're reading this, please do not release a mod to disable it or share footage, so we can keep the spoiler");
							}
							floor4.prevfloor.numPlanets = num6;
						}
						break;
					case LevelEventType.AutoPlayTiles:
						auto = (ToggleBool)item2.data["enabled"] == ToggleBool.Enabled;
						if (item2.data.Keys.Contains("safetyTiles"))
						{
							isSafe = (ToggleBool)item2.data["safetyTiles"] == ToggleBool.Enabled;
						}
						break;
					case LevelEventType.ScaleMargin:
						num7 = (float)item2.GetInt("scale") / 100f;
						break;
					case LevelEventType.Hide:
						hideJudgment = (ToggleBool)item2.data["hideJudgment"] == ToggleBool.Enabled;
						hideIcon = (ToggleBool)item2.data["hideTileIcon"] == ToggleBool.Enabled;
						break;
					case LevelEventType.FreeRoam:
						if (item2.GetInt("duration") >= 2)
						{
							num8 = 0f;
						}
						break;
					case LevelEventType.ChangeTrack:
						color = Convert.ToString(item2.data["trackColor"]).HexToColor();
						color2 = Convert.ToString(item2.data["secondaryTrackColor"]).HexToColor();
						colorAnimDuration = item2.GetFloat("trackColorAnimDuration");
						trackColorType = (TrackColorType)item2.data["trackColorType"];
						pulseType = (TrackColorPulse)item2.data["trackColorPulse"];
						pulseLength = (int)item2["trackPulseLength"];
						seqID = floor4.seqID;
						animationType = (TrackAnimationType)item2.data["trackAnimation"];
						animationType2 = (TrackAnimationType2)item2.data["trackDisappearAnimation"];
						trackStyle = (TrackStyle)item2.data["trackStyle"];
						num3 = item2.GetFloat("beatsAhead");
						num4 = item2.GetFloat("beatsBehind");
						break;
				}
			}
			floor4.numPlanets = num6;
			floor4.isSafe = isSafe;
			floor4.auto = auto;
			floor4.hideJudgment = hideJudgment;
			floor4.hideIcon = hideIcon;
			floor4.marginScale = num7;
			floor4.lengthMult = lengthMult;
			floor4.widthMult = widthMult;
			floor4.planetEase = planetEase;
			floor4.planetEaseParts = planetEaseParts;
			floor4.planetEasePartBehavior = planetEasePartBehavior;
			zero += new Vector2(Mathf.Cos(Convert.ToSingle(floor4.entryangle) - MathF.PI / 2f), Mathf.Sin(Convert.ToSingle(floor4.entryangle) + MathF.PI / 2f)) * (0f - (floor4.radiusScale - 1f)) * 1.5f;
			floor4.transform.position = floor4.startPos + new Vector3(vector2.x, vector2.y, 0f) + new Vector3(zero.x, zero.y, 0f);
			floor4.offsetPos = new Vector3(vector2.x, vector2.y, 0f) + new Vector3(zero.x, zero.y, 0f);
			vector2 = vector;
			floor4.customTexture = texture2D;
			floor4.customTextureScale = customTextureScale;
			switch (trackColorType)
			{
				case TrackColorType.Single:
				case TrackColorType.Glow:
				case TrackColorType.Blink:
				case TrackColorType.Switch:
				case TrackColorType.Volume:
					floor4.SetColor(color);
					break;
				case TrackColorType.Stripes:
					floor4.SetColor(((floor4.seqID - seqID) % 2 == 0) ? color : color2);
					break;
				case TrackColorType.Rainbow:
					floor4.SetColor(Color.white);
					break;
			}
			floor4.styleNum = (int)trackStyle;
			floor4.UpdateAngle();
			floor4.SetTrackStyle(trackStyle, initial: true);
			ffxChangeTrack orAddComponent = floor4.GetOrAddComponent<ffxChangeTrack>();
			orAddComponent.color1 = color;
			orAddComponent.color2 = color2;
			orAddComponent.colorType = trackColorType;
			orAddComponent.colorAnimDuration = colorAnimDuration;
			orAddComponent.pulseType = pulseType;
			orAddComponent.pulseLength = pulseLength;
			float num18 = speed / num5;
			orAddComponent.animationType = animationType;
			orAddComponent.animationType2 = animationType2;
			orAddComponent.tilesAhead = num3 * num18;
			orAddComponent.tilesBehind = num4 * num18;
			float num19 = (float)floor4.entryangle;
			float num20 = (float)floor4.exitangle;
			if (!Mathf.Approximately(Mathf.Round((float)scrMisc.GetAngleMoved(num19, num20, !floor4.isCCW) * 57.29578f), 0f) || floor4.midSpin)
			{
				foreach (LevelEvent item3 in list)
				{
					if (item3.data.Keys.Contains("angleOffset"))
					{
						float @float = item3.GetFloat("angleOffset");
						item3.data["angleOffset"] = @float;
					}
				}
			}
			floor4.transform.localScale = new Vector3(num12, num12, 0f);
			floor4.SetOpacity(scrController.instance.paused ? Mathf.Max(num11, 0.1f) : num11);
			floor4.opacityVal = num11;
			floor4.rotationOffset = num13;
			floor4.SetRotation(num13);
			num12 = num9;
			num11 = num8;
			num13 = num10;
			a = Mathf.Max(a, speed * bpm2 * num2);
			num14++;
		}
		CustomLevel customLevel2 = instance;
		if (customLevel2 != null)
		{
			customLevel2.highestBPM = a;
		}
		num6 = 2;
		lm.ClearFreeroam();
		foreach (scrFloor floor in floors)
		{
			foreach (LevelEvent item4 in events.FindAll((LevelEvent x) => x.floor == floor.seqID))
			{
				switch (item4.eventType)
				{
					case LevelEventType.FreeRoam:
						if (floor.nextfloor != null && item4.GetInt("duration") >= 2)
						{
							floor.freeroamRegion = num15;
							num15++;
							floor.freeroam = true;
							floor.freeroamDimensions = (Vector2)item4["size"];
							floor.freeroamOffset = (Vector2)item4["positionOffset"];
							int @int = item4.GetInt("duration");
							int num23 = item4.GetInt("outTime");
							if (num23 > @int - 1)
							{
								num23 = @int - 1;
							}
							floor.freeroamEndEarlyBeats = num23;
							floor.freeroamEndEase = (Ease)item4.data["outEase"];
							if (item4.data.Keys.Contains("hitsoundOnBeats"))
							{
								floor.freeroamSoundOnBeat = (HitSound)item4.data["hitsoundOnBeats"];
							}
							if (item4.data.Keys.Contains("hitsoundOffBeats"))
							{
								floor.freeroamSoundOffBeat = (HitSound)item4.data["hitsoundOffBeats"];
							}
							floor.SetOpacity(scrController.instance.paused ? 0.1f : 0f);
							floor.opacityVal = 0f;
							lm.MakeFreeroamGrid(floor);
						}
						break;
					case LevelEventType.FreeRoamTwirl:
						if (floor.nextfloor != null)
						{
							Vector2 vector5 = (Vector2)item4["position"];
							int num22 = (int)floor.freeroamDimensions.x * (int)vector5.y + (int)vector5.x;
							if ((float)num22 < floor.freeroamDimensions.x * floor.freeroamDimensions.y)
							{
								scrFloor obj = lm.listFreeroam[floor.freeroamRegion][num22];
								obj.floorIcon = FloorIcon.Swirl;
								obj.UpdateIconSprite();
								obj.isSwirl = true;
							}
						}
						break;
					case LevelEventType.FreeRoamRemove:
						{
							if (!(floor.nextfloor != null))
							{
								break;
							}
							Vector2 vector6 = (Vector2)item4["position"];
							Vector2 vector7 = (Vector2)item4["size"];
							for (int j = (int)vector6.y; j < (int)vector6.y + (int)vector7.y; j++)
							{
								for (int k = (int)vector6.x; k < (int)vector6.x + (int)vector7.x; k++)
								{
									int num24 = (int)floor.freeroamDimensions.x * j + k;
									if ((float)num24 < floor.freeroamDimensions.x * floor.freeroamDimensions.y)
									{
										scrFloor obj2 = lm.listFreeroam[floor.freeroamRegion][num24];
										obj2.isLandable = false;
										obj2.transform.position = Vector3.one * 99999f;
									}
								}
							}
							break;
						}
					case LevelEventType.FreeRoamWarning:
						if (floor.nextfloor != null)
						{
							Vector2 vector4 = (Vector2)item4["position"];
							int num21 = (int)floor.freeroamDimensions.x * (int)vector4.y + (int)vector4.x;
							if ((float)num21 < floor.freeroamDimensions.x * floor.freeroamDimensions.y)
							{
								lm.listFreeroam[floor.freeroamRegion][num21].isWarning = true;
							}
						}
						break;
				}
			}
		}
		scrController.instance.usingOutlines = levelData.floorIconOutlines;
		ffxFlashPlus.legacyFlash = levelData.legacyFlash;
		ffxCameraPlus.legacyRelativeTo = levelData.legacyCamRelativeTo;
		int num25 = 0;
		foreach (scrFloor floor5 in floors)
		{
			FloorIcon floorIcon = FloorIcon.None;
			List<LevelEvent> list2 = array[floor5.seqID];
			bool flag4 = false;
			if (list2.Count > 0)
			{
				flag4 = list2.Any((LevelEvent e) => e.eventType == LevelEventType.EditorComment);
				floorIcon = FloorIcon.Vfx;
				floor5.eventIcon = LevelEventType.None;
				bool flag5 = false;
				int num26 = 0;
				int num27 = 2;
				LevelEventType eventType = list2[0].eventType;
				LevelEventType filteredEvent = GCS.filteredEvent;
				bool flag6 = filteredEvent != 0 && scrController.instance.paused;
				bool flag7 = false;
				foreach (LevelEvent item5 in list2)
				{
					if (!item5.active)
					{
						continue;
					}
					LevelEventType eventType2 = item5.eventType;
					if (eventType2 == filteredEvent && flag6)
					{
						flag7 = true;
					}
					if (eventType2 == LevelEventType.Checkpoint)
					{
						if (num26 >= 1)
						{
							continue;
						}
						num26 = 1;
						floorIcon = FloorIcon.Checkpoint;
						flag5 = true;
					}
					switch (eventType2)
					{
						case LevelEventType.SetSpeed:
							{
								if (num26 >= 2)
								{
									continue;
								}
								num26 = 2;
								float num30 = ((floor5.seqID <= 0) ? 1f : floors[floor5.seqID - 1].speed);
								float num31 = (floor5.speed - num30) / num30;
								float num32 = Mathf.Abs(num31);
								if (num32 > 0.05f)
								{
									floorIcon = ((!(num31 > 0f)) ? ((1f - num32 > 0.45f) ? FloorIcon.Snail : FloorIcon.DoubleSnail) : ((num32 < 1.05f) ? FloorIcon.Rabbit : FloorIcon.DoubleRabbit));
								}
								else
								{
									floorIcon = FloorIcon.SameSpeed;
									num26 = 0;
								}
								flag5 = true;
								break;
							}
						case LevelEventType.Twirl:
							if (num26 >= 2)
							{
								continue;
							}
							num26 = 2;
							floorIcon = FloorIcon.Swirl;
							flag5 = true;
							break;
						default:
							if (item5.eventType == LevelEventType.Hold)
							{
								if (num26 >= 2)
								{
									continue;
								}
								floorIcon = ((floor5.holdLength != 0) ? FloorIcon.HoldArrowLong : FloorIcon.HoldArrowShort);
								flag5 = true;
							}
							else if (item5.eventType == LevelEventType.MultiPlanet)
							{
								if (num26 >= 2)
								{
									continue;
								}
								int num28 = ((floor5.seqID <= 0) ? 1 : floors[floor5.seqID - 1].numPlanets);
								float num29 = floor5.numPlanets;
								if (num29 == 2f)
								{
									floorIcon = FloorIcon.MultiPlanetTwo;
								}
								else if (num29 > (float)num28)
								{
									floorIcon = FloorIcon.MultiPlanetThreeMore;
								}
								else if (num29 <= (float)num28)
								{
									floorIcon = FloorIcon.MultiPlanetThreeLess;
								}
								flag5 = true;
							}
							else if (!flag5 && eventType2 != eventType && num26 == 0)
							{
								flag5 = true;
							}
							break;
					}
					if (num26 == num27)
					{
						break;
					}
				}
				if (!flag5)
				{
					floorIcon = FloorIcon.Vfx;
					floor5.eventIcon = eventType;
				}
				if (flag6)
				{
					if (flag7)
					{
						floorIcon = FloorIcon.Vfx;
						floor5.eventIcon = filteredEvent;
					}
					else
					{
						floorIcon = FloorIcon.None;
					}
				}
			}
			if (num25 > 0 && (floorIcon == FloorIcon.Vfx || floorIcon == FloorIcon.None))
			{
				floorIcon = ((num25 == 1) ? FloorIcon.HoldReleaseShort : FloorIcon.HoldReleaseLong);
			}
			num25 = 0;
			if (list2.Exists((LevelEvent x) => x.eventType == LevelEventType.Hold))
			{
				num25 = ((floor5.holdLength == 0) ? 1 : 2);
			}
			floor5.floorIcon = floorIcon;
			floor5.UpdateIconSprite();
			floor5.UpdateCommentGlow(scrController.instance.paused && flag4);
		}
		if (scrController.instance.paused)
		{
			return;
		}
		Dictionary<int, Dictionary<string, Tuple<int, float>>> dictionary = new Dictionary<int, Dictionary<string, Tuple<int, float>>>();
		foreach (LevelEvent item6 in events.FindAll((LevelEvent x) => x.info.type == LevelEventType.RepeatEvents))
		{
			if (item6.active)
			{
				int item = Convert.ToInt32(item6.data["repetitions"]);
				float float2 = item6.GetFloat("interval");
				string[] array2 = (item6.GetString("tag") ?? "").Split(" ");
				if (!dictionary.ContainsKey(item6.floor))
				{
					dictionary[item6.floor] = new Dictionary<string, Tuple<int, float>>();
				}
				string[] array3 = array2;
				foreach (string key in array3)
				{
					dictionary[item6.floor][key] = new Tuple<int, float>(item, float2);
				}
			}
		}
		Dictionary<int, string[]> dictionary2 = new Dictionary<int, string[]>();
		foreach (LevelEvent item7 in events.FindAll((LevelEvent x) => x.info.type == LevelEventType.SetConditionalEvents))
		{
			if (item7.active)
			{
				string[] value = new string[5]
				{
					item7.GetString("perfectTag"),
					item7.GetString("hitTag"),
					item7.GetString("barelyTag"),
					item7.GetString("missTag"),
					item7.GetString("lossTag")
				};
				dictionary2.Add(item7.floor, value);
				floors[item7.floor].hasConditionalChange = true;
			}
		}
		foreach (LevelEvent @event in events)
		{
			if (!@event.active)
			{
				continue;
			}
			int num33 = 0;
			float num34 = 0f;
			int floor2 = @event.floor;
			@event.data.TryGetValue("eventTag", out var value2);
			if (value2 != null)
			{
				string[] array4 = (value2 as string).Split(" ");
				if (dictionary.Keys.Contains(floor2))
				{
					Dictionary<string, Tuple<int, float>> dictionary3 = dictionary[floor2];
					string[] array3 = array4;
					foreach (string key2 in array3)
					{
						if (dictionary3.ContainsKey(key2))
						{
							num33 = dictionary3[key2].Item1;
							num34 = dictionary3[key2].Item2;
							break;
						}
					}
				}
			}
			for (num = 0; num <= num33; num++)
			{
				float offset = num34 * (float)num * 180f;
				ffxPlusBase ffxPlusBase2 = ApplyEvent(@event, bpm, num2, floors, offset);
				if (EditorConstants.soloTypes.Contains(@event.eventType) || @event.eventType == LevelEventType.RepeatEvents)
				{
					break;
				}
				if (!dictionary2.Keys.Contains(floor2))
				{
					continue;
				}
				if (!(ffxPlusBase2 == null))
				{
					bool[] array5 = new bool[5];
					for (int l = 0; l < dictionary2[floor2].Length; l++)
					{
						string text2 = dictionary2[floor2][l];
						array5[l] = text2 != "NONE" && @event.GetString("eventTag") == text2;
					}
					ffxPlusBase2.conditionalInfo = array5;
				}
				break;
			}
		}
		ffxCameraPlus ffxCameraPlus2 = floors[0].gameObject.AddComponent<ffxCameraPlus>();
		floors[0].plusEffects.Add(ffxCameraPlus2);
		ffxCameraPlus2.startTime = 0.0;
		ffxCameraPlus2.duration = 0f;
		ffxCameraPlus2.targetPos = levelData.camPosition * 1.5f;
		ffxCameraPlus2.targetRot = levelData.camRotation;
		ffxCameraPlus2.targetZoom = levelData.camZoom / 100f;
		ffxCameraPlus2.ease = Ease.Linear;
		ffxCameraPlus2.movementType = levelData.camRelativeTo;
	}

	public static ffxPlusBase ApplyEvent(LevelEvent evnt, float bpm, float pitch, List<scrFloor> floors, float offset = 0f)
	{
		int floor = evnt.floor;
		scrFloor scrFloor2 = floors[floor];
		GameObject gameObject = scrFloor2.gameObject;
		float speed = floors[floor].speed;
		float num = 60f / (bpm * pitch * speed);
		_ = evnt.data.Keys;
		ffxPlusBase ffxPlusBase2 = null;
		switch (evnt.eventType)
		{
			case LevelEventType.SetHitsound:
				{
					ffxSetHitsound ffxSetHitsound2 = gameObject.AddComponent<ffxSetHitsound>();
					ffxSetHitsound2.gameSound = (GameSound)evnt.data["gameSound"];
					ffxSetHitsound2.hitSound = (HitSound)evnt.data["hitsound"];
					ffxSetHitsound2.volume = (float)evnt.GetInt("hitsoundVolume") / 100f;
					scrFloor2.setHitsound = ffxSetHitsound2;
					break;
				}
			case LevelEventType.CustomBackground:
				if (!(instance == null))
				{
					((ffxCustomBackgroundPlus)(ffxPlusBase2 = gameObject.AddComponent<ffxCustomBackgroundPlus>())).color = Convert.ToString(evnt.data["color"]).HexToColor();
				}
				break;
			case LevelEventType.Flash:
				{
					ffxFlashPlus ffxFlashPlus2 = gameObject.AddComponent<ffxFlashPlus>();
					ffxPlusBase2 = ffxFlashPlus2;
					ffxFlashPlus2.duration = evnt.GetFloat("duration") * num;
					Color startColor = Convert.ToString(evnt.data["startColor"]).HexToColor();
					Color endColor = Convert.ToString(evnt.data["endColor"]).HexToColor();
					ffxFlashPlus2.startColor = startColor;
					ffxFlashPlus2.endColor = endColor;
					ffxFlashPlus2.startColor.a = evnt.GetFloat("startOpacity") / 100f;
					ffxFlashPlus2.endColor.a = evnt.GetFloat("endOpacity") / 100f;
					ffxFlashPlus2.ease = (Ease)evnt.data["ease"];
					int num2 = Convert.ToInt32(evnt.data["plane"]);
					ffxFlashPlus2.FG = num2 == 0;
					if (Application.isPlaying)
					{
						ffxFlashPlus2.FlashSetup();
					}
					break;
				}
			case LevelEventType.MoveCamera:
				{
					ffxCameraPlus ffxCameraPlus2 = gameObject.AddComponent<ffxCameraPlus>();
					ffxPlusBase2 = ffxCameraPlus2;
					ffxCameraPlus2.duration = RDUtils.GetRandomFloat(evnt, "duration") * num;
					Vector2 randomVector = RDUtils.GetRandomVector2(evnt, "position");
					ffxCameraPlus2.targetPos = randomVector * 1.5f;
					ffxCameraPlus2.positionUsed = !evnt.disabled["position"];
					ffxCameraPlus2.targetRot = RDUtils.GetRandomFloat(evnt, "rotation");
					ffxCameraPlus2.rotationUsed = !evnt.disabled["rotation"];
					ffxCameraPlus2.targetZoom = RDUtils.GetRandomFloat(evnt, "zoom") / 100f;
					ffxCameraPlus2.zoomUsed = !evnt.disabled["zoom"];
					ffxCameraPlus2.ease = (Ease)evnt.data["ease"];
					ffxCameraPlus2.movementType = (CamMovementType)evnt.data["relativeTo"];
					ffxCameraPlus2.movementTypeUsed = !evnt.disabled["relativeTo"];
					if (evnt.data.ContainsKey("dontDisable"))
					{
						ffxCameraPlus2.dontDisable = (ToggleBool)evnt.data["dontDisable"] == ToggleBool.Enabled;
					}
					if (evnt.data.ContainsKey("minVfxOnly"))
					{
						ffxCameraPlus2.disableIfMaxFx = (ToggleBool)evnt.data["minVfxOnly"] == ToggleBool.Enabled;
					}
					break;
				}
			case LevelEventType.SetHoldSound:
				{
					ffxSetHoldsound obj10 = gameObject.AddComponent<ffxSetHoldsound>();
					obj10.holdStartSound = (HoldStartSound)evnt.data["holdStartSound"];
					obj10.holdLoopSound = (HoldLoopSound)evnt.data["holdLoopSound"];
					obj10.holdEndSound = (HoldEndSound)evnt.data["holdEndSound"];
					obj10.holdMidSound = (HoldMidSound)evnt.data["holdMidSound"];
					obj10.holdMidSoundType = (HoldMidSoundType)evnt.data["holdMidSoundType"];
					obj10.holdMidSoundDelay = evnt.GetFloat("holdMidSoundDelay") * num;
					obj10.holdMidSoundTiming = (HoldMidSoundTimingRelativeTo)evnt.data["holdMidSoundTimingRelativeTo"];
					obj10.volume = (float)evnt.GetInt("holdSoundVolume") / 100f;
					break;
				}
			case LevelEventType.RecolorTrack:
				{
					ffxPlusBase obj11 = (ffxPlusBase2 = gameObject.AddComponent<ffxRecolorFloorPlus>());
					Tuple<int, TileRelativeTo> tile3 = evnt.data["startTile"] as Tuple<int, TileRelativeTo>;
					((ffxRecolorFloorPlus)obj11).start = IDFromTile(tile3, floor, floors);
					Tuple<int, TileRelativeTo> tile4 = evnt.data["endTile"] as Tuple<int, TileRelativeTo>;
					((ffxRecolorFloorPlus)obj11).end = IDFromTile(tile4, floor, floors);
					((ffxRecolorFloorPlus)obj11).color1 = Convert.ToString(evnt.data["trackColor"]).HexToColor();
					((ffxRecolorFloorPlus)obj11).color2 = Convert.ToString(evnt.data["secondaryTrackColor"]).HexToColor();
					((ffxRecolorFloorPlus)obj11).colorAnimDuration = evnt.GetFloat("trackColorAnimDuration");
					((ffxRecolorFloorPlus)obj11).colorType = (TrackColorType)evnt.data["trackColorType"];
					((ffxRecolorFloorPlus)obj11).pulseType = (TrackColorPulse)evnt.data["trackColorPulse"];
					((ffxRecolorFloorPlus)obj11).pulseLength = (int)evnt["trackPulseLength"];
					((ffxRecolorFloorPlus)obj11).style = (TrackStyle)evnt.data["trackStyle"];
					break;
				}
			case LevelEventType.MoveTrack:
				{
					ffxMoveFloorPlus ffxMoveFloorPlus2 = gameObject.AddComponent<ffxMoveFloorPlus>();
					ffxPlusBase2 = ffxMoveFloorPlus2;
					ffxMoveFloorPlus2.duration = evnt.GetFloat("duration") * num;
					Tuple<int, TileRelativeTo> tile = evnt.data["startTile"] as Tuple<int, TileRelativeTo>;
					ffxMoveFloorPlus2.start = IDFromTile(tile, floor, floors);
					Tuple<int, TileRelativeTo> tile2 = evnt.data["endTile"] as Tuple<int, TileRelativeTo>;
					ffxMoveFloorPlus2.end = IDFromTile(tile2, floor, floors);
					if (evnt.data.ContainsKey("gapLength"))
					{
						ffxMoveFloorPlus2.gapLength = evnt.GetInt("gapLength");
					}
					Vector2 vector3 = (Vector2)evnt.data["positionOffset"];
					ffxMoveFloorPlus2.targetPos = 1.5f * vector3;
					ffxMoveFloorPlus2.positionUsed = !evnt.disabled["positionOffset"];
					ffxMoveFloorPlus2.targetRot = evnt.GetFloat("rotationOffset");
					ffxMoveFloorPlus2.rotationUsed = !evnt.disabled["rotationOffset"];
					ffxMoveFloorPlus2.targetScaleV2 = (Vector2)evnt.data["scale"] / 100f;
					ffxMoveFloorPlus2.scaleUsed = !evnt.disabled["scale"];
					ffxMoveFloorPlus2.targetOpacity = evnt.GetFloat("opacity") / 100f;
					ffxMoveFloorPlus2.opacityUsed = !evnt.disabled["opacity"];
					if (evnt.data.ContainsKey("maxVfxOnly"))
					{
						ffxMoveFloorPlus2.disableIfMinFx = (ToggleBool)evnt.data["maxVfxOnly"] == ToggleBool.Enabled;
					}
					ffxMoveFloorPlus2.ease = (Ease)evnt.data["ease"];
					break;
				}
			case LevelEventType.MoveDecorations:
				{
					ffxMoveDecorationsPlus ffxMoveDecorationsPlus2 = gameObject.AddComponent<ffxMoveDecorationsPlus>();
					ffxPlusBase2 = ffxMoveDecorationsPlus2;
					ffxMoveDecorationsPlus2.decManager = ((instance == null) ? GameObject.Find("Decoration Container").GetComponent<scrDecorationManager>() : instance.decManager);
					ffxMoveDecorationsPlus2.duration = evnt.GetFloat("duration") * num;
					ffxMoveDecorationsPlus2.targetImageFilename = evnt.GetString("decorationImage");
					ffxMoveDecorationsPlus2.imageFilenameUsed = !evnt.disabled["decorationImage"];
					ffxMoveDecorationsPlus2.targetDepth = evnt.GetInt("depth");
					ffxMoveDecorationsPlus2.depthUsed = !evnt.disabled["depth"];
					ffxMoveDecorationsPlus2.targetParallax = (Vector2)evnt.data["parallax"];
					ffxMoveDecorationsPlus2.parallaxUsed = !evnt.disabled["parallax"];
					Vector2 vector4 = (Vector2)evnt.data["positionOffset"];
					ffxMoveDecorationsPlus2.positionUsed = !evnt.disabled["positionOffset"];
					ffxMoveDecorationsPlus2.targetPos = 1.5f * vector4;
					ffxMoveDecorationsPlus2.targetRot = evnt.GetFloat("rotationOffset");
					ffxMoveDecorationsPlus2.rotationUsed = !evnt.disabled["rotationOffset"];
					ffxMoveDecorationsPlus2.targetScaleV2 = (Vector2)evnt.data["scale"] / 100f;
					ffxMoveDecorationsPlus2.scaleUsed = !evnt.disabled["scale"];
					ffxMoveDecorationsPlus2.targetColor = evnt.GetString("color").HexToColor();
					ffxMoveDecorationsPlus2.colorUsed = !evnt.disabled["color"];
					ffxMoveDecorationsPlus2.targetOpacity = evnt.GetFloat("opacity") / 100f;
					ffxMoveDecorationsPlus2.opacityUsed = !evnt.disabled["opacity"];
					string[] array2 = evnt.data["tag"].ToString().Split(new char[1] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
					if (array2.Length == 0)
					{
						array2 = new string[1] { "NO TAG" };
					}
					ffxMoveDecorationsPlus2.targetTags.AddRange(array2);
					ffxMoveDecorationsPlus2.ease = (Ease)evnt.data["ease"];
					break;
				}
			case LevelEventType.SetText:
				{
					ffxSetTextPlus ffxSetTextPlus2 = gameObject.AddComponent<ffxSetTextPlus>();
					ffxPlusBase2 = ffxSetTextPlus2;
					ffxSetTextPlus2.decManager = instance.decManager;
					ffxSetTextPlus2.targetString = evnt.GetString("decText");
					string[] array = evnt.data["tag"].ToString().Split(new char[1] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
					if (array.Length == 0)
					{
						array = new string[1] { "NO TAG" };
					}
					ffxSetTextPlus2.targetTags.AddRange(array);
					break;
				}
			case LevelEventType.SetFilter:
				{
					ffxPlusBase obj9 = (ffxPlusBase2 = gameObject.AddComponent<ffxSetFilterPlus>());
					((ffxSetFilterPlus)obj9).filter = (Filter)evnt.data["filter"];
					((ffxSetFilterPlus)obj9).enableFilter = (ToggleBool)evnt.data["enabled"] == ToggleBool.Enabled;
					((ffxSetFilterPlus)obj9).intensity = evnt.GetFloat("intensity") / 100f;
					((ffxSetFilterPlus)obj9).disableOthers = (ToggleBool)evnt.data["disableOthers"] == ToggleBool.Enabled;
					obj9.duration = evnt.GetFloat("duration") * num;
					obj9.ease = (Ease)evnt.data["ease"];
					break;
				}
			case LevelEventType.HallOfMirrors:
				((ffxHallOfMirrorsPlus)(ffxPlusBase2 = gameObject.AddComponent<ffxHallOfMirrorsPlus>())).enableHOM = (ToggleBool)evnt.data["enabled"] == ToggleBool.Enabled;
				break;
			case LevelEventType.ShakeScreen:
				{
					ffxPlusBase obj8 = (ffxPlusBase2 = gameObject.AddComponent<ffxShakeScreenPlus>());
					((ffxShakeScreenPlus)obj8).intensity = evnt.GetFloat("intensity") / 100f;
					((ffxShakeScreenPlus)obj8).strength = evnt.GetFloat("strength") / 100f;
					obj8.duration = evnt.GetFloat("duration") * num;
					((ffxShakeScreenPlus)obj8).fadeOut = (ToggleBool)evnt.data["fadeOut"] == ToggleBool.Enabled;
					break;
				}
			case LevelEventType.Bloom:
				{
					ffxPlusBase obj7 = (ffxPlusBase2 = gameObject.AddComponent<ffxBloomPlus>());
					((ffxBloomPlus)obj7).enableBloom = (ToggleBool)evnt.data["enabled"] == ToggleBool.Enabled;
					((ffxBloomPlus)obj7).threshold = evnt.GetFloat("threshold") / 100f;
					((ffxBloomPlus)obj7).intensity = evnt.GetFloat("intensity") / 100f;
					((ffxBloomPlus)obj7).color = Convert.ToString(evnt.data["color"]).HexToColor();
					obj7.duration = evnt.GetFloat("duration") * num;
					obj7.ease = (Ease)evnt.data["ease"];
					break;
				}
			case LevelEventType.ScreenTile:
				{
					ffxPlusBase obj6 = (ffxPlusBase2 = gameObject.AddComponent<ffxScreenTilePlus>());
					Vector2 vector2 = (Vector2)evnt.data["tile"];
					((ffxScreenTilePlus)obj6).tileX = vector2.x;
					((ffxScreenTilePlus)obj6).tileY = vector2.y;
					break;
				}
			case LevelEventType.ScreenScroll:
				{
					ffxPlusBase obj5 = (ffxPlusBase2 = gameObject.AddComponent<ffxScreenScrollPlus>());
					Vector2 vector = (Vector2)evnt.data["scroll"];
					((ffxScreenScrollPlus)obj5).scrollX = vector.x / 100f;
					((ffxScreenScrollPlus)obj5).scrollY = vector.y / 100f;
					break;
				}
			case LevelEventType.CallMethod:
				{
					ffxPlusBase obj4 = (ffxPlusBase2 = gameObject.AddComponent<ffxCallMethod>());
					((ffxCallMethod)obj4).methodName = evnt.GetString("method");
					((ffxCallMethod)obj4).Setup();
					break;
				}
			case LevelEventType.AddComponent:
				{
					ffxPlusBase obj3 = (ffxPlusBase2 = gameObject.AddComponent<ffxAddComponent>());
					((ffxAddComponent)obj3).componentName = evnt.GetString("component");
					((ffxAddComponent)obj3).properties = evnt.GetString("properties");
					obj3.duration = evnt.GetFloat("duration") * num;
					((ffxAddComponent)obj3).Setup();
					break;
				}
			case LevelEventType.KillPlayer:
				{
					ffxPlusBase obj2 = (ffxPlusBase2 = gameObject.AddComponent<ffxKillPlayer>());
					((ffxKillPlayer)obj2).instant = (ToggleBool)evnt.data["playAnimation"] == ToggleBool.Disabled;
					((ffxKillPlayer)obj2).failMessage = evnt.GetString("failMessage");
					break;
				}
			case LevelEventType.PlaySound:
				{
					ffxPlusBase obj = (ffxPlusBase2 = gameObject.AddComponent<ffxPlaySound>());
					((ffxPlaySound)obj).hitSound = (HitSound)evnt.data["hitsound"];
					((ffxPlaySound)obj).volume = (float)evnt.GetInt("hitsoundVolume") / 100f;
					break;
				}
		}
		if (ffxPlusBase2 != null)
		{
			floors[floor].plusEffects.Add(ffxPlusBase2);
			ffxPlusBase2.SetStartTime(bpm, evnt.GetFloat("angleOffset") + offset);
			return ffxPlusBase2;
		}
		return null;
	}

	public static int IDFromTile(Tuple<int, TileRelativeTo> tile, int tileID, List<scrFloor> floors)
	{
		int value = 0;
		switch (tile.Item2)
		{
			case TileRelativeTo.ThisTile:
				value = tileID + tile.Item1;
				break;
			case TileRelativeTo.Start:
				value = tile.Item1;
				break;
			case TileRelativeTo.End:
				value = floors.Count - 1 + tile.Item1;
				break;
		}
		return Mathf.Clamp(value, 0, floors.Count - 1);
	}

	public static Tuple<int, TileRelativeTo> StringToTile(string sTile)
	{
		if (sTile.StartsWith("(") && sTile.EndsWith(")"))
		{
			sTile = sTile.Substring(1, sTile.Length - 2);
		}
		string[] array = sTile.Split(',');
		return new Tuple<int, TileRelativeTo>(int.Parse(array[0]), (TileRelativeTo)Enum.Parse(typeof(TileRelativeTo), array[1]));
	}

	public void UpdateDecorationObjects(bool reloadDecorations = true)
	{
		decManager.ClearDecorations();
		foreach (LevelEvent decoration in decorations)
		{
			if (!decoration.active)
			{
				continue;
			}
			bool spritesLoaded = false;
			object value;
			if (reloadDecorations)
			{
				decManager.CreateDecoration(decoration, out spritesLoaded);
			}
			else if (decoration.data.TryGetValue("decorationImage", out value))
			{
				string text = (string)value;
				if (!string.IsNullOrEmpty(text))
				{
					string filePath = Path.Combine(Path.GetDirectoryName(levelPath), text);
					ADOBase.customLevel.imgHolder.AddSprite(text, filePath, out var status);
					ADOBase.editor.UpdateImageLoadResult(text, status);
				}
			}
		}
		foreach (LevelEvent @event in events)
		{
			if (@event.eventType == LevelEventType.MoveDecorations && @event.data.TryGetValue("decorationImage", out var value2) && value2 is string text2 && !string.IsNullOrEmpty(text2))
			{
				string filePath2 = Path.Combine(Path.GetDirectoryName(levelPath), text2);
				imgHolder.AddSprite(text2, filePath2, out var status2);
				ADOBase.editor.UpdateImageLoadResult(text2, status2);
			}
		}
	}

	public void RemakePath(bool applyEventsToFloors = true)
	{
		ADOBase.conductor.SetupConductorWithLevelData(levelData);
		levelMaker.leveldata = levelData.pathData;
		levelMaker.floorAngles = levelData.angleData.ToArray();
		levelMaker.isOldLevel = levelData.isOldLevel;
		levelMaker.MakeLevel();
		_ = GCS.checkpointNum;
		scrConductor.instance.countdownTicks = levelData.countdownTicks;
		_ = ADOBase.controller.redPlanet;
		_ = ADOBase.controller.bluePlanet;
		if (applyEventsToFloors)
		{
			ApplyEventsToFloors(floors);
		}
		levelMaker.DrawHolds();
		levelMaker.DrawMultiPlanet();
	}

	public void UpdateBackgroundSprites()
	{
		printesp("");
		if (GCS.standaloneLevelMode && backgroundsLoaded)
		{
			return;
		}
		foreach (LevelEvent item in events.FindAll((LevelEvent x) => x.eventType == LevelEventType.CustomBackground))
		{
			string text = item.data["bgImage"].ToString();
			if (!string.IsNullOrEmpty(text))
			{
				string filePath = Path.Combine(Path.GetDirectoryName(levelPath), text);
				imgHolder.AddSprite(text, filePath, out var _);
			}
		}
		backgroundsLoaded = true;
	}

	public void UpdateFloorSprites()
	{
		printesp("");
		if (GCS.standaloneLevelMode && floorSpritesLoaded)
		{
			return;
		}
		foreach (LevelEvent item in events.FindAll((LevelEvent x) => x.eventType == LevelEventType.ColorTrack))
		{
			string text = item.data["trackTexture"] as string;
			if (!string.IsNullOrEmpty(text))
			{
				string filePath = Path.Combine(Path.GetDirectoryName(levelPath), text);
				imgHolder.AddTexture(text, out var _, filePath);
			}
		}
		floorSpritesLoaded = true;
	}

	public bool Play(int seqID = 0)
	{
		LevelEvent miscSettings = levelData.miscSettings;
		if (miscSettings.data.ContainsKey("customClass"))
		{
			string text = miscSettings["customClass"] as string;
			printe("trying to load " + text);
			Type type = Type.GetType(text);
			if (type != null)
			{
				Level level = Activator.CreateInstance(type) as Level;
				scrController.instance.level = level;
				printe("custom level loaded " + (level != null));
			}
		}
		printesp("Play");
		if (floors.Count == 1)
		{
			return false;
		}
		scrController scrController2 = scrController.instance;
		scrConductor scrConductor2 = scrConductor.instance;
		checkpointsUsed = scrController.checkpointsUsed;
		scrController2.gameworld = true;
		string fullCaptionTagged = levelData.fullCaptionTagged;
		scrPlanet redPlanet = scrController2.redPlanet;
		scrPlanet bluePlanet = scrController2.bluePlanet;
		scrController2.caption = fullCaptionTagged;
		scrController2.stickToFloor = levelData.stickToFloors;
		scrController2.chosenplanet = redPlanet;
		if (ADOBase.isLevelEditor)
		{
			redPlanet.Rewind();
			redPlanet.transform.localPosition = Vector3.zero;
			redPlanet.shouldPrint = true;
			bluePlanet.Rewind();
			bluePlanet.transform.localPosition = Vector3.right;
			bluePlanet.shouldPrint = true;
			for (int i = 0; i < scrController2.planetList.Count; i++)
			{
				if (i > 1)
				{
					scrController2.planetList[i].Rewind();
					scrController2.planetList[i].transform.localPosition = Vector3.zero;
					scrController2.planetList[i].shouldPrint = true;
				}
			}
		}
		scrConductor2.onBeats.Clear();
		redPlanet.samuraiSprite.GetComponent<scrSamurai>().Setup();
		bluePlanet.samuraiSprite.GetComponent<scrSamurai>().Setup();
		AudioManager.Instance.StopAllSounds();
		scrCamera.instance.Rewind();
		camParent.transform.position = Vector3.zero;
		scrConductor2.Rewind();
		scrController2.Awake_Rewind();
		scrCamera scrCamera2 = scrCamera.instance;
		scrCamera2.zoomSize = 1f;
		if (!Input.GetKey(KeyCode.LeftShift) && !Input.GetKey(KeyCode.RightShift))
		{
			scrCamera2.userSizeMultiplier = 1f;
		}
		scrCamera2.SetHoldOffset(Vector3.zero);
		scrCamera2.lastEventRelativePosition = Vector2.zero;
		scrCamera2.lastUsedMovementType = CamMovementType.Player;
		scrVfxPlus.instance.vidOffset = (float)(int)levelData.miscSettings.data["vidOffset"] * 0.001f;
		scrFloor scrFloor2 = floors[seqID];
		scrController2.isCW = !scrFloor2.isCCW;
		scrController2.speed = scrFloor2.speed;
		scrController2.rotationEase = scrFloor2.planetEase;
		scrController2.chosenplanet = ((scrFloor2.seqID % 2 == 0) ? redPlanet : bluePlanet);
		scrController2.keyTimes.Clear();
		if (ADOBase.isLevelEditor)
		{
			scrController2.forceNoCountdown = (scrConductor2.fastTakeoff = RDC.auto && seqID == 0);
		}
		scrController2.txtAllStrictClear.gameObject.SetActive(value: false);
		scrController2.txtCongrats.gameObject.SetActive(value: false);
		scrController2.txtResults.gameObject.SetActive(value: false);
		scrController2.txtPercent.gameObject.SetActive(value: false);
		printe("CustomLevel.Play()");
		if (GCS.standaloneLevelMode)
		{
			printe("waiting....");
			StartCoroutine(scrController2.WaitForStartCo(seqID));
		}
		else
		{
			scrConductor2.Start();
			FinishCustomLevelLoading(seqID, bluePlanet, redPlanet);
			scrController2.Start_Rewind(seqID);
			RDUtils.SetGarbageCollectionEnabled(enabled: false);
		}
		if ((bool)ADOBase.editor)
		{
			ADOBase.editor.inStrictlyEditingMode = false;
		}
		return true;
	}

	public void FinishCustomLevelLoading(int seqID, scrPlanet bluePlanet, scrPlanet redPlanet)
	{
		if (!GCS.standaloneLevelMode)
		{
			ApplyEventsToFloors(floors);
			ADOBase.controller.chosenplanet.FirstFloorAngleSetup();
		}
		foreach (scrFloor floor in floors)
		{
			if (floor.seqID <= seqID)
			{
				floor.topGlow.enabled = true;
				continue;
			}
			break;
		}
		if (!GCS.standaloneLevelMode)
		{
			UpdateDecorationObjects();
		}
		UpdateVideo();
		if (!GCS.standaloneLevelMode)
		{
			PrepVfx(seqID == 0);
		}
		if (GCS.standaloneLevelMode && !GCS.useNoFail)
		{
			Persistence.IncrementCustomWorldAttempts(levelData.Hash);
		}
		ADOBase.controller.currentSeqID = GCS.checkpointNum;
	}

	public void PrepVfx(bool floorZero)
	{
		PrepVfx(floors, floorZero, events);
	}

	public static void PrepVfx(List<scrFloor> floors, bool floorZero, List<LevelEvent> events = null)
	{
		List<LevelEvent>[] array = new List<LevelEvent>[floors.Count];
		if (events != null)
		{
			int num = 0;
			for (num = 0; num < array.Length; num++)
			{
				array[num] = new List<LevelEvent>();
			}
			for (num = 0; num < events.Count; num++)
			{
				LevelEvent levelEvent = events[num];
				array[levelEvent.floor].Add(levelEvent);
			}
		}
		scrVfxPlus scrVfxPlus2 = scrVfxPlus.instance;
		scrVfxPlus2.Reset();
		scrConductor scrConductor2 = scrConductor.instance;
		foreach (scrFloor floor in floors)
		{
			ffxChangeTrack component = floor.GetComponent<ffxChangeTrack>();
			if (component != null)
			{
				component.PrepFloor();
			}
			floor.startPos = floor.transform.position;
			if (floor.opacityVal != 1f)
			{
				floor.SetOpacity(floor.opacityVal);
			}
			if (events != null)
			{
				List<LevelEvent> list = array[floor.seqID];
				Dictionary<string, Tuple<int, float>> dictionary = new Dictionary<string, Tuple<int, float>>();
				string[] array2 = new string[5];
				bool flag = false;
				foreach (LevelEvent item2 in list.FindAll((LevelEvent x) => x.eventType == LevelEventType.RepeatEvents))
				{
					int item = Convert.ToInt32(item2.data["repetitions"]);
					float @float = item2.GetFloat("interval");
					string[] array3 = (item2.GetString("tag") ?? "").Split(" ");
					foreach (string key in array3)
					{
						dictionary[key] = new Tuple<int, float>(item, @float);
					}
				}
				foreach (LevelEvent item3 in list.FindAll((LevelEvent x) => x.eventType == LevelEventType.SetConditionalEvents))
				{
					array2[0] = item3.GetString("perfectTag");
					array2[1] = item3.GetString("hitTag");
					array2[2] = item3.GetString("barelyTag");
					array2[3] = item3.GetString("missTag");
					array2[4] = item3.GetString("lossTag");
					flag = true;
				}
				foreach (LevelEvent item4 in list)
				{
					if (!item4.data.Keys.Contains("bgImage") || string.IsNullOrEmpty(Convert.ToString(item4.data["bgImage"])))
					{
						continue;
					}
					int num2 = 0;
					float num3 = 0f;
					string[] array3 = (item4.GetString("eventTag") ?? "").Split(" ");
					foreach (string key2 in array3)
					{
						if (dictionary.ContainsKey(key2))
						{
							num2 = dictionary[key2].Item1;
							num3 = dictionary[key2].Item2;
							break;
						}
					}
					for (int j = 0; j <= num2; j++)
					{
						ffxCustomBackgroundPlus ffxCustomBackgroundPlus2 = floor.gameObject.AddComponent<ffxCustomBackgroundPlus>();
						ffxCustomBackgroundPlus2.SetStartTime(scrConductor2.bpm, item4.GetFloat("angleOffset") + num3 * (float)j * 180f);
						ffxCustomBackgroundPlus2.color = Convert.ToString(item4.data["color"]).HexToColor();
						ffxCustomBackgroundPlus2.filePath = item4.data["bgImage"].ToString();
						ffxCustomBackgroundPlus2.imageColor = Convert.ToString(item4.data["imageColor"]).HexToColor();
						Vector2 vector = (Vector2)item4.data["parallax"];
						ffxCustomBackgroundPlus2.parallax = (item4.info.propertiesInfo["parallax"].isEnabled ? (new Vector2(vector.x, vector.y) / 100f) : Vector2.one);
						ffxCustomBackgroundPlus2.tiled = (BgDisplayMode)item4.data["bgDisplayMode"] == BgDisplayMode.Tiled;
						ffxCustomBackgroundPlus2.fitScreen = (BgDisplayMode)item4.data["bgDisplayMode"] != BgDisplayMode.Unscaled;
						ffxCustomBackgroundPlus2.looping = (ToggleBool)item4.data["loopBG"] == ToggleBool.Enabled;
						ffxCustomBackgroundPlus2.unscaledSize = (float)item4.GetInt("unscaledSize") / 100f;
						ffxCustomBackgroundPlus2.lockRot = (ToggleBool)item4.data["lockRot"] == ToggleBool.Enabled;
						floor.plusEffects.Add(ffxCustomBackgroundPlus2);
						if (flag)
						{
							bool[] array4 = new bool[5];
							for (int k = 0; k < array2.Length; k++)
							{
								string text = array2[k];
								array4[k] = text != "NONE" && item4.GetString("eventTag") == text;
							}
							ffxCustomBackgroundPlus2.conditionalInfo = array4;
							break;
						}
					}
				}
			}
			foreach (ffxPlusBase plusEffect in floor.plusEffects)
			{
				bool flag2 = false;
				if (plusEffect.conditionalInfo[0])
				{
					floor.perfectEffects.Add(plusEffect);
					flag2 = true;
				}
				if (plusEffect.conditionalInfo[1])
				{
					floor.hitEffects.Add(plusEffect);
					flag2 = true;
				}
				if (plusEffect.conditionalInfo[2])
				{
					floor.barelyEffects.Add(plusEffect);
					flag2 = true;
				}
				if (plusEffect.conditionalInfo[3])
				{
					floor.missEffects.Add(plusEffect);
					flag2 = true;
				}
				if (plusEffect.conditionalInfo[4])
				{
					floor.lossEffects.Add(plusEffect);
					flag2 = true;
				}
				if (!flag2)
				{
					scrVfxPlus2.effects.Add(plusEffect);
					continue;
				}
				plusEffect.triggered = true;
				floor.hasConditionalChange = true;
			}
		}
		if (floorZero)
		{
			scrVfxPlus2.ScrubToTime(0f);
		}
		scrVfxPlus2.effects = (from fx in scrVfxPlus2.effects
							   orderby fx.startTime, fx.floor.seqID
							   select fx).ToList();
	}

	public void UpdateVideo()
	{
		printesp("");
		if (!levelData.miscSettings.data["bgVideo"].ToString().IsNullOrEmpty())
		{
			videoBG.gameObject.SetActive(value: true);
			string text = Path.Combine(Path.GetDirectoryName(levelPath), levelData.miscSettings.data["bgVideo"].ToString());
			if (File.Exists(text))
			{
				videoBG.url = text;
				printe("preparing video: " + videoBG.url);
				videoBG.Stop();
				videoBG.Prepare();
				videoBG.prepareCompleted += PrepareCompleted;
				videoBG.errorReceived += VideoErrorReceived;
				videoBG.isLooping = (ToggleBool)levelData.miscSettings.data["loopVideo"] == ToggleBool.Enabled;
			}
			else
			{
				printesp("Video does not exist in path " + text);
			}
		}
		else
		{
			videoBG.gameObject.SetActive(value: false);
		}
	}

	public void PrepareCompleted(VideoPlayer source)
	{
		printe("video " + source.url + " is now prepared");
	}

	public void VideoErrorReceived(VideoPlayer source, string message)
	{
		printe("video " + source.url + " has error: " + message);
	}

	public void UpdateTrackColors()
	{
		printe("we are here");
	}

	public void SetBackground()
	{
		printesp("");
		scrCamera.instance.Bgcamstatic.backgroundColor = levelData.backgroundColor;
		if (!string.IsNullOrEmpty(levelData.bgImage))
		{
			string filePath = Path.Combine(Path.GetDirectoryName(levelPath), levelData.bgImage);
			custBG.SetBaseSprite(filePath, levelData.bgImage);
			custBG.SetCustomBG(custBG.baseSprite, levelData.bgImageColor, levelData.bgTiling, levelData.bgLooping, levelData.bgFitScreen, levelData.unscaledSize, levelData.bgLockRot);
			if (levelData.backgroundSettings.info.propertiesInfo["parallax"].isEnabled)
			{
				custBG.parallax.multiplier_x = levelData.bgParallax.x;
				custBG.parallax.multiplier_y = levelData.bgParallax.y;
			}
			else
			{
				custBG.parallax.multiplier_x = 1f;
				custBG.parallax.multiplier_y = 1f;
			}
			editorBG.SetActive(value: false);
		}
		else
		{
			custBG.baseSprite = null;
			custBG.SetCustomBG(null, Color.white);
			editorBG.SetActive(levelData.bgShowDefaultBGIfNoImage);
		}
	}

	public void SetStartingBG()
	{
		printesp("SetStartingBG");
		if (custBG.baseSprite != null)
		{
			editorBG.SetActive(value: false);
			custBG.SetCustomBG(custBG.baseSprite, levelData.bgImageColor, levelData.bgTiling, levelData.bgLooping, levelData.bgFitScreen, levelData.unscaledSize, levelData.bgLockRot);
			custBG.parallax.multiplier_x = levelData.bgParallax.x;
			custBG.parallax.multiplier_y = levelData.bgParallax.y;
		}
		else
		{
			custBG.SetCustomBG(null, Color.white);
			editorBG.SetActive(levelData.bgShowDefaultBGIfNoImage);
		}
	}

	public static void SetFxPlusFromComponents(List<scrFloor> listFloors, bool useComponentNotation)
	{
		scrConductor scrConductor2 = scrConductor.instance;
		TrackColorType trackColorType = TrackColorType.Single;
		TrackAnimationType animationType = TrackAnimationType.None;
		TrackAnimationType2 animationType2 = TrackAnimationType2.None;
		Color color;
		Color color2 = (color = listFloors[0].floorRenderer.color);
		int num = 0;
		foreach (scrFloor listFloor in listFloors)
		{
			float num2 = 60f / (scrConductor2.bpm * scrConductor2.song.pitch * listFloor.speed);
			ffxPlusBase[] components = listFloor.GetComponents<ffxPlusBase>();
			foreach (ffxPlusBase ffxPlusBase2 in components)
			{
				ffxPlusBase2.SetStartTime(scrConductor2.bpm, ffxPlusBase2.degreeOffset);
				listFloor.plusEffects.Add(ffxPlusBase2);
				if (useComponentNotation)
				{
					ffxPlusBase2.duration *= num2;
				}
			}
			ffxChangeTrack component = listFloor.GetComponent<ffxChangeTrack>();
			if (component != null)
			{
				color = ((component.color1 == Color.clear) ? listFloor.floorRenderer.color : component.color1);
				color2 = ((component.color2 == Color.clear) ? listFloor.floorRenderer.color : component.color2);
				trackColorType = component.colorType;
				animationType = component.animationType;
				animationType2 = component.animationType2;
			}
			else
			{
				ffxChangeTrack obj = listFloor.gameObject.AddComponent<ffxChangeTrack>();
				obj.color1 = color;
				obj.color2 = color2;
				obj.colorType = trackColorType;
				obj.animationType = animationType;
				obj.animationType2 = animationType2;
			}
			switch (trackColorType)
			{
				case TrackColorType.Single:
					listFloor.SetColor(color);
					break;
				case TrackColorType.Stripes:
					listFloor.SetColor(((listFloor.seqID - num) % 2 == 0) ? color : color2);
					break;
			}
		}
	}

	public void ReloadSong()
	{
		if (string.IsNullOrEmpty(levelData.songFilename))
		{
			AudioManager audioManager = AudioManager.Instance;
			if (!string.IsNullOrEmpty(currentSongKey))
			{
				audioManager.audioLib.Remove(currentSongKey);
			}
			scrConductor.instance.song.clip = null;
		}
		else
		{
			StartCoroutine(ReloadSongCo());
		}
	}

	private IEnumerator ReloadSongCo()
	{
		printesp("ReloadSongCo");
		string newSongKey = levelData.songFilename + "*external";
		if (currentSongKey != newSongKey)
		{
			string path = Path.Combine(Path.GetDirectoryName(levelPath), levelData.songFilename);
			AudioManager audioManager = AudioManager.Instance;
			if (!string.IsNullOrEmpty(currentSongKey))
			{
				audioManager.audioLib.Remove(currentSongKey);
			}
			if (GCS.standaloneLevelMode)
			{
				double entryTime = floors[floors.Count - 1].entryTime;
				yield return audioManager.FindOrLoadAudioClipExternal(path, mp3Streaming: true, (float)entryTime);
			}
			else
			{
				yield return audioManager.FindOrLoadAudioClipExternal(path, mp3Streaming: false);
			}
			_ = GCS.standaloneLevelMode;
			Dictionary<string, AudioClip> audioLib = audioManager.audioLib;
			if (audioLib.ContainsKey(newSongKey))
			{
				AudioClip clip = audioLib[newSongKey];
				scrConductor.instance.song.clip = clip;
				currentSongKey = newSongKey;
			}
			else if (ADOBase.editor != null)
			{
				ADOBase.editor.ShowNotification(RDString.Get("editor.notification.songNotFound", new Dictionary<string, object> { { "file", levelData.songFilename } }));
			}
		}
	}

	public float GetTileSize()
	{
		return 1.5f;
	}

	public string[] GetWorldPaths()
	{
		return GetWorldPaths(levelPath);
	}

	public static string[] GetWorldPaths(string levelPath, bool excludeMain = false, bool renamed = false)
	{
		List<string> list = new List<string>();
		string directoryName = Path.GetDirectoryName(levelPath);
		string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(levelPath);
		int num = 1;
		string item;
		while (RDFile.Exists(item = Path.Combine(directoryName, ((renamed || fileNameWithoutExtension == "main") ? "sub" : fileNameWithoutExtension) + num + ".adofai")))
		{
			list.Add(item);
			num++;
		}
		if (!excludeMain)
		{
			list.Add(levelPath);
		}
		return list.ToArray();
	}

	public void ResetScene()
	{
		isLoading = true;
		printesp("ResetScene");
		scrUIController.instance.txtCountdown.GetComponent<scrCountdown>().CancelGo();
		ADOBase.controller.perfectEffects.Clear();
		ADOBase.controller.hitEffects.Clear();
		ADOBase.controller.barelyEffects.Clear();
		ADOBase.controller.missEffects.Clear();
		ADOBase.controller.lossEffects.Clear();
		ReloadAssets();
		scrCamera scrCamera2 = scrCamera.instance;
		scrCamera2.transform.localPosition = scrCamera2.transform.position;
		scrCamera2.transform.rotation = Quaternion.identity;
		camParent.transform.position = Vector3.zero;
		scrCamera2.followMode = true;
		scrCamera2.zoomSize = 1f;
		scrCamera2.shake = Vector3.zero;
		scrCamera2.Bgcamstatic.enabled = true;
		scrCamera2.Bgcamstatic.backgroundColor = levelData.backgroundColor;
		scrCamera2.GetComponent<VideoBloom>().enabled = false;
		scrCamera2.GetComponent<ScreenTile>().enabled = false;
		scrCamera2.GetComponent<ScreenScroll>().enabled = false;
		DisableFilters();
		SetStartingBG();
		if (videoBG.isPlaying)
		{
			videoBG.Stop();
		}
		ResetPlanetsPosition();
		if (!GCS.standaloneLevelMode && !paused)
		{
			ADOBase.controller.TogglePauseGame();
		}
		if (GCS.standaloneLevelMode && paused)
		{
			ADOBase.controller.TogglePauseGame();
		}
		GameObject[] array = GameObject.FindGameObjectsWithTag("MissIndicator");
		for (int i = 0; i < array.Length; i++)
		{
			UnityEngine.Object.Destroy(array[i]);
		}
		ADOBase.controller.missesOnCurrFloor.Clear();
		foreach (scrLetterPress typingLetter in ADOBase.controller.typingLetters)
		{
			UnityEngine.Object.Destroy(typingLetter.gameObject);
		}
		ADOBase.controller.typingLetters.Clear();
		DOTween.KillAll();
		scrCamera.instance.flashPlusRendererBg.material.color = Color.clear;
		scrCamera.instance.flashPlusRendererFg.material.color = Color.clear;
		scrVfxPlus.instance.Reset();
		RemakePath();
		UpdateDecorationObjects();
	}

	public void ResetPlanetsPosition()
	{
		scrPlanet redPlanet = ADOBase.controller.redPlanet;
		scrPlanet bluePlanet = ADOBase.controller.bluePlanet;
		if (floors.Count > 0)
		{
			redPlanet.transform.position = floors[0].transform.position;
		}
		if (floors.Count > 1)
		{
			bluePlanet.transform.position = floors[1].transform.position;
		}
		redPlanet.Destroy();
		bluePlanet.Destroy();
		for (int i = 0; i < ADOBase.controller.planetList.Count; i++)
		{
			if (i > 1)
			{
				ADOBase.controller.planetList[i].transform.localPosition = Vector3.right * 1.5f * i;
				ADOBase.controller.planetList[i].Destroy();
			}
		}
		ADOBase.controller.ResetNumPlanets();
		redPlanet.samuraiSprite.gameObject.SetActive(value: false);
		bluePlanet.samuraiSprite.gameObject.SetActive(value: false);
		redPlanet.faceHolder.gameObject.SetActive(value: false);
		bluePlanet.faceHolder.gameObject.SetActive(value: false);
		if (ADOBase.controller.hitTextContainer != null)
		{
			ADOBase.controller.hitTextContainer.SetActive(value: false);
		}
	}

	private static void printesp(string s)
	{
		string.IsNullOrEmpty(s);
	}

	private void Unload()
	{
		imgHolder.Unload(onlyIfUnused: false);
	}
}
