using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ShootLevel : GameLevel
{
	public const float GOAL_X_BOUNDARIES = 7.5f;

	public const float GOAL_MID_HEIGHT = 1.2f;

	[SerializeField]
	private float _wallOffset;

	[SerializeField]
	private float _accuracy = 0.6f;

	[SerializeField]
	private List<ShootTargetSettings> _targets = new List<ShootTargetSettings>();

	private List<ShootingTarget> _activeTargets = new List<ShootingTarget>();

	private int _knockoutIndex = -1;

	public List<ShootTargetSettings> Targets => _targets;

	public float WallOffset
	{
		get
		{
			return _wallOffset;
		}
		set
		{
			_wallOffset = value;
		}
	}

	public float Accuracy
	{
		get
		{
			return _accuracy;
		}
		set
		{
			_accuracy = value;
		}
	}

	public override bool AllTargetsHit
	{
		get
		{
			if (base.IsRoundBased)
			{
				return _currentHits > _targets.Count - 1;
			}
			return _activeTargets.Count == 0;
		}
	}

	public override bool CanPassNextRound => _currentHits >= _settings.successHits;

	public override void Init()
	{
		ShootingTarget.WillRemoveTarget += HandleTargetRemoval;
		InitialSetup();
		if (base.IsRoundBased)
		{
			CreateKnockoutTarget();
			return;
		}
		foreach (ShootTargetSettings target in _targets)
		{
			CreateTarget(target);
		}
	}

	public override void Finish()
	{
		RemoveAllTargets();
		ShootingTarget.WillRemoveTarget -= HandleTargetRemoval;
		base.Finish();
	}

	public override void Reset()
	{
		base.Reset();
		_knockoutIndex = -1;
	}

	private void CreateTarget(ShootTargetSettings settings)
	{
		ShootingTarget component = GameData.MinigameObject.ShootingData.InstantiateTarget(settings.Identifier).GetComponent<ShootingTarget>();
		component.Setup(settings);
		_activeTargets.Add(component);
	}

	private void CreateKnockoutTarget()
	{
		if (base.IsRoundBased && _knockoutIndex != _currentHits && _currentHits <= _targets.Count - 1)
		{
			_knockoutIndex = _currentHits;
			CreateTargetWithIndex(_knockoutIndex);
		}
	}

	private void CreateTargetWithIndex(int index)
	{
		index = Mathf.Clamp(index, 0, _targets.Count - 1);
		CreateTarget(_targets[index]);
	}

	public void RemoveAllTargets()
	{
		foreach (ShootingTarget activeTarget in _activeTargets)
		{
			if (activeTarget != null)
			{
				activeTarget.DestroyTarget();
			}
		}
		_activeTargets.Clear();
	}

	protected override void BringInObstacle()
	{
		ObstacleManager.Instance.BringInWall(_obstacle, GameData.MinigameObject.CurrentMinigame.TargetPosition, GameData.Ball.transform.position, Vector3.zero);
	}

	protected override void RemoveAllObstacles()
	{
	}

	public override void UpdateLevel()
	{
		_availableRounds--;
		CreateKnockoutTarget();
		BringPowerUps();
		ObstacleManager.Instance.UpdateWallPosition(GameData.MinigameObject.CurrentMinigame.TargetPosition, GameData.Ball.transform.position, -Vector3.forward * _wallOffset);
	}

	private void HandleTargetRemoval(ShootingTarget target)
	{
		_activeTargets.Remove(target);
	}

	private void CreatePowerUp(ShootingTarget.ID id)
	{
		ShootTargetSettings shootTargetSettings = new ShootTargetSettings();
		shootTargetSettings.RandomSettingsForPowerup(id);
		CreateTarget(shootTargetSettings);
	}

	private void BringPowerUps()
	{
		IEnumerator enumerator = Enum.GetValues(typeof(SCORE_TYPE)).GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				switch ((int)enumerator.Current)
				{
				case 2:
					if (GameData.ProbabilityTest(GameData.MinigameObject.CurrentMinigame.GetBonusProbabiltyForType(POWER_UP.TIME_BONUS)))
					{
						CreatePowerUp(ShootingTarget.ID.TIMER);
					}
					break;
				case 1:
					if (GameData.ProbabilityTest(GameData.MinigameObject.CurrentMinigame.GetBonusProbabiltyForType(POWER_UP.HEART_BONUS)))
					{
						CreatePowerUp(ShootingTarget.ID.HEART);
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
}
