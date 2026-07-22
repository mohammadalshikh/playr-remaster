using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

[Serializable]
public abstract class MinigameChallenge
{
	public delegate void HandleBlastPhase(bool isEnabled);

	public delegate void HandleBlastPhaseUpdate(float currentStreak, int totalStreak);

	public delegate void HandleShieldChanged(int currentShield);

	public delegate void HandleRushRemaining(int scoreRemaining);

	public delegate void HandleRushCompleted(int rushBonus);

	protected const string JSON_TYPE_ID_WALL = "wall";

	protected const string JSON_TYPE_ID_BUCKET = "bucket";

	protected const string JSON_TYPE_ID_SHOOTING = "shooting";

	private const int MAX_MULTIPLIER = 9;

	protected MINIGAME_TYPE m_type;

	private Vector3 m_player_pos = Vector3.zero;

	private Vector3 m_player_direction = Vector3.right;

	public GAME_MODE m_gameMode;

	public DIFFICULTY m_current_difficulty;

	[SerializeField]
	protected GameData.GAME_TYPE _gameType;

	[SerializeField]
	protected GameData.GAME_LOCATION _gameLocation;

	public string m_name = "minigame";

	private string m_drill_name = "Default Drill";

	private string m_drill_description = "Drill description: Do the drill.";

	private Dictionary<string, object> m_json_dictionary;

	private bool m_first_time_execution = true;

	public Vector3 m_ball_player_offset;

	protected float m_energy_consumption;

	public DrillModifierValues m_drill_modifier_values = new DrillModifierValues();

	public int m_total_tries = 10;

	private int m_current_target = 100;

	protected int m_bronze_score = 100;

	protected int m_silver_score = 101;

	protected int m_gold_score = 102;

	public int m_time_limit;

	private int m_initial_time_limit;

	private bool m_time_limit_used;

	public int m_rush_max_score;

	public int m_rush_bonus_per_sec;

	private int m_rush_bonus;

	private int m_current_rush_score;

	public int m_lives = -1;

	private bool m_lives_used;

	private int m_score;

	private int m_plain_score;

	private int m_curl_score;

	private int m_streak_score;

	private AudioSource m_timer_audiosource;

	public bool m_stat_modifier;

	private int m_streak_count;

	private bool m_successful_attempt;

	private int m_streak_modifier_list_index;

	protected bool m_rounds_panel_setup;

	protected PackedSprite[] m_rounds_success_dots;

	public List<ScoreStreakModifier> m_streak_modifiers = new List<ScoreStreakModifier>();

	public float m_endShotDelay;

	protected int[] m_currentPowerUps;

	protected int m_current_multiplier = 1;

	protected int m_current_time_bonus;

	protected int m_current_shield;

	protected bool m_current_super_control;

	protected int m_super_control_index;

	public int m_starting_multiplier = 1;

	public int m_starting_shield;

	public int m_starting_time_bonus;

	protected bool m_is_end_screen_shown;

	private List<ScoreStreakModifier> m_current_modifiers = new List<ScoreStreakModifier>();

	private DrillSettings drillData;

	public DifficultyManager m_difficultyManager = new DifficultyManager();

	private bool m_isDifficultyBased;

	private float _blastPhaseCounter;

	protected float _blastPhaseDuration = 8f;

	private bool _isBlastPhaseOn;

	private int _blastPhaseMultiplier = 2;

	protected float _blastReduceRate = 0.05f;

	private float _blasPhaseGlobalTime;

	[SerializeField]
	private GridManager _obstacleGrid = new GridManager();

	public MINIGAME_TYPE Type => m_type;

	public GameData.GAME_TYPE GameType => _gameType;

	public GameData.GAME_LOCATION GameLocation => _gameLocation;

	public string Name
	{
		get
		{
			return m_name;
		}
		set
		{
			m_name = value;
		}
	}

	public string DrillName
	{
		get
		{
			return m_drill_name;
		}
		set
		{
			m_drill_name = value;
		}
	}

	public string DrillDescription
	{
		get
		{
			return m_drill_description;
		}
		set
		{
			m_drill_description = value;
		}
	}

	public Vector3 PlayerPosition
	{
		get
		{
			return m_player_pos;
		}
		set
		{
			m_player_pos = value;
		}
	}

	public Vector3 PlayerDirection
	{
		get
		{
			return m_player_direction;
		}
		set
		{
			m_player_direction = value;
		}
	}

	public DIFFICULTY CurrentDifficulty
	{
		get
		{
			return m_current_difficulty;
		}
		set
		{
			m_current_difficulty = value;
		}
	}

	public int TotalTries => m_total_tries;

	public bool ApplyStatModifier => m_stat_modifier;

	public bool GameStillActive => !m_time_limit_used || m_time_limit > 0;

	public bool UsingTimeLimit => m_time_limit_used;

	public bool UsingRushMode => m_rush_max_score > 0;

	public int RushMaxScore => m_rush_max_score;

	public bool RushScoreReached => m_score >= m_rush_max_score;

	public int RushBonus => m_rush_bonus;

	public bool UsingLives => m_lives_used;

	public float EndShotDelay => m_endShotDelay;

	public int CurrentMultiplier
	{
		get
		{
			return m_current_multiplier;
		}
		set
		{
			m_current_multiplier = value;
		}
	}

	public int CurrentShield
	{
		get
		{
			return m_current_shield;
		}
		set
		{
			m_current_shield = value;
		}
	}

