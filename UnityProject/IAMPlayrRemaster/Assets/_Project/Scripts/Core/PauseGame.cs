using UnityEngine;

public class PauseGame : MonoBehaviour
{
	public bool pause;

	public void PauseEvent()
	{
		Minigame.Pause(pause);
		GameData.MinigameObject.GUIManager.ShowPauseScreen(pause);
	}
}
