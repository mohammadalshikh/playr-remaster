using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DrillSettings
{
	public enum LEVEL
	{
		LEVEL_1,
		LEVEL_2,
		LEVEL_3,
		LEVEL_4
	}

	[SerializeField]
	private DrillID _id = new DrillID();

	[SerializeField]
	private string _description = string.Empty;

	[SerializeField]
	private int _unlockedAtLevel;

	[SerializeField]
	private List<POWER_UP> _powerupsAvailable = new List<POWER_UP>();

	[SerializeField]
	private int _bronzeScore;

	[SerializeField]
	private int _silverScore;

	[SerializeField]
	private int _goldScore;

	[SerializeField]
	private int coinsToSkipAppt;

	public string Name => _id.FileName;

	public DrillID Id => _id;

	public string Description => _description;

	public bool HasBeenPlayed => PlayerPrefs.HasKey("played_" + Name);

	public int UnlockedAtLevel => _unlockedAtLevel;

	public int CoinsToSkipAppt => coinsToSkipAppt;

	public int GetScoreForMedal(MEDAL medal)
	{
		return medal switch
		{
			MEDAL.BRONZE => _bronzeScore, 
			MEDAL.SILVER => _silverScore, 
			MEDAL.GOLD => _goldScore, 
			_ => 0, 
		};
	}

	public List<POWER_UP> GetAvailablePowerUps()
	{
		return _powerupsAvailable;
	}

	public void MarkPlayed()
	{
		if (!PlayerPrefs.HasKey("played_" + Name))
		{
			PlayerPrefs.SetInt("played_" + Name, 0);
			PlayerPrefs.Save();
		}
	}
}
