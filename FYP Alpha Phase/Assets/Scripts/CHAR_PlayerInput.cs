using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(CHAR_Movement))]
public class CHAR_PlayerInput : MonoBehaviour
{
	// Components
	public CHAR_Movement characterMove { get; protected set; }
	public WPN_WeaponHandler weaponHandler { get; protected set; }
	private Transform trans;

	[System.Serializable]
	public class InputSettings
	{
		public string verticalAxis = "Vertical";
		public string horizontalAxis = "Horizontal";
		public string jumpButton = "Jump";
		public string fireButton = "Fire1";
		public string aimButton = "Fire2";
		public string reloadButton = "ReloadWeapon";
		public string dropButton = "DropWeapon";
		public string switchButton = "SwitchWeapon";
	}
	[SerializeField]
	private InputSettings inputSettings;

	[System.Serializable]
	public class LookSettings
	{
		public float lookSpeed = 100f;
		public float lookDistance = 10f;
		public bool requireInputToTurn = true;
		public LayerMask aimLayer;
	}
	[SerializeField]
	private LookSettings lookSettings;

	[Header("For Debugging")]
	public bool debugAim;
	public Transform spine;

	private bool isAiming;
	private Transform mainCamTrans;

	private void Awake()
	{
		characterMove = GetComponent<CHAR_Movement>();
		weaponHandler = GetComponent<WPN_WeaponHandler>();
		trans = GetComponent<Transform>();
	}

	private void Start()
	{
		mainCamTrans = Camera.main.transform;
	}

	private void Update()
	{
		CharacterLogic();
		CameraLogic();
		WeaponLogic();
	}

	private void LateUpdate()
	{
		if(!weaponHandler)
			return;

		if(weaponHandler.currentWeapon)
		{
			if(isAiming)
				PositionSpine();
		}
	}

	private void CharacterLogic() // Handles character logic
	{
		if(!characterMove)
			return;

		// Movement
		float v = Mathf.Clamp(Input.GetAxis(inputSettings.verticalAxis), -.5f, 1f);
		float h = (Input.GetAxis(inputSettings.verticalAxis) < 0) ? Mathf.Clamp(Input.GetAxis(inputSettings.horizontalAxis), -.5f, .5f) : Input.GetAxis(inputSettings.horizontalAxis);

		if(!isAiming)
			characterMove.AnimateCharacter(v, h);
		else
			characterMove.AnimateCharacter(v * .49f, h * .49f);


		// Jump
		if(Input.GetButtonDown(inputSettings.jumpButton))
			characterMove.Jump();
	}

	private void CameraLogic() // Handles camera logic
	{
		// Auto turn when aiming?
		lookSettings.requireInputToTurn = !isAiming; 

		if(lookSettings.requireInputToTurn)
		{
			if(Input.GetAxis(inputSettings.verticalAxis) != 0 || Input.GetAxis(inputSettings.horizontalAxis) != 0)
				CharacterLook();
		}
		else
			CharacterLook();
	}

	private void CharacterLook() // Make the character look at the same direction as the camera
	{
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

	private void PositionSpine()
	{
		if(!spine)
			return;

		//RaycastHit hit;
		Transform mainCamTrans = Camera.main.transform;
		Vector3 mainCamPos = mainCamTrans.position;
		Vector3 dir = mainCamTrans.forward;
		Ray ray = new Ray(mainCamPos, dir);
		//if(Physics.Raycast(ray, out hit, 100f, lookSettings.aimLayer))
		//{
		//	Vector3 hitPoint = hit.point;
		//	spine.LookAt(hitPoint);
		//}
		//else
			spine.LookAt(ray.GetPoint(50f));

		Vector3 eulerAngleOffset = weaponHandler.currentWeapon.modelSettings.spineRotation;
		spine.Rotate(eulerAngleOffset);
	}

	private void WeaponLogic() // Handles all weapon logic + inputs
	{
		if(!weaponHandler)
			return;

		#region Aim
		isAiming = Input.GetButton(inputSettings.aimButton) || debugAim;
		weaponHandler.AimWeapon(isAiming);
		#endregion

		#region Switch
		if(Input.GetButtonDown(inputSettings.switchButton))
			weaponHandler.SwitchNextWeapon();
		#endregion

		if(!weaponHandler.currentWeapon) // If no weapon equipped
			return;

		#region Fire
		Ray ray = new Ray(mainCamTrans.position, mainCamTrans.forward);
		weaponHandler.currentWeapon.aimRay = ray;

		if(isAiming && Input.GetButton(inputSettings.fireButton))
			weaponHandler.FireCurrentWeapon(ray);
		#endregion

		#region Reload
		if(Input.GetButtonDown(inputSettings.reloadButton))
			weaponHandler.ReloadWeapon();
		#endregion

		#region Drop
		if(Input.GetButtonDown(inputSettings.dropButton))
			weaponHandler.DropCurrentWeapon();
		#endregion
	}
}
