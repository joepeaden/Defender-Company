using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public static MapManager Instance;

    public List<MapNode> mapNodes = new List<MapNode>();

    public GameObject mapGO;

    private void Awake()
    {
        Instance = this;

        for (int i = 0; i < mapNodes.Count; i++)
        {
            MapNode node = mapNodes[i];
            node.index = i;
        }
    }

    private void Start()
    {
        PlayerParty.Instance.CurrentNode = mapNodes[GameManager.Instance.playerMapNodeIndex];
        PlayerParty.Instance.TeleportToCurrentNode();
    }

    public void StartAmbush()
    {
        SceneLoader.Instance.ChangeScene(SceneLoader.SceneName.Mission, null, true);
        mapGO.SetActive(false);
    }

    public void RenableMap()
    {
        mapGO.SetActive(true);
    }
}
