using System;
using UnityEngine;

public class TakeScreenshot : ADOBase
{
	private const int bufferWidth = 352;

	private const int bufferHeight = 198;

	[Range(0f, 10f)]
	public int iterations = 2;

	[Range(0f, 1f)]
	public float blurSpread = 0.6f;

	public RenderTexture blurredTexture;

	public Material mat;

	private bool goToSettings = true;

	[NonSerialized]
	public int getScreenshot;

	private void Awake()
	{
		if (GCS.lofiVersion && !ADOBase.isMobile)
		{
			base.enabled = false;
		}
		else
		{
			mat = new Material(RDConstants.data.blurEffectConeTap);
		}
	}

	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		scrController instance = scrController.instance;
		if (getScreenshot > 0)
		{
			getScreenshot--;
		}
		if (getScreenshot == 1)
		{
			if (blurredTexture == null)
			{
				blurredTexture = new RenderTexture(352, 198, source.depth);
			}
			mat.mainTexture = source;
			int width = blurredTexture.width / 4;
			int height = blurredTexture.height / 4;
			RenderTexture temporary = RenderTexture.GetTemporary(width, height, 0);
			DownSample4x(source, temporary);
			RenderTexture temporary2 = RenderTexture.GetTemporary(width, height, 0);
			for (int i = 0; i < iterations; i++)
			{
				FourTapCone(temporary, temporary2, i);
				temporary.DiscardContents();
				Graphics.Blit(temporary2, temporary);
				temporary2.DiscardContents();
			}
			RenderTexture.ReleaseTemporary(temporary2);
			Graphics.Blit(temporary, blurredTexture);
			RenderTexture.ReleaseTemporary(temporary);
			PauseMenu pauseMenu = instance.pauseMenu;
			pauseMenu.background.texture = blurredTexture;
			if (pauseMenu.lastInputSelected == -1)
			{
				pauseMenu.Show();
			}
			else
			{
				pauseMenu.Show(instance.pauseMenu.lastInputSelected);
			}
			if (goToSettings)
			{
				pauseMenu.ShowSettingsMenu();
			}
		}
		if (getScreenshot == 0)
		{
			base.enabled = false;
		}
		Graphics.Blit(source, destination);
	}

	public void ShowPauseMenu(bool goToSettings)
	{
		getScreenshot = 2;
		this.goToSettings = goToSettings;
		base.enabled = true;
	}

	public void FourTapCone(RenderTexture source, RenderTexture dest, int iteration)
	{
		float num = 0.5f + (float)iteration * blurSpread;
		Graphics.BlitMultiTap(source, dest, mat, new Vector2(0f - num, 0f - num), new Vector2(0f - num, num), new Vector2(num, num), new Vector2(num, 0f - num));
	}

	private void DownSample4x(RenderTexture source, RenderTexture dest)
	{
		float num = 1f;
		Graphics.BlitMultiTap(source, dest, mat, new Vector2(0f - num, 0f - num), new Vector2(0f - num, num), new Vector2(num, num), new Vector2(num, 0f - num));
	}
}
