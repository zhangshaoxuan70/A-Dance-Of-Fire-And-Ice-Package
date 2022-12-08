using UnityEngine;

public class HifiEnabler : MonoBehaviour
{
	public GameObject[] objectsToActivate;

	public MonoBehaviour[] scriptsToActivate;

	private void Awake()
	{
		if (Persistence.GetVisualQuality() != VisualQuality.High)
		{
			return;
		}
		if (objectsToActivate != null)
		{
			GameObject[] array = objectsToActivate;
			foreach (GameObject gameObject in array)
			{
				if (gameObject != null)
				{
					gameObject.SetActive(value: true);
				}
			}
		}
		if (scriptsToActivate == null)
		{
			return;
		}
		MonoBehaviour[] array2 = scriptsToActivate;
		foreach (MonoBehaviour monoBehaviour in array2)
		{
			if (monoBehaviour != null)
			{
				monoBehaviour.enabled = true;
			}
		}
	}
}
