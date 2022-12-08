using UnityEngine;

public class SetHalloweenClouds : ADOBase
{
	public Color color;

	private void Awake()
	{
		if (ADOBase.IsHalloweenWeek())
		{
			GetComponent<ParticleSystem>().main.startColor = color;
		}
	}
}
