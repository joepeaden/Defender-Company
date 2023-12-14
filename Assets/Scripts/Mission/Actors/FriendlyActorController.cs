using UnityEngine;
using System.Collections.Generic;

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

        string[] nameOptions =
        {
            "Rourke",
            "Niels",
            "Smith",
            "Danson",
            "Peters",
            "Wang",
            "O'Malley",
            "Bauer",
            "Rochefort",
            "Dumas",
            "Garcia",
            "Vargas",
            "Anderson",
            "Thomas",
            "Brown",
            "Grey",
            "Benson",
            "Al-Hilli",
            "Cohen",
            "Rosenberg",
            "Goldstein"
        };
        int randomIndex = Random.Range(0, nameOptions.Length - 1);
        MissionUI.Instance.AddEntityMarker(this, nameOptions[randomIndex]);
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
