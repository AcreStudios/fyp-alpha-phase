using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(CHAR_Movement))]
public class CHAR_PlayerInput : MonoBehaviour 
{
	// Components
	private CHAR_Movement characterMove;
	private Transform trans;

	[System.Serializable]
	public class InputSettings
	{
		public string verticalAxis = "Vertical";
		public string horizontalAxis = "Horizontal";
		public string jumpButton = "Jump";
	}
	[SerializeField]
	private InputSettings inputSettings;

	[System.Serializable]
	public class LookSettings
	{
		public float lookSpeed = 5f;
		public float lookDistance = 10f;
		public bool requireInputToTurn = true;
	}
	[SerializeField]
	private LookSettings lookSettings;

	private Camera mainCam;

	private void Awake()
	{
		characterMove = GetComponent<CHAR_Movement>();
		trans = GetComponent<Transform>();
		mainCam = Camera.main;
	}

	private void Start() 
	{
		
	}

	private void Update() 
	{
		if(!characterMove)
			return;

		characterMove.AnimateCharacter(Input.GetAxis(inputSettings.verticalAxis), Input.GetAxis(inputSettings.horizontalAxis));
		if(Input.GetButtonDown(inputSettings.jumpButton))
			characterMove.Jump();

		if(mainCam)
		{
			if(lookSettings.requireInputToTurn)
			{
				if(Input.GetAxis(inputSettings.verticalAxis) != 0 || Input.GetAxis(inputSettings.horizontalAxis) != 0)
					CharacterLook();
			}
			else
				CharacterLook();
		}
	}

	private void CharacterLook()
	{
		Transform mainCamTrans = mainCam.transform.parent;
		Transform pivot = mainCamTrans.parent;
		Vector3 pivotPos = pivot.position;
		Vector3 lookTarget = pivotPos + (pivot.forward * lookSettings.lookDistance);
		Vector3 thisPos = trans.position;
		Vector3 lookDir = lookTarget - thisPos;
		Quaternion lookRot = Quaternion.LookRotation(lookDir);
		lookRot.x = 0f;
		lookRot.z = 0f;

		Quaternion newRot = Quaternion.Lerp(trans.rotation, lookRot, lookSettings.lookSpeed * Time.deltaTime);
		trans.rotation = newRot;
	}
}
