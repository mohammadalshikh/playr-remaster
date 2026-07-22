using System;
using UnityEngine;

public class GlowSprite : MonoBehaviour
{
	[SerializeField]
	private AutoSpriteControlBase _sprite;

	[SerializeField]
	private float _time = 0.2f;

	[SerializeField]
	private float _fromOpacity = 0.5f;

	[SerializeField]
	private iTween.LoopType _loopType;

	private Color _cachedColor;

	private void OnEnable()
	{
		Glow();
	}

	private void Start()
	{
		_cachedColor = _sprite.Color;
	}

	private void OnDisable()
	{
		iTween.StopByName(base.gameObject, base.gameObject.name + "_glow_text");
		_sprite.SetColor(_cachedColor);
	}

	private void Glow()
	{
		if (!(_sprite == null))
		{
			iTween.ValueTo(base.gameObject, iTween.Hash("name", base.gameObject.name + "_glow_text", "from", _fromOpacity, "to", 1f, "time", _time, "looptype", _loopType, "onUpdate", (Action<object>)delegate(object x)
			{
				_sprite.SetColor(new Color(_cachedColor.r, _cachedColor.g, _cachedColor.b, (float)x));
			}));
		}
	}
}
