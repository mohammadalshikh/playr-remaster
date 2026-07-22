using System;
using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;

public class GameTypeMenuManager : MonoSingleton<GameTypeMenuManager>
{
	private const string backgroundPath = "GameTypeScreens/";

	[SerializeField]
	private BackgroundMask backgroundMask;

	[SerializeField]
	private LoadGame gameToLoad;

	[SerializeField]
	private Camera bgCamera;

	[SerializeField]
	private Camera mainCamera;

	[SerializeField]
	private GUITexture currentBackground;

	[SerializeField]
	private BuyCoinsView buyCoinsView;

	[SerializeField]
	private UIButton homeButton;

	[SerializeField]
	private GameTypePanel gameTypePanel;

	private UnityEngine.Object currentBackgroundObj;

	public GameTypePanel GameTypePanel => gameTypePanel;

	[method: MethodImpl(32)]
	public static event Action CamOrthgraphicSizeChanged;

	private void Awake()
	{
		MonoSingleton<GameTypeMenuManager>.Instance = this;
	}

	private void Start()
	{
		if (GameData.Instance != null)
		{
			if (GameData.SelectedGameLocation == GameData.GAME_LOCATION.ClubHouse)
			{
				currentBackgroundObj = (Texture)Resources.Load("GameTypeScreens/" + GameData.GAME_LOCATION.PassingDrills, typeof(Texture));
			}
			else
			{
				currentBackgroundObj = Resources.Load("GameTypeScreens/" + GameData.SelectedGameLocation, typeof(Texture));
			}
			currentBackground.texture = (Texture)currentBackgroundObj;
		}
		backgroundMask.UnmaskBackground();
		if ((bool)homeButton)
		{
			homeButton.AddInputDelegate(HandleHomeButtonPressed);
		}
		if (DeviceInfo.IsTablet)
		{
			mainCamera.orthographicSize = 111f;
			GameTypeMenuManager.CamOrthgraphicSizeChanged();
		}
	}

	public void PlayGame(GameData.GAME_TYPE gameType)
	{
		gameTypePanel.SaveAppointments();
		float defaultTransitionTime = backgroundMask.DefaultTransitionTime;
		UIManager.instance.blockInput = true;
		bgCamera.depth = 1f;
		backgroundMask.MaskBackground(defaultTransitionTime);
		StartCoroutine(gameToLoad.PlayGame(GameData.SelectedGameLocation, gameType, defaultTransitionTime));
	}

	private void HandleHomeButtonPressed(ref POINTER_INFO ptr)
	{
		if (ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			gameTypePanel.SaveAppointments();
			gameTypePanel.ProcessGameLocationsWithApptsDue();
			ReturnToMap();
		}
	}

	private IEnumerator LoadSceneDelay(float delayTime, SCENES scene)
	{
		yield return new WaitForSeconds(delayTime * 0.5f);
		StartCoroutine(GameData.Instance.LoadLevel(scene));
	}

	public void ReturnToMap()
	{
		float defaultTransitionTime = backgroundMask.DefaultTransitionTime;
		bgCamera.depth = 1f;
		if (MonoSingleton<TapjoyManager>.IsInitialized)
		{
			MonoSingleton<TapjoyManager>.Instance.ToggleAdDisplaying(isDisplaying: false);
		}
		backgroundMask.MaskBackground(defaultTransitionTime);
		StartCoroutine(LoadSceneDelay(defaultTransitionTime, SCENES.MAP));
	}

	private void OnDisable()
	{
		Resources.UnloadAsset(currentBackgroundObj);
	}

	public void DisplayCoinsView()
	{
		buyCoinsView.DisplayView(isVisible: true);
	}
}
