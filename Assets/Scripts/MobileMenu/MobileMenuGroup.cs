using System;
using System.Collections.Generic;
using UnityEngine;

namespace MobileMenu
{
	public class MobileMenuGroup
	{
		[Serializable]
		public struct BackgroundTheme
		{
			public Color? backgroundColor;

			public Color? gradientColor;

			public Color? cloudColor;

			public BackgroundTheme WithDefaults(BackgroundTheme defaultTheme)
			{
				backgroundColor = (backgroundColor ?? defaultTheme.backgroundColor);
				gradientColor = (gradientColor ?? defaultTheme.gradientColor);
				cloudColor = (cloudColor ?? defaultTheme.cloudColor);
				return this;
			}
		}

		public string id;

		public List<MobileMenuScreen> screens;

		public List<MobileMenuScreen> visibleScreens;

		public string captionKey;

		public float horizontalGap;

		public float height = 1f;

		public Dictionary<MoveDirection, MobileMenuGroup> linkedGroup;

		public Dictionary<MoveDirection, string> groupToSpawn;

		public BackgroundTheme theme;

		public MobileMenuScreen this[int index] => visibleScreens[index];

		public IEnumerator<MobileMenuScreen> GetEnumerator()
		{
			return visibleScreens.GetEnumerator();
		}

		public void Decode(Dictionary<string, object> dict)
		{
			id = (dict["name"] as string);
			screens = new List<MobileMenuScreen>();
			groupToSpawn = new Dictionary<MoveDirection, string>();
			foreach (MoveDirection value in Enum.GetValues(typeof(MoveDirection)))
			{
				if (dict.TryGetValueAs(value.ToString().ToLower() + "Group", out string valueAs))
				{
					groupToSpawn.Add(value, valueAs);
				}
			}
			dict.TryGetValueAs("horizontalGap", out horizontalGap);
			dict.TryGetValueAs("caption", out captionKey);
			if (dict.TryGetValueAs("backgroundColor", out string valueAs2))
			{
				theme.backgroundColor = valueAs2.HexToColor();
			}
			if (dict.TryGetValueAs("gradientColor", out string valueAs3))
			{
				theme.gradientColor = valueAs3.HexToColor();
			}
			if (dict.TryGetValueAs("cloudColor", out string valueAs4))
			{
				theme.cloudColor = valueAs4.HexToColor();
			}
			foreach (object item in dict["screens"] as List<object>)
			{
				Dictionary<string, object> dictionary = item as Dictionary<string, object>;
				MobileMenuScreen mobileMenuScreen = MobileMenuScreen.New((string)dictionary["type"]);
				if (mobileMenuScreen != null)
				{
					mobileMenuScreen.Decode(dictionary);
					screens.Add(mobileMenuScreen);
				}
			}
		}

		public float GetHeight()
		{
			return height * 2.5f * Camera.main.orthographicSize;
		}
	}
}
