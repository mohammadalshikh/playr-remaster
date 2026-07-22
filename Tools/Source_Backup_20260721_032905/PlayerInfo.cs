using UnityEngine;

public class PlayerInfo : MonoBehaviour
{
	private string m_nId;

	private int m_nPosition;

	private string m_sName;

	public SpriteText m_PlayerName;

	public SpriteText m_PlayerScore;

	public SpriteText m_PlayerPosition;

	public PackedSprite m_PlayerPositionImage;

	public PackedSprite m_PlayerThumbnail;

	private bool m_bThumbnailSet;

	public void SetInfo(string id, int position, string name, int score)
	{
		m_nId = id;
		m_PlayerName.Text = name;
		m_PlayerScore.Text = score.ToString("#,##0");
		m_nPosition = position;
		if (position < 4)
		{
			m_PlayerPosition.Hide(tf: true);
			m_PlayerPositionImage.Hide(tf: false);
			m_PlayerPositionImage.SetFrame(0, m_nPosition - 1);
		}
		else
		{
			m_PlayerPosition.Hide(tf: false);
			m_PlayerPosition.Text = "#" + m_nPosition;
			m_PlayerPositionImage.Hide(tf: true);
		}
	}

	private void Update()
	{
		if (m_bThumbnailSet)
		{
			return;
		}
		Texture thumbnail = GameData.getThumbnail(m_nId);
		if (thumbnail != null)
		{
			if (thumbnail.width == 50 && thumbnail.height == 50)
			{
				m_PlayerThumbnail.SetUVs(new Rect(0f, 0f, 1f, 1f));
				m_PlayerThumbnail.SetTexture((Texture2D)thumbnail);
			}
			m_bThumbnailSet = true;
		}
	}
}
