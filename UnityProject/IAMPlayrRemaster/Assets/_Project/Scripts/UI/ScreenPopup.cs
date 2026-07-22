using UnityEngine;

public class ScreenPopup : MonoBehaviour
{
	[SerializeField]
	private UIPanel _panel;

	[SerializeField]
	protected UIButton _shareButton;

	protected bool _isActive;

	protected UIPanel Panel => _panel;

	public virtual bool CanShow => false;

	public bool IsActive => _isActive;

	protected virtual void Init()
	{
	}

	protected virtual void Reset()
	{
	}

	public virtual void Bring()
	{
		_isActive = true;
		_panel.BringIn();
		Init();
	}

	public virtual void Dismiss()
	{
		_isActive = false;
		_panel.Dismiss();
		Reset();
	}

	public virtual void Share()
	{
		MinigameGUI.GreyOutButton(_shareButton, bGrayOut: true);
		if (Application.internetReachability == NetworkReachability.NotReachable)
		{
			GameData.GUIManager.ShowInfoView("INTERNET CONNECTION", "Doesn't look like you're connected to the internet. Please check your connection and try again.", null);
		}
		else if (FacebookHandler.IsSessionValid() && Facebook.instance != null)
		{
			Post();
		}
		else
		{
			GameData.GUIManager.ShowInfoView("FACEBOOK LOGIN", "Doesn't look like you're logged in to Facebook. Please log in and try again later.", null);
		}
	}

	protected virtual void DidSucceedPosting(string error, object result)
	{
		MinigameGUI.GreyOutButton(_shareButton, bGrayOut: false);
	}

	protected void DismissPanel()
	{
		_panel.Dismiss();
	}

	protected virtual void Post()
	{
	}
}
