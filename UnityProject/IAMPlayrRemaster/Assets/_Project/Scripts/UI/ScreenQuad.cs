using UnityEngine;

/// <summary>
/// Screen quad for fullscreen effects (fade, overlays)
/// Modernized from GUITexture (obsolete) to SpriteRenderer
/// </summary>
public class ScreenQuad : MonoBehaviour
{
	[SerializeField]
	private SpriteRenderer screenQuad;
	
	private Color _color = Color.white;

	public Color QuadColor
	{
		get
		{
			if (screenQuad != null)
				return screenQuad.color;
			return _color;
		}
		set
		{
			_color = value;
			if (screenQuad != null)
			{
				screenQuad.color = value;
				screenQuad.enabled = (screenQuad.color.a > 0f);
			}
		}
	}
}
