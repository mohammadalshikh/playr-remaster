using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinnerSequence : MonoBehaviour
{
	private List<SpinnerSettings> spinners = new List<SpinnerSettings>();

	private bool totalSpinnerFinished;

	private SpriteText totalizer;

	private int currentTotal;

	private int maximumValue;

	private bool useTotalizer;

	public bool TotalCounterFinished => totalSpinnerFinished;

	public void AddTotalizer(SpriteText totalizerSprite)
	{
		totalizer = totalizerSprite;
		useTotalizer = true;
		currentTotal = 0;
	}

	public void AddSpinner(SpriteText _sprite, int _curVal, int _targetVal, float _speed = 3f)
	{
		SpinnerSettings spinnerSettings = new SpinnerSettings();
		spinnerSettings.Setup(_sprite, _curVal, _targetVal, _speed);
		spinners.Add(spinnerSettings);
	}

	public void Run()
	{
		StartCoroutine(RunSequence());
	}

	private IEnumerator RunSequence()
	{
		foreach (SpinnerSettings sp in spinners)
		{
			if (sp.targetVal == 0)
			{
				continue;
			}
			Spinner spinner = CountUpText(sp.sprite, sp.currentVal, sp.targetVal, sp.speedVal);
			while (!spinner.HasStopped)
			{
				if (useTotalizer)
				{
					if (spinner.CurrentValue > spinner.TargetValue)
					{
						totalizer.Text = string.Empty + (currentTotal + spinner.TargetValue);
					}
					else
					{
						totalizer.Text = string.Empty + (currentTotal + spinner.CurrentValue);
					}
				}
				yield return null;
			}
			currentTotal += spinner.TargetValue;
		}
		totalSpinnerFinished = true;
	}

	private Spinner CountUpText(SpriteText spriteText, int currentValue, int targetValue, float speed = 3f)
	{
		Spinner spinner = spriteText.transform.GetComponent<Spinner>();
		if (spinner == null)
		{
			spinner = spriteText.gameObject.AddComponent<Spinner>() as Spinner;
		}
		spinner.InitialValue = currentValue;
		spinner.TargetValue = targetValue;
		spinner.Speed = speed;
		spinner.StartRunning(null);
		return spinner;
	}
}
