using System.Collections.Generic;
using UnityEngine;

public static class GameCenterManager
{
	private const string gameCenterId = "com.werinteractive.playr";

	public static void AuthenciateUser()
	{
		if (Application.internetReachability != 0)
		{
			Social.localUser.Authenticate(ProcessAuthentication);
		}
	}

	public static void PostAchievementComplete(string achievementId)
	{
		PostAchievementProgress(achievementId, 100.0);
	}

	public static void PostAchievementProgress(string achievementId, double progress)
	{
		if (Social.localUser.authenticated)
		{
			Social.ReportProgress("com.werinteractive.playr." + achievementId, progress, delegate(bool success)
			{
				ProcessPostAchievement(success);
			});
		}
	}

	private static void ProcessAuthentication(bool success)
	{
		if (success)
		{
			Debug.Log("Authentication - Successful");
			PlayerManager.SendFeedback("GameCenterAuthd", string.Empty, string.Empty);
			List<string> unlockedTrophiesGCIds = Achievement.GetUnlockedTrophiesGCIds();
			{
				foreach (string item in unlockedTrophiesGCIds)
				{
					PostAchievementComplete(item);
				}
				return;
			}
		}
		Debug.Log("Authentication - Failed");
	}

	private static void ProcessPostAchievement(bool success)
	{
		if (success)
		{
			Debug.Log("Achievement post - Successful");
		}
		else
		{
			Debug.Log("Achievement post - Failed");
		}
	}
}
