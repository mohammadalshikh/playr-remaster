using UnityEngine;

public class FlyingTarget : MinigameTarget
{
	[SerializeField]
	private PackedSprite _targetSprite;

	public void SetupTexture(int frameIndex)
	{
		_targetSprite.SetFrame(_scoreType.ToString().ToLower(), frameIndex);
	}
}
