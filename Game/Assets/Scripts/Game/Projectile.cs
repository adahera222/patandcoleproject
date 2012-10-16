using UnityEngine;
using System.Collections;
using PatStuff;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(ConstantForce))]
public class Projectile : RecyclableGameObject
{
	
	#region Variables
	
	public AudioClip launchSound;
	public float timeoutInSeconds = 6.0f;
	public float explosionRadius = 2.0f;
	
	private ParticleSystem myParticles;
	private bool hasExploded = false;
	private GameObject explosionTrigger;
	#endregion
	
	#region Init
	
	protected override void Start()
	{
		base.Start();
		
		myParticles = GetComponentInChildren(typeof(ParticleSystem)) as ParticleSystem;
		explosionTrigger = transform.Find("ExplosionCollider").gameObject;
		explosionTrigger.active = false;
	}
	
	#endregion
	
	#region Methods
	
	public void Launch()
	{
		hasExploded = false;
		
		collider.isTrigger = false;
		rigidbody.useGravity = true;
		constantForce.enabled = true;
		SoundManager.PlaySoundOnce(launchSound);
		//Timer.CreateMillisecondTimer(Timer.ConvertSecondsToMilliseconds(timeoutInSeconds), OnTimeOut);
	}
	
	void OnTimeOut(float elapsedTime)
	{
		Recycle();
	}
	
	public override void Recycle()
	{
		base.Recycle();
		
		constantForce.enabled = false;
		rigidbody.useGravity = false;
		rigidbody.velocity = Vector3.zero;
		rigidbody.angularVelocity = Vector3.zero;
		
	}
	
	public override void OnReused()
	{
		base.OnReused ();
		
		SetStateToIdle();
	}
	
	void SetStateToIdle()
	{
		collider.isTrigger = true;
		
		 if (constantForce.force.y != -10000)
			constantForce.force = new Vector3(0, -10000, 0);
		
		constantForce.enabled = false;
		rigidbody.useGravity = false;
		transform.FindChild("ExplosionCollider").gameObject.active = false;
	}
	
	void OnCollisionEnter(Collision collision)
	{
		constantForce.force = Vector3.zero;
		constantForce.torque = Vector3.zero;
		
		if (myParticles != null && hasExploded == false)
		{
			hasExploded = true;
			myParticles.Play();
			//Need to activate the collider quickly here
			explosionTrigger.active = true;
			//explosionTrigger.isTrigger = true;
			//Will need to deactivate it after the explosion... do that later.
			Timer.CreateMillisecondTimer(Timer.ConvertSecondsToMilliseconds(0.3f), OnTimeOut);
			
		}
	}
	
	#endregion
}