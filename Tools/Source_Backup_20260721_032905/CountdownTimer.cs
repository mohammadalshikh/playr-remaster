using System;
using UnityEngine;

[Serializable]
public class CountdownTimer
{
	[SerializeField]
	private int hours;

	[SerializeField]
	private int minutes;

	[SerializeField]
	private int seconds;

	public int Hours => hours;

	public int Minutes => minutes;

	public int Seconds => seconds;

	public override string ToString()
	{
		return $"[CountdownTimer: Hours={Hours}, Minutes={Minutes}, Seconds={Seconds}]";
	}
}
