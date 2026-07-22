using System.Collections;
using UnityEngine;

public class HighscorePopup : ScreenPopup
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
				return GameData.PlayerStats.NewHighScore && MonoSingleton<FacebookHandler>.Instance.HasLoggedIntoFBBefore;
			}
			return GameData.PlayerStats.NewHighScore;
		}
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

	private void StampDustAnimation()
	{
		GameObject gameObject = (GameObject)Object.Instantiate(_stampDustPrefab, _stampPoint.transform.position, _stampDustPrefab.transform.rotation);
		gameObject.transform.parent = base.transform;
		AudioManager.PlaySFX(AudioManager.Instance.audioCollection.CurrentGame.standardAudio.stamp, 0f);
	}

	protected override void Post()
	{
		Facebook.instance.postMessageWithLinkAndLinkToImage(GameData.m_PlayerName + " just got a new highscore in I AM PLAYR", "http://www.facebook.com/appcenter/iamplayr", "New Highscore", "https://sites.google.com/a/werinteractive.com/mobile/resources/images/soccerplayr.png", "I AM PLAYR", DidSucceedPosting);
		PlayerManager.SendFeedback("Hit Share highscore", string.Empty, string.Empty);
	}

	protected override void DidSucceedPosting(string error, object result)
	{
		base.DidSucceedPosting(error, result);
		if (error != null)
		{
			GameData.GUIManager.ShowInfoView("FACEBOOK POST", "Failed posting your new highscore to Facebook, please try again", null);
		}
		else
		{
			GameData.GUIManager.ShowInfoView("FACEBOOK POST", "You have posted your new highscore to Facebook succesfully", null);
		}
	}
}
