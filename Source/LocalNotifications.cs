using System.Collections.Generic;
using UnityEngine;

public class LocalNotifications
{
	private const string ENABLED_KEY = "enabled_key";

	public static List<int> androidNotifactionsIds = new List<int>();

	private static bool notificationsEnabled = true;

	private static string[] messages = new string[2] { "You've been missed down the training ground! Head to IAM PLAYR and set a new high score!", "Still got what it takes to make the leaderboards in IAM PLAYR?" };

	private static int[] alertTimes = new int[2] { 172800, 432000 };

	public static bool NotificationsEnabled
	{
		get
		{
			if (PlayerPrefs.HasKey("enabled_key"))
			{
				return PlayerPrefs.GetInt("enabled_key") == 1;
			}
			return notificationsEnabled;
		}
	}

	public static void ScheduleNotification(NOTIFICATION_KEY key)
	{
		if (notificationsEnabled)
		{
			string title = "Play Now";
			androidNotifactionsIds.Add(EtceteraAndroid.scheduleNotification(alertTimes[(int)key], title, messages[(int)key], messages[(int)key], string.Empty));
		}
	}

	public static void ScheduleAllNotifactions()
	{
		ScheduleNotification(NOTIFICATION_KEY.ENERGY_FULL);
		ScheduleNotification(NOTIFICATION_KEY.FIVE_DAYS_COMEBACK);
	}

	public static void Enable(bool bEnabled)
	{
		notificationsEnabled = bEnabled;
		PlayerPrefs.SetInt("enabled_key", bEnabled ? 1 : 0);
		PlayerPrefs.Save();
		if (!bEnabled)
		{
			CancelAll();
		}
	}

	public static void CancelAll()
	{
		foreach (int androidNotifactionsId in androidNotifactionsIds)
		{
			EtceteraAndroid.cancelNotification(androidNotifactionsId);
		}
	}
}
