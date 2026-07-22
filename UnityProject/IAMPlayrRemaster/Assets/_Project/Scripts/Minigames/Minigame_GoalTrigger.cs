using UnityEngine;

public class Minigame_GoalTrigger : MonoBehaviour
{
	private GameObject m_obj_hit;

	public int m_ID = -1;

	public int m_GoalPoints;

	public int ID
	{
		get
		{
			return m_ID;
		}
		set
		{
			m_ID = value;
		}
	}

	public int GoalPoints
	{
		get
		{
			return m_GoalPoints;
		}
		set
		{
			m_GoalPoints = value;
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (base.enabled && m_obj_hit != other.gameObject && (other.gameObject.GetComponent<SoccerBall>() != null || other.gameObject.GetComponent<Minigame_TargetTrigger>() != null))
		{
			m_obj_hit = other.gameObject;
			GameData.MinigameObject.CurrentMinigame.GoalScored(this);
		}
	}
}
