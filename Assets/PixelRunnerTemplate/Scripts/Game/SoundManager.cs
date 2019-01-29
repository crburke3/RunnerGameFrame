using UnityEngine;
using System.Collections;

// A simple class for handling playing sound effects and music

public class SoundManager : MonoBehaviour 
{
	public AudioClip[] sounds;					// list of available sound clips
	public AudioClip[] music;					// list of available music tracks

	private static SoundManager soundMan;		// global SoundManager instance
	private AudioSource sfxAudio;				// AudioSource component for playing sound fx.
	private AudioSource musicAudio;				// AudioSource component for playing music

	void Awake()
	{
		if(soundMan != null)
		{
			Debug.LogError("More than one SoundManager found in the scene");
			return;
		}

		soundMan = this;
		sfxAudio = gameObject.AddComponent<AudioSource>();
		musicAudio = gameObject.AddComponent<AudioSource>();

		sfxAudio.playOnAwake = false;
		musicAudio.playOnAwake = false;
		musicAudio.loop = true;
	}

	public static void PlaySfx(string sfxName)
	{
		if(soundMan == null)
		{
			Debug.LogWarning("Attempt to play a sound with no SoundManager in the scene");
			return;
		}

		soundMan.PlaySound(sfxName, soundMan.sounds, soundMan.sfxAudio);
	}

	public static void PlayMusic(string trackName)
	{
		if(soundMan == null)
		{
			Debug.LogWarning("Attempt to play a sound with no SoundManager in the scene");
			return;
		}

		// reset track to beginning
		soundMan.musicAudio.time = 0.0f;
		soundMan.musicAudio.volume = 1.0f;

		soundMan.PlaySound(trackName, soundMan.music, soundMan.musicAudio);
	}

	public static void PauseMusic(float fadeTime)
	{
		if(fadeTime > 0.0f)
			soundMan.StartCoroutine(soundMan.FadeMusicOut(fadeTime));
		else
			soundMan.musicAudio.Pause();
	}

	public static void UnpauseMusic()
	{
		soundMan.musicAudio.volume = 1.0f;
		soundMan.musicAudio.Play();
	}

	private void PlaySound(string soundName, AudioClip[] pool, AudioSource audioOut)
	{
		// loop through our list of clips until we find the right one.
		foreach(AudioClip clip in pool)
		{
			if(clip.name == soundName)
			{
				// play the clip
				audioOut.clip = clip;
				audioOut.Play();
				return;
			}
		}

		Debug.LogWarning("No sound clip found with name " + soundName);
	}

	IEnumerator FadeMusicOut(float time)
	{
		float startVol = musicAudio.volume;
		float startTime = Time.realtimeSinceStartup;

		while(true)
		{
			// use realtimeSinceStartup because Time.time doesn't increase when the game is paused.
			float t = (Time.realtimeSinceStartup - startTime) / time;
			if(t < 1.0f)
			{
				musicAudio.volume = (1.0f - t) * startVol;
				yield return 0;
			}
			else
			{
				break;
			}
		}

		// once we've fully faded out, pause the track
		musicAudio.Pause();
	}
}
