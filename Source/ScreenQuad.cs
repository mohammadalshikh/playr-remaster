using UnityEngine;

public class ScreenQuad : MonoBehaviour
{
	[SerializeField]
	private GUITexture screenQuad;

	public Color QuadColor
	{
		get
		{
			return screenQuad.color;
		}
		set
		{
			screenQuad.color = value;
			if (screenQuad.color.a == 0f)
			{
				screenQuad.enabled = false;
			}
			else if (!screenQuad.enabled)
			{
				screenQuad.enabled = true;
			}
		}
	}
}
