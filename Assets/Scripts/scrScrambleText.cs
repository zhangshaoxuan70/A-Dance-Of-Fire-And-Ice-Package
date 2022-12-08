using System;
using UnityEngine;
using UnityEngine.UI;

public class scrScrambleText : ADOBase
{
	public Text text;

	private float currTimer;

	private char[] charArray = new char[150];

	private void Awake()
	{
		ChangeText();
	}

	private void Update()
	{
		if (ADOBase.controller.visualEffects != 0)
		{
			ChangeText();
		}
	}

	private void ChangeText()
	{
		currTimer += Time.deltaTime;
		if (currTimer >= 0.0166666675f)
		{
			for (int i = 0; i < 150; i++)
			{
				int value = UnityEngine.Random.Range(33, 127);
				charArray[i] = Convert.ToChar(value);
			}
			text.text = new string(charArray);
			currTimer = 0f;
		}
	}
}
