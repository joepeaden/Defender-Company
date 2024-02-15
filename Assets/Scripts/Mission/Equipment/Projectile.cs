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
    //private Vector3 lastPoint;
    private bool destroying;

    public bool firedWhileCrouching;

    private BoxCollider2D theCollider;
    private SpriteRenderer spriteRenderer;
    private float spawnTime;
    private int damage;

    private int FiringHeightLevel;
    private int TargetHeightLevel;

    public float lifetime;

    public float projVelocity;

    private Vector3 lastFixedUpdatePos;

    public bool useProjectilePhysics;

    private void OnEnable()
    {
        //lastPoint = transform.position;
        //audioSource = GetComponent<AudioSource>();
        //theCollider = GetComponent<BoxCollider2D>();
        //spriteRenderer = GetComponent<SpriteRenderer>();

        lastFixedUpdatePos = transform.position;

        spawnTime = Time.time;
    }

    private void FixedUpdate()
    {
        if (Time.time - spawnTime > lifetime)
        {
            gameObject.SetActive(false);
        }

        if (useProjectilePhysics)
        {
            float travelDist = (transform.position - lastFixedUpdatePos).magnitude;
            RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, -transform.up * travelDist, travelDist, LayerMask.GetMask("HitBoxes", "Obstacle"));
            foreach (RaycastHit2D hit in hits)
            {
                if (hit.transform.GetComponent<HitBox>() != null)
                {
                    hit.transform.GetComponentInParent<Actor>().ProcessHit(FiringHeightLevel)
                }
            }
        }

        //    if (data.projVelocity != 0)
        //    {
        GetComponent<Rigidbody2D>().velocity = projVelocity * transform.up;
        //    }
        //    else
        //    {
        //        Debug.LogWarning("Projectile velocity for " + gameObject.name + " is zero");
        //    }

        lastFixedUpdatePos = transform.position;
    }

    /// <summary>
    /// Init the damage, etc. and play the fire sound.
    /// </summary>
    /// <param name="firingActor">Actor who fired it</param>
    /// <param name="weaponData">The data of the firing weapon (pass in when fired)</param>
    /// <param name="siblingNumber">If more than one bullet fired, which sibling is this?</param>
    public void Initialize(Actor firingActor, WeaponData weaponData, int siblingNumber)
    {
        spawnTime = Time.time;

        data = weaponData;
        actor = firingActor;
        damage = data.damage;

        if (actor.IsPlayer)
        {
            GameObject playerAttackTarget = MissionManager.Instance.Player.FindPlayerTarget();
            // target height lvl is ignored for player
            TargetHeightLevel = int.MaxValue;
            FiringHeightLevel = playerAttackTarget != null ? actor.HeightLevel > playerAttackTarget.GetComponent<Actor>().HeightLevel ? actor.HeightLevel : playerAttackTarget.GetComponent<Actor>().HeightLevel : actor.HeightLevel;
        }
        else
        {
            GameObject aiAttackTarget = actor.GetComponent<AIActorController>().AttackTarget;
            TargetHeightLevel = aiAttackTarget != null ? aiAttackTarget.GetComponent<Actor>().HeightLevel : int.MaxValue;
            FiringHeightLevel = aiAttackTarget != null ? (actor.HeightLevel > aiAttackTarget.GetComponent<Actor>().HeightLevel ? actor.HeightLevel : aiAttackTarget.GetComponent<Actor>().HeightLevel) : actor.HeightLevel;
        }


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


        RaycastHit2D[] hits = Physics2D.RaycastAll(actor.transform.position, transform.up, 500f, LayerMask.GetMask("HitBoxes", "Obstacle"));

        foreach (RaycastHit2D hit in hits)
        {
            //if (hit)
            //{
                //Actor hitActor = hit.transform.GetComponentInParent<Actor>();

                // maybe use a different way to see if it's a building

                Actor actor = hit.transform.GetComponentInParent<Actor>();
                if (actor != null)
                {
                    HitTarget(hit.transform.GetComponent<Collider2D>());
                }

            //}
        }
    }



    //public GameObject lastHitCover;
    void HitTarget (Collider2D other)
    {
        if (!destroying)
        {

            Actor hitActor = other.gameObject.GetComponentInParent<Actor>();

            // please don't kill yo self or teammates
            if (hitActor != null && hitActor.team == this.actor.team)
                return;

            //HitBox hitBox = other.gameObject.GetComponent<HitBox>();
            Building building = other.GetComponent<Building>();

            // if hit a hit box or a building and at the correct height level, need to destroy bullet
            bool shouldDestroy = (hitActor != null && (hitActor.HeightLevel == TargetHeightLevel || actor.IsPlayer)) || (building != null && building.HeightLevel >= FiringHeightLevel); //|| (other.CompareTag("Cover") && other.GetComponent<Cover>().coverType == Cover.CoverType.Floor && firedWhileCrouching);//&& lastHitCover != other.gameObject && actor == null);

            // if hitting an actor's hitbox and we're at the same height level, process hit
            if (hitActor != null && (hitActor.HeightLevel == TargetHeightLevel || actor.IsPlayer))
            {
                // may not always destroy if hit actor, i.e. if actor is in cover and it "missed"
                //shouldDestroy = hitActor.ProcessHit(damagetarget);//, projectile: this);

                //if (data.isExplosive)
                //{
                //    // implement method per projectile types
                //    CreateExplosion();
                //}
            }

            // only destroy projectiles if they hit something solid
            if (shouldDestroy)
            {
                gameObject.SetActive(false);
                //Destroy(gameObject);
                //destroying = true;
                //StartCoroutine(BeginDestruction());
            }
        }
    }

    /// <summary>
    /// Wait a period of time to allow audio source to fully play out.
    /// </summary>
    /// <returns></returns>
    //private IEnumerator BeginDestruction()
    //{
    //    GetComponent<Collider2D>().enabled = false;
    //    GetComponentInChildren<SpriteRenderer>().enabled = false;
    //    yield return new WaitForSeconds(1.5f);
    //    Destroy(gameObject);
    //}

    public WeaponData GetData()
    {
        return data;
    }

    //protected void CreateExplosion()
    //{
    //    Debug.Log("Boom");
    //}
}
