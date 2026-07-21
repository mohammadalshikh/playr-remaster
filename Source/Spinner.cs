using UnityEngine;

public class Spinner : MonoBehaviour
{
	private SpriteText _target;

	private int _targetValue;

	private int _currentValue;

	private int _initialValue;

	private int _speed;

	private bool _flag;

	private int _nextScore;

	private float _countTime;

	private int _timeSpinning;

	private float _speedRate;

	private bool _hasStopped;

	private SFX _audioEffect;

	private bool _soundEnabled;

	public SpriteText Target
	{
		get
		{
			return _target;
		}
		set
		{
			_target = value;
		}
	}

	public int TargetValue
	{
		get
		{
			return _targetValue;
		}
		set
		{
			_targetValue = value;
		}
	}

	public int CurrentValue => _currentValue;

	public int InitialValue
	{
		get
		{
			return _initialValue;
		}
		set
		{
			_initialValue = value;
		}
	}

	public float Speed
	{
		set
		{
			_speedRate = value / 100f;
		}
	}

	public bool HasStopped => _hasStopped;

	public bool SoundEnabled
	{
		set
		{
			_soundEnabled = value;
		}
	}

	private void Start()
	{
		if (_target == null)
		{
			_target = base.transform.GetComponent<SpriteText>();
		}
		_countTime = 0.05f;
		_speed = (int)Mathf.Ceil(1f / 30f / _countTime);
		_target.Text = "0";
	}

	public void StartRunning(SFX audioEffect)
	{
		_hasStopped = false;
		_flag = true;
		_currentValue = _initialValue;
		if (_soundEnabled)
		{
			_audioEffect = audioEffect;
			if (_targetValue > 0 && _soundEnabled)
			{
				AudioManager.PlaySFX(_audioEffect, 0f);
			}
		}
	}

	private int ScoreCounter()
	{
		if (_currentValue < _targetValue)
		{
			_currentValue += (int)Mathf.Ceil((float)(_targetValue - _initialValue) * _speedRate) * _speed;
			_target.Text = _currentValue.ToString("#,##0");
		}
		else
		{
			_currentValue = _targetValue;
			_target.Text = _currentValue.ToString("#,##0");
			_flag = false;
			_hasStopped = true;
			if (_soundEnabled)
			{
				AudioManager.StopSound(_audioEffect);
			}
		}
		return _currentValue;
	}

	private void Update()
	{
		if (_flag)
		{
			ScoreCounter();
		}
	}
}
