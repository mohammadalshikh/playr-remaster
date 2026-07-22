using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using JsonFx.Json;
using UnityEngine;

public class Minigame : MonoBehaviour
{
	public delegate void HandleMinigameWillStart();

	public delegate void HandleMinigameReady();

	public delegate void HandleNewChanceWillStart();

	public delegate void HandleMinigameFinished();

	public delegate void HandleSuperControlChanged(float newRate);

	public const float CAMERA_FOLLOW_DIST_NORMAL = 6f;

	public const float CAMERA_FOLLOW_DIST_SUPER = 10f;

	public const float BIG_CURL_MINIMUM = 0.95f;

	public MinigameGUI minigameGUI;

	public SpinnerSequence m_spinnerManager;

	public GamesSourceFiles gamesSourceFiles;

	public MinigameData m_minigames_data;

	public MinigameTest m_test_minigame;

	private MinigameChallenge m_curr_minigame;

	private List<MinigameChallenge> m_minigames = new List<MinigameChallenge>();

	private GameObject m_curr_ball;

	private int m_current_try;

	private bool m_game_ended;

	private static bool m_next_chance_called;

	private int m_d_fps_counter;

	private float m_d_fps_last_time;

	private float m_d_fps_timer = 1f;

	private int m_current_time;

	private static bool m_superControlActive;

	public MinigameChallenge CurrentMinigame => m_curr_minigame;

	public WallMinigameData WallData => m_minigames_data.m_wall_data;

	public BucketMinigameData BucketData => m_minigames_data.m_bucketData;

	public ShootingMinigameData ShootingData => m_minigames_data.m_shootingData;

	public MinigameGUI GUIManager => minigameGUI;

	public SpinnerSequence SpinnerManager
	{
		get
		{
			return m_spinnerManager;
		}
		set
		{
			m_spinnerManager = value;
		}
	}

	public MinigameTest TestUnit => m_test_minigame;

	public int CurrentTry => m_current_try;

	public float GetCurlScoreModifier => (!(CurrentMinigame.m_drill_modifier_values.m_curl_score_modifier < 0f)) ? CurrentMinigame.m_drill_modifier_values.m_curl_score_modifier : 0.4f;

	public float CurlScoreModifier => GetCurlScoreModifier * GameData.PlayerControl.CurlApplied;

	public bool GameEnded => m_game_ended;

	public static bool NextChanceCalled
	{
		get
		{
			return m_next_chance_called;
		}
		set
		{
			m_next_chance_called = value;
		}
	}

	public static bool IsSuperControlActive
	{
		get
		{
			return m_superControlActive;
		}
		set
		{
			m_superControlActive = value;
		}
	}

	[method: MethodImpl(32)]
	public static event HandleMinigameWillStart MinigameWillStart;

	[method: MethodImpl(32)]
	public static event HandleMinigameReady MinigameReadyEvent;

	[method: MethodImpl(32)]
	public static event HandleNewChanceWillStart NewChanceWillStartEvent;

	[method: MethodImpl(32)]
	public static event HandleMinigameFinished MinigameFinished;

	[method: MethodImpl(32)]
	public static event HandleSuperControlChanged SuperControlChangedEvent;

	private void Awake()
	{
		GameData.CheckInstance();
		GameData.MinigameObject = this;
		GameData.EndShotFunction = ShootEnd;
		InitializeGameFromSource();
		SetupMinigame(GameData.MinigameData);
	}

	private void OnEnable()
	{
		TutorialView.WillShowEvent += TutorialWillShow;
		TutorialView.WillHideEvent += TutorialWillHide;
	}

	private void OnDisable()
	{
		MinigameChallenge.BlastPhaseEnable -= BlastPhaseEnabled;
		SoccerBall.BallFirstCollision -= BallFirstCollisionDelegate;
		QuitGame.WillQuitGame -= WillQuitGame;
		TutorialView.WillShowEvent -= TutorialWillShow;
		TutorialView.WillHideEvent -= TutorialWillHide;
		Reset();
	}

	private void Start()
	{
		m_curr_ball = GameData.Ball.gameObject;
		InitializeMinigame();
		MinigameChallenge.BlastPhaseEnable += BlastPhaseEnabled;
		SoccerBall.BallFirstCollision += BallFirstCollisionDelegate;
		QuitGame.WillQuitGame += WillQuitGame;
	}

	public void InitializeMinigame()
	{
		StartCoroutine(StartMinigame());
	}

	public void TryAgain()
	{
		Reset();
		SetupMinigame(GameData.MinigameData);
		InitializeMinigame();
	}

