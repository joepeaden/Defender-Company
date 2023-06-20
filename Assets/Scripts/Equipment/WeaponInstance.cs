using System.Collections;
using UnityEngine;

/// <summary>
/// Weapon that functions and exists in the game world.
/// </summary>
public class WeaponInstance : MonoBehaviour
{
    public static Vector3 aimDir;
    public InventoryWeapon inventoryWeapon;

    private const string WEAPON_SPRITE_TAG = "WeaponModel";

    // components & child objects
    [SerializeField] private GameObject weaponFlash;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private GameObject aimGlow;
    [SerializeField] private LineRenderer line;
    [SerializeField] private Transform muzzle;

    /// <summary>
    /// Just a debug option.
    /// </summary>
    //[SerializeField] private bool infiniteAmmo;

    // the actor who is using this weapon
    private Actor actor;
    private Actor _Actor
    {
        get
        {
            if (actor == null)
            {
                actor = transform.GetComponentInParent<Actor>();
            }
            return actor;
        }
        set
        {
            actor = value;
        }
    }

    private GameObject weaponSpriteGameObject;
    private Vector3 actorVelocity;
    private int ammoInWeapon;
    // states
    private bool aiming;
    private bool readyToAttack = true;
    private bool reloading;

    [Header("Debug Options")]
    [SerializeField]
    private bool drawAccuracyAngle;

    private void Start()
    {
        _Actor.OnActorBeginAim += BeginAim;
        _Actor.OnActorEndAim += EndAim;
        _Actor.EmitVelocityInfo.AddListener(ReceiveActorVelocityData);
    }

    private void Update()
    {
        if (drawAccuracyAngle)
        {
            float accuracy = GetAccuracy();

            Quaternion projRot = transform.rotation;
            Quaternion ray1Dir = new Quaternion();
            ray1Dir.eulerAngles = new Vector3(projRot.eulerAngles.x, projRot.eulerAngles.y + accuracy, projRot.eulerAngles.z);
            Quaternion ray2Dir = new Quaternion();
            ray2Dir.eulerAngles = new Vector3(projRot.eulerAngles.x, projRot.eulerAngles.y - accuracy, projRot.eulerAngles.z);

            Debug.DrawRay(transform.position, ray1Dir * Vector3.forward * 10f, Color.red);
            Debug.DrawRay(transform.position, ray2Dir * Vector3.forward * 10f, Color.red);
        }
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        readyToAttack = true;
        reloading = false;
    }


    private void BeginAim()
    {
        aiming = true;
        aimGlow.SetActive(true);
        StartCoroutine(ProjectRayCastAndMoveAimGlowToFirstCollision());
    }
    
    private void EndAim()
    {
        aiming = false;
        line.enabled = false;
        aimGlow.SetActive(false);
        StopCoroutine(ProjectRayCastAndMoveAimGlowToFirstCollision());
    }

    public void UpdateWeapon(InventoryWeapon weapon)
    {
        StopAllCoroutines();

        readyToAttack = weapon.amountLoaded > 0;
        reloading = false;

        // save current ammo before swapping
        if (inventoryWeapon != null)
        {
            inventoryWeapon.amountLoaded = ammoInWeapon;
        }

        if (_Actor.IsPlayer)
        {
            // play reload when switch for cools
            PlayAudioClip(weapon.data.reloadSound);
        }

        ammoInWeapon = weapon.amountLoaded;

        if (weaponSpriteGameObject != null)
        {
            Destroy(weaponSpriteGameObject);
            // Destroy doesn't immediately make it null, it happens at end of frame.
            weaponSpriteGameObject = null;
        }
        weaponSpriteGameObject = Instantiate(weapon.data.spritePrefab, transform);
        weaponSpriteGameObject.tag = WEAPON_SPRITE_TAG;

        // update position cause weapons are different lengths
        transform.localPosition = weapon.data.muzzlePosition;
        inventoryWeapon = weapon;
    }

    /// <summary>
    /// Coroutine that projects a raycast from the weapon's position in the direction it is facing, and moves the aim light to the first collision point.
    /// </summary>
    private IEnumerator ProjectRayCastAndMoveAimGlowToFirstCollision()
    {
        while (aiming)
        {
            RaycastHit hit;
            Ray ray = new Ray(transform.position, transform.forward);

            // CLEAN: collision layer stuff is probably too complicated plus names are all outdated.
            int layerMask = LayerMask.GetMask(LayerNames.CollisionLayers.HouseAndFurniture.ToString(), LayerNames.CollisionLayers.Actors.ToString(), LayerNames.CollisionLayers.IgnoreFurniture.ToString(), "PlayerZoneCollider");

            if (Physics.Raycast(ray, out hit, int.MaxValue, layerMask))
            {
                aimGlow.transform.position = hit.point;

                line.enabled = true;
                line.SetPosition(0, transform.position);
                line.SetPosition(1, hit.point);

                if (actor.IsPlayer)
                {
                    aimDir = (hit.point - transform.position).normalized;
                }
            }

            yield return null;
        }
    }

