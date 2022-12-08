using System;
using UnityEngine;

public class ffxCustomBackgroundPlus : ffxPlusBase
{
	[NonSerialized]
	public scrCustomBackgroundSprite custBG;

	public Color color;

	public Color imageColor;

	public string filePath;

	public Camera bgCam;

	public Vector2 parallax;

	public bool tiled;

	public bool looping = true;

	public bool fitScreen = true;

	public bool lockRot;

	public float unscaledSize;

	private scrExtImgHolder imgHolder;

	public override void Awake()
	{
		base.Awake();
		custBG = ADOBase.customLevel.custBG;
		bgCam = cam.Bgcamstatic;
		imgHolder = ADOBase.customLevel.imgHolder;
	}

	public override void StartEffect()
	{
		CustomLevel instance = CustomLevel.instance;
		bgCam.backgroundColor = color;
		if (!string.IsNullOrEmpty(filePath))
		{
			scrExtImgHolder.CustomSprite customSprite = imgHolder.customSprites[filePath];
			if (!(customSprite.sprite == null))
			{
				custBG.SetCustomBG(customSprite.sprite, imageColor, tiled, looping, fitScreen, unscaledSize, lockRot);
				custBG.parallax.multiplier_x = parallax.x;
				custBG.parallax.multiplier_y = parallax.y;
				instance.editorBG.SetActive(value: false);
			}
		}
		else
		{
			custBG.SetCustomBG(null, Color.white);
			instance.editorBG.SetActive(!instance.videoBG.gameObject.activeSelf && instance.levelData.bgShowDefaultBGIfNoImage);
		}
	}
}
