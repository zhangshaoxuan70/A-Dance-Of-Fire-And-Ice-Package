using Rewired.Platforms;
using Rewired.Utils;
using Rewired.Utils.Interfaces;
using System.ComponentModel;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Rewired
{
	[AddComponentMenu("Rewired/Input Manager")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public sealed class InputManager : InputManager_Base
	{
		private bool ignoreRecompile;

		protected override void OnInitialized()
		{
			SubscribeEvents();
		}

		protected override void OnDeinitialized()
		{
			UnsubscribeEvents();
		}

		protected override void DetectPlatform()
		{
			scriptingBackend = ScriptingBackend.Mono;
			scriptingAPILevel = ScriptingAPILevel.Net20;
			editorPlatform = EditorPlatform.None;
			platform = Rewired.Platforms.Platform.Unknown;
			webplayerPlatform = WebplayerPlatform.None;
			isEditor = false;
			if (SystemInfo.deviceName == null)
			{
				string empty = string.Empty;
			}
			if (SystemInfo.deviceModel == null)
			{
				string empty2 = string.Empty;
			}
			platform = Rewired.Platforms.Platform.Windows;
			scriptingBackend = ScriptingBackend.Mono;
			scriptingAPILevel = ScriptingAPILevel.Net46;
		}

		protected override void CheckRecompile()
		{
		}

		protected override IExternalTools GetExternalTools()
		{
			return new ExternalTools();
		}

		private bool CheckDeviceName(string searchPattern, string deviceName, string deviceModel)
		{
			if (!Regex.IsMatch(deviceName, searchPattern, RegexOptions.IgnoreCase))
			{
				return Regex.IsMatch(deviceModel, searchPattern, RegexOptions.IgnoreCase);
			}
			return true;
		}

		private void SubscribeEvents()
		{
			UnsubscribeEvents();
			SceneManager.sceneLoaded += OnSceneLoaded;
		}

		private void UnsubscribeEvents()
		{
			SceneManager.sceneLoaded -= OnSceneLoaded;
		}

		private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
		{
			OnSceneLoaded();
		}
	}
}
