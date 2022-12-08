using Rewired;
using System;
using System.Collections.Generic;
using System.Windows.Input;
using TMPro;
using UnityEngine;

[CreateAssetMenu(fileName = "RDConstants", menuName = "RhythmDoctor/Constants", order = 1)]
public class RDConstants : ScriptableObject
{
	[Serializable]
	public struct HitSoundOffset
	{
		public HitSound sound;

		public double offset;
	}

	private static RDConstants internalData;

	[Header("Tweaks")]
	public bool debug;

	public bool auto;

	public bool practice;

	public bool noHud;

	public bool forceMobile;

	public bool useOldAuto;

	public bool skipCutscenes;

	public bool customCheckpoint;

	public int customCheckpointPos;

	public bool forceNoSteamworks;

	public bool runningOnSteamDeck;

	public bool hideTaroGimmicks;

	public bool deleteSavedProgress;

	[Header("Build")]
	public string version;

	public bool macRetinaSupport;

	[Header("Localization")]
	public bool forceLanguage;

	public SystemLanguage forcedLanguage;

	[Header("Fonts")]
	public Font latinFont;

	public Font chineseFont;

	public Font koreanFont;

	public Font japaneseFont;

	public Font legacyFont;

	public TMP_FontAsset latinFontTMPro;

	public TMP_FontAsset chineseFontTMPro;

	public TMP_FontAsset chineseFontTMProDuplicate;

	public TMP_FontAsset koreanFontTMPro;

	public TMP_FontAsset japaneseFontTMPro;

	public TMP_FontAsset legacyFontTMPro;

	public Font editorFont;

	[Header("Other Fonts")]
	public Font arialFont;

	public Font comicSansMSFont;

	public Font courierNewFont;

	public Font georgiaFont;

	public Font impactFont;

	public Font timesNewRomanFont;

	[Header("Game Prefabs")]
	public InputManager prefab_rewiredManager;

	public GameObject prefab_errorCanvas;

	public GameObject prefab_sfxHandler;

	public GameObject prefab_butterfly;

	public GameObject prefab_notification;

	public GameObject prefab_PA_diamond;

	public GameObject prefab_PA_circle;

	public GameObject prefab_PA_heart;

	public GameObject prefab_MD_ghostRing;

	public GameObject prefab_MD_ghostPopup;

	public GameObject prefab_MD_ghostPopupL;

	public GameObject prefab_options_shape;

	public GameObject prefab_ML_window;

	public GameObject prefab_ML_torch;

	public GameObject prefab_lanternWithChain;

	public GameObject prefab_petals;

	public GameObject prefab_petalsInstant;

	public GameObject prefab_topGlow;

	public GameObject prefab_bottomGlow;

	public GameObject prefab_floorIcon;

	[Header("Level Editor Prefabs")]
	public GameObject prefab_propertiesPanel;

	public GameObject prefab_property;

	public GameObject prefab_controlText;

	public GameObject prefab_controlLongText;

	public GameObject prefab_controlEnum;

	public GameObject prefab_controlDropdown;

	public GameObject prefab_controlColor;

	public GameObject prefab_controlToggle;

	public GameObject prefab_controlBrowse;

	public GameObject prefab_controlVector2;

	public GameObject prefab_controlTile;

	public GameObject prefab_controlExport;

	public GameObject prefab_controlRating;

	public GameObject prefab_controlList;

	public GameObject prefab_controlListItem;

	public GameObject prefab_tab;

	public GameObject prefab_cycleButtons;

	public GameObject prefab_eventIndicator;

	[Header("Sprites")]
	public Sprite sprite_star_empty;

	public Sprite sprite_star_half;

	public Sprite sprite_star_full;

	public Sprite sprite_defaultPortal;

	public Texture2D tex_defaultIcon;

	public Sprite sprite_options_circle;

	public Sprite sprite_options_square;

	public Sprite sprite_options_cross;

	public Sprite sprite_options_plus;

	public Sprite sprite_halloween_lantern_small;

	public Sprite sprite_halloween_lantern_big;

	public Sprite sprite_cny_lantern;

	public Sprite[] halloweenLanternSprites;

	public Sprite[] CNYLanternSprites;

	public Sprite[] bullseyeSprites;

	public Sprite[] faceBaseSprites;

	public Sprite[] faceBaseDetailSprites;

	[Header("Colors")]
	public Color autoplayOn;

	public Color autoplayOff;

	public Color difficultyEasy;

	public Color difficultyMedium;

	public Color difficultyHard;

	public Color difficultyHardest;

	public Color allowedColor;

	public Color particallyDeclinedColor;

	public Color declinedColor;

	public Color goldTextColor;

	public Color portalColor;

	public Color speedPortalColor;

	public Color goldExplosionColor1;

	public Color goldExplosionColor2;

	public Color overseerExplosionColor1;

	public Color overseerExplosionColor2;

	public NamedColor[] planetColors;

	public ColourSchemeHitMargin hitMarginColours;

	public ColourSchemeHitMargin hitMarginColoursUI;

	[Header("Shaders")]
	public Shader overlayShader;

	public Shader tileBlurShader;

