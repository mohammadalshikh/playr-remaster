using System.Collections;
using System.Collections.Generic;
using Prime31;
using UnityEngine;

public class LeaderBoardManager : MonoSingleton<LeaderBoardManager>
{
	[SerializeField]
	private LeaderboardView leaderboardView;

	private void Awake()
	{
		MonoSingleton<LeaderBoardManager>.Instance = this;
	}

	private void Start()
	{
		if (FacebookHandler.IsSessionValid())
		{
			GetScoresFromFB();
		}
	}

	private void GetScoresFromFB()
	{
		Facebook.instance.graphRequest("me", HTTPVerb.GET, delegate(string error, object obj)
		{
			if (error == null)
			{
				Hashtable hashtable = (Hashtable)obj;
				if (hashtable.ContainsKey("id") && hashtable.ContainsKey("name"))
				{
					GameData.facebook_userID = hashtable["id"].ToString();
					GameData.m_PlayerName = hashtable["name"].ToString();
					Facebook.instance.getAppAccessToken(MonoSingleton<FacebookHandler>.Instance.AppId, MonoSingleton<FacebookHandler>.Instance.AppSecret, delegate
					{
						Facebook.instance.postScore(GameData.facebook_userID, GameData.PlayerStats.GlobalScore, delegate
						{
							GetScores();
						});
					});
				}
			}
		});
	}

	public void GetScores()
	{
		Facebook.instance.getAllScores(MonoSingleton<FacebookHandler>.Instance.AppId, GetScoresCompletionHandler);
	}

	private void GetScoresCompletionHandler(string error, object result)
	{
		if (error != null)
		{
			Debug.LogError(error);
		}
		else if (result != null)
		{
			GameData.Scores = JSON.JsonEncode(result);
			if (GameData.Scores != null)
			{
				FillLeaderInfo();
			}
		}
	}

	public void FillLeaderInfo()
	{
		if (GameData.m_Leaders.Count != 0)
		{
			GameData.m_Leaders.Clear();
		}
		Dictionary<string, object> dictionary = (Dictionary<string, object>)Json.Deserialize(GameData.Scores);
		List<object> list = (List<object>)dictionary["data"];
		Dictionary<string, object> dictionary2 = new Dictionary<string, object>();
		Dictionary<string, object> dictionary3 = new Dictionary<string, object>();
		int i = 0;
		for (int count = list.Count; i < count; i++)
		{
			dictionary2 = (Dictionary<string, object>)list[i];
			dictionary3 = (Dictionary<string, object>)dictionary2["user"];
			long score = (long)dictionary2["score"];
			string text = (string)dictionary3["name"];
			text = text.Substring(0, text.LastIndexOf(' '));
			int num = GameData.isPresentInLeaderBoard((string)dictionary3["id"]);
			if (num != -1)
			{
				GameData.LeaderInfo leaderInfo = (GameData.LeaderInfo)GameData.m_Leaders[i];
				leaderInfo.m_Score = (long)dictionary2["score"];
				leaderInfo.m_ThumbnailLoaded = false;
				GameData.m_Leaders.RemoveAt(i);
				GameData.m_Leaders.Insert(i, leaderInfo);
			}
			else
			{
				GameData.LeaderInfo leaderInfo2 = new GameData.LeaderInfo();
				leaderInfo2.m_Name = text;
				leaderInfo2.m_Id = (string)dictionary3["id"];
				leaderInfo2.m_Score = score;
				GameData.m_Leaders.Add(leaderInfo2);
			}
		}
		if (list.Count != 0)
		{
			leaderboardView.FillLeaderBoard();
		}
	}

	public IEnumerator FetchCurrentPlayerFBPic()
	{
		WWW www = new WWW("https://graph.facebook.com/" + GameData.facebook_userID + "/picture?type=square");
		yield return www;
		if (www.error != null)
		{
			Debug.Log(www.error);
		}
		else
		{
			GameData.m_MyPicture = www.texture;
		}
	}
}
