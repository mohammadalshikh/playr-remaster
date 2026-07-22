using UnityEngine;

public class XmasTree : MonoBehaviour
{
	public void Shake()
	{
		iTween.ShakePosition(base.gameObject, new Vector3(0f, 0f, 0.1f), 1f);
	}
}
