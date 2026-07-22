using System;
using UnityEngine;

[Serializable]
public class NeonGUIStyle : GameGUIStyle
{
	public UIPanel instructionsPanel;

	public SpriteText instructionsTitle;

	public SpriteText instructionsContent;

	public SpriteText scoreNotification;

	public UIPanel scoreNotificationPanel;

	public UIPanel comboBonusPanel;

	public SpriteText comboBonusText;

	public UIPanel tutorialMessagePanel;

	public SpriteText tutorialMessage;

	public UIPanel tutorialResultPanel;

	public SpriteText tutorialResult;

	public UIPanel medalPanel;

	public PackedSprite medalImage;

	public SpriteText medalScore;

	public PackedSprite successDotPrefab;

	public UIPanel multiplierPanel;

	public PackedSprite multiplierSprite;

	public GameObject textDustPrefab;
}
