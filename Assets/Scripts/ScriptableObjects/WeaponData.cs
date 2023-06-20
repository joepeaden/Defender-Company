using UnityEngine;

[CreateAssetMenu(fileName = "WeaponData", menuName = "MyScriptables/WeaponData")]
public class WeaponData : ShopRewardData
{
	public GameObject projectile;
	public float recoil;
	public float attackInterval;
	public int range;
	public int optimalRange;
	public int ammoCapacity;
	/// <summary>
    /// Refers to total amount of ammo
    /// </summary>
	public int totalAmount;
	public AudioClip attackSound;
	public AudioClip emptyWeaponSound;
	public AudioClip reloadSound;
	public float reloadTime;
	public bool isAutomatic;
	public bool isPistol;
	public int projFiredPerShot;
	public Vector3 muzzlePosition;

	[Header("Model Info")]
	public GameObject spritePrefab;
	//public Vector3 modelPosition;
	//public Vector3 modelRotation;

	public bool hasInfiniteBackupAmmo;

	[Header("Accuracy")]
	/// <summary>
	/// The arc that bullets may be rotated in. (degrees)
	/// Lower numbers are more accurate.
	/// </summary>
	public float accuracyAngle;
	/// <summary>
	/// Used in accuracy calculation; accuracyAngle is multiplied by this.
	/// Lower is more accurate.
	/// </summary>
	public float movementAccuracyPenalty;
	/// <summary>
	/// Used in accuracy calculation; accuracyAngle is divided by this.
    /// Higher is more accurate.
	/// </summary>
	public float aimingBoon;
}
