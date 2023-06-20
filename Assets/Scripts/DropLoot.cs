using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Actor))]
public class DropLoot : MonoBehaviour
{
    [SerializeField] private GameObject loot;
    private Actor actor;

    // temporary, eventually will do a dropdown to select the loot type.
    public string lootType;

    private void Start()
    {
        actor = GetComponent<Actor>();
        actor.OnDeath.AddListener(DropTheLoot);        
    }

    private void DropTheLoot()
    {
        Loot l = Instantiate(loot, transform.position, Quaternion.identity).GetComponent<Loot>();
        l.rewardKey = lootType;
    }

    private void OnDestroy()
    {
        actor.OnDeath.RemoveListener(DropTheLoot);   
    }
}
