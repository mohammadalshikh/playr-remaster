using System.Collections.Generic;
using UnityEngine;

public class GlobalSettings : MonoBehaviour
{
	[SerializeField]
	private List<DrillSettings> _trainingCenter = new List<DrillSettings>();

	[SerializeField]
	private List<DrillSettings> _passingDrills = new List<DrillSettings>();

	[SerializeField]
	private List<DrillSettings> _shootingDrills = new List<DrillSettings>();

	public static GlobalSettings Instance;

	private void Awake()
	{
		if (Instance != null)
		{
			Debug.LogWarning("only one GlobalSettings is allowed, destroying " + base.gameObject.name);
			Object.Destroy(base.gameObject);
		}
		else
		{
			Instance = this;
			Object.DontDestroyOnLoad(Instance);
		}
	}

	public static DrillSettings GetGame(GameData.GAME_TYPE gameType, GameData.GAME_LOCATION gameLocation)
	{
		foreach (DrillSettings item in Instance.GetDrillsForLocation(gameLocation))
		{
			if (item.Id.GameType == gameType)
			{
				return item;
			}
		}
		return new DrillSettings();
	}

	public List<DrillSettings> GetDrillsForLocation(GameData.GAME_LOCATION gameLocation)
	{
		return gameLocation switch
		{
			GameData.GAME_LOCATION.TrainingCenter => _trainingCenter, 
			GameData.GAME_LOCATION.PassingDrills => _passingDrills, 
			GameData.GAME_LOCATION.ShootingDrills => _shootingDrills, 
			_ => _trainingCenter, 
		};
	}

	public static DrillSettings GetCurrentGame()
	{
		return GetGame(GameData.SelectedGameType, GameData.SelectedGameLocation);
	}

	public static int GetLevelForUnlock(GameData.GAME_TYPE gameType, GameData.GAME_LOCATION gameLocation)
	{
		return GetGame(gameType, gameLocation).UnlockedAtLevel;
	}
}
