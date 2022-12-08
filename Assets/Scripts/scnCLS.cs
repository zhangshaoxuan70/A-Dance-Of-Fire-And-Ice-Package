using ADOFAI;
using DG.Tweening;
using GDMiniJSON;
using RDTools;
using SFB;
using Steamworks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class scnCLS : ADOBase
{
	private enum SortType
	{
		Difficulty,
		LastPlayed,
		Song,
		Artist,
		Author
	}

	[Serializable]
	[CompilerGenerated]
	private sealed class _003C_003Ec
	{
		public static readonly _003C_003Ec _003C_003E9 = new _003C_003Ec();

		public static UnityAction _003C_003E9__109_0;

		public static UnityAction _003C_003E9__109_1;

		internal void _003CStart_003Eb__109_0()
		{
			SteamWorkshop.OpenWorkshop();
		}

		internal void _003CStart_003Eb__109_1()
		{
			SteamWorkshop.OpenWorkshop();
		}
	}

	[CompilerGenerated]
	private sealed class _003CLoadSong_003Ed__114 : IEnumerator<object>, IEnumerator, IDisposable
	{
		private int _003C_003E1__state;

		private object _003C_003E2__current;

		public string path;

		public string songKey;

		public scnCLS _003C_003E4__this;

		object IEnumerator<object>.Current
		{
			[DebuggerHidden]
			get
			{
				return _003C_003E2__current;
			}
		}

		object IEnumerator.Current
		{
			[DebuggerHidden]
			get
			{
				return _003C_003E2__current;
			}
		}

		[DebuggerHidden]
		public _003CLoadSong_003Ed__114(int _003C_003E1__state)
		{
			this._003C_003E1__state = _003C_003E1__state;
		}

		[DebuggerHidden]
		void IDisposable.Dispose()
		{
		}

		private bool MoveNext()
		{
			int num = _003C_003E1__state;
			scnCLS scnCLS = _003C_003E4__this;
			switch (num)
			{
			default:
				return false;
			case 0:
				_003C_003E1__state = -1;
				_003C_003E2__current = AudioManager.Instance.FindOrLoadAudioClipExternal(path, mp3Streaming: true);
				_003C_003E1__state = 1;
				return true;
			case 1:
			{
				_003C_003E1__state = -1;
				Dictionary<string, AudioClip> audioLib = ADOBase.audioManager.audioLib;
				if (audioLib.ContainsKey(songKey))
				{
					AudioClip audioClip = audioLib[songKey];
					if (audioClip != null && scnCLS.loadedLevels[scnCLS.levelToSelect].isLevel)
					{
						LevelDataCLS level = scnCLS.loadedLevels[scnCLS.levelToSelect].level;
						if (songKey == scnCLS.newSongKey)
						{
							scnCLS.previewSongPlayer.Play(audioClip, level.previewSongStart, level.previewSongDuration, scnCLS.currentSongVolume);
						}
						else
						{
							scnCLS.printe("song was quickly changed!");
						}
						scnCLS.lastSongsLoaded.Add(songKey);
						while (scnCLS.lastSongsLoaded.Count > 5)
						{
							string key = scnCLS.lastSongsLoaded[0];
							scnCLS.lastSongsLoaded.RemoveAt(0);
							if (ADOBase.audioManager.audioLib.ContainsKey(key))
							{
								AudioClip audioClip2 = ADOBase.audioManager.audioLib[key];
								ADOBase.audioManager.audioLib.Remove(key);
								audioClip2.UnloadAudioData();
								UnityEngine.Object.DestroyImmediate(audioClip2, allowDestroyingAssets: true);
								Resources.UnloadAsset(audioClip2);
							}
						}
					}
				}
				else
				{
					UnityEngine.Debug.LogWarning("Couldn't load preview: " + songKey);
				}
				scnCLS.loadSongCoroutine = null;
				return false;
			}
			}
		}

		bool IEnumerator.MoveNext()
		{
			//ILSpy generated this explicit interface implementation from .override directive in MoveNext
			return this.MoveNext();
		}

		[DebuggerHidden]
		void IEnumerator.Reset()
		{
			throw new NotSupportedException();
		}
	}

	[StructLayout(LayoutKind.Auto)]
	[CompilerGenerated]
	private struct _003CRefresh_003Ed__116 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder _003C_003Et__builder;

		public scnCLS _003C_003E4__this;

		public bool setup;

		private CancellationToken _003CcancelToken_003E5__2;

		private float _003CsteamWaitStartTime_003E5__3;

		private TaskAwaiter _003C_003Eu__1;

		private void MoveNext()
		{
			int num = _003C_003E1__state;
			scnCLS scnCLS = _003C_003E4__this;
			try
			{
				TaskAwaiter awaiter;
				switch (num)
				{
				default:
					if (!scnCLS.showingInitialMenu)
					{
						scnCLS.refreshing = true;
						scnCLS.printe("Start CLS refresh");
						float realtimeSinceStartup = Time.realtimeSinceStartup;
						if (scnCLS.refreshTokenSource != null)
						{
							scnCLS.refreshTokenSource.Cancel();
						}
						scnCLS.refreshTokenSource = new CancellationTokenSource();
						_003CcancelToken_003E5__2 = scnCLS.refreshTokenSource.Token;
						scnCLS.DisableCLS(disable: true);
						scnCLS.DisablePlanets(disable: true);
						scnCLS.levelToSelect = null;
						scnCLS.loadingText.gameObject.SetActive(value: true);
						scnCLS.lastSongsLoaded = new List<string>();
						scnCLS.lastTexturesLoaded = new List<string>();
						scnCLS.StartCoroutine(SteamWorkshop.GetSubscribedItems());
						awaiter = Task.Delay(500, _003CcancelToken_003E5__2).GetAwaiter();
						if (!awaiter.IsCompleted)
						{
							num = (_003C_003E1__state = 0);
							_003C_003Eu__1 = awaiter;
							_003C_003Et__builder.AwaitUnsafeOnCompleted(ref awaiter, ref this);
							return;
						}
						goto IL_011a;
					}
					goto end_IL_000e;
				case 0:
					awaiter = _003C_003Eu__1;
					_003C_003Eu__1 = default(TaskAwaiter);
					num = (_003C_003E1__state = -1);
					goto IL_011a;
				case 1:
					awaiter = _003C_003Eu__1;
					_003C_003Eu__1 = default(TaskAwaiter);
					num = (_003C_003E1__state = -1);
					goto IL_0207;
				case 2:
					break;
					IL_0219:
					if (!SteamWorkshop.gettingSubscribedItemsInProgress || !(Time.realtimeSinceStartup - _003CsteamWaitStartTime_003E5__3 < 5f))
					{
						break;
					}
					awaiter = Task.Delay(500, _003CcancelToken_003E5__2).GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = (_003C_003E1__state = 1);
						_003C_003Eu__1 = awaiter;
						_003C_003Et__builder.AwaitUnsafeOnCompleted(ref awaiter, ref this);
						return;
					}
					goto IL_0207;
					IL_0207:
					awaiter.GetResult();
					_003CcancelToken_003E5__2.ThrowIfCancellationRequested();
					goto IL_0219;
					IL_011a:
					awaiter.GetResult();
					if (!setup && scnCLS.levelCount > 0)
					{
						Dictionary<string, CustomLevelTile>.ValueCollection.Enumerator enumerator = scnCLS.loadedLevelTiles.Values.GetEnumerator();
						try
						{
							while (enumerator.MoveNext())
							{
								UnityEngine.Object.Destroy(enumerator.Current.gameObject);
							}
						}
						finally
						{
							if (num < 0)
							{
								((IDisposable)enumerator).Dispose();
							}
						}
						scnCLS.loadedLevels = new Dictionary<string, GenericDataCLS>();
						scnCLS.loadedLevelDirs = new Dictionary<string, string>();
						scnCLS.loadedLevelTiles = new Dictionary<string, CustomLevelTile>();
						scnCLS.levelCount = 0;
					}
					_003CsteamWaitStartTime_003E5__3 = Time.realtimeSinceStartup;
					goto IL_0219;
				}
				try
				{
					if (num != 2)
					{
						awaiter = scnCLS.ScanLevels(_003CcancelToken_003E5__2).GetAwaiter();
						if (!awaiter.IsCompleted)
						{
							num = (_003C_003E1__state = 2);
							_003C_003Eu__1 = awaiter;
							_003C_003Et__builder.AwaitUnsafeOnCompleted(ref awaiter, ref this);
							return;
						}
					}
					else
					{
						awaiter = _003C_003Eu__1;
						_003C_003Eu__1 = default(TaskAwaiter);
						num = (_003C_003E1__state = -1);
					}
					awaiter.GetResult();
					scnCLS.loadingText.gameObject.SetActive(value: false);
				}
				catch (Exception ex)
				{
					scnCLS.printe("Cancelled CLS refresh: " + ex?.ToString());
					scnCLS.refreshing = false;
					goto IL_039e;
				}
				bool flag = scnCLS.levelCount > 0;
				if (flag)
				{
					scnCLS.DisableCLS(disable: false);
					scnCLS.DisablePlanets(disable: false);
					scnCLS.CreateFloors();
				}
				scnCLS.errorCanvas.gameObject.SetActive(!flag);
				scnCLS.optionsPanels.searchMode = false;
				scnCLS.optionsPanels.searchInputField.text = string.Empty;
				scnCLS.currentSearchText.text = RDString.Get("cls.shortcut.find");
				scnCLS.currentSearchText.SetLocalizedFont();
				scnCLS.optionsPanels.UpdateOrderText();
				float realtimeSinceStartup2 = Time.realtimeSinceStartup;
				scnCLS.printe("refreshing = false");
				scnCLS.refreshing = false;
				end_IL_000e:;
			}
			catch (Exception exception)
			{
				_003C_003E1__state = -2;
				_003CcancelToken_003E5__2 = default(CancellationToken);
				_003C_003Et__builder.SetException(exception);
				return;
			}
			goto IL_039e;
			IL_039e:
			_003C_003E1__state = -2;
			_003CcancelToken_003E5__2 = default(CancellationToken);
			_003C_003Et__builder.SetResult();
		}

		void IAsyncStateMachine.MoveNext()
		{
			//ILSpy generated this explicit interface implementation from .override directive in MoveNext
			this.MoveNext();
		}

		[DebuggerHidden]
		private void SetStateMachine(IAsyncStateMachine stateMachine)
		{
			_003C_003Et__builder.SetStateMachine(stateMachine);
		}

		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine stateMachine)
		{
			//ILSpy generated this explicit interface implementation from .override directive in SetStateMachine
			this.SetStateMachine(stateMachine);
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass129_0
	{
		public scnCLS _003C_003E4__this;

		public string levelKey;

		public float animDur;

		internal void _003CDisplayLevel_003Eb__1()
		{
			_003C_003E4__this.portalQuad.SetTexture(_003C_003E4__this.emptyTexture);
		}

		private void _003CDisplayLevel_003Eg__DoWarningAnimation_007C0(RectTransform rt, Tween tween)
		{
			tween?.Pause();
			rt.DOKill();
			tween = rt.DOScale(Vector3.zero, animDur * 0.5f).SetEase(Ease.InBack);
		}
	}

	[StructLayout(LayoutKind.Auto)]
	[CompilerGenerated]
	private struct _003C_003Ec__DisplayClass129_1
	{
		public bool purePerfected;
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass129_2
	{
		public string link;

		public _003C_003Ec__DisplayClass129_0 CS_0024_003C_003E8__locals1;

		internal void _003CDisplayLevel_003Eb__4()
		{
			if (!link.StartsWith("http://") && !link.StartsWith("https://") && !link.StartsWith("www."))
			{
				link = "https://" + link;
			}
			CS_0024_003C_003E8__locals1._003C_003E4__this.printe("Attempting to open URL: " + link);
			Application.OpenURL(link);
			EventSystem.current.SetSelectedGameObject(null);
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass129_3
	{
		public Texture2D texture;

		public _003C_003Ec__DisplayClass129_0 CS_0024_003C_003E8__locals2;

		internal void _003CDisplayLevel_003Eb__5()
		{
			CS_0024_003C_003E8__locals2._003C_003E4__this.portalQuad.SetTexture(texture);
			CS_0024_003C_003E8__locals2._003C_003E4__this.portalTransitionParticle.Stop(withChildren: true, ParticleSystemStopBehavior.StopEmitting);
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass129_4
	{
		public string portalImagePath;

		public _003C_003Ec__DisplayClass129_0 CS_0024_003C_003E8__locals3;

		internal void _003CDisplayLevel_003Eb__6()
		{
			CS_0024_003C_003E8__locals3._003C_003E4__this.LoadTexture(portalImagePath, CS_0024_003C_003E8__locals3.levelKey);
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass135_0
	{
		public string levelPath;

		internal Dictionary<string, object> _003CScanLevels_003Eb__0()
		{
			return Json.Deserialize(RDFile.ReadAllText(levelPath)) as Dictionary<string, object>;
		}
	}

	[StructLayout(LayoutKind.Auto)]
	[CompilerGenerated]
	private struct _003CScanLevels_003Ed__135 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder _003C_003Et__builder;

		public bool local;

		public scnCLS _003C_003E4__this;

		public bool workshop;

		public CancellationToken cancelToken;

		private string[] _003CitemDirs_003E5__2;

		private TaskAwaiter<Dictionary<string, object>[]> _003C_003Eu__1;

		private void MoveNext()
		{
			int num = _003C_003E1__state;
			scnCLS scnCLS = _003C_003E4__this;
			try
			{
				TaskAwaiter<Dictionary<string, object>[]> awaiter;
				if (num == 0)
				{
					awaiter = _003C_003Eu__1;
					_003C_003Eu__1 = default(TaskAwaiter<Dictionary<string, object>[]>);
					num = (_003C_003E1__state = -1);
					goto IL_0218;
				}
				if (!local || Directory.Exists(scnCLS.levelsDir))
				{
					if (!featuredLevelsMode)
					{
						string[] array = new string[0];
						string[] first = new string[0];
						if (workshop)
						{
							array = new string[SteamWorkshop.resultItems.Count];
							for (int i = 0; i < SteamWorkshop.resultItems.Count; i++)
							{
								array[i] = SteamWorkshop.resultItems[i].path;
								scnCLS.isWorkshopLevel[Path.GetFileName(array[i])] = true;
							}
						}
						if (local)
						{
							first = Directory.GetDirectories(scnCLS.levelsDir);
						}
						_003CitemDirs_003E5__2 = first.Concat(array).ToArray();
						cancelToken.ThrowIfCancellationRequested();
						List<Task<Dictionary<string, object>>> list = new List<Task<Dictionary<string, object>>>();
						string[] array2 = _003CitemDirs_003E5__2;
						foreach (string text in array2)
						{
							string levelPath = Path.Combine(text, "main.adofai");
							string fileName = Path.GetFileName(text);
							bool flag = false;
							if (scnCLS.loadedLevelIsDeleted.ContainsKey(fileName))
							{
								flag = scnCLS.loadedLevelIsDeleted[fileName];
							}
							if (RDFile.Exists(levelPath) && !flag)
							{
								list.Add(Task.Run(() => Json.Deserialize(RDFile.ReadAllText(levelPath)) as Dictionary<string, object>, cancelToken));
							}
							else if (!flag)
							{
								UnityEngine.Debug.LogWarning("No level file at " + text + "!");
								list.Add(Task.FromResult<Dictionary<string, object>>(null));
							}
						}
						cancelToken.ThrowIfCancellationRequested();
						awaiter = Task.WhenAll(list).GetAwaiter();
						if (!awaiter.IsCompleted)
						{
							num = (_003C_003E1__state = 0);
							_003C_003Eu__1 = awaiter;
							_003C_003Et__builder.AwaitUnsafeOnCompleted(ref awaiter, ref this);
							return;
						}
						goto IL_0218;
					}
					Dictionary<string, GenericDataCLS>.Enumerator enumerator = scnCLS.extraLevels.GetEnumerator();
					try
					{
						while (enumerator.MoveNext())
						{
							KeyValuePair<string, GenericDataCLS> current = enumerator.Current;
							string key = current.Key;
							if (!scnCLS.loadedLevels.ContainsKey(key))
							{
								scnCLS.loadedLevels.Add(key, current.Value);
								scnCLS.loadedLevelDirs.Add(key, null);
								scnCLS.loadedLevelIsDeleted[key] = false;
								scnCLS.isWorkshopLevel[key] = true;
							}
						}
					}
					finally
					{
						if (num < 0)
						{
							((IDisposable)enumerator).Dispose();
						}
					}
					goto IL_033e;
				}
				UnityEngine.Debug.LogWarning("First time launching CLS, making directory");
				RDDirectory.CreateDirectory(scnCLS.levelsDir);
				goto end_IL_000e;
				IL_033e:
				scnCLS.levelCount = scnCLS.loadedLevels.Count;
				goto end_IL_000e;
				IL_0218:
				Dictionary<string, object>[] result = awaiter.GetResult();
				cancelToken.ThrowIfCancellationRequested();
				for (int k = 0; k < _003CitemDirs_003E5__2.Length; k++)
				{
					string text2 = _003CitemDirs_003E5__2[k];
					string fileName2 = Path.GetFileName(text2);
					Dictionary<string, object> dictionary = result[k];
					if (dictionary != null)
					{
						LevelDataCLS levelDataCLS = new LevelDataCLS();
						levelDataCLS.Setup();
						if (levelDataCLS.Decode(dictionary))
						{
							scnCLS.loadedLevels.Add(fileName2, levelDataCLS);
							scnCLS.loadedLevelDirs.Add(fileName2, text2);
							scnCLS.loadedLevelIsDeleted[fileName2] = false;
						}
					}
				}
				_003CitemDirs_003E5__2 = null;
				goto IL_033e;
				end_IL_000e:;
			}
			catch (Exception exception)
			{
				_003C_003E1__state = -2;
				_003C_003Et__builder.SetException(exception);
				return;
			}
			_003C_003E1__state = -2;
			_003C_003Et__builder.SetResult();
		}

		void IAsyncStateMachine.MoveNext()
		{
			//ILSpy generated this explicit interface implementation from .override directive in MoveNext
			this.MoveNext();
		}

		[DebuggerHidden]
		private void SetStateMachine(IAsyncStateMachine stateMachine)
		{
			_003C_003Et__builder.SetStateMachine(stateMachine);
		}

		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine stateMachine)
		{
			//ILSpy generated this explicit interface implementation from .override directive in SetStateMachine
			this.SetStateMachine(stateMachine);
		}
	}

	[CompilerGenerated]
	private sealed class _003CEnableInputCo_003Ed__138 : IEnumerator<object>, IEnumerator, IDisposable
	{
		private int _003C_003E1__state;

		private object _003C_003E2__current;

		object IEnumerator<object>.Current
		{
			[DebuggerHidden]
			get
			{
				return _003C_003E2__current;
			}
		}

		object IEnumerator.Current
		{
			[DebuggerHidden]
			get
			{
				return _003C_003E2__current;
			}
		}

		[DebuggerHidden]
		public _003CEnableInputCo_003Ed__138(int _003C_003E1__state)
		{
			this._003C_003E1__state = _003C_003E1__state;
		}

		[DebuggerHidden]
		void IDisposable.Dispose()
		{
		}

		private bool MoveNext()
		{
			switch (_003C_003E1__state)
			{
			default:
				return false;
			case 0:
				_003C_003E1__state = -1;
				_003C_003E2__current = new WaitForEndOfFrame();
				_003C_003E1__state = 1;
				return true;
			case 1:
				_003C_003E1__state = -1;
				ADOBase.controller.responsive = true;
				return false;
			}
		}

		bool IEnumerator.MoveNext()
		{
			//ILSpy generated this explicit interface implementation from .override directive in MoveNext
			return this.MoveNext();
		}

		[DebuggerHidden]
		void IEnumerator.Reset()
		{
			throw new NotSupportedException();
		}
	}

	[StructLayout(LayoutKind.Auto)]
	[CompilerGenerated]
	private struct _003CFeaturedLevelsPortal_003Ed__142 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncVoidMethodBuilder _003C_003Et__builder;

		public scnCLS _003C_003E4__this;

		private TaskAwaiter _003C_003Eu__1;

		private void MoveNext()
		{
			int num = _003C_003E1__state;
			scnCLS scnCLS = _003C_003E4__this;
			try
			{
				TaskAwaiter awaiter;
				if (num != 0)
				{
					scnCLS.HideInitialMenu();
					featuredLevelsMode = true;
					awaiter = scnCLS.Refresh(setup: true).GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter;
						_003C_003Et__builder.AwaitUnsafeOnCompleted(ref awaiter, ref this);
						return;
					}
				}
				else
				{
					awaiter = _003C_003Eu__1;
					_003C_003Eu__1 = default(TaskAwaiter);
					num = (_003C_003E1__state = -1);
				}
				awaiter.GetResult();
				RDBaseDll.printem("");
			}
			catch (Exception exception)
			{
				_003C_003E1__state = -2;
				_003C_003Et__builder.SetException(exception);
				return;
			}
			_003C_003E1__state = -2;
			_003C_003Et__builder.SetResult();
		}

		void IAsyncStateMachine.MoveNext()
		{
			//ILSpy generated this explicit interface implementation from .override directive in MoveNext
			this.MoveNext();
		}

		[DebuggerHidden]
		private void SetStateMachine(IAsyncStateMachine stateMachine)
		{
			_003C_003Et__builder.SetStateMachine(stateMachine);
		}

		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine stateMachine)
		{
			//ILSpy generated this explicit interface implementation from .override directive in SetStateMachine
			this.SetStateMachine(stateMachine);
		}
	}

	[StructLayout(LayoutKind.Auto)]
	[CompilerGenerated]
	private struct _003CWorkshopLevelsPortal_003Ed__143 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncVoidMethodBuilder _003C_003Et__builder;

		public scnCLS _003C_003E4__this;

		private TaskAwaiter _003C_003Eu__1;

		private void MoveNext()
		{
			int num = _003C_003E1__state;
			scnCLS scnCLS = _003C_003E4__this;
			try
			{
				TaskAwaiter awaiter;
				if (num != 0)
				{
					scnCLS.HideInitialMenu();
					featuredLevelsMode = false;
					awaiter = scnCLS.Refresh(setup: true).GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter;
						_003C_003Et__builder.AwaitUnsafeOnCompleted(ref awaiter, ref this);
						return;
					}
				}
				else
				{
					awaiter = _003C_003Eu__1;
					_003C_003Eu__1 = default(TaskAwaiter);
					num = (_003C_003E1__state = -1);
				}
				awaiter.GetResult();
				RDBaseDll.printem("");
			}
			catch (Exception exception)
			{
				_003C_003E1__state = -2;
				_003C_003Et__builder.SetException(exception);
				return;
			}
			_003C_003E1__state = -2;
			_003C_003Et__builder.SetResult();
		}

		void IAsyncStateMachine.MoveNext()
		{
			//ILSpy generated this explicit interface implementation from .override directive in MoveNext
			this.MoveNext();
		}

		[DebuggerHidden]
		private void SetStateMachine(IAsyncStateMachine stateMachine)
		{
			_003C_003Et__builder.SetStateMachine(stateMachine);
		}

		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine stateMachine)
		{
			//ILSpy generated this explicit interface implementation from .override directive in SetStateMachine
			this.SetStateMachine(stateMachine);
		}
	}

	private const string FeaturedLevelsLocalPath = "FeaturedLevels";

	public static scnCLS instance;

	public static int loadSongMode;

	[Header("Components")]
	public OptionsPanelsCLS optionsPanels;

	public Button workshopButton;

	public PreviewSongPlayer previewSongPlayer;

	public SpriteRenderer bgSprite;

	public Transform gemTop;

	public Transform gemBottom;

	public Transform gemExitFolder;

	public GameObject chainTop;

	public GameObject chainBottom;

	public Text currentSearchText;

	public Text loadingText;

	[Header("Intro")]
	public GameObject introContainer;

	public Button btnIntroOk;

	public Button btnIntroBrowse;

	[Header("Canvas")]
	public RectTransform levelInfoCanvas;

	public RectTransform errorCanvas;

	public CanvasScaler canvasScaler;

	public CanvasGroup levelInfoCanvasGroup;

	[Header("Floor")]
	public Transform floorContainer;

	public SpriteRenderer entranceIcon;

	public scrFloor entranceTile;

	public SpriteRenderer downloadIcon;

	public SpriteRenderer downloadingIcon;

	public Text downloadText;

	public Text downloadingText;

	public GameObject DLCEntranceIcon;

	[Header("Portal")]
	public Transform signContainer;

	public Transform portalAndSign;

	public Transform portalContainer;

	public PortalQuad portalQuad;

	public RectTransform seizureWarning;

	public RectTransform newlyInstalledSign;

	public RectTransform DLCWarningSign;

	public Text[] DLCWarningText;

	public scrPortal portalScript;

	public Transform portalSign;

	public ParticleSystem portalTransitionParticle;

	public Texture2D emptyTexture;

	public GameObject padlockContainer;

	[Header("Level Information")]
	public Text portalArtist;

	public Transform artistMediaContainer;

	public Text portalName;

	public Text portalDescription;

	public Text portalDifficultyText;

	public DifficultyIndicator portalDifficulty;

	public Text portalAuthor;

	public Text portalStats;

	public Text portalFutureVersion;

	[Header("Prefabs")]
	public GameObject tilePrefab;

	public GameObject mediaButton;

	[Header("Tweakables")]
	public float levelCountForLoop;

	public float secondsForHold;

	public float secondsForHoldExtra;

	public float autoScrollInterval;

	public float portalTransitionTimeNormal;

	public float portalTransitionTimeInstant;

	public float portalImageLoadDelay;

	[Header("Initial Menu")]
	public GameObject initialPath;

	public GameObject featuredPortal;

	public GameObject workshopPortal;

	public PortalQuad featuredPortalQuad;

	public PortalQuad workshopPortalQuad;

	public Texture2D featuredPortalTexture;

	public Texture2D workshopPortalTexture;

	public Canvas initialQuitLabel;

	[NonSerialized]
	public int gemTopY = 99;

	[NonSerialized]
	public int gemBottomY = -99;

	[NonSerialized]
	public int levelCount;

	[NonSerialized]
	public bool showingInitialMenu = true;

	[NonSerialized]
	public List<string> sortedLevelKeys;

	[NonSerialized]
	public List<string> newlyInstalledLevelKeys = new List<string>();

	public Dictionary<string, GenericDataCLS> loadedLevels = new Dictionary<string, GenericDataCLS>();

	private Dictionary<string, GenericDataCLS> extraLevels = new Dictionary<string, GenericDataCLS>();

	private Dictionary<string, string> loadedLevelDirs = new Dictionary<string, string>();

	private Dictionary<string, CustomLevelTile> loadedLevelTiles = new Dictionary<string, CustomLevelTile>();

	private Dictionary<string, bool> loadedLevelIsDeleted = new Dictionary<string, bool>();

	private Dictionary<string, bool> isWorkshopLevel = new Dictionary<string, bool>();

	private LayerMask floorLayerMask;

	private string levelToSelect;

	private float holdTimer;

	private float autoscrollTimer;

	private float levelTransitionTimer;

	private bool changingLevel;

	private bool disablePlanets;

	private scrCamera camera;

	private string newSongKey;

	private Coroutine loadSongCoroutine;

	private bool instantSelect;

	private scrExtImgHolder imgHolder;

	[NonSerialized]
	public string searchParameter = "";

	private List<string> lastSongsLoaded;

	private List<string> lastTexturesLoaded;

	private CancellationTokenSource refreshTokenSource;

	private string levelsDir;

	[NonSerialized]
	public int lastFrameSearchModeAvailable = -1;

	private string currentFolderName;

	private bool wasInPortalPreviousFrame;

	[NonSerialized]
	public bool refreshing;

	[NonSerialized]
	public bool initializing = true;

	private static bool featuredLevelsMode;

	private float currentSongVolume;

	private Tween seizureWarningAnimation;

	private Tween newLevelAnimation;

	private Tween DLCWarningAnimation;

	private Tween delayedTextureLoad;

	private Text[] portalTexts => new Text[7]
	{
		portalStats,
		portalDifficultyText,
		portalScript.worldName,
		portalAuthor,
		portalDescription,
		portalArtist,
		portalName
	};

	public bool levelDeleted => loadedLevelIsDeleted[levelToSelect];

	private void Awake()
	{
		instance = this;
		initializing = true;
		levelsDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "A Dance of Fire and Ice", "Worlds");
		floorLayerMask = LayerMask.GetMask("Floor");
		camera = scrCamera.instance;
		Text[] portalTexts = this.portalTexts;
		foreach (Text obj in portalTexts)
		{
			obj.SetLocalizedFont();
			obj.text = "";
		}
		loadingText.SetLocalizedFont();
		loadingText.text = RDString.Get("status.loading");
		imgHolder = new scrExtImgHolder();
		uint[] extraLevelsIds = GCNS.ExtraLevelsIds;
		for (int i = 0; i < extraLevelsIds.Length; i++)
		{
			uint id = extraLevelsIds[i];
			extraLevels.Add(id.ToString(), _003CAwake_003Eg__DecodeLevel_007C108_0(id));
		}
		string text = "Folder:Feral";
		FolderDataCLS folderDataCLS = new FolderDataCLS("Feral", 4, "meganeko", "", RDString.Get("workshop.FeralFolder.description"), "portal.png", "icon.png", "EEAAFF".HexToColor());
		extraLevelsIds = GCNS.FeralNormalLevelsIds;
		for (int i = 0; i < extraLevelsIds.Length; i++)
		{
			uint id2 = extraLevelsIds[i];
			LevelDataCLS value = _003CAwake_003Eg__DecodeLevel_007C108_0(id2);
			folderDataCLS.containingLevels.Add(id2.ToString(), value);
			extraLevels[id2.ToString()].parentFolderName = text;
		}
		extraLevels.Add(text, folderDataCLS);
		string text2 = "Folder:Skyscape";
		FolderDataCLS folderDataCLS2 = new FolderDataCLS("Skyscape", 5, "Plum", "", RDString.Get("workshop.SkyscapeFolder.description"), "portal.png", "icon.png", "15b7f8".HexToColor());
		extraLevelsIds = GCNS.SkyscapeLevelsIds;
		for (int i = 0; i < extraLevelsIds.Length; i++)
		{
			uint id3 = extraLevelsIds[i];
			LevelDataCLS value2 = _003CAwake_003Eg__DecodeLevel_007C108_0(id3);
			folderDataCLS2.containingLevels.Add(id3.ToString(), value2);
			extraLevels[id3.ToString()].parentFolderName = text2;
		}
		extraLevels.Add(text2, folderDataCLS2);
		featuredPortalQuad.SetTexture(featuredPortalTexture);
		workshopPortalQuad.SetTexture(workshopPortalTexture);
		string text3 = "[dlc]";
		string text4 = RDString.Get("cls.needsDLC");
		int num = text4.IndexOf(text3);
		DLCWarningText[0].text = text4.Substring(0, num);
		DLCWarningText[0].SetLocalizedFont();
		DLCWarningText[1].text = text4.Substring(num + text3.Length);
		DLCWarningText[1].SetLocalizedFont();
	}

	private void Start()
	{
		RDInput.SetMapping("CLS");
		SubscribeToFeatured();
		loadingText.rectTransform.DOScale(1.1f * Vector3.one, 0.5f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);
		DisableCLS(disable: true);
		ADOBase.controller.chosenplanet.transform.LocalMoveXY(0f, 0f);
		workshopButton.onClick.AddListener(delegate
		{
			SteamWorkshop.OpenWorkshop();
		});
		btnIntroBrowse.onClick.AddListener(delegate
		{
			SteamWorkshop.OpenWorkshop();
		});
		btnIntroOk.onClick.AddListener(delegate
		{
			Persistence.SetDisplayedCLSIntro(displayed: true);
			introContainer.SetActive(value: false);
		});
		portalFutureVersion.text = RDString.Get("cls.worldFutureVersion");
		if (!Persistence.GetDisplayedCLSIntro())
		{
			introContainer.SetActive(value: true);
		}
		if (!Path.GetFileName(Path.GetDirectoryName(GCS.customLevelPaths?.Last())).IsNullOrEmpty())
		{
			HideInitialMenu();
			Refresh(setup: true);
		}
		printe("initializing = false");
		initializing = false;
	}

	public void SubscribeToFeatured()
	{
		ulong[] featuredWorkshopLevels = ADOBase.gc.featuredWorkshopLevels;
		foreach (ulong num in featuredWorkshopLevels)
		{
			if (!Persistence.HasSubscribedToFeatured(num))
			{
				SteamWorkshop.Subscribe(new PublishedFileId_t(num));
				Persistence.SetSubscribedToFeatured(num, subscribed: true);
			}
		}
	}

	private void Update()
	{
		float num = (float)Screen.width * 1f / (float)Screen.height;
		float num2 = canvasScaler.referenceResolution.x / canvasScaler.referenceResolution.y;
		bool flag = num >= num2;
		canvasScaler.matchWidthOrHeight = (flag ? 1f : 0f);
		float num3 = Mathf.Max(1f, num2 / num);
		camera.camobj.orthographicSize = 5f * num3;
		signContainer.LocalMoveY(camera.camobj.orthographicSize - 1.4f);
		SteamIntegration.Instance.CheckCallbacks();
		SteamWorkshop.CheckDownloadInfo();
		bool flag2 = Mathf.Approximately(ADOBase.controller.chosenplanet.transform.localPosition.x, -1f);
		bool flag3;
		int num6;
		if (flag2)
		{
			flag3 = !wasInPortalPreviousFrame;
		}
		else
			num6 = 0;
		wasInPortalPreviousFrame = flag2;
		if (disablePlanets)
		{
			ADOBase.controller.responsive = false;
		}
		if (levelToSelect != null)
		{
			if (loadedLevels[levelToSelect].isLevel && ulong.TryParse(levelToSelect, out ulong result))
			{
				PublishedFileId_t publishedFileId_t = new PublishedFileId_t(result);
				EItemState itemState = (EItemState)SteamUGC.GetItemState(publishedFileId_t);
				if (itemState.HasFlag(EItemState.k_EItemStateSubscribed) && itemState.HasFlag(EItemState.k_EItemStateInstalled))
				{
					downloadIcon.enabled = false;
					downloadText.enabled = false;
					downloadingIcon.enabled = false;
					downloadingText.enabled = false;
					if (loadedLevelDirs.ContainsKey(levelToSelect) && loadedLevelDirs[levelToSelect] == null)
					{
						string pchFolder = "";
						if (SteamUGC.GetItemInstallInfo(publishedFileId_t, out ulong _, out pchFolder, 1024u, out uint _))
						{
							loadedLevelDirs[levelToSelect] = pchFolder;
							if (flag2)
							{
								EnterLevel();
							}
						}
					}
				}
				else if (!SteamWorkshop.ItemIsUsable(publishedFileId_t))
				{
					downloadIcon.enabled = false;
					downloadText.enabled = false;
					downloadingIcon.enabled = true;
					downloadingText.enabled = true;
				}
				else
				{
					downloadIcon.enabled = true;
					downloadText.enabled = true;
					downloadingIcon.enabled = false;
					downloadingText.enabled = false;
				}
			}
			else
			{
				downloadIcon.enabled = false;
				downloadText.enabled = false;
				downloadingIcon.enabled = false;
				downloadingText.enabled = false;
			}
			downloadingIcon.transform.localEulerAngles = Vector3.back * Time.unscaledTime * 150f;
		}
		if (!ADOBase.controller.paused && !optionsPanels.searchMode && !showingInitialMenu)
		{
			if (optionsPanels.ChecksInputs() || !ADOBase.controller.responsive)
			{
				return;
			}
			if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha7))
			{
				loadSongMode = (loadSongMode + 1) % 3;
				if (loadSongMode == 0)
				{
					MonoBehaviour.print("loading all");
				}
				if (loadSongMode == 1)
				{
					MonoBehaviour.print("not loading mp3s");
				}
				if (loadSongMode == 2)
				{
					MonoBehaviour.print("loading none");
				}
			}
			if (!ADOBase.controller.moving)
			{
				if (RDInput.upPress || RDInput.downPress)
				{
					holdTimer += Time.deltaTime;
				}
				else
				{
					holdTimer = 0f;
					autoscrollTimer = 0f;
				}
				if (holdTimer > secondsForHold)
				{
					float num4 = (holdTimer > secondsForHoldExtra) ? 2f : 1f;
					autoscrollTimer += Time.deltaTime * num4;
					if (autoscrollTimer > autoScrollInterval)
					{
						ShiftPlanet(!RDInput.upPress);
						autoscrollTimer = 0f;
					}
				}
				else if (RDInput.upPress || Input.mouseScrollDelta.y > 0.4f)
				{
					ShiftPlanet(down: false);
				}
				else if (RDInput.downPress || Input.mouseScrollDelta.y < -0.4f)
				{
					ShiftPlanet(down: true);
				}
			}
		}
		if (!changingLevel || disablePlanets)
		{
			return;
		}
		float num5 = instantSelect ? portalTransitionTimeInstant : portalTransitionTimeNormal;
		if (levelTransitionTimer >= num5)
		{
			DisplayLevel(levelToSelect);
			if (loadedLevels[levelToSelect].isLevel)
			{
				LevelDataCLS level = loadedLevels[levelToSelect].level;
				if (!string.IsNullOrEmpty(level.songFilename))
				{
					string text = loadedLevelDirs[levelToSelect];
					if (text != null)
					{
						string text2 = Path.Combine(text, level.songFilename);
						printe("levelData.songFilename: " + level.songFilename);
						newSongKey = Path.GetFileName(text2) + "*external";
						if (!text2.ToLower().EndsWith(".mp3"))
						{
							loadSongCoroutine = StartCoroutine(LoadSong(text2, newSongKey));
						}
					}
				}
			}
			instantSelect = false;
		}
		else
		{
			levelTransitionTimer += Time.deltaTime;
		}
	}

	private void LateUpdate()
	{
		portalAndSign.MoveY(camera.yGlobal);
	}

	private void OnDestroy()
	{
		if (refreshTokenSource != null)
		{
			refreshTokenSource.Cancel();
		}
	}

	private IEnumerator LoadSong(string path, string songKey)
	{
		yield return AudioManager.Instance.FindOrLoadAudioClipExternal(path, mp3Streaming: true);
		Dictionary<string, AudioClip> audioLib = ADOBase.audioManager.audioLib;
		if (audioLib.ContainsKey(songKey))
		{
			AudioClip audioClip = audioLib[songKey];
			if (audioClip != null && loadedLevels[levelToSelect].isLevel)
			{
				LevelDataCLS level = loadedLevels[levelToSelect].level;
				if (songKey == newSongKey)
				{
					previewSongPlayer.Play(audioClip, level.previewSongStart, level.previewSongDuration, currentSongVolume);
				}
				else
				{
					printe("song was quickly changed!");
				}
				lastSongsLoaded.Add(songKey);
				while (lastSongsLoaded.Count > 5)
				{
					string key = lastSongsLoaded[0];
					lastSongsLoaded.RemoveAt(0);
					if (ADOBase.audioManager.audioLib.ContainsKey(key))
					{
						AudioClip audioClip2 = ADOBase.audioManager.audioLib[key];
						ADOBase.audioManager.audioLib.Remove(key);
						audioClip2.UnloadAudioData();
						UnityEngine.Object.DestroyImmediate(audioClip2, allowDestroyingAssets: true);
						Resources.UnloadAsset(audioClip2);
					}
				}
			}
		}
		else
		{
			UnityEngine.Debug.LogWarning("Couldn't load preview: " + songKey);
		}
		loadSongCoroutine = null;
	}

	public void DeleteLevel()
	{
		if (loadedLevelIsDeleted[levelToSelect])
		{
			return;
		}
		scrFlash.Flash(Color.white, 0.5f);
		changingLevel = true;
		levelTransitionTimer = 0f;
		loadedLevelTiles[levelToSelect].SetDeleted();
		if (isWorkshopLevel.ContainsKey(levelToSelect) && isWorkshopLevel[levelToSelect])
		{
			if (ulong.TryParse(levelToSelect, out ulong result))
			{
				foreach (SteamWorkshop.ResultItem resultItem in SteamWorkshop.resultItems)
				{
					if ((ulong)resultItem.id == result)
					{
						SteamWorkshop.Unsubscribe(resultItem.id);
					}
				}
			}
			else
			{
				printe("parse failed");
			}
		}
		else
		{
			string text = loadedLevelDirs[levelToSelect];
			if (text != null)
			{
				Directory.Delete(text, recursive: true);
			}
		}
		loadedLevelIsDeleted[levelToSelect] = true;
		DisplayLevel();
	}

	[AsyncStateMachine(typeof(_003CRefresh_003Ed__116))]
	public Task Refresh(bool setup = false)
	{
		_003CRefresh_003Ed__116 stateMachine = default(_003CRefresh_003Ed__116);
		stateMachine._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
		stateMachine._003C_003E4__this = this;
		stateMachine.setup = setup;
		stateMachine._003C_003E1__state = -1;
		stateMachine._003C_003Et__builder.Start(ref stateMachine);
		return stateMachine._003C_003Et__builder.Task;
	}

	private void DisableCLS(bool disable)
	{
		StopCurrentLevelSong();
		levelInfoCanvas.gameObject.SetActive(!disable);
		entranceTile.gameObject.SetActive(!disable);
		portalAndSign.gameObject.SetActive(!disable);
		gemTop.gameObject.SetActive(value: false);
		gemBottom.gameObject.SetActive(value: false);
	}

	private void DisablePlanets(bool disable)
	{
		disablePlanets = disable;
		ADOBase.controller.responsive = !disable;
		if (disable)
		{
			ADOBase.controller.chosenplanet.transform.MoveX(-99f);
		}
	}

	private void ShiftPlanet(bool down)
	{
		int num = (!down) ? 1 : (-1);
		Vector3 position = ADOBase.controller.chosenplanet.transform.position;
		position = new Vector3(position.x, position.y + (float)num, position.z);
		MoveToFloorAtPosition(position);
		instantSelect = true;
	}

	private void MoveToFloorAtPosition(Vector3 position, bool ignoreFfx = false)
	{
		RaycastHit2D[] array = Physics2D.RaycastAll(new Vector2(position.x, position.y), new Vector2(0f, 0f), 0f, floorLayerMask);
		if (array.Length == 0 || array == null)
		{
			return;
		}
		Transform transform = array[0].collider.transform;
		Transform transform2 = ADOBase.controller.chosenplanet.transform;
		transform2.position = transform.position;
		ADOBase.controller.chosenplanet.currfloor = transform.GetComponent<scrFloor>();
		if (!ignoreFfx)
		{
			ffxBase[] components = transform.GetComponents<ffxBase>();
			foreach (ffxBase item in components)
			{
				ADOBase.controller.chosenplanet.DoFFX(item);
			}
		}
		camera.Refocus(transform2);
	}

	public void SelectLevel(CustomLevelTile tileToSelect, bool snap)
	{
		printe("selectingLevel: " + tileToSelect.gameObject.name);
		Transform component = tileToSelect.GetComponent<Transform>();
		if (snap)
		{
			camera.frompos.y = component.position.y;
			tileToSelect.Highlight(highlight: true, snap);
			MoveToFloorAtPosition(tileToSelect.transform.position);
			camera.timer += float.MaxValue;
		}
		foreach (CustomLevelTile value in loadedLevelTiles.Values)
		{
			if (value != tileToSelect)
			{
				value.Highlight(highlight: false, snap);
			}
		}
		int targetIndex = -1;
		for (int i = 0; i < sortedLevelKeys.Count; i++)
		{
			if (loadedLevelTiles[sortedLevelKeys[i]] == tileToSelect)
			{
				targetIndex = i;
				break;
			}
		}
		LoadTileIconsNearby(targetIndex);
		if (!changingLevel)
		{
			DisplayLevel();
		}
		levelToSelect = tileToSelect.levelKey;
		StopCurrentLevelSong();
		changingLevel = true;
		levelTransitionTimer = 0f;
	}

	private void StopCurrentLevelSong()
	{
		if (previewSongPlayer.playing)
		{
			previewSongPlayer.Stop();
		}
		AudioClipData audioClipData = ADOBase.audioManager.audioClipData;
		if (audioClipData != null && !audioClipData.loaded)
		{
			audioClipData.StopLoading();
			ADOBase.audioManager.audioClipData = null;
		}
	}

	public void LoadTexture(string path, string levelKey)
	{
		if (!File.Exists(path))
		{
			RDBaseDll.printem("file doesnt exist: " + path);
			return;
		}
		if (!imgHolder.customTextures.ContainsKey(levelKey))
		{
			imgHolder.AddTexture(levelKey, out LoadResult _, path, 512);
			lastTexturesLoaded.Add(levelKey);
			while (lastTexturesLoaded.Count > 5)
			{
				string key = lastTexturesLoaded[0];
				imgHolder.UnloadTexture(key);
				lastTexturesLoaded.RemoveAt(0);
			}
		}
		if (levelKey == levelToSelect)
		{
			Texture2D texture = imgHolder.customTextures[levelKey].texture;
			portalQuad.SetTexture(texture);
		}
		portalTransitionParticle.Stop(withChildren: true, ParticleSystemStopBehavior.StopEmitting);
	}

	public void DisplayLevel(string levelKey = null)
	{
		changingLevel = false;
		bool flag = levelKey != null;
		if (flag && loadedLevelIsDeleted[levelKey])
		{
			return;
		}
		float animDur = 0.5f;
		bool flag2 = flag;
		if (flag)
		{
			string str = "<color=white>";
			string text = "</color>";
			bool flag3 = false;
			bool flag4 = false;
			foreach (Transform item in artistMediaContainer.transform)
			{
				UnityEngine.Object.Destroy(item.gameObject);
			}
			GenericDataCLS genericDataCLS = loadedLevels[levelKey];
			if (genericDataCLS.isFolder)
			{
				FolderDataCLS folder = genericDataCLS.folder;
				flag3 = true;
				foreach (LevelDataCLS value in folder.containingLevels.Values)
				{
					if (!value.seizureWarning)
					{
						flag3 = false;
						break;
					}
				}
			}
			else
			{
				LevelDataCLS level = genericDataCLS.level;
				string hash = level.Hash;
				float num = Persistence.GetCustomWorldCompletion(hash) * 1f;
				bool flag5 = num >= 1f;
				bool num2 = num != 0f || Persistence.GetCustomWorldPlayIndex(hash) != -1;
				float customWorldAccuracy = Persistence.GetCustomWorldAccuracy(hash);
				float customWorldXAccuracy = Persistence.GetCustomWorldXAccuracy(hash);
				bool flag6 = Persistence.GetShowXAccuracy() && customWorldXAccuracy != 0f;
				float num3 = flag6 ? customWorldXAccuracy : customWorldAccuracy;
				int customWorldAttempts = Persistence.GetCustomWorldAttempts(hash);
				float customWorldSpeedTrial = Persistence.GetCustomWorldSpeedTrial(hash);
				int customWorldMinDeaths = Persistence.GetCustomWorldMinDeaths(hash);
				_003C_003Ec__DisplayClass129_1 _003C_003Ec__DisplayClass129_ = default(_003C_003Ec__DisplayClass129_1);
				_003C_003Ec__DisplayClass129_.purePerfected = (flag6 ? (customWorldXAccuracy == 1f) : Persistence.GetCustomWorldIsHighestPossibleAcc(hash));
				string str2 = RDString.Get("levelSelect.multiplier", new Dictionary<string, object>
				{
					{
						"multiplier",
						customWorldSpeedTrial.ToString("0.0")
					}
				});
				Dictionary<string, object> parameters = new Dictionary<string, object>
				{
					{
						"pctCompleted",
						str + Mathf.FloorToInt(num * 100f).ToString("0") + "%" + text
					},
					{
						"pctAccuracy",
						str + _003CDisplayLevel_003Eg__GoldAccuracy_007C129_3((num3 * 100f).ToString("0.00") + "%", ref _003C_003Ec__DisplayClass129_) + text
					},
					{
						"speedTrial",
						str + str2 + text
					},
					{
						"attempts",
						str + customWorldAttempts.ToString("0") + text
					}
				};
				string text2 = (!num2) ? RDString.Get("levelSelect.neverPlayed", parameters) : ((!flag5) ? RDString.Get("cls.worldStatsIncomplete", parameters) : ((!flag6) ? RDString.Get("cls.worldStatsComplete", parameters) : RDString.Get("cls.worldStatsCompleteXAccuracy", parameters)));
				if (customWorldMinDeaths >= 0)
				{
					string str3 = (customWorldMinDeaths == 0) ? RDString.Get("cls.noCheckpointsUsed") : customWorldMinDeaths.ToString();
					text2 = text2 + "\n" + RDString.Get("cls.lowestCheckpointsUsed", new Dictionary<string, object>
					{
						{
							"checkpoints",
							str + str3 + text
						}
					});
				}
				portalStats.text = text2;
				portalScript.UpdateLanternStates(flag5, customWorldAccuracy >= 1f, customWorldSpeedTrial > 1f);
				int num4 = CustomLevel.GetWorldPaths(loadedLevelDirs[levelKey] + Path.DirectorySeparatorChar.ToString() + "main.adofai").Length;
				portalScript.worldName.text = ((num4 > 1) ? (RDString.Get("cls.worldCount", new Dictionary<string, object>
				{
					{
						"levelCount",
						num4
					}
				}).Replace("\n", $"\n<size={Mathf.RoundToInt((float)portalScript.worldName.fontSize * 0.5f)}>") + "</size>") : RDString.Get("cls.singleLevel"));
				currentSongVolume = (float)level.volume / 100f;
				string artistLinks = level.artistLinks;
				if (artistLinks != "")
				{
					string[] array = artistLinks.Replace(" ", "").Split(',');
					for (int i = 0; i < array.Length; i++)
					{
						string link = array[i];
						string[] array2 = link.Replace("https://", "").Replace("http://", "").Replace("www.", "")
							.Split('.');
						if (array2.Length < 2)
						{
							RDBaseDll.printem("linkParts is smaller than 2! first part is " + array2[0]);
							continue;
						}
						GameObject gameObject = UnityEngine.Object.Instantiate(mediaButton, artistMediaContainer);
						string str4 = "link_white";
						switch (array2[0])
						{
						case "youtube":
						case "youtu":
							str4 = "youtube";
							break;
						case "spotify":
						case "bandcamp":
						case "twitter":
						case "soundcloud":
							str4 = array2[0];
							break;
						}
						string a = array2[1];
						if (a == "bandcamp" || a == "spotify")
						{
							str4 = array2[1];
						}
						gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>("CLS/MediaIcons/" + str4);
						gameObject.GetComponent<Button>().onClick.AddListener(delegate
						{
							if (!link.StartsWith("http://") && !link.StartsWith("https://") && !link.StartsWith("www."))
							{
								link = "https://" + link;
							}
							printe("Attempting to open URL: " + link);
							Application.OpenURL(link);
							EventSystem.current.SetSelectedGameObject(null);
						});
					}
				}
				if (level.loadResult == LoadResult.FutureVersion)
				{
					portalFutureVersion.gameObject.SetActive(value: true);
					flag2 = false;
					entranceTile.floorRenderer.material.DOColor(Color.gray, animDur);
				}
				else if (portalFutureVersion.gameObject.activeSelf)
				{
					portalFutureVersion.gameObject.SetActive(value: false);
					entranceTile.floorRenderer.material.DOColor(Color.white, animDur);
				}
				flag3 = level.seizureWarning;
				flag4 = level.requiresNeoCosmos;
			}
			LayoutRebuilder.ForceRebuildLayoutImmediate(portalArtist.GetComponent<RectTransform>());
			bool exists;
			string withCheck = RDString.GetWithCheck("workshop." + levelKey + ".description", out exists);
			if (!exists)
			{
				withCheck = RDString.GetWithCheck(genericDataCLS.description, out exists);
			}
			string text3 = (exists && !withCheck.IsNullOrEmpty()) ? withCheck : ((genericDataCLS.description == string.Empty) ? string.Empty : ("\"" + RDUtils.RemoveRichTags(genericDataCLS.description) + "\""));
			portalDescription.text = text3;
			portalDescription.SetLocalizedFont();
			string text4 = RDUtils.RemoveRichTags(genericDataCLS.artist);
			portalArtist.text = text4;
			portalArtist.SetLocalizedFont();
			string text5 = RDUtils.RemoveRichTags(genericDataCLS.title);
			portalName.text = text5;
			portalName.SetLocalizedFont();
			string text6 = RDString.Get("cls.difficulty");
			portalDifficultyText.text = text6;
			int difficulty = genericDataCLS.difficulty;
			portalDifficulty.SetStars(difficulty);
			string text7 = RDString.Get("cls.worldAuthor", new Dictionary<string, object>
			{
				{
					"author",
					str + RDUtils.RemoveRichTags(genericDataCLS.author) + text
				}
			});
			portalAuthor.text = text7;
			portalAuthor.SetLocalizedFont();
			if (delayedTextureLoad != null && delayedTextureLoad.IsActive())
			{
				delayedTextureLoad.Kill();
			}
			if (genericDataCLS.isFolder)
			{
				string path = levelKey.Replace("Folder:", "");
				Texture2D texture = Resources.Load<Texture2D>(Path.Combine("FeaturedLevels", path, "portal"));
				delayedTextureLoad = DOVirtual.DelayedCall(portalImageLoadDelay, delegate
				{
					portalQuad.SetTexture(texture);
					portalTransitionParticle.Stop(withChildren: true, ParticleSystemStopBehavior.StopEmitting);
				});
			}
			else if (genericDataCLS.previewImage != "")
			{
				string text8 = loadedLevelDirs[levelKey];
				if (text8 != null)
				{
					string portalImagePath = Path.Combine(text8, genericDataCLS.previewImage);
					delayedTextureLoad = DOVirtual.DelayedCall(portalImageLoadDelay, delegate
					{
						LoadTexture(portalImagePath, levelKey);
					});
				}
			}
			else
			{
				delayedTextureLoad = DOVirtual.DelayedCall(portalImageLoadDelay, delegate
				{
					portalQuad.SetTexture(emptyTexture);
				});
			}
			portalContainer.DOScale(new Vector3(1f, 1f, 0f), animDur).SetEase(Ease.OutBack);
			portalQuad.Fade(1f, animDur);
			if (flag4)
			{
				_003CDisplayLevel_003Eg__DoWarningAnimation_007C129_2(DLCWarningSign, DLCWarningAnimation, left: false);
			}
			else if (flag3)
			{
				_003CDisplayLevel_003Eg__DoWarningAnimation_007C129_2(seizureWarning, seizureWarningAnimation, left: false);
			}
			if (newlyInstalledLevelKeys.Contains(levelKey))
			{
				_003CDisplayLevel_003Eg__DoWarningAnimation_007C129_2(newlyInstalledSign, newLevelAnimation, left: true);
			}
			DLCEntranceIcon.SetActive(flag4);
			padlockContainer.SetActive(flag4);
			portalStats.enabled = genericDataCLS.isLevel;
			portalAuthor.enabled = !genericDataCLS.author.IsNullOrEmpty();
		}
		else
		{
			portalTransitionParticle.Play(withChildren: true);
			portalContainer.DOScale(new Vector3(0.85f, 0.85f, 0f), animDur).SetEase(Ease.InOutSine);
			portalQuad.Fade(0f, animDur);
			DLCEntranceIcon.SetActive(value: false);
			padlockContainer.SetActive(value: false);
			_003C_003Ec__DisplayClass129_0 CS_0024_003C_003E8__locals3;
			CS_0024_003C_003E8__locals3._003CDisplayLevel_003Eg__DoWarningAnimation_007C0(seizureWarning, seizureWarningAnimation);
			CS_0024_003C_003E8__locals3._003CDisplayLevel_003Eg__DoWarningAnimation_007C0(newlyInstalledSign, newLevelAnimation);
			CS_0024_003C_003E8__locals3._003CDisplayLevel_003Eg__DoWarningAnimation_007C0(DLCWarningSign, DLCWarningAnimation);
		}
		float num5 = flag ? 1f : 0f;
		levelInfoCanvasGroup.DOFade(num5, animDur);
		entranceIcon.DOFade(flag2 ? num5 : 0f, animDur);
		portalSign.DOLocalMoveY(flag ? 0f : 11f, animDur).SetEase((!flag) ? Ease.Linear : Ease.OutSine);
		entranceTile.isLandable = flag2;
	}

	public void EnterLevel()
	{
		printe("enter level");
		GCS.customLevelIndex = 0;
		GCS.speedTrialMode = optionsPanels.speedTrial;
		GCS.practiceMode = false;
		ADOBase.audioManager.StopLoadingMP3File();
		string text = loadedLevelDirs[levelToSelect];
		GenericDataCLS genericDataCLS = loadedLevels[levelToSelect];
		if (genericDataCLS.isFolder)
		{
			EnterFolder();
			return;
		}
		if (text == null)
		{
			printe("subscribing to " + levelToSelect);
			SteamWorkshop.Subscribe(new PublishedFileId_t(ulong.Parse(levelToSelect)));
			return;
		}
		if (!ADOBase.ownsTaroDLC && genericDataCLS.isLevel && genericDataCLS.level.requiresNeoCosmos)
		{
			SteamFriends.ActivateGameOverlayToWebPage("https://store.steampowered.com/app/" + 1977570u.ToString());
			return;
		}
		GCS.customLevelIndex = 0;
		GCS.speedTrialMode = optionsPanels.speedTrial;
		string levelPath = Path.Combine(text, "main.adofai");
		ADOBase.audioManager.StopLoadingMP3File();
		string hash = loadedLevels[levelToSelect].Hash;
		Persistence.IncrementCLSTotalPlays();
		Persistence.SetCustomWorldPlayIndex(hash, Persistence.GetCLSTotalPlays());
		GCS.nextSpeedRun = (GCS.speedTrialMode ? 1.1f : 1f);
		bool skipToMain = Persistence.GetCustomWorldAttempts(hash) > 0;
		if (GCS.speedTrialMode)
		{
			ADOBase.controller.LoadCustomLevel(levelPath);
		}
		else
		{
			ADOBase.controller.LoadCustomWorld(levelPath, skipToMain);
		}
	}

	public void EnterFolder()
	{
		DOVirtual.DelayedCall(0f, delegate
		{
			currentFolderName = levelToSelect;
			sortedLevelKeys = optionsPanels.SortedLevelKeys();
			SearchLevels(searchParameter);
		});
	}

	public void ExitFolder()
	{
		DOVirtual.DelayedCall(0f, delegate
		{
			string key = currentFolderName;
			currentFolderName = null;
			sortedLevelKeys = optionsPanels.SortedLevelKeys();
			SearchLevels(searchParameter, alsoSelect: false);
			SelectLevel(loadedLevelTiles[key], snap: true);
		});
	}

	private void BrowseZip()
	{
		ExtensionFilter[] extensions = new ExtensionFilter[1]
		{
			new ExtensionFilter(RDString.Get("editor.dialog.adofaizipDescription"), GCS.levelZipExtensions)
		};
		string[] array = StandaloneFileBrowser.OpenFilePanel(RDString.Get("editor.dialog.importAdofaizip"), Persistence.GetLastUsedFolder(), extensions, multiselect: true);
		if (array.Length != 0 && !string.IsNullOrEmpty(array[0]))
		{
			Persistence.UpdateLastUsedFolder(array[0]);
			int num = 0;
			string[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				string text = array2[i];
				UnpackZip(array[num]);
				num++;
			}
			Refresh();
		}
	}

	private void UnpackZip(string zip)
	{
		string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(zip);
		string text = Path.Combine(levelsDir, fileNameWithoutExtension);
		if (!Directory.Exists(text))
		{
			try
			{
				ZipUtil.Unzip(zip, text);
			}
			catch (Exception ex)
			{
				UnityEngine.Debug.LogError("Unzip failed: " + ex.ToString());
			}
		}
		else
		{
			UnityEngine.Debug.LogWarning("Level not unpacked: Directory already exists!");
		}
	}

	[AsyncStateMachine(typeof(_003CScanLevels_003Ed__135))]
	private Task ScanLevels(CancellationToken cancelToken, bool workshop = true, bool local = false)
	{
		_003CScanLevels_003Ed__135 stateMachine = default(_003CScanLevels_003Ed__135);
		stateMachine._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
		stateMachine._003C_003E4__this = this;
		stateMachine.cancelToken = cancelToken;
		stateMachine.workshop = workshop;
		stateMachine.local = local;
		stateMachine._003C_003E1__state = -1;
		stateMachine._003C_003Et__builder.Start(ref stateMachine);
		return stateMachine._003C_003Et__builder.Task;
	}

	private void CreateFloors()
	{
		if (loadedLevels.Count == 0)
		{
			RDBaseDll.printem("There are no levels :(");
			return;
		}
		sortedLevelKeys = optionsPanels.SortedLevelKeys();
		if (!featuredLevelsMode)
		{
			string path = Persistence.DataPath + Path.DirectorySeparatorChar.ToString() + "clslevels.txt";
			newlyInstalledLevelKeys = new List<string>();
			if (File.Exists(path))
			{
				List<string> list = new List<string>();
				string[] source = File.ReadAllLines(path);
				foreach (string sortedLevelKey in sortedLevelKeys)
				{
					if (!source.Contains(sortedLevelKey))
					{
						newlyInstalledLevelKeys.Add(sortedLevelKey);
					}
					else
					{
						list.Add(sortedLevelKey);
					}
				}
				if (newlyInstalledLevelKeys.Count != 0)
				{
					sortedLevelKeys = newlyInstalledLevelKeys.Union(list).ToList();
					File.WriteAllLines(path, sortedLevelKeys);
				}
			}
			else
			{
				File.WriteAllLines(path, sortedLevelKeys);
			}
		}
		string directoryName = Path.GetDirectoryName(GCS.customLevelPaths?.Last());
		bool flag = directoryName != null;
		string fileName = Path.GetFileName(directoryName);
		CustomLevelTile customLevelTile = null;
		CustomLevelTile customLevelTile2 = null;
		CustomLevelTile customLevelTile3 = null;
		CustomLevelTile customLevelTile4 = null;
		int num = loadedLevels.Count((KeyValuePair<string, GenericDataCLS> d) => d.Value.parentFolderName != currentFolderName);
		if (flag)
		{
			currentFolderName = loadedLevels[fileName].parentFolderName;
		}
		int num2 = 0;
		foreach (string sortedLevelKey2 in sortedLevelKeys)
		{
			GenericDataCLS genericDataCLS = loadedLevels[sortedLevelKey2];
			GameObject gameObject = UnityEngine.Object.Instantiate(tilePrefab, floorContainer);
			gameObject.name = sortedLevelKey2;
			gameObject.GetComponent<scrFloor>().topGlow.gameObject.SetActive(value: false);
			gameObject.transform.LocalMoveY(gameObject.transform.localPosition.y - (float)num2 + (float)Mathf.FloorToInt((levelCount - num) / 2));
			CustomLevelTile component = gameObject.GetComponent<CustomLevelTile>();
			loadedLevelTiles.Add(sortedLevelKey2, component);
			if (genericDataCLS.isFolder)
			{
				FolderDataCLS folder = genericDataCLS.folder;
			}
			else
			{
				LevelDataCLS level = loadedLevels[sortedLevelKey2].level;
				if (flag && (loadedLevelDirs[sortedLevelKey2] == directoryName || sortedLevelKey2 == fileName))
				{
					customLevelTile = component;
				}
				if (level.loadResult == LoadResult.FutureVersion)
				{
					component.MarkUnavailable();
				}
			}
			if (num2 == Mathf.FloorToInt((levelCount - num) / 2))
			{
				customLevelTile2 = component;
			}
			component.levelKey = sortedLevelKey2;
			string text = RDUtils.RemoveRichTags(genericDataCLS.title);
			bool flag2 = newlyInstalledLevelKeys.Contains(sortedLevelKey2);
			component.title.text = (flag2 ? ("<color=#368BE6>" + text + "</color>") : text);
			string text2 = RDUtils.RemoveRichTags(genericDataCLS.artist);
			component.artist.text = text2;
			component.image.enabled = false;
			if (genericDataCLS.parentFolderName != currentFolderName)
			{
				component.gameObject.SetActive(value: false);
			}
			else
			{
				if (num2 == 0)
				{
					customLevelTile3 = component;
				}
				customLevelTile4 = component;
				num2++;
			}
		}
		bool flag3 = currentFolderName != null;
		bool flag4 = (float)levelCount >= levelCountForLoop && !flag3;
		if (flag4)
		{
			gemTop.MoveY(customLevelTile3.transform.position.y + 1f);
			gemTopY = Mathf.RoundToInt(gemTop.position.y);
			gemBottom.MoveY(customLevelTile4.transform.position.y - 1f);
			gemBottomY = Mathf.RoundToInt(gemBottom.position.y);
		}
		else if (flag3)
		{
			gemExitFolder.MoveY(customLevelTile3.transform.position.y + 1f);
			gemTopY = Mathf.RoundToInt(gemExitFolder.position.y);
		}
		chainTop.transform.MoveY(customLevelTile3.transform.position.y);
		chainBottom.transform.MoveY(customLevelTile4.transform.position.y);
		gemTop.gameObject.SetActive(flag4);
		gemBottom.gameObject.SetActive(flag4);
		chainTop.gameObject.SetActive(!flag4 && !flag3);
		chainBottom.gameObject.SetActive(!flag4);
		gemExitFolder.gameObject.SetActive(flag3);
		SelectLevel((customLevelTile != null) ? customLevelTile : customLevelTile2, snap: true);
		ADOBase.controller.chosenplanet.cosmeticRadius = 1f;
	}

	public void ToggleSearchMode(bool search)
	{
	}

	private IEnumerator EnableInputCo()
	{
		yield return new WaitForEndOfFrame();
		ADOBase.controller.responsive = true;
	}

	public void SearchLevels(string sub, bool alsoSelect = true)
	{
		RDBaseDll.printem($"searchlevels {sub}, alsoSelect {alsoSelect}");
		if (initializing || refreshing)
		{
			printe($"impossible because initializing {initializing} ref {refreshing}");
			return;
		}
		searchParameter = sub;
		List<CustomLevelTile> list = new List<CustomLevelTile>();
		string value = sub.ToLower();
		foreach (string sortedLevelKey in sortedLevelKeys)
		{
			CustomLevelTile customLevelTile = loadedLevelTiles[sortedLevelKey];
			GenericDataCLS genericDataCLS = loadedLevels[sortedLevelKey];
			string[] array = new string[3]
			{
				genericDataCLS.artist,
				genericDataCLS.author,
				genericDataCLS.title
			};
			bool flag = false;
			if (loadedLevels[sortedLevelKey].parentFolderName == currentFolderName)
			{
				if (!sub.IsNullOrEmpty())
				{
					string[] array2 = array;
					for (int i = 0; i < array2.Length; i++)
					{
						if (array2[i].RemoveRichTags().ToLower().Contains(value))
						{
							flag = true;
							break;
						}
					}
				}
				else
				{
					flag = true;
				}
			}
			if (flag)
			{
				list.Add(customLevelTile);
			}
			else
			{
				customLevelTile.gameObject.SetActive(value: false);
			}
		}
		int num = Mathf.RoundToInt(ADOBase.controller.chosenplanet.transform.position.y);
		for (int j = 0; j < list.Count; j++)
		{
			CustomLevelTile customLevelTile2 = list[j];
			customLevelTile2.gameObject.SetActive(value: true);
			customLevelTile2.transform.MoveY(num - j);
		}
		bool flag2 = currentFolderName != null;
		bool flag3 = (float)list.Count >= levelCountForLoop && !flag2;
		if (list.Count != 0)
		{
			CustomLevelTile customLevelTile3 = list.First();
			CustomLevelTile customLevelTile4 = list.Last();
			if (flag3)
			{
				gemTop.MoveY(customLevelTile3.transform.position.y + 1f);
				gemTopY = Mathf.RoundToInt(gemTop.position.y);
				gemBottom.MoveY(customLevelTile4.transform.position.y - 1f);
				gemBottomY = Mathf.RoundToInt(gemBottom.position.y);
			}
			else if (flag2)
			{
				gemExitFolder.MoveY(customLevelTile3.transform.position.y + 1f);
				gemTopY = Mathf.RoundToInt(gemExitFolder.position.y);
			}
			chainTop.transform.MoveY(customLevelTile3.transform.position.y);
			chainBottom.transform.MoveY(customLevelTile4.transform.position.y);
		}
		else
		{
			chainTop.transform.MoveY(num);
			chainBottom.transform.MoveY(num);
		}
		gemTop.gameObject.SetActive(flag3);
		gemBottom.gameObject.SetActive(flag3);
		chainTop.gameObject.SetActive(!flag3 && !flag2);
		chainBottom.gameObject.SetActive(!flag3);
		gemExitFolder.gameObject.SetActive(flag2);
		if (list.Count != 0 && alsoSelect)
		{
			SelectLevel(list[0], snap: true);
		}
		else
		{
			DisplayLevel();
			StopCurrentLevelSong();
		}
		string text = RDString.Get("cls.shortcut.find");
		if (!sub.IsNullOrEmpty())
		{
			text = text + " <color=#ffd000><i>" + RDString.Get("cls.currentlySearching", new Dictionary<string, object>
			{
				{
					"filter",
					sub
				}
			}) + "</i></color>";
		}
		currentSearchText.text = text;
	}

	public void LoadTileIconsNearby(int targetIndex, int nearbyCount = 10)
	{
		targetIndex = Mathf.Clamp(targetIndex, 0, sortedLevelKeys.Count - 1);
		int num = Math.Max(0, targetIndex - nearbyCount);
		int num2 = Math.Min(sortedLevelKeys.Count - 1, targetIndex + nearbyCount);
		for (int i = num; i <= num2; i++)
		{
			string text = sortedLevelKeys[i];
			GenericDataCLS genericDataCLS = loadedLevels[text];
			CustomLevelTile customLevelTile = loadedLevelTiles[text];
			if (customLevelTile.gameObject.activeSelf && !string.IsNullOrEmpty(genericDataCLS.previewIcon) && !customLevelTile.didStartLoadingIcon && !customLevelTile.didProcessIcon)
			{
				if (featuredLevelsMode)
				{
					string path = genericDataCLS.isFolder ? text.Replace("Folder:", "") : text;
					Texture2D icon = Resources.Load<Texture2D>(Path.Combine("FeaturedLevels", path, "icon"));
					customLevelTile.ProcessIconTexture(icon, genericDataCLS.previewIconColor);
				}
				else
				{
					string iconPath = Path.Combine(loadedLevelDirs[text], genericDataCLS.previewIcon);
					customLevelTile.LoadTileIcon(iconPath, genericDataCLS.previewIconColor);
				}
			}
		}
	}

	private void HideInitialMenu()
	{
		DisableCLS(disable: false);
		initialPath.SetActive(value: false);
		featuredPortal.SetActive(value: false);
		workshopPortal.SetActive(value: false);
		camera.positionState = PositionState.CLS;
		ADOBase.controller.chosenplanet.other.transform.LocalMoveXY(0f, 0f);
		initialQuitLabel.enabled = false;
		showingInitialMenu = false;
		optionsPanels.UpdateOrderText();
		optionsPanels.currentOrderText.SetLocalizedFont();
	}

	[AsyncStateMachine(typeof(_003CFeaturedLevelsPortal_003Ed__142))]
	public void FeaturedLevelsPortal()
	{
		_003CFeaturedLevelsPortal_003Ed__142 stateMachine = default(_003CFeaturedLevelsPortal_003Ed__142);
		stateMachine._003C_003Et__builder = AsyncVoidMethodBuilder.Create();
		stateMachine._003C_003E4__this = this;
		stateMachine._003C_003E1__state = -1;
		stateMachine._003C_003Et__builder.Start(ref stateMachine);
	}

	[AsyncStateMachine(typeof(_003CWorkshopLevelsPortal_003Ed__143))]
	public void WorkshopLevelsPortal()
	{
		_003CWorkshopLevelsPortal_003Ed__143 stateMachine = default(_003CWorkshopLevelsPortal_003Ed__143);
		stateMachine._003C_003Et__builder = AsyncVoidMethodBuilder.Create();
		stateMachine._003C_003E4__this = this;
		stateMachine._003C_003E1__state = -1;
		stateMachine._003C_003Et__builder.Start(ref stateMachine);
	}

	public void QuitPortal()
	{
		GCS.customLevelPaths = null;
		ADOBase.controller.QuitToMainMenu();
	}

	[CompilerGenerated]
	private static LevelDataCLS _003CAwake_003Eg__DecodeLevel_007C108_0(uint id)
	{
		Dictionary<string, object> rootDict = Json.Deserialize(Resources.Load<TextAsset>(Path.Combine("FeaturedLevels", id.ToString(), "main")).text) as Dictionary<string, object>;
		LevelDataCLS levelDataCLS = new LevelDataCLS();
		levelDataCLS.Setup();
		levelDataCLS.Decode(rootDict);
		return levelDataCLS;
	}

	[CompilerGenerated]
	private static string _003CDisplayLevel_003Eg__GoldAccuracy_007C129_3(string accText, ref _003C_003Ec__DisplayClass129_1 P_1)
	{
		if (!P_1.purePerfected)
		{
			return accText;
		}
		return "<color=#FFDA00>" + accText + "</color>";
	}

	[CompilerGenerated]
	private static void _003CDisplayLevel_003Eg__DoWarningAnimation_007C129_2(RectTransform rt, Tween tween, bool left)
	{
		tween?.Kill();
		int num = (!left) ? 1 : (-1);
		tween = DOTween.Sequence().Append(rt.DOScale(Vector3.zero, 0f)).Join(rt.DORotate(new Vector3(0f, 0f, 30 * num), 0f))
			.Join(rt.DOScale(new Vector3(1f, 1f, 1f), 0.333333343f).SetEase(Ease.OutBack))
			.Join(rt.DORotate(new Vector3(0f, 0f, -30 * num), 0.5f).SetEase(Ease.OutBack));
	}
}
