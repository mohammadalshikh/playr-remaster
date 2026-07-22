using System.Collections;
using UnityEngine;

public class LoadGame : MonoBehaviour
{
	public void PlayGame(GameData.GAME_LOCATION gameLocation, GameData.GAME_TYPE gameType)
	{
		GameData.Instance.LoadGame(gameLocation, gameType);
	}

	public IEnumerator PlayGame(GameData.GAME_LOCATION gameLocation, GameData.GAME_TYPE gameType, float delayTime)
	{
		yield return new WaitForSeconds(delayTime);
		GameData.Instance.LoadGame(gameLocation, gameType);
	}
}
