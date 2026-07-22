using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaderboardView : View
{
	[SerializeField]
	private UIButton achievementsButton;

	[SerializeField]
	private CharacterNotificationView leaderboardIntroView;

	[SerializeField]
	private GameObject m_PrefabLeaderBoardItem;

	[SerializeField]
	private UIScrollList m_HighScoreList;

	private List<HighScoreListItem> m_highScoresItems = new List<HighScoreListItem>();

	private HighScoreListItem CurrentPlayerHighScoreItem
	{
		get
		{
			foreach (HighScoreListItem highScoresItem in m_highScoresItems)
			{
				if (highScoresItem.Id == GameData.facebook_userID)
				{
					return highScoresItem;
				}
			}
			return null;
		}
	}

	private void Start()
	{
		if (GameData.SelectedGameLocation == GameData.GAME_LOCATION.ClubHouse)
		{
			if (!PlayerPrefs.HasKey("leaderboardIntroView"))
			{
				DisplayView(isVisible: false);
				leaderboardIntroView.Init("Leaderboard", string.Empty, "Leaderboard score is all your mini-game high scores added together");
				leaderboardIntroView.DisplayView(isVisible: true);
				PlayerPrefs.SetInt("leaderboardIntroView", 1);
				leaderboardIntroView.Panel.GetTransition(UIPanelManager.SHOW_MODE.DismissForward).AddTransitionEndDelegate(delegate
				{
					DisplayView(isVisible: true);
				});
			}
			else
			{
				DisplayView(isVisible: true);
			}
			achievementsButton.gameObject.SetActiveRecursively(state: false);
		}
		else
		{
			DisplayView(isVisible: false);
		}
	}

	private void HandleAchievementsButtonPressed(ref POINTER_INFO ptr)
	{
		if (ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			if (Social.localUser.authenticated)
			{
				Social.ShowAchievementsUI();
			}
			else
			{
				GameCenterManager.AuthenciateUser();
			}
		}
	}

	public void FillLeaderBoard()
	{
		if (m_HighScoreList.Count != 0)
		{
			m_HighScoreList.ClearList(destroy: true);
		}
		int i = 0;
		for (int count = GameData.m_Leaders.Count; i < count; i++)
		{
			GameData.LeaderInfo leaderInfo = (GameData.LeaderInfo)GameData.m_Leaders[i];
			if (leaderInfo.m_Id.Equals(GameData.facebook_userID))
			{
				GameData.SetPosition(i + 1);
			}
			if (GameData.SelectedGameLocation == GameData.GAME_LOCATION.ClubHouse)
			{
				FetchAndAdd(i, leaderInfo);
			}
		}
	}

	private void FetchAndAdd(int i, GameData.LeaderInfo leaderInfo)
	{
		leaderInfo.m_GameObject = (GameObject)Object.Instantiate(m_PrefabLeaderBoardItem);
		HighScoreListItem component = leaderInfo.m_GameObject.GetComponent<HighScoreListItem>();
		if ((bool)component)
		{
			component.PlayerName = leaderInfo.m_Name;
			component.Score = leaderInfo.m_Score;
			component.Id = leaderInfo.m_Id;
			component.Position = i;
			m_HighScoreList.AddItem(leaderInfo.m_GameObject);
			m_highScoresItems.Add(component);
			if ((bool)CurrentPlayerHighScoreItem)
			{
				CurrentPlayerHighScoreItem.SetCurrentPlayer();
			}
			StartCoroutine(FetchFBPic(component.Id, component));
		}
	}

	private IEnumerator FetchFBPic(string id, HighScoreListItem listItem)
	{
		WWW www = new WWW("https://graph.facebook.com/" + id + "/picture?type=square");
		yield return www;
		if (www.error != null)
		{
			Debug.Log(www.error);
			yield break;
		}
		Texture2D texture = www.texture;
		GameData.Thumbnail thumbnail = new GameData.Thumbnail();
		thumbnail.SetValues(id, texture);
		GameData.AddThumbnail(thumbnail);
		if (id == GameData.facebook_userID)
		{
			GameData.m_MyPicture = www.texture;
		}
		listItem.ProcessPlayerImage();
	}

	public override void HandleCloseButtonPressed(ref POINTER_INFO ptr)
	{
		if (ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			TapjoyPlugin.HideDisplayAd();
			MonoSingleton<GameTypeMenuManager>.Instance.ReturnToMap();
		}
	}
}
