using System;
using System.Collections;
using UnityEngine;

public class Appointment : MonoBehaviour
{
	private const float carouselInterval = 3f;

	[SerializeField]
	private CountdownTimer timer;

	[SerializeField]
	private NextGameView nextGameView;

	private bool isAppointmentDue;

	private string timerId;

	private int cachedTimeInSecs;

	private int coinsReqToSkipToAppointment;

	private bool isCarouselSpinning;

	public NextGameView NextGameView
	{
		get
		{
			return nextGameView;
		}
		set
		{
			nextGameView = value;
		}
	}

	public bool IsAppointmentDue
	{
		get
		{
			return isAppointmentDue;
		}
		private set
		{
			isAppointmentDue = value;
			if (value)
			{
				CancelInvoke("TimerCarouselTransition");
				StartCoroutine(SetAppointmentDue(0.1f));
			}
			else
			{
				StartCountdown();
				InvokeRepeating("TimerCarouselTransition", 3f, 3f);
			}
		}
	}

	public string TimerId
	{
		get
		{
			return timerId;
		}
		set
		{
			timerId = value;
		}
	}

	public int CoinsReqToSkipToAppointment
	{
		get
		{
			return coinsReqToSkipToAppointment;
		}
		set
		{
			coinsReqToSkipToAppointment = value;
		}
	}

	public void Init()
	{
		Load();
	}

	public void StartCountdown()
	{
		if (!IsAppointmentDue)
		{
			StartCountdown(0.1f);
		}
	}

	public void StartCountdown(float delayTime)
	{
		if (!IsAppointmentDue)
		{
			StartCoroutine(StartCountdownDelay(delayTime));
			InvokeRepeating("Countdown", delayTime, 1f);
		}
	}

	private IEnumerator StartCountdownDelay(float delay)
	{
		yield return new WaitForSeconds(delay);
		nextGameView.DisplayView(isVisible: true);
		if (!IsAppointmentDue)
		{
			nextGameView.ClockFormatting(cachedTimeInSecs);
		}
	}

	public void StopCountdown()
	{
		CancelInvoke("Countdown");
	}

	public void StopCountdownForSecs(float delayTime)
	{
		StopCountdown();
		StartCoroutine(CountdownRefreshDelay(delayTime));
	}

	private IEnumerator CountdownRefreshDelay(float delayTime)
	{
		float delayForCancelInvoke = 1f;
		yield return new WaitForSeconds(delayForCancelInvoke);
		StartCountdown(delayTime - delayForCancelInvoke);
	}

	private void Countdown()
	{
		if (cachedTimeInSecs > 0 && !IsAppointmentDue)
		{
			cachedTimeInSecs--;
			if (isCarouselSpinning)
			{
				nextGameView.PayToFastTrackAppointment(coinsReqToSkipToAppointment);
			}
			else
			{
				nextGameView.ClockFormatting(cachedTimeInSecs);
			}
		}
		else
		{
			IsAppointmentDue = true;
			StopCountdown();
		}
	}

	public static string FormatClock(int timeInSecs)
	{
		int num = timeInSecs % 60;
		int num2 = timeInSecs / 60 % 60;
		int num3 = timeInSecs / 60 / 60;
		return $"{num3:00}:{num2:00}:{num:00}";
	}

	private void Load()
	{
		if (PlayerPrefs.HasKey(timerId + "secs"))
		{
			cachedTimeInSecs = PlayerPrefs.GetInt(timerId + "secs");
		}
		else
		{
			cachedTimeInSecs = timer.Hours * 60 * 60 + timer.Minutes * 60 + timer.Seconds;
		}
		if (PlayerPrefs.HasKey(timerId + "date"))
		{
			long dateData = Convert.ToInt64(PlayerPrefs.GetString(timerId + "date"));
			DateTime value = DateTime.FromBinary(dateData);
			cachedTimeInSecs -= (int)DateTime.Now.Subtract(value).TotalSeconds;
			if (cachedTimeInSecs <= 0)
			{
				IsAppointmentDue = true;
			}
			else
			{
				IsAppointmentDue = false;
			}
		}
		else
		{
			IsAppointmentDue = false;
		}
	}

	private IEnumerator SetAppointmentDue(float delay)
	{
		yield return new WaitForSeconds(delay);
		nextGameView.NextGameAvailable();
	}

	public void Save()
	{
		PlayerPrefs.SetInt(timerId + "secs", cachedTimeInSecs);
		PlayerPrefs.SetString(timerId + "date", DateTime.Now.ToBinary().ToString());
		PlayerPrefs.Save();
	}

	public void Reset()
	{
		PlayerPrefs.DeleteKey(timerId + "secs");
		PlayerPrefs.DeleteKey(timerId + "date");
		isAppointmentDue = false;
		PlayerPrefs.Save();
	}

	public static void ResetCurrentAppointment()
	{
		string text = "timerId" + PlayerManager.GetGameInfoForLocationAndType(GameData.SelectedGameLocation, GameData.SelectedGameType).Name;
		PlayerPrefs.DeleteKey(text + "secs");
		PlayerPrefs.DeleteKey(text + "date");
	}

	private void TimerCarouselTransition()
	{
		isCarouselSpinning ^= true;
	}

	public void FastTrackAppointmentForFree()
	{
		IsAppointmentDue = true;
		nextGameView.NextGameAvailable();
	}

	private void OnApplicationPause(bool isPaused)
	{
		if (isPaused)
		{
			Save();
			return;
		}
		CancelInvoke();
		StopAllCoroutines();
		Load();
	}
}
