using UnityEngine;

[AddComponentMenu("Utility/Super Game Feel Effects/Constant Shake")]
public class ConstantShake : MonoBehaviour
{
	private Transform t;

	private Vector3 lastShake;

	public Vector3 strength = Vector2.one;

	public float intensity = 1f;

	private void Awake()
	{
		t = base.transform;
	}

	private void Update()
	{
		t.localPosition -= lastShake;
		lastShake = new Vector3(UnityEngine.Random.Range(0f - strength.x, strength.x) * intensity, UnityEngine.Random.Range(0f - strength.y, strength.y) * intensity, UnityEngine.Random.Range(0f - strength.z, strength.z) * intensity);
		t.localPosition += lastShake;
	}
}
