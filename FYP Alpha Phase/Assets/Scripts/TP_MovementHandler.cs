using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TP_MovementHandler : MonoBehaviour 
{
	// Cache
	TP_InputsHandler inputs;
	TP_CameraHandler cam;
	TP_StatesHandler states;
	Rigidbody rb;
	CapsuleCollider col;
	Transform trans;

	// Physics materials
	PhysicMaterial zFriction;
	PhysicMaterial mFriction;

	[Header("Movement speeds")]
	public float sprintSpeed = 30f;
	public float walkSpeed = 15f;
	public float aimSpeed = 10f;

	public float rotateSpeed = 20f;
	public float turnSpeed = 50f;

	private Vector3 storeDir;
	private Vector3 lookDir;

	void Awake()
	{
		inputs = GetComponent<TP_InputsHandler>();
		cam = GetComponent<TP_CameraHandler>();
		states = GetComponent<TP_StatesHandler>();
		rb = GetComponent<Rigidbody>();
		col = GetComponent<CapsuleCollider>();
		trans = GetComponent<Transform>();
	}

	void Start() 
	{
		CreatePhysicsMaterials();
	}

	void FixedUpdate() 
	{
		lookDir = states.lookPosition - trans.position;

		// Assign physics material depending on state
		if(inputs.horizontal != 0 || inputs.vertical != 0 || !states.isGrounded)
			col.material = zFriction;
		else
			col.material = mFriction;

		Vector3 h = cam.camTrans.right * inputs.horizontal;
		Vector3 v = cam.camTrans.forward * inputs.vertical;
		h.y = 0;
		v.y = 0;

		HandleRotation(h, v);

		if(states.isGrounded)
		{
			rb.drag = 4f;

			rb.AddForce((h + v).normalized * SelectSpeed());
		}
		else
			rb.drag = 0f;
	}

	float SelectSpeed()
	{
		float speed = 0f;

		if(states.isAiming)
			speed = aimSpeed;
		else if(states.isWalking || states.isReloading)
			speed = walkSpeed;
		else
			speed = sprintSpeed;

		return speed;
	}

	void HandleRotation(Vector3 h, Vector3 v)
	{
		if(states.isAiming)
		{
			lookDir.y = 0f;

			Quaternion targetRot = Quaternion.LookRotation(lookDir);
			trans.rotation = Quaternion.Slerp(rb.rotation, targetRot, Time.deltaTime * rotateSpeed);
		}
		else
		{
			storeDir = trans.position + h + v;

			Vector3 dir = storeDir - trans.position;
			dir.y = 0f;

			if(inputs.horizontal != 0 || inputs.vertical != 0)
			{
				float posAngle = Vector3.Angle(trans.forward, dir);
				if(posAngle != 0)
				{
					float rotAngle = Quaternion.Angle(trans.rotation, Quaternion.LookRotation(dir));
					if(rotAngle != 0)
						rb.rotation = Quaternion.Slerp(trans.rotation, Quaternion.LookRotation(dir), Time.deltaTime * turnSpeed);
				}
			}
		}
	}

	void CreatePhysicsMaterials()
	{
		zFriction = new PhysicMaterial();
		zFriction.dynamicFriction = 0f;
		zFriction.staticFriction = 0f;

		mFriction = new PhysicMaterial();
		mFriction.dynamicFriction = 1f;
		mFriction.staticFriction = 1f;
	}
}
