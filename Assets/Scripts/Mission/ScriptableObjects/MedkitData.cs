using UnityEngine;

[CreateAssetMenu(fileName = "MedkitData", menuName = "MyScriptables/MedkitData")]
public class MedkitData : GearData
{
    public int amountHealed;
    public int totalAmount;
    public AudioClip soundEffect;
}
