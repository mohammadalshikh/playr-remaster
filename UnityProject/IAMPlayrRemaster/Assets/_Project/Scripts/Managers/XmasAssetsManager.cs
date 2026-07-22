using UnityEngine;

public class XmasAssetsManager : MonoBehaviour
{
	public SpawnParticles treeEmitterL;

	public SpawnParticles treeEmitterR;

	public static XmasAssetsManager Instance;

	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
		}
	}

	public void EmitSnowOnTrees()
	{
		treeEmitterL.EmitParticles();
		treeEmitterR.EmitParticles();
	}

	private void AddPoints(int points)
	{
		if (GameData.MinigameObject != null)
		{
			GameData.MinigameObject.CurrentMinigame.AddScore(points, ApplyStatModBonuses: true);
		}
	}

	public void SnowmanBonus()
	{
		AddPoints(250);
	}

	public void TreeBonus()
	{
		AddPoints(400);
	}
}
