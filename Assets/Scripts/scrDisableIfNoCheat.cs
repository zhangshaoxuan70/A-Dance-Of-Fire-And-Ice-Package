using UnityEngine;

public class scrDisableIfNoCheat : MonoBehaviour
{
	private void Start()
	{
		if (!RDC.debug)
		{
			base.gameObject.SetActive(value: false);
		}
	}

	private void Update()
	{
	}
}
