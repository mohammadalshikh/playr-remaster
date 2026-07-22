using UnityEngine;

public class AchievementDetailView : NotificationView
{
	[SerializeField]
	private AchievementIconView achievementIcon;

	public void Init(string topBarTitle, string headerTitle, string bodyText, Achievement achievement)
	{
		Init(topBarTitle, headerTitle, bodyText);
		achievementIcon.SetAchievement(achievement);
	}
}
