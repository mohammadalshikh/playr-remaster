using System.Collections.Generic;
using UnityEngine;

public class WeatherManager : MonoSingleton<WeatherManager>
{
	public enum RainType
	{
		LIGHT,
		HEAVY
	}

	[SerializeField]
	private Light _sun;

	[SerializeField]
	private List<GameObject> _clouds = new List<GameObject>();

	[SerializeField]
	private List<GameObject> _rainPanels = new List<GameObject>();

	[SerializeField]
	private GameObject _rainObject;

	[SerializeField]
	private GameObject _rainSplash;

	[SerializeField]
	private Color _cloudRainColor = Color.white;

	[SerializeField]
	private Color _cloudNormalColor = Color.white;

	private bool _isRaining;

	private Vector3 _sunInitialPosition;

	public bool IsRaining => _isRaining;

	private void Awake()
	{
		MonoSingleton<WeatherManager>.Instance = this;
	}

	private void Start()
	{
		_sunInitialPosition = _sun.transform.position;
	}

	private void OnEnable()
	{
		QuitGame.WillQuitGame += QuitGameEvent;
	}

	private void OnDisable()
	{
		QuitGame.WillQuitGame -= QuitGameEvent;
	}

	private void QuitGameEvent()
	{
		ShowSun(bActive: false);
	}

	public void ShowClouds(bool bShow)
	{
		foreach (GameObject cloud in _clouds)
		{
			if (cloud != null)
			{
				cloud.SetActive(bShow);
			}
		}
	}

	public void SetRandomConditions()
	{
		if (DeviceInfo.IsHighEnd || Application.platform == RuntimePlatform.OSXEditor)
		{
			bool flag = GameData.ProbabilityTest(0.3f);
			Rain(flag, Random.Range(-30f, 30f), (RainType)Random.Range(0, 2));
			if (flag)
			{
				SetCloudColor(_cloudRainColor);
				SunIntensity(Random.Range(0.2f, 0.5f));
			}
			else
			{
				SetCloudColor(_cloudNormalColor);
				SunIntensity(Random.Range(0.3f, 1f));
			}
		}
	}

	public void Rain(bool bRain, float angle, RainType type)
	{
		if (!DeviceInfo.IsHighEnd && Application.platform != 0)
		{
			return;
		}
		_isRaining = bRain;
		foreach (GameObject rainPanel in _rainPanels)
		{
			rainPanel.SetActive(_rainPanels).IndexOf(rainPanel) <= (int)type && bRain;
		}
		_rainSplash.SetActiveRecursively(type == RainType.HEAVY && bRain);
		SetRainAngle(angle);
	}

	public void SetCloudColor(Color newColor)
	{
		foreach (GameObject cloud in _clouds)
		{
			cloud.renderer.material.color = newColor;
		}
	}

	public void SetCloudColor(Color newColor, float time)
	{
		foreach (GameObject cloud in _clouds)
		{
			iTween.ColorTo(cloud, newColor, time);
		}
	}

	public void SetRainAngle(float angle)
	{
		angle = Mathf.Clamp(angle, -45f, 45f);
		_rainObject.transform.localRotation = Quaternion.Euler(new Vector3(0f, 180f, angle));
	}

	public void ShowSun(bool bActive)
	{
		if (!bActive || DeviceInfo.IsHighEnd || Application.platform == RuntimePlatform.OSXEditor)
		{
			_sun.gameObject.SetActive(bActive);
			SunOffset(new Vector3(0f, 0f, Random.Range(-15f, 15f)));
		}
	}

	public void SunColor(Color newColor)
	{
		_sun.color = newColor;
	}

	public void SunOffset(Vector3 offset)
	{
		_sun.transform.position = _sunInitialPosition + offset;
	}

	public void SunIntensity(float intensity)
	{
		_sun.intensity = intensity;
	}
}
