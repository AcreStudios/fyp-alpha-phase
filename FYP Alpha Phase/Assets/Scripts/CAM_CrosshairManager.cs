using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CAM_CrosshairManager : MonoBehaviour 
{
	public int index;
	public CAM_Crosshair activeCrosshair;
	public CAM_Crosshair[] crosshairs;

	public static CAM_CrosshairManager instance;
	public static CAM_CrosshairManager GetInstance()
	{
		return instance;
	}

	void Awake()
	{
		instance = this;
	}

	void Start() 
	{
		for(int i = 0; i < crosshairs.Length; i++)
		{
			crosshairs[i].gameObject.SetActive(false);
		}

		crosshairs[index].gameObject.SetActive(true);
		activeCrosshair = crosshairs[index];
	}

	public void DefineCrosshairByIndex(int findIndex)
	{
		activeCrosshair = crosshairs[findIndex];
	}

	public void DefineCrosshairByName(string findName)
	{
		for(int i = 0; i < crosshairs.Length; i++)
		{
			if(string.Equals(crosshairs[i].name, findName))
			{
				activeCrosshair = crosshairs[i];
				break;
			}
		}
	}
}
