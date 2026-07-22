using System.Collections;
using UnityEngine;

public class BoundingPost : MonoBehaviour
{
	public float blinkTime = 0.5f;

	private Color _initColor;

	public void Scale(bool scaleUp)
	{
		StartCoroutine(Scaling(scaleUp));
	}

	private IEnumerator Scaling(bool scaleUp)
	{
		while (GameData.IsPaused)
		{
			yield return false;
		}
		if (scaleUp)
		{
			base.GetComponent<Renderer>().enabled = true;
			base.transform.GetComponent<UIPanel>().StartTransition(UIPanelManager.SHOW_MODE.BringInForward);
		}
		else
		{
			base.transform.GetComponent<UIPanel>().StartTransition(UIPanelManager.SHOW_MODE.DismissForward);
			StartCoroutine(DestroyWithDelay(0.7f));
		}
	}

	private IEnumerator DestroyWithDelay(float delay)
	{
		yield return new WaitForSeconds(delay);
		base.GetComponent<Renderer>().enabled = false;
		if (base.gameObject != null)
		{
			Object.Destroy(base.gameObject);
		}
	}
}
