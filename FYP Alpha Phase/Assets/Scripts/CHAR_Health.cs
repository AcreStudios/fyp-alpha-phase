using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CHAR_Health : MonoBehaviour 
{
	// Components
	private CharacterController characterController;
	private CHAR_Ragdoll ragdoll;

	[Header("-Health-")]
	[Range(0f, 100f)]
	public float curHealth = 100f;

	[Header("-Death-")]
	public MonoBehaviour[] scriptsToDisable;

	private void Awake()
	{
		characterController = GetComponent<CharacterController>();
		ragdoll = GetComponentInChildren<CHAR_Ragdoll>();
	}

	public void ReceiveDamage(float dmg)
	{
		curHealth -= dmg;
		curHealth = Mathf.Clamp(curHealth, 0f, 100f);

		// If no health, die
		if(curHealth <= 0)
			Die();
	}

	private void Die()
	{
		characterController.enabled = false;

		if(scriptsToDisable.Length == 0)
			return;

		if(ragdoll)
			ragdoll.BecomeRagdoll();

		foreach(MonoBehaviour script in scriptsToDisable)
		{
			script.enabled = false;
		}
	}
}
