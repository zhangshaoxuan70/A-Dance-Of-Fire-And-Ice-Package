using System;
using UnityEngine;
using UnityEngine.UI;

public class CUIColorPicker : MonoBehaviour
{
	public GameObject satvalGO;

	public GameObject satvalKnob;

	public GameObject hueGO;

	public GameObject hueKnob;

	public GameObject result;

	public Image resultImage;

	public Color startColor;

	public AlphaSlider alphaSlider;

	public Color[] hueColors = new Color[6]
	{
		Color.red,
		new Color(1f, 1f, 0f, 1f),
		Color.green,
		Color.cyan,
		Color.blue,
		Color.magenta
	};

	private Color _color;

	private Action<Color> _onValueChange;

	private Action _update;

	public Color Color
	{
		get
		{
			return _color;
		}
		set
		{
			Setup(value);
		}
	}

	public void SetOnValueChangeCallback(Action<Color> onValueChange)
	{
		_onValueChange = onValueChange;
	}

	private static void RGBToHSV(Color color, out float h, out float s, out float v)
	{
		float num = Mathf.Min(color.r, color.g, color.b);
		float num2 = Mathf.Max(color.r, color.g, color.b);
		float num3 = num2 - num;
		if (num3 == 0f)
		{
			h = 0f;
		}
		else if (num2 == color.r)
		{
			h = Mathf.Repeat((color.g - color.b) / num3, 6f);
		}
		else if (num2 == color.g)
		{
			h = (color.b - color.r) / num3 + 2f;
		}
		else
		{
			h = (color.r - color.g) / num3 + 4f;
		}
		s = ((num2 == 0f) ? 0f : (num3 / num2));
		v = num2;
	}

	private static bool GetLocalMouse(GameObject go, out Vector2 result)
	{
		RectTransform rectTransform = (RectTransform)go.transform;
		Vector3 vector = rectTransform.InverseTransformPoint(UnityEngine.Input.mousePosition);
		result.x = Mathf.Clamp(vector.x, rectTransform.rect.min.x, rectTransform.rect.max.x);
		result.y = Mathf.Clamp(vector.y, rectTransform.rect.min.y, rectTransform.rect.max.y);
		return rectTransform.rect.Contains(vector);
	}

	private static Vector2 GetWidgetSize(GameObject go)
	{
		return ((RectTransform)go.transform).rect.size;
	}

	private void Setup(Color inputColor)
	{
		Color[] satvalColors = new Color[4]
		{
			new Color(0f, 0f, 0f, 1f),
			new Color(0f, 0f, 0f, 1f),
			new Color(1f, 1f, 1f, 1f),
			hueColors[0]
		};
		Texture2D texture2D = new Texture2D(1, 7);
		for (int i = 0; i < 7; i++)
		{
			texture2D.SetPixel(0, i, hueColors[i % 6]);
		}
		texture2D.Apply();
		hueGO.GetComponent<Image>().sprite = Sprite.Create(texture2D, new Rect(0f, 0.5f, 1f, 6f), new Vector2(0.5f, 0.5f));
		Vector2 hueSz = GetWidgetSize(hueGO);
		Texture2D satvalTex = new Texture2D(2, 2);
		satvalGO.GetComponent<Image>().sprite = Sprite.Create(satvalTex, new Rect(0.5f, 0.5f, 1f, 1f), new Vector2(0.5f, 0.5f));
		Action resetSatValTexture = delegate
		{
			for (int j = 0; j < 2; j++)
			{
				for (int k = 0; k < 2; k++)
				{
					satvalTex.SetPixel(k, j, satvalColors[k + j * 2]);
				}
			}
			satvalTex.Apply();
		};
		Vector2 satvalSz = GetWidgetSize(satvalGO);
		RGBToHSV(inputColor, out float Hue, out float Saturation, out float Value);
		Action applyHue = delegate
		{
			int num = Mathf.Clamp((int)Hue, 0, 5);
			int num2 = (num + 1) % 6;
			Color color3 = Color.Lerp(hueColors[num], hueColors[num2], Hue - (float)num);
			satvalColors[3] = color3;
			resetSatValTexture();
		};
		Action applySaturationValue = delegate
		{
			Vector2 vector4 = new Vector2(Saturation, Value);
			Vector2 vector5 = new Vector2(1f - vector4.x, 1f - vector4.y);
			Color a = vector5.x * vector5.y * satvalColors[0];
			Color b = vector4.x * vector5.y * satvalColors[1];
			Color b2 = vector5.x * vector4.y * satvalColors[2];
			Color b3 = vector4.x * vector4.y * satvalColors[3];
			Color color = a + b + b2 + b3;
			color = (result.GetComponent<Image>().color = new Color(color.r, color.g, color.b, inputColor.a));
			if (_color != color)
			{
				if (_onValueChange != null)
				{
					_onValueChange(color);
				}
				_color = color;
			}
		};
		applyHue();
		applySaturationValue();
		satvalKnob.transform.localPosition = new Vector2(Saturation * satvalSz.x, Value * satvalSz.y);
		hueKnob.transform.localPosition = new Vector2(hueKnob.transform.localPosition.x, Hue / 6f * satvalSz.y);
		Action dragH = null;
		Action dragSV = null;
		Action idle = delegate
		{
			if (Input.GetMouseButtonDown(0))
			{
				if (GetLocalMouse(hueGO, out Vector2 vector3))
				{
					_update = dragH;
				}
				else if (GetLocalMouse(satvalGO, out vector3))
				{
					_update = dragSV;
				}
			}
		};
		dragH = delegate
		{
			GetLocalMouse(hueGO, out Vector2 vector2);
			Hue = vector2.y / hueSz.y * 6f;
			applyHue();
			applySaturationValue();
			hueKnob.transform.localPosition = new Vector2(hueKnob.transform.localPosition.x, vector2.y);
			if (Input.GetMouseButtonUp(0))
			{
				_update = idle;
			}
		};
		dragSV = delegate
		{
			GetLocalMouse(satvalGO, out Vector2 vector);
			Saturation = vector.x / satvalSz.x;
			Value = vector.y / satvalSz.y;
			applySaturationValue();
			satvalKnob.transform.localPosition = vector;
			if (Input.GetMouseButtonUp(0))
			{
				_update = idle;
			}
		};
		_update = idle;
	}

	public void SetRandomColor()
	{
		System.Random random = new System.Random();
		float r = (float)(random.Next() % 1000) / 1000f;
		float g = (float)(random.Next() % 1000) / 1000f;
		float b = (float)(random.Next() % 1000) / 1000f;
		Color = new Color(r, g, b);
	}

	private void OnEnable()
	{
		result.GetComponent<Image>().color = startColor;
		Color = startColor;
		result.GetComponent<Image>().color = startColor;
		if (alphaSlider != null)
		{
			alphaSlider.slider.value = startColor.a;
		}
	}

	private void Update()
	{
		_update();
	}
}
