using System;
using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;

public class MinigameGUI : MonoBehaviour
{
	public delegate void HandleGUIVisibility(bool bEnabled);

	protected GUI_STYLE m_currentStyle;

	public Camera GUICamera;

	[SerializeField]
	private UIPanel _mainLeftPanel;

	[SerializeField]
	private UIPanel _mainRightPanel;

	public NeonGUIStyle neonStyle = new NeonGUIStyle();

	[SerializeField]
	private InfoView _infoView;

	private bool _animating;

	private PROGRESS_PANEL_TYPE _currentProgressPanel;

	private bool _isDisplayed;

	private bool _isTimerRed;

	private Color _timerNeonDefault = new Color(28f / 85f, 0.9137255f, 1f, 1f);

	private bool _isReadyToStart;

	private bool _finishedIntro;

	private bool _shouldShowStreak;

	private bool _shouldShowCurl;

	private int _acummulatedCurl;

	private bool _isDisplayingNotification;

	public GameGUIStyle CurrentSyle
	{
		get
		{
			if (m_currentStyle == GUI_STYLE.NEON)
			{
				return neonStyle;
			}
			return neonStyle;
		}
	}

	public PROGRESS_PANEL_TYPE ProgressPanelType
	{
		get
		{
			return _currentProgressPanel;
		}
		set
		{
			_currentProgressPanel = value;
		}
	}

	public GUI_STYLE SetGUIStyle
	{
		set
		{
			m_currentStyle = value;
		}
	}

	public string ScoreText
	{
		set
		{
			CurrentSyle.standardHUDElements.scoreText.Text = value;
		}
	}

	public SpriteText ScoreSprite => CurrentSyle.standardHUDElements.scoreText;

	public string TimerText
	{
		set
		{
			CurrentSyle.standardHUDElements.timerText.Text = value;
		}
	}

	public bool IsDisplayed
	{
		get
		{
			return _isDisplayed;
		}
		set
		{
			_isDisplayed = value;
		}
	}

	public bool IsTimerRed => _isTimerRed;

	public bool FinishedIntro => _finishedIntro;

	public string RushScoreText
	{
		set
		{
			CurrentSyle.standardHUDElements.rushScore.Text = value;
		}
	}

	[method: MethodImpl(32)]
	public static event HandleGUIVisibility MakeGUIEnabled;

	private void Awake()
	{
		GameData.CheckInstance();
		GameData.GUIManager = this;
		if (DeviceInfo.IsTablet)
		{
			GUICamera.orthographicSize = 10.75f;
		}
	}

	private void OnEnable()
	{
		TutorialView.WillShowEvent += HidePanels;
		TutorialView.WillHideEvent += ShowPanels;
	}

	private void OnDisable()
	{
		TutorialView.WillShowEvent -= HidePanels;
		TutorialView.WillHideEvent -= ShowPanels;
	}

	public void IntroEvent()
	{
		StartCoroutine(FireIntroEvent());
	}

	public IEnumerator PowerUpScreen()
	{
		if (PowerUpManager.Instance.IsEnabled)
		{
			_isReadyToStart = false;
			if (!ShowIntroView())
			{
				PowerUpManager.Instance.ShowPanel(bShow: true);
			}
			CurrentSyle.standardHUDElements.pauseButton.Hide(true, true);
			GameData.GameCamera.BlurCamera();
			while (!_isReadyToStart)
			{
				yield return null;
			}
			yield return true;
		}
	}

	private bool ShowIntroView()
	{
		if (GlobalSettings.GetCurrentGame().HasBeenPlayed)
		{
			return false;
		}
		GlobalSettings.GetCurrentGame().MarkPlayed();
		string text = GameData.SelectedGameType.ToString();
		_infoView.SetAndShow(text.ToUpper(), GlobalSettings.GetCurrentGame().Description, PowerUpManager.Instance.Panel, "CONTINUE");
		return true;
	}

	public void ShowInfoView(string title, string message, UIPanel swapPanel, string buttonText = "CLOSE")
	{
		_infoView.SetAndShow(title, message, swapPanel, buttonText);
	}

