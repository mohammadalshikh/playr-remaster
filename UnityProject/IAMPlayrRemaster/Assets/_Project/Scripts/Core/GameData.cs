using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using UnityEngine;

using UnityEngine.SceneManagement;
public class GameData : MonoBehaviour
{
	public enum GAME_LOCATION
	{
		TrainingCenter,
		MatchPractice,
		PassingDrills,
		ShootingDrills,
		ClubHouse
	}

	public enum GAME_TYPE
	{
		Countdown,
		Lifeline,
		Knockout,
		Rush,
		Tutorial
	}

	public class LeaderInfo
	{
		public long m_Score;

		public string m_Id;

		public string m_Name;

		public Texture2D m_Thumbnail;

		public bool m_ThumbnailLoaded;

		public GameObject m_GameObject;
	}

	public class Thumbnail
	{
		public string m_Id;

		private Texture m_Thumbnail;

		public void SetValues(string id, Texture thumbnail)
		{
			m_Id = id;
			m_Thumbnail = thumbnail;
		}

		public Texture GetTexture()
		{
			return m_Thumbnail;
		}
	}

	public class MessageInfo
	{
		public string fromname;

		public string id;

		public string messageId;

		public string message;

		public string data;

		public GameObject m_GameObject;

		public Texture2D m_Thumbnail;

		public bool m_ThumbnailLoaded;

		public bool m_Status;

		public void SetMessage(string sdata)
		{
			data = sdata;
			string text = fromname.Substring(0, fromname.LastIndexOf(' '));
			if (data == "REQUEST" || data == "REQ")
			{
				message = text + " needs energy! Help them!";
			}
			else if (data == "GIFT")
			{
				message = text + " has sent you some energy.";
			}
		}
	}

	public enum GENERIC_FRONTEND_TROPHIES
	{
		MILLIONAIRE,
		MASCOT,
		TOP_SPOT,
		ON_THE_PODIUM,
		CLIMBING_THE_LADDER,
		SYNCHED_IN,
		POSTMAN,
		BIG_DRAW,
		PAYING_IT_FORWARD,
		GIFTED,
		PUMPED
	}

	public class GenericFrontEndTrophies
	{
		public GENERIC_FRONTEND_TROPHIES m_GenericFrontEndTrophy;

		public string m_Description;

		public string m_Title;

		public string m_FBDescription;

		public void InitTrophy(GENERIC_FRONTEND_TROPHIES genericFrontEndTrophy)
		{
			m_GenericFrontEndTrophy = genericFrontEndTrophy;
			m_Description = sFrontEndTrophyDescription[(int)genericFrontEndTrophy];
			m_Title = genericFrontEndTrophy.ToString();
			m_FBDescription = sFrontEndTrophyDescriptionFB[(int)genericFrontEndTrophy];
		}

		public bool isUnlocked()
		{
			if (PlayerPrefs.HasKey(m_Title) && PlayerPrefs.GetInt(m_Title) > 0)
			{
				return true;
			}
			return false;
		}

		public void Unlock()
		{
			PlayerPrefs.SetInt(m_Title, 1);
			PlayerPrefs.Save();
		}
	}

	public delegate void EndGameFunc();

	private const float FIXED_DELTA_TIME = 0.01f;

	private const int APP_TARGET_FRAMERATE = 30;

	public const float FRAME_DELTA = 1f / 30f;

	public const char STRING_DATA_SPLIT_CHAR = ';';

	public const char LIST_DATA_SPLIT_CHAR = '|';

	public const float BALL_START_Y = 0.6f;

	public const string CURRENT_ENERGY_KEY = "currentEnergy";

	public const string CURRENT_RELOADING_TIME = "reloadTime";

	public const string DEVICE_REAL_TIME = "device_time";

	public const string ADDITIONAL_ENERGY_KEY = "additional_energy";

	private const float SLOW_MOTION_TIME_SCALE = 0.5f;

	public static string m_language = "EN";

	private static readonly string[][] _availableGameFileNames = new string[4][]
	{
		new string[5] { "countdown_wall", "lifeline_wall", "knockout_wall", "rush_wall", "tutorial_wall" },
		new string[0],
		new string[5]
		{
			"countdown_pass",
			"lifeline_pass",
			"knockout_pass",
			"rush_pass",
			string.Empty
		},
		new string[5]
		{
			"countdown_shoot",
			"lifeline_shoot",
			"knockout_shoot",
			"rush_shoot",
			string.Empty
		}
	};

