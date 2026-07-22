using UnityEngine;

public class AnimatingColorScript : MonoBehaviour
{
	public PackedSprite battery;

	public SpriteText minutes;

	public bool m_Animrunning;

	private void StartAnim()
	{
		m_Animrunning = true;
		iTween.FadeFrom(base.gameObject, iTween.Hash("alpha", 1f, "time", 1.5f, "color", new Color(1f, 0f, 0f, 1f), "looptype", iTween.LoopType.loop, "ignoretimescale", true));
		iTween.ScaleFrom(base.gameObject, iTween.Hash("scale", default(Vector3), "time", 1.5f, "looptype", iTween.LoopType.loop, "ignoretimescale", true));
	}

	private void StopAnim()
	{
		m_Animrunning = false;
	}
}
