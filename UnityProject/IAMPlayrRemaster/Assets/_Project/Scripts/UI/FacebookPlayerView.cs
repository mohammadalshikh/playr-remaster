using UnityEngine;

public class FacebookPlayerView : MonoBehaviour
{
	[SerializeField]
	private SpriteText _name;

	[SerializeField]
	private SpriteText _score;

	[SerializeField]
	private SpriteText _position;

	[SerializeField]
	private PackedSprite _thumbnail;

	[SerializeField]
	private PackedSprite _positionImage;

	[SerializeField]
	private int _moveByAmount = 4;

	private FacebookPlayer playerInfo;

	public bool IsTop3
	{
		get
		{
			if (playerInfo != null)
			{
				return playerInfo.Position < 4;
			}
			return false;
		}
	}

	public void Setup(FacebookPlayer player)
	{
		playerInfo = player;
		UpdateValues();
	}

	public void UpdateValues()
	{
		if (playerInfo != null)
		{
			SetText();
			SetImagePosition();
			SetThumbnail();
		}
	}

	private void SetText()
	{
		_name.Text = playerInfo.Name;
		_score.Text = playerInfo.Score.ToString("#,##0");
		_position.Text = (IsTop3 ? string.Empty : (string.Empty + playerInfo.Position));
	}

	private void SetImagePosition()
	{
		if (IsTop3)
		{
			_positionImage.SetFrame(0, playerInfo.Position - 1);
		}
		_positionImage.transform.GetComponent<Renderer>().enabled = IsTop3;
	}

	private void SetThumbnail()
	{
		Texture thumbnail = GameData.getThumbnail(playerInfo.PlayerId);
		if (thumbnail != null && thumbnail.width == 50 && thumbnail.height == 50)
		{
			_thumbnail.SetUVs(new Rect(0f, 0f, 1f, 1f));
			_thumbnail.SetTexture((Texture2D)thumbnail);
		}
	}

	public void AnimateUp(float delay)
	{
		iTween.MoveBy(base.gameObject, iTween.Hash("amount", new Vector3(0f, _moveByAmount, 0f), "time", 1f, "delay", delay));
	}

	public void AnimateDown(float delay)
	{
		iTween.MoveBy(base.gameObject, iTween.Hash("amount", new Vector3(0f, -_moveByAmount, 0f), "time", 1f, "delay", delay));
	}
}
