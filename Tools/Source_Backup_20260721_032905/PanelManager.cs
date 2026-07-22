using UnityEngine;

public class PanelManager : MonoSingleton<PanelManager>
{
	[SerializeField]
	private View[] views;

	[SerializeField]
	private NotificationView genericNotification;

	public bool IsPanelDisplaying
	{
		get
		{
			bool result = false;
			View[] array = views;
			foreach (View view in array)
			{
				if (view.IsDisplaying)
				{
					result = true;
				}
			}
			return result;
		}
	}

	private void Awake()
	{
		MonoSingleton<PanelManager>.Instance = this;
	}

	public void ToggleViews(View v)
	{
		if (!MonoSingleton<PanelManager>.IsInitialized)
		{
			return;
		}
		View[] array = views;
		foreach (View view in array)
		{
			if (view == v)
			{
				view.DisplayView(isVisible: true);
			}
			else if (view.IsDisplaying)
			{
				view.DisplayView(isVisible: false);
			}
		}
	}

	public void DisplayNotConenctedView()
	{
		DisplayGenericView("NO CONNECTION", string.Empty, "Doesn't look like you're connected to the internet. Please check your connection and try again.");
	}

	public void DisplayGenericView(string topBarTitle, string headerTitle, string bodyText)
	{
		genericNotification.Init(topBarTitle, headerTitle, bodyText);
		ToggleViews(genericNotification);
	}
}
