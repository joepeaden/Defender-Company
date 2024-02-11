using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles sprite animations for actors as well as blood effects.
/// </summary>
public class ActorSprite : MonoBehaviour
{
    [SerializeField] private Actor actor;
    //[SerializeField] private GameObject bloodPoofEffect;
    private List<Sprite> bloodSplats;
    private List<Sprite> bodyParts;

    //[SerializeField] private Sprite actorFrontStand;
    //[SerializeField] private Sprite actorBackStand;
    //[SerializeField] private Sprite actorRightStand;
    //[SerializeField] private Sprite actorLeftStand;
    //[SerializeField] private Sprite actorFrontCrouch;
    //[SerializeField] private Sprite actorBackCrouch;
    //[SerializeField] private Sprite actorRightCrouch;
    //[SerializeField] private Sprite actorLeftCrouch;
    [SerializeField] private List<Sprite> deadSprites;

    //private Sprite frontSprite;
    //private Sprite backSprite;
    //private Sprite leftSprite;
    //private Sprite rightSprite;

    [SerializeField] private Animator animator;

    public SpriteRenderer spriteRend;
    //[SerializeField] private AnimationClip reloadClip;

    public SpriteRenderer faceSpriteRend;

    private bool isMoving;

    private void Awake()
    {
        actor.OnDataSet.AddListener(HandleActorDataSet);
    }

    void Start()
    {
        actor.OnDeath.AddListener(HandleActorDeath);
        actor.OnGetHit.AddListener(HandleActorHit);

        

        // leaving all this in just in case we add anims later.
        //actor.OnGetHit.AddListener(HandleActorHit);
        //actor.OnCrouch.AddListener(HandleCrouch);
        //actor.OnStand.AddListener(HandleStand);
        //reloadClip = ragAnim.runtimeAnimatorController.animationClips.Where(clip => clip.name == "Reload").FirstOrDefault();
        actor.EmitVelocityInfo.AddListener(UpdateVelocityBasedAnimations);

        //frontSprite = actorFrontStand;
        //backSprite = actorBackStand;
        //leftSprite = actorLeftStand;
        //rightSprite = actorRightStand;

    }

    private void OnDestroy()
    {
        actor.OnDeath.RemoveListener(HandleActorDeath);
        actor.EmitVelocityInfo.RemoveListener(UpdateVelocityBasedAnimations);
        //actor.OnCrouch.RemoveListener(HandleCrouch);
        //actor.OnStand.RemoveListener(HandleStand);
        actor.OnDataSet.RemoveListener(HandleActorDataSet);
    }

    private void HandleActorDataSet()
    {
        spriteRend.sprite = actor.Data.sprite;
        bloodSplats = actor.Data.bloodSplats;
        bodyParts = actor.Data.bodyPartSprites;
        deadSprites = actor.Data.deadSprites;


        if (actor.team == Actor.ActorTeam.Friendly)
        {
            faceSpriteRend.sprite = actor.GetComponent<FriendlyActorController>().TheCompanySoldier.Face;
        }

    }

