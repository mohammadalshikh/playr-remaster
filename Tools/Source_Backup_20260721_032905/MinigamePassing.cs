using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MinigamePassing : MinigameChallenge
{
	public const float MIN_SHOOTING_DISTANCE = 10f;

	public const float MAX_SHOOTING_DISTANCE = 40f;

	public const float MAX_SHOOTING_WIDTH = 30f;

	public const float MAX_SHOOTING_ANGLE = 35f;

	public const int MAX_SCORE = 3000;

	public const int MIN_SCORE = 500;

	public List<PassingLevel> _levels = new List<PassingLevel>();

	private MinigameGUI _GUI;

	private float m_cameraFollowDistance = 40f;

	private PassingLevel m_currentLevel;

	private float m_currentMinShootingDistance = 10f;

	private float m_currentMaxShootingDistance = 40f;

	private bool _multiplierObtained;

	private bool _shieldObtained;

	private bool _timerObtained;

	private bool _heartObtained;

	private string m_trigger_tag = string.Empty;

	private bool m_success;

	private bool m_sweetShot;

	private bool m_applySweetBonus;

	private bool m_ballSpeedupCalled;

	private bool m_ballSlowDownCalled;

	private bool m_timedFinish;

	private PassingTarget m_currentTargetHit;

	private bool m_cameraRotateEnabled;

	private Vector3 m_cameraRotateObject;

	private float m_cameraRotateSpeed;

	public override Vector3 TargetPosition => new Vector3(52f, 0f, 0f);

	public override bool DollyEffectActive => true;

	public override float CameraFollowDistance => m_cameraFollowDistance;

	public override float MaxFlickForCurl => (float)Screen.width * 0.3f;

	public override float EndShotBallSpeed => 0.3f;

	public MinigamePassing()
	{
		m_type = MINIGAME_TYPE.PASSING;
	}

	public override void Reset()
	{
		base.Reset();
		PassingTarget.TargetHitEvent -= HandleTargetHitEvent;
	}

	protected override void Start()
	{
		PassingTarget.TargetHitEvent += HandleTargetHitEvent;
		_GUI = GameData.MinigameObject.GUIManager;
		AudioManager.PlaySFX(GameData.MinigameObject.BucketData.trainingOutdoorLoop, 0f);
		m_currentLevel.Init();
		MonoSingleton<WeatherManager>.Instance.ShowSun(bActive: true);
		MonoSingleton<WeatherManager>.Instance.SetRandomConditions();
	}

	public override void Finish()
	{
		if (m_currentLevel != null)
		{
			m_currentLevel.RemoveTargets();
		}
	}

	protected override void LoadParameter(string key, string value)
	{
		switch (key)
		{
		case "levels_difficulty":
		{
			string[] array2 = value.Split(';');
			for (int num3 = array2.Length - _levels.Count; num3 > 0; num3--)
			{
				_levels.Add(new PassingLevel());
			}
			for (int num4 = array2.Length - 1; num4 >= 0; num4--)
			{
				_levels[num4].difficultyLevel = (DIFFICULTY)int.Parse(array2[num4]);
			}
			break;
		}
		case "levels_distance":
		{
			string[] array3 = value.Split(';');
			for (int num5 = array3.Length - _levels.Count; num5 > 0; num5--)
			{
				_levels.Add(new PassingLevel());
			}
			for (int num6 = array3.Length - 1; num6 >= 0; num6--)
			{
				float.TryParse(array3[num6], out _levels[num6]._settings.distance);
			}
			break;
		}
		case "levels_position_range":
		{
			string[] array10 = value.Split(';');
			for (int num19 = array10.Length - _levels.Count; num19 > 0; num19--)
			{
				_levels.Add(new PassingLevel());
			}
			for (int num20 = array10.Length - 1; num20 >= 0; num20--)
			{
				float.TryParse(array10[num20], out _levels[num20]._settings.positionRange);
			}
			break;
		}
		case "levels_step_back":
		{
			string[] array7 = value.Split(';');
			for (int num13 = array7.Length - _levels.Count; num13 > 0; num13--)
			{
				_levels.Add(new PassingLevel());
			}
			for (int num14 = array7.Length - 1; num14 >= 0; num14--)
			{
				_levels[num14]._settings.stepsBackwards = (PLAYER_STEPS)int.Parse(array7[num14]);
			}
			break;
		}
		case "levels_reset_direction":
		{
			string[] array8 = value.Split(';');
			for (int num15 = array8.Length - _levels.Count; num15 > 0; num15--)
			{
				_levels.Add(new PassingLevel());
			}
			for (int num16 = array8.Length - 1; num16 >= 0; num16--)
			{
				_levels[num16]._settings.resetDirection = ((int.Parse(array8[num16]) != 0) ? true : false);
			}
			break;
		}
		case "levels_rounds":
		{
			string[] array12 = value.Split(';');
			for (int num23 = array12.Length - _levels.Count; num23 > 0; num23--)
			{
				_levels.Add(new PassingLevel());
			}
			for (int num24 = array12.Length - 1; num24 >= 0; num24--)
			{
				_levels[num24]._settings.rounds = int.Parse(array12[num24]);
			}
			break;
		}
		case "levels_round_targets":
		{
			string[] array4 = value.Split(';');
			for (int num7 = array4.Length - _levels.Count; num7 > 0; num7--)
			{
				_levels.Add(new PassingLevel());
			}
			for (int num8 = array4.Length - 1; num8 >= 0; num8--)
			{
				_levels[num8]._settings.roundTargets = int.Parse(array4[num8]);
			}
			break;
		}
		case "levels_success_hits":
		{
			string[] array11 = value.Split(';');
			for (int num21 = array11.Length - _levels.Count; num21 > 0; num21--)
			{
				_levels.Add(new PassingLevel());
			}
			for (int num22 = array11.Length - 1; num22 >= 0; num22--)
			{
				_levels[num22]._settings.successHits = int.Parse(array11[num22]);
			}
			break;
		}
		case "levels_success_text":
		{
			string[] array6 = value.Split(';');
			for (int num11 = array6.Length - _levels.Count; num11 > 0; num11--)
			{
				_levels.Add(new PassingLevel());
			}
			for (int num12 = array6.Length - 1; num12 >= 0; num12--)
			{
				_levels[num12]._settings.roundInformation = array6[num12];
			}
			break;
		}
		case "levels_spawn_area_x":
		{
			string[] array13 = value.Split(';');
			for (int num25 = array13.Length - _levels.Count; num25 > 0; num25--)
			{
				_levels.Add(new PassingLevel());
			}
			for (int num26 = array13.Length - 1; num26 >= 0; num26--)
			{
				_levels[num26].bucketsSetup.areaWidth = int.Parse(array13[num26]);
			}
			break;
		}
		case "levels_spawn_area_y":
		{
			string[] array9 = value.Split(';');
			for (int num17 = array9.Length - _levels.Count; num17 > 0; num17--)
			{
				_levels.Add(new PassingLevel());
			}
			for (int num18 = array9.Length - 1; num18 >= 0; num18--)
			{
				_levels[num18].bucketsSetup.areaDepth = int.Parse(array9[num18]);
			}
			break;
		}
		case "levels_spawn_distance":
		{
			string[] array5 = value.Split(';');
			for (int num9 = array5.Length - _levels.Count; num9 > 0; num9--)
			{
				_levels.Add(new PassingLevel());
			}
			for (int num10 = array5.Length - 1; num10 >= 0; num10--)
			{
				float.TryParse(array5[num10], out _levels[num10].bucketsSetup.distance);
			}
			break;
		}
		case "levels_multiplier_chance":
		{
			string[] array = value.Split(';');
			for (int num = array.Length - _levels.Count; num > 0; num--)
			{
				_levels.Add(new PassingLevel());
			}
			for (int num2 = array.Length - 1; num2 >= 0; num2--)
			{
				float.TryParse(array[num2], out _levels[num2].bucketsSetup.multiplierChance);
			}
			break;
		}
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
		string text14 = string.Empty + ';';
		foreach (PassingLevel level in _levels)
		{
			text = text + text14 + (int)level.difficultyLevel;
			text2 = text2 + text14 + level._settings.distance;
			text3 = text3 + text14 + level._settings.positionRange;
			text4 = text4 + text14 + (int)level._settings.stepsBackwards;
			text5 = text5 + text14 + (level._settings.resetDirection ? 1 : 0);
			text6 = text6 + text14 + level._settings.rounds;
			text7 = text7 + text14 + level._settings.roundTargets;
			text8 = text8 + text14 + level._settings.successHits;
			text9 = text9 + text14 + level._settings.roundInformation;
			text10 = text10 + text14 + level.bucketsSetup.areaWidth;
			text11 = text11 + text14 + level.bucketsSetup.areaDepth;
			text12 = text12 + text14 + level.bucketsSetup.distance;
			text13 = text13 + text14 + level.bucketsSetup.multiplierChance;
		}
		json_dictionary.Add("levels_difficulty", text.Remove(0, 1));
		json_dictionary.Add("levels_distance", text2.Remove(0, 1));
		json_dictionary.Add("levels_position_range", text3.Remove(0, 1));
		json_dictionary.Add("levels_step_back", text4.Remove(0, 1));
		json_dictionary.Add("levels_reset_direction", text5.Remove(0, 1));
		json_dictionary.Add("levels_rounds", text6.Remove(0, 1));
		json_dictionary.Add("levels_round_targets", text7.Remove(0, 1));
		json_dictionary.Add("levels_success_hits", text8.Remove(0, 1));
		json_dictionary.Add("levels_success_text", text9.Remove(0, 1));
		json_dictionary.Add("levels_spawn_area_x", text10.Remove(0, 1));
		json_dictionary.Add("levels_spawn_area_y", text11.Remove(0, 1));
		json_dictionary.Add("levels_spawn_distance", text12.Remove(0, 1));
		json_dictionary.Add("levels_multiplier_chance", text13.Remove(0, 1));
	}

	public override void FinalizeParameters()
	{
		PlayerPrefs.DeleteKey("FirstChance");
		m_currentLevel = _levels[0];
	}

	protected override void GenerateTarget()
	{
	}

	protected override void GenerateShootingChance()
	{
		GameData.PlayerControl.EnableShoot(bEnable: true);
		if (CurrentLevel().ShouldEnd() || CurrentBucketLevel().AllBucketsHit())
		{
			NextLevel();
		}
		m_currentMinShootingDistance = m_currentLevel.MinDistanceRange;
		m_currentMaxShootingDistance = m_currentLevel.MaxDistanceRange;
		if (!m_success && PlayerPrefs.HasKey("FirstChance"))
		{
			if (m_current_shield <= 0 && !_multiplierObtained)
			{
				LoseMultiplier();
				CurrentBucketLevel().UpdateMultiplier();
			}
			if (m_current_shield > 0)
			{
				if (m_current_multiplier > 1 || base.UsingLives)
				{
					LoseShield();
				}
				if (m_current_shield == 0)
				{
					PROGRESS_PANEL_TYPE progressPanelType = _GUI.ProgressPanelType;
					if (progressPanelType == PROGRESS_PANEL_TYPE.SHOTS)
					{
						_GUI.ShowShotsPanel(bShow: true);
					}
				}
			}
		}
		CheckRoundsSystem();
		_multiplierObtained = false;
		_shieldObtained = false;
		_timerObtained = false;
		_heartObtained = false;
		m_trigger_tag = string.Empty;
		m_success = false;
		m_sweetShot = true;
		m_applySweetBonus = false;
		m_ballSpeedupCalled = false;
		m_ballSlowDownCalled = false;
		m_cameraRotateEnabled = false;
		m_currentTargetHit = null;
		if (!PlayerPrefs.HasKey("FirstChance"))
		{
			DisplayInstructions(CurrentLevel().IndexID, CurrentLevel().IndexID == _levels.Count - 1);
			PlayerPrefs.SetInt("FirstChance", 1);
			PlayerPrefs.Save();
		}
	}

	public override void DisplayGUI()
	{
		if (Minigame.GameInfoExists())
		{
			GameData.GUIManager.ShowThresholdToBeat(GameData.PlayerStats.CurrentGameInfo().HighScore, m_bronze_score, m_silver_score, m_gold_score);
		}
	}

	public override Vector3 GenerateShootingPosition()
	{
		base.PlayerPosition = new Vector3(-25f, 0f, 0f);
		return base.PlayerPosition;
	}

	private void NextLevel()
	{
		if (NextLevelAvailable() && CurrentLevel().CanPassNextRound)
		{
			ResetRoundsPanel();
			m_currentLevel.Finish();
			m_currentLevel = _levels[_levels.IndexOf(m_currentLevel) + 1];
			DisplayInstructions(CurrentLevel().IndexID, CurrentLevel().IndexID == _levels.Count - 1);
			m_currentLevel.Init();
		}
		else
		{
			GameData.MinigameObject.EndMinigame();
		}
	}

	private void DisplayInstructionsForLevel(PassingLevel level)
	{
		int num = _levels.IndexOf(level);
		if (level.CanDisplayIntroMessage)
		{
			string text = "Round " + (num + 1);
			if (num == _levels.Count - 1)
			{
				text += " - Final Round";
			}
			GameData.GUIManager.DisplayInstructions(text, level._settings.roundInformation, 0.5f, Color.yellow);
		}
	}

	private bool NextLevelAvailable()
	{
		return _levels.IndexOf((PassingLevel)CurrentLevel()) < _levels.Count - 1;
	}

	public override GameLevel CurrentLevel()
	{
		return m_currentLevel;
	}

	public PassingLevel CurrentBucketLevel()
	{
		return (PassingLevel)CurrentLevel();
	}

	protected override void ConsumeEnergy()
	{
		switch (m_name)
		{
		case "Parker":
			m_energy_consumption += 0.3f;
			break;
		default:
			m_energy_consumption += 1f;
			break;
		}
	}

	private void HandleTargetHitEvent(PassingTarget target)
	{
		if (m_sweetShot)
		{
			m_applySweetBonus = true;
		}
		m_currentTargetHit = target;
		m_currentLevel.TargetCleanup(target);
		m_currentLevel.CurrentHits++;
		m_success = true;
	}

	public override bool TargetHit(Minigame_TargetTrigger trigger)
	{
		return true;
	}

	public override float GetBonusProbabiltyForType(POWER_UP bonusType)
	{
		return GetBonusIndexFor(bonusType) switch
		{
			0 => 0f, 
			1 => 0.3f, 
			2 => 0.5f, 
			3 => 0.7f, 
			4 => 0.9f, 
			_ => 0.3f, 
		};
	}

	public override void BallFirstCollision(Collision collision)
	{
		if (!Minigame.NextChanceCalled)
		{
			m_trigger_tag = collision.gameObject.tag;
		}
	}

	public override void NextChance(float delay = 0f)
	{
		GameData.PlayerControl.EnableControls(bEnabled: false);
		Minigame.NextChanceCalled = true;
		if (!m_success && m_current_shield <= 0)
		{
			LoseLife();
		}
		if (m_currentTargetHit != null)
		{
			ScreenSizeButton.Instance.SetupAndBring(FastForwardEvent);
			m_timedFinish = true;
			int num = 0;
			GameData.GameCamera.SetXPosition(GameData.Ball.transform.position.x - 10f);
			GameData.GameCamera.SetYPosition(5f);
			GameData.GameCamera.CameraTransform.LookAt(m_currentTargetHit.transform);
			DollyEffect.Instance.Enable(bEnable: false);
			GameData.GameCamera.SetFieldOfView(70f);
			GameData.MinigameObject.Invoke("FinishCurrentChance", 1.5f);
			num = m_currentTargetHit.Score;
			if (m_applySweetBonus)
			{
				GameData.GUIManager.BringStampText("CLEAN SHOT!", 0f, 0f);
				num *= 2;
			}
			GameNotification.Instance.SetNotification(num.ToString("#,##0"), null, null, null, null, null);
			AddScore(num * GameData.MinigameObject.CurrentMinigame.CurrentMultiplier, ApplyStatModBonuses: true);
			GameNotification.Instance.BringIn();
			GameData.GUIManager.ShowBonus(0.5f);
			if (Minigame.GameInfoExists())
			{
				GameData.GUIManager.ShowThresholdToBeat(base.Score, m_bronze_score, m_silver_score, m_gold_score, GameData.PlayerStats.CurrentGameInfo().HighScore);
			}
		}
		else
		{
			GameData.EndShotWithDelay(bForce: false, delay);
		}
	}

	public override void FinishCurrentChance()
	{
		if (m_timedFinish)
		{
			FinishChanceImmediately();
		}
	}

	private void FinishChanceImmediately()
	{
		GameNotification.Instance.Dismiss();
		GameData.GUIManager.DismissStampText();
		ScreenSizeButton.Instance.Dismiss();
		GameData.EndShot(bForce: false);
	}

	public void FastForwardEvent()
	{
		m_timedFinish = false;
		GameData.MinigameObject.CancelInvoke("FinishCurrentChance");
		FinishChanceImmediately();
	}

	public override void CustomTargetHit(MinigameTarget target)
	{
		FlyingTarget flyingTarget = (FlyingTarget)target;
		switch (flyingTarget.ScoreType)
		{
		case SCORE_TYPE.MULTIPLIER:
			_multiplierObtained = true;
			CheckMultiplier();
			break;
		case SCORE_TYPE.SECONDS:
			CheckTimer();
			break;
		case SCORE_TYPE.LIVES:
			_heartObtained = true;
			CheckHeart();
			break;
		}
	}

	public override void BallSpeedUpDelegate()
	{
		if (!m_ballSpeedupCalled)
		{
			GameData.GUIManager.ShowNotification("Super Power!");
			m_ballSpeedupCalled = true;
		}
	}

	public override void BallSlowDownDelegate()
	{
		if (!m_ballSlowDownCalled)
		{
			GameData.GUIManager.ShowNotification("Slowing down");
			m_ballSlowDownCalled = true;
		}
	}

	private void CheckMultiplier()
	{
		if (_multiplierObtained)
		{
			GameData.MinigameObject.CurrentMinigame.IncrementMultiplier();
		}
	}

	private void CheckShield()
	{
		if (_shieldObtained)
		{
			AwardShield();
		}
	}

	private void CheckTimer()
	{
		AddTimeBonus(6);
	}

	private void CheckHeart()
	{
		if (_heartObtained)
		{
			AddLife();
		}
	}

	public override float BallShootingHeight(float flickLength)
	{
		return 0.45f;
	}

	public override float BallShootingSpeed(float flickLength, float currentHeight)
	{
		float num = Mathf.Lerp(m_currentMinShootingDistance - base.PlayerPosition.x, m_currentMaxShootingDistance - base.PlayerPosition.x, Mathf.InverseLerp(0f, Screen.height / 2, flickLength));
		DollyEffect.Instance.SetTargetPosition(new Vector3(num + 10f, 0f, 0f));
		float result = Mathf.Sqrt(num * Physics.gravity.magnitude / 0.55f);
		m_cameraFollowDistance = 40f;
		return result;
	}

	public override float MaxShootAngle()
	{
		return 35f;
	}

	public override float CurlAppliedWithDelta(Vector2 deltaFlick)
	{
		float from = 2f;
		float to = 20f;
		return Mathf.Lerp(from, to, Mathf.InverseLerp(0f, MaxFlickForCurl, Mathf.Abs(deltaFlick.x)));
	}

	public override void CameraFollow()
	{
		if (GameData.Ball.IsFalling)
		{
			GameData.GameCamera.SmoothFollow(GameData.Ball.transform, 2f, 0.8f, xzOnly: false);
			GameData.GameCamera.SmoothLookAt(GameData.Ball.transform, 8f);
			BallFollowEffects(bEnabled: true);
		}
		else
		{
			GameData.GameCamera.SmoothFollow(GameData.Ball.transform, 3f, 0.7f, xzOnly: false);
			GameData.GameCamera.SmoothLookAt(GameData.Ball.transform, 0.7f);
		}
	}

	private void CameraRotateAround(Vector3 point, float speed)
	{
		m_cameraRotateObject = point;
		m_cameraRotateSpeed = speed;
		m_cameraRotateEnabled = true;
	}

	public override void OnBallCollision(Collision collision, SoccerBall ball)
	{
		if (Minigame.NextChanceCalled)
		{
			return;
		}
		bool flag = false;
		if (ball.State == SoccerBall.STATE.IN_AIR)
		{
			flag = true;
			m_sweetShot = false;
		}
		if (flag)
		{
			flag = false;
			GameData.PlayerControl.CameraFollow = false;
			DollyEffect.Instance.Enable(bEnable: false);
			BallFollowEffects(bEnabled: false);
		}
		switch (m_trigger_tag)
		{
		case "end_trigger":
			if (collision.gameObject.tag == m_trigger_tag)
			{
				m_trigger_tag = string.Empty;
				NextChance(0.5f);
			}
			break;
		case "bucket":
			if (collision.gameObject.tag == "end_trigger")
			{
				m_trigger_tag = string.Empty;
				NextChance(0f);
			}
			break;
		}
	}

	private void BallFollowEffects(bool bEnabled)
	{
		GameData.GameCamera.EnableMotionLines(bEnabled);
		if (!Minigame.IsSuperControlActive)
		{
			GameData.SetTimeScale((!bEnabled) ? 1f : 0.7f, bForce: false);
		}
	}

	public override void FixedUpdate()
	{
		if (m_cameraRotateEnabled)
		{
			GameData.GameCamera.CameraTransform.RotateAround(m_cameraRotateObject, Vector3.up, m_cameraRotateSpeed * (1f / 30f));
		}
	}

	public override void OnDrawGizmos(Minigame minigame)
	{
		base.OnDrawGizmos(minigame);
	}
}
