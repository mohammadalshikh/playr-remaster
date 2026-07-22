using UnityEngine;

public class SwapPanels : MonoBehaviour
{
	[SerializeField]
	private UIPanel _bringInPanel;

	[SerializeField]
	private UIPanel _dismissPanel;

	public void SwapPanel()
	{
		if (_bringInPanel != null && _dismissPanel != null)
		{
			_dismissPanel.Dismiss();
			_bringInPanel.BringIn();
		}
		else
		{
			Debug.LogWarning("Swapping panels need to be assigned in the inspector for " + base.name);
		}
	}

	public void SwapPanel(UIPanel comeBackPanel)
	{
		_dismissPanel = comeBackPanel;
		SwapPanel();
	}
}
