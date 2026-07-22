using System;
using UnityEngine;

[Serializable]
public class DrillID
{
	[SerializeField]
	private GameData.GAME_TYPE _gameType;

	[SerializeField]
	private GameData.GAME_LOCATION _gameLocation;

	public GameData.GAME_TYPE GameType => _gameType;

	public GameData.GAME_LOCATION GameLocation => _gameLocation;

	public string FileName => GameData.GetGameName(_gameLocation, _gameType);

	public DrillID()
	{
	}

	public DrillID(GameData.GAME_TYPE type, GameData.GAME_LOCATION location)
	{
		_gameType = type;
		_gameLocation = location;
	}
}
