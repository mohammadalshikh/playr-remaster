using System;
using UnityEngine;

[Serializable]
public class Tutorial
{
	private const float GESTURE_TIME = 1f;

	public const string TUTORIAL_PASSED_KEY = "tutorial_passed";

	public TUTORIAL_TYPE type;

	public string introMessage = string.Empty;

	public string successMessage = string.Empty;

	public float successHoldDelay;

	public string failureMessage = string.Empty;

	public int clockTime;

	public VISUAL_GESTURES visualGesture;

	public float trailTime;

	public string GestureName => GetGestureName(visualGesture);

	public static string GetGestureName(VISUAL_GESTURES gesture)
	{
		return gesture.ToString().ToLower();
	}

	public void ShowGesture()
	{
		if (visualGesture != 0)
		{
			if (trailTime == 0f)
			{
				trailTime = 1f;
			}
			GameData.PathManager.ShowTrail(show: true);
			GameData.PathManager.FollowPath(GestureName, trailTime);
		}
	}

	public static void SetTutorialPassed()
	{
		if (!HasPassedTutorial())
		{
			PlayerPrefs.SetInt("tutorial_passed", 0);
			Debug.Log("Tutorial passed!!");
		}
		else
		{
			Debug.Log("Tutorial had been already passed");
		}
	}

	public static bool HasPassedTutorial()
	{
		return PlayerPrefs.HasKey("tutorial_passed");
	}
}
