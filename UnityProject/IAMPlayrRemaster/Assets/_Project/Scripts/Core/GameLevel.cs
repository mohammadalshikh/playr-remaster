using UnityEngine;

public class GameLevel
{
	public DIFFICULTY difficultyLevel = DIFFICULTY.NORMAL;

	[SerializeField]
	protected ObstacleTarget.ID _obstacle;

	[SerializeField]
	private bool _allowLastObstacle;

	public LevelSettings _settings = new LevelSettings();

	protected int _indexID;

	protected bool _hasBeenPlayed;

	protected int _currentHits;

	protected int _availableRounds;

	private bool _isInitialized;

	public int IndexID
	{
		get
		{
			return _indexID;
		}
		set
		{
			_indexID = value;
		}
	}

	public bool HasBeenPlayed
	{
		get
		{
			return _hasBeenPlayed;
		}
		set
		{
			_hasBeenPlayed = value;
		}
	}

	public int CurrentHits
	{
		get
		{
			return _currentHits;
		}
		set
		{
			_currentHits = value;
		}
	}

	public bool IsRoundBased
	{
		get
		{
			if (_settings.rounds > 0)
			{
				return true;
			}
			return false;
		}
	}

	public virtual int CurrentTries => _settings.rounds - _availableRounds;

	public int AvailableRounds => _availableRounds;

	public bool DisplaySuccessText => (_settings.roundInformation != string.Empty) ? true : false;

	public virtual float Distance => _settings.distance;

	public virtual float Range => _settings.positionRange;

	public ObstacleTarget.ID Obstacle
	{
		get
		{
			return _obstacle;
		}
		set
		{
			_obstacle = value;
		}
	}

	public bool AllowLastObstacle
	{
		get
		{
			return _allowLastObstacle;
		}
		set
		{
			_allowLastObstacle = value;
		}
	}

	public bool IsInitialized => _isInitialized;

	public virtual bool CanPassNextRound
	{
		get
		{
			if (_currentHits >= _settings.successHits)
			{
				return true;
			}
			return false;
		}
	}

	public bool CanDisplayIntroMessage => !_settings.roundInformation.Equals(string.Empty);

	public virtual bool AllTargetsHit => false;

	public virtual void Init()
	{
	}

	public virtual void Reset()
	{
		_currentHits = 0;
		_isInitialized = false;
	}

	protected void InitialSetup()
	{
		if (!_isInitialized)
		{
			_isInitialized = true;
		}
		_availableRounds = _settings.rounds;
		SetupObstacles();
	}

	protected virtual void SetupObstacles()
	{
		if (!(ObstacleManager.Instance == null))
		{
			if (!_allowLastObstacle)
			{
				RemoveAllObstacles();
			}
			if (_obstacle != 0)
			{
				BringInObstacle();
			}
		}
	}

	protected virtual void BringInObstacle()
	{
	}

	protected virtual void RemoveAllObstacles()
	{
	}

	public virtual void UpdateLevel()
	{
	}

	public virtual void Finish()
	{
		_hasBeenPlayed = true;
	}

	public virtual bool ShouldEnd()
	{
		if (CurrentTries >= _settings.rounds && IsRoundBased)
		{
			return true;
		}
		return false;
	}

	public virtual void AdditionalBehaviours()
	{
	}
}
