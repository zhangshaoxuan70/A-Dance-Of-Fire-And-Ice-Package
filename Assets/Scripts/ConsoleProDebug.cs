using UnityEngine;

public static class ConsoleProDebug
{
	public static void Clear()
	{
	}

	public static void LogToFilter(string inLog, string inFilterName)
	{
		UnityEngine.Debug.Log(inLog + "\nCPAPI:{\"cmd\":\"Filter\" \"name\":\"" + inFilterName + "\"}");
	}

	public static void Watch(string inName, string inValue)
	{
		UnityEngine.Debug.Log(inName + " : " + inValue + "\nCPAPI:{\"cmd\":\"Watch\" \"name\":\"" + inName + "\"}");
	}

	public static void Search(string inText)
	{
		UnityEngine.Debug.Log("\nCPAPI:{\"cmd\":\"Search\" \"text\":\"" + inText + "\"}");
	}
}
