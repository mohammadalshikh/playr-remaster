using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
	public delegate void HandleMatchFitnessEarned();

	public delegate void HandleGameWonMedal(GameInfo gameinfo);

	public delegate void HandleAchievementUnlocked(Achievement achievementUnlocked);

	public delegate void HandleNewHighscore(int newHighscore);

	public delegate void HandleCoinsStock();

	public delegate void HandleLevelledUp(int level);

	public const int TIME_LEVEL_1 = 5;

	public const int TIME_LEVEL_2 = 7;

	public const int TIME_LEVEL_3 = 9;

	public const int TIME_LEVEL_4 = 10;

	public const string PLAYER_NAME_KEY = "player_name";

	public const string MUSIC_VOLUME_KEY = "music_volume";

	public const string SOUNDFX_VOLUME_KEY = "soundfx_volume";

	public const string GLOBAL_SCORE_KEY = "global_score";

	public const string CURRENT_MULTIPLIER = "current_multiplier";

	public const string CURRENT_TIME_BONUS = "current_time_bonus";

	public const string CURRENT_SHIELD = "current_shield";

	public const string COINS_STOCK_KEY = "coins_stock";

	public const string MULTIPLIER_UNLOCKED = "multiplier_unlocked";

	public const string TIME_BONUS_UNLOCKED = "time_unlocked";

	public const string SHIELD_UNLOCKED = "shield_unlocked";

	public const string SUPER_CONTROL_UNLOCKED = "supercontrol_unlocked";

	public const string FIRST_TIME_PLAYED = "first_time";

	public const string GAMES_PLAYED_KEY = "games_played";

	public const string ENERGY_PURCHASES = "energy_purchases";

	private const string currentLevelKey = "currentPlayerLevel";

	private const string aggregateScoreKey = "aggregateScore";

	private string m_player_name = string.Empty;

	private float m_music_global_volume = 1f;

	private float m_soundfx_global_volume = 1f;

	private List<GameInfo> _games = new List<GameInfo>();

	private string m_current_game;

	private int m_games_played;

	private List<Achievement> _gameUnlocks = new List<Achievement>();

	private List<Achievement> _powerUpUnlocks = new List<Achievement>();

	private List<Achievement> _powerUpUpgrades = new List<Achievement>();

	private List<Achievement> _trophies = new List<Achievement>();

	private int m_global_highscore;

	private bool m_new_highscore;

	private int m_coins_stock = 500;

	private int m_coins_just_earned;

	private int m_energy_purchases;

	private int coinRewardForScore;

	private int m_match_fitness_earned;

	private int m_current_level;

	private int m_aggregate_score;

	private int[] nextLevelTargetScores = new int[24]
	{
		10000, 50000, 75000, 100000, 125000, 150000, 175000, 200000, 250000, 300000,
		350000, 400000, 450000, 500000, 550000, 600000, 650000, 700000, 750000, 800000,
		850000, 900000, 950000, 975000
	};

	private string[] ranks = new string[51]
	{
		"Ball Boy", "Grounds Keeper", "Kit Washer", "Boot Polisher", "Club Mascot", "Scout", "Street Player", "Apprentice", "Bench Warmer", "Debut Scorer",
		"Pub League", "All Rounder", "Pub League Hero", "3rd Division", "Local Hero", "Rising Star", "Hat-trick Hero", "Volley Specialist", "Goal Scorer", "Nutmegger",
		"2nd Division", "Trick Artist", "Fancy Finisher", "Cup Winner", "Play Maker", "Top League", "FK Specialist", "Talisman", "Top Scorer", "Most Assits",
		"Top Class", "Club Captain", "Golden Boot", "National Hero", "MVP", "National Captian", "Renowed", "Club Legend", "International", "Champion",
		"Superstar", "World Class", "Media Sensation", "Legend", "World XI", "Player of the Year", "Unbeatable", "Master", "Immortal", "All-Time XI",
		"G.O.A.T"
	};

	private int _hitCounter;

	private int _totalShots;

	private static PlayerManager m_instance;

	private List<PowerUp> m_powerUps = new List<PowerUp>();

	private int m_super_control_index = 1;

	public List<GameInfo> Games => _games;

	public List<Achievement> Trophies => _trophies;

	public List<PowerUp> PowerUps => m_powerUps;

	public string PlayerName
	{
		get
		{
			return m_player_name;
		}
		set
		{
			m_player_name = value;
		}
	}

	public float MusicGlobalVolume => m_music_global_volume;

	public float SoundFXGlobalVolume => m_soundfx_global_volume;

	public int CurrentShield => PowerUpWithType(POWER_UP.SHIELD).UpgradeIndex;

	public string CurrentGame => m_current_game;

	public int CurrentTimeBonus => PowerUpWithType(POWER_UP.TIME).UpgradeIndex;

	public bool NewHighScore => m_new_highscore;

	public int GlobalScore => m_global_highscore;

	public int CoinsStock => m_coins_stock;

	public int CoinsJustEarned => m_coins_just_earned;

	public int SuperControlIndex => m_super_control_index;

	public int GamesPlayed => m_games_played;

	public int EnergyPurchases => m_energy_purchases;

	public int MatchFitnessEarned => m_match_fitness_earned;

	public int CurrentLevel => m_current_level;

	public int AggregateScore => m_aggregate_score;

	public int NextLevelTargetScore
	{
		get
		{
			if (m_current_level < nextLevelTargetScores.Length - 1)
			{
				return nextLevelTargetScores[m_current_level];
			}
			return 500000 * (m_current_level - 23);
		}
	}

	public string Rank
	{
		get
		{
			if (m_current_level < MaxLevel && ranks[m_current_level] != null)
			{
				return ranks[m_current_level];
			}
			return ranks[ranks.Length - 1];
		}
	}

	public int MaxLevel => ranks.Length - 1;

	public int HitCounter => _hitCounter;

	public int TotalShots
	{
		get
		{
			return _totalShots;
		}
		set
		{
			_totalShots = value;
		}
	}

	public int CoinRewardForScore => coinRewardForScore;

	public float Accuracy
	{
		get
		{
			if (_totalShots == 0)
			{
				return 0f;
			}
			return Mathf.Round(Mathf.Clamp01((float)_hitCounter / (float)_totalShots) * 100f);
		}
	}

	public int CurrentTimePowerUp => CurrentTimeBonus switch
	{
		0 => 5, 
		1 => 7, 
		2 => 9, 
		3 => 10, 
		_ => 5, 
	};

	public int CoinRewardForLevelUp
	{
		get
		{
			int num = 0;
			num = ((m_current_level < 11) ? 10 : ((m_current_level < 11 || m_current_level >= 21) ? 20 : 15));
			return 50 + num * m_current_level;
		}
	}

	[method: MethodImpl(32)]
	public event HandleMatchFitnessEarned MatchFitnessEarnedChanged;

	[method: MethodImpl(32)]
	public static event HandleGameWonMedal GameWonNewMedal;

	[method: MethodImpl(32)]
	public static event HandleAchievementUnlocked NewAchievementUnlocked;

	[method: MethodImpl(32)]
	public static event HandleNewHighscore HighscoreChanged;

	[method: MethodImpl(32)]
	public event HandleCoinsStock CoinsStockChanged;

	[method: MethodImpl(32)]
	public static event HandleLevelledUp LevelledUp;

	[method: MethodImpl(32)]
	public static event Action<DateTime> checkDatedEvent;

	private void Awake()
	{
		if (m_instance == null)
		{
			m_instance = this;
		}
		InitGames();
		SetupPowerUps();
		SetupTrophies();
		GetSavedData();
	}

	private void Start()
	{
		FlurryAndroid.onStartSession("XXG4GHKPM2H2SG7C7JT3");
		if (!PlayerPrefs.HasKey("HasMigratedGameSaves"))
		{
			UpdateGamesToNewVersion();
			PlayerPrefs.SetInt("HasMigratedGameSaves", 0);
		}
		UpdateUnlocks();
	}

	public void UpdateGamesToNewVersion()
	{
		foreach (GameInfo game in _games)
		{
			if (game.IsUnlocked && game.Type == GameData.GAME_TYPE.Knockout && !GetGameInfoForLocationAndType(game.Location, GameData.GAME_TYPE.Rush).IsUnlocked)
			{
				PlayerPrefs.SetInt(GameData.GetGameName(game.Location, GameData.GAME_TYPE.Rush) + "_unlocked", 0);
				PlayerPrefs.DeleteKey(GameData.GetGameName(game.Location, GameData.GAME_TYPE.Knockout) + "_unlocked");
			}
		}
	}

	private void GetSavedData()
	{
		if (PlayerPrefs.HasKey("player_name"))
		{
			m_player_name = PlayerPrefs.GetString("player_name");
			m_music_global_volume = PlayerPrefs.GetFloat("music_volume");
			m_soundfx_global_volume = PlayerPrefs.GetFloat("soundfx_volume");
			m_global_highscore = PlayerPrefs.GetInt("global_score");
			m_coins_stock = PlayerPrefs.GetInt("coins_stock", 500);
			m_games_played = PlayerPrefs.GetInt("games_played");
			m_energy_purchases = PlayerPrefs.GetInt("energy_purchases");
			_hitCounter = PlayerPrefs.GetInt("hit_counter", 0);
			_totalShots = PlayerPrefs.GetInt("totalShots", 0);
			m_current_level = PlayerPrefs.GetInt("currentPlayerLevel");
			m_aggregate_score = PlayerPrefs.GetInt("aggregateScore");
			{
				foreach (GameInfo game in _games)
				{
					if (game.DataExists())
					{
						game.GetData();
					}
				}
				return;
			}
		}
		Debug.Log("No available games data, No previous Games have been saved before");
	}

	public void SaveData()
	{
		CalculateGlobalScore();
		if (!PlayerPrefs.HasKey("player_name"))
		{
			PlayerPrefs.SetString("player_name", m_player_name);
		}
		PlayerPrefs.SetFloat("music_volume", m_music_global_volume);
		PlayerPrefs.SetFloat("soundfx_volume", m_soundfx_global_volume);
		PlayerPrefs.SetInt("global_score", m_global_highscore);
		PlayerPrefs.SetInt("coins_stock", m_coins_stock);
		PlayerPrefs.SetInt("games_played", m_games_played);
		PlayerPrefs.SetInt("energy_purchases", m_energy_purchases);
		PlayerPrefs.SetInt("currentPlayerLevel", m_current_level);
		PlayerPrefs.SetInt("aggregateScore", m_aggregate_score);
		PlayerPrefs.SetInt("hit_counter", _hitCounter);
		PlayerPrefs.SetInt("totalShots", _totalShots);
		foreach (GameInfo game in _games)
		{
			game.SaveData();
		}
	}

	public static void SendFeedback(string key, string paramName = "", string paramValue = "")
	{
		if (!PlayerPrefs.HasKey(key) || !(paramName == string.Empty))
		{
			PlayerPrefs.SetInt(key, 1);
			if (paramName != string.Empty && paramValue != string.Empty)
			{
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				dictionary.Add(paramName, paramValue);
				FlurryAndroid.logEvent(key, dictionary);
			}
			else
			{
				FlurryAndroid.logEvent(key);
			}
		}
	}

	public void AwardPowerUp(POWER_UP type)
	{
		PowerUpWithType(type).Award();
	}

	public void NextLevelPowerUp()
	{
		DrillID drillID = new DrillID(GameData.SelectedGameType, GameData.SelectedGameLocation);
		foreach (PowerUp powerUp in m_powerUps)
		{
			PowerUpWithType(powerUp.Type).SetUpgradeIndex(powerUp.CurrentUpgradeIndex, drillID.GameType, drillID.GameLocation);
		}
	}

	public PowerUp PowerUpWithType(POWER_UP powerUpType)
	{
		return m_powerUps[(int)powerUpType];
	}

	public bool IsPowerUpUnlocked(POWER_UP powerUpType)
	{
		return PowerUpWithType(powerUpType).IsUnlocked;
	}

	public void CheckPowerUpUnlocks()
	{
		foreach (PowerUp powerUp in m_powerUps)
		{
			if (powerUp.CheckUnlockThreshold())
			{
				SendFeedback("Unlocked " + powerUp.Type, string.Empty, string.Empty);
			}
		}
	}

	public void ResetPowerUps()
	{
		foreach (PowerUp powerUp in m_powerUps)
		{
			powerUp.Award(bAward: false);
		}
	}

	public bool IsPowerUpAwarded(POWER_UP type)
	{
		return PowerUpWithType(type).IsAwarded;
	}

	private void SetupPowerUps()
	{
		for (int i = 0; i < Enum.GetValues(typeof(POWER_UP)).Length; i++)
		{
			PowerUp powerUp = new PowerUp((POWER_UP)i);
			m_powerUps.Add(powerUp);
			if (powerUp.IsEnabled)
			{
				_powerUpUnlocks.Add(new Achievement(Achievement.TYPE.POWERUP_UNLOCK, powerUp.Name));
				for (int j = 1; j < Enum.GetValues(typeof(MEDAL)).Length; j++)
				{
					_powerUpUpgrades.Add(new Achievement(Achievement.TYPE.POWERUP_UPGRADE, powerUp.Name, j));
				}
			}
		}
	}

	private bool CheckPowerupAllUnlock()
	{
		foreach (Achievement powerUpUnlock in _powerUpUnlocks)
		{
			if (!powerUpUnlock.Unlocked)
			{
				return false;
			}
		}
		return true;
	}

	private bool CheckBlastPhaseAllUnlock()
	{
		int num = 0;
		foreach (Achievement powerUpUnlock in _powerUpUnlocks)
		{
			if (powerUpUnlock.Unlocked)
			{
				num++;
			}
		}
		return num == _powerUpUnlocks.Count - 1;
	}

	private bool CheckBlastPhaseAllUpgradedForMedal(MEDAL medal)
	{
		if (medal == MEDAL.NONE)
		{
			return false;
		}
		int num = 0;
		foreach (Achievement powerUpUpgrade in _powerUpUpgrades)
		{
			if (powerUpUpgrade.Value == (int)medal && powerUpUpgrade.Unlocked)
			{
				num++;
			}
		}
		return num == _powerUpUnlocks.Count - 1;
	}

	private bool CheckPowerupAllGold()
	{
		foreach (Achievement powerUpUnlock in _powerUpUnlocks)
		{
			if (powerUpUnlock.Value != 3)
			{
				return false;
			}
		}
		return true;
	}

	public bool AddScore(string minigameName, int score)
	{
		m_current_game = minigameName;
		AwardCoinsForScore(score);
		AwardMatchFitness(score);
		AwardLevel(score);
		if (IsNewHighScore(minigameName, score))
		{
			GameInfoWithName(minigameName).HighScore = score;
			m_new_highscore = true;
			CalculateGlobalScore();
			if (PlayerManager.HighscoreChanged != null)
			{
				PlayerManager.HighscoreChanged(m_global_highscore);
			}
			return true;
		}
		return false;
	}

	public void AwardLevel(int score)
	{
		m_aggregate_score += score;
		if (m_current_level < MaxLevel && m_aggregate_score >= NextLevelTargetScore)
		{
			m_current_level++;
			AddCoins(CoinRewardForLevelUp);
			SendFeedback("Levelled Up", "Level", m_current_level.ToString());
			if (PlayerManager.LevelledUp != null)
			{
				PlayerManager.LevelledUp(m_current_level);
			}
			m_aggregate_score = 0;
		}
	}

	public int GetHighScoreForMinigame(string minigameName)
	{
		return GameInfoWithName(minigameName).HighScore;
	}

	public int GetLastGameHighScore()
	{
		return GetHighScoreForMinigame(m_current_game);
	}

	private void ResetHighScoreStatus()
	{
		m_new_highscore = false;
	}

	private void CalculateGlobalScore()
	{
		int num = 0;
		foreach (GameInfo game in _games)
		{
			num += game.HighScore;
		}
		m_global_highscore = num;
	}

	private bool IsNewHighScore(string minigameName, int score)
	{
		if (!Minigame.GameInfoExists())
		{
			return false;
		}
		if (GameInfoWithName(minigameName).HighScore < score)
		{
			return true;
		}
		return false;
	}

	public void ForceScoreForGame(string gameName, int score)
	{
		if (GameInfoExists(gameName))
		{
			GameInfo gameInfo = GameInfoWithName(gameName);
			gameInfo.HighScore = score;
			gameInfo.SaveData();
		}
	}

	public void ResetScoreForGame(string gameName)
	{
		ForceScoreForGame(gameName, 0);
	}

	public bool GameInfoExists(string gameInfoName)
	{
		foreach (GameInfo game in _games)
		{
			if (game.Name.Equals(gameInfoName))
			{
				return true;
			}
		}
		Debug.LogWarning("No GameInfo named " + gameInfoName + " was found");
		return false;
	}

	public void AddCoins(int numberOfCoins)
	{
		m_coins_just_earned = numberOfCoins;
		m_coins_stock += numberOfCoins;
		if (m_coins_stock >= 10000)
		{
			TrophyUnlocked(Achievement.TROPHY.High_Roller);
		}
		if (this.CoinsStockChanged != null)
		{
			this.CoinsStockChanged();
		}
	}

	public bool UseCoins(int numberOfCoins)
	{
		if (numberOfCoins <= m_coins_stock && numberOfCoins > 0)
		{
			m_coins_stock -= numberOfCoins;
			if (this.CoinsStockChanged != null)
			{
				this.CoinsStockChanged();
			}
			return true;
		}
		return false;
	}

	private void ResetRecentCoinsEarnings()
	{
		m_coins_just_earned = 0;
	}

	public GameInfo CurrentGameInfo()
	{
		return GameInfoWithName(GameData.SelectedGame);
	}

	public static GameInfo GetCurrentGame()
	{
		return GetGameInfoWithName(GameData.SelectedGame);
	}

	public GameInfo GameInfoWithName(string minigameName)
	{
		return _games.Find((GameInfo gInfo) => gInfo.Name == minigameName);
	}

	public bool GetIsGameUnlockedAtLocation(GameData.GAME_LOCATION gameLocation)
	{
		foreach (GameInfo game in _games)
		{
			if (game.Location == gameLocation && game.IsUnlocked && game.TimesPlayed == 0 && game.Name != "tutorial_wall")
			{
				return true;
			}
		}
		return false;
	}

	public static GameInfo GetGameInfoWithName(string minigameName)
	{
		if (m_instance == null)
		{
			Debug.LogError("PlayerManager has not been initialized yet. Returning empty gameInfo instead");
			return new GameInfo();
		}
		GameInfo gameInfo = m_instance.GameInfoWithName(minigameName);
		if (gameInfo != null)
		{
			return gameInfo;
		}
		Debug.LogWarning("No GameInfo found for game name " + minigameName + ". Returning empty gameInfo instead");
		return new GameInfo();
	}

	public static GameInfo GetGameInfoForLocationAndType(GameData.GAME_LOCATION location, GameData.GAME_TYPE type)
	{
		if (m_instance == null)
		{
			Debug.LogError("PlayerManager has not been initialized yet. Returning empty gameInfo instead");
			return new GameInfo();
		}
		string gameName = GameData.GetGameName(location, type);
		GameInfo gameInfo = m_instance.GameInfoWithName(gameName);
		if (gameInfo != null)
		{
			return gameInfo;
		}
		Debug.LogWarning("No GameInfo found for game name " + gameName + ". Returning empty gameInfo instead");
		return new GameInfo();
	}

	public List<GameInfo> GetGameInfoForLocation(GameData.GAME_LOCATION location)
	{
		return _games.FindAll((GameInfo gInfo) => gInfo.Location == location);
	}

	public void UpdateMedals()
	{
		CurrentGameInfo().UpdateMedal();
	}

	public void ResetPostGameData()
	{
		ResetRecentCoinsEarnings();
		ResetHighScoreStatus();
		m_match_fitness_earned = 0;
		foreach (GameInfo game in _games)
		{
			game.ClearNewTrophiesList();
		}
	}

	private void InitGames()
	{
		for (int i = 0; i < Enum.GetValues(typeof(GameData.GAME_LOCATION)).Length; i++)
		{
			if (i != 4 && i != 1)
			{
				for (int j = 0; j < Enum.GetValues(typeof(GameData.GAME_TYPE)).Length; j++)
				{
					SetUpGame((GameData.GAME_TYPE)j, (GameData.GAME_LOCATION)i);
				}
			}
		}
	}

	private void SetUpGame(GameData.GAME_TYPE gameType, GameData.GAME_LOCATION location)
	{
		string gameName = GameData.GetGameName(location, gameType);
		if (!(gameName == string.Empty))
		{
			GameInfo gameInfo = new GameInfo();
			gameInfo.CreateWithName(gameType, location);
			_games.Add(gameInfo);
			_gameUnlocks.Add(new Achievement(Achievement.TYPE.GAME_UNLOCK, gameType, location));
		}
	}

	public void SetMusicGlobalVolume(float volume)
	{
		if (volume >= 0f && volume <= 1f)
		{
			m_music_global_volume = volume;
		}
	}

	public void SetSoundFXGlobalVolume(float volume)
	{
		if (volume >= 0f && volume <= 1f)
		{
			m_soundfx_global_volume = volume;
		}
	}

	public void FirstTimePlayed()
	{
		PlayerPrefs.SetInt("first_time", 1);
	}

	public bool IsFirstTimePlayed()
	{
		return !PlayerPrefs.HasKey("first_time");
	}

	public void IncreaseGamesPlayed()
	{
		m_games_played++;
		if (m_games_played == 25)
		{
			TrophyUnlocked(Achievement.TROPHY.Game_On);
		}
		if (m_games_played == 100)
		{
			TrophyUnlocked(Achievement.TROPHY.Centurion);
		}
	}

	public static void IncrementHitCounter()
	{
		m_instance._hitCounter++;
		int hitCounter = m_instance._hitCounter;
		if (hitCounter == 100)
		{
			TrophyUnlocked(Achievement.TROPHY.Century);
		}
		if (hitCounter == 1000)
		{
			TrophyUnlocked(Achievement.TROPHY.Millenium);
		}
	}

	private void AwardMatchFitness(int score)
	{
		m_match_fitness_earned = RewardManager.MatchFitnessReward(GlobalSettings.GetCurrentGame().Name, score);
		if (this.MatchFitnessEarnedChanged != null)
		{
			this.MatchFitnessEarnedChanged();
		}
		if (m_match_fitness_earned >= 100 && m_match_fitness_earned < 1000)
		{
			TrophyUnlocked(Achievement.TROPHY.High_Roller);
		}
		else if (m_match_fitness_earned >= 1000)
		{
			TrophyUnlocked(Achievement.TROPHY.Superman);
		}
	}

	private void AwardCoinsForScore(int score)
	{
		coinRewardForScore = RewardManager.CoinReward(GlobalSettings.GetCurrentGame().Name, score);
		AddCoins(coinRewardForScore);
	}

	public static void BroadcastGameWonMedal(GameInfo gameInfo)
	{
		if (PlayerManager.GameWonNewMedal != null)
		{
			PlayerManager.GameWonNewMedal(gameInfo);
		}
	}

	private void UpdateUnlocks()
	{
		foreach (GameInfo game in _games)
		{
			BroadcastGameWonMedal(game);
		}
	}

	private void SetupTrophies()
	{
		for (int i = 0; i < Enum.GetValues(typeof(Achievement.TROPHY)).Length; i++)
		{
			Achievement item = new Achievement(Achievement.TYPE.TROPHY, (Achievement.TROPHY)i);
			_trophies.Add(item);
		}
	}

	public static Achievement GetTrophy(Achievement.TROPHY trophyType)
	{
		return m_instance._trophies[(int)trophyType];
	}

	public static void GameUnlocked(GameInfo gameInfo)
	{
		Achievement achievement = m_instance._gameUnlocks.Find((Achievement achv) => achv.CheckGameUnlocker(gameInfo.Type, gameInfo.Location));
		if (achievement != null)
		{
			achievement.Unlock();
			if (PlayerManager.NewAchievementUnlocked != null)
			{
				PlayerManager.NewAchievementUnlocked(achievement);
			}
		}
	}

	public static void PowerUpUnlocked(PowerUp powerUp)
	{
		Achievement achievement = m_instance._powerUpUnlocks.Find((Achievement ach) => ach.Name == powerUp.Name);
		if (achievement != null)
		{
			if (PlayerManager.NewAchievementUnlocked != null)
			{
				achievement.Unlock();
				PlayerManager.NewAchievementUnlocked(achievement);
			}
			if (m_instance.CheckPowerupAllUnlock())
			{
				TrophyUnlocked(Achievement.TROPHY.Charged);
			}
			else if (m_instance.CheckBlastPhaseAllUnlock())
			{
				m_instance.PowerUpWithType(POWER_UP.BLAST_BONUS).Unlock();
			}
		}
	}

	public static void PowerUpUpgraded(PowerUp powerUp)
	{
		Achievement achievement = m_instance._powerUpUpgrades.Find((Achievement ach) => ach.Name + ach.Value == powerUp.Name + powerUp.GetRoughIndex());
		if (achievement != null)
		{
			if (PlayerManager.NewAchievementUnlocked != null)
			{
				achievement.Unlock();
				PlayerManager.NewAchievementUnlocked(achievement);
			}
			if (m_instance.CheckPowerupAllGold())
			{
				TrophyUnlocked(Achievement.TROPHY.Super_Charged);
			}
			else if (m_instance.CheckBlastPhaseAllUpgradedForMedal((MEDAL)achievement.Value))
			{
				m_instance.PowerUpWithType(POWER_UP.BLAST_BONUS).SetUpgradeIndex(achievement.Value);
			}
		}
	}

	public static void TrophyUnlocked(Achievement.TROPHY trophyType)
	{
		Achievement achievement = null;
		Achievement achievement2 = m_instance._trophies.Find(delegate(Achievement ach)
		{
			achievement = ach;
			return ach.Name == trophyType.ToString();
		});
		if (PlayerManager.NewAchievementUnlocked != null && achievement2 != null && !achievement2.Unlocked)
		{
			achievement2.Unlock();
			PlayerManager.NewAchievementUnlocked(achievement2);
			GameCenterManager.PostAchievementComplete(Achievement.gameCenterIds[(int)trophyType]);
			string fBDescription = achievement.GetFBDescription(trophyType);
			if (FacebookHandler.IsSessionValid() && achievement != null && fBDescription != string.Empty)
			{
				FacebookHandler instance = MonoSingleton<FacebookHandler>.Instance;
				instance.StartCoroutine(instance.PostAchievement(fBDescription, achievement.Title, achievement.Name));
			}
		}
	}

	public static void FireDatedEvents()
	{
		if (PlayerManager.checkDatedEvent != null)
		{
			PlayerManager.checkDatedEvent(DateTime.Now);
		}
	}
}
