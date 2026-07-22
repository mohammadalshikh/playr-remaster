using System;
using UnityEngine;

public class MatchFitnessBarView : View
{
	[SerializeField]
	private UIProgressBar matchFitnessCompletionBar;

	public void AnimateBarToValue(float value)
	{
		if ((bool)matchFitnessCompletionBar)
		{
			iTween.ValueTo(base.transform.gameObject, iTween.Hash("from", 0f, "to", value, "onUpdate", (Action<object>)delegate(object x)
			{
				matchFitnessCompletionBar.Value = (float)x;
			}));
		}
	}
}