    private void HandleActorDeath(bool fromExplosive)
    {
        if (fromExplosive)
        {
            Quaternion randomRotation = Quaternion.Euler(new Vector3(0, 0, Random.Range(0, 359)));
            GameObject bloodSplat = Instantiate(new GameObject(), transform.position, randomRotation);
            SpriteRenderer bloodSpltSpriteRenderer = bloodSplat.AddComponent<SpriteRenderer>();
            bloodSpltSpriteRenderer.sprite = bloodSplats[Random.Range(0, bloodSplats.Count)];

            randomRotation = Quaternion.Euler(new Vector3(0, 0, Random.Range(0, 359)));
            GameObject bodyPart = Instantiate(new GameObject(), transform.position, randomRotation);
            SpriteRenderer bodyPartSpriteRenderer = bodyPart.AddComponent<SpriteRenderer>();
            bodyPartSpriteRenderer.sprite = bodyParts[0];
            Rigidbody2D rb = bodyPart.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0f;
            rb.drag = Random.Range(3f, 5f);
            float randomForce = Random.Range(200f, 800f);
            Vector2 direction = new Vector2(0f, Random.Range(-1f, 1f));
            rb.AddForceAtPosition(direction * randomForce, transform.position);
            randomForce = Random.Range(200f, 800f);
            direction = new Vector2(Random.Range(-1f, 1f), 0f);
            rb.AddForceAtPosition(direction * randomForce, transform.position);

            randomRotation = Quaternion.Euler(new Vector3(0, 0, Random.Range(0, 359)));
            bodyPart = Instantiate(new GameObject(), transform.position, randomRotation);
            bodyPartSpriteRenderer = bodyPart.AddComponent<SpriteRenderer>();
            bodyPartSpriteRenderer.sprite = bodyParts[1];
            rb = bodyPart.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0f;
            rb.drag = Random.Range(3f, 5f);
            randomForce = Random.Range(200f, 800f);
            direction = new Vector2(Random.Range(-1f, 1f), 0f);
            rb.AddForceAtPosition(direction * randomForce, transform.position);
            randomForce = Random.Range(200f, 800f);
            direction = new Vector2(0f, Random.Range(-1f, 1f));
            rb.AddForceAtPosition(direction * randomForce, transform.position);

            randomRotation = Quaternion.Euler(new Vector3(0, 0, Random.Range(0, 359)));
            bodyPart = Instantiate(new GameObject(), transform.position, randomRotation);
            bodyPartSpriteRenderer = bodyPart.AddComponent<SpriteRenderer>();
            bodyPartSpriteRenderer.sprite = bodyParts[2];
            rb = bodyPart.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0f;
            rb.drag = Random.Range(3f, 5f);
            randomForce = Random.Range(200f, 800f);
            direction = new Vector2(0f, Random.Range(-1f, 1f));
            rb.AddForceAtPosition(direction * randomForce, transform.position);
            randomForce = Random.Range(200f, 800f);
            direction = new Vector2(Random.Range(-1f, 1f), 0f);
            rb.AddForceAtPosition(direction * randomForce, transform.position);

            spriteRend.enabled = false;
        }
        else
        {
            spriteRend.sprite = deadSprites[Random.Range(0, deadSprites.Count)];
        }
        //StartCoroutine(DeletePoofAfterWait(poof));
    }

    private void HandleActorHit()
    {
        // don't
        // lol, don't what?
        if (Random.Range(0, 10) > 7)
        {
            GameObject bloodSplat = Instantiate(new GameObject(), transform.position, transform.rotation);
            SpriteRenderer bloodSpltSpriteRenderer = bloodSplat.AddComponent<SpriteRenderer>();
            bloodSpltSpriteRenderer.sprite = bloodSplats[Random.Range(0, bloodSplats.Count)];
        }
    }

    //private IEnumerator DeletePoofAfterWait(GameObject poof)
    //{
    //    yield return new WaitForSeconds(1.5f);
    //    Destroy(poof);
    //}

    private void Update()
    {
        //Vector2 facing = actor.GetActorFacing();

        //if (facing == Vector2.up)
        //{
        //    spriteRend.sprite = backSprite;
        //}

        //if (facing == Vector2.left)
        //{
        //    spriteRend.sprite = leftSprite;
        //}

        //if (facing == Vector2.down)
        //{
        //    spriteRend.sprite = frontSprite;
        //}

        //if (facing == Vector2.right)
        //{
        //    spriteRend.sprite = rightSprite;
        //}
    }

        //    if (actor.IsAlive)
        //    {
        //        if (actor.transform.rotation.eulerAngles.y > 315 || actor.transform.rotation.eulerAngles.y < 45)
        //        {
        //            spriteRend.sprite = backSprite;
        //        }
        //        else if (actor.transform.rotation.eulerAngles.y > 225 && actor.transform.rotation.eulerAngles.y < 315)
        //        {
        //            spriteRend.sprite = leftSprite;
        //        }
        //        else if (actor.transform.rotation.eulerAngles.y > 135 && actor.transform.rotation.eulerAngles.y < 225)
        //        {
        //            spriteRend.sprite = frontSprite;
        //        }
        //        else
        //        {
        //            spriteRend.sprite = rightSprite;
        //        }
        //    }
    