    public bool InitiateAttack(float actorRecoilControl, bool triggerPull)
    {
        if (!reloading && (triggerPull || inventoryWeapon.data.isAutomatic))
        {
            // wait for gun to cycle (fire rate)
            if (readyToAttack)
            {
                LaunchAttack(actorRecoilControl);
                return true;
            }
            // returning false to indicate to player out of ammo
            else if (triggerPull && ammoInWeapon <= 0)
            {
                PlayAudioClip(inventoryWeapon.data.emptyWeaponSound);
            }
        }

        return false;
    }

    private void LaunchAttack(float actorRecoilControl)
    {
        // remove as many bullets as are fired
        ammoInWeapon--;

        readyToAttack = false;

        for (int proj = 0; proj < inventoryWeapon.data.projFiredPerShot; proj++)
        {
            float accuracyAngle = GetAccuracy();

            // make the bullet less accurate
            float rotAdjust = Random.Range(-accuracyAngle / 2, accuracyAngle / 2);
            Quaternion projRot = transform.rotation;
            projRot.eulerAngles = new Vector3(projRot.eulerAngles.x, projRot.eulerAngles.y + rotAdjust, projRot.eulerAngles.z);

            Projectile projectile = Instantiate(inventoryWeapon.data.projectile, transform.position, projRot).GetComponent<Projectile>();
            projectile.Initialize(actor, inventoryWeapon.data, proj);
        }

        StopCoroutine(Flash());
        StartCoroutine(Flash());

        if (ammoInWeapon > 0)
            StartCoroutine("PrepareToAttack");
    }

    /// <summary>
    /// determine accuracy (aiming, is the player standing still?)
    /// </summary>
    /// <returns>The angle that projectiles can be fired in.</returns>
    private float GetAccuracy()
    {
        float accuracyAngle = inventoryWeapon.data.accuracyAngle;
        if (aiming)
        {
            accuracyAngle /= inventoryWeapon.data.aimingBoon;
        }
        if (actorVelocity.magnitude > 1f)
        {
            accuracyAngle *= inventoryWeapon.data.movementAccuracyPenalty;
        }

        return accuracyAngle;
    }

    private IEnumerator Flash()
    {
        weaponFlash.SetActive(true);

        yield return new WaitForSeconds(0.1f);

        weaponFlash.SetActive(false);
    }

    // refers to the time in between shots
    private IEnumerator PrepareToAttack()
    {
        yield return new WaitForSeconds(inventoryWeapon.data.attackInterval);
        readyToAttack = true;
    }

    // returns false if no spare ammunition
    public bool StartReload()
    {
        readyToAttack = false;

        if (!reloading)
        {
            StartCoroutine("Reload");
        }

        reloading = true;

        return true;
    }

    private IEnumerator Reload()
    {
        PlayAudioClip(inventoryWeapon.data.reloadSound);

        if (_Actor.IsPlayer)
        {
            GameplayUI.Instance.StartReloadBarAnimation(inventoryWeapon.data.reloadTime);
        }

        yield return new WaitForSeconds(inventoryWeapon.data.reloadTime);

        audioSource.Stop();

        // just some mafths. first IF is if there's not enough ammo for a full mag left, second is otherwise.
        int amountNeeded = inventoryWeapon.data.ammoCapacity - ammoInWeapon;
        inventoryWeapon.amount = inventoryWeapon.amount - amountNeeded;
        if (inventoryWeapon.amount < 0)
        {
            inventoryWeapon.amount = 0;
            ammoInWeapon = inventoryWeapon.data.ammoCapacity + inventoryWeapon.amount;
        
        }
        else
        {
            ammoInWeapon = inventoryWeapon.data.ammoCapacity;
        }
            
        // if infinite ammo or it's not the player, don't deplete backup ammo.
        if (inventoryWeapon.data.hasInfiniteBackupAmmo|| !_Actor.IsPlayer)
        {
            inventoryWeapon.amount = inventoryWeapon.data.totalAmount;
        }

        readyToAttack = true;
        reloading = false;
    }

    private void PlayAudioClip(AudioClip clip)
    {
        // don't need to set this slomo every time can just do it in an event once.
        audioSource.pitch = GameManager.isSlowMotion ? GameManager.slowMotionSpeed : 1f;
        audioSource.clip = clip;
        audioSource.time = 0;
        audioSource.Play();
    }

    public void AddAmmo(int ammo)
    {
        ammoInWeapon += ammo;
        StartCoroutine("PrepareToAttack");
    }

    public int GetAmmo()
    {
        return ammoInWeapon;
    }

    public bool HasAmmo()
    {
        return ammoInWeapon > 0;
    }

    public string GetName()
    {
        return inventoryWeapon.data.displayName;
    }

    private void ReceiveActorVelocityData(Vector3 vel)
    {
        actorVelocity = vel;
    }

    private void OnDestroy()
    {
        _Actor.OnActorBeginAim -= BeginAim;
        _Actor.OnActorEndAim -= EndAim;
        _Actor.EmitVelocityInfo.RemoveListener(ReceiveActorVelocityData);
    }
}
