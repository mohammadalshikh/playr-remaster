using System.Collections;
using UnityEngine;

public class EndShotWithDelay : MonoBehaviour
{
	private static Collider m_last_collider;

	public float delay;

	public void OnTriggerEnter(Collider other)
	{
		if (!(m_last_collider == other))
		{
			m_last_collider = other;
			if (delay == 0f)
			{
				delay = GameData.MinigameObject.CurrentMinigame.EndShotDelay;
			}
			StartCoroutine(EndShot(delay, other.gameObject));
		}
	}

	private IEnumerator EndShot(float delay, GameObject other = null)
	{
		yield return new WaitForSeconds(delay);
		if (m_last_collider != null && m_last_collider.gameObject == GameData.Ball.gameObject)
		{
			m_last_collider = null;
			GameData.MinigameObject.NextChance();
		}
	}
}
