using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles sprite animations for actors as well as blood effects.
/// </summary>
public class ActorSprite : MonoBehaviour
{
    [SerializeField] private Actor actor;
    [SerializeField] private GameObject bloodPoofEffect;
    [SerializeField] private Sprite actorFrontStand;
    [SerializeField] private Sprite actorBackStand;
    [SerializeField] private Sprite actorRightStand;
    [SerializeField] private Sprite actorLeftStand;
    [SerializeField] private Sprite actorFrontCrouch;
    [SerializeField] private Sprite actorBackCrouch;
    [SerializeField] private Sprite actorRightCrouch;
    [SerializeField] private Sprite actorLeftCrouch;
    [SerializeField] private List<Sprite> deadSprites;

    private Sprite frontSprite;
    private Sprite backSprite;
    private Sprite leftSprite;
    private Sprite rightSprite;

    private SpriteRenderer spriteRend;
    //[SerializeField] private AnimationClip reloadClip;

    void Start()
    {
        actor.OnDeath.AddListener(HandleActorDeath);

        spriteRend = GetComponent<SpriteRenderer>();

        // leaving all this in just in case we add anims later.
        //actor.OnGetHit.AddListener(HandleActorHit);
        actor.OnCrouch.AddListener(HandleCrouch);
        actor.OnStand.AddListener(HandleStand);
        //reloadClip = ragAnim.runtimeAnimatorController.animationClips.Where(clip => clip.name == "Reload").FirstOrDefault();
        //actor.EmitVelocityInfo.AddListener(UpdateVelocityBasedAnimations);

        frontSprite = actorFrontStand;
        backSprite = actorBackStand;
        leftSprite = actorLeftStand;
        rightSprite = actorRightStand;
    }

    private void OnDestroy()
    {
        actor.OnDeath.RemoveListener(HandleActorDeath);
        actor.EmitVelocityInfo.RemoveListener(UpdateVelocityBasedAnimations);
        actor.OnCrouch.RemoveListener(HandleCrouch);
        actor.OnStand.RemoveListener(HandleStand);
    }

    private void HandleActorDeath()
    {
        spriteRend.sprite = deadSprites[Random.Range(0, deadSprites.Count)];

        GameObject poof = Instantiate(bloodPoofEffect, transform.position, Quaternion.identity);
        StartCoroutine(DeletePoofAfterWait(poof));
    }

    private IEnumerator DeletePoofAfterWait(GameObject poof)
    {
        yield return new WaitForSeconds(1.5f);
        Destroy(poof);
    }

    private void Update()
    {
        if (actor.IsAlive)
        {
            if (actor.transform.rotation.eulerAngles.y > 315 || actor.transform.rotation.eulerAngles.y < 45)
            {
                spriteRend.sprite = backSprite;
            }
            else if (actor.transform.rotation.eulerAngles.y > 225 && actor.transform.rotation.eulerAngles.y < 315)
            {
                spriteRend.sprite = leftSprite;
            }
            else if (actor.transform.rotation.eulerAngles.y > 135 && actor.transform.rotation.eulerAngles.y < 225)
            {
                spriteRend.sprite = frontSprite;
            }
            else
            {
                spriteRend.sprite = rightSprite;
            }
        }
    }

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

        //ragAnim.SetFloat("Velocity", velocity.magnitude);
        //ragAnim.SetFloat("VerticalAxis", vert);
        //ragAnim.SetFloat("HorizontalAxis", horiz);
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

    private void HandleCrouch()
    {
        frontSprite = actorFrontCrouch;
        backSprite = actorBackCrouch;
        leftSprite = actorLeftCrouch;
        rightSprite = actorRightCrouch;
    }

    private void HandleStand()
    {
        frontSprite = actorFrontStand;
        backSprite = actorBackStand;
        leftSprite = actorLeftStand;
        rightSprite = actorRightStand;
    }

    #endregion
}
