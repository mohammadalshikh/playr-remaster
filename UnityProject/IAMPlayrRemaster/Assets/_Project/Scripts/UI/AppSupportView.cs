using UnityEngine;

public class AppSupportView : View
{
	private const string faqLink = "http://support.werinteractive.com/categories/20066706-PLAYR-MOBILE";

	[SerializeField]
	private UIButton faqLinkButton;

	[SerializeField]
	private UIButton emailLinkButton;

	[SerializeField]
	private SpriteText emailButtonText;

	[SerializeField]
	private SpriteText faqButtonText;

	private void Start()
	{
		if ((bool)faqLinkButton)
		{
			faqLinkButton.AddInputDelegate(HandleFAQLinkButtonPressed);
		}
		if ((bool)emailLinkButton)
		{
			emailLinkButton.AddInputDelegate(HandleEmailLinkPressed);
		}
		if ((bool)emailButtonText)
		{
			emailButtonText.Text = MonoSingleton<LocalisationManager>.Instance.GetLocalisedText("EmailButton");
		}
		if ((bool)faqButtonText)
		{
			faqButtonText.Text = MonoSingleton<LocalisationManager>.Instance.GetLocalisedText("FAQButton");
		}
	}

	private void HandleFAQLinkButtonPressed(ref POINTER_INFO ptr)
	{
		if (ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			Application.OpenURL("http://support.werinteractive.com/categories/20066706-PLAYR-MOBILE");
		}
	}

	private void HandleEmailLinkPressed(ref POINTER_INFO ptr)
	{
		if (ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			Application.OpenURL("mailto:sphelp@werinteractive.com?subject=Support&body=");
		}
	}
}
