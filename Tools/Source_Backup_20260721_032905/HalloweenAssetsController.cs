using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HalloweenAssetsController : MonoBehaviour
{
	public static HalloweenAssetsController Instance;

	public List<ThunderController> thunders = new List<ThunderController>();

	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
		}
	}

	public void ActivateThunders(bool active)
	{
		foreach (ThunderController thunder in thunders)
		{
			thunder.EnableThunder(active);
		}
	}

	public void ShowThundersForTime(float time, float delay = 0f)
	{
		StartCoroutine(TimedThunders(time, delay));
	}

	private IEnumerator TimedThunders(float duration, float delay)
	{
		yield return new WaitForSeconds(delay);
		ActivateThunders(active: true);
		yield return new WaitForSeconds(duration);
		ActivateThunders(active: false);
	}
}
