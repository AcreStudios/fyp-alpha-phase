using UnityEngine;
using System.Collections;

[RequireComponent(typeof(NavMeshAgent))]

public class AI : AIFunctions {

    enum AIStates {
        Idle,
        Attacking,
        Retreating,
        AttackingInOpen
    }

    public float reactionTime;
     AIStates currentState;
    NavMeshAgent agent;
    Vector3 destination;
    bool hasStarted;
    float timer;

    public bool damageTest;

    void Start() {
        currentState = AIStates.Idle;
        agent = GetComponent<NavMeshAgent>();
        startingPoint = transform.position;
        eColl = GetComponent<Collider>();

		moveInst = GetComponent<CHAR_Movement> ();
		weapInst = GetComponent<WPN_WeaponHandler> ();
    }

    void Update() {
        switch (currentState) {
            case AIStates.Idle:
                
                if (target != null ) {
                    if (!hasStarted) {
                        timer = Time.time + reactionTime;
                        hasStarted = true;
                    }
                    if (Time.time > timer) {
                        AlertOtherTroops();
                        currentState = AIStates.Attacking;
                        hasStarted = false;
                    }
                } else {
                    hasStarted = false;
                    if ((startingPoint - transform.position).magnitude < 5) {
                        LookAround();
                    } else {
                        agent.destination = startingPoint;
					moveInst.AnimateCharacter (agent.speed, 0);
                    }
                }
                break;

            case AIStates.Attacking:
                RaycastHit hit;

                if (tempObs == null) {
                    currentState = AIStates.AttackingInOpen;
                } else {
                    if (Physics.Linecast(lastHidingPoint, target.position, out hit)) { //if player can see it in its hiding spot
                        if (hit.transform.tag == "Player" || hit.transform == transform) {
                            destination = ObstacleHunting();
                            currentState = AIStates.Retreating;
                        }
                    }
                    Debug.Log((target.position - tempObs.transform.position).magnitude);

                    if ((target.position - tempObs.transform.position).magnitude > range) {
                        destination = ObstacleHunting();
                        currentState = AIStates.Retreating;
                    }
                    if (tempObs != null)

                    if ((destination - transform.position).magnitude < 5) {
                        if ((lastAttackPoint - transform.position).magnitude < 5) {
                            if (Shooting()) {							
                               transform.LookAt(target);
							weapInst.AimWeapon (true);
							AlertOtherTroops();
                            }
                        }
                    } else {
                        agent.destination = destination;
					moveInst.AnimateCharacter (agent.speed, 0);
                        transform.LookAt(destination);
                        transform.eulerAngles = new Vector3(transform.eulerAngles.x,0, transform.eulerAngles.z);
                    }
                }              
                break;

            case AIStates.Retreating:
                agent.destination = destination;
			moveInst.AnimateCharacter (agent.speed, 0);

                if ((destination - transform.position).magnitude < 5) {
                    destination = ObstacleHunting();
                    AlertOtherTroops();
                    if ((destination - transform.position).magnitude < 5) {
                        currentState = AIStates.Attacking;
                    }
                }
                break;

            case AIStates.AttackingInOpen:
                destination = ObstacleHunting();
                agent.destination = destination;
			moveInst.AnimateCharacter (agent.speed, 0);

                if ((destination - transform.position).magnitude < 5) {
                    if (Shooting()) {
                        transform.LookAt(target);
					weapInst.AimWeapon (true);
                        AlertOtherTroops();
                    }
                }
                    if (tempObs != null) {
                    currentState = AIStates.Attacking;
                }
                break;
        }
    }

    public override void DamageRecieved(float damage) {
        if (currentState == AIStates.Attacking || currentState == AIStates.AttackingInOpen) 
        StartCoroutine(ChangeDestination());
        base.DamageRecieved(damage);
    }

    IEnumerator ChangeDestination() {
        destination = lastHidingPoint;
        yield return new WaitForSeconds(3);
        destination = lastAttackPoint;
    }
}
