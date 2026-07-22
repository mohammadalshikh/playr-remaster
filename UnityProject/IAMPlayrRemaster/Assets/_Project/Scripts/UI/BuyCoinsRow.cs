using UnityEngine;

public class BuyCoinsRow : MonoBehaviour
{
	[SerializeField]
	private InAppPurchasing.BUNDLE _productToBuy;

	[SerializeField]
	private SpriteText _priceTag;

	[SerializeField]
	private SpriteText _numCoins;

	[SerializeField]
	private UIButton _buyButton;

	private void OnEnable()
	{
		GameData.CheckInstance();
		if (BuyController.Instance != null)
		{
			BuyController.Instance.ProductPricesUpdated += UpdatePrice;
		}
		GameData.PlayerStats.CoinsStockChanged += ResetState;
		GoogleIABManager.purchaseFailedEvent += FailedCancelledPurchase;
		GoogleIABManager.consumePurchaseFailedEvent += FailedCancelledPurchase;
	}

	private void OnDisable()
	{
		if (BuyController.Instance != null)
		{
			BuyController.Instance.ProductPricesUpdated -= UpdatePrice;
		}
		GameData.PlayerStats.CoinsStockChanged -= ResetState;
		GoogleIABManager.purchaseFailedEvent -= FailedCancelledPurchase;
		GoogleIABManager.consumePurchaseFailedEvent -= FailedCancelledPurchase;
	}

	private void Start()
	{
		Init();
	}

	private void UpdatePrice()
	{
		if (BuyController.Instance != null)
		{
			_priceTag.Text = BuyController.Instance.GetPriceForProduct(_productToBuy);
		}
		ResetState();
	}

	private void Init()
	{
		UpdatePrice();
		SetNumCoins();
	}

	private void SetNumCoins()
	{
		if (_numCoins != null)
		{
			_numCoins.Text = InAppPurchasing.GetValueForProduct(_productToBuy).ToString("#,##0");
		}
	}

	private void ResetState()
	{
		_buyButton.Text = "BUY";
	}

	private void FailedCancelledPurchase(string error)
	{
		ResetState();
	}

	public void BuyCoins()
	{
		if (MonoSingleton<InAppPurchasing>.IsInitialized && BuyController.Instance.Buy(MonoSingleton<InAppPurchasing>.Instance.GetIdentifierForProduct(_productToBuy)) && MonoSingleton<InAppPurchasing>.Instance.PurchasingEnabled())
		{
			_buyButton.Text = "BUYING";
		}
	}
}