	private static GameData m_Instance = null;

	private static Quaternion m_half_time_rot = Quaternion.AngleAxis(180f, Vector3.up);

	private bool b_debug_mode;

	private bool m_paused;

	private bool m_playing_video;

	private float m_paused_timeScale = 1f;

	private bool m_was_paused;

	private SoccerPlayer m_player;

	private GameObject m_main_camera;

	private CameraControl m_camera_control;

	private PlayerControlSystem m_player_control;

	private SoccerBall m_ball;

	private StadiumHandler m_stadium;

	private StadiumManager m_stadium_manager;

	private EndGameFunc m_end_game_function;

	private PlayerManager m_player_manager;

	private InAppPurchasing m_in_app_purchasing;

	private Minigame m_minigame;

	private MinigameGUI m_minigame_gui;

	private string m_minigame_data;

	private MINIGAME_TYPE m_current_type;

	private string m_selected_game_name = string.Empty;

	private bool m_minigame_play_video = true;

	private PathsController m_path_controller;

	public static string m_d_version = "v 2.2 Release";

	private string m_scenario;

	private int m_stadium_id;

	private bool m_home_match = true;

	private int m_rpfc_kit;

	private string m_gk_id = string.Empty;

	private bool m_second_half;

	private string m_player_name = "Carlos Revelo";

	private bool m_coming_from_game;

	private string m_video_to_play = string.Empty;

	private string m_previous_loaded_scene = string.Empty;

	private GAME_MODE m_last_game_mode;

	private DIFFICULTY m_current_difficulty;

	private GAME_LOCATION m_selected_location;

	private GAME_TYPE m_selected_type;

	private bool m_isCurrentApptDue;

	private GAME_LOCATION m_current_game_type;

	private Dictionary<GAME_LOCATION, bool> gameLocationsWhichHaveApptDue = new Dictionary<GAME_LOCATION, bool>();

	public static bool m_bMultiplierUnlocked = false;

	public static bool m_bTimeBonusUnlocked = false;

	public static bool m_bShieldUnlocked = false;

	public static bool m_bSuperControlUnlocked = false;

	public static string m_PreviousAchievement;

	public static string Scores = string.Empty;

	public static int m_PreviousScore = 0;

	public static int m_CurrentScore = -1;

	public static int m_CurrentPosition = -1;

	public static int m_PreviousPosition = -1;

	public static string facebook_userID = string.Empty;

	public static string m_PlayerName = string.Empty;

	public static Texture m_MyPicture = null;

	public static ArrayList m_Thumbnails = new ArrayList();

	public static ArrayList m_Leaders = new ArrayList();

	public static string PrevMessage = string.Empty;

	public static ArrayList m_MessageListArray = new ArrayList();

	public static string[] sFrontEndTrophyDescription = new string[11]
	{
		"Hit 1m points across all games", "Have 10 friends playing", "Made leaderboard number 1", "Made Top 3 in the leaderboard", "Beat a friend in the leaderboard", "Connect to Facebook", "Share something", "Invite a friend", "Send energy", "Receive energy",
		"Use a powerup"
	};

	public static string[] sFrontEndTrophyDescriptionFB = new string[11]
	{
		"<player> just scored over 1m points in Soccer Playr. Can you get close?",
		"Have 10 friends playing",
		"<player> is number 1 on the Soccer Playr leaderboard. Who's going to knock him off?",
		"<player> made top 3 in the Soccer Playr leaderboard!",
		string.Empty,
		string.Empty,
		string.Empty,
		string.Empty,
		string.Empty,
		string.Empty,
		string.Empty
	};

	public static int NUM_TROPHIES = 11;

	public static ArrayList m_GenericFrontEndTrophies = new ArrayList();

	private bool firstEverBoot = true;

	private bool hasIntroSeqOccured;

	private bool hasIntroSceneFinished;

	private Dictionary<int, GAME_LOCATION> mapUpgrades = new Dictionary<int, GAME_LOCATION>();

	public static GameData Instance => m_Instance;

	public static string Scenario => Instance.m_scenario;

	public static bool DebugMode => Instance.b_debug_mode;

