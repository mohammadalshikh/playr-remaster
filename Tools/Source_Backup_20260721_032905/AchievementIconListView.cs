using System.Runtime.CompilerServices;
using UnityEngine;

public class AchievementIconListView : MonoBehaviour
{
	public delegate void HandleIconViewInput(Achievement achievement);

	[SerializeField]
	private UIScrollList achievements;

	[SerializeField]
	private GameObject achievementIconPrefab;

	private Achievement currentAchievement;

	[method: MethodImpl(32)]
	public static event HandleIconViewInput IconViewInputted;

	private void OnEnable()
	{
		PlayerManager.NewAchievementUnlocked += HandleNewAchievementUnlocked;
	}

	private void OnDisable()
	{
		PlayerManager.NewAchievementUnlocked -= HandleNewAchievementUnlocked;
	}

	private void Start()
	{
		ClearAchievementsList();
	}

	private void HandleNewAchievementUnlocked(Achievement achievement)
	{
		if (achievement.Type != 0)
		{
			IUIListObject iUIListObject = achievements.CreateItem(achievementIconPrefab);
			iUIListObject.transform.GetComponent<AchievementIconView>().SetAchievement(achievement);
			iUIListObject.SetInputDelegate(HandleAchievementIconInput);
		}
	}

	private void HandleAchievementIconInput(ref POINTER_INFO ptr)
	{
		if (ptr.evt == POINTER_INFO.INPUT_EVENT.TAP && AchievementIconListView.IconViewInputted != null)
		{
			AchievementIconListView.IconViewInputted(ptr.targetObj.gameObject.GetComponent<AchievementIconView>().Achievement);
		}
	}

	public void ClearAchievementsList()
	{
		achievements.ClearList(destroy: true);
	}
}
