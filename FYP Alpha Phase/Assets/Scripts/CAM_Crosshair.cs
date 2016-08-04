using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

public class CAM_Crosshair : MonoBehaviour
{
	public string CrosshairName = "Cross"; //we can use this as a tag to sort through our crosshairs if needed

	[Header("Crosshair properties")]
	public float defaultSpread = .9f;
	public float maxSpread = 5f;
	public float wiggleSpread = 5f;
	public float wiggleSpreadMaxTimer = 60f;

	public float spreadSpeed = .18f;
	public float rotationSpeed = .5f;
	public bool allowRotating = true;

	public bool spreadWhileRotating = true;
	public float rotationSpread = 30f;
	public bool allowSpread = true;

	[HideInInspector]
	public float currentSpread = 0;
	private float targetSpread = 0;
	private float spreadT = 0;
	private Quaternion defaultRotation;
	private bool isSpreadWorking = true;

	private float rotationTimer = 0;
	private bool isRotating = false;

	private bool wiggle = false;
	private float wiggleTimer = 0;

	Transform trans;

	//a n array to store each part of our crosshair
	public CrosshairParts[] parts;

	//constructor for the array of our parts
	[Serializable]
	public class CrosshairParts
	{
		public Image image;
		public Vector2 up;
	}

	void Awake()
	{
		trans = GetComponent<Transform>();
	}

	void Start()
	{
		//store our default rotation and the default spread
		defaultRotation = trans.rotation;
		currentSpread = defaultSpread;

		//change the crosshair spread
		ChangeCursorSpread(defaultSpread);
	}

	void Update()
	{
		if(isSpreadWorking) //if we want the crosshair to spread
		{
			//add to the timer
			spreadT += Time.deltaTime / spreadSpeed;

			//calculate our spread value with an equtation
			float spreadValue = AccelDecelInterpolation(currentSpread, targetSpread, spreadT);

			//if the timer is higher than one
			if(spreadT > 1)
			{
				//we reached our target spread
				spreadValue = targetSpread;
				spreadT = 0;

				if(wiggle) // and if we want it to wiggle
				{
					
					if(wiggleTimer < wiggleSpreadMaxTimer) // add to the wiggle timer
						wiggleTimer += Time.deltaTime;
					else
					{
						wiggleTimer = 0;
						wiggle = false;
						targetSpread = defaultSpread; //our targetSpread is the defaultSpread
					}
				}
				else
					isSpreadWorking = false;
			}
			else // if(spreadT > 1)
				ChangeCursorSpread(defaultSpread);


			currentSpread = spreadValue;
			ApplySpread();
		} // if(isSpreadWorking)

		if(isRotating) //if we want it to rotate
		{
			if(rotationTimer > 0) //and the rotation timer is higher than 0
			{
				rotationTimer -= Time.deltaTime; //decrease the timer

				//and apply the rotation
				trans.rotation = Quaternion.Euler(trans.rotation.eulerAngles.x,
												  trans.rotation.eulerAngles.y,
												  trans.rotation.eulerAngles.z + (360f * Time.deltaTime * rotationSpeed));
			}
			else
			{
				isRotating = false;
				trans.rotation = defaultRotation; // The default rotation

				if(spreadWhileRotating)
					ChangeCursorSpread(defaultSpread);// The default spread if we changed it
			}
		}
	}

	public void ApplySpread() // Applies the spread
	{
		foreach(CrosshairParts img in parts)
		{
			img.image.rectTransform.anchoredPosition = img.up * currentSpread;
		}
	}

	public void WiggleCrosshair() // Wiggles the crosshair
	{
		if(allowSpread)
		{
			ChangeCursorSpread(wiggleSpread);
			wiggle = true;
		}
	}

	public void ChangeCursorSpread(float value) // Changes the spread
	{
		if(allowSpread)
		{
			isSpreadWorking = true;
			targetSpread = value;
			spreadT = 0;
		}
	}

	public void rotateCursor(float seconds) //rotates it
	{
		if(allowRotating)
		{
			isRotating = true;
			rotationTimer = seconds;

			if(spreadWhileRotating)
				ChangeCursorSpread(rotationSpeed);
		}
	}

	public static float AccelDecelInterpolation(float start, float end, float t) // Interpolation equation, this is just math
	{
		float x = end - start;
		float newT = (Mathf.Cos((t + 1) * Mathf.PI) / 2) + 0.5f;

		x *= newT;

		float retVal = start + x;
		return retVal;
	}
}