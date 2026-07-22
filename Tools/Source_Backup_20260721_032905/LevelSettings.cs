using System;

[Serializable]
public class LevelSettings
{
	public float distance;

	public float positionRange;

	public PLAYER_STEPS stepsBackwards;

	public bool resetDirection;

	public int rounds;

	public int roundTargets;

	public int successHits;

	public string roundInformation = string.Empty;
}