	public static int StadiumID
	{
		get
		{
			return Instance.m_stadium_id;
		}
		set
		{
			Instance.m_stadium_id = value;
		}
	}

	public static string PlayerName => Instance.m_player_name;

	public static string GoalkeeperID => Instance.m_gk_id;

	public static SoccerPlayer Player
	{
		get
		{
			return Instance.m_player;
		}
		set
		{
			Instance.m_player = value;
		}
	}

	public static CameraControl GameCamera
	{
		get
		{
			return Instance.m_camera_control;
		}
		set
		{
			Instance.m_camera_control = value;
		}
	}

	public static Transform CameraTransform => (!(GameCamera != null)) ? Camera.main.transform : GameCamera.CameraTransform;

	public static PlayerControlSystem PlayerControl
	{
		get
		{
			return Instance.m_player_control;
		}
		set
		{
			Instance.m_player_control = value;
		}
	}

	public static SoccerBall Ball
	{
		get
		{
			return Instance.m_ball;
		}
		set
		{
			Instance.m_ball = value;
		}
	}

	public static StadiumHandler Stadium
	{
		get
		{
			return Instance.m_stadium;
		}
		set
		{
			Instance.m_stadium = value;
		}
	}

	public static StadiumManager StadiumObject
	{
		get
		{
			return Instance.m_stadium_manager;
		}
		set
		{
			Instance.m_stadium_manager = value;
		}
	}

	public static PlayerManager PlayerStats
	{
		get
		{
			return Instance.m_player_manager;
		}
		set
		{
			Instance.m_player_manager = value;
		}
	}

	public static InAppPurchasing StoreManager
	{
		get
		{
			return Instance.m_in_app_purchasing;
		}
		set
		{
			Instance.m_in_app_purchasing = value;
		}
	}

	public static Minigame MinigameObject
	{
		get
		{
			return Instance.m_minigame;
		}
		set
		{
			Instance.m_minigame = value;
		}
	}

	public static MinigameGUI GUIManager
	{
		get
		{
			return Instance.m_minigame_gui;
		}
		set
		{
			Instance.m_minigame_gui = value;
		}
	}

	public static string MinigameData
	{
		get
		{
			return Instance.m_minigame_data;
		}
		set
		{
			Instance.m_minigame_data = value;
		}
	}

	public static GAME_LOCATION SelectedGameLocation
	{
		get
		{
			return Instance.m_selected_location;
		}
		set
		{
			Instance.m_selected_location = value;
		}
	}

	public static GAME_TYPE SelectedGameType
	{
		get
		{
			return Instance.m_selected_type;
		}
		set
		{
			Instance.m_selected_type = value;
		}
	}

	public static string SelectedGame
	{
		get
		{
			return Instance.m_selected_game_name;
		}
		set
		{
			Instance.m_selected_game_name = value;
		}
	}

	public static string SelectedGameLocationFormatted => FormattedGameLocation(SelectedGameLocation);

	public static MINIGAME_TYPE CurrentMinigameType
	{
		get
		{
			return Instance.m_current_type;
		}
		set
		{
			Instance.m_current_type = value;
		}
	}

	public static bool IsSecondHalf => (bool)Instance && Instance.m_second_half;

	public static bool IsHomeMatch => Instance.m_home_match;

	public static string RpfcKitAssetName => (Instance.m_rpfc_kit != 0) ? "Away" : "Home";

	public static bool IsPaused => Instance.m_paused;

	public static bool WasPaused => Instance.m_was_paused;

	public static float PauseTimeScale
	{
		set
		{
			Instance.m_paused_timeScale = value;
		}
	}

	public static bool IsPlayingVideo
	{
		get
		{
			return Instance.m_playing_video;
		}
		set
		{
			Instance.m_playing_video = value;
		}
	}

	public static EndGameFunc EndShotFunction
	{
		get
		{
			return Instance.m_end_game_function;
		}
		set
		{
			Instance.m_end_game_function = value;
		}
	}

	public static bool ShouldPlayVideo
	{
		get
		{
			return Instance.m_minigame_play_video;
		}
		set
		{
			Instance.m_minigame_play_video = value;
		}
	}

	public static bool IsComingFromGame
	{
		get
		{
			return Instance.m_coming_from_game;
		}
		set
		{
			Instance.m_coming_from_game = value;
		}
	}

