using System.Collections.Generic;
using UnityEngine;

public class Achievement
{
	public enum TYPE
	{
		GAME_UNLOCK,
		POWERUP_UNLOCK,
		POWERUP_UPGRADE,
		TROPHY
	}

	public enum TROPHY
	{
		Rookie,
		Game_On,
		Centurion,
		On_Target,
		Consistent,
		Bullseye,
		On_Fire,
		Inferno,
		Century,
		Millenium,
		Lucky_Strike,
		On_The_Spot,
		Freekick_Specialist,
		Charged,
		Super_Charged,
		Goldenballs,
		Superfit,
		Superman,
		High_Roller,
		Veteran,
		Chart_Topper,
		Mobot,
		Combo,
		Combo_King
	}

	private string _name = string.Empty;

	private TYPE _type;

	private TROPHY _trophyType;

	private int _value;

	private bool _unlocked;

	private GameData.GAME_LOCATION _location;

	private static string[] _trophyDescriptions = new string[24]
	{
		"Played first game", "Played 25 games", "Played 100 games", "Hit 10 targets in a row", "Hit 20 targets in a row", "Hit 50 targets in a row", "Triggered first blast phase", "Blast phase activated for more than 5 minutes", "Hit 100 targets", "Hit 1000 targets",
		"Curled round a freekick wall", "Curled round 10 freekick walls", "Curled round 100 freekick walls", "Unlocked all powerups", "Unlocked all gold level powerups", "Won all gold trophies", "Earned 100 match fitness", "Earned 1000 match fitness", "Earned 10000 coins", "Played a season of IAP matches",
		"Reached number one slot on a leaderboard", "Shot over 10000m on the training pitch", "Hit 3 targets in one shot", "Hit 5 targets in one shot"
	};

	private static string[] _fbDescriptions = new string[24]
	{
		"kicked off in I AM PLAYR",
		"has played 25 games",
		"has played 100 games",
		string.Empty,
		string.Empty,
		"is on fire, and just hit 50 targets in a row in I AM PLAYR!",
		string.Empty,
		string.Empty,
		"makes a century in I AM PLAYR. 100 targets hit!",
		"is in the running for world player of the year. 1000 targets hit in I AM PLAYR!",
		string.Empty,
		string.Empty,
		"has curled around 100 freekick walls",
		string.Empty,
		"is fully charged",
		"takes gold in every I AM PLAYR mini game!",
		"has earned 100 match fitness",
		"has earned 1000 match fitness",
		"has earned 10000 coins",
		"has played a season of I AM PLAYR matches",
		"has reached number 1 on the leaderboard!",
		"has shot over 10000m in the I AM PLAYR training ground!",
		string.Empty,
		string.Empty
	};

	private static string[] _description = new string[3] { "Unlocked in ", "Unlocked", "Upgraded to " };

	public static string[] gameCenterIds = new string[24]
	{
		"rookie", "gameOn", "centurion", "onTarget", "consistent", "bullseye", "onFire", "inferno", "century", "millenium",
		"luckyStrike", "onTheSpot", "freekickSpecialist", "charged", "superCharged", "goldenBalls", "superfit", "superman", "highRoller", "veteran",
		"chartTopper", "mobot", "combo", "comboKing"
	};

	public string Name => _name;

	public TYPE Type => _type;

	public TROPHY TrophyType => _trophyType;

	public int Value => _value;

	public bool Unlocked => _unlocked;

	public GameData.GAME_LOCATION Location => _location;

	public string FormattedLocation => GameData.FormattedGameLocation(_location);

	public string Title => _type switch
	{
		TYPE.TROPHY => _name.Replace("_", " "), 
		TYPE.GAME_UNLOCK => _name, 
		_ => GameData.CapitalFirstLetter(_name.Replace("_", " ")), 
	};

	public string Description => Type switch
	{
		TYPE.GAME_UNLOCK => _description[(int)_type] + _location, 
		TYPE.POWERUP_UPGRADE => _description[(int)_type] + ((MEDAL)_value).ToString() + " " + _name, 
		TYPE.TROPHY => _trophyDescriptions[(int)_trophyType], 
		_ => _description[(int)_type], 
	};

	public Achievement(TYPE type, string name, int val = 0, GameData.GAME_LOCATION location = GameData.GAME_LOCATION.TrainingCenter)
	{
		_type = type;
		_name = name;
		_value = val;
		_location = location;
		GetUnlockValue();
	}

	public Achievement(TYPE type, TROPHY trophy)
	{
		_type = type;
		_trophyType = trophy;
		_name = trophy.ToString();
		_value = (int)trophy;
		GetUnlockValue();
	}

	public Achievement(TYPE type, GameData.GAME_TYPE gameType, GameData.GAME_LOCATION gameLocation)
	{
		_type = type;
		_name = gameType.ToString();
		_value = 0;
		_location = gameLocation;
		GetUnlockValue();
	}

	public string GetFBDescription(TROPHY trophy)
	{
		return _fbDescriptions[(int)trophy];
	}

	private void GetUnlockValue()
	{
		_unlocked = PlayerPrefs.HasKey(_type.ToString() + _name);
	}

	public void Unlock()
	{
		if (!_unlocked)
		{
			_unlocked = true;
			PlayerPrefs.SetInt(_type.ToString() + _name, 0);
			PlayerManager.SendFeedback("Unlocked " + Title, string.Empty, string.Empty);
		}
	}

	public static List<string> GetUnlockedTrophiesGCIds()
	{
		List<string> list = new List<string>();
		List<Achievement> trophies = GameData.PlayerStats.Trophies;
		int i = 0;
		for (int count = trophies.Count; i < count; i++)
		{
			if (trophies[i].Unlocked)
			{
				list.Add(gameCenterIds[i]);
			}
		}
		return list;
	}

	public bool CheckGameUnlocker(GameData.GAME_TYPE gameType, GameData.GAME_LOCATION gameLocation)
	{
		if (gameType.ToString() == _name && gameLocation == _location)
		{
			return true;
		}
		return false;
	}
}
