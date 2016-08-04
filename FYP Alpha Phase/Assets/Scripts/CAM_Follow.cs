using UnityEngine;

public abstract class CAM_Follow : MonoBehaviour 
{
	[Header("Target player")]
	public Transform target;
	public bool autoTarget = true;

	virtual protected void Start() 
	{
		if(autoTarget)
			FindTarget();
	}

	void FixedUpdate() 
	{
		if(target == null)
			return;

		Follow(Time.deltaTime);
	}

	protected abstract void Follow(float deltaTime);

	public void FindTarget()
	{
		if(!target)
		{
			GameObject player = GameObject.FindGameObjectWithTag("Player");

			if(player)
				SetTarget(player.transform);
			else
				Debug.Log("Player is null. Please assign player tag to player!");
		}
	}

	public virtual void SetTarget(Transform newTrans)
	{
		target = newTrans;
	}
}
