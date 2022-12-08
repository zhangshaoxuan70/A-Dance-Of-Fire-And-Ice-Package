using SA.GoogleDoc;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public static class RDString
{
	public static SystemLanguage[] AvailableLanguages = new SystemLanguage[14]
	{
		SystemLanguage.ChineseSimplified,
		SystemLanguage.ChineseTraditional,
		SystemLanguage.Korean,
		SystemLanguage.Japanese,
		SystemLanguage.English,
		SystemLanguage.Spanish,
		SystemLanguage.Portuguese,
		SystemLanguage.French,
		SystemLanguage.Polish,
		SystemLanguage.Romanian,
		SystemLanguage.Russian,
		SystemLanguage.Vietnamese,
		SystemLanguage.Czech,
		SystemLanguage.German
	};

	public static SystemLanguage language = SystemLanguage.English;

	public const SystemLanguage FallbackLanguage = SystemLanguage.English;

	public const string StringsFolder = "Strings/";

	public const string StringsFilePrefix = "RDStrings_";

	public const string GamepadKeySuffix = ".gamepad";

	public const string MobileKeySuffix = ".mobile";

	public const string BoothButtonKeySuffix = ".boothButton";

	public const string DivekickKeySuffix = ".divekick";

	public const string NintendoSwitchKeySuffix = ".nx";

	public static FontData enFontData;

	public static bool initialized = false;

	public static FontData fontData
	{
		get;
		private set;
	}

	public static string languageSuffix
	{
		get
		{
			Setup();
			if (SystemLanguage.English != language)
			{
				return "_" + language.ToString();
			}
			return "";
		}
	}

	public static bool isCJK
	{
		get
		{
			if (language != SystemLanguage.ChineseSimplified && language != SystemLanguage.ChineseTraditional && language != SystemLanguage.Japanese)
			{
				return language == SystemLanguage.Korean;
			}
			return true;
		}
	}

	public static void Setup()
	{
		if (initialized)
		{
			return;
		}
		if (GCNS.devBranches.Contains(GCS.steamBranchName))
		{
			AvailableLanguages = new SystemLanguage[15]
			{
				SystemLanguage.ChineseSimplified,
				SystemLanguage.ChineseTraditional,
				SystemLanguage.Korean,
				SystemLanguage.Japanese,
				SystemLanguage.English,
				SystemLanguage.Spanish,
				SystemLanguage.Portuguese,
				SystemLanguage.French,
				SystemLanguage.Finnish,
				SystemLanguage.Polish,
				SystemLanguage.Romanian,
				SystemLanguage.Russian,
				SystemLanguage.Vietnamese,
				SystemLanguage.Czech,
				SystemLanguage.German
			};
		}
		string text = Persistence.GetLanguage();
		bool flag = false;
		if (text == SystemLanguage.Chinese.ToString())
		{
			flag = true;
			language = SystemLanguage.ChineseSimplified;
		}
		else
		{
			SystemLanguage[] availableLanguages = AvailableLanguages;
			for (int i = 0; i < availableLanguages.Length; i++)
			{
				SystemLanguage systemLanguage = availableLanguages[i];
				if (systemLanguage.ToString() == text)
				{
					flag = true;
					language = systemLanguage;
					break;
				}
			}
		}
		if (!flag)
		{
			language = SystemLanguage.English;
		}
		LangCode defaultValue = RDUtils.ParseEnum(SystemLanguage.English.ToString(), LangCode.English);
		Localization.SetLanguage(RDUtils.ParseEnum(language.ToString(), defaultValue));
		initialized = true;
		fontData = GetFontDataForLanguage(language);
		enFontData = GetFontDataForLanguage(SystemLanguage.English);
	}

	public static void SetLocalizedFont(this Text text)
	{
		Setup();
		if (fontData.font != text.font)
		{
			float fontScale = fontData.fontScale;
			float lineSpacing = fontData.lineSpacing;
			text.fontSize = Mathf.RoundToInt((float)text.fontSize * fontScale);
			text.lineSpacing *= lineSpacing;
			text.resizeTextMaxSize = Mathf.RoundToInt((float)text.resizeTextMaxSize * fontScale);
			text.resizeTextMinSize = Mathf.RoundToInt((float)text.resizeTextMinSize * fontScale);
			text.font = fontData.font;
		}
	}

	public static void SetLocalizedFont(this TMP_Text text)
	{
		Setup();
		if (fontData.font != text.font)
		{
			float num = (fontData.fontScale < 1f) ? fontData.fontScale : (fontData.fontScale / 1.25f);
			float lineSpacing = fontData.lineSpacing;
			if (language != SystemLanguage.Japanese)
			{
				num *= 1.2f;
			}
			text.fontSize = Mathf.RoundToInt(text.fontSize * num);
			text.lineSpacing *= lineSpacing;
			text.font = fontData.fontTMP;
		}
	}

	public static void SetLocalizedFont(this TextMesh text)
	{
		Setup();
		if (fontData.font != text.font)
		{
			float fontScale = fontData.fontScale;
			float lineSpacing = fontData.lineSpacing;
			text.fontSize = Mathf.RoundToInt((float)text.fontSize * fontScale);
			text.lineSpacing *= lineSpacing;
			text.font = fontData.font;
			text.GetComponent<MeshRenderer>().material = text.font.material;
		}
	}

	public static string GetInlinedFontForString(string s)
	{
		StringBuilder stringBuilder = new StringBuilder();
		Font[] array = new Font[4]
		{
			RDConstants.data.latinFont,
			RDConstants.data.koreanFont,
			RDConstants.data.chineseFont,
			RDConstants.data.japaneseFont
		};
		int num = 0;
		bool flag = false;
		foreach (char c in s)
		{
			int num2 = 0;
			if (!char.IsWhiteSpace(c))
			{
				num2 = num;
			}
			else if (IsKoreanCharacter(c))
			{
				num2 = 1;
			}
			else if (IsChineseCharacter(c))
			{
				num2 = 2;
			}
			else if (IsJapaneseCharacter(c))
			{
				num2 = 3;
			}
			if (num != num2)
			{
				if (array[num].name != fontData.font.name)
				{
					stringBuilder.Append("</font>");
					flag = false;
				}
				if (array[num2].name != fontData.font.name)
				{
					stringBuilder.Append("<font=\"" + array[num2].name + "\">");
					flag = true;
				}
			}
			num = num2;
			stringBuilder.Append(c);
		}
		if (flag)
		{
			stringBuilder.Append("</font>");
		}
		return stringBuilder.ToString();
	}

	public static FontData GetFontDataForLanguage(SystemLanguage language)
	{
		FontData result = default(FontData);
		RDConstants data = RDConstants.data;
		result.fontScale = 1f;
		switch (language)
		{
		case SystemLanguage.Korean:
			result.lineSpacing = 0.75f;
			result.font = data.koreanFont;
			result.fontTMP = data.koreanFontTMPro;
			break;
		case SystemLanguage.Japanese:
			result.lineSpacing = 1.1f;
			result.font = data.japaneseFont;
			result.fontTMP = data.japaneseFontTMPro;
			break;
		case SystemLanguage.Chinese:
		case SystemLanguage.ChineseSimplified:
		case SystemLanguage.ChineseTraditional:
			result.lineSpacing = 1f;
			result.font = data.chineseFont;
			result.fontTMP = data.chineseFontTMPro;
			break;
		default:
			result.lineSpacing = 0.75f;
			result.font = (GCS.bb ? BBManager.instance.font : data.latinFont);
			result.fontTMP = data.latinFontTMPro;
			break;
		}
		result.fontScale = 1f;
		return result;
	}

	public static void SetRDString(this Text text, string key)
	{
		Setup();
		text.SetLocalizedFont();
		text.text = Get(key);
	}

	public static string Get(string key, Dictionary<string, object> parameters = null)
	{
		Setup();
		bool exists = false;
		return GetWithCheck(key, out exists, parameters);
	}

	public static string GetEnumValue(string type, string value)
	{
		string text = value.ToString();
		string key = "enum." + type + "." + text;
		bool exists = false;
		string result = GetWithCheck(key, out exists);
		if (!exists)
		{
			result = Get("enum.common." + text);
		}
		return result;
	}

	public static string GetEnumValue<T>(T value)
	{
		return GetEnumValue(typeof(T).Name, value.ToString());
	}

	public static string GetWithCheck(string key, out bool exists, Dictionary<string, object> parameters = null)
	{
		Setup();
		bool flag = key == "editor.PlaySound";
		exists = false;
		string text = "";
		string token = key + ".gamepad";
		string token2 = key + ".mobile";
		string token3 = key + ".nx";
		if (ADOBase.isSwitch && Localization.ExistsLocalizedString(token3))
		{
			text = Localization.GetLocalizedString(token3);
			exists = true;
		}
		else if (ADOBase.isGamepad && Localization.ExistsLocalizedString(token))
		{
			text = Localization.GetLocalizedString(token);
			exists = true;
		}
		else if (ADOBase.isMobile && Localization.ExistsLocalizedString(token2))
		{
			text = Localization.GetLocalizedString(token2);
			exists = true;
		}
		else if (Localization.ExistsLocalizedString(key))
		{
			text = Localization.GetLocalizedString(key);
			exists = true;
		}
		if (text == "null")
		{
			text = Localization.GetLocalizedString(key, LangSection.Translations, LangCode.English);
			exists = true;
		}
		if (exists)
		{
			text = ReplaceParameters(text, parameters);
		}
		return text;
	}

	public static string Join(params object[] _string)
	{
		return string.Join("", (string[])_string);
	}

	public static void ChangeLanguage(SystemLanguage language)
	{
		RDConstants.data.forceLanguage = false;
		Persistence.SetLanguage(language);
		print("changed language to: " + language.ToString());
		initialized = false;
		Setup();
	}

	private static void print(object message)
	{
		UnityEngine.Debug.Log(message);
	}

	public static string AddSpacesToChineseString(string s)
	{
		List<char> list = new List<char>(s.ToCharArray());
		int num;
		for (num = 0; num < list.Count - 1; num++)
		{
			char c = list[num];
			char c2 = list[num + 1];
			bool flag = !IsChineseCharacter(c) && !IsChineseCharacter(c2);
			if (!CharIsCJKPunctuation(c2) && !flag)
			{
				list.Insert(num + 1, ' ');
			}
			num++;
		}
		return new string(list.ToArray());
	}

	private static bool CharIsCJKPunctuation(char c)
	{
		uint[] array = new uint[12]
		{
			65292u,
			65281u,
			65311u,
			65307u,
			65306u,
			65288u,
			65289u,
			65339u,
			65341u,
			12304u,
			12305u,
			12290u
		};
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i] == c)
			{
				return true;
			}
		}
		return false;
	}

	public static Font GetAppropiateFontForString(string s)
	{
		if (s.Any((char c) => IsKoreanCharacter(c)))
		{
			return RDConstants.data.koreanFont;
		}
		if (s.Any((char c) => IsJapaneseCharacter(c)))
		{
			return RDConstants.data.japaneseFont;
		}
		if (s.Any((char c) => IsChineseCharacter(c)))
		{
			return RDConstants.data.chineseFont;
		}
		return RDConstants.data.latinFont;
	}

	public static bool IsChineseCharacter(char c)
	{
		if (((uint)c < 19968u || (uint)c > 40959u) && ((uint)c < 13312u || (uint)c > 19903u) && ((uint)c < 131072u || (uint)c > 173791u) && ((uint)c < 173824u || (uint)c > 191471u) && ((uint)c < 196608u || (uint)c > 201551u) && ((uint)c < 13056u || (uint)c > 13311u) && ((uint)c < 65072u || (uint)c > 65103u) && ((uint)c < 63744u || (uint)c > 64255u))
		{
			if ((uint)c >= 194560u)
			{
				return (uint)c <= 195103u;
			}
			return false;
		}
		return true;
	}

	public static bool IsKoreanCharacter(char c)
	{
		if (((uint)c < 44032u || (uint)c > 55203u) && ((uint)c < 4352u || (uint)c > 4607u) && ((uint)c < 12592u || (uint)c > 12687u) && ((uint)c < 43360u || (uint)c > 43391u))
		{
			if ((uint)c >= 55216u)
			{
				return (uint)c <= 55295u;
			}
			return false;
		}
		return true;
	}

	public static bool IsJapaneseCharacter(char c)
	{
		if ((uint)c >= 12288u)
		{
			return (uint)c <= 12543u;
		}
		return false;
	}

	public static void DownloadStringsFromWeb()
	{
		print("download strings from web");
		API.RetrievePublicSheetData(Settings.Instance.GetDocByKey(LocalizationConfig.Instance.LocalizationDocKey));
		Localization.SetLanguage(Localization.CurrentLanguage);
	}

	public static string GetColon()
	{
		if (language != SystemLanguage.Korean && language != SystemLanguage.ChineseSimplified && language != SystemLanguage.ChineseTraditional && language != SystemLanguage.Japanese)
		{
			return ":";
		}
		return "：";
	}

	public static string ReplaceParameters(string str, Dictionary<string, object> parameters)
	{
		if (parameters != null)
		{
			foreach (string key in parameters.Keys)
			{
				str = Regex.Replace(str, "(\\[" + key + "\\])", parameters[key].ToString());
			}
			return str;
		}
		return str;
	}
}
