using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MobileMenu
{
	public class PlanetColorSwapper : ADOBase
	{
		public GameObject colorCloudPrefab;

		public Transform emojiPaintTransform;

		public Transform colorsRedPlanet;

		public Transform colorsBluePlanet;

		private Transform[] colorClouds;

		private List<PlanetColor> availableColors;

		private bool changedEmojiMode;

		private int paintCombo;

		private void Awake()
		{
			UpdateColors();
		}

		public void UpdateColors()
		{
			availableColors = new List<PlanetColor>();
			if (Persistence.IsWorldComplete(0))
			{
				availableColors.Add(PlanetColor.DefaultRed, PlanetColor.DefaultBlue);
			}
			if (Persistence.IsWorldComplete(1))
			{
				availableColors.Add(PlanetColor.Orange, PlanetColor.LightBlue);
			}
			if (Persistence.IsWorldComplete(2))
			{
				availableColors.Add(PlanetColor.Pink, PlanetColor.Green);
			}
			if (Persistence.IsWorldComplete(3))
			{
				availableColors.Add(PlanetColor.Purple, PlanetColor.Grass);
			}
			if (Persistence.IsWorldComplete(4))
			{
				availableColors.Add(PlanetColor.TransBlue, PlanetColor.TransPink);
			}
			if (Persistence.IsWorldComplete(5))
			{
				availableColors.Add(PlanetColor.Black, PlanetColor.White);
			}
			if (Persistence.IsWorldComplete(6))
			{
				availableColors.Add(PlanetColor.Gold);
			}
			if (Persistence.IsWorldComplete(7))
			{
				availableColors.Add(PlanetColor.Aqua, PlanetColor.Violet);
			}
			if (Persistence.IsWorldComplete(8))
			{
				availableColors.Add(PlanetColor.Jungle, PlanetColor.Vine);
			}
			if (Persistence.IsWorldComplete(9))
			{
				availableColors.Add(PlanetColor.Crimson, PlanetColor.Maroon);
			}
			if (Persistence.IsWorldComplete(10))
			{
				availableColors.Add(PlanetColor.Cyan, PlanetColor.Teal);
			}
			if (Persistence.IsWorldComplete(11))
			{
				availableColors.Add(PlanetColor.Jester, PlanetColor.Stone);
			}
			if (Persistence.IsWorldComplete(12))
			{
				availableColors.Add(PlanetColor.Rust, PlanetColor.Metal);
			}
			if (Persistence.IsWorldComplete("T5"))
			{
				availableColors.Add(PlanetColor.Overseer);
			}
			emojiPaintTransform.gameObject.SetActive(Persistence.IsWorldComplete(ADOBase.worldData["MO"].index));
			if (colorClouds != null)
			{
				Transform[] array = colorClouds;
				for (int i = 0; i < array.Length; i++)
				{
					UnityEngine.Object.Destroy(array[i]);
				}
			}
			colorClouds = new Transform[availableColors.Count];
			float[] array2 = new float[availableColors.Count];
			float S = 0f;
			float V = 0f;
			for (int j = 0; j < availableColors.Count; j++)
			{
				Color.RGBToHSV(availableColors[j].GetColor(), out array2[j], out S, out V);
			}
			availableColors = (from planetColor in availableColors
				orderby RDUtils.GetHue(planetColor.GetColor())
				select planetColor).ToList();
			for (int k = 0; k < colorClouds.Length; k++)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(colorCloudPrefab);
				gameObject.transform.SetParent(base.transform, worldPositionStays: false);
				ParticleSystem.MainModule main = gameObject.GetComponent<ParticleSystem>().main;
				PlanetColor planetColor2 = availableColors[k];
				main.startColor = planetColor2.GetColor();
				ColorCloud component = gameObject.GetComponent<ColorCloud>();
				component.SetSortingOrder(k);
				if (planetColor2 == PlanetColor.Gold)
				{
					component.goldSparks.SetActive(value: true);
				}
				colorClouds[k] = gameObject.transform;
			}
			PositionClouds();
		}

		public void PositionClouds()
		{
			Vector2 vector = new Vector2(4f, 3f);
			for (int i = 0; i < colorClouds.Length; i++)
			{
				Vector2 vector2 = vector;
				float f = (float)i * 1f / (float)colorClouds.Length * 2f * MathF.PI;
				float num = (i % 2 == 0) ? 1f : 1f;
				vector2.x *= num;
				vector2.y *= num;
				Vector3 localPosition = new Vector3(Mathf.Cos(f) * vector2.x, Mathf.Sin(f) * vector2.y, 0f);
				colorClouds[i].transform.localPosition = localPosition;
			}
		}

		private void Update()
		{
			float t = 0.15f;
			float num = Time.timeSinceLevelLoad * 3f % (MathF.PI * 2f);
			float f = (num + MathF.PI) % (MathF.PI * 2f);
			float num2 = 1f;
			MobileMenuGrabbable grabbedObject = MobileMenuController.instance.grabController.grabbedObject;
			MobileMenuGrabbablePlanet mobileMenuGrabbablePlanet = null;
			if (grabbedObject is MobileMenuGrabbablePlanet)
			{
				mobileMenuGrabbablePlanet = (grabbedObject as MobileMenuGrabbablePlanet);
			}
			if (mobileMenuGrabbablePlanet == null || !mobileMenuGrabbablePlanet.planet.isRed)
			{
				Vector3 b = new Vector3(Mathf.Cos(num) * num2, Mathf.Sin(num) * num2, 0f);
				colorsRedPlanet.localPosition = Vector3.Lerp(colorsRedPlanet.localPosition, b, t);
			}
			if (mobileMenuGrabbablePlanet == null || mobileMenuGrabbablePlanet.planet.isRed)
			{
				Vector3 b2 = new Vector3(Mathf.Cos(f) * num2, Mathf.Sin(f) * num2, 0f);
				colorsBluePlanet.localPosition = Vector3.Lerp(colorsBluePlanet.localPosition, b2, t);
			}
			if ((bool)mobileMenuGrabbablePlanet)
			{
				scrPlanet planet = mobileMenuGrabbablePlanet.planet;
				if (emojiPaintTransform.gameObject.activeSelf && !changedEmojiMode && Vector2.Distance(emojiPaintTransform.localPosition, mobileMenuGrabbablePlanet.transform.localPosition) < 0.3f)
				{
					planet.SetFaceMode(enabled: true);
					Persistence.SetFaceMode(enabled: true, planet.isRed);
					changedEmojiMode = true;
					scrSfx.instance.PlaySfxPitch(SfxSound.ModifierActivate);
					return;
				}
				int num3 = 0;
				while (true)
				{
					if (num3 < colorClouds.Length)
					{
						if (Vector2.Distance(colorClouds[num3].transform.localPosition, mobileMenuGrabbablePlanet.transform.localPosition) < 0.3f)
						{
							break;
						}
						num3++;
						continue;
					}
					return;
				}
				PlanetColor planetColor = availableColors[num3];
				scrLogoText.instance.UpdateColors();
				if (planet.previousPaintedColor != planetColor)
				{
					if (!changedEmojiMode)
					{
						planet.SetFaceMode(enabled: false);
						Persistence.SetFaceMode(enabled: false, planet.isRed);
					}
					planet.SetColor(planetColor);
					float pitch = 1f * Mathf.Pow(1.05946314f, paintCombo);
					if (paintCombo < 20)
					{
						paintCombo++;
					}
					scrSfx.instance.PlaySfxPitch(SfxSound.PlanetPaint, 1f, pitch);
				}
			}
			else
			{
				changedEmojiMode = false;
				paintCombo = 0;
			}
		}
	}
}
