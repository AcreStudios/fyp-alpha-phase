using UnityEngine;
using System.Collections.Generic;

public class SimpleLengthExperiement : MonoBehaviour {

    public List<GameObject> test;

    void Start () {
        Destroy(test[0]);
	}
	
	void Update () {
        Debug.Log(test.Count);
	}
}
