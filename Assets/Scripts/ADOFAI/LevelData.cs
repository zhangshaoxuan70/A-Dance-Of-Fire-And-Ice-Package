using DG.Tweening;
using GDMiniJSON;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ADOFAI
{
	[Serializable]
	public class LevelData
	{
		private string _hash;

		public string pathData;

		public List<float> angleData;

		public List<LevelEvent> levelEvents = new List<LevelEvent>();

		public List<LevelEvent> decorations = new List<LevelEvent>();

		[NonSerialized]
		public LevelEvent songSettings;

		[NonSerialized]
		public LevelEvent levelSettings;

		[NonSerialized]
		public LevelEvent trackSettings;

		[NonSerialized]
		public LevelEvent backgroundSettings;

		[NonSerialized]
		public LevelEvent cameraSettings;

		[NonSerialized]
		public LevelEvent miscSettings;

		[NonSerialized]
		public LevelEvent decorationSettings;

		public int version;

		public bool legacyFlash;

		public bool legacyCamRelativeTo;

		public bool isOldLevel;

		public bool oldCameraFollowStyle;

		public static bool shouldTryMigrate;

		private uint[] _requiredDLC;

		public string Hash
		{
			get
			{
				if (string.IsNullOrEmpty(_hash))
				{
					_hash = MD5Hash.GetHash(author + artist + song);
				}
				return _hash;
			}
		}

		public LevelEvent[] settings => new LevelEvent[7]
		{
			songSettings,
			levelSettings,
			trackSettings,
			backgroundSettings,
			cameraSettings,
			miscSettings,
			decorationSettings
		};

		public uint[] requiredDLC => _requiredDLC;

		public string fullCaption => RDUtils.RemoveRichTags(fullCaptionTagged);

		public string fullCaptionTagged
		{
			get
			{
				if (song.IsNullOrEmpty())
				{
					return "";
				}
				if (artist.IsNullOrEmpty())
				{
					return song;
				}
				string text = artist.Trim();
				if (text.EndsWith(")"))
				{
					int num = artist.IndexOf("(");
					if (num > 0)
					{
						text = artist.Substring(0, num);
					}
				}
				text = text.Trim();
				return text + " - " + song;
			}
		}

		public string artist
		{
			get
			{
				return (string)levelSettings["artist"];
			}
			set
			{
				levelSettings["artist"] = value;
			}
		}

		public SpecialArtistType specialArtistType => (SpecialArtistType)levelSettings["specialArtistType"];

		public string song => (string)levelSettings["song"];

		public string author => (string)levelSettings["author"];

		public string previewImage
		{
			get
			{
				return (string)levelSettings["previewImage"];
			}
			set
			{
				levelSettings["previewImage"] = value;
			}
		}

		public string previewIcon
		{
			get
			{
				if (levelSettings.data.ContainsKey("previewIcon"))
				{
					return (string)levelSettings["previewIcon"];
				}
				return null;
			}
			set
			{
				levelSettings["previewIcon"] = value;
			}
		}

		public Color previewIconColor => ((string)levelSettings["previewIconColor"]).HexToColor();

		public int previewSongStart => (int)levelSettings["previewSongStart"];

		public int previewSongDuration => (int)levelSettings["previewSongDuration"];

		public bool seizureWarning => (ToggleBool)levelSettings["seizureWarning"] == ToggleBool.Enabled;

		public string levelDesc => (string)levelSettings["levelDesc"];

		public string levelTags => (string)levelSettings["levelTags"];

		public string artistPermission
		{
			get
			{
				return (string)levelSettings["artistPermission"];
			}
			set
			{
				levelSettings["artistPermission"] = value;
			}
		}

		public string artistLinks => (string)levelSettings["artistLinks"];

		public int difficulty
		{
			get
			{
				if (!levelSettings.data.ContainsKey("difficulty"))
				{
					return 0;
				}
				return (int)levelSettings["difficulty"];
			}
		}

		public string songFilename
		{
			get
			{
				return (string)songSettings["songFilename"];
			}
			set
			{
				songSettings["songFilename"] = value;
			}
		}

		public float bpm => (float)songSettings["bpm"];

		public int volume => (int)songSettings["volume"];

		public int pitch => (int)songSettings["pitch"];

		public int offset => (int)songSettings["offset"];

		public HitSound hitsound => (HitSound)songSettings["hitsound"];

		public int hitsoundVolume => (int)songSettings["hitsoundVolume"];

		public bool separateCountdownTime => (ToggleBool)levelSettings["separateCountdownTime"] == ToggleBool.Enabled;

		public int countdownTicks => (int)songSettings["countdownTicks"];

		public TrackColorType trackColorType => (TrackColorType)trackSettings["trackColorType"];

		public Color trackColor => ((string)trackSettings["trackColor"]).HexToColor();

		public Color secondaryTrackColor => ((string)trackSettings["secondaryTrackColor"]).HexToColor();

		public float trackColorAnimDuration => Convert.ToSingle(trackSettings["trackColorAnimDuration"].ToString());

		public TrackColorPulse trackColorPulse => (TrackColorPulse)trackSettings["trackColorPulse"];

		public int trackPulseLength => (int)trackSettings["trackPulseLength"];

		public TrackStyle trackStyle => (TrackStyle)trackSettings["trackStyle"];

		public TrackAnimationType trackAnimation => (TrackAnimationType)trackSettings["trackAnimation"];

		public TrackAnimationType2 trackDisappearAnimation => (TrackAnimationType2)trackSettings["trackDisappearAnimation"];

		public float trackBeatsAhead => Convert.ToSingle(trackSettings["beatsAhead"].ToString());

		public float trackBeatsBehind => Convert.ToSingle(trackSettings["beatsBehind"].ToString());

		public Color backgroundColor => ((string)backgroundSettings["backgroundColor"]).HexToColor();

		public string bgImage
		{
			get
			{
				return (string)backgroundSettings["bgImage"];
			}
			set
			{
				backgroundSettings["bgImage"] = value;
			}
		}

		public Color bgImageColor => ((string)backgroundSettings["bgImageColor"]).HexToColor();

		public Vector2 bgParallax => (Vector2)backgroundSettings["parallax"] / 100f;

		public bool bgTiling => (BgDisplayMode)backgroundSettings["bgDisplayMode"] == BgDisplayMode.Tiled;

		public bool bgLooping => (ToggleBool)backgroundSettings["loopBG"] == ToggleBool.Enabled;

		public bool bgFitScreen => (BgDisplayMode)backgroundSettings["bgDisplayMode"] != BgDisplayMode.Unscaled;

		public bool bgLockRot => (ToggleBool)backgroundSettings["lockRot"] == ToggleBool.Enabled;

		public bool bgShowDefaultBGIfNoImage => (ToggleBool)backgroundSettings["showDefaultBGIfNoImage"] == ToggleBool.Enabled;

		public float unscaledSize => (float)(int)backgroundSettings["unscaledSize"] / 100f;

		public CamMovementType camRelativeTo => (CamMovementType)cameraSettings["relativeTo"];

		public Vector2 camPosition => (Vector2)cameraSettings["position"];

		public float camRotation => Convert.ToSingle(cameraSettings["rotation"].ToString());

		public float camZoom => (float)cameraSettings["zoom"];

		public string bgVideo
		{
			get
			{
				return (string)miscSettings["bgVideo"];
			}
			set
			{
				miscSettings["bgVideo"] = value;
			}
		}

		public bool floorIconOutlines => (ToggleBool)miscSettings["floorIconOutlines"] == ToggleBool.Enabled;

		public bool stickToFloors => (ToggleBool)miscSettings["stickToFloors"] == ToggleBool.Enabled;

		public Ease planetEase => (Ease)miscSettings["planetEase"];

		public int planetEaseParts => (int)miscSettings["planetEaseParts"];

		public EasePartBehavior planetEasePartBehavior => (EasePartBehavior)miscSettings["planetEasePartBehavior"];

		public void Setup()
		{
			if (CustomLevel.instance != null)
			{
				isOldLevel = CustomLevel.instance.forceOldLevelStyle;
			}
			angleData = new List<float>(new float[10]);
			pathData = "RRRRRRRRRR";
			Dictionary<string, LevelEventInfo> settingsInfo = GCS.settingsInfo;
			songSettings = new LevelEvent(0, LevelEventType.SongSettings, settingsInfo["SongSettings"]);
			levelSettings = new LevelEvent(0, LevelEventType.LevelSettings, settingsInfo["LevelSettings"]);
			trackSettings = new LevelEvent(0, LevelEventType.TrackSettings, settingsInfo["TrackSettings"]);
			backgroundSettings = new LevelEvent(0, LevelEventType.BackgroundSettings, settingsInfo["BackgroundSettings"]);
			cameraSettings = new LevelEvent(0, LevelEventType.CameraSettings, settingsInfo["CameraSettings"]);
			miscSettings = new LevelEvent(0, LevelEventType.MiscSettings, settingsInfo["MiscSettings"]);
			decorationSettings = new LevelEvent(0, LevelEventType.DecorationSettings, settingsInfo["DecorationSettings"]);
		}

		public bool LoadLevel(string levelPath, out LoadResult status)
		{
			status = LoadResult.Error;
			string text = RDFile.ReadAllText(levelPath);
			Dictionary<string, object> dictionary = Json.Deserialize(text) as Dictionary<string, object>;
			if (dictionary != null)
			{
				Decode(dictionary, out status);
				bool flag = status == LoadResult.Successful;
				if (flag)
				{
					shouldTryMigrate = true;
					if (version == 7)
					{
						text = text.Replace("\"enabled\": true", "\"active\": true").Replace("\"enabled\": false", "\"active\": false");
						dictionary = (Json.Deserialize(text) as Dictionary<string, object>);
						if (dictionary == null)
						{
							return false;
						}
						Decode(dictionary, out status);
					}
				}
				return flag;
			}
			return false;
		}

		public string Encode()
		{
			string text = "\n";
			string text2 = "\t";
			RDEditorUtils.IndentationLevel = 0;
			StringBuilder stringBuilder = new StringBuilder(1000000);
			stringBuilder.Append(RDEditorUtils.OpenDictionary());
			RDEditorUtils.IndentationLevel = 1;
			if (isOldLevel)
			{
				string value = RDEditorUtils.EncodeString("pathData", pathData) + text;
				stringBuilder.Append(value);
			}
			else
			{
				string value2 = RDEditorUtils.EncodeFloatArray("angleData", angleData.ToArray()) + text;
				stringBuilder.Append(value2);
			}
			stringBuilder.Append(RDEditorUtils.OpenDictionary("settings"));
			RDEditorUtils.IndentationLevel = 2;
			stringBuilder.AppendJoin(",\n", RDEditorUtils.EncodeInt("version", 11, lastValue: true), levelSettings.Encode(settings: true), songSettings.Encode(settings: true), trackSettings.Encode(settings: true), backgroundSettings.Encode(settings: true), cameraSettings.Encode(settings: true), miscSettings.Encode(settings: true), RDEditorUtils.EncodeBool("legacyFlash", legacyFlash, lastValue: true), RDEditorUtils.EncodeBool("legacyCamRelativeTo", legacyCamRelativeTo, lastValue: true), RDEditorUtils.EncodeBool("legacySpriteTiles", isOldLevel, lastValue: true));
			stringBuilder.Append("\n");
			RDEditorUtils.IndentationLevel = 1;
			stringBuilder.Append(RDEditorUtils.CloseDictionary());
			stringBuilder.Append(RDEditorUtils.OpenArray("actions"));
			RDEditorUtils.IndentationLevel = 0;
			List<LevelEvent> list = (from x in levelEvents
				orderby x.floor
				select x).ToList();
			foreach (LevelEvent item in list)
			{
				if (item.info.isActive)
				{
					stringBuilder.Append(text2 + text2 + "{ " + item.Encode() + "}," + text);
				}
			}
			if (list.Count > 0)
			{
				stringBuilder.Remove(stringBuilder.Length - 2, 2);
				stringBuilder.Append('\n');
			}
			RDEditorUtils.IndentationLevel = 1;
			stringBuilder.Append(RDEditorUtils.CloseArray(isLastMember: true));
			stringBuilder.Append(RDEditorUtils.OpenArray("decorations"));
			RDEditorUtils.IndentationLevel = 0;
			foreach (LevelEvent decoration in decorations)
			{
				if (decoration.info.isActive)
				{
					stringBuilder.Append(text2 + text2 + "{ " + decoration.Encode() + " }," + text);
				}
			}
			if (decorations.Count > 0)
			{
				stringBuilder.Remove(stringBuilder.Length - 2, 2);
				stringBuilder.Append('\n');
			}
			RDEditorUtils.IndentationLevel = 1;
			stringBuilder.Append(RDEditorUtils.CloseArray(isLastMember: true));
			RDEditorUtils.IndentationLevel = 0;
			stringBuilder.Append(RDEditorUtils.CloseDictionary(isLastMember: true));
			return stringBuilder.ToString();
		}

		public void Decode(Dictionary<string, object> dict, out LoadResult status)
		{
			status = LoadResult.Error;
			Dictionary<string, object> dictionary = dict["settings"] as Dictionary<string, object>;
			string key = "version";
			version = ((!dictionary.ContainsKey(key)) ? 1 : Convert.ToInt32(dictionary[key]));
			if (version > 11)
			{
				status = LoadResult.FutureVersion;
				return;
			}
			if (version < 4)
			{
				legacyFlash = true;
			}
			else
			{
				string key2 = "legacyFlash";
				legacyFlash = (dictionary.ContainsKey(key2) && RDEditorUtils.DecodeBool(dictionary[key2]));
			}
			if (version < 5)
			{
				isOldLevel = true;
			}
			else
			{
				isOldLevel = (dictionary.ContainsKey("legacySpriteTiles") && RDEditorUtils.DecodeBool(dictionary["legacySpriteTiles"]));
			}
			if (version < 11)
			{
				legacyCamRelativeTo = true;
			}
			else
			{
				string key3 = "legacyCamRelativeTo";
				legacyCamRelativeTo = (dictionary.ContainsKey(key3) && RDEditorUtils.DecodeBool(dictionary[key3]));
			}
			levelSettings.Decode(dictionary, "LevelSettings", isGlobal: true);
			songSettings.Decode(dictionary, "SongSettings", isGlobal: true);
			trackSettings.Decode(dictionary, "TrackSettings", isGlobal: true);
			backgroundSettings.Decode(dictionary, "BackgroundSettings", isGlobal: true);
			cameraSettings.Decode(dictionary, "CameraSettings", isGlobal: true);
			miscSettings.Decode(dictionary, "MiscSettings", isGlobal: true);
			decorationSettings.Decode(dictionary, "DecorationSettings", isGlobal: true);
			if (Application.isPlaying && RDEditorUtils.CheckModsDependency(levelSettings["requiredMods"] as object[]))
			{
				status = LoadResult.ModRequired;
				return;
			}
			if (dict.ContainsKey("pathData"))
			{
				if (isOldLevel)
				{
					pathData = RDEditorUtils.DecodeString(dict["pathData"]);
					angleData = new List<float>();
				}
				else
				{
					angleData = new List<float>(scrLevelMaker.StringToAngleArray(dict["pathData"] as string));
					pathData = "";
				}
			}
			else if (!Application.isPlaying || !CustomLevel.instance.forceOldLevelStyle)
			{
				angleData = new List<float>(RDEditorUtils.DecodeFloatArray(dict["angleData"]));
				pathData = "";
				isOldLevel = false;
			}
			int num = Math.Max(pathData.Length, angleData.Count);
			levelEvents = new List<LevelEvent>();
			decorations = new List<LevelEvent>();
			List<object> obj = dict["actions"] as List<object>;
			List<object> list = new List<object>();
			if (dict.ContainsKey("decorations"))
			{
				list = (dict["decorations"] as List<object>);
			}
			foreach (object item in obj)
			{
				LevelEvent levelEvent = new LevelEvent(item as Dictionary<string, object>);
				if (Application.isPlaying && !levelEvent.info.taroDLCCheck)
				{
					status = LoadResult.TaroDLCRequired;
					return;
				}
				if (levelEvent.info.isActive && levelEvent.floor <= num && levelEvent.floor >= 0)
				{
					if (levelEvent.eventType == LevelEventType.AddDecoration || levelEvent.eventType == LevelEventType.AddText)
					{
						decorations.Add(levelEvent);
					}
					else
					{
						levelEvents.Add(levelEvent);
					}
				}
			}
			foreach (object item2 in list)
			{
				LevelEvent levelEvent2 = new LevelEvent(item2 as Dictionary<string, object>);
				if (levelEvent2.info.isActive)
				{
					decorations.Add(levelEvent2);
				}
			}
			if (version < 2)
			{
				songSettings["pitch"] = (int)songSettings.info.propertiesInfo["pitch"].value_default;
			}
			if (version < 4)
			{
				legacyFlash = true;
			}
			if (version < 11)
			{
				legacyCamRelativeTo = true;
			}
			status = LoadResult.Successful;
		}

		public LevelData Copy()
		{
			LevelData levelData = new LevelData();
			levelData.pathData = pathData;
			levelData.angleData = new List<float>(angleData);
			levelData.isOldLevel = isOldLevel;
			levelData.legacyFlash = legacyFlash;
			levelData.legacyCamRelativeTo = legacyCamRelativeTo;
			levelData.levelEvents = new List<LevelEvent>();
			levelData.version = version;
			foreach (LevelEvent levelEvent in levelEvents)
			{
				levelData.levelEvents.Add(levelEvent.Copy());
			}
			foreach (LevelEvent decoration in decorations)
			{
				levelData.decorations.Add(decoration.Copy());
			}
			levelData.songSettings = songSettings.Copy();
			levelData.levelSettings = levelSettings.Copy();
			levelData.trackSettings = trackSettings.Copy();
			levelData.backgroundSettings = backgroundSettings.Copy();
			levelData.cameraSettings = cameraSettings.Copy();
			levelData.miscSettings = miscSettings.Copy();
			levelData.decorationSettings = decorationSettings.Copy();
			return levelData;
		}

		public void RefreshRequiredDLC()
		{
			List<uint> list = new List<uint>();
			bool flag = false;
			foreach (LevelEvent levelEvent in levelEvents)
			{
				if (levelEvent.info.taroDLC)
				{
					flag = true;
				}
			}
			if (flag)
			{
				list.Add(1977570u);
			}
			_requiredDLC = list.ToArray();
		}
	}
}
