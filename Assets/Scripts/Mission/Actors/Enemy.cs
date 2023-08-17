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
public class Enemy : ActorController, ISetActive
{
	public static UnityEvent OnEnemyKilled = new UnityEvent();
	public static UnityEvent OnEnemySpawned = new UnityEvent();

	public bool activateOnStart;
	[SerializeField] private AIControllersData aiData;

	//private enum AIMovementStates
 //   {
	//	SeekingPlayer,
	//	SeekingCover,
	//	InCover,
	//	StandingStill
 //   }

	//private enum AIAttackingStates
 //   {
	//	NotAttacking,
	//	Reloading,
	//	Attacking
 //   }

	private bool pauseFurtherAttacks;
	private bool isReloading;
	private bool recoveringFromHit;
	private GameObject target;
	private bool targetInRange;
	private bool targetInOptimalRange;

	// these two are actually used by the AIPoppingOutOfCoverState class so they are needed. For now.
	public bool TargetInLOS => targetInLOS;
	private bool targetInLOS;
	private bool targetInLOSIgnoreFloorCover;

	private bool takingCover;
	private bool poppingOutOfCover;
	private bool lookingForCover;
	/// <summary>
    /// Is the enemy taking horizontal cover (for the purpose of how to behave taking the cover.
    /// </summary>
	private bool inHorizontalCover;

    private new void Awake()
    {
		base.Awake();

		aiState = new AIMovingToTargetState();
    }

    private new void Start()
	{
		base.Start();


		//actor.AddCoverListener(ActorHasPotentialCover);
		actor.SetWeaponFromData(data.startWeapon);

		if (activateOnStart)
		{
			Activate();
		}

		OnEnemySpawned.Invoke();
	}

	AIState aiState;
	float timeSinceLastDecision;

