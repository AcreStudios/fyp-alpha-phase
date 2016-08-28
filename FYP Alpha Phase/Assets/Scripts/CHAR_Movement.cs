using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CharacterController))]
public class CHAR_Movement : MonoBehaviour 
{
	// Components
	private Animator animator;
	private CharacterController characterController;
	private Transform trans;

	[System.Serializable] // Show in inspector for classes
	public class AnimationSettings
	{
		public string verticalFloat = "Forward";
		public string horizontalFloat = "Strafe";
		public string groundedBool = "isGrounded";
		public string jumpingBool = "isJumping";
	}
	[SerializeField]
	public AnimationSettings animationSettings;

	[System.Serializable]
	public class PhysicsSettings
	{
		public float gravityModifier = 9.81f;
		public float baseGravity = 50f;
		public float resetGravityValue = 1.2f;
		public LayerMask groundLayer;
		public float airSpeed = 5f;
	}
	[SerializeField]
	public PhysicsSettings physicsSettings;

	[System.Serializable]
	public class MovementSettings
	{
		public float jumpSpeed = 6f;
		public float jumpTime = .25f;
	}
	[SerializeField]
	public MovementSettings movementSettings;

	private bool isJumping = false;
	private bool resetGravity;
	private float gravity;
	private Vector3 airControlVector;
	private float forward;
	private float strafe;

	[Header("For Debugging")]
	public bool setupComponents = true;

	private void Awake()
	{
		// Cache components
		animator = GetComponent<Animator>();
		SetupAnimator();
		trans = GetComponent<Transform>();
	}

	private void Start() 
	{
		characterController = GetComponent<CharacterController>();
		SetupComponents();
	}

	private void Update()
	{
		AirControl(forward, strafe);
		ApplyGravity();
	}

	public void AnimateCharacter(float fw, float sf) // Animates the character and root motion handles the movement
	{
		forward = fw;
		strafe = sf;

		animator.SetFloat(animationSettings.verticalFloat, fw);
		animator.SetFloat(animationSettings.horizontalFloat, sf);
		animator.SetBool(animationSettings.groundedBool, CheckGrounded());
		animator.SetBool(animationSettings.jumpingBool, isJumping);
	}

	public void Jump()
	{
		if(isJumping)
			return;

		if(CheckGrounded())
		{
			isJumping = true;
			StartCoroutine(ResetJump());
		}
	}

	private IEnumerator ResetJump() // Stops jumping
	{
		yield return new WaitForSeconds(movementSettings.jumpTime);
		isJumping = false;
	}

	private void AirControl(float fw, float sf)
	{
		if(CheckGrounded())
			return;

		airControlVector.x = strafe;
		airControlVector.z = forward;
		airControlVector = trans.TransformDirection(airControlVector);
		airControlVector *= physicsSettings.airSpeed;

		characterController.Move(airControlVector * Time.deltaTime);
	}

	private bool CheckGrounded()
	{
		RaycastHit hit;
		Vector3 start = trans.position + trans.up;
		Vector3 dir = Vector3.down;

		if(Physics.SphereCast(start, characterController.radius, dir, out hit, characterController.height * .5f, physicsSettings.groundLayer))
			return true;
		else
			return false;
	}

	private void ApplyGravity()
	{
		if(!CheckGrounded())
		{
			if(!resetGravity)
			{
				gravity = physicsSettings.resetGravityValue;
				resetGravity = true;
			}
			gravity += Time.deltaTime * physicsSettings.gravityModifier;
		}
		else
		{
			gravity = physicsSettings.baseGravity;
			resetGravity = false;
		}

		Vector3 gravityVector = new Vector3();
		if(!isJumping)
			gravityVector.y -= gravity; // Apply gravity to character
		else
			gravityVector.y = movementSettings.jumpSpeed; // Let character jump

		characterController.Move(gravityVector * Time.deltaTime);
	}

	private void SetupComponents() // Initialise the components values so the script can be drag 'n' dropped
	{
		if(setupComponents)
		{
			// Check for animator controller
			if(!animator.runtimeAnimatorController)
				Debug.Log("There is no animator controller found! Please assign the approriate animator controller to " + name + "!");

			// Animator
			animator.applyRootMotion = true;

			// Character controller
			characterController.skinWidth = 0.0001f;
			characterController.center = new Vector3(0f, .8f, 0f);
			characterController.height = 1.6f;
			characterController.radius = .25f;
		}
	}

	private void SetupAnimator() // Setup the animator with the child avatar
	{
		Animator wantedAnim = GetComponentsInChildren<Animator>()[1];
		Avatar wantedAva = wantedAnim.avatar;

		animator.avatar = wantedAva;
		Destroy(wantedAnim);

		#region Auto search and destroy all child animators
		//Animator[] animators = GetComponentsInChildren<Animator>();
		//if(animators.Length > 0)
		//{
		//	foreach(Animator a in animators)
		//	{
		//		Animator anim = a;
		//		Avatar ava = anim.avatar;

		//		if(anim != animator)
		//		{
		//			animator.avatar = ava;
		//			Destroy(anim);
		//		}
		//	}
		//}
		#endregion
	}
}
