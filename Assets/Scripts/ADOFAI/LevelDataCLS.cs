using GDMiniJSON;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ADOFAI
{
	public class LevelDataCLS : GenericDataCLS
	{
		public LevelEvent songSettings;

		public LevelEvent levelSettings;

		public override string artist => (string)levelSettings["artist"];

		public override string title => (string)levelSettings["song"];

		public override string author => (string)levelSettings["author"];

		public override string previewImage => (string)levelSettings["previewImage"];

		public override string previewIcon
		{
			get
			{
				if (!levelSettings.data.ContainsKey("previewIcon"))
				{
					return null;
				}
				return (string)levelSettings["previewIcon"];
			}
		}

		public override Color previewIconColor => ((string)levelSettings["previewIconColor"]).HexToColor();

		public int previewSongStart => (int)levelSettings["previewSongStart"];

		public int previewSongDuration => (int)levelSettings["previewSongDuration"];

		public bool seizureWarning => (ToggleBool)levelSettings["seizureWarning"] == ToggleBool.Enabled;

		public override string description => (string)levelSettings["levelDesc"];

		public string artistLinks => (string)levelSettings["artistLinks"];

		public override int difficulty
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

		public string[] levelTags => ((string)levelSettings["levelTags"]).Replace(", ", ",").Split(',');

		public bool requiresNeoCosmos => levelTags.Contains("Neo Cosmos");

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

		public int volume => (int)songSettings["volume"];

		public LoadResult loadResult
		{
			get;
			private set;
		}

		public void Setup()
		{
			Dictionary<string, LevelEventInfo> settingsInfo = GCS.settingsInfo;
			songSettings = new LevelEvent(0, LevelEventType.SongSettings, settingsInfo["SongSettings"]);
			levelSettings = new LevelEvent(0, LevelEventType.LevelSettings, settingsInfo["LevelSettings"]);
		}

		public bool LoadLevel(string levelPath)
		{
			Dictionary<string, object> dictionary = Json.Deserialize(RDFile.ReadAllText(levelPath)) as Dictionary<string, object>;
			if (dictionary != null)
			{
				return Decode(dictionary);
			}
			return false;
		}

		public bool Decode(Dictionary<string, object> rootDict)
		{
			loadResult = LoadResult.Error;
			Dictionary<string, object> dictionary = rootDict["settings"] as Dictionary<string, object>;
			if ((int)dictionary["version"] > 11)
			{
				loadResult = LoadResult.FutureVersion;
			}
			if (dictionary.TryGetValue("requiredMods", out object value) && RDEditorUtils.CheckModsDependency(value as object[]))
			{
				loadResult = LoadResult.ModRequired;
				return false;
			}
			levelSettings.Decode(dictionary, "LevelSettings", isGlobal: true);
			songSettings.Decode(dictionary, "SongSettings", isGlobal: true);
			return true;
		}
	}
}