	[Header("Textures")]
	public Texture2D tex_perlinDefault;

	public Texture2D tex_perlinNone;

	public Texture2D tex_floorTileNone;

	public Texture2D tex_floorShadowDefault;

	public Texture2D tex_floorShadowGlow;

	public Texture2D tex_floorShadowNone;

	public Texture2D tex_floorEdgeDefault;

	public Texture2D tex_floorTileDefault;

	public Texture2D tex_floorEdgeBasic;

	public Texture2D tex_floorEdgeNeon;

	public Texture2D tex_floorEdgeNeon2;

	public Texture2D tex_floorEdgeNeonLight;

	public Texture2D tex_floorEdgeMinimal;

	public Texture2D tex_gem;

	public Texture2D tex_clear;

	[Header("Audio")]
	public AudioClip halloweenMusic;

	public AudioClip[] soundEffects;

	[Header("Tile Sprites")]
	public Sprite[] rabbitSpritesArr;

	public Sprite[] snailSpritesArr;

	public Sprite sprPortal;

	public Sprite sprIconRabbit;

	public Sprite sprIconRabbitGray;

	public Sprite sprIconDoubleRabbit;

	public Sprite sprIconSnail;

	public Sprite sprIconSnailGray;

	public Sprite sprIconDoubleSnail;

	public Sprite sprIconSwirlBlue;

	public Sprite sprIconSwirlRed;

	public Sprite sprIconSameSpeed;

	public Sprite sprIconVfx;

	public Sprite sprIconCheckpoint;

	public Sprite sprIconCheckpointLit;

	public GameObject prefabLetterPress;

	[Header("Tile Outlines")]
	public Sprite sprOutlineRabbit;

	public Sprite sprOutlineDoubleRabbit;

	public Sprite sprOutlineSnail;

	public Sprite sprOutlineDoubleSnail;

	public Sprite sprOutlineSwirl;

	public Sprite sprOutlineSameSpeed;

	public Sprite sprOutlineVfx;

	public Sprite sprOutlineCheckpoint;

	[Header("Controller Prefabs")]
	public GameObject canvasPrefab;

	public GameObject hitTextPrefab;

	public GameObject animatedPortalPrefab;

	public GameObject errorMeterPrefab;

	public GameObject missIndicator;

	public GameObject failIndicator;

	[Header("Materials")]
	public Material floorMeshDefault;

	public Material floorSpriteDefault;

	public HitSoundOffset[] hitSoundOffsetsArray;

	public Dictionary<HitSound, double> hitSoundOffsets = new Dictionary<HitSound, double>();

	[Header("Web Version Stuff")]
	public ColourScheme[] worldColourScheme;

	[Header("Other Constants")]
	public Shader blurEffectConeTap;

	public ulong[] featuredWorkshopLevels;

	[Header("Mobile Menu")]
	public GameObject prefab_worldPortal;

	public GameObject prefab_titleScreen;

	public GameObject prefab_colorsScreen;

	public GameObject prefab_creditsScreen;

	public Sprite sprSpeedTrialButtonOn;

	public Sprite sprSpeedTrialButtonOff;

	[Header("DLC")]
	public GameObject collider180;

	public Sprite sprIconHoldArrowLong;

	public Sprite sprIconHoldArrowShort;

	public Sprite sprIconHoldReleaseLong;

	public Sprite sprIconHoldReleaseShort;

	public Sprite sprIconMultiPlanetTwo;

	public Sprite sprIconMultiPlanetThreeMore;

	public Sprite sprIconMultiPlanetThreeLess;

	public Sprite sprOutlineHoldArrowLong;

	public Sprite sprOutlineHoldArrowShort;

	public Sprite sprOutlineHoldReleaseLong;

	public Sprite sprOutlineHoldReleaseShort;

	public Sprite sprOutlineMultiPlanetTwo;

	public Sprite sprOutlineMultiPlanetThreeMore;

	public Sprite sprOutlineMultiPlanetThreeLess;

	public Texture2D planetPolygonTex;

	public Shader holdShader;

	public static RDConstants data
	{
		get
		{
			if (internalData == null)
			{
				internalData = Resources.Load<RDConstants>("RDConstants");
				internalData.Setup();
				internalData.debug = false;
				internalData.auto = false;
				internalData.forceMobile = false;
				internalData.forceLanguage = false;
				internalData.noHud = false;
				internalData.useOldAuto = false;
			}
			return internalData;
		}
	}

	public Color GetPlanetColor(PlanetColor color)
	{
		for (int i = 0; i < planetColors.Length; i++)
		{
			NamedColor namedColor = planetColors[i];
			if (color == namedColor.name)
			{
				return namedColor.color;
			}
		}
		UnityEngine.Debug.LogError("PlanetColor color not found " + color.ToString() + ", returning gray.");
		return Color.gray;
	}

	public void Setup()
	{
		if (Application.isPlaying)
		{
			HitSoundOffset[] array = hitSoundOffsetsArray;
			foreach (HitSoundOffset hitSoundOffset in array)
			{
				internalData.hitSoundOffsets.Add(hitSoundOffset.sound, hitSoundOffset.offset);
			}
		}
	}
}
