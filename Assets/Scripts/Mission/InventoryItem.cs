using UnityEngine;

public class InventoryItem
{
    public Actor owningActor;
    public int amount;
    // temporary, eventually need a better thing like an enum or something.
    public GearData.GearType gearType;

    public InventoryItem() { }

    // these two constructors can probably be merged if the InventoryItem / WeaponData / GearData / EquipmentData stuff is all ironed out
    public InventoryItem(WeaponData gear)
    {
        gearType = gear.gearType;
        amount = gear.totalAmount;
    }

    public InventoryItem(GearData gear)
    {
        gearType = gear.gearType;
        amount = 2;
    }

    /// <summary>
    /// Add some uses of this inventory item. Could refer to ammo or charges of equipmpent, etc.
    /// </summary>
    /// <param name="amount"></param>
    public void AddAmount(int amount)
    {
        this.amount += amount;
    }
}