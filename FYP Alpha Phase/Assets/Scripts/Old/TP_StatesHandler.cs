using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TP_StatesHandler : MonoBehaviour 
{
	[Header("Player states")]
	public bool isAiming;
	public bool isWalking;
	public bool isShooting;
	public bool isReloading;
	public bool isGrounded;
	public bool canSprint;

	[HideInInspector]
	public Vector3 lookPosition, lookHitPosition;
	[HideInInspector]
	public LayerMask layerMask;

	private Transform trans;
	private TP_InputsHandler inputs;
	[HideInInspector]
	public TP_ShootingHandler shootingHandler;
	//[HideInInspector]
	//public TP_AnimationsHandler anims;

	//public TP_AudioHandler audioHandler;

	void Awake()
	{
		trans = GetComponent<Transform>();
		inputs = GetComponent<TP_InputsHandler>();

		shootingHandler = GetComponent<TP_ShootingHandler>();
		//anims = GetComponent<TP_AnimationsHandler>();
		//audioHandler = GetComponent<TP_AudioHandler>();
	}

	void FixedUpdate() 
	{
		// Check if our character is on the ground
		isGrounded = RaycastGroundCheck();

		// Define states
		UpdateStates();
	}

	bool RaycastGroundCheck()
	{
		bool isOnGround = false;

		Vector3 origin = trans.position + new Vector3(0f, .05f, 0f);
		RaycastHit hit;
		if(Physics.Raycast(origin, -Vector3.up, out hit, .5f, layerMask))
			isOnGround = true;

		return isOnGround;
	}

	void UpdateStates()
	{
		isAiming = isGrounded && inputs.RMB;
		canSprint = !isAiming;
		isWalking = inputs.sprint;
	}
}
