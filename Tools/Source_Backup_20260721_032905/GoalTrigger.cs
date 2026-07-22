using System.Runtime.CompilerServices;
using UnityEngine;

public class GoalTrigger : HitEvent
{
	public delegate void HandleGoalEntered(GoalTrigger goalTrigger);

	private Vector3 _hitPosition;

	public Vector3 HitPosition => _hitPosition;

	[method: MethodImpl(32)]
	public static event HandleGoalEntered GoalEntered;

	private void OnEnable()
	{
		SoccerBall.StateHasChanged += HandleBallFirstCollision;
	}

	private void OnDisable()
	{
		SoccerBall.StateHasChanged -= HandleBallFirstCollision;
	}

	private void HandleBallFirstCollision(SoccerBall.STATE newState)
	{
		if (newState == SoccerBall.STATE.STATIC)
		{
			Reset();
		}
	}

	protected override void OnActionEnter(Collider other)
	{
		ValidHit();
		_hitPosition = other.transform.position;
		if (GoalTrigger.GoalEntered != null)
		{
			GoalTrigger.GoalEntered(this);
		}
	}

	public override void Reset()
	{
		base.Reset();
		_hitPosition = Vector3.zero;
	}
}
