using UnityEngine;

public class Skeletons : ADOBase
{
	public GameObject chains;

	public GameObject skeletons;

	private scrParallax[] parallaxComponents;

	public void Awake()
	{
		chains.SetActive(value: false);
		skeletons.SetActive(value: false);
		parallaxComponents = base.gameObject.GetComponentsInChildren<scrParallax>(includeInactive: true);
	}

	public void Show()
	{
		chains.SetActive(value: true);
		skeletons.SetActive(value: true);
		scrParallax[] array = parallaxComponents;
		foreach (scrParallax obj in array)
		{
			obj.relativeToCamera = true;
			obj.posCamAtStart = ADOBase.controller.camy.transform.position;
		}
	}

	public void Hide()
	{
		chains.SetActive(value: false);
		skeletons.SetActive(value: false);
		scrParallax[] array = parallaxComponents;
		foreach (scrParallax obj in array)
		{
			obj.relativeToCamera = true;
			obj.posCamAtStart = ADOBase.controller.camy.transform.position;
		}
	}
}
