﻿using UnityEngine;
using System.Collections;

public class LoadingControllerScript : MonoBehaviour {

    public NewMenuScript newMenuScript;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if(Input.GetKeyDown("space")) {
            newMenuScript.loadApplication();
        }
	}
}
