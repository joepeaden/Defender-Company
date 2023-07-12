using System;
using UnityEngine;

public class GearData : ScriptableObject
{
    public enum GearType
    {
        Medkit,
        PlasticExplosive,
        AssaultRifle,
        SubMachineGun,
        SemiAutoRifle,
        Shotgun,
        Pistol,
        Sabre
    } 

    public GearType gearType;
	public string displayName;
	public string rewardKey;
	public int cost;
}
