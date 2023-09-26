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
		aiState = new AIHoldingPositionCombatState();

        MissionManager.Instance.friendlyActors.Add(this);
    }

    public override void MoveToPosition(Vector3 position)
    {
        // add a little variance so units aren't on top of eachother
        position.x += Random.Range(-3f, 3f);
        position.z += Random.Range(-3f, 3f);

        base.MoveToPosition(position);
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
