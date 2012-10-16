using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PatStuff;

public class EnemyWaveController : MonoBehaviour 
{
	//Only need 1 basic enemy prefab. The Enemy script will randomize shader,color, and size
	
	#region Public Variables

	public List<Transform> SpawnPositions;
	
	public static int Wave;
	
	public static int EnemiesToSpawn = 100;
	
	public static int EnemiesRemaining;
	
	public float enemySpawnTimeInSeconds = 2.0f;
			
	public Enemy enemyPrefab;
	#endregion
	
	
	#region Private Variables
	
	private bool _waveComplete = false;
	
	#endregion
	
	#region Init
	
	void Start () 
	{
		
		RecycleBin.CreateNewRecylable<Enemy>(enemyPrefab);
		Timer.CreateMillisecondTimer(Timer.ConvertSecondsToMilliseconds(enemySpawnTimeInSeconds), SpawnNewEnemy);
	}
	
	#if UNITY_EDITOR
		
	void OnDrawGizmos()
	{
		foreach(Transform t in SpawnPositions)
			Gizmos.DrawIcon(t.position, "Babe", true);
	}
	
	#endif
	
	#endregion
	
	#region Methods
	
	void SpawnNewEnemy(float elapsedTime)
	{
		if (_waveComplete == true)
			return;
		
		System.Random rand = new System.Random();
		
		int newSpawnIndex = rand.Next(SpawnPositions.Count); //get random number within SpawnPositions Range
		
		Enemy newEnemy = RecycleBin.GetFreeObject<Enemy>(); //get free enemy
		
		Transform newEnemyTransform = SpawnPositions[newSpawnIndex];
		
		//Spawnpoints have SpawnParticleEffects. Play them when we spawn an enemy
		newEnemyTransform.GetComponentInChildren<ParticleSystem>().Play ();
		
		newEnemy.MyTransform.parent = newEnemyTransform ; //set its spawn and posish
		newEnemy.MyTransform.localPosition = Vector3.zero;
		
		Timer.CreateMillisecondTimer(Timer.ConvertSecondsToMilliseconds(enemySpawnTimeInSeconds), SpawnNewEnemy); //set timer to spawn another enemy
	}
	
	#endregion
}
