using UnityEngine;

public class FaceCamera : MonoBehaviour
{
	public Transform lookAtObject;

	public bool faceBack;

	private Quaternion m_originalRot;

	private void Start()
	{
		if (lookAtObject == null)
		{
			lookAtObject = Camera.main.transform;
		}
		m_originalRot = base.transform.rotation;
	}

	private void LateUpdate()
	{
		Vector3 vector = lookAtObject.position - lookAtObject.forward;
		base.transform.LookAt(new Vector3(vector.x, base.transform.position.y, vector.z));
		base.transform.rotation *= m_originalRot;
		if (faceBack)
		{
			base.transform.rotation *= Quaternion.AngleAxis(180f, Vector3.up);
		}
	}
}
