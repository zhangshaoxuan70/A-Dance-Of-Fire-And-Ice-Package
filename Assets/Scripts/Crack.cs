using DG.Tweening;
using System;
using System.Linq;
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
					UnityEngine.Debug.Log("wow!: " + (i + 1).ToString() + " -> " + (j + 1).ToString());
					break;
				}
			}
		}
		UnityEngine.Debug.Log($"{textInfo2.characterInfo} {length} / {textInfo2.characterInfo.Length}");
		for (int k = 0; k < length; k++)
		{
			TMP_CharacterInfo tMP_CharacterInfo = textInfo2.characterInfo[k];
			int charIndex = tMP_CharacterInfo.vertexIndex;
			int materialReferenceIndex = tMP_CharacterInfo.materialReferenceIndex;
			Vector3[] vertices = textInfo2.meshInfo[materialReferenceIndex].vertices;
			if (tMP_CharacterInfo.isVisible)
			{
				TMP_CharacterInfo tMP_CharacterInfo2 = textInfo.characterInfo[array[k]];
				int vertexIndex = tMP_CharacterInfo2.vertexIndex;
				int materialReferenceIndex2 = tMP_CharacterInfo2.materialReferenceIndex;
				Vector3[] vertices2 = textInfo.meshInfo[materialReferenceIndex2].vertices;
				Vector2 size = vertices[charIndex + 2] - vertices[charIndex];
				Vector2 pos = (Vector2)vertices[charIndex] + size / 2f;
				Vector2 endValue = (Vector2)vertices2[vertexIndex] + size / 2f;
				float num = 2f;
				float num2 = 1f;
				Vector2 vector = (Vector2)tbcText.transform.position;
				float f = Mathf.Lerp(MathF.PI, 0f, (float)k / (float)(length - 1)) + MathF.PI / 2f;
				new Vector2(Mathf.Sin(f) * num, Mathf.Cos(f) * num2);
				_003C_003Ec__DisplayClass10_0 CS_0024_003C_003E8__locals0;
				DOTween.To(() => pos, delegate(Vector2 x)
				{
					CS_0024_003C_003E8__locals0._003CTextSequence_003Eg__SetPos_007C0(x);
				}, endValue, 3f).SetEase(Ease.OutSine).SetDelay(1f);
			}
		}
	}
}
