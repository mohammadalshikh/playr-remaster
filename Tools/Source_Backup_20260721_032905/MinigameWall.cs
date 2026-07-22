using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class MinigameWall : MinigameChallenge
{
	public const float WALL_POSITION = 15.8f;

	public const float MIN_WALL_DISTANCE = 15f;

	public const float MAX_WALL_DISTANCE = 15f;

	public const float WALL_LATERAL_LIMIT = 5f;

	private const int MAX_TRIES_PER_LEVEL = 200;

	public const int MAX_MULTIPLIER = 9;

	public const float WALL_WIDTH_BOUNDS = 17f;

	private const float MIN_SHOOTING_HEIGHT = 0.28f;

	private const float MAX_SHOOTING_HEIGHT = 0.4f;

	private const float MAX_SHOOTING_ANGLE = 35f;

	private const float SPEED_DIVIDER = 0.4f;

	public float distanceFromWall;

	public float positionRange;

	public int debugLevel;

	public List<WallLevel> levels = new List<WallLevel>();

	[HideInInspector]
	public GameObject shoottingWall;

	public static float m_game_speed = 1f;

	private WallDisplay _wallDisplay;

	private int _currentLevelIndex = -1;

	private bool _main_camera_setup;

	private static int _hitCount;

	private int _comboPointsCount;

	private int _comboPointsScore;

	private bool _multiplierApplies;

	private MinigameGUI _GUI;

	private GameObject _rightPost;

	private GameObject _leftPost;

	public static float HitCount => _hitCount;

	public static bool TargetHitOnCurrentShot => _hitCount > 0;

	public override Vector3 TargetPosition => new Vector3(16f, 0f, 0f);

	public override bool UseDefaultStreakCounterIncrement => false;

	public override bool TimeRunningOutSound => true;

	public Vector3 GetPosition => new Vector3(PlayerDistance, 0f, PositionRange);

	public float StepBackwards => (float)CurrentWallLevel()._settings.stepsBackwards / 3f;

	public float PlayerDistance
	{
		get
		{
			distanceFromWall = Mathf.Clamp(distanceFromWall, 15f, 15f);
			return 15.8f - distanceFromWall;
		}
		set
		{
			distanceFromWall = 15.8f - value;
		}
	}

	public float PositionRange
	{
		get
		{
			positionRange = Mathf.Clamp(positionRange, -5f, 5f);
			return (!(positionRange > 0f)) ? 0f : UnityEngine.Random.Range(0f - positionRange, positionRange);
		}
	}

	public MinigameWall()
	{
		m_type = MINIGAME_TYPE.WALL;
	}

	protected override void Start()
	{
		_GUI = GameData.MinigameObject.GUIManager;
		_GUI.SetGUIStyle = GUI_STYLE.NEON;
		Debug.Log("Drill name : " + base.Name);
		debugLevel = Mathf.Clamp(debugLevel, 0, levels.Count - 1);
		_wallDisplay.WallGame = this;
	}

	public override void Finish()
	{
	}

	public override void DisplayGUI()
	{
		if (Minigame.GameInfoExists())
		{
			GameData.GUIManager.ShowThresholdToBeat(GameData.PlayerStats.CurrentGameInfo().HighScore, m_bronze_score, m_silver_score, m_gold_score);
		}
	}

	protected override void ConsumeEnergy()
	{
		switch (m_name)
		{
		case "Christmas":
			m_energy_consumption += 0.5f;
			break;
		default:
			m_energy_consumption += 1f;
			break;
		}
	}

	public override void Reset()
	{
		base.Reset();
		_currentLevelIndex = -1;
		_main_camera_setup = false;
		_hitCount = 0;
		_comboPointsCount = 0;
		_comboPointsScore = 0;
		_multiplierApplies = false;
		if (_wallDisplay != null)
		{
			_wallDisplay.RemoveChildren();
		}
		foreach (WallLevel level in levels)
		{
			level.Reset();
		}
		if (_rightPost != null)
		{
			UnityEngine.Object.Destroy(_rightPost.gameObject);
		}
		if (_leftPost != null)
		{
			UnityEngine.Object.Destroy(_leftPost.gameObject);
		}
	}

	public void InsertNewGameLevel(int index)
	{
		if (index < 0 || index > levels.Count - 1)
		{
			Debug.LogError("GameLevel index must be a number within 0 and " + (levels.Count - 1));
			return;
		}
		Debug.Log("Inserting Level at index " + index);
		levels.Insert(index, new WallLevel());
	}

	public void RemoveGameLevel(int index)
	{
		if (index < 0 || index > levels.Count - 1)
		{
			Debug.LogError("GameLevel index must be a number within 0 and " + (levels.Count - 1));
			return;
		}
		Debug.Log("Removing Level at index " + index);
		levels.RemoveAt(index);
	}

	protected override void WriteParameters(Dictionary<string, object> json_dictionary)
	{
		json_dictionary.Add("shooting_distance", distanceFromWall);
		json_dictionary.Add("shooting_range", positionRange);
		if (levels.Count <= 0)
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
		string text25 = string.Empty;
		string text26 = string.Empty;
		string text27 = string.Empty;
		string text28 = string.Empty;
		string text29 = string.Empty;
		string text30 = string.Empty;
		string text31 = string.Empty;
		string text32 = string.Empty;
		string text33 = string.Empty;
		string text34 = string.Empty;
		string text35 = string.Empty;
		string text36 = string.Empty;
		string text37 = string.Empty;
		string text38 = string.Empty;
		string text39 = string.Empty + ';';
		foreach (WallLevel level in levels)
		{
			int count = level.targets.Count;
			text = text + text39 + (int)level.difficultyLevel;
			text2 = text2 + text39 + (int)level.Obstacle;
			text3 = text3 + text39 + (level.AllowLastObstacle ? 1 : 0);
			text4 = text4 + text39 + level._settings.distance;
			text5 = text5 + text39 + level._settings.positionRange;
			text6 = text6 + text39 + (int)level._settings.stepsBackwards;
			text7 = text7 + text39 + (level._settings.resetDirection ? 1 : 0);
			text8 = text8 + text39 + level._settings.rounds;
			text9 = text9 + text39 + level._settings.roundTargets;
			text10 = text10 + text39 + level._settings.successHits;
			text11 = text11 + text39 + level._settings.roundInformation;
			text12 = text12 + text39 + level.TargetsRange;
			text13 = text13 + text39 + (int)level.tutorial.type;
			text14 = text14 + text39 + level.tutorial.introMessage;
			text15 = text15 + text39 + level.tutorial.successMessage;
			text16 = text16 + text39 + level.tutorial.successHoldDelay;
			text17 = text17 + text39 + level.tutorial.failureMessage;
			text18 = text18 + text39 + level.tutorial.clockTime;
			text19 = text19 + text39 + (int)level.tutorial.visualGesture;
			text20 = text20 + text39 + level.tutorial.trailTime;
			text21 = text21 + text39 + count;
			text22 = text22 + text39 + count;
			text23 = text23 + text39 + count;
			text24 = text24 + text39 + count;
			text25 = text25 + text39 + count;
			text26 = text26 + text39 + count;
			text27 = text27 + text39 + count;
			text28 = text28 + text39 + count;
			text29 = text29 + text39 + count;
			text30 = text30 + text39 + count;
			text31 = text31 + text39 + count;
			text32 = text32 + text39 + count;
			text33 = text33 + text39 + count;
			text34 = text34 + text39 + count;
			text35 = text35 + text39 + count;
			text36 = text36 + text39 + count;
			text37 = text37 + text39 + count;
			text38 = text38 + text39 + count;
			foreach (WallTargetSettings target in level.targets)
			{
				text21 = text21 + text39 + (int)target.id;
				text22 = text22 + text39 + target.scoreSettings.minScore;
				text23 = text23 + text39 + target.scoreSettings.maxScore;
				text24 = text24 + text39 + target.life.initialDelay;
				text25 = text25 + text39 + target.life.lifeTime;
				text26 = text26 + text39 + target.life.respawnTime;
				text27 = text27 + text39 + target.life.livesNumber;
				text28 = text28 + text39 + target.life.probability;
				text29 = text29 + text39 + target.activity.speed.x;
				text30 = text30 + text39 + target.activity.speed.y;
				text31 = text31 + text39 + (int)target.activity.size.initialSize;
				text32 = text32 + text39 + (int)target.activity.size.endSize;
				text33 = text33 + text39 + target.activity.size.duration;
				text34 = text34 + text39 + target.activity.size.startDelay;
				text35 = text35 + text39 + (target.activity.size.loop ? 1 : 0);
				text36 = text36 + text39 + (int)target.activity.positioning.vertical;
				text37 = text37 + text39 + target.activity.positioning.horizontal;
				text38 = text38 + text39 + target.activity.positioning.boundBox;
			}
		}
		json_dictionary.Add("levels_difficulty", text.Remove(0, 1));
		json_dictionary.Add("levels_obstacle", text2.Remove(0, 1));
		json_dictionary.Add("levels_multi_obstacle", text3.Remove(0, 1));
		json_dictionary.Add("levels_distance", text4.Remove(0, 1));
		json_dictionary.Add("levels_position_range", text5.Remove(0, 1));
		json_dictionary.Add("levels_step_back", text6.Remove(0, 1));
		json_dictionary.Add("levels_reset_direction", text7.Remove(0, 1));
		json_dictionary.Add("levels_rounds", text8.Remove(0, 1));
		json_dictionary.Add("levels_round_targets", text9.Remove(0, 1));
		json_dictionary.Add("levels_success_hits", text10.Remove(0, 1));
		json_dictionary.Add("levels_success_text", text11.Remove(0, 1));
		json_dictionary.Add("levels_visual_bound_box", text12.Remove(0, 1));
		json_dictionary.Add("tutorials_type", text13.Remove(0, 1));
		json_dictionary.Add("tutorials_intro", text14.Remove(0, 1));
		json_dictionary.Add("tutorials_success", text15.Remove(0, 1));
		json_dictionary.Add("tutorials_success_delay", text16.Remove(0, 1));
		json_dictionary.Add("tutorials_failure", text17.Remove(0, 1));
		json_dictionary.Add("tutorials_clock", text18.Remove(0, 1));
		json_dictionary.Add("tutorials_gesture_type", text19.Remove(0, 1));
		json_dictionary.Add("tutorials_gesture_time", text20.Remove(0, 1));
		json_dictionary.Add("targets_id", text21.Remove(0, 1));
		json_dictionary.Add("targets_start_score", text22.Remove(0, 1));
		json_dictionary.Add("targets_end_score", text23.Remove(0, 1));
		json_dictionary.Add("targets_initial_delay", text24.Remove(0, 1));
		json_dictionary.Add("targets_life_time", text25.Remove(0, 1));
		json_dictionary.Add("targets_respawn_time", text26.Remove(0, 1));
		json_dictionary.Add("targets_lives_number", text27.Remove(0, 1));
		json_dictionary.Add("targets_probability", text28.Remove(0, 1));
		json_dictionary.Add("targets_speed_x", text29.Remove(0, 1));
		json_dictionary.Add("targets_speed_y", text30.Remove(0, 1));
		json_dictionary.Add("targets_initial_size", text31.Remove(0, 1));
		json_dictionary.Add("targets_end_size", text32.Remove(0, 1));
		json_dictionary.Add("targets_duration", text33.Remove(0, 1));
		json_dictionary.Add("targets_start_delay", text34.Remove(0, 1));
		json_dictionary.Add("targets_loop", text35.Remove(0, 1));
		json_dictionary.Add("targets_vertical_pos", text36.Remove(0, 1));
		json_dictionary.Add("targets_horizontal_pos", text37.Remove(0, 1));
		json_dictionary.Add("targets_bound_box", text38.Remove(0, 1));
	}

	protected override void LoadParameter(string key, string value)
	{
		string[] array = null;
		switch (key)
		{
		case "shooting_distance":
			float.TryParse(value, out distanceFromWall);
			break;
		case "shooting_range":
			float.TryParse(value, out positionRange);
			break;
		case "levels_difficulty":
		{
			string[] array14 = value.Split(';');
			for (int num25 = array14.Length - levels.Count; num25 > 0; num25--)
			{
				levels.Add(new WallLevel());
			}
			for (int num26 = array14.Length - 1; num26 >= 0; num26--)
			{
				levels[num26].difficultyLevel = (DIFFICULTY)int.Parse(array14[num26]);
			}
			break;
		}
		case "levels_obstacle":
		{
			string[] array8 = value.Split(';');
			for (int num13 = array8.Length - levels.Count; num13 > 0; num13--)
			{
				levels.Add(new WallLevel());
			}
			for (int num14 = array8.Length - 1; num14 >= 0; num14--)
			{
				levels[num14].Obstacle = (ObstacleTarget.ID)int.Parse(array8[num14]);
			}
			break;
		}
		case "levels_multi_obstacle":
		{
			string[] array11 = value.Split(';');
			for (int num19 = array11.Length - levels.Count; num19 > 0; num19--)
			{
				levels.Add(new WallLevel());
			}
			for (int num20 = array11.Length - 1; num20 >= 0; num20--)
			{
				levels[num20].AllowLastObstacle = ((int.Parse(array11[num20]) != 0) ? true : false);
			}
			break;
		}
		case "levels_distance":
		{
			string[] array18 = value.Split(';');
			for (int num33 = array18.Length - levels.Count; num33 > 0; num33--)
			{
				levels.Add(new WallLevel());
			}
			for (int num34 = array18.Length - 1; num34 >= 0; num34--)
			{
				float.TryParse(array18[num34], out levels[num34]._settings.distance);
			}
			break;
		}
		case "levels_position_range":
		{
			string[] array3 = value.Split(';');
			for (int num3 = array3.Length - levels.Count; num3 > 0; num3--)
			{
				levels.Add(new WallLevel());
			}
			for (int num4 = array3.Length - 1; num4 >= 0; num4--)
			{
				float.TryParse(array3[num4], out levels[num4]._settings.positionRange);
			}
			break;
		}
		case "levels_step_back":
		{
			string[] array16 = value.Split(';');
			for (int num29 = array16.Length - levels.Count; num29 > 0; num29--)
			{
				levels.Add(new WallLevel());
			}
			for (int num30 = array16.Length - 1; num30 >= 0; num30--)
			{
				levels[num30]._settings.stepsBackwards = (PLAYER_STEPS)int.Parse(array16[num30]);
			}
			break;
		}
		case "levels_reset_direction":
		{
			string[] array7 = value.Split(';');
			for (int num11 = array7.Length - levels.Count; num11 > 0; num11--)
			{
				levels.Add(new WallLevel());
			}
			for (int num12 = array7.Length - 1; num12 >= 0; num12--)
			{
				levels[num12]._settings.resetDirection = ((int.Parse(array7[num12]) != 0) ? true : false);
			}
			break;
		}
		case "levels_rounds":
		{
			string[] array19 = value.Split(';');
			for (int num35 = array19.Length - levels.Count; num35 > 0; num35--)
			{
				levels.Add(new WallLevel());
			}
			for (int num36 = array19.Length - 1; num36 >= 0; num36--)
			{
				levels[num36]._settings.rounds = int.Parse(array19[num36]);
			}
			break;
		}
		case "levels_round_targets":
		{
			string[] array12 = value.Split(';');
			for (int num21 = array12.Length - levels.Count; num21 > 0; num21--)
			{
				levels.Add(new WallLevel());
			}
			for (int num22 = array12.Length - 1; num22 >= 0; num22--)
			{
				levels[num22]._settings.roundTargets = int.Parse(array12[num22]);
			}
			break;
		}
		case "levels_success_hits":
		{
			string[] array6 = value.Split(';');
			for (int num9 = array6.Length - levels.Count; num9 > 0; num9--)
			{
				levels.Add(new WallLevel());
			}
			for (int num10 = array6.Length - 1; num10 >= 0; num10--)
			{
				levels[num10]._settings.successHits = int.Parse(array6[num10]);
			}
			break;
		}
		case "levels_success_text":
		{
			string[] array20 = value.Split(';');
			for (int num37 = array20.Length - levels.Count; num37 > 0; num37--)
			{
				levels.Add(new WallLevel());
			}
			for (int num38 = array20.Length - 1; num38 >= 0; num38--)
			{
				levels[num38]._settings.roundInformation = array20[num38];
			}
			break;
		}
		case "levels_visual_bound_box":
		{
			string[] array15 = value.Split(';');
			for (int num27 = array15.Length - levels.Count; num27 > 0; num27--)
			{
				levels.Add(new WallLevel());
			}
			for (int num28 = array15.Length - 1; num28 >= 0; num28--)
			{
				float.TryParse(array15[num28], out levels[num28]._targetsRange);
			}
			break;
		}
		case "tutorials_type":
		{
			string[] array10 = value.Split(';');
			for (int num17 = array10.Length - levels.Count; num17 > 0; num17--)
			{
				levels.Add(new WallLevel());
			}
			for (int num18 = array10.Length - 1; num18 >= 0; num18--)
			{
				levels[num18].tutorial.type = (TUTORIAL_TYPE)int.Parse(array10[num18]);
			}
			break;
		}
		case "tutorials_intro":
		{
			string[] array4 = value.Split(';');
			for (int num5 = array4.Length - levels.Count; num5 > 0; num5--)
			{
				levels.Add(new WallLevel());
			}
			for (int num6 = array4.Length - 1; num6 >= 0; num6--)
			{
				levels[num6].tutorial.introMessage = array4[num6];
			}
			break;
		}
		case "tutorials_success":
		{
			string[] array21 = value.Split(';');
			for (int num39 = array21.Length - levels.Count; num39 > 0; num39--)
			{
				levels.Add(new WallLevel());
			}
			for (int num40 = array21.Length - 1; num40 >= 0; num40--)
			{
				levels[num40].tutorial.successMessage = array21[num40];
			}
			break;
		}
		case "tutorials_success_delay":
		{
			string[] array17 = value.Split(';');
			for (int num31 = array17.Length - levels.Count; num31 > 0; num31--)
			{
				levels.Add(new WallLevel());
			}
			for (int num32 = array17.Length - 1; num32 >= 0; num32--)
			{
				float.TryParse(array17[num32], out levels[num32].tutorial.successHoldDelay);
			}
			break;
		}
		case "tutorials_failure":
		{
			string[] array13 = value.Split(';');
			for (int num23 = array13.Length - levels.Count; num23 > 0; num23--)
			{
				levels.Add(new WallLevel());
			}
			for (int num24 = array13.Length - 1; num24 >= 0; num24--)
			{
				levels[num24].tutorial.failureMessage = array13[num24];
			}
			break;
		}
		case "tutorials_clock":
		{
			string[] array9 = value.Split(';');
			for (int num15 = array9.Length - levels.Count; num15 > 0; num15--)
			{
				levels.Add(new WallLevel());
			}
			for (int num16 = array9.Length - 1; num16 >= 0; num16--)
			{
				levels[num16].tutorial.clockTime = int.Parse(array9[num16]);
			}
			break;
		}
		case "tutorials_gesture_type":
		{
			string[] array5 = value.Split(';');
			for (int num7 = array5.Length - levels.Count; num7 > 0; num7--)
			{
				levels.Add(new WallLevel());
			}
			for (int num8 = array5.Length - 1; num8 >= 0; num8--)
			{
				levels[num8].tutorial.visualGesture = (VISUAL_GESTURES)int.Parse(array5[num8]);
			}
			break;
		}
		case "tutorials_gesture_time":
		{
			string[] array2 = value.Split(';');
			for (int num = array2.Length - levels.Count; num > 0; num--)
			{
				levels.Add(new WallLevel());
			}
			for (int num2 = array2.Length - 1; num2 >= 0; num2--)
			{
				float.TryParse(array2[num2], out levels[num2].tutorial.trailTime);
			}
			break;
		}
		case "targets_id":
			array = value.Split(';');
			LoadNestedParameter(array, key);
			break;
		case "targets_start_score":
			array = value.Split(';');
			LoadNestedParameter(array, key);
			break;
		case "targets_end_score":
			array = value.Split(';');
			LoadNestedParameter(array, key);
			break;
		case "targets_initial_delay":
			array = value.Split(';');
			LoadNestedParameter(array, key);
			break;
		case "targets_life_time":
			array = value.Split(';');
			LoadNestedParameter(array, key);
			break;
		case "targets_respawn_time":
			array = value.Split(';');
			LoadNestedParameter(array, key);
			break;
		case "targets_lives_number":
			array = value.Split(';');
			LoadNestedParameter(array, key);
			break;
		case "targets_probability":
			array = value.Split(';');
			LoadNestedParameter(array, key);
			break;
		case "targets_speed_x":
			array = value.Split(';');
			LoadNestedParameter(array, key);
			break;
		case "targets_speed_y":
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
		case "targets_start_delay":
			array = value.Split(';');
			LoadNestedParameter(array, key);
			break;
		case "targets_loop":
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
		case "targets_bound_box":
			array = value.Split(';');
			LoadNestedParameter(array, key);
			break;
		}
	}

	public void LoadNestedParameter(string[] str_list, string key)
	{
		int num = str_list.Length;
		int num2 = 0;
		int num3 = 0;
		int num4;
		for (num4 = 0; num4 < num; num4++)
		{
			if (num3 == levels.Count)
			{
				levels.Add(new WallLevel());
			}
			WallLevel wallLevel = levels[num3];
			num2 = int.Parse(str_list[num4]);
			for (int num5 = num2 - wallLevel.targets.Count; num5 > 0; num5--)
			{
				wallLevel.targets.Add(new WallTargetSettings());
			}
			for (int num6 = num2 - 1; num6 >= 0; num6--)
			{
				switch (key)
				{
				case "targets_id":
					wallLevel.targets[num6].id = (TARGET_TYPE)int.Parse(str_list[num4 + 1 + num6]);
					break;
				case "targets_start_score":
					wallLevel.targets[num6].scoreSettings.minScore = int.Parse(str_list[num4 + 1 + num6]);
					break;
				case "targets_end_score":
					wallLevel.targets[num6].scoreSettings.maxScore = int.Parse(str_list[num4 + 1 + num6]);
					break;
				case "targets_initial_delay":
					float.TryParse(str_list[num4 + 1 + num6], out wallLevel.targets[num6].life.initialDelay);
					break;
				case "targets_life_time":
					float.TryParse(str_list[num4 + 1 + num6], out wallLevel.targets[num6].life.lifeTime);
					break;
				case "targets_respawn_time":
					float.TryParse(str_list[num4 + 1 + num6], out wallLevel.targets[num6].life.respawnTime);
					break;
				case "targets_lives_number":
					wallLevel.targets[num6].life.livesNumber = int.Parse(str_list[num4 + 1 + num6]);
					break;
				case "targets_probability":
					float.TryParse(str_list[num4 + 1 + num6], out wallLevel.targets[num6].life.probability);
					break;
				case "targets_speed_x":
					float.TryParse(str_list[num4 + 1 + num6], out wallLevel.targets[num6].activity.speed.x);
					break;
				case "targets_speed_y":
					float.TryParse(str_list[num4 + 1 + num6], out wallLevel.targets[num6].activity.speed.y);
					break;
				case "targets_initial_size":
					wallLevel.targets[num6].activity.size.initialSize = (SIZE)int.Parse(str_list[num4 + 1 + num6]);
					break;
				case "targets_end_size":
					wallLevel.targets[num6].activity.size.endSize = (SIZE)int.Parse(str_list[num4 + 1 + num6]);
					break;
				case "targets_duration":
					float.TryParse(str_list[num4 + 1 + num6], out wallLevel.targets[num6].activity.size.duration);
					break;
				case "targets_start_delay":
					float.TryParse(str_list[num4 + 1 + num6], out wallLevel.targets[num6].activity.size.startDelay);
					break;
				case "targets_loop":
					wallLevel.targets[num6].activity.size.loop = ((int.Parse(str_list[num4 + 1 + num6]) != 0) ? true : false);
					break;
				case "targets_vertical_pos":
					wallLevel.targets[num6].activity.positioning.vertical = (WALL_LOCATION)int.Parse(str_list[num4 + 1 + num6]);
					break;
				case "targets_horizontal_pos":
					float.TryParse(str_list[num4 + 1 + num6], out wallLevel.targets[num6].activity.positioning.horizontal);
					break;
				case "targets_bound_box":
					float.TryParse(str_list[num4 + 1 + num6], out wallLevel.targets[num6].activity.positioning.boundBox);
					break;
				default:
					Debug.LogError("Unrecognize parameter: " + key);
					break;
				}
			}
			num4 += num2;
			num3++;
		}
	}

	public override void FinalizeParameters()
	{
		base.FinalizeParameters();
		WallMinigameData wallData = GameData.MinigameObject.WallData;
		if (shoottingWall == null && WallDisplay.Instance == null)
		{
			shoottingWall = (GameObject)UnityEngine.Object.Instantiate(wallData.wallPrefab);
		}
		_wallDisplay = WallDisplay.Instance;
		m_difficultyManager.SetLevelList(levels.Cast<GameLevel>().ToList());
		if (m_difficultyManager.IsDifficultyAvailable(m_current_difficulty))
		{
			List<WallLevel> list = (List<WallLevel>)m_difficultyManager.GamesAvailableListForDifficulty(m_current_difficulty);
			levels.Clear();
			levels = list;
		}
	}

	public override IEnumerator CustomIntro()
	{
		switch (m_name)
		{
		default:
			GameData.StadiumObject.Settings.SetScenarioLighting(0.5f, 2f);
			break;
		case "Pumpkin":
			break;
		}
		yield return true;
	}

	protected override void GenerateTarget()
	{
	}

	public override Vector3 GenerateShootingPosition()
	{
		if (_currentLevelIndex == -1)
		{
			_currentLevelIndex = ((debugLevel <= 0) ? _currentLevelIndex : (debugLevel - 1));
			NextLevel();
		}
		else if (levels[_currentLevelIndex].IsRoundBased)
		{
			if (levels[_currentLevelIndex].AvailableRounds > 0)
			{
				_wallDisplay.RemoveChildren();
				_wallDisplay.SetTargets(levels[_currentLevelIndex].NextRoundTargets());
			}
			else
			{
				_wallDisplay.RemoveChildren();
				NextLevel();
			}
		}
		else if (!ActiveTargets())
		{
			NextLevel();
		}
		_wallDisplay.UpdateWall();
		CheckRoundsSystem();
		base.PlayerPosition = GetPosition;
		return base.PlayerPosition;
	}

	protected override void GenerateShootingChance()
	{
		GameData.PlayerControl.EnableShoot(bEnable: true);
		_hitCount = 0;
		_comboPointsCount = 0;
		_comboPointsScore = 0;
		_multiplierApplies = false;
	}

	public override void BallFirstCollision(Collision collision)
	{
		if (!Minigame.NextChanceCalled)
		{
			switch (collision.gameObject.tag)
			{
			case "wall_trigger":
				NextChance(0.5f);
				break;
			default:
				NextChance(0.8f);
				break;
			}
		}
	}

	public override void NextChance(float delay = 0f)
	{
		switch (m_name)
		{
		case "Christmas":
			if (!NextLevelAvailable())
			{
				if (CanPassNextLevel())
				{
					SantaController.Instance.TurnOn();
					XmasAssetsManager.Instance.EmitSnowOnTrees();
					if (CurrentLevel().AvailableRounds > 0)
					{
						FinishCurrentChance();
					}
					else
					{
						GameData.MinigameObject.Invoke("FinishCurrentChance", 3f);
					}
				}
				else
				{
					GameData.EndShot(bForce: false);
				}
			}
			else
			{
				GameData.EndShot(bForce: false);
			}
			break;
		default:
			GameData.EndShotWithDelay(bForce: false, delay);
			break;
		}
	}

	public override void FinishCurrentChance()
	{
		GameData.EndShot(bForce: false);
	}

	public override void FinalizeGameSettings()
	{
		GameData.StadiumObject.Settings.EnableLights(bEnable: true);
	}

	public override bool OnShootEnd()
	{
		if (!_multiplierApplies && base.CurrentShield <= 0 && base.CurrentMultiplier > 1)
		{
			LoseMultiplier();
			if (_wallDisplay.ActiveMultiplier())
			{
				_wallDisplay.SwapMultiplier();
			}
		}
		if (_hitCount == 0)
		{
			bool flag = false;
			if (m_current_shield > 0)
			{
				if (m_current_multiplier > 1 || base.UsingLives)
				{
					LoseShield();
					flag = true;
				}
				if (base.CurrentShield == 0)
				{
					if (_GUI.ProgressPanelType == PROGRESS_PANEL_TYPE.LIVES)
					{
						_GUI.ShowLifePanel(bShow: true);
					}
					else if (_GUI.ProgressPanelType == PROGRESS_PANEL_TYPE.SHOTS)
					{
						_GUI.ShowShotsPanel(bShow: true);
					}
					else if (_GUI.ProgressPanelType != PROGRESS_PANEL_TYPE.SHIELD)
					{
					}
				}
				else if (!flag)
				{
				}
			}
			else if (base.UsingLives)
			{
				LoseLife();
			}
		}
		if (IsRoundBased())
		{
		}
		return false;
	}

	public override bool MinigameEnd()
	{
		if (IsRoundBased())
		{
			if (levels[_currentLevelIndex].AvailableRounds > 0)
			{
				return false;
			}
			if (NextLevelAvailable())
			{
				return !CanPassNextLevel();
			}
			return true;
		}
		if (!ActiveTargets() && !NextLevelAvailable() && !base.IsDifficultyBased)
		{
			return true;
		}
		return false;
	}

	public override bool TargetHit(Minigame_TargetTrigger trigger)
	{
		WallTargetTrigger wallTargetTrigger = (WallTargetTrigger)trigger;
		if (wallTargetTrigger.Parent.HasBeenHit || _wallDisplay.WallHasBeenHit)
		{
			return false;
		}
		if (_hitCount == 0 && trigger.m_hit_noise.audioClip == null)
		{
			AudioManager.PlaySFX(AudioManager.Instance.audioCollection.minigameWall.targetHit, 0f);
		}
		_hitCount++;
		ComboHitCount(wallTargetTrigger);
		_multiplierApplies = true;
		wallTargetTrigger.Parent.HasBeenHit = true;
		CurrentWallLevel().CurrentHits++;
		if (Minigame.GameInfoExists())
		{
			GameData.GUIManager.ShowThresholdToBeat(GameData.PlayerStats.CurrentGameInfo().HighScore, m_bronze_score, m_silver_score, m_gold_score, base.Score);
		}
		if (m_gameMode == GAME_MODE.TUTORIAL && CurrentTutorial().type == TUTORIAL_TYPE.BLAST)
		{
			FireUpBlastPhase();
			PassTutorialOfType(TUTORIAL_TYPE.BLAST);
		}
		return true;
	}

	public override float BallShootingHeight(float flickLength)
	{
		return Mathf.Lerp(0.28f, 0.4f, Mathf.InverseLerp(0f, Screen.height / 2, flickLength));
	}

	public override float BallShootingSpeed(float flickLength, float currentHeight)
	{
		Vector2 vector = new Vector2(0f, TargetPosition.x - GameData.Ball.transform.position.x);
		float magnitude = vector.magnitude;
		return Mathf.Sqrt(magnitude * Physics.gravity.magnitude / 0.4f);
	}

	public override float MaxShootAngle()
	{
		return 35f;
	}

	private void ComboHitCount(WallTargetTrigger target)
	{
		if (target.ScoreType == SCORE_TYPE.POINTS)
		{
			_comboPointsCount++;
			_comboPointsScore += target.Points;
		}
	}

	private void RevealScore()
	{
		if (_comboPointsScore > 0)
		{
			_GUI.RevealScoreWithDelay(_comboPointsScore * CurrentBlastPhaseMultiplier(), base.IsBlastPhaseOn);
		}
	}

	public void WallHasBeenHit(Vector3 hitPoint)
	{
		ComboBonus(hitPoint);
		RevealScore();
	}

	private void ComboBonus(Vector3 spawnPoint)
	{
		if (_comboPointsCount > 1)
		{
			_GUI.RevealComboBonus(_comboPointsCount);
			GameData.MinigameObject.CurrentMinigame.AddScore(_comboPointsScore * base.CurrentMultiplier, ApplyStatModBonuses: true, playSound: false);
			AudioManager.PlaySFX(AudioManager.Instance.audioCollection.minigameWall.multipleHit, 0f);
		}
		if (m_gameMode == GAME_MODE.TUTORIAL)
		{
			if (_comboPointsCount >= 3)
			{
				PassTutorialOfType(TUTORIAL_TYPE.TRIPLE_HIT);
			}
			if (_comboPointsCount > 3)
			{
				PassTutorialOfType(TUTORIAL_TYPE.OVER_TRIPLE);
			}
		}
	}

	public override bool ResetPlayerCameraDirection()
	{
		distanceFromWall += StepBackwards;
		if (CurrentWallLevel()._settings.resetDirection)
		{
			GameData.GameCamera.LookAt(new Vector3(15.8f, 1.5f, 0f));
		}
		bool result = false;
		if (!_main_camera_setup)
		{
			result = true;
			_main_camera_setup = true;
		}
		return result;
	}

	public override void BallKicked()
	{
		GameData.PathManager.ShowTrail(show: false);
		_GUI.ShowTutorialGesture(bShow: false);
		_GUI.DismissTutorialText();
	}

	private void SetCurrrentLevel(int level)
	{
		bool flag = false;
		if (IsRoundBased())
		{
			_wallDisplay.RemoveChildren();
			flag = true;
			switch (m_name)
			{
			case "Christmas":
				if (XmasLightsController.Instance != null)
				{
					XmasLightsController.Instance.TurnOnAtIndex(_currentLevelIndex);
				}
				break;
			}
		}
		AudioManager.PlaySFX(AudioManager.Instance.audioCollection.minigameWall.roundPassed, 0f);
		if (_currentLevelIndex == level && !IsTutorialBased() && !base.IsDifficultyBased)
		{
			return;
		}
		float delay = ((_currentLevelIndex < 0) ? 0f : CurrentTutorial().successHoldDelay);
		_wallDisplay.DestroyHiddenTargets();
		_currentLevelIndex = level;
		CurrentWallLevel().TargetsRange = 17f;
		SetBounds();
		_wallDisplay.SetTargets(levels[_currentLevelIndex].TargetsToSpawn());
		_GUI.ShowTutorialMessage(CurrentTutorial().introMessage, delay);
		if (IsRoundBased())
		{
			m_total_tries += levels[_currentLevelIndex]._settings.rounds + 1;
		}
		else
		{
			m_total_tries = 200;
		}
		if (IsRoundBased())
		{
			m_rounds_panel_setup = false;
			CheckRoundsSystem();
			DisplayInstructions(CurrentLevel().IndexID, CurrentLevel().IndexID == levels.Count - 1);
			if (_currentLevelIndex > 0 && !flag)
			{
				_GUI.ShowLifePanel(bShow: false);
				_GUI.ShowShotsPanel(bShow: true, base.CurrentShield > 0);
			}
		}
		else
		{
			if (_currentLevelIndex != 0 || GameData.MinigameObject.CurrentMinigame.DrillDescription != string.Empty)
			{
			}
			if (_currentLevelIndex > 0 && flag)
			{
				_GUI.ShowShotsPanel(bShow: false);
				_GUI.ShowLifePanel(bShow: true, base.CurrentShield > 0);
			}
		}
		if (m_gameMode == GAME_MODE.TUTORIAL)
		{
			if (CurrentTutorial().clockTime > 0)
			{
				m_time_limit = CurrentTutorial().clockTime;
				CheckTimeSystem(bEnable: true);
			}
			_GUI.ShowTimerPanel(m_time_limit > 0);
		}
		if (CurrentWallLevel().Distance > 0f)
		{
			PlayerDistance = CurrentWallLevel().Distance;
		}
		positionRange = ((!(CurrentWallLevel()._settings.positionRange >= 0f)) ? positionRange : CurrentWallLevel()._settings.positionRange);
	}

	public void NextLevel()
	{
		if (_currentLevelIndex != -1)
		{
			CurrentLevel().Finish();
		}
		if (NextLevelAvailable() || base.IsDifficultyBased)
		{
			int currrentLevel = _currentLevelIndex;
			string message = CurrentTutorial().failureMessage;
			Color textColor = Color.red;
			if (HasPassedTutorial())
			{
				message = ((_currentLevelIndex < 0) ? string.Empty : CurrentTutorial().successMessage);
				textColor = Color.white;
				currrentLevel = _currentLevelIndex + 1;
			}
			if (base.IsDifficultyBased)
			{
				currrentLevel = m_difficultyManager.NextLevelIndex();
			}
			_GUI.ShowTutorialResult(message, textColor, CurrentTutorial().successHoldDelay);
			if (!m_difficultyManager.IsReadyToEnd)
			{
				SetCurrrentLevel(currrentLevel);
				return;
			}
		}
		Debug.Log("No more Tiers available, finishing game...");
		GameData.MinigameObject.EndMinigame();
	}

	public bool HasPassedTutorial()
	{
		if (IsTutorialBased() && _currentLevelIndex >= 0)
		{
			return CurrentWallLevel().TutorialPassed;
		}
		return true;
	}

	public void PassTutorialOfType(TUTORIAL_TYPE type)
	{
		if (CurrentWallLevel().tutorial.type == type)
		{
			CurrentWallLevel().TutorialPassed = true;
		}
	}

	public bool IsRoundBased()
	{
		if (_currentLevelIndex >= 0)
		{
			return levels[_currentLevelIndex].IsRoundBased;
		}
		return false;
	}

	public bool NextLevelAvailable()
	{
		return _currentLevelIndex < levels.Count - 1;
	}

	public override GameLevel CurrentLevel()
	{
		return levels[CurrentIndex()];
	}

	private int CurrentIndex()
	{
		return (_currentLevelIndex >= 0) ? _currentLevelIndex : 0;
	}

	public WallLevel CurrentWallLevel()
	{
		return (WallLevel)CurrentLevel();
	}

	public Tutorial CurrentTutorial()
	{
		return CurrentWallLevel().tutorial;
	}

	private bool ActiveTargets()
	{
		return _wallDisplay.ActiveTargets();
	}

	private bool CanPassNextLevel()
	{
		return levels[_currentLevelIndex].CanPassNextRound;
	}

	private bool IsTutorialBased()
	{
		return CurrentWallLevel().tutorial.type != TUTORIAL_TYPE.DEFAULT;
	}

	public bool IsHalloweenGame()
	{
		return m_name == "Pumpkin";
	}

	private void SetBounds()
	{
		if (CurrentWallLevel().TargetsRange <= 0f)
		{
			if (_rightPost != null)
			{
				_rightPost.GetComponent<BoundingPost>().Scale(scaleUp: false);
			}
			if (_leftPost != null)
			{
				_leftPost.GetComponent<BoundingPost>().Scale(scaleUp: false);
			}
			return;
		}
		if (_rightPost == null)
		{
			_rightPost = UnityEngine.Object.Instantiate(GameData.MinigameObject.WallData.postPrefab) as GameObject;
			Vector3 position = _rightPost.transform.position;
			position.z = 0f - CurrentWallLevel().TargetsRange / 2f;
			_rightPost.transform.position = position;
			_rightPost.GetComponent<BoundingPost>().Scale(scaleUp: true);
		}
		else
		{
			iTween.MoveTo(_rightPost, new Vector3(_rightPost.transform.position.x, _rightPost.transform.position.y, 0f - CurrentWallLevel().TargetsRange / 2f), 2f);
		}
		if (_leftPost == null)
		{
			_leftPost = UnityEngine.Object.Instantiate(GameData.MinigameObject.WallData.postPrefab) as GameObject;
			Vector3 position2 = _leftPost.transform.position;
			position2.z = CurrentWallLevel().TargetsRange / 2f;
			_leftPost.transform.position = position2;
			_leftPost.GetComponent<BoundingPost>().Scale(scaleUp: true);
		}
		else
		{
			iTween.MoveTo(_leftPost, new Vector3(_leftPost.transform.position.x, _leftPost.transform.position.y, CurrentWallLevel().TargetsRange / 2f), 2f);
		}
	}

	public override void OnDrawGizmos(Minigame minigame)
	{
		base.OnDrawGizmos(minigame);
		Vector3 center = new Vector3(15.8f - distanceFromWall, 0f, 0f);
		Gizmos.color = Color.red;
		Gizmos.DrawSphere(center, 0.5f);
		Gizmos.color = Color.blue;
		Gizmos.DrawWireCube(center, new Vector3(1f, 1f, positionRange * 2f));
	}
}
