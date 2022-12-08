using System.Collections.Generic;
using UnityEngine;

namespace ADOFAI
{
	public static class EditorConstants
	{
		public struct EditorKeyShortcut
		{
			public string key;

			public KeyCode keyCode;

			public KeyCode otherKeyCode;

			public bool usingShift;

			public bool usingCtrl;

			public bool usingAlt;

			public bool otherKeyDoesntUseModifierKeys;

			public EditorKeyShortcut(string key, KeyCode keyCode, KeyCode otherKeyCode, bool usingShift, bool usingCtrl, bool usingAlt, bool otherKeyDoesntUseModifierKeys)
			{
				this.key = key;
				this.keyCode = keyCode;
				this.otherKeyCode = otherKeyCode;
				this.usingShift = usingShift;
				this.usingCtrl = usingCtrl;
				this.usingAlt = usingAlt;
				this.otherKeyDoesntUseModifierKeys = otherKeyDoesntUseModifierKeys;
			}
		}

		public const int adofaiFileVersion = 11;

		public const string key_eventType = "eventType";

		public const string key_floor = "floor";

		public const string key_enabled = "enabled";

		public const string key_active = "active";

		public const string key_visible = "visible";

		public const string key_pathData = "pathData";

		public const string key_angleData = "angleData";

		public const string key_settings = "settings";

		public const string key_actions = "actions";

		public const string key_decorations = "decorations";

		public const string key_version = "version";

		public const string key_songFilename = "songFilename";

		public const string key_author = "author";

		public const string key_song = "song";

		public const string key_specialArtistType = "specialArtistType";

		public const string key_artist = "artist";

		public const string key_previewImage = "previewImage";

		public const string key_previewIcon = "previewIcon";

		public const string key_previewIconColor = "previewIconColor";

		public const string key_previewSongStart = "previewSongStart";

		public const string key_previewSongDuration = "previewSongDuration";

		public const string key_seizureWarning = "seizureWarning";

		public const string key_levelDesc = "levelDesc";

		public const string key_levelTags = "levelTags";

		public const string key_artistPermission = "artistPermission";

		public const string key_artistLinks = "artistLinks";

		public const string key_difficulty = "difficulty";

		public const string key_bpm = "bpm";

		public const string key_volume = "volume";

		public const string key_hitsound = "hitsound";

		public const string key_hitsoundVolume = "hitsoundVolume";

		public const string key_separateCountdownTime = "separateCountdownTime";

		public const string key_countdownTicks = "countdownTicks";

		public const string key_trackColorType = "trackColorType";

		public const string key_trackColor = "trackColor";

		public const string key_secondaryTrackColor = "secondaryTrackColor";

		public const string key_trackColorAnimDuration = "trackColorAnimDuration";

		public const string key_trackColorPulse = "trackColorPulse";

		public const string key_trackPulseLength = "trackPulseLength";

		public const string key_trackStyle = "trackStyle";

		public const string key_trackAnimation = "trackAnimation";

		public const string key_trackDisappearAnimation = "trackDisappearAnimation";

		public const string key_trackBeatsAhead = "beatsAhead";

		public const string key_trackBeatsBehind = "beatsBehind";

		public const string key_pitch = "pitch";

		public const string key_offset = "offset";

		public const string key_backgroundColor = "backgroundColor";

		public const string key_bgImage = "bgImage";

		public const string key_bgImageColor = "bgImageColor";

		public const string key_parallax = "parallax";

		public const string key_bgDisplayMode = "bgDisplayMode";

		public const string key_loopBG = "loopBG";

		public const string key_bgLockRot = "lockRot";

		public const string key_bgShowDefault = "showDefaultBGIfNoImage";

		public const string key_unscaledSize = "unscaledSize";

		public const string key_camRelativeTo = "relativeTo";

		public const string key_camPosition = "position";

		public const string key_camRotation = "rotation";

		public const string key_camZoom = "zoom";

		public const string key_bgVideo = "bgVideo";

		public const string key_floorIconOutlines = "floorIconOutlines";

		public const string key_stickToFloors = "stickToFloors";

		public const string key_planetEase = "planetEase";

		public const string key_planetEaseParts = "planetEaseParts";

		public const string key_planetEasePartBehavior = "planetEasePartBehavior";

		public const string key_legacyFlash = "legacyFlash";

		public const string key_legacySpriteTiles = "legacySpriteTiles";

		public const string key_legacyCamRelativeTo = "legacyCamRelativeTo";

		public const string key_requiredMods = "requiredMods";

