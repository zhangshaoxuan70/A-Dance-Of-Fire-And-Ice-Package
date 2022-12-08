using UnityEngine;

public class scrDisableIfNotMessupPossible : MonoBehaviour
{
	private void Start()
	{
		if (GCS.d_boothDisablePossibleMessUpButtons)
		{
			base.gameObject.SetActive(value: false);
		}
	}

	private void Update()
	{
	}
}
