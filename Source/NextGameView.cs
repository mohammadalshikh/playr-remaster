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
		coinIcon.renderer.enabled = false;
		timerText.renderer.enabled = false;
		nextGameText.Text = "Play Now";
	}

	public void PayToFastTrackAppointment(int coinsReq)
	{
		timerText.renderer.enabled = false;
		nextGameText.Text = "Play Now\n" + coinsReq;
		coinIcon.renderer.enabled = true;
		coinIcon.transform.localPosition = new Vector3(NextGameText.BottomRight.x * 0.16f, coinIcon.transform.localPosition.y, coinIcon.transform.localPosition.z);
	}

	public void ClockFormatting(int seconds)
	{
		coinIcon.renderer.enabled = false;
		nextGameText.Text = "NEXT GAME";
		timerText.renderer.enabled = true;
		timerText.Text = Appointment.FormatClock(seconds);
	}

	public void InsuffientCoins()
	{
		coinIcon.renderer.enabled = false;
		nextGameText.Text = "NEED MORE COINS";
	}
}
