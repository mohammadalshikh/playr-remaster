using System.Collections;
using UnityEngine;

public class RushNotification : MonoBehaviour
{
	[SerializeField]
	private UIPanel _rushPanel;

	[SerializeField]
	private SpriteText _rushText;

	private int _showTextCounter;

	private void OnEnable()
	{
		MinigameChallenge.RushModeCompleted += Completed;
		MinigameChallenge.RushRemaining += RemainingScoreChanged;
	}

	private void OnDisable()
	{
		MinigameChallenge.RushModeCompleted -= Completed;
		MinigameChallenge.RushRemaining -= RemainingScoreChanged;
	}

	private void Completed(int rushBonus)
	{
		_rushText.Text = "RUSH COMPLETED!";
		_rushText.SetColor(Color.green);
		StartCoroutine(Show());
	}

	private void RemainingScoreChanged(int remainingScore)
	{
		if (remainingScore > 0)
		{
			_showTextCounter++;
			if (_showTextCounter >= 10)
			{
				_showTextCounter = 0;
				_rushText.SetColor(new Color(40f / 51f, 1f, 16f / 51f, 1f));
				_rushText.Text = remainingScore.ToString("#,##0") + " TO FINISH!";
				StartCoroutine(Show());
			}
		}
	}

	private IEnumerator Show()
	{
		if (!(_rushPanel == null))
		{
			_rushPanel.BringIn();
			while (_rushPanel.IsTransitioning())
			{
				yield return null;
			}
			_rushPanel.Dismiss();
		}
	}
}
