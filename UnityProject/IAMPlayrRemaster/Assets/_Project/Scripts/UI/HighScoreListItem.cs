using UnityEngine;

public class HighScoreListItem : MonoBehaviour
{
	[SerializeField]
	private PackedSprite m_PlayerImage;

	[SerializeField]
	private PackedSprite positionImage;

	[SerializeField]
	private SpriteText positionText;

	[SerializeField]
	private SpriteText playerNameText;

	[SerializeField]
	private SpriteText playerScore;

	[SerializeField]
	private Button9Slice background;

	private string id;

	public string Id
	{
		get
		{
			return id;
		}
		set
		{
			id = value;
		}
	}

	public string PlayerName
	{
		get
		{
			return playerNameText.Text;
		}
		set
		{
			playerNameText.Text = value;
		}
	}

	public long Score
	{
		set
		{
			playerScore.Text = value.ToString("#,##0") + " PTS";
		}
	}

	public int Position
	{
		set
		{
			if (value < 3)
			{
				positionImage.SetFrame("Position", value);
				positionText.Text = string.Empty;
			}
			else
			{
				positionImage.SetSize(0f, 0f);
				positionText.Text = "#" + (value + 1);
			}
		}
	}

	public void ProcessPlayerImage()
	{
		m_PlayerImage.SetUVs(new Rect(0f, 0f, 1f, 1f));
		Texture thumbnail = GameData.getThumbnail(id);
		if (thumbnail != null && thumbnail.width == 50 && thumbnail.height == 50)
		{
			m_PlayerImage.SetTexture((Texture2D)thumbnail);
		}
	}

	public void SetCurrentPlayer()
	{
		background.controlIsEnabled = false;
	}
}
