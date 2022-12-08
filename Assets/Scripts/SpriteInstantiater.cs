using UnityEngine;

public class SpriteInstantiater : MonoBehaviour
{
	public Sprite mySprite;

	public float delay = 0.1f;

	public float spriteAliveTime = 5f;

	public bool fadeSprite = true;

	public AnimationCurve fadeCurve = new AnimationCurve(new Keyframe(0f, 1f, -1f, -1f), new Keyframe(1f, 0f, -1f, -1f));

	public string sortingLayerName = "Default";

	private float timer;

	private Vector3 lastPosition;

	private Transform trailParent;

	private void Start()
	{
		trailParent = new GameObject().transform;
		trailParent.name = "Sprite Instantiater Parent";
	}

	private void OnValidate()
	{
		if (delay < 0f)
		{
			delay = 0f;
		}
		if (spriteAliveTime < 0f)
		{
			spriteAliveTime = 0f;
		}
	}

	private void Update()
	{
		if (timer >= delay && lastPosition != base.transform.position)
		{
			lastPosition = base.transform.position;
			GameObject gameObject = new GameObject();
			gameObject.transform.position = base.transform.position;
			gameObject.transform.rotation = base.transform.rotation;
			gameObject.transform.localScale = base.transform.localScale;
			gameObject.transform.parent = trailParent;
			SpriteRenderer spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
			spriteRenderer.sprite = mySprite;
			spriteRenderer.sortingLayerName = sortingLayerName;
			KillSpriteAfterTime killSpriteAfterTime = gameObject.AddComponent<KillSpriteAfterTime>();
			killSpriteAfterTime.aliveTime = spriteAliveTime;
			killSpriteAfterTime.fadeAway = fadeSprite;
			killSpriteAfterTime.fadeCurve = fadeCurve;
			killSpriteAfterTime.sr = spriteRenderer;
			timer = 0f;
		}
		timer += Time.deltaTime;
	}
}
