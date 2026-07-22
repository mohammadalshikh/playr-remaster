using UnityEngine;

public class NextGameView : View
{
	[SerializeField]
	private SpriteText nextGameText;

	[SerializeField]
	private SpriteText timerText;

	[SerializeField]
	private PackedSprite coinIcon;

	public SpriteText NextGameText => nextGameText;

	public SpriteText TimerText => timerText;

	public void NextGameAvailable()
	{
		coinIcon.GetComponent<Renderer>().enabled = false;
		timerText.GetComponent<Renderer>().enabled = false;
		nextGameText.Text = "Play Now";
	}

	public void PayToFastTrackAppointment(int coinsReq)
	{
		timerText.GetComponent<Renderer>().enabled = false;
		nextGameText.Text = "Play Now\n" + coinsReq;
		coinIcon.GetComponent<Renderer>().enabled = true;
		coinIcon.transform.localPosition = new Vector3(NextGameText.BottomRight.x * 0.16f, coinIcon.transform.localPosition.y, coinIcon.transform.localPosition.z);
	}

	public void ClockFormatting(int seconds)
	{
		coinIcon.GetComponent<Renderer>().enabled = false;
		nextGameText.Text = "NEXT GAME";
		timerText.GetComponent<Renderer>().enabled = true;
		timerText.Text = Appointment.FormatClock(seconds);
	}

	public void InsuffientCoins()
	{
		coinIcon.GetComponent<Renderer>().enabled = false;
		nextGameText.Text = "NEED MORE COINS";
	}
}
