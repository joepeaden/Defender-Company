using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Data for actor controllers. Mostly unique data, shared data can be found in AIControllersData.
/// </summary>
[CreateAssetMenu(fileName = "ControllerData", menuName = "MyScriptables/ControllerData")]
public class ControllerData : ScriptableObject
{
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
}
