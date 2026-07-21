using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class TutorialView : InfoView
{
	public delegate void HandleWillShow();

	public delegate void HandleWillHide();

	[SerializeField]
	private List<TutorialData> _shootingDrills = new List<TutorialData>();

	[SerializeField]
	private List<TutorialData> _passingDrills = new List<TutorialData>();

	[SerializeField]
	private List<TutorialData> _trainingCenter = new List<TutorialData>();

	public List<TutorialData> ActiveData => GameData.SelectedGameLocation switch
	{
		GameData.GAME_LOCATION.ShootingDrills => _shootingDrills, 
		GameData.GAME_LOCATION.PassingDrills => _passingDrills, 
		GameData.GAME_LOCATION.TrainingCenter => _trainingCenter, 
		_ => _shootingDrills, 
	};

	public string CurrentDataKey => GameData.SelectedGameLocation.ToString();

	public int CurrentDataIndex
	{
		get
		{
			return PlayerPrefs.GetInt(CurrentDataKey + "tutorial", 0);
		}
		set
		{
			PlayerPrefs.SetInt(CurrentDataKey + "tutorial", value);
		}
	}

	public bool AnyTutorialAvailable => CurrentDataIndex < ActiveData.Count;

	[method: MethodImpl(32)]
	public static event HandleWillShow WillShowEvent;

	[method: MethodImpl(32)]
	public static event HandleWillHide WillHideEvent;

	private void OnEnable()
	{
		Minigame.MinigameReadyEvent += ShowView;
		Minigame.NewChanceWillStartEvent += ShowView;
	}

	private void OnDisable()
	{
		Minigame.MinigameReadyEvent -= ShowView;
		Minigame.NewChanceWillStartEvent -= ShowView;
	}

	private void ShowView()
	{
		if (ActiveData.Count != 0 && AnyTutorialAvailable)
		{
			int currentDataIndex = CurrentDataIndex;
			TutorialData tutorialData = ActiveData[currentDataIndex];
			CurrentDataIndex = currentDataIndex + 1;
			if (TutorialView.WillShowEvent != null)
			{
				TutorialView.WillShowEvent();
			}
			SetAndShow(tutorialData.Title, tutorialData.Content, null, "GOT IT!");
		}
	}

	public void HideView()
	{
		if (TutorialView.WillHideEvent != null)
		{
			TutorialView.WillHideEvent();
		}
		Dismiss();
	}
}
