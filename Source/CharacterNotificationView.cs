using UnityEngine;

public class CharacterNotificationView : NotificationView
{
	public enum CharacterId
	{
		Coach,
		Manager
	}

	[SerializeField]
	private PackedSprite characterSprite;

	public void Init(string topBarTitle, string headerTitle, string bodyText, CharacterId characterId)
	{
		Init(topBarTitle, headerTitle, bodyText);
		characterSprite.SetFrame(0, (int)characterId);
	}
}
