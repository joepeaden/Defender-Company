using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SoldierBackgroundData", menuName = "MyScriptables/SoldierBackgroundData")]
public class SoldierBackgroundData : ScriptableObject
{
    public string backgroundName;

    public int minHP;
    public int maxHP;
    public int minSpeed;
    public int maxSpeed;
    public int minAcc;
    public int maxAcc;

    public string backgroundDescription;
}
