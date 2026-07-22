using System.Collections.Generic;
using UnityEngine;

public class ShootControlSystem : ControlMechanism
{
	private const float FLICK_TIME_WINDOW = 0.1f;

	private const float TOUCH_HORIZONTAL_RANGE = 100f;

	private const float TOUCH_VERTICAL_LIMIT = 400f;

	private Vector2 m_initial_touch;

	private float m_flick_timer;

	private float m_curl_applied;

	private bool m_boosterEnabled;

	private Vector2 TouchAreaMinimum()
	{
		return new Vector2((float)(Screen.width / 2) - 100f, 0f);
	}

	private Vector2 TouchAreaMaximum()
	{
		return new Vector2((float)(Screen.width / 2) + 100f, 400f);
	}

	private bool IsValidTouch(Vector2 touchPosition)
	{
		if (touchPosition.x > TouchAreaMinimum().x && touchPosition.x < TouchAreaMaximum().x && touchPosition.y > TouchAreaMinimum().y && touchPosition.y < TouchAreaMaximum().y)
		{
			return true;
		}
		return false;
	}

	public override void Reset()
	{
		m_initial_touch = Vector2.zero;
		m_flick_timer = 0f;
		m_curl_applied = 0f;
		m_boosterEnabled = false;
		GameData.PlayerControl.FollowBall(bEnabled: false);
	}

	public override void ShotEnabled()
	{
	}

	public override void ShotDisabled()
	{
	}

	public override void TouchBegan(List<Touch> touches)
	{
		if (touches.Count == 1)
		{
			if (GameData.PlayerControl.IsShootingEnabled() && IsValidTouch(touches[0].position))
			{
				GameData.PlayerControl.StartShooting();
				m_initial_touch = touches[0].position;
			}
			else if (!GameData.PlayerControl.HasKicked())
			{
			}
		}
	}

	public override void TouchMoved(List<Touch> touches)
	{
		if (GameData.PlayerControl.CanShoot(m_initial_touch, touches[0].position))
		{
			if (!IsFlicking())
			{
				GameData.PlayerControl.KickBall(m_initial_touch, touches[0].position);
			}
		}
		else if (GameData.PlayerControl.HasKicked())
		{
			ApplyCurl(touches[0].deltaPosition);
		}
	}

	public override void TouchEnded(List<Touch> touches)
	{
		if (GameData.PlayerControl.HasKicked())
		{
			m_boosterEnabled = true;
		}
		else if (GameData.PlayerControl.CanShoot(m_initial_touch, touches[0].position))
		{
			GameData.PlayerControl.KickBall(m_initial_touch, touches[0].position);
		}
	}

	public override void TouchCanceled(List<Touch> touches)
	{
	}

	private bool IsFlicking()
	{
		m_flick_timer += Time.deltaTime;
		return m_flick_timer <= 0.1f;
	}

	private void ApplyCurl(Vector2 deltaDirection)
	{
		float maxFlickForCurl = GameData.MinigameObject.CurrentMinigame.MaxFlickForCurl;
		if (m_curl_applied < Mathf.Abs(deltaDirection.x) / maxFlickForCurl)
		{
			m_curl_applied = Mathf.Clamp01(Mathf.Abs(deltaDirection.x) / maxFlickForCurl);
			GameData.PlayerControl.CurlApplied = m_curl_applied;
		}
		float magnitude = GameData.MinigameObject.CurrentMinigame.CurlAppliedWithDelta(deltaDirection);
		Vector2 normalized = deltaDirection.normalized;
		if (Mathf.Abs(normalized.x) > 0.5f)
		{
			if (!(Mathf.Abs(normalized.x) < Mathf.Abs(normalized.y)))
			{
				GameData.Ball.AddCurl(normalized, magnitude);
			}
		}
		else if (m_boosterEnabled)
		{
			GameData.Ball.ApplyBooster(normalized.y);
		}
	}

	public override void Kick()
	{
		GameData.PlayerControl.FollowBall(bEnabled: true);
	}
}
