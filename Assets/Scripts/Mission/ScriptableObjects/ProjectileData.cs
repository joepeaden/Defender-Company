using UnityEngine;

[CreateAssetMenu(fileName = "ProjectileData", menuName = "MyScriptables/ProjectileData")]
public class ProjectileData : ScriptableObject
{
    public Sprite sprite;
    public float colliderLength;
    public float colliderWidth;

    public bool causesHeadShots;
    public float velocity;
    //public int force;
    public int damage;

    //public bool isExplosive;
}
