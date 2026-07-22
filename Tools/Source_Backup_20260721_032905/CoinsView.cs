using UnityEngine;

public class CoinsView : MonoBehaviour
{
	[SerializeField]
	private SpriteText _coinsText;

	private int _currentValue;

	private void Start()
	{
		GameData.PlayerStats.CoinsStockChanged += CoinsChanged;
		CoinsChanged();
	}

	private void OnDisable()
	{
		GameData.PlayerStats.CoinsStockChanged -= CoinsChanged;
	}

	private void CoinsChanged()
	{
		MinigameGUI.CountUpText(_coinsText, _currentValue, GameData.PlayerStats.CoinsStock, null, 5f);
		_currentValue = GameData.PlayerStats.CoinsStock;
	}
}
