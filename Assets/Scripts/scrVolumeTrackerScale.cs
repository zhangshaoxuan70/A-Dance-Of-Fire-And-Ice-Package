using UnityEngine;

public class scrVolumeTrackerScale : ADOBase
{
	public int freq = 20;

	private Vector3 originalScale;

	public AudioSource conductoraudio;

	private float[] spectrum = new float[1024];

	public float maxmultiplier = 1.5f;

	public float minmultiplier = 1f;

	private new scrConductor conductor;

	private void Awake()
	{
		originalScale = base.transform.localScale;
		conductor = scrConductor.instance;
		conductoraudio = conductor.gameObject.GetComponent<AudioSource>();
	}

	private void Update()
	{
		float num = Mathf.Log(conductor.spectrum[freq]) / 10f * (maxmultiplier - minmultiplier) + minmultiplier;
		if (num > 0f)
		{
			base.transform.localScale = new Vector3(originalScale.x * num, originalScale.y * num, 1f);
		}
	}
}
