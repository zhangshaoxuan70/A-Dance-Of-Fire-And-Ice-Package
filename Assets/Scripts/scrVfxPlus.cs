using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Video;
using UnityStandardAssets.ImageEffects;

public class scrVfxPlus : ADOBase
{
	public VideoPlayer videoBG;

	public float vidOffset;

	[NonSerialized]
	public List<ffxPlusBase> effects;

	[NonSerialized]
	public scrVolumeTrackerFloat vTrackerFloat;

	public Dictionary<Filter, MonoBehaviour> filterToComp = new Dictionary<Filter, MonoBehaviour>();

	public Dictionary<Filter, float> filterCurrIntensity = new Dictionary<Filter, float>();

	public Dictionary<Filter, Tween> filterTween = new Dictionary<Filter, Tween>();

	private scrConductor cond;

	private scrController ctrl;

	private scrCamera cam;

	private int currentVfxIndex;

	[NonSerialized]
	public bool hasPlayed;

	private float _camAngle;

	private static scrVfxPlus _instance;

	public float camAngle
	{
		get
		{
			return _camAngle;
		}
		set
		{
			_camAngle = value;
			cam.transform.rotation = Quaternion.Euler(0f, 0f, value);
		}
	}

	public static scrVfxPlus instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = UnityEngine.Object.FindObjectOfType<scrVfxPlus>();
			}
			return _instance;
		}
	}

	private void Awake()
	{
		cond = scrConductor.instance;
		ctrl = scrController.instance;
		cam = UnityEngine.Object.FindObjectOfType<scrCamera>();
		vTrackerFloat = GetComponent<scrVolumeTrackerFloat>();
		effects = new List<ffxPlusBase>();
		if (ADOBase.customLevel != null)
		{
			videoBG = ADOBase.customLevel.videoBG;
		}
		else if (videoBG != null && videoBG.clip != null && Persistence.GetVisualQuality() == VisualQuality.High)
		{
			videoBG.gameObject.SetActive(value: true);
			videoBG.Stop();
			videoBG.Prepare();
		}
		MakeNewFilterDictionary();
	}

	public void MakeNewFilterDictionary()
	{
		filterToComp.Add(Filter.Grayscale, _003CMakeNewFilterDictionary_003Eg__GetFilter_007C20_0<CameraFilterPack_Color_GrayScale>());
		filterToComp.Add(Filter.Sepia, _003CMakeNewFilterDictionary_003Eg__GetFilter_007C20_0<CameraFilterPack_Color_Sepia>());
		filterToComp.Add(Filter.Invert, _003CMakeNewFilterDictionary_003Eg__GetFilter_007C20_0<CameraFilterPack_Color_Invert>());
		filterToComp.Add(Filter.VHS, _003CMakeNewFilterDictionary_003Eg__GetFilter_007C20_0<CameraFilterPack_Real_VHS>());
		filterToComp.Add(Filter.EightiesTV, _003CMakeNewFilterDictionary_003Eg__GetFilter_007C20_0<CameraFilterPack_TV_80>());
		filterToComp.Add(Filter.FiftiesTV, _003CMakeNewFilterDictionary_003Eg__GetFilter_007C20_0<CameraFilterPack_TV_50>());
		filterToComp.Add(Filter.Arcade, _003CMakeNewFilterDictionary_003Eg__GetFilter_007C20_0<CameraFilterPack_TV_ARCADE>());
		filterToComp.Add(Filter.LED, _003CMakeNewFilterDictionary_003Eg__GetFilter_007C20_0<CameraFilterPack_TV_LED>());
		filterToComp.Add(Filter.Rain, _003CMakeNewFilterDictionary_003Eg__GetFilter_007C20_0<CameraFilterPack_Atmosphere_Rain>());
		filterToComp.Add(Filter.Blizzard, _003CMakeNewFilterDictionary_003Eg__GetFilter_007C20_0<CameraFilterPack_Blizzard>());
		filterToComp.Add(Filter.PixelSnow, _003CMakeNewFilterDictionary_003Eg__GetFilter_007C20_0<CameraFilterPack_Atmosphere_Snow_8bits>());
		filterToComp.Add(Filter.Compression, _003CMakeNewFilterDictionary_003Eg__GetFilter_007C20_0<CameraFilterPack_TV_CompressionFX>());
		filterToComp.Add(Filter.Glitch, _003CMakeNewFilterDictionary_003Eg__GetFilter_007C20_0<CameraFilterPack_FX_Glitch1>());
		filterToComp.Add(Filter.Pixelate, _003CMakeNewFilterDictionary_003Eg__GetFilter_007C20_0<CameraFilterPack_Pixel_Pixelisation>());
		filterToComp.Add(Filter.Waves, _003CMakeNewFilterDictionary_003Eg__GetFilter_007C20_0<CameraFilterPack_Distortion_Wave_Horizontal>());
		filterToComp.Add(Filter.Static, _003CMakeNewFilterDictionary_003Eg__GetFilter_007C20_0<CameraFilterPack_Noise_TV>());
		filterToComp.Add(Filter.Grain, _003CMakeNewFilterDictionary_003Eg__GetFilter_007C20_0<CameraFilterPack_Film_Grain>());
		filterToComp.Add(Filter.MotionBlur, _003CMakeNewFilterDictionary_003Eg__GetFilter_007C20_0<CameraMotionBlur>());
		filterToComp.Add(Filter.Blur, _003CMakeNewFilterDictionary_003Eg__GetFilter_007C20_0<CameraFilterPack_Blur_Blurry>());
		filterToComp.Add(Filter.BlurFocus, _003CMakeNewFilterDictionary_003Eg__GetFilter_007C20_0<CameraFilterPack_Blur_Focus>());
		filterToComp.Add(Filter.GaussianBlur, _003CMakeNewFilterDictionary_003Eg__GetFilter_007C20_0<CameraFilterPack_Blur_GaussianBlur>());
		filterToComp.Add(Filter.Fisheye, _003CMakeNewFilterDictionary_003Eg__GetFilter_007C20_0<CameraFilterPack_Distortion_FishEye>());
		filterToComp.Add(Filter.Aberration, _003CMakeNewFilterDictionary_003Eg__GetFilter_007C20_0<CameraFilterPack_Color_Chromatic_Aberration>());
		filterToComp.Add(Filter.Drawing, _003CMakeNewFilterDictionary_003Eg__GetFilter_007C20_0<CameraFilterPack_Drawing_Paper>());
		filterToComp.Add(Filter.Neon, _003CMakeNewFilterDictionary_003Eg__GetFilter_007C20_0<CameraFilterPack_Edge_Neon>());
		filterToComp.Add(Filter.HexagonBlack, _003CMakeNewFilterDictionary_003Eg__GetFilter_007C20_0<CameraFilterPack_FX_Hexagon_Black>());
		filterToComp.Add(Filter.Posterize, _003CMakeNewFilterDictionary_003Eg__GetFilter_007C20_0<CameraFilterPack_TV_Posterize>());
		filterToComp.Add(Filter.Sharpen, _003CMakeNewFilterDictionary_003Eg__GetFilter_007C20_0<CameraFilterPack_Sharpen_Sharpen>());
		filterToComp.Add(Filter.Contrast, _003CMakeNewFilterDictionary_003Eg__GetFilter_007C20_0<CameraFilterPack_Color_Contrast>());
		filterToComp.Add(Filter.EdgeBlackLine, _003CMakeNewFilterDictionary_003Eg__GetFilter_007C20_0<CameraFilterPack_Edge_BlackLine>());
		filterToComp.Add(Filter.OilPaint, _003CMakeNewFilterDictionary_003Eg__GetFilter_007C20_0<CameraFilterPack_Pixelisation_OilPaint>());
		filterToComp.Add(Filter.SuperDot, _003CMakeNewFilterDictionary_003Eg__GetFilter_007C20_0<CameraFilterPack_FX_superDot>());
		filterToComp.Add(Filter.WaterDrop, _003CMakeNewFilterDictionary_003Eg__GetFilter_007C20_0<CameraFilterPack_AAA_WaterDrop>());
		filterToComp.Add(Filter.LightWater, _003CMakeNewFilterDictionary_003Eg__GetFilter_007C20_0<CameraFilterPack_Light_Water2>());
		filterToComp.Add(Filter.Handheld, _003CMakeNewFilterDictionary_003Eg__GetFilter_007C20_0<CameraFilterPack_FX_8bits_gb>());
		filterToComp.Add(Filter.NightVision, _003CMakeNewFilterDictionary_003Eg__GetFilter_007C20_0<CameraFilterPack_Oculus_NightVision1>());
		filterToComp.Add(Filter.Funk, _003CMakeNewFilterDictionary_003Eg__GetFilter_007C20_0<CameraFilterPack_FX_Funk>());
		filterToComp.Add(Filter.Tunnel, _003CMakeNewFilterDictionary_003Eg__GetFilter_007C20_0<PolarScreen>());
		filterToComp.Add(Filter.Weird3D, _003CMakeNewFilterDictionary_003Eg__GetFilter_007C20_0<CameraFilterPack_TV_Video3D>());
		filterToComp.Add(Filter.Petals, _003CMakeNewFilterDictionary_003Eg__GetFilter_007C20_0<FallingPetals>());
		filterToComp.Add(Filter.PetalsInstant, _003CMakeNewFilterDictionary_003Eg__GetFilter_007C20_0<FallingPetals>());
	}

	public void Reset()
	{
		currentVfxIndex = 0;
		camAngle = 0f;
		effects.Clear();
		hasPlayed = false;
		filterTween.Clear();
		ResetFilterIntensityDefaults();
	}

	private void ResetFilterIntensityDefaults()
	{
		foreach (Filter value in Enum.GetValues(typeof(Filter)))
		{
			filterCurrIntensity[value] = 0f;
		}
		filterCurrIntensity[Filter.Aberration] = 0.5f;
		filterCurrIntensity[Filter.Blizzard] = 1f;
		filterCurrIntensity[Filter.Fisheye] = 0.5f;
		filterCurrIntensity[Filter.LED] = 1f;
		filterCurrIntensity[Filter.Pixelate] = 0.01f;
	}

	private void Update()
	{
		if (!(!ctrl.paused & cond.hasSongStarted))
		{
			return;
		}
		VisualQuality visualQuality = (ADOBase.isLevelEditor && !GCS.standaloneLevelMode) ? VisualQuality.High : ADOBase.controller.visualQuality;
		VisualEffects visualEffects = ADOBase.isLevelEditor ? VisualEffects.Full : ADOBase.controller.visualEffects;
		int num = 0;
		while (currentVfxIndex < effects.Count)
		{
			ffxPlusBase ffxPlusBase = effects[currentVfxIndex];
			if (!ffxPlusBase.triggered)
			{
				if (cond.songposition_minusi < ffxPlusBase.startTime - ffxPlusBase.startEffectOffset)
				{
					break;
				}
				if ((visualQuality == VisualQuality.High || !ffxPlusBase.hifiEffect) && (!ffxPlusBase.disableIfMinFx || visualEffects != 0) && (!ffxPlusBase.disableIfMaxFx || visualEffects != VisualEffects.Full) && (!GCS.practiceMode || ADOBase.controller.currentState < States.Fail))
				{
					ffxPlusBase.StartEffect();
				}
				ffxPlusBase.triggered = true;
			}
			currentVfxIndex++;
			num++;
			if (num > 1000000)
			{
				printe("this stuff went crazy, abort");
				break;
			}
		}
		if (!(videoBG != null) || !videoBG.gameObject.activeSelf || videoBG.isPlaying || hasPlayed || !videoBG.isPrepared)
		{
			return;
		}
		double num2 = cond.separateCountdownTime ? (cond.crotchetAtStart * (double)ADOBase.conductor.adjustedCountdownTicks) : 0.0;
		if (cond.songposition_minusi >= num2 - (double)vidOffset)
		{
			bool num3 = videoBG.transform.GetComponent<scrDisableIfMinimumVFX>() != null;
			bool flag = Persistence.GetVisualQuality() == VisualQuality.Low && ADOBase.sceneName == "MO-X";
			if (num3 && ((ADOBase.controller.visualEffects == VisualEffects.Minimum) | flag))
			{
				videoBG.Stop();
			}
			else
			{
				videoBG.Play();
			}
			hasPlayed = true;
			videoBG.playbackSpeed = cond.song.pitch;
			double time = cond.songposition_minusi - num2 + (double)vidOffset;
			videoBG.time = time;
			if (ADOBase.customLevel != null)
			{
				ADOBase.customLevel.editorBG.SetActive(value: false);
			}
		}
	}

	public void ScrubToTime(float t)
	{
		VisualQuality visualQuality = ((bool)ADOBase.editor && !GCS.standaloneLevelMode) ? VisualQuality.High : ADOBase.controller.visualQuality;
		VisualEffects visualEffects = ADOBase.editor ? VisualEffects.Full : ADOBase.controller.visualEffects;
		List<ffxPlusBase> list = new List<ffxPlusBase>();
		foreach (ffxPlusBase effect in effects)
		{
			if (!effect.triggered && (visualQuality == VisualQuality.High || !effect.hifiEffect) && (!effect.disableIfMinFx || visualEffects != 0) && (!effect.disableIfMaxFx || visualEffects != VisualEffects.Full))
			{
				effect.ScrubToTime(t);
			}
			if (effect.triggered)
			{
				list.Add(effect);
			}
		}
		foreach (ffxPlusBase item in list)
		{
			effects.Remove(item);
		}
	}

	[CompilerGenerated]
	private static T _003CMakeNewFilterDictionary_003Eg__GetFilter_007C20_0<T>()
	{
		return scrCamera.instance.GetComponent<T>();
	}
}
