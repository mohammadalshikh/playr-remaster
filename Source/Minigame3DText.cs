using System;
using System.Collections;
using UnityEngine;

public class Minigame3DText : SpriteText
{
	protected Color _cachedColor;

	public bool IsEmpty => base.Text == string.Empty;

	public bool IsActive => base.renderer.enabled;

	protected void ActivateRenderer(bool bActive)
	{
		base.renderer.enabled = bActive;
	}

	public virtual void Show()
	{
		if (!IsEmpty)
		{
			ActivateRenderer(bActive: true);
		}
	}

	public virtual void Show(string message)
	{
		base.Text = message;
		Show();
	}

	public virtual void Dismiss()
	{
		Clear();
		if (IsActive)
		{
			ActivateRenderer(bActive: false);
		}
	}

	public virtual void Dismiss(float delay)
	{
		if (IsActive)
		{
			StartCoroutine(DismissWithDelay(delay));
		}
	}

	private IEnumerator DismissWithDelay(float delay)
	{
		yield return new WaitForSeconds(delay);
		Dismiss();
	}

	public virtual void Reset()
	{
		Clear();
	}

	public virtual void Glow()
	{
		StopGlow();
		_cachedColor = base.Color;
		iTween.ValueTo(base.gameObject, iTween.Hash("name", "_glow_text", "from", 0.6f, "to", 1f, "time", 0.2f, "looptype", iTween.LoopType.pingPong, "onUpdate", (Action<object>)delegate(object x)
		{
			SetColor(new Color(_cachedColor.r, _cachedColor.g, _cachedColor.b, (float)x));
		}));
	}

	public virtual void StopGlow()
	{
		iTween.Stop(base.gameObject, "_glow_text");
	}

	public virtual void Punch()
	{
		iTween.PunchScale(base.gameObject, Vector3.one * 0.3f, 1f);
	}

	public virtual void Clear()
	{
		base.Text = string.Empty;
	}
}
