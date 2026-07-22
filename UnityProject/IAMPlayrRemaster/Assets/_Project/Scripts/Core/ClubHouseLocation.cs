using UnityEngine;

public class ClubHouseLocation : MapLocation
{
	public override void Start()
	{
		base.Start();
		if (Application.internetReachability != 0 && !FacebookHandler.IsSessionValid())
		{
			HandleFacebookLoggedOut();
		}
		else
		{
			HandleFacebookSesssionOpened();
		}
		FacebookManager.sessionOpenedEvent += HandleFacebookSesssionOpened;
		FacebookHandler.FacebookLoggedOut += HandleFacebookLoggedOut;
	}

	protected override void HandleButtonPressed(ref POINTER_INFO ptr)
	{
		if (ptr.evt == POINTER_INFO.INPUT_EVENT.TAP && GameData.Instance.HasIntroSceneFinished)
		{
			if (Application.internetReachability == NetworkReachability.NotReachable)
			{
				MonoSingleton<PanelManager>.Instance.DisplayGenericView("Not Connected", string.Empty, "You're not connected to the internet. Connect to access the leaderboard.");
			}
			else if (Application.internetReachability != 0 && !FacebookHandler.IsSessionValid())
			{
				MonoSingleton<FacebookHandler>.Instance.LoginFacebook();
			}
			else
			{
				base.HandleButtonPressed(ref ptr);
			}
		}
	}

	private void HandleFacebookSesssionOpened()
	{
		alertText.Text = "Beat your friends!";
	}

	private void HandleFacebookLoggedOut()
	{
		if ((bool)alertText)
		{
			alertText.Text = "Connect to Facebook";
		}
	}

	public override void OnDisable()
	{
		base.OnDisable();
		FacebookManager.sessionOpenedEvent -= HandleFacebookSesssionOpened;
		FacebookHandler.FacebookLoggedOut -= HandleFacebookLoggedOut;
	}
}