    private void LateUpdate()
    {
        transform.position = actor.transform.position;
    }

    // leaving all this in just in case we add anims later.
    #region Commented Animation Stuff

    // not using it, but just in case, who knows if i'll need it later
    // ya know what i mean?
    // work was tough this week.
    //private IEnumerator CheckOutline()
    //{
    //    while (actor.IsAlive)
    //    {
    //        Ray r = new Ray(Camera.main.transform.position, transform.position - Camera.main.transform.position);
    //        if (Physics.Raycast(ray: r, out RaycastHit hitInfo, maxDistance: float.MaxValue, layerMask: ~LayerMask.GetMask(LayerNames.CollisionLayers.MouseOnly.ToString())))
    //        {
    //            // should highlight
    //            if (hitInfo.transform.GetComponent<Actor>() == null && hitInfo.transform.GetComponentInParent<Actor>() == null)
    //            {
    //                if (!isHighlighted)
    //                {
    //                    gameObject.layer = actor.IsPlayer ? (int)LayerNames.CollisionLayers.PlayerOutline : (int)LayerNames.CollisionLayers.EnemyOutline;

    //                    // this could be waaay optimized.
    //                    //Transform[] transforms = GetComponentsInChildren<Transform>();
    //                    foreach (GameObject g in bodyParts)
    //                    {
    //                        g.layer = actor.IsPlayer ? (int)LayerNames.CollisionLayers.PlayerOutline : (int)LayerNames.CollisionLayers.EnemyOutline;
    //                    }

    //                    isHighlighted = true;
    //                }
    //            }
    //            // should unhighlight
    //            else if (isHighlighted)
    //            {
    //                gameObject.layer = (int)LayerNames.CollisionLayers.IgnoreActors;

    //                foreach (GameObject g in bodyParts)
    //                {
    //                    g.layer = (int)LayerNames.CollisionLayers.IgnoreActors;
    //                }

    //                isHighlighted = false;
    //            }
    //        }

    //        yield return new WaitForSeconds(.1f);
    //    }
    //}

    private void HandleActorHit(Projectile projectile)
    {
        //projectileThatKilledJim = projectile;

        //if (actor.IsAlive)
        //{
        //    ragAnim.SetTrigger("Wound");
        //}
    }

    public void UpdateVelocityBasedAnimations(Vector3 velocity)
    {
        //float vert = Vector3.Dot(velocity, transform.forward);
        //float horiz = Vector3.Dot(velocity, transform.right);

        float velocityValue = velocity.magnitude;
        // make sure it's negative so it's "less than zero" so that the anim controller knows we stopped
        if (velocity.magnitude == 0)
        {
            velocityValue = -1f;
            isMoving = false;
        }
        else if (!isMoving)
        {
            isMoving = true;
            animator.SetFloat("AnimOffset", Random.Range(0f, 1f));
        }
        animator.SetFloat("Velocity", velocityValue);
        //animator.SetFloat("VerticalAxis", vert);
        //animator.SetFloat("HorizontalAxis", horiz);
    }

    public void StartReloadAnimation(float reloadWeaponDuration)
    {
        //ragAnim.SetTrigger("Reload");

        //ragAnim.SetFloat("ReloadSpeed", reloadClip.length / reloadWeaponDuration);
    }

    public void StartInteractAnimation()
    {
        //ragAnim.SetTrigger("Interact");
    }

    public void UpdateWeaponAnimation()
    {
        //ragAnim.SetTrigger("DrawHandgun");
    }

    public void FireAnimation()
    {
        //ragAnim.SetTrigger("Fire");
    }

    //private void HandleCrouch()
    //{
        //frontSprite = actorFrontCrouch;
        //backSprite = actorBackCrouch;
        //leftSprite = actorLeftCrouch;
        //rightSprite = actorRightCrouch;
    //}

    //private void HandleStand()
    //{
        //frontSprite = actorFrontStand;
        //backSprite = actorBackStand;
        //leftSprite = actorLeftStand;
        //rightSprite = actorRightStand;
    //}

    #endregion
}