	//AIInput input;
	private void Update()
	{
		if (actor.IsAlive)
		{
			timeSinceLastDecision += Time.deltaTime;
			if (timeSinceLastDecision > aiData.decisionInterval)
			{
				timeSinceLastDecision = 0f;
			}
			targetInRange = IsTargetInRange(false);
			targetInOptimalRange = IsTargetInRange(true);
			targetInLOS = IsTargetInLOS(false);
			targetInLOSIgnoreFloorCover = IsTargetInLOS(true);

			AIInput newInput = new AIInput
			{
				timeForDecision = timeSinceLastDecision == 0f,
				targetInRange = targetInRange,
				targetInOptimalRange = targetInOptimalRange,
				targetInLOS = targetInLOS
			};

			aiState = aiState.HandleInput(newInput);
			aiState.StateUpdate(this, aiState);
		}
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
			// they know where you are.
			target = MissionManager.Instance.GetPlayerGO();
			actor.target = target.GetComponent<Actor>().GetShootAtMeTransform();
			//StartCoroutine(MoveRoutine());
			StartCoroutine(AttackRoutine());
			//StartCoroutine(LookForCoverRoutine());
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

	public GameObject GetTarget()
	{
		return target;
	}

	public AIControllersData GetAIData()
    {
		return aiData;
    }


	//private IEnumerator MoveRoutine()
	//{
	//	// I can see that there needs to be some kind of data structure applied here. I will probably implement state behavior. Could also use Behavior Trees asset.
	//	while (actor.IsAlive)
	//	{
	//		if (target != null)
	//		{
	//			// if reloading or got hit recently, stop
	//			if (isReloading || recoveringFromHit)
	//			{
	//				actor.StopMoving();
	//			}
	//			else
	//			{
	//				if ((targetInLOSIgnoreFloorCover && inHorizontalCover && targetInOptimalRange) || targetInLOS && targetInOptimalRange)
	//				{
	//					// stop moving if in optimal range to attack
	//					//if (targetInOptimalRange)
	//					//{
	//						if (lookingForCover && !poppingOutOfCover)
	//						{
	//							Collider closestCover = null;
	//							// cast overlap sphere with radius = range to see if target is possibly in range
	//							Collider[] hitColliders = Physics.OverlapSphere(transform.position, aiData.coverSearchRadius, LayerMask.GetMask("HouseAndFurniture"), QueryTriggerInteraction.Collide);
	//							foreach (Collider c in hitColliders)
	//							{
	//								if (c.CompareTag("Cover"))
	//								{
	//									if (closestCover == null)
	//									{
	//										closestCover = c;
	//									}
	//									else if ((transform.position - c.ClosestPoint(transform.position)).magnitude < (transform.position - closestCover.ClosestPoint(transform.position)).magnitude)
	//									{
	//										closestCover = c;
	//									}
	//								}
	//							}

	//							if (closestCover != null)
	//							{
	//								// if it's a "Floor" cover (not "Wall"), then it's horizontal cover.
	//								if (closestCover.GetComponent<Cover>().coverType == Cover.CoverType.Floor)
	//								{
	//									inHorizontalCover = true;
	//								}

	//								Vector3[] arr = new Vector3[4];
	//								arr[0] = closestCover.transform.position - Vector3.right * 50f;
	//								arr[1] = closestCover.transform.position + Vector3.right * 50f;
	//								arr[2] = closestCover.transform.position - Vector3.up * 50f;
	//								arr[3] = closestCover.transform.position + Vector3.up * 50f;

	//								Vector3 targetMovePosition = arr[0];
	//								for (int i = 1; i < arr.Length; i++)
	//								{
	//									if ((target.transform.position - arr[i]).magnitude > (target.transform.position - targetMovePosition).magnitude)
	//									{
	//										targetMovePosition = arr[i];
	//									}
	//								}

	//								actor.Move(closestCover.ClosestPoint(targetMovePosition));
	//								takingCover = true;
	//							}
	//							// can't find cover, just stand there and look pretty
	//							else
	//							{
	//								actor.StopMoving();
	//							}
	//						}
	//						else
	//						{
	//							if (poppingOutOfCover)
	//							{
	//								yield return new WaitForSeconds(.5f);
	//							}

	//							actor.StopMoving();

	//							if (poppingOutOfCover)
	//							{
	//								yield return new WaitForSeconds(5f);
	//								poppingOutOfCover = false;
	//							}
	//						}
	//					//}
	//					// otherwise move towards target
	//					//else
	//					//{
	//					//	if (data.canAim)
	//					//	{
	//					//		actor.EndAiming();
	//					//	}

	//					//	actor.Move(target.transform.position);
	//					//}
	//				}
	//				else
	//				{
	//					if (data.canAim)
	//					{
	//						actor.EndAiming();
	//					}

	//					// probabbly want to change this to (if not have lOS but in range & seekinCover) or something like that ... ?
	//					if (takingCover)
	//					{
	//						// wait for the actor to get fully behind cover
	//						yield return new WaitForSeconds(.5f);
	//						actor.StopMoving();

	//						if (inHorizontalCover)
	//						{
	//							actor.SetCrouch(true);
	//						}

	//						// hide for a period of time
	//						yield return new WaitForSeconds(Random.Range(aiData.minBehindCoverTime, aiData.maxBehindCoverTime));

	//						if (inHorizontalCover)
	//						{
	//							actor.SetCrouch(false);
	//						}

	//						takingCover = false;
	//						lookingForCover = false;
	//						poppingOutOfCover = true;
	//					}

	//					if (!targetInOptimalRange)
	//					{
	//						actor.Move(target.transform.position);
	//					}
	//				}
	//			}
	//		}

	//		yield return null;
	//	}
	//}

	private IEnumerator AttackRoutine()
    {
		while (actor.IsAlive)
		{
			// if don't have ammo, reload
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

			// if we're in optimal range (and have stopped), OR if we're dope enough to move and shoot, open fire (and not crouching!!!!!! This is bad! Should also check if in cover.)
			if (target != null && !isReloading && !recoveringFromHit && (targetInRange && (targetInOptimalRange || data.canMoveAndShoot)) && !actor.state[Actor.State.Crouching])
			{
				actor.UpdateActorRotation(target.transform.position);

				if (!pauseFurtherAttacks)
				{
					int numToFire = (int)Random.Range(data.minBurstFrames, data.maxBurstFrames);

					StartCoroutine(FireBurst(numToFire));
				}
			}

			yield return null;
		}
	}

	/// <summary>
    /// Every decision rate interval, decide if should search for cover.
    /// </summary>
    /// <returns></returns>
	private IEnumerator LookForCoverRoutine()
    {
		while (actor.IsAlive)
		{
			float lookForCoverChance = Random.Range(0f, 1f);
			lookingForCover = lookForCoverChance < data.useCoverChance || lookingForCover;
			yield return new WaitForSeconds(aiData.decisionInterval);
		}
	}

	protected override void HandleGetHit(Projectile p)
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
    /// <param name="ignoreFloorCover">Should we ignore floor cover?</param>
	//private (bool, bool) TargetInRangeAndLOS(bool ignoreFloorCover)
 //   {
	//	InventoryWeapon weapon = actor.GetEquippedWeapon();

	//	// cast overlap sphere with radius = range to see if target is possibly in range
	//	Collider[] hitColliders = Physics.OverlapSphere(transform.position, weapon.data.range, LayerMask.GetMask("Actors"), QueryTriggerInteraction.Collide);
	//	foreach (Collider c in hitColliders.Where(col => col.GetComponent<HitBox>() != null))
 //       {
	//		HitBox h = c.GetComponent<HitBox>();
	//		// if it's the target, then check if have LOS of target
	//		if (h.GetActor().gameObject == target)
 //           {
	//			Ray r = new Ray(transform.position, (target.transform.position - transform.position));
	//			RaycastHit[] hits = Physics.RaycastAll(r, weapon.data.range);

	//			RaycastHit[] targetHits = hits.Where(hit => hit.collider.GetComponent<HitBox>() != null && hit.collider.GetComponent<HitBox>().GetActor().gameObject == target).ToArray();
	//			RaycastHit[] blockHits = hits.Where(hit => hit.collider.gameObject.layer == (int) LayerNames.CollisionLayers.HouseAndFurniture).ToArray();

	//			// in Line of Sight
	//			int blockingHits = 0;
	//			// should only be one or zero targetHits. Check if any blocking hit is closer than the target, if so, can't shoot 
	//			foreach (RaycastHit targetHit in targetHits)
 //               {
	//				foreach (RaycastHit blockHit in blockHits)
	//				{
	//					Cover cover = blockHit.transform.GetComponent<Cover>();
	//					// if blocking thing is closer than target and not a "floor" cover.
	//					if (blockHit.distance < targetHit.distance)
	//					{
	//						if (cover != null && cover.coverType == Cover.CoverType.Floor)
	//						{
	//							blockingHits += ignoreFloorCover ? 0 : 1;
	//						}
	//						else
	//						{
	//							blockingHits += 1;
	//						}
	//					}
	//				}
	//			}

	//			// if we made it here and hit a target, then we do indeed have LOS and Range (otherwise would have returned or skipped this block)
	//			bool targetInRangeAndLOS = blockingHits == 0 && targetHits.Count() > 0;
	//			bool targetInOptimalRange = targetInRangeAndLOS ? ((target.transform.position - transform.position).magnitude <= weapon.data.optimalRange) : false;

	//			return (targetInRangeAndLOS, targetInOptimalRange);
 //           }
 //       }

	//	return (false, false);
	//}

	private bool IsTargetInRange(bool optimalRange)
    {
		InventoryWeapon weapon = actor.GetEquippedWeapon();
		float distFromTarget = (target.transform.position - transform.position).magnitude;
		return distFromTarget <= (optimalRange ? weapon.data.optimalRange : weapon.data.range);
    }

	private bool IsTargetInLOS(bool ignoreFloorCover)
    {
		InventoryWeapon weapon = actor.GetEquippedWeapon();

		Ray r = new Ray(transform.position, (target.transform.position - transform.position));
		RaycastHit[] hits = Physics.RaycastAll(r, weapon.data.range);

		RaycastHit[] targetHits = hits.Where(hit => hit.collider.GetComponent<HitBox>() != null && hit.collider.GetComponent<HitBox>().GetActor().gameObject == target).ToArray();
		RaycastHit[] blockHits = hits.Where(hit => hit.collider.gameObject.layer == (int)LayerNames.CollisionLayers.HouseAndFurniture).ToArray();

		// in Line of Sight
		int blockingHits = 0;
		// should only be one or zero targetHits. Check if any blocking hit is closer than the target, if so, can't shoot 
		foreach (RaycastHit targetHit in targetHits)
		{
			foreach (RaycastHit blockHit in blockHits)
			{
				Cover cover = blockHit.transform.GetComponent<Cover>();
				// if blocking thing is closer than target and not a "floor" cover.
				if (blockHit.distance < targetHit.distance)
				{
					if (cover != null && cover.coverType == Cover.CoverType.Floor)
					{
						blockingHits += ignoreFloorCover ? 0 : 1;
					}
					else
					{
						blockingHits += 1;
					}
				}
			}
		}

		// if we made it here and hit a target, then we do indeed have LOS and Range (otherwise would have returned or skipped this block)
		bool targetInLOSInput = blockingHits == 0 && targetHits.Count() > 0;
		
		return targetInLOSInput;
    }

	protected override void HandleDeath()
    {
		OnEnemyKilled.Invoke();
		base.HandleDeath();
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

		while (numToFire > 0 && currentWeaponAmmo > 0 && targetInRange)
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
