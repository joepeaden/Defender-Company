using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Data for actor controllers. Mostly unique data, shared data can be found in AIControllersData.
/// </summary>
[CreateAssetMenu(fileName = "ControllerData", menuName = "MyScriptables/ControllerData")]
public class ControllerData : ScriptableObject
{
	/// <summary>
    /// Defines how an AI Actor will act.
    /// </summary>
	public enum AIBehaviourType
    {
		Attacker,
		Sapper
    }

	// not all of these are gonna be used. Can split this data up between player and AI if deemed necessary.
	// deemed is a cool word.

	// AI control stuff (and navAgentSpeed which is for both)
	public AIBehaviourType behaviourType;
	public bool canMoveAndShoot;
	public bool canAim;
	/// <summary>
    /// Chance they'll use cover; from 0 - 1 (0% - 100%) (rolled every AiControllersData.decisionRate seconds)
    /// </summary>
	public float useCoverChance;
	public int scoreValue;
	public WeaponData startWeapon;
	public float shootPauseTimeMax;
	public float shootPauseTimeMin;
	public float maxBurstFrames;
	public float minBurstFrames;
	public float navAgentSpeed;

	// player control stuff
	public float baseControllerAimRotaitonSensitivity;
	public float controllerMaxRotationSensitivity;
	public float controllerRotationSensitivity;
}
