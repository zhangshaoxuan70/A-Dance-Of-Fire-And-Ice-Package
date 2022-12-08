using DG.Tweening;
using UnityEngine;

public class PortalQuad : ADOBase
{
	private MeshRenderer meshRenderer;

	public bool keepRenderQueue;

	private Material material => meshRenderer.material;

	private void Awake()
	{
		meshRenderer = GetComponent<MeshRenderer>();
		if (!keepRenderQueue)
		{
			material.renderQueue = 2001;
		}
	}

	public void SetTexture(Texture2D texture)
	{
		if (texture == null)
		{
			UnityEngine.Debug.LogError("texture is null");
			return;
		}
		material.mainTexture = texture;
		Vector3 localScale = base.transform.localScale;
		float value = (float)texture.width * 1f / (float)texture.height / (localScale.x / localScale.y);
		material.SetFloat("_Ratio", value);
	}

	public void Fade(float alpha, float duration)
	{
		material.DOColor(Color.white.WithAlpha(alpha), "_Color", duration).SetUpdate(isIndependentUpdate: true);
	}
}
