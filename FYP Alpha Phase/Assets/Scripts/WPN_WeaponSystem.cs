using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class WPN_WeaponSystem : MonoBehaviour 
{
	// Components
	private Collider col;
	private Rigidbody rb;
	private Animator animator;
	private Transform trans;
	private WPN_WeaponHandler weaponHandler;

	public enum WeaponType { PISTOL, RIFLE }
	public WeaponType weaponType;

	[System.Serializable]
	public class ModelSettings
	{
		public Transform leftHandIKTarget;
		public Vector3 spineRotation;
	}
	[SerializeField]
	public ModelSettings modelSettings;

	[System.Serializable]
	public class WeaponSettings
	{
		[Header("-Weapon Options-")]
		public float bulletDamage = 5f;
		public float bulletSpread = 5f;
		public float fireRate = .2f;
		public float fireRange = 200f;
		public float reloadDuration = 2f;
		public bool autoReload = false;
		public LayerMask bulletLayer;

		[Header("-Effects VFX-")]
		public GameObject bulletImpactPrefab;
		public GameObject muzzleFlashPrefab;
		public GameObject bulletCasingPrefab;

		public Transform bulletSpawnPoint;
		public Transform casingSpawnPoint;
		public Transform clipSpawnPoint;

		public float casingSpawnSpeed = 1f;

		[Header("-Crosshair-")]
		public GameObject crosshairPrefab;

		[Header("-Positioning-")]
		public Vector3 equipPosition;
		public Vector3 equipRotation;
		public Vector3 unequipPosition;
		public Vector3 unequipRotation;

		[Header("-Animation-")]
		public bool useAnimation;
		public int fireLayer = 0;
		public string fireName = "Fire";
	}
	[SerializeField]
	public WeaponSettings weaponSettings;

	[System.Serializable]
	public class AmmoSettings
	{
		public int totalAmmo;
		public int currentAmmo;
		public int maxAmmo;
	}
	[SerializeField]
	public AmmoSettings ammoSettings;

	[System.Serializable]
	public class SoundSettings
	{
		public AudioClip[] gunshotSounds;
		public AudioClip reloadSound;
		[Range(0.1f, 1f)]
		public float pitchMin = 1f;
		[Range(1f, 2f)]
		public float pitchMax = 1.2f;
		public AudioSource audioSrc;
	}
	[SerializeField]
	public SoundSettings soundSettings;

	// States
	private bool equipped;
	private bool loading;

	private void Awake()
	{
		col = GetComponent<Collider>(); // Must have the collider first!
		rb = GetComponent<Rigidbody>();
		animator = GetComponent<Animator>();
		trans = GetComponent<Transform>();

		EquipWeapon();
	}

	private void Update() 
	{
		if(weaponHandler)
		{
			DisableEnableComponents(false);
			if(equipped)
					EquipWeapon();
			else
				UnequipWeapon(weaponType);
		}
		else // If !weaponHandler
		{
			DisableEnableComponents(true);
			trans.SetParent(null);
		}
	}

	public void FireWeapon(Ray ray) // Fires the weapon
	{
		if(ammoSettings.currentAmmo <= 0 || loading || !weaponSettings.bulletSpawnPoint || !equipped)
			return;

		RaycastHit hit;
		Transform bSpawn = weaponSettings.bulletSpawnPoint;
		Vector3 bPoint = bSpawn.position;
		Vector3 dir = ray.GetPoint(weaponSettings.fireRange) - bPoint;
		dir += (Vector3)Random.insideUnitCircle * weaponSettings.bulletSpread; // Spread the bullets, reduces accuracy

		if(Physics.Raycast(bPoint, dir, out hit, weaponSettings.fireRange, weaponSettings.bulletLayer))
		{
			CHAR_Health hp = hit.transform.root.GetComponent<CHAR_Health>();
			if(hp)
				hp.ReceiveDamage(weaponSettings.bulletDamage);

			#region Spawn bullet impact
			if(weaponSettings.bulletImpactPrefab)
			{
				if(hit.collider.gameObject.isStatic)
				{
					Vector3 hitPoint = hit.point;
					Quaternion lookRot = Quaternion.LookRotation(hit.normal);
					GameObject decal = (GameObject)Instantiate(weaponSettings.bulletImpactPrefab, hitPoint, lookRot);
					Transform decalTrans = decal.transform;
					Transform hitTrans = hit.transform;
					decalTrans.SetParent(hitTrans);
					Destroy(decal, 20f);
				}
			}
			#endregion
		}

		#region Spawn muzzle flash
		if(weaponSettings.muzzleFlashPrefab)
		{
			Vector3 bSpawnPos = weaponSettings.bulletSpawnPoint.position;
			GameObject mFlash = (GameObject)Instantiate(weaponSettings.muzzleFlashPrefab, bSpawnPos, Quaternion.identity);
			Transform mFlashTrans = mFlash.transform;
			mFlashTrans.SetParent(weaponSettings.bulletSpawnPoint);
			Destroy(mFlash, .5f);
		}
		#endregion

		#region Spawn casing
		if(weaponSettings.bulletCasingPrefab && weaponSettings.casingSpawnPoint)
		{
			Vector3 casingPos = weaponSettings.casingSpawnPoint.position;
			Quaternion casingRot = weaponSettings.casingSpawnPoint.rotation;
			GameObject shell = (GameObject)Instantiate(weaponSettings.bulletCasingPrefab, casingPos, casingRot);
			if(shell.GetComponent<Rigidbody>())
			{
				Rigidbody shellRb = shell.GetComponent<Rigidbody>();
				shellRb.AddForce(weaponSettings.casingSpawnPoint.forward * weaponSettings.casingSpawnSpeed, ForceMode.Impulse);
				shellRb.AddTorque(weaponSettings.casingSpawnPoint.forward * weaponSettings.casingSpawnSpeed, ForceMode.Impulse);
			}
			Destroy(shell, 10f);
		}
		#endregion

		// Animation
		if(weaponSettings.useAnimation)
			animator.Play(weaponSettings.fireName, weaponSettings.fireLayer);

		// Load next ammo
		ammoSettings.currentAmmo--;
		StartCoroutine(FireCooldown());

		// Play sounds
		WeaponSounds();
	}

	private IEnumerator FireCooldown() // Loads next bullet; Cooldown interval between bullets
	{
		loading = true;
		yield return new WaitForSeconds(weaponSettings.fireRate);
		loading = false;
	}

	private void WeaponSounds() 
	{
		if(!SND_Manager.instance || !soundSettings.audioSrc)
			return;

		#region Gun shot sounds
		if(soundSettings.gunshotSounds.Length > 0)
		{
			SND_Manager.instance.PlaySoundOnce(weaponSettings.bulletSpawnPoint.position,
			soundSettings.gunshotSounds[Random.Range(0, soundSettings.gunshotSounds.Length)],
			2f,
			true,
			soundSettings.pitchMin,
			soundSettings.pitchMax);
		}
		#endregion
	}

	private void DisableEnableComponents(bool enabled) // Disable or enable rigidbody and collider
	{
		if(!enabled)
		{
			rb.isKinematic = true;
			col.enabled = false;
		}
		else
		{
			rb.isKinematic = false;
			col.enabled = true;
		}
	}

	private void EquipWeapon() // Equip this weapon to the hand
	{
		if(!weaponHandler)
			return;
		else if(!weaponHandler.modelSettings.rightHand)
			return;

		trans.SetParent(weaponHandler.modelSettings.rightHand);
		trans.localPosition = weaponSettings.equipPosition;
		Quaternion equipRot = Quaternion.Euler(weaponSettings.equipRotation);
		trans.localRotation = equipRot;
	}

	private void UnequipWeapon(WeaponType wpType) // Unequip and place weapon to the desired location
	{
		// Fix muzzle staying
		if(weaponSettings.bulletSpawnPoint.childCount > 0)
		{
			foreach(Transform t in weaponSettings.bulletSpawnPoint.GetComponentsInChildren<Transform>())
			{
				if(t != weaponSettings.bulletSpawnPoint)
					Destroy(t.gameObject);
			}
		}

		if(!weaponHandler)
			return;

		switch(wpType)
		{
			case WeaponType.PISTOL:
				trans.SetParent(weaponHandler.modelSettings.pistolUnequipSlot);
				break;
			case WeaponType.RIFLE:
				trans.SetParent(weaponHandler.modelSettings.rifleUnequipSlot);
				break;
		}
		trans.localPosition = weaponSettings.unequipPosition;
		Quaternion unequipRot = Quaternion.Euler(weaponSettings.unequipRotation);
		trans.localRotation = unequipRot;
	}

	public void LoadAmmo() // Load and calculate ammo
	{
		int ammoNeeded = ammoSettings.maxAmmo - ammoSettings.currentAmmo;

		if(ammoNeeded >= ammoSettings.totalAmmo) // If we have lesser than total ammo
		{
			ammoSettings.currentAmmo = ammoSettings.totalAmmo;
			ammoSettings.totalAmmo = 0;
		}
		else // We have enough total ammo
		{
			ammoSettings.totalAmmo -= ammoNeeded;
			ammoSettings.currentAmmo = ammoSettings.maxAmmo;
		}
	}

	public void SetEquipState(bool eq) // Sets the weapon equip state
	{
		equipped = eq;
	}

	public void SetOwner(WPN_WeaponHandler wp) // Sets the owner of this weapon
	{
		weaponHandler = wp;
	}
}
