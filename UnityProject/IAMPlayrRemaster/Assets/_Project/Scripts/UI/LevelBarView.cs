using UnityEngine;

public class LevelBarView : MonoBehaviour
{
	[SerializeField]
	private UIProgressBar levelProgressBar;

	[SerializeField]
	private SpriteText currentLevel;

	[SerializeField]
	private SpriteText currentScoreProgress;

	private void Start()
	{
		if (GameData.PlayerStats.CurrentLevel != GameData.PlayerStats.MaxLevel)
		{
			int aggregateScore = GameData.PlayerStats.AggregateScore;
			int nextLevelTargetScore = GameData.PlayerStats.NextLevelTargetScore;
			if (GameData.PlayerStats.AggregateScore != 0)
			{
				levelProgressBar.Value = (float)aggregateScore / (float)nextLevelTargetScore;
			}
			if (GameData.Instance.HasIntroSeqOccured)
			{
				levelProgressBar.Hide(tf: false);
			}
			currentScoreProgress.Text = GameData.FormatScore(aggregateScore) + " / " + GameData.FormatScore(nextLevelTargetScore);
		}
		else
		{
			currentScoreProgress.Color = Color.white;
			currentScoreProgress.Text = "MAX LEVEL!";
		}
		currentLevel.Text = GameData.PlayerStats.CurrentLevel.ToString();
	}

	private void HandleIntroSceneCompleted()
	{
		if (GameData.PlayerStats.CurrentLevel != GameData.PlayerStats.MaxLevel)
		{
			levelProgressBar.Hide(tf: false);
		}
		else
		{
			levelProgressBar.Hide(tf: true);
		}
	}

	private void OnEnable()
	{
		IntroView.IntroSceneCompleted += HandleIntroSceneCompleted;
	}

	private void OnDisable()
	{
		IntroView.IntroSceneCompleted -= HandleIntroSceneCompleted;
	}
}
