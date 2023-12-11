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
public abstract class AIActorController : ActorController, ISetActive
{
	public static UnityEvent<Actor.ActorTeam> OnActorKilled = new UnityEvent<Actor.ActorTeam>();
	
	public bool activateOnStart;
	[SerializeField] private AIControllersData aiData;

	private bool pauseFurtherAttacks;
	private bool isReloading;
	private bool recoveringFromHit;
	public GameObject AttackTarget => attackTarget;
	protected GameObject attackTarget;
	public Transform FollowTarget => followTarget;
	protected Transform followTarget;
	public Vector3 MovePosition => movePosition;
	public Vector3 movePosition;
	private bool targetInRange;
	private bool targetInOptimalRange;
	public bool targetInLOS;

	public bool fullyInCover;
	public bool fullyOutOfCover;

	protected AIState aiState;
	private float timeSinceLastDecision;

	protected new void Start()
	{
		base.Start();

		//actor.AddCoverListener(ActorHasPotentialCover);
		actor.SetWeaponFromData(data.startWeapon);

		if (activateOnStart)
		{
			Activate();
		}
	}

	protected void Update()
	{
		if (actor.IsAlive && isActiveAndEnabled && aiState != null)
		{
			//timeSinceLastDecision += Time.deltaTime;
			//if (timeSinceLastDecision > aiData.decisionInterval)
			//{
			//	timeSinceLastDecision = 0f;
			//}

			//targetInRange = IsTargetInRange(false);
			//targetInOptimalRange = IsTargetInRange(true);
			//targetInLOS = IsTargetInLOS(true);

			////bool targetInDetectionRadius = IsTargetInDetectionRadius();

			////if (IsTargetInDetectionRadius() && targetInLOS)
			////         {
			////	pod.isAlerted = true;
			////         }

			//fullyInCover = AmIFullyInOrOutOfCover(true);
			//fullyOutOfCover = AmIFullyInOrOutOfCover(false);

			//// ya know. Maybe a smarter way to do this would be have the state call whatever methods to get whatever information it needs. Not all this info is relevant to every state.
			//AIInput newInput = new AIInput
			//{
			//	timeForDecision = timeSinceLastDecision == 0f,
			//	targetInRange = targetInRange,
			//	targetInOptimalRange = targetInOptimalRange,
			//	targetInLOS = targetInLOS,
			//	//targetInDetectionRadius = targetInDetectionRadius,
			//	//distFromPodLeader = 0f,//(transform.position - pod.leader.transform.position).magnitude,
			//	//podAlerted = true,//pod.isAlerted,
			//	fullyInCover = fullyInCover,
			//	fullyOutOfCover = fullyOutOfCover
			//};

			//aiState = aiState.HandleInput(newInput);
			AIState oldAiState = aiState;
			aiState = aiState.StateUpdate(this, aiState);
			if (oldAiState != aiState)
			{
				aiState.EnterState(this, oldAiState);
			}
		}
	}

  //  public void Move(Vector3 movePosition)
  //  {
		//if (movePosition)
		//GetActor().Move(movePosition);
  //  }

