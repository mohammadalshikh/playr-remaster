using System.Collections;
using UnityEngine;

public class Snowman : MonoBehaviour
{
	public PackedSprite m_rightEye;

	public PackedSprite m_leftEye;

	public void EyesAnimation()
	{
		StartCoroutine(FlashEyes());
	}

	private IEnumerator FlashEyes()
	{
		OpenCloseEyes(open: true);
		yield return new WaitForSeconds(0.4f);
		OpenCloseEyes(open: false);
		yield return new WaitForSeconds(0.2f);
		OpenCloseEyes(open: true);
		yield return new WaitForSeconds(0.4f);
		OpenCloseEyes(open: false);
	}

	private void OpenCloseEyes(bool open)
	{
		m_rightEye.Hide(!open);
		m_leftEye.Hide(!open);
	}
}