	private void WillQuitGame()
	{
		if (GameData.MinigameObject.CurrentMinigame != null)
		{
			GameData.GUIManager.ShowLoadingScreen();
			EndScreenManager.Instance.StopEndScreenEvent();
		}
	}

	public void Reset()
	{
		if (CurrentMinigame != null)
		{
			CurrentMinigame.Reset();
		}
		GUIManager.IsDisplayed = false;
		GameData.PauseTimeScale = 1f;
		if (GameData.Ball != null)
		{
			GameData.Ball.Reset();
		}
		m_game_ended = false;
		m_next_chance_called = false;
	}

	private void BallFirstCollisionDelegate(Collision collission)
	{
		SuperControlRate(1f);
		if (CurrentMinigame != null)
		{
			CurrentMinigame.BallFirstCollision(collission);
		}
	}

	private void Update()
	{
		TrackTime();
	}

	private void TrackTime()
	{
		float realtimeSinceStartup = Time.realtimeSinceStartup;
		m_d_fps_timer -= realtimeSinceStartup - m_d_fps_last_time;
		m_d_fps_last_time = realtimeSinceStartup;
		m_d_fps_counter++;
		if (m_d_fps_timer <= 0f)
		{
			m_current_time++;
			SecondHasPassed();
			m_d_fps_counter = 0;
			m_d_fps_timer += 1f;
			if (m_d_fps_timer <= 0f)
			{
				m_d_fps_timer = 1f;
			}
		}
	}

	private void FixedUpdate()
	{
		if (CurrentMinigame != null)
		{
			CurrentMinigame.FixedUpdate();
		}
	}

	private void InitializeGameFromSource()
	{
		if (!GameData.DebugMode)
		{
			GameData.MinigameData = gamesSourceFiles.GetFile(GameData.SelectedGame);
		}
	}

	private IEnumerator PlayVideo()
	{
		if (GameData.ShouldPlayVideo)
		{
			Screen.orientation = ScreenOrientation.LandscapeLeft;
			Handheld.PlayFullScreenMovie(CurrentMinigame.Name + ".mov", Color.black, FullScreenMovieControlMode.Hidden, FullScreenMovieScalingMode.AspectFit);
			yield return StartCoroutine(GameData.WaitSeconds(0.8f));
		}
		GameData.Instance.PauseGame(pause: false);
		yield return true;
	}

	private bool ImportMinigameFromJson(string json)
	{
		Debug.Log("importing from json = " + json);
		MinigameChallenge currentTest = TestUnit.CurrentTest;
		JsonReaderSettings jsonReaderSettings = new JsonReaderSettings();
		jsonReaderSettings.TypeHintName = "__type";
		jsonReaderSettings.AllowNullValueTypes = true;
		jsonReaderSettings.AllowUnquotedObjectKeys = true;
		JsonReader jsonReader = new JsonReader(json, jsonReaderSettings);
		object obj = jsonReader.Deserialize();
		Dictionary<string, object> dictionary = null;
		if (obj is Dictionary<string, object>)
		{
			dictionary = (Dictionary<string, object>)obj;
		}
		if (dictionary == null)
		{
			Debug.LogError("invalid result json string");
			return false;
		}
		object obj2 = null;
		foreach (KeyValuePair<string, object> item in dictionary)
		{
			if (item.Value == null || item.Key == "ItemKey" || !(item.Key == "ItemData"))
			{
				continue;
			}
			foreach (KeyValuePair<string, object> item2 in (Dictionary<string, object>)item.Value)
			{
				if (item2.Value == null || (string)item2.Value == string.Empty || !(item2.Key.ToLower() == "minigame_type"))
				{
					continue;
				}
				if (((string)item2.Value).ToLower() == currentTest.GetJsonTypeID())
				{
					obj2 = item.Value;
					break;
				}
				Debug.LogError("ABORTING... trying to import a <" + ((string)item2.Value).ToLower() + "> minigame into a <" + currentTest.GetJsonTypeID() + "> one");
				return false;
			}
		}
		if (obj2 != null)
		{
			Debug.Log("importing minigame...");
			currentTest.AssignSetupDictionary((Dictionary<string, object>)obj2);
			currentTest.SetupMinigame();
		}
		return true;
	}

	public void ExportToFile(string filename)
	{
		Debug.Log("=== EXPORT TO FILE " + filename + " ===");
		string path = Application.dataPath + "/GameSourceFiles/" + filename + ".txt";
		MinigameChallenge currentTest = TestUnit.CurrentTest;
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		Dictionary<string, object> value = currentTest.ExportMinigame();
		dictionary.Add("ItemData", value);
		FileStream fileStream = File.Create(path);
		string text = JsonWriter.Serialize(dictionary);
		Debug.Log("writing json = " + text);
		AddText(fileStream, text + "\r\n");
		fileStream.Close();
		Debug.Log("=== EXPORT TO FILE DONE ===");
	}

