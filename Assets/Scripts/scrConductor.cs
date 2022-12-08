using ADOFAI;
using DG.Tweening;
using RDTools;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class scrConductor : ADOBase
{
	private struct HitSoundsData
	{
		public HitSound hitSound;

		public double time;

		public float volume;

		public bool played;

		public HitSoundsData(HitSound hitSound, double time, float volume)
		{
			this.hitSound = hitSound;
			this.time = time;
			this.volume = volume;
			played = false;
		}
	}

	private struct HoldSoundsData
	{
		public string name;

		public double time;

		public double endTime;

		public float volume;

		public bool played;

		public HoldSoundsData(string name, double time, double endTime, float volume)
		{
			this.name = name;
			this.time = time;
			this.endTime = endTime;
			this.volume = volume;
			played = false;
		}
	}

	public struct ExtraTickData
	{
		public double time;

		public int count;

		public float speed;

		public ExtraTickData(double time, int count, float speed)
		{
			this.time = time;
			this.count = count;
			this.speed = speed;
		}
	}

	public enum DuckState
	{
		None,
		Starting,
		Ducked,
		Stopping
	}

	public static List<CalibrationPreset> defaultPresets = new List<CalibrationPreset>();

	public static List<CalibrationPreset> userPresets = new List<CalibrationPreset>();

	public static CalibrationPreset currentPreset;

	public static int visualOffset;

	[Header("Components")]
	public AudioSource song;

	public AudioSource song2;

	public scnEditor editorComponent;

	public scnCLS CLSComponent;

	public CustomLevel customLevelComponent;

	public Text txtOffset;

	[Header("Song Parameters")]
	public double addoffset;

	public float bpm;

	public bool isGameWorld = true;

	[Tooltip("Set to true if the song file does NOT have a countdown baked into it. The game will add its own countdown and cymbal crash")]
	public bool separateCountdownTime;

	public bool playCountdownHihats = true;

	public bool playEndingCymbal = true;

	[Header("Hitsounds")]
	[Tooltip("In non-editor levels, i.e. game scene, needs this to be on to have separate hitsounds.")]
	public bool forceHitSounds;

	public HitSound hitSound = HitSound.Kick;

	public AudioMixerGroup hitSoundGroup;

	[Range(0f, 2f)]
	public float hitSoundVolume = 1f;

	public HoldStartSound holdStartSound;

	public HoldEndSound holdEndSound;

	public HoldLoopSound holdLoopSound;

	public HoldMidSound holdMidSound = HoldMidSound.None;

	public HoldMidSoundType holdMidSoundType;

	public float holdMidSoundDelay = 0.5f;

	public HoldMidSoundTimingRelativeTo holdMidSoundTiming = HoldMidSoundTimingRelativeTo.End;

	[Range(0f, 2f)]
	public float holdSoundVolume = 1f;

	private List<HoldSoundsData> holdSoundsData = new List<HoldSoundsData>();

	private int nextHoldSoundToSchedule;

	public List<ExtraTickData> extraTicksCountdown = new List<ExtraTickData>();

	private int nextExtraTickToSchedule;

	public bool useMidspinHitSound;

	private const HitSound DefaultMidspinHitSound = HitSound.ReverbClack;

	public HitSound midspinHitSound = HitSound.ReverbClack;

	[Header("Mixer")]
	[Range(0f, 2f)]
	public float worldBalanceVolume = 1f;

	[NonSerialized]
	public double crotchet;

	[NonSerialized]
	public double crotchetAtStart;

	[NonSerialized]
	public double dspTime;

	[NonSerialized]
	public double deltaSongPos;

	[NonSerialized]
	public double lastHit;

	[NonSerialized]
	public double actualLastHit;

	[NonSerialized]
	public int beatNumber;

	[NonSerialized]
	public int barNumber;

	public int countdownTicks = 4;

	public float countdownSpeedMultiplier = 1f;

	[NonSerialized]
	public bool hasSongStarted;

	[NonSerialized]
	public bool skipOffset;

	[NonSerialized]
	public bool fastTakeoff;

	[NonSerialized]
	public bool getSpectrum;

	private bool playedHitSounds;

	private List<HitSoundsData> hitSoundsData = new List<HitSoundsData>();

	private int nextHitSoundToSchedule;

	private int crotchetsPerBar = 8;

	private double _songposition_minusi;

	private double previousFrameTime;

	private double lastReportedPlayheadPosition;

	private double nextBeatTime;

	private double nextBarTime;

	[NonSerialized]
	public int onBeatFrame = -1;

	private float buffer = 1f;

	private double dspTimeSong;

	private DuckState duckState;

	private float songPreduckVolume;

	private float song2PreduckVolume;

	private double[] countdownTimes = new double[4];

	private List<ADOBase> _onBeats = new List<ADOBase>();

	private float[] _spectrum = new float[1024];

	private static scrConductor _instance;

	private Coroutine startMusicCoroutine;

	private bool hasSetMidspinHitsound;

	public static float calibration_i => (float)currentPreset.inputOffset / 1000f;

	public static float calibration_v => (float)visualOffset / 1000f;

	public float adjustedCountdownTicks => (float)countdownTicks / countdownSpeedMultiplier;

	public double dspTimeSongPosZero => dspTimeSong + addoffset / (double)song.pitch;

	public List<ADOBase> onBeats
	{
		get
		{
			if (_onBeats.Count == 0)
			{
				LoadOnBeats();
			}
			return _onBeats;
		}
		set
		{
			_onBeats = value;
		}
	}

	public double songposition_minusi
	{
		get
		{
			if (!GCS.d_webglConductor)
			{
				return _songposition_minusi;
			}
			if (song != null)
			{
				return _songposition_minusi - (double)scrMisc.Remap(song.pitch, 1f, 1.8f, 0f, 0.074f);
			}
			return _songposition_minusi;
		}
		set
		{
			_songposition_minusi = value;
		}
	}

	public float[] spectrum
	{
		get
		{
			getSpectrum = true;
			return _spectrum;
		}
	}

	public static scrConductor instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = UnityEngine.Object.FindObjectOfType<scrConductor>();
			}
			return _instance;
		}
	}

	public double songposition_minusv => songposition_minusi + (double)calibration_i - (double)calibration_v;

	private void Awake()
	{
		ADOBase.Startup();
		if (GCS.d_webglConductor)
		{
			buffer = 0.5f;
		}
		if (Application.platform == RuntimePlatform.WindowsPlayer)
		{
			buffer = 0.5f;
		}
		if (Application.platform == RuntimePlatform.WindowsEditor)
		{
			buffer = 0.5f;
		}
		if (Application.platform == RuntimePlatform.OSXPlayer)
		{
			buffer = 0.5f;
		}
		if (Application.platform == RuntimePlatform.OSXEditor)
		{
			buffer = 0.5f;
		}
		if (Application.platform == RuntimePlatform.LinuxPlayer)
		{
			buffer = 0.5f;
		}
		if (Application.platform == RuntimePlatform.LinuxEditor)
		{
			buffer = 0.5f;
		}
		if (ADOBase.uiController != null)
		{
			txtOffset = ADOBase.uiController.txtOffset;
		}
		if (scnEditor.instance == null)
		{
			scnEditor.instance = editorComponent;
		}
		if (scnEditor.instance == null)
		{
			scnCLS.instance = CLSComponent;
		}
		if (CustomLevel.instance == null)
		{
			CustomLevel.instance = customLevelComponent;
		}
		if (ADOBase.controller != null)
		{
			ADOBase.controller.SetupImportantVariables();
		}
	}

	public void Start()
	{
		crotchet = 60f / bpm;
		crotchetAtStart = crotchet;
		AudioSource[] components = GetComponents<AudioSource>();
		song = components[0];
		if (components.Length > 1)
		{
			song2 = components[1];
		}
		nextBeatTime = 0.0;
		nextBarTime = 0.0;
		if (txtOffset != null)
		{
			txtOffset.text = "";
		}
		lastReportedPlayheadPosition = AudioSettings.dspTime;
		dspTime = AudioSettings.dspTime;
		previousFrameTime = Time.unscaledTime;
		if (song.pitch == 0f && !ADOBase.isLevelEditor)
		{
			UnityEngine.Debug.LogError("Song pitch is zero set to zero?!");
		}
		if (ADOBase.controller != null)
		{
			ADOBase.controller.startVolume = song.volume;
		}
		RDUtils.SetMixerVolume("WorldVolume", worldBalanceVolume);
	}

	public void Rewind()
	{
		isGameWorld = true;
		crotchet = 0.0;
		nextBeatTime = 0.0;
		nextBarTime = 0.0;
		beatNumber = 0;
		barNumber = 0;
		hasSongStarted = false;
		getSpectrum = false;
		txtOffset = null;
		dspTimeSong = 0.0;
		lastHit = 0.0;
		lastReportedPlayheadPosition = AudioSettings.dspTime;
		dspTime = AudioSettings.dspTime;
		previousFrameTime = Time.unscaledTime;
	}

	public void SetupConductorWithLevelData(LevelData levelData)
	{
		bpm = levelData.bpm;
		crotchet = 60f / bpm;
		crotchetAtStart = crotchet;
		addoffset = (float)levelData.offset * 0.001f;
		song.volume = (float)levelData.volume * 0.01f;
		hitSoundVolume = (float)levelData.hitsoundVolume * 0.01f;
		hitSound = levelData.hitsound;
		separateCountdownTime = levelData.separateCountdownTime;
		float num = (float)levelData.pitch * 0.01f;
		if (GCS.standaloneLevelMode)
		{
			num *= GCS.currentSpeedTrial;
		}
		else if (ADOBase.isEditingLevel)
		{
			num *= ADOBase.editor.playbackSpeed;
		}
		song.pitch = num;
	}

	public void PlayHitTimes()
	{
		if (playedHitSounds)
		{
			AudioManager.Instance.StopAllSounds();
		}
		playedHitSounds = true;
		if (ADOBase.sceneName.Contains("scnCalibration") || ADOBase.lm == null || !GCS.d_hitsounds || (ADOBase.controller != null && !ADOBase.isLevelEditor && !forceHitSounds))
		{
			return;
		}
		int num = (GCS.checkpointNum >= ADOBase.lm.listFloors.Count) ? 1 : (GCS.checkpointNum + 1);
		int num2 = GCS.practiceMode ? (GCS.checkpointNum + GCS.practiceLength) : (ADOBase.lm.listFloors.Count - 1);
		extraTicksCountdown.Clear();
		extraTicksCountdown = new List<ExtraTickData>();
		nextExtraTickToSchedule = 0;
		for (int i = 1; i < ADOBase.lm.listFloors.Count - 1; i++)
		{
			if (ADOBase.lm.listFloors[i].countdownTicks <= 0)
			{
				continue;
			}
			new List<double>();
			for (int j = 0; j < ADOBase.lm.listFloors[i].countdownTicks; j++)
			{
				double num3 = dspTimeSong + ADOBase.lm.listFloors[i + 1].entryTimePitchAdj - (double)(ADOBase.lm.listFloors[i].countdownTicks - j) * (crotchet / (double)ADOBase.lm.listFloors[i].speed) / (double)song.pitch / (double)countdownSpeedMultiplier + addoffset / (double)song.pitch;
				if (i >= num && i < num2 && num3 > dspTime)
				{
					extraTicksCountdown.Add(new ExtraTickData(num3, ADOBase.lm.listFloors[i].countdownTicks - (j + 1), ADOBase.lm.listFloors[i].speed));
				}
			}
		}
		HitSound hitSound = this.hitSound;
		float volume = hitSoundVolume;
		List<scrFloor> listFloors = ADOBase.lm.listFloors;
		int num4 = (GCS.checkpointNum >= listFloors.Count) ? 1 : (GCS.checkpointNum + 1);
		int num5 = GCS.practiceMode ? (GCS.checkpointNum + GCS.practiceLength) : (listFloors.Count - 1);
		double num6 = dspTimeSong + addoffset / (double)song.pitch;
		hitSoundsData = new List<HitSoundsData>();
		nextHitSoundToSchedule = 0;
		midspinHitSound = hitSound;
		for (int k = 1; k < listFloors.Count; k++)
		{
			scrFloor scrFloor = listFloors[k];
			ffxSetHitsound setHitsound = scrFloor.setHitsound;
			if (setHitsound != null)
			{
				if (setHitsound.gameSound == GameSound.Midspin)
				{
					useMidspinHitSound = true;
					midspinHitSound = setHitsound.hitSound;
				}
				else
				{
					hitSound = setHitsound.hitSound;
					if (!hasSetMidspinHitsound)
					{
						hasSetMidspinHitsound = true;
						midspinHitSound = hitSound;
					}
				}
				volume = setHitsound.volume;
			}
			if (ADOBase.lm.listFloors[k].holdLength > -1 || ADOBase.lm.listFloors[k - 1].holdLength > -1 || (ADOBase.lm.listFloors[k - 1].midSpin && ADOBase.lm.listFloors[k - 2].holdLength > -1))
			{
				continue;
			}
			scrFloor scrFloor2 = listFloors[k - 1];
			double value = 0.0;
			HitSound key = (scrFloor2 != null && scrFloor2.midSpin && useMidspinHitSound) ? midspinHitSound : hitSound;
			ADOBase.gc.hitSoundOffsets.TryGetValue(key, out value);
			double num7 = dspTimeSongPosZero + scrFloor.entryTimePitchAdj - value;
			if (k >= num4 && k <= num5 && num7 > dspTime && !scrFloor.midSpin && hitSound != HitSound.None)
			{
				HitSound hitSound2 = (scrFloor2 != null && scrFloor2.midSpin && useMidspinHitSound) ? midspinHitSound : hitSound;
				if (scrFloor.tapsNeeded > 1)
				{
					hitSound2 = midspinHitSound;
				}
				hitSoundsData.Add(new HitSoundsData(hitSound2, num7, volume));
			}
			if (scrFloor.freeroam && (scrFloor.freeroamSoundOnBeat != HitSound.None || scrFloor.freeroamSoundOffBeat != HitSound.None))
			{
				float extraBeats = scrFloor.extraBeats;
				HitSound freeroamSoundOnBeat = scrFloor.freeroamSoundOnBeat;
				HitSound freeroamSoundOffBeat = scrFloor.freeroamSoundOffBeat;
				float[] array = new float[4]
				{
					0.8f,
					0.7f,
					0.8f,
					0.7f
				};
				int num8 = -1;
				for (float num9 = 0f; num9 < extraBeats - (float)scrFloor.countdownTicks / countdownSpeedMultiplier + 1f; num9 += 0.5f)
				{
					num8 = (num8 + 1) % 4;
					double num10 = (double)num9 * (scrMisc.bpm2crotchet(bpm) / (double)scrFloor.speed) / (double)song.pitch;
					if (((num8 % 2 == 0 && scrFloor.freeroamSoundOnBeat != HitSound.None) || (num8 % 2 == 1 && scrFloor.freeroamSoundOffBeat != HitSound.None)) && k >= num4 && k <= num5 && num7 + num10 > dspTime && !scrFloor.midSpin)
					{
						hitSoundsData.Add(new HitSoundsData((num8 % 2 == 0) ? freeroamSoundOnBeat : freeroamSoundOffBeat, num7 + num10, volume * array[num8]));
					}
				}
			}
			double num11 = num6 + scrFloor.entryTimePitchAdj;
			if (k >= num4 && num11 > dspTime && (bool)scrFloor.prevfloor && scrFloor.numPlanets != scrFloor.prevfloor.numPlanets)
			{
				HitSound hitSound3 = (scrFloor.numPlanets > scrFloor.prevfloor.numPlanets) ? HitSound.VehiclePositive : HitSound.VehicleNegative;
				HitSoundsData item = new HitSoundsData(hitSound3, num11, volume * 0.6f);
				hitSoundsData.Add(item);
			}
		}
		HoldStartSound holdStartSound = this.holdStartSound;
		HoldEndSound holdEndSound = this.holdEndSound;
		HoldLoopSound holdLoopSound = this.holdLoopSound;
		HoldMidSound holdMidSound = this.holdMidSound;
		HoldMidSoundType holdMidSoundType = this.holdMidSoundType;
		float num12 = holdMidSoundDelay;
		HoldMidSoundTimingRelativeTo holdMidSoundTimingRelativeTo = holdMidSoundTiming;
		float volume2 = holdSoundVolume;
		int num13 = (GCS.checkpointNum >= ADOBase.lm.listFloors.Count) ? 1 : GCS.checkpointNum;
		int num14 = GCS.practiceMode ? (GCS.checkpointNum + GCS.practiceLength) : (listFloors.Count - 1);
		holdSoundsData = new List<HoldSoundsData>();
		nextHoldSoundToSchedule = 0;
		for (int l = 1; l < ADOBase.lm.listFloors.Count - 1; l++)
		{
			ffxSetHoldsound component = ADOBase.lm.listFloors[l].GetComponent<ffxSetHoldsound>();
			if (component != null)
			{
				holdStartSound = component.holdStartSound;
				holdEndSound = component.holdEndSound;
				holdLoopSound = component.holdLoopSound;
				holdMidSound = component.holdMidSound;
				holdMidSoundType = component.holdMidSoundType;
				num12 = component.holdMidSoundDelay;
				holdMidSoundTimingRelativeTo = component.holdMidSoundTiming;
				volume2 = component.volume;
			}
			if (ADOBase.lm.listFloors[l].holdLength == -1)
			{
				continue;
			}
			int holdLength = ADOBase.lm.listFloors[l].holdLength;
			double num15 = ADOBase.lm.listFloors[l].exitangle + (double)((float)holdLength * MathF.PI * 2f) - ADOBase.lm.listFloors[l].entryangle;
			if (ADOBase.lm.listFloors[l].entryangle > ADOBase.lm.listFloors[l].exitangle - 0.05000000074505806)
			{
				num15 += 6.2831854820251465;
			}
			double num16 = 0.0;
			num16 = ((holdStartSound != 0) ? 0.0 : 0.0);
			double num17 = dspTimeSong + ADOBase.lm.listFloors[l].entryTimePitchAdj - num16 + addoffset / (double)song.pitch;
			double endTime = -1.0;
			if (l >= num13 && num17 > dspTime && !ADOBase.lm.listFloors[l].midSpin && l < num14 && holdStartSound.ToString() != "None")
			{
				HoldSoundsData item2 = new HoldSoundsData("sndHeldbeatStart" + holdStartSound.ToString(), num17, endTime, volume2);
				holdSoundsData.Add(item2);
			}
			num16 = ((holdLoopSound != 0) ? 0.0 : 0.0);
			num17 = dspTimeSong + ADOBase.lm.listFloors[l].entryTimePitchAdj - num16 + addoffset / (double)song.pitch;
			endTime = dspTimeSong + ADOBase.lm.listFloors[l + 1].entryTimePitchAdj + addoffset / (double)song.pitch;
			if (l >= num13 && num17 > dspTime && !ADOBase.lm.listFloors[l].midSpin && l < num14 && holdLoopSound.ToString() != "None")
			{
				HoldSoundsData item3 = new HoldSoundsData("sndHeldbeatLoop" + holdLoopSound.ToString(), num17, endTime, volume2);
				holdSoundsData.Add(item3);
			}
			switch (holdMidSound)
			{
			case HoldMidSound.Fuse:
				num16 = 0.107;
				break;
			case HoldMidSound.SingSing:
				num16 = 0.212;
				break;
			default:
				num16 = 0.0;
				break;
			}
			if (holdMidSound.ToString() != "None")
			{
				float num18 = num12 / song.pitch;
				if (holdMidSoundType == HoldMidSoundType.Once)
				{
					num17 = dspTimeSong + ADOBase.lm.listFloors[l].entryTimePitchAdj - num16 + addoffset / (double)song.pitch;
					endTime = dspTimeSong + ADOBase.lm.listFloors[l + 1].entryTimePitchAdj - num16 + addoffset / (double)song.pitch;
					double num19 = (holdMidSoundTimingRelativeTo == HoldMidSoundTimingRelativeTo.Start) ? (num17 + (double)num18) : (endTime - (double)num18);
					if (l >= num13 && num19 > num17 && num19 < endTime && num19 > dspTime && !ADOBase.lm.listFloors[l].midSpin && l < num14)
					{
						HoldSoundsData item4 = new HoldSoundsData("sndHeldbeatMid" + holdMidSound.ToString(), num19, -1.0, volume2);
						holdSoundsData.Add(item4);
					}
				}
				else
				{
					num17 = dspTimeSong + ADOBase.lm.listFloors[l].entryTimePitchAdj - num16 + addoffset / (double)song.pitch;
					endTime = dspTimeSong + ADOBase.lm.listFloors[l + 1].entryTimePitchAdj - num16 + addoffset / (double)song.pitch;
					if (num18 >= 0f && holdMidSoundTimingRelativeTo == HoldMidSoundTimingRelativeTo.Start)
					{
						for (double num20 = num17 + (double)num18; num20 < endTime; num20 += (double)num18)
						{
							if (l >= num13 && num20 > num17 && num20 < endTime && num20 > dspTime && l < num14)
							{
								HoldSoundsData item5 = new HoldSoundsData("sndHeldbeatMid" + holdMidSound.ToString(), num20, -1.0, volume2);
								holdSoundsData.Add(item5);
							}
						}
					}
					else if (num18 >= 0f && holdMidSoundTimingRelativeTo == HoldMidSoundTimingRelativeTo.End)
					{
						int count = holdSoundsData.Count;
						for (double num21 = endTime - (double)num18; num21 > num17; num21 -= (double)num18)
						{
							if (l >= num13 && num21 > num17 && num21 < endTime && num21 > dspTime && l < num14)
							{
								HoldSoundsData item6 = new HoldSoundsData("sndHeldbeatMid" + holdMidSound.ToString(), num21, -1.0, volume2);
								holdSoundsData.Insert(count, item6);
							}
						}
					}
				}
			}
			num16 = ((holdEndSound != 0) ? 0.0 : 0.099);
			num17 = dspTimeSong + ADOBase.lm.listFloors[l + 1].entryTimePitchAdj - num16 + addoffset / (double)song.pitch;
			endTime = -1.0;
			if (l >= num13 && num17 > dspTime && !ADOBase.lm.listFloors[l].midSpin && l < num14 && holdEndSound.ToString() != "None")
			{
				HoldSoundsData item7 = new HoldSoundsData("sndHeldbeatEnd" + holdEndSound.ToString(), num17, endTime, volume2);
				holdSoundsData.Add(item7);
			}
		}
		hasSetMidspinHitsound = false;
		if (!playCountdownHihats)
		{
			return;
		}
		if (!fastTakeoff)
		{
			countdownTimes = new double[countdownTicks];
			for (int m = 0; m < countdownTicks; m++)
			{
				double countdownTime = GetCountdownTime(m);
				if (countdownTime > dspTime)
				{
					countdownTimes[m] = countdownTime;
					AudioManager.Play("sndHat", countdownTime, hitSoundGroup, hitSoundVolume, 10);
				}
			}
		}
		if (playEndingCymbal)
		{
			AudioManager.Play("sndCymbalCrash", dspTimeSong + ADOBase.lm.listFloors[num5].entryTimePitchAdj + addoffset / (double)song.pitch, hitSoundGroup, hitSoundVolume, 10);
		}
	}

	public double GetCountdownTime(int i)
	{
		if (GCS.checkpointNum != 0)
		{
			int index = (GCS.checkpointNum != ADOBase.lm.listFloors.Count - 1) ? (GCS.checkpointNum + 1) : GCS.checkpointNum;
			return dspTimeSong + ADOBase.lm.listFloors[index].entryTimePitchAdj - (double)(countdownTicks - i) * (crotchet / ((double)(ADOBase.lm.listFloors[GCS.checkpointNum].speed * (float)ADOBase.lm.listFloors[GCS.checkpointNum].numPlanets) * 0.5)) / (double)song.pitch / (double)countdownSpeedMultiplier + addoffset / (double)song.pitch;
		}
		return dspTimeSong + (double)i * crotchet / (double)song.pitch / (double)countdownSpeedMultiplier + addoffset / (double)song.pitch;
	}

	public void StartMusic(Action onComplete = null, Action onSongScheduled = null)
	{
		if (startMusicCoroutine != null)
		{
			StopCoroutine(startMusicCoroutine);
		}
		startMusicCoroutine = StartCoroutine(StartMusicCo(onComplete, onSongScheduled));
	}

	private IEnumerator StartMusicCo(Action onComplete, Action onSongScheduled = null)
	{
		if (!isGameWorld)
		{
			nextBeatTime = 0.0;
		}
		dspTimeSong = dspTime + (double)buffer + 0.10000000149011612;
		if (!GCS.d_oldConductor)
		{
			for (float timer = 0.1f; timer >= 0f; timer -= Time.deltaTime)
			{
				yield return 0;
			}
			dspTimeSong = dspTime + (double)buffer;
			if (fastTakeoff)
			{
				dspTimeSong -= (double)adjustedCountdownTicks * crotchetAtStart / (double)song.pitch;
			}
			song.UnPause();
			skipOffset = (Persistence.GetSkipIntroAfterFirstTry() && scrController.deaths > 0 && addoffset > GCS.longIntroThresholdSec && !RDC.auto);
			if (dspTimeSong - addoffset / (double)song.pitch + (separateCountdownTime ? (crotchet / (double)song.pitch * (double)adjustedCountdownTicks) : 0.0) > dspTime + (double)buffer)
			{
				skipOffset = false;
			}
			if (skipOffset)
			{
				dspTimeSong -= addoffset / (double)song.pitch;
			}
			double num = dspTimeSong + (separateCountdownTime ? (crotchet / (double)song.pitch * (double)adjustedCountdownTicks) : 0.0);
			double num2 = dspTime + (double)buffer;
			if (skipOffset)
			{
				song.PlayScheduled(num2);
				song.time = ((float)num2 - (float)num) * song.pitch;
			}
			else
			{
				song.PlayScheduled(num);
				song.time = 0f;
			}
			if (song2 != null)
			{
				song2.PlayScheduled(num);
			}
			StartCoroutine(ToggleHasSongStarted(dspTimeSong));
		}
		else
		{
			song.volume = 0f;
			song.Play();
			printe("is playing: " + song.time.ToString());
			while (!song.isPlaying)
			{
				UnityEngine.Debug.Log("song is not playing right after play called");
				yield return null;
			}
			yield return new WaitForSeconds(0.2f);
			song.Pause();
			yield return new WaitForSeconds(0.1f);
			song.UnPause();
			song.volume = 1f;
		}
		if (GCS.checkpointNum == 0)
		{
			PlayHitTimes();
		}
		yield return 0;
		onSongScheduled?.Invoke();
		yield return new WaitForSeconds(4f);
		while (song.isPlaying)
		{
			yield return null;
		}
		onComplete?.Invoke();
	}

	private IEnumerator ToggleHasSongStarted(double songstarttime)
	{
		if (GCS.d_webglConductor)
		{
			song.volume = 0f;
		}
		while (instance.dspTime < songstarttime)
		{
			yield return null;
		}
		hasSongStarted = true;
		scrDebugHUDMessage.Log("Song started forreal!");
		if (GCS.d_webglConductor)
		{
			yield return new WaitForSeconds(0.2f);
			song.Pause();
			yield return new WaitForSeconds(0.1f);
			song.UnPause();
			song.volume = 1f;
		}
	}

	private void Update()
	{
		if (!AudioListener.pause && Application.isFocused && (double)Time.unscaledTime - previousFrameTime < 0.10000000149011612)
		{
			double num = (double)Time.unscaledTime - previousFrameTime;
			dspTime += num;
			if (AsyncInputManager.isActive)
			{
				AsyncInputManager.dspTime += (double)Time.unscaledTime - AsyncInputManager.previousFrameTime;
			}
		}
		previousFrameTime = Time.unscaledTime;
		if (AudioSettings.dspTime != lastReportedPlayheadPosition)
		{
			dspTime = AudioSettings.dspTime;
			lastReportedPlayheadPosition = AudioSettings.dspTime;
		}
		if (AsyncInputManager.isActive)
		{
			AsyncInputManager.prevFrameTick = AsyncInputManager.currFrameTick;
			AsyncInputManager.currFrameTick = DateTime.Now.Ticks;
			if (!AudioListener.pause && Application.isFocused && (double)Time.unscaledTime - AsyncInputManager.previousFrameTime < 0.1)
			{
				AsyncInputManager.dspTime += (double)Time.unscaledTime - AsyncInputManager.previousFrameTime;
			}
			AsyncInputManager.previousFrameTime = Time.unscaledTime;
			if (AudioSettings.dspTime - AsyncInputManager.lastReportedDspTime != 0.0)
			{
				AsyncInputManager.lastReportedDspTime = AudioSettings.dspTime;
				AsyncInputManager.dspTime = AudioSettings.dspTime;
				AsyncInputManager.offsetTick = AsyncInputManager.currFrameTick - (long)(AsyncInputManager.dspTime * 10000000.0);
				AsyncInputManager.offsetTickUpdated = true;
			}
			AsyncInputManager.dspTimeSong = dspTimeSong;
			if (ADOBase.controller != null)
			{
				ADOBase.controller.UpdateInput();
			}
		}
		if (hasSongStarted && isGameWorld)
		{
			States state = ADOBase.controller.state;
			if (state != States.Fail && state != States.Fail2)
			{
				while (nextExtraTickToSchedule < extraTicksCountdown.Count)
				{
					double time = extraTicksCountdown[nextExtraTickToSchedule].time;
					if (!(dspTime + 5.0 > time))
					{
						break;
					}
					AudioManager.Play("sndHat", time, hitSoundGroup, hitSoundVolume, 10);
					nextExtraTickToSchedule++;
				}
				while (nextHoldSoundToSchedule < this.holdSoundsData.Count)
				{
					HoldSoundsData holdSoundsData = this.holdSoundsData[nextHoldSoundToSchedule];
					if (!(dspTime + 5.0 > holdSoundsData.time))
					{
						break;
					}
					if (holdSoundsData.endTime > 0.0)
					{
						PlayWithEndTime(holdSoundsData.name, holdSoundsData.time, holdSoundsData.endTime, holdSoundsData.volume);
					}
					else
					{
						AudioManager.Play(holdSoundsData.name, holdSoundsData.time, hitSoundGroup, holdSoundsData.volume);
					}
					nextHoldSoundToSchedule++;
				}
				while (nextHitSoundToSchedule < this.hitSoundsData.Count)
				{
					HitSoundsData hitSoundsData = this.hitSoundsData[nextHitSoundToSchedule];
					if (hitSoundsData.time >= dspTime + 5.0)
					{
						break;
					}
					AudioManager.Play("snd" + hitSoundsData.hitSound.ToString(), hitSoundsData.time, hitSoundGroup, hitSoundsData.volume);
					nextHitSoundToSchedule++;
				}
			}
		}
		crotchet = 60f / bpm;
		double songposition_minusi = this.songposition_minusi;
		if (!GCS.d_oldConductor && !GCS.d_webglConductor)
		{
			this.songposition_minusi = (double)((float)(dspTime - dspTimeSong - (double)calibration_i) * song.pitch) - addoffset;
		}
		else
		{
			this.songposition_minusi = (double)(song.time - calibration_i) - addoffset / (double)song.pitch;
		}
		deltaSongPos = this.songposition_minusi - songposition_minusi;
		deltaSongPos = Math.Max(deltaSongPos, 0.0);
		if (this.songposition_minusi > nextBeatTime)
		{
			OnBeat();
			nextBeatTime += crotchet;
			beatNumber++;
		}
		if (this.songposition_minusi > nextBarTime)
		{
			nextBarTime += crotchet * (double)crotchetsPerBar;
			barNumber++;
		}
		if (UnityEngine.Input.GetKeyDown(KeyCode.G) && Application.isEditor)
		{
			float time2 = song.time;
			if (separateCountdownTime)
			{
				double crotchetAtStart2 = crotchetAtStart;
				float adjustedCountdownTick = adjustedCountdownTicks;
			}
			double songposition_minusi2 = this.songposition_minusi;
			float calibration_i = calibration_i;
			float pitch = song.pitch;
			double addoffset2 = addoffset;
		}
		if (GCS.d_calibration)
		{
			bool num2 = ADOBase.editor == null || (ADOBase.editor != null && !ADOBase.controller.paused);
		}
		if (!getSpectrum || GCS.lofiVersion)
		{
			return;
		}
		AudioSource audioSource = song;
		if (CLSComponent != null)
		{
			PreviewSongPlayer previewSongPlayer = CLSComponent.previewSongPlayer;
			if (previewSongPlayer.playing)
			{
				audioSource = previewSongPlayer.audioSource;
			}
		}
		audioSource.GetSpectrumData(spectrum, 0, FFTWindow.BlackmanHarris);
	}

	public override void OnBeat()
	{
		List<ADOBase> onBeats = this.onBeats;
		if (onBeats == null)
		{
			return;
		}
		int count = onBeats.Count;
		for (int i = 0; i < count; i++)
		{
			onBeats[i].OnBeat();
		}
		if (ADOBase.controller != null && ADOBase.controller.gameworld)
		{
			scrLevelMaker lm = ADOBase.lm;
			List<scrFloor> listFloors = ADOBase.lm.listFloors;
			int count2 = listFloors.Count;
			for (int j = 0; j < count2; j++)
			{
				listFloors[j].OnBeat();
			}
		}
		onBeatFrame = Time.frameCount;
	}

	public void ScrubMusicToTile(int tileID)
	{
		AudioListener.pause = true;
		AudioManager.Instance.StopAllSounds();
		song.SetScheduledStartTime(dspTime);
		double num = separateCountdownTime ? (crotchetAtStart * (double)adjustedCountdownTicks) : 0.0;
		song.time = (float)(ADOBase.lm.listFloors[tileID].entryTime + addoffset - num);
		dspTimeSong = dspTime - ADOBase.lm.listFloors[tileID].entryTimePitchAdj - addoffset / (double)song.pitch;
		lastHit = ADOBase.lm.listFloors[tileID].entryTime;
		StartCoroutine(DesyncFix());
	}

	public void ScrubMusicToTime(double newTime)
	{
		double num = newTime / (double)song.pitch;
		AudioListener.pause = true;
		AudioManager.Instance.StopAllSounds();
		song.SetScheduledStartTime(dspTime);
		double num2 = separateCountdownTime ? (crotchetAtStart * (double)countdownTicks) : 0.0;
		song.time = (float)(newTime + addoffset - num2);
		dspTimeSong = dspTime - num - addoffset / (double)song.pitch;
		lastHit = newTime;
		StartCoroutine(DesyncFix());
	}

	private IEnumerator DesyncFix()
	{
		for (int framecounty = 2; framecounty > 0; framecounty--)
		{
			yield return 0;
		}
		AudioListener.pause = false;
		PlayHitTimes();
		int numberOfAttempts = 0;
		int framesToWait = 10;
		double maxDifference = 0.005;
		for (int i = 0; i < numberOfAttempts; i++)
		{
			for (int framecount = framesToWait; framecount > 0; framecount--)
			{
				yield return 0;
			}
			if (song.isPlaying || song.clip == null)
			{
				break;
			}
			double num = (double)song.time + (separateCountdownTime ? (crotchetAtStart * (double)adjustedCountdownTicks) : 0.0);
			double num2 = songposition_minusi + (double)(calibration_i * song.pitch) + addoffset;
			if (Math.Abs(num2 - num) > maxDifference)
			{
				double num3 = num - num2;
				UnityEngine.Debug.Log("Desync Fix Attempt: found difference " + num3.ToString());
				UnityEngine.Debug.Log("Attempt " + i.ToString());
				UnityEngine.Debug.Log("song time " + num.ToString());
				UnityEngine.Debug.Log("dsptime " + num2.ToString());
				dspTimeSong -= num3;
				PlayHitTimes();
			}
		}
	}

	public static AudioOutputType GetCurrentAudioOutputType()
	{
		return AudioOutputType.Speaker;
	}

	public static string GetCurrentAudioOutputName()
	{
		return "*";
	}

	public static void UpdateCurrentAudioOutput()
	{
		currentPreset = GetSuitablePresetForCurrentAudioOutput();
	}

	public static CalibrationPreset GetSuitablePresetForCurrentAudioOutput(bool searchOnlyForDefaultPresets = false)
	{
		AudioOutputType currentAudioOutputType = GetCurrentAudioOutputType();
		string currentAudioOutputName = GetCurrentAudioOutputName();
		if (!searchOnlyForDefaultPresets)
		{
			foreach (CalibrationPreset userPreset in userPresets)
			{
				if (userPreset.outputType == currentAudioOutputType)
				{
					bool flag = userPreset.outputType == AudioOutputType.Bluetooth || userPreset.outputType == AudioOutputType.Other;
					if ((flag && userPreset.outputName == currentAudioOutputName) || !flag)
					{
						CalibrationPreset result = userPreset;
						result.confident = true;
						return result;
					}
				}
			}
		}
		List<CalibrationPreset> list = new List<CalibrationPreset>();
		foreach (CalibrationPreset defaultPreset in defaultPresets)
		{
			if (defaultPreset.outputType == currentAudioOutputType || defaultPreset.outputType == AudioOutputType.Any)
			{
				list.Add(defaultPreset);
			}
		}
		foreach (CalibrationPreset item in from preset in list
			orderby preset.priority descending
			select preset)
		{
			if (CalibrationPreset.StringMatchesFilter(currentAudioOutputName, item.outputName))
			{
				CalibrationPreset result2 = item;
				result2.outputType = currentAudioOutputType;
				result2.outputName = currentAudioOutputName;
				return result2;
			}
		}
		UnityEngine.Debug.LogError("We didn't find a suitable preset, this should never happen because all fallbacks should be in the default presets file.");
		CalibrationPreset result3 = default(CalibrationPreset);
		result3.outputType = currentAudioOutputType;
		result3.outputName = currentAudioOutputName;
		result3.inputOffset = 100;
		result3.confident = false;
		result3.priority = -1;
		return result3;
	}

	public static void UsePreset(CalibrationPreset preset)
	{
		RDBaseDll.printem(currentPreset);
		currentPreset = preset;
	}

	public static bool HasAudioOutputChanged()
	{
		AudioOutputType currentAudioOutputType = GetCurrentAudioOutputType();
		if (currentAudioOutputType != currentPreset.outputType)
		{
			return true;
		}
		if ((currentAudioOutputType == AudioOutputType.Bluetooth || currentAudioOutputType == AudioOutputType.Other) && GetCurrentAudioOutputName() != currentPreset.outputName)
		{
			return true;
		}
		return false;
	}

	private int GetOffsetChange(bool fine)
	{
		if (!fine)
		{
			return 10;
		}
		return 1;
	}

	public void ReduceInputOffset(bool fine = false)
	{
		currentPreset.inputOffset -= GetOffsetChange(fine);
		SaveCurrentPreset();
	}

	public void IncreaseInputOffset(bool fine = false)
	{
		currentPreset.inputOffset += GetOffsetChange(fine);
		SaveCurrentPreset();
	}

	public void ReduceVisualOffset(bool fine = false)
	{
		visualOffset -= GetOffsetChange(fine);
		SaveVisualOffset(calibration_v);
	}

	public void IncreaseVisualOffset(bool fine = false)
	{
		visualOffset += GetOffsetChange(fine);
		SaveVisualOffset(calibration_v);
	}

	public static void SaveCurrentPreset()
	{
		for (int i = 0; i < userPresets.Count; i++)
		{
			CalibrationPreset calibrationPreset = userPresets[i];
			if (currentPreset.outputType == calibrationPreset.outputType && currentPreset.outputName == calibrationPreset.outputName)
			{
				calibrationPreset.inputOffset = currentPreset.inputOffset;
				userPresets[i] = calibrationPreset;
				return;
			}
		}
		CalibrationPreset calibrationPreset2 = currentPreset;
		RDBaseDll.printem("adding preset: " + calibrationPreset2.ToString());
		userPresets.Add(currentPreset);
	}

	public void DuckSongStart(float duckFactor = 0.25f, float fadeLength = 0.1f)
	{
		if (duckState != DuckState.Starting && duckState != DuckState.Ducked)
		{
			if (duckState == DuckState.None)
			{
				songPreduckVolume = song.volume;
				song2PreduckVolume = song2.volume;
			}
			duckState = DuckState.Starting;
			RDBaseDll.printem("Ducking!");
			float volume = song.volume;
			float volume2 = song2.volume;
			float num = (songPreduckVolume - volume) / (songPreduckVolume * (1f - duckFactor));
			float num2 = (song2PreduckVolume - volume2) / (song2PreduckVolume * (1f - duckFactor));
			song.DOKill();
			song2.DOKill();
			song.DOFade(songPreduckVolume * duckFactor, fadeLength * num).SetEase(Ease.OutExpo).OnComplete(delegate
			{
				DuckSongDucked();
			});
			song2.DOFade(song2PreduckVolume * duckFactor, fadeLength * num2).SetEase(Ease.OutExpo);
		}
	}

	public void DuckSongDucked()
	{
		if (duckState == DuckState.Starting)
		{
			duckState = DuckState.Ducked;
		}
		RDBaseDll.printem("Ducked!");
	}

	public void DuckSongStop(float fadeLength = 1f)
	{
		if (duckState != 0 && duckState != DuckState.Stopping)
		{
			duckState = DuckState.Stopping;
			RDBaseDll.printem("Duck recovering!");
			song.DOFade(songPreduckVolume, fadeLength).SetEase(Ease.OutExpo).OnComplete(delegate
			{
				DuckSongFinish();
			});
			song2.DOFade(song2PreduckVolume, fadeLength).SetEase(Ease.OutExpo);
		}
	}

	public void DuckSongFinish()
	{
		if (duckState == DuckState.Stopping)
		{
			duckState = DuckState.None;
		}
		RDBaseDll.printem("Duck recovered!");
	}

	private void SaveVisualOffset(double offset)
	{
		Persistence.SetVisualOffset((float)offset);
		PlayerPrefs.SetFloat("offset_v", (float)offset);
		PlayerPrefs.Save();
	}

	public static string GetDeviceInfo()
	{
		return "Device Info:\n" + $"Platform: {ADOBase.platform}\n" + "OS: " + SystemInfo.operatingSystem + "\nDevice Model: " + SystemInfo.deviceModel + "\n" + $"Output Type: {currentPreset.outputType}\n" + "Output Name: " + currentPreset.outputName + "\n" + $"Input Offset: {currentPreset.inputOffset}\n" + $"One-line:\n{ADOBase.platform},{SystemInfo.operatingSystem},{SystemInfo.deviceModel},{currentPreset.outputType},{currentPreset.outputName},{currentPreset.inputOffset}";
	}

	public void LoadOnBeats()
	{
		if (_onBeats.Count != 0)
		{
			return;
		}
		_onBeats = new List<ADOBase>();
		GameObject[] array = GameObject.FindGameObjectsWithTag("Beat");
		for (int i = 0; i < array.Length; i++)
		{
			ADOBase component = array[i].GetComponent<ADOBase>();
			if (component != null && (bool)component)
			{
				_onBeats.Add(component);
			}
		}
	}

	public void PlayWithEndTime(string snd, double time, double endTime, float volume = 1f, int priority = 128)
	{
		AudioSource audioSource = AudioManager.Instance.MakeSource(snd);
		audioSource.loop = true;
		audioSource.pitch = 1f;
		audioSource.volume = volume;
		audioSource.priority = priority;
		audioSource.PlayScheduled(time);
		audioSource.SetScheduledEndTime(endTime);
	}

	public void KillAllSounds()
	{
		StartCoroutine(KillAllSoundsCoroutine());
	}

	private IEnumerator KillAllSoundsCoroutine()
	{
		AudioManager.Instance.StopAllSounds();
		yield return null;
		yield return null;
		yield return null;
		AudioManager.Instance.StopAllSounds();
	}
}
