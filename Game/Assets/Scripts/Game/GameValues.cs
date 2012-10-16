using UnityEngine;
using System.Collections;

/// <summary>
/// A class of all constants used in game
/// </summary>
public class GameValues
{
	#region EZGUI
	
	public static readonly float defaultDragThreshHold = 25f;
	
	public static readonly POINTER_INFO.INPUT_EVENT defaultInputEvent = POINTER_INFO.INPUT_EVENT.TAP;
	public static readonly EZAnimation.EASING_TYPE defaultEasingType = EZAnimation.EASING_TYPE.QuadraticInOut;
	
	public static readonly float defaultPopupAnimationTime = 0.4f;
	
	public static readonly float buttonTextPaddingPercentage = 0.15f; //0.5 = 50%
	
	#endregion
	
	#region FileIO
	
	public static readonly string popupPath = "/Popups/";
	
	#endregion
}