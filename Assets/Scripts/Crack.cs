// Crack
using System;
using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class Crack : MonoBehaviour
{
	public Mawaru_Sprite crack;

	public Mawaru_Sprite mask;

	public TextMeshPro tbcText;

	private Color whiteClear = new Color(1f, 1f, 1f, 0f);

	public void SetState(int state)
	{
		crack.SetState(state);
		mask.SetState(state);
	}

	private void Awake()
	{
		Localize();
		tbcText.DOColor(whiteClear, 0f);
	}

	public void Localize()
	{
		tbcText.text = RDString.Get("neoCosmosStory.toBeContinued");
		tbcText.SetLocalizedFont();
	}

	private void Start()
	{
	}

	public void FadeText(float dur = 1f)
	{
		tbcText.DOColor(Color.white, dur);
	}

	private void Update()
	{
		TMP_TextInfo textInfo = tbcText.textInfo;
		for (int i = 0; i < textInfo.meshInfo.Length; i++)
		{
			textInfo.meshInfo[i].mesh.vertices = textInfo.meshInfo[i].vertices;
			tbcText.UpdateGeometry(textInfo.meshInfo[i].mesh, i);
		}
	}

	public void TextSequence()
	{
		FadeText();
		string text = "Neo Cosmos...";
		string text2 = "Comes sooN...";
		int length = text.Length;
		TMP_TextInfo textInfo = UnityEngine.Object.Instantiate(tbcText).GetTextInfo(text2);
		tbcText.text = text;
		tbcText.SetLocalizedFont();
		tbcText.ForceMeshUpdate();
		TMP_TextInfo textInfo2 = tbcText.textInfo;
		int[] array = new int[length];
		for (int i = 0; i < length; i++)
		{
			char c = text[i];
			for (int j = 0; j < length; j++)
			{
				if (text2[j] == c && !array.Contains(j))
				{
					array[i] = j;
					Debug.Log("wow!: " + (i + 1) + " -> " + (j + 1));
					break;
				}
			}
		}
		Debug.Log($"{textInfo2.characterInfo} {length} / {textInfo2.characterInfo.Length}");
		for (int k = 0; k < length; k++)
		{
			TMP_CharacterInfo tMP_CharacterInfo = textInfo2.characterInfo[k];
			int charIndex = tMP_CharacterInfo.vertexIndex;
			int materialReferenceIndex = tMP_CharacterInfo.materialReferenceIndex;
			Vector3[] vertices = textInfo2.meshInfo[materialReferenceIndex].vertices;
			Vector2 size;
			if (tMP_CharacterInfo.isVisible)
			{
				TMP_CharacterInfo tMP_CharacterInfo2 = textInfo.characterInfo[array[k]];
				int vertexIndex = tMP_CharacterInfo2.vertexIndex;
				int materialReferenceIndex2 = tMP_CharacterInfo2.materialReferenceIndex;
				Vector3[] vertices2 = textInfo.meshInfo[materialReferenceIndex2].vertices;
				size = vertices[charIndex + 2] - vertices[charIndex];
				Vector2 pos2 = (Vector2)vertices[charIndex] + size / 2f;
				Vector2 endValue = (Vector2)vertices2[vertexIndex] + size / 2f;
				float num = 2f;
				float num2 = 1f;
				_ = (Vector2)tbcText.transform.position;
				float f = Mathf.Lerp(MathF.PI, 0f, (float)k / (float)(length - 1)) + MathF.PI / 2f;
				new Vector2(Mathf.Sin(f) * num, Mathf.Cos(f) * num2);
				DOTween.To(() => pos2, delegate (Vector2 x)
				{
					SetPos(x);
				}, endValue, 3f).SetEase(Ease.OutSine).SetDelay(1f);
			}
			void SetPos(Vector2 pos)
			{
				vertices[charIndex] = pos + new Vector2(0f - size.x, 0f - size.y) / 2f;
				vertices[charIndex + 1] = pos + new Vector2(0f - size.x, size.y) / 2f;
				vertices[charIndex + 2] = pos + new Vector2(size.x, size.y) / 2f;
				vertices[charIndex + 3] = pos + new Vector2(size.x, 0f - size.y) / 2f;
			}
		}
	}
}
