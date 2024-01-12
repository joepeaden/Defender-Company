using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Actual in-game instance of an explosive.
/// </summary>
public class ExplosiveInstance : MonoBehaviour
{
    public ExplosiveData data;
    private AudioSource audioSource;
    //private Collider collision;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        //collision = GetComponent<Collider>();

        StartCoroutine("Explode");
    }

    private IEnumerator Explode()
    {
        yield return new WaitForSeconds(3f);

        // generate an explosive force
        Vector3 explosionPos = transform.position;

        // generate target hits
        Collider2D[] colliders = Physics2D.OverlapCircleAll(explosionPos, data.explosionRadius);
        foreach (Collider2D hit in colliders)
        {
            // keeping the shatter stuff - if we go back to 3D, that'll be a good idea.

            // lol
            //Shatter shat = hit.GetComponent<Shatter>();
            //if (shat != null)
            //{
            //    shat.ShatterObject();
            //}

            Actor hitActor = hit.GetComponent<Actor>();
            if (hitActor != null)
            {
                hitActor.ProcessHit(data.damage, fromExplosive: true);
            }

            Building hitBuilding = hit.GetComponent<Building>();
            if (hitBuilding != null)
            {
                hitBuilding.hitPoints -= data.damage;
            }

        }


        //Instantiate(data.explosionPrefab, transform.position, transform.rotation);

        audioSource.Play();
        //GetComponent<SpriteRenderer>().enabled = false;

        StartCoroutine("DestroyObject");
    }

    // wait 3 seconds before destroying object
    private IEnumerator DestroyObject()
    {
        //collision.enabled = false;

        yield return new WaitForSeconds(3f);

        Destroy(gameObject);
    }
}
