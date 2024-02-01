using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Holds data for friendly and enemy types
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

	[Header("AI")]
	#region AI Stuff
	public AIBehaviourType behaviourType;
	public bool canMoveAndShoot;
	public bool canAim;
	public WeaponData startWeapon;
	public int accuracyRating;
	/// <summary>
    /// How long the actor waits between shots
    /// </summary>
	public float pauseBetweenBursts;
	public float moveSpeed;
	public int hitPoints;
	#endregion

	[Header("Visuals")]
	#region Visuals
	public Sprite sprite;
	public List<Sprite> bloodSplats;
	public List<Sprite> bodyPartSprites;
	public List<Sprite> deadSprites;
	#endregion

	#region Audio
	[Header("Audio")]
	public AudioClip woundSound1;
	public AudioClip woundSound2;
	public AudioClip woundSound3;
	public AudioClip woundSound4;
	public AudioClip woundSound5;
	public AudioClip woundSound6;
	public AudioClip deathSound1;
	public AudioClip deathSound2;
	public AudioClip deathSound3;
	#endregion


	[Header("Player")]
	// may wanna move this to another SO sometime. Or just set it as a constant in script. Because probably not unique
	#region Player Controlled Stuff
	public float slowWalkMoveForce;
	public float fastWalkMoveForce;
	public float sprintMoveForce;
    #endregion
}
