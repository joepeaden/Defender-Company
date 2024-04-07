using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapNode : MonoBehaviour
{
    public Button button;
    private Image image;

    public bool hasAmbush;
    public bool isEndTarget;

    [SerializeField]
    private List<MapNode> connectedNodes = new List<MapNode>();

    /// <summary>
    /// The index number in the map nodes in the map manager
    /// </summary>
    public int index;

    private void Awake()
    {
        image = GetComponent<Image>();
        button.onClick.AddListener(MoveToThisNode);

        UpdateColor(false);
    }

    private void MoveToThisNode()
    {
        if (PlayerParty.Instance.CurrentNode.connectedNodes.Contains(this))
        {
            PlayerParty.Instance.MoveToNode(this);
        }
    }

    public void TriggerArrivalEvent()
    {
        if (hasAmbush)
        {
            MapManager.Instance.StartAmbush();
            hasAmbush = false;
        }
    }

    public void SetConnectedNodeColors(bool isActive)
    {
        UpdateColor(isActive);
        foreach (MapNode node in connectedNodes)
        {
            node.UpdateColor(isActive);
        }
    }

    public void UpdateColor(bool isConnected)
    {
        image.color = isConnected ? Color.yellow : Color.grey;
    }
}
