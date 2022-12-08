using UnityEngine;
using UnityEngine.UI;

namespace MobileMenu
{
	public class MobileMenuDebug : ADOBase
	{
		public MobileMenuController menuController;

		public GameObject debugPanel;

		public InputField resolutionInputField;

		public Text resolutionLabel;

		private int tapCount;

		private MobileMenuScreen screenForCombo;

		private void Awake()
		{
			resolutionInputField.text = Screen.width.ToString() + "x" + Screen.height.ToString();
			debugPanel.SetActive(value: false);
		}

		private void Update()
		{
			CheckDebugCombo();
		}

		private void CheckDebugCombo()
		{
			Touch[] touches = Input.touches;
			for (int i = 0; i < touches.Length; i++)
			{
				Touch touch = touches[i];
				if (touch.phase == TouchPhase.Began && UnityEngine.Input.touchCount == 4)
				{
					tapCount++;
				}
			}
			if (UnityEngine.Input.touchCount < 3 && tapCount == 7)
			{
				debugPanel.SetActive(value: true);
			}
			if (UnityEngine.Input.touchCount < 3 || UnityEngine.Input.touchCount > 4)
			{
				tapCount = 0;
			}
		}

		public void CompleteFirst5()
		{
			Persistence.CompleteFirst5();
			ADOBase.GoToLevelSelect();
		}

		public void CompleteMain()
		{
			Persistence.CompleteAllMainLevels();
			ADOBase.GoToLevelSelect();
		}

		public void UnlockBonus()
		{
			Persistence.CompleteAllMainLevelsAndSpeedTrials();
			ADOBase.GoToLevelSelect();
		}

		public void BeatSelectedWorld()
		{
			string world = (menuController.currentScreen as MobileMenuPortal).world;
			Persistence.CompleteWorld(GCNS.worldData[world].index);
			Persistence.SetPassedMobileTutorial(passed: true);
			Persistence.Save();
			MobileMenuMap map = menuController.map;
			map.EvaluateAllConditions();
			map.Build();
			foreach (MobileMenuPortal value in map.portalLUT.Values)
			{
				if (value.visible)
				{
					value.CheckLocked(speedTrial: false);
				}
			}
		}

		public void BeatFirstWorld()
		{
			Persistence.CompleteFirst();
			ADOBase.GoToLevelSelect();
		}

		public void Complete100Percent()
		{
			Persistence.Complete100();
			ADOBase.GoToLevelSelect();
		}

		public void StartBenchmark()
		{
			GCS.turnOnBenchmarkMode = true;
		}

		public void SetResolution()
		{
			string[] array = resolutionInputField.text.Split('x');
			if (array.Length == 2)
			{
				int result = 0;
				int result2 = 0;
				bool flag = int.TryParse(array[0], out result);
				bool flag2 = int.TryParse(array[1], out result2);
				if (flag && flag2 && result > 0 && result2 > 0)
				{
					printe("w: " + result.ToString() + " h: " + result2.ToString());
					Screen.SetResolution(result, result2, fullscreen: true);
					resolutionLabel.color = Color.black;
				}
				else
				{
					resolutionLabel.color = Color.red;
				}
			}
			else
			{
				resolutionLabel.color = Color.red;
			}
		}

		public void ClearAchievements()
		{
			ADOBase.controller.ClearAllAchievements();
		}

		public void TurnOffDebugMode()
		{
			RDC.debug = false;
		}
	}
}
