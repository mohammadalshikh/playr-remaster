using UnityEngine;

[AddComponentMenu("Effects/SetRenderQueue")]
[RequireComponent(typeof(Renderer))]
public class SetRenderQueue : MonoBehaviour
{
	protected void Start()
	{
		Renderer[] componentsInChildren = GetComponentsInChildren<Renderer>();
		int num = 1;
		Renderer[] array = componentsInChildren;
		foreach (Renderer renderer in array)
		{
			renderer.material.renderQueue = num;
			num++;
		}
	}
}
