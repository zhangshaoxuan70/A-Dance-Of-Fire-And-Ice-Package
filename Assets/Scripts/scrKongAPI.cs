using System;
using UnityEngine;

public class scrKongAPI : MonoBehaviour
{
	private static scrKongAPI instance;

	public static bool Connected
	{
		get;
		private set;
	}

	public static int UserId
	{
		get;
		private set;
	}

	public static string Username
	{
		get;
		private set;
	}

	public static string GameAuthToken
	{
		get;
		private set;
	}

	public void Start()
	{
		if (instance == null)
		{
			instance = this;
		}
		else if (instance != this)
		{
			UnityEngine.Object.Destroy(base.gameObject);
			return;
		}
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		base.gameObject.name = "KongregateAPI";
		Application.ExternalEval("if(typeof(kongregateUnitySupport) != 'undefined'){\n        kongregateUnitySupport.initAPI('KongregateAPI', 'OnKongregateAPILoaded');\n      };");
	}

	public void OnKongregateAPILoaded(string userInfoString)
	{
		UnityEngine.Debug.Log("Kong connected!");
		Connected = true;
		OnKongregateUserInfo(userInfoString);
	}

	public void OnKongregateUserInfo(string userInfoString)
	{
		string[] array = userInfoString.Split('|');
		int num = Convert.ToInt32(array[0]);
		string str = array[1];
		string text = array[2];
		UnityEngine.Debug.Log("Kongregate User Info: " + str + ", userId: " + num.ToString());
	}

	public static void Submit(string statisticName, int value)
	{
		if (Connected)
		{
			Application.ExternalCall("kongregate.stats.submit", statisticName, value);
		}
	}
}
