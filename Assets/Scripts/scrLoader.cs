using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;

public class scrLoader : ADOBase
{
	public GameObject planets;

	public Transform ring;

	public GameObject canvas;

	private float timeElapsed;

	private bool objectsShown;

	private AsyncOperation load;

	private void Awake()
	{
		planets.gameObject.SetActive(value: false);
		canvas.gameObject.SetActive(value: false);
		if (GCS.sceneToLoad != null)
		{
			string sceneToLoad = GCS.sceneToLoad;
			printe("scene loading: " + sceneToLoad);
			if (sceneToLoad.StartsWith("scnTaroMenu") || sceneToLoad.IsTaro())
			{
				printe("loading async through addressables! " + sceneToLoad);
				Addressables.LoadSceneAsync(sceneToLoad);
			}
			else
			{
				load = SceneManager.LoadSceneAsync(GCS.sceneToLoad, LoadSceneMode.Additive);
			}
		}
	}

	private void Update()
	{
		if (!objectsShown)
		{
			timeElapsed += Time.unscaledDeltaTime;
			if (timeElapsed > 1f)
			{
				planets.gameObject.SetActive(value: true);
				canvas.gameObject.SetActive(value: true);
				objectsShown = true;
			}
			if (load != null && load.isDone)
			{
				base.gameObject.SetActive(value: false);
				SceneManager.SetActiveScene(SceneManager.GetSceneByName(GCS.sceneToLoad));
			}
			ring.Rotate(Vector3.back, -30f * Time.unscaledDeltaTime);
		}
	}
}
