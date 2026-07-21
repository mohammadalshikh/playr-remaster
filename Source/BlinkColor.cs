using UnityEngine;

public class BlinkColor : MonoBehaviour
{
	private const string m_BlinkColor = "BlinkColor";

	public Color m_Color = Color.green;

	public float m_BlinkTime = 0.5f;

	public iTween.LoopType m_LoopType = iTween.LoopType.pingPong;

	public iTween.EaseType m_EaseType = iTween.EaseType.linear;

	private void Start()
	{
		StartBlinking();
	}

	public void StartBlinking()
	{
		iTween.ColorTo(base.gameObject, iTween.Hash("id", "BlinkColor" + base.gameObject.name, "time", m_BlinkTime, "looptype", m_LoopType, "color", m_Color, "easetype", m_EaseType));
	}

	public void StopBlinking()
	{
		iTween.StopByName(base.gameObject, "BlinkColor" + base.gameObject.name, includechildren: true);
	}
}
