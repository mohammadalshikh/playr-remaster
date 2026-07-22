using UnityEngine;

public class HitEvent : MonoBehaviour
{
	public bool m_singleHit;

	[SerializeField]
	private bool _onTriggerOnly;

	protected int _num_hits;

	protected bool _active = true;

	public bool IsActive
	{
		get
		{
			return _active;
		}
		set
		{
			_active = value;
		}
	}

	protected virtual bool WasHit => _num_hits > 0;

	protected virtual void OnActionEnter(Collider other)
	{
	}

	protected virtual void OnActionExit(Collider other)
	{
	}

	private void OnTriggerEnter(Collider other)
	{
		if ((!m_singleHit || _num_hits == 0) && base.enabled && _active && other.CompareTag("ball"))
		{
			OnActionEnter(other);
		}
	}

	public void OnCollisionEnter(Collision other)
	{
		if ((!m_singleHit || _num_hits == 0) && base.enabled && !_onTriggerOnly && _active)
		{
			OnActionEnter(other.collider);
		}
	}

	public void OnTriggerExit(Collider other)
	{
		if (base.enabled && _active)
		{
			OnActionExit(other);
		}
	}

	public void OnCollisionExit(Collision other)
	{
		if (base.enabled && _active)
		{
			OnActionExit(other.collider);
		}
	}

	public void OnTriggerStay(Collider other)
	{
		if (base.enabled && _active)
		{
			OnActionStay(other);
		}
	}

	public void OnCollisionStay(Collision other)
	{
		if (base.enabled && _active)
		{
			OnActionStay(other.collider);
		}
	}

	protected virtual void OnActionStay(Collider other)
	{
	}

	public virtual void ValidHit()
	{
		_num_hits++;
	}

	public virtual void Reset()
	{
		_num_hits = 0;
		_active = true;
	}
}
