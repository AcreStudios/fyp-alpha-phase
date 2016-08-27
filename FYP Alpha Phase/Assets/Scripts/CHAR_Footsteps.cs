using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(AudioSource))]
public class CHAR_Footsteps : MonoBehaviour 
{
	// Components
	private Transform trans;

	[Header("-Ground Settings-")]
	public GroundTextureType[] groundTypes;

	[Header("-Sounds Settings-")]
	public bool randomizePitch = true;
	[Range(0.1f, 1f)]
	public float minPitch = 1f;
	[Range(0.1f, 2f)]
	public float maxPitch = 1.2f;
	public AudioSource aSrc;

	private void Awake()
	{
		aSrc = GetComponent<AudioSource>();
		trans = GetComponent<Transform>();
	}

	private void PlayFootsteps()
	{
		RaycastHit hit;
		Vector3 start = trans.position + trans.up;
		Vector3 dir = Vector3.down;

		if(Physics.Raycast(start, dir, out hit, 1.3f))
		{
			MeshRenderer mRend = hit.collider.GetComponent<MeshRenderer>();
			if(mRend)
				PlayMeshSound(mRend);
		}
	}

	private void PlayMeshSound(MeshRenderer rend)
	{
		#region Debug errors
		if(!aSrc)
		{
			Debug.LogError("PlayMeshSound() - We have no audio source to play the sound from!");
			return;
		}

		if(!SND_Manager.instance)
		{
			Debug.LogError("PlayMeshSound() - We have no sound manager!");
			return;
		}
		#endregion

		if(groundTypes.Length > 0)
		{
			foreach(GroundTextureType gTypes in groundTypes)
			{
				if(gTypes.footstepSounds.Length < 1)
					return;

				foreach(Material mat in gTypes.mats)
				{
					if(rend.material.mainTexture == mat.mainTexture)
					{
						SND_Manager.instance.PlaySound(aSrc,
							gTypes.footstepSounds[Random.Range(0, gTypes.footstepSounds.Length)],
							randomizePitch,
							minPitch,
							maxPitch);
					}
				}
			}
		}
	}
}

[System.Serializable]
public class GroundTextureType
{
	public string groundName;
	public Material[] mats;
	public AudioClip[] footstepSounds;
}
