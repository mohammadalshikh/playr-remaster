using UnityEngine;

public class SoccerPlayer : MonoBehaviour
{
	private void Awake()
	{
		GameData.CheckInstance();
		GameData.Player = this;
		GameData.HalfTimeRotation(base.transform);
	}

	public void Reset()
	{
	}
}
