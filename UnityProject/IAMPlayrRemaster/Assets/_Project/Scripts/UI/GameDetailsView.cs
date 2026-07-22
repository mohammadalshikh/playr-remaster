using System.Collections.Generic;
using UnityEngine;

public class GameDetailsView : View
{
	[SerializeField]
	private SpriteText topBarTitleText;

	[SerializeField]
	private SpriteText gameDescriptionText;

	[SerializeField]
	private SpriteText costToPlayText;

	[SerializeField]
	private PackedSprite gameScreenshot;

	[SerializeField]
	private PackedSprite gameSymbol;

	[SerializeField]
	private PackedSprite coinSymbol;

	[SerializeField]
	private DrillProgressView drillProgressView;

	[SerializeField]
	private UIButton playButton;

	[SerializeField]
	private Transform playNowTransform;

	private Vector3 playNowPos;

	private GameData.GAME_TYPE selectedGameType;

	private Dictionary<GameData.GAME_TYPE, string> gameDescriptions = new Dictionary<GameData.GAME_TYPE, string>
	{
		{
			GameData.GAME_TYPE.Countdown,
			"You've got sixty seconds on the clock to score as many points as you can!"
		},
		{
			GameData.GAME_TYPE.Rush,
			"This challenge is called Rush. I've set you a score to reach, you need to hit it as fast as you can."
		},
		{
			GameData.GAME_TYPE.Lifeline,
			"In Lifeline you've only got three lives to score as many points as possible. Be careful!"
		},
		{
			GameData.GAME_TYPE.Knockout,
			"This challenge is called Knockout. There's a minimum number of targets to hit in each round."
		}
	};

	private void Start()
	{
		if ((bool)playButton)
		{
			playButton.AddInputDelegate(HandlePlayButtonInput);
		}
		playNowPos = playNowTransform.localPosition;
	}

	private void HandleGameTypeButtonTapped(GameTypeMenuView gameTypeMenuView)
	{
		SetPlayButton(gameTypeMenuView);
		drillProgressView.SetProgressBars(gameTypeMenuView.GameInfo, gameTypeMenuView.GameDrillSettings, gameTypeMenuView.GameInfo.HighScore);
		selectedGameType = gameTypeMenuView.GameType;
		string text = selectedGameType.ToString();
		if ((bool)topBarTitleText)
		{
			topBarTitleText.Text = text;
		}
		gameSymbol.PlayAnim(text);
		if ((bool)gameScreenshot)
		{
			gameScreenshot.PlayAnim(GameData.SelectedGameLocation.ToString());
		}
		foreach (KeyValuePair<GameData.GAME_TYPE, string> gameDescription in gameDescriptions)
		{
			if (gameDescription.Key == selectedGameType)
			{
				gameDescriptionText.Text = gameDescription.Value;
			}
		}
		DisplayView(isVisible: true);
	}

	private void HandlePlayButtonInput(ref POINTER_INFO ptr)
	{
		if (ptr.evt == POINTER_INFO.INPUT_EVENT.TAP && MonoSingleton<GameTypeMenuManager>.IsInitialized && !MonoSingleton<PanelManager>.Instance.IsPanelDisplaying)
		{
			MonoSingleton<GameTypeMenuManager>.Instance.PlayGame(selectedGameType);
		}
	}

	private void SetPlayButton(GameTypeMenuView gameTypeMenuView)
	{
		if (gameTypeMenuView.Appointment.IsAppointmentDue || gameTypeMenuView.IsNewlyUnlocked)
		{
			coinSymbol.GetComponent<Renderer>().enabled = false;
			costToPlayText.GetComponent<Renderer>().enabled = false;
			playNowTransform.localPosition = new Vector3(playNowPos.x + 10f, playNowPos.y, playNowPos.z);
		}
		else
		{
			coinSymbol.GetComponent<Renderer>().enabled = true;
			costToPlayText.GetComponent<Renderer>().enabled = true;
			costToPlayText.Text = gameTypeMenuView.Appointment.CoinsReqToSkipToAppointment.ToString();
			playNowTransform.localPosition = playNowPos;
		}
	}

	private void OnEnable()
	{
		GameTypeMenuView.GameTypeButtonTapped += HandleGameTypeButtonTapped;
	}

	private void OnDisable()
	{
		GameTypeMenuView.GameTypeButtonTapped -= HandleGameTypeButtonTapped;
	}
}
