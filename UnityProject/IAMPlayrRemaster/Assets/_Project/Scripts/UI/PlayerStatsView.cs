public class PlayerStatsView : CharacterNotificationView
{
	private void Start()
	{
		Init("PLAYER STATS", "Rank:\nGames Played:\nGlobal Score:\nTargets Hit:\nTotal Shots:\nAccuracy:", GameData.PlayerStats.Rank + "\n" + GameData.PlayerStats.GamesPlayed + "\n" + GameData.FormatScore(GameData.PlayerStats.GlobalScore) + "\n" + GameData.PlayerStats.HitCounter.ToString("#,##0") + "\n" + GameData.PlayerStats.TotalShots.ToString("#,##0") + "\n" + GameData.PlayerStats.Accuracy + "%", CharacterId.Coach);
	}
}
