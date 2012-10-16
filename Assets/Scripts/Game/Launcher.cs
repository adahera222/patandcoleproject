using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PatStuff;

public sealed class Launcher : TSingleton<Launcher>
{
	#region Variables
	
	public Transform projectileSpawn;
	
	public Projectile projetilePrefab;

    private Projectile currentProjectile;
	public Projectile CurrentProjectile
	{
		get{ return currentProjectile; }
		set{ currentProjectile = value; }
	}
		
	#region ThrowStuff

   	public float respawnIntervalMilliseconds = 200;
	public float clampAngle = 45;
    private Vector2 touchStartPosition;
    private float startTouchTime;
    public float minSwipeTime = 0.13f;
    public float maxSwipeTime = 0.8f;
    private float minSwipeDistance;
    private float maxSwipeDistance;
    private float maxPossibleSpeed;
    public int maxVerticalLaunchForce = 35000;
    public Vector2 throwVelocity = new Vector2(45000, 21);
    private float throwPower;
    public float swipeWidthAsPercent = 0.3f;
    private float swipeMinX = 6f;
    private float swipeMaxX = 12f;
	
	#endregion
	
	#endregion
	
	#region Init
	
	protected override void Awake()
	{
		base.Awake();
		Physics.gravity = new Vector3(0, -50, 0);
		Application.targetFrameRate = 60;
		ScreenLog.AddMessage("Set Framerate to 60");
	}

    void Start()
    {
        swipeMinX = (Screen.width / 2) - (Screen.width * swipeWidthAsPercent / 2);
        swipeMaxX = (Screen.width / 2) + (Screen.width * swipeWidthAsPercent / 2);
		
        minSwipeDistance = Screen.height / 6;
        maxSwipeDistance = Screen.height / 1.3f;
        maxPossibleSpeed = maxSwipeDistance / minSwipeTime;
		
		FingerGestures.OnFingerDragBegin += OnTouchStart;
		FingerGestures.OnFingerDragEnd += OnTouchEnd;
		
		RecycleBin.CreateNewRecylable<Projectile>(projetilePrefab);
		GetNewProjectile(0.0f);
    }
	
	void GetNewProjectile(float elapsedTime)
	{
		currentProjectile = RecycleBin.GetFreeObject<Projectile>();
		
		if (currentProjectile != null)
		{
			currentProjectile.MyTransform.parent = projectileSpawn;
			currentProjectile.MyTransform.localPosition = Vector3.zero;
		}
	}
	
	#endregion
	
	#region Methods
	
	void OnTouchStart(int fingerIndex, Vector2 fingerPos, Vector2 startPos)
	{
		if (fingerIndex != 0 || instructionPopup != null)
			return;
		
		touchStartPosition = startPos;
        startTouchTime = Time.time;
	}
	
	void OnTouchEnd(int fingerIndex, Vector2 fingerPos)
	{
		if (fingerIndex != 0)
			return;
		
		Vector2 touchEndPosition = fingerPos;

        float currentSwipeTime = Time.time - startTouchTime;
        float clampedSwipeTime = Mathf.Clamp(currentSwipeTime, minSwipeTime, maxSwipeTime);
		
        if (touchStartPosition.x > swipeMinX && touchStartPosition.x < swipeMaxX &&
            touchStartPosition.y < minSwipeDistance &&
			touchStartPosition.y < touchEndPosition.y && currentSwipeTime < maxSwipeTime)
        {
            float currentThrowAngle = ThrowAngle(touchStartPosition, touchEndPosition);

            float currentSwipeDistance = Vector2.Distance(touchStartPosition, touchEndPosition);

            float overallSwipe = currentSwipeDistance / clampedSwipeTime;

            if (overallSwipe > maxPossibleSpeed)
                overallSwipe = maxPossibleSpeed;
			
			if (currentSwipeDistance > minSwipeDistance)
                Launch(currentThrowAngle, overallSwipe, throwVelocity);
        }
	}

    void Launch(float angle, float swipePower, Vector2 swipeVelocity)
    {
        if (currentProjectile != null)
        {
			currentProjectile.MyTransform.position = projectileSpawn.position;
			currentProjectile.constantForce.enabled = false;
			
			projectileSpawn.localRotation = Quaternion.Euler(new Vector3(0, angle, 0));

            float forceY = Mathf.Clamp((swipeVelocity.y * swipePower * 0.6f), 0, maxVerticalLaunchForce);

            currentProjectile.Launch();
			
			//adds forward force
			currentProjectile.rigidbody.AddForce(projectileSpawn.forward.x * swipeVelocity.x, projectileSpawn.forward.y + forceY, projectileSpawn.forward.z * swipeVelocity.x, ForceMode.Impulse);
			
			currentProjectile.constantForce.torque = new Vector3(0, 0, angle * 300); //makes it look like its rotating
			Timer.CreateMillisecondTimer<Projectile, float>(250, ResetProjectileForceAfterTime, currentProjectile, angle);	
			
			currentProjectile.MyTransform.parent = null;
          	currentProjectile = null;
			
			Timer.CreateMillisecondTimer(respawnIntervalMilliseconds, GetNewProjectile);
        }
    }
	
	/// <summary>
	/// Resets the "curve" we put on the projectile
	/// </summary>
	/// <param name='elapsedTime'>
	/// Elapsed time.
	/// </param>
	/// <param name='affectedProjectile'>
	/// Affected projectile.
	/// </param>
	/// <param name='angle'>
	/// Angle.
	/// </param>
	void ResetProjectileForceAfterTime(float elapsedTime, Projectile affectedProjectile, float angle)
	{
		if (affectedProjectile.gameObject.active == true)
			affectedProjectile.constantForce.force = new Vector3(angle * -600, affectedProjectile.constantForce.force.y, 0); //angle * -val will make it curve the oposite way!
	}

    float ThrowAngle(Vector2 pointOne, Vector2 pointTwo)
    {
       float x1 =  (pointOne.x < pointTwo.x) ? Mathf.Abs(pointOne.x - pointTwo.x) :  Mathf.Abs(pointTwo.x - pointOne.x);

        float y1 = Mathf.Abs(pointOne.y - pointTwo.y);
        float angle = Mathf.Atan(x1 / y1);
        angle *= Mathf.Rad2Deg;

        if (pointOne.x > pointTwo.x)
            angle *= -1;

        return Mathf.Clamp(angle, -clampAngle, clampAngle);
    }
	
	OneButtonPopup instructionPopup;
	
	void OnGUI()
	{
		if (GUI.Button(new Rect(Screen.width - 100, Screen.height - 50, 100, 50), "Instructions"))	
		{
			if (instructionPopup != null)
				return;
			
			instructionPopup = PopupManager.CreatePopup<OneButtonPopup>(false) as OneButtonPopup;
			instructionPopup.titleText.Text = "Instructions";
			instructionPopup.messageText.Text = "Swipe with finger or mouse from ball at angle you want to throw!";
			instructionPopup.cancelButton.Text = "OK NIGGA!";
		}
	}
	
	#endregion
}