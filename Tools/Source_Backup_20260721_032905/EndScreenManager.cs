using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class EndScreenManager : MonoBehaviour
{
	public delegate void HandleTrophyAward();

	[SerializeField]
	private UIPanel _endScreenPanel;

	[SerializeField]
	private UIPanel _scorePanel;

	[SerializeField]
	private UIPanel _streakPanel;

	[SerializeField]
	private UIPanel _curlPanel;

	[SerializeField]
	private UIPanel _rushPanel;

	[SerializeField]
	private UIPanel _coinsPanel;

	[SerializeField]
	private SpriteText _coinsValue;

	[SerializeField]
	private SpriteText _scoreText;

	[SerializeField]
	private SpriteText _streakText;

	[SerializeField]
	private SpriteText _curlText;

	[SerializeField]
	private SpriteText _rushText;

	[SerializeField]
	private SpriteText _mainTitle;

	[SerializeField]
	private SpriteText _playAgainCoinValue;

	[SerializeField]
	private PackedSprite _playAgainCoinSprite;

	[SerializeField]
	private PackedSprite coinsEarntSprite;

	[SerializeField]
	private Transform _scoreStampPoint;

	[SerializeField]
	private Transform _streakStampPoint;

	[SerializeField]
	private Transform _curlStampPoint;

	[SerializeField]
	private SFX _tallysUpSound;

	[SerializeField]
	private UIButton _playAgainButton;

	[SerializeField]
	private QuitGame _quitGameEvent;

	[SerializeField]
	private List<ScreenPopup> _screenPopups = new List<ScreenPopup>();

	[SerializeField]
	private DrillProgressView drillProgressView;

	[SerializeField]
	private AchievementIconListView achievementIconListView;

	[SerializeField]
	private AchievementDetailView achievementDetailView;

	[SerializeField]
	private ParticleSystem stampParticles;

	private int _playAgainCost;

	public static EndScreenManager Instance;

	public bool CanPlayAgain => GameData.PlayerStats.CoinsStock >= _playAgainCost;

	public bool IsScreenPopActive
	{
		get
		{
			bool result = false;
			foreach (ScreenPopup screenPopup in _screenPopups)
			{
				if (screenPopup.IsActive)
				{
					result = true;
				}
			}
			if (achievementDetailView.IsDisplaying)
			{
				result = true;
			}
			return result;
		}
	}

	[method: MethodImpl(32)]
	public event HandleTrophyAward TrophyAwarded;

	private void Awake()
	{
		GameData.CheckInstance();
		if (Instance == null)
		{
			Instance = this;
		}
	}

	private void Start()
	{
		_playAgainCost = GlobalSettings.GetCurrentGame().CoinsToSkipAppt;
		bool flag = GameData.SelectedGameType == GameData.GAME_TYPE.Tutorial;
		if (!_playAgainButton.enabled)
		{
			_playAgainButton.gameObject.SetActiveRecursively(!flag);
		}
		SetPlayAgainCoins(_playAgainCost);
	}

	private void OnEnable()
	{
		Minigame.MinigameWillStart += Initialize;
		AchievementIconListView.IconViewInputted += HandleAchievementIconListViewIconViewInputted;
		PlayerManager.NewAchievementUnlocked += HandleNewAchievementUnlocked;
	}

	private void HandleAchievementIconListViewIconViewInputted(Achievement achievement)
	{
		achievementDetailView.Init(achievement.Title, string.Empty, achievement.Description, achievement);
		achievementDetailView.DisplayView(isVisible: true);
	}

	private void OnDisable()
	{
		Minigame.MinigameWillStart -= Initialize;
		AchievementIconListView.IconViewInputted -= HandleAchievementIconListViewIconViewInputted;
		PlayerManager.NewAchievementUnlocked -= HandleNewAchievementUnlocked;
	}

	private void Initialize()
	{
		_mainTitle.Text = GameData.SelectedGameType.ToString().ToUpper();
	}

	public void StopEndScreenEvent()
	{
		StopCoroutine("EndScreenEvent");
	}

	public void StartEndScreenEvent()
	{
		StartCoroutine(EndScreenEvent());
	}

	private IEnumerator EndScreenEvent()
	{
		drillProgressView.Reset();
		ResetElements();
		GameData.GUIManager.ShowGameGUI(bShow: false);
		GameData.MinigameObject.CurrentMinigame.TimerSound(active: false);
		CR_TouchManager.IsActive = false;
		ShowEndScreen(bShow: true);
		AudioManager.PlaySFX(AudioManager.Instance.audioCollection.CurrentGame.standardAudio.endScreenIn, 0f);
		MinigameChallenge mc = GameData.MinigameObject.CurrentMinigame;
		GameData.GameCamera.BlurCamera();
		SetPlayAgainCoins(_playAgainCost);
		while (_endScreenPanel.IsTransitioning)
		{
			yield return null;
		}
		if (mc.PlainScore > 0)
		{
			int currentScore3 = mc.PlainScore;
			MinigameGUI.CountUpText(_scoreText, 0, currentScore3, _tallysUpSound);
			SetElementScore(_curlText, mc.CurlScore);
			SetElementScore(_streakText, mc.StreakScore);
			AnimateStampElement(_scorePanel, _scoreStampPoint.position);
			yield return new WaitForSeconds(1f);
			AnimateStampElement(_curlPanel, _curlStampPoint.position);
			MinigameGUI.CountUpText(_scoreText, currentScore3, currentScore3 + mc.CurlScore, _tallysUpSound);
			currentScore3 += mc.CurlScore;
			yield return new WaitForSeconds(1f);
			AnimateStampElement(_streakPanel, _streakStampPoint.position);
			MinigameGUI.CountUpText(_scoreText, currentScore3, currentScore3 + mc.StreakScore, _tallysUpSound);
			currentScore3 += mc.StreakScore;
			if (mc.UsingRushMode)
			{
				SetElementScore(_rushText, mc.RushBonus);
				yield return new WaitForSeconds(0.5f);
				_rushPanel.BringIn();
				MinigameGUI.CountUpText(_scoreText, currentScore3, currentScore3 + mc.RushBonus, _tallysUpSound);
				currentScore3 += mc.RushBonus;
			}
			drillProgressView.SetProgressBars(PlayerManager.GetGameInfoForLocationAndType(GameData.SelectedGameLocation, mc.GameType), GlobalSettings.GetGame(mc.GameType, GameData.SelectedGameLocation), currentScore3);
			yield return new WaitForSeconds(2f);
			_coinsPanel.BringIn();
			_coinsPanel.GetTransition(UIPanelManager.SHOW_MODE.BringInForward).AddTransitionEndDelegate(delegate
			{
				iTween.PunchScale(coinsEarntSprite.gameObject, new Vector3(1f, 1f, 1f), 3f);
				MinigameGUI.CountUpText(_coinsValue, 0, GameData.PlayerStats.CoinRewardForScore, _tallysUpSound);
			});
			yield return new WaitForSeconds(2f);
			foreach (ScreenPopup popup in _screenPopups)
			{
				if (popup.CanShow)
				{
					popup.Bring();
					while (popup.IsActive)
					{
						yield return null;
					}
				}
			}
		}
		PlayerManager.SendFeedback("Completed " + GameData.SelectedGame, string.Empty, string.Empty);
		PlayerManager.SendFeedback("Coins Awarded " + GameData.SelectedGame, "Number of Coins", GameData.PlayerStats.CoinsJustEarned.ToString());
		yield return true;
	}

	private void SendTrophyMessage()
	{
		if (this.TrophyAwarded != null)
		{
			this.TrophyAwarded();
		}
	}

	private void SetPlayAgainCoins(int coins)
	{
		_playAgainCost = coins;
		_playAgainCoinValue.Text = _playAgainCost.ToString();
		_playAgainCoinValue.Color = ((!CanPlayAgain) ? Color.red : Color.white);
	}

	public void ShowEndScreen(bool bShow)
	{
		GameData.MinigameObject.CurrentMinigame.IsEndScreenShown = bShow;
		_endScreenPanel.StartTransition((!bShow) ? UIPanelManager.SHOW_MODE.DismissForward : UIPanelManager.SHOW_MODE.BringInForward);
	}

	private UIPanel AnimateStampElement(UIPanel element, Vector3 stampPoint)
	{
		element.StartTransition(UIPanelManager.SHOW_MODE.BringInForward);
		element.GetTransition(UIPanelManager.SHOW_MODE.BringInForward).AddTransitionEndDelegate(delegate
		{
			StampDustAnimation(stampPoint);
		});
		return element;
	}

	private bool AnyPopupActive()
	{
		foreach (ScreenPopup screenPopup in _screenPopups)
		{
			if (screenPopup.CanShow)
			{
				return true;
			}
		}
		return false;
	}

	private void SetElementScore(SpriteText element, int score)
	{
		element.Text = score.ToString("#,##0");
	}

	private void StampDustAnimation(Vector3 spawnPosition)
	{
		stampParticles.transform.position = spawnPosition;
		stampParticles.Play();
		AudioManager.PlaySFX(AudioManager.Instance.audioCollection.CurrentGame.standardAudio.stamp, 0f);
	}

	private void ResetElements()
	{
		SetElementScore(_scoreText, 0);
		SetElementScore(_curlText, 0);
		SetElementScore(_streakText, 0);
		SetElementScore(_rushText, 0);
		SetElementScore(_coinsValue, 0);
		_scorePanel.Dismiss();
		_curlPanel.Dismiss();
		_streakPanel.Dismiss();
		_rushPanel.Dismiss();
		_coinsPanel.Dismiss();
	}

	public void ResetState()
	{
		ShowEndScreen(bShow: false);
		ResetElements();
	}

	public void QuitGame()
	{
		if (!IsScreenPopActive)
		{
			if (_quitGameEvent.sceneToCall == SCENES.MAP)
			{
				Appointment.ResetCurrentAppointment();
			}
			StopEndScreenEvent();
			_endScreenPanel.GetTransition(UIPanelManager.SHOW_MODE.DismissForward).AddTransitionEndDelegate(delegate
			{
				_quitGameEvent.GoToScene();
			});
			ShowEndScreen(bShow: false);
		}
	}

	public void PlayAgain()
	{
		if (!IsScreenPopActive)
		{
			if (CanPlayAgain)
			{
				drillProgressView.StopParticles();
				stampParticles.Stop();
				stampParticles.Clear();
				achievementIconListView.ClearAchievementsList();
				GameData.PlayerStats.UseCoins(_playAgainCost);
				PlayAgainHandler.Instance.MinigamePlayAgain();
			}
			else
			{
				BuyController.Instance.ShowAndComeBackToPanel(_endScreenPanel, "OUT OF COINS");
			}
		}
	}

	private void GreyOutPlayAgainButton(bool bGreyout)
	{
		MinigameGUI.GreyOutButton(_playAgainButton, bGreyout);
		MinigameGUI.GreyOutSpriteText(_playAgainCoinValue, bGreyout);
		MinigameGUI.GreyOutPackedSprite(_playAgainCoinSprite, bGreyout);
	}

	private void HandleNewAchievementUnlocked(Achievement achievement)
	{
		if (achievement.Type == Achievement.TYPE.GAME_UNLOCK)
		{
			_quitGameEvent.sceneToCall = SCENES.MAP;
		}
	}
}
