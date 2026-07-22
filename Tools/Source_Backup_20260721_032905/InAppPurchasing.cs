using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class InAppPurchasing : MonoSingleton<InAppPurchasing>
{
	public enum BUNDLE
	{
		PACK_1,
		PACK_2,
		PACK_3,
		PACK_4,
		PACK_5
	}

	public delegate void TransactionSuccessful(int coinsBought);

	public delegate void TransactionFailed();

	public delegate void HandlePricesUpdated(Dictionary<BUNDLE, string> priceList);

	private List<GoogleSkuInfo> skus = new List<GoogleSkuInfo>();

	private List<GooglePurchase> purchases = new List<GooglePurchase>();

	private Dictionary<BUNDLE, string> _priceList = new Dictionary<BUNDLE, string>();

	private static string[] _productsIdentifiers = new string[6] { "coinbundle_1", "coinbundle_2", "coinbundle_3", "coinbundle_4", "coinbundle_5", "vippass" };

	private static int[] _productsValues = new int[5] { 200, 750, 2000, 6000, 25000 };

	private bool inAppBillingSupported;

	[method: MethodImpl(32)]
	public static event HandlePricesUpdated PricesUpdated;

	private void Awake()
	{
		MonoSingleton<InAppPurchasing>.Instance = this;
		Object.DontDestroyOnLoad(this);
	}

	private void Start()
	{
		GoogleIABManager.queryInventorySucceededEvent += HandleQueryInventorySucceeded;
		GoogleIABManager.purchaseSucceededEvent += HandleGooglePurchaseSucceededEvent;
		GoogleIABManager.consumePurchaseSucceededEvent += HandleGoogleConsumePurchaseSucceededEvent;
		GoogleIAB.init("MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAs+w4jVeWs/r3EGHKJrc7xS9aiFlG/+2iBIhNpKUw9gbm6HCMvbsCBUUwvq8qfbbNyv1hfTa2ymK4XRXT8k5d6C32JvDRHsVoz/n4RmYX27VfiNMsPeT5O6iL87k9Nm+L/3UK/L+WbBSy1tDOR/OTTiiXH0P8CjcdLCIBySuxYjB/3F1KWcXiRur0lcHs747pD8gGOKfF4oPkQ1WyIK/zij0fUVAGRPRUeD6BxT1ohI8M/uQ7DQpcMdj37ROnDpSJ1hkeFDiD9Ae/R8ywdT79ZgT7+6OHHM69bTgPtIgIxTZCzZ4Ev2zoXwJnxteb807MzCY7DteCJ96Z+lweR2R4mwIDAQAB");
		GoogleIABManager.billingSupportedEvent += delegate
		{
			inAppBillingSupported = true;
			GoogleIAB.queryInventory(_productsIdentifiers);
		};
	}

	private void HandleQueryInventorySucceeded(List<GooglePurchase> arg1, List<GoogleSkuInfo> arg2)
	{
		purchases = arg1;
		skus = arg2;
		_priceList.Clear();
		List<string> list = new List<string>();
		foreach (GooglePurchase purchase in purchases)
		{
			if (purchase.purchaseState == GooglePurchase.GooglePurchaseState.Purchased)
			{
				list.Add(purchase.productId);
			}
		}
		if (list.Count != 0)
		{
			GoogleIAB.consumeProducts(list.ToArray());
		}
		foreach (GoogleSkuInfo item in arg2)
		{
			BUNDLE typeForIdentifier = GetTypeForIdentifier(item.productId);
			if (!_priceList.ContainsKey(typeForIdentifier))
			{
				_priceList.Add(typeForIdentifier, string.Empty + item.price);
			}
		}
		if (InAppPurchasing.PricesUpdated != null)
		{
			InAppPurchasing.PricesUpdated(_priceList);
		}
	}

	public void PurchaseProduct(string productID)
	{
		if (inAppBillingSupported)
		{
			GoogleIAB.purchaseProduct(productID);
		}
	}

	private void HandleGooglePurchaseSucceededEvent(GooglePurchase obj)
	{
		GoogleIAB.consumeProduct(obj.productId);
	}

	private void HandleGoogleConsumePurchaseSucceededEvent(GooglePurchase obj)
	{
		if (obj.productId == "vippass")
		{
			AwardVIPPass();
		}
		else
		{
			int valueForProduct = GetValueForProduct(GetTypeForIdentifier(obj.productId));
			GameData.PlayerStats.AddCoins(valueForProduct);
			PlayerManager.SendFeedback("Purchased " + valueForProduct + " Coins on Android", string.Empty, string.Empty);
		}
		PlayerPrefs.Save();
	}

	public string GetIdentifierForProduct(BUNDLE coinsPack)
	{
		string text = _productsIdentifiers[(int)coinsPack];
		foreach (GoogleSkuInfo sku in skus)
		{
			if (sku.productId == text)
			{
				return sku.productId;
			}
		}
		return string.Empty;
	}

	public static BUNDLE GetTypeForIdentifier(string identifier)
	{
		for (int i = 0; i < _productsIdentifiers.Length; i++)
		{
			if (_productsIdentifiers[i] == identifier)
			{
				return (BUNDLE)i;
			}
		}
		Debug.LogWarning("Product identifier = " + identifier + " not matching with any ID");
		return BUNDLE.PACK_1;
	}

	public static int GetValueForProduct(BUNDLE coinBundle)
	{
		return _productsValues[(int)coinBundle];
	}

	public void RequestProductsData()
	{
	}

	public bool PurchasingEnabled()
	{
		return inAppBillingSupported;
	}

	public string GetPriceForProduct(BUNDLE productID)
	{
		if (_priceList.ContainsKey(productID))
		{
			return _priceList[productID];
		}
		Debug.LogWarning(string.Concat("No product of type ", productID, " was found"));
		return string.Empty;
	}

	private void productListRequestFailed(string error)
	{
		Debug.Log("productListRequestFailed: " + error);
	}

	private void receiptValidationSuccessful()
	{
		Debug.Log("receipt validation successful");
	}

	private void receiptValidationFailed(string error)
	{
		Debug.Log("receipt validation failed with error: " + error);
	}

	private void receiptValidationRawResponseReceived(string response)
	{
		Debug.Log("receipt validation raw response: " + response);
	}

	private void purchaseFailed(string error)
	{
		Debug.Log("purchase failed with error: " + error);
	}

	private void purchaseCancelled(string error)
	{
		Debug.Log("purchase cancelled with error: " + error);
	}

	private void restoreTransactionsFailed(string error)
	{
		Debug.Log("restoreTransactionsFailed: " + error);
	}

	private void restoreTransactionsFinished()
	{
		Debug.Log("restoreTransactionsFinished");
		CheckForVIPPass();
	}

	public void AwardVIPPass()
	{
		if (!PlayerPrefs.HasKey("VIPPass"))
		{
			PlayerPrefs.SetInt("VIPPass", 1);
			GameData.PlayerStats.AddCoins(1000);
			PlayerManager.SendFeedback("Purchased VIP Pass", string.Empty, string.Empty);
		}
	}

	public void CheckForVIPPass()
	{
	}

	public void RestorePurchases()
	{
	}
}
