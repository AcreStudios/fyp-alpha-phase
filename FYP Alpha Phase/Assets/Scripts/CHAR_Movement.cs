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

	private bool isGrounded = true;
	private bool isJumping = false;
	private bool resetGravity;
	private float gravity;

	[Header("For Debugging")]
	public bool setupComponents = true;

	private void Awake()
	{
		// Cache components
		animator = GetComponent<Animator>();
		characterController = GetComponent<CharacterController>();
	}

	private void Start() 
	{
		SetupComponents();
		SetupAnimator();
	}

	private void Update() 
	{
		ApplyGravity();
		isGrounded = characterController.isGrounded;
	}

	public void AnimateCharacter(float forward, float strafe) // Animates the character and root motion handles the movement
	{
		animator.SetFloat(animationSettings.verticalFloat, forward);
		animator.SetFloat(animationSettings.horizontalFloat, strafe);
		animator.SetBool(animationSettings.groundedBool, isGrounded);
		animator.SetBool(animationSettings.jumpingBool, isJumping);
	}

	public void Jump()
	{
		if(isJumping)
			return;

		if(isGrounded)
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

	private void ApplyGravity()
	{
		if(!characterController.isGrounded)
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
			characterController.center = new Vector3(0f, 1f, 0f);
			characterController.radius = .25f;
		}
	}

	private void SetupAnimator() // Setup the animator with the child avatar
	{
		Animator[] animators = GetComponentsInChildren<Animator>();

		if(animators.Length > 0)
		{
			foreach(Animator a in animators)
			{
				Animator anim = a;
				Avatar ava = anim.avatar;

				if(anim != animator)
				{
					animator.avatar = ava;
					Destroy(anim);
				}
			}
		}
	}
}
