using UnityEngine;

public class ThunderController : MonoBehaviour
{
	public SFX sound;

	public bool enable = true;

	private void Start()
	{
		EnableThunder(enable);
	}

	public void EnableThunder(bool bEnabled)
	{
		enable = bEnabled;
		if (sound.audioClip != null)
		{
			if (bEnabled)
			{
				AudioManager.PlaySFX(sound, 0f);
			}
			else
			{
				AudioManager.VolumeDownSound(sound, 2f);
			}
		}
	}
}
