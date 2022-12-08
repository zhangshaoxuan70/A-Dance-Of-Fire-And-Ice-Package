using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace ADOFAI
{
	public class LevelEvent
	{
		public enum DecodeResult
		{
			Success,
			NoTypeFound,
			NeedsTaroDLC
		}

		private const int NoFloor = -1;

		public int floor = -1;

		public LevelEventType eventType;

		public Dictionary<string, object> data;

		public Dictionary<string, bool> disabled;

		public LevelEventInfo info;

		public bool active = true;

		public bool visible = true;

		public bool forceHide;

		public object this[string key]
		{
			get
			{
				if (data.TryGetValue(key, out object value))
				{
					return value;
				}
				UnityEngine.Debug.Log($"Event doesn't contain key {key}\n{this}");
				return null;
			}
			set
			{
				data[key] = value;
			}
		}

		public float GetFloat(string key)
		{
			if (data.TryGetValue(key, out object value))
			{
				return (float)value;
			}
			return 0f;
		}

		public string GetString(string key)
		{
			if (data.TryGetValue(key, out object value))
			{
				return (string)value;
			}
			return null;
		}

		public int GetInt(string key)
		{
			if (data.TryGetValue(key, out object value))
			{
				return (int)value;
			}
			return 0;
		}

		public LevelEvent(int newFloor, LevelEventType type)
			: this(newFloor, type, null, null, null, visible: true)
		{
		}

		public LevelEvent(int newFloor, LevelEventType type, LevelEventInfo customInfo)
			: this(newFloor, type, customInfo, null, null, visible: true)
		{
		}

		public LevelEvent(int newFloor, LevelEventType type, LevelEventInfo customInfo, Dictionary<string, object> data, Dictionary<string, bool> disabled, bool visible)
		{
			floor = newFloor;
			eventType = type;
			this.visible = visible;
			string key = GCS.levelEventTypeString[eventType];
			info = ((customInfo != null) ? customInfo : GCS.levelEventsInfo[key]);
			if (data == null)
			{
				this.data = new Dictionary<string, object>();
				this.disabled = new Dictionary<string, bool>();
				foreach (string key2 in info.propertiesInfo.Keys)
				{
					PropertyInfo propertyInfo = info.propertiesInfo[key2];
					this.data[key2] = propertyInfo.value_default;
					this.disabled[key2] = !propertyInfo.startEnabled;
				}
			}
			else
			{
				this.data = new Dictionary<string, object>(data);
				this.disabled = new Dictionary<string, bool>(disabled);
			}
		}

		public LevelEvent(Dictionary<string, object> dict)
		{
			Decode(dict);
		}

		public DecodeResult Decode(Dictionary<string, object> dict, string explicitEventType = null, bool isGlobal = false)
		{
			string text = (explicitEventType != null) ? explicitEventType : (dict["eventType"] as string);
			eventType = RDUtils.ParseEnum(text, LevelEventType.None);
			if (eventType == LevelEventType.None)
			{
				return DecodeResult.NoTypeFound;
			}
			info = (isGlobal ? GCS.settingsInfo[text] : GCS.levelEventsInfo[text]);
			if (!info.taroDLCCheck)
			{
				return DecodeResult.NeedsTaroDLC;
			}
			if (dict.ContainsKey("floor"))
			{
				floor = Convert.ToInt32(dict["floor"]);
			}
			int num;
			if (dict.TryGetValue("active", out object value) && value is bool)
			{
				bool flag = (bool)value;
				num = (flag ? 1 : 0);
			}
			else
			{
				num = 1;
			}
			active = ((byte)num != 0);
			int num2;
			if (dict.TryGetValue("visible", out object value2) && value2 is bool)
			{
				bool flag2 = (bool)value2;
				num2 = (flag2 ? 1 : 0);
			}
			else
			{
				num2 = 1;
			}
			visible = ((byte)num2 != 0);
			FixDefaultValues(dict);
			data = new Dictionary<string, object>();
			disabled = new Dictionary<string, bool>();
			foreach (KeyValuePair<string, PropertyInfo> item3 in info.propertiesInfo)
			{
				string key = item3.Key;
				PropertyInfo value3 = item3.Value;
				if (!(key == "floor") && value3.encode)
				{
					disabled.Add(key, !dict.ContainsKey(key));
					if (!dict.ContainsKey(key))
					{
						data.Add(key, value3.value_default);
					}
					else
					{
						object obj = dict[key];
						if (value3.type == PropertyType.Enum)
						{
							if (obj is int)
							{
								data.Add(key, Enum.ToObject(value3.enumType, (int)obj));
							}
							else
							{
								data.Add(key, Enum.Parse(value3.enumType, obj as string));
							}
						}
						else if (value3.type == PropertyType.Float)
						{
							data.Add(key, Convert.ToSingle(obj));
						}
						else if (value3.type == PropertyType.Int || value3.type == PropertyType.Rating)
						{
							data.Add(key, Convert.ToInt32(obj));
						}
						else if (value3.type == PropertyType.Vector2)
						{
							if (obj is float)
							{
								float num3 = (float)obj;
								data.Add(key, new Vector2(num3, num3));
							}
							else if (obj is int)
							{
								int num4 = (int)obj;
								data.Add(key, new Vector2(num4, num4));
							}
							else
							{
								List<object> obj2 = obj as List<object>;
								float x = Convert.ToSingle(obj2[0]);
								float y = Convert.ToSingle(obj2[1]);
								data.Add(key, new Vector2(x, y));
							}
						}
						else if (value3.type == PropertyType.Tile)
						{
							List<object> list = obj as List<object>;
							int item = Convert.ToInt32(list[0]);
							TileRelativeTo item2 = (TileRelativeTo)Enum.Parse(typeof(TileRelativeTo), list[1].ToString());
							data.Add(key, new Tuple<int, TileRelativeTo>(item, item2));
						}
						else if (value3.type == PropertyType.Array)
						{
							data.Add(key, RDEditorUtils.DecodeModsArray(obj));
						}
						else if (value3.type == PropertyType.List)
						{
							data.Add(key, obj);
						}
						else
						{
							data.Add(key, obj);
						}
					}
				}
			}
			return DecodeResult.Success;
		}

		private void FixDefaultValues(Dictionary<string, object> dict)
		{
			foreach (string item in new List<string>(info.propertiesInfo.Keys))
			{
				if (eventType == LevelEventType.AddDecoration && item == "decorationImage" && !dict.ContainsKey(item))
				{
					dict["decorationImage"] = dict["decText"];
					dict.Remove("decText");
				}
				if ((eventType == LevelEventType.AddDecoration || eventType == LevelEventType.AddText) && item == "parallax" && !dict.ContainsKey(item))
				{
					int num = (int)dict["depth"];
					num = ((num != 1 && num != -1) ? num : 0);
					dict["parallax"] = num;
				}
			}
		}

		public string Encode(bool settings = false)
		{
			StringBuilder stringBuilder = new StringBuilder();
			int count = data.Keys.Count;
			if (!settings)
			{
				if (floor != -1)
				{
					stringBuilder.Append(RDEditorUtils.EncodeInt("floor", floor));
				}
				stringBuilder.Append(RDEditorUtils.EncodeString("eventType", eventType.ToString(), count == 0));
				if (!active)
				{
					stringBuilder.Append(RDEditorUtils.EncodeBool("active", active, count == 0));
				}
				if (!visible)
				{
					stringBuilder.Append(RDEditorUtils.EncodeBool("visible", visible, count == 0));
				}
			}
			int num = 0;
			foreach (string key in data.Keys)
			{
				object obj = data[key];
				bool lastValue = !settings && num == count - 1;
				if (info.propertiesInfo.ContainsKey(key) && (!disabled[key] || !info.propertiesInfo[key].canBeDisabled))
				{
					PropertyInfo propertyInfo = info.propertiesInfo[key];
					if (propertyInfo.encode && !(key == "floor"))
					{
						PropertyType type = propertyInfo.type;
						switch (type)
						{
						case PropertyType.Int:
						case PropertyType.Rating:
							stringBuilder.Append(RDEditorUtils.EncodeInt(key, (int)data[key], lastValue));
							break;
						case PropertyType.String:
						case PropertyType.LongString:
						case PropertyType.File:
							stringBuilder.Append(RDEditorUtils.EncodeString(key, EscapeTextForJSON((string)data[key]), lastValue));
							break;
						case PropertyType.Color:
							stringBuilder.Append(RDEditorUtils.EncodeString(key, (string)data[key], lastValue));
							break;
						case PropertyType.Bool:
							stringBuilder.Append(RDEditorUtils.EncodeBool(key, (bool)data[key], lastValue));
							break;
						case PropertyType.Float:
							stringBuilder.Append(RDEditorUtils.EncodeFloat(key, Convert.ToSingle(data[key]), lastValue));
							break;
						case PropertyType.Enum:
							stringBuilder.Append(RDEditorUtils.EncodeString(key, data[key].ToString(), lastValue));
							break;
						case PropertyType.Vector2:
							stringBuilder.Append(RDEditorUtils.EncodeVector2(key, (Vector2)data[key], lastValue));
							break;
						case PropertyType.Tile:
							stringBuilder.Append(RDEditorUtils.EncodeTile(key, data[key] as Tuple<int, TileRelativeTo>, lastValue));
							break;
						case PropertyType.Array:
							stringBuilder.Append(RDEditorUtils.EncodeModsArray(key, (object[])data[key], lastValue));
							break;
						default:
							UnityEngine.Debug.LogWarning(key + " not parsed! it is type: " + type.ToString());
							break;
						case PropertyType.Export:
							break;
						}
						if (settings)
						{
							stringBuilder.Append("\n");
						}
						num++;
					}
				}
			}
			if (settings)
			{
				stringBuilder.Length = Math.Max(stringBuilder.Length - 3, 0);
			}
			return stringBuilder.ToString();
		}

		public static string EscapeTextForJSON(string text)
		{
			if (string.IsNullOrEmpty(text))
			{
				return "";
			}
			return text.Replace("\\", "\\\\").Replace("\n", "\\n").Replace("\t", "\t")
				.Replace("\"", "\\\"");
		}

		public LevelEvent Copy()
		{
			return new LevelEvent(floor, eventType, info, data, disabled, visible);
		}

		public override string ToString()
		{
			string text = "type: " + eventType.ToString() + "\n";
			foreach (KeyValuePair<string, object> datum in data)
			{
				text += $"key {datum.Key}, value {datum.Value}\n";
			}
			return text;
		}
	}
}
