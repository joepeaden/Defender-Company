using UnityEngine;
using System.Collections.Generic;

public class FriendlyActorController : AIActorController, ISetActive
{
    [SerializeField]
    private GameObject movePositionSprite;

    [SerializeField]
    private GameObject selectionHighlight;

    CompanySoldier companySoldier;

    protected new void Start()
    {
        base.Start();
		SetInitialState(new AIHoldingPositionCombatState());
        MissionUI.Instance.AddEntityMarker(this, companySoldier.Name);

        actor.OnGotKill.AddListener(HandleGotKill);
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
        actor.SetSoldier(companySoldier);
        actor.SetWeaponFromData(companySoldier.CurrentWeapon);

        companySoldier.ResetMissionVariables();
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
