using System;
using UnityEngine;

[Serializable]
public class TargetSettings
{
	public enum SIZE
	{
		SMALL,
		MEDIUM,
		LARGE
	}

	[Serializable]
	public class Score
	{
		[SerializeField]
		private int _min;

		[SerializeField]
		private int _max;

		public int Min
		{
			get
			{
				return _min;
			}
			set
			{
				_min = value;
			}
		}

		public int Max
		{
			get
			{
				return _max;
			}
			set
			{
				_max = value;
			}
		}
	}

	[Serializable]
	public class Size
	{
		[SerializeField]
		private SIZE _start = SIZE.MEDIUM;

		[SerializeField]
		private SIZE _end = SIZE.MEDIUM;

		[SerializeField]
		private float _duration;

		[SerializeField]
		private LOOP_TYPE _loop;

		public SIZE Start
		{
			get
			{
				return _start;
			}
			set
			{
				_start = value;
			}
		}

		public SIZE End
		{
			get
			{
				return _end;
			}
			set
			{
				_end = value;
			}
		}

		public float Duration
		{
			get
			{
				return _duration;
			}
			set
			{
				_duration = value;
			}
		}

		public LOOP_TYPE LoopType
		{
			get
			{
				return _loop;
			}
			set
			{
				_loop = value;
			}
		}
	}

	[SerializeField]
	protected Score _score = new Score();

	[SerializeField]
	protected Vector2 _speed;

	[SerializeField]
	protected Size _size = new Size();

	public Vector2 Speed
	{
		get
		{
			return _speed;
		}
		set
		{
			_speed = value;
		}
	}

	public Score ScoreSettings => _score;

	public Size SizeSettings => _size;
}
