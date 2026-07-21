using System;
using UnityEngine;

[Serializable]
public class SFX
{
	public AudioClip audioClip;

	public SOUND_TYPE type;

	public float volume = 1f;

	public float pitch = 1f;

	public bool loop;

	public float GetVolume()
	{
		float num = ((type != 0) ? GameData.PlayerStats.MusicGlobalVolume : GameData.PlayerStats.SoundFXGlobalVolume);
		return num * volume;
	}
}