	public static string VideoToPlay
	{
		get
		{
			return Instance.m_video_to_play;
		}
		set
		{
			Instance.m_video_to_play = value;
		}
	}

	public static string PreviousLevel => Instance.m_previous_loaded_scene;

	public static GAME_MODE LastGameMode
	{
		get
		{
			return Instance.m_last_game_mode;
		}
		set
		{
			Instance.m_last_game_mode = value;
		}
	}

	public static PathsController PathManager
	{
		get
		{
			return Instance.m_path_controller;
		}
		set
		{
			Instance.m_path_controller = value;
		}
	}

	public static DIFFICULTY CurrentDifficulty
	{
		get
		{
			return Instance.m_current_difficulty;
		}
		set
		{
			Instance.m_current_difficulty = value;
		}
	}

	public bool IsCurrentApptDue
	{
		get
		{
			return m_isCurrentApptDue;
		}
		set
		{
			m_isCurrentApptDue = value;
		}
	}

	public static float GlobalVolume
	{
		get
		{
			return AudioListener.volume;
		}
		set
		{
			AudioListener.volume = value;
		}
	}

	public bool FirstEverBoot
	{
		get
		{
			return firstEverBoot;
		}
		set
		{
			firstEverBoot = value;
		}
	}

	public bool HasIntroSeqOccured
	{
		get
		{
			return hasIntroSeqOccured;
		}
		set
		{
			hasIntroSeqOccured = value;
		}
	}

	public bool HasIntroSceneFinished
	{
		get
		{
			return hasIntroSceneFinished;
		}
		set
		{
			hasIntroSceneFinished = value;
		}
	}

	public Dictionary<int, GAME_LOCATION> MapUpgrades => mapUpgrades;

	public static bool BeatFriend => m_PreviousPosition > m_CurrentPosition && m_CurrentPosition < m_Leaders.Count && m_CurrentPosition >= 1;

	public static string GetGameName(GAME_LOCATION gameLocation, GAME_TYPE gameType)
	{
		return _availableGameFileNames[(int)gameLocation][(int)gameType];
	}

	public bool DoesGameLocationHaveApptDue(GAME_LOCATION gameLocation)
	{
		foreach (KeyValuePair<GAME_LOCATION, bool> item in gameLocationsWhichHaveApptDue)
		{
			if (item.Key == gameLocation && item.Value)
			{
				return true;
			}
		}
		return false;
	}

	public void AddGameLocationHasApptDue(GAME_LOCATION gameLocation, bool hasApptDue)
	{
		if (!gameLocationsWhichHaveApptDue.ContainsKey(gameLocation))
		{
			gameLocationsWhichHaveApptDue.Add(gameLocation, hasApptDue);
		}
	}

	public void RemoveGameLocationHasApptDue(GAME_LOCATION gameLocation)
	{
		if (gameLocationsWhichHaveApptDue.ContainsKey(gameLocation))
		{
			gameLocationsWhichHaveApptDue.Remove(gameLocation);
		}
	}

	public static void SetPosition(int position)
	{
		m_PreviousPosition = m_CurrentPosition;
		m_CurrentPosition = position;
	}

	public static int isPresent(string id)
	{
		int num = 0;
		foreach (Thumbnail thumbnail in m_Thumbnails)
		{
			if (thumbnail.m_Id.Equals(id))
			{
				return num;
			}
			num++;
		}
		return -1;
	}

	public static Texture getThumbnail(string id)
	{
		int num = isPresent(id);
		if (num != -1)
		{
			Thumbnail thumbnail = (Thumbnail)m_Thumbnails[num];
			return thumbnail.GetTexture();
		}
		return null;
	}

	public static void AddThumbnail(Thumbnail thumbnail)
	{
		int num = isPresent(thumbnail.m_Id);
		if (num == -1)
		{
			m_Thumbnails.Add(thumbnail);
			return;
		}
		m_Thumbnails.RemoveAt(num);
		m_Thumbnails.Insert(num, thumbnail);
	}

	public static int isPresentInLeaderBoard(string userId)
	{
		int num = 0;
		foreach (LeaderInfo leader in m_Leaders)
		{
			if (leader.m_Id == userId)
			{
				return num;
			}
			num++;
		}
		return -1;
	}

