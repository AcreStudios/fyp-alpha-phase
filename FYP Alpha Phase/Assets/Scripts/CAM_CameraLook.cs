using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CAM_CameraLook : CAM_Pivot 
{
	[Header("Camera properties")]
	public float cameraMoveSpeed = 5f;
	public float cameraTurnSpeed = 1.5f;
	public float turnSmoothing = .1f;
	public float maxCameraUp = -45f; // Max look up
	public float maxCameraDown = 75f; // Max look down

	private float lookAngle;
	private float tiltAngle;
	private const float lookDistance = 100f;

	private float smoothX = 0f;
	private float smoothY = 0f;
	private float smoothVelocityX = 0f;
	private float smoothVelocityY = 0f;

	private float offsetX;
	private float offsetY;

	Transform trans;

	[Header("Crosshair properties")]
	public float crosshairOffsetWiggle = .2f;
	CAM_CrosshairManager chManager;

	[Header("Cursor behaviour")]
	public CursorLockMode cursorMode = CursorLockMode.None;

	public static CAM_CameraLook instance;
	public static CAM_CameraLook GetInstance()
	{
		return instance;
	}

	protected override void Awake()
	{
		instance = this;

		base.Awake();

		trans = GetComponent<Transform>();
	}

	protected override void Start() 
	{
		base.Start();

		Cursor.lockState = cursorMode;

		chManager = CAM_CrosshairManager.GetInstance();
	}

	protected override void Update() 
	{
		base.Update();

		HandleCameraRotationalMovement();
	}

	protected override void Follow(float deltaTime)
	{
		trans.position = Vector3.Lerp(trans.position, target.position, cameraMoveSpeed * deltaTime);
	}

	void HandleCameraRotationalMovement()
	{
		HandleOffsets();

		float x = Input.GetAxis("Mouse X") + offsetX;
		float y = Input.GetAxis("Mouse Y") + offsetY;

		if(turnSmoothing > 0)
		{
			smoothX = Mathf.SmoothDamp(smoothX, x, ref smoothVelocityX, turnSmoothing);
			smoothY = Mathf.SmoothDamp(smoothY, y, ref smoothVelocityY, turnSmoothing);
		}
		else
		{
			smoothX = x;
			smoothY = y;
		}

		// Horizontal rotation
		lookAngle += smoothX * cameraTurnSpeed;
		trans.rotation = Quaternion.Euler(0f, lookAngle, 0);

		// Vertical rotation
		tiltAngle -= smoothY * cameraTurnSpeed;
		tiltAngle = Mathf.Clamp(tiltAngle, -Mathf.Abs(maxCameraUp), maxCameraDown);
		pivot.localRotation = Quaternion.Euler(tiltAngle, 0, 0);

		if(x > crosshairOffsetWiggle || x < -crosshairOffsetWiggle || y > crosshairOffsetWiggle || y < -crosshairOffsetWiggle)
			WiggleCrosshairAndCamera(0f);
	}

	void HandleOffsets()
	{
		if(offsetX != 0)
			offsetX = Mathf.MoveTowards(offsetX, 0, Time.deltaTime);

		if(offsetY != 0)
			offsetY = Mathf.MoveTowards(offsetY, 0, Time.deltaTime);
	}

	public void WiggleCrosshairAndCamera(float kickback)
	{
		offsetY = kickback;

		chManager.activeCrosshair.WiggleCrosshair();
	}
}
