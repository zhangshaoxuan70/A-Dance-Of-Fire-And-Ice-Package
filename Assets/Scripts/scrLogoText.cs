using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class scrLogoText : MonoBehaviour
{
	public static scrLogoText instance;

	public Image fireImage;

	public Image fireLight;

	public Image iceImage;

	public Image iceLight;

	public bool logoIsChinese;

	private Sequence fireRainbowSeq;

	private Sequence fireLightRainbowSeq;

	private Sequence iceRainbowSeq;

	private Sequence iceLightRainbowSeq;

	private void Awake()
	{
		SystemLanguage language = RDString.language;
		bool flag = language == SystemLanguage.Chinese || language == SystemLanguage.ChineseSimplified || language == SystemLanguage.ChineseTraditional;
		if ((flag && logoIsChinese) || (!flag && !logoIsChinese))
		{
			instance = this;
		}
		UpdateColors();
	}

	private Color LoadLogoColor(bool isRed)
	{
		Color color = Persistence.GetPlayerColor(isRed);
		if (color == scrPlanet.goldColor)
		{
			color = new Color(1f, 0.8078431f, 0.1607843f);
		}
		else if (color == scrPlanet.transBlueColor)
		{
			color = new Color(0.3607843f, 67f / 85f, 0.9294118f);
		}
		else if (color == scrPlanet.transPinkColor)
		{
			color = new Color(0.9568627f, 164f / 255f, 0.7098039f);
		}
		else if (color == scrPlanet.nbYellowColor)
		{
			color = new Color(0.996f, 0.953f, 0.18f);
		}
		else if (color == scrPlanet.nbPurpleColor)
		{
			color = new Color(0.612f, 0.345f, 0.82f);
		}
		else if (color == scrPlanet.overseerColor)
		{
			color = new Color(0.1058824f, 0.6470588f, 0.7843137f);
		}
		return color;
	}

	public void UpdateColors()
	{
		Color col = LoadLogoColor(isRed: false);
		Color col2 = LoadLogoColor(isRed: true);
		ColorLogo(col, isRed: false);
		ColorLogo(col2, isRed: true);
	}

	public void ColorLogo(Color col, bool isRed)
	{
		Color.RGBToHSV(col, out float H, out float S, out float V);
		Color color = Color.HSVToRGB(H, S * 0.6f, V);
		bool flag = col == scrPlanet.rainbowColor;
		if (isRed)
		{
			if (fireRainbowSeq != null)
			{
				fireRainbowSeq.Kill();
			}
			if (fireLightRainbowSeq != null)
			{
				fireLightRainbowSeq.Kill();
			}
			if (flag)
			{
				fireRainbowSeq = MakeRainbow(fireImage);
				fireLightRainbowSeq = MakeRainbow(fireLight);
			}
			else
			{
				fireImage.color = color.WithAlpha(fireImage.color.a);
				fireLight.color = color.WithAlpha(fireLight.color.a);
			}
		}
		else
		{
			if (iceRainbowSeq != null)
			{
				iceRainbowSeq.Kill();
			}
			if (iceLightRainbowSeq != null)
			{
				iceLightRainbowSeq.Kill();
			}
			if (flag)
			{
				iceRainbowSeq = MakeRainbow(iceImage);
				iceLightRainbowSeq = MakeRainbow(iceLight);
			}
			else
			{
				iceImage.color = color.WithAlpha(iceImage.color.a);
				iceLight.color = color.WithAlpha(iceLight.color.a);
			}
		}
	}

	private Sequence MakeRainbow(Image img)
	{
		Color red = Color.red;
		Color.RGBToHSV(Color.red, out float _, out float S, out float V);
		Tween[] array = new Tween[10];
		Sequence rainbowSeq = DOTween.Sequence();
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = DOVirtual.Float(0.1f * (float)i, 0.1f * (float)(i + 1), 0.5f, delegate(float x)
			{
				if (img != null)
				{
					img.color = Color.HSVToRGB(0.1f + x, S, V).WithAlpha(img.color.a);
				}
				else
				{
					rainbowSeq.Kill();
				}
			});
			rainbowSeq.Append(array[i]);
		}
		rainbowSeq.SetLoops(-1, LoopType.Restart).SetUpdate(isIndependentUpdate: true);
		return rainbowSeq;
	}
}
