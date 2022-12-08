using UnityEngine;

public class scrPAPRopeHex : ADOBase
{
	public float rotationSpeed;

	public Sprite[] frames;

	private scrSpriteAnimator spriteAnimator;

	private void Awake()
	{
		spriteAnimator = GetComponent<scrSpriteAnimator>();
	}

	private void Update()
	{
		float z = base.transform.localRotation.eulerAngles.z;
		base.transform.localRotation = Quaternion.Euler(0f, 0f, z + rotationSpeed * Time.deltaTime);
	}

	public void SetAnim(int anim)
	{
		int num = anim * 4;
		if (num + 3 >= frames.Length || num < 0)
		{
			UnityEngine.Debug.Log("Invalid anim index!");
		}
		else
		{
			spriteAnimator.sprites = new Sprite[4]
			{
				frames[num],
				frames[num + 1],
				frames[num + 2],
				frames[num + 3]
			};
		}
	}
}
