using System.Collections.Generic;
using UnityEngine;

public class XmasLightsController : MonoBehaviour
{
	public List<PackedSprite> m_lanterns = new List<PackedSprite>();

	public SFX m_lightOnSound;

	public GameObject m_sparksPrefab;

	public static XmasLightsController Instance;

	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
		}
	}

	private void SetFrameForLantern(PackedSprite lantern, int frameIndex)
	{
		lantern.SetFrame(0, frameIndex);
	}

	private void SetLanternAtIndex(int index, bool on)
	{
		index = Mathf.Clamp(index, 0, m_lanterns.Count - 1);
		SetFrameForLantern(m_lanterns[index], on ? 1 : 0);
		if (on)
		{
			m_lanterns[index].SetSize(1f, 1f);
			SpawnLightOnSparks(m_lanterns[index].transform);
		}
		else
		{
			m_lanterns[index].SetSize(0.5f, 0.5f);
		}
	}

	public void TurnOnAtIndex(int index)
	{
		SetLanternAtIndex(index, on: true);
		AudioManager.PlaySFX(m_lightOnSound, 0f);
	}

	public void TurnOffAtIndex(int index)
	{
		SetLanternAtIndex(index, on: false);
	}

	private void SpawnLightOnSparks(Transform target)
	{
		if (m_sparksPrefab != null)
		{
			Object.Instantiate(m_sparksPrefab, target.position, target.rotation);
		}
	}
}
