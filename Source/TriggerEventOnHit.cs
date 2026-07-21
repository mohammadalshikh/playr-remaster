using System.Collections.Generic;
using UnityEngine;

public class TriggerEventOnHit : HitEvent
{
	[SerializeField]
	private List<TriggerEvent> _events = new List<TriggerEvent>();

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
		foreach (TriggerEvent @event in _events)
		{
			if (@event.Type == callingFrom)
			{
				@event.Fire();
			}
		}
	}
}
