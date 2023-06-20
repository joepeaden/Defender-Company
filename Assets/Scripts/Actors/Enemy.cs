 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Events;

/// <summary>
/// Base class for enemy actors.
/// </summary>
/// <remarks>
/// It probably will be worth it to have an AIActor subclass and take some of the functionality from here.
/// Stuff like the NavMesh and "perception"
/// </remarks>
public class Enemy : MonoBehaviour, ISetActive
{
	public static UnityEvent<int> OnEnemyKilled = new UnityEvent<int>();

	[SerializeField]
	private ControllerData data;

	public bool activateOnStart;

	private Actor actor;
	private bool pauseFurtherAttacks;
	private GameObject target;
	// a little lazy here
	private bool isReloading;
	private bool recoveringFromHit;

	private void Awake()
    {
		// probably should make like an init method for the actor.... but since it's really just two controllers and not likely to grow...
		// whatever.
		actor = GetComponent<Actor>();
		actor.team = Actor.ActorTeam.Enemy;
		actor.SetAgentSpeed(data.navAgentSpeed);
		actor.OnGetHit.AddListener(RecoverFromHit);
    }

	private void Start()
	{
		actor.AddCoverListener(ActorHasPotentialCover);
		actor.OnDeath.AddListener(HandleEnemyDeath);

		actor.SetWeaponFromData(data.startWeapon);

		if (activateOnStart)
		{
			Activate();
		}

		if (actor.IsAlive)
		{
			WaveManager.totalEnemiesAlive++;
		}
	}

    private void OnDestroy()
    {
		actor.OnGetHit.RemoveListener(RecoverFromHit);
	}

    private void Update()
    {
		// kind of a yucky update method. please clean up before messing with it more.

		if (actor.IsAlive && target != null)
		{
			if (actor.GetEquippedWeaponAmmo() <= 0)
			{
				if (!isReloading)
				{
					isReloading = actor.AttemptReload();
				}
			}
			else
			{
				isReloading = false;
			}

			if (isReloading || recoveringFromHit)
			{
				actor.StopMoving();
			}

			if (!isReloading && !recoveringFromHit)
			{
				(bool targetInRangeAndLOS, bool targetInOptimalRange) = TargetInRangeAndLOS();
				if (targetInRangeAndLOS)
				{
					if (targetInOptimalRange)
					{
						actor.StopMoving();
					}
					else
					{
						if (data.canAim)
						{
							actor.EndAiming();
						}

						actor.Move(target.transform.position);
					}

					// if we're close enough to stop and shoot, OR if we're dope enough to move and shoot
					if (targetInOptimalRange || data.canMoveAndShoot)
					{
						actor.UpdateActorRotation(target.transform.position);

						if (!pauseFurtherAttacks)
						{
							int numToFire = (int)Random.Range(data.minBurstFrames, data.maxBurstFrames);

							StartCoroutine(FireBurst(numToFire));
						}
					}

				}
				else
				{
					actor.Move(target.transform.position);
				}
			}
		}
	}

	private void RecoverFromHit(Projectile p)
	{
		StartCoroutine(RecoverFromHitCoroutine());
	}
	
	/// <summary>
    /// Basically for a stagger effect.
    /// </summary>
    /// <returns></returns>
	private IEnumerator RecoverFromHitCoroutine()
    {
		recoveringFromHit = true;

		// don't feel like making this configurable - I'm lazy and trying to actually finish this thing.
		yield return new WaitForSeconds(.5f);

		recoveringFromHit = false;
    }

