using UnityEngine;

[CreateAssetMenu(fileName = "ExplosiveData", menuName = "MyScriptables/ExplosiveData")]
public class ExplosiveData : ShopRewardData
{
    public int damage;
    public float explosionRadius;
    public float explosionPower;
    public float upwardsForce;
    public GameObject explosionPrefab;
    public GameObject instancePrefab;
    /// <summary>
    /// Total charges of use.
    /// </summary>
    public int totalAmount;
}
