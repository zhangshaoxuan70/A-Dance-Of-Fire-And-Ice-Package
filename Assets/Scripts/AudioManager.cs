using RDTools;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Networking;

public class AudioManager : ADOBase
{
	public struct MusicSource
	{
		public AudioSource audioSource;

		public bool isTrackActive;
	}

	public Dictionary<string, AudioClip> audioLib;

	public List<RDMP3Stream> rdMP3Streams;

	public PriorityQueue<AudioSource, double> liveSources;

	public Queue<AudioSource> reusableSources;

	public List<MusicSource> musicTracks;

	public AudioSource audioSourcePrefab;

	public float maxMusicVol = 0.4f;

	public static bool pauseGameSounds;

	[Header("Note: This variable is for display only")]
	public int audioClipsLoaded;

	private GameObject audioSourceContainer;

	public AudioMixer Mixer;

	public AudioMixerGroup fallbackMixerGroup;

	private static AudioManager _instance;

	public AudioClipData audioClipData;

	public bool loadingExternalSong
	{
		get;
		private set;
	}

	public static AudioManager Instance
	{
		get
		{
			if (_instance == null)
			{
				GameObject gameObject = GameObject.Find("AudioManager");
				if (gameObject != null)
				{
					_instance = gameObject.GetComponent<AudioManager>();
				}
				else
				{
					_instance = new GameObject("AudioManager").AddComponent<AudioManager>();
				}
			}
			return _instance;
		}
	}

	public void Awake()
	{
		_instance = this;
		if (audioLib == null)
		{
			audioLib = new Dictionary<string, AudioClip>();
		}
		audioSourceContainer = RDUtils.SpawnIfNotFound("AudioSource Container");
		rdMP3Streams = new List<RDMP3Stream>();
		liveSources = new PriorityQueue<AudioSource, double>();
		reusableSources = new Queue<AudioSource>();
		musicTracks = new List<MusicSource>();
		if (audioSourcePrefab == null)
		{
			audioSourcePrefab = Resources.Load<AudioSource>("Audio Source");
		}
		Mixer = Resources.Load<AudioMixer>("MasterMixer");
		fallbackMixerGroup = Mixer.FindMatchingGroups("Fallback")[0];
	}

	public void Update()
	{
		if (pauseGameSounds)
		{
			return;
		}
		while (liveSources.Count != 0)
		{
			AudioSource audioSource = liveSources.Peek();
			if (audioSource == null)
			{
				liveSources.Dequeue();
				continue;
			}
			if (audioSource.isPlaying)
			{
				break;
			}
			liveSources.Dequeue();
			audioSource.gameObject.SetActive(value: false);
			audioSource.Stop();
			audioSource.clip = null;
			audioSource.loop = false;
			reusableSources.Enqueue(audioSource);
		}
		int num = 0;
		while (num < musicTracks.Count)
		{
			AudioSource audioSource2 = musicTracks[num].audioSource;
			if (audioSource2 == null)
			{
				musicTracks.RemoveAt(num);
			}
			else if (audioSource2.isPlaying)
			{
				UnityEngine.Object.Destroy(audioSource2.gameObject, 3.5f);
				musicTracks.RemoveAt(num);
			}
			else
			{
				CrossFadeAudioSource(audioSource2, musicTracks[num].isTrackActive);
				num++;
			}
		}
	}

