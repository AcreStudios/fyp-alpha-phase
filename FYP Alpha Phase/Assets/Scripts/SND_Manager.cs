using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SND_Manager : MonoBehaviour 
{
	// Singleton
	public static SND_Manager instance;

	private void Awake()
	{
		// Get instance
		if(!instance)
			instance = this;
		else
		{
			if(instance != this)
				Destroy(gameObject);
		}
	}

	public void PlaySound(AudioSource aSrc, AudioClip aClip, bool randomPitch = false, float minRandomPitch = 1, float maxRandomPitch = 1)
	{
		// Plays sound globally (Non-location specific)
		if(randomPitch)
			aSrc.pitch = Random.Range(minRandomPitch, maxRandomPitch);

		aSrc.clip = aClip;
		aSrc.Play();
	}

	public void PlaySoundOnce(Vector3 pos, AudioClip aClip, float duration = 2f, bool randomPitch = false, float minRandomPitch = 1, float maxRandomPitch = 1)
	{
		// Creates a 3D sound at target location
		GameObject os = new GameObject("OneShotAudio: " + aClip.name);
		os.transform.position = pos;
		AudioSource a = os.AddComponent<AudioSource>();
		a.spatialBlend = 1f;
		a.clip = aClip;
		a.Play();

		// Destroy sound after specific duration
		Destroy(os, duration);
	}
}
