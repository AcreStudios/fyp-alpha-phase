using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class _Dummy : MonoBehaviour 
{
	private CHAR_Movement movement;

	private void Awake()
	{
		movement = GetComponent<CHAR_Movement>();
	}

	private void Update() 
	{
		movement.AnimateCharacter(0f, 0f);
	}
}
