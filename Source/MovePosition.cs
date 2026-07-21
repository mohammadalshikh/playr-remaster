using System.Collections;
using UnityEngine;

public class MovePosition : MonoBehaviour
{
	private const string m_MoveName = "MovePosition";

	public Vector3 m_DeltaPosition = Vector3.zero;

	public float m_MoveTime = 0.5f;

	public float m_destroyAfter;

	public bool m_MoveAtStart = true;

	public iTween.LoopType m_LoopType = iTween.LoopType.pingPong;

	public iTween.EaseType m_EaseType = iTween.EaseType.linear;

	public void StartMoving()
	{
		iTween.MoveBy(base.gameObject, iTween.Hash("id", "MovePosition" + base.gameObject.name, "time", m_MoveTime, "looptype", m_LoopType, "amount", m_DeltaPosition, "easetype", m_EaseType));
		if (m_destroyAfter > 0f)
		{
			StartCoroutine(DestroyWithDelay(m_MoveTime + m_destroyAfter));
		}
	}

	public void StopMoving()
	{
		iTween.StopByName(base.gameObject, "MovePosition" + base.gameObject.name, includechildren: true);
	}

	private void Start()
	{
		if (m_MoveAtStart)
		{
			StartMoving();
		}
	}

	private IEnumerator DestroyWithDelay(float delay)
	{
		yield return new WaitForSeconds(delay);
		Object.Destroy(base.gameObject);
	}
}
