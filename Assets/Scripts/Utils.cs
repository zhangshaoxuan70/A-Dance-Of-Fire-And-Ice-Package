using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils
{
	public static Vector2 Add(this Vector2 origin, float angle, float distance)
	{
		return new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * distance + origin;
	}

	public static void AddMany<T>(this List<T> list, params T[] values)
	{
		list.AddRange(values);
	}

	public static float ToDeg(this float angle)
	{
		return angle * 57.29578f;
	}

	public static int WithinArray(this int value, ICollection array)
	{
		return Mathf.Clamp(value, 0, array.Count - 1);
	}

	public static int WithinArray(this int value, Array array)
	{
		return Mathf.Clamp(value, 0, array.Length - 1);
	}

	public static int Conn(this Vector3 v)
	{
		return (int)v.z;
	}
}
