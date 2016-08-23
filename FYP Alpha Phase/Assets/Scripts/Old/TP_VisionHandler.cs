using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class TP_VisionHandler : MonoBehaviour 
{
	[Header("Vision Progress")]
	[Range(0f, 1f)]
	public float intensity = 0f;

	// For cameras
	//private Camera mCam;
	//private Camera vCam;

	// For normal vision
	private UnityStandardAssets.CinematicEffects.ScreenSpaceReflection reflection;

	// For electro vision
	private UnityStandardAssets.ImageEffects.VignetteAndChromaticAberration vignette;
	private UnityStandardAssets.CinematicEffects.Bloom bloom;

	void Awake()
	{
		//mCam = Camera.main;
	}

	void Start() 
	{
		
	}

	void Update() 
	{
		
	}
}
