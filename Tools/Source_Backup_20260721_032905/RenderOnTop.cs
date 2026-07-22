using UnityEngine;

public class RenderOnTop : MonoBehaviour
{
	public int renderOrder = 3500;

	private void Start()
	{
		if (base.transform.renderer != null)
		{
			base.transform.renderer.material.renderQueue = renderOrder;
		}
	}
}
