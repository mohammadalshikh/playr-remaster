using UnityEngine;

public class GlowItems : MonoBehaviour
{
	public float m_Amount = 0.5f;

	public float m_Time = 0.5f;

	public iTween.LoopType m_LoopType = iTween.LoopType.pingPong;

	public iTween.EaseType m_EaseType = iTween.EaseType.easeInOutBounce;

	public Color m_Color = Color.white;

	public bool m_Fade = true;

	public bool m_ColorOn;

	public void StopAll()
	{
		base.gameObject.GetComponent<Renderer>().material.color = Color.white;
		iTween.Stop(base.gameObject);
	}

	public void Hide()
	{
		base.transform.GetComponent<Renderer>().enabled = false;
	}

	public void Show()
	{
		base.transform.GetComponent<Renderer>().enabled = true;
	}

	private void Start()
	{
		if (m_Fade)
		{
			iTween.FadeTo(base.gameObject, iTween.Hash("name", base.transform.name + "Fade", "alpha", 1f, "time", m_Time, "amount", m_Amount, "looptype", m_LoopType, "easetype", m_EaseType, "ignoretimescale", true));
		}
		if (m_ColorOn)
		{
			iTween.ColorTo(base.gameObject, iTween.Hash("name", base.transform.name + "Fade", "color", m_Color, "time", m_Time, "looptype", m_LoopType, "easetype", m_EaseType, "ignoretimescale", true));
		}
	}
}
