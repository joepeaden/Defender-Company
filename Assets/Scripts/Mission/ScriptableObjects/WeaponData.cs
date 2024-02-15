using UnityEngine;

[CreateAssetMenu(fileName = "WeaponData", menuName = "MyScriptables/WeaponData")]
public class WeaponData : GearData
{
	[Header("Basic Stats")]
	/// <summary>
	/// Damage caused on hitting target
	/// </summary>
	public int damage;
	/// <summary>
	/// Range where an AI actor can fire the weapon
	/// </summary>
	public int range;
	/// <summary>
    /// 
    /// </summary>
	public int projectileLifetime;
	/// <summary>
	/// Time it takes to prepare another shot
	/// </summary>
	public float rateOfFire;
	/// <summary>
	/// When an AI actor fires this weapon, how many shots do they fire?
	/// </summary>
	public int shotPerBurst;
	/// <summary>
	/// How much ammo the weapon can hold in a magazine
	/// </summary>
	public int ammoCapacity;
	/// <summary>
	/// Time to reload
	/// </summary>
	public float reloadTime;
	/// <summary>
	/// Is the weapon automatic?
	/// </summary>
	public bool isAutomatic;

	[Header("Audio")]
	/// <summary>
    /// Audio when weapon is used to attack
    /// </summary>
	public AudioClip attackSound;
	/// <summary>
    /// Audio when weapon is out of ammo
    /// </summary>
	public AudioClip emptyWeaponSound;
	/// <summary>
    /// Reload sound effect
    /// </summary>
	public AudioClip reloadSound;

	[Header("Sprites")]
	/// <summary>
    /// Left-facing sprite
    /// </summary>
	public Sprite leftSprite;
	/// <summary>
	/// Right-facing sprite
	/// </summary>
	public Sprite rightSprite;
	/// <summary>
	/// Sprite for projectile
	/// </summary>
	public Sprite projSprite;

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

	[Header("Projectile")]
	/// <summary>
    /// Is this an instant-hit kind of projectile (single instant raycast on fire), or a slow-moving (raycast every frame) 
    /// </summary>
	public bool isHitScan;
	/// <summary>
    /// Length of projectile's collider
    /// </summary>
	public float projColLength;
	/// <summary>
    /// Width of projectile's collider
    /// </summary>
	public float projColWidth;
	/// <summary>
    /// Velocity of projectile
    /// </summary>
	public float projVelocity;
	/// <summary>
	/// How many projectiles does this fire each trigger pull (i.e. shotguns)
	/// </summary>
	public int projPerShot;
}
