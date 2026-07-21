using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

[Serializable]
public class PassingLevel : GameLevel
{
	public delegate void HandleLevelUpdate();

	public const float BUCKET_SPACING = 8f;

	public BucketSetup bucketsSetup = new BucketSetup();

	private List<Bucket> _bucketList = new List<Bucket>();

	private List<PassingTarget> _activeTargets = new List<PassingTarget>();

	private int _currentActive;

	private FlyingTarget _currentMultiplier;

	private List<GridSpawnPos> _gridPositions = new List<GridSpawnPos>();

	private List<FlyingTarget> _activePowerUps = new List<FlyingTarget>();

	private int[] rangePositionsX;

	private int[] rangePositionsY;

	public float MaxDistanceRange => bucketsSetup.distance + (float)bucketsSetup.areaDepth * 8f;

	public float MinDistanceRange => bucketsSetup.distance;

	[method: MethodImpl(32)]
	public event Action OnLevelUpdate;

	[method: MethodImpl(32)]
	public static event HandleLevelUpdate LevelUpdateEvent;

	public override void Init()
	{
		InitialSetup();
		InitGridPosition();
		for (int i = 0; i < _settings.roundTargets; i++)
		{
			CreateTarget();
		}
	}

	private void InitGridPosition()
	{
		for (int i = 0; i < bucketsSetup.areaWidth; i++)
		{
			for (int j = 0; j < bucketsSetup.areaDepth; j++)
			{
				GridSpawnPos gridSpawnPos = new GridSpawnPos();
				gridSpawnPos.position = new Vector2(((float)i - (float)bucketsSetup.areaWidth / 2f) * 8f, (float)j * 8f + bucketsSetup.distance);
				_gridPositions.Add(gridSpawnPos);
			}
		}
	}

	public override void UpdateLevel()
	{
		_availableRounds--;
		BringPowerUps();
		if (PassingLevel.LevelUpdateEvent != null)
		{
			PassingLevel.LevelUpdateEvent();
		}
	}

	public override void Finish()
	{
		RemoveTargets();
		PowerUpCleanUp();
		base.Finish();
	}

	public override void Reset()
	{
		base.Reset();
		Finish();
	}

	public void RemoveTargets()
	{
		foreach (PassingTarget activeTarget in _activeTargets)
		{
			if (activeTarget != null)
			{
				activeTarget.RemoveFromView();
			}
		}
		_activeTargets.Clear();
		DestroyMultiplier();
	}

	private void CreateBucket()
	{
	}

	private void CreateTarget()
	{
		PassingTarget passingTarget = (PassingTarget)UnityEngine.Object.Instantiate(GameData.MinigameObject.BucketData.targetPrefab);
		passingTarget.gameObject.name = "Target" + _activeTargets.Count;
		GameData.Instance.StartCoroutine(AssignGridPosition(passingTarget));
		_activeTargets.Add(passingTarget);
	}

	public void TargetCleanup(PassingTarget target)
	{
		_activeTargets.Remove(target);
	}

	public bool AllBucketsHit()
	{
		return _activeTargets.Count == 0;
	}

	public List<Bucket> CurrentBucketScoring()
	{
		List<Bucket> list = new List<Bucket>();
		foreach (Bucket bucket in _bucketList)
		{
			if (bucket.CurrentScoring != 0)
			{
				list.Add(bucket);
			}
		}
		return list;
	}

	private void CheckMultiplierState()
	{
		if (_currentMultiplier != null)
		{
			DestroyMultiplier();
		}
		_currentMultiplier = null;
		if (GameData.ProbabilityTest(bucketsSetup.multiplierChance) && _currentMultiplier == null)
		{
			_currentMultiplier = (FlyingTarget)UnityEngine.Object.Instantiate(GameData.MinigameObject.BucketData.multiplierHoopPrefab);
			PassingTarget passingTarget = RandomActiveBucket();
			if (passingTarget != null)
			{
				Vector3 vector = (GameData.MinigameObject.CurrentMinigame.PlayerPosition - GameData.Ball.transform.position) / 2f;
				_currentMultiplier.transform.position = new Vector3(vector.x, _currentMultiplier.transform.position.y, vector.z + 2.5f);
			}
		}
	}

