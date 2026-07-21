using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class WallLevel : GameLevel
{
	public float _targetsRange;

	public Tutorial tutorial = new Tutorial();

	public List<WallTargetSettings> targets = new List<WallTargetSettings>();

	private List<WallTargetSettings> _targetsSet = new List<WallTargetSettings>();

	private bool _tutorialPassed;

	public override float Distance => (!(_settings.distance > 0f)) ? 0f : (15f - _settings.distance);

	public bool TutorialPassed
	{
		get
		{
			return _tutorialPassed;
		}
		set
		{
			_tutorialPassed = value;
		}
	}

	public float TargetsRange
	{
		get
		{
			return _targetsRange;
		}
		set
		{
			_targetsRange = value;
		}
	}

	public List<WallTargetSettings> TargetsToSpawn()
	{
		InitialSetup();
		if (base.IsRoundBased)
		{
			if (_settings.roundTargets == 0)
			{
				_settings.roundTargets = 1;
			}
			return NextRoundTargets();
		}
		return targets;
	}

	public List<WallTargetSettings> NextRoundTargets()
	{
		_targetsSet.Clear();
		List<int> list = new List<int>();
		for (int i = 0; i < _settings.roundTargets; i++)
		{
			bool flag = false;
			while (!flag)
			{
				int num = UnityEngine.Random.Range(0, targets.Count);
				WallTargetSettings wallTargetSettings = targets[num];
				wallTargetSettings.life.probability = Mathf.Clamp(wallTargetSettings.life.probability, 0.1f, 1f);
				if (UnityEngine.Random.value <= wallTargetSettings.life.probability && MultiplierCheck(wallTargetSettings) && !list.Contains(num))
				{
					_targetsSet.Add(wallTargetSettings);
					list.Add(num);
					flag = true;
				}
				else if (list.Count == targets.Count || GameData.MinigameObject.CurrentMinigame.CurrentMultiplier >= 9)
				{
					flag = true;
					Debug.LogError("Number of targets per round is greater than number of targets set or multiplier maximum has been reached but still trying to spawn more multipliers");
				}
			}
		}
		_availableRounds--;
		return _targetsSet;
	}

	private bool MultiplierCheck(WallTargetSettings target)
	{
		if (target.id == TARGET_TYPE.MULTIPLIER)
		{
			if (GameData.MinigameObject.CurrentMinigame.CurrentMultiplier < 9)
			{
				return true;
			}
			return false;
		}
		return true;
	}

	public float SetGlobalBoundBox(WallTargetSettings target)
	{
		if (_targetsRange == 0f)
		{
			return target.activity.positioning.boundBox;
		}
		if (target.activity.positioning.boundBox > _targetsRange || target.activity.positioning.boundBox == 0f)
		{
			return _targetsRange;
		}
		return target.activity.positioning.boundBox;
	}

	public override void AdditionalBehaviours()
	{
		WallDisplay.Instance.AddExtraTargets(GetListOfBonusTargets());
	}

	private WallTargetSettings GenerateExtraTargetForType(TARGET_TYPE targetType)
	{
		WallTargetSettings wallTargetSettings = new WallTargetSettings();
		wallTargetSettings.id = targetType;
		return wallTargetSettings;
	}

	private WallTargetSettings GenerateExtraPowerUpForType(TARGET_TYPE targetType)
	{
		WallTargetSettings wallTargetSettings = GenerateExtraTargetForType(targetType);
		wallTargetSettings.activity.speed.Set(UnityEngine.Random.Range(1f, 2f), UnityEngine.Random.Range(0.5f, 1f));
		wallTargetSettings.life.lifeTime = 5f;
		return wallTargetSettings;
	}

	private List<WallTargetSettings> GetListOfBonusTargets()
	{
		List<WallTargetSettings> list = new List<WallTargetSettings>();
		IEnumerator enumerator = Enum.GetValues(typeof(TARGET_TYPE)).GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				switch ((int)enumerator.Current)
				{
				case 9:
					if (!WallDisplay.Instance.IsBonusTargetOnScreen(SCORE_TYPE.MULTIPLIER) && UnityEngine.Random.value <= GameData.MinigameObject.CurrentMinigame.GetBonusProbabiltyForType(POWER_UP.MULTIPLIER_BONUS) && GameData.MinigameObject.CurrentMinigame.CurrentMultiplier < 9 && !WallDisplay.Instance.ExistingMultiplier())
					{
						list.Add(GenerateExtraPowerUpForType(TARGET_TYPE.MULTIPLIER));
					}
					break;
				case 0:
					if (!WallDisplay.Instance.IsBonusTargetOnScreen(SCORE_TYPE.LIVES) && UnityEngine.Random.value <= GameData.MinigameObject.CurrentMinigame.GetBonusProbabiltyForType(POWER_UP.HEART_BONUS))
					{
						list.Add(GenerateExtraPowerUpForType(TARGET_TYPE.HEART));
					}
					break;
				case 1:
					if (!WallDisplay.Instance.IsBonusTargetOnScreen(SCORE_TYPE.SECONDS) && UnityEngine.Random.value <= GameData.MinigameObject.CurrentMinigame.GetBonusProbabiltyForType(POWER_UP.TIME_BONUS))
					{
						list.Add(GenerateExtraPowerUpForType(TARGET_TYPE.TIME));
					}
					break;
				}
			}
			return list;
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

	protected override void BringInObstacle()
	{
		switch (difficultyLevel)
		{
		case DIFFICULTY.NORMAL:
			ObstacleManager.Instance.BringInObstacle(ObstacleTarget.ID.WALL_2);
			break;
		case DIFFICULTY.HARD:
			ObstacleManager.Instance.BringInObstacle(ObstacleTarget.ID.WALL_3);
			break;
		case DIFFICULTY.PRO:
			ObstacleManager.Instance.BringInObstacle(ObstacleTarget.ID.WALL_5);
			break;
		}
	}

	protected override void RemoveAllObstacles()
	{
		ObstacleManager.Instance.RemoveAll();
	}
}
