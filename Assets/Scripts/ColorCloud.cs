using UnityEngine;

public class ColorCloud : ADOBase
{
	public GameObject goldSparks;

	private ParticleSystem particleSystem;

	private void Awake()
	{
		particleSystem = GetComponent<ParticleSystem>();
	}

	public void SetSortingOrder(int order)
	{
		particleSystem.GetComponent<ParticleSystemRenderer>().sortingOrder = order;
	}
}
