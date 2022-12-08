using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace ADOFAI
{
	public class RDColorPickerPopup : ADOBase
	{
		private const float MaxY = -175f;

		private const float MinY = -542f;

		public GameObject alphaSlider;

		public GameObject alphaInputField;

		public RectTransform panel;

		public Ease showEase;

		public Ease hideEase;

		public float animDuration;

		public RectTransform arrow;

		public CUIColorPicker cuiColorPicker;

		public InputField hexColor;

		public InputField rInput;

		public InputField gInput;

		public InputField bInput;

		public InputField aInput;

		public string lastValidColor = "ffddee";

		private PropertyControl_Color colorPC;

		private bool isAnyRGBAInputFocused;

		private RectTransform rectTransform;

		private bool UsesAlpha => colorPC.propertyInfo.color_usesAlpha;

		private string Color
		{
			get
			{
				if (!hexColor.text.IsValidHexColor(UsesAlpha))
				{
					return lastValidColor;
				}
				return hexColor.text;
			}
			set
			{
				if (value.IsValidHexColor(UsesAlpha))
				{
					hexColor.text = value;
				}
			}
		}

		private void Awake()
		{
			rectTransform = GetComponent<RectTransform>();
			hexColor.onValueChanged.AddListener(delegate
			{
				if (hexColor.text.IsValidHexColor(UsesAlpha) && hexColor.isFocused)
				{
					lastValidColor = hexColor.text;
					cuiColorPicker.Color = lastValidColor.HexToColor();
				}
			});
		}

		private void Update()
		{
			isAnyRGBAInputFocused = (rInput.isFocused || gInput.isFocused || bInput.isFocused || aInput.isFocused);
			if (!isAnyRGBAInputFocused && hexColor.text.IsValidHexColor(UsesAlpha))
			{
				Color color = hexColor.text.HexToColor();
				rInput.text = Convert.ToInt32(color.r * 255f).ToString();
				gInput.text = Convert.ToInt32(color.g * 255f).ToString();
				bInput.text = Convert.ToInt32(color.b * 255f).ToString();
				aInput.text = Convert.ToInt32(color.a * 255f).ToString();
			}
			if (!hexColor.isFocused)
			{
				UpdateHexInputColor();
			}
		}

		public void SetResultColorFromRGBAInputs(InputField currentRGBAInput)
		{
			if (isAnyRGBAInputFocused)
			{
				if (string.IsNullOrEmpty(currentRGBAInput.text))
				{
					currentRGBAInput.text = "0";
				}
				else if (Convert.ToInt32(currentRGBAInput.text) < 0)
				{
					currentRGBAInput.text = "0";
				}
				else if (Convert.ToInt32(currentRGBAInput.text) > 255)
				{
					currentRGBAInput.text = "255";
				}
				Color color = new Color((float)Convert.ToInt32(rInput.text) / 255f, (float)Convert.ToInt32(gInput.text) / 255f, (float)Convert.ToInt32(bInput.text) / 255f, (float)Convert.ToInt32(aInput.text) / 255f);
				cuiColorPicker.Color = color;
			}
		}

		private void UpdateHexInputColor()
		{
			hexColor.text = ColorUtility.ToHtmlStringRGBA(cuiColorPicker.Color).ToLower();
		}

		public void Show(PropertyControl_Color propertyControl_Color)
		{
			colorPC = propertyControl_Color;
			RectTransform component = hexColor.GetComponent<RectTransform>();
			if (UsesAlpha)
			{
				alphaSlider.SetActive(value: true);
				alphaInputField.SetActive(value: true);
				panel.sizeDelta = new Vector2(300f, 332f);
				hexColor.characterLimit = 8;
				component.sizeDelta = component.sizeDelta.WithX(96f);
			}
			else
			{
				alphaSlider.SetActive(value: false);
				alphaInputField.SetActive(value: false);
				panel.sizeDelta = new Vector2(300f, 297f);
				hexColor.characterLimit = 6;
				component.sizeDelta = component.sizeDelta.WithX(80f);
			}
			alphaSlider.GetComponent<AlphaSlider>().result = colorPC.sample;
			cuiColorPicker.startColor = colorPC.text.HexToColor();
			cuiColorPicker.result = colorPC.sample.gameObject;
			hexColor.text = ColorUtility.ToHtmlStringRGBA(colorPC.sample.color);
			base.gameObject.SetActive(value: true);
			bool flag = propertyControl_Color.propertiesPanel.inspectorPanel == ADOBase.editor.settingsPanel;
			if (flag)
			{
				float num = ((colorPC.transform as RectTransform).sizeDelta.x / 2f + 1f) * (float)Screen.width / ADOBase.editor.levelEditorCanvas.GetComponent<CanvasScaler>().referenceResolution.x;
				Vector3 position = colorPC.sample.rectTransform.position;
				position.x += num + 4f;
				rectTransform.position = position;
			}
			else
			{
				Vector3 position2 = colorPC.rectTransform.position;
				position2.y = colorPC.sample.rectTransform.position.y;
				rectTransform.position = position2;
			}
			arrow.ScaleX(flag ? (-1f) : 1f);
			panel.pivot = panel.pivot.WithX(flag ? 0f : 1f);
			panel.AnchorPosX(flag ? 12 : (-12));
			Vector2 anchoredPosition = rectTransform.anchoredPosition;
			float y = 0f;
			if (anchoredPosition.y < -542f)
			{
				y = -542f - anchoredPosition.y;
			}
			else if (anchoredPosition.y > -175f)
			{
				y = -175f - anchoredPosition.y;
			}
			panel.AnchorPosY(y);
			ADOBase.editor.ShowPopupBlocker(delegate
			{
				Hide();
			});
			Animate(show: true);
		}

		public void Hide()
		{
			if (base.gameObject.activeSelf)
			{
				colorPC.text = Color;
				colorPC.OnEndEdit(Color);
				Animate(show: false);
			}
		}

		public void Animate(bool show)
		{
			Vector3 localScale = show ? Vector3.zero : Vector3.one;
			Vector3 endValue = show ? Vector3.one : Vector3.zero;
			Ease ease = show ? showEase : hideEase;
			rectTransform.DOKill();
			rectTransform.localScale = localScale;
			rectTransform.DOScale(endValue, animDuration).SetEase(ease).SetUpdate(isIndependentUpdate: true)
				.OnComplete(delegate
				{
					base.gameObject.SetActive(show);
				});
		}
	}
}
