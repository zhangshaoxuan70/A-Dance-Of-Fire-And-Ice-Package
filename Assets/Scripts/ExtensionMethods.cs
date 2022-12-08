using System;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

public static class ExtensionMethods
{
	public static Color WithAlpha(this Color color, float alpha)
	{
		return new Color(color.r, color.g, color.b, alpha);
	}

	public static Vector2 xy(this Vector3 v)
	{
		return new Vector2(v.x, v.y);
	}

	public static Vector3 WithX(this Vector3 v, float x)
	{
		return new Vector3(x, v.y, v.z);
	}

	public static Vector3 WithY(this Vector3 v, float y)
	{
		return new Vector3(v.x, y, v.z);
	}

	public static Vector3 WithZ(this Vector3 v, float z)
	{
		return new Vector3(v.x, v.y, z);
	}

	public static Vector2 WithX(this Vector2 v, float x)
	{
		return new Vector2(x, v.y);
	}

	public static Vector2 WithY(this Vector2 v, float y)
	{
		return new Vector2(v.x, y);
	}

	public static Vector3 WithZ(this Vector2 v, float z)
	{
		return new Vector3(v.x, v.y, z);
	}

	public static Vector3 NearestPointOnAxis(this Vector3 axisDirection, Vector3 point, bool isNormalized = false)
	{
		if (!isNormalized)
		{
			axisDirection.Normalize();
		}
		float d = Vector3.Dot(point, axisDirection);
		return axisDirection * d;
	}

	public static Vector3 NearestPointOnLine(this Vector3 lineDirection, Vector3 point, Vector3 pointOnLine, bool isNormalized = false)
	{
		if (!isNormalized)
		{
			lineDirection.Normalize();
		}
		float d = Vector3.Dot(point - pointOnLine, lineDirection);
		return pointOnLine + lineDirection * d;
	}

	public static string Truncate(this string value, int maxLength)
	{
		if (string.IsNullOrEmpty(value))
		{
			return value;
		}
		if (value.Length > maxLength)
		{
			return value.Substring(0, maxLength);
		}
		return value;
	}

	public static string ToString(this object anObject, string aFormat)
	{
		return anObject.ToString(aFormat, null);
	}

	public static string ToString(this object anObject, string aFormat, IFormatProvider formatProvider)
	{
		StringBuilder stringBuilder = new StringBuilder();
		Type type = anObject.GetType();
		MatchCollection matchCollection = new Regex("({)([^}]+)(})", RegexOptions.IgnoreCase).Matches(aFormat);
		int num = 0;
		foreach (Match item in matchCollection)
		{
			Group group = item.Groups[2];
			int length = group.Index - num - 1;
			stringBuilder.Append(aFormat.Substring(num, length));
			string empty = string.Empty;
			string text = string.Empty;
			int num2 = group.Value.IndexOf(":");
			if (num2 == -1)
			{
				empty = group.Value;
			}
			else
			{
				empty = group.Value.Substring(0, num2);
				text = group.Value.Substring(num2 + 1);
			}
			PropertyInfo property = type.GetProperty(empty);
			Type type2 = null;
			object target = null;
			if (property != null)
			{
				type2 = property.PropertyType;
				target = property.GetValue(anObject, null);
			}
			else
			{
				FieldInfo field = type.GetField(empty);
				if (field != null)
				{
					type2 = field.FieldType;
					target = field.GetValue(anObject);
				}
			}
			if (type2 != null)
			{
				string empty2 = string.Empty;
				empty2 = ((!(text == string.Empty)) ? (type2.InvokeMember("ToString", BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.InvokeMethod, null, target, new object[2]
				{
					text,
					formatProvider
				}) as string) : (type2.InvokeMember("ToString", BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.InvokeMethod, null, target, null) as string));
				stringBuilder.Append(empty2);
			}
			else
			{
				stringBuilder.Append("{");
				stringBuilder.Append(group.Value);
				stringBuilder.Append("}");
			}
			num = group.Index + group.Length + 1;
		}
		if (num < aFormat.Length)
		{
			stringBuilder.Append(aFormat.Substring(num));
		}
		return stringBuilder.ToString();
	}

	public static void CopyToClipboard(this string str)
	{
		GUIUtility.systemCopyBuffer = str;
	}
}
