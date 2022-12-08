using UnityEngine;

public class scrFlipOnBeat : ADOBase
{
	public Sprite[] sprites;

	private int counter;

	public override void OnBeat()
	{
		counter++;
		int num = counter % sprites.Length;
		GetComponent<SpriteRenderer>().sprite = sprites[num];
	}
}
