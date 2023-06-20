using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ControllerData", menuName = "MyScriptables/ControllerData")]
public class ControllerData : ScriptableObject
{
	public bool canMoveAndShoot;
	public bool canAim;
	public int scoreValue;
	public WeaponData startWeapon;

	public float shootPauseTimeMax;
	public float shootPauseTimeMin;
	public float maxBurstFrames;
	public float minBurstFrames;

	public float navAgentSpeed;
}
