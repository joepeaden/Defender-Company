using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public abstract class ActorController : MonoBehaviour
{
    /// <summary>
    /// A per-actor death event (OnActorKilled in AIActorController is a static event)
    /// </summary>
    [HideInInspector]
    public UnityEvent OnActorDeath = new UnityEvent();

    public ControllerData Data => data;
    [SerializeField]
    protected ControllerData data;
    protected Actor actor;
    public bool PauseFurtherAttacks => pauseFurtherAttacks;
    protected bool pauseFurtherAttacks;

    protected void Awake()
    {
        actor = GetComponent<Actor>();
        actor.Initialize(data);
        actor.OnDeath.AddListener(HandleDeath);
        actor.OnGetHit.AddListener(HandleGetHit);
    }

    protected void OnDestroy()
    {
        actor.OnDeath.RemoveListener(HandleDeath);
    }

    /// <summary>
    /// Some times ya get hit. But you gotta just get up. Keep pushing forward. Don't stop. You can do this.
    /// </summary>
    protected abstract void HandleGetHit();

    /// <summary>
    /// Think about life. How it went. Did you do all you could? Did you say what you needed to say?
    /// </summary>
    protected virtual void HandleDeath(bool fromExplosive)
    {
        StopAllCoroutines();
        enabled = false;

        OnActorDeath.Invoke();
    }

    /// <summary>
    /// Get the actor script.
    /// </summary>
    /// <returns></returns>
    public Actor GetActor()
    {
        return actor;
    }

    public void SetControllerData(ControllerData newControllerData)
    {
        data = newControllerData;
    }

    public IEnumerator FireBurst(int numToFire)
    {
        if (data.canAim && !actor.IsPlayer)
        {
            actor.BeginAiming();
            // wait and aim for a second
            yield return new WaitForSeconds(1f);
        }

        pauseFurtherAttacks = true;

        InventoryWeapon equippedWeapon = actor.GetEquippedWeapon();

        int initialWeaponAmmo = actor.GetEquippedWeaponAmmo();
        int currentWeaponAmmo = initialWeaponAmmo;

        while (numToFire > 0 && currentWeaponAmmo > 0)// && (actor.IsPlayer || targetInLOS))
        {
            // if it's the first shot, make sure to pass triggerpull param correctly.
            actor.AttemptAttack(true);
            currentWeaponAmmo = actor.GetEquippedWeaponAmmo();

            numToFire--;

            yield return new WaitUntil(() => actor.GetWeaponInstance().IsReadyToAttack());
        }

        if (data.canAim && !actor.IsPlayer)
        {
            actor.EndAiming();
        }

        yield return new WaitForSeconds(data.timeBetweenBursts);

        pauseFurtherAttacks = false;
    }
}
