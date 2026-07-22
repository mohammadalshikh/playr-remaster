using System;
using UnityEngine;

public class BackgroundMask : ScreenQuad
{
	private const float defaultTransitionTime = 0.75f;

	public float DefaultTransitionTime => 0.75f;

	public void MaskBackground()
	{
		MaskBackground(0.75f);
	}

	public void MaskBackground(float time)
	{
		SetMaskingAmount(time, 0f, 1f);
	}

	public void UnmaskBackground()
	{
		UnmaskBackground(0.75f);
	}

	public void UnmaskBackground(float time)
	{
		SetMaskingAmount(time, 1f, 0f);
	}

	public void SetMaskingAmount(float time, float amountFrom, float amountTo)
	{
		iTween.ValueTo(base.transform.gameObject, iTween.Hash("from", amountFrom, "to", amountTo, "time", time, "onUpdate", (Action<object>)delegate(object x)
		{
			base.QuadColor = new Color(base.QuadColor.r, base.QuadColor.g, base.QuadColor.b, (float)x);
		}));
	}
}
