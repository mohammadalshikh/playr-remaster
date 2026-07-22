using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;

public class MapLocation : GenericButton
{
	public delegate void HandleMapButtonTapped(Vector3 targetPos);

	[SerializeField]
	private bool isLocked;

	[SerializeField]
	private GameData.GAME_LOCATION gameLocation;

	[SerializeField]
	private Transform cameraLerpToTrans;

	[SerializeField]
	private PackedSprite mapLocationLabel;

	[SerializeField]
	private PackedSprite labelTail;

	[SerializeField]
	protected SpriteText labelTitle;

	[SerializeField]
	protected SpriteText alertText;

	[SerializeField]
	private UpgradePath[] upgradePaths;

	[SerializeField]
	private PackedSprite lockSprite;

	[SerializeField]
	private TickerTapeText alertTickerTapeText;

	private int currentUpgradeLevel;

	public virtual bool IsLocked
	{
		get
		{
			return isLocked;
		}
		protected set
		{
			isLocked = value;
			if (isLocked && (bool)alertText)
			{
				alertText.Text = string.Empty;
			}
			else if (gameLocation == GameData.GAME_LOCATION.PassingDrills || gameLocation == GameData.GAME_LOCATION.ShootingDrills)
			{
				ProcessAlertMessage();
			}
			if ((bool)lockSprite)
			{
				lockSprite.renderer.enabled = isLocked;
			}
		}
	}

	public GameData.GAME_LOCATION GameLocation => gameLocation;

	public UpgradePath[] UpgradePaths => upgradePaths;

	public bool IsNewGameUnlocked => GameData.PlayerStats.GetIsGameUnlockedAtLocation(gameLocation);

	[method: MethodImpl(32)]
	public static event HandleMapButtonTapped MapButtonTapped;

	public override void Start()
	{
		base.Start();
		if (!GameData.Instance.HasIntroSeqOccured)
		{
			controlIsEnabled = false;
			DisplayLabelView(isVisible: false);
			IntroView.IntroSceneCompleted += HandleIntroSceneCompleted;
		}
		else
		{
			DisplayLabelView(isVisible: true);
			if (gameLocation == GameData.GAME_LOCATION.TrainingCenter || gameLocation == GameData.GAME_LOCATION.PassingDrills || gameLocation == GameData.GAME_LOCATION.ShootingDrills)
			{
				ProcessAlertMessage();
			}
		}
		View.PanelTransitioned += HandlePanelTransitioned;
		ProcessLocationUpgrades();
	}

	protected override void HandleButtonPressed(ref POINTER_INFO ptr)
	{
		if (IsLocked || MapLocation.MapButtonTapped == null || !GameData.Instance.HasIntroSceneFinished)
		{
			return;
		}
		base.HandleButtonPressed(ref ptr);
		if (ptr.evt == POINTER_INFO.INPUT_EVENT.TAP && !MonoSingleton<PanelManager>.Instance.IsPanelDisplaying)
		{
			if ((bool)mapLocationLabel)
			{
				mapLocationLabel.gameObject.SetActiveRecursively(state: false);
			}
			if ((bool)cameraLerpToTrans)
			{
				MapLocation.MapButtonTapped(cameraLerpToTrans.position);
			}
			else
			{
				MapLocation.MapButtonTapped(base.transform.position);
			}
			if (GameData.Instance != null)
			{
				GameData.SelectedGameLocation = gameLocation;
				MapController.gameLocation = gameLocation;
			}
			PlayerManager.SendFeedback("Hit" + gameLocation, string.Empty, string.Empty);
		}
	}

	private void ProcessAlertMessage()
	{
		if (IsNewGameUnlocked)
		{
			StartCoroutine(SetFlashingAlertMessage("New Game!", 0.5f));
		}
		else if (GameData.Instance != null && GameData.Instance.DoesGameLocationHaveApptDue(gameLocation))
		{
			StartCoroutine(SetFlashingAlertMessage("Game Available", 0.5f));
		}
		else
		{
			alertText.Text = string.Empty;
		}
	}

	protected IEnumerator SetFlashingAlertMessage(string message, float flashDelay)
	{
		alertText.Text = message;
		while (true)
		{
			yield return new WaitForSeconds(flashDelay);
			alertText.renderer.enabled ^= true;
		}
	}

	public void ProcessLocationUpgrades()
	{
		int currentLevel = GameData.PlayerStats.CurrentLevel;
		int i = 0;
		for (int num = upgradePaths.Length; i < num; i++)
		{
			if (currentLevel >= upgradePaths[i].LevelToUpgradeAt)
			{
				currentUpgradeLevel++;
			}
		}
		ProcessUpgradeMeshes();
	}

	private void ProcessUpgradeMeshes()
	{
		if (currentUpgradeLevel > upgradePaths.Length)
		{
			return;
		}
		int i = 0;
		for (int num = upgradePaths.Length; i < num; i++)
		{
			int j = 0;
			for (int num2 = upgradePaths[i].UpgradeMeshs.Length; j < num2; j++)
			{
				if (i <= currentUpgradeLevel)
				{
					upgradePaths[i].UpgradeMeshs[j].enabled = true;
				}
				else
				{
					upgradePaths[i].UpgradeMeshs[j].enabled = false;
				}
			}
		}
	}

	protected virtual void HandleIntroSceneCompleted()
	{
		DisplayLabelView(isVisible: true);
		controlIsEnabled = true;
		if (gameLocation == GameData.GAME_LOCATION.TrainingCenter || gameLocation == GameData.GAME_LOCATION.PassingDrills || gameLocation == GameData.GAME_LOCATION.ShootingDrills)
		{
			if (PlayerPrefs.HasKey("HasVisitedMapBefore"))
			{
				ProcessAlertMessage();
			}
			else if (gameLocation == GameData.GAME_LOCATION.ShootingDrills)
			{
				StartCoroutine(SetFlashingAlertMessage("New Game!", 0.5f));
				PlayerPrefs.SetInt("HasVisitedMapBefore", 1);
			}
			else
			{
				alertText.Text = string.Empty;
			}
		}
		IntroView.IntroSceneCompleted -= HandleIntroSceneCompleted;
	}

	public void DisplayLabelView(bool isVisible)
	{
		if ((bool)mapLocationLabel)
		{
			mapLocationLabel.renderer.enabled = isVisible;
		}
		if ((bool)labelTitle)
		{
			labelTitle.renderer.enabled = isVisible;
		}
		if ((bool)alertText)
		{
			alertText.renderer.enabled = isVisible;
		}
		if ((bool)lockSprite)
		{
			lockSprite.renderer.enabled = isVisible;
		}
		if ((bool)labelTail)
		{
			labelTail.renderer.enabled = isVisible;
		}
	}

	private void HandlePanelTransitioned(bool isPanelDisplaying)
	{
		controlIsEnabled = !isPanelDisplaying;
		base.collider.enabled = !isPanelDisplaying;
	}

	protected override void OnDisable()
	{
		base.OnDisable();
		View.PanelTransitioned -= HandlePanelTransitioned;
	}
}
