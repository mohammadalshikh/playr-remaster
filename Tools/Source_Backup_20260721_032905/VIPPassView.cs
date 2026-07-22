using UnityEngine;

public class VIPPassView : CharacterNotificationView
{
	[SerializeField]
	private GenericButton purchaseVIPPassButton;

	[SerializeField]
	private ParticleSystem sparkles;

	private void Start()
	{
		Init("VIP CLUB", "What do you get?", "- All levels unlocked\n- All power ups unlocked\n- 1000 coins", CharacterId.Manager);
		if ((bool)purchaseVIPPassButton)
		{
			purchaseVIPPassButton.AddInputDelegate(HandlePurchaseVIPPassButtonPressed);
		}
	}

	private void HandlePurchaseVIPPassButtonPressed(ref POINTER_INFO ptr)
	{
		if (ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			if (!PlayerPrefs.HasKey("VIPPass"))
			{
				MonoSingleton<InAppPurchasing>.Instance.PurchaseProduct("vippass");
			}
			else if (Application.internetReachability != 0)
			{
				MonoSingleton<PanelManager>.Instance.DisplayGenericView("ALREADY VIP MEMBER", "You're already a member of the VIP Club", string.Empty);
			}
			else
			{
				MonoSingleton<PanelManager>.Instance.DisplayNotConenctedView();
			}
		}
	}

	public void ToggleParticles(bool isActive)
	{
		sparkles.enableEmission = isActive;
		if (isActive)
		{
			sparkles.Play();
		}
		else
		{
			sparkles.Stop();
		}
	}

	public override void HandleCloseButtonPressed(ref POINTER_INFO ptr)
	{
		base.HandleCloseButtonPressed(ref ptr);
		ToggleParticles(isActive: false);
	}
}
