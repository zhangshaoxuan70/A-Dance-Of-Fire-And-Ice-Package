using UnityEngine;

public class scrVisualDecoration : scrDecoration
{
	public SpriteRenderer spriteRenderer;

	private SpriteRenderer bordersRenderer;

	private SpriteRenderer hitboxRenderer;

	private bool smoothing;

	private Vector2 cachedBorderSize;

	private bool shouldLateUpdate = true;

	public Vector2 slicedSize;

	private void Awake()
	{
		if (ADOBase.isLevelEditor)
		{
			bordersRenderer = selectionBordersObject.GetComponent<SpriteRenderer>();
			hitboxRenderer = hitboxBordersObject.GetComponent<SpriteRenderer>();
		}
	}

	public void LateUpdate()
	{
		if (!shouldLateUpdate || bordersRenderer == null || spriteRenderer.sprite == null)
		{
			return;
		}
		if (bordersRenderer.gameObject.activeSelf)
		{
			float orthographicSize = Camera.main.orthographicSize;
			Vector2 vector = spriteRenderer.sprite.bounds.size * base.transform.localScale.xy() / orthographicSize;
			if (!Mathf.Approximately(cachedBorderSize.x, vector.x) || !Mathf.Approximately(cachedBorderSize.y, vector.y))
			{
				bordersRenderer.size = vector;
				cachedBorderSize = vector;
			}
			Vector2 vector2 = new Vector2(Mathf.Abs(base.transform.localScale.x), Mathf.Abs(base.transform.localScale.y));
			Vector3 localScale = new Vector3(orthographicSize / Mathf.Max(vector2.x, 0.01f), orthographicSize / Mathf.Max(vector2.y, 0.01f), 1f);
			bordersRenderer.transform.localScale = localScale;
		}
		if (!(hitboxRenderer == null) && !(spriteRenderer.sprite == null) && hitboxRenderer.transform.gameObject.activeSelf)
		{
			float orthographicSize2 = Camera.main.orthographicSize;
			if (hitboxType != Hitbox.Circle)
			{
				hitboxRenderer.size = spriteRenderer.sprite.bounds.size * hitboxScale * base.transform.localScale.xy() / orthographicSize2;
			}
			else
			{
				hitboxRenderer.size = Vector2.one * hitboxScale.magnitude * base.transform.localScale.x * spriteRenderer.sprite.bounds.size.x / orthographicSize2;
			}
			Vector2 vector3 = new Vector2(Mathf.Abs(base.transform.localScale.x), Mathf.Abs(base.transform.localScale.y));
			Vector3 localScale2 = new Vector3(orthographicSize2 / Mathf.Max(vector3.x, 0.01f), orthographicSize2 / Mathf.Max(vector3.y, 0.01f), 1f);
			hitboxRenderer.transform.localScale = localScale2;
			hitboxRenderer.transform.localPosition = hitboxOffset;
			if (hitboxType != Hitbox.Circle)
			{
				hitboxRenderer.transform.localEulerAngles = Vector3.zero;
			}
			else
			{
				hitboxRenderer.transform.localEulerAngles = -Vector3.forward * hitboxRotation;
			}
		}
	}

	private void OnBecameVisible()
	{
		printe("Became visible");
		shouldLateUpdate = true;
	}

	private void OnBecameInvisible()
	{
		printe("Became INVISIBLE");
		shouldLateUpdate = false;
	}

	public void SetSprite(Sprite sprite)
	{
		spriteRenderer.sprite = sprite;
		if ((bool)boxCollider && sprite != null)
		{
			boxCollider.size = spriteRenderer.sprite.bounds.size;
		}
		SetSmoothing(smoothing);
	}

	public override void UpdateHitbox()
	{
		if (!(spriteRenderer.sprite != null))
		{
			return;
		}
		damageBox.enabled = false;
		damageCircle.enabled = false;
		damageCapsule.enabled = false;
		damageBox.gameObject.transform.localEulerAngles = Vector3.forward * hitboxRotation;
		if (!canHitPlanets)
		{
			return;
		}
		if (hitboxType == Hitbox.Box)
		{
			damageBox.size = spriteRenderer.sprite.bounds.size * hitboxScale;
			damageBox.offset = hitboxOffset;
			damageBox.enabled = true;
		}
		else if (hitboxType == Hitbox.Capsule)
		{
			if (hitboxScale.x > hitboxScale.y)
			{
				damageCapsule.direction = CapsuleDirection2D.Horizontal;
			}
			else
			{
				damageCapsule.direction = CapsuleDirection2D.Vertical;
			}
			damageCapsule.size = spriteRenderer.sprite.bounds.size * hitboxScale;
			damageCapsule.offset = hitboxOffset;
			damageCapsule.enabled = true;
		}
		else
		{
			damageCircle.radius = hitboxScale.magnitude * spriteRenderer.sprite.bounds.size.x / 2f;
			damageCircle.offset = hitboxOffset;
			damageCircle.enabled = true;
		}
	}

	public override void SetDepth(int depth)
	{
		string sortingLayerName = (depth >= 0) ? "Bg" : "Default";
		int layer = (depth >= 0) ? 9 : 7;
		spriteRenderer.sortingLayerName = sortingLayerName;
		spriteRenderer.gameObject.layer = layer;
		int sortingOrder = -depth;
		spriteRenderer.sortingOrder = sortingOrder;
	}

	public override void ApplyColor()
	{
		spriteRenderer.color = color.WithAlpha(color.a * opacity);
	}

	public override float GetAlpha()
	{
		return spriteRenderer.color.a * opacity;
	}

	public void SetTile(Vector2 newTile)
	{
		Material material = spriteRenderer.material;
		material.SetFloat("RepeatX", newTile.x);
		material.SetFloat("RepeatY", newTile.y);
		Sprite sprite = spriteRenderer.sprite;
		if (sprite != null)
		{
			Texture2D texture = sprite.texture;
			texture.wrapModeU = ((newTile.x == 1f) ? TextureWrapMode.Clamp : TextureWrapMode.Repeat);
			texture.wrapModeV = ((newTile.y == 1f) ? TextureWrapMode.Clamp : TextureWrapMode.Repeat);
		}
	}

	public void SetSmoothing(bool smoothing)
	{
		this.smoothing = smoothing;
		if ((bool)spriteRenderer.sprite)
		{
			spriteRenderer.sprite.texture.filterMode = (smoothing ? FilterMode.Bilinear : FilterMode.Point);
		}
	}

	public override void SetVisible(bool visible)
	{
		spriteRenderer.enabled = visible;
	}

	public override bool GetVisible()
	{
		return spriteRenderer.enabled;
	}
}
