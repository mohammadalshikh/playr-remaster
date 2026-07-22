public class BlastPhaseBar : ProgressBar
{
	private bool _increasing;

	private void OnEnable()
	{
		Minigame.MinigameWillStart += Reset;
		MinigameChallenge.BlastPhaseChanged += UpdateBar;
		MinigameGUI.MakeGUIEnabled += Show;
	}

	private void OnDisable()
	{
		Minigame.MinigameWillStart -= Reset;
		MinigameChallenge.BlastPhaseChanged -= UpdateBar;
		MinigameGUI.MakeGUIEnabled -= Show;
	}

	private void UpdateBar(float current, int max)
	{
		if (_increasing && current <= _progress)
		{
			return;
		}
		GlowBar(current == (float)max);
		float num = current / (float)max;
		if (current == _progress)
		{
			SetProgress(current, animated: false);
			OnCompleteAnimation();
			return;
		}
		if (num >= _progress)
		{
			_increasing = true;
		}
		SetProgress(num);
	}

	protected override void OnCompleteAnimation()
	{
		_increasing = false;
	}

	private void Show(bool bShow)
	{
		base.GetComponent<UnityEngine.Renderer>().enabled = bShow;
	}

	public override void GlowBar(bool bFlash)
	{
		SetPulseBar(bFlash);
		if (bFlash)
		{
			Glow();
		}
		else
		{
			StopGlowing();
		}
	}

	private void SetPulseBar(bool bPulseBar)
	{
		SetBarAnimationFrame(0, bPulseBar ? 1 : 0);
	}

	private void Reset()
	{
		SetProgress(0f, animated: false);
		_increasing = false;
	}
}
