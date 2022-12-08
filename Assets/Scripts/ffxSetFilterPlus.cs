using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.ImageEffects;

public class ffxSetFilterPlus : ffxPlusBase
{
	public Filter filter;

	public bool enableFilter;

	public bool disableOthers;

	public float intensity;

	public bool dontDisable;

	private Dictionary<Filter, MonoBehaviour> filterToComp => vfx.filterToComp;

	private Dictionary<Filter, float> filterCurrIntensity => vfx.filterCurrIntensity;

	private Dictionary<Filter, Tween> filterTween => vfx.filterTween;

	protected override IEnumerable<Tween> eventTweens
	{
		get
		{
			if (filterTween.ContainsKey(filter))
			{
				return new Tween[1]
				{
					filterTween[filter]
				};
			}
			return null;
		}
	}

	public override void Awake()
	{
		base.Awake();
		hifiEffect = true;
		disableIfMinFx = !dontDisable;
	}

	public override void StartEffect()
	{
		AdjustDurationForHardbake();
		if (filterTween.ContainsKey(filter))
		{
			filterTween[filter].Kill(complete: true);
			filterTween.Remove(filter);
		}
		if (duration == 0f)
		{
			filterCurrIntensity[filter] = intensity;
			SetFilter(filter, enableFilter, intensity);
		}
		else
		{
			filterTween[filter] = DOTween.To(() => filterCurrIntensity[filter], delegate(float i)
			{
				filterCurrIntensity[filter] = i;
				SetFilter(filter, enableFilter, filterCurrIntensity[filter]);
			}, intensity, duration).SetEase(ease);
		}
		if (disableOthers)
		{
			foreach (Filter key in filterToComp.Keys)
			{
				if (key != filter && filterToComp[key] != null)
				{
					DOTween.Kill(key, complete: true);
					filterToComp[key].enabled = false;
				}
			}
		}
	}

