using RDTools;
using System.IO;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.Video;

namespace ADOFAI
{
	public class PropertyControl_File : PropertyControl
	{
		private enum FileFormat
		{
			File,
			Audio,
			Image
		}

		public InputField inputField;

		public Image border;

		public Image verticalLine;

		public Button button;

		public Image buttonIcon;

		private string filename;

		private FileFormat format;

		public UnityEvent onFileChange;

		public override string text
		{
			get
			{
				return inputField.text;
			}
			set
			{
				inputField.text = value;
				filename = value;
			}
		}

		private void Awake()
		{
			button.onClick.AddListener(delegate
			{
				BrowseFile();
			});
			inputField.onEndEdit.AddListener(delegate(string s)
			{
				if (CheckIfLevelIsSaved())
				{
					ProcessFile(s, propertyInfo.fileType);
				}
			});
		}

		private bool CheckIfLevelIsSaved()
		{
			if (string.IsNullOrEmpty(ADOBase.levelPath))
			{
				RDBaseDll.printesw("Level has not been not saved yet.");
				scnEditor.PopupType popupType = scnEditor.PopupType.SaveBeforeSongImport;
				switch (propertyInfo.fileType)
				{
				case FileType.Audio:
					popupType = scnEditor.PopupType.SaveBeforeSongImport;
					break;
				case FileType.Image:
					popupType = scnEditor.PopupType.SaveBeforeImageImport;
					break;
				case FileType.Video:
					popupType = scnEditor.PopupType.SaveBeforeVideoImport;
					break;
				}
				ADOBase.editor.ShowPopup(show: true, popupType);
				return false;
			}
			return true;
		}

		private void BrowseFile()
		{
			if (CheckIfLevelIsSaved())
			{
				string text = null;
				FileType fileType = propertyInfo.fileType;
				switch (fileType)
				{
				case FileType.Audio:
					text = RDEditorUtils.ShowFileSelectorForAudio(RDString.Get("editor.dialog.selectSound"), -1L);
					break;
				case FileType.Image:
					text = RDEditorUtils.ShowFileSelectorForImage(RDString.Get("editor.dialog.selectImage"), -1L);
					break;
				case FileType.Video:
					text = RDEditorUtils.ShowFileSelectorForVideo(RDString.Get("editor.dialog.selectVideo"), -1L);
					break;
				}
				printe("browsing file: " + text);
				ProcessFile(text, fileType);
			}
		}

		private void ProcessFile(string newFilename, FileType fileType)
		{
			if (newFilename == null)
			{
				RDBaseDll.printesw("Filename is null.");
				return;
			}
			if (filename == newFilename)
			{
				RDBaseDll.printesw("Old filename and new one are the same (" + newFilename + ").");
				return;
			}
			if (newFilename != "")
			{
				string text = Path.Combine(Path.GetDirectoryName(ADOBase.levelPath), newFilename);
				if (!File.Exists(text))
				{
					RDBaseDll.printesw("File doesn't exist at path: " + text + ".");
				}
			}
			switch (fileType)
			{
			case FileType.Audio:
			{
				LevelEvent selectedEvent3 = propertiesPanel.inspectorPanel.selectedEvent;
				filename = newFilename;
				ToggleOthersEnabled();
				if (Path.GetExtension(filename).Replace(".", string.Empty) == "mp3")
				{
					ADOBase.editor.songToConvert = filename;
					ADOBase.editor.ShowPopup(show: true, scnEditor.PopupType.OggEncode);
				}
				else
				{
					selectedEvent3[propertyInfo.name] = filename;
					inputField.text = filename;
					ADOBase.editor.UpdateSongAndLevelSettings();
				}
				break;
			}
			case FileType.Image:
				using (new SaveStateScope(ADOBase.editor))
				{
					LevelEvent selectedEvent2 = propertiesPanel.inspectorPanel.selectedEvent;
					filename = newFilename;
					selectedEvent2[propertyInfo.name] = filename;
					inputField.text = filename;
					ToggleOthersEnabled();
					if (selectedEvent2.eventType == LevelEventType.BackgroundSettings)
					{
						ADOBase.customLevel.SetBackground();
					}
					else if (selectedEvent2.eventType == LevelEventType.AddDecoration)
					{
						ADOBase.editor.UpdateDecorationObject(selectedEvent2);
					}
					else if (selectedEvent2.eventType == LevelEventType.ColorTrack)
					{
						ADOBase.customLevel.UpdateFloorSprites();
						ADOBase.editor.ApplyEventsToFloors();
					}
					else
					{
						ADOBase.customLevel.UpdateBackgroundSprites();
					}
				}
				break;
			case FileType.Video:
			{
				LevelEvent selectedEvent = propertiesPanel.inspectorPanel.selectedEvent;
				filename = newFilename;
				selectedEvent[propertyInfo.name] = filename;
				inputField.text = filename;
				VideoPlayer videoBG = ADOBase.customLevel.videoBG;
				videoBG.gameObject.SetActive(value: true);
				videoBG.url = Path.Combine(Path.GetDirectoryName(ADOBase.levelPath), ADOBase.editor.levelData.miscSettings.data["bgVideo"].ToString());
				videoBG.Stop();
				videoBG.Prepare();
				ToggleOthersEnabled();
				break;
			}
			default:
				RDBaseDll.printesw($"File type {fileType} is not supported.");
				break;
			}
		}

		private void Update()
		{
			button.interactable = inputField.interactable;
			Color color = Color.white.WithAlpha(inputField.interactable ? 1f : 0.4f);
			border.color = color;
			buttonIcon.color = color;
			verticalLine.color = color;
		}

		public override void OnRightClick()
		{
			LevelEvent selectedEvent = propertiesPanel.inspectorPanel.selectedEvent;
			if (string.IsNullOrEmpty(selectedEvent[propertyInfo.name] as string))
			{
				return;
			}
			filename = "";
			selectedEvent[propertyInfo.name] = filename;
			inputField.text = filename;
			ToggleOthersEnabled();
			FileType fileType = propertyInfo.fileType;
			if (propertyInfo.fileType == FileType.Audio)
			{
				ADOBase.editor.levelData.songFilename = filename;
				ADOBase.editor.UpdateSongAndLevelSettings();
			}
			else if (propertyInfo.fileType == FileType.Image)
			{
				if (selectedEvent.eventType == LevelEventType.BackgroundSettings)
				{
					ADOBase.customLevel.SetBackground();
				}
				else if (selectedEvent.eventType == LevelEventType.AddDecoration)
				{
					ADOBase.editor.UpdateDecorationObject(selectedEvent);
				}
				else
				{
					ADOBase.customLevel.UpdateBackgroundSprites();
				}
			}
			else if (propertyInfo.fileType == FileType.Video)
			{
				VideoPlayer videoBG = ADOBase.customLevel.videoBG;
				videoBG.Stop();
				videoBG.gameObject.SetActive(value: false);
			}
		}
	}
}
