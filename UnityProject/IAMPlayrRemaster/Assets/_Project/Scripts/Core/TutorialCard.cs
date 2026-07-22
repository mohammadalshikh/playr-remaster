using System;
using UnityEngine;

public class TutorialCard : MonoBehaviour
{
	private bool m_Flipped;

	public static TutorialCard Instance;

	public UISlider m_DifficultySlider;

	public SpriteText m_DifficultyText;

	private void FlipBack()
	{
		iTween.RotateTo(base.gameObject, iTween.Hash("y", 0f, "time", 1.5f, "islocal", true, "ignoretimescale", true, "easetype", iTween.EaseType.easeOutElastic));
		m_Flipped = !m_Flipped;
	}

	private void FlipCard()
	{
		InitCard();
		iTween.RotateTo(base.gameObject, iTween.Hash("y", 180f, "time", 1.5f, "islocal", true, "ignoretimescale", true, "easetype", iTween.EaseType.easeOutElastic));
		m_Flipped = !m_Flipped;
	}

	public void InitCard()
	{
		m_DifficultyText.Text = GameData.CurrentDifficulty.ToString();
	}

	public void DifficultyChanged(IUIObject obj)
	{
		int num = (int)(GameData.CurrentDifficulty = (DIFFICULTY)Mathf.Clamp((int)(m_DifficultySlider.Value * (float)(Enum.GetValues(typeof(DIFFICULTY)).Length - 1)) + 1, 1, 4));
		InitCard();
	}

	private void Destroy()
	{
		m_DifficultySlider.RemoveValueChangedDelegate(DifficultyChanged);
	}

	private void Start()
	{
		if (GameData.CurrentDifficulty == DIFFICULTY.NONE)
		{
			GameData.CurrentDifficulty = DIFFICULTY.EASY;
		}
		InitCard();
		float value = (float)(GameData.CurrentDifficulty - 1) / 4f;
		m_DifficultySlider.Value = value;
	}

	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
		}
		m_DifficultySlider.AddValueChangedDelegate(DifficultyChanged);
	}
}
