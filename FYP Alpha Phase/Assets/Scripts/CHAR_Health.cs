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

	[Header("-Is Enemy?-")]
	public bool isEnemy = false;
	[System.Serializable]
	public class EnemySettings
	{
		[Header("-If Is Enemy-")]
		public Color curHealthColor;
		[HideInInspector]
		public SkinnedMeshRenderer[] meshes;
		public float visibilitySpeed = 2f;
	}
	[SerializeField]
	public EnemySettings enemySettings;

	private bool isVisible = false;
	private bool switching = false;


	private void Awake()
	{
		characterController = GetComponent<CharacterController>();
		ragdoll = GetComponentInChildren<CHAR_Ragdoll>();
	}

	private void Start()
	{
		if(isEnemy)
			enemySettings.meshes = GetComponentsInChildren<SkinnedMeshRenderer>();

		// Initialise visibility
		isVisible = false;
		foreach(SkinnedMeshRenderer m in enemySettings.meshes)
		{
			Material mat = m.sharedMaterial;
			if(mat)
				mat.SetFloat("_EdgeVisibility", 0f);
		}

		CalculateVision();
	}

	public void ReceiveDamage(float dmg)
	{
		curHealth -= dmg;
		curHealth = Mathf.Clamp(curHealth, 0f, 100f);

		// If no health, die
		if(curHealth <= 0)
			Die();
		else
			CalculateVision();
	}

	private void Die()
	{
		// Remove color
		enemySettings.curHealthColor = new Color(0f, 0f, 0f);
		VisionUpdate();

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

	private void CalculateVision()
	{
		float lerp = Mathf.Clamp01(curHealth * 0.01f);
		float green = Mathf.Lerp(0, 1, lerp);
		float red = Mathf.Lerp(1, 0, lerp);

		enemySettings.curHealthColor = new Color(red, green, 0f);
		VisionUpdate();
	}

	private void VisionUpdate()
	{
		if(!isEnemy)
			return;

		foreach(SkinnedMeshRenderer m in enemySettings.meshes)
		{
			Material mat = m.sharedMaterial;
			if(mat && curHealth != 0)
				mat.SetColor("_EdgeColor", enemySettings.curHealthColor);
			else
				mat.SetColor("_EdgeColor", enemySettings.curHealthColor);
		}
	}

	public void Ping()
	{
		if(switching)
			return;

		if(!isVisible)
			StartCoroutine(BecomeVisible());
		else
			StartCoroutine(BecomeNotVisible());
	}

	private IEnumerator BecomeVisible()
	{
		switching = true;

		float progress = 0f;
		while(progress < 1f)
		{
			progress += enemySettings.visibilitySpeed * Time.deltaTime;

			foreach(SkinnedMeshRenderer m in enemySettings.meshes)
			{
				Material mat = m.sharedMaterial;
				if(mat)
					mat.SetFloat("_EdgeVisibility", progress);
			}

			yield return null;
		}

		foreach(SkinnedMeshRenderer m in enemySettings.meshes)
		{
			Material mat = m.sharedMaterial;
			if(mat)
				mat.SetFloat("_EdgeVisibility", 1f);
		}

		isVisible = true;
		switching = false;
	}

	private IEnumerator BecomeNotVisible()
	{
		switching = true;

		float progress = 1f;
		while(progress > 0f)
		{
			progress -= enemySettings.visibilitySpeed * Time.deltaTime;

			foreach(SkinnedMeshRenderer m in enemySettings.meshes)
			{
				Material mat = m.sharedMaterial;
				if(mat)
					mat.SetFloat("_EdgeVisibility", progress);
			}

			yield return null;
		}

		foreach(SkinnedMeshRenderer m in enemySettings.meshes)
		{
			Material mat = m.sharedMaterial;
			if(mat)
				mat.SetFloat("_EdgeVisibility", 0f);
		}

		isVisible = false;
		switching = false;
	}
}
