using UnityEngine.Events;
using UnityEngine;

/// <summary>
/// Base class for enemy actors.
/// </summary>
public class EnemyActorController : AIActorController, ISetActive
{
	public static UnityEvent OnEnemySpawned = new UnityEvent();

    protected new void Start()
	{
		base.Start();

		OnEnemySpawned.Invoke();

		if (isActiveAndEnabled)
		{
			SetBehaviour();
		}

		actor.SetStats(data.hitPoints, data.moveSpeed, data.accuracyRating, data.startWeapon);

		//MissionUI.Instance.AddEntityMarker(this, "Enemy");
	}

    private void OnEnable()
    {
		PickTarget();
	}

	private void PickTarget()
	{
		//float smallestDist = float.MaxValue;
		for (int i = 0; i < MissionManager.Instance.friendlyActors.Count; i++)
		{
			//if (!MissionManager.Instance.friendlyActors[i].GetActor().IsAlive)
			//{
			//	continue;
			//}

			//float dist = (transform.position - MissionManager.Instance.friendlyActors[i].transform.position).magnitude;
			//if (dist < smallestDist)
			//{
			//	smallestDist = dist;
				AddAttackTarget(MissionManager.Instance.friendlyActors[i].GetActor(), true);
			//}
		}
	}

	private void SetBehaviour()
    {
		switch (data.behaviourType)
		{
			case ControllerData.AIBehaviourType.Sapper:
				SetInitialState(new AIDeliveringBombState());
				break;
			case ControllerData.AIBehaviourType.Attacker:
				SetInitialState(new AIFollowTargetState());
				break;
			default:
				aiState = new AIHoldingPositionCombatState();
				break;
		}
	}

	new void Update()
    {
		base.Update();

		// just doing it this way because there is already a collection of player characters and there isn't one for enemy characters.
		// it really doesn't matter.
		foreach (FriendlyActorController playerFriendlySoldier in MissionManager.Instance.friendlyActors)
        {
			playerFriendlySoldier.CheckIfInEmergencyRange(this);
		}
    }
}
