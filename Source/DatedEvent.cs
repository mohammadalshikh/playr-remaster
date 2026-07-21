using System;

public class DatedEvent
{
	private DateTime _date;

	public DateTime Date => _date;

	public void SetDate(int year, int month, int day, int hour = 12, int min = 0, int sec = 0)
	{
		_date = new DateTime(year, month, day, hour, min, sec);
	}
}
