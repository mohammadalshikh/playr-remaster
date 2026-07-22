using System.Collections.Generic;
using UnityEngine;

public class PopupViewController : MonoBehaviour
{
	[SerializeField]
	protected UIPanel _popupPanel;

	[SerializeField]
	protected SpriteText _title;

	[SerializeField]
	protected SpriteText _message;

	protected bool _isReady = true;

	protected Stack<PopupView> _currentPopups = new Stack<PopupView>();

	protected virtual void AddMessageToQueue(PopupView popupView)
	{
		_currentPopups.Push(popupView);
	}

	protected virtual void ShowPopup(PopupView popupView)
	{
		_isReady = false;
		_title.Text = popupView.Title;
	}

	protected virtual void ProcessPopup(PopupView popView)
	{
	}

	public virtual void CreatePopup(string title, string message)
	{
	}
}
