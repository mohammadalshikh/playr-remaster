using System;
using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;

public class IntroView : CharacterNotificationView
{
	public delegate void HandleIntroSceneCompleted();

	[SerializeField]
	private PackedSprite titleLogo;

	[SerializeField]
	private PackedSprite tapToStartTextBG;

	[SerializeField]
	private GenericButton fbButton;

	[SerializeField]
	private GenericButton skipSignInButton;

	[SerializeField]
	private SpriteText tapToStartText;

	[method: MethodImpl(32)]
	public static event HandleIntroSceneCompleted IntroSceneCompleted;

	private void Start()
	{
		CameraScrollController.CameraAnimCompleted += HandleCameraAnimCompleted;
		if (PlayerPrefs.HasKey("FirstTimeBoot"))
		{
			GameData.Instance.FirstEverBoot = PlayerPrefs.GetInt("FirstTimeBoot") == 1;
		}
	}

	private void HandleCameraAnimCompleted()
	{
		titleLogo.renderer.enabled = true;
		tapToStartText.renderer.enabled = true;
		tapToStartTextBG.renderer.enabled = true;
		iTween.ValueTo(titleLogo.gameObject, iTween.Hash("from", 0f, "to", 1f, "time", 1f, "delay", 0.5f, "onUpdate", (Action<object>)delegate(object x)
		{
			titleLogo.SetColor(new Color(titleLogo.Color.r, titleLogo.Color.g, titleLogo.Color.b, (float)x));
		}));
		iTween.ValueTo(tapToStartText.gameObject, iTween.Hash("from", 0f, "to", 1f, "time", 1f, "delay", 0.5f, "onUpdate", (Action<object>)delegate(object x)
		{
			tapToStartText.SetColor(new Color(tapToStartText.Color.r, tapToStartText.Color.g, tapToStartText.Color.b, (float)x));
		}));
		iTween.ValueTo(tapToStartTextBG.gameObject, iTween.Hash("from", 0f, "to", 1f, "time", 1f, "delay", 0.5f, "onUpdate", (Action<object>)delegate(object x)
		{
			tapToStartTextBG.SetColor(new Color(tapToStartTextBG.Color.r, tapToStartTextBG.Color.g, tapToStartTextBG.Color.b, (float)x));
		}));
		if (UIManager.instance != null)
		{
			UIManager.instance.AddMouseTouchPtrListener(TouchStartScreen);
		}
	}

	private void TouchStartScreen(POINTER_INFO ptr)
	{
		if (ptr.evt != POINTER_INFO.INPUT_EVENT.TAP)
		{
			return;
		}
		UIManager.instance.RemoveMouseTouchPtrListener(TouchStartScreen);
		if (GameData.Instance.FirstEverBoot)
		{
			titleLogo.gameObject.active = false;
			tapToStartText.gameObject.active = false;
			tapToStartTextBG.gameObject.active = false;
			Init("Welcome to River Park FC", string.Empty, string.Empty, CharacterId.Coach);
			DisplayView(isVisible: true);
			StartCoroutine(InputDelegate());
		}
		else
		{
			titleLogo.gameObject.active = false;
			tapToStartText.gameObject.active = false;
			tapToStartTextBG.gameObject.active = false;
			if (IntroView.IntroSceneCompleted != null)
			{
				IntroView.IntroSceneCompleted();
			}
			GameData.Instance.HasIntroSceneFinished = true;
		}
		PlayerPrefs.SetInt("FirstTimeBoot", 0);
	}

	private IEnumerator InputDelegate()
	{
		yield return new WaitForSeconds(1f);
		fbButton.AddInputDelegate(HandleFacebookPressed);
		skipSignInButton.AddInputDelegate(HandleSkipSignInButtonInput);
	}

	private void HandleFacebookPressed(ref POINTER_INFO ptr)
	{
		if (ptr.evt == POINTER_INFO.INPUT_EVENT.TAP && MonoSingleton<FacebookHandler>.IsInitialized)
		{
			PlayerManager.SendFeedback("IntroFBButtonPressed", string.Empty, string.Empty);
			IntroButtonPressed();
			if (MonoSingleton<FacebookHandler>.IsInitialized)
			{
				MonoSingleton<FacebookHandler>.Instance.LoginFacebook();
			}
		}
	}

	private void HandleSkipSignInButtonInput(ref POINTER_INFO ptr)
	{
		if (ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			PlayerManager.SendFeedback("SkippedSignInButtonPressed", string.Empty, string.Empty);
			IntroButtonPressed();
		}
	}

	private void IntroButtonPressed()
	{
		GameData.Instance.HasIntroSceneFinished = true;
		DisplayView(isVisible: false);
		if (IntroView.IntroSceneCompleted != null)
		{
			IntroView.IntroSceneCompleted();
		}
		titleLogo.gameObject.active = false;
	}

	private void OnDisable()
	{
		CameraScrollController.CameraAnimCompleted -= HandleCameraAnimCompleted;
	}
}
