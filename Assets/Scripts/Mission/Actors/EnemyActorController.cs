﻿using UnityEngine.Events;
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

		actor.SetAgentSpeed(data.navAgentSpeed);

		actor.SetWeaponFromData(data.startWeapon);
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

    //protected override void HandleDeath()
    ////{
    //    base.HandleDeath();
    //}

    // public void OnCollisionEnter(Collider col)
    // {
    //     if (col)
    //     {
    //bumpedIntoWall = true;
    //     }
    // }
}
