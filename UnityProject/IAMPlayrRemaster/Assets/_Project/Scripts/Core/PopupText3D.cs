using System;
using UnityEngine;

public class PopupText3D : MonoBehaviour
{
	[SerializeField]
	private SpriteText _text;

	[SerializeField]
	private bool _triggerAtStart;

	[SerializeField]
	private float _moveAnimTime = 0.5f;

	[SerializeField]
	private float _rotateAnimTime = 0.7f;

	[SerializeField]
	private float _rotateFromAngle;

	[SerializeField]
	private float _dismissDelay;

	[SerializeField]
	private Vector3 _moveTo = Vector3.zero;

	private Color _cachedColor;

	private void Start()
	{
		_cachedColor = _text.Color;
		if (_triggerAtStart)
		{
			Show(_text.Text, _cachedColor);
		}
	}

	public void Setup(string message, float moveToX, float moveToY, float moveToZ, float time = 0.5f)
	{
		_text.Text = message;
		_moveAnimTime = time;
		_moveTo = new Vector3(moveToX, moveToY, moveToZ);
	}

	public void Show(string message, Color textColor)
	{
		if (message != string.Empty)
		{
			_text.Text = message;
		}
		_text.Color = textColor;
		base.transform.localPosition = Vector3.zero;
		base.GetComponent<Renderer>().enabled = true;
		AnimateMoveTo();
	}

	private void AnimateMoveTo()
	{
		iTween.MoveTo(base.gameObject, iTween.Hash("position", _moveTo, "time", _moveAnimTime, "easetype", iTween.EaseType.linear, "islocal", true, "oncomplete", "Fade"));
		base.transform.localRotation = Quaternion.identity;
		if (_rotateFromAngle != 0f)
		{
			iTween.RotateFrom(base.gameObject, iTween.Hash("rotation", new Vector3(0f, 0f, _rotateFromAngle), "time", _rotateAnimTime, "islocal", true));
		}
	}

	private void Fade()
	{
		iTween.ValueTo(base.gameObject, iTween.Hash("from", 1f, "to", 0f, "time", _moveAnimTime / 2f, "oncomplete", "Dismiss", "delay", _dismissDelay, "onUpdate", (Action<object>)delegate(object x)
		{
			_text.SetColor(new Color(_cachedColor.r, _cachedColor.g, _cachedColor.b, (float)x));
		}));
	}

	private void Dismiss()
	{
		if ((bool)base.gameObject)
		{
			iTween.Stop(base.gameObject);
		}
		EnableRenderer(bEnable: false);
		base.transform.localPosition = Vector3.zero;
		_text.Color = _cachedColor;
	}

	private void EnableRenderer(bool bEnable)
	{
		base.GetComponent<Renderer>().enabled = bEnable;
	}
}
