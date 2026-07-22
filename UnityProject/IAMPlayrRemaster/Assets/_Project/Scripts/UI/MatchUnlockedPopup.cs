using UnityEngine;

public class MatchUnlockedPopup : ScreenPopup
{
	[SerializeField]
	private SpriteText _message;

	[SerializeField]
	private UIButton learnMoreButton;

	public override bool CanShow
	{
		get
		{
			if (!PlayerPrefs.HasKey("MatchUnlockedPopupShown") && MonoSingleton<WebIntegrationManager>.IsInitialized)
			{
				PlayerPrefs.SetInt("MatchUnlockedPopupShown", 1);
				return MonoSingleton<WebIntegrationManager>.Instance.IsMatchUnlocked;
			}
			return false;
		}
	}

	private void Start()
	{
		learnMoreButton.AddInputDelegate(HandleLearnMoreButtonInput);
	}

	protected override void Init()
	{
		_message.Text = "Go to I AM PLAYR on your desktop or laptop to play River Park vs " + MonoSingleton<WebIntegrationManager>.Instance.OpponentName + "\n\nI AM PLAYR on web lets you play the life of a professional footballer. Live the dream!";
	}

	public void HandleLearnMoreButtonInput(ref POINTER_INFO ptr)
	{
		if (ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			if (Application.internetReachability == NetworkReachability.NotReachable)
			{
				GameData.GUIManager.ShowInfoView("INTERNET CONNECTION", "Doesn't look like you're connected to the internet. Please check your connection and try again.", null);
			}
			else
			{
				Application.OpenURL("https://www.facebook.com/iamplayr");
			}
		}
	}
}
