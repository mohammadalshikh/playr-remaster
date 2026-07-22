using UnityEngine;

public class PlayAgainCoins : MonoBehaviour
{
	[SerializeField]
	private SpriteText _coinsValue;

	[SerializeField]
	private PackedSprite _coin;

	private void Start()
	{
		if (PlayerPrefs.GetInt(GameData.SelectedGameLocation.ToString() + GameData.SelectedGameType.ToString() + "IsNewlyUnlocked") == 0 && !GameData.Instance.IsCurrentApptDue)
		{
			SetCoins(GlobalSettings.GetCurrentGame().CoinsToSkipAppt);
			_coin.GetComponent<Renderer>().enabled = true;
		}
		else
		{
			_coinsValue.Text = string.Empty;
			_coin.GetComponent<Renderer>().enabled = false;
		}
	}

	public void SetCoins(int coinsValue)
	{
		if (_coinsValue != null)
		{
			_coinsValue.Text = coinsValue.ToString();
		}
	}
}
