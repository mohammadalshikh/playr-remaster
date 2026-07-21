using UnityEngine;

public class SuperControlTrigger : MonoBehaviour
{
	[SerializeField]
	private UIButton _controlButton;

	[SerializeField]
	private UIPanel _controlPannel;

	private bool _enabled;

	private void OnEnable()
	{
		MinigameGUI.MakeGUIEnabled += Show;
		Minigame.MinigameFinished += Dismiss;
		Minigame.SuperControlChangedEvent += HandleSuperControlChanged;
	}

	private void OnDisable()
	{
		MinigameGUI.MakeGUIEnabled -= Show;
		Minigame.MinigameFinished -= Dismiss;
		Minigame.SuperControlChangedEvent -= HandleSuperControlChanged;
	}

	public void SuperControl()
	{
		_enabled = !_enabled;
		UpdateButton();
		Minigame.SuperControlRate((!_enabled) ? 1f : 0.5f);
	}

	public void Show(bool bEnable)
	{
		if (bEnable && GameData.MinigameObject.CurrentMinigame.IsSuperControlAwarded)
		{
			_controlPannel.BringIn();
		}
		else
		{
			_controlPannel.Dismiss();
		}
	}

	public void Dismiss()
	{
		_controlPannel.Dismiss();
	}

	private void HandleSuperControlChanged(float newRate)
	{
		_enabled = newRate != 1f;
		UpdateButton();
	}

	private void ChangeText()
	{
		_controlButton.Text = string.Empty + ((!_enabled) ? "NORMAL" : "SUPER") + " Control";
	}

	private void ChangeColor()
	{
		_controlButton.SetColor(_enabled ? Color.green : Color.blue);
	}

	private void UpdateButton()
	{
		ChangeText();
		ChangeColor();
	}
}
