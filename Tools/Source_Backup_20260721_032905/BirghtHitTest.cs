using UnityEngine;

public class BirghtHitTest : MonoBehaviour
{
	public Brightness bright;

	private void Start()
	{
	}

	private void OnMouseDown()
	{
		bright.Bright();
	}

	private void Update()
	{
	}
}
