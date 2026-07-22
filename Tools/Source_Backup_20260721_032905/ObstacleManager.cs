using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class ObstacleManager : MonoBehaviour
{
	public delegate void HandleObstacleUpdate();

	private GridManager _positionsGrid = new GridManager();

	[SerializeField]
	private List<ObstacleTarget> _obstaclePrefabs = new List<ObstacleTarget>();

	public static ObstacleManager Instance;

	private int _hitCounter;

	private GameObject _currentWall;

	private List<ObstacleTarget> _currentObstacles = new List<ObstacleTarget>();

	private List<ObstacleTarget> _obstaclesHit = new List<ObstacleTarget>();

	public int ObstaclesActive => _currentObstacles.Count;

	public bool AnyObstacleActive => ObstaclesActive > 0;

	[method: MethodImpl(32)]
	public static event HandleObstacleUpdate ObstacleUpdate;

	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
		}
		_hitCounter = PlayerPrefs.GetInt("obstacle_hit", 0);
	}

	private void OnEnable()
	{
		ObstacleTarget.ObstacleHit += ObstacleHasBeenHit;
	}

	private void OnDisable()
	{
		ObstacleTarget.ObstacleHit -= ObstacleHasBeenHit;
	}

	public void SetupPositions(GridManager currentGrid)
	{
		_positionsGrid = currentGrid;
	}

	private void ObstacleHasBeenHit(ObstacleTarget obstacle)
	{
		_obstaclesHit.Add(obstacle);
		CheckHitCounter();
	}

	private void CheckHitCounter()
	{
		_hitCounter++;
		PlayerPrefs.SetInt("obstacle_hit", _hitCounter);
		if (_hitCounter == 1)
		{
			PlayerManager.TrophyUnlocked(Achievement.TROPHY.Lucky_Strike);
		}
		else if (_hitCounter == 100)
		{
			PlayerManager.TrophyUnlocked(Achievement.TROPHY.On_The_Spot);
		}
		else if (_hitCounter == 1000)
		{
			PlayerManager.TrophyUnlocked(Achievement.TROPHY.Freekick_Specialist);
		}
	}

	public int GetCurrentBonusPoints()
	{
		int num = 0;
		foreach (ObstacleTarget item in _obstaclesHit)
		{
			num += item.CurrentScore;
		}
		return num;
	}

	public void BringInObstacle(ObstacleTarget.ID obstacleID)
	{
		if (_positionsGrid.AreAllPositionsTaken())
		{
			Debug.Log("Can't bring any more obstacles in, there no position available to take");
			return;
		}
		GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(_obstaclePrefabs.Find((ObstacleTarget obs) => obs.Identifier == obstacleID).gameObject);
		ObstacleTarget component = gameObject.GetComponent<ObstacleTarget>();
		component.SetGridPosition(GetGridCenterUnit());
		_currentObstacles.Add(component);
	}

	public void BringInWall(ObstacleTarget.ID obstacleID, Vector3 target, Vector3 shootPosition, Vector3 offset)
	{
		if (obstacleID == ObstacleTarget.ID.NONE)
		{
			if (_currentWall != null)
			{
				UnityEngine.Object.Destroy(_currentWall);
			}
			return;
		}
		if (_currentWall == null)
		{
			_currentWall = (GameObject)UnityEngine.Object.Instantiate(_obstaclePrefabs.Find((ObstacleTarget obs) => obs.Identifier == obstacleID).gameObject);
		}
		else if (_currentWall.GetComponent<ObstacleTarget>().Identifier != obstacleID)
		{
			UnityEngine.Object.DestroyImmediate(_currentWall);
			_currentWall = (GameObject)UnityEngine.Object.Instantiate(_obstaclePrefabs.Find((ObstacleTarget obs) => obs.Identifier == obstacleID).gameObject);
		}
		UpdateWallPosition(target, shootPosition, offset);
	}

	public void BringRandomWall(Vector3 target, Vector3 shootPosition, Vector3 offset)
	{
		int obstacleID = UnityEngine.Random.Range(1, Enum.GetValues(typeof(ObstacleTarget.ID)).Length);
		BringInWall((ObstacleTarget.ID)obstacleID, target, shootPosition, offset);
	}

	public void UpdateWallPosition(Vector3 target, Vector3 shootPosition, Vector3 offset)
	{
		if (!(_currentWall == null))
		{
			_currentWall.transform.position = (target - shootPosition) / 2f + shootPosition + offset;
			_currentWall.transform.LookAt(target);
		}
	}

	public void RemoveObstacle(ObstacleTarget.ID obstacleID)
	{
		ObstacleTarget obstacle = _currentObstacles.Find((ObstacleTarget obs) => obs.Identifier == obstacleID);
		RemoveObstacle(obstacle);
	}

	public void RemoveWall()
	{
		if (_currentWall != null)
		{
			UnityEngine.Object.Destroy(_currentWall);
		}
	}

	public void RemoveAll()
	{
		RemoveObstacleList(_obstaclesHit);
		RemoveObstacleList(_currentObstacles);
		if (_currentWall != null)
		{
			UnityEngine.Object.DestroyImmediate(_currentWall.gameObject);
		}
	}

	private void FreeUpObstaclePosition(ObstacleTarget obstacle)
	{
		if (obstacle != null)
		{
			_positionsGrid.ReleaseGridUnit(obstacle.Grid);
		}
	}

	private void RemoveObstacle(ObstacleTarget obstacle)
	{
		FreeUpObstaclePosition(obstacle);
		_currentObstacles.Remove(obstacle);
		if (obstacle != null)
		{
			UnityEngine.Object.Destroy(obstacle.gameObject);
		}
	}

	private void RemoveObstacleList(List<ObstacleTarget> obstacles)
	{
		foreach (ObstacleTarget obstacle in obstacles)
		{
			if (obstacle != null)
			{
				FreeUpObstaclePosition(obstacle);
				UnityEngine.Object.Destroy(obstacle.gameObject);
			}
		}
		obstacles.Clear();
	}

	private void SortByDifficulty()
	{
		_currentObstacles.Sort((ObstacleTarget x, ObstacleTarget y) => x.Type.CompareTo(y.Type));
	}

	public void Reset()
	{
		_obstaclesHit.Clear();
	}

	public void UpdateObstacles(bool bSuccess)
	{
		if (ObstacleManager.ObstacleUpdate != null)
		{
			ObstacleManager.ObstacleUpdate();
		}
	}

	public void SetAvailablePositions(GridManager currentGrid)
	{
		_positionsGrid = currentGrid;
	}

	private GridUnit GetGridCenterUnit()
	{
		return _positionsGrid.GetCenterPositionInGrid();
	}
}
