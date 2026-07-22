using UnityEngine;

public class CameraFaceY : MonoBehaviour
{
	private Quaternion m_original_rot;

	private void Start()
	{
		m_original_rot = base.transform.rotation;
	}

	private void LateUpdate()
	{
		Vector3 vector = Camera.mainCamera.transform.position + Camera.mainCamera.transform.forward;
		base.transform.LookAt(new Vector3(vector.x, base.transform.position.y, vector.z));
		base.transform.rotation *= m_original_rot;
	}
}
