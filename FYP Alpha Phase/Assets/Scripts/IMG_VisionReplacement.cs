using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class IMG_VisionReplacement : MonoBehaviour 
{
	private Camera visionCam;
	public Shader XRayShader;

	void Awake()
	{
		visionCam = GetComponent<Camera>();
	}

	void OnEnable()
	{
		visionCam.SetReplacementShader(XRayShader, "Vision");
	}

	void OnDisable()
	{
		visionCam.ResetReplacementShader();
	}
}
