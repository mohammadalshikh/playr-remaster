using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public class ShootingTarget : SpriteTarget
{
	public enum ID
	{
		MULTIPLIER,
		HEART,
		SHIELD,
		TIMER,
		ARCHERY
	}

	public delegate void HandleTargetRemoval(ShootingTarget target);

	[SerializeField]
	private ID _id = ID.ARCHERY;

	public ID Identifier => _id;

	[method: MethodImpl(32)]
	public static event HandleTargetRemoval WillRemoveTarget;

	public override void OnEnable()
	{
		base.OnEnable();
		GoalTrigger.GoalEntered += HandleGoalEntered;
	}

	public override void OnDisable()
	{
		base.OnDisable();
		GoalTrigger.GoalEntered -= HandleGoalEntered;
	}

	public void Setup(ShootTargetSettings settings)
	{
		Initialize();
		Init(settings.Speed, settings.ScoreSettings.Min);
		SizeSettings(settings.StartSize, settings.EndSize, settings.SizeSettings.Duration, settings.SizeSettings.LoopType);
		float width = ((!(settings.XBoundaries > 0f)) ? 7.5f : Mathf.Clamp(settings.XBoundaries, 0f, 7.5f));
		SetBounds(new Rect(0f, 1.2f, width, 2.4f));
		SetPosition(settings.Position);
		UpdateColor();
	}

	protected override void HandleBallStateChanged(SoccerBall.STATE newState)
	{
		if (newState == SoccerBall.STATE.STATIC)
		{
			if (WasHit)
			{
				RemoveFromView();
			}
			else
			{
				Reset();
			}
		}
	}

	protected override void ShowCelebration(Vector3 celebrationPosition)
	{
		if (base.Accuracy > 0.895f && base.ScoreType == SCORE_TYPE.POINTS)
		{
			iTween.ValueTo(base.gameObject, iTween.Hash("name", "super_celeb", "from", 0.5f, "to", 1f, "time", 0.2f, "looptype", iTween.LoopType.pingPong, "onUpdate", (Action<object>)delegate(object x)
			{
				_sprite.SetColor(new Color(1f - (float)x, (float)x, 0.20392157f, (float)x - 0.3f));
			}));
			if (base.Accuracy > 0.995f)
			{
				GameData.MinigameObject.ShootingData.FlameCelebrationAt(base.transform.position);
			}
			else
			{
				GameData.MinigameObject.ShootingData.SuperCelebrationAt(base.transform.position);
			}
		}
		else
		{
			_sprite.SetColor(new Color(0f, 1f, 0.20392157f, 0.5f));
		}
	}

	private void HandleGoalEntered(GoalTrigger goalTrigger)
	{
		base.IsActive = false;
	}

	public override void RemoveFromView()
	{
		if (ShootingTarget.WillRemoveTarget != null)
		{
			ShootingTarget.WillRemoveTarget(this);
		}
		base.RemoveFromView();
	}

	public void DestroyTarget()
	{
		base.RemoveFromView();
	}
}
