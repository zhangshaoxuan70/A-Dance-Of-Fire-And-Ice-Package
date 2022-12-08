using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class ffxCallMethod : ffxPlusBase
{
	public string methodName;

	private MethodInfo method;

	private bool hasArguments;

	private string[] argString;

	private object instance;

	private FieldInfo field;

	private bool setup;

	public static string[] GetParameters(string text)
	{
		text = text.Trim();
		if (!text.StartsWith("(") || !text.EndsWith(")"))
		{
			return null;
		}
		if (text.Length == 2)
		{
			return null;
		}
		text = text.Trim('(', ')');
		return text.Split(',');
	}

	public void Setup()
	{
		if (!Application.isPlaying)
		{
			return;
		}
		methodName = methodName.TrimAllSpaces();
		Type type = null;
		Level level = ADOBase.controller.level;
		if (level == null)
		{
			printe("level is null");
		}
		Type typeFromHandle = typeof(Level);
		if (methodName.IndexOf("(") == -1 || methodName.IndexOf("=") != -1)
		{
			return;
		}
		string name = methodName;
		string text = null;
		bool flag = false;
		int num = methodName.IndexOf("(");
		if (num > 0)
		{
			name = methodName.Substring(0, num);
			text = methodName.Substring(num, methodName.Length - num);
			flag = true;
		}
		if (type != null)
		{
			method = type.GetMethod(name);
		}
		if (method == null && level != null)
		{
			method = level.GetType().GetMethod(name);
		}
		if (method == null)
		{
			method = typeof(Level).GetMethod(name);
		}
		if (method != null && instance == null)
		{
			instance = level;
		}
		if (method == null)
		{
			method = typeFromHandle.GetMethod(name);
		}
		if (method != null && instance == null)
		{
			instance = level;
		}
		if (flag)
		{
			argString = GetParameters(text);
			if (argString != null)
			{
				hasArguments = true;
			}
		}
		if (method == null)
		{
			UnityEngine.Debug.LogWarning("CallCustomMethod: Method " + methodName + " doesn't exist");
		}
	}

	public override void StartEffect()
	{
		AdjustDurationForHardbake();
		if (!setup)
		{
			Setup();
		}
		if (!(method != null))
		{
			return;
		}
		List<object> list = new List<object>();
		if (hasArguments)
		{
			string[] array = argString;
			foreach (string text in array)
			{
				if (text.StartsWith("str:"))
				{
					list.Add(RDEditorUtils.DecodeString(text).Remove(0, 4));
				}
				else if (text.Contains("true"))
				{
					list.Add(true);
				}
				else if (text.Contains("false"))
				{
					list.Add(false);
				}
				else if (text.Contains("."))
				{
					list.Add(RDEditorUtils.DecodeFloat(text));
				}
				else
				{
					list.Add(RDEditorUtils.DecodeInt(text));
				}
			}
		}
		method.Invoke(instance, hasArguments ? list.ToArray() : null);
	}
}
