using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ControllerData", menuName = "MyScriptables/ControllerData")]
public class ControllerData : ScriptableObject
{
	// not all of these are gonna be used. Can split this data up between player and AI if deemed necessary.
	// deemed is a cool word.

	// AI control stuff (and navAgentSpeed which is for both)
	public bool canMoveAndShoot;
	public bool canAim;
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
