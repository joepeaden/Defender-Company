using System.Collections;
using UnityEngine;

/// <summary>
/// Handles sprite animations for actors as well as blood effects.
/// </summary>
public class ActorSprite : MonoBehaviour
{
    [SerializeField] private Actor actor;
    [SerializeField] private GameObject bloodPoofEffect;
    [SerializeField] private Sprite actorFrontFacing;
    [SerializeField] private Sprite actorBackFacing;
    [SerializeField] private Sprite actorRightFacing;
    [SerializeField] private Sprite actorLeftFacing;

    private SpriteRenderer spriteRend;
    //[SerializeField] private AnimationClip reloadClip;

    void Start()
    {
        actor.OnDeath.AddListener(HandleActorDeath);

        spriteRend = GetComponent<SpriteRenderer>();

        // leaving all this in just in case we add anims later.
        //actor.OnGetHit.AddListener(HandleActorHit);
        //actor.OnCrouch.AddListener(HandleCrouch);
        //actor.OnStand.AddListener(HandleStand);
        //reloadClip = ragAnim.runtimeAnimatorController.animationClips.Where(clip => clip.name == "Reload").FirstOrDefault();
        //actor.EmitVelocityInfo.AddListener(UpdateVelocityBasedAnimations);
    }

    private void OnDestroy()
    {
        actor.OnDeath.RemoveListener(HandleActorDeath);
        actor.EmitVelocityInfo.RemoveListener(UpdateVelocityBasedAnimations);
    }

    private void HandleActorDeath()
    {
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
        if (actor.RotationParent.rotation.eulerAngles.y > 315 || actor.RotationParent.rotation.eulerAngles.y < 45)
        {
            spriteRend.sprite = actorBackFacing;
        }
        else if (actor.RotationParent.rotation.eulerAngles.y > 225 && actor.RotationParent.rotation.eulerAngles.y < 315)
        {
            spriteRend.sprite = actorLeftFacing;
        }
        else if (actor.RotationParent.rotation.eulerAngles.y > 135 && actor.RotationParent.rotation.eulerAngles.y < 225)
        {
            spriteRend.sprite = actorFrontFacing;
        }
        else
        {
            spriteRend.sprite = actorRightFacing;
        }
    }

    private void LateUpdate()
    {
        transform.position = actor.RotationParent.position;
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
        //transform.localScale = new Vector3(1f, .5f, 1f);
        //transform.localPosition = new Vector3(0f, 0f, 0f);
    }

    private void HandleStand()
    {
        //transform.localScale = originalDimensions;
        //transform.localPosition = new Vector3(0f, 0.5f, 0f);
    }

    #endregion
}
