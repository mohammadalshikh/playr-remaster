using UnityEngine;

public class Minigame_TargetTrigger : MonoBehaviour
{
	private GameObject m_obj_hit;

	public int m_ID = -1;

	public bool m_single_hit;

	private int num_hits;

	public SFX m_hit_noise;

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

	public bool SingleHit
	{
		get
		{
			return m_single_hit;
		}
		set
		{
			m_single_hit = value;
		}
	}

	public virtual void OnTriggerEnter(Collider other)
	{
		if ((!SingleHit || num_hits == 0) && base.enabled && m_obj_hit != other.gameObject && (other.CompareTag("ball") || other.gameObject.GetComponent<Minigame_TargetTrigger>() != null))
		{
			m_obj_hit = other.gameObject;
			OnAction();
		}
	}

	public virtual void OnCollisionEnter(Collision other)
	{
		if ((!SingleHit || num_hits == 0) && base.enabled && m_obj_hit != other.gameObject && (other.gameObject.CompareTag("ball") || other.gameObject.GetComponent<Minigame_TargetTrigger>() != null))
		{
			m_obj_hit = other.gameObject;
			OnAction();
		}
	}

	protected virtual void OnAction()
	{
		GameData.MinigameObject.TargetHit(this);
		if (m_hit_noise.audioClip != null)
		{
			AudioManager.PlaySFX(m_hit_noise, 0f);
		}
	}

	public void ValidHit()
	{
		num_hits++;
	}
}
