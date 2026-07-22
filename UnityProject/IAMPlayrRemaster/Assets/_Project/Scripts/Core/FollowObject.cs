using UnityEngine;

public class FollowObject : MonoBehaviour
{
	public Transform m_target;

	public bool m_look_at;

	public bool m_follow = true;

	public bool m_lock_y;

	private Vector3 m_offset;

	public Vector3 Offset
	{
		set
		{
			m_offset = value;
		}
	}

	private void Start()
	{
		m_offset = base.transform.position;
	}

	private void Update()
	{
		if (m_target == null)
		{
			base.enabled = false;
			Object.Destroy(base.gameObject);
			return;
		}
		if (m_follow)
		{
			Vector3 position = m_target.position + m_offset;
			if (m_lock_y)
			{
				position.y = m_offset.y;
			}
			base.transform.position = position;
		}
		if (m_look_at)
		{
			base.transform.LookAt(m_target);
		}
	}

	public void SetTarget(Transform target, bool lock_y)
	{
		m_target = target;
		m_lock_y = lock_y;
	}
}
