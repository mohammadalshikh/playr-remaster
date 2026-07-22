using System;

[Serializable]
public class WallTargetSettings
{
	public TARGET_TYPE id;

	public ScoreSettings scoreSettings = new ScoreSettings();

	public Life life = new Life();

	public Activity activity = new Activity();
}
