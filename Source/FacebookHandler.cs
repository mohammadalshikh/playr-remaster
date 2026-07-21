using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Prime31;
using UnityEngine;

public class FacebookHandler : MonoSingleton<FacebookHandler>
{
	public delegate void HandleFacebookLoggedOut();

	private const string m_AppId = "155727011143809";

	[SerializeField]
	private RIJNDAEL_encryption encryptionTool;

	private byte[] appS1;

	public string AppId => "155727011143809";

	public string AppSecret => encryptionTool.byte_array_to_string(encryptionTool.decrypt(appS1));

	public bool HasLoggedIntoFBBefore => PlayerPrefs.HasKey("HasLoggedIntoFBBefore");

	[method: MethodImpl(32)]
	public static event HandleFacebookLoggedOut FacebookLoggedOut;

	private void Awake()
	{
		MonoSingleton<FacebookHandler>.Instance = this;
		Object.DontDestroyOnLoad(this);
	}

	private void HandleEncryptionToolInitialised()
	{
		string text = "d6b2223a428a6b";
		string text2 = "549cb9693215e9343f";
		appS1 = encryptionTool.encrypt(encryptionTool.string_to_byte_array(text + text2), random_iv: true);
		encryptionTool.EncryptionToolInitialised -= HandleEncryptionToolInitialised;
		InitFacebook();
	}

	private void InitFacebook()
	{
		FacebookAndroid.init();
		if (IsSessionValid())
		{
			PlayerPrefs.SetInt("HasLoggedIntoFBBefore", 1);
			StartCoroutine(SetMyName());
		}
	}

	private void HandleSessionOpenedEvent()
	{
		PlayerPrefs.SetInt("HasLoggedIntoFBBefore", 1);
		StartCoroutine(SetMyName());
	}

	private void OnEnable()
	{
		encryptionTool.EncryptionToolInitialised += HandleEncryptionToolInitialised;
		FacebookManager.sessionOpenedEvent += HandleSessionOpenedEvent;
		PlayerManager.HighscoreChanged += HandleHighscoreChanged;
	}

	private void OnDisable()
	{
		PlayerManager.HighscoreChanged -= HandleHighscoreChanged;
		FacebookManager.sessionOpenedEvent -= HandleSessionOpenedEvent;
	}

	public void LoginFacebook()
	{
		if (MonoSingleton<FacebookHandler>.IsInitialized && !IsSessionValid() && Application.internetReachability != 0)
		{
			string[] permissions = new string[1] { "email" };
			FacebookAndroid.loginWithReadPermissions(permissions);
		}
	}

	public void LogoutFacebook()
	{
		FacebookAndroid.logout();
		if (FacebookHandler.FacebookLoggedOut != null)
		{
			FacebookHandler.FacebookLoggedOut();
		}
	}

	public static bool IsSessionValid()
	{
		return FacebookAndroid.isSessionValid();
	}