	public IEnumerator FireIntroEvent()
	{
		StartCoroutine(GameData.MinigameObject.CurrentMinigame.CustomIntro());
		Minigame.BlockTouchInput(bBlock: true);
		GUICamera.enabled = false;
		CurrentSyle.standardHUDElements.introText.GetComponent<Renderer>().enabled = true;
		CurrentSyle.standardHUDElements.introText.Text = "READY??";
		CurrentSyle.standardHUDElements.fadePlane.StartTransition(UIPanelManager.SHOW_MODE.BringInBack);
		yield return StartCoroutine(GameData.WaitSeconds(0.8f));
		GUICamera.enabled = true;
		CurrentSyle.standardHUDElements.introTextPanel.StartTransition(UIPanelManager.SHOW_MODE.BringInForward);
		CurrentSyle.standardHUDElements.introCountText.Hide(false, false);
		CurrentSyle.standardHUDElements.introCountText.Text = "3";
		yield return StartCoroutine(GameData.WaitSeconds(0.8f));
		CurrentSyle.standardHUDElements.introCountText.Text = "2";
		yield return StartCoroutine(GameData.WaitSeconds(0.8f));
		CurrentSyle.standardHUDElements.introCountText.Text = "1";
		yield return StartCoroutine(GameData.WaitSeconds(0.8f));
		CurrentSyle.standardHUDElements.introCountText.Text = "0";
		CurrentSyle.standardHUDElements.introTextPanel.StartTransition(UIPanelManager.SHOW_MODE.BringInBack);
		CurrentSyle.standardHUDElements.introText.Text = "GO!!";
		CurrentSyle.standardHUDElements.introCountText.Hide(true, true);
		while (CurrentSyle.standardHUDElements.introTextPanel.IsTransitioning())
		{
			yield return null;
		}
		Minigame.BlockTouchInput(bBlock: false);
		CurrentSyle.standardHUDElements.introText.GetComponent<Renderer>().enabled = false;
		CurrentSyle.standardHUDElements.introTextPanel.StartTransition(UIPanelManager.SHOW_MODE.DismissForward);
		_finishedIntro = true;
		while (CurrentSyle.standardHUDElements.introTextPanel.IsTransitioning())
		{
			yield return null;
		}
		GameData.GameCamera.RemoveBlur();
	}

	public void ClosePowerUpScreen()
	{
		_isReadyToStart = true;
	}

	public void SetRemainingShots(int current_shot, int total_shots)
	{
		string text = ((current_shot >= 10) ? (string.Empty + current_shot) : ("0" + current_shot));
		text += "/";
		text += ((total_shots >= 10) ? (string.Empty + total_shots) : ("0" + total_shots));
		CurrentSyle.standardHUDElements.shotsText.Text = text;
	}

	public void ShowLeftCurrentPanel(bool bShow)
	{
		switch (_currentProgressPanel)
		{
		case PROGRESS_PANEL_TYPE.LIVES:
			ShowLifePanel(bShow);
			break;
		case PROGRESS_PANEL_TYPE.SHOTS:
			ShowShotsPanel(bShow);
			break;
		case PROGRESS_PANEL_TYPE.RUSH:
			ShowRushPanel(bShow);
			break;
		case PROGRESS_PANEL_TYPE.SHIELD:
			break;
		}
	}

	public void ShowLifePanel(bool bShow, bool bShieldUsed = false)
	{
		if (bShow)
		{
			ProgressPanelType = PROGRESS_PANEL_TYPE.LIVES;
			CurrentSyle.standardHUDElements.leftPanelLives.StartTransition(bShieldUsed ? UIPanelManager.SHOW_MODE.BringInBack : UIPanelManager.SHOW_MODE.BringInForward);
		}
		else
		{
			ProgressPanelType = PROGRESS_PANEL_TYPE.NONE;
			CurrentSyle.standardHUDElements.leftPanelLives.StartTransition(UIPanelManager.SHOW_MODE.DismissForward);
		}
	}

