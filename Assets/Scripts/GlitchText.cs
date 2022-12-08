using System.Runtime.CompilerServices;
using UnityEngine;

public class GlitchText : MonoBehaviour
{
	public TextMesh[] textMeshes;

	public string characters = "";

	public float shortInterval;

	public float longInterval;

	private float nextVariation;

	private bool longVariation = true;

	private void Update()
	{
		float timeSinceLevelLoad = Time.timeSinceLevelLoad;
		if (timeSinceLevelLoad > nextVariation)
		{
			float average = longVariation ? shortInterval : longInterval;
			nextVariation = timeSinceLevelLoad + _003CUpdate_003Eg__RandomWithVariation_007C6_0(average, 0.2f);
			ChangeGlitch();
			longVariation = !longVariation;
		}
	}

	private void ChangeGlitch()
	{
		string text = " " + _003CChangeGlitch_003Eg__RandomChars_007C7_1(3) + " \n" + _003CChangeGlitch_003Eg__RandomChars_007C7_1(5) + "\n" + " " + _003CChangeGlitch_003Eg__RandomChars_007C7_1(3) + " ";
		TextMesh[] array = textMeshes;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].text = text;
		}
	}

	[CompilerGenerated]
	private static float _003CUpdate_003Eg__RandomWithVariation_007C6_0(float average, float variation)
	{
		return UnityEngine.Random.Range(average - average * variation, average + average * variation);
	}

	[CompilerGenerated]
	private char _003CChangeGlitch_003Eg__RandomCharacter_007C7_0()
	{
		int index = UnityEngine.Random.Range(0, characters.Length);
		return characters[index];
	}

	[CompilerGenerated]
	private string _003CChangeGlitch_003Eg__RandomChars_007C7_1(int charCount)
	{
		string text = "";
		for (int i = 0; i < charCount; i++)
		{
			text += _003CChangeGlitch_003Eg__RandomCharacter_007C7_0().ToString();
		}
		return text;
	}
}