	public void ImportFromFile(string filename)
	{
		Debug.Log("=== IMPORT FROM FILE " + filename + " ===");
		StreamReader streamReader = new StreamReader(Application.dataPath + "/GameSourceFiles/" + filename + ".txt");
		string json = streamReader.ReadLine();
		streamReader.Close();
		if (ImportMinigameFromJson(json))
		{
			Debug.Log("=== IMPORT FROM FILE SUCCESS ===");
		}
		else
		{
			Debug.Log("=== IMPORT FROM FILE ERROR ===");
		}
	}

	public void SetupMinigame(string json)
	{
		if (json != null && json != string.Empty)
		{
			JsonReaderSettings jsonReaderSettings = new JsonReaderSettings();
			jsonReaderSettings.TypeHintName = "__type";
			jsonReaderSettings.AllowNullValueTypes = true;
			jsonReaderSettings.AllowUnquotedObjectKeys = true;
			JsonReader jsonReader = new JsonReader(json, jsonReaderSettings);
			Dictionary<string, object> dictionary = (Dictionary<string, object>)jsonReader.Deserialize();
			if (dictionary != null)
			{
				m_minigames.Clear();
				{
					foreach (KeyValuePair<string, object> item in dictionary)
					{
						if (item.Value == null || !(item.Value is Dictionary<string, object>))
						{
							continue;
						}
						MinigameChallenge minigameChallenge = null;
						Dictionary<string, object> dictionary2 = (Dictionary<string, object>)item.Value;
						foreach (KeyValuePair<string, object> item2 in dictionary2)
						{
							if (item2.Value != null && !item2.Value.ToString().Equals(string.Empty) && item2.Key.ToLower() == "minigame_type")
							{
								minigameChallenge = ((m_curr_minigame != null) ? m_curr_minigame : MinigameChallenge.CreateMinigame((string)item2.Value));
								if (minigameChallenge == null)
								{
									Debug.LogError("no minigame found with type = " + (string)item2.Value);
									break;
								}
							}
						}
						if (minigameChallenge != null)
						{
							minigameChallenge.Name = item.Key;
							minigameChallenge.AssignSetupDictionary(dictionary2);
							minigameChallenge.SetupMinigame();
							m_minigames.Add(minigameChallenge);
						}
					}
					return;
				}
			}
			Debug.LogError("invalid json string = " + json);
			base.enabled = false;
		}
		else
		{
			Debug.LogWarning("Minigame needs a json TextAsset, using editor values for the current minigame");
			m_test_minigame.CurrentTest.FinalizeParameters();
			m_test_minigame.CurrentTest.SaveSetupDictionary();
			m_minigames.Add(m_test_minigame.CurrentTest);
		}
	}

	private IEnumerator StartMinigame()
	{
		GameData.Instance.PauseGame(pause: true);
		MinigameGUI.GUIEnabled(bShow: false);
		PowerUpManager.Instance.Refresh();
		yield return StartCoroutine(GUIManager.PowerUpScreen());
		if (!StartNextMinigame())
		{
			Debug.LogError("there's no minigame to start");
			yield break;
		}
		GameData.LastGameMode = CurrentMinigame.m_gameMode;
		PlayerManager.SendFeedback("Started " + GameData.SelectedGame, string.Empty, string.Empty);
		if (Minigame.MinigameWillStart != null)
		{
			Minigame.MinigameWillStart();
		}
		GenerateShootingChance();
		yield return StartCoroutine(GUIManager.FireIntroEvent());
		GameData.Instance.PauseGame(pause: false);
		GUIManager.AnimateFadePlane(bShow: false);
		DisplayHUD();
		CurrentMinigame.FinalizeGameSettings();
		if (Minigame.MinigameReadyEvent != null)
		{
			Minigame.MinigameReadyEvent();
		}
	}

	private bool StartNextMinigame()
	{
		if (m_minigames.Count > 0)
		{
			if (m_curr_minigame != null)
			{
				m_curr_minigame.Finish();
			}
			m_curr_minigame = m_minigames[0];
			m_minigames.RemoveAt(0);
			m_current_try = 0;
			m_curr_minigame.StartMinigame();
			return true;
		}
		return false;
	}

