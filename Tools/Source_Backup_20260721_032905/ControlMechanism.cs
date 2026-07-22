using System.Collections.Generic;
using UnityEngine;

public class ControlMechanism : MonoBehaviour
{
	public virtual void ShotEnabled()
	{
	}

	public virtual void ShotDisabled()
	{
	}

	public virtual void Reset()
	{
	}

	public virtual void Kick()
	{
	}

	public virtual void TouchBegan(List<Touch> touches)
	{
	}

	public virtual void TouchMoved(List<Touch> touches)
	{
	}

	public virtual void TouchEnded(List<Touch> touches)
	{
	}

	public virtual void TouchCanceled(List<Touch> touches)
	{
	}
}
