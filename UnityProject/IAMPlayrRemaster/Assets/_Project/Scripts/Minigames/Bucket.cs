using System.Collections.Generic;
using UnityEngine;

public class Bucket : Minigame_TargetTrigger
{
	public int m_basePoints = 500;

	public int m_innerRing = 250;

	public int m_outterRing = 100;

	public MinigameTriggerPoints pointsController;

	private BUCKET_CURRENT_SCORING _currentScoring;

	public Animation m_netAnimation;

	public SpriteText m_notification3DPrefab;

	private bool _isActive = true;

	private bool _hasBeenHit;

	private Rect _containerRect;

	private bool _powerUpAssigned;

	private List<GameObject> _notificationsPool = new List<GameObject>();

	private float _currentNotificationHeight;

	public bool IsActive => _isActive;

	public bool HasBeenHit => _hasBeenHit;

	public Rect ContainerRect => _containerRect;

	public BUCKET_CURRENT_SCORING CurrentScoring
	{
		get
		{
			return _currentScoring;
		}
		set
		{
			_currentScoring = value;
		}
	}

	public bool PowerUpAssigned
	{
		get
		{
			return _powerUpAssigned;
		}
		set
		{
			_powerUpAssigned = value;
		}
	}

	public int Score => _currentScoring switch
	{
		BUCKET_CURRENT_SCORING.OUTER => m_outterRing, 
		BUCKET_CURRENT_SCORING.INNER => m_innerRing, 
		BUCKET_CURRENT_SCORING.BUCKET => m_basePoints, 
		BUCKET_CURRENT_SCORING.SWEET => m_basePoints * 2, 
		_ => 0, 
	};

	public string Celebration => _currentScoring switch
	{
		BUCKET_CURRENT_SCORING.OUTER => "NOT BAD", 
		BUCKET_CURRENT_SCORING.INNER => "GOOD AIM", 
		BUCKET_CURRENT_SCORING.BUCKET => "GREAT SHOT!", 
		BUCKET_CURRENT_SCORING.SWEET => "CLEAN SHOT!", 
		_ => "TOO BAD!!", 
	};

	public string ScoringText => _currentScoring switch
	{
		BUCKET_CURRENT_SCORING.OUTER => "OUTER RING", 
		BUCKET_CURRENT_SCORING.INNER => "INNER RING", 
		BUCKET_CURRENT_SCORING.BUCKET => "GOOD AIMING", 
		BUCKET_CURRENT_SCORING.SWEET => "SWEET, RIGHT IN THE BUCKET", 
		_ => "KEEP PRACTICING", 
	};

	private void Start()
	{
		SetPoints(m_basePoints);
		RegisterDelegates(bRegister: true);
		ActivateBucket(bActivate: true);
	}

	public void SetPoints(int points)
	{
		m_basePoints = points;
		m_innerRing = (int)((float)m_basePoints * 0.5f);
		m_outterRing = (int)((float)m_basePoints * 0.2f);
		pointsController.SetText(m_basePoints);
	}

	public void SetPosition(Vector2 newPosition)
	{
		base.transform.position = new Vector3(newPosition.y, 0f, newPosition.x);
		_containerRect = new Rect(-4f, -4f, 8f, 8f);
		_containerRect.center = new Vector2(base.transform.position.x, base.transform.position.y);
		SetPoints(CalculateScore(newPosition));
	}

	public void ActivateBucket(bool bActivate)
	{
		base.GetComponent<Collider>().enabled = bActivate;
		_isActive = bActivate;
	}

	public new void OnTriggerEnter(Collider collider)
	{
		_hasBeenHit = true;
		_isActive = false;
		AnimateNet();
		collider.attachedRigidbody.velocity = Vector3.zero;
		_currentScoring = BUCKET_CURRENT_SCORING.BUCKET;
		base.OnTriggerEnter(collider);
	}

	public void AnimateNet()
	{
		if (!m_netAnimation.isPlaying)
		{
			m_netAnimation.Play();
		}
	}

	protected override void OnAction()
	{
		base.OnAction();
	}

	public void ShowNotification(string notificationText, float destroyDelay = 0f, float ExtraOffset = 0f)
	{
		Vector3 position = base.transform.position;
		position.y = _currentNotificationHeight + ExtraOffset;
		GameObject gameObject = (GameObject)Object.Instantiate(m_notification3DPrefab.gameObject, position, Quaternion.identity);
		_notificationsPool.Add(gameObject);
		gameObject.transform.parent = base.transform;
		gameObject.GetComponent<SpriteText>().Text = notificationText;
		gameObject.GetComponent<MovePosition>().m_destroyAfter = destroyDelay;
		_currentNotificationHeight += 1f;
		if (_currentNotificationHeight >= 4f)
		{
			_currentNotificationHeight = 0f;
		}
	}

	public void ScoreReset()
	{
		_currentScoring = BUCKET_CURRENT_SCORING.NONE;
	}

	private void LevelUpdateDelegate()
	{
		DestroyNotifications();
		_powerUpAssigned = false;
		if (!IsActive)
		{
			RemoveAndDestroy();
		}
	}

	public void DestroyNotifications()
	{
		foreach (GameObject item in _notificationsPool)
		{
			if (item != null)
			{
				Object.Destroy(item);
			}
		}
	}

	private void RemoveAndDestroy()
	{
		RegisterDelegates(bRegister: false);
		if (this != null)
		{
			Object.Destroy(base.gameObject);
		}
	}

	private void RegisterDelegates(bool bRegister)
	{
		MinigamePassing minigamePassing = (MinigamePassing)GameData.MinigameObject.CurrentMinigame;
		if (bRegister)
		{
			minigamePassing.CurrentBucketLevel().OnLevelUpdate += LevelUpdateDelegate;
		}
		else
		{
			minigamePassing.CurrentBucketLevel().OnLevelUpdate -= LevelUpdateDelegate;
		}
	}

	private int CalculateScore(Vector2 currentPosition)
	{
		int num = (int)Mathf.Lerp(500f, 3000f, Mathf.InverseLerp(0f, 15f, Mathf.Abs(currentPosition.x))) / 2;
		int num2 = (int)Mathf.Lerp(500f, 3000f, Mathf.InverseLerp(10f, 40f, currentPosition.y)) / 2;
		return num + num2;
	}
}
