using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("WeRInteractive/Arvind/TargetPlacer")]
public class TargetPlacer : MonoBehaviour
{
	public GameObject m_Prefab;

	public string m_TargetName = "New Target";

	public bool m_TargetsVisible = true;

	public List<Transform> nodes = new List<Transform>();

	public static Dictionary<string, TargetPlacer> m_Targets = new Dictionary<string, TargetPlacer>();

	private void OnEnable()
	{
		if (!m_Targets.ContainsKey(m_TargetName))
		{
			m_Targets.Add(m_TargetName.ToLower(), this);
		}
	}

	private void OnDisable()
	{
		m_Targets.Remove(m_TargetName.ToLower());
	}

	private void Start()
	{
		m_Prefab = GameObject.CreatePrimitive(PrimitiveType.Sphere);
	}

	private void OnDrawGizmos()
	{
		if (!m_TargetsVisible)
		{
		}
	}
}
