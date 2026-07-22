using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class PowerUpManager : MonoBehaviour
{
	public delegate void HandleWillPlayGame();

	[SerializeField]
	private UIScrollList _powerUpScrollList;

	[SerializeField]
	private GameObject _powerUpCellPrefab;

	[SerializeField]
	private UIPanel _controllerPanel;

	[SerializeField]
	private InfoView _infoView;

	public static PowerUpManager Instance;

	private bool _firstTimeExecution = true;

	private List<POWER_UP> _availablePowerUps = new List<POWER_UP>();

	public bool IsEnabled => GlobalSettings.GetCurrentGame().GetAvailablePowerUps().Count > 0;

	public UIPanel Panel => _controllerPanel;

	[method: MethodImpl(32)]
	public static event HandleWillPlayGame WillPlayGame;

	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
		}
	}

	private void Start()
	{
		Init();
		if (GameData.SelectedGameLocation == GameData.GAME_LOCATION.TrainingCenter)
		{
			MonoSingleton<WeatherManager>.Instance.ShowClouds(bShow: false);
		}
	}

	private void SetCellForType(POWER_UP type)
	{
		_powerUpScrollList.CreateItem(_powerUpCellPrefab);
	}

	private void SetupPowerUps(List<POWER_UP> availablePowerUps)
	{
		_availablePowerUps = availablePowerUps;
		InstantiateCells(_availablePowerUps);
		SetupCells();
	}

	private void InstantiateCells(List<POWER_UP> cells)
	{
		foreach (POWER_UP cell in cells)
		{
			SetCellForType(cell);
		}
	}

	private void SetupCells()
	{
		for (int i = 0; i < _availablePowerUps.Count; i++)
		{
			_powerUpScrollList.GetItem(i).transform.GetComponent<PowerUpCell>().SetupForType(_availablePowerUps[i]);
		}
	}

	public void UseCoins(int coins)
	{
		GameData.PlayerStats.UseCoins(coins);
	}

	public void Init()
	{
		SetupPowerUps(GlobalSettings.GetCurrentGame().GetAvailablePowerUps());
		_firstTimeExecution = false;
	}

	public void Refresh()
	{
		if (!_firstTimeExecution)
		{
			SetupCells();
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
	}

	private void EndDismissing()
	{
	}

	public void ShowPowerupInfo(string title, string message)
	{
		if (_controllerPanel != null && _infoView != null)
		{
			_infoView.SetAndShow(title, message, _controllerPanel);
		}
	}

	public void PlayGame()
	{
		if (PowerUpManager.WillPlayGame != null)
		{
			PowerUpManager.WillPlayGame();
		}
		GameData.IsComingFromGame = true;
		if (PlayerPrefs.GetInt(GameData.SelectedGameLocation.ToString() + GameData.SelectedGameType.ToString() + "IsNewlyUnlocked") == 0 && !GameData.Instance.IsCurrentApptDue)
		{
			GameData.PlayerStats.UseCoins(GlobalSettings.GetCurrentGame().CoinsToSkipAppt);
			PlayerManager.SendFeedback("BoughtGameBeforeApt " + GameData.SelectedGame, string.Empty, string.Empty);
		}
		PlayerPrefs.SetInt(GameData.SelectedGameLocation.ToString() + GameData.SelectedGameType.ToString() + "IsNewlyUnlocked", 0);
		_controllerPanel.AddTempTransitionDelegate(delegate
		{
			GameData.GUIManager.ClosePowerUpScreen();
		});
		_controllerPanel.Dismiss();
	}

	public void AddCoinsView()
	{
		BuyController.Instance.ShowAndComeBackToPanel(_controllerPanel);
	}
}
