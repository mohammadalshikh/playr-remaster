using UnityEngine;

public class NewWebPlayerView : CharacterNotificationView
{
	[SerializeField]
	private UIButton playrWebLink;

	[SerializeField]
	private UIButton connectToFBButton;

	private void Start()
	{
		Init("Welcome", string.Empty, "I AM PLAYR is also a web game that lets you play the life of a professional footballer on and off the pitch.");
		if ((bool)playrWebLink)
		{
			playrWebLink.AddInputDelegate(HandlePlayerWebLinkPressed);
		}
		if ((bool)connectToFBButton)
		{
			connectToFBButton.AddInputDelegate(HandleConnectToFBButtonPressed);
		}
	}

	private void HandlePlayerWebLinkPressed(ref POINTER_INFO ptr)
	{
		if (ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			Handheld.PlayFullScreenMovie("IAPTrailer.mp4", Color.black, FullScreenMovieControlMode.Full, FullScreenMovieScalingMode.AspectFit);
		}
	}

	private void HandleConnectToFBButtonPressed(ref POINTER_INFO ptr)
	{
		if (ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			MonoSingleton<FacebookHandler>.Instance.LoginFacebook();
		}
	}
}
