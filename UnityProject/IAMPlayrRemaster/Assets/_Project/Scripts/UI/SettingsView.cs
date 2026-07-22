using UnityEngine;

public class SettingsView : View
{
	[SerializeField]
	private UISlider musicSlider;

	[SerializeField]
	private UISlider soundFXSlider;

	[SerializeField]
	private SpriteText facebookConnectionStatus;

	[SerializeField]
	private PackedSprite facebookStatusIcon;

	[SerializeField]
	private Color fbStatusIconConnectedColor;

	[SerializeField]
	private AudioSource playrThemeMusic;

	[SerializeField]
	private UIButton notificationToggle;

	[SerializeField]
	private UIButton fbLoginButton;

	[SerializeField]
	private UIButton fbLogoutButton;

	[SerializeField]
	private UIButton restorePurchasesButton;

	private void Start()
	{
		musicSlider.AddValueChangedDelegate(HandleSliderChanged);
		if (FacebookHandler.IsSessionValid())
		{
			HandleConnectedToFB();
		}
		else
		{
			HandleFacebookLoggedOut();
		}
		FacebookManager.sessionOpenedEvent += HandleConnectedToFB;
		FacebookHandler.FacebookLoggedOut += HandleFacebookLoggedOut;
		SetNotifactionLabel();
		if ((bool)notificationToggle)
		{
			notificationToggle.AddInputDelegate(HandleNotificationTogglePressed);
		}
		if ((bool)fbLoginButton)
		{
			fbLoginButton.AddInputDelegate(HandleFBLoginButtonPressed);
		}
		if ((bool)fbLogoutButton)
		{
			fbLogoutButton.AddInputDelegate(HandleFBLogoutButtonPressed);
		}
		if ((bool)restorePurchasesButton)
		{
			restorePurchasesButton.AddInputDelegate(HandleRestorePurchasesButton);
		}
	}

	private void HandleNotificationTogglePressed(ref POINTER_INFO ptr)
	{
		if (ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			LocalNotifications.Enable(!LocalNotifications.NotificationsEnabled);
			SetNotifactionLabel();
		}
	}

	private void HandleFBLoginButtonPressed(ref POINTER_INFO ptr)
	{
		if (ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			MonoSingleton<FacebookHandler>.Instance.LoginFacebook();
		}
	}

	private void HandleFBLogoutButtonPressed(ref POINTER_INFO ptr)
	{
		if (ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			MonoSingleton<FacebookHandler>.Instance.LogoutFacebook();
		}
	}

	private void HandleRestorePurchasesButton(ref POINTER_INFO ptr)
	{
		if (ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			MonoSingleton<InAppPurchasing>.Instance.RestorePurchases();
		}
	}

	private void HandleSliderChanged(IUIObject obj)
	{
		if ((UISlider)obj == musicSlider)
		{
			if ((bool)playrThemeMusic)
			{
				playrThemeMusic.volume = musicSlider.Value;
			}
			GameData.PlayerStats.SetMusicGlobalVolume(musicSlider.Value);
		}
		if ((UISlider)obj == soundFXSlider)
		{
			GameData.PlayerStats.SetSoundFXGlobalVolume(soundFXSlider.Value);
		}
	}

	public override void DisplayView(bool isVisible)
	{
		base.DisplayView(isVisible);
		if (musicSlider.GetKnob() != null)
		{
			musicSlider.GetKnob().GetComponent<Renderer>().enabled = isVisible;
		}
		if (soundFXSlider.GetKnob() != null)
		{
			soundFXSlider.GetKnob().GetComponent<Renderer>().enabled = isVisible;
		}
		if ((bool)fbLogoutButton && !FacebookHandler.IsSessionValid())
		{
			fbLogoutButton.GetComponent<Renderer>().enabled = false;
		}
	}

	private void HandleConnectedToFB()
	{
		facebookConnectionStatus.Text = "Connected to Facebook";
		facebookStatusIcon.SetColor(Color.green);
		fbLoginButton.GetComponent<Renderer>().enabled = false;
		fbLoginButton.GetComponent<Collider>().enabled = false;
		fbLogoutButton.GetComponent<Renderer>().enabled = true;
		fbLogoutButton.GetComponent<Collider>().enabled = true;
	}

	private void HandleFacebookLoggedOut()
	{
		facebookConnectionStatus.Text = "Not Connected to Facebook";
		facebookStatusIcon.SetColor(Color.red);
		fbLoginButton.GetComponent<Renderer>().enabled = true;
		fbLoginButton.GetComponent<Collider>().enabled = true;
		fbLogoutButton.GetComponent<Renderer>().enabled = false;
		fbLogoutButton.GetComponent<Collider>().enabled = false;
	}

	private void OnDisable()
	{
		FacebookManager.sessionOpenedEvent -= HandleConnectedToFB;
		FacebookHandler.FacebookLoggedOut -= HandleFacebookLoggedOut;
	}

	private void SetNotifactionLabel()
	{
		if (!LocalNotifications.NotificationsEnabled)
		{
			notificationToggle.SetColor(Color.red);
			notificationToggle.Text = "Notifications Off";
		}
		else
		{
			notificationToggle.SetColor(Color.white);
			notificationToggle.Text = "Notifications On";
		}
	}
}
