using UnityEngine;

public class ScreenSprite : MonoBehaviour
{
	public enum ScreenMode
	{
		MaxX,
		MaxY,
		FullScreen
	}

	[SerializeField]
	private PackedSprite screenSprite;

	[SerializeField]
	private ScreenMode currentScreenMode;

	protected void Awake()
	{
		if ((bool)screenSprite)
		{
			switch (currentScreenMode)
			{
			case ScreenMode.MaxX:
				screenSprite.SetSize(Screen.width, screenSprite.height);
				break;
			case ScreenMode.MaxY:
				screenSprite.SetSize(screenSprite.width, Screen.height);
				break;
			case ScreenMode.FullScreen:
				screenSprite.SetSize(Screen.width, Screen.height);
				break;
			}
		}
	}
}
