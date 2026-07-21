using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public class GameTypeMenuView : GenericButton
{
	public delegate void HandleGameTypeButtonTapped(GameTypeMenuView gameTypeMenuView);

	private bool isLocked;

	[SerializeField]
	private GameData.GAME_TYPE gameType;

	[SerializeField]
	private SpriteText buttonLabel;

	[SerializeField]
	private PackedSprite lockSprite;

	[SerializeField]
	private PackedSprite gameTypeSymbol;

	[SerializeField]
	private Color unlockedButtonColor = Color.black;

	[SerializeField]
	private Color lockedButtonColor = Color.black;

	[SerializeField]
	private Color unlockedSymbolColor = Color.black;

	[SerializeField]
	private Color lockedSymbolColor = Color.black;

	[SerializeField]
	private TrophyView[] trophyViews;

	[SerializeField]
	private Appointment appointment;

	[SerializeField]
	private TickerTapeText gameStatusTickerTape;

	[SerializeField]
	private SpriteText gameStatusLabel;

	private DrillSettings gameDrillSettings;

	private GameInfo gameInfo;

	private float newlyUnlockedDelayTime = 3f;

	private string isNewlyUnlockedPrefsKey;

	public bool IsLocked
	{
		get
		{
			return isLocked;
		}
		set
		{
			isLocked = value;
			controlIsEnabled = !isLocked;
			if (gameInfo.HighScore != 0)
			{
				int i = 0;
				for (int num = trophyViews.Length; i < num; i++)
				{
					trophyViews[i].TrophySprite.Hide(isLocked);
				}
			}
			else
			{
				int j = 0;
				for (int num2 = trophyViews.Length; j < num2; j++)
				{
					trophyViews[j].TrophySprite.Hide(tf: true);
				}
			}
			if (!IsLocked)
			{
				if (!PlayerPrefs.HasKey(isNewlyUnlockedPrefsKey))
				{
					PlayerPrefs.SetInt(isNewlyUnlockedPrefsKey, 1);
				}
				if (IsNewlyUnlocked)
				{
					appointment.NextGameView.DisplayView(isVisible: false);
					gameStatusLabel.Text = "Unlocked!";
					iTween.ShakePosition(base.transform.gameObject, iTween.Hash("amount", new Vector3(1f, 1f, 1f), "time", 0.5f, "delay", NewlyUnlockedDelayTime, "onComplete", (Action<object>)delegate
					{
						appointment.NextGameView.DisplayView(isVisible: true);
						appointment.FastTrackAppointmentForFree();
						InitHighScoreText();
					}));
				}
				else
				{
					InitHighScoreText();
					appointment.Init();
				}
				SetColor(unlockedButtonColor);
				gameTypeSymbol.SetColor(unlockedSymbolColor);
			}
			else
			{
				appointment.NextGameView.DisplayView(isVisible: false);
				SetColor(lockedButtonColor);
				gameTypeSymbol.SetColor(lockedSymbolColor);
				if (gameDrillSettings != null && (bool)gameStatusLabel)
				{
					gameStatusLabel.Text = "Get to Level " + GlobalSettings.GetLevelForUnlock(gameType, GameData.SelectedGameLocation) + " to unlock";
				}
			}
			if ((bool)lockSprite)
			{
				lockSprite.Hide(!IsLocked);
			}
		}
	}

	public GameData.GAME_TYPE GameType => gameType;

	public TickerTapeText GameStatusTickerTape => gameStatusTickerTape;

	public Appointment Appointment => appointment;

	public DrillSettings GameDrillSettings => gameDrillSettings;

	public GameInfo GameInfo => gameInfo;

	public bool IsNewlyUnlocked
	{
		get
		{
			if (PlayerPrefs.HasKey(isNewlyUnlockedPrefsKey))
			{
				return PlayerPrefs.GetInt(isNewlyUnlockedPrefsKey) == 1;
			}
			return false;
		}
	}

	public float NewlyUnlockedDelayTime
	{
		get
		{
			return newlyUnlockedDelayTime;
		}
		set
		{
			newlyUnlockedDelayTime = value;
		}
	}

	[method: MethodImpl(32)]
	public static event HandleGameTypeButtonTapped GameTypeButtonTapped;

	protected override void Awake()
	{
		base.Awake();
		isNewlyUnlockedPrefsKey = GameData.SelectedGameLocation.ToString() + gameType.ToString() + "IsNewlyUnlocked";
	}

	public override void Start()
	{
		base.Start();
		if (GameData.SelectedGameLocation != GameData.GAME_LOCATION.ClubHouse)
		{
			gameDrillSettings = GlobalSettings.GetGame(gameType, GameData.SelectedGameLocation);
			gameInfo = PlayerManager.GetGameInfoForLocationAndType(GameData.SelectedGameLocation, gameType);
			appointment.TimerId = "timerId" + gameInfo.Name;
			if (GameData.IsComingFromGame && GameData.SelectedGameType == gameType && GameData.SelectedGameLocation == MapController.gameLocation)
			{
				appointment.Reset();
			}
			appointment.CoinsReqToSkipToAppointment = gameDrillSettings.CoinsToSkipAppt;
			if ((bool)buttonLabel)
			{
				buttonLabel.Text = gameType.ToString().ToUpper();
			}
			IsLocked = !gameInfo.IsUnlocked;
			gameTypeSymbol.PlayAnim(gameType.ToString());
			int i = 0;
			for (int num = trophyViews.Length; i < num; i++)
			{
				trophyViews[i].UpdateState(gameInfo);
			}
		}
	}

	protected override void HandleButtonPressed(ref POINTER_INFO ptr)
	{
		if (MonoSingleton<PanelManager>.Instance.IsPanelDisplaying)
		{
			return;
		}
		base.HandleButtonPressed(ref ptr);
		if (!IsLocked && ptr.evt == POINTER_INFO.INPUT_EVENT.TAP && GameTypeMenuView.GameTypeButtonTapped != null)
		{
			if (GameData.Instance != null)
			{
				GameData.Instance.IsCurrentApptDue = appointment.IsAppointmentDue;
			}
			if (appointment.IsAppointmentDue)
			{
				GameTypeMenuView.GameTypeButtonTapped(this);
			}
			else if (GameData.PlayerStats.CoinsStock >= appointment.CoinsReqToSkipToAppointment)
			{
				GameTypeMenuView.GameTypeButtonTapped(this);
			}
			else if (MonoSingleton<GameTypeMenuManager>.IsInitialized)
			{
				MonoSingleton<GameTypeMenuManager>.Instance.DisplayCoinsView();
			}
		}
	}

	private void InitHighScoreText()
	{
		if (gameDrillSettings != null && (bool)gameStatusLabel)
		{
			if (gameInfo.HighScore == 0)
			{
				gameStatusLabel.Text = "Shoot for a high score!";
			}
			else
			{
				gameStatusLabel.Text = "Score " + gameInfo.HighScore.ToString("#,##0") + " PTS";
			}
		}
	}
}
