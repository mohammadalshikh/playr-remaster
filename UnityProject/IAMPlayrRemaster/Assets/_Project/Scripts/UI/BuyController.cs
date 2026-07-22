using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class BuyController : MonoBehaviour
{
	public delegate void HandleProductPrices();

	public const string POUND_SIGN = "£";

	[SerializeField]
	private UIPanel _controllerPanel;

	[SerializeField]
	private SpriteText _title;

	private UIPanel _dismissPanel;

	public static BuyController Instance;

	private string[] _prices;

	private Dictionary<InAppPurchasing.BUNDLE, string> _priceList = new Dictionary<InAppPurchasing.BUNDLE, string>();

	private Dictionary<InAppPurchasing.BUNDLE, string> _priceListInCents = new Dictionary<InAppPurchasing.BUNDLE, string>();

	[SerializeField]
	private UIButton _closeButton;

	[SerializeField]
	private UIButton earnMoreButton;

	private string[] _defaultPrices = new string[5] { "0.69", "1.99", "4.99", "13.99", "59.99" };

	private string[] _defaultPriceInCents = new string[5] { "99", "299", "699", "1999", "8999" };

	[method: MethodImpl(32)]
	public event HandleProductPrices ProductPricesUpdated;

	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
		}
		for (int i = 0; i < _defaultPrices.Length; i++)
		{
			_priceList.Add((InAppPurchasing.BUNDLE)i, "£" + _defaultPrices[i]);
		}
		int j = 0;
		for (int num = _defaultPriceInCents.Length; j < num; j++)
		{
			_priceListInCents.Add((InAppPurchasing.BUNDLE)j, _defaultPriceInCents[j]);
		}
	}

	private void OnEnable()
	{
		InAppPurchasing.PricesUpdated += HandleUpdatedPrices;
	}

	private void OnDisable()
	{
		InAppPurchasing.PricesUpdated -= HandleUpdatedPrices;
	}

	private void Start()
	{
		if ((bool)_closeButton)
		{
			_closeButton.AddInputDelegate(HandleCloseButton);
		}
		if ((bool)earnMoreButton)
		{
			earnMoreButton.AddInputDelegate(HandleEarnButtonInput);
		}
		if (MonoSingleton<InAppPurchasing>.Instance != null)
		{
			MonoSingleton<InAppPurchasing>.Instance.RequestProductsData();
		}
	}

	private void HandleCloseButton(ref POINTER_INFO ptr)
	{
		if (ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			SwapPanel(show: false);
		}
	}

	private void HandleEarnButtonInput(ref POINTER_INFO ptr)
	{
		if (ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			TapjoyPlugin.ShowOffers();
		}
	}

	public bool Buy(string productID)
	{
		if (Application.internetReachability == NetworkReachability.NotReachable)
		{
			return false;
		}
		if (MonoSingleton<InAppPurchasing>.IsInitialized)
		{
			MonoSingleton<InAppPurchasing>.Instance.PurchaseProduct(productID);
		}
		return true;
	}

	public string GetPriceForProduct(InAppPurchasing.BUNDLE productID)
	{
		if (_priceList.ContainsKey(productID))
		{
			return _priceList[productID];
		}
		Debug.LogWarning(string.Concat("No product of type ", productID, " was found"));
		return string.Empty;
	}

	public string GetPriceForProductInCents(InAppPurchasing.BUNDLE productID)
	{
		if (_priceListInCents.ContainsKey(productID))
		{
			return _priceListInCents[productID];
		}
		Debug.LogWarning(string.Concat("No product of type ", productID, " was found"));
		return string.Empty;
	}

	public void UpdatePriceTags()
	{
		if (this.ProductPricesUpdated != null)
		{
			this.ProductPricesUpdated();
		}
	}

	public void ShowPanel(bool bShow)
	{
		if (bShow)
		{
			_controllerPanel.BringIn();
		}
		else
		{
			_controllerPanel.Dismiss();
		}
		UpdatePriceTags();
	}

	public void ShowAndComeBackToPanel(UIPanel comeBackPanel, string windowTitle = "BUY COINS")
	{
		_title.Text = windowTitle;
		_dismissPanel = comeBackPanel;
		SwapPanel(show: true);
	}

	private void SwapPanel(bool show)
	{
		if (!_controllerPanel)
		{
			return;
		}
		if ((bool)_dismissPanel)
		{
			if (show)
			{
				_dismissPanel.Dismiss();
			}
			else
			{
				_dismissPanel.BringIn();
			}
		}
		if (show)
		{
			_controllerPanel.BringIn();
		}
		else
		{
			_controllerPanel.Dismiss();
		}
	}

	private void HandleUpdatedPrices(Dictionary<InAppPurchasing.BUNDLE, string> priceList)
	{
		_priceList = priceList;
	}
}
