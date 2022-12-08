using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class scrTextDecoration : scrDecoration
{
	public Text text;

	public Canvas canvas;

	private Image bordersRenderer;

	private int startingSize;

	private float startingSpacing;

	private int startingResizeTextMaxSize;

	private int startingResizeTextMinSize;

	private FontData fontData;

	private float width;

	private float height;

	private void Awake()
	{
		startingSize = text.fontSize;
		startingSpacing = text.lineSpacing;
		startingResizeTextMaxSize = text.resizeTextMaxSize;
		startingResizeTextMinSize = text.resizeTextMinSize;
		if (selectionBordersObject != null)
		{
			bordersRenderer = selectionBordersObject.GetComponent<Image>();
		}
	}

	public void LateUpdate()
	{
		if (!(bordersRenderer == null) && bordersRenderer.transform.gameObject.activeSelf)
		{
			float orthographicSize = Camera.main.orthographicSize;
			bordersRenderer.pixelsPerUnitMultiplier = 1f / orthographicSize;
		}
	}

	public void SetText(string newText)
	{
		text.text = newText;
		StartCoroutine(SetCollider());
	}

	public void SetFont(FontName fontName)
	{
		text.fontSize = startingSize;
		text.lineSpacing = startingSpacing;
		text.resizeTextMaxSize = startingResizeTextMaxSize;
		text.resizeTextMinSize = startingResizeTextMinSize;
		fontData = ((fontName == FontName.Default) ? RDString.fontData : RDString.enFontData);
		text.font = ADOBase.controller.nameToFont[fontName];
		float fontScale = fontData.fontScale;
		float lineSpacing = fontData.lineSpacing;
		text.fontSize = Mathf.RoundToInt((float)text.fontSize * fontScale);
		text.lineSpacing *= lineSpacing;
		text.resizeTextMaxSize = Mathf.RoundToInt((float)text.resizeTextMaxSize * fontScale);
	}

	public override void SetDepth(int depth)
	{
		string sortingLayerName = (depth >= 0) ? "Bg" : "Default";
		int layer = (depth >= 0) ? 9 : 7;
		canvas.sortingLayerName = sortingLayerName;
		canvas.gameObject.layer = layer;
		int sortingOrder = -depth;
		canvas.sortingOrder = sortingOrder;
	}

	public override void ApplyColor()
	{
		text.color = color.WithAlpha(color.a * opacity);
	}

	public override float GetAlpha()
	{
		return text.color.a * opacity;
	}

	private IEnumerator SetCollider()
	{
		yield return null;
		TextGenerator textGenerator = new TextGenerator();
		TextGenerationSettings generationSettings = text.GetGenerationSettings(text.rectTransform.rect.size);
		width = textGenerator.GetPreferredWidth(text.text, generationSettings);
		height = textGenerator.GetPreferredHeight(text.text, generationSettings);
		Vector2 vector = new Vector2(width, height);
		boxCollider.size = vector;
		selectionBordersObject.GetComponent<RectTransform>().sizeDelta = vector;
	}

	public override void SetVisible(bool visible)
	{
		text.enabled = visible;
	}

	public override bool GetVisible()
	{
		return text.enabled;
	}
}