		public static readonly LevelEventType[] soloTypes = new LevelEventType[24]
		{
			LevelEventType.SetSpeed,
			LevelEventType.Twirl,
			LevelEventType.Multitap,
			LevelEventType.Checkpoint,
			LevelEventType.SetHitsound,
			LevelEventType.ChangeTrack,
			LevelEventType.ColorTrack,
			LevelEventType.AnimateTrack,
			LevelEventType.SetPlanetRotation,
			LevelEventType.KillPlayer,
			LevelEventType.PositionTrack,
			LevelEventType.Hold,
			LevelEventType.SetHoldSound,
			LevelEventType.SetConditionalEvents,
			LevelEventType.MultiPlanet,
			LevelEventType.FreeRoam,
			LevelEventType.Pause,
			LevelEventType.AutoPlayTiles,
			LevelEventType.Hide,
			LevelEventType.ScaleMargin,
			LevelEventType.ScaleRadius,
			LevelEventType.TileDimensions,
			LevelEventType.SetConditionalEvents,
			LevelEventType.Bookmark
		};

		public static readonly LevelEventType[] toggleableTypes = new LevelEventType[4]
		{
			LevelEventType.Twirl,
			LevelEventType.Multitap,
			LevelEventType.Checkpoint,
			LevelEventType.Bookmark
		};

		public static readonly LevelEventType[] settingsTypes = new LevelEventType[7]
		{
			LevelEventType.SongSettings,
			LevelEventType.LevelSettings,
			LevelEventType.TrackSettings,
			LevelEventType.BackgroundSettings,
			LevelEventType.CameraSettings,
			LevelEventType.MiscSettings,
			LevelEventType.DecorationSettings
		};

