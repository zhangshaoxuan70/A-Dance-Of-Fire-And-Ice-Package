using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class CustomLevelTile : ffxBase
{
	[NonSerialized]
	public string levelKey;

	public CustomLevelTile customLevelTile;

	public Text title;

	public Text artist;

	public Text removedText;

	public RawImage image;

	public RectTransform InfoContainer;

	public scrBlur blur;

	public bool selected;

	private Coroutine loadTextureCoroutine;

	public bool didStartLoadingIcon
	{
		get;
		private set;
	}

	public bool didProcessIcon
	{
		get;
		private set;
	}

	public override void doEffect()
	{
		if (!scnCLS.instance.initializing)
		{
			scnCLS.instance.SelectLevel(customLevelTile, snap: false);
			Highlight();
		}
	}

	public void Highlight(bool highlight = true, bool instant = false)
	{
		float duration = instant ? 0f : 0.25f;
		float num = highlight ? 1f : 0.7f;
		float alpha = highlight ? 1f : Mathf.Clamp(0.8f - Mathf.Abs(scrController.instance.chosenplanet.transform.position.y - base.transform.position.y) * 0.15f, 0.1f, 1f);
		Ease ease = highlight ? Ease.OutBack : Ease.OutSine;
		InfoContainer.DOScale(new Vector3(num, num, 1f), duration).SetEase(ease);
		title.DOColor(title.color.WithAlpha(alpha), duration).SetEase(ease);
		artist.DOColor(artist.color.WithAlpha(alpha), duration).SetEase(ease);
		removedText.DOColor(removedText.color.WithAlpha(alpha), duration).SetEase(ease);
	}

	public void SetDeleted()
	{
		title.gameObject.SetActive(value: false);
		artist.gameObject.SetActive(value: false);
		removedText.gameObject.SetActive(value: true);
		image.gameObject.SetActive(value: false);
		MarkUnavailable();
	}

	public void MarkUnavailable()
	{
		GetComponent<SpriteRenderer>().color = Color.gray;
	}

	public void LoadTileIcon(string iconPath, Color iconColor)
	{
		if (!didStartLoadingIcon)
		{
			didStartLoadingIcon = true;
			loadTextureCoroutine = StartCoroutine(LoadTexture(iconPath, iconColor));
		}
	}

	private IEnumerator LoadTexture(string iconPath, Color iconColor)
	{
		yield return new WaitForSeconds(UnityEngine.Random.Range(0f, 1f));
		Texture2D texture;
		using (UnityWebRequest imageRequest = UnityWebRequestTexture.GetTexture("file://" + iconPath))
		{
			yield return imageRequest.SendWebRequest();
			if (imageRequest.result == UnityWebRequest.Result.ConnectionError)
			{
				printe("Failed to load tile image (NetworkError): " + iconPath);
			}
			else
			{
				if (imageRequest.result != UnityWebRequest.Result.ProtocolError)
				{
					texture = ((DownloadHandlerTexture)imageRequest.downloadHandler).texture;
					goto IL_010e;
				}
				printe("Failed to load tile image (HttpError): " + iconPath);
			}
		}
		/*Error near IL_013d: Unexpected return in MoveNext()*/;
		IL_010e:
		ProcessIconTexture(texture, iconColor);
	}

	public void ProcessIconTexture(Texture2D icon, Color iconColor)
	{
		if (!didProcessIcon)
		{
			didProcessIcon = true;
			image.enabled = true;
			image.color = Color.clear;
			scrExtImgHolder.ShrinkImage(icon, 128);
			image.texture = icon;
			blur.baseTint = iconColor;
			blur.blurTint = Color.black;
			blur.UpdateTexture();
			image.DOColor(Color.white, 0.5f);
		}
	}

	private void OnDestroy()
	{
		DOTween.Kill(title);
		DOTween.Kill(artist);
		DOTween.Kill(image);
		DOTween.Kill(removedText);
		DOTween.Kill(InfoContainer);
		if (loadTextureCoroutine != null)
		{
			StopCoroutine(loadTextureCoroutine);
		}
	}

	private void Start()
	{
		removedText.text = RDString.Get("cls.worldRemoved");
	}
}
