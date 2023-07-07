using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Author: Joseph Peaden

/// <summary>
/// Just holds enum that clarifies what layers are what
/// </summary>
public class LayerNames : MonoBehaviour
{
    public enum CollisionLayers
    {
        Default = 0,
        Actors = 3,
        MouseOnly = 6,
        InCover = 7,
        Projectiles = 8,
        HouseAndFurniture = 9,
        IgnoreActorsAndFurniture = 10,
        IgnoreFurniture = 11,
        IgnoreActors = 13,
        PlayerOutline = 15,
        EnemyOutline = 16,
        EnemyFill = 17
    }
}
