using System;
using System.Collections;
using UnityEngine;

public class PowerUpCell : MonoBehaviour
{
	[SerializeField]
	private PackedSprite _icon;

	[SerializeField]
	private Transform _padlock;

	[SerializeField]
	private Transform _coinImage;

	[SerializeField]
	private Transform _checkedIcon;

	[SerializeField]
	private SpriteText _description;

	[SerializeField]
	private SpriteText _coins;

	[SerializeField]
	private UIRadioBtn _addButton;

	private string _name = string.Empty;

	private bool _awarded;

	private POWER_UP _type = POWER_UP.MULTIPLIER_BONUS;

	private bool _isTransitioning;

	private void OnEnable()
	{
		PowerUpManager.WillPlayGame += AwardPowerUp;
	}

	private void OnDisable()
	{
		PowerUpManager.WillPlayGame -= AwardPowerUp;
	}

	public void SetupForType(POWER_UP powerupType)
	{
		Reset();
		_type = powerupType;
		Unlock(GameData.PlayerStats.IsPowerUpUnlocked(powerupType));
		SetName();
		SetDescription();
		SetCost();
		SetIcon(powerupType, GameData.PlayerStats.PowerUpWithType(powerupType).GetRoughIndex());
	}

	private void Reset()
	{
		_addButton.SetState(0);
		HideTransForm(_checkedIcon, bHide: true);
		HideTransForm(_coins.transform, bHide: false);
		HideTransForm(_coinImage, bHide: false);
		_awarded = false;
	}

	private void SetIcon(POWER_UP type, int level)
	{
		_icon.SetFrame(type.ToString().ToLower(), level);
	}

	private void Unlock(bool bUnlocked)
	{
		_addButton.controlIsEnabled = bUnlocked;
		_icon.Color = new Color(1f, 1f, 1f, (!bUnlocked) ? 0.5f : 1f);
		HideTransForm(_padlock, bUnlocked);
		HideTransForm(_coins.transform, !bUnlocked);
		HideTransForm(_coinImage, !bUnlocked);
		HideTransForm(_description.transform, bUnlocked);
	}

	private void SetDescription()
	{
		POWER_UP type = _type;
		if (type == POWER_UP.BLAST_BONUS)
		{
			_description.Text = "Unlock all power ups";
			return;
		}
		_description.Text = string.Concat("Reach level ", Color.yellow, PowerUp.UnlockThresholdForType(_type).ToString(), Color.white, " to unlock");
	}

	private void SetName()
	{
		_name = _type.ToString().Replace("_", " ");
	}

	private void SetCost()
	{
		_coins.Text = string.Empty + PowerUp.CostForType(_type);
	}

	private void HideTransForm(Transform t, bool bHide)
	{
		t.localScale = ((!bHide) ? new Vector3(1f, 1f, 1f) : new Vector3(0f, 0f, 0f));
	}

	public void AddPowerUp()
	{
		if (GameData.PlayerStats.CoinsStock < PowerUp.CostForType(_type) && !_awarded)
		{
			PowerUpManager.Instance.AddCoinsView();
			return;
		}
		_awarded = !_awarded;
		if (_awarded)
		{
			PowerUpManager.Instance.UseCoins(PowerUp.CostForType(_type));
		}
		else
		{
			GameData.PlayerStats.AddCoins(PowerUp.CostForType(_type));
		}
		_addButton.controlIsEnabled = true;
		_addButton.SetState(_awarded ? 2 : 0);
		HideTransForm(_checkedIcon, !_awarded);
		HideTransForm(_coins.transform, _awarded);
		HideTransForm(_coinImage, _awarded);
	}

	private void AwardPowerUp()
	{
		if (_awarded)
		{
			GameData.PlayerStats.AwardPowerUp(_type);
			PlayerManager.SendFeedback("Buy " + _name, string.Empty, string.Empty);
		}
	}

	private void FlashButton()
	{
		iTween.ValueTo(base.gameObject, iTween.Hash("from", 1f, "to", 0f, "time", 0.3f, "oncomplete", "FlashAnimComplete", "onUpdate", (Action<object>)delegate(object x)
		{
			_addButton.SetColor(new Color(_addButton.Color.r, (float)x, (float)x, 1f));
			_coins.SetColor(new Color(_coins.Color.r, (float)x, (float)x, 1f));
		}));
	}

	private void FlashAnimComplete()
	{
		_addButton.Color = Color.white;
		_coins.Color = Color.white;
		PowerUpManager.Instance.AddCoinsView();
	}

	private IEnumerator ShowCoinsWindow()
	{
		_isTransitioning = true;
		yield return StartCoroutine(GameData.WaitSeconds(0.3f));
		_isTransitioning = false;
		PowerUpManager.Instance.AddCoinsView();
	}

	public void ShowInfo()
	{
		PowerUpManager.Instance.ShowPowerupInfo(_name, PowerUp.DescriptionForType(_type));
	}
}
