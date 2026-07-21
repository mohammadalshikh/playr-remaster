using UnityEngine;

public class MinigameTarget : MonoBehaviour
{
	[SerializeField]
	protected SCORE_TYPE _scoreType;

	[SerializeField]
	protected int _currentScore;

	protected bool _hasBeenHit;

	public SCORE_TYPE ScoreType => _scoreType;

	public virtual bool HasBeenHit
	{
		get
		{
			return _hasBeenHit;
		}
		set
		{
			_hasBeenHit = value;
		}
	}

	public int CurrentScore
	{
		get
		{
			return _currentScore;
		}
		set
		{
			_currentScore = value;
		}
	}

	public void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("ball"))
		{
			OnActionTrigger(other);
		}
	}

	public void OnCollisionEnter(Collision other)
	{
		if (other.gameObject.CompareTag("ball"))
		{
			OnActionCollision(other.collider);
		}
	}

	public virtual void OnActionCollision(Collider collider)
	{
		GameData.MinigameObject.CurrentMinigame.CustomTargetHit(this);
	}

	public virtual void OnActionTrigger(Collider collider)
	{
		GameData.MinigameObject.CurrentMinigame.CustomTargetHit(this);
	}
}