	public void ShowShotsPanel(bool bShow, bool bShieldUsed = false)
	{
		if (bShow)
		{
			ProgressPanelType = PROGRESS_PANEL_TYPE.SHOTS;
			CurrentSyle.standardHUDElements.leftPanelShots.StartTransition(bShieldUsed ? UIPanelManager.SHOW_MODE.BringInBack : UIPanelManager.SHOW_MODE.BringInForward);
		}
		else
		{
			ProgressPanelType = PROGRESS_PANEL_TYPE.NONE;
			CurrentSyle.standardHUDElements.leftPanelShots.StartTransition(UIPanelManager.SHOW_MODE.DismissForward);
		}
	}

	public void ShowRushPanel(bool bShow)
	{
		if (bShow)
		{
			ProgressPanelType = PROGRESS_PANEL_TYPE.RUSH;
			CurrentSyle.standardHUDElements.leftPanelRush.BringIn();
		}
		else
		{
			ProgressPanelType = PROGRESS_PANEL_TYPE.NONE;
			CurrentSyle.standardHUDElements.leftPanelRush.Dismiss();
		}
	}

	public void SetRushScore(int previous, int newScore)
	{
		iTween.ValueTo(base.gameObject, iTween.Hash("from", previous, "to", newScore, "time", 0.5f, "onUpdate", (Action<object>)delegate(object x)
		{
			CurrentSyle.standardHUDElements.rushScore.Text = Mathf.RoundToInt((float)x).ToString("#,##0");
		}));
	}

	public void ShowScorePanel(bool bShow, bool bMultiplierused = false)
	{
		if (bShow)
		{
			CurrentSyle.standardHUDElements.rightPanelScore.StartTransition(bMultiplierused ? UIPanelManager.SHOW_MODE.BringInBack : UIPanelManager.SHOW_MODE.BringInForward);
		}
		else
		{
			CurrentSyle.standardHUDElements.rightPanelScore.StartTransition(UIPanelManager.SHOW_MODE.DismissForward);
		}
	}

	public void ShowTimerPanel(bool bShow)
	{
		CurrentSyle.standardHUDElements.timerPanel.StartTransition((!bShow) ? UIPanelManager.SHOW_MODE.DismissForward : UIPanelManager.SHOW_MODE.BringInForward);
	}

	public void AddTimeAnimation(int addedTime)
	{
		CurrentSyle.standardHUDElements.addTimeText.Text = "+" + addedTime + "s";
		StartCoroutine(AnimateHUDPanel(CurrentSyle.standardHUDElements.addTimeAnimation, 0f));
		ShowNotification("Time Bonus +" + addedTime + "s");
	}

	public void SetTimerColor(bool lastSeconds)
	{
		if (lastSeconds != _isTimerRed)
		{
			if (lastSeconds)
			{
				CurrentSyle.standardHUDElements.timerText.Color = Color.red;
				_isTimerRed = true;
			}
			else
			{
				CurrentSyle.standardHUDElements.timerText.Color = _timerNeonDefault;
				_isTimerRed = false;
			}
			if (CurrentSyle.standardHUDElements.timerIcon != null)
			{
				CurrentSyle.standardHUDElements.timerIcon.PlayAnim(lastSeconds ? 1 : 0);
			}
		}
	}

	public void SetMultiplierIcon(int multiplier_num, bool startup = false)
	{
		if (multiplier_num > 1)
		{
			if (startup || multiplier_num == 2)
			{
				neonStyle.multiplierSprite.PlayAnim("Times_" + multiplier_num);
				neonStyle.multiplierPanel.StartTransition(UIPanelManager.SHOW_MODE.BringInForward);
			}
			else
			{
				StartCoroutine(ReplaceMultiplier(multiplier_num));
			}
			AudioManager.PlaySFX(AudioManager.Instance.audioCollection.minigameWall.multiplierAddedHUD, 0f);
			ShowNotification("x" + multiplier_num + " Multiplier!!");
		}
	}

