using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class CAM_CameraRig : MonoBehaviour 
{
	// Components
	private Transform trans;

	[System.Serializable]
	public class CameraSettings
	{
		[Header("-Positioning-")]
		public Vector3 camPosOffsetLeft = new Vector3(-.75f, .5f, -2f);
		public Vector3 camPosOffsetRight = new Vector3(.75f, .5f, -2f);

		[Header("-Camera Options-")]
		public float mouseSensitivityX = 2.5f;
		public float mouseSensitivityY = 2.5f;
		public float minAngle = 30f;
		public float maxAngle = 70f;
		public float cameraRotateSpeed = 5f;
		public float cameraMoveSpeed = 5f;

		[Header("-Aiming-")]
		public float defaultFOV = 70f;
		public float aimingFOV = 30f;
		public float zoomSpeed = 10f;

		[Header("-Visual Options-")]
		public float wallCheckDist = .1f;
		public float hideMeshWhenDistance = .5f;	
	}
	[SerializeField]
	public CameraSettings cameraSettings;

	[System.Serializable]
	public class InputSettings
	{
		public string xAxis = "Mouse X";
		public string yAxis = "Mouse Y";
		public string aimButton = "Fire2";
		public string switchShoulderButton = "Fire3";
	}
	[SerializeField]
	public InputSettings inputSettings;

	public Transform target;
	public bool autoTargetPlayer = true;
	public LayerMask wallLayer;
	public enum Shoulder { RIGHT, LEFT }
	public Shoulder shoulder;

	private Transform pivot;
	private Transform camTrans;
	private Camera mainCam;
	private float newX = 0f, newY = 0f;

	private void Awake()
	{
		// Cache components
		trans = GetComponent<Transform>();

		pivot = trans.GetChild(0); // Parent it to Trans_CamPivot
		camTrans = pivot.GetChild(0);
		mainCam = Camera.main;
	}

	private void Start() 
	{
		PrePositionMainCamera();
	}

	private void Update() 
	{
		if(Application.isPlaying)
		{
			RotateCamera();
			CheckCameraCollision();
			CheckMeshDistance();
			Aim(Input.GetButton(inputSettings.aimButton));
			if(Input.GetButtonDown(inputSettings.switchShoulderButton))
				SwitchShoulders();
		}
	}

	private void LateUpdate()
	{
		if(!target)
			TargetPlayer();

		Vector3 targetPos = target.position;
		Quaternion targetRot = target.rotation;
		FollowTarget(targetPos, targetRot);
	}

	private void TargetPlayer() // Finds player and set it as target
	{
		if(target)
			return;

		if(autoTargetPlayer)
		{
			GameObject player = GameObject.FindGameObjectWithTag("Player");
			if(player)
				target = player.transform;
		}
	}

	private void FollowTarget(Vector3 pos, Quaternion rot) // Follow the target smoothly
	{
		if(!Application.isPlaying)
		{
			trans.position = pos;
			trans.rotation = rot;
		}
		else
		{
			Vector3 newPos = Vector3.Lerp(trans.position, pos, cameraSettings.cameraMoveSpeed * Time.deltaTime);
			trans.position = newPos;
		}
	}

	private void RotateCamera() // Rotate the camera with input
	{
		// Get mouse movement
		newX += cameraSettings.mouseSensitivityX * Input.GetAxis(inputSettings.xAxis);
		newY += cameraSettings.mouseSensitivityY * Input.GetAxis(inputSettings.yAxis);

		// Clamping
		newX = Mathf.Repeat(newX, 360f);
		newY = Mathf.Clamp(newY, -Mathf.Abs(cameraSettings.minAngle), cameraSettings.maxAngle);

		// Rotation
		Vector3 eulerAngleAxis = new Vector3(newY, newX);
		Quaternion newRotation = Quaternion.Slerp(pivot.localRotation, Quaternion.Euler(eulerAngleAxis), cameraSettings.cameraRotateSpeed * Time.deltaTime);
		pivot.localRotation = newRotation;
	}

	private void CheckCameraCollision() // Check for walls
	{
		Transform mainCamTrans = camTrans;
		Vector3 mainCamPos = mainCamTrans.position;
		Vector3 pivotPos = pivot.position;

		// Do spherecast
		RaycastHit hit;
		Vector3 start = pivotPos;
		Vector3 dir = mainCamPos - pivotPos;
		float dist = Mathf.Abs(shoulder == Shoulder.LEFT ? cameraSettings.camPosOffsetLeft.z : cameraSettings.camPosOffsetRight.z);
		if(Physics.SphereCast(start, cameraSettings.wallCheckDist, dir, out hit, dist, wallLayer))
			RepositionCamera(hit, pivotPos, dir, mainCamTrans);
		else
		{
			switch(shoulder)
			{
				case Shoulder.LEFT:
					PositionCamera(cameraSettings.camPosOffsetLeft);
					break;
				case Shoulder.RIGHT:
					PositionCamera(cameraSettings.camPosOffsetRight);
					break;
			}
		}
	}

	private void RepositionCamera(RaycastHit hit, Vector3 pivotPos, Vector3 dir, Transform mainCamTrans) // Moves camera forward when we hit a wall
	{
		float hitDist = hit.distance;
		Vector3 sphereCastCenter = pivotPos + (dir.normalized * hitDist);
		mainCamTrans.position = sphereCastCenter;
	}

	private void PositionCamera(Vector3 camPos) // Position camera's localPosition to a given location
	{
		Transform mainCamTrans = camTrans;
		Vector3 mainCamPos = mainCamTrans.localPosition;

		Vector3 newPos = Vector3.Lerp(mainCamPos, camPos, cameraSettings.cameraMoveSpeed * Time.deltaTime);
		mainCamTrans.localPosition = newPos;
	}

	private void CheckMeshDistance() // Hide the meshes if within distance
	{
		if(!mainCam || !target)
			return;

		SkinnedMeshRenderer[] meshes = target.GetComponentsInChildren<SkinnedMeshRenderer>();
		Transform mainCamTrans = camTrans;
		Vector3 mainCamPos = mainCamTrans.position;
		Vector3 targetPos = target.position;
		float dist = Vector3.Distance(mainCamPos, (targetPos + target.up));
	
		if(meshes.Length > 0)
		{
			foreach(SkinnedMeshRenderer rend in meshes)
			{
				if(dist < cameraSettings.hideMeshWhenDistance)
					rend.enabled = false;
				else
					rend.enabled = true;
			}
		}
	}

	private void Aim(bool isAiming) // Switch camera FOV for aiming
	{
		if(isAiming)
		{
			float newFOV = Mathf.Lerp(mainCam.fieldOfView, cameraSettings.aimingFOV, cameraSettings.zoomSpeed * Time.deltaTime);
			mainCam.fieldOfView = newFOV;
		}
		else
		{
			float original = Mathf.Lerp(mainCam.fieldOfView, cameraSettings.defaultFOV, cameraSettings.zoomSpeed * Time.deltaTime);
			mainCam.fieldOfView = original;
		}
	}

	public void SwitchShoulders() // Set shoulder
	{
		switch(shoulder)
		{
			case Shoulder.LEFT:
				shoulder = Shoulder.RIGHT;
				break;
			case Shoulder.RIGHT:
				shoulder = Shoulder.LEFT;
				break;
		}
	}

	private void PrePositionMainCamera() // Parent and position the main camera properly
	{
		mainCam.transform.parent = camTrans; // Parent it to Trans_CamPos
		mainCam.transform.localPosition = Vector3.zero;
		mainCam.transform.localRotation = Quaternion.Euler(Vector3.zero);
	}
}
