using UnityEngine;
using System.Collections.Generic;

public class FriendlyActorController : AIActorController, ISetActive
{
    private const int EMERGENCY_RANGE = 12;

    [SerializeField]
    private GameObject movePositionSprite;

    [SerializeField]
    private GameObject selectionHighlight;

    public CompanySoldier TheCompanySoldier => companySoldier;
    private CompanySoldier companySoldier;

    // the index of teh character in the list of player characters, for following etc.
    public Vector3 followOffset;

    protected new void Start()
    {
        base.Start();
        actor.OnGotKill.AddListener(HandleGotKill);
        MissionManager.Instance.HandleFriendlySpawned();

        //FollowPlayer();
    }

    void OnEnable()
    {
        GetFollowPosition();
    }

    void GetFollowPosition()
    {
        bool foundFollowOffset = false;
        foreach (KeyValuePair<Vector3, bool> kvp in MissionManager.Instance.followOffsets)
        {
            bool isOccupied = kvp.Value;
            Vector3 position = kvp.Key;

            if (isOccupied)
            {
                continue;
            }

            followOffset = position;
            foundFollowOffset = true;

            break;
        }

        if (foundFollowOffset)
        {
            MissionManager.Instance.followOffsets[followOffset] = true;
        }
    }

    //void FollowPlayer()
    //{
        //SetInitialState(new AIFollowTargetState());
        //FollowThisThing(MissionManager.Instance.Player.ControlledActor.transform);
    //}


    new void Update()
    {
        base.Update();

        if (!actor.IsPlayer)
        {
            Vector3 newPos = MissionManager.Instance.Player.ControlledActor.transform.position + followOffset;
            
            actor.Move(newPos);
        }
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

        //MissionUI.Instance.AddEntityMarker(this, companySoldier.Name);
    }

    public override void SetActorControlled(bool isControlled)
    {
        movePositionSprite.SetActive(!isControlled);

        base.SetActorControlled(isControlled);
    }

    public void CheckIfInEmergencyRange(EnemyActorController enemy)
    {
        if ((transform.position - enemy.transform.position).magnitude <= EMERGENCY_RANGE)
        {
            AddAttackTarget(enemy.GetActor(), true);
        }
        else
        {
            RemoveAttackTarget(enemy.GetActor());
        }
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
