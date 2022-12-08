using UnityEngine;

public class MaskLightGradient : ADOBase
{
	private SpriteRenderer spriteRenderer;

	private SpriteRenderer darkness;

	private SpriteMask spriteMask;

	private void Start()
	{
		spriteRenderer = GetComponent<SpriteRenderer>();
		spriteMask = base.transform.parent.GetComponent<SpriteMask>();
		darkness = Level.FindDecorationComponent<SpriteRenderer>("darkness");
		if (darkness == null)
		{
			printe("darkness is null");
		}
		if (spriteRenderer == null)
		{
			printe("sr is null");
		}
		if (spriteMask == null)
		{
			printe("spritemask is null");
		}
	}

	private void Update()
	{
		spriteRenderer.color = Color.white.WithAlpha(darkness.color.a);
		spriteMask.enabled = (darkness.color.a > 0f);
	}
}
