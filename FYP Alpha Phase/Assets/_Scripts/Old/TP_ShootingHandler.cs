using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TP_ShootingHandler : MonoBehaviour 
{
	[Header("Shooting variables")]
	public int currentBullets;
	public float fireRate = .2f; // Cooldown between shots
	private float fireTimer;
	public float fireRange = 100f;
	//public Animator weaponAnim;

	// States
	private bool shooting;
	//private bool dontShoot;
	private bool gunIsEmpty;

	[Header("Shooting VFXs")]
	public Transform bulletSpawn; // Put this in front of the gun
	//public GameObject dustImpactPrefab;
	//public ParticleSystem muzzleFlashPrefab;
	//public Transform caseSpawn;
	//public GameObject casingPrefab;

	private TP_StatesHandler states;

	void Awake()
	{
		states = GetComponent<TP_StatesHandler>();
	}

	void Start()
	{
		bulletSpawn = transform.Find("Trans_BulletSpawn");

		// Temp, for testing
		currentBullets = 30;
	}

	void Update() 
	{
		shooting = states.isShooting;
		if(shooting)
		{
			if(fireTimer <= 0f)
			{
				//weaponAnim.SetBool("Shoot", false);

				// If we have bullets
				if(currentBullets > 0)
				{
					currentBullets -= 1;
					gunIsEmpty = false;

					//states.audioHandler.PlayGunShotSound();
					SpawnCasing();
					SpawnMuzzleFlash();
					RaycastShoot();
				}
				else // If currentBullets < 0
				{
					if(gunIsEmpty)
					{
						// Reload()
						currentBullets = 30;
					}
					else
					{
						//states.audioHandler.PlayEffect("Reload_Gun");
						gunIsEmpty = true;
					}
				}

				fireTimer = fireRate;
			}
			else // fireTimer > 0f
			{
				//weaponAnim.SetBool("Shoot", true);
				fireTimer -= Time.deltaTime;
			}
		}
		else
		{
			//weaponAnim.SetBool("Shoot", false);
			fireTimer -= 1f;
		}
	}

	void RaycastShoot()
	{
		Vector3 dir = states.lookHitPosition - bulletSpawn.position;
		RaycastHit hit;

		if(Physics.Raycast(bulletSpawn.position, dir, out hit, fireRange, states.layerMask))
		{
			SpawnDustImpact(hit);

			// If hits an enemy
			//if(hit.transform.GetComponent<Health>())
		}
	}

	void SpawnCasing()
	{
		//GameObject c = (GameObject)Instantiate(casingPrefab, caseSpawn.position, caseSpawn.rotation);
		//Rigidbody r = c.GetComponent<Rigidbody>();
		//r.AddForce(transform.right.normalized * 2 + Vector3.up * 1.3f, ForceMode.Impulse);
		//r.AddRelativeTorque(c.transform.right * 1.5f, ForceMode.Impulse);
	}

	void SpawnMuzzleFlash()
	{
		//muzzleFlashPrefab.Emit(1);
	}

	void SpawnDustImpact(RaycastHit hit)
	{
		//GameObject dust = (GameObject)Instantiate(dustImpactPrefab, hit.point, Quaternion.identity);
		//dust.transform.LookAt(bulletSpawn.position);
	}
}