	public void ShowPauseScreen(bool bShow)
	{
		if (!GameData.MinigameObject.CurrentMinigame.IsEndScreenShown || !bShow)
		{
			CurrentSyle.standardHUDElements.pausePanel.StartTransition((!bShow) ? UIPanelManager.SHOW_MODE.DismissForward : UIPanelManager.SHOW_MODE.BringInForward);
			if (bShow)
			{
				GameData.GameCamera.BlurCamera();
			}
			else if (!GameData.MinigameObject.CurrentMinigame.IsEndScreenShown)
			{
				GameData.GameCamera.RemoveBlur();
			}
			AnimateFadePlane(bShow);
			CurrentSyle.standardHUDElements.pauseButton.Hide(bShow);
		}
	}

	public void ShowGUIGameButtons(bool bShow)
	{
		CurrentSyle.standardHUDElements.pauseButton.Hide(!bShow);
	}

	public void ShowGameGUI(bool bShow)
	{
		ShowLeftCurrentPanel(bShow);
		ShowPauseScreen(bShow: false);
		ShowGUIGameButtons(bShow);
		ShowScorePanel(bShow);
		if (!bShow)
		{
			HideMultiplier();
		}
		HideMedalPanel();
		if (GameData.MinigameObject.CurrentMinigame.UsingTimeLimit)
		{
			ShowTimerPanel(bShow);
		}
		GUIEnabled(bShow);
	}

	public static void GUIEnabled(bool bShow)
	{
		if (MinigameGUI.MakeGUIEnabled != null)
		{
			MinigameGUI.MakeGUIEnabled(bShow);
		}
	}

	public void ResetGameGUI()
	{
		ShowGameGUI(bShow: false);
		HideMultiplier();
	}

	public void ShowLoadingScreen()
	{
		ShowGameGUI(bShow: false);
		CurrentSyle.standardHUDElements.fadePlane.StartTransition(UIPanelManager.SHOW_MODE.DismissBack);
		ShowFadePlane(bShow: true);
		GameData.GameCamera.EnableLoadingLogo(bEnabled: true);
	}

	public bool IsFadingCamera()
	{
		return CurrentSyle.standardHUDElements.fadePlane.IsTransitioning();
	}

	public void AnimateFadePlane(bool bShow)
	{
		CurrentSyle.standardHUDElements.fadePlane.StartTransition((!bShow) ? UIPanelManager.SHOW_MODE.DismissForward : UIPanelManager.SHOW_MODE.BringInForward);
		ShowFadePlane(bShow);
	}

	public void ShowFadePlane(bool bShow)
	{
		CurrentSyle.standardHUDElements.fadePlane.transform.GetComponent<Renderer>().enabled = bShow;
	}

	public void ShowNotification(string text)
	{
		StartCoroutine(Notification(text));
	}

	private IEnumerator Notification(string text)
	{
		while (CurrentSyle.standardHUDElements.notificationPanel.IsTransitioning() || _isDisplayingNotification)
		{
			yield return null;
		}
		_isDisplayingNotification = true;
		StartCoroutine(AnimateHUDPanel(CurrentSyle.standardHUDElements.notificationPanel, 0f));
		CurrentSyle.standardHUDElements.notificationText.Text = text;
		yield return new WaitForSeconds(0.8f);
		_isDisplayingNotification = false;
		yield return true;
	}

	public void LoseMultiplier()
	{
		HideMultiplier();
		CurrentSyle.standardHUDElements.rightPanelScore.StartTransition(UIPanelManager.SHOW_MODE.BringInForward);
		AudioManager.PlaySFX(AudioManager.Instance.audioCollection.minigameWall.multiplierLost, 0f);
	}

	private void HideMultiplier()
	{
		neonStyle.multiplierPanel.StartTransition(UIPanelManager.SHOW_MODE.DismissForward);
	}

