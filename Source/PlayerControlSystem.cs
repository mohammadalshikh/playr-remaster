using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class PlayerControlSystem : MonoBehaviour, ICRTouchListener
{
	public delegate void HandleKickedBall();

	public const float MIN_FLICK_THRESHOLD = 20f;

	private const int SHOOT_STATUS_ENABLED = 1;

	private const int SHOOT_STATUS_STARTED = 2;

	private const int SHOOT_STATUS_CURL = 4;

	private const int SHOOT_STATUS_KICKED = 8;

	public CONTROL_SYSTEM currentSystem;

	public List<ControlMechanism> controlSystems = new List<ControlMechanism>();

	private int m_status;

	private bool m_following_ball;

	private float m_curl_applied;

	private bool m_controlEnabled = true;

	public int Status => m_status;

	public float CurlApplied
	{
		get
		{
			return m_curl_applied;
		}
		set
		{
			m_curl_applied = value;
		}
	}

	public bool CameraFollow
	{
		set
		{
			m_following_ball = value;
		}
	}

	public ControlMechanism CurrenSystem
	{
		get
		{
			if (controlSystems.Count > 0)
			{
				return controlSystems[(int)currentSystem];
			}
			Debug.LogError("No control mechanisms were found, please assign at least one in PlayerControlSystem from the inspector");
			return null;
		}
	}

	[method: MethodImpl(32)]
	public static event HandleKickedBall KickedBallEvent;

	private void Awake()
	{
		GameData.CheckInstance();
		GameData.PlayerControl = this;
	}

	private void OnEnable()
	{
		CR_TouchManager.AddListener(this);
	}

	private void OnDisable()
	{
		CR_TouchManager.RemoveListener(this);
	}

	public void Reset()
	{
		m_status = 0;
		m_curl_applied = 0f;
		GameData.GameCamera.ResetPosition();
		CurrenSystem.Reset();
		m_controlEnabled = true;
	}

	private void FixedUpdate()
	{
		if (m_following_ball && GameData.CameraTransform.position.x < GameData.MinigameObject.CurrentMinigame.CameraFollowDistance)
		{
			GameData.MinigameObject.CurrentMinigame.CameraFollow();
		}
	}

	public bool IsShootingEnabled()
	{
		return (m_status & 1) != 0;
	}

	public bool IsKicking()
	{
		return (m_status & 2) != 0;
	}

	public bool HasKicked()
	{
		return (m_status & 8) != 0;
	}

	public bool CanShoot(Vector2 touchStart, Vector2 touchEnd)
	{
		return IsKicking() && ValidFlick(touchStart, touchEnd);
	}

	public bool ValidFlick(Vector2 touchStart, Vector2 touchEnd)
	{
		return touchEnd.y - touchStart.y > 20f;
	}

	public void StartShooting()
	{
		m_status |= 2;
	}

	public void KickBall(Vector2 touchStart, Vector2 touchEnd)
	{
		m_status &= -7;
		m_status |= 8;
		EnableShoot(bEnable: false);
		GameData.Ball.ShootBall(touchStart, touchEnd);
		GameData.PlayerStats.TotalShots++;
		CurrenSystem.Kick();
		GameData.MinigameObject.CurrentMinigame.BallKicked();
		if (PlayerControlSystem.KickedBallEvent != null)
		{
			PlayerControlSystem.KickedBallEvent();
		}
	}

	public void FollowBall(bool bEnabled)
	{
		if (bEnabled)
		{
			DollyEffect.Instance.Init();
		}
		else
		{
			DollyEffect.Instance.Stop();
		}
		m_following_ball = bEnabled;
	}

	public void Kicked()
	{
		m_status &= -7;
		m_status |= 8;
	}

	public void EnableShoot(bool bEnable)
	{
		if (bEnable == IsShootingEnabled())
		{
			return;
		}
		if (bEnable)
		{
			if ((m_status & 1) == 0)
			{
				m_status = 1;
				CurrenSystem.ShotEnabled();
			}
		}
		else if (((uint)m_status & (true ? 1u : 0u)) != 0)
		{
			m_status &= -4;
			CurrenSystem.ShotDisabled();
		}
	}

	public void EnableControls(bool bEnabled)
	{
		m_controlEnabled = bEnabled;
	}

	public void OnTouchBegan(List<Touch> touches)
	{
		if (m_controlEnabled)
		{
			CurrenSystem.TouchBegan(touches);
		}
	}

	public void OnTouchMoved(List<Touch> touches)
	{
		if (m_controlEnabled)
		{
			CurrenSystem.TouchMoved(touches);
		}
	}

	public void OnTouchEnded(List<Touch> touches)
	{
		if (m_controlEnabled)
		{
			CurrenSystem.TouchEnded(touches);
		}
	}

	public void OnTouchCanceled(List<Touch> touches)
	{
		if (m_controlEnabled)
		{
			CurrenSystem.TouchCanceled(touches);
		}
	}
}
