using System;
using UnityEngine;

[Serializable]
public class ScoreStreakModifier
{
	public int m_streak_number = 1;

	public int m_streak_points = 100;

	public Color m_bonus_text_color = new Color(0.99607843f, 0.39607844f, 0f);
}
