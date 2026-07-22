using UnityEngine;

public class MatchFitnessLocation : MapLocation
{
	[SerializeField]
	private MatchFitnessView matchfitnessView;

	[SerializeField]
	private MatchFitnessView matchFitnessViewExpaned;

	[SerializeField]
	private NewWebPlayerView newWebPlayerView;

	public override void Start()
	{
		base.Start();
		FacebookManager.sessionOpenedEvent += HandleConnectedToFB;
	}

	protected override void HandleButtonPressed(ref POINTER_INFO ptr)
	{
		if (ptr.evt == POINTER_INFO.INPUT_EVENT.TAP && !MonoSingleton<PanelManager>.Instance.IsPanelDisplaying && GameData.Instance.HasIntroSceneFinished)
		{
			base.HandleButtonPressed(ref ptr);
			if (!FacebookHandler.IsSessionValid() || Application.internetReachability == NetworkReachability.NotReachable)
			{
				MonoSingleton<PanelManager>.Instance.ToggleViews(newWebPlayerView);
			}
			else if (!FacebookHandler.IsSessionValid() && MonoSingleton<FacebookHandler>.Instance.HasLoggedIntoFBBefore)
			{
				MonoSingleton<FacebookHandler>.Instance.LoginFacebook();
			}
			else if (MonoSingleton<WebIntegrationManager>.IsInitialized && MonoSingleton<WebIntegrationManager>.Instance.IsWebUserDataReceived && MonoSingleton<WebIntegrationManager>.Instance.IsWebPlayer && FacebookHandler.IsSessionValid())
			{
				MonoSingleton<PanelManager>.Instance.ToggleViews(matchFitnessViewExpaned);
				matchFitnessViewExpaned.DisplayView(isVisible: true);
			}
			else if (Application.internetReachability == NetworkReachability.NotReachable)
			{
				MonoSingleton<PanelManager>.Instance.DisplayNotConenctedView();
			}
		}
	}

	protected override void HandleIntroSceneCompleted()
	{
		base.HandleIntroSceneCompleted();
		matchfitnessView.MatchFitnessAlert.GetComponent<Renderer>().enabled = true;
		if (!FacebookHandler.IsSessionValid())
		{
			matchfitnessView.DisplayView(isVisible: true);
		}
		else if (MonoSingleton<WebIntegrationManager>.IsInitialized && MonoSingleton<WebIntegrationManager>.Instance.IsWebUserDataReceived)
		{
			matchfitnessView.DisplayView(isVisible: true);
		}
	}

	private void HandleConnectedToFB()
	{
		if (newWebPlayerView.IsDisplaying)
		{
			newWebPlayerView.DisplayView(isVisible: false);
		}
	}

	public override void OnDisable()
	{
		base.OnDisable();
		FacebookManager.sessionOpenedEvent -= HandleConnectedToFB;
	}
}
