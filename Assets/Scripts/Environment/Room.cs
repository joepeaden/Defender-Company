using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles toggling cut away walls when player enters or exits a room.
/// </summary>
public class Room : MonoBehaviour
{
    /// <summary>
    /// Unity doesn't show Interfaces in inspector I guess. So this is just for assignment to setActives list.
    /// </summary>
    [SerializeField] private List<GameObject> setActiveGameObjects = new List<GameObject>();
    [SerializeField] private List<Door> doors = new List<Door>();
    [SerializeField] private List<Shatter> breakableWalls = new List<Shatter>();

    [SerializeField] private GameObject roof;

    /// <summary>
    /// Collection of stuff inside the room that needs to be activated or deactivated based on if the player is present. Things like Actors, traps, etc.
    /// </summary>
    private List<ISetActive> setActives = new List<ISetActive>();

    private void Start()
    {
        // fill list of stuff in the room, set them all to inactive initially.
        foreach (GameObject go in setActiveGameObjects)
        {
            if (go.activeInHierarchy)
            {
                ISetActive setActive = go.GetComponent<ISetActive>();
                setActives.Add(setActive);
                setActive.DeActivate();
            }
        }

        foreach (Door door in doors) 
        {
            door.OnDoorOpen.AddListener(delegate { UpdateRoomState(true); } );
        }
        
        foreach (Shatter shatter in breakableWalls) 
        {
            shatter.OnShatter.AddListener(delegate { UpdateRoomState(true); } );
        }

    }

    /// <summary>
    /// Set cut away walls active and hide furniture, activate actors.
    /// </summary>
    private void UpdateRoomState(bool playerIsEntering)
    {
        if (playerIsEntering)
        {
            foreach (ISetActive a in setActives)
            {
                a.Activate();
            }

            //if (roof) roof.SetActive(false);
        }
    }
}
