using DG.Tweening;
using UnityEngine;

public class scrVfx : ADOBase
{
	public ColourScheme currentColourScheme;

	public Color[] arrTileFlashColours;

	public TileFlashStyle tileFlashStyle;

	public bool overrideTileSprites;

	public TileOverrideStyle overrideStyle;

	public Sprite[] arrLitTiles;

	public Sprite unlitTile;

	public bool overrideGlowSprites;

	public Sprite[] arrTopGlowSprites;

	public bool overrideScale;

	public float bottomGlowScale = 1f;

	private static scrVfx _instance;

	private int startFrame;

	public static scrVfx instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = Object.FindObjectOfType<scrVfx>();
			}
			return _instance;
		}
	}

	private void Awake()
	{
		startFrame = Time.frameCount;
	}

	private void Start()
	{
		if (GCS.lofiVersion)
		{
			int num = 0;
			if (ADOBase.controller.gameworld)
			{
				num = scrController.currentWorld;
			}
			else
			{
				printe("uncomment the following line of code:");
			}
			ColourScheme[] worldColourScheme = RDConstants.data.worldColourScheme;
			if (num < worldColourScheme.Length)
			{
				currentColourScheme = worldColourScheme[num];
			}
			ADOBase.controller.camy.SetBgColour();
		}
	}

	private void Update()
	{
		int num = (!GCS.standaloneLevelMode) ? 1 : 4;
		if (ADOBase.controller.gameworld && Time.frameCount - startFrame == num)
		{
			scrUIController.instance.WipeFromBlack();
			DOTween.TweensByTarget(scrUIController.instance.transitionPanel.rectTransform);
		}
	}
}