	/// <summary>
	/// Check if the actor's current target is in range and in LOS
	/// </summary>
	/// <returns>A pair of bools: (TargetInRangeAndLOS, TargetAtOptimalRange)</returns>
	private (bool, bool) TargetInRangeAndLOS()
    {
		InventoryWeapon weapon = actor.GetEquippedWeapon();

		// cast overlap sphere with radius = range to see if target is possibly in range
		Collider[] hitColliders = Physics.OverlapSphere(transform.position, weapon.data.range, LayerMask.GetMask("Actors"), QueryTriggerInteraction.Collide);
		foreach (Collider c in hitColliders.Where(col => col.GetComponent<HitBox>() != null))
        {
			HitBox h = c.GetComponent<HitBox>();
			// if it's the target, then check if have LOS of target
			if (h.GetActor().gameObject == target)
            {
				Ray r = new Ray(transform.position, (target.transform.position - transform.position));
				RaycastHit[] hits = Physics.RaycastAll(r, weapon.data.range);

				RaycastHit[] targetHits = hits.Where(hit => hit.collider.GetComponent<HitBox>() != null && hit.collider.GetComponent<HitBox>().GetActor().gameObject == target).ToArray();
				RaycastHit[] blockHits = hits.Where(hit => hit.collider.gameObject.layer == (int) LayerNames.CollisionLayers.HouseAndFurniture).ToArray();
				
				// should only be one or zero targetHits. Check if any blocking hit is closer than the target, if so, can't shoot 
				foreach (RaycastHit targetHit in targetHits)
                {
					foreach (RaycastHit blockHit in blockHits)
					{
						if (blockHit.distance < targetHit.distance)
						{
							return (false, false);
						}
					}
				}

				// if we made it here and hit a target, then we do indeed have LOS and Range (otherwise would have returned or skipped this block)
				bool targetInRangeAndLOS = targetHits.Count() > 0;
				bool targetInOptimalRange = targetInRangeAndLOS ? ((target.transform.position - transform.position).magnitude <= weapon.data.optimalRange) : false;

				return (targetInRangeAndLOS, targetInOptimalRange);
            }
        }

		return (false, false);
	}

	/// <summary>
    /// Begin lookng for player and show model.
    /// </summary>
	public void Activate()
    {
		actor.SetVisibility(true);

		if (actor.IsAlive)
		{
			pauseFurtherAttacks = false;
			StartCoroutine(LookForTarget());
		}
	}

	/// <summary>
    /// Hide model, and stop looking for players or doing anything else.
    /// </summary>
	public void DeActivate()
	{
		actor.SetVisibility(false);

		if (actor.IsAlive)
		{
			pauseFurtherAttacks = true;
			target = null;
			StopAllCoroutines();

			// if remove this move order, the actor goes to last player position. Might want it to be like that down the line. Just something to consider.
			//actor.Move(transform.position);
		}
	}

	private IEnumerator LookForTarget()
    {
		// they know where you are.
		target = GameManager.Instance.GetPlayerGO();
		actor.target = target.GetComponent<Actor>().GetShootAtMeTransform();
		yield return null;
	}

	/// <summary>
	/// For now, just rotates the actor 90 degrees to look ded.
	/// </summary>
	private void HandleEnemyDeath()
    {
		StopAllCoroutines();

		if (!actor.IsAlive)
		{
			WaveManager.totalEnemiesAlive--;
		}

		OnEnemyKilled.Invoke(data.scoreValue);
	}

	/// <summary>
    /// Was used for cover system but didn't implement yet.
    /// </summary>
	private void ActorHasPotentialCover()
    {
		//Debug.Log("Enemy sees potential cover");
    }

	private IEnumerator FireBurst(int numToFire)
    {
		if (data.canAim)
		{
			actor.BeginAiming();
		}

		pauseFurtherAttacks = true;

		yield return new WaitForSeconds(1f);

		int initialWeaponAmmo = actor.GetEquippedWeaponAmmo();
		int currentWeaponAmmo = initialWeaponAmmo;

		while (numToFire > 0 && currentWeaponAmmo > 0)
        {
			// if it's the first shot, make sure to pass triggerpull param correctly.
            actor.AttemptAttack(true);
			currentWeaponAmmo = actor.GetEquippedWeaponAmmo();

			numToFire--;

			InventoryWeapon weapon = actor.GetEquippedWeapon();
			if (!weapon.data.isAutomatic)
			{
				yield return new WaitForSeconds(Random.Range(actor.data.minSemiAutoFireRate, actor.data.maxSemiAutoFireRate));
			}
            else
            {
				yield return null;
			}
		}

		actor.EndAiming();

		// the -1 is to account for the 1 second pause at beginning
		yield return new WaitForSeconds(Random.Range(data.shootPauseTimeMin, data.shootPauseTimeMax) - 1f);

		pauseFurtherAttacks = false;
	}
}
