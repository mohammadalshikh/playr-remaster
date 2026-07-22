using System;
using System.Collections;
using UnityEngine;

public class FBGiftsManager : MonoBehaviour
{
	public static FBGiftsManager Instance;

	public static int TotalMinutes = 60;

	public ArrayList m_closedList = new ArrayList();

	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
		}
	}

	public TimeSpan GetTimePassed(string id)
	{
		string @string = PlayerPrefs.GetString(id, string.Empty);
		DateTime dateTime = DateTime.Parse(@string);
		return DateTime.Now - dateTime;
	}

	public bool SendGift(string id)
	{
		if (m_closedList.Contains(id))
		{
			return false;
		}
		FacebookHandler.SendMessage(id, "Hey, I've sent you some energy.", "Send gift", "GIFT");
		PlayerManager.SendFeedback("Energy sent", string.Empty, string.Empty);
		m_closedList.Add(id);
		PlayerPrefs.SetString(id, DateTime.Now.ToString());
		PlayerPrefs.Save();
		return true;
	}

	public bool isClosed(string id)
	{
		return m_closedList.Contains(id);
	}

	public void ClearList()
	{
		m_closedList.Clear();
	}

	public void AddId(string id)
	{
		m_closedList.Add(id);
	}

	public void CheckAndRemove()
	{
		for (int i = 0; i < m_closedList.Count; i++)
		{
			string text = (string)m_closedList[i];
			string @string = PlayerPrefs.GetString(text, string.Empty);
			if (@string.Length > 0)
			{
				if (GetTimePassed(text).TotalMinutes >= (double)TotalMinutes)
				{
					m_closedList.RemoveAt(i);
					PlayerPrefs.SetString(text, string.Empty);
					i = 0;
				}
			}
			else
			{
				m_closedList.RemoveAt(i);
				i = 0;
			}
		}
	}

	private void Update()
	{
		CheckAndRemove();
	}
}
