using System;
using UnityEngine;
using UnityEngine.UI;

namespace MobileMenu
{
	public class MobileMenuReviewPrompt : ADOBase
	{
		public MobileMenuController menuController;

		public Button buttonSubmit;

		public Button buttonCancel;

		public Transform background;

		public Camera camera;

		public Canvas canvas;

		private void Awake()
		{
			buttonSubmit.onClick.AddListener(delegate
			{
				GoToReviewPage();
			});
			buttonCancel.onClick.AddListener(delegate
			{
				ShowRatingPrompt(show: false);
			});
		}

		public bool TryRunReviewPrompt()
		{
			if (!Persistence.HasRatedGame() && Persistence.GetPercentCompletion(0) >= 1f)
			{
				int nextRatingPromptDay = Persistence.GetNextRatingPromptDay();
				DateTime now = DateTime.Now;
				DateTime d = new DateTime(2020, 1, 1, 0, 0, 0);
				int days = (now - d).Days;
				if (nextRatingPromptDay < 0 || nextRatingPromptDay <= days)
				{
					Persistence.SetNextRatingPromptDay(days + 7);
					if (ShowRatingPrompt())
					{
						return true;
					}
				}
			}
			return false;
		}

		private void LateUpdate()
		{
			int num = Mathf.CeilToInt((float)Screen.width * 1f / (float)Screen.height);
			float orthographicSize = camera.orthographicSize;
			float x = orthographicSize * 2f * (float)num;
			float y = orthographicSize * 2f;
			background.ScaleXY(x, y);
			canvas.GetComponent<RectTransform>().sizeDelta = new Vector2(x, y);
		}

		public bool ShowRatingPrompt(bool show = true)
		{
			base.gameObject.SetActive(show);
			menuController.Enable(!show);
			if (!show)
			{
				menuController.JumpToMenuEntrance();
			}
			return true;
		}

		private void GoToReviewPage()
		{
			ShowRatingPrompt(show: false);
			Persistence.SetRatedGame(rated: true);
			Application.OpenURL("https://play.google.com/store/apps/details?id=com.fizzd.connectedworlds");
		}
	}
}
