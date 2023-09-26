using UnityEngine.Events;

/// <summary>
/// Base class for enemy actors.
/// </summary>
public class EnemyActorController : AIActorController, ISetActive
{
	public static UnityEvent OnEnemySpawned = new UnityEvent();

	protected new void Awake()
	{
		base.Awake();

		if (isActiveAndEnabled)
		{
			aiState = new AIMovingToTargetState();//AIHoldingPositionState();
		}
	}

    protected new void Start()
	{
		base.Start();

		OnEnemySpawned.Invoke();

		if (isActiveAndEnabled)
		{
			SetMoveTarget(MissionManager.Instance.GetPlayerGO().transform);
		}
		//SetTarget(MissionManager.Instance.GetGateGO());
		//SetMoveTarget(MissionManager.GetGate());
	}
}
