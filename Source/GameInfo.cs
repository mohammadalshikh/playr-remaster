using System;
using System.Collections.Generic;
using UnityEngine;

public class GameInfo
{
	private const string HIGHSCORE_KEY = "HighScore";

	private const string MEDAL_KEY = "Medal";

	private const string TROPHY_KEY = "Thophy";

	private const string TIMES_PLAYED_KEY = "Times_Played";

	private const string UNLOCKED_NUMBER_KEY = "unlocked_number";

	private string _name = string.Empty;

	private GameData.GAME_TYPE _gameType;

	private GameData.GAME_LOCATION _gameLocation;

	private string _fullName = string.Empty;

	private string _firstName = string.Empty;

	private int _highScore;

	private MEDAL _currentMedal;

	private List<Trophy> _trophies = new List<Trophy>();

	private List<Trophy> _newUnlockedTrophies = new List<Trophy>();

	private int _timesPlayed;

	private int _numberOfUnlocked;

	public GameData.GAME_TYPE GameType => _gameType;

	public string Name => _name;

	public string FullName => _fullName;

	public string FirstName => _firstName;

	public int HighScore
	{
		get
		{
			return _highScore;
		}
		set
		{
			_highScore = value;
		}
	}

	public MEDAL CurrentMedal
	{
		get
		{
			return _currentMedal;
		}
		set
		{
			_currentMedal = value;
		}
	}

	public int TimesPlayed => _timesPlayed;

	public string SavingKey => _name + "GameInfo";

	public List<Trophy> TrophyList => _trophies;

	public List<Trophy> NewUnlockedTrophies => _newUnlockedTrophies;

	public GameData.GAME_TYPE Type => _gameType;

	public GameData.GAME_LOCATION Location => _gameLocation;

	public bool HasGameEverBeenUnlocked => PlayerPrefs.HasKey(_name + "_unlocked");

	public bool IsUnlocked
	{
		get
		{
			if (PlayerPrefs.HasKey("VIPPass"))
			{
				return true;
			}
			if (HasGameEverBeenUnlocked)
			{
				return true;
			}
			return GameData.PlayerStats.CurrentLevel >= UnlockedAt;
		}
	}

	private int TrophyCount => _name switch
	{
		"Downing" => Enum.GetNames(typeof(DOWNING_TROPHIES)).Length, 
		"Henderson" => Enum.GetNames(typeof(HENDERSON_TROPHIES)).Length, 
		"Carragher" => Enum.GetNames(typeof(CARRAGHER_TROPHIES)).Length, 
		"Gerrard" => Enum.GetNames(typeof(GERRARD_TROPHIES)).Length, 
		_ => Enum.GetNames(typeof(GENERIC_TROPHY)).Length, 
	};

	public int UnlockedAt => GlobalSettings.GetLevelForUnlock(_gameType, _gameLocation);

	private string getFullName(string name)
	{
		string empty = string.Empty;
		return name switch
		{
			"Henderson" => "Jordan Henderson", 
			"Carragher" => "Jamie Carragher", 
			"Downing" => "Stewart Downing", 
			"Gerrard" => "Steven Gerrard", 
			"Pumpkin" => "Pumpkin Game", 
			"Parker" => "Scott Parker", 
			"Christmas" => "The Christmas Challenge", 
			_ => name, 
		};
	}

	private string getFirstName(string name)
	{
		string empty = string.Empty;
		return name switch
		{
			"Henderson" => "Jordan", 
			"Carragher" => "Jamie", 
			"Downing" => "Stewart", 
			"Gerrard" => "Steven", 
			"Pumpkin" => "Pumpkin", 
			"Parker" => "Scott", 
			"Christmas" => "Christmas", 
			_ => name, 
		};
	}

	public void CreateWithName(GameData.GAME_TYPE gameType, GameData.GAME_LOCATION gameLocation)
	{
		_name = GameData.GetGameName(gameLocation, gameType);
		_gameType = gameType;
		_gameLocation = gameLocation;
		PlayerManager.GameWonNewMedal += CheckUnlock;
		_currentMedal = (MEDAL)PlayerPrefs.GetInt(string.Empty + _name + "Medal");
	}

	public void UpdateMedal()
	{
		SetMedal(_currentMedal);
	}

	public void SetMedal(MEDAL medal)
	{
		if (_currentMedal < medal || medal == MEDAL.NONE)
		{
			_currentMedal = medal;
			PlayerManager.BroadcastGameWonMedal(this);
			PlayerManager.SendFeedback("Hit " + medal.ToString() + " in " + _gameType, string.Empty, string.Empty);
		}
	}

