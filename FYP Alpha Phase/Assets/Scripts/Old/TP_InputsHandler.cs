using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TP_InputsHandler : MonoBehaviour 
{
	[Header("WASD")]
	[Range(-1f, 1f)]
	public float horizontal;
	[Range(-1f, 1f)]
	public float vertical;

	[Header("Mouse Axes")]
	public float mouseX;
	public float mouseY;

	[Header("Mouse Buttons")]
	public bool LMB;
	public bool RMB;
	public bool MMB;

	[Header("Keyboard Buttons")]
	public bool vision;
	public KeyCode visionBtn = KeyCode.E;
	public bool reload;
	public KeyCode reloadBtn = KeyCode.R;
	public bool sprint;
	public KeyCode sprintBtn = KeyCode.LeftShift;
	public bool crouch;
	public KeyCode crouchBtn = KeyCode.LeftControl;

	void FixedUpdate() 
	{
		mouseX = Input.GetAxis("Mouse X");
		mouseY = Input.GetAxis("Mouse Y");
		horizontal = Input.GetAxis("Horizontal");
		vertical = Input.GetAxis("Vertical");

		LMB = Input.GetButton("Fire1");
		RMB = Input.GetButton("Fire2");
		MMB = Input.GetButton("Fire3");

		vision = Input.GetKey(visionBtn);
		reload = Input.GetKey(reloadBtn);
		sprint = Input.GetKey(sprintBtn);
		crouch = Input.GetKey(crouchBtn);
	}
}
