using UnityEngine;
using System.Collections.Generic;

public class FriendlyActorController : AIActorController, ISetActive
{
    [SerializeField]
    private GameObject movePositionSprite;

    [SerializeField]
    private GameObject selectionHighlight;

    public CompanySoldier TheCompanySoldier => companySoldier;
    private CompanySoldier companySoldier;

    protected new void Start()
    {
        base.Start();
		SetInitialState(new AIHoldingPositionCombatState());
        actor.OnGotKill.AddListener(HandleGotKill);
        MissionManager.Instance.HandleFriendlySpawned();
    }

    /// <summary>
    /// Set the soldier for this friendly actor to be associated with
    /// </summary>
    /// <param name="soldier"></param>
    public void SetSoldier(CompanySoldier soldier)
    {
        companySoldier = soldier;
        transform.parent.gameObject.SetActive(true);
        transform.parent.gameObject.name = companySoldier.Name;
        actor.SetStats(companySoldier);
        SetControllerData(data);

        companySoldier.ResetMissionVariables();

        gameObject.name = soldier.Name;
        nameIsSet = true;

        MissionUI.Instance.AddEntityMarker(this, companySoldier.Name);
    }

    public override void SetActorControlled(bool isControlled)
    {
        movePositionSprite.SetActive(!isControlled);

        base.SetActorControlled(isControlled);
    }

    private void HandleGotKill()
    {
        companySoldier.MissionKills += 1;
    }

    protected override void HandleGetHit()
    {
        base.HandleGetHit();
        companySoldier.MissionHP = actor.HitPoints;
    }

    public override void GoToPosition(Vector3 position)
    {
        if (position == Vector3.zero)
        {
            movePositionSprite.SetActive(false);
        }
        else
        {
            movePositionSprite.SetActive(true);
        }

        base.GoToPosition(position);
        movePositionSprite.transform.position = position;
    }

    public void UpdateSelection(bool isSelected)
    {
        selectionHighlight.SetActive(isSelected);
    }

    protected override void HandleDeath(bool fromExplosive)
    {
        base.HandleDeath(fromExplosive);
        MissionManager.Instance.friendlyActors.Remove(this);
    }
}