	public static bool isNewMessage(string messageId)
	{
		string @string = PlayerPrefs.GetString("MessageIds", string.Empty);
		if (@string.Contains(messageId))
		{
			return false;
		}
		@string = @string + " " + messageId;
		PlayerPrefs.SetString("MessageIds", @string);
		PlayerPrefs.Save();
		return true;
	}

	public static void AddToDeleted(string messageId)
	{
		string @string = PlayerPrefs.GetString("DeletedMessages", string.Empty);
		@string = @string + " " + messageId;
		PlayerPrefs.SetString("DeletedMessages", @string);
		PlayerPrefs.Save();
	}

	public static bool isDeletedMessage(string messageId)
	{
		string @string = PlayerPrefs.GetString("DeletedMessages", string.Empty);
		if (@string.Contains(messageId))
		{
			return true;
		}
		return false;
	}

	public static bool isPresentInInbox(string messageId)
	{
		int num = 0;
		foreach (MessageInfo item in m_MessageListArray)
		{
			if (item.messageId.Equals(messageId))
			{
				return true;
			}
			num++;
		}
		return false;
	}

	public static bool isTrophyUnlocked(GENERIC_FRONTEND_TROPHIES trophytounlock)
	{
		GenericFrontEndTrophies genericFrontEndTrophies = m_GenericFrontEndTrophies[(int)trophytounlock] as GenericFrontEndTrophies;
		return genericFrontEndTrophies.isUnlocked();
	}

	public static GenericFrontEndTrophies GetTrophy(GENERIC_FRONTEND_TROPHIES trophytounlock)
	{
		return m_GenericFrontEndTrophies[(int)trophytounlock] as GenericFrontEndTrophies;
	}

	public static void UnlockTrophy(GENERIC_FRONTEND_TROPHIES trophytounlock)
	{
		GenericFrontEndTrophies genericFrontEndTrophies = m_GenericFrontEndTrophies[(int)trophytounlock] as GenericFrontEndTrophies;
		genericFrontEndTrophies.Unlock();
	}

	public static List<Trophy> GetAllTrophies()
	{
		List<Trophy> list = new List<Trophy>();
		foreach (GenericFrontEndTrophies genericFrontEndTrophy in m_GenericFrontEndTrophies)
		{
			Trophy trophy = new Trophy();
			if (genericFrontEndTrophy.isUnlocked())
			{
				trophy.UnLock();
			}
			trophy.Type = genericFrontEndTrophy.m_Title;
			trophy.Description = genericFrontEndTrophy.m_Description;
			trophy.TrophyIndex = (int)genericFrontEndTrophy.m_GenericFrontEndTrophy;
			trophy.FBDescription = genericFrontEndTrophy.m_FBDescription;
			list.Add(trophy);
		}
		return list;
	}

	private void Awake()
	{
		if (Instance != null)
		{
			Debug.LogWarning("only one GameData is allowed, destroying " + base.gameObject.name);
			UnityEngine.Object.Destroy(base.gameObject);
			return;
		}
		m_Instance = this;
		Application.targetFrameRate = 30;
		PlayerStats = base.gameObject.AddComponent<PlayerManager>();
		UnityEngine.Object.DontDestroyOnLoad(Instance);
		Time.fixedDeltaTime = 0.01f;
		Time.timeScale = 1f;
		if (SceneManager.GetActiveScene().buildIndex == 0)
		{
			StartCoroutine(LoadLevel(SCENES.MAP));
		}
		else if (SceneManager.GetActiveScene().buildIndex == 3)
		{
			b_debug_mode = true;
		}
		for (int i = 0; i < NUM_TROPHIES; i++)
		{
			GenericFrontEndTrophies genericFrontEndTrophies = new GenericFrontEndTrophies();
			genericFrontEndTrophies.InitTrophy((GENERIC_FRONTEND_TROPHIES)i);
			m_GenericFrontEndTrophies.Add(genericFrontEndTrophies);
		}
	}

	private void Start()
	{
		EtceteraAndroidManager.alertButtonClickedEvent += delegate(string message)
		{
			if (message == "Quit")
			{
				Application.Quit();
			}
		};
	}

	public static void CheckInstance()
	{
		if (!Instance)
		{
			Debug.Log("creating GameData instance");
			GameObject gameObject = new GameObject("GameData");
			gameObject.AddComponent<GameData>();
		}
	}

