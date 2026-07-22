using UnityEngine;

public class EndShot : MonoBehaviour
{
	public void EndCurrentShot()
	{
		GameData.MinigameObject.NextChance();
	}
}
