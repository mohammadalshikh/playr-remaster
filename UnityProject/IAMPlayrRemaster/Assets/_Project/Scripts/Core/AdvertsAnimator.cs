using UnityEngine;

public class AdvertsAnimator : MonoBehaviour
{
	public Material m_material;

	public int m_num_rows;

	public Vector2 m_anim_length;

	public Vector2 m_size;

	private Vector2 m_offset;

	private float m_timer;

	private float m_start_offset;

	private int m_anim_direction;

	private void Start()
	{
		if (m_size.y == 0f)
		{
			m_size.y = 1f / (float)m_num_rows;
		}
		m_offset = new Vector2(0f, m_size.y * (float)Random.Range(0, m_num_rows));
		m_anim_direction = 0;
		m_start_offset = m_offset.x;
	}

	private void Update()
	{
		m_timer += Time.deltaTime;
		if (m_timer >= m_anim_length[m_anim_direction])
		{
			m_timer = m_anim_length[m_anim_direction];
		}
		m_offset[m_anim_direction] = m_start_offset - m_size[m_anim_direction] * (m_timer / m_anim_length[m_anim_direction]);
		m_material.mainTextureOffset = m_offset;
		if (m_timer >= m_anim_length[m_anim_direction])
		{
			if (m_anim_direction == 0)
			{
				m_anim_direction = 1;
			}
			else
			{
				m_anim_direction = 0;
			}
			m_start_offset = m_offset[m_anim_direction];
			m_timer = 0f;
		}
	}

	public void SetTexture(Texture text)
	{
		m_material.mainTexture = text;
	}
}
