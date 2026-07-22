using System.Collections.Generic;
using UnityEngine;

public class ShieldController : MonoBehaviour
{
	[SerializeField]
	private UIPanel _controllerPanel;

	[SerializeField]
	private List<ActiveSprite> _shieldSprites = new List<ActiveSprite>();

	private int _shieldsActive;

	private void OnEnable()
	{
		MinigameChallenge.ShieldHasChanged += ShieldChanged;
		MinigameGUI.MakeGUIEnabled += ShowPanel;
		PlayAgainHandler.PlayAgainFired += Empty;
	}

	private void OnDisable()
	{
		MinigameChallenge.ShieldHasChanged -= ShieldChanged;
		MinigameGUI.MakeGUIEnabled -= ShowPanel;
		PlayAgainHandler.PlayAgainFired -= Empty;
	}

	private void ShieldChanged(int currentShield)
	{
		if (currentShield == 0)
		{
			Empty();
			return;
		}
		currentShield = Mathf.Clamp(currentShield, 0, _shieldSprites.Count);
		if (currentShield > _shieldsActive)
		{
			for (int i = _shieldsActive; i <= currentShield - 1; i++)
			{
				ActivateShield(i, bActive: true);
			}
		}
		else
		{
			ActivateShield(currentShield, bActive: false);
		}
		_shieldsActive = currentShield;
	}

	private void ActivateShield(int index, bool bActive)
	{
		if (bActive)
		{
			_shieldSprites[index].Activate("full");
		}
		else
		{
			_shieldSprites[index].Deactivate("empty");
		}
	}

	private void ShowPanel(bool bShow)
	{
		if (bShow)
		{
			_controllerPanel.BringIn();
		}
		else
		{
			_controllerPanel.Dismiss();
		}
	}

	private void Empty()
	{
		foreach (ActiveSprite shieldSprite in _shieldSprites)
		{
			if (shieldSprite.IsActive)
			{
				shieldSprite.Deactivate("empty");
			}
		}
		_shieldsActive = 0;
	}
}
