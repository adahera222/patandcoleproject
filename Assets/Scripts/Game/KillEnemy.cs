using UnityEngine;
using System.Collections;
using PatStuff;
//Attach this to the particle effect gameobject, like I attach my lips to Meri's bumhole

public class KillEnemy : MonoBehaviour
{

	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
	void OnTriggerEnter(Collider col)
	{
		if(col.CompareTag("Grenade"))
		{
			//kill code
			//need to call it from the parent
			transform.parent.GetComponent<Enemy>().Recycle();	
		}
	}
}
