﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    // move this stuff into a Scriptable.

    //   [SerializeField]
    //protected float velocity; //50
    //[SerializeField]
    //protected bool isExplosive;

    public Vector3 movementDirection { private set; get; }

    [SerializeField]
    protected ProjectileData data;

    public Actor OwningActor => owningActor;
    protected Actor owningActor;
    //protected int damage;

    private AudioSource audioSource;
    private Vector3 lastPoint;
    private bool destroying;

    public bool firedWhileCrouching;

    private void Awake()
    {
        lastPoint = transform.position;
        audioSource = GetComponent<AudioSource>();

        GetComponent<MeshRenderer>().material = data.material;
    }

    private void FixedUpdate()
    {
        if (data.velocity != 0)
        {
            GetComponent<Rigidbody>().velocity = data.velocity * transform.forward;
        }
        else
        {
            Debug.LogWarning("Projectile velocity for " + gameObject.name + " is zero");
        }

        movementDirection = (transform.position - lastPoint);
        Physics.Raycast(lastPoint, movementDirection.normalized, out RaycastHit hitInfo, movementDirection.magnitude);
        if (hitInfo.collider != null)
        {
            OnTriggerEnter(hitInfo.collider);
        }
        lastPoint = transform.position;
    }

    public GameObject lastHitCover;
    void OnTriggerEnter (Collider other)
    {
        if (!destroying)
        {
            if (other.gameObject.GetComponent<Cover>())
            {
                lastHitCover = other.gameObject;
            }

            Actor actor = other.gameObject.GetComponentInParent<Actor>();

            // please don't kill yo self or teammates
            if (actor != null && actor.team == owningActor.team)
                return;

            // don't destroy if hit actor's collider (only do so on hit box), (commented: also don't destroy if this is a collision with horizontal cover.)
            bool shouldDestroy = other.CompareTag("HitBox") || (other.CompareTag("Cover") && other.GetComponent<Cover>().coverType == Cover.CoverType.Floor && firedWhileCrouching);//&& lastHitCover != other.gameObject && actor == null);

            // only hit an actor if it's the actor's hit box
            if (actor != null && other.gameObject.GetComponent<HitBox>())
            {
                // may not always destroy if hit actor, i.e. if actor is in cover and it "missed"
                shouldDestroy = actor.ProcessHit(data.damage, this);

                if (data.isExplosive)
                {
                    // implement method per projectile types
                    CreateExplosion();
                }
            }

            // only destroy projectiles if they hit something solid
            if (shouldDestroy)
            {
                destroying = true;
                GetComponent<MeshRenderer>().enabled = false;
                StartCoroutine(BeginDestruction());
            }
        }
    }

    /// <summary>
    /// Init the damage, etc. and play the fire sound.
    /// </summary>
    /// <param name="firingActor">Actor who fired it</param>
    /// <param name="data">The data of the firing weapon (pass in when fired)</param>
    /// <param name="siblingNumber">If more than one bullet fired, which sibling is this?</param>
    public void Initialize(Actor firingActor, WeaponData data, int siblingNumber)
    {
        owningActor = firingActor;

        // make sure friendlie(?)'s bullet sounds are never cut off.
        if (owningActor.team == Actor.ActorTeam.Friendly)
        {
            audioSource.priority = 0;
        }

        // only play one sound when fired.
        if (siblingNumber < 1 && audioSource != null)
        {
            audioSource.clip = data.attackSound;
            audioSource.Play();
        }

        // just to tell if it should hit floor cover or not
        if (owningActor.state[Actor.State.Crouching])
        {
            firedWhileCrouching = true;
        }
    }

    public ProjectileData GetData()
    {
        return data;
    }

    /// <summary>
    /// Wait a period of time to allow audio source to fully play out.
    /// </summary>
    /// <returns></returns>
    private IEnumerator BeginDestruction()
    {
        yield return new WaitForSeconds(1.5f);
        Destroy(gameObject);
    }

    protected void CreateExplosion()
    {
        Debug.Log("Boom");
    }
}
