using UnityEngine;

public class Shooting3DText : Minigame3DText
{
	public void ShowResult(float accuracy, int score)
	{
		int num = Mathf.RoundToInt(accuracy * 100f);
		SetAccuracyColor(accuracy);
		Show("Accuracy " + num + "%");
		Glow();
		Punch();
	}

	public void ShowKnockoutResult(float shotAccuracy, float averageAccuracy)
	{
		int num = Mathf.RoundToInt(shotAccuracy * 100f);
		int num2 = Mathf.RoundToInt(averageAccuracy * 100f);
		SetAccuracyColor(averageAccuracy);
		Show("+" + num + "% = " + num2 + "%");
		Glow();
	}

	public void MissedShot(string message)
	{
		SetColor(Color.red);
		Show(message);
		Glow();
	}

	private void SetAccuracyColor(float accuracy)
	{
		if (accuracy == 1f)
		{
			SetColor(Color.yellow);
		}
		else if (accuracy > 0.895f)
		{
			SetColor(Color.green);
		}
		else
		{
			SetColor(new Color(Mathf.Clamp01(1.5f - accuracy), accuracy, 0f, 1f));
		}
	}
}
