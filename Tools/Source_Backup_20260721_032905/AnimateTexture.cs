using System;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class AnimateTexture : MonoBehaviour
{
	[SerializeField]
	private float _speedX = 0.1f;

	[SerializeField]
	private float _speedY = 0.1f;

	[SerializeField]
	private float fSineTransformFactor;

	[SerializeField]
	private float fSineTransformVelocity = 1f;

	[SerializeField]
	private float fSineTransformPhase;

	private Renderer _cachedRenderer;

	private void Start()
	{
		_cachedRenderer = base.transform.renderer;
	}

	private void Update()
	{
		float x = Time.time * _speedX + Mathf.Cos(Time.time * fSineTransformVelocity + fSineTransformPhase) / (float)Math.PI * fSineTransformFactor;
		float y = Time.time * _speedY + Mathf.Sin(Time.time * fSineTransformVelocity + fSineTransformPhase) / (float)Math.PI * fSineTransformFactor;
		_cachedRenderer.material.SetTextureOffset("_MainTex", new Vector2(x, y));
	}
}
