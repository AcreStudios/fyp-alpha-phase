using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WPN_WeaponHandler : MonoBehaviour 
{
	// Components
	private Animator animator;

	[System.Serializable]
	public class ModelSettings
	{
		public Transform rightHand;
		public Transform pistolUnequipSlot;
		public Transform rifleUnequipSlot;
	}
	[SerializeField]
	public ModelSettings modelSettings;

	[System.Serializable]
	public class AnimSettings
	{
		public string weaponTypeInt = "WeaponType";
		public string reloadingBool = "isReloading";
		public string aimingBool = "isAiming";
	}
	[SerializeField]
	public AnimSettings animSettings;

	[Header("-Weapons-")]
	public WPN_WeaponSystem currentWeapon;
	public List<WPN_WeaponSystem> weaponsList = new List<WPN_WeaponSystem>();
	public int maxWeapons = 2;
	private bool aiming;
	private bool reloading;
	private int weaponType;
	private bool switchingWeapon;

	private void Awake()
	{
		animator = GetComponent<Animator>();
	}

	private void OnEnable()
	{
		SetupWeapons();
	}

	private void SetupWeapons()
	{
		if(currentWeapon)
		{
			currentWeapon.SetEquipState(true);
			currentWeapon.SetOwner(this);
			AddWeaponToList(currentWeapon);

			if(reloading)
				if(switchingWeapon)
					reloading = false;
		}

		if(weaponsList.Count > 0)
		{
			for(int i = 0; i < weaponsList.Count; i++)
			{
				if(weaponsList[i] != currentWeapon)
				{
					weaponsList[i].SetEquipState(false);
					weaponsList[i].SetOwner(this);
				}
			}
		}
	}

	private void Update() 
	{
		AnimateWeapon();
	}

	private void AnimateWeapon() // Animates the character with weapon
	{
		if(!animator)
			return;

		animator.SetBool(animSettings.aimingBool, aiming);
		animator.SetBool(animSettings.reloadingBool, reloading);
		animator.SetInteger(animSettings.weaponTypeInt, weaponType);

		if(!currentWeapon)
		{
			weaponType = 0;
			return;
		}

		switch(currentWeapon.weaponType)
		{
			case WPN_WeaponSystem.WeaponType.PISTOL:
				weaponType = 1;
				break;
			case WPN_WeaponSystem.WeaponType.RIFLE:
				weaponType = 2;
				break;
		}
	}

	private void AddWeaponToList(WPN_WeaponSystem wp) // Add a weapon to weaponsList
	{
		if(weaponsList.Contains(wp))
			return;

		weaponsList.Add(wp);
	}

	public void FireCurrentWeapon(Ray aimRay) // Puts finger on trigger and check if we fired
	{
		// Fire
		currentWeapon.FireWeapon(aimRay);

		// Auto reload?
		if(!currentWeapon.weaponSettings.autoReload)
			return;

		if(currentWeapon.ammoSettings.currentAmmo == 0)
		{
			ReloadWeapon();
			return;
		}
	}

	public void ReloadWeapon() // Reload current weapon
	{
		if(reloading || !currentWeapon)
			return;

		if(currentWeapon.ammoSettings.totalAmmo <= 0 || currentWeapon.ammoSettings.currentAmmo == currentWeapon.ammoSettings.maxAmmo)
			return;

		// Play reload sound
		if(SND_Manager.instance)
		{
			if(currentWeapon.soundSettings.reloadSound && currentWeapon.soundSettings.audioSrc)
			{
				SND_Manager.instance.PlaySound(currentWeapon.soundSettings.audioSrc,
					currentWeapon.soundSettings.reloadSound,
					true, 
					currentWeapon.soundSettings.pitchMin,
					currentWeapon.soundSettings.pitchMax);
			}
		}

		StartCoroutine(FinishReload());
	}

	private IEnumerator FinishReload() // Finish the reloading of the weapon
	{
		reloading = true;
		yield return new WaitForSeconds(currentWeapon.weaponSettings.reloadDuration);
		currentWeapon.LoadAmmo();
		reloading = false;
	}

	public void AimWeapon(bool aim) // Sets the aiming state
	{
		aiming = aim;
	}

	public void DropCurrentWeapon() // Drops the current weapon
	{
		if(!currentWeapon)
			return;

		currentWeapon.SetEquipState(false);
		currentWeapon.SetOwner(null);
		weaponsList.Remove(currentWeapon);
		currentWeapon = null;
		SwitchNextWeapon();
	}

	public void SwitchNextWeapon() // Switch to the next weapon
	{
		if(switchingWeapon || weaponsList.Count == 0 || reloading)
			return;

		if(currentWeapon)
		{
			int currentWeaponIndex = weaponsList.IndexOf(currentWeapon);
			int nextWeaponIndex = (currentWeaponIndex + 1) % weaponsList.Count;

			currentWeapon = weaponsList[nextWeaponIndex];
		}
		else
			currentWeapon = weaponsList[0];

		StartCoroutine(FinishSwitchWeapon());
		SetupWeapons();
	}

	private IEnumerator FinishSwitchWeapon() // Finish switching weapons
	{
		switchingWeapon = true;
		yield return new WaitForSeconds(.7f);
		switchingWeapon = false;
	}

	private void OnAnimatorIK()
	{
		if(!animator)
			return;

		if(currentWeapon && currentWeapon.modelSettings.leftHandIKTarget && weaponType == 2 && !reloading && !switchingWeapon)
		{
			animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1f);
			animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1f);
			Transform target = currentWeapon.modelSettings.leftHandIKTarget;
			Vector3 targetPos = target.position;
			Quaternion targetRot = target.rotation;
			animator.SetIKPosition(AvatarIKGoal.LeftHand, targetPos);
			animator.SetIKRotation(AvatarIKGoal.LeftHand, targetRot);
		}
		else
		{
			animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 0f);
			animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 0f);
		}
	}
}
