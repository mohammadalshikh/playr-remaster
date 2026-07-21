using System;
using UnityEngine;

[Serializable]
public class HUDElements
{
	public UIPanel leftPanelShots;

	public SpriteText shotsText;

	public GameObject roundDotPrefab;

	public UIPanel leftPanelLives;

	public PackedSprite[] heartsSprites;

	public UIPanel shieldPanelBackdrop;

	public UIPanel shieldPanelIcon;

	public PackedSprite shieldSprite;

	public UIPanel earnLifeAnim;

	public UIPanel loseLifeAnim;

	public UIPanel rightPanelScore;

	public SpriteText scoreText;

	public UIPanel leftPanelRush;

	public SpriteText rushScore;

	public UIPanel timerPanel;

	public SpriteText timerText;

	public PackedSprite timerIcon;

	public UIPanel addTimeAnimation;

	public SpriteText addTimeText;

	public UIPanel shieldPanel;

	public UIPanel pausePanel;

	public UIButton pauseButton;

	public UIButton tryAgainPause;

	public UIButton resumePause;

	public UIButton quitPause;

	public SpriteText introText;

	public SpriteText introCountText;

	public UIPanel introTextPanel;

	public UIPanel fadePlane;

	public UIPanel curlBonusPanel;

	public SpriteText curlBonusText;

	public UIPanel streakBonusPanel;

	public SpriteText streakBonusText;

	public SpriteText streakBonusTitle;

	public UIPanel notificationPanel;

	public SpriteText notificationText;

	public UIPanel stampText;

	public SpriteText stampTextMessage;

	public Transform stampContainer;
}
