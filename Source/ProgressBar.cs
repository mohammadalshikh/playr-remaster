using System;
using UnityEngine;

public class ProgressBar : MonoBehaviour
{
	[SerializeField]
	private PackedSprite _barSprite;

	[SerializeField]
	private bool _progressColor;

	[SerializeField]
	protected float _progressAnimationTime = 0.3f;

	[SerializeField]
	protected float _glowTime = 0.5f;

	protected Color _cachedColor;

	protected float _progress;

	private bool _isGlowing;

	private void Awake()
	{
		_cachedColor = _barSprite.Color;
	}

	public virtual void SetProgress(float current, bool animated = true)
	{
		if (animated)
		{
			iTween.ValueTo(base.gameObject, iTween.Hash("name", base.gameObject.name, "from", _progress, "to", current, "time", _progressAnimationTime, "oncomplete", "OnCompleteAnimation", "ignoretimescale", true, "onUpdate", (Action<object>)delegate(object x)
			{
				SetValue((float)x);
			}));
		}
		else
		{
			SetValue(current);
		}
		_progress = current;
	}

	protected virtual void OnCompleteAnimation()
	{
	}

	private void SetValue(float current)
	{
		_barSprite.TruncateRight(current);
		if (_progressColor)
		{
			_barSprite.Color = new Color(1f, 1f * current, 0f, 1f);
		}
		_progress = current;
	}

	public virtual void GlowBar(bool bFlash)
	{
		if (bFlash)
		{
			Glow();
		}
		else
		{
			StopGlowing();
		}
	}

	protected void Glow()
	{
		if (!_isGlowing)
		{
			_isGlowing = true;
			iTween.ValueTo(base.gameObject, iTween.Hash("name", base.gameObject.name + "_flash", "from", 0.5f, "to", 1f, "time", _glowTime, "looptype", iTween.LoopType.pingPong, "ignoretimescale", true, "onUpdate", (Action<object>)delegate(object x)
			{
				_barSprite.SetColor(new Color(_cachedColor.r, _cachedColor.g, _cachedColor.b, (float)x));
			}));
		}
	}

	protected void StopGlowing()
	{
		if (_isGlowing)
		{
			_isGlowing = false;
			iTween.StopByName(base.gameObject, base.gameObject.name + "_flash");
		}
	}

	protected void SetBarAnimationFrame(int anim, int frame)
	{
		_barSprite.SetFrame(anim, frame);
	}
}
