using System;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
	public Transform m_camera;

	public Transform m_motionLines;

	[SerializeField]
	private BlurEffect _cameraBlur;

	public Renderer m_loadingLogo;

	private Vector3 m_original_position;

	private float m_previousHeight;

	private bool m_isHeightLocked;

	public Transform CameraTransform => m_camera;

	private void Awake()
	{
		GameData.CheckInstance();
		GameData.GameCamera = this;
		m_original_position = m_camera.localPosition;
	}

	private void ResetRotation()
	{
	}

	public void ResetPosition()
	{
		m_camera.localPosition = m_original_position;
	}

	public void EnableRotation(bool bEnable)
	{
	}

	public void EnableMotionLines(bool bEnable)
	{
		if (bEnable)
		{
			m_motionLines.GetComponent<Renderer>().enabled = true;
			iTween.FadeTo(m_motionLines.gameObject, iTween.Hash("alpha", 0.5f, "time", 1f, "looptype", "none", "easetype", "linear", "ignoretimescale", true));
			return;
		}
		iTween.FadeTo(m_motionLines.gameObject, iTween.Hash("alpha", 0f, "time", 0.5f, "looptype", "none", "easetype", "linear", "ignoretimescale", true, "oncomplete", (Action<object>)delegate
		{
			m_motionLines.GetComponent<Renderer>().enabled = false;
		}));
	}

	public void BlurCamera()
	{
		if (!DeviceInfo.IsHighEnd)
		{
			return;
		}
		iTween.StopByName(base.gameObject, "blur");
		_cameraBlur.enabled = true;
		iTween.ValueTo(base.gameObject, iTween.Hash("name", "blur", "from", _cameraBlur.iterations, "to", 5, "time", 0.3f, "ignoretimescale", true, "onUpdate", (Action<object>)delegate(object x)
		{
			if (_cameraBlur.enabled)
			{
				_cameraBlur.iterations = Mathf.RoundToInt((float)x);
			}
		}));
	}

	public void RemoveBlur()
	{
		if (!_cameraBlur.enabled)
		{
			return;
		}
		iTween.StopByName(base.gameObject, "blur");
		iTween.ValueTo(base.gameObject, iTween.Hash("name", "blur", "from", _cameraBlur.iterations, "to", 0, "time", 0.3f, "ignoretimescale", true, "onComplete", "DisableBlurEffect", "onUpdate", (Action<object>)delegate(object x)
		{
			if (_cameraBlur.enabled)
			{
				_cameraBlur.iterations = Mathf.RoundToInt((float)x);
			}
		}));
	}

	private void DisableBlurEffect()
	{
		_cameraBlur.enabled = false;
	}

	public void LookAt(Vector3 pos)
	{
		GameData.Player.transform.LookAt(pos);
		m_camera.localRotation = Quaternion.identity;
		ResetRotation();
	}

	public void SmoothFollow(Transform target, float distanceFromTarget, float damping, bool xzOnly, Vector3 offset)
	{
		if (distanceFromTarget * distanceFromTarget < (target.position - m_camera.position).sqrMagnitude)
		{
			Vector3 position = Vector3.Lerp(m_camera.position, target.position + offset, 1f / 30f * damping);
			if (xzOnly)
			{
				position.y = m_camera.position.y;
			}
			m_camera.position = position;
		}
	}

	public void SmoothFollow(Transform target, float distanceFromTarget, float damping, bool xzOnly)
	{
		SmoothFollow(target, distanceFromTarget, damping, xzOnly, Vector3.zero);
	}

	public void LockHeight(bool bLock)
	{
		if (bLock)
		{
			if (m_isHeightLocked != bLock)
			{
				m_previousHeight = m_camera.position.y;
			}
			m_camera.position = new Vector3(m_camera.position.x, m_previousHeight, m_camera.position.z);
			m_isHeightLocked = true;
		}
		else
		{
			m_isHeightLocked = false;
		}
	}

	public void SmoothLookAt(Transform target, float damping)
	{
		if (!(target == null))
		{
			Quaternion to = Quaternion.LookRotation(target.position - m_camera.position);
			m_camera.rotation = Quaternion.Slerp(m_camera.rotation, to, Time.deltaTime * damping);
		}
	}

	public void SmoothLookAt(Vector3 targetPosition, float damping)
	{
		Quaternion to = Quaternion.LookRotation(targetPosition - m_camera.position);
		m_camera.rotation = Quaternion.Slerp(m_camera.rotation, to, Time.deltaTime * damping);
	}

	public void SetFieldOfView(float fieldOfView)
	{
		m_camera.GetComponent<Camera>().fieldOfView = fieldOfView;
	}

	public void SetXPosition(float x)
	{
		Vector3 position = new Vector3(x, m_camera.position.y, m_camera.position.z);
		m_camera.position = position;
	}

	public void SetYPosition(float y)
	{
		Vector3 position = new Vector3(m_camera.position.x, y, m_camera.position.z);
		m_camera.position = position;
	}

	public void EnableLoadingLogo(bool bEnabled)
	{
		m_loadingLogo.enabled = bEnabled;
	}
}
