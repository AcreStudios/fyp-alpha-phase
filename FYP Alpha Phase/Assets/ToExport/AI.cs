using UnityEngine;
using System.Collections;

[RequireComponent(typeof(NavMeshAgent))]

public class AI : AIFunctions {

    public enum AIStates {
        Idle,
        Attacking,
        Retreating,
        AttackingInOpen
    }

    public float reactionTime;
    public AIStates currentState;
    NavMeshAgent agent;
    Vector3 destination;
    bool hasStarted;
    float timer;

    //public bool damageTest;

    void Start() {
        currentState = AIStates.Idle;
        agent = GetComponent<NavMeshAgent>();
        startingPoint = transform.position;
        eColl = GetComponent<Collider>();
    }

    void Update() {
        if (showGunEffect) {
            //gunEffect.transform.position += scaleValue*10;
        }
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

                    if ((target.position - tempObs.transform.position).magnitude > range) {
                        destination = ObstacleHunting();
                        currentState = AIStates.Retreating;
                    }
                    if (tempObs != null)

                    if ((destination - transform.position).magnitude < 5) {
                        if ((lastAttackPoint - transform.position).magnitude < 5) {
                            if (Shooting()) {
                               transform.LookAt(target);
                            }
                        }
                    } else {
                        agent.destination = destination;
                        transform.LookAt(destination);
                        transform.eulerAngles = new Vector3(transform.eulerAngles.x,0, transform.eulerAngles.z);
                    }
                }              
                break;

            case AIStates.Retreating:
                agent.destination = destination;

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

                if ((destination - transform.position).magnitude < 5) {
                    if (Shooting()) {
                        transform.LookAt(target);
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
