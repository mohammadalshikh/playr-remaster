using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;

public class QuitGame : MonoBehaviour
{
	public delegate void HandleWillQuitGame();

	public SCENES sceneToCall;

	[method: MethodImpl(32)]
	public static event HandleWillQuitGame WillQuitGame;

	public void GoToScene()
	{
		StartCoroutine(QuitEvent());
	}

	private IEnumerator QuitEvent()
	{
		if (QuitGame.WillQuitGame != null)
		{
			QuitGame.WillQuitGame();
		}
		yield return StartCoroutine(GameData.WaitSeconds(0.3f));
		GameData.Instance.PauseGame(pause: true);
		sceneToCall = ((GameData.SelectedGameType == GameData.GAME_TYPE.Tutorial) ? SCENES.MAP : sceneToCall);
		StartCoroutine(GameData.Instance.QuitGame(sceneToCall));
		yield return true;
	}
}
