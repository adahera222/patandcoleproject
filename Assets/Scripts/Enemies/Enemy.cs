using UnityEngine;
using System.Collections;
using PatStuff;
using System;

public class Enemy : RecyclableGameObject
{
	#region Variables
	
	#region Public
	
	public int hitsToKill = 1;
	public float speed = 10;
	//public Texture2D texture;
	
	#endregion
	#region Private
	
	//private Shader shader;
	private Routine attackPlayerRoutine;

	#endregion
	#endregion
	
	#region Init

	protected override void Start() 
	{
		base.Start ();
		//shader = Shader.Find("Mobile/Unlit (Supports Lightmap)");
	}
	
	public override void OnReused()
	{
		base.OnReused();
		
		SafelyKillRoutine();
		
		attackPlayerRoutine = CoroutineManager.CreateRoutine(MoveTowardsPlayer());
	}
	
	#endregion
	
	#region Methods
	
	void SafelyKillRoutine()
	{
		if (attackPlayerRoutine != null)
		{
			attackPlayerRoutine.Kill();
			attackPlayerRoutine = null;
		}
	}
	
	IEnumerator MoveTowardsPlayer()
	{
		while (Vector3.Distance(MyTransform.position, Launcher.I.projectileSpawn.position) > 2)
		{
			//have the enemy move towards the camera
			MyTransform.LookAt(Launcher.I.projectileSpawn);
			MyTransform.position += myTransform.forward * speed * Time.smoothDeltaTime;
			
			yield return null;
		}
		
		OnReachPlayer(); //we have reached the player
	}
	
	void OnReachPlayer()
	{
		SafelyKillRoutine();
		//KILL CODE HERE
		Recycle();
	}
	
	#endregion
}
