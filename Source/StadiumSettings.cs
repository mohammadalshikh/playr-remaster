using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StadiumSettings : MonoBehaviour
{
	public List<Transform> m_lights = new List<Transform>();

	private bool _isAmbientDark;

	public void SetScenarioLighting(float lightPercentage, float time)
	{
		iTween.ColorTo(base.gameObject, iTween.Hash("r", lightPercentage, "g", lightPercentage, "b", lightPercentage, "time", time, "includechildren", false));
	}

	public void EnableLights(bool bEnable)
	{
		foreach (Transform light in m_lights)
		{
			light.renderer.enabled = bEnable;
		}
	}

	public void DimAmbientLight()
	{
		if (!_isAmbientDark)
		{
			_isAmbientDark = true;
			StartCoroutine(ChangeAmbientLightOverTime());
		}
	}

	private IEnumerator ChangeAmbientLightOverTime()
	{
		float timer = 0f;
		while (timer < 1f)
		{
			Color ambientColor = RenderSettings.ambientLight;
			ambientColor.r -= Time.deltaTime / 1.2f;
			ambientColor.g -= Time.deltaTime / 1.2f;
			ambientColor.b -= Time.deltaTime / 1.2f;
			RenderSettings.ambientLight = ambientColor;
			timer += Time.deltaTime;
			yield return null;
		}
	}
}
