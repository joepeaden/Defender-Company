using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The logical representation of an medkit for inventory.
/// Can 
/// </summary>
public class MedkitEquipment : Equipment
{
    public MedkitData medData;

    public MedkitEquipment(MedkitData data)
    {
        this.data = data;
        medData = (MedkitData)data;
        amount = data.totalAmount;
    }

    public override bool Use()
    {
        if (amount > 0)
        {
            try
            {
                owningActor.AddHitPoints(medData.amountHealed);
                amount--;

                // hmm. the thing is, this class is not a monobehavior. So it needs access to a gameobject to play audio. So for now,
                // just have the manager make one and play the sound.
                GameManager.Instance.PlaySound(medData.soundEffect);

                return true;
            }
            catch
            {
                return false;
            }
        }

        return false;
    }

}
