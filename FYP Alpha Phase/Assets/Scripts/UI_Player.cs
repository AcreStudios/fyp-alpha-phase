using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class UI_Player : MonoBehaviour 
{
	// Singleton
	public static UI_Player instance;

	// Player components
	private GameObject player;
	private WPN_WeaponHandler playerWeapon;
	private CHAR_Health playerHealth;

	[System.Serializable]
	public class UI_PlayerStats
	{
		[Header("-Ammo-")]
		public Text curAmmoText;
		public Text totAmmoText;
		[Header("-Health-")]
		public Text healthText;
	}
	[SerializeField]
	public UI_PlayerStats playerUI;

	private void Awake()
	{
		// Get instance
		if(!instance)
			instance = this;
		else
		{
			if(instance != this)
				Destroy(gameObject);
		}
	}

	private void Start()
	{
		// Cache components
		GameObject target = GameObject.FindGameObjectWithTag("Player");

		if(target)
		{
			player = target;
			playerWeapon = player.GetComponent<WPN_WeaponHandler>();
			playerHealth = player.GetComponent<CHAR_Health>();
		}
		else
			Debug.LogError("No Prefab_Player with the tag Player found! Please assign tag!");
	}

	private void Update() 
	{
		UpdateDisplayUI();
	}

	private void UpdateDisplayUI()
	{
		if(!player)
			return;

		#region Update health
		if(playerHealth)
		{
			if(playerUI.healthText)
				playerUI.healthText.text = "Health: " + playerHealth.curHealth;
		}
		#endregion

		if(!playerWeapon)
			return;

		#region Update ammo
		if(playerUI.curAmmoText)
		{
			if(!playerWeapon.currentWeapon)
			{
				playerUI.curAmmoText.enabled = false;
				playerUI.totAmmoText.text = "-";
			}
			else
			{
				playerUI.curAmmoText.enabled = true;
				playerUI.curAmmoText.text = playerWeapon.currentWeapon.ammoSettings.currentAmmo.ToString();
				playerUI.totAmmoText.text = "/" + playerWeapon.currentWeapon.ammoSettings.totalAmmo;
			}
		}
		#endregion
	}
}
