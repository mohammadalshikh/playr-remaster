using System.Collections.Generic;
using UnityEngine;

public class StadiumManager : MonoBehaviour
{
	public STADIUM_ID stadiumID;

	public List<GameObject> stadiumPrefabs = new List<GameObject>();

	private GameObject _stadium;

	private StadiumSettings _settings;

	public GameObject Stadium => _stadium;

	public StadiumSettings Settings => _settings;

	private void Awake()
	{
		GameData.CheckInstance();
		GameData.StadiumObject = this;
		LoadStadium();
	}

	private void LoadStadium()
	{
		stadiumID = (GameData.DebugMode ? stadiumID : GetStadiumIDForLocation(GameData.SelectedGameLocation));
		if (stadiumPrefabs.Count > 0 && _stadium == null)
		{
			_stadium = (GameObject)Object.Instantiate(stadiumPrefabs[(int)stadiumID]);
			_settings = _stadium.GetComponent<StadiumSettings>();
		}
	}

	private STADIUM_ID GetStadiumIDForLocation(GameData.GAME_LOCATION gameLocation)
	{
		return gameLocation switch
		{
			GameData.GAME_LOCATION.TrainingCenter => STADIUM_ID.LIVERPOOL_ARENA, 
			GameData.GAME_LOCATION.PassingDrills => STADIUM_ID.TRAINING_GROUND, 
			GameData.GAME_LOCATION.ShootingDrills => STADIUM_ID.TRAINING_GROUND, 
			_ => STADIUM_ID.TRAINING_GROUND, 
		};
	}
}