	public void DisplayHUD()
	{
		if (GUIManager.IsDisplayed)
		{
			return;
		}
		MinigameGUI.GUIEnabled(bShow: true);
		GUIManager.ShowScorePanel(bShow: true, CurrentMinigame.CurrentMultiplier > 1);
		GUIManager.SetMultiplierIcon(CurrentMinigame.CurrentMultiplier, startup: true);
		if (CurrentMinigame.CurrentLevel() != null && CurrentMinigame.CurrentLevel().IsRoundBased)
		{
			GUIManager.ShowShotsPanel(bShow: true, CurrentMinigame.CurrentShield > 0);
		}
		else if (CurrentMinigame.m_time_limit <= 0 || CurrentMinigame.m_lives > 0)
		{
			GUIManager.ShowLifePanel(bShow: true, CurrentMinigame.CurrentShield > 0);
			CurrentMinigame.CheckLivesSystem();
		}
		if (CurrentMinigame.m_time_limit > 0)
		{
			CurrentMinigame.CheckTimeSystem(bEnable: true);
			GUIManager.ShowTimerPanel(bShow: true);
			if (CurrentMinigame.UsingRushMode)
			{
				GUIManager.RushScoreText = CurrentMinigame.RushMaxScore.ToString("#,##0");
				GUIManager.ShowRushPanel(bShow: true);
			}
		}
		if (CurrentMinigame.CurrentShield > 0)
		{
			CurrentMinigame.BringShields();
		}
		GUIManager.ShowGUIGameButtons(bShow: true);
		CurrentMinigame.DisplayGUI();
		GUIManager.IsDisplayed = true;
	}

	private void GenerateShootingChance()
	{
		if (m_curr_ball == null)
		{
			m_curr_ball.rigidbody.isKinematic = false;
		}
		m_curr_ball.SetActive(true);
		GameData.Ball.enabled = true;
		GameData.PlayerControl.Reset();
		GameData.Ball.Reset();
		m_next_chance_called = false;
		GameData.Ball.OnBooster += CurrentMinigame.BallSpeedUpDelegate;
		GameData.Ball.OnSlowDown += CurrentMinigame.BallSlowDownDelegate;
		Vector3 vector = CurrentMinigame.GenerateShootingPosition();
		vector.y = 0.6f;
		CurrentMinigame.AdditionalGameLevelBehaviours();
		Vector3 targetPosition = CurrentMinigame.TargetPosition;
		Vector3 normalized = (vector - targetPosition).normalized;
		normalized.y = 0f;
		GameData.Player.transform.position = new Vector3(vector.x, CurrentMinigame.m_ball_player_offset.y, vector.z) - CurrentMinigame.m_ball_player_offset.x * normalized - CurrentMinigame.m_ball_player_offset.z * Vector3.Cross(Vector3.up, normalized);
		m_curr_ball.transform.position = vector;
		GameData.Player.Reset();
		if (CurrentMinigame.LookTarget && CurrentMinigame.ResetPlayerCameraDirection())
		{
			GameData.GameCamera.LookAt(new Vector3(targetPosition.x, GameData.CameraTransform.position.y, targetPosition.z));
		}
		CurrentMinigame.GenerateNewAttempt();
		if (CurrentMinigame.m_gameMode != GAME_MODE.TUTORIAL)
		{
			PlayerManager.TrophyUnlocked(Achievement.TROPHY.Rookie);
		}
	}

	public void EndMinigame()
	{
		if (m_game_ended)
		{
			return;
		}
		m_game_ended = true;
		PlayerManager.GetCurrentGame().IncreaseTimesPlayed();
		CurrentMinigame.GameEnded();
		switch (CurrentMinigame.m_gameMode)
		{
		case GAME_MODE.NORMAL:
			GameData.PlayerStats.IncreaseGamesPlayed();
			GameData.PlayerStats.AddScore(m_curr_minigame.Name, m_curr_minigame.Score);
			CheckMedals();
			UnlockPowerUpsWithScore();
			GameData.PlayerStats.NextLevelPowerUp();
			if (!PlayerPrefs.HasKey("first_time"))
			{
				GameData.PlayerStats.FirstTimePlayed();
			}
			GameData.PlayerStats.SaveData();
			break;
		case GAME_MODE.TUTORIAL:
		{
			MinigameWall minigameWall = (MinigameWall)CurrentMinigame;
			GUIManager.ShowTutorialResult(minigameWall.CurrentTutorial().successMessage, Color.white, 0f, finalResult: true);
			Tutorial.SetTutorialPassed();
			break;
		}
		}
		if (Minigame.MinigameFinished != null)
		{
			Minigame.MinigameFinished();
		}
		GameData.LastGameMode = CurrentMinigame.m_gameMode;
		GameData.EndShot(bForce: true);
	}

