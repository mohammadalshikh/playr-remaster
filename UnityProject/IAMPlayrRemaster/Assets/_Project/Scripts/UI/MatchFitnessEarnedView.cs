using System;
using UnityEngine;

public class MatchFitnessEarnedView : MonoBehaviour
{
	[SerializeField]
	private SpriteText matchFitnessEarnedValue;

	public void SetMatchFitnessEarnedValue(int matchFitness)
	{
		if ((bool)matchFitnessEarnedValue)
		{
			iTween.ValueTo(matchFitnessEarnedValue.gameObject, iTween.Hash("from", 0f, "to", (float)matchFitness, "time", 1f, "onupdate", (Action<object>)delegate(object x)
			{
				matchFitnessEarnedValue.Text = ((float)x).ToString("0");
			}));
		}
	}
}
