using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The logical representation of an explosive for inventory.
/// </summary>
public class ExplosiveEquipment : Equipment
{
    public ExplosiveData expData;

    public ExplosiveEquipment(ExplosiveData data)
    {
        this.data = data;
        amount = data.totalAmount;
        expData = (ExplosiveData)data;
    }

    public override bool Use()
    {
        
        if (amount > 0)
        {
            try
            {
                Vector3 actorPos = owningActor.transform.position;
                ExplosiveInstance inst = GameObject.Instantiate(expData.instancePrefab, actorPos, Quaternion.identity).GetComponent<ExplosiveInstance>();
                inst.data = expData;
                amount--;

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
