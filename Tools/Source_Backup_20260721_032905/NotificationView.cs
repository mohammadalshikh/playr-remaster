using UnityEngine;

public class NotificationView : View
{
	[SerializeField]
	protected SpriteText topBarTitle;

	[SerializeField]
	protected SpriteText headerTitle;

	[SerializeField]
	protected SpriteText bodyText;

	public void Init(string topBarTitle, string headerTitle, string bodyText)
	{
		if ((bool)this.topBarTitle)
		{
			this.topBarTitle.Text = topBarTitle;
		}
		if ((bool)this.headerTitle)
		{
			this.headerTitle.Text = headerTitle;
		}
		if ((bool)this.bodyText)
		{
			this.bodyText.Text = bodyText;
		}
	}
}
