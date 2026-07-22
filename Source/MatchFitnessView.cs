using System.Collections;
using UnityEngine;

using UnityEngine.SceneManagement;
public class MatchFitnessView : View
{
	public enum MatchFitnessViewState
	{
		NotConnected,
		MatchProgressing,
		MatchUnlocked,
		Unavailable
	}

	[SerializeField]
	private SpriteText matchFitnessStatus;

	[SerializeField]
	private SpriteText matchFitnessAlert;

	[SerializeField]
	private MatchFitnessBarView matchFitnessBarView;

	[SerializeField]
	private bool isExpandedView;

	[SerializeField]
	private UIButton iamplayrLinkButton;

	private MatchFitnessViewState currentState;

	public SpriteText MatchFitnessAlert => matchFitnessAlert;

	private void Start()
	{
		if ((bool)iamplayrLinkButton)
		{
			iamplayrLinkButton.renderer.enabled = false;
			iamplayrLinkButton.collider.enabled = false;
			iamplayrLinkButton.AddInputDelegate(HandleIamplayerLinkButtonInput);
		}
		if (!FacebookHandler.IsSessionValid() && GameData.Instance.HasIntroSceneFinished && (bool)matchFitnessAlert)
		{
			matchFitnessAlert.renderer.enabled = true;
			matchFitnessAlert.Text = "Learn More";
		}
		if (MonoSingleton<WebIntegrationManager>.IsInitialized)
		{
			if (SceneManager.GetActiveScene().buildIndex != 3)
			{
				MonoSingleton<WebIntegrationManager>.Instance.RefreshData();
				MonoSingleton<WebIntegrationManager>.Instance.WebUserDataReceived += HandleWebUserDataReceived;
			}
			else if (GameData.Instance != null && GameData.SelectedGameType != GameData.GAME_TYPE.Tutorial)
			{
				GameData.PlayerStats.MatchFitnessEarnedChanged += MatchFitnessEarnedChanged;
			}
		}
		FacebookHandler.FacebookLoggedOut += HandleFacebookLoggedOut;
	}

	protected virtual void HandleWebUserDataReceived(int mfFitness, int mfFitnessReq, bool isMatchFitnessDisabled)
	{
		if (isMatchFitnessDisabled)
		{
			if ((bool)matchFitnessAlert)
			{
				matchFitnessAlert.Text = "UNAVAILABLE";
			}
			currentState = MatchFitnessViewState.Unavailable;
		}
		else if (MonoSingleton<WebIntegrationManager>.Instance.IsMatchUnlocked)
		{
			if ((bool)matchFitnessAlert)
			{
				if (!isExpandedView)
				{
					matchFitnessAlert.Text = "MATCH UNLOCKED";
				}
				else
				{
					matchFitnessAlert.Text = string.Concat("Match available on I AM PLAYR web. Play River Park vs ", MonoSingleton<WebIntegrationManager>.Instance.OpponentName, " at ", Color.blue, " https://www.facebook.com/iamplayr ", Color.white, "on your PC or laptop");
					iamplayrLinkButton.renderer.enabled = true;
					iamplayrLinkButton.collider.enabled = true;
				}
			}
			currentState = MatchFitnessViewState.MatchUnlocked;
		}
		else
		{
			if ((bool)matchFitnessStatus)
			{
				matchFitnessStatus.Text = mfFitness + " / " + mfFitnessReq;
			}
			if ((bool)matchFitnessBarView)
			{
				matchFitnessBarView.AnimateBarToValue((float)mfFitness / (float)mfFitnessReq);
			}
			currentState = MatchFitnessViewState.MatchProgressing;
		}
		if (!isExpandedView)
		{
			if (GameData.Instance.HasIntroSceneFinished)
			{
				DisplayView(isVisible: true);
			}
			else if (MonoSingleton<MapController>.IsInitialized)
			{
				StartCoroutine(DisplayViewDelay(MonoSingleton<MapController>.Instance.CamAnimationTime));
			}
		}
	}

	public override void DisplayView(bool isVisible)
	{
		base.DisplayView(isVisible);
		MatchFitnessViewState matchFitnessViewState = currentState;
		if (matchFitnessViewState == MatchFitnessViewState.MatchProgressing)
		{
			if ((bool)matchFitnessAlert)
			{
				matchFitnessAlert.renderer.enabled = false;
			}
			if ((bool)matchFitnessStatus)
			{
				matchFitnessStatus.renderer.enabled = true;
			}
			if ((bool)matchFitnessBarView)
			{
				matchFitnessBarView.DisplayView(isVisible: true);
			}
		}
		else
		{
			if ((bool)matchFitnessAlert)
			{
				matchFitnessAlert.renderer.enabled = true;
			}
			if ((bool)matchFitnessStatus)
			{
				matchFitnessStatus.renderer.enabled = false;
			}
			if ((bool)matchFitnessBarView)
			{
				matchFitnessBarView.DisplayView(isVisible: false);
			}
		}
	}

	private IEnumerator DisplayViewDelay(float delay)
	{
		yield return new WaitForSeconds(delay);
		DisplayView(isVisible: true);
	}

	private void MatchFitnessEarnedChanged()
	{
		if ((bool)matchFitnessBarView && (bool)matchFitnessStatus && GameData.Instance != null && FacebookHandler.IsSessionValid())
		{
			float num = GameData.PlayerStats.MatchFitnessEarned;
			float num2 = RewardManager.HighestMatchAvailableForLevel();
			matchFitnessBarView.AnimateBarToValue(num / num2);
			matchFitnessStatus.Text = num + " / " + num2;
		}
		else
		{
			DisplayView(isVisible: false);
		}
	}

	private void HandleFacebookLoggedOut()
	{
		currentState = MatchFitnessViewState.NotConnected;
		RefreshMatchAlert();
		if (!isExpandedView)
		{
			DisplayView(isVisible: true);
		}
	}

	private void RefreshMatchAlert()
	{
		matchFitnessAlert.renderer.enabled = true;
		matchFitnessAlert.Text = "Learn More";
	}

	private void HandleIamplayerLinkButtonInput(ref POINTER_INFO ptr)
	{
		if (ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			Application.OpenURL("https://www.facebook.com/iamplayr");
		}
	}

	private void OnDisable()
	{
		if (MonoSingleton<WebIntegrationManager>.IsInitialized)
		{
			MonoSingleton<WebIntegrationManager>.Instance.WebUserDataReceived -= HandleWebUserDataReceived;
		}
		GameData.PlayerStats.MatchFitnessEarnedChanged -= MatchFitnessEarnedChanged;
		FacebookHandler.FacebookLoggedOut -= HandleFacebookLoggedOut;
	}
}
