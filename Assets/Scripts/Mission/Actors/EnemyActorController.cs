using UnityEngine.Events;
using UnityEngine;

/// <summary>
/// Base class for enemy actors.
/// </summary>
public class EnemyActorController : AIActorController, ISetActive
{
	public static UnityEvent OnEnemySpawned = new UnityEvent();

	//protected new void Awake()
	//{
	//	base.Awake();

		
	//}

    protected new void Start()
	{
		base.Start();

		OnEnemySpawned.Invoke();

		if (isActiveAndEnabled)
		{
			SetBehaviour();
		}
	}

	private void SetBehaviour()
    {
		switch (data.behaviourType)
		{
			case ControllerData.AIBehaviourType.Sapper:
				
				aiState = new AIMovingToPositionStateSapper();
				aiState.EnterState(this, null);
				//foreach (GameObject wallGameObject in GameObject.FindGameObjectsWithTag("WallBuilding"))
				//            {
				//                if (!wallGameObject.GetComponent<Building>().isTargeted)
				//                {
				//                    wallGameObject.GetComponent<Building>().isTargeted = true;
				//                    MoveToPosition(wallGameObject.transform.position);
				//                    return;
				//                }
				//            }
				break;

                // if we made it here then there's no available targets, just attack instead of bombing.
                // THAT'S RIGHT, IT'S A GOTO! EVERYBODY FREAK OUT!
                //goto case ControllerData.AIBehaviourType.Attacker;

			case ControllerData.AIBehaviourType.Attacker:
				//aiState = new AIFollowTargetState();
				SetFollowTarget(MissionManager.Instance.GetPlayerGO().transform);
				return;
			default:
				aiState = new AIHoldingPositionCombatState();
				return;
		}
	}
}
