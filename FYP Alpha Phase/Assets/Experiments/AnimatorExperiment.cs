using UnityEngine;
using System.Collections;

public class AnimatorExperiment : MonoBehaviour {

    Animator anim;
    public int currentState;
   
	void Start () {
        anim = GetComponent<Animator>();
	}
	
	void Update () {
        anim.SetInteger("TreeState", currentState);
	}
}
