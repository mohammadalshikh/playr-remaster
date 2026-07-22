using System;
using System.Collections;
using UnityEngine;

public class TrophyView : MonoBehaviour
{
	private const string TWEEN_KEY = "_glow";

	[SerializeField]
	private MEDAL _trophyType;

	[SerializeField]
	private PackedSprite _trophySprite;

	[SerializeField]
	private SFX _audioCeleb;

	[SerializeField]
	private float _glowTime = 1.2f;

	[SerializeField]
	private float _delay;

	private Color _cachedColor;

	private bool _audioAllowed;

	private bool _awarded;

	public PackedSprite TrophySprite => _trophySprite;

	private void OnDisable()
	{
		if (EndScreenManager.Instance != null)
		{
			EndScreenManager.Instance.TrophyAwarded -= Earn;
		}
	}

	private void Start()
	{
		if (EndScreenManager.Instance != null)
		{
			EndScreenManager.Instance.TrophyAwarded += Earn;
			UpdateState();
		}
		_cachedColor = _trophySprite.Color;
	}

	public void Earn()
	{
		_audioAllowed = true;
		UpdateState();
	}

	private void Glow()
	{
		iTween.ValueTo(base.gameObject, iTween.Hash("name", base.gameObject.name + "_glow", "from", 0.5f, "to", 1f, "time", 0.2f, "looptype", iTween.LoopType.pingPong, "onUpdate", (Action<object>)delegate(object x)
		{
			_trophySprite.SetColor(new Color(_cachedColor.r, _cachedColor.g, _cachedColor.b, (float)x));
		}));
		StartCoroutine(StopBlinkingTimed());
	}

	private void Stop()
	{
		iTween.StopByName(base.gameObject, base.gameObject.name + "_glow");
		_trophySprite.Color = _cachedColor;
	}

	private IEnumerator StopBlinkingTimed()
	{
		yield return StartCoroutine(GameData.WaitSeconds(_glowTime));
		Stop();
	}

	private bool UpdateState()
	{
		return UpdateState(GameData.PlayerStats.CurrentGameInfo());
	}

	public bool UpdateState(GameInfo gameInfo)
	{
		if (gameInfo == null || _awarded)
		{
			return false;
		}
		if (gameInfo.CurrentMedal < _trophyType)
		{
			_trophySprite.SetFrame(0, 0);
			return false;
		}
		Invoke("Award", _delay);
		return true;
	}

	private void Award()
	{
		_awarded = true;
		_trophySprite.SetFrame(0, (int)_trophyType);
		if (_audioCeleb.audioClip != null && _audioAllowed)
		{
			_audioAllowed = false;
			AudioManager.PlaySFX(_audioCeleb, 0f);
			Glow();
		}
	}
}
