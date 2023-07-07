using UnityEngine;

public class InventoryItem
{
    public Actor owningActor;
    public int amount;
    // temporary, eventually need a better thing like an enum or something.
    public string rewardKey;

    /// <summary>
    /// Add some uses of this inventory item. Could refer to ammo or charges of equipmpent, etc.
    /// </summary>
    /// <param name="amount"></param>
    public void AddAmount(int amount)
    {
        this.amount += amount;
    }
}