	protected void SetInitialState(AIState initialState)
    {
		if (initialState as AIFollowTargetState != null)
		{
			FollowThisThing(MissionManager.Instance.GetPlayerGO().transform);
		}
		else if (initialState as AIDeliveringBombState != null)
        {
			bool foundWall = false;
            foreach (GameObject wallGameObject in GameObject.FindGameObjectsWithTag("WallBuilding"))
            {
                if (!wallGameObject.GetComponent<Building>().isTargeted)
                {
                    wallGameObject.GetComponent<Building>().isTargeted = true;

					GoToPosition(wallGameObject.transform.position - (transform.forward * 3f));
                    foundWall = true;
                    break;
                }
            }

			// if can't find suitible target, just attack
            if (!foundWall)
            {
				initialState = new AIFollowTargetState();
                FollowThisThing(MissionManager.Instance.GetPlayerGO().transform);
            }
        }

		aiState = initialState;
		aiState.EnterState(this, null);
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
			//target = MissionManager.Instance.GetPlayerGO();
			//actor.target = target.GetComponent<Actor>().GetShootAtMeTransform();
			//StartCoroutine(AttackRoutine());
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
			attackTarget = null;
			StopAllCoroutines();

			// if remove this move order, the actor goes to last player position. Might want it to be like that down the line. Just something to consider.
			//actor.Move(transform.position);
		}
	}

	public GameObject bombPrefab;
	public void PlaceBomb()
    {
		Instantiate(bombPrefab, transform.position, transform.rotation);
    }

	public bool ShouldGoToPosition()
    {
		return movePosition != Vector3.zero;
    }

	public void StopGoingToPosition()
    {
		movePosition = Vector3.zero;
    }

	public virtual void GoToPosition(Vector3 position)
    {
		// don't tell the navagent to go to the same place twice
		if (position == movePosition)
        {
			return;
        }

		movePosition = position;
	}

	public bool ShouldFollowSomething()
    {
		return followTarget != null;
    }

	public void StopFollowingSomething()
    {
		followTarget = null;
    }

	public void FollowThisThing(Transform t)
    {
		followTarget = t;
	}

	public void SetAttackTarget(GameObject g)
	{
		if (isActiveAndEnabled)
		{
			attackTarget = g;
			//if (target.GetComponent<Actor>() != null)
			//      {
			//	actor.target = target.GetComponent<Actor>().GetShootAtMeTransform();
			//}
			//else
			//      {
			//	actor.target = target.transform;
			//      }

			StartCoroutine(AttackRoutine());
		}
	}

	public void ClearAttackTarget()
    {
		StopCoroutine(AttackRoutine());
		attackTarget = null;
		//actor.target = null;
    }

	public AIControllersData GetAIData()
    {
		return aiData;
    }

	private IEnumerator AttackRoutine()
    {
		// need to also check if the actor is on the player's screen (unless they are a sniper?)
		
		while (actor.IsAlive)
		{
			if (!attackTarget.GetComponent<Actor>().IsAlive)
            {
				ClearAttackTarget();
				break;
            }
			//if (target == null)
   //         {
			//	yield return null;
			//	continue;
   //         }

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
			if (attackTarget != null && !isReloading && !recoveringFromHit && targetInLOS && (targetInOptimalRange || targetInRange && data.canMoveAndShoot))//&& pod.isAlerted && targetInLOS) //&& !actor.state[Actor.State.Crouching])
			{
				actor.UpdateActorRotation(attackTarget.transform.position);

				if (!pauseFurtherAttacks)
				{
					int numToFire = (int)Random.Range(data.minBurstFrames, data.maxBurstFrames);

					StartCoroutine(FireBurst(numToFire));
				}
			}

			yield return null;
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

	private bool IsTargetInRange(bool optimalRange)
    {
		if (attackTarget == null)
        {
			return false;
        }

		InventoryWeapon weapon = actor.GetEquippedWeapon();
		float distFromTarget = (attackTarget.transform.position - transform.position).magnitude;
		return distFromTarget <= (optimalRange ? weapon.data.optimalRange : weapon.data.range);
    }

	private bool IsTargetInDetectionRadius()
	{
		// if in detection radius return true
		if (attackTarget != null && (transform.position - attackTarget.transform.position).magnitude <= aiData.detectionRadius)
        {
			return true;
        }

		return false;
    }

	/// <summary>
	/// Is the actor fully in cover (from one side of body to the other?) based on actor's transform width
	/// </summary>
	/// <returns></returns>
	private bool AmIFullyInOrOutOfCover(bool intoCover)
	{
		if (attackTarget == null)
        {
			return false;
        }

		float actorWidth = actor.GetWidth();

		Quaternion q = Quaternion.LookRotation(attackTarget.transform.position - transform.position);
		Vector3 newRot = q * Vector3.right;


		//Debug.DrawRay(transform.position + (transform.right * actorWidth / 2), ((transform.position + (transform.right * actorWidth / 2))) * 10f, Color.red);

		Vector3 rayStartPos = transform.position + (newRot * (actorWidth / 2));
		Vector3 rayDir = attackTarget.transform.position - rayStartPos;//((transform.position + (transform.right * actorWidth / 2)));

		Ray r = new Ray(rayStartPos, rayDir);

		//Debug.DrawRay(rayStartPos, rayDir * 100f, Color.red);

		RaycastHit[] hits = Physics.RaycastAll(r, 1000f);
		// 1000f is a arbitrary number but maybe don't limit the LOS//aiData.detectionRadius);

		RaycastHit[] targetHits = hits.Where(hit => hit.collider.GetComponent<HitBox>() != null && hit.collider.GetComponent<HitBox>().GetActor().gameObject == attackTarget).ToArray();
		RaycastHit[] blockHits = hits.Where(hit => hit.collider.gameObject.layer == (int)LayerNames.CollisionLayers.HouseAndFurniture).ToArray();

		// in Line of Sight
		int blockingHits1 = 0;
		// should only be one or zero targetHits. Check if any blocking hit is closer than the target, if so, can't shoot 
		foreach (RaycastHit targetHit in targetHits)
		{
			foreach (RaycastHit blockHit in blockHits)
			{
				Cover cover = blockHit.transform.GetComponent<Cover>();
				// if blocking thing is closer than target and not a "floor" cover.
				if (blockHit.distance < targetHit.distance)
				{
					//if (cover != null && cover.coverType == Cover.CoverType.Floor)
					//{
					//	blockingHits += ignoreFloorCover ? 0 : 1;
					//}
					//else
					//{
					blockingHits1 += 1;
					//}
				}
			}
		}

		rayStartPos = transform.position + (-newRot * (actorWidth / 2));
		rayDir = attackTarget.transform.position - rayStartPos;//((transform.position - (transform.right * actorWidth / 2)));
		r = new Ray(rayStartPos, rayDir);

		//Debug.DrawRay(rayStartPos, rayDir * 100f, Color.red);

		hits = Physics.RaycastAll(r, 1000f);
		// 1000f is a arbitrary number but maybe don't limit the LOS//aiData.detectionRadius);

		targetHits = hits.Where(hit => hit.collider.GetComponent<HitBox>() != null && hit.collider.GetComponent<HitBox>().GetActor().gameObject == attackTarget).ToArray();
		blockHits = hits.Where(hit => hit.collider.gameObject.layer == (int)LayerNames.CollisionLayers.HouseAndFurniture).ToArray();

		int blockingHits2 = 0;
		// should only be one or zero targetHits. Check if any blocking hit is closer than the target, if so, can't shoot 
		foreach (RaycastHit targetHit in targetHits)
		{
			foreach (RaycastHit blockHit in blockHits)
			{
				Cover cover = blockHit.transform.GetComponent<Cover>();
				// if blocking thing is closer than target and not a "floor" cover.
				if (blockHit.distance < targetHit.distance)
				{
					//if (cover != null && cover.coverType == Cover.CoverType.Floor)
					//{
					//	blockingHits += ignoreFloorCover ? 0 : 1;
					//}
					//else
					//{
					blockingHits2 += 1;
					//}
				}
			}
		}

		// if we made it here and hit a target, then we do indeed have LOS and Range (otherwise would have returned or skipped this block)
		//;

		if (intoCover)
		{
			return blockingHits1 > 0 && blockingHits2 > 0 && targetHits.Count() > 0;
		}
		else
        {
			return blockingHits1 == 0 && blockingHits2 == 0 && targetHits.Count() > 0;
        }
	}

    private bool IsTargetInLOS(bool ignoreFloorCover)
    {
		if (attackTarget == null)
        {
			return false;
        }

		Ray r = new Ray(transform.position, (attackTarget.transform.position - transform.position));
		RaycastHit[] hits = Physics.RaycastAll(r, 1000f);
		// 1000f is a arbitrary number but maybe don't limit the LOS//aiData.detectionRadius);

		RaycastHit[] targetHits = hits.Where(hit => hit.collider.GetComponent<HitBox>() != null && hit.collider.GetComponent<HitBox>().GetActor().gameObject == attackTarget).ToArray();
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
		OnActorKilled.Invoke(actor.team);
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

		while (numToFire > 0 && currentWeaponAmmo > 0 && targetInLOS)
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
