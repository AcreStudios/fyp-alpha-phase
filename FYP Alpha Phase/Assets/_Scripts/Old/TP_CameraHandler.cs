using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TP_CameraHandler : MonoBehaviour 
{
	// Cache
	private CAM_CameraLook camProperties;
	[HideInInspector]
	public Transform camPivot, camTrans;
	private Camera cam;

	//private CAM_CrosshairManager chManager;
	private CAM_CameraShake camShake;
	private TP_InputsHandler inputs;
	private TP_StatesHandler states;

	[Header("Camera FOVs")]
	public float normalFOV = 60f;
	public float aimingFOV = 40f;
	public float switchFOVSpeed = 5f;
	private float targetFOV, currentFOV;

	[Header("Camera distances")]
	public float normalZ = -2f;
	public float aimingZ = -.86f;
	public float switchPosSpeed = 15f;
	private float targetZ, actualZ, currentZ;
	LayerMask layerMask;

	[Header("Recoil")]
	public float shakeRecoil = .5f;
	public float shakeMovement = .3f;
	public float shakeMin = .1f;
	public float addRecoilFOV = 5f;
	public float switchShakeSpeed = 10f;
	private float targetShake, currentShake;

	void Awake()
	{
		inputs = GetComponent<TP_InputsHandler>();
		states = GetComponent<TP_StatesHandler>();

		cam = Camera.main;
	}

	void Start() 
	{
		camProperties = CAM_CameraLook.GetInstance();
		//chManager = CAM_CrosshairManager.GetInstance();
		camPivot = camProperties.transform.GetChild(0);
		camTrans = camPivot.GetChild(0);
		camShake = camTrans.GetChild(0).GetComponent<CAM_CameraShake>();

		layerMask = ~(1 << gameObject.layer); // Everything, except player lol
		states.layerMask = layerMask;
	}

	void FixedUpdate() 
	{
		HandleCameraFOV();
		HandleCameraShake();
		HandleCameraPosition();
		HandleCameraCollision(layerMask);
	}

	void HandleCameraFOV()
	{
		if(states.isAiming)
		{
			targetZ = aimingZ;
			targetFOV = aimingFOV;

			if(inputs.LMB && !states.isReloading)
				states.isShooting = true;
			else
				states.isShooting = false;
		}
		else
		{
			states.isShooting = false;

			targetZ = normalZ;
			targetFOV = normalFOV;
		}

		// Update FOV
		currentFOV = Mathf.Lerp(currentFOV, targetFOV, Time.deltaTime * switchFOVSpeed);
		cam.fieldOfView = currentFOV;
	}

	void HandleCameraShake()
	{
		if(states.isShooting && states.shootingHandler.currentBullets > 0)
		{
			targetShake = shakeRecoil;
			camProperties.WiggleCrosshairAndCamera(.2f);
			targetFOV += addRecoilFOV;
		}
		else
		{
			if(inputs.vertical != 0f || inputs.horizontal != 0f)
				targetShake = shakeMovement;
			else
				targetShake = shakeMin;
		}

		// Update camera shake
		currentShake = Mathf.Lerp(currentShake, targetShake, Time.deltaTime * switchShakeSpeed);
		camShake.shakeSpeed = currentShake;
	}

	void HandleCameraPosition()
	{
		// Find where the camera is looking
		Ray ray = new Ray(camTrans.position, camTrans.forward);
		states.lookPosition = ray.GetPoint(20f);
		RaycastHit hit;

		Debug.DrawRay(ray.origin, ray.direction, Color.red);
		if(Physics.Raycast(ray.origin, ray.direction, out hit, 100f, layerMask))
			states.lookHitPosition = hit.point;
		else
			states.lookHitPosition = states.lookPosition;
		
		// Update camera position
		currentZ = Mathf.Lerp(currentZ, actualZ, Time.deltaTime * switchPosSpeed);
		camTrans.localPosition = new Vector3(0f, 0f, currentZ);
	}

	void HandleCameraCollision(LayerMask mask)
	{
		// Do a raycast from the pivot of the camera to the camera
		Vector3 origin = camPivot.TransformPoint(Vector3.zero);
		Vector3 direction = camTrans.TransformPoint(Vector3.zero) - origin;
		RaycastHit hit;

		// The distance of raycast is determined by if we are aiming or not
		actualZ = targetZ;

		Debug.DrawRay(origin, direction, Color.blue);
		// If obstacle, find distance
		if(Physics.Raycast(origin, direction, out hit, Mathf.Abs(targetZ), mask))
		{
			float dist = Vector3.Distance(camPivot.position, hit.point);
			actualZ = -dist; // The opposite of that distance is where we want our camera
		}
	}
}