	private IEnumerator ReplaceMultiplier(int multiplier_num)
	{
		neonStyle.multiplierPanel.StartTransition(UIPanelManager.SHOW_MODE.DismissForward);
		yield return new WaitForSeconds(0.4f);
		neonStyle.multiplierSprite.PlayAnim("Times_" + multiplier_num);
		neonStyle.multiplierPanel.StartTransition(UIPanelManager.SHOW_MODE.BringInBack);
	}

	public void ChangeHeartState(int heart_num, bool full)
	{
		if (heart_num >= 0 && heart_num < CurrentSyle.standardHUDElements.heartsSprites.Length)
		{
			if (full)
			{
				CurrentSyle.standardHUDElements.heartsSprites[heart_num].PlayAnim(1);
			}
			else
			{
				CurrentSyle.standardHUDElements.heartsSprites[heart_num].PlayAnim(0);
			}
		}
	}

	public void EarnLifeAnimation()
	{
		StartCoroutine(AnimateHUDPanel(CurrentSyle.standardHUDElements.earnLifeAnim, 0f));
		ShowNotification("+1 Life");
	}

	public void LoseLifeAnimation()
	{
		StartCoroutine(AnimateHUDPanel(CurrentSyle.standardHUDElements.loseLifeAnim, 0f));
	}

	public void DisplayInstructions(string title, string content, float delay, Color contentColor)
	{
		neonStyle.instructionsTitle.Text = title;
		neonStyle.instructionsContent.Text = content;
		neonStyle.instructionsContent.Color = contentColor;
		StartCoroutine(AnimateMessage(neonStyle.instructionsPanel, 2f));
		HidePanels();
		GUIEnabled(bShow: false);
		Minigame.BlockTouchInput(bBlock: true);
	}

	public void InstructionsEffects()
	{
		StartCoroutine(StopInstructionsEffects());
	}

	private IEnumerator StopInstructionsEffects()
	{
		yield return new WaitForSeconds(1.5f);
		Minigame.BlockTouchInput(bBlock: false);
		GameData.GameCamera.RemoveBlur();
		GUIEnabled(bShow: true);
		ShowPanels();
	}

	public void ShowPanels()
	{
		_mainLeftPanel.BringIn();
		_mainRightPanel.BringIn();
	}

	public void HidePanels()
	{
		_mainLeftPanel.Dismiss();
		_mainRightPanel.Dismiss();
	}

	private IEnumerator AnimateMessage(UIPanel messagePanel, float transitionTime)
	{
		messagePanel.StartTransition(UIPanelManager.SHOW_MODE.BringInForward);
		yield return new WaitForSeconds(transitionTime);
		messagePanel.StartTransition(UIPanelManager.SHOW_MODE.DismissForward);
	}

	public static Spinner CountUpText(SpriteText spriteText, int currentValue, int targetValue, SFX soundFX = null, float speed = 3f)
	{
		if (spriteText != null)
		{
			Spinner spinner = spriteText.transform.GetComponent<Spinner>();
			if (spinner == null)
			{
				spinner = spriteText.gameObject.AddComponent<Spinner>();
			}
			spinner.InitialValue = currentValue;
			spinner.TargetValue = targetValue;
			spinner.Speed = speed;
			spinner.SoundEnabled = soundFX != null;
			spinner.StartRunning(soundFX);
			return spinner;
		}
		return null;
	}

	public void AnimateMatchFitnessStamp(int matchFitness)
	{
	}

	public void RevealScoreWithDelay(int score, bool bonusColor = false)
	{
		neonStyle.scoreNotification.Text = "+" + score.ToString("#,##0");
		neonStyle.scoreNotification.Color = ((!bonusColor) ? new Color(78f / 85f, 1f, 11f / 51f, 1f) : new Color(0f, 78f / 85f, 1f, 1f));
		StartCoroutine(AnimateHUDPanel(neonStyle.scoreNotificationPanel, 0f));
	}

