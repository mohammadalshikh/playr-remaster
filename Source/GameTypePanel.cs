using UnityEngine;

public class GameTypePanel : MonoBehaviour
{
	[SerializeField]
	private SpriteText gameTypeTitle;

	[SerializeField]
	private GameTypeMenuView[] gameTypeMenuViews;

	[SerializeField]
	private UIPanel gameTypePanel;

	private void Awake()
	{
		if (GameData.SelectedGameLocation == GameData.GAME_LOCATION.TrainingCenter || GameData.SelectedGameLocation == GameData.GAME_LOCATION.PassingDrills || GameData.SelectedGameLocation == GameData.GAME_LOCATION.ShootingDrills)
		{
			float width = gameTypeMenuViews[0].width;
			float num = 0.5f;
			float newlyUnlockedDelayTime = (float)gameTypeMenuViews.Length * num + 1f;
			int i = 0;
			for (int num2 = gameTypeMenuViews.Length; i < num2; i++)
			{
				gameTypeMenuViews[i].NewlyUnlockedDelayTime = newlyUnlockedDelayTime;
				iTween.MoveFrom(gameTypeMenuViews[i].gameObject, iTween.Hash("position", gameTypeMenuViews[i].gameObject.transform.position + new Vector3((0f - width) * 2f, 0f, 0f), "time", 0.25f, "delay", (float)i * num));
			}
		}
		if ((bool)gameTypeTitle)
		{
			gameTypeTitle.Text = GameData.SelectedGameLocationFormatted;
		}
		if (GameData.SelectedGameLocation == GameData.GAME_LOCATION.ClubHouse)
		{
			base.transform.Translate(new Vector3(-200f, 0f, 0f));
		}
	}

	public void SaveAppointments()
	{
		GameTypeMenuView[] array = gameTypeMenuViews;
		foreach (GameTypeMenuView gameTypeMenuView in array)
		{
			gameTypeMenuView.Appointment.Save();
		}
	}

	public void ProcessGameLocationsWithApptsDue()
	{
		GameTypeMenuView[] array = gameTypeMenuViews;
		foreach (GameTypeMenuView gameTypeMenuView in array)
		{
			if (gameTypeMenuView.Appointment.IsAppointmentDue)
			{
				GameData.Instance.AddGameLocationHasApptDue(GameData.SelectedGameLocation, hasApptDue: true);
			}
			else
			{
				GameData.Instance.RemoveGameLocationHasApptDue(GameData.SelectedGameLocation);
			}
		}
	}
}
