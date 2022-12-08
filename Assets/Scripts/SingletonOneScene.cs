using UnityEngine;

public abstract class SingletonOneScene<T> : MonoBehaviour where T : SingletonOneScene<T>
{
	private static T _instance;

	public static T Instance
	{
		get
		{
			if ((Object)_instance == (Object)null)
			{
				_instance = (T)UnityEngine.Object.FindObjectOfType(typeof(T));
			}
			if ((Object)_instance == (Object)null)
			{
				UnityEngine.Debug.LogError("An instance of " + typeof(T)?.ToString() + " is needed in the scene, but there is none.");
			}
			return _instance;
		}
	}

	protected void Awake()
	{
		if ((Object)_instance == (Object)null)
		{
			_instance = (this as T);
		}
		else if (_instance != this)
		{
			UnityEngine.Object.Destroy(this);
		}
	}
}
