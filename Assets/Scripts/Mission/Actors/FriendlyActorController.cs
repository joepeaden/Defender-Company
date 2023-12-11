using UnityEngine;

public class FriendlyActorController : AIActorController, ISetActive
{
    [SerializeField]
    private GameObject movePositionSprite;

    [SerializeField]
    private GameObject selectionHighlight;

    protected new void Start()
    {
        base.Start();
		SetInitialState(new AIHoldingPositionCombatState());

        MissionManager.Instance.friendlyActors.Add(this);
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
