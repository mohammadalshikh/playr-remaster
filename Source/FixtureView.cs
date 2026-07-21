using UnityEngine;

public class FixtureView : MonoBehaviour
{
	[SerializeField]
	private SpriteText homeClubText;

	[SerializeField]
	private SpriteText awayClubText;

	private void Start()
	{
		if (MonoSingleton<WebIntegrationManager>.IsInitialized)
		{
			MonoSingleton<WebIntegrationManager>.Instance.FixtureDataReceived += HandleFixtureDataReceieved;
		}
	}

	private void HandleFixtureDataReceieved(string homeClubName, string awayClubName)
	{
		if ((bool)homeClubText)
		{
			homeClubText.Text = homeClubName;
		}
		if ((bool)awayClubText)
		{
			awayClubText.Text = awayClubName;
		}
	}

	private void OnDisable()
	{
		if (MonoSingleton<WebIntegrationManager>.IsInitialized)
		{
			MonoSingleton<WebIntegrationManager>.Instance.FixtureDataReceived -= HandleFixtureDataReceieved;
		}
	}
}
