using GDMiniJSON;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace MobileMenu
{
	public class MobileMenuMap
	{
		public MobileMenuGroup rootGroup;

		public Dictionary<string, MobileMenuGroup> groupLUT;

		public Dictionary<string, MobileMenuPortal> portalLUT;

		public Transform transform;

		private Transform pivotTransform;

		private const string MapsDirectory = "MobileMenuMaps";

		public MobileMenuMap(string mapName, bool build = true)
		{
			ImportMap(mapName, build);
		}

		public void ImportMap(string mapName, bool build = true)
		{
			Dictionary<string, object> dict = Json.Deserialize(Resources.Load<TextAsset>(Path.Combine("MobileMenuMaps", mapName)).text) as Dictionary<string, object>;
			Decode(dict);
			EvaluateAllConditions();
			if (build)
			{
				Build();
			}
		}

		public void SetMapCenter(MobileMenuScreen screen)
		{
			pivotTransform.localPosition = pivotTransform.position - screen.transform.position;
		}

		public void Decode(Dictionary<string, object> dict)
		{
			groupLUT = new Dictionary<string, MobileMenuGroup>();
			portalLUT = new Dictionary<string, MobileMenuPortal>();
			foreach (object item in dict["groups"] as List<object>)
			{
				Dictionary<string, object> dict2 = item as Dictionary<string, object>;
				MobileMenuGroup mobileMenuGroup = new MobileMenuGroup();
				mobileMenuGroup.Decode(dict2);
				groupLUT.Add(mobileMenuGroup.id, mobileMenuGroup);
				foreach (MobileMenuScreen screen in mobileMenuGroup.screens)
				{
					if (screen is MobileMenuPortal)
					{
						MobileMenuPortal mobileMenuPortal = screen as MobileMenuPortal;
						portalLUT.Add(mobileMenuPortal.world, mobileMenuPortal);
					}
				}
			}
			string key = (string)dict["rootGroup"];
			rootGroup = groupLUT[key];
		}

		public void Build()
		{
			if (transform == null)
			{
				transform = new GameObject("Mobile Menu Map Container").transform;
				pivotTransform = new GameObject("Pivot").transform;
				pivotTransform.SetParent(transform);
			}
			BuildGroupRecursively(rootGroup);
			SetMapCenter(rootGroup[0]);
		}

		private MobileMenuGroup BuildGroupRecursively(MobileMenuGroup group, MoveDirection spawnDirection = MoveDirection.Up, Vector2 position = default(Vector2))
		{
			Vector2Int vector = spawnDirection.GetVector();
			bool flag = spawnDirection == MoveDirection.Left;
			position.y += group.GetHeight() / 2f * (float)vector.y;
			bool flag2 = false;
			do
			{
				group.visibleScreens = new List<MobileMenuScreen>();
				position.x += group.horizontalGap * MobileMenuScreen.GetBaseWidth() * (float)spawnDirection.GetVector().x;
				for (int i = 0; i < group.screens.Count; i++)
				{
					MobileMenuScreen mobileMenuScreen = group.screens[i];
					if (mobileMenuScreen.visible)
					{
						flag2 = true;
						group.visibleScreens.Add(mobileMenuScreen);
						float width = mobileMenuScreen.GetWidth();
						float num = (MobileMenuScreen.GetAspect() < 1.5f) ? width : (width / 2f);
						position.x += num * (float)spawnDirection.GetVector().x;
						if (!mobileMenuScreen.transform)
						{
							mobileMenuScreen.Instantiate();
						}
						mobileMenuScreen.transform.position = position;
						mobileMenuScreen.transform.SetParent(pivotTransform);
						mobileMenuScreen.parentGroup = group;
						spawnDirection = (flag ? MoveDirection.Left : MoveDirection.Right);
						position.x += mobileMenuScreen.GetWidth() / 2f * (float)spawnDirection.GetVector().x;
					}
				}
				if (!flag2)
				{
					if (!group.groupToSpawn.ContainsKey(spawnDirection))
					{
						return null;
					}
					group = groupLUT[group.groupToSpawn[spawnDirection]];
				}
			}
			while (!flag2);
			group.linkedGroup = new Dictionary<MoveDirection, MobileMenuGroup>();
			foreach (KeyValuePair<MoveDirection, string> item in group.groupToSpawn)
			{
				MoveDirection key = item.Key;
				string value = item.Value;
				Vector2Int vector2 = key.GetVector();
				List<MobileMenuScreen> visibleScreens = group.visibleScreens;
				int count = visibleScreens.Count;
				int offset = ((key == MoveDirection.Right) ? new Index(1, fromEnd: true) : ((Index)0)).GetOffset(count);
				MobileMenuScreen mobileMenuScreen2 = visibleScreens[offset];
				Vector3 position2 = mobileMenuScreen2.transform.position;
				position2.x += mobileMenuScreen2.GetWidth() / 2f * (float)vector2.x;
				position2.y += group.GetHeight() / 2f * (float)vector2.y;
				MobileMenuGroup group2 = groupLUT[value];
				MobileMenuGroup mobileMenuGroup = BuildGroupRecursively(group2, key, position2);
				if (mobileMenuGroup != null)
				{
					mobileMenuGroup.linkedGroup.Add(key.Invert(), group);
					group.linkedGroup.Add(key, mobileMenuGroup);
				}
			}
			return group;
		}

		public void EvaluateAllConditions()
		{
			foreach (MobileMenuGroup value in groupLUT.Values)
			{
				foreach (MobileMenuScreen screen in value.screens)
				{
					bool flag = screen.visible = EvaluateConditions(screen.visibilityConditions);
				}
			}
		}

		public static bool EvaluateConditions(string[] conditions)
		{
			bool flag = true;
			if (conditions != null)
			{
				for (int i = 0; i < conditions.Length; i++)
				{
					string[] array = conditions[i].Split(':');
					string text = array[0];
					string text2 = array[1];
					switch (text)
					{
					case "completed":
						flag &= Persistence.IsWorldComplete(GCNS.worldData[text2].index);
						break;
					case "speedTrial":
						flag &= Persistence.IsSpeedTrialComplete(GCNS.worldData[text2].index);
						break;
					case "stage":
						flag &= (Persistence.GetOverallProgressStage() >= int.Parse(text2));
						break;
					case "condition":
						flag &= CheckCustomCondition(text2);
						break;
					}
				}
			}
			return flag;
		}

		private static bool CheckCustomCondition(string world)
		{
			if (!(world == "XF"))
			{
				if (!(world == "XC"))
				{
					if (!(world == "XR"))
					{
						if (world == "XH")
						{
							return Persistence.GetUnlockedXH();
						}
						return false;
					}
					return Persistence.GetUnlockedXR();
				}
				return Persistence.GetUnlockedXC();
			}
			return Persistence.GetUnlockedXF();
		}
	}
}
