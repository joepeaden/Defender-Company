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
    private Collider collision;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        collision = GetComponent<Collider>();

        StartCoroutine("Explode");
    }

    private IEnumerator Explode()
    {
        yield return new WaitForSeconds(3f);

        // generate an explosive force
        Vector3 explosionPos = transform.position;

        // generate target hits
        Collider[] colliders = Physics.OverlapSphere(explosionPos, data.explosionRadius);
        foreach (Collider hit in colliders)
        {
            // lol
            Shatter shat = hit.GetComponent<Shatter>();
            if (shat != null)
            {
                shat.ShatterObject();
            }

            //Actor hitActor = hit.GetComponent<Actor>();
            //if (hitActor != null)
            //{
            //    hitActor.GetHit(data.damage);
            //}
        }

        // apply force
        // colliders is retrieved twice becasue if there's a Shatter object now we need to detect the pieces.
        // Yeah, there's better ways to do this, whatever.
        colliders = Physics.OverlapSphere(explosionPos, data.explosionRadius);
        foreach (Collider hit in colliders)
        {
            Rigidbody rb = hit.GetComponent<Rigidbody>();

            if (rb != null)
                rb.AddExplosionForce(data.explosionPower, explosionPos, data.explosionRadius, data.upwardsForce);
        }

        Instantiate(data.explosionPrefab, transform.position, transform.rotation);

        audioSource.Play();
        GetComponent<MeshRenderer>().enabled = false;

        StartCoroutine("DestroyObject");
    }

    // wait 3 seconds before destroying object
    private IEnumerator DestroyObject()
    {
        collision.enabled = false;

        yield return new WaitForSeconds(3f);

        Destroy(gameObject);
    }
}
