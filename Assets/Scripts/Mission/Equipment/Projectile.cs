using System.Collections;
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

    protected WeaponData data;

    public Actor Actor => actor;
    protected Actor actor;
    //protected int damage;

    private AudioSource audioSource;
    private Vector3 lastPoint;
    private bool destroying;

    public bool firedWhileCrouching;

    private BoxCollider2D theCollider;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        lastPoint = transform.position;
        audioSource = GetComponent<AudioSource>();
        theCollider = GetComponent<BoxCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void FixedUpdate()
    {
        if (data.projVelocity != 0)
        {
            GetComponent<Rigidbody2D>().velocity = data.projVelocity * transform.up;
        }
        else
        {
            Debug.LogWarning("Projectile velocity for " + gameObject.name + " is zero");
        }

        //movementDirection = (transform.position - lastPoint);
        //RaycastHit2D hitInfo = Physics2D.Raycast(lastPoint, movementDirection.normalized, movementDirection.magnitude);
        //if (hitInfo.collider != null)
        //{
        //    OnTriggerEnter2D(hitInfo.collider);
        //}
        //lastPoint = transform.position;
    }

    /// <summary>
    /// Init the damage, etc. and play the fire sound.
    /// </summary>
    /// <param name="firingActor">Actor who fired it</param>
    /// <param name="weaponData">The data of the firing weapon (pass in when fired)</param>
    /// <param name="siblingNumber">If more than one bullet fired, which sibling is this?</param>
    public void Initialize(Actor firingActor, WeaponData weaponData, int siblingNumber)
    {
        data = weaponData;
        actor = firingActor;

        // make sure friendlie(?)'s bullet sounds are never cut off.
        if (actor.team == Actor.ActorTeam.Friendly)
        {
            audioSource.priority = 0;
        }

        theCollider.size = new Vector2(data.projColWidth, data.projColLength);
        spriteRenderer.sprite = data.projSprite;

        // only play one sound when fired.
        if (siblingNumber < 1 && audioSource != null)
        {
            audioSource.clip = weaponData.attackSound;
            audioSource.Play();
        }

        // just to tell if it should hit floor cover or not
        if (actor.state[Actor.State.Crouching])
        {
            firedWhileCrouching = true;
        }
    }



    public GameObject lastHitCover;
    void OnTriggerEnter2D (Collider2D other)
    {
        if (!destroying)
        {
            if (other.gameObject.GetComponent<Cover>())
            {
                lastHitCover = other.gameObject;
            }

            Actor actor = other.gameObject.GetComponentInParent<Actor>();

            // please don't kill yo self or teammates
            if (actor != null && actor.team == this.actor.team)
                return;

            // if hit a HIT BOX (not other actor components) or a building, need to destroy bullet
            bool shouldDestroy = other.CompareTag("HitBox") || other.GetComponent<Building>() != null; //|| (other.CompareTag("Cover") && other.GetComponent<Cover>().coverType == Cover.CoverType.Floor && firedWhileCrouching);//&& lastHitCover != other.gameObject && actor == null);

            // only hit an actor if it's the actor's hit box
            if (actor != null && other.gameObject.GetComponent<HitBox>())
            {
                // may not always destroy if hit actor, i.e. if actor is in cover and it "missed"
                shouldDestroy = actor.ProcessHit(data.damage, projectile: this);

                //if (data.isExplosive)
                //{
                //    // implement method per projectile types
                //    CreateExplosion();
                //}
            }

            // only destroy projectiles if they hit something solid
            if (shouldDestroy)
            {
                destroying = true;
                //GetComponent<MeshRenderer>().enabled = false;
                StartCoroutine(BeginDestruction());
            }
        }
    }

    /// <summary>
    /// Wait a period of time to allow audio source to fully play out.
    /// </summary>
    /// <returns></returns>
    private IEnumerator BeginDestruction()
    {
        GetComponent<Collider2D>().enabled = false;
        GetComponentInChildren<SpriteRenderer>().enabled = false;
        yield return new WaitForSeconds(1.5f);
        Destroy(gameObject);
    }

    public WeaponData GetData()
    {
        return data;
    }

    //protected void CreateExplosion()
    //{
    //    Debug.Log("Boom");
    //}
}
