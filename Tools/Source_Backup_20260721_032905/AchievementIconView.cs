using UnityEngine;

public class AchievementIconView : MonoBehaviour
{
	[SerializeField]
	private PackedSprite achievementIcon;

	private Achievement achievement;

	public Achievement Achievement => achievement;

	public void SetAchievement(Achievement a)
	{
		achievement = a;
		achievementIcon.SetFrame(0, (int)achievement.TrophyType);
	}
}
