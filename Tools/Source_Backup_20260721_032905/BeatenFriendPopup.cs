using UnityEngine;

public class BeatenFriendPopup : ScreenPopup
{
	[SerializeField]
	private FacebookPlayerView _playerView;

	[SerializeField]
	private FacebookPlayerView _friendView;

	[SerializeField]
	private SpriteText _message;

	public override bool CanShow => GameData.BeatFriend;

	protected override void Init()
	{
		int currentPosition = GameData.m_CurrentPosition;
		GameData.LeaderInfo leaderInfo = (GameData.LeaderInfo)GameData.m_Leaders[currentPosition - 1];
		GameData.LeaderInfo leaderInfo2 = (GameData.LeaderInfo)GameData.m_Leaders[currentPosition];
		SetupPlayers(new FacebookPlayer(leaderInfo.m_Id, leaderInfo.m_Name, (int)leaderInfo.m_Score, currentPosition), new FacebookPlayer(leaderInfo2.m_Id, leaderInfo2.m_Name, (int)leaderInfo2.m_Score, currentPosition + 1));
		BringInPanels(0.5f);
	}

	protected override void Reset()
	{
		DismissPanels(0.5f);
	}

	private void SetupPlayers(FacebookPlayer player, FacebookPlayer friend)
	{
		_playerView.Setup(player);
		_friendView.Setup(friend);
		SetMessage(friend.Name);
	}

	private void BringInPanels(float delay)
	{
		_playerView.AnimateUp(delay);
		_friendView.AnimateDown(delay);
	}

	private void DismissPanels(float delay)
	{
		if ((bool)_playerView)
		{
			_playerView.AnimateDown(delay);
		}
		if ((bool)_friendView)
		{
			_friendView.AnimateUp(delay);
		}
	}

	private void SetMessage(string friendName)
	{
		_message.Text = "You just beat " + friendName;
	}

	protected override void Post()
	{
		Facebook.instance.postMessageWithLinkAndLinkToImage(GameData.m_PlayerName + " has just outscored one of their friends in I AM PLAYR. Can you beat them?", "http://www.facebook.com/appcenter/iamplayr", "Friend beaten", "https://sites.google.com/a/werinteractive.com/mobile/resources/images/soccerplayr.png", "I AM PLAYR", DidSucceedPosting);
		PlayerManager.SendFeedback("Hit Share leaderboard Jump", string.Empty, string.Empty);
	}

	protected override void DidSucceedPosting(string error, object result)
	{
		base.DidSucceedPosting(error, result);
		if (error != null)
		{
			GameData.GUIManager.ShowInfoView("FACEBOOK POST", "Failed posting to Facebook", null);
		}
		else
		{
			GameData.GUIManager.ShowInfoView("FACEBOOK POST", "You have posted to Facebook succesfully", null);
		}
	}
}