	private void SetFilter(Filter f, bool fEnable, float fIntensity)
	{
		if (!filterToComp.ContainsKey(f))
		{
			return;
		}
		MonoBehaviour monoBehaviour = filterToComp[f];
		if (!ADOBase.isMobile || !(monoBehaviour is CameraMotionBlur))
		{
			monoBehaviour.enabled = fEnable;
		}
		if (!fEnable)
		{
			return;
		}
		switch (f)
		{
		case Filter.Invert:
		case Filter.EightiesTV:
		case Filter.FiftiesTV:
		case Filter.Arcade:
		case Filter.Glitch:
		case Filter.Neon:
		case Filter.Handheld:
		case Filter.NightVision:
		case Filter.Funk:
		case Filter.Tunnel:
		case Filter.Weird3D:
		case Filter.EdgeBlackLine:
		case Filter.SuperDot:
			break;
		case Filter.VHS:
			(monoBehaviour as CameraFilterPack_Real_VHS).TRACKING = 0.212f * fIntensity;
			break;
		case Filter.LED:
			(monoBehaviour as CameraFilterPack_TV_LED).Size = Mathf.RoundToInt(5f * fIntensity);
			break;
		case Filter.Drawing:
			(monoBehaviour as CameraFilterPack_Drawing_Paper).Fade_With_Original = Mathf.Clamp(fIntensity, 0f, 1f);
			break;
		case Filter.Compression:
			(monoBehaviour as CameraFilterPack_TV_CompressionFX).Parasite = 3f * fIntensity;
			break;
		case Filter.Waves:
			(monoBehaviour as CameraFilterPack_Distortion_Wave_Horizontal).WaveIntensity = 10f * fIntensity;
			CameraFilterPack_Distortion_Wave_Horizontal.ChangeWaveIntensity = 10f * fIntensity;
			break;
		case Filter.Pixelate:
			(monoBehaviour as CameraFilterPack_Pixel_Pixelisation)._Pixelisation = 4f * fIntensity;
			CameraFilterPack_Pixel_Pixelisation.ChangePixel = 4f * fIntensity;
			break;
		case Filter.Rain:
			(monoBehaviour as CameraFilterPack_Atmosphere_Rain).Intensity = 0.5f * fIntensity;
			break;
		case Filter.Blizzard:
			(monoBehaviour as CameraFilterPack_Blizzard)._Speed = fIntensity;
			break;
		case Filter.PixelSnow:
			(monoBehaviour as CameraFilterPack_Atmosphere_Snow_8bits).Threshold = 0.9f + 0.1f * fIntensity;
			break;
		case Filter.Static:
			(monoBehaviour as CameraFilterPack_Noise_TV).Fade = fIntensity;
			CameraFilterPack_Noise_TV.ChangeValue = fIntensity;
			break;
		case Filter.Grain:
			(monoBehaviour as CameraFilterPack_Film_Grain).Value = 32f * fIntensity;
			CameraFilterPack_Film_Grain.ChangeValue = 32f * fIntensity;
			break;
		case Filter.MotionBlur:
			if (!ADOBase.isMobile)
			{
				(monoBehaviour as CameraMotionBlur).velocityScale = 0.375f * fIntensity;
			}
			break;
		case Filter.Fisheye:
			(monoBehaviour as CameraFilterPack_Distortion_FishEye).Distortion = fIntensity;
			CameraFilterPack_Distortion_FishEye.ChangeDistortion = fIntensity;
			break;
		case Filter.Aberration:
			(monoBehaviour as CameraFilterPack_Color_Chromatic_Aberration).Offset = fIntensity * 0.04f - 0.02f;
			CameraFilterPack_Color_Chromatic_Aberration.ChangeOffset = fIntensity * 0.04f - 0.02f;
			break;
		case Filter.Sepia:
			(monoBehaviour as CameraFilterPack_Color_Sepia).Intensity = fIntensity;
			break;
		case Filter.Grayscale:
			(monoBehaviour as CameraFilterPack_Color_GrayScale).Intensity = fIntensity;
			break;
		case Filter.HexagonBlack:
			(monoBehaviour as CameraFilterPack_FX_Hexagon_Black).Value = Mathf.Max(0.2f, fIntensity);
			CameraFilterPack_FX_Hexagon_Black.ChangeValue = Mathf.Max(0.2f, fIntensity);
			break;
		case Filter.Posterize:
			(monoBehaviour as CameraFilterPack_TV_Posterize).Posterize = fIntensity * 20f;
			CameraFilterPack_TV_Posterize.ChangePosterize = fIntensity * 20f;
			break;
		case Filter.Sharpen:
			(monoBehaviour as CameraFilterPack_Sharpen_Sharpen).Value2 = fIntensity;
			CameraFilterPack_Sharpen_Sharpen.ChangeValue2 = fIntensity;
			break;
		case Filter.Contrast:
			(monoBehaviour as CameraFilterPack_Color_Contrast).Contrast = fIntensity + 1f;
			CameraFilterPack_Color_Contrast.ChangeContrast = fIntensity + 1f;
			break;
		case Filter.OilPaint:
			(monoBehaviour as CameraFilterPack_Pixelisation_OilPaint).Value = fIntensity;
			CameraFilterPack_Pixelisation_OilPaint.ChangeValue = fIntensity;
			break;
		case Filter.Blur:
			(monoBehaviour as CameraFilterPack_Blur_Blurry).Amount = fIntensity * 2f;
			CameraFilterPack_Blur_Blurry.ChangeAmount = fIntensity * 2f;
			break;
		case Filter.BlurFocus:
			(monoBehaviour as CameraFilterPack_Blur_Focus)._Size = Mathf.Max(0.10001f, fIntensity);
			CameraFilterPack_Blur_Focus.ChangeSize = Mathf.Max(0.10001f, fIntensity);
			break;
		case Filter.GaussianBlur:
			(monoBehaviour as CameraFilterPack_Blur_GaussianBlur).Size = Mathf.Max(0.10001f, fIntensity);
			CameraFilterPack_Blur_GaussianBlur.ChangeSize = fIntensity;
			break;
		case Filter.WaterDrop:
			(monoBehaviour as CameraFilterPack_AAA_WaterDrop).Distortion = Mathf.Lerp(64f, 8f, Mathf.Clamp(fIntensity, 0f, 1f));
			CameraFilterPack_AAA_WaterDrop.ChangeDistortion = Mathf.Lerp(64f, 8f, Mathf.Clamp(fIntensity, 0f, 1f));
			break;
		case Filter.LightWater:
			(monoBehaviour as CameraFilterPack_Light_Water2).Intensity = fIntensity * 2.4f;
			CameraFilterPack_Light_Water2.ChangeValue4 = fIntensity * 2.4f;
			break;
		case Filter.Petals:
			(monoBehaviour as FallingPetals).TogglePetals(enabled: true, 6f);
			break;
		case Filter.PetalsInstant:
			(monoBehaviour as FallingPetals).TogglePetals(enabled: true, 28f);
			break;
		}
	}
}