	public IEnumerator FindOrLoadAudioClipExternal(string path, bool mp3Streaming, float length = 0f)
	{
		string filename = Path.GetFileName(path);
		string extension = Path.GetExtension(path);
		MonoBehaviour.print("AudioManager: path = " + path + " filename = " + filename + " extension = " + extension + " ");
		if (audioLib.ContainsKey(filename + "*external"))
		{
			MonoBehaviour.print("was already here! " + filename);
			yield return new RDAudioLoadResult(RDAudioLoadType.SuccessExternalClipLoaded, audioLib[filename + "*external"]);
			yield break;
		}
		bool loadSuccess = false;
		extension = extension.ToLower().Replace(".", "");
		AudioClip clip = null;
		bool flag = extension == "ogg";
		bool flag2 = extension == "wav";
		bool flag3 = extension == "aif";
		bool flag4 = extension == "aiff";
		bool flag5 = extension == "mp3";
		if (!RDFile.Exists(path))
		{
			RDBaseDll.printes("File doesn't exist in path: " + path);
			new RDAudioLoadResult(RDAudioLoadType.ErrorFileNotFound, null);
			yield break;
		}
		if (flag | flag2 | flag3 | flag4)
		{
			AudioType audioType = flag ? AudioType.OGGVORBIS : (flag2 ? AudioType.WAV : ((!flag5) ? AudioType.AIFF : AudioType.MPEG));
			using (UnityWebRequest webRequest = UnityWebRequestMultimedia.GetAudioClip("file://" + path, audioType))
			{
				(webRequest.downloadHandler as DownloadHandlerAudioClip).streamAudio = true;
				yield return webRequest.SendWebRequest();
				if (webRequest.result != UnityWebRequest.Result.Success)
				{
					UnityEngine.Debug.LogWarning($"webRequest error: {webRequest.error} ({webRequest.result})");
				}
				else
				{
					clip = DownloadHandlerAudioClip.GetContent(webRequest);
				}
				if (clip == null)
				{
					UnityEngine.Debug.LogWarning("AudioClip couldn't be found on url: " + path);
					yield return new RDAudioLoadResult(RDAudioLoadType.ErrorFileNotFound, null);
				}
				else if (clip.length == 0f)
				{
					UnityEngine.Debug.LogWarning("Audio is invalid for url: " + path + ". Make sure is a valid ogg format.");
					yield return new RDAudioLoadResult(RDAudioLoadType.ErrorLoadingOggVorbis, null);
				}
				else
				{
					loadSuccess = true;
				}
			}
		}
		else if (extension == "mp3")
		{
			if (mp3Streaming)
			{
				string key = filename + "*external";
				if (!audioLib.ContainsKey(key))
				{
					RDMP3Stream rDMP3Stream = new RDMP3Stream(path);
					clip = rDMP3Stream.CreateMP3Stream(length);
					audioLib.Add(key, clip);
					rdMP3Streams.Add(rDMP3Stream);
					loadSuccess = true;
				}
				else
				{
					yield return new RDAudioLoadResult(RDAudioLoadType.SuccessExternalClipLoaded, audioLib[key]);
					loadSuccess = false;
				}
			}
			else
			{
				clip = new AudioClipData(path).CreateAudioClip();
				loadSuccess = true;
			}
		}
		else
		{
			yield return new RDAudioLoadResult(RDAudioLoadType.ErrorFormatNotSupported, null);
		}
		if (loadSuccess && clip != null)
		{
			string conductorName = clip.name = filename + "*external";
			yield return new RDAudioLoadResult(RDAudioLoadType.SuccessExternalClipLoaded, clip);
			if (!audioLib.ContainsKey(conductorName))
			{
				audioLib.Add(conductorName, clip);
			}
		}
	}

	public void StopLoadingMP3File()
	{
		if (audioClipData != null)
		{
			audioClipData.StopLoading();
			audioClipData = null;
		}
	}

	public void StopAllSounds()
	{
		AudioSource element;
		double priority;
		while (liveSources.TryDequeue(out element, out priority))
		{
			UnityEngine.Object.Destroy(element.gameObject);
		}
		liveSources = new PriorityQueue<AudioSource, double>();
	}

	public void CrossFadeAudioSource(AudioSource audioSource, bool fadingIn = false)
	{
		if (!audioSource)
		{
			return;
		}
		if (fadingIn)
		{
			if (audioSource.volume < maxMusicVol - 0.1f)
			{
				audioSource.volume = Mathf.Lerp(audioSource.volume, maxMusicVol, 0.75f * Time.deltaTime);
			}
			else
			{
				audioSource.volume = maxMusicVol;
			}
		}
		else if ((double)audioSource.volume > 0.05)
		{
			audioSource.volume = Mathf.Lerp(audioSource.volume, 0f, 1f * Time.deltaTime);
		}
		else
		{
			audioSource.volume = 0f;
		}
	}

	public AudioSource MakeSource(string clipName = "Error", double scheduleTimeInfo = 0.0)
	{
		if (reusableSources.TryDequeue(out AudioSource result))
		{
			result.gameObject.SetActive(value: true);
		}
		else
		{
			result = Object.Instantiate(audioSourcePrefab, audioSourceContainer.transform);
		}
		result.clip = FindOrLoadAudioClip(clipName);
		liveSources.Enqueue(result, scheduleTimeInfo + (double)result.clip.length);
		return result;
	}

	public AudioClip FindOrLoadAudioClip(string clipName)
	{
		if (audioLib.ContainsKey(clipName))
		{
			return audioLib[clipName];
		}
		AudioClip audioClip = Resources.Load<AudioClip>("Audio/" + clipName);
		if (audioClip != null)
		{
			audioClipsLoaded++;
			audioLib.Add(clipName, audioClip);
			return audioLib[clipName];
		}
		return null;
	}

	private AudioSource _Play(string snd, double time, AudioMixerGroup group, float volume = 1f, int priority = 128)
	{
		AudioSource audioSource = MakeSource(snd, time);
		audioSource.pitch = 1f;
		if (group != null)
		{
			audioSource.outputAudioMixerGroup = group;
		}
		else
		{
			audioSource.outputAudioMixerGroup = fallbackMixerGroup;
		}
		audioSource.volume = volume;
		audioSource.priority = priority;
		audioSource.PlayScheduled(time);
		return audioSource;
	}

	public static AudioSource Play(string snd, double time, AudioMixerGroup group, float volume = 1f, int priority = 128)
	{
		return Instance._Play(snd, time, group, volume, priority);
	}

	public void FlushData()
	{
		audioLib.Clear();
		rdMP3Streams.Clear();
		AudioSource element;
		double priority;
		while (liveSources.TryDequeue(out element, out priority))
		{
			UnityEngine.Object.Destroy(element.gameObject);
		}
		reusableSources = new Queue<AudioSource>();
		for (int i = 0; i < musicTracks.Count; i++)
		{
			UnityEngine.Object.Destroy(musicTracks[i].audioSource.gameObject);
		}
	}
}
