using System;
using System.Collections.Generic;
using UnityEngine;

public class DatedEventsController : MonoBehaviour
{
	private List<DatedEvent> datedEvents = new List<DatedEvent>();

	private int[][] dates = new int[1][] { new int[6] { 2013, 1, 6, 12, 0, 1 } };

	private int[] GetInfoForEventAtIndex(int index)
	{
		return dates[index];
	}

	private DateTime GetDateForEvent(DATED_EVENTS selectedEvent)
	{
		return datedEvents[(int)selectedEvent].Date;
	}

	private void Start()
	{
		InitEvents();
		PlayerManager.checkDatedEvent += FireEvents;
	}

	public void FireEvents(DateTime date)
	{
		EndPromotion(date);
	}

	private void InitEvents()
	{
		for (int i = 0; i < dates.Length; i++)
		{
			DatedEvent datedEvent = new DatedEvent();
			int[] infoForEventAtIndex = GetInfoForEventAtIndex(i);
			datedEvent.SetDate(infoForEventAtIndex[0], infoForEventAtIndex[1], infoForEventAtIndex[2], infoForEventAtIndex[3], infoForEventAtIndex[4], infoForEventAtIndex[5]);
			datedEvents.Add(datedEvent);
		}
	}

	private void EndPromotion(DateTime dateTime)
	{
		if (!(dateTime.Date <= GetDateForEvent(DATED_EVENTS.CHRISTMAS)))
		{
			Debug.Log("Christmas Expired");
		}
	}
}
