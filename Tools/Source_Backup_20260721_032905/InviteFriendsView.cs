using UnityEngine;

public class InviteFriendsView : View
{
	[SerializeField]
	private UIButton textFriendButton;

	[SerializeField]
	private UIButton emailFriendButton;

	private void Start()
	{
		if ((bool)textFriendButton)
		{
			textFriendButton.AddInputDelegate(HandleTextFriendButtonPressed);
		}
		if ((bool)emailFriendButton)
		{
			emailFriendButton.AddInputDelegate(HandleEmailFriendButtonPressed);
		}
	}

	private void HandleTextFriendButtonPressed(ref POINTER_INFO ptr)
	{
		if (ptr.evt == POINTER_INFO.INPUT_EVENT.TAP && EtceteraAndroid.isSMSComposerAvailable())
		{
			EtceteraAndroid.showSMSComposer("I'm playing I AM PLAYR mobile - a FREE flick football game on mobiles and tablets! Try it here https://play.google.com/store/apps/details?id=com.werinteractive.playr and see if you've got what it takes to beat my high score!");
		}
	}

	private void HandleEmailFriendButtonPressed(ref POINTER_INFO ptr)
	{
		if (ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			Application.OpenURL("mailto:?subject=I AM PLAYR on Android&body= Hi, I'm playing I AM PLAYR mobile - a FREE flick football game on mobiles and tablets! \n\n - Shoot against targets in the training center, or head out to the pitch and practice passing.\n\n - Use precision flick mechanics and power ups to max out your scores!\n\n - Facebook connected - Log in to compete against your friends!\n\n - Linked to the I AM PLAYR web game, earn match fitness on the go!\n\n - Download it here: https://play.google.com/store/apps/details?id=com.werinteractive.playr and see if you've got what it takes to beat my high score!");
		}
	}
}
