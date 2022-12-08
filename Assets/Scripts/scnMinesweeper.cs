using DG.Tweening;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

public class scnMinesweeper : ADOBase
{
	private struct SweeperStage
	{
		public Vector2Int gridSize;

		public int bombCount;

		public float zoom;

		public SweeperStage(int width, int height, int bombCount, float zoom = 1f)
		{
			gridSize = new Vector2Int(width, height);
			this.bombCount = bombCount;
			this.zoom = zoom;
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass56_0
	{
		public scrFloor f;

		internal void _003CWin_003Eb__0()
		{
			f.enabled = false;
			f.transform.position += Vector3.up * 9999f;
		}
	}

	[StructLayout(LayoutKind.Auto)]
	[CompilerGenerated]
	private struct _003C_003Ec__DisplayClass60_0
	{
		public string art;

		public int artOffset;
	}

	[StructLayout(LayoutKind.Auto)]
	[CompilerGenerated]
	private struct _003C_003Ec__DisplayClass60_1
	{
		public int i;
	}

	public static int stage = 0;

	public static string sceneToReturnTo;

	private static readonly SweeperStage[] stageData = new SweeperStage[3]
	{
		new SweeperStage(7, 7, 7),
		new SweeperStage(15, 9, 25),
		new SweeperStage(41, 25, 0, 2.56f)
	};

	private static readonly string[] colors = new string[9]
	{
		"FFFFFF",
		"A2BAFF",
		"AFFFAC",
		"FF999F",
		"D6A5FF",
		"AD7072",
		"00EEEE",
		"353535",
		"FFFFFF"
	};

	private const string colorIdle = "C7C7E2";

	private const string colorPressed = "ABABC2";

	public GameObject floorPrefab;

	public Transform floorParent;

	public GameObject minePrefab;

	public AudioClip music;

	public AudioClip musicTutorial;

	public GameObject dokuroCredits;

	private int[] tileValue;

	private bool[] hasBomb;

	private scrFloor[] floors;

	private int selectedFloor;

	private Vector2 lastPlanetPos;

	private int lastBeat;

	private int timerStartPosition = 9999;

	private int revealedFloors;

	private bool readySetGo;

	private bool madeFirstMove;

	private bool transitioningToNextLevel;

	private AudioSource[] countdownTicks = new AudioSource[20];

	private float[] endingDistances;

	private float endingTimer;

	private bool doingEnding;

	private bool won;

	private bool dead;

	private static readonly Vector2Int[] neighbourOffsets = new Vector2Int[8]
	{
		Vector2Int.up,
		Vector2Int.right,
		Vector2Int.down,
		Vector2Int.left,
		Vector2Int.left + Vector2Int.up,
		Vector2Int.up + Vector2Int.right,
		Vector2Int.right + Vector2Int.down,
		Vector2Int.down + Vector2Int.left
	};

	private string goodJobArt = "\n    -----------------------------------------\n    ------BBBB----BBB---BBBBB--BB-BB-BB------\n    -----BBBBBB--BBBBB--BBBBBB-BB-BB-BB------\n    -----BB--BB-BBB-BBB-BB--BB---------------\n    -----BB--BB-BB---BB-BB--BB---------------\n    ---------BB-BB---BB-BBBBB--BB-BB-BB------\n    ---------BB-BB---BB-BBBBB--BB-BB-BB------\n    ---------BB-BB---BB-BB--BB-BB-BB-BB------\n    ---------BB-BBB-BBB-BB--BB-BB-BB-BB------\n    ---------BB--BBBBB--BBBBBB-BB-BB-BB------\n    ---------BB---BBB---BBBBB--BB-BB-BB------\n    -----------------------------------------\n    -----------------------------------------\n    -----------------------------------------\n    -------RRRR----RRR-----RRR---RRRRR-------\n    ------RRRRRR--RRRRR---RRRRR--RRRRRR------\n    -----RRR--RR-RRR-RRR-RRR-RRR-RR--RR------\n    -----RR---RR-RR---RR-RR---RR-RR--RR------\n    -----RR-RRRR-RR---RR-RR---RR-RR--RR------\n    -----RR-RRRR-RR---RR-RR---RR-RR--RR------\n    -----RR------RR---RR-RR---RR-RR--RR------\n    -----RRR--RR-RRR-RRR-RRR-RRR-RR--RR------\n    ------RRRRRR--RRRRR---RRRRR--RRRRRR------\n    -------RRRR----RRR-----RRR---RRRRR-------\n    -----------------------------------------\n    ";

	private string goodJobArtCN = "\n        -----------------------------------------\n        -----------------------------------------\n        -----------------------------------------\n        -----------------------------------------\n        -----------------------------------------\n        -----------------------------------------\n        -----------------------------------------\n        ------RR----RR-----B----B---------R------\n        --------R--R-------B-BBBBBBB------R------\n        -----RRRRRRRRRR----B----B----------------\n        -------R----R----B-B-B-BBB-B------R------\n        -------R-RRRR----B-B--B-B-B-------R------\n        -------RRRR-R-----BB-BBBBBBB------R------\n        -------R----R-----BBB--B---------RRR-----\n        -------RRRRRR------B--BBBBB------RRR-----\n        ---------R-------BBBBB--B--------RRR-----\n        -----RRRRRRRRRR----B--BBBBBB-----RRR-----\n        ---------R---------B----B---------R------\n        -----------------------------------------\n        -----------------------------------------\n        -----------------------------------------\n        -----------------------------------------\n        -----------------------------------------\n        -----------------------------------------\n        -----------------------------------------\n    ";

	private Vector2Int gridSize => stageData[stage].gridSize;

	private int bombCount => stageData[stage].bombCount;

	private float zoom => stageData[stage].zoom;

	private int lastStage => stageData.Length - 1;

	private bool isLastStage => stage == lastStage;

	private int Index(Vector2Int xy)
	{
		return Index(xy.x, xy.y);
	}

	private int Index(int x, int y)
	{
		return y * gridSize.x + x;
	}

	private Vector2Int Pos(int index)
	{
		return new Vector2Int(index % gridSize.x, index / gridSize.x);
	}

	private bool IsOnBoard(Vector2Int pos)
	{
		if (pos.x >= 0 && pos.x < gridSize.x && pos.y >= 0)
		{
			return pos.y < gridSize.y;
		}
		return false;
	}

	public static void EnterScene()
	{
		if (ADOBase.hasTaroDLC)
		{
			stage = 0;
			scrController instance = scrController.instance;
			sceneToReturnTo = instance.levelName;
			GCS.sceneToLoad = "scnMinesweeper";
			GCS.speedTrialMode = false;
			instance.StartLoadingScene(WipeDirection.StartsFromRight);
		}
	}

	private void GenerateBoard()
	{
		int num = gridSize.x * gridSize.y;
		tileValue = new int[num];
		hasBomb = new bool[num];
		floors = new scrFloor[num];
		int num2 = 1000;
		for (int i = 0; i < bombCount; i++)
		{
			int num3 = Random.Range(0, num);
			if (hasBomb[num3] && num2 > 0)
			{
				num2--;
				i--;
			}
			else
			{
				hasBomb[num3] = true;
			}
		}
		for (int j = 0; j < num; j++)
		{
			Vector2Int xy = Pos(j);
			GameObject gameObject = Object.Instantiate(floorPrefab, new Vector2((float)xy.x - (float)gridSize.x / 2f, (float)xy.y - (float)gridSize.y / 2f) + Vector2.one * 0.5f, Quaternion.identity, floorParent);
			gameObject.name = $"Floor ({xy.x}, {xy.y}; Index {j} aka {Index(xy)})";
			floors[j] = gameObject.GetComponent<scrFloor>();
		}
		selectedFloor = num / 2;
		ADOBase.controller.camy.zoomSize = zoom;
	}

	private void Awake()
	{
		ADOBase.controller.camy.followMode = false;
		scrUIController.instance.txtCountdown.enabled = false;
		GenerateBoard();
		bool flag = stage == 0;
		ADOBase.conductor.song.clip = (flag ? musicTutorial : music);
		dokuroCredits.SetActive(flag);
		readySetGo = true;
		timerStartPosition = -1;
	}

	private void Start()
	{
		ADOBase.controller.chosenplanet.currfloor = floors[selectedFloor];
		lastPlanetPos = ADOBase.controller.chosenplanet.transform.position;
		ADOBase.controller.responsive = false;
	}

	private void ProcessTile(int index, Vector2Int direction = default(Vector2Int))
	{
		Vector2Int vector2Int = Pos(index) + direction;
		if (!IsOnBoard(vector2Int))
		{
			return;
		}
		index = Index(vector2Int);
		if (tileValue[index] != 0 || hasBomb[index])
		{
			return;
		}
		int num = 0;
		Vector2Int[] array = neighbourOffsets;
		foreach (Vector2Int b in array)
		{
			Vector2Int vector2Int2 = vector2Int + b;
			if (IsOnBoard(vector2Int2))
			{
				int num2 = Index(vector2Int2);
				if (hasBomb[num2])
				{
					num++;
				}
			}
		}
		tileValue[index] = num + 1;
		scrFloor scrFloor = floors[index];
		if (num > 0)
		{
			Text componentInChildren = scrFloor.GetComponentInChildren<Text>();
			componentInChildren.text = num.ToString();
			componentInChildren.color = colors[num].HexToColor();
		}
		else
		{
			array = neighbourOffsets;
			foreach (Vector2Int direction2 in array)
			{
				ProcessTile(index, direction2);
			}
		}
		scrFloor.SetColor("ABABC2".HexToColor());
		scrFloor.transform.localScale = Vector2.one * 0.8f;
		scrFloor.transform.DOScale(1f, 0.5f).SetEase(Ease.OutSine);
		revealedFloors++;
	}

	private void Update()
	{
		Vector2 vector = ADOBase.controller.chosenplanet.transform.position;
		int num = (int)(ADOBase.conductor.songposition_minusi / ADOBase.conductor.crotchet) + 1;
		bool flag = isLastStage && !readySetGo;
		int num2 = readySetGo ? 8 : (flag ? 10 : 3);
		bool flag2 = !vector.Approximately(lastPlanetPos);
		if (flag2)
		{
			Vector2Int b = Vector2Int.RoundToInt(vector - lastPlanetPos);
			selectedFloor = Index(Pos(selectedFloor) + b);
			timerStartPosition = num;
			StopCountdownAudio();
			scrUIController.instance.txtCountdown.text = "";
			madeFirstMove = true;
		}
		bool flag3 = tileValue[selectedFloor] == 0;
		if ((readySetGo || (flag3 && madeFirstMove)) && (flag2 || num != lastBeat) && !won && !dead)
		{
			int num3 = num2 + 1 - (num - timerStartPosition - 1);
			Text txtCountdown = scrUIController.instance.txtCountdown;
			if (num3 >= 1 && num3 <= num2)
			{
				if (!readySetGo)
				{
					txtCountdown.gameObject.SetActive(value: true);
					txtCountdown.GetComponent<scrCountdown>().enabled = false;
					txtCountdown.text = num3.ToString();
				}
			}
			else if (num3 == 0)
			{
				if (readySetGo)
				{
					ADOBase.controller.responsive = true;
					readySetGo = false;
				}
				else
				{
					TryRevealTile(selectedFloor);
					txtCountdown.text = "";
				}
			}
			if (flag2)
			{
				ScheduleCountdownAudio(num + 1, num2, !readySetGo);
			}
		}
		if (doingEnding)
		{
			EndingUpdate();
		}
		if (won)
		{
			WaitForEndLevel();
		}
		lastPlanetPos = vector;
		lastBeat = num;
		if (ADOBase.gc.debug && UnityEngine.Input.GetKeyDown(KeyCode.F5))
		{
			Win();
		}
		if (readySetGo)
		{
			ADOBase.controller.responsive = false;
		}
	}

	private void StopCountdownAudio()
	{
		AudioSource[] array = countdownTicks;
		foreach (AudioSource audioSource in array)
		{
			if (audioSource != null && audioSource.time <= 0f)
			{
				audioSource.Stop();
			}
		}
	}

	private void ScheduleCountdownAudio(int beat, int duration, bool decliningPitch = false)
	{
		scrConductor conductor = ADOBase.conductor;
		float num = -0.01f;
		for (int i = 0; i < duration; i++)
		{
			double time = conductor.dspTimeSongPosZero + (double)(beat + i) * conductor.crotchet / (double)conductor.song.pitch + (double)num;
			AudioSource audioSource = AudioManager.Play("sndHat", time, conductor.hitSoundGroup);
			audioSource.pitch = Mathf.Lerp(1f, 0.1f, (float)i / (float)duration);
			countdownTicks[i] = audioSource;
		}
	}

	private void TryRevealTile(int index)
	{
		if (isLastStage)
		{
			PlayEnding();
			return;
		}
		if (revealedFloors == 0)
		{
			MoveBombsAt(index);
			ProcessTile(index);
			return;
		}
		if (hasBomb[index])
		{
			Fail();
			return;
		}
		ProcessTile(index);
		if (revealedFloors == gridSize.x * gridSize.y - bombCount)
		{
			Win();
		}
	}

	private void MoveBombsAt(int index)
	{
		Vector2Int a = Pos(index);
		List<int> list = new List<int>();
		Vector2Int[] array = neighbourOffsets;
		foreach (Vector2Int b in array)
		{
			Vector2Int vector2Int = a + b;
			if (IsOnBoard(vector2Int))
			{
				list.Add(Index(vector2Int));
			}
		}
		list.Add(index);
		foreach (int item in list)
		{
			if (hasBomb[item])
			{
				int num = item;
				bool flag;
				do
				{
					num = Random.Range(0, gridSize.x * gridSize.y);
					flag = hasBomb[num];
					if (!flag)
					{
						foreach (int item2 in list)
						{
							if (num == item2)
							{
								flag = true;
								break;
							}
						}
					}
				}
				while (flag);
				hasBomb[item] = false;
				hasBomb[num] = true;
			}
		}
	}

	private void Fail()
	{
		dead = true;
		ADOBase.controller.FailAction();
		StopCountdownAudio();
		for (int i = 0; i < floors.Length; i++)
		{
			if (hasBomb[i])
			{
				bool num = i == selectedFloor;
				float delay = num ? 0f : Random.Range(0f, 0.5f);
				float duration = 0.33f;
				floors[i].TweenColor(Color.red, duration, delay);
				if (!num)
				{
					scrSpike component = Object.Instantiate(minePrefab, floors[i].transform.position, default(Quaternion)).GetComponent<scrSpike>();
					component.transform.localScale = Vector2.zero;
					component.transform.DOScale(Vector2.one, duration).SetEase(Ease.OutBack).SetDelay(delay);
					component.ballSprite.curFrame = Random.Range(0, component.ballSprite.frames.Count);
				}
			}
		}
	}

	private void Win()
	{
		if (won)
		{
			return;
		}
		won = true;
		ADOBase.controller.responsive = false;
		if (!isLastStage)
		{
			scrUIController.instance.txtCountdown.text = RDString.Get("status.congratulations");
		}
		scrFlash.Flash(Color.white.WithAlpha(0.3f));
		scrSfx.instance.PlaySfx(SfxSound.Applause);
		for (int i = 0; i < floors.Length; i++)
		{
			if (hasBomb[i])
			{
				scrFloor scrFloor = floors[i];
				scrFloor.ToggleCollider(collEn: false);
				scrFloor.MoveToBack();
				scrFloor f = scrFloor;
				f.TweenOpacity(0f, 3f, Ease.InCubic);
				if ((bool)f.legacyFloorSpriteRenderer)
				{
					f.legacyFloorSpriteRenderer.DOColor(new Color(1f, 1f, 1f, 0f), 3f).SetEase(Ease.InCubic);
				}
				scrFloor.transform.DOScale(0.5f, 3f).SetEase(Ease.InCubic).OnComplete(delegate
				{
					f.enabled = false;
					f.transform.position += Vector3.up * 9999f;
				});
				scrFloor.transform.DOMoveY(-2f, 3f).SetRelative(isRelative: true).SetEase(Ease.InCubic);
				scrFloor.transform.DORotate(Vector3.forward * 45f, 3f).SetRelative(isRelative: true).SetEase(Ease.InCubic);
			}
		}
	}

	private void WaitForEndLevel()
	{
		if (ADOBase.controller.CountValidKeysPressed() > 0)
		{
			EndLevel();
		}
	}

	private void EndLevel()
	{
		if (!transitioningToNextLevel)
		{
			transitioningToNextLevel = true;
			if (isLastStage)
			{
				GCS.sceneToLoad = sceneToReturnTo;
			}
			else
			{
				stage++;
				GCS.sceneToLoad = ADOBase.controller.levelName;
			}
			ADOBase.controller.StartLoadingScene(WipeDirection.StartsFromRight);
		}
	}

	private void PlayEnding()
	{
		doingEnding = true;
		ADOBase.controller.responsive = false;
		int num = gridSize.x * gridSize.y;
		endingDistances = new float[num];
		for (int i = 0; i < num; i++)
		{
			endingDistances[i] = Vector2.Distance(Pos(selectedFloor), Pos(i));
		}
		DOVirtual.DelayedCall(3.5f, delegate
		{
			Win();
		});
	}

	private void EndingUpdate()
	{
		endingTimer += Time.deltaTime;
		int num = gridSize.x * gridSize.y;
		_003C_003Ec__DisplayClass60_0 _003C_003Ec__DisplayClass60_ = default(_003C_003Ec__DisplayClass60_0);
		_003C_003Ec__DisplayClass60_.artOffset = 0;
		_003C_003Ec__DisplayClass60_.art = ((RDString.language == SystemLanguage.Chinese || RDString.language == SystemLanguage.ChineseSimplified || RDString.language == SystemLanguage.ChineseTraditional) ? goodJobArtCN : goodJobArt);
		Color color = _003CEndingUpdate_003Eg__GetTextColor_007C60_0(red: false);
		Color color2 = _003CEndingUpdate_003Eg__GetTextColor_007C60_0(red: true);
		Color a = "C7C7E2".HexToColor();
		_003C_003Ec__DisplayClass60_1 _003C_003Ec__DisplayClass60_2 = default(_003C_003Ec__DisplayClass60_1);
		_003C_003Ec__DisplayClass60_2.i = 0;
		while (_003C_003Ec__DisplayClass60_2.i < num)
		{
			int artOffset;
			while (_003CEndingUpdate_003Eg__getPixel_007C60_1(ref _003C_003Ec__DisplayClass60_, ref _003C_003Ec__DisplayClass60_2) != '-' && _003CEndingUpdate_003Eg__getPixel_007C60_1(ref _003C_003Ec__DisplayClass60_, ref _003C_003Ec__DisplayClass60_2) != 'R' && _003CEndingUpdate_003Eg__getPixel_007C60_1(ref _003C_003Ec__DisplayClass60_, ref _003C_003Ec__DisplayClass60_2) != 'B')
			{
				artOffset = _003C_003Ec__DisplayClass60_.artOffset;
				_003C_003Ec__DisplayClass60_.artOffset = artOffset + 1;
			}
			Color a2 = (_003CEndingUpdate_003Eg__getPixel_007C60_1(ref _003C_003Ec__DisplayClass60_, ref _003C_003Ec__DisplayClass60_2) == '-') ? Color.white : ((_003CEndingUpdate_003Eg__getPixel_007C60_1(ref _003C_003Ec__DisplayClass60_, ref _003C_003Ec__DisplayClass60_2) == 'R') ? color2 : color);
			a2 = Color.Lerp(a2, Color.white, 0.25f);
			float num2 = endingDistances[_003C_003Ec__DisplayClass60_2.i];
			int num3 = 8;
			float t = endingTimer * (float)num3 - num2;
			int i = _003C_003Ec__DisplayClass60_2.i;
			floors[i].floorRenderer.color = Color.Lerp(a, a2, t);
			floors[i].transform.localScale = Vector2.Lerp(Vector2.one, Vector2.one * 1.055f, t);
			artOffset = _003C_003Ec__DisplayClass60_2.i;
			_003C_003Ec__DisplayClass60_2.i = artOffset + 1;
		}
	}

	[CompilerGenerated]
	private static Color _003CEndingUpdate_003Eg__GetTextColor_007C60_0(bool red)
	{
		Color color = scrMisc.PlayerColorToRealColor(Persistence.GetPlayerColor(red));
		float num = (color.r + color.g + color.b) / 3f;
		if ((double)(1f - num) <= 0.25)
		{
			color = Color.gray;
		}
		return color;
	}

	[CompilerGenerated]
	private static char _003CEndingUpdate_003Eg__getPixel_007C60_1(ref _003C_003Ec__DisplayClass60_0 P_0, ref _003C_003Ec__DisplayClass60_1 P_1)
	{
		return P_0.art[P_1.i + P_0.artOffset];
	}
}
