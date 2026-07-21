using UnityEngine;

public class CrowdTranslate : MonoBehaviour
{
	public Vector3 m_offset;

	public float m_anim_length;

	private float m_timer;

	private int m_sign;

	private Vector3 m_start_pos;

	public Vector3 Offset
	{
		get
		{
			return m_offset;
		}
		set
		{
			m_offset = value;
		}
	}

	public float AnimLength
	{
		get
		{
			return m_anim_length;
		}
		set
		{
			m_anim_length = value;
		}
	}

	private void Start()
	{
		m_start_pos = base.transform.position;
		m_timer = 0f;
		m_sign = 1;
	}

	private void Update()
	{
		m_timer += Time.deltaTime;
		if (m_timer >= m_anim_length)
		{
			m_timer = m_anim_length;
		}
		base.transform.localPosition = Vector3.Lerp(m_start_pos, m_start_pos + m_sign * m_offset, m_timer / m_anim_length);
		if (m_timer >= m_anim_length)
		{
			m_timer = 0f;
			m_start_pos = base.transform.localPosition;
			m_sign *= -1;
		}
	}
}
