using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GridManager
{
	[SerializeField]
	private int _width;

	[SerializeField]
	private int _height;

	[SerializeField]
	private float _offsetX;

	[SerializeField]
	private float _offsetY;

	[SerializeField]
	private Vector2 _origin = Vector2.zero;

	private List<GridUnit> _allUnits = new List<GridUnit>();

	public int Width
	{
		get
		{
			return _width;
		}
		set
		{
			_width = value;
		}
	}

	public int Height
	{
		get
		{
			return _height;
		}
		set
		{
			_height = value;
		}
	}

	public Vector2 Origin
	{
		get
		{
			return _origin;
		}
		set
		{
			_origin = value;
		}
	}

	public float OffsetX
	{
		get
		{
			return _offsetX;
		}
		set
		{
			_offsetX = value;
		}
	}

	public float OffsetY
	{
		get
		{
			return _offsetY;
		}
		set
		{
			_offsetY = value;
		}
	}

	public void Init()
	{
		_allUnits.Clear();
		for (int i = 0; i < _width; i++)
		{
			for (int j = 0; j < _height; j++)
			{
				GridUnit gridUnit = new GridUnit();
				gridUnit.SetUp(new Vector2(_origin.x + _offsetX * (float)i, _origin.y + _offsetY * (float)j), i, j);
				_allUnits.Add(gridUnit);
			}
		}
	}

	public GridUnit GetCenterPositionInGrid()
	{
		return GetAvailableUnitAt(_width / 2, _height / 2);
	}

	public GridUnit GetAvailableUnitAt(int x, int y)
	{
		if (AreAllPositionsTaken())
		{
			Debug.LogWarning("All positions are taken, returning 0,0");
			return new GridUnit();
		}
		x = Mathf.Clamp(x, 0, _width);
		y = Mathf.Clamp(y, 0, _height);
		if (IsPositionAvailableAt(x, y))
		{
			return GetGridUnitAt(x, y);
		}
		int num = 0;
		int num2 = 0;
		do
		{
			num = UnityEngine.Random.Range(0, _width);
			num2 = UnityEngine.Random.Range(0, _height);
		}
		while (!IsPositionAvailableAt(num, num2));
		Debug.LogWarning("Position at " + x + "," + y + " is not available returning " + num + "," + num2);
		return GetGridUnitAt(num, num2);
	}

	public void ReleaseGridUnit(GridUnit unit)
	{
		_allUnits.Find((GridUnit g) => g == unit)?.FreeUp();
	}

	private GridUnit GetGridUnitAt(int x, int y)
	{
		GridUnit gridUnit = FindGridUnitAt(x, y);
		gridUnit.Use();
		return gridUnit;
	}

	private bool IsPositionAvailableAt(int x, int y)
	{
		return _allUnits.Exists((GridUnit g) => g.CompareGridPosition(x, y) && g.IsAvailable);
	}

	private GridUnit FindGridUnitAt(int x, int y)
	{
		return _allUnits.Find((GridUnit g) => g.CompareGridPosition(x, y));
	}

	public bool AreAllPositionsTaken()
	{
		foreach (GridUnit allUnit in _allUnits)
		{
			if (allUnit.IsAvailable)
			{
				return false;
			}
		}
		return true;
	}
}
