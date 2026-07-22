using UnityEngine;

public class PlayAnimOnHit : HitEvent
{
	public TRIGGER_TYPE fireEvent;

	public Animation animationEvent;

	public string clipName;

	protected override void OnActionEnter(Collider other)
	{
		FireEvent(TRIGGER_TYPE.ON_ENTER);
	}

	protected override void OnActionExit(Collider other)
	{
		FireEvent(TRIGGER_TYPE.ON_EXIT);
	}

	protected override void OnActionStay(Collider other)
	{
		FireEvent(TRIGGER_TYPE.ON_STAY);
	}

	protected virtual void FireEvent(TRIGGER_TYPE callingFrom)
	{
		if (callingFrom == fireEvent)
		{
			if (animationEvent == null)
			{
				animationEvent = GetComponent<Animation>();
			}
			if (clipName.Equals(string.Empty))
			{
				animationEvent.Play();
			}
			else
			{
				animationEvent.Play(clipName);
			}
		}
	}
}