	public int CurrentTimeBonus => m_current_time_bonus;

	public bool CurrentSuperControl => m_current_super_control;

	public int SuperControlIndex
	{
		get
		{
			return m_super_control_index;
		}
		set
		{
			m_super_control_index = value;
		}
	}

	public bool IsDifficultyBased => m_isDifficultyBased;

	public bool SuccesfulAttemp => m_successful_attempt;

	public abstract Vector3 TargetPosition { get; }

	public virtual float CameraFollowDistance => 5f;

	public virtual bool DollyEffectActive => true;

	public virtual Vector3 BallOffset => new Vector3(0f, 0f, -0.5f);

	public virtual float FlickBoosterFactor => 0.4f;

	public virtual float EndShotBallSpeed => 0.1f;

	public virtual float MaxFlickForCurl => (float)Screen.width * 0.15f;

	public virtual bool DestroyBall => true;

	public virtual bool SetPathPosition => true;

	public virtual string EndText => string.Empty + m_score;

	public virtual bool IsTargetOnGoal => false;

	public virtual bool LookTarget => true;

	public int Score => m_score;

	public int PlainScore => m_plain_score;

	public int CurlScore => m_curl_score;

	public int StreakScore => m_streak_score;

	public virtual int MaximumMultiplier => 9;

	public virtual bool UseDefaultStreakCounterIncrement => true;

	public virtual bool TimeRunningOutSound => false;

	public virtual bool StreakBonusSound => false;

	public int EnergyConsumption
	{
		get
		{
			return (int)m_energy_consumption;
		}
		set
		{
			m_energy_consumption = value;
		}
	}

	public bool IsEndScreenShown
	{
		get
		{
			return m_is_end_screen_shown;
		}
		set
		{
			m_is_end_screen_shown = value;
		}
	}

	protected virtual int BlastPhaseTrigger => 10;

	public bool IsBlastPhaseOn => _isBlastPhaseOn;

	public int BlastPhaseMultiplier
	{
		set
		{
			_blastPhaseMultiplier = value;
		}
	}

	public bool IsSuperControlAwarded => CurrentPowerUpForType(POWER_UP.SUPER_CONTROL) > 0;

	public virtual bool CheckStreakCounter => true;

	[method: MethodImpl(32)]
	public static event HandleBlastPhase BlastPhaseEnable;

	[method: MethodImpl(32)]
	public static event HandleBlastPhaseUpdate BlastPhaseChanged;

	[method: MethodImpl(32)]
	public static event HandleShieldChanged ShieldHasChanged;

	[method: MethodImpl(32)]
	public static event HandleRushRemaining RushRemaining;

	[method: MethodImpl(32)]
	public static event HandleRushCompleted RushModeCompleted;

	public static MinigameChallenge CreateMinigame(string id)
	{
		return id.ToLower() switch
		{
			"wall" => new MinigameWall(), 
			"bucket" => new MinigamePassing(), 
			"shooting" => new MinigameShooting(), 
			_ => null, 
		};
	}

	public virtual void ScenarioLoaded()
	{
	}

	public virtual void OnDisable()
	{
	}

	protected abstract void Start();

	public abstract void Finish();

	protected abstract void LoadParameter(string key, string value);

	protected abstract void WriteParameters(Dictionary<string, object> json_dictionary);

	protected virtual void InitializeParameters(Dictionary<string, object> json_dictionary)
	{
	}

	public virtual void FinalizeParameters()
	{
	}

	protected abstract void GenerateTarget();

	protected abstract void GenerateShootingChance();

	public abstract Vector3 GenerateShootingPosition();

	public virtual void FinalizeGameSettings()
	{
	}

	public abstract GameLevel CurrentLevel();

	public abstract bool TargetHit(Minigame_TargetTrigger trigger);

	public virtual void NextChance(float delay = 0f)
	{
		GameData.EndShotWithDelay(bForce: false, delay);
	}

	public virtual void FinishCurrentChance()
	{
	}

	public virtual int GetNumActiveTargets()
	{
		return 1;
	}

	public virtual void CustomTargetHit(MinigameTarget target)
	{
	}

	public virtual bool ResetPlayerCameraDirection()
	{
		return true;
	}

	public virtual void GoalScored(Minigame_GoalTrigger trigger)
	{
	}

	public virtual bool OnShootEnd()
	{
		return false;
	}

	public virtual bool MinigameEnd()
	{
		return false;
	}

	public abstract float BallShootingHeight(float flickLength);

	public abstract float BallShootingSpeed(float flickLength, float currentHeight);

	public abstract float MaxShootAngle();

	public virtual void BallKicked()
	{
	}

	public virtual void OnBallCollision(Collision collision, SoccerBall ball)
	{
	}

	public virtual void OnDrawGizmos(Minigame minigame)
	{
	}

	public virtual bool ReadyToEnd()
	{
		return true;
	}

	public virtual bool CustomTimeOutAction()
	{
		return false;
	}

	public bool FirstTimeExecution()
	{
		return m_first_time_execution;
	}

	public virtual void FixedUpdate()
	{
	}

	public virtual void Reset()
	{
		m_first_time_execution = false;
		if (CurrentLevel() != null)
		{
			CurrentLevel().Reset();
		}
		ResetRoundsPanel();
		DestroyRoundDots();
		ResetBlastPhase();
		m_current_shield = 0;
	}

