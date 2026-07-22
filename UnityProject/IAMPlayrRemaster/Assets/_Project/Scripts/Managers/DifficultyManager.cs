using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DifficultyManager
{
	public int easyLevels;

	public int normalLevels;

	public int hardLevels;

	public int proLevels;

	private List<GameLevel> _levelList = new List<GameLevel>();

	private bool _readyToEnd;

	public bool IsReadyToEnd
	{
		get
		{
			return _readyToEnd;
		}
		set
		{
			_readyToEnd = value;
		}
	}

	public bool IsEnabled()
	{
		return easyLevels != 0 || normalLevels != 0 || hardLevels != 0 || proLevels != 0;
	}

	public int GetNumberOfLevelsFor(DIFFICULTY diff)
	{
		return diff switch
		{
			DIFFICULTY.EASY => easyLevels, 
			DIFFICULTY.NORMAL => normalLevels, 
			DIFFICULTY.HARD => hardLevels, 
			DIFFICULTY.PRO => proLevels, 
			_ => 0, 
		};
	}

	public bool IsLevelInfite(DIFFICULTY diff)
	{
		return GetNumberOfLevelsFor(diff) < 0;
	}

	public bool IsLevelEnabledFor(DIFFICULTY diff)
	{
		return GetNumberOfLevelsFor(diff) != 0;
	}

	public void ConsumeLevelFor(DIFFICULTY diff)
	{
		switch (diff)
		{
		case DIFFICULTY.EASY:
			easyLevels--;
			break;
		case DIFFICULTY.NORMAL:
			normalLevels--;
			break;
		case DIFFICULTY.HARD:
			hardLevels--;
			break;
		case DIFFICULTY.PRO:
			proLevels--;
			break;
		}
	}

	public void SetLevelList(List<GameLevel> levelList)
	{
		_levelList = levelList;
		foreach (GameLevel level in _levelList)
		{
			level.IndexID = _levelList.IndexOf(level);
			level.HasBeenPlayed = false;
		}
	}

	public int NextLevelIndex()
	{
		if (_levelList.Count == 0)
		{
			Debug.LogWarning("No Level List has been passed to DifficultyManager");
		}
		int num = -1;
		for (int i = 0; i < Enum.GetNames(typeof(DIFFICULTY)).Length; i++)
		{
			if (i != 0 && IsDifficultyAvailable((DIFFICULTY)i) && (IsLevelEnabledFor((DIFFICULTY)i) || IsLevelInfite((DIFFICULTY)i)))
			{
				num = GetRandomGameForDifficulty((DIFFICULTY)i).IndexID;
				ConsumeLevelFor((DIFFICULTY)i);
				break;
			}
		}
		if (num == -1)
		{
			_readyToEnd = true;
		}
		return num;
	}

	public bool IsDifficultyAvailable(DIFFICULTY diff)
	{
		if (diff == DIFFICULTY.NONE)
		{
			return false;
		}
		foreach (GameLevel level in _levelList)
		{
			if (level.difficultyLevel == diff && !level.HasBeenPlayed)
			{
				return true;
			}
		}
		return false;
	}

	public IList GamesAvailableListForDifficulty(DIFFICULTY difficulty)
	{
		List<GameLevel> list = new List<GameLevel>();
		foreach (GameLevel level in _levelList)
		{
			if (level.difficultyLevel == difficulty && (!level.HasBeenPlayed || IsLevelInfite(difficulty)))
			{
				list.Add(level);
			}
		}
		return list;
	}

	private GameLevel GetRandomGameForDifficulty(DIFFICULTY difficulty)
	{
		List<GameLevel> list = (List<GameLevel>)GamesAvailableListForDifficulty(difficulty);
		return list[UnityEngine.Random.Range(0, list.Count - 1)];
	}
}
