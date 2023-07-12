using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Equipment : InventoryItem
{
    public GearData data;
    public abstract bool Use();
}