	public void OnValidHit()
	{
		GameData.GUIManager.ShowBonus(0f);
		GenerateTarget();
	}

	public void ResetStreakCounter()
	{
		m_streak_count = 0;
		m_streak_modifier_list_index = 0;
	}

	public void AssignSetupDictionary(Dictionary<string, object> json_dictionary)
	{
		m_json_dictionary = json_dictionary;
	}

	public Dictionary<string, object> ExportMinigame()
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary.Add("game_mode", string.Empty + (int)m_gameMode);
		dictionary.Add("game_type", string.Empty + (int)_gameType);
		dictionary.Add("game_location", string.Empty + (int)_gameLocation);
		dictionary.Add("minigame_type", UppercaseFirst(GetJsonTypeID()));
		dictionary.Add("Name", m_name);
		dictionary.Add("ball_offset", string.Empty + m_ball_player_offset.x + "," + m_ball_player_offset.y + "," + m_ball_player_offset.z);
		dictionary.Add("n_tries_total", string.Empty + m_total_tries);
		dictionary.Add("time_limit", string.Empty + m_time_limit);
		dictionary.Add("rush_max_score_limit", string.Empty + m_rush_max_score);
		dictionary.Add("rush_bonus_per_sec", string.Empty + m_rush_bonus_per_sec);
		dictionary.Add("lives", string.Empty + m_lives);
		dictionary.Add("stat_modifier", string.Empty + (m_stat_modifier ? 1 : 0));
		dictionary.Add("endshot_delay", string.Empty + m_endShotDelay);
		dictionary.Add("difficulty_easy_levels", m_difficultyManager.easyLevels);
		dictionary.Add("difficulty_normal_levels", m_difficultyManager.normalLevels);
		dictionary.Add("difficulty_hard_levels", m_difficultyManager.hardLevels);
		dictionary.Add("difficulty_pro_levels", m_difficultyManager.proLevels);
		dictionary.Add("obstacle_grid_width", _obstacleGrid.Width);
		dictionary.Add("obstacle_grid_height", _obstacleGrid.Height);
		dictionary.Add("obstacle_grid_offset_x", string.Empty + _obstacleGrid.OffsetX);
		dictionary.Add("obstacle_grid_offset_y", string.Empty + _obstacleGrid.OffsetY);
		dictionary.Add("obstacle_origin", string.Empty + _obstacleGrid.Origin.x + "," + _obstacleGrid.Origin.y);
		if (m_streak_modifiers.Count > 0)
		{
			string text = string.Empty;
			string text2 = string.Empty;
			string text3 = string.Empty;
			string text4 = string.Empty + ';';
			foreach (ScoreStreakModifier streak_modifier in m_streak_modifiers)
			{
				text = text + text4 + streak_modifier.m_streak_number;
				text2 = text2 + text4 + streak_modifier.m_streak_points;
				string text5 = text3;
				text3 = text5 + text4 + streak_modifier.m_bonus_text_color.r + text4 + streak_modifier.m_bonus_text_color.g + text4 + streak_modifier.m_bonus_text_color.b;
			}
			dictionary.Add("streak_numbers", text.Remove(0, 1));
			dictionary.Add("streak_points", text2.Remove(0, 1));
			dictionary.Add("streak_colours", text3.Remove(0, 1));
		}
		WriteParameters(dictionary);
		return dictionary;
	}

	public void SetupMinigame()
	{
		if (m_json_dictionary == null)
		{
			Debug.LogError("No JSON setup dictionary found!");
			return;
		}
		m_total_tries = 1;
		m_time_limit = 0;
		m_initial_time_limit = 0;
		m_rush_max_score = 0;
		m_rush_bonus_per_sec = 0;
		m_lives = -1;
		m_rounds_panel_setup = false;
		m_current_multiplier = 1;
		m_stat_modifier = false;
		m_streak_modifiers = new List<ScoreStreakModifier>();
		m_endShotDelay = 0.5f;
		m_score = 0;
		m_plain_score = 0;
		m_curl_score = 0;
		m_streak_score = 0;
		m_difficultyManager = new DifficultyManager();
		InitializeParameters(m_json_dictionary);
		foreach (KeyValuePair<string, object> item in m_json_dictionary)
		{
			if (item.Value == null || item.Value.ToString().Equals(string.Empty))
			{
				continue;
			}
			string text = item.Key.ToLower();
			string text2 = item.Value.ToString();
			switch (text)
			{
			case "game_mode":
				m_gameMode = (GAME_MODE)int.Parse(text2);
				break;
			case "game_type":
				_gameType = (GameData.GAME_TYPE)int.Parse(text2);
				break;
			case "game_location":
				_gameLocation = (GameData.GAME_LOCATION)int.Parse(text2);
				break;
			case "name":
				m_name = text2;
				break;
			case "drill_name":
				m_drill_name = text2;
				break;
			case "drill_description":
				m_drill_description = text2;
				break;
			case "n_tries_total":
				m_total_tries = int.Parse(text2);
				break;
			case "time_limit":
				m_time_limit = ((int.Parse(text2) > 0) ? int.Parse(text2) : (-2));
				m_initial_time_limit = m_time_limit;
				break;
			case "rush_max_score_limit":
				m_rush_max_score = int.Parse(text2);
				break;
			case "rush_bonus_per_sec":
				m_rush_bonus_per_sec = int.Parse(text2);
				break;
			case "lives":
				m_lives = ((int.Parse(text2) > 0) ? int.Parse(text2) : (-1));
				break;
			case "stat_modifier":
				m_stat_modifier = ((int.Parse(text2) != 0) ? true : false);
				break;
			case "endshot_delay":
				float.TryParse(text2, out m_endShotDelay);
				break;
			case "difficulty_easy_levels":
				m_difficultyManager.easyLevels = int.Parse(text2);
				break;
			case "difficulty_normal_levels":
				m_difficultyManager.normalLevels = int.Parse(text2);
				break;
			case "difficulty_hard_levels":
				m_difficultyManager.hardLevels = int.Parse(text2);
				break;
			case "difficulty_pro_levels":
				m_difficultyManager.proLevels = int.Parse(text2);
				break;
			case "obstacle_grid_width":
				_obstacleGrid.Width = int.Parse(text2);
				break;
			case "obstacle_grid_height":
				_obstacleGrid.Height = int.Parse(text2);
				break;
			case "obstacle_grid_offset_x":
				_obstacleGrid.OffsetX = float.Parse(text2);
				break;
			case "obstacle_grid_offset_y":
				_obstacleGrid.OffsetY = float.Parse(text2);
				break;
			case "obstacle_origin":
			{
				string[] array5 = text2.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
				_obstacleGrid.Origin = new Vector2(float.Parse(array5[0]), float.Parse(array5[1]));
				break;
			}
			case "ball_offset":
			{
				string[] array4 = text2.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
				m_ball_player_offset = new Vector3(float.Parse(array4[0]), float.Parse(array4[1]), float.Parse(array4[2]));
				break;
			}
			case "streak_numbers":
			{
				string[] array3 = text2.Split(';');
				for (int num5 = array3.Length - m_streak_modifiers.Count; num5 > 0; num5--)
				{
					m_streak_modifiers.Add(new ScoreStreakModifier());
				}
				for (int num6 = array3.Length - 1; num6 >= 0; num6--)
				{
					m_streak_modifiers[num6].m_streak_number = int.Parse(array3[num6]);
				}
				break;
			}
			case "streak_points":
			{
				string[] array2 = text2.Split(';');
				for (int num3 = array2.Length - m_streak_modifiers.Count; num3 > 0; num3--)
				{
					m_streak_modifiers.Add(new ScoreStreakModifier());
				}
				for (int num4 = array2.Length - 1; num4 >= 0; num4--)
				{
					m_streak_modifiers[num4].m_streak_points = int.Parse(array2[num4]);
				}
				break;
			}
			case "streak_colours":
			{
				string[] array = text2.Split(';');
				int num = array.Length;
				if (num % 3 != 0)
				{
					Debug.LogError("Invalid number of colour arguments passed in, default used; please check the <streak_colors> key of your minigame");
					break;
				}
				for (int num2 = num / 3 - m_streak_modifiers.Count; num2 > 0; num2--)
				{
					m_streak_modifiers.Add(new ScoreStreakModifier());
				}
				Color black = Color.black;
				for (int i = 0; i < num; i += 3)
				{
					float.TryParse(array[i], out black.r);
					float.TryParse(array[i + 1], out black.g);
					float.TryParse(array[i + 2], out black.b);
					m_streak_modifiers[i / 3].m_bonus_text_color = black;
				}
				break;
			}
			default:
				LoadParameter(text, text2);
				break;
			}
		}
		if (m_gameMode == GAME_MODE.NORMAL)
		{
			m_current_difficulty = DIFFICULTY.NONE;
		}
		else
		{
			m_current_difficulty = GameData.CurrentDifficulty;
		}
		m_current_rush_score = m_rush_max_score;
		FinalizeParameters();
	}

	public virtual IEnumerator CustomIntro()
	{
		yield return true;
	}

	protected virtual void SetLighting()
	{
		RenderSettings.ambientLight = Color.gray;
	}

	public void StartMinigame()
	{
		Minigame minigameObject = GameData.MinigameObject;
		_obstacleGrid.Init();
		ObstacleManager.Instance.SetAvailablePositions(_obstacleGrid);
		drillData = GlobalSettings.GetCurrentGame();
		m_bronze_score = drillData.GetScoreForMedal(MEDAL.BRONZE);
		m_silver_score = drillData.GetScoreForMedal(MEDAL.SILVER);
		m_gold_score = drillData.GetScoreForMedal(MEDAL.GOLD);
		if (m_first_time_execution)
		{
			m_current_modifiers = m_streak_modifiers;
			m_current_modifiers.Sort(StreakModifierComparer);
		}
		SetupPowerUps();
		GameData.PlayerStats.ResetPostGameData();
		ObstacleManager.Instance.RemoveAll();
		m_current_time_bonus = ((m_starting_time_bonus <= 0) ? m_current_time_bonus : m_starting_time_bonus);
		m_isDifficultyBased = m_difficultyManager.IsEnabled();
		Start();
		SetScore(0);
		m_plain_score = 0;
		m_successful_attempt = false;
		CheckLivesSystem();
		_blasPhaseGlobalTime = PlayerPrefs.GetFloat("blast_time", 0f);
		MinigameGUI gUIManager = minigameObject.GUIManager;
		m_current_target = m_bronze_score;
		gUIManager.ScoreText = "0";
		string text = _gameType.ToString() + _gameLocation;
		if (!PlayerPrefs.HasKey(text))
		{
			PlayerManager.SendFeedback("First play " + text, string.Empty, string.Empty);
			PlayerPrefs.SetInt(text, 0);
		}
		GenerateTarget();
		SetLighting();
	}

	public void GenerateNewAttempt()
	{
		UpdateObstacles(m_successful_attempt);
		if (CheckStreakCounter)
		{
			if (!m_successful_attempt)
			{
				ResetStreakCounter();
				StopBlastPhase();
			}
			m_successful_attempt = false;
		}
		if (_blastPhaseCounter >= (float)BlastPhaseTrigger && !_isBlastPhaseOn)
		{
			StartBlastPhase();
		}
		GenerateShootingChance();
	}

	public virtual void GameEnded()
	{
		if (UsingRushMode)
		{
			m_score += m_rush_bonus;
		}
	}

	private void RushModeLogic()
	{
		if (UsingRushMode)
		{
			m_rush_bonus = RushCalculateBonus();
			if (RushScoreReached)
			{
				RushCompleted(m_rush_bonus);
			}
			else
			{
				RushRemainingChanged(m_rush_max_score - m_score);
			}
		}
	}

	protected virtual int RushCalculateBonus()
	{
		return Mathf.Clamp(m_time_limit * m_rush_bonus_per_sec, 0, m_initial_time_limit * m_rush_bonus_per_sec);
	}

	protected virtual void RushRemainingChanged(int scoreRemaining)
	{
		if (MinigameChallenge.RushRemaining != null)
		{
			MinigameChallenge.RushRemaining(scoreRemaining);
		}
		GameData.GUIManager.SetRushScore(m_current_rush_score, scoreRemaining);
		m_current_rush_score = scoreRemaining;
	}

	protected virtual void RushCompleted(int rushBonus)
	{
		if (MinigameChallenge.RushModeCompleted != null)
		{
			MinigameChallenge.RushModeCompleted(rushBonus);
		}
	}

	public virtual void BallFirstCollision(Collision collision)
	{
		switch (collision.gameObject.tag)
		{
		case "end_trigger":
			NextChance(0f);
			break;
		}
	}

	protected virtual void UpdateObstacles(bool bSucess)
	{
		if (!(ObstacleManager.Instance == null))
		{
			ObstacleManager.Instance.UpdateObstacles(bSucess);
			ObstacleManager.Instance.Reset();
		}
	}

	public virtual void CameraFollow()
	{
		GameData.GameCamera.SmoothFollow(GameData.Ball.CachedTransform, 2f, 0.5f, xzOnly: false);
	}

	public void SetScore(int new_score)
	{
		m_score = new_score;
		if (m_score >= m_current_target && m_current_target <= m_gold_score)
		{
			if (m_plain_score >= m_gold_score)
			{
				m_current_target = m_gold_score + 1;
			}
			else if (m_plain_score >= m_silver_score)
			{
				m_current_target = m_gold_score;
			}
			else if (m_plain_score >= m_bronze_score)
			{
				m_current_target = m_silver_score;
			}
		}
	}

	public void AddScore(int new_score, bool ApplyStatModBonuses, bool playSound = true)
	{
		new_score *= CurrentBlastPhaseMultiplier();
		MinigameGUI gUIManager = GameData.MinigameObject.GUIManager;
		MinigameGUI.CountUpText(gUIManager.ScoreSprite, m_plain_score, new_score + m_plain_score, AudioManager.Instance.audioCollection.CurrentGame.standardAudio.countUpHUD);
		m_plain_score += new_score;
		Minigame minigameObject = GameData.MinigameObject;
		if (ApplyStatModifier && ApplyStatModBonuses)
		{
			int num = Mathf.RoundToInt(minigameObject.CurlScoreModifier * (float)new_score);
			int num2 = num;
			if (num > 0)
			{
				GameData.GUIManager.SetCurlBonus(num);
				m_curl_score += num;
			}
			new_score += num2;
		}
		if (CheckStreakCounter)
		{
			if (!m_successful_attempt)
			{
				m_successful_attempt = true;
			}
			if (UseDefaultStreakCounterIncrement)
			{
				IncrementStreakCounter();
			}
		}
		SuccessHitsCounter();
		SetScore(m_score + new_score);
		RushModeLogic();
	}

	protected virtual void SuccessHitsCounter()
	{
		PlayerManager.IncrementHitCounter();
	}

	public void IncrementStreakCounter()
	{
		m_streak_count++;
		if (m_gameMode != GAME_MODE.TUTORIAL)
		{
			CheckStreakTrophies();
		}
		IncreaseBlastPhase();
		if (m_streak_modifier_list_index < m_current_modifiers.Count && m_current_modifiers[m_streak_modifier_list_index].m_streak_number == m_streak_count)
		{
			m_score += m_current_modifiers[m_streak_modifier_list_index].m_streak_points;
			GameData.MinigameObject.GUIManager.ScoreText = string.Empty + m_score;
			GameData.GUIManager.SetStreakBonus(m_current_modifiers[m_streak_modifier_list_index].m_streak_points, m_current_modifiers[m_streak_modifier_list_index].m_streak_number);
			m_streak_score += m_current_modifiers[m_streak_modifier_list_index].m_streak_points;
			m_streak_modifier_list_index++;
		}
	}

	private void CheckStreakTrophies()
	{
		switch (m_streak_count)
		{
		case 10:
			PlayerManager.TrophyUnlocked(Achievement.TROPHY.On_Target);
			break;
		case 20:
			PlayerManager.TrophyUnlocked(Achievement.TROPHY.Consistent);
			break;
		case 50:
			PlayerManager.TrophyUnlocked(Achievement.TROPHY.Bullseye);
			break;
		}
	}

	protected void FireUpBlastPhase()
	{
		_blastPhaseCounter = BlastPhaseTrigger;
		IncreaseBlastPhase();
		StartBlastPhase();
	}

	private void IncreaseBlastPhase()
	{
		_blastPhaseCounter += 1f;
		_blastPhaseCounter = Mathf.Clamp(_blastPhaseCounter, 0f, BlastPhaseTrigger);
		if (!_isBlastPhaseOn)
		{
			BlastPhaseHasChanged();
		}
	}

	private void ResetBlastPhase()
	{
		_blastPhaseCounter = 0f;
		BlastPhaseHasChanged();
		if (_isBlastPhaseOn)
		{
			_isBlastPhaseOn = false;
			_blasPhaseGlobalTime += GetBlastDuration();
			if (_blasPhaseGlobalTime >= 300f)
			{
				PlayerManager.TrophyUnlocked(Achievement.TROPHY.Inferno);
			}
			PlayerPrefs.SetFloat("blast_time", _blasPhaseGlobalTime);
		}
	}

	private void BlastPhaseHasChanged()
	{
		if (MinigameChallenge.BlastPhaseChanged != null)
		{
			MinigameChallenge.BlastPhaseChanged(_blastPhaseCounter, BlastPhaseTrigger);
		}
	}

	private void StartBlastPhase()
	{
		if (_isBlastPhaseOn)
		{
			return;
		}
		_isBlastPhaseOn = true;
		if (MinigameChallenge.BlastPhaseEnable != null)
		{
			MinigameChallenge.BlastPhaseEnable(isEnabled: true);
		}
		iTween.ValueTo(GameData.Instance.gameObject, iTween.Hash("name", string.Concat(GameData.Instance.gameObject, "Blast"), "from", 0f, "to", 1f, "time", GetBlastDuration(), "onUpdate", (Action<object>)delegate(object x)
		{
			if ((float)x == 1f)
			{
				_isBlastPhaseOn = false;
				StopBlastPhase();
			}
		}));
		if (_blasPhaseGlobalTime == 0f && m_gameMode != GAME_MODE.TUTORIAL)
		{
			PlayerManager.TrophyUnlocked(Achievement.TROPHY.On_Fire);
		}
	}

	protected void StopBlastPhase()
	{
		if (!_isBlastPhaseOn)
		{
			ResetBlastPhase();
			if (MinigameChallenge.BlastPhaseEnable != null)
			{
				MinigameChallenge.BlastPhaseEnable(isEnabled: false);
			}
		}
	}

	protected virtual float GetBlastDuration()
	{
		return _blastPhaseDuration + (float)CurrentPowerUpForType(POWER_UP.BLAST_BONUS);
	}

	protected virtual int CurrentBlastPhaseMultiplier()
	{
		if (!_isBlastPhaseOn)
		{
			return 1;
		}
		return _blastPhaseMultiplier;
	}

	public void IncrementMultiplier()
	{
		m_current_multiplier = Mathf.Clamp(m_current_multiplier + 1, 1, MaximumMultiplier);
		HandleStreakCounter();
		if (m_current_multiplier == 2)
		{
			GameData.GUIManager.ShowScorePanel(bShow: true, bMultiplierused: true);
		}
		GameData.GUIManager.SetMultiplierIcon(m_current_multiplier);
	}

	public virtual void LoseMultiplier()
	{
		if (m_current_multiplier > 1 && m_current_shield <= 0)
		{
			m_current_multiplier = 1;
			GameData.GUIManager.LoseMultiplier();
		}
	}

	public virtual void AwardShield()
	{
		if (m_current_shield < 4)
		{
			HandleStreakCounter();
			if (MinigameChallenge.ShieldHasChanged != null)
			{
				MinigameChallenge.ShieldHasChanged(m_current_shield);
			}
		}
	}

	public virtual void LoseShield()
	{
		if (m_current_shield != 0)
		{
			m_current_shield--;
			if (MinigameChallenge.ShieldHasChanged != null)
			{
				MinigameChallenge.ShieldHasChanged(m_current_shield);
			}
			AudioManager.PlaySFX(AudioManager.Instance.audioCollection.CurrentGame.standardAudio.lostShield, 0f);
		}
	}

	public virtual void BringShields()
	{
		AwardShield();
	}

	public void AddLife()
	{
		if (m_lives < 3)
		{
			m_lives++;
			GameData.MinigameObject.GUIManager.ChangeHeartState(m_lives - 1, full: true);
			GameData.MinigameObject.GUIManager.EarnLifeAnimation();
			HandleStreakCounter();
			AudioManager.PlaySFX(AudioManager.Instance.audioCollection.CurrentGame.standardAudio.addLife, 0f);
		}
	}

	public void LoseLife()
	{
		if ((CurrentLevel() == null || !CurrentLevel().IsRoundBased) && m_gameMode == GAME_MODE.NORMAL && UsingLives)
		{
			if (m_current_shield > 0)
			{
				LoseShield();
				return;
			}
			m_lives--;
			GameData.MinigameObject.GUIManager.ChangeHeartState(m_lives, full: false);
			GameData.MinigameObject.GUIManager.LoseLifeAnimation();
			AudioManager.PlaySFX(AudioManager.Instance.audioCollection.CurrentGame.standardAudio.lostLife, 0f);
		}
	}

	protected int GetBonusIndexFor(POWER_UP bonusType)
	{
		return CurrentPowerUpForType(bonusType);
	}

	public virtual float GetBonusProbabiltyForType(POWER_UP bonusType)
	{
		return GetBonusIndexFor(bonusType) switch
		{
			0 => 0f, 
			1 => 0.03f, 
			2 => 0.07f, 
			3 => 0.1f, 
			4 => 0.15f, 
			_ => 0.03f, 
		};
	}

	public virtual void AdditionalGameLevelBehaviours()
	{
		if (CurrentLevel() != null)
		{
			CurrentLevel().AdditionalBehaviours();
		}
	}

	public void AddTimeBonus(int time_bonus)
	{
		m_time_limit += time_bonus;
		if (UsingRushMode)
		{
			AddScore(time_bonus * m_rush_bonus_per_sec, ApplyStatModBonuses: false, playSound: false);
		}
		GameData.MinigameObject.GUIManager.AddTimeAnimation(time_bonus - 1);
		SetTimer();
		HandleStreakCounter();
		AudioManager.PlaySFX(AudioManager.Instance.audioCollection.CurrentGame.standardAudio.addTime, 0f);
	}

	private void HandleStreakCounter()
	{
		if (CheckStreakCounter && !m_successful_attempt)
		{
			m_successful_attempt = true;
			if (UseDefaultStreakCounterIncrement)
			{
				IncrementStreakCounter();
			}
		}
	}

	public void DisplayBonusAfterShot()
	{
	}

	protected virtual void DisplayInstructions(int index, bool isFinalRound)
	{
		if (CurrentLevel().CanDisplayIntroMessage)
		{
			string text = "Round " + (index + 1);
			if (isFinalRound)
			{
				text += " - Final Round";
			}
			GameData.GUIManager.DisplayInstructions(text, CurrentLevel()._settings.roundInformation, 0.5f, Color.yellow);
			GameData.GameCamera.BlurCamera();
			GameData.GUIManager.InstructionsEffects();
		}
	}

	public void CheckLivesSystem()
	{
		m_lives_used = m_lives >= 0;
		if (m_lives_used)
		{
			for (int i = 0; i < m_lives && i != 3; i++)
			{
				GameData.MinigameObject.GUIManager.ChangeHeartState(i, full: true);
			}
		}
	}

	public void CheckRoundsSystem()
	{
		GameLevel gameLevel = CurrentLevel();
		if (gameLevel == null)
		{
			return;
		}
		gameLevel.UpdateLevel();
		MinigameGUI gUIManager = GameData.MinigameObject.GUIManager;
		if (!gameLevel.IsRoundBased)
		{
			return;
		}
		m_total_tries += gameLevel._settings.rounds + 1;
		if (m_rounds_panel_setup)
		{
			gUIManager.SetRemainingShots(gameLevel.CurrentTries, gameLevel._settings.rounds);
			for (int i = 0; i < gameLevel._settings.successHits; i++)
			{
				if (i < gameLevel.CurrentHits)
				{
					m_rounds_success_dots[i].PlayAnim("active");
				}
			}
			return;
		}
		DestroyRoundDots();
		gUIManager.SetRemainingShots(gameLevel.CurrentTries, gameLevel._settings.rounds);
		m_rounds_success_dots = new PackedSprite[gameLevel._settings.successHits];
		for (int j = 0; j < gameLevel._settings.successHits; j++)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(gUIManager.CurrentSyle.standardHUDElements.roundDotPrefab) as GameObject;
			gameObject.gameObject.transform.parent = gUIManager.CurrentSyle.standardHUDElements.leftPanelShots.gameObject.transform;
			gameObject.transform.localPosition = new Vector3(0f + (float)j * 0.8f, -1.5f, 1f);
			m_rounds_success_dots[j] = gameObject.GetComponent<PackedSprite>();
		}
		m_rounds_panel_setup = true;
	}

	private void DestroyRoundDots()
	{
		if (m_rounds_success_dots == null || m_rounds_success_dots.Length <= 0)
		{
			return;
		}
		for (int i = 0; i < m_rounds_success_dots.Length; i++)
		{
			if (m_rounds_success_dots[i] != null)
			{
				UnityEngine.Object.Destroy(m_rounds_success_dots[i].gameObject);
			}
		}
	}

	protected void ResetRoundsPanel()
	{
		m_rounds_panel_setup = false;
	}

	protected int CurrentPowerUpForType(POWER_UP powerUpType)
	{
		return m_currentPowerUps[(int)powerUpType];
	}

	public virtual void SetupPowerUps()
	{
		m_currentPowerUps = new int[Enum.GetNames(typeof(POWER_UP)).Length];
		for (int i = 0; i < m_currentPowerUps.Length; i++)
		{
			if (GameData.PlayerStats.IsPowerUpAwarded((POWER_UP)i))
			{
				m_currentPowerUps[i] = GameData.PlayerStats.PowerUpWithType((POWER_UP)i).UpgradeIndex;
			}
			else
			{
				m_currentPowerUps[i] = 0;
			}
		}
		foreach (PowerUp powerUp in GameData.PlayerStats.PowerUps)
		{
			if (powerUp.Type == POWER_UP.SHIELD_BONUS && powerUp.IsAwarded)
			{
				int currentShield;
				if (powerUp.CurrentUpgradeIndex > 3)
				{
					int num2 = (CurrentShield = 3);
					currentShield = num2;
				}
				else
				{
					currentShield = powerUp.CurrentUpgradeIndex;
				}
				CurrentShield = currentShield;
			}
		}
		if (GameData.PlayerStats.IsPowerUpAwarded(POWER_UP.BLAST_BONUS))
		{
			FireUpBlastPhase();
		}
		if (GameData.PlayerStats.IsPowerUpAwarded(POWER_UP.TIME))
		{
			m_current_time_bonus = GameData.PlayerStats.CurrentTimePowerUp;
		}
		GameData.PlayerStats.ResetPowerUps();
	}

	public void CheckTimeSystem(bool bEnable)
	{
		m_time_limit_used = bEnable;
		if (bEnable)
		{
			SetTimer();
		}
	}

	public void SetTimer()
	{
		GameData.MinigameObject.GUIManager.TimerText = GameData.OutputClockFormat(m_time_limit);
	}

	public bool SecondHasPassed()
	{
		bool result = false;
		ConsumeEnergy();
		ConsumeBlastBar();
		if (m_time_limit_used)
		{
			if (m_time_limit >= 0 && GetNumActiveTargets() > 0)
			{
				SetTimer();
				m_time_limit--;
			}
			else if (m_time_limit < 0)
			{
				result = true;
			}
			if (m_time_limit < 10)
			{
				TimerSound(active: true);
				GameData.MinigameObject.GUIManager.SetTimerColor(lastSeconds: true);
			}
			else if (GameData.MinigameObject.GUIManager.IsTimerRed)
			{
				TimerSound(active: false);
				GameData.MinigameObject.GUIManager.SetTimerColor(lastSeconds: false);
			}
		}
		if (m_gameMode == GAME_MODE.NORMAL)
		{
			return result;
		}
		return false;
	}

	public void TimerSound(bool active)
	{
		if (!TimeRunningOutSound)
		{
			return;
		}
		if (active && m_time_limit >= 0)
		{
			if (m_timer_audiosource == null)
			{
				m_timer_audiosource = AudioManager.PlaySFX(AudioManager.Instance.audioCollection.CurrentGame.standardAudio.timerCountDown, 0f);
			}
			if (m_time_limit > 10 - Mathf.RoundToInt(m_timer_audiosource.time))
			{
				m_timer_audiosource.time = 9f - (float)m_time_limit;
			}
		}
		else if ((bool)m_timer_audiosource)
		{
			m_timer_audiosource = null;
			AudioManager.StopSound(AudioManager.Instance.audioCollection.CurrentGame.standardAudio.timerCountDown);
		}
	}

	protected virtual void ConsumeBlastBar()
	{
		if (!_isBlastPhaseOn && !(_blastPhaseCounter >= (float)BlastPhaseTrigger))
		{
			if (_blastPhaseCounter < 0f)
			{
				_blastPhaseCounter = 0f;
			}
			BlastPhaseHasChanged();
			_blastPhaseCounter -= _blastReduceRate;
		}
	}

	protected virtual void ConsumeEnergy()
	{
		m_energy_consumption += 1f;
	}

	public virtual void DisplayGUI()
	{
	}

	public virtual void BallSpeedUpDelegate()
	{
	}

	public virtual void BallSlowDownDelegate()
	{
	}

	public virtual float CurlAppliedWithDelta(Vector2 deltaFlick)
	{
		float from = 2f;
		float to = 10f;
		return Mathf.Lerp(from, to, Mathf.InverseLerp(0f, 100f, Mathf.Abs(deltaFlick.x)));
	}

	public bool WonMedal(MEDAL medal)
	{
		return medal switch
		{
			MEDAL.NONE => false, 
			MEDAL.BRONZE => (m_score >= m_bronze_score) ? true : false, 
			MEDAL.SILVER => (m_score >= m_silver_score) ? true : false, 
			MEDAL.GOLD => (m_score >= m_gold_score) ? true : false, 
			_ => false, 
		};
	}

	private static int StreakModifierComparer(ScoreStreakModifier a, ScoreStreakModifier b)
	{
		return (a.m_streak_number >= b.m_streak_number) ? 1 : (-1);
	}

	public void SaveSetupDictionary()
	{
		if (m_json_dictionary == null)
		{
			m_json_dictionary = ExportMinigame();
		}
		else
		{
			SetupMinigame();
		}
		m_initial_time_limit = m_time_limit;
		m_current_rush_score = m_rush_max_score;
		GameData.SelectedGame = m_name;
		GameData.SelectedGameType = _gameType;
		GameData.SelectedGameLocation = _gameLocation;
	}

	public string GetJsonTypeID()
	{
		return m_type switch
		{
			MINIGAME_TYPE.WALL => "wall", 
			MINIGAME_TYPE.PASSING => "bucket", 
			MINIGAME_TYPE.SHOOTING => "shooting", 
			_ => string.Empty, 
		};
	}

	private static string UppercaseFirst(string s)
	{
		if (string.IsNullOrEmpty(s))
		{
			return string.Empty;
		}
		return char.ToUpper(s[0]) + s.Substring(1);
	}
}
