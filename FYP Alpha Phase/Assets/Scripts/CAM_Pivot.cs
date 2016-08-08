using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CAM_Pivot : CAM_Follow 
{
	protected Transform cam;
	protected Transform pivot;
	protected Vector3 lastTargetPos;

	protected virtual void Awake()
	{
		cam = Camera.main.transform;
		pivot = cam.parent.parent.parent;
	}

	protected override void Start() 
	{
		base.Start();
	}

	virtual protected void Update() 
	{
		if(!Application.isPlaying)
		{
			if(target != null)
			{
				Follow(999f);
				lastTargetPos = target.position;
			}

			if(Mathf.Abs(cam.localPosition.x) > .5f || Mathf.Abs(cam.localPosition.y) > .5f)
			{
				cam.localPosition = Vector3.Scale(cam.localPosition, Vector3.forward);
			}

			cam.localPosition = Vector3.Scale(cam.localPosition, Vector3.forward);
		}
	}

	protected override void Follow(float deltaTime)
	{

	}
}
