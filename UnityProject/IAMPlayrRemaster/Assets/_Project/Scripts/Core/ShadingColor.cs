using UnityEngine;

public class ShadingColor : MonoBehaviour
{
	private Collider m_collider;

	public Color m_color;

	public Color Color => m_color;

	public bool Contains(Vector3 pos)
	{
		return m_collider != null && m_collider.bounds.Contains(pos);
	}

	private void Awake()
	{
		m_collider = GetComponent<Collider>();
	}

	private void Update()
	{
		base.enabled = false;
		Object.Destroy(base.gameObject);
	}
}
