using UnityEngine;
using UnityEngine.UI;

public class scrButtonURL : MonoBehaviour
{
	public string link;

	public bool localized;

	public virtual void Start()
	{
		base.transform.GetComponent<Button>().onClick.AddListener(delegate
		{
			OpenURL();
		});
	}

	public virtual void OpenURL()
	{
		scrController.instance.TogglePauseGame();
		Application.OpenURL(localized ? RDString.Get(link) : link);
	}
}