	private void CheckUnlock(GameInfo gameinfo)
	{
		if (IsUnlocked && !PlayerPrefs.HasKey(_name + "_unlocked"))
		{
			PlayerManager.GameUnlocked(this);
			PlayerPrefs.SetInt(_name + "_unlocked", 0);
			PlayerManager.SendFeedback(string.Concat("Unlocked ", Type, " at", Location), string.Empty, string.Empty);
		}
	}

	public void IncreaseTimesPlayed()
	{
		_timesPlayed++;
	}

	public Trophy GetTrophy(int index)
	{
		return _trophies.Find((Trophy obj) => obj.Type == GetTrophyTypeAtIndex(index));
	}

	public void ClearNewTrophiesList()
	{
		_newUnlockedTrophies.Clear();
	}

	private bool UnlockTrophyWithIndex(int index)
	{
		Trophy trophy = _trophies.Find((Trophy obj) => obj.Type == GetTrophyTypeAtIndex(index));
		if (trophy.State == ITEM_STATE.LOCKED)
		{
			trophy.UnLock();
			_newUnlockedTrophies.Add(trophy);
			_numberOfUnlocked++;
			Debug.Log("Trophy - " + trophy.Type + " - has been unlocked for " + _name + " game");
			return true;
		}
		return false;
	}

	private string GetTrophyTypeAtIndex(int index)
	{
		string empty = string.Empty;
		return _name switch
		{
			"Downing" => ((DOWNING_TROPHIES)index).ToString(), 
			"Henderson" => ((HENDERSON_TROPHIES)index).ToString(), 
			"Carragher" => ((CARRAGHER_TROPHIES)index).ToString(), 
			"Gerrard" => ((GERRARD_TROPHIES)index).ToString(), 
			_ => ((GENERIC_TROPHY)index).ToString(), 
		};
	}

	public bool IsGameFirstTimePlayed()
	{
		Trophy trophy = GetTrophy(0);
		return trophy.State == ITEM_STATE.LOCKED;
	}

	public void SaveData()
	{
		if (!PlayerPrefs.HasKey(SavingKey))
		{
			InitialSave();
		}
		else
		{
			NormalSaving();
		}
	}

	private void InitialSave()
	{
		PlayerPrefs.SetString(SavingKey, _name);
		PlayerPrefs.SetInt(string.Empty + _name + "HighScore", _highScore);
		PlayerPrefs.SetInt(string.Empty + _name + "Medal", (int)_currentMedal);
		PlayerPrefs.SetInt(string.Empty + _name + "Times_Played", _timesPlayed);
		PlayerPrefs.SetInt(string.Empty + _name + "unlocked_number", _numberOfUnlocked);
		foreach (Trophy trophy in _trophies)
		{
			if (trophy.State == ITEM_STATE.UNLOCKED)
			{
				PlayerPrefs.SetString(string.Empty + _name + trophy.Type + "Thophy", trophy.State.ToString());
			}
		}
	}

	private void NormalSaving()
	{
		if (_highScore > PlayerPrefs.GetInt(string.Empty + _name + "HighScore"))
		{
			PlayerPrefs.SetInt(string.Empty + _name + "HighScore", _highScore);
		}
		if ((int)_currentMedal > PlayerPrefs.GetInt(string.Empty + _name + "Medal"))
		{
			PlayerPrefs.SetInt(string.Empty + _name + "Medal", (int)_currentMedal);
		}
		if (_timesPlayed > PlayerPrefs.GetInt(string.Empty + _name + "Times_Played"))
		{
			PlayerPrefs.SetInt(string.Empty + _name + "Times_Played", _timesPlayed);
		}
		if (_numberOfUnlocked > PlayerPrefs.GetInt(string.Empty + _name + "unlocked_number"))
		{
			PlayerPrefs.SetInt(string.Empty + _name + "unlocked_number", _numberOfUnlocked);
		}
		if (_newUnlockedTrophies.Count <= 0)
		{
			return;
		}
		foreach (Trophy newUnlockedTrophy in _newUnlockedTrophies)
		{
			PlayerPrefs.SetString(string.Empty + _name + newUnlockedTrophy.Type + "Thophy", newUnlockedTrophy.State.ToString());
		}
	}

	public bool DataExists()
	{
		return PlayerPrefs.HasKey(SavingKey);
	}

	public void GetData()
	{
		_highScore = PlayerPrefs.GetInt(string.Empty + _name + "HighScore");
		_currentMedal = (MEDAL)PlayerPrefs.GetInt(string.Empty + _name + "Medal");
		_timesPlayed = PlayerPrefs.GetInt(string.Empty + _name + "Times_Played");
		_numberOfUnlocked = PlayerPrefs.GetInt(string.Empty + _name + "unlocked_number");
		foreach (Trophy trophy in _trophies)
		{
			if (PlayerPrefs.HasKey(string.Empty + _name + trophy.Type + "Thophy"))
			{
				trophy.UnLock();
			}
		}
	}
}
