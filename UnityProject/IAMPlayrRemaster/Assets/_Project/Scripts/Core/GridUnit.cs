using UnityEngine;

public class GridUnit
{
	private Vector2 _worldPosition = Vector2.zero;

	private bool _isAvailable = true;

	private int _gridPositionX;

	private int _gridPositionY;

	public bool IsAvailable => _isAvailable;

	public virtual Vector3 WorldPosition => new Vector3(_worldPosition.y, 0f, _worldPosition.x);

	public void SetUp(Vector2 position, int x, int y)
	{
		_worldPosition = position;
		_gridPositionX = x;
		_gridPositionY = y;
	}

	public bool CompareGridPosition(int x, int y)
	{
		return _gridPositionX == x && _gridPositionY == y;
	}

	public void Use()
	{
		_isAvailable = false;
	}

	public void FreeUp()
	{
		_isAvailable = true;
	}
}