	private void PowerUpCleanUp()
	{
		if (_activePowerUps.Count <= 0)
		{
			return;
		}
		foreach (FlyingTarget activePowerUp in _activePowerUps)
		{
			if (activePowerUp != null)
			{
				UnityEngine.Object.Destroy(activePowerUp.gameObject);
			}
		}
		_activePowerUps.Clear();
	}

	private void BringPowerUps()
	{
		PowerUpCleanUp();
		IEnumerator enumerator = Enum.GetValues(typeof(SCORE_TYPE)).GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				switch ((int)enumerator.Current)
				{
				case 3:
					if (GameData.ProbabilityTest(bucketsSetup.multiplierChance + GameData.MinigameObject.CurrentMinigame.GetBonusProbabiltyForType(POWER_UP.MULTIPLIER_BONUS)))
					{
						CreatePowerUp(GameData.MinigameObject.BucketData.multiplierHoopPrefab, GameData.MinigameObject.CurrentMinigame.CurrentMultiplier - 1);
					}
					break;
				case 2:
					if (GameData.ProbabilityTest(GameData.MinigameObject.CurrentMinigame.GetBonusProbabiltyForType(POWER_UP.TIME_BONUS)))
					{
						CreatePowerUp(GameData.MinigameObject.BucketData.timerPrefab);
					}
					break;
				case 1:
					if (GameData.ProbabilityTest(GameData.MinigameObject.CurrentMinigame.GetBonusProbabiltyForType(POWER_UP.HEART_BONUS)))
					{
						CreatePowerUp(GameData.MinigameObject.BucketData.heartPrefab);
					}
					break;
				}
			}
		}
		finally
		{
			IDisposable disposable = enumerator as IDisposable;
			if (disposable != null)
			{
				disposable.Dispose();
			}
		}
	}

	private FlyingTarget CreatePowerUp(FlyingTarget prefab, int frameIndex = 0)
	{
		FlyingTarget flyingTarget = (FlyingTarget)UnityEngine.Object.Instantiate(prefab);
		_activePowerUps.Add(flyingTarget);
		flyingTarget.SetupTexture(frameIndex);
		PassingTarget passingTarget = RandomActiveBucket();
		if (passingTarget != null)
		{
			passingTarget.PowerUpAssigned = true;
			Vector3 vector = (passingTarget.transform.position + GameData.Ball.transform.position) / 2f;
			flyingTarget.transform.position = new Vector3(vector.x, flyingTarget.transform.position.y, vector.z + 2.5f);
		}
		return flyingTarget;
	}

	public void UpdateMultiplier()
	{
	}

	private PassingTarget RandomActiveBucket()
	{
		IOrderedEnumerable<PassingTarget> orderedEnumerable = _activeTargets.OrderBy((PassingTarget a) => Guid.NewGuid());
		foreach (PassingTarget item in orderedEnumerable)
		{
			if (item.IsActive && !item.PowerUpAssigned)
			{
				return item;
			}
		}
		return null;
	}

	private void DestroyMultiplier()
	{
		if (_currentMultiplier != null)
		{
			UnityEngine.Object.Destroy(_currentMultiplier.gameObject);
		}
	}

	private void ActivateBucket(Bucket b)
	{
		b.ActivateBucket(bActivate: true);
		_currentActive++;
	}

	public void DeactivateBucket(Bucket b)
	{
		b.ActivateBucket(bActivate: false);
		_currentActive--;
	}

	private IEnumerator AssignGridPosition(PassingTarget target)
	{
		bool taken = true;
		GridSpawnPos gPos2 = null;
		while (taken)
		{
			gPos2 = _gridPositions[UnityEngine.Random.Range(0, _gridPositions.Count)];
			taken = gPos2.isTaken;
			if (!taken && target != null)
			{
				target.SetPosition(gPos2.position);
				gPos2.isTaken = true;
			}
			yield return null;
		}
	}
}
