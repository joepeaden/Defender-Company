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

	//private bool pauseFurtherAttacks;
	private bool isReloading;
	private bool isDazed;
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

	protected bool nameIsSet;
	private static int nameIndex;

	private bool isPlayerControlled;

	protected void Start()
	{
		if (activateOnStart)
		{
			Activate();
		}

		if (!nameIsSet)
        {
			gameObject.name = $"Actor {nameIndex} ({actor.team})";
			nameIndex++;
        }
	}

	protected void Update()
	{
		if (actor.IsAlive && !isPlayerControlled && isActiveAndEnabled && aiState != null)
		{
			targetInRange = IsTargetInRange(false);
            targetInOptimalRange = IsTargetInRange(true);
            targetInLOS = IsTargetInLOS(true);

			aiState = aiState.StateUpdate(this, aiState);
		}
	}

	public virtual void SetActorControlled(bool isControlled)
	{
		isPlayerControlled = isControlled;
		actor.Pathfinder.enabled = actor.IsAlive && !isControlled;
		actor.MainCollider.isTrigger = false;
	}

	protected void SetInitialState(AIState initialState)
    {
		if (initialState as AIFollowTargetState != null)
		{
			FollowThisThing(FindTarget());
		}
		else if (initialState as AIDeliveringBombState != null)
        {
			bool foundWall = false;
            foreach (GameObject wallGameObject in GameObject.FindGameObjectsWithTag("WallBuilding"))
            {
                if (!wallGameObject.GetComponent<Building>().isTargeted)
                {
                    wallGameObject.GetComponent<Building>().isTargeted = true;

					Pathfinding.NNInfo info = AstarPath.active.GetNearest(wallGameObject.transform.position);
					Vector3 targetPos = info.position - ((info.position - transform.position).normalized * actor.GetColliderRadius()*3);

					GoToPosition(targetPos);
                    foundWall = true;
                    break;
                }
            }

			// if can't find suitible target, just attack
            if (!foundWall)
            {
				initialState = new AIFollowTargetState();
                FollowThisThing(FindTarget());
            }
        }

		aiState = initialState;
		aiState.EnterState(this, null);
	}

	// a temporary method just for finding some friendly AI to hunt
	private Transform FindTarget()
    {
		int result = Random.Range(0, MissionManager.Instance.friendlyActors.Count);
		return MissionManager.Instance.friendlyActors[result].transform;
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

	private IEnumerator AttackRoutine()
    {
		// need to also check if the actor is on the player's screen (unless they are a sniper?)
		
		while (actor.IsAlive)
		{
			if (isPlayerControlled)
			{
				yield return null;
				continue;
			}

			if (!attackTarget.GetComponent<Actor>().IsAlive)
            {
				ClearAttackTarget();
				break;
            }

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
			if (attackTarget != null && !isReloading && !isDazed && targetInLOS && (targetInOptimalRange || targetInRange && data.canMoveAndShoot))//&& pod.isAlerted && targetInLOS) //&& !actor.state[Actor.State.Crouching])
			{
				actor.UpdateActorRotation(attackTarget.transform.position);

				if (!pauseFurtherAttacks)
				{
					StartCoroutine(FireBurst(actor.GetEquippedWeapon().data.projPerBurst));
				}
			}

			// shootInterval should be a member var that is set by either data (if an enemy), or by the CompanySoldier info. Maybe tie it to Accuracy
			// Rating or something like that. "Shooting Skill". Idk. But yeah. Bro. Sometime. Maybe. Ya know?
			yield return new WaitForSeconds(data.shootInterval);

			yield return null;
		}
	}

	protected override void HandleGetHit()
	{
		isDazed = true;
	}

	public bool IsDazed()
    {
		return isDazed;
    }

	public void StopBeingDazed()
    {
		isDazed = false;
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

    private bool IsTargetInLOS(bool ignoreFloorCover)
    {
		if (attackTarget == null)
        {
			return false;
        }

		//Ray r = new Ray();
		RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, (attackTarget.transform.position - transform.position), 1000f);
		RaycastHit2D[] targetHits = hits.Where(hit => hit.collider.GetComponent<HitBox>() != null && hit.collider.GetComponent<HitBox>().GetActor().gameObject == attackTarget).ToArray();
		RaycastHit2D[] blockHits = hits.Where(hit => hit.collider.gameObject.layer == LayerMask.NameToLayer("Building")).ToArray();

		// in Line of Sight
		int blockingHits = 0;
		// should only be one or zero targetHits. Check if any blocking hit is closer than the target, if so, can't shoot 
		foreach (RaycastHit2D targetHit in targetHits)
		{
			foreach (RaycastHit2D blockHit in blockHits)
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

	protected override void HandleDeath(bool fromExplosive)
    {
		OnActorKilled.Invoke(actor.team);
		base.HandleDeath(fromExplosive);
    }
}
