using System.Collections.Generic;
using UnityEngine;

public class TouchTrail : MonoBehaviour, ICRTouchListener
{
	public Camera TargetCamera;

	private Transform cachedTransform;

	private void Start()
	{
		CR_TouchManager.AddListener(this);
		cachedTransform = base.transform;
	}

	public void OnTouchBegan(List<Touch> touches)
	{
	}

	public void OnTouchEnded(List<Touch> touches)
	{
	}

	public void OnTouchCanceled(List<Touch> touches)
	{
	}

	public void OnTouchMoved(List<Touch> touches)
	{
		MoveTrail(touches[0]);
	}

	private void MoveTrail(Touch touch)
	{
		Vector3 vector = TargetCamera.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, 0f));
		cachedTransform.position = new Vector3(vector.x, vector.y, cachedTransform.position.z);
	}
}
