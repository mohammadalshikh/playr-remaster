using System.Runtime.CompilerServices;
using UnityEngine;

public class PlayAgainHandler : MonoBehaviour
{
	public delegate void HandlePlayAgain();

	public static PlayAgainHandler Instance;

	[method: MethodImpl(32)]
	public static event HandlePlayAgain PlayAgainFired;

	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
		}
	}

	public void MinigamePlayAgain()
	{
		if (PlayAgainHandler.PlayAgainFired != null)
		{
			PlayAgainHandler.PlayAgainFired();
		}
		GameData.MinigameObject.GUIManager.ShowPauseScreen(bShow: false);
		GameData.MinigameObject.GUIManager.ResetGameGUI();
		EndScreenManager.Instance.ResetState();
		GameData.MinigameObject.TryAgain();
	}
}
