using UnityEngine;

public class SpecialFX : MonoBehaviour
{
	[SerializeField]
	private SFX _fireAudioFX;

	public static SpecialFX Instance;

	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
		}
	}

	private void OnEnable()
	{
		MinigameChallenge.BlastPhaseEnable += TurnOnScreenFire;
	}

	private void OnDisable()
	{
		MinigameChallenge.BlastPhaseEnable -= TurnOnScreenFire;
	}

	private void TurnOnScreenFire(bool bTurnOn)
	{
		if (bTurnOn)
		{
			AudioManager.PlaySFX(_fireAudioFX, 0f);
		}
	}
}
