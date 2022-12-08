using ADOFAI;
using DG.Tweening;
using RDTools;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;

public class scrLevelMaker : ADOBase
{
	private const double epsilon = 1E-06;

	public const string FloorContainerName = "Floors";

	public const char Angle0 = 'R';

	public const char Angle45 = 'E';

	public const char Angle60 = 'T';

	public const char Angle90 = 'U';

	public const char Angle135 = 'Q';

	public const char Angle150 = 'G';

	public const char Angle180 = 'L';

	public const char Angle240 = 'F';

	public const char Angle270 = 'D';

	public const char Angle225 = 'Z';

	public const char Angle330 = 'B';

	public const char Angle315 = 'C';

	public const char Angle30 = 'J';

	public const char Angle120 = 'H';

	public const char Angle210 = 'N';

	public const char Angle300 = 'M';

	public const char Angle15 = 'p';

	public const char Angle75 = 'o';

	public const char Angle105 = 'q';

	public const char Angle165 = 'W';

	public const char Angle195 = 'x';

	public const char Angle255 = 'V';

	public const char Angle285 = 'Y';

	public const char Angle345 = 'A';

	public const char AngleMidspin = '!';

	public const char Angle60Add = 't';

	public const char Angle300Add = 'y';

	public const char Angle120CW = 'h';

	public const char Angle120CCW = 'j';

	public const char Angle108CW = '5';

	public const char Angle108CCW = '6';

	public const char Angle128CW = '7';

	public const char Angle128CCW = '8';

	public const char Angle210CW = '9';

	public const float midSpinAngle = 999f;

	public const float sAngle = -999f;

	public GameObject spriteFloor;

	public GameObject meshFloor;

	public string caption;

	public float addoffset;

	public bool lockChanges;

	public bool isgameworld = true;

	public bool isOldLevel;

	public bool useInitialTrackStyle;

	public bool hideDifficultyUI;

	[HideInInspector]
	public string leveldata;

	[HideInInspector]
	public float[] floorAngles;

	[HideInInspector]
	public List<scrFloor> listFloors = new List<scrFloor>();

	[HideInInspector]
	public List<FreeroamArea> listFreeroam = new List<FreeroamArea>();

	[HideInInspector]
	public List<scrFloor> listFreeroamStartTiles = new List<scrFloor>();

	[HideInInspector]
	public float bpm_forUnityUseOnly;

	[HideInInspector]
	public float pitch_forUnityUseOnly;

	[HideInInspector]
	public float volume_forUnityUseOnly;

	[HideInInspector]
	public float highestBPM;

	[NonSerialized]
	public GameObject holdContainer;

	[NonSerialized]
	public scrLevelMaker2 lm2;

	private List<GameObject> holdGOs = new List<GameObject>();

	private Material holdMaterial;

	private GameObject floorContainer;

	private static scrLevelMaker _instance;

	[NonSerialized]
	public float baseFloorLength;

	[NonSerialized]
	public float baseFloorWidth;

