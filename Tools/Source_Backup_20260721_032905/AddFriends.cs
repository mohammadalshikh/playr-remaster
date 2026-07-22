using UnityEngine;

public class AddFriends : MonoBehaviour
{
	public int friendsrequestsent;

	private string playerprefs_key = "RequestSentCounter";

	private void Awake()
	{
		if (PlayerPrefs.HasKey(playerprefs_key))
		{
			friendsrequestsent = PlayerPrefs.GetInt(playerprefs_key);
		}
	}

	private void AddFriendsPanelCalled()
	{
	}

	public void SuccessAddedFriends(string url)
	{
		if (url.Contains("request"))
		{
			Debug.Log(url);
			friendsrequestsent++;
			PlayerPrefs.SetInt(playerprefs_key, friendsrequestsent);
			PlayerPrefs.Save();
			PlayerManager.SendFeedback("Sent Add friends Facebook request", string.Empty, string.Empty);
		}
	}

	private void AddFacebookFriends()
	{
	}

	private void AddSMSFriends()
	{
	}

	private void AddMailFriends()
	{
	}
}
