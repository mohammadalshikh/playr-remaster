using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class MinigameShooting : MinigameChallenge
{
	public const float MIDPOINT_TO_TARGET = 52f;

	public const float MAX_SHOOT_DISTANCE = 40f;

	public const float MIN_SHOOT_DISTANCE = 25f;

	public const float MAX_SHOOTING_OFFSET = 25f;

	[SerializeField]
	private int _levelDebug;

	[SerializeField]
	private List<ShootLevel> _levels = new List<ShootLevel>();

	private ShootLevel _currentLevel;

	private ShootLevel _previousLevel;

	private Shooting3DText _text3D;

	private int _goalScore = 100;

	private int _acumulatedScore;

	private float _accuracy;

	private Vector3 _celebPosition;

	private bool _targetHit;

	private bool _bonusHit;

	private float _accuracyAverage;

	private List<float> _accuracyList = new List<float>();

	public override Vector3 TargetPosition => new Vector3(52f, 0f, 0f);

	public override bool DollyEffectActive => true;

	public override float CameraFollowDistance => (TargetPosition.x - base.PlayerPosition.x) * 0.65f + base.PlayerPosition.x;

	public override float EndShotBallSpeed => 1f;

	public override float MaxFlickForCurl => (float)Screen.width * 0.1f;

	public override bool UseDefaultStreakCounterIncrement => false;

	protected override int BlastPhaseTrigger => 5;

	private bool NextLevelAvailable => _levels.IndexOf((ShootLevel)CurrentLevel()) < _levels.Count - 1;

	private Shooting3DText Text3D => _text3D;

	public bool HitPointsTarget => _targetHit && !_bonusHit;

	private ShootLevel CurrentShootLevel => _currentLevel;

	public float AccuracyAverage
	{
		get
		{
			return _accuracyAverage;
		}
		set
		{
			_accuracyList.Add(Mathf.Clamp01(value));
			float num = 0f;
			foreach (float accuracy in _accuracyList)
			{
				float num2 = accuracy;
				num += num2;
			}
			if (_accuracyList.Count > 0)
			{
				_accuracyAverage = num / (float)_accuracyList.Count;
			}
			else
			{
				_accuracyAverage = 0f;
			}
		}
	}

	private bool CanProgressLevel
	{
		get
		{
			if (CurrentShootLevel.IsRoundBased)
			{
				return CurrentShootLevel.Accuracy <= AccuracyAverage;
			}
			return true;
		}
	}

	public MinigameShooting()
	{
		m_type = MINIGAME_TYPE.SHOOTING;
	}

	protected override void Start()
	{
		GoalTrigger.GoalEntered += HandleGoalEntered;
		SpriteTarget.TargetHit += HandleTargetHit;
		DollyEffect.Instance.SetTargetPosition(TargetPosition + Vector3.right * 5f);
		_levelDebug = Mathf.Clamp(_levelDebug, 0, _levels.Count - 1);
		m_difficultyManager.SetLevelList(_levels.Cast<GameLevel>().ToList());
		int index = ((!base.IsDifficultyBased) ? _levelDebug : m_difficultyManager.NextLevelIndex());
		_currentLevel = _levels[index];
		ObstacleManager.Instance.RemoveWall();
		if (_text3D == null)
		{
			GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(GameData.MinigameObject.ShootingData.Text3DPrefab.gameObject);
			gameObject.transform.position = TargetPosition + Vector3.up * 3.5f;
			_text3D = gameObject.GetComponent<Shooting3DText>();
		}
		Text3D.Dismiss();
		MonoSingleton<WeatherManager>.Instance.ShowSun(bActive: true);
		MonoSingleton<WeatherManager>.Instance.SetRandomConditions();
	}

	public override void Reset()
	{
		base.Reset();
		ObstacleManager.Instance.RemoveAll();
		GoalTrigger.GoalEntered -= HandleGoalEntered;
		SpriteTarget.TargetHit -= HandleTargetHit;
		ResetAccuracy();
	}

	public override void Finish()
	{
		GoalTrigger.GoalEntered -= HandleGoalEntered;
		SpriteTarget.TargetHit -= HandleTargetHit;
	}

	protected override void LoadParameter(string key, string value)
	{
		string[] array = null;
		switch (key)
		{
		case "levels_difficulty":
		{
			string[] array3 = value.Split(';');
			for (int num3 = array3.Length - _levels.Count; num3 > 0; num3--)
			{
				_levels.Add(new ShootLevel());
			}
			for (int num4 = array3.Length - 1; num4 >= 0; num4--)
			{
				_levels[num4].difficultyLevel = (DIFFICULTY)int.Parse(array3[num4]);
			}
			break;
		}
		case "levels_obstacle":
		{
			string[] array4 = value.Split(';');
			for (int num5 = array4.Length - _levels.Count; num5 > 0; num5--)
			{
				_levels.Add(new ShootLevel());
			}
			for (int num6 = array4.Length - 1; num6 >= 0; num6--)
			{
				_levels[num6].Obstacle = (ObstacleTarget.ID)int.Parse(array4[num6]);
			}
			break;
		}
		case "levels_multi_obstacle":
		{
			string[] array11 = value.Split(';');
			for (int num19 = array11.Length - _levels.Count; num19 > 0; num19--)
			{
				_levels.Add(new ShootLevel());
			}
			for (int num20 = array11.Length - 1; num20 >= 0; num20--)
			{
				_levels[num20].AllowLastObstacle = ((int.Parse(array11[num20]) != 0) ? true : false);
			}
			break;
		}
		case "levels_wall_offset":
		{
			string[] array8 = value.Split(';');
			for (int num13 = array8.Length - _levels.Count; num13 > 0; num13--)
			{
				_levels.Add(new ShootLevel());
			}
			for (int num14 = array8.Length - 1; num14 >= 0; num14--)
			{
				float.TryParse(array8[num14], out var result);
				_levels[num14].WallOffset = result;
			}
			break;
		}
		case "levels_accuracy":
		{
			string[] array9 = value.Split(';');
			for (int num15 = array9.Length - _levels.Count; num15 > 0; num15--)
			{
				_levels.Add(new ShootLevel());
			}
			for (int num16 = array9.Length - 1; num16 >= 0; num16--)
			{
				float.TryParse(array9[num16], out var result2);
				_levels[num16].Accuracy = result2;
			}
			break;
		}
		case "levels_distance":
		{
			string[] array13 = value.Split(';');
			for (int num23 = array13.Length - _levels.Count; num23 > 0; num23--)
			{
				_levels.Add(new ShootLevel());
			}
			for (int num24 = array13.Length - 1; num24 >= 0; num24--)
			{
				float.TryParse(array13[num24], out _levels[num24]._settings.distance);
			}
			break;
		}
		case "levels_position_range":
		{
			string[] array5 = value.Split(';');
			for (int num7 = array5.Length - _levels.Count; num7 > 0; num7--)
			{
				_levels.Add(new ShootLevel());
			}
			for (int num8 = array5.Length - 1; num8 >= 0; num8--)
			{
				float.TryParse(array5[num8], out _levels[num8]._settings.positionRange);
			}
			break;
		}
		case "levels_step_back":
		{
			string[] array12 = value.Split(';');
			for (int num21 = array12.Length - _levels.Count; num21 > 0; num21--)
			{
				_levels.Add(new ShootLevel());
			}
			for (int num22 = array12.Length - 1; num22 >= 0; num22--)
			{
				_levels[num22]._settings.stepsBackwards = (PLAYER_STEPS)int.Parse(array12[num22]);
			}
			break;
		}
		case "levels_reset_direction":
		{
			string[] array7 = value.Split(';');
			for (int num11 = array7.Length - _levels.Count; num11 > 0; num11--)
			{
				_levels.Add(new ShootLevel());
			}
			for (int num12 = array7.Length - 1; num12 >= 0; num12--)
			{
				_levels[num12]._settings.resetDirection = ((int.Parse(array7[num12]) != 0) ? true : false);
			}
			break;
		}
		case "levels_rounds":
		{
			string[] array14 = value.Split(';');
			for (int num25 = array14.Length - _levels.Count; num25 > 0; num25--)
			{
				_levels.Add(new ShootLevel());
			}
			for (int num26 = array14.Length - 1; num26 >= 0; num26--)
			{
				_levels[num26]._settings.rounds = int.Parse(array14[num26]);
			}
			break;
		}
		case "levels_round_targets":
		{
			string[] array10 = value.Split(';');
			for (int num17 = array10.Length - _levels.Count; num17 > 0; num17--)
			{
				_levels.Add(new ShootLevel());
			}
			for (int num18 = array10.Length - 1; num18 >= 0; num18--)
			{
				_levels[num18]._settings.roundTargets = int.Parse(array10[num18]);
			}
			break;
		}
		case "levels_success_hits":
		{
			string[] array6 = value.Split(';');
			for (int num9 = array6.Length - _levels.Count; num9 > 0; num9--)
			{
				_levels.Add(new ShootLevel());
			}
			for (int num10 = array6.Length - 1; num10 >= 0; num10--)
			{
				_levels[num10]._settings.successHits = int.Parse(array6[num10]);
			}
			break;
		}
		case "levels_success_text":
		{
			string[] array2 = value.Split(';');
			for (int num = array2.Length - _levels.Count; num > 0; num--)
			{
				_levels.Add(new ShootLevel());
			}
			for (int num2 = array2.Length - 1; num2 >= 0; num2--)
			{
				_levels[num2]._settings.roundInformation = array2[num2];
			}
			break;
		}
		case "targets_start_score":
			array = value.Split(';');
			LoadNestedParameter(array, key);
			break;
		case "targets_end_score":
			array = value.Split(';');
			LoadNestedParameter(array, key);
			break;
		case "targets_speed":
			array = value.Split(';');
			LoadNestedParameter(array, key);
			break;
		case "targets_initial_size":
			array = value.Split(';');
			LoadNestedParameter(array, key);
			break;
		case "targets_end_size":
			array = value.Split(';');
			LoadNestedParameter(array, key);
			break;
		case "targets_duration":
			array = value.Split(';');
			LoadNestedParameter(array, key);
			break;
		case "targets_loop":
			array = value.Split(';');
			LoadNestedParameter(array, key);
			break;
		case "targets_id":
			array = value.Split(';');
			LoadNestedParameter(array, key);
			break;
		case "targets_vertical_pos":
			array = value.Split(';');
			LoadNestedParameter(array, key);
			break;
		case "targets_horizontal_pos":
			array = value.Split(';');
			LoadNestedParameter(array, key);
			break;
		case "targets_xboundaries":
			array = value.Split(';');
			LoadNestedParameter(array, key);
			break;
		}
	}

	private void LoadNestedParameter(string[] str_list, string key)
	{
		int num = str_list.Length;
		int num2 = 0;
		int num3 = 0;
		int num4;
		for (num4 = 0; num4 < num; num4++)
		{
			if (num3 == _levels.Count)
			{
				_levels.Add(new ShootLevel());
			}
			ShootLevel shootLevel = _levels[num3];
			num2 = int.Parse(str_list[num4]);
			for (int num5 = num2 - shootLevel.Targets.Count; num5 > 0; num5--)
			{
				shootLevel.Targets.Add(new ShootTargetSettings());
			}
			for (int num6 = num2 - 1; num6 >= 0; num6--)
			{
				switch (key)
				{
				case "targets_start_score":
					shootLevel.Targets[num6].ScoreSettings.Min = int.Parse(str_list[num4 + 1 + num6]);
					break;
				case "targets_end_score":
					shootLevel.Targets[num6].ScoreSettings.Max = int.Parse(str_list[num4 + 1 + num6]);
					break;
				case "targets_speed":
				{
					string[] array = str_list[num4 + 1 + num6].Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
					shootLevel.Targets[num6].Speed = new Vector2(float.Parse(array[0]), float.Parse(array[1]));
					break;
				}
				case "targets_initial_size":
					shootLevel.Targets[num6].SizeSettings.Start = (TargetSettings.SIZE)int.Parse(str_list[num4 + 1 + num6]);
					break;
				case "targets_end_size":
					shootLevel.Targets[num6].SizeSettings.End = (TargetSettings.SIZE)int.Parse(str_list[num4 + 1 + num6]);
					break;
				case "targets_duration":
				{
					float.TryParse(str_list[num4 + 1 + num6], out var result2);
					shootLevel.Targets[num6].SizeSettings.Duration = result2;
					break;
				}
				case "targets_loop":
					shootLevel.Targets[num6].SizeSettings.LoopType = (LOOP_TYPE)int.Parse(str_list[num4 + 1 + num6]);
					break;
				case "targets_id":
					shootLevel.Targets[num6].Identifier = (ShootingTarget.ID)int.Parse(str_list[num4 + 1 + num6]);
					break;
				case "targets_vertical_pos":
					shootLevel.Targets[num6].VerticalPosition = (ShootTargetSettings.VERTICAL)int.Parse(str_list[num4 + 1 + num6]);
					break;
				case "targets_horizontal_pos":
					shootLevel.Targets[num6].HorizontalPosition = (ShootTargetSettings.HORIZONTAL)int.Parse(str_list[num4 + 1 + num6]);
					break;
				case "targets_xboundaries":
				{
					float.TryParse(str_list[num4 + 1 + num6], out var result);
					shootLevel.Targets[num6].XBoundaries = result;
					break;
				}
				default:
					Debug.LogError("Unrecognize parameter: " + key);
					break;
				}
			}
			num4 += num2;
			num3++;
		}
	}

	protected override void WriteParameters(Dictionary<string, object> json_dictionary)
	{
		if (_levels.Count <= 0)
		{
			return;
		}
		string text = string.Empty;
		string text2 = string.Empty;
		string text3 = string.Empty;
		string text4 = string.Empty;
		string text5 = string.Empty;
		string text6 = string.Empty;
		string text7 = string.Empty;
		string text8 = string.Empty;
		string text9 = string.Empty;
		string text10 = string.Empty;
		string text11 = string.Empty;
		string text12 = string.Empty;
		string text13 = string.Empty;
		string text14 = string.Empty;
		string text15 = string.Empty;
		string text16 = string.Empty;
		string text17 = string.Empty;
		string text18 = string.Empty;
		string text19 = string.Empty;
		string text20 = string.Empty;
		string text21 = string.Empty;
		string text22 = string.Empty;
		string text23 = string.Empty;
		string text24 = string.Empty;
		string text25 = string.Empty + ';';
		foreach (ShootLevel level in _levels)
		{
			int count = level.Targets.Count;
			text = text + text25 + (int)level.difficultyLevel;
			text2 = text2 + text25 + (int)level.Obstacle;
			text3 = text3 + text25 + (level.AllowLastObstacle ? 1 : 0);
			text4 = text4 + text25 + level.WallOffset;
			text5 = text5 + text25 + level.Accuracy;
			text6 = text6 + text25 + level._settings.distance;
			text7 = text7 + text25 + level._settings.positionRange;
			text8 = text8 + text25 + (int)level._settings.stepsBackwards;
			text9 = text9 + text25 + (level._settings.resetDirection ? 1 : 0);
			text10 = text10 + text25 + level._settings.rounds;
			text11 = text11 + text25 + level._settings.roundTargets;
			text12 = text12 + text25 + level._settings.successHits;
			text13 = text13 + text25 + level._settings.roundInformation;
			text14 = text14 + text25 + count;
			text15 = text15 + text25 + count;
			text16 = text16 + text25 + count;
			text17 = text17 + text25 + count;
			text18 = text18 + text25 + count;
			text19 = text19 + text25 + count;
			text20 = text20 + text25 + count;
			text21 = text21 + text25 + count;
			text22 = text22 + text25 + count;
			text23 = text23 + text25 + count;
			text24 = text24 + text25 + count;
			foreach (ShootTargetSettings target in level.Targets)
			{
				text14 = text14 + text25 + target.ScoreSettings.Min;
				text15 = text15 + text25 + target.ScoreSettings.Max;
				string text26 = text16;
				text16 = text26 + text25 + target.Speed.x + "," + target.Speed.y;
				text17 = text17 + text25 + (int)target.SizeSettings.Start;
				text18 = text18 + text25 + (int)target.SizeSettings.End;
				text19 = text19 + text25 + target.SizeSettings.Duration;
				text20 = text20 + text25 + (int)target.SizeSettings.LoopType;
				text21 = text21 + text25 + (int)target.Identifier;
				text22 = text22 + text25 + (int)target.VerticalPosition;
				text23 = text23 + text25 + (int)target.HorizontalPosition;
				text24 = text24 + text25 + target.XBoundaries;
			}
		}
		json_dictionary.Add("levels_difficulty", text.Remove(0, 1));
		json_dictionary.Add("levels_obstacle", text2.Remove(0, 1));
		json_dictionary.Add("levels_multi_obstacle", text3.Remove(0, 1));
		json_dictionary.Add("levels_wall_offset", text4.Remove(0, 1));
		json_dictionary.Add("levels_accuracy", text5.Remove(0, 1));
		json_dictionary.Add("levels_distance", text6.Remove(0, 1));
		json_dictionary.Add("levels_position_range", text7.Remove(0, 1));
		json_dictionary.Add("levels_step_back", text8.Remove(0, 1));
		json_dictionary.Add("levels_reset_direction", text9.Remove(0, 1));
		json_dictionary.Add("levels_rounds", text10.Remove(0, 1));
		json_dictionary.Add("levels_round_targets", text11.Remove(0, 1));
		json_dictionary.Add("levels_success_hits", text12.Remove(0, 1));
		json_dictionary.Add("levels_success_text", text13.Remove(0, 1));
		json_dictionary.Add("targets_start_score", text14.Remove(0, 1));
		json_dictionary.Add("targets_end_score", text15.Remove(0, 1));
		json_dictionary.Add("targets_speed", text16.Remove(0, 1));
		json_dictionary.Add("targets_initial_size", text17.Remove(0, 1));
		json_dictionary.Add("targets_end_size", text18.Remove(0, 1));
		json_dictionary.Add("targets_duration", text19.Remove(0, 1));
		json_dictionary.Add("targets_loop", text20.Remove(0, 1));
		json_dictionary.Add("targets_id", text21.Remove(0, 1));
		json_dictionary.Add("targets_vertical_pos", text22.Remove(0, 1));
		json_dictionary.Add("targets_horizontal_pos", text23.Remove(0, 1));
		json_dictionary.Add("targets_xboundaries", text24.Remove(0, 1));
	}

	public override void FinalizeParameters()
	{
		foreach (ShootLevel level in _levels)
		{
			level.Reset();
		}
		if (CurrentLevel() != null)
		{
			CurrentLevel().Finish();
		}
	}

	public override GameLevel CurrentLevel()
	{
		return _currentLevel;
	}

	protected override void GenerateTarget()
	{
	}

	protected override void GenerateShootingChance()
	{
		GameData.PlayerControl.EnableShoot(bEnable: true);
		GameData.GameCamera.SetFieldOfView(25f);
		_acumulatedScore = 0;
		_targetHit = false;
		_bonusHit = false;
		_accuracy = 0f;
		Text3D.Dismiss(0.5f);
		CheckRoundsSystem();
		MonoSingleton<AccuracyView>.Instance.IsFailing(!CanProgressLevel);
	}

	public override Vector3 GenerateShootingPosition()
	{
		if (!CurrentLevel().IsInitialized)
		{
			CurrentLevel().Init();
			DisplayInstructions(CurrentLevel().IndexID, CurrentLevel().IndexID == _levels.Count - 1);
			CheckAccuracy();
		}
		if (CurrentLevel().ShouldEnd() || CurrentLevel().AllTargetsHit)
		{
			NextLevel();
		}
		float distance = CurrentLevel().Distance;
		distance = ((!(distance > 0f)) ? 19.5f : distance);
		distance = Mathf.Clamp(distance, 25f, 40f);
		distance = 52f - distance;
		float range = CurrentLevel().Range;
		range = Mathf.Clamp(range, -25f, 25f);
		base.PlayerPosition = new Vector3(distance, 0f, 0f - range);
		return base.PlayerPosition;
	}

	public override void NextChance(float delay)
	{
		base.NextChance(delay);
	}

	private void NextLevel()
	{
		_previousLevel = _currentLevel;
		if (NextLevelAvailable && CurrentLevel().CanPassNextRound && CanProgressLevel)
		{
			ResetRoundsPanel();
			int num = ((!base.IsDifficultyBased) ? (_levels.IndexOf(_currentLevel) + 1) : m_difficultyManager.NextLevelIndex());
			if (!m_difficultyManager.IsReadyToEnd)
			{
				_currentLevel = _levels[num];
				DisplayInstructions(num, num == _levels.Count - 1);
				_currentLevel.Init();
				if (_currentLevel.IsRoundBased)
				{
					MonoSingleton<AccuracyView>.Instance.TargetAccuracy = _currentLevel.Accuracy;
					ResetAccuracy();
				}
				return;
			}
		}
		if (_previousLevel != null)
		{
			_previousLevel.Finish();
		}
		GameData.MinigameObject.EndMinigame();
	}

	private void CheckAccuracy()
	{
		if (CurrentLevel() != null && CurrentLevel().IsRoundBased)
		{
			MonoSingleton<AccuracyView>.Instance.TargetAccuracy = CurrentShootLevel.Accuracy;
			MonoSingleton<AccuracyView>.Instance.CurrentAccuracy(AccuracyAverage, !CanProgressLevel);
			MonoSingleton<AccuracyView>.Instance.Show();
		}
	}

	private void HandleGoalEntered(GoalTrigger goalTrigger)
	{
		int new_score;
		if (_acumulatedScore > 0)
		{
			AudioManager.PlaySFX(GameData.MinigameObject.ShootingData.HitSound, 0f);
			new_score = _acumulatedScore;
		}
		else
		{
			_celebPosition = goalTrigger.HitPosition;
			new_score = _goalScore * base.CurrentMultiplier;
		}
		GameData.MinigameObject.ShootingData.CelebrateGoalAt(_celebPosition);
		AddScore(new_score, _acumulatedScore > 0);
		if (!_targetHit)
		{
			Text3D.MissedShot("TARGET MISSED");
		}
	}

	private void HandleTargetHit(SpriteTarget target)
	{
		IncrementStreakCounter();
		_targetHit = true;
		_bonusHit = true;
		CurrentLevel().CurrentHits++;
		switch (target.ScoreType)
		{
		case SCORE_TYPE.POINTS:
		{
			_celebPosition = target.HitPosition;
			int num = target.Score * base.CurrentMultiplier * CurrentBlastPhaseMultiplier();
			num = Mathf.RoundToInt((float)num * target.Accuracy);
			_acumulatedScore += num;
			_accuracy = target.Accuracy;
			_bonusHit = false;
			if (_accuracy >= 0.895f)
			{
				IncrementMultiplier();
			}
			if (_accuracy > 0.995f && !base.IsBlastPhaseOn)
			{
				FireUpBlastPhase();
			}
			AccuracyAverage = _accuracy;
			MonoSingleton<AccuracyView>.Instance.CurrentAccuracy(AccuracyAverage, !CanProgressLevel);
			Text3D.ShowResult(target.Accuracy, num);
			break;
		}
		case SCORE_TYPE.LIVES:
			AddLife();
			break;
		case SCORE_TYPE.SHIELD:
			AwardShield();
			break;
		case SCORE_TYPE.SECONDS:
			AddTimeBonus(6);
			break;
		case SCORE_TYPE.MULTIPLIER:
			IncrementMultiplier();
			break;
		}
	}

	public override bool TargetHit(Minigame_TargetTrigger trigger)
	{
		return true;
	}

	public override bool MinigameEnd()
	{
		if ((_accuracy < 0.895f && HitPointsTarget) || !_targetHit)
		{
			if (base.CurrentShield > 0)
			{
				if (base.CurrentMultiplier > 1 || base.UsingLives)
				{
					LoseShield();
				}
			}
			else
			{
				LoseMultiplier();
			}
			if (!_targetHit && base.UsingLives)
			{
				LoseLife();
			}
		}
		if (!base.SuccesfulAttemp)
		{
			Text3D.MissedShot("GOAL MISSED");
		}
		return false;
	}

	private void ResetAccuracy()
	{
		_accuracyList.Clear();
		_accuracyAverage = 0f;
		MonoSingleton<AccuracyView>.Instance.CurrentAccuracy(AccuracyAverage, bFailing: true);
	}

	public override void CameraFollow()
	{
		GameData.GameCamera.SmoothFollow(GameData.Ball.transform, 2f, 0.7f, xzOnly: false, Vector3.up * 1f);
		GameData.GameCamera.SmoothLookAt(TargetPosition + new Vector3(-1f, 1f, 0f), 4f);
	}

	public override float BallShootingHeight(float flickLength)
	{
		return Mathf.Lerp(0.14f, 0.24f, Mathf.InverseLerp(0f, (float)Screen.height / 2f, flickLength));
	}

	public override float BallShootingSpeed(float flickLength, float currentHeight)
	{
		float num = Vector2.Distance(new Vector2(TargetPosition.x, 0f), new Vector2(GameData.Ball.transform.position.x, GameData.Ball.transform.position.z));
		return Mathf.Sqrt(num * Physics.gravity.magnitude / 0.25f);
	}

	public override float MaxShootAngle()
	{
		return 15f;
	}

	public override void BallFirstCollision(Collision collision)
	{
	}

	public override float CurlAppliedWithDelta(Vector2 deltaFlick)
	{
		float from = 2f;
		float to = 10f;
		return Mathf.Lerp(from, to, Mathf.InverseLerp(0f, MaxFlickForCurl, Mathf.Abs(deltaFlick.x)));
	}

	public override void DisplayGUI()
	{
		GameData.GUIManager.ShowThresholdToBeat(GameData.PlayerStats.CurrentGameInfo().HighScore, m_bronze_score, m_silver_score, m_gold_score);
	}

	public override float GetBonusProbabiltyForType(POWER_UP bonusType)
	{
		return GetBonusIndexFor(bonusType) switch
		{
			0 => 0f, 
			1 => 0.1f, 
			2 => 0.2f, 
			3 => 0.4f, 
			4 => 0.6f, 
			_ => 0.1f, 
		};
	}

	public override void OnDrawGizmos(Minigame minigame)
	{
		base.OnDrawGizmos(minigame);
	}
}