	public static scrLevelMaker instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = UnityEngine.Object.FindObjectOfType<scrLevelMaker>();
			}
			return _instance;
		}
	}

	private GameObject GetFloorContainer()
	{
		GameObject gameObject = GameObject.Find("Floors");
		if (gameObject == null)
		{
			gameObject = new GameObject("Floors");
		}
		return gameObject;
	}

	private void Awake()
	{
		lm2 = GetComponent<scrLevelMaker2>();
		if (meshFloor != null)
		{
			FloorMesh component = meshFloor.GetComponent<FloorMesh>();
			baseFloorLength = component._length;
			baseFloorWidth = component._width;
		}
	}

	public void FixListFloors()
	{
		scrFloor[] source = UnityEngine.Object.FindObjectsOfType<scrFloor>();
		listFloors = (from o in source
			orderby o.seqID
			select o).ToList();
		UnityEngine.Debug.Log(listFloors.Count);
		lm2 = GetComponent<scrLevelMaker2>();
		lm2.listFloorClone = listFloors;
	}

	public void CopyProperties()
	{
		lm2 = GetComponent<scrLevelMaker2>();
		scrConductor instance = scrConductor.instance;
		instance.addoffset = addoffset;
		instance.bpm = bpm_forUnityUseOnly;
		instance.song.pitch = pitch_forUnityUseOnly;
		instance.song.volume = volume_forUnityUseOnly;
		scrController instance2 = scrController.instance;
		instance2.isbigtiles = lm2.BigTiles;
		instance2.caption = caption;
	}

	public static double OldToNewAngle(double oldAngle)
	{
		return (7.8539818525314331 - oldAngle) % 6.2831854820251465;
	}

	public List<scrFloor> MakeLevel()
	{
		if (isOldLevel)
		{
			InstantiateStringFloors();
		}
		else
		{
			InstantiateFloatFloors();
		}
		lm2 = GetComponent<scrLevelMaker2>();
		for (int i = 0; i < listFloors.Count; i++)
		{
			scrFloor scrFloor = listFloors[i];
			scrFloor.styleNum = 0;
			if (isgameworld)
			{
				scrFloor.UpdateAngle();
			}
			scrFloor.SetTileColor(lm2.tilecolor);
			int num = 100 + (listFloors.Count - i);
			num *= 5;
			scrFloor.SetSortingOrder(num);
			scrFloor.startPos = scrFloor.transform.position;
			scrFloor.startRot = scrFloor.transform.rotation.eulerAngles;
			scrFloor.tweenRot = scrFloor.startRot;
			scrFloor.offsetPos = Vector3.zero;
			if (scrFloor.isportal && Application.isPlaying)
			{
				scrFloor.SpawnPortalParticles();
			}
		}
		if (LevelData.shouldTryMigrate && (bool)ADOBase.editor && ADOBase.editor.levelData.version == 9)
		{
			LevelData.shouldTryMigrate = false;
			bool flag = true;
			foreach (LevelEvent levelEvent in ADOBase.editor.levelData.levelEvents)
			{
				if (levelEvent.eventType == LevelEventType.Twirl)
				{
					flag = !flag;
				}
				else if (levelEvent.eventType == LevelEventType.Pause && levelEvent.floor < listFloors.Count)
				{
					scrFloor scrFloor2 = listFloors[levelEvent.floor];
					bool num2 = Math.Abs(scrFloor2.entryangle % (Math.PI / 2.0) - scrFloor2.exitangle % (Math.PI / 2.0)) <= 0.0001;
					bool flag2 = Math.Abs(scrMisc.GetAngleMoved(scrFloor2.entryangle, scrFloor2.exitangle, flag) - 6.2831854820251465) < 0.0001;
					if (num2 && !flag2)
					{
						RDBaseDll.printem($"we should modify duration: entry is {scrFloor2.entryangle} and exit is {scrFloor2.exitangle}");
						levelEvent["duration"] = (float)levelEvent["duration"] + 1f;
					}
				}
			}
		}
		return listFloors;
	}

	public void InstantiateStringFloors()
	{
		bool flag = Application.isPlaying;
		GameObject gameObject = GetFloorContainer();
		string text = leveldata;
		int num = listFloors.Count;
		int num2 = text.Length + 1;
		Material floorSpriteDefault = RDConstants.data.floorSpriteDefault;
		if (num > 0 && listFloors[0].GetComponent<FloorMeshRenderer>() != null)
		{
			flag = false;
		}
		if (flag)
		{
			if (num > num2)
			{
				for (int i = num2; i < num; i++)
				{
					scrFloor scrFloor = listFloors[i];
					if (scrFloor != null)
					{
						UnityEngine.Object.DestroyImmediate(scrFloor.gameObject);
					}
				}
				listFloors.RemoveRange(num2, num - num2);
			}
		}
		else
		{
			foreach (scrFloor listFloor in listFloors)
			{
				if (listFloor != null)
				{
					UnityEngine.Object.DestroyImmediate(listFloor.gameObject);
				}
			}
			num = 0;
			listFloors.Clear();
		}
		if (listFloors.Count > 0)
		{
			ADOBase.conductor.onBeats.Clear();
		}
		if (listFloors.Count == 0)
		{
			scrFloor component = UnityEngine.Object.Instantiate(spriteFloor, Vector3.zero, Quaternion.identity).GetComponent<scrFloor>();
			component.gameObject.transform.parent = gameObject.transform;
			component.hasLit = true;
			component.entryangle = 4.71238899230957;
			component.name = "0/FloorR";
			listFloors.Add(component);
		}
		else
		{
			scrFloor scrFloor2 = listFloors[0];
			ResetFloor(scrFloor2, Vector3.zero, floorSpriteDefault);
			scrFloor2.hasLit = true;
			scrFloor2.entryangle = 4.71238899230957;
			scrFloor2.name = "0/FloorR";
		}
		int num3 = 1;
		float num4 = Mathf.Sin(MathF.PI / 4f);
		float num5 = Mathf.Sin(MathF.PI / 6f);
		Transform transform = gameObject.transform;
		Vector3 vector = Vector3.zero;
		bool flag2 = true;
		float num6 = 1f;
		for (int j = 0; j < text.Length; j++)
		{
			double radius = scrController.instance.startRadius;
			double num7 = 0.0;
			bool isEditor = Application.isEditor;
			scrFloor scrFloor3 = listFloors[j];
			switch (text[j])
			{
			case '[':
			{
				if (!isEditor)
				{
					break;
				}
				bool flag3 = false;
				int num8 = j + 1;
				bool flag4 = false;
				if (j + 1 <= text.Length && text[j + 1] == '*')
				{
					flag4 = true;
					num8++;
				}
				while (j + 1 <= text.Length && !flag3)
				{
					j++;
					if (text[j] == ']')
					{
						flag3 = true;
					}
				}
				string s = text.Substring(num8, j - num8).Replace(" ", "");
				float result = 0f;
				if (float.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out result))
				{
					float num9 = MathF.PI / 180f * result;
					num7 = (flag4 ? scrMisc.incrementAngle(scrFloor3.entryangle, num9) : ((double)num9));
				}
				break;
			}
			case 'R':
				num7 = 1.5707963705062866;
				break;
			case 'U':
				num7 = 0.0;
				break;
			case 'L':
				num7 = 4.71238899230957;
				break;
			case 'D':
				num7 = 3.1415927410125732;
				break;
			case 'Q':
				num7 = 5.4977874755859375;
				break;
			case 'E':
				num7 = 0.78539818525314331;
				break;
			case 'Z':
				num7 = 3.9269909858703613;
				break;
			case 'C':
				num7 = 2.3561944961547852;
				break;
			case 't':
				num7 = scrMisc.incrementAngle(scrFloor3.entryangle, MathF.PI / 3f * (float)(flag2 ? 1 : (-1)));
				break;
			case 'h':
				num7 = scrMisc.incrementAngle(scrFloor3.entryangle, 2.0943951606750488);
				break;
			case 'y':
				num7 = scrMisc.incrementAngle(scrFloor3.entryangle, MathF.PI / 3f * (float)((!flag2) ? 1 : (-1)));
				break;
			case 'j':
				num7 = scrMisc.incrementAngle(scrFloor3.entryangle, -2.0943951606750488);
				break;
			case '5':
				num7 = scrMisc.incrementAngle(scrFloor3.entryangle, 1.8849555253982544);
				break;
			case '6':
				num7 = scrMisc.incrementAngle(scrFloor3.entryangle, -1.8849555253982544);
				break;
			case '7':
				num7 = scrMisc.incrementAngle(scrFloor3.entryangle, 2.2439949512481689);
				break;
			case '8':
				num7 = scrMisc.incrementAngle(scrFloor3.entryangle, -2.2439949512481689);
				break;
			case '9':
				num7 = scrMisc.incrementAngle(scrFloor3.entryangle, 3.6651914119720459);
				break;
			case 'T':
				num7 = 0.52359879016876221;
				break;
			case 'B':
				num7 = 2.6179938316345215;
				break;
			case 'F':
				num7 = 3.6651914119720459;
				break;
			case 'G':
				num7 = 5.7595863342285156;
				break;
			case 'J':
				num7 = 1.0471975803375244;
				break;
			case 'M':
				num7 = 2.0943951606750488;
				break;
			case 'N':
				num7 = 4.1887903213500977;
				break;
			case 'H':
				num7 = 5.235987663269043;
				break;
			case 'p':
				num7 = 1.3089969158172607;
				break;
			case 'o':
				num7 = 0.2617993950843811;
				break;
			case 'q':
				num7 = 6.021385669708252;
				break;
			case 'W':
				num7 = 4.9741883277893066;
				break;
			case 'x':
				num7 = 4.450589656829834;
				break;
			case 'V':
				num7 = 3.4033920764923096;
				break;
			case 'Y':
				num7 = 2.8797931671142578;
				break;
			case 'A':
				num7 = 1.8325957059860229;
				break;
			case '!':
				num7 = (float)scrFloor3.entryangle;
				break;
			}
			Vector3 vectorFromAngle = scrMisc.getVectorFromAngle(num7, radius);
			vector += vectorFromAngle;
			if (listFloors.Count > 0)
			{
				listFloors[j].exitangle = num7;
			}
			scrFloor scrFloor4;
			if (j < num - 1)
			{
				scrFloor4 = listFloors[j + 1];
				GameObject gameObject2 = scrFloor4.gameObject;
				ResetFloor(scrFloor4, vector, floorSpriteDefault);
			}
			else
			{
				scrFloor4 = UnityEngine.Object.Instantiate(spriteFloor, vector, Quaternion.identity, transform).GetComponent<scrFloor>();
				listFloors.Add(scrFloor4);
			}
			scrFloor3.nextfloor = scrFloor4;
			scrFloor4.stringDirection = text[j];
			scrFloor4.seqID = num3;
			scrFloor4.entryangle = (num7 + 3.1415927410125732) % 6.2831854820251465;
			char c = text[j];
			if (c == '!')
			{
				listFloors[num3 - 1].midSpin = true;
			}
			bool flag5 = true;
			bool bigTiles = lm2.BigTiles;
			while (flag5 && j < text.Length - 1)
			{
				flag5 = false;
				if ("UDLRQEZCthTFGByjHJMN!56789[qWVYAxop".Contains(text[j + 1]))
				{
					break;
				}
				j++;
				flag5 = true;
				switch (c)
				{
				case 'S':
					scrFloor4.speed = 0.25f;
					break;
				case 'X':
					scrFloor4.speed = 0.5f;
					break;
				case 'O':
					scrFloor4.speed = 2f;
					break;
				case 'P':
					scrFloor4.speed = 4f;
					break;
				case '>':
					scrFloor4.floorIcon = FloorIcon.Rabbit;
					num6 *= 2f;
					if (bigTiles)
					{
						scrFloor4.SetIconAngle(MathF.PI);
					}
					break;
				case '*':
					scrFloor4.floorIcon = FloorIcon.DoubleRabbit;
					num6 *= 4f;
					if (bigTiles)
					{
						scrFloor4.SetIconAngle(MathF.PI);
					}
					break;
				case '_':
					scrFloor4.floorIcon = FloorIcon.Rabbit;
					num6 /= 0.75f;
					if (bigTiles)
					{
						scrFloor4.SetIconAngle(MathF.PI);
					}
					break;
				case '<':
					scrFloor4.floorIcon = FloorIcon.Snail;
					num6 /= 2f;
					if (bigTiles)
					{
						scrFloor4.SetIconAngle(MathF.PI);
					}
					break;
				case '%':
					scrFloor4.floorIcon = FloorIcon.DoubleSnail;
					num6 /= 4f;
					if (bigTiles)
					{
						scrFloor4.SetIconAngle(MathF.PI);
					}
					break;
				case '-':
					scrFloor4.floorIcon = FloorIcon.Snail;
					num6 *= 0.75f;
					if (bigTiles)
					{
						scrFloor4.SetIconAngle(MathF.PI);
					}
					break;
				case '/':
					flag2 = !flag2;
					break;
				}
			}
			scrFloor4.isCCW = !flag2;
			scrFloor4.speed = num6;
			scrFloor4.isportal = (j == text.Length - 1 && scrController.isGameWorld);
			scrFloor4.levelnumber = -1;
			if (j < num)
			{
				scrFloor4.CheckPortalSprite();
				scrFloor4.UpdateIconSprite();
			}
			num3++;
		}
		listFloors.Last().exitangle = listFloors.Last().entryangle + 3.1415927410125732;
	}

	private void ResetFloor(scrFloor floor, Vector3 position, Material material)
	{
		GameObject gameObject2 = floor.gameObject;
		GameObject gameObject = floor.gameObject;
		gameObject.transform.position = position;
		gameObject.transform.rotation = Quaternion.identity;
		gameObject.transform.localScale = Vector3.one;
		ffxPlusBase[] components = gameObject.GetComponents<ffxPlusBase>();
		ffxBase[] components2 = gameObject.GetComponents<ffxBase>();
		ffxPlusBase[] array = components;
		for (int i = 0; i < array.Length; i++)
		{
			UnityEngine.Object.DestroyImmediate(array[i]);
		}
		ffxBase[] array2 = components2;
		for (int i = 0; i < array2.Length; i++)
		{
			UnityEngine.Object.DestroyImmediate(array2[i]);
		}
		if (Application.isPlaying)
		{
			floor.floorRenderer.material.CopyPropertiesFromMaterial(material);
		}
		floor.Reset();
	}

	public void InstantiateFloatFloors()
	{
		RDBaseDll.printem("");
		bool flag = Application.isPlaying;
		GameObject gameObject = GameObject.Find("Floors");
		if (gameObject == null)
		{
			gameObject = new GameObject("Floors");
		}
		int num = listFloors.Count;
		int num2 = floorAngles.Length + 1;
		Material floorMeshDefault = RDConstants.data.floorMeshDefault;
		if (!Application.isPlaying || (num > 0 && listFloors[0].GetComponent<FloorSpriteRenderer>() != null))
		{
			flag = false;
		}
		if (flag)
		{
			if (num > num2)
			{
				for (int i = num2; i < num; i++)
				{
					scrFloor scrFloor = listFloors[i];
					if (scrFloor != null)
					{
						UnityEngine.Object.DestroyImmediate(scrFloor.gameObject);
					}
				}
				listFloors.RemoveRange(num2, num - num2);
			}
		}
		else
		{
			foreach (scrFloor listFloor in listFloors)
			{
				if (listFloor != null && listFloor.gameObject != null)
				{
					UnityEngine.Object.DestroyImmediate(listFloor.gameObject);
				}
			}
			num = 0;
			listFloors.Clear();
		}
		if (listFloors.Count > 0)
		{
			ADOBase.conductor.onBeats.Clear();
		}
		if (listFloors.Count == 0)
		{
			GameObject gameObject2 = null;
			if (Application.isPlaying)
			{
				gameObject2 = UnityEngine.Object.Instantiate(meshFloor, Vector3.zero, Quaternion.identity);
			}
			scrFloor component = gameObject2.GetComponent<scrFloor>();
			component.gameObject.transform.parent = gameObject.transform;
			component.hasLit = true;
			component.entryangle = 4.71238899230957;
			component.name = "0/Floor 0";
			listFloors.Add(component);
		}
		else
		{
			scrFloor scrFloor2 = listFloors[0];
			ResetFloor(scrFloor2, Vector3.zero, floorMeshDefault);
			scrFloor2.hasLit = true;
			scrFloor2.entryangle = 4.71238899230957;
			scrFloor2.name = "0/Floor 0";
		}
		scrFloor scrFloor3 = listFloors[0];
		bool flag2 = true;
		Vector3 vector = Vector3.zero;
		for (int j = 0; j < floorAngles.Length; j++)
		{
			double radius = scrController.instance.startRadius;
			double num3 = 0.0;
			bool isEditor = Application.isEditor;
			float num4 = floorAngles[j];
			num3 = (scrFloor3.exitangle = ((num4 == 999f) ? ((double)(float)scrFloor3.entryangle) : ((double)((0f - num4 + 90f) * (MathF.PI / 180f)))));
			Vector3 vectorFromAngle = scrMisc.getVectorFromAngle(num3, radius);
			vector += vectorFromAngle;
			scrFloor scrFloor4 = null;
			GameObject gameObject3 = null;
			if (j < num - 1)
			{
				scrFloor4 = listFloors[j + 1];
				gameObject3 = scrFloor4.gameObject;
				ResetFloor(scrFloor4, vector, floorMeshDefault);
			}
			else
			{
				if (Application.isPlaying)
				{
					gameObject3 = UnityEngine.Object.Instantiate(meshFloor, vector, Quaternion.identity);
				}
				gameObject3.gameObject.transform.parent = gameObject.transform;
				scrFloor4 = gameObject3.GetComponent<scrFloor>();
				listFloors.Add(scrFloor4);
			}
			scrFloor3.nextfloor = scrFloor4;
			scrFloor4.floatDirection = num4;
			scrFloor4.seqID = j + 1;
			scrFloor4.entryangle = (num3 + 3.1415927410125732) % 6.2831854820251465;
			scrFloor4.isCCW = !flag2;
			scrFloor4.speed = 1f;
			if (num4 == 999f)
			{
				scrFloor3.midSpin = true;
			}
			if (j == floorAngles.Length - 1 && scrController.isGameWorld)
			{
				scrFloor4.isportal = true;
				scrFloor4.levelnumber = -1;
			}
			scrFloor3 = scrFloor4;
		}
		scrFloor3.exitangle = scrFloor3.entryangle + 3.1415927410125732;
	}

	public void CalculateFloorAngleLengths()
	{
		scrFloor scrFloor = listFloors[0];
		scrFloor.entryangle = 4.71238898038469;
		float num = ADOBase.conductor.adjustedCountdownTicks - 1f;
		scrFloor.angleLength = (double)num * Math.PI + scrMisc.GetAngleMoved(scrFloor.entryangle, scrFloor.exitangle, !scrFloor.isCCW);
		scrFloor.entryBeat = -1.0;
		double num2 = 0.0;
		for (int i = 1; i < listFloors.Count - 1; i++)
		{
			scrFloor scrFloor2 = listFloors[i];
			scrFloor2.entryBeat = num2;
			double num3 = CalculateSingleFloorAngleLength(scrFloor2);
			num2 += num3 / Math.PI;
		}
	}

	public double CalculateSingleFloorAngleLength(scrFloor cf)
	{
		cf.prevfloor = listFloors[cf.seqID - 1];
		double num = scrMisc.GetInverseAnglePerBeatMultiplanet(cf.numPlanets) * (double)((!cf.isCCW) ? 1 : (-1));
		if (cf.midSpin)
		{
			num = 0.0;
		}
		if (cf.prevfloor.midSpin && cf.numPlanets > 2)
		{
			num -= (6.2831854820251465 + scrMisc.GetInverseAnglePerBeatMultiplanet(cf.numPlanets)) * (double)((!cf.isCCW) ? 1 : (-1));
		}
		double num2 = scrMisc.GetAngleMoved(cf.entryangle + num, cf.exitangle + (cf.midSpin ? num : 0.0), !cf.isCCW);
		double num3 = Math.Abs(num2);
		if (num3 <= 1E-06 || num3 >= 6.2831844820251463)
		{
			num2 = (cf.midSpin ? 0f : (MathF.PI * 2f));
		}
		if (cf.holdLength > 0)
		{
			num2 += (double)((float)(cf.holdLength * 2) * MathF.PI);
		}
		cf.angleLength = num2;
		return num2;
	}

	public void CalculateFloorEntryTimes()
	{
		scrConductor conductor = ADOBase.conductor;
		float pitch = conductor.song.pitch;
		highestBPM = 0f;
		double num = 0.0;
		if (listFloors.Count == 1)
		{
			printe("listFloors is 1");
			return;
		}
		scrFloor scrFloor = listFloors[0];
		float num2 = conductor.adjustedCountdownTicks - 1f;
		num += conductor.crotchet * (double)num2 + scrMisc.GetTimeBetweenAngles(scrFloor.entryangle, scrFloor.exitangle, scrFloor.speed, conductor.bpm, !scrFloor.isCCW);
		listFloors[0].entryTime = 0.0;
		listFloors[1].entryTime = num;
		listFloors[1].entryTimePitchAdj = num / (double)pitch;
		for (int i = 1; i < listFloors.Count - 1; i++)
		{
			scrFloor scrFloor2 = listFloors[i];
			scrFloor2.prevfloor = listFloors[i - 1];
			scrFloor scrFloor3 = listFloors[i + 1];
			double num3 = scrMisc.GetInverseAnglePerBeatMultiplanet(scrFloor2.numPlanets) * (double)((!scrFloor2.isCCW) ? 1 : (-1));
			if (scrFloor2.midSpin)
			{
				num3 = 0.0;
			}
			if (scrFloor2.prevfloor.midSpin && scrFloor2.numPlanets > 2)
			{
				num3 -= (6.2831854820251465 + scrMisc.GetInverseAnglePerBeatMultiplanet(scrFloor2.numPlanets)) * (double)((!scrFloor2.isCCW) ? 1 : (-1));
			}
			double num4 = scrMisc.GetTimeBetweenAngles(scrFloor2.entryangle + num3, scrFloor2.exitangle + (scrFloor2.midSpin ? num3 : 0.0), scrFloor2.speed, conductor.bpm, !scrFloor2.isCCW);
			bool flag = num4 <= 1E-06 || num4 >= (double)(2f * (float)conductor.crotchetAtStart / scrFloor2.speed) - 1E-06;
			if (flag)
			{
				num4 = (scrFloor2.midSpin ? 0.0 : (2.0 * scrMisc.GetTimeBetweenAngles(0.0, 3.1415927410125732, scrFloor2.speed, conductor.bpm, isCW: false)));
			}
			num += num4;
			if (scrFloor2.holdLength > 0)
			{
				num += (double)(scrFloor2.holdLength * 2) * scrMisc.GetTimeBetweenAngles(0.0, 3.1415927410125732, scrFloor2.speed, conductor.bpm, isCW: false);
			}
			float num5 = scrFloor2.extraBeats;
			if (num5 > 0f && flag)
			{
				num5 -= 1f;
			}
			num = (scrFloor3.entryTime = num + (double)num5 * scrMisc.GetTimeBetweenAngles(0.0, 3.1415927410125732, scrFloor2.speed, conductor.bpm, isCW: false));
			scrFloor3.entryTimePitchAdj = num / (double)pitch;
			float speed = scrFloor2.speed;
			float bpm = conductor.bpm;
			float num6 = scrFloor2.speed * conductor.bpm;
			if (num6 > highestBPM)
			{
				highestBPM = num6;
			}
		}
	}

	public static float GetAngleFromFloorCharDirectionWithCheck(char direction, out bool exists)
	{
		float result = 0f;
		exists = true;
		switch (direction)
		{
		case 'R':
			result = 0f;
			break;
		case 'E':
			result = 45f;
			break;
		case 'U':
			result = 90f;
			break;
		case 'Q':
			result = 135f;
			break;
		case 'L':
			result = 180f;
			break;
		case 'Z':
			result = 225f;
			break;
		case 'D':
			result = 270f;
			break;
		case 'C':
			result = 315f;
			break;
		case 'B':
			result = 300f;
			break;
		case 'T':
			result = 60f;
			break;
		case 'G':
			result = 120f;
			break;
		case 'F':
			result = 240f;
			break;
		case 'J':
			result = 30f;
			break;
		case 'H':
			result = 150f;
			break;
		case 'N':
			result = 210f;
			break;
		case 'M':
			result = 330f;
			break;
		case 'p':
			result = 15f;
			break;
		case 'o':
			result = 75f;
			break;
		case 'q':
			result = 105f;
			break;
		case 'W':
			result = 165f;
			break;
		case 'x':
			result = 195f;
			break;
		case 'V':
			result = 255f;
			break;
		case 'Y':
			result = 285f;
			break;
		case 'A':
			result = 345f;
			break;
		case '!':
			result = 999f;
			break;
		default:
			exists = false;
			break;
		}
		return result;
	}

	public static float GetAngleFromFloorCharDirection(char direction)
	{
		bool exists = false;
		return GetAngleFromFloorCharDirectionWithCheck(direction, out exists);
	}

	public char GetHFlippedDirection(char direction)
	{
		char result = direction;
		switch (direction)
		{
		case 'R':
			result = 'L';
			break;
		case 'E':
			result = 'Q';
			break;
		case 'U':
			result = 'U';
			break;
		case 'Q':
			result = 'E';
			break;
		case 'L':
			result = 'R';
			break;
		case 'Z':
			result = 'C';
			break;
		case 'D':
			result = 'D';
			break;
		case 'C':
			result = 'Z';
			break;
		case 'B':
			result = 'F';
			break;
		case 'T':
			result = 'G';
			break;
		case 'G':
			result = 'T';
			break;
		case 'F':
			result = 'B';
			break;
		case 'J':
			result = 'H';
			break;
		case 'H':
			result = 'J';
			break;
		case 'N':
			result = 'M';
			break;
		case 'M':
			result = 'N';
			break;
		case 'p':
			result = 'W';
			break;
		case 'o':
			result = 'q';
			break;
		case 'q':
			result = 'o';
			break;
		case 'W':
			result = 'p';
			break;
		case 'x':
			result = 'A';
			break;
		case 'V':
			result = 'Y';
			break;
		case 'Y':
			result = 'V';
			break;
		case 'A':
			result = 'x';
			break;
		case '6':
			result = '5';
			break;
		case '5':
			result = '6';
			break;
		case '8':
			result = '7';
			break;
		case '7':
			result = '8';
			break;
		}
		return result;
	}

	public float GetHFlippedDirection(float direction)
	{
		if (direction == 999f)
		{
			return direction;
		}
		return (0f - direction + 180f) % 360f;
	}

	public char GetVFlippedDirection(char direction)
	{
		char result = direction;
		switch (direction)
		{
		case 'R':
			result = 'R';
			break;
		case 'E':
			result = 'C';
			break;
		case 'U':
			result = 'D';
			break;
		case 'Q':
			result = 'Z';
			break;
		case 'L':
			result = 'L';
			break;
		case 'Z':
			result = 'Q';
			break;
		case 'D':
			result = 'U';
			break;
		case 'C':
			result = 'E';
			break;
		case 'B':
			result = 'T';
			break;
		case 'T':
			result = 'B';
			break;
		case 'G':
			result = 'F';
			break;
		case 'F':
			result = 'G';
			break;
		case 'J':
			result = 'M';
			break;
		case 'H':
			result = 'N';
			break;
		case 'N':
			result = 'H';
			break;
		case 'M':
			result = 'J';
			break;
		case 'p':
			result = 'A';
			break;
		case 'o':
			result = 'Y';
			break;
		case 'q':
			result = 'V';
			break;
		case 'W':
			result = 'x';
			break;
		case 'x':
			result = 'W';
			break;
		case 'V':
			result = 'q';
			break;
		case 'Y':
			result = 'o';
			break;
		case 'A':
			result = 'p';
			break;
		case '6':
			result = '5';
			break;
		case '5':
			result = '6';
			break;
		case '8':
			result = '7';
			break;
		case '7':
			result = '8';
			break;
		}
		return result;
	}

	public float GetVFlippedDirection(float direction)
	{
		if (direction == 999f)
		{
			return direction;
		}
		return (0f - direction) % 360f;
	}

	public char GetRotDirection(char direction, bool CW)
	{
		char result = direction;
		switch (direction)
		{
		case 'R':
			result = (CW ? 'D' : 'U');
			break;
		case 'E':
			result = (CW ? 'C' : 'Q');
			break;
		case 'U':
			result = (CW ? 'R' : 'L');
			break;
		case 'Q':
			result = (CW ? 'E' : 'Z');
			break;
		case 'L':
			result = (CW ? 'U' : 'D');
			break;
		case 'Z':
			result = (CW ? 'Q' : 'C');
			break;
		case 'D':
			result = (CW ? 'L' : 'R');
			break;
		case 'C':
			result = (CW ? 'Z' : 'E');
			break;
		case 'B':
			result = (CW ? 'N' : 'J');
			break;
		case 'T':
			result = (CW ? 'M' : 'H');
			break;
		case 'G':
			result = (CW ? 'J' : 'N');
			break;
		case 'F':
			result = (CW ? 'H' : 'M');
			break;
		case 'J':
			result = (CW ? 'B' : 'G');
			break;
		case 'H':
			result = (CW ? 'T' : 'F');
			break;
		case 'N':
			result = (CW ? 'G' : 'B');
			break;
		case 'M':
			result = (CW ? 'F' : 'T');
			break;
		case 'p':
			result = (CW ? 'Y' : 'q');
			break;
		case 'o':
			result = (CW ? 'A' : 'W');
			break;
		case 'q':
			result = (CW ? 'p' : 'x');
			break;
		case 'W':
			result = (CW ? 'o' : 'V');
			break;
		case 'x':
			result = (CW ? 'q' : 'Y');
			break;
		case 'V':
			result = (CW ? 'W' : 'A');
			break;
		case 'Y':
			result = (CW ? 'x' : 'p');
			break;
		case 'A':
			result = (CW ? 'V' : 'o');
			break;
		}
		return result;
	}

	public float GetRotDirection(float direction, bool CW)
	{
		if (direction == 999f)
		{
			return direction;
		}
		return direction + (float)((!CW) ? 1 : (-1)) * 90f;
	}

	public static float[] StringToAngleArray(string levelStr)
	{
		float[] array = new float[levelStr.Length];
		float num = 0f;
		for (int i = 0; i < levelStr.Length; i++)
		{
			char c = levelStr[i];
			bool exists;
			float num2 = GetAngleFromFloorCharDirectionWithCheck(c, out exists);
			if (!exists)
			{
				switch (c)
				{
				case '5':
					num2 = num + 72f;
					break;
				case '6':
					num2 = num - 72f;
					break;
				case '7':
					num2 = num + 52f;
					break;
				case '8':
					num2 = num - 52f;
					break;
				case '9':
					num2 = num - 30f;
					break;
				case 'h':
					num2 = num + 120f;
					break;
				case 'j':
					num2 = num - 120f;
					break;
				case 't':
					num2 = num + 60f;
					break;
				case 'y':
					num2 = num + 300f;
					break;
				}
			}
			array[i] = num2;
			num = num2;
		}
		return array;
	}

	public void ClearFreeroam()
	{
		if (listFreeroam.Count > 0)
		{
			foreach (FreeroamArea item in listFreeroam)
			{
				if (item.Count > 0)
				{
					foreach (scrFloor item2 in item)
					{
						if (item2 != null)
						{
							UnityEngine.Object.DestroyImmediate(item2.gameObject);
						}
					}
				}
				item.Clear();
			}
		}
		listFreeroam.Clear();
		listFreeroamStartTiles = new List<scrFloor>();
	}

	public void DrawFreeroam()
	{
		ClearFreeroam();
		foreach (scrFloor listFloor in listFloors)
		{
			if (listFloor.freeroam)
			{
				MakeFreeroamGrid(listFloor);
			}
		}
	}

	public void MakeFreeroamGrid(scrFloor floorComp)
	{
		floorContainer = GetFloorContainer();
		float radiusScale = floorComp.radiusScale;
		float x = floorComp.transform.localScale.x;
		floorComp.radiusScale = radiusScale;
		double num = scrController.instance.startRadius * radiusScale;
		int num2 = (int)floorComp.freeroamDimensions.x;
		int num3 = (int)floorComp.freeroamDimensions.y;
		FreeroamArea item = new FreeroamArea(floorComp);
		listFreeroam.Add(item);
		listFreeroamStartTiles.Add(floorComp);
		Vector3 b = new Vector3(floorComp.freeroamOffset.x, floorComp.freeroamOffset.y, 0f) * (float)num;
		for (int i = 0; i < num2 * num3; i++)
		{
			int num4 = i % num2;
			int num5 = (int)Mathf.Floor((float)i / (float)num2);
			float num6 = 90f;
			num6 *= MathF.PI / 180f;
			Vector3 right = Vector3.right;
			Vector3 a = new Vector3(Mathf.Cos(num6), Mathf.Sin(num6), 0f);
			GameObject gameObject = null;
			Vector3 position = floorComp.gameObject.transform.position + b + right * num4 * (float)num + a * num5 * (float)num;
			if (Application.isPlaying)
			{
				gameObject = UnityEngine.Object.Instantiate(meshFloor, position, Quaternion.identity);
			}
			gameObject.name = $"{floorComp.seqID}-{i + 1}/Floor freeroam x{num4} y{num5}";
			gameObject.gameObject.transform.parent = floorContainer.transform;
			scrFloor component = gameObject.GetComponent<scrFloor>();
			component.prevfloor = floorComp;
			component.nextfloor = floorComp.nextfloor;
			component.freeroam = true;
			component.freeroamGenerated = true;
			component.freeroamRegion = listFreeroamStartTiles.Count - 1;
			component.freeroamPosition = new Vector2Int(num4, num5);
			FloorMesh floorMesh = component.GetComponent<FloorMeshRenderer>().floorMesh;
			floorComp.freeroamFloors.Add(component);
			component.isCCW = floorComp.isCCW;
			component.numPlanets = floorComp.numPlanets;
			listFreeroam.Last().Add(component);
			component.angleCorrectionType = floorComp.angleCorrectionType;
			component.floatDirection = 0f;
			component.stringDirection = 'R';
			component.styleNum = floorComp.styleNum;
			component.initialTrackStyle = floorComp.initialTrackStyle;
			component.seqID = floorComp.seqID;
			component.entryangle = -1.5707963705062866;
			component.exitangle = 1.5707963705062866;
			component.radiusScale = radiusScale;
			component.speed = floorComp.speed;
			component.transform.localScale = Vector3.up * x + Vector3.right * x;
			if (Application.isPlaying)
			{
				component.SetColor(floorComp.floorRenderer.color);
			}
			component.SetTrackStyle(component.initialTrackStyle, initial: true);
			float num9 = floorMesh._width = (floorMesh._length = Mathf.Lerp(floorMesh._width, floorMesh._length, 0.4f));
			component.UpdateAngle();
		}
	}

	public void ColorFreeroam()
	{
		foreach (FreeroamArea item in listFreeroam)
		{
			foreach (scrFloor item2 in item)
			{
				item2.SetColor(item.parentFloor.floorRenderer.material.GetColor("_Color"));
			}
			item.parentFloor.floorRenderer.renderer.enabled = false;
		}
	}

	public void DrawHolds(bool unfillHolds = false)
	{
		GameObject x = GameObject.Find("Hold Container");
		if (x == null)
		{
			x = new GameObject("Hold Container");
		}
		holdContainer = x;
		if (holdMaterial == null)
		{
			holdMaterial = new Material(RDConstants.data.holdShader);
			holdMaterial.SetTexture("_MainTex", lm2.holdTex);
			holdMaterial.renderQueue = 2900;
		}
		foreach (GameObject holdGO in holdGOs)
		{
			UnityEngine.Object.Destroy(holdGO);
		}
		holdGOs.Clear();
		foreach (scrFloor listFloor in listFloors)
		{
			if (listFloor.holdLength > -1)
			{
				GameObject gameObject = new GameObject();
				scrHoldRenderer scrHoldRenderer = gameObject.AddComponent<scrHoldRenderer>();
				gameObject.AddComponent<MeshFilter>();
				gameObject.AddComponent<MeshRenderer>();
				gameObject.name = "Hold";
				gameObject.transform.parent = holdContainer.transform;
				holdGOs.Add(gameObject);
				Mesh mesh = gameObject.GetComponent<MeshFilter>().mesh;
				MeshRenderer component = gameObject.GetComponent<MeshRenderer>();
				mesh.Clear();
				component.material = new Material(holdMaterial);
				scrHoldRenderer.m_mesh = mesh;
				scrHoldRenderer.m_meshRenderer = component;
				listFloor.holdRenderer = scrHoldRenderer;
				scrHoldRenderer.startFloor = listFloor;
				scrHoldRenderer.CreateMesh();
				if (unfillHolds)
				{
					scrHoldRenderer.Unfill();
				}
			}
		}
	}

	public int DrawMultiPlanet()
	{
		foreach (scrPlanet dummyPlanet in scrController.instance.dummyPlanets)
		{
			if (dummyPlanet != null)
			{
				UnityEngine.Object.Destroy(dummyPlanet.gameObject);
			}
		}
		foreach (LineRenderer multiPlanetLine in scrController.instance.multiPlanetLines)
		{
			if (multiPlanetLine != null)
			{
				UnityEngine.Object.Destroy(multiPlanetLine.gameObject);
			}
		}
		if (RDC.hideTaroGimmicks)
		{
			return 2;
		}
		scrController.instance.dummyPlanets.Clear();
		scrController.instance.multiPlanetLines.Clear();
		int num = 2;
		int num2 = 2;
		foreach (scrFloor listFloor in listFloors)
		{
			int numPlanet = listFloor.numPlanets;
			float speed = listFloor.speed;
			bool isCCW = listFloor.isCCW;
			int numPlanets = listFloor.numPlanets;
			num = Math.Max(num, numPlanets);
			if (numPlanets > num2)
			{
				float num3 = (float)listFloor.entryangle + (float)((!isCCW) ? 1 : (-1)) * (float)scrMisc.GetInverseAnglePerBeatMultiplanet(numPlanets) / 2f;
				double num4 = (double)scrController.instance.startRadius / (2.0 * (double)Mathf.Sin(3.141592f / (float)numPlanets));
				double num5 = 0.0;
				Vector3 vector = new Vector3(Mathf.Sin(num3) * (float)num4, Mathf.Cos(num3) * (float)num4, 0f);
				for (int i = num2; i < numPlanets; i++)
				{
					scrPlanet scrPlanet = UnityEngine.Object.Instantiate(scrController.instance.redPlanet);
					scrPlanet.name = $"PlanetDummy {i} on floor {listFloor.seqID}";
					scrPlanet.EnableCustomColor();
					scrPlanet.SetPlanetColor(new Color(0.9f, 0.9f, 0.9f, 1f));
					scrPlanet.SetTailColor(new Color(0.9f, 0.9f, 0.9f, 1f));
					scrPlanet.SetFaceMode(enabled: false);
					scrPlanet.isExtra = true;
					scrPlanet.dummyPlanets = true;
					scrPlanet.DisableParticles();
					Renderer[] componentsInChildren = scrPlanet.GetComponentsInChildren<Renderer>();
					for (int j = 0; j < componentsInChildren.Length; j++)
					{
						componentsInChildren[j].sortingOrder = -1;
					}
					if (scrController.instance.paused)
					{
						scrPlanet.Destroy();
					}
					scrPlanet.attachedDummyFloor = listFloor.seqID;
					listFloor.dummyPlanets.Add(scrPlanet);
					num5 = (double)((!isCCW) ? 1 : (-1)) * (6.2831852 * (((double)(1 - i) - (double)numPlanets / 2.0) / (double)numPlanets)) + (double)num3;
					scrPlanet.transform.localPosition = new Vector3(vector.x + Mathf.Sin((float)num5) * (float)num4, vector.y + Mathf.Cos((float)num5) * (float)num4, vector.z);
					scrController.instance.dummyPlanets.Add(scrPlanet);
					scrPlanet.transform.SetParent(listFloor.transform, worldPositionStays: false);
				}
				GameObject gameObject = new GameObject();
				LineRenderer lineRenderer = gameObject.AddComponent<LineRenderer>();
				lineRenderer.positionCount = numPlanets + 1;
				gameObject.transform.parent = listFloor.transform;
				lineRenderer.material = scrController.instance.lineMaterial;
				lineRenderer.textureMode = LineTextureMode.Tile;
				lineRenderer.sortingLayerName = "Floor";
				lineRenderer.sortingOrder = listFloor.floorRenderer.renderer.sortingOrder + 6;
				lineRenderer.startWidth = 0.05f;
				lineRenderer.endWidth = 0.05f;
				Color color3 = lineRenderer.endColor = (lineRenderer.startColor = scrController.instance.lineColor.WithAlpha(listFloor.opacity * 0.5f));
				lineRenderer.name = "Planet polygon indicator (floor " + listFloor.seqID.ToString() + ")";
				lineRenderer.useWorldSpace = false;
				for (int k = 0; k < numPlanets + 1; k++)
				{
					num5 = (double)((!isCCW) ? 1 : (-1)) * (Math.PI * 2.0 * (((double)(1 - k) - (double)numPlanets / 2.0) / (double)numPlanets)) + (double)num3;
					lineRenderer.SetPosition(k, new Vector3(vector.x + Mathf.Sin((float)num5) * (float)num4, vector.y + Mathf.Cos((float)num5) * (float)num4, vector.z));
				}
				listFloor.multiplanetLine = lineRenderer;
				lineRenderer.transform.position = listFloor.transform.position;
				lineRenderer.transform.eulerAngles = listFloor.transform.eulerAngles;
				scrController.instance.multiPlanetLines.Add(lineRenderer);
			}
			else if (numPlanets < num2)
			{
				float num6 = (float)listFloor.entryangle + (float)((!isCCW) ? 1 : (-1)) * (float)scrMisc.GetInverseAnglePerBeatMultiplanet(num2) / 2f;
				double num7 = (double)scrController.instance.startRadius / (2.0 * (double)Mathf.Sin(3.141592f / (float)num2));
				double num8 = 0.0;
				Vector3 vector2 = new Vector3(Mathf.Sin(num6) * (float)num7, Mathf.Cos(num6) * (float)num7, 0f);
				for (int l = numPlanets; l < num2; l++)
				{
					scrPlanet scrPlanet2 = UnityEngine.Object.Instantiate(scrController.instance.redPlanet);
					scrPlanet2.name = "PlanetDummy " + l.ToString() + " on floor " + listFloor.seqID.ToString();
					scrPlanet2.EnableCustomColor();
					scrPlanet2.SetPlanetColor(new Color(0.9f, 0.9f, 0.9f, 1f));
					scrPlanet2.SetTailColor(new Color(0.9f, 0.9f, 0.9f, 1f));
					scrPlanet2.isExtra = true;
					scrPlanet2.SetFaceMode(enabled: false);
					scrPlanet2.dummyPlanets = true;
					scrPlanet2.DisableParticles();
					if (scrController.instance.paused)
					{
						scrPlanet2.Destroy();
					}
					else
					{
						scrPlanet2.DestroyAllButRing();
					}
					scrPlanet2.attachedDummyFloor = listFloor.seqID;
					listFloor.dummyPlanets.Add(scrPlanet2);
					num8 = (double)((!isCCW) ? 1 : (-1)) * (6.2831852 * (((double)(1 - l) - (double)num2 / 2.0) / (double)num2)) + (double)num6;
					scrPlanet2.transform.localPosition = new Vector3(vector2.x + Mathf.Sin((float)num8) * (float)num7, vector2.y + Mathf.Cos((float)num8) * (float)num7, vector2.z) + listFloor.transform.position;
					scrController.instance.dummyPlanets.Add(scrPlanet2);
					scrPlanet2.transform.SetParent(listFloor.transform);
				}
			}
			num2 = numPlanets;
		}
		scrController.instance.lineMaterial.DOFloat(scrController.instance.lineMaterial.GetFloat("_Time0") + 10f, "_Time0", 10f).SetEase(Ease.Linear).SetLoops(-1, LoopType.Incremental)
			.SetUpdate(isIndependentUpdate: true);
		return num;
	}

	public void RefreshAngles()
	{
		foreach (scrFloor listFloor in listFloors)
		{
			listFloor.UpdateAngle();
		}
	}
}