	public IEnumerator AnimateHUDPanel(UIPanel panelObject, float holdDelay = 0f)
	{
		if ((bool)panelObject.transform.GetComponent<Renderer>())
		{
			panelObject.transform.GetComponent<Renderer>().enabled = true;
		}
		panelObject.StartTransition(UIPanelManager.SHOW_MODE.BringInForward);
		while (panelObject.IsTransitioning())
		{
			yield return null;
		}
		yield return new WaitForSeconds(holdDelay);
		panelObject.Dismiss();
		while (panelObject.IsTransitioning())
		{
			yield return null;
		}
		if ((bool)panelObject.transform.GetComponent<Renderer>())
		{
			panelObject.transform.GetComponent<Renderer>().enabled = false;
		}
	}

	public void ShowTutorialMessage(string message, float delay)
	{
		StartCoroutine(TutorialMessage(message, delay));
	}

	public void DismissTutorialText()
	{
		neonStyle.tutorialMessagePanel.StartTransition(UIPanelManager.SHOW_MODE.DismissBack);
		neonStyle.tutorialResultPanel.StartTransition(UIPanelManager.SHOW_MODE.DismissBack);
	}

	public void ShowTutorialGesture(bool bShow)
	{
		if (bShow)
		{
			StartCoroutine("ShowGesture");
		}
		else
		{
			StopCoroutine("ShowGesture");
		}
	}

	private IEnumerator ShowGesture()
	{
		MinigameWall wallGame = (MinigameWall)GameData.MinigameObject.CurrentMinigame;
		if (wallGame != null && wallGame.CurrentTutorial() != null)
		{
			while (true)
			{
				wallGame.CurrentTutorial().ShowGesture();
				yield return new WaitForSeconds(2f);
				yield return null;
			}
		}
	}

	private IEnumerator TutorialMessage(string message, float delay)
	{
		while (!_finishedIntro)
		{
			yield return null;
		}
		if (!(message == string.Empty))
		{
			yield return new WaitForSeconds(delay);
			neonStyle.tutorialMessage.Text = message;
			StartCoroutine(AnimateHUDPanel(neonStyle.tutorialMessagePanel, 0f));
			yield return new WaitForSeconds(1.3f);
			MinigameWall wallGame = (MinigameWall)GameData.MinigameObject.CurrentMinigame;
			if (wallGame != null)
			{
				ShowTutorialGesture(bShow: true);
			}
		}
	}

	public void ShowTutorialResult(string message, Color textColor, float holdDelay, bool finalResult = false)
	{
		if (message == string.Empty)
		{
			return;
		}
		neonStyle.tutorialResult.Color = textColor;
		neonStyle.tutorialResult.Text = message;
		if (finalResult)
		{
			neonStyle.tutorialResultPanel.StartTransition(UIPanelManager.SHOW_MODE.BringInBack);
			return;
		}
		StartCoroutine(AnimateHUDPanel(neonStyle.tutorialResultPanel, holdDelay));
		if (textColor == Color.white)
		{
			StartCoroutine(SpawnPrefabAtPointWithDelay(neonStyle.textDustPrefab, Vector3.zero, 0.3f));
		}
	}

	private IEnumerator SpawnPrefabAtPointWithDelay(GameObject prefab, Vector3 point, float delay = 0f)
	{
		yield return new WaitForSeconds(delay);
		GameObject newGO = (GameObject)UnityEngine.Object.Instantiate(prefab);
		if (point != Vector3.zero)
		{
			newGO.transform.position = point;
		}
	}

	public void RevealComboBonus(int comboHitNumber)
	{
		neonStyle.comboBonusText.Text = "Combo Bonus x" + comboHitNumber;
		StartCoroutine(AnimateHUDPanel(neonStyle.comboBonusPanel, 0f));
	}

	public void SetCurlBonus(int curlBonus)
	{
		_acummulatedCurl += curlBonus;
		CurrentSyle.standardHUDElements.curlBonusText.Text = string.Empty + _acummulatedCurl;
		_shouldShowCurl = true;
	}

	public void SetStreakBonus(int streakBonus, int streakCount)
	{
		CurrentSyle.standardHUDElements.streakBonusTitle.Text = string.Empty + streakCount + " IN A ROW";
		CurrentSyle.standardHUDElements.streakBonusText.Text = string.Empty + streakBonus;
		_shouldShowStreak = true;
	}

