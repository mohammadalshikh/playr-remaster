using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
	public List<AudioSource> audioSources = new List<AudioSource>();

	public AudioCollection audioCollection = new AudioCollection();

	private static AudioManager m_Instance;

	private List<AudioSource> m_added_audio_sources = new List<AudioSource>();

	public static AudioManager Instance => m_Instance;

	private void Awake()
	{
		if (Instance != null)
		{
			Debug.LogWarning("only one AudioManager is allowed, destroying " + base.gameObject.name);
			Object.Destroy(base.gameObject);
		}
		else
		{
			m_Instance = this;
		}
	}

	public static AudioSource PlaySFX(AudioClip clip, Hashtable audioData)
	{
		if (clip == null)
		{
			return null;
		}
		AudioSource audioSource = GetAudioSource();
		bool flag = false;
		GameObject gameObject = null;
		if (audioSource == null)
		{
			gameObject = new GameObject("SoundFX: " + clip.name);
			gameObject.transform.position = ((!audioData.ContainsKey("point")) ? Vector3.zero : ((Vector3)audioData["point"]));
			if (Instance.transform != null)
			{
				gameObject.transform.parent = Instance.transform;
			}
			audioSource = gameObject.AddComponent<AudioSource>();
			Instance.m_added_audio_sources.Add(audioSource);
			flag = true;
		}
		audioSource.clip = clip;
		audioSource.volume = ((!audioData.ContainsKey("volume")) ? 1f : ((float)audioData["volume"]));
		audioSource.pitch = ((!audioData.ContainsKey("pitch")) ? 1f : ((float)audioData["pitch"]));
		audioSource.time = ((!audioData.ContainsKey("time")) ? 0f : ((float)audioData["time"]));
		audioSource.loop = audioData.ContainsKey("loop") && (bool)audioData["loop"];
		audioSource.Play();
		if (flag)
		{
			if (!audioSource.loop)
			{
				Instance.StartCoroutine(RemoveSourceWithDelay(gameObject, audioSource, clip.length));
			}
			return audioSource;
		}
		return audioSource;
	}

	public static AudioSource PlaySFX(SFX soundFX, float delay = 0f)
	{
		if (soundFX.audioClip == null)
		{
			return null;
		}
		AudioSource audioSource = GetAudioSource();
		bool flag = false;
		GameObject gameObject = null;
		if (audioSource == null)
		{
			gameObject = new GameObject("SoundFX: " + soundFX.audioClip.name);
			gameObject.transform.position = Vector3.zero;
			if (Instance.transform != null)
			{
				gameObject.transform.parent = Instance.transform;
			}
			audioSource = gameObject.AddComponent<AudioSource>();
			Instance.m_added_audio_sources.Add(audioSource);
			flag = true;
		}
		audioSource.clip = soundFX.audioClip;
		audioSource.volume = soundFX.GetVolume();
		audioSource.pitch = soundFX.pitch;
		audioSource.loop = soundFX.loop;
		ulong delay2 = (ulong)Mathf.RoundToInt(delay * 44100f);
		audioSource.Play(delay2);
		if (flag)
		{
			if (!audioSource.loop)
			{
				Instance.StartCoroutine(RemoveSourceWithDelay(gameObject, audioSource, soundFX.audioClip.length + delay));
			}
			return audioSource;
		}
		return audioSource;
	}

	public static void StopSound(SFX soundFX)
	{
		if (Instance == null)
		{
			return;
		}
		foreach (AudioSource audioSource in Instance.audioSources)
		{
			if (!(audioSource == null))
			{
				AudioSource component = audioSource.GetComponent<AudioSource>();
				if (component.clip == soundFX.audioClip && component != null)
				{
					component.volume = 0f;
					component.clip = null;
				}
			}
		}
		foreach (AudioSource added_audio_source in Instance.m_added_audio_sources)
		{
			if (!(added_audio_source == null))
			{
				AudioSource component2 = added_audio_source.GetComponent<AudioSource>();
				if (component2.clip == soundFX.audioClip && component2 != null)
				{
					component2.volume = 0f;
					Object.Destroy(component2.gameObject);
				}
			}
		}
	}

	public static void StopSound(AudioClip aClip)
	{
		foreach (AudioSource audioSource in Instance.audioSources)
		{
			AudioSource component = audioSource.GetComponent<AudioSource>();
			if (component.clip == aClip)
			{
				component.volume = 0f;
				component.clip = null;
			}
		}
		foreach (AudioSource added_audio_source in Instance.m_added_audio_sources)
		{
			AudioSource component2 = added_audio_source.GetComponent<AudioSource>();
			if (component2.clip == aClip)
			{
				component2.volume = 0f;
				Object.Destroy(component2.gameObject);
			}
		}
	}

	private static AudioSource GetAudioSource()
	{
		foreach (AudioSource audioSource in Instance.audioSources)
		{
			if (audioSource.isPlaying)
			{
				continue;
			}
			return audioSource;
		}
		return null;
	}

	private static IEnumerator RemoveSourceWithDelay(GameObject container, AudioSource audioSource, float delay)
	{
		yield return new WaitForSeconds(delay);
		Instance.m_added_audio_sources.Remove(audioSource);
		Object.Destroy(container);
	}

	public static void VolumeDownSound(SFX soundFX, float time = 1f)
	{
		Instance.StartCoroutine(Instance.VolumeDownRoutine(soundFX, time));
	}

	private IEnumerator VolumeDownRoutine(SFX soundFX, float time)
	{
		List<AudioSource> sources = FindSourceWithClip(soundFX.audioClip);
		foreach (AudioSource aSource in sources)
		{
			iTween.AudioTo(aSource.gameObject, iTween.Hash("volume", 0, "time", time));
		}
		yield return new WaitForSeconds(time);
		StopSound(soundFX);
	}

	private List<AudioSource> FindSourceWithClip(AudioClip clip, bool isPlaying = true)
	{
		List<AudioSource> list = new List<AudioSource>();
		foreach (AudioSource audioSource in Instance.audioSources)
		{
			if (audioSource.clip == clip && audioSource.isPlaying == isPlaying)
			{
				list.Add(audioSource);
			}
		}
		foreach (AudioSource added_audio_source in Instance.m_added_audio_sources)
		{
			if (added_audio_source.clip == clip && added_audio_source.isPlaying == isPlaying)
			{
				list.Add(added_audio_source);
			}
		}
		return list;
	}
}
