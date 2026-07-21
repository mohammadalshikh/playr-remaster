using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ShootTargetSettings : TargetSettings
{
	public enum HORIZONTAL
	{
		LEFT,
		CENTER,
		RIGHT
	}

	public enum VERTICAL
	{
		TOP,
		CENTER,
		BOTTOM
	}

	[SerializeField]
	private ShootingTarget.ID _id = ShootingTarget.ID.ARCHERY;

	[SerializeField]
	private VERTICAL _verticalPos = VERTICAL.CENTER;

	[SerializeField]
	private HORIZONTAL _horizontalPos = HORIZONTAL.CENTER;

	[SerializeField]
	private float _xBoundaries;

	private Dictionary<HORIZONTAL, float> _xPosition = new Dictionary<HORIZONTAL, float>
	{
		{
			HORIZONTAL.LEFT,
			-3.5f
		},
		{
			HORIZONTAL.CENTER,
			0f
		},
		{
			HORIZONTAL.RIGHT,
			3.5f
		}
	};

	private Dictionary<VERTICAL, float> _yPosition = new Dictionary<VERTICAL, float>
	{
		{
			VERTICAL.TOP,
			1.8f
		},
		{
			VERTICAL.CENTER,
			1.2f
		},
		{
			VERTICAL.BOTTOM,
			0.6f
		}
	};

	public VERTICAL VerticalPosition
	{
		get
		{
			return _verticalPos;
		}
		set
		{
			_verticalPos = value;
		}
	}

	public HORIZONTAL HorizontalPosition
	{
		get
		{
			return _horizontalPos;
		}
		set
		{
			_horizontalPos = value;
		}
	}

	public float XBoundaries
	{
		get
		{
			return _xBoundaries;
		}
		set
		{
			_xBoundaries = value;
		}
	}

	public ShootingTarget.ID Identifier
	{
		get
		{
			return _id;
		}
		set
		{
			_id = value;
		}
	}

	public Vector3 Position => new Vector3(52f, _yPosition[_verticalPos], 0f - _xPosition[_horizontalPos]);

	public float StartSize => _size.Start switch
	{
		SIZE.SMALL => 0.5f, 
		SIZE.MEDIUM => 0.7f, 
		SIZE.LARGE => 1f, 
		_ => 1f, 
	};

	public float EndSize => _size.End switch
	{
		SIZE.SMALL => 0.5f, 
		SIZE.MEDIUM => 0.7f, 
		SIZE.LARGE => 1f, 
		_ => 1f, 
	};

	public void RandomSettingsForPowerup(ShootingTarget.ID id)
	{
		_id = id;
		_size.Start = SIZE.LARGE;
		_size.End = SIZE.LARGE;
		_speed = new Vector2(UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 0.3f));
		_verticalPos = (VERTICAL)UnityEngine.Random.Range(0, 2);
		_horizontalPos = (HORIZONTAL)UnityEngine.Random.Range(0, 2);
	}
}
