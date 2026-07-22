using System;

[Serializable]
public class AudioCollection
{
	public MinigameWallAudio minigameWall = new MinigameWallAudio();

	public MinigameAudio CurrentGame
	{
		get
		{
			if (GameData.MinigameObject.CurrentMinigame.Type == MINIGAME_TYPE.WALL)
			{
				return minigameWall;
			}
			return minigameWall;
		}
	}
}