		public static Dictionary<string, List<EditorKeyShortcut>> editorShortcuts = new Dictionary<string, List<EditorKeyShortcut>>
		{
			["BasicEditing"] = new List<EditorKeyShortcut>
			{
				new EditorKeyShortcut("Midspin", KeyCode.Tab, KeyCode.None, usingShift: false, usingCtrl: false, usingAlt: false, otherKeyDoesntUseModifierKeys: false),
				new EditorKeyShortcut("Create360Floor", KeyCode.Space, KeyCode.None, usingShift: true, usingCtrl: false, usingAlt: false, otherKeyDoesntUseModifierKeys: false),
				new EditorKeyShortcut("Play", KeyCode.Space, KeyCode.P, usingShift: false, usingCtrl: false, usingAlt: false, otherKeyDoesntUseModifierKeys: false),
				new EditorKeyShortcut("ExitPlayMode", KeyCode.Escape, KeyCode.None, usingShift: false, usingCtrl: false, usingAlt: false, otherKeyDoesntUseModifierKeys: false),
				new EditorKeyShortcut("ExitPlayModeAtTile", KeyCode.Escape, KeyCode.None, usingShift: true, usingCtrl: false, usingAlt: false, otherKeyDoesntUseModifierKeys: false),
				new EditorKeyShortcut("CopyFloor", KeyCode.C, KeyCode.None, usingShift: false, usingCtrl: true, usingAlt: false, otherKeyDoesntUseModifierKeys: false),
				new EditorKeyShortcut("CutFloor", KeyCode.X, KeyCode.None, usingShift: false, usingCtrl: true, usingAlt: false, otherKeyDoesntUseModifierKeys: false),
				new EditorKeyShortcut("PasteFloor", KeyCode.V, KeyCode.None, usingShift: false, usingCtrl: true, usingAlt: false, otherKeyDoesntUseModifierKeys: true),
				new EditorKeyShortcut("Undo", KeyCode.Z, KeyCode.None, usingShift: false, usingCtrl: true, usingAlt: false, otherKeyDoesntUseModifierKeys: false),
				new EditorKeyShortcut("Redo", KeyCode.Z, KeyCode.None, usingShift: true, usingCtrl: true, usingAlt: false, otherKeyDoesntUseModifierKeys: false)
			},
			["SelectionAndDeletion"] = new List<EditorKeyShortcut>
			{
				new EditorKeyShortcut("MultiselectTiles", KeyCode.Mouse0, KeyCode.None, usingShift: true, usingCtrl: false, usingAlt: false, otherKeyDoesntUseModifierKeys: false),
				new EditorKeyShortcut("MoveSelection", KeyCode.LeftArrow, KeyCode.RightArrow, usingShift: true, usingCtrl: false, usingAlt: false, otherKeyDoesntUseModifierKeys: false),
				new EditorKeyShortcut("DeleteFloors", KeyCode.Delete, KeyCode.Backspace, usingShift: false, usingCtrl: false, usingAlt: false, otherKeyDoesntUseModifierKeys: false),
				new EditorKeyShortcut("DeleteSubsequentFloors", KeyCode.Delete, KeyCode.None, usingShift: false, usingCtrl: true, usingAlt: false, otherKeyDoesntUseModifierKeys: false),
				new EditorKeyShortcut("DeletePrecedingFloors", KeyCode.Backspace, KeyCode.None, usingShift: false, usingCtrl: true, usingAlt: false, otherKeyDoesntUseModifierKeys: false),
				new EditorKeyShortcut("SelectFirstFloor", KeyCode.LeftArrow, KeyCode.Home, usingShift: false, usingCtrl: true, usingAlt: false, otherKeyDoesntUseModifierKeys: true),
				new EditorKeyShortcut("SelectLastFloor", KeyCode.RightArrow, KeyCode.End, usingShift: false, usingCtrl: true, usingAlt: false, otherKeyDoesntUseModifierKeys: true)
			},
			["FlippingAndRotation"] = new List<EditorKeyShortcut>
			{
				new EditorKeyShortcut("FlipFloorsHorizontal", KeyCode.L, KeyCode.None, usingShift: false, usingCtrl: true, usingAlt: false, otherKeyDoesntUseModifierKeys: false),
				new EditorKeyShortcut("FlipFloorsVertical", KeyCode.L, KeyCode.None, usingShift: true, usingCtrl: true, usingAlt: false, otherKeyDoesntUseModifierKeys: false),
				new EditorKeyShortcut("RotateFloors90", KeyCode.Comma, KeyCode.Period, usingShift: false, usingCtrl: true, usingAlt: false, otherKeyDoesntUseModifierKeys: false),
				new EditorKeyShortcut("RotateFloors180", KeyCode.Slash, KeyCode.None, usingShift: false, usingCtrl: true, usingAlt: false, otherKeyDoesntUseModifierKeys: false),
				new EditorKeyShortcut("RotateTileMouse", KeyCode.Mouse1, KeyCode.None, usingShift: false, usingCtrl: false, usingAlt: false, otherKeyDoesntUseModifierKeys: false),
				new EditorKeyShortcut("RotateTileMouseFine", KeyCode.Mouse1, KeyCode.None, usingShift: true, usingCtrl: false, usingAlt: false, otherKeyDoesntUseModifierKeys: false)
			},
			["AdvancedEditing"] = new List<EditorKeyShortcut>
			{
				new EditorKeyShortcut("CopyEvents", KeyCode.C, KeyCode.None, usingShift: true, usingCtrl: true, usingAlt: false, otherKeyDoesntUseModifierKeys: false),
				new EditorKeyShortcut("CutEvents", KeyCode.X, KeyCode.None, usingShift: true, usingCtrl: true, usingAlt: false, otherKeyDoesntUseModifierKeys: false),
				new EditorKeyShortcut("PasteEvents", KeyCode.V, KeyCode.None, usingShift: true, usingCtrl: true, usingAlt: false, otherKeyDoesntUseModifierKeys: false),
				new EditorKeyShortcut("PositionTrack", KeyCode.Mouse0, KeyCode.None, usingShift: false, usingCtrl: false, usingAlt: true, otherKeyDoesntUseModifierKeys: false),
				new EditorKeyShortcut("DuplicateDecorations", KeyCode.D, KeyCode.None, usingShift: false, usingCtrl: true, usingAlt: false, otherKeyDoesntUseModifierKeys: false),
				new EditorKeyShortcut("CopyTrackColor", KeyCode.E, KeyCode.None, usingShift: true, usingCtrl: true, usingAlt: false, otherKeyDoesntUseModifierKeys: false),
				new EditorKeyShortcut("PasteTrackColor", KeyCode.R, KeyCode.None, usingShift: true, usingCtrl: true, usingAlt: false, otherKeyDoesntUseModifierKeys: false),
				new EditorKeyShortcut("PasteTrackColorSingleTile", KeyCode.T, KeyCode.None, usingShift: true, usingCtrl: true, usingAlt: false, otherKeyDoesntUseModifierKeys: false),
				new EditorKeyShortcut("CopyHitSound", KeyCode.Alpha1, KeyCode.None, usingShift: true, usingCtrl: true, usingAlt: false, otherKeyDoesntUseModifierKeys: false),
				new EditorKeyShortcut("PasteHitSoundSingleTile", KeyCode.Alpha2, KeyCode.None, usingShift: true, usingCtrl: true, usingAlt: false, otherKeyDoesntUseModifierKeys: false)
			},
			["Bookmarks"] = new List<EditorKeyShortcut>
			{
				new EditorKeyShortcut("ToggleBookmark", KeyCode.B, KeyCode.None, usingShift: false, usingCtrl: false, usingAlt: true, otherKeyDoesntUseModifierKeys: false),
				new EditorKeyShortcut("SelectPreviousBookmark", KeyCode.LeftArrow, KeyCode.None, usingShift: false, usingCtrl: false, usingAlt: true, otherKeyDoesntUseModifierKeys: false),
				new EditorKeyShortcut("SelectNextBookmark", KeyCode.RightArrow, KeyCode.None, usingShift: false, usingCtrl: false, usingAlt: true, otherKeyDoesntUseModifierKeys: false),
				new EditorKeyShortcut("ToggleFindFloorPanel", KeyCode.Semicolon, KeyCode.None, usingShift: false, usingCtrl: false, usingAlt: true, otherKeyDoesntUseModifierKeys: false)
			},
			["SavingAndLoading"] = new List<EditorKeyShortcut>
			{
				new EditorKeyShortcut("SaveLevel", KeyCode.S, KeyCode.None, usingShift: false, usingCtrl: true, usingAlt: false, otherKeyDoesntUseModifierKeys: false),
				new EditorKeyShortcut("SaveLevelAs", KeyCode.S, KeyCode.None, usingShift: true, usingCtrl: true, usingAlt: false, otherKeyDoesntUseModifierKeys: false),
				new EditorKeyShortcut("OpenLevel", KeyCode.O, KeyCode.None, usingShift: false, usingCtrl: true, usingAlt: false, otherKeyDoesntUseModifierKeys: false),
				new EditorKeyShortcut("OpenRecent", KeyCode.O, KeyCode.None, usingShift: true, usingCtrl: true, usingAlt: false, otherKeyDoesntUseModifierKeys: false),
				new EditorKeyShortcut("OpenUrl", KeyCode.U, KeyCode.None, usingShift: false, usingCtrl: true, usingAlt: false, otherKeyDoesntUseModifierKeys: false)
			},
			["Other"] = new List<EditorKeyShortcut>
			{
				new EditorKeyShortcut("ToggleShortcutsPanel", KeyCode.H, KeyCode.None, usingShift: false, usingCtrl: true, usingAlt: false, otherKeyDoesntUseModifierKeys: false),
				new EditorKeyShortcut("ShowCopyrightPopup", KeyCode.F1, KeyCode.None, usingShift: false, usingCtrl: false, usingAlt: false, otherKeyDoesntUseModifierKeys: false),
				new EditorKeyShortcut("ToggleAuto", KeyCode.A, KeyCode.None, usingShift: false, usingCtrl: false, usingAlt: false, otherKeyDoesntUseModifierKeys: false),
				new EditorKeyShortcut("ToggleNoFail", KeyCode.N, KeyCode.None, usingShift: false, usingCtrl: false, usingAlt: false, otherKeyDoesntUseModifierKeys: false),
				new EditorKeyShortcut("CycleEventsPage", KeyCode.LeftBracket, KeyCode.RightBracket, usingShift: false, usingCtrl: false, usingAlt: false, otherKeyDoesntUseModifierKeys: false),
				new EditorKeyShortcut("CycleSelectedEvent", KeyCode.UpArrow, KeyCode.DownArrow, usingShift: true, usingCtrl: false, usingAlt: false, otherKeyDoesntUseModifierKeys: false),
				new EditorKeyShortcut("ToggleFloorNums", KeyCode.F, KeyCode.None, usingShift: false, usingCtrl: true, usingAlt: false, otherKeyDoesntUseModifierKeys: false),
				new EditorKeyShortcut("ShowCurrentFloorNumber", KeyCode.F, KeyCode.None, usingShift: true, usingCtrl: true, usingAlt: false, otherKeyDoesntUseModifierKeys: false)
			}
		};

		public static bool IsSetting(this LevelEventType type)
		{
			if (type != LevelEventType.SongSettings && type != LevelEventType.LevelSettings && type != LevelEventType.TrackSettings && type != LevelEventType.BackgroundSettings && type != LevelEventType.CameraSettings && type != LevelEventType.MiscSettings)
			{
				return type == LevelEventType.DecorationSettings;
			}
			return true;
		}
	}
}
