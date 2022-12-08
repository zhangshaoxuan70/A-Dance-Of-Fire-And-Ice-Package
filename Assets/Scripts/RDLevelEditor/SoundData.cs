using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RDLevelEditor
{
	public class SoundData
	{
		public string filename;

		public int volume;

		public int offset;

		public bool externalClip;

		public bool loadSuccessful;

		public bool itsASong;

		public string filenameKey;

		public string volumeKey;

		public string offsetKey;

		public float volumePercentage => (float)volume * 0.01f;

		public float offsetInSeconds => (float)offset * 0.001f;

		public string conductorFilename
		{
			get
			{
				if (externalClip)
				{
					return filename + "*external";
				}
				return filename;
			}
		}

		public SoundData(bool itsASong, string filename, string filenameKey = "filename", string volumeKey = "volume", string offsetKey = "offset")
		{
			this.itsASong = itsASong;
			this.filename = filename;
			this.filenameKey = filenameKey;
			this.volumeKey = volumeKey;
			this.offsetKey = offsetKey;
			volume = 100;
			offset = 0;
			externalClip = false;
		}

		public IEnumerator Prepare()
		{
			loadSuccessful = false;
			scnEditor instance = scnEditor.instance;
			bool flag = false;
			if (instance.preparedAudioClips == null)
			{
				UnityEngine.Debug.LogError("preparedaudioclips is null");
			}
			if (string.IsNullOrEmpty(filename))
			{
				UnityEngine.Debug.LogError("filename is null or empty");
			}
			if (instance.preparedAudioClips.ContainsKey(filename) || instance.preparedAudioClips.ContainsKey(filename + "*external"))
			{
				UnityEngine.Debug.Log("sound was already on preparedAudioClips");
				flag = true;
			}
			if (flag)
			{
				yield break;
			}
			switch (RDEditorUtils.FindClip(filename))
			{
			case RDAudioLoadType.SuccessExternalClipLoaded:
			{
				yield return RDEditorUtils.AudioClipFromFilename(filename);
				externalClip = true;
				loadSuccessful = true;
				string conductorFilename = this.conductorFilename;
				Dictionary<string, AudioClip> audioLib = AudioManager.Instance.audioLib;
				if (!audioLib.ContainsKey(conductorFilename))
				{
					UnityEngine.Debug.LogWarning("Trying to find sound in library but was not found! " + conductorFilename);
				}
				else
				{
					AudioClip audioClip = audioLib[conductorFilename];
				}
				break;
			}
			case RDAudioLoadType.SuccessInternalClipLoaded:
			{
				externalClip = false;
				loadSuccessful = true;
				string conductorFilename2 = this.conductorFilename;
				break;
			}
			default:
				externalClip = false;
				break;
			}
		}

		public void Decode(Dictionary<string, object> dict)
		{
			filename = (dict[filenameKey] as string);
			if (dict.ContainsKey(offsetKey))
			{
				offset = Convert.ToInt32(dict[offsetKey]);
				volume = Convert.ToInt32(dict[volumeKey]);
			}
			if (scnEditor.instance.version <= 9 && filename.HasAudioFileExtension() && !itsASong)
			{
				volume = Mathf.FloorToInt((float)volume / 0.4f);
			}
		}

		public string Encode(bool isLastValue = false)
		{
			return RDEditorUtils.EncodeString(filenameKey, filename) + RDEditorUtils.EncodeInt(volumeKey, volume) + RDEditorUtils.EncodeInt(offsetKey, offset, isLastValue);
		}

		public void CopyFrom(SoundData another)
		{
			filename = another.filename;
			offset = another.offset;
			volume = another.volume;
		}
	}
}