	public static void AddFriends()
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("message", "I'm playing Soccer Playr on iOS. Join me and see if you can beat my score");
		dictionary.Add("title", "Invite Friends");
		dictionary.Add("filters", "['app_non_users']");
		Dictionary<string, string> parameters = dictionary;
		FacebookAndroid.showDialog("apprequests", parameters);
	}

	public static void SendMessage(string id, string message, string title, string data)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("to", id);
		dictionary.Add("message", message);
		dictionary.Add("data", data);
		dictionary.Add("title", title);
		Dictionary<string, string> parameters = dictionary;
		FacebookAndroid.showDialog("apprequests", parameters);
	}

	private void PostFriendsBeatenHandler(string error, object result)
	{
	}

	public IEnumerator SetMyName()
	{
		if (GameData.m_PlayerName.Length > 0)
		{
			yield return null;
		}
		bool bGotName = false;
		Facebook.instance.graphRequest("me", HTTPVerb.GET, delegate(string error1, object obj)
		{
			bGotName = true;
			if (error1 == null)
			{
				Hashtable hashtable = obj as Hashtable;
				GameData.facebook_userID = hashtable["id"].ToString();
				GameData.m_PlayerName = hashtable["name"].ToString();
			}
		});
		int tries = 0;
		while (!bGotName && tries < 10)
		{
			tries++;
			yield return new WaitForSeconds(0.5f);
		}
		yield return null;
	}

	public IEnumerator PostFriendsBeaten()
	{
		if (IsSessionValid())
		{
			yield return StartCoroutine(SetMyName());
			Facebook.instance.postMessageWithLinkAndLinkToImage(GameData.m_PlayerName + " has just outscored one of their friends in Soccer Playr. Can you beat them?", "http://www.facebook.com/appcenter/iamplayr", "Friend beaten", "https://sites.google.com/a/werinteractive.com/mobile/resources/images/soccerplayr.png", "Soccer Playr", PostFriendsBeatenHandler);
		}
		yield return null;
	}

	private void PostAchievementHandler(string error, object result)
	{
	}

	public IEnumerator PostAchievement(string achievement, string title, string imageurlname)
	{
		if (IsSessionValid())
		{
			yield return StartCoroutine(SetMyName());
			achievement = achievement.Replace("<player>", GameData.m_PlayerName);
			imageurlname = imageurlname.ToLower();
			Dictionary<string, object> achievementParams = new Dictionary<string, object>
			{
				{ "type", "iamplayr:medal" },
				{
					"url",
					"http://www.facebook.com/appcenter/iamplayr/objects/medals/" + title.Replace(" ", string.Empty)
				},
				{ "title", title },
				{
					"image",
					"https://sites.google.com/a/werinteractive.com/mobile/resources/images/" + title.Replace(" ", string.Empty) + ".png"
				}
			};
			Facebook.instance.graphRequest("me/iamplayr:earn", HTTPVerb.POST, achievementParams, HandlePostAchievement);
		}
		yield return null;
	}

	private void HandlePostAchievement(string feedbackMessage, object result)
	{
		MonoBehaviour.print("feedbackMessage = " + feedbackMessage + " Result  =" + result);
	}

	private void PostNewHighScoreHandler(string error, object result)
	{
	}

	public IEnumerator PostNewHighScore()
	{
		if (IsSessionValid())
		{
			yield return StartCoroutine(SetMyName());
			string highscoretext = GameData.m_PlayerName + " just scored " + GameData.PlayerStats.GlobalScore.ToString("#,##0") + " in Soccer Playr. Can you beat it?";
			Facebook.instance.postMessageWithLinkAndLinkToImage(highscoretext, "http://www.facebook.com/appcenter/iamplayr", "New Highscore", "https://sites.google.com/a/werinteractive.com/mobile/resources/images/soccerplayr.png", "Soccer Playr", PostNewHighScoreHandler);
			Facebook.instance.postMessage(highscoretext, PostNewHighScoreHandler);
		}
		yield return null;
	}

	private void HandleHighscoreChanged(int newHighscore)
	{
		if (Application.internetReachability == NetworkReachability.NotReachable || !IsSessionValid() || Facebook.instance == null)
		{
			return;
		}
		Facebook.instance.postScore(GameData.facebook_userID, newHighscore, delegate(bool didSucceed)
		{
			if (didSucceed)
			{
				Facebook.instance.getAllScores(AppId, getScoresCompletionHandler);
			}
		});
	}

	private void getScoresCompletionHandler(string error, object result)
	{
		if (error != null)
		{
			Debug.LogError(error);
			return;
		}
		GameData.Scores = JSON.JsonEncode(result);
		FillLeaderInfo();
		for (int i = 0; i < GameData.m_Leaders.Count; i++)
		{
			GameData.LeaderInfo leaderInfo = (GameData.LeaderInfo)GameData.m_Leaders[i];
			if (leaderInfo.m_Id.Equals(GameData.facebook_userID))
			{
				GameData.SetPosition(i + 1);
			}
		}
	}

	private void FillLeaderInfo()
	{
		GameData.m_Leaders.Clear();
		Dictionary<string, object> dictionary = (Dictionary<string, object>)Json.Deserialize(GameData.Scores);
		List<object> list = (List<object>)dictionary["data"];
		for (int i = 0; i < list.Count; i++)
		{
			Dictionary<string, object> dictionary2 = (Dictionary<string, object>)list[i];
			Dictionary<string, object> dictionary3 = (Dictionary<string, object>)dictionary2["user"];
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
	}
}