	private static void AddText(FileStream fs, string value)
	{
		byte[] bytes = new UTF8Encoding(encoderShouldEmitUTF8Identifier: true).GetBytes(value);
		fs.Write(bytes, 0, bytes.Length);
	}

	public bool TargetHit(Minigame_TargetTrigger trigger)
	{
		if (!CurrentMinigame.TargetHit(trigger))
		{
			Debug.Log("invalid target hit");
			return false;
		}
		CurrentMinigame.OnValidHit();
		return true;
	}

	public void NextChance()
	{
		if (!m_next_chance_called)
		{
			m_next_chance_called = true;
			CurrentMinigame.NextChance(0f);
		}
	}

	public void FinishCurrentChance()
	{
		CurrentMinigame.FinishCurrentChance();
	}

	public void ShootEnd()
	{
		if (CurrentMinigame.OnShootEnd())
		{
			CurrentMinigame.OnValidHit();
		}
		m_current_try++;
		if (CurrentMinigame.MinigameEnd() || (!CurrentMinigame.UsingTimeLimit && !CurrentMinigame.UsingLives && m_current_try >= CurrentMinigame.TotalTries) || (CurrentMinigame.UsingTimeLimit && CurrentMinigame.m_time_limit < 0 && CurrentMinigame.ReadyToEnd() && CurrentMinigame.m_gameMode == GAME_MODE.NORMAL) || (CurrentMinigame.UsingLives && CurrentMinigame.m_lives <= 0) || (CurrentMinigame.UsingRushMode && CurrentMinigame.RushScoreReached))
		{
			EndMinigame();
		}
		else
		{
			SetNewChance();
		}
	}

	public void SetNewChance()
	{
		if (Minigame.NewChanceWillStartEvent != null)
		{
			Minigame.NewChanceWillStartEvent();
		}
		GenerateShootingChance();
	}

	public void SecondHasPassed()
	{
		if (CurrentMinigame != null && !GameData.IsPaused && CurrentMinigame.SecondHasPassed() && !GameData.PlayerControl.HasKicked())
		{
			EndMinigame();
		}
	}

	private void OnDrawGizmos()
	{
		if (CurrentMinigame != null)
		{
			CurrentMinigame.OnDrawGizmos(this);
		}
		m_test_minigame.CurrentTest.OnDrawGizmos(this);
	}

	private void BlastPhaseEnabled(bool bEnabled)
	{
		GameData.Ball.SetEffects(bEnabled ? SoccerBall.BALL_EFFECTS.FIREBALL : SoccerBall.BALL_EFFECTS.NONE);
	}

	private void CheckMedals()
	{
		if (GameData.PlayerStats.CurrentGameInfo() != null)
		{
			if (CurrentMinigame.WonMedal(MEDAL.GOLD))
			{
				GameData.PlayerStats.CurrentGameInfo().SetMedal(MEDAL.GOLD);
			}
			else if (CurrentMinigame.WonMedal(MEDAL.SILVER))
			{
				GameData.PlayerStats.CurrentGameInfo().SetMedal(MEDAL.SILVER);
			}
			else if (CurrentMinigame.WonMedal(MEDAL.BRONZE))
			{
				GameData.PlayerStats.CurrentGameInfo().SetMedal(MEDAL.BRONZE);
			}
		}
	}

	private void UnlockPowerUpsWithScore()
	{
		GameData.PlayerStats.CheckPowerUpUnlocks();
	}

	public static void BlockTouchInput(bool bBlock)
	{
		if (CR_TouchManager.IsInitialized)
		{
			CR_TouchManager.IsActive = !bBlock;
		}
	}

	public static void Pause(bool bPaused)
	{
		BlockTouchInput(bPaused);
		GameData.Instance.PauseGame(bPaused);
	}

	public static bool GameInfoExists()
	{
		return GameData.PlayerStats.CurrentGameInfo() != null;
	}

	public static void SuperControlRate(float rate)
	{
		GameData.MotionScale(rate);
		IsSuperControlActive = rate < 1f;
		if (Minigame.SuperControlChangedEvent != null)
		{
			Minigame.SuperControlChangedEvent(rate);
		}
	}

	private void TutorialWillShow()
	{
		BlockTouchInput(bBlock: true);
		Pause(bPaused: true);
	}

	private void TutorialWillHide()
	{
		BlockTouchInput(bBlock: false);
		Pause(bPaused: false);
	}
}