	private void Update()
	{
		if (!Input.GetKeyDown(KeyCode.Escape))
		{
			return;
		}
		switch (SceneManager.GetActiveScene().buildIndex)
		{
		case 1:
			EtceteraAndroid.showAlert("I AM PLAYR", "Do you really want to quit?", "No", "Quit");
			break;
		case 2:
			MonoSingleton<GameTypeMenuManager>.Instance.GameTypePanel.SaveAppointments();
			StartCoroutine(LoadPreviousLevel());
			break;
		case 3:
			if (MinigameObject.CurrentMinigame != null)
			{
				Minigame.Pause(bPaused: true);
				MinigameObject.GUIManager.ShowPauseScreen(bShow: true);
			}
			break;
		}
	}

	public void PauseGame(bool pause)
	{
		if (pause)
		{
			if (!IsPaused)
			{
				m_paused_timeScale = Time.timeScale;
				SetTimeScale(0f, bForce: true);
				m_paused = true;
			}
		}
		else if (IsPaused)
		{
			SetTimeScale(m_paused_timeScale, bForce: true);
			m_paused = false;
		}
	}

	private void OnApplicationQuit()
	{
		PlayerStats.SaveData();
	}

	private void OnApplicationPause(bool pause)
	{
		if (pause)
		{
			LocalNotifications.ScheduleAllNotifactions();
			PlayerStats.SaveData();
			if (MinigameObject != null && !ShouldPlayVideo)
			{
				Minigame.Pause(pause);
			}
			FlurryAndroid.onEndSession();
		}
		else
		{
			FlurryAndroid.onStartSession("XXG4GHKPM2H2SG7C7JT3");
			if (MinigameObject != null && !ShouldPlayVideo && !MinigameObject.CurrentMinigame.IsEndScreenShown)
			{
				MinigameObject.GUIManager.ShowPauseScreen(bShow: true);
			}
		}
	}

	public static string OutputClockFormat(int timeInSeconds)
	{
		int num = timeInSeconds / 60;
		int num2 = Mathf.Clamp(timeInSeconds % 60, 0, 59);
		string text = ((num2 >= 10) ? (string.Empty + num2) : ("0" + num2));
		return string.Empty + num + ":" + text;
	}

	public IEnumerator LoadLevel(SCENES scene)
	{
		PlayerPrefs.Save();
		m_previous_loaded_scene = SceneManager.GetActiveScene().name;
		if (SceneManager.GetActiveScene().buildIndex != 3)
		{
			m_coming_from_game = false;
		}
		SceneManager.LoadScene((int)scene);
		Instance.PauseGame(pause: false);
		yield return true;
	}

	public IEnumerator LoadPreviousLevel()
	{
		PlayerPrefs.Save();
		m_previous_loaded_scene = SceneManager.GetActiveScene().name;
		if (SceneManager.GetActiveScene().buildIndex != 3)
		{
			m_coming_from_game = false;
		}
		if (SceneManager.GetActiveScene().buildIndex - 1 > 0)
		{
			SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
		}
		Instance.PauseGame(pause: false);
		yield return true;
	}

	public void LoadGame(GAME_LOCATION gameLocation, GAME_TYPE gameType)
	{
		m_selected_location = gameLocation;
		m_selected_type = gameType;
		m_selected_game_name = GetGameName(m_selected_location, m_selected_type);
		StartCoroutine(LoadLevel(SCENES.GAME_LEVEL));
	}

	public static void EndShot(bool bForce)
	{
		if (!bForce && EndShotFunction != null)
		{
			EndShotFunction();
		}
		else
		{
			EndScreenManager.Instance.StartEndScreenEvent();
		}
	}

	public static void EndShotWithDelay(bool bForce, float delay)
	{
		m_Instance.StartCoroutine(m_Instance.EndShot(bForce, delay));
	}

	public IEnumerator EndShot(bool bForce, float delay)
	{
		yield return new WaitForSeconds(delay);
		EndShot(bForce);
	}

	public IEnumerator QuitGame(SCENES scene)
	{
		Reset();
		PauseGame(pause: false);
		StartCoroutine(LoadLevel(scene));
		yield return true;
	}

	public static void Reset()
	{
		SetTimeScale(1f, bForce: false);
	}

