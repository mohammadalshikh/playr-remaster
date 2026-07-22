using UnityEngine;

public class HighscoreView : MonoBehaviour
{
	[SerializeField]
	private SpriteText _scoreText;

	private int _lastScore;

	public void UpdateHighScore(GameInfo currentGame)
	{
		int highScore = currentGame.HighScore;
		if (highScore == 0)
		{
			_scoreText.Text = highScore.ToString("#,##0");
		}
		else
		{
			MinigameGUI.CountUpText(_scoreText, _lastScore, highScore);
		}
		_lastScore = highScore;
	}
}
