using System.Collections;
using UnityEngine;

public class PlayVideoLevel : MonoBehaviour
{
	private void Awake()
	{
		GameData.CheckInstance();
	}

	private void Start()
	{
		Screen.orientation = ScreenOrientation.LandscapeLeft;
		StartCoroutine(PlayVideo());
	}

	private void OnApplicationPause(bool pause)
	{
		if (pause)
		{
			Debug.Log("Pausing Unity to Play video");
		}
		else
		{
			Debug.Log("Video Finished Playing");
		}
	}

	private IEnumerator PlayVideo()
	{
		Handheld.PlayFullScreenMovie(GameData.VideoToPlay + ".mov", Color.black, FullScreenMovieControlMode.Hidden, FullScreenMovieScalingMode.AspectFit);
		yield return StartCoroutine(GameData.WaitSeconds(0.8f));
		GameData.Instance.PauseGame(pause: false);
		Screen.orientation = ScreenOrientation.Portrait;
		yield return new WaitForSeconds(0.1f);
		StartCoroutine(GameData.Instance.LoadPreviousLevel());
	}
}
