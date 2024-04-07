using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerParty : MonoBehaviour
{
    public static PlayerParty Instance => _instance;
    private static PlayerParty _instance;

    //[SerializeField] MapNode startNode;
    public MapNode CurrentNode;

    public float moveSpeed;
    public float stopDist;

    public bool hasTriggeredArrival;

    private void Awake()
    {
        _instance = this;
        //CurrentNode = startNode;
    }

    private void Start()
    {
        CurrentNode.SetConnectedNodeColors(true);
    }

    private void Update()
    {
        Vector3 moveVector = CurrentNode.transform.position - transform.position;
        if (moveVector.magnitude > stopDist)
        {
            transform.Translate(moveVector.normalized * moveSpeed * Time.deltaTime);
        }
        else if (!hasTriggeredArrival)
        {
            hasTriggeredArrival = true;
            CurrentNode.TriggerArrivalEvent();
        }
    }

    public void MoveToNode(MapNode newNode)
    {
        hasTriggeredArrival = false;
        CurrentNode.SetConnectedNodeColors(false);
        CurrentNode = newNode;
        CurrentNode.SetConnectedNodeColors(true);

        GameManager.Instance.playerMapNodeIndex = CurrentNode.index;
    }

    public void TeleportToCurrentNode()
    {
        transform.position = new Vector2(CurrentNode.transform.position.x - 10f, CurrentNode.transform.position.y);
    }
}
