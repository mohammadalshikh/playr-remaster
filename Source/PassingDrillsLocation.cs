public class PassingDrillsLocation : MapLocation
{
	private GameInfo passingDrillsInfo;

	protected override void HandleIntroSceneCompleted()
	{
		base.HandleIntroSceneCompleted();
		GetIsLockedData();
	}

	public override void Start()
	{
		base.Start();
		if (GameData.Instance != null && GameData.Instance.HasIntroSeqOccured)
		{
			GetIsLockedData();
		}
	}

	private void GetIsLockedData()
	{
		passingDrillsInfo = PlayerManager.GetGameInfoForLocationAndType(GameData.GAME_LOCATION.PassingDrills, GameData.GAME_TYPE.Countdown);
		IsLocked = !passingDrillsInfo.IsUnlocked;
	}
}
