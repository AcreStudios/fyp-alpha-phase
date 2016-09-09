using UnityEngine;
using System.Collections;

public class CivillianAIExperiment : MonoBehaviour {

    public enum CivillianStates
    {
        Calm, Panic, Alert
    }

    public CivillianStates currentState;
    public float detectionRadius;
    public Transform target;

    NavMeshAgent agent;
    Vector3 destination;
    RaycastHit hit;

    void Start () {
        //currentState = CivillianStates.Calm;

        agent = GetComponent<NavMeshAgent>();
	}
	
	void Update () { 
        switch (currentState)
        {
            case CivillianStates.Calm:
                Debug.Log("Do nothing, talk to other AI");
                break;

            case CivillianStates.Panic:
                if ((destination - transform.position).magnitude < 5){
                    if (Physics.Linecast(transform.position,target.position, out hit))
                    {
                        if (hit.transform == target)
                        {
                            destination = HuntForHidingSpot();
                        }
                    }
                    Debug.Log("Doing nothing, drawing linecast");
                }
                else
                {
                    agent.destination = destination;
                }
                
                break;

            case CivillianStates.Alert:
                if ((target.position - transform.position).magnitude < detectionRadius)
                {
                    destination = HuntForHidingSpot();
                    currentState = CivillianStates.Panic;
                }

                Debug.Log("Player not in range, but AI on alert"); //Maybe play a heightened talking anim
                break;
        }
	}

    Vector3 HuntForHidingSpot()
    {
        Collider placeToHide = null;
        Collider[] temp;

        float dist = Mathf.Infinity;

        temp = Physics.OverlapSphere(transform.position, detectionRadius);

        if (temp.Length > 0)
        {

            foreach (Collider obs in temp)
            {
                if (obs.transform.tag == "Obstacles")
                {
                    float tempDist = (obs.transform.position - transform.position).magnitude;
                    if (tempDist < dist)
                    {
                        dist = tempDist;
                        placeToHide = obs;
                    }
                }
            }

            return placeToHide.ClosestPointOnBounds(target.position) + ((placeToHide.bounds.center - placeToHide.ClosestPointOnBounds(target.position)) * 2);
        }
        return transform.position; //Maybe return a further location for AI to run away 
    }
}
