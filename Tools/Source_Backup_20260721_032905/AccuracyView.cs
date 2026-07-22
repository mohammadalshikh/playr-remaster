using UnityEngine;

public class AccuracyView : MonoSingleton<AccuracyView>
{
	[SerializeField]
	private UIPanel _mainPanel;

	[SerializeField]
	private SpriteText _targetAccuracy;

	[SerializeField]
	private SpriteText _currentAccuracy;

	private bool _isActive;

	public float TargetAccuracy
	{
		set
		{
			int num = Mathf.RoundToInt(Mathf.Clamp01(value) * 100f);
			_targetAccuracy.Text = string.Empty + num + "%";
		}
	}

	private float Accuracy
	{
		set
		{
			int num = Mathf.RoundToInt(Mathf.Clamp01(value) * 100f);
			_currentAccuracy.Text = string.Empty + num + "%";
		}
	}

	private void Awake()
	{
		MonoSingleton<AccuracyView>.Instance = this;
	}

	public void Show()
	{
		if (!_isActive)
		{
			_isActive = true;
			_mainPanel.BringIn();
		}
	}

	public void Hide()
	{
		if (_isActive)
		{
			_isActive = false;
			_mainPanel.Dismiss();
		}
	}

	public void Reset()
	{
		TargetAccuracy = 0f;
		Accuracy = 0f;
	}

	public void CurrentAccuracy(float newAccuracy, bool bFailing)
	{
		if (!(_currentAccuracy == null))
		{
			Accuracy = newAccuracy;
			IsFailing(bFailing);
		}
	}

	public void IsFailing(bool failing)
	{
		_currentAccuracy.SetColor((!failing) ? Color.yellow : Color.red);
	}
}
