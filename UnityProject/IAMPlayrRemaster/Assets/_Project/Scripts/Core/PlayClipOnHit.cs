using UnityEngine;

public class PlayClipOnHit : HitEvent
{
	public SFX onHitAudio;

	protected override void OnActionEnter(Collider other)
	{
		if (onHitAudio.audioClip != null)
		{
			AudioManager.PlaySFX(onHitAudio, 0f);
		}
	}
}
