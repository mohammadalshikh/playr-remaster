using UnityEngine;

public struct iPhoneTouch
{
	public int fingerId;

	public Vector2 position;

	public Vector2 positionDelta;

	public float timeDelta;

	public int tapCount;

	public iPhoneTouchPhase phase;
}
