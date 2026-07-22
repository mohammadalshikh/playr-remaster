using System;
using UnityEngine;

public class GlowText : MonoBehaviour
{
	[SerializeField]
	private SpriteText _spriteText;

	[SerializeField]
	private float _time = 0.2f;

	[SerializeField]
	private iTween.LoopType _loopType;

	private Color _cachedColor;

	private void OnEnable()
	{
		Glow();
	}

	private void Start()
	{
		_cachedColor = _spriteText.Color;
	}

	private void OnDisable()
	{
		iTween.StopByName(base.gameObject, base.gameObject.name + "_glow_text");
	}

	private void Glow()
	{
		iTween.ValueTo(base.gameObject, iTween.Hash("name", base.gameObject.name + "_glow_text", "from", 0.5f, "to", 1f, "time", _time, "looptype", _loopType, "onUpdate", (Action<object>)delegate(object x)
		{
			_spriteText.SetColor(new Color(_cachedColor.r, _cachedColor.g, _cachedColor.b, (float)x));
		}));
	}
}
