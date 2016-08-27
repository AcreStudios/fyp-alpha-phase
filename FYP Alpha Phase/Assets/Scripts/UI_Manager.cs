using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class UI_Manager : MonoBehaviour 
{
	// Singleton
	public static UI_Manager instance;

	// Components, optimised*!
	private CHAR_PlayerInput player; //{ get { return FindObjectOfType<CHAR_PlayerInput>(); } set { player = value; } }
	private WPN_WeaponHandler weaponHandler; //{ get { return player.GetComponent<WPN_WeaponHandler>(); } set { weaponHandler = value; } }

	[System.Serializable]
	public class UI_Player
	{
		[Header("-Ammo-")]
		public Text ammoText;
		[Header("-Health-")]
		public Slider healthBar;
		public Text healthText;
	}
	[SerializeField]
	public UI_Player playerUI;

	private void Awake()
	{
		// Cache components
		player = FindObjectOfType<CHAR_PlayerInput>();
		weaponHandler = player.GetComponent<WPN_WeaponHandler>();

		// Get instance
		if(!instance)
			instance = this;
		else
		{
			if(instance != this)
				Destroy(gameObject);
		}
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
		if(playerUI.healthBar && playerUI.healthText)
			playerUI.healthText.text = playerUI.healthBar.value.ToString();
		#endregion

		if(!weaponHandler)
			return;

		#region Update ammo
		if(playerUI.ammoText)
		{
			if(!weaponHandler.currentWeapon)
				playerUI.ammoText.text = "Unarmed";
			else
				playerUI.ammoText.text = weaponHandler.currentWeapon.ammoSettings.currentAmmo + " / " + weaponHandler.currentWeapon.ammoSettings.totalAmmo;
		}
		#endregion
	}
}
