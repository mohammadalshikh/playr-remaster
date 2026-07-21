using UnityEngine;

public class PowerUp
{
	private POWER_UP _type;

	private bool _isUnlocked;

	private bool _isAwarded;

	private int _upgradeIndex;

	private static int[] _cost = new int[8] { 50, 50, 50, 20, 20, 10, 20, 10 };

	private static int[,] _unlockThresholds = new int[8, 4]
	{
		{ 6, 9, 11, 20 },
		{ 2, 7, 12, 17 },
		{ 4, 9, 14, 19 },
		{ 6, 9, 11, 20 },
		{ 8, 10, 14, 19 },
		{ 3, 8, 13, 18 },
		{ 2, 7, 12, 17 },
		{ 5, 10, 15, 20 }
	};

	private static string[] _unlockGames = new string[8]
	{
		string.Empty,
		string.Empty,
		"countdown_wall",
		"knockout_wall",
		"countdown_pass",
		string.Empty,
		"lifeline_wall",
		string.Empty
	};

	private GameData.GAME_TYPE[] _upgradeGameTypes = new GameData.GAME_TYPE[8]
	{
		GameData.GAME_TYPE.Tutorial,
		GameData.GAME_TYPE.Tutorial,
		GameData.GAME_TYPE.Countdown,
		GameData.GAME_TYPE.Knockout,
		GameData.GAME_TYPE.Countdown,
		GameData.GAME_TYPE.Countdown,
		GameData.GAME_TYPE.Lifeline,
		GameData.GAME_TYPE.Countdown
	};

	private GameData.GAME_LOCATION[] _upgradeGameLocations = new GameData.GAME_LOCATION[8]
	{
		GameData.GAME_LOCATION.TrainingCenter,
		GameData.GAME_LOCATION.TrainingCenter,
		GameData.GAME_LOCATION.TrainingCenter,
		GameData.GAME_LOCATION.TrainingCenter,
		GameData.GAME_LOCATION.PassingDrills,
		GameData.GAME_LOCATION.TrainingCenter,
		GameData.GAME_LOCATION.TrainingCenter,
		GameData.GAME_LOCATION.TrainingCenter
	};

	private static string[] _descriptions = new string[8]
	{
		string.Empty,
		string.Empty,
		"The multiplier bonus means more multiplier targets appear.\n\nEarn massive points quickly by unlocking Bronze, Silver and Gold levels to get even more targets.",
		"Use this one to increase the number of bonus time targets you get!\n\nUnlock Bronze, Silver and Gold levels to get even more.",
		"More heart targets appear on the wall if you use this one.\n\nEarn Bronze, Silver and Gold levels to make even more appear!",
		"Hit enough targets in a row and your blast bar fires up.\n\nDouble points while it's active!\n\nUpgrades keep it alight for longer.",
		"This one makes more shield targets appear on the wall. Shields protect your lives and multiplier level.\n\nBronze, Silver and Gold levels increase the number of shield targets you get!",
		"Use super control to slow time down, allowing you more time, to get your shot perfect!"
	};

	private int[] _minUpgradeIndex = new int[8] { 0, 1, 1, 1, 1, 5, 1, 1 };

	private int[] _maxUpgradeIndex = new int[8] { 3, 4, 4, 4, 4, 8, 3, 4 };

	public int CurrentUpgradeIndex
	{
		get
		{
			int num = 0;
			int i = 0;
			for (int num2 = 4; i < num2; i++)
			{
				if (_unlockThresholds[(int)_type, i] <= GameData.PlayerStats.CurrentLevel)
				{
					num++;
				}
			}
			return num;
		}
	}

	public POWER_UP Type => _type;

	public bool IsUnlocked
	{
		get
		{
			if (PlayerPrefs.HasKey("VIPPass"))
			{
				return true;
			}
			return _isUnlocked;
		}
	}

	public bool IsAwarded => _isAwarded;

	public int UpgradeIndex => _upgradeIndex;

	public string Name => _type.ToString().ToLower();

	public bool IsEnabled => _descriptions[(int)_type] != string.Empty;

	public PowerUp(POWER_UP type)
	{
		_type = type;
		InitState();
	}

	public void Unlock()
	{
		_isUnlocked = true;
		if (_descriptions[(int)_type] != string.Empty)
		{
			PlayerManager.PowerUpUnlocked(this);
		}
		SaveState();
	}

	public void Award(bool bAward = true)
	{
		_isAwarded = bAward;
	}

	public bool CheckUnlockThreshold()
	{
		if (_isUnlocked || _type == POWER_UP.BLAST_BONUS)
		{
			return false;
		}
		if (GetUnlockThreshold() <= GameData.PlayerStats.CurrentLevel)
		{
			Unlock();
			return true;
		}
		return false;
	}

	public void SetUpgradeIndex(int index, GameData.GAME_TYPE gameType = GameData.GAME_TYPE.Tutorial, GameData.GAME_LOCATION location = GameData.GAME_LOCATION.TrainingCenter)
	{
		if (((gameType != _upgradeGameTypes[(int)_type] || location != _upgradeGameLocations[(int)_type]) && _type != POWER_UP.BLAST_BONUS) || (gameType != GameData.GAME_TYPE.Tutorial && _type == POWER_UP.BLAST_BONUS))
		{
			return;
		}
		int num = index + GetMinUpgradeIndex();
		if (num != _upgradeIndex)
		{
			_upgradeIndex = num;
			if (_upgradeIndex > GetMaxUpgradeIndex())
			{
				_upgradeIndex = GetMaxUpgradeIndex();
			}
			if (_descriptions[(int)_type] != string.Empty)
			{
				PlayerManager.PowerUpUpgraded(this);
			}
			PlayerManager.SendFeedback(string.Concat("Upgrade ", _type, " to ", ((MEDAL)_upgradeIndex).ToString()), string.Empty, string.Empty);
			SaveState();
		}
	}

	public int GetRoughIndex()
	{
		return 0 + (_upgradeIndex - GetMinUpgradeIndex());
	}

	private void SaveState()
	{
		PlayerPrefs.SetInt(SavingKey(), _isUnlocked ? 1 : 0);
		PlayerPrefs.SetInt(SavingKey() + "upgradeIndex", _upgradeIndex);
		PlayerPrefs.Save();
	}

	private void InitState()
	{
		_isUnlocked = ((PlayerPrefs.GetInt(SavingKey(), 0) != 0) ? true : false);
		_upgradeIndex = PlayerPrefs.GetInt(SavingKey() + "upgradeIndex", GetMinUpgradeIndex());
	}

	private int GetUnlockThreshold()
	{
		return _unlockThresholds[(int)_type, 0];
	}

	private string GetGameNameUnlock()
	{
		return _unlockGames[(int)_type];
	}

	public static int UnlockThresholdForType(POWER_UP powerType)
	{
		return _unlockThresholds[(int)powerType, 0];
	}

	public static int CostForType(POWER_UP powerType)
	{
		return _cost[(int)powerType];
	}

	public static string DescriptionForType(POWER_UP powerType)
	{
		return _descriptions[(int)powerType];
	}

	private int GetMinUpgradeIndex()
	{
		return _minUpgradeIndex[(int)_type];
	}

	private int GetMaxUpgradeIndex()
	{
		return _maxUpgradeIndex[(int)_type];
	}

	private string SavingKey()
	{
		return _type.ToString();
	}
}