	public void ShowBonus(float holdDelay = 0f)
	{
		if (_shouldShowCurl)
		{
			StartCoroutine(AnimateHUDPanel(CurrentSyle.standardHUDElements.curlBonusPanel, holdDelay));
			_acummulatedCurl = 0;
			_shouldShowCurl = false;
			if (GameData.PlayerControl.CurlApplied >= 0.95f)
			{
				ShowNotification("Big Curl!!");
			}
		}
		if (_shouldShowStreak)
		{
			AudioManager.PlaySFX(AudioManager.Instance.audioCollection.CurrentGame.standardAudio.streakBonus, holdDelay);
			StartCoroutine(AnimateHUDPanel(CurrentSyle.standardHUDElements.streakBonusPanel, 0f));
			_shouldShowStreak = false;
		}
	}

	public void ShowThresholdToBeat(int currentScore, int bronze, int silver, int gold, int gameScore = 0)
	{
		if (gameScore > currentScore)
		{
			currentScore = gameScore;
		}
		if (currentScore < bronze)
		{
			SetMedal(MEDAL.BRONZE, bronze);
		}
		else if (currentScore < silver)
		{
			SetMedal(MEDAL.SILVER, silver);
		}
		else if (currentScore < gold)
		{
			SetMedal(MEDAL.GOLD, gold);
		}
		else
		{
			neonStyle.medalPanel.Dismiss();
		}
	}

	private void SetMedal(MEDAL medal, int medalScore)
	{
		string text = "bronze";
		Color color = Color.grey;
		switch (medal)
		{
		case MEDAL.BRONZE:
			text = "bronze";
			color = new Color(1f, 0.6509804f, 0f, 1f);
			break;
		case MEDAL.SILVER:
			text = "silver";
			color = new Color(40f / 51f, 40f / 51f, 40f / 51f, 1f);
			break;
		case MEDAL.GOLD:
			text = "gold";
			color = new Color(1f, 44f / 51f, 0f, 1f);
			break;
		}
		neonStyle.medalScore.Text = string.Empty + medalScore.ToString("#,##0");
		neonStyle.medalScore.Color = color;
		neonStyle.medalImage.PlayAnim(text);
		neonStyle.medalPanel.BringIn();
	}

	private void HideMedalPanel()
	{
		neonStyle.medalPanel.Dismiss();
	}

	public void BringStampText(string message, float offsetX = 0f, float offsetY = 0f)
	{
		CurrentSyle.standardHUDElements.stampTextMessage.Text = message;
		CurrentSyle.standardHUDElements.stampContainer.transform.localPosition = new Vector3(offsetX, offsetY, 0f);
		CurrentSyle.standardHUDElements.stampText.BringIn();
	}

	public void DismissStampText()
	{
		CurrentSyle.standardHUDElements.stampTextMessage.Text = string.Empty;
		CurrentSyle.standardHUDElements.stampContainer.transform.position = Vector3.zero;
		CurrentSyle.standardHUDElements.stampText.Dismiss();
	}

	public static void GreyOutButton(UIButton button, bool bGrayOut)
	{
		if (!(button == null))
		{
			button.Color = new Color(1f, 1f, 1f, (!bGrayOut) ? 1f : 0.3f);
			GreyOutSpriteText(button.spriteText, bGrayOut);
			button.controlIsEnabled = !bGrayOut;
		}
	}

	public static void GreyOutSpriteText(SpriteText spriteText, bool bGrayOut)
	{
		if (!(spriteText == null))
		{
			spriteText.Color = new Color(0f, 0f, 0f, (!bGrayOut) ? 1f : 0.3f);
		}
	}

	public static void GreyOutPackedSprite(PackedSprite packedSprite, bool bGrayOut)
	{
		if (!(packedSprite == null))
		{
			packedSprite.SetColor(new Color(1f, 1f, 1f, (!bGrayOut) ? 1f : 0.3f));
		}
	}
}
