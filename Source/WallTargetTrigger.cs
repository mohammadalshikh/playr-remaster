using UnityEngine;

public class WallTargetTrigger : Minigame_TargetTrigger
{
	private WallTarget _parentObject;

	private WallDisplay wallDisplay;

	public WallTarget Parent => _parentObject;

	public SCORE_TYPE ScoreType
	{
		get
		{
			if (_parentObject != null)
			{
				return _parentObject.ScoreType;
			}
			return SCORE_TYPE.POINTS;
		}
	}

	public int Points
	{
		get
		{
			if (_parentObject != null)
			{
				return _parentObject.CurrentScore;
			}
			return 0;
		}
	}

	private void Start()
	{
		_parentObject = base.transform.parent.GetComponent<WallTarget>();
		wallDisplay = _parentObject.transform.parent.GetComponent<WallDisplay>();
	}

	public override void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("ball") && !wallDisplay.WallHasBeenHit)
		{
			_parentObject.OnHitEnter(this);
			base.OnTriggerEnter(other);
		}
	}

	public void SetSize(float _size)
	{
		base.transform.localScale = new Vector3(_size, _size, 1f) * 0.5f;
	}
}
