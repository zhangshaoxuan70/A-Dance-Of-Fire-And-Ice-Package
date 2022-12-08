using ADOFAI;
using System;
using UnityEngine;

public class scrCustomBackgroundSprite : ADOBase
{
	public SpriteRenderer displayedSprite;

	public Sprite baseSprite;

	public SpriteRenderer[] loopSprites = new SpriteRenderer[8];

	public Vector2[] loopCoords = new Vector2[8];

	public bool looping = true;

	public bool tiled;

	public bool fitScreen = true;

	public bool lockRot;

	[NonSerialized]
	public scrParallax parallax;

	[NonSerialized]
	public Vector2 imgSize = Vector2.zero;

	private scrExtImgHolder imgHolder;

	private Camera gameCam;

	private float baseWidth;

	private float baseHeight;

	private Vector2 baseSize;

	private void Awake()
	{
		displayedSprite = base.gameObject.GetComponent<SpriteRenderer>();
		gameCam = ADOBase.controller.camy.GetComponent<Camera>();
		parallax = base.gameObject.GetComponent<scrParallax>();
		imgHolder = ADOBase.customLevel.imgHolder;
		baseHeight = 2f * gameCam.orthographicSize;
		baseWidth = baseHeight * gameCam.aspect;
		baseSize = new Vector2(baseWidth, baseHeight);
	}

	private void LateUpdate()
	{
		if (displayedSprite.sprite == null)
		{
			return;
		}
		float num = 2f * gameCam.orthographicSize;
		float num2 = num * gameCam.aspect;
		Vector2 vector = new Vector2(num2, num);
		float num3 = fitScreen ? num2 : imgSize.x;
		float num4 = fitScreen ? num : imgSize.y;
		Vector2 vector2 = Vector2.zero;
		if (tiled)
		{
			int num5 = Mathf.CeilToInt(num2 / imgSize.x);
			if (num5 % 2 != 0)
			{
				num5++;
			}
			int num6 = Mathf.CeilToInt(num / imgSize.y);
			if (num6 % 2 != 0)
			{
				num6++;
			}
			vector2 = new Vector2((float)num5 * imgSize.x, (float)num6 * imgSize.y);
		}
		if (!tiled)
		{
			displayedSprite.size = (fitScreen ? vector : imgSize);
			for (int i = 0; i < loopSprites.Length; i++)
			{
				loopSprites[i].transform.localPosition = new Vector2(loopCoords[i].x * num3, loopCoords[i].y * num4);
				loopSprites[i].size = (fitScreen ? vector : imgSize);
			}
		}
		else
		{
			displayedSprite.size = (looping ? (3f * vector2) : vector2);
		}
		if (looping)
		{
			float num7 = Mathf.Approximately(parallax.multiplier_x, 0f) ? (-1f) : (1f / parallax.multiplier_x);
			float num8 = Mathf.Approximately(parallax.multiplier_y, 0f) ? (-1f) : (1f / parallax.multiplier_y);
			parallax.dontalter_x = (num7 == -1f);
			parallax.dontalter_y = (num8 == -1f);
			float num9 = 0f;
			float num10 = 0f;
			if (!tiled)
			{
				num9 = num3;
				num10 = num4;
			}
			else
			{
				num9 = vector2.x;
				num10 = vector2.y;
			}
			num9 *= base.transform.localScale.x;
			num10 *= base.transform.localScale.y;
			bool flag = false;
			bool flag2 = false;
			int num11 = 0;
			while (!flag || !flag2)
			{
				if (num11 >= 100)
				{
					UnityEngine.Debug.Log("Too many repeats! >:(");
					break;
				}
				float num12 = (gameCam.transform.position.x - base.gameObject.transform.position.x) / base.transform.localScale.x;
				float num13 = (gameCam.transform.position.y - base.gameObject.transform.position.y) / base.transform.localScale.y;
				if (num12 < 0f - num3)
				{
					if (num7 != -1f)
					{
						parallax.posCamAtStart += num7 * Vector3.right * num9;
					}
					else
					{
						base.gameObject.transform.position += Vector3.left * num9;
					}
				}
				else if (num12 > num3)
				{
					if (num7 != -1f)
					{
						parallax.posCamAtStart += num7 * Vector3.left * num9;
					}
					else
					{
						base.gameObject.transform.position += Vector3.right * num9;
					}
				}
				else
				{
					flag = true;
				}
				if (num13 < 0f - num4)
				{
					if (num8 != -1f)
					{
						parallax.posCamAtStart += num8 * Vector3.up * num10;
					}
					else
					{
						base.gameObject.transform.position += Vector3.down * num10;
					}
				}
				else if (num13 > num4)
				{
					if (num8 != -1f)
					{
						parallax.posCamAtStart += num8 * Vector3.down * num10;
					}
					else
					{
						base.gameObject.transform.position += Vector3.up * num10;
					}
				}
				else
				{
					flag2 = true;
				}
				Vector3 position = base.transform.position;
				parallax.SetTrans();
				Vector3 position2 = base.transform.position;
				if ((!flag || !flag2) && position2 == position)
				{
					break;
				}
				num11++;
			}
		}
		else
		{
			parallax.posCamAtStart = Vector3.zero;
		}
		if (lockRot)
		{
			base.gameObject.transform.rotation = gameCam.gameObject.transform.rotation;
		}
		else
		{
			base.gameObject.transform.rotation = Quaternion.identity;
		}
	}

	public void SetBaseSprite(string filePath, string filename)
	{
		baseSprite = imgHolder.AddSprite(filename, filePath, out LoadResult _);
	}

	public void SetCustomBG(Sprite sprite, Color color, bool tiledToggle = false, bool loopingToggle = true, bool fitScreenToggle = true, float scale = 1f, bool lockRotToggle = false)
	{
		base.transform.position = Vector3.zero;
		displayedSprite.sprite = sprite;
		displayedSprite.color = color;
		SpriteRenderer[] array = loopSprites;
		foreach (SpriteRenderer obj in array)
		{
			obj.sprite = sprite;
			obj.color = color;
		}
		if (sprite != null)
		{
			imgSize = new Vector2(sprite.rect.width / sprite.pixelsPerUnit, sprite.rect.height / sprite.pixelsPerUnit);
		}
		parallax.dontalter_x = false;
		parallax.dontalter_y = false;
		lockRot = lockRotToggle;
		ToggleTiledAndLooping(tiledToggle, loopingToggle, fitScreenToggle);
		SetScale(scale);
	}

	private void ToggleTiledAndLooping(bool tiledToggle, bool loopingToggle, bool fitScreenToggle)
	{
		tiled = tiledToggle;
		looping = loopingToggle;
		fitScreen = fitScreenToggle;
		displayedSprite.drawMode = ((!tiled) ? SpriteDrawMode.Sliced : SpriteDrawMode.Tiled);
		bool active = !tiledToggle && loopingToggle;
		SpriteRenderer[] array = loopSprites;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].gameObject.SetActive(active);
		}
	}

	private void SetScale(float scale)
	{
		if (fitScreen)
		{
			base.transform.localScale = Vector3.one;
		}
		else
		{
			base.transform.localScale = new Vector3(scale, scale, 1f);
		}
	}
}
