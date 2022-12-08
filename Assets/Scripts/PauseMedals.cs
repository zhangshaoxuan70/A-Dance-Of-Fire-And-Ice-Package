using UnityEngine;

public class PauseMedals : ADOBase
{
	public GameObject medalPrefab;

	private RectTransform rt;

	private int[] displayedMedals;

	private float medalScale = 16f;

	private int curSection;

	public bool transition;

	private bool init;

	private float clickTimer;

	private int lastClicked;

	private void Update()
	{
		if (init && clickTimer > 0f)
		{
			clickTimer -= Time.deltaTime;
		}
		if (init)
		{
			Scale();
		}
	}

	public void OnClick(int id)
	{
		if (!transition)
		{
			if (clickTimer > 0f && id == lastClicked)
			{
				WarpToSection(lastClicked);
				transition = true;
			}
			else
			{
				lastClicked = id;
				clickTimer = 0.4f;
			}
		}
	}

	public void WarpToSection(int id)
	{
		int num = GCS.pauseMedalFloors[id] - 1;
		if (num < 0)
		{
			num = 0;
		}
		while ((scrLevelMaker.instance.listFloors[num].midSpin || scrLevelMaker.instance.listFloors[num].freeroam) && num > 0)
		{
			num--;
		}
		GCS.checkpointNum = num;
		Persistence.DeleteSavedProgress();
		scrController.instance.Restart();
	}

	public void Init()
	{
		bool flag = ADOBase.controller.gameworld || (!ADOBase.controller.gameworld && (ADOBase.controller.currFloor.freeroam || ADOBase.controller.currFloor.freeroamGenerated));
		if (!ADOBase.sceneName.IsTaro() || ADOBase.controller.isPuzzleRoom || !ADOBase.controller.isbosslevel || !flag || GCS.speedTrialMode || GCS.practiceMode || !Persistence.IsWorldComplete(scrController.currentWorldString))
		{
			base.gameObject.SetActive(value: false);
			return;
		}
		base.gameObject.SetActive(value: true);
		rt = GetComponent<RectTransform>();
		if (TaroBGScript.instance != null && GCS.pauseMedalStatsCurrent != null)
		{
			printe($"current world {scrController.currentWorldString}. Complete? {Persistence.IsWorldComplete(scrController.currentWorldString)}");
			displayedMedals = TaroBGScript.instance.SaveMedals(scrController.currentWorldString, GCS.pauseMedalStatsCurrent);
			if ((bool)ADOBase.controller.currFloor)
			{
				for (int i = 0; i < GCS.pauseMedalFloors.Count && ADOBase.controller.currFloor.seqID >= GCS.pauseMedalFloors[i] - 1; i++)
				{
					curSection = i;
				}
			}
			init = true;
			Scale();
			for (int j = 0; j < displayedMedals.Length; j++)
			{
				RectTransform component = Object.Instantiate(medalPrefab, rt).GetComponent<RectTransform>();
				component.anchoredPosition = new Vector3(((float)j - (float)displayedMedals.Length / 2f + 0.5f) * medalScale, (j % 2 == 0) ? (medalScale * 0.5f) : ((0f - medalScale) * 0.5f), 0f);
				MenuMedal component2 = component.gameObject.GetComponent<MenuMedal>();
				component2.id = j;
				component2.SetState(displayedMedals[j]);
				if (curSection == j)
				{
					component2.TintBack(Color.white);
				}
				else
				{
					component2.TintBack(Color.black);
				}
			}
		}
		else
		{
			base.gameObject.SetActive(value: false);
		}
	}

	private void Scale()
	{
		float num = (float)Screen.width / (float)Screen.height;
		if (num < 1.77f)
		{
			rt.localScale = Vector3.one * Mathf.Sqrt(num / 1.77f);
		}
	}
}
