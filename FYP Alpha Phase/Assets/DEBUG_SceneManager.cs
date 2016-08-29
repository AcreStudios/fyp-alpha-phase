using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class DEBUG_SceneManager : MonoBehaviour 
{
	private void Start()
	{
		Cursor.lockState = CursorLockMode.Locked;
	}

	private void Update() 
	{
		if(Input.GetKeyDown(KeyCode.I))
			SceneManager.LoadScene(0);

		if(Input.GetKeyDown(KeyCode.O))
			SceneManager.LoadScene(1);

		if(Input.GetKeyDown(KeyCode.P))
			SceneManager.LoadScene(2);
	}
}
