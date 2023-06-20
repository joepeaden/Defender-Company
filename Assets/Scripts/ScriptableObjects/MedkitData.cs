using UnityEngine;

[CreateAssetMenu(fileName = "MedkitData", menuName = "MyScriptables/MedkitData")]
public class MedkitData : ShopRewardData
{
    public int amountHealed;
    public int totalAmount;
    public AudioClip soundEffect;
}
