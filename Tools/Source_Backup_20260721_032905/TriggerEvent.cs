using System;
using UnityEngine;

[Serializable]
public class TriggerEvent
{
	[SerializeField]
	private TRIGGER_TYPE _type;

	[SerializeField]
	private MonoBehaviour _scriptWithEventToInvoke;

	[SerializeField]
	private string _eventNameToInvoke;

	[SerializeField]
	private float _delay;

	public TRIGGER_TYPE Type => _type;

	public void Fire()
	{
		if (_scriptWithEventToInvoke != null)
		{
			_scriptWithEventToInvoke.Invoke(_eventNameToInvoke, _delay);
		}
	}
}
