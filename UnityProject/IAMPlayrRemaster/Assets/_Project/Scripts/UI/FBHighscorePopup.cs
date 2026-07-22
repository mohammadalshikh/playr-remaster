using System.Collections;
using UnityEngine;

public class FBHighscorePopup : ScreenPopup
{
	[SerializeField]
	private UIPanel _highscoreStamp;

	[SerializeField]
	private HighscoreView _highscoreView;

	[SerializeField]
	private GameObject _stampPoint;

	[SerializeField]
	private GameObject _stampDustPrefab;

	public override bool CanShow
	{
		get
		{
			if (MonoSingleton<FacebookHandler>.IsInitialized)
			{
				return GameData.PlayerStats.NewHighScore && !MonoSingleton<FacebookHandler>.Instance.HasLoggedIntoFBBefore;
			}
			return false;
		}
	}

	private void OnEnable()
	{
		FacebookManager.sessionOpenedEvent += DidSucceedConnecting;
		FacebookManager.loginFailedEvent += FailedToConnect;
	}

	private void OnDisable()
	{
		FacebookManager.sessionOpenedEvent -= DidSucceedConnecting;
		FacebookManager.loginFailedEvent -= FailedToConnect;
	}

	protected override void Init()
	{
		StartCoroutine(AnimateStampElement(_highscoreStamp, _stampPoint.transform.position));
		_highscoreView.UpdateHighScore(PlayerManager.GetCurrentGame());
	}

	private IEnumerator AnimateStampElement(UIPanel element, Vector3 stampPoint)
	{
		element.StartTransition(UIPanelManager.SHOW_MODE.BringInForward);
		while (element.IsTransitioning())
		{
			yield return null;
		}
		StampDustAnimation();
		yield return true;
	}

	public override void Share()
	{
		MinigameGUI.GreyOutButton(_shareButton, bGrayOut: true);
		if (Application.internetReachability == NetworkReachability.NotReachable)
		{
			GameData.GUIManager.ShowInfoView("INTERNET CONNECTION", "Doesn't look like you're connected to the internet. Please check your connection and try again.", null);
		}
		else
		{
			MonoSingleton<FacebookHandler>.Instance.LoginFacebook();
		}
	}

	private void StampDustAnimation()
	{
		GameObject gameObject = (GameObject)Object.Instantiate(_stampDustPrefab, _stampPoint.transform.position, _stampDustPrefab.transform.rotation);
		gameObject.transform.parent = base.transform;
		AudioManager.PlaySFX(AudioManager.Instance.audioCollection.CurrentGame.standardAudio.stamp, 0f);
	}

	private void DidSucceedConnecting()
	{
		Dismiss();
		GameData.PlayerStats.AddCoins(500);
		GameData.GUIManager.ShowInfoView("FACEBOOK CONNECTED", "You have connected to Facebook, you won 500 coins!", null);
	}

	private void FailedToConnect(string error)
	{
		Dismiss();
		GameData.GUIManager.ShowInfoView("FACEBOOK CONNECT FAILED", "Failed while connecting to facebook, please try again later", null);
	}
}
