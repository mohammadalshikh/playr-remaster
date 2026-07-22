using UnityEngine;

public class NavBarManager : View
{
	[SerializeField]
	private UIButton settingsButton;

	[SerializeField]
	private UIButton helpButton;

	[SerializeField]
	private UIButton addCoinsButton;

	[SerializeField]
	private UIButton inviteFriendsButton;

	[SerializeField]
	private UIButton playerStatsButton;

	[SerializeField]
	private SettingsView settingsView;

	[SerializeField]
	private AppSupportView appSupportView;

	[SerializeField]
	private BuyCoinsView buyCoinsView;

	[SerializeField]
	private InviteFriendsView inviteFriendsView;

	[SerializeField]
	private PlayerStatsView playerStatsView;

	private void Start()
	{
		if (!GameData.Instance.HasIntroSeqOccured)
		{
			DisplayView(isVisible: false);
		}
		if (!GameData.Instance.HasIntroSeqOccured)
		{
			IntroView.IntroSceneCompleted += HandleIntroSceneCompleted;
		}
		if ((bool)settingsButton)
		{
			settingsButton.AddInputDelegate(HandleSettingsButtonPressed);
		}
		if ((bool)helpButton)
		{
			helpButton.AddInputDelegate(HandleHelpButtonPressed);
		}
		if ((bool)addCoinsButton)
		{
			addCoinsButton.AddInputDelegate(HandleAddCoinsButtonPressed);
		}
		if ((bool)inviteFriendsButton)
		{
			inviteFriendsButton.AddInputDelegate(HandleInviteFriendsButtonPressed);
		}
		if ((bool)playerStatsButton)
		{
			playerStatsButton.AddInputDelegate(HandlePlayersStatsButtonPressed);
		}
	}

	private void HandleSettingsButtonPressed(ref POINTER_INFO ptr)
	{
		if (ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			MonoSingleton<PanelManager>.Instance.ToggleViews(settingsView);
		}
	}

	private void HandleFacebookPressed(ref POINTER_INFO ptr)
	{
		if (ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			if (Application.internetReachability == NetworkReachability.NotReachable)
			{
				MonoSingleton<PanelManager>.Instance.DisplayNotConenctedView();
			}
			else if (MonoSingleton<FacebookHandler>.IsInitialized)
			{
				MonoSingleton<FacebookHandler>.Instance.LoginFacebook();
			}
		}
	}

	private void HandleHelpButtonPressed(ref POINTER_INFO ptr)
	{
		if (ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			MonoSingleton<PanelManager>.Instance.ToggleViews(appSupportView);
			PlayerManager.SendFeedback("HelpButtonPressed", string.Empty, string.Empty);
		}
	}

	private void HandleAddCoinsButtonPressed(ref POINTER_INFO ptr)
	{
		if (ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			MonoSingleton<PanelManager>.Instance.ToggleViews(buyCoinsView);
		}
	}

	private void HandleInviteFriendsButtonPressed(ref POINTER_INFO ptr)
	{
		if (ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			MonoSingleton<PanelManager>.Instance.ToggleViews(inviteFriendsView);
		}
	}

	private void HandlePlayersStatsButtonPressed(ref POINTER_INFO ptr)
	{
		if (ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			MonoSingleton<PanelManager>.Instance.ToggleViews(playerStatsView);
		}
	}

	private void HandleIntroSceneCompleted()
	{
		DisplayView(isVisible: true);
		IntroView.IntroSceneCompleted -= HandleIntroSceneCompleted;
	}
}
