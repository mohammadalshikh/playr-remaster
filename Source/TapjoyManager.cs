using UnityEngine;

public class TapjoyManager : MonoSingleton<TapjoyManager>
{
	private const string appId = "c0f3c02d-ccbd-4850-8afa-64f04af80243";

	[SerializeField]
	private RIJNDAEL_encryption encryptionTool;

	private void Awake()
	{
		Object.DontDestroyOnLoad(this);
		MonoSingleton<TapjoyManager>.Instance = this;
	}

	private void HandleEncryptionToolInitialised()
	{
		string text = "quERfR41H";
		string text2 = "7eU2A63iOpF";
		byte[] encoded = encryptionTool.encrypt(encryptionTool.string_to_byte_array(text + text2), random_iv: true);
		TapjoyPlugin.RequestTapjoyConnect("c0f3c02d-ccbd-4850-8afa-64f04af80243", encryptionTool.byte_array_to_string(encryptionTool.decrypt(encoded)));
		encryptionTool.EncryptionToolInitialised -= HandleEncryptionToolInitialised;
	}

	private void HandleGetDisplayAdSuccedded()
	{
		int x = Screen.width / 2 - 320;
		int y = Screen.height - 100;
		TapjoyPlugin.MoveDisplayAd(x, y);
		TapjoyPlugin.SetTransitionEffect(1);
		TapjoyPlugin.HideDisplayAd();
	}

	private void HandleTapjoyPlugintapPointsEarned(int obj)
	{
		GameData.PlayerStats.AddCoins(obj);
		TapjoyPlugin.SpendTapPoints(obj);
	}

	public void ToggleAdDisplaying(bool isDisplaying)
	{
		if (isDisplaying)
		{
			TapjoyPlugin.ShowDisplayAd();
		}
		else
		{
			TapjoyPlugin.HideDisplayAd();
		}
	}

	private void OnEnable()
	{
		encryptionTool.EncryptionToolInitialised += HandleEncryptionToolInitialised;
		TapjoyPlugin.connectCallSucceeded += delegate
		{
			TapjoyPlugin.GetDisplayAd();
		};
		TapjoyPlugin.getDisplayAdSucceeded += HandleGetDisplayAdSuccedded;
		TapjoyPlugin.tapPointsEarned += HandleTapjoyPlugintapPointsEarned;
		View.PanelTransitioned += delegate(bool isDisplaying)
		{
			ToggleAdDisplaying(isDisplaying);
		};
	}

	private void OnDisable()
	{
		TapjoyPlugin.connectCallSucceeded -= delegate
		{
			TapjoyPlugin.GetDisplayAd();
		};
		TapjoyPlugin.getDisplayAdSucceeded -= HandleGetDisplayAdSuccedded;
		TapjoyPlugin.tapPointsEarned -= HandleTapjoyPlugintapPointsEarned;
		View.PanelTransitioned -= delegate(bool isDisplaying)
		{
			ToggleAdDisplaying(isDisplaying);
		};
	}
}
