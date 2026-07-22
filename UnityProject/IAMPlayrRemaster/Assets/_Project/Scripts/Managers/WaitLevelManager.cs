using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitLevelManager : MonoBehaviour
{
	public List<GameTip> gameTips = new List<GameTip>();

	private List<GameTip> availableTips = new List<GameTip>();

	public GameTip customIntro;

	public SpriteText messageText;

	public PackedSprite background;

	public UIPanel mainContent;

	public PackedSprite turnLandscapeSprite;

	public PackedSprite loadingWheel;

	private bool _readyToFinish;

	private bool _shouldPlayVideo;

	private GameInfo _currentGameInfo;

	private int _currentTip;

	public static WaitLevelManager Instance;

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
		InitGameInfo();
		SetTipsForGameType();
		PlayerManager.SendFeedback("Played " + GameData.SelectedGame + " game", string.Empty, string.Empty);
		SetBackground();
		StartCoroutine(AnimateIntro());
	}

	private void InitGameInfo()
	{
		if (GameData.PlayerStats != null)
		{
			_currentGameInfo = GameData.PlayerStats.GameInfoWithName(GameData.SelectedGame);
		}
	}

	private void SetBackground()
	{
		if (background.GetAnim(GameData.SelectedGame) != null)
		{
			background.PlayAnim(GameData.SelectedGame);
		}
		background.Hide(!IsGameInfoAvailable());
	}

	private void SetTipsForGameType()
	{
		foreach (GameTip gameTip in gameTips)
		{
			if (gameTip.type == GameData.CurrentMinigameType || (gameTip.genericType && gameTips.IndexOf(gameTip) != 0))
			{
				availableTips.Add(gameTip);
			}
		}
	}

	public void CloseButton()
	{
		_readyToFinish = true;
		SpinWheel();
		customIntro.Dismiss();
	}

	private IEnumerator AnimateIntro()
	{
		AnimateInRandomTip();
		if (IsGameInfoAvailable())
		{
			SpinWheel();
		}
		yield return new WaitForSeconds(2f);
		availableTips[_currentTip].Dismiss();
		if (IsGameInfoAvailable())
		{
			BringGameInfo();
			yield return new WaitForSeconds(0.5f);
			StartCoroutine(NextStep(2f));
			yield break;
		}
		string message = string.Empty;
		string popupBoxBg = "default";
		switch (GameData.SelectedGame)
		{
		case "Pumpkin":
			message = "Our spooky Halloween spectacular is only available until our next update!\n\nAs the clock ticks down pump shots at pumpkins, grab ghosts for extra time and batter bats for bonus multipliers.\n\nWatch out for the witch! Big points for  knocking her off her broom...\n\nCome November we'll clear the Pumpkin scores from the leader board so make the most of bagging big points for bragging rights!";
			popupBoxBg = "Pumpkin".ToString().ToLower();
			customIntro.m_tipDescription.SetCharacterSize(0.7f);
			break;
		case "Christmas":
			message = "Live until midnight January 5th, see how much you can score in our holiday season special!\n\nSnowflakes earn points and gift targets earn you bonus multipliers.\n\nSmash Santa when he appears for big bonus points!\n\nCan you beat all 25 levels and light up the Christmas sleigh!?";
			break;
		case "Tutorial":
			message = "Jordan Henderson, Jamie Carragher and Stuart Downing have set the scores in target shooting challenges.\n\nYou need to beat them all to take on Steve Gerrard. Get practicing...";
			break;
		case "Practice":
			message = "Unlimited time to perfect your technique in the Practice Zone!\n\nWhen you're ready to leave, hit the pause button and tap quit.";
			break;
		}
		customIntro.SetDescription(message);
		customIntro.SetBackgroundFrame(popupBoxBg);
		customIntro.Show();
		while (!_readyToFinish)
		{
			AnimateInRandomTip();
			yield return new WaitForSeconds(3f);
			availableTips[_currentTip].Dismiss();
			yield return null;
		}
		yield return new WaitForSeconds(0.5f);
		StartCoroutine(NextStep(0f));
	}

	private void BringGameInfo()
	{
		string empty = string.Empty;
		_currentTip = 0;
		if (IsGameInfoAvailable())
		{
		}
		gameTips[_currentTip].SetDescription(empty);
		gameTips[_currentTip].Show();
	}

	private void AnimateInRandomTip()
	{
		int num = _currentTip;
		while (_currentTip == num && availableTips.Count > 1)
		{
			num = Random.Range(1, availableTips.Count);
		}
		_currentTip = num;
		availableTips[_currentTip].Show();
	}

	private IEnumerator NextStep(float delay)
	{
		yield return StartCoroutine(GameData.WaitSeconds(delay));
		switch (GameData.SelectedGame)
		{
		case "Parker":
			if (!PlayerPrefs.HasKey("ParkerTutorial"))
			{
				gameTips[_currentTip].Dismiss();
				Tutorials.Instance.SetupType(0);
				Tutorials.Instance.BringIn();
				PlayerPrefs.SetInt("ParkerTutorial", 1);
				PlayerPrefs.Save();
			}
			else
			{
				PlayOrSkipVideo(ShouldPlayVideo());
			}
			break;
		default:
			PlayOrSkipVideo(ShouldPlayVideo());
			break;
		}
	}

	public void PlayGame()
	{
		GoToGame(ShouldPlayVideo());
	}

	private void GoToGame(bool playVideo)
	{
		GameData.ShouldPlayVideo = playVideo;
		turnLandscapeSprite.Hide(!playVideo);
		Screen.orientation = ScreenOrientation.LandscapeLeft;
	}

	private void PlayOrSkipVideo(bool playVideo)
	{
		GoToGame(playVideo);
	}

	public void ShowLoadingMessage()
	{
		messageText.Hide(tf: false);
	}

	private bool IsGameInfoAvailable()
	{
		return _currentGameInfo != null && !IsCustomGame();
	}

	private void SpinWheel()
	{
		loadingWheel.Hide(tf: false);
		loadingWheel.PlayAnim("loading");
	}

	private bool IsCustomGame()
	{
		return GameData.SelectedGame switch
		{
			"Pumpkin" => true, 
			"Christmas" => true, 
			_ => false, 
		};
	}

	private bool ShouldPlayVideo()
	{
		if (IsGameInfoAvailable())
		{
			if (_currentGameInfo.IsGameFirstTimePlayed())
			{
				return false;
			}
			return false;
		}
		return false;
	}
}
