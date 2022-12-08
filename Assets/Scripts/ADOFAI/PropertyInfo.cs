using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ADOFAI
{
	public class PropertyInfo
	{
		public string name;

		public int order;

		public PropertyType type;

		public MonoBehaviour control;

		public ControlType controlType;

		public Type enumType;

		public string enumTypeString;

		public string unit;

		public string placeholder;

		public string customLocalizationKey;

		public bool pro;

		public LevelEventInfo levelEventInfo;

		public bool canBeDisabled;

		public bool startEnabled = true;

		public List<Tuple<string, string>> disableIfVals = new List<Tuple<string, string>>();

		public List<Tuple<string, string>> enableIfVals = new List<Tuple<string, string>>();

		public bool isEnabled = true;

		public bool hasRandomValue;

		public string randModeKey;

		public string randValueKey;

		public object value_default;

		public int int_min;

		public int int_max;

		public float float_min;

		public float float_max;

		public bool color_usesAlpha;

		public int string_minLength;

		public int string_maxLength;

		public bool string_needsUnicode;

		public FileType fileType;

		public Array enumValidation;

		public Vector2 minVec;

		public Vector2 maxVec;

		public bool required;

		public bool encode;

		public bool affectsFloors = true;

		private bool isFloor;

		public PropertyInfo(Dictionary<string, object> dict, LevelEventInfo levelEventInfo)
		{
			this.levelEventInfo = levelEventInfo;
			name = (dict["name"] as string);
			string text = dict["type"] as string;
			object obj = null;
			bool flag = false;
			if (dict.ContainsKey("default"))
			{
				obj = dict["default"];
				flag = true;
			}
			if (name == "floor")
			{
				isFloor = true;
			}
			if (dict.ContainsKey("unit"))
			{
				unit = (dict["unit"] as string);
			}
			if (dict.ContainsKey("key"))
			{
				customLocalizationKey = (dict["key"] as string);
			}
			if (dict.ContainsKey("pro"))
			{
				pro = (bool)dict["pro"];
			}
			if (dict.ContainsKey("canBeDisabled"))
			{
				canBeDisabled = (bool)dict["canBeDisabled"];
			}
			if (canBeDisabled)
			{
				startEnabled = false;
				if (dict.ContainsKey("startEnabled"))
				{
					startEnabled = (bool)dict["startEnabled"];
				}
			}
			ControlType controlType = ControlType.NotAssigned;
			dict.ContainsKey("hasRandomValue");
			if (dict.ContainsKey("enableIf"))
			{
				string[] array = RDEditorUtils.DecodeStringArray(dict["enableIf"]);
				if (array.Length % 2 != 0)
				{
					UnityEngine.Debug.Log("Not all keys have values");
				}
				else
				{
					for (int i = 0; i < array.Length; i += 2)
					{
						string item = array[i];
						string item2 = array[i + 1];
						enableIfVals.Add(new Tuple<string, string>(item, item2));
					}
				}
			}
			if (dict.ContainsKey("disableIf"))
			{
				string[] array2 = RDEditorUtils.DecodeStringArray(dict["disableIf"]);
				if (array2.Length % 2 != 0)
				{
					UnityEngine.Debug.Log("Not all keys have values");
				}
				else
				{
					for (int j = 0; j < array2.Length; j += 2)
					{
						string item3 = array2[j];
						string item4 = array2[j + 1];
						disableIfVals.Add(new Tuple<string, string>(item3, item4));
					}
				}
			}
			if (dict.ContainsKey("required"))
			{
				required = (bool)dict["required"];
			}
			encode = (!dict.ContainsKey("encode") || (bool)dict["encode"]);
			affectsFloors = (bool)dict.GetValueOrDefault("affectsFloors", false);
			if (text == "Float")
			{
				type = PropertyType.Float;
				value_default = (flag ? Convert.ToSingle(obj) : 0f);
				float_min = (dict.ContainsKey("min") ? Convert.ToSingle(dict["min"]) : float.NegativeInfinity);
				float_max = (dict.ContainsKey("max") ? Convert.ToSingle(dict["max"]) : float.PositiveInfinity);
				controlType = ControlType.InputField;
			}
			else if (text == "Bool")
			{
				type = PropertyType.Bool;
				value_default = (flag && (bool)obj);
				controlType = ControlType.ToggleGroup;
			}
			else if (text == "Int")
			{
				type = PropertyType.Int;
				value_default = (flag ? Convert.ToInt32(obj) : 0);
				int_min = (dict.ContainsKey("min") ? Convert.ToInt32(dict["min"]) : int.MinValue);
				int_max = (dict.ContainsKey("max") ? Convert.ToInt32(dict["max"]) : int.MaxValue);
				controlType = ControlType.InputField;
			}
			else if (text == "Color")
			{
				type = PropertyType.Color;
				color_usesAlpha = (!dict.ContainsKey("usesAlpha") || Convert.ToBoolean(dict["usesAlpha"]));
				string text2 = color_usesAlpha ? "ffffffff" : "ffffff";
				value_default = (flag ? (obj as string) : text2);
				controlType = ControlType.ColorPicker;
			}
			else if (text == "File")
			{
				type = PropertyType.File;
				value_default = (flag ? (obj as string) : string.Empty);
				fileType = (dict["fileType"] as string).ToEnum(FileType.Audio);
				controlType = ControlType.File;
			}
			else if (text == "String")
			{
				type = PropertyType.String;
				value_default = (flag ? RDString.Get(obj as string) : string.Empty);
				string_minLength = (dict.ContainsKey("minLength") ? Convert.ToInt32(dict["minLength"]) : int.MinValue);
				string_maxLength = (dict.ContainsKey("maxLength") ? Convert.ToInt32(dict["maxLength"]) : int.MaxValue);
				string_needsUnicode = (!dict.ContainsKey("needsUnicode") || Convert.ToBoolean(dict["needsUnicode"]));
				controlType = ControlType.InputField;
			}
			else if (text == "Text")
			{
				type = PropertyType.LongString;
				value_default = (flag ? RDString.Get(obj as string) : string.Empty);
				controlType = ControlType.LongInputField;
			}
			else if (text.StartsWith("Enum:"))
			{
				type = PropertyType.Enum;
				enumTypeString = text.Replace("Enum:", string.Empty);
				if (enumTypeString == "Ease")
				{
					enumType = typeof(Ease);
				}
				else
				{
					enumType = Type.GetType(enumTypeString);
				}
				value_default = (flag ? Enum.Parse(enumType, obj as string) : ((object)0));
				if (!flag)
				{
					UnityEngine.Debug.LogWarning("Default value for type: " + enumType?.ToString() + " doesn't exist; it should for enums.");
				}
				controlType = ControlType.Dropdown;
			}
			else if (text == "Vector2")
			{
				type = PropertyType.Vector2;
				if (flag)
				{
					List<object> obj2 = obj as List<object>;
					float x = Convert.ToSingle(obj2[0]);
					float y = Convert.ToSingle(obj2[1]);
					value_default = new Vector2(x, y);
				}
				else
				{
					value_default = new Vector2(0f, 0f);
				}
				if (dict.ContainsKey("min"))
				{
					List<object> obj3 = dict["min"] as List<object>;
					float x = Convert.ToSingle(obj3[0]);
					float y = Convert.ToSingle(obj3[1]);
					minVec = new Vector2(x, y);
				}
				else
				{
					minVec = new Vector2(float.NegativeInfinity, float.NegativeInfinity);
				}
				if (dict.ContainsKey("max"))
				{
					List<object> obj4 = dict["max"] as List<object>;
					float x = Convert.ToSingle(obj4[0]);
					float y = Convert.ToSingle(obj4[1]);
					maxVec = new Vector2(x, y);
				}
				else
				{
					maxVec = new Vector2(float.PositiveInfinity, float.PositiveInfinity);
				}
			}
			else if (text == "Tile")
			{
				type = PropertyType.Tile;
				enumType = typeof(TileRelativeTo);
				if (flag)
				{
					List<object> list = obj as List<object>;
					int item5 = Convert.ToInt32(list[0]);
					TileRelativeTo item6 = (TileRelativeTo)Enum.Parse(typeof(TileRelativeTo), Convert.ToString(list[1]));
					value_default = new Tuple<int, TileRelativeTo>(item5, item6);
				}
				else
				{
					value_default = new Tuple<int, TileRelativeTo>(0, TileRelativeTo.ThisTile);
				}
				int_min = (dict.ContainsKey("min") ? Convert.ToInt32(dict["min"]) : int.MinValue);
				int_max = (dict.ContainsKey("max") ? Convert.ToInt32(dict["max"]) : int.MaxValue);
			}
			else if (text == "Export")
			{
				type = PropertyType.Export;
			}
			else if (text == "Rating")
			{
				type = PropertyType.Rating;
				value_default = (flag ? Convert.ToInt32(obj) : 0);
			}
			else if (text == "Array")
			{
				type = PropertyType.Array;
				value_default = (flag ? obj : new object[0]);
			}
			else if (text == "List")
			{
				type = PropertyType.List;
			}
			else
			{
				UnityEngine.Debug.LogWarning("didn't recognize type: " + text);
			}
			if (dict.ContainsKey("control") && controlType == ControlType.NotAssigned)
			{
				this.controlType = RDUtils.ParseEnum(dict["control"] as string, ControlType.NotAssigned);
			}
			string key = "editor." + levelEventInfo.name + "." + name + ".placeholder";
			bool exists = false;
			string withCheck = RDString.GetWithCheck(key, out exists);
			if (exists)
			{
				placeholder = withCheck;
			}
		}

		public float Validate(float value)
		{
			return Mathf.Clamp(value, float_min, float_max);
		}

		public int Validate(int value)
		{
			int min = (!isFloor) ? int_min : 0;
			int max = isFloor ? (CustomLevel.instance.levelMaker.listFloors.Count - 1) : int_max;
			return Mathf.Clamp(value, min, max);
		}

		public Vector2 Validate(Vector2 value)
		{
			float value2 = value.x;
			float value3 = value.y;
			float x = Mathf.Clamp(value2, minVec.x, maxVec.x);
			float y = Mathf.Clamp(value3, minVec.y, maxVec.y);
			return new Vector2(x, y);
		}
	}
}