	public static bool SetTimeScale(float timeScale, bool bForce)
	{
		if (timeScale == Time.timeScale)
		{
			return true;
		}
		if (!bForce && IsPaused && IsPlayingVideo)
		{
			return false;
		}
		Time.timeScale = timeScale;
		Time.fixedDeltaTime = 0.01f * timeScale;
		return true;
	}

	public IEnumerator EnableSlowMotion(bool bEnable)
	{
		if (bEnable)
		{
			while (!SetTimeScale(0.5f, bForce: false))
			{
				yield return false;
			}
		}
		else
		{
			while (!SetTimeScale(1f, bForce: false))
			{
				yield return false;
			}
		}
		yield return true;
	}

	public static void MotionScale(float motionScale)
	{
		motionScale = Mathf.Clamp01(motionScale);
		SetTimeScale(motionScale, bForce: false);
	}

	public static Vector3 HalfTimeRotation(Vector3 tgt)
	{
		if (IsSecondHalf)
		{
			tgt = m_half_time_rot * tgt;
		}
		return tgt;
	}

	public static void HalfTimeRotation(Transform tgt)
	{
		if (IsSecondHalf)
		{
			Vector3 eulerAngles = tgt.eulerAngles;
			tgt.eulerAngles = new Vector3(0f, eulerAngles.y, 0f);
			tgt.position = m_half_time_rot * tgt.position;
			tgt.rotation *= m_half_time_rot;
			tgt.eulerAngles = new Vector3(eulerAngles.x, tgt.eulerAngles.y, eulerAngles.z);
		}
	}

	public static Hashtable Hash(params object[] args)
	{
		Hashtable hashtable = new Hashtable(args.Length / 2);
		if (args.Length % 2 != 0)
		{
			Debug.LogError("Hash requires an even number of arguments!");
			return null;
		}
		for (int i = 0; i < args.Length - 1; i += 2)
		{
			hashtable.Add(args[i], args[i + 1]);
		}
		return hashtable;
	}

	public static IEnumerator WaitSeconds(float seconds)
	{
		float currentTime = 0f;
		while (currentTime < seconds)
		{
			currentTime += 1f / 30f;
			yield return null;
		}
	}

	public static void PlayVideoForGame(string videoName)
	{
		VideoToPlay = videoName;
	}

	public static void CallCoroutineWithName(IEnumerator eventPassed)
	{
		Instance.StartCoroutine(eventPassed);
	}

	public static string CapitalFirstLetter(string lowerCaseString)
	{
		if (lowerCaseString == string.Empty)
		{
			return string.Empty;
		}
		return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(lowerCaseString.ToLower());
	}

	public static bool ProbabilityTest(float probabilty)
	{
		if (probabilty == 0f)
		{
			return false;
		}
		float num = Mathf.Clamp01(probabilty);
		return num >= UnityEngine.Random.value;
	}

	public static string FormattedGameLocation(GAME_LOCATION gameLocation)
	{
		return Regex.Replace(gameLocation.ToString(), "(\\B[A-Z])", " $1");
	}

	public static string GetProperNameForLocation(GAME_LOCATION gameLocation)
	{
		return gameLocation switch
		{
			GAME_LOCATION.MatchPractice => "Stadium", 
			GAME_LOCATION.PassingDrills => "Training Pitch", 
			GAME_LOCATION.ShootingDrills => "Training Pitch", 
			_ => FormattedGameLocation(gameLocation), 
		};
	}

	public static string FormatScore(int score)
	{
		if (score >= 1000 && score < 1000000)
		{
			return score / 1000 + "K";
		}
		if (score >= 1000000)
		{
			return (float)score / 1000000f + "M";
		}
		return score.ToString();
	}

	public static void AnimateBarToValue(UIProgressBar bar, float to, float delay)
	{
		AnimateBarToValue(bar, 0f, to, delay);
	}

	public static void AnimateBarToValue(UIProgressBar bar, float from, float to, float delay)
	{
		if ((bool)bar)
		{
			iTween.ValueTo(bar.gameObject, iTween.Hash("from", from, "to", to, "delay", delay, "easetype", iTween.EaseType.linear, "onUpdate", (Action<object>)delegate(object x)
			{
				bar.Value = (float)x;
			}));
		}
	}
}
