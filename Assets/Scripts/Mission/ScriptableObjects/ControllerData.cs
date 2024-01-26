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
	public WeaponData startWeapon;

	#region AttackData
	/// <summary>
    /// Pause between bursts
    /// </summary>
    public float timeBetweenBursts;
	/// <summary>
	/// Number of projectiles fired in a burst
	/// </summary>
	//public int projectilesInBurst;

	public int accuracyRating;

    #endregion

    public float moveSpeed;

	public int hitPoints;

	public AudioClip woundSound1;
	public AudioClip woundSound2;
	public AudioClip woundSound3;
	public AudioClip woundSound4;
	public AudioClip woundSound5;
	public AudioClip woundSound6;
	public AudioClip deathSound1;
	public AudioClip deathSound2;
	public AudioClip deathSound3;
	public float minSemiAutoFireRate;
	public float maxSemiAutoFireRate;

	public float slowWalkMoveForce;
	public float fastWalkMoveForce;
	public float sprintMoveForce;
}
