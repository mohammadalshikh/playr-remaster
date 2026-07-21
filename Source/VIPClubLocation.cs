using UnityEngine;

public class VIPClubLocation : MapLocation
{
	[SerializeField]
	private VIPPassView vipClubView;

	[SerializeField]
	private ParticleSystem starParticleSystem;

	protected override void HandleButtonPressed(ref POINTER_INFO ptr)
	{
		if (ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			vipClubView.DisplayView(isVisible: true);
			vipClubView.ToggleParticles(isActive: true);
		}
	}

	protected override void HandleIntroSceneCompleted()
	{
		base.HandleIntroSceneCompleted();
		starParticleSystem.Play();
	}

	public override void Start()
	{
		base.Start();
		if (GameData.Instance.HasIntroSeqOccured)
		{
			starParticleSystem.Play();
		}
	}
}
