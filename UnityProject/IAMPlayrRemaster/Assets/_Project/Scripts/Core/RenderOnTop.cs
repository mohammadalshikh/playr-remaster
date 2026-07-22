using UnityEngine;

public class RenderOnTop : MonoBehaviour
{
	public int renderOrder = 3500;

	private void Start()
	{
		if (base.transform.GetComponent<Renderer>() != null)
		{
			base.transform.GetComponent<Renderer>().material.renderQueue = renderOrder;
		}
	}
}
