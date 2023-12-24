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
        actor.SetSoldier(soldier);
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

    protected override void HandleDeath()
    {
        base.HandleDeath();
        MissionManager.Instance.friendlyActors.Remove(this);
    }
}
