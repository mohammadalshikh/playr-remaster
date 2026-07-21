using UnityEngine;

public class InfoView : MonoBehaviour
{
	[SerializeField]
	private UIPanel _infoPanel;

	[SerializeField]
	private SpriteText _title;

	[SerializeField]
	private SpriteText _message;

	[SerializeField]
	private UIButton _button;

	private UIPanel _bringPanel;

	public void SetAndShow(string title, string message, UIPanel bringPanel, string buttonText = "CLOSE")
	{
		_title.Text = title;
		_message.Text = message;
		_button.Text = buttonText;
		_bringPanel = bringPanel;
		Show();
	}

	public void Show()
	{
		if (_bringPanel != null)
		{
			_bringPanel.Dismiss();
		}
		_infoPanel.BringIn();
	}

	public void Dismiss()
	{
		_infoPanel.Dismiss();
		if (_bringPanel != null)
		{
			_bringPanel.BringIn();
		}
	}
}
