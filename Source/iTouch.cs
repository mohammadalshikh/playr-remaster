using UnityEngine;

public class iTouch : MonoBehaviour
{
	private Vector2 pSpeed;

	private Vector3 pLastPos;

	public static void ResetTouch()
	{
		iPhoneInput.touches[0].timeDelta = Time.time;
		iPhoneInput.touches[0].phase = iPhoneTouchPhase.Canceled;
		iPhoneInput.touchCount = 0;
	}

	private void Awake()
	{
		iPhoneInput.touches = new iPhoneTouch[1];
		iPhoneInput.touches[0] = default(iPhoneTouch);
		iPhoneInput.touches[0].fingerId = 0;
		iPhoneInput.touches[0].position = Input.mousePosition;
		iPhoneInput.touches[0].positionDelta = new Vector2(0f, 0f);
		iPhoneInput.touches[0].timeDelta = Time.time;
		iPhoneInput.touches[0].tapCount = 0;
		iPhoneInput.touches[0].phase = iPhoneTouchPhase.Canceled;
		iPhoneInput.touchCount = 0;
		iPhoneInput.multiTouchEnabled = false;
		iPhoneInput.orientation = iPhoneOrientation.LandscapeLeft;
	}

	private void Update()
	{
		Vector2 vector = Input.mousePosition;
		switch (iPhoneInput.touches[0].phase)
		{
		case iPhoneTouchPhase.Canceled:
			if (Input.GetMouseButtonDown(0))
			{
				iPhoneInput.touches[0].fingerId = 0;
				iPhoneInput.touches[0].position = vector;
				iPhoneInput.touches[0].positionDelta = new Vector2(0f, 0f);
				iPhoneInput.touches[0].timeDelta = Time.time;
				iPhoneInput.touches[0].tapCount = 0;
				iPhoneInput.touches[0].phase = iPhoneTouchPhase.Began;
				iPhoneInput.touchCount = 1;
			}
			break;
		case iPhoneTouchPhase.Began:
			iPhoneInput.touches[0].timeDelta = Time.time;
			iPhoneInput.touches[0].phase = iPhoneTouchPhase.Stationary;
			break;
		case iPhoneTouchPhase.Stationary:
			if (iPhoneInput.touches[0].position != vector)
			{
				iPhoneInput.touches[0].positionDelta.x = vector.x - iPhoneInput.touches[0].position.x;
				iPhoneInput.touches[0].positionDelta.y = vector.y - iPhoneInput.touches[0].position.y;
				iPhoneInput.touches[0].position = vector;
				iPhoneInput.touches[0].timeDelta = Time.time;
				iPhoneInput.touches[0].phase = iPhoneTouchPhase.Moved;
			}
			if (Input.GetMouseButtonUp(0))
			{
				iPhoneInput.touches[0].timeDelta = Time.time;
				iPhoneInput.touches[0].phase = iPhoneTouchPhase.Ended;
			}
			break;
		case iPhoneTouchPhase.Moved:
			if (iPhoneInput.touches[0].position != vector)
			{
				iPhoneInput.touches[0].positionDelta.x = vector.x - iPhoneInput.touches[0].position.x;
				iPhoneInput.touches[0].positionDelta.y = vector.y - iPhoneInput.touches[0].position.y;
				iPhoneInput.touches[0].position = vector;
				iPhoneInput.touches[0].timeDelta = Time.time;
			}
			else
			{
				iPhoneInput.touches[0].positionDelta = new Vector2(0f, 0f);
				iPhoneInput.touches[0].timeDelta = Time.time;
				iPhoneInput.touches[0].phase = iPhoneTouchPhase.Stationary;
			}
			if (Input.GetMouseButtonUp(0))
			{
				iPhoneInput.touches[0].timeDelta = Time.time;
				iPhoneInput.touches[0].phase = iPhoneTouchPhase.Ended;
			}
			break;
		case iPhoneTouchPhase.Ended:
			iPhoneInput.touches[0].timeDelta = Time.time;
			iPhoneInput.touches[0].phase = iPhoneTouchPhase.Canceled;
			iPhoneInput.touchCount = 0;
			break;
		}
	}
}
