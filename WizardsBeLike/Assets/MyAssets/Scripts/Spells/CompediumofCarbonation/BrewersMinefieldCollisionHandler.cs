using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrewersMinefieldCollisionHandler : MonoBehaviour
{
    private float minDamage;
    private float maxDamage;
    private float impactRadius;
    private float deformRadius; // Radius for deforming the mesh
    private GameObject explosionEffectPrefab;
    private Rigidbody rb; // Rigidbody component for applying physics
    public GameObject[] piecePrefabs;
    public float explosionForce = 1000f; // Adjust the force as needed
    public float explosionRadius = 5f; // Adjust the radius as needed
    public Vector3 explosionOffset = new Vector3(0, 1, 0); // Adjust the offset as needed
    private GameObject stickingEffectPrefab;


    // Updated Setup function to include impactRadius
    void Awake()
{
    rb = GetComponent<Rigidbody>();
}
    public void Setup(float minDamage, float maxDamage, float impactRadius, float deformRadius, GameObject explosionEffectPrefab, GameObject stickingEffectPrefab)
    {
        this.minDamage = minDamage;
        this.maxDamage = maxDamage;
        this.impactRadius = impactRadius;
        this.deformRadius = deformRadius;
        this.explosionEffectPrefab = explosionEffectPrefab;
        this.stickingEffectPrefab = stickingEffectPrefab;
    }

    void OnCollisionEnter(Collision collision)
    {
        StickToSurface(collision); // Pass the entire collision object
    }

    
    private void StickToSurface(Collision collision)
    {
        rb.isKinematic = true; // Disable physics interactions
        ContactPoint contact = collision.contacts[0];

        // Position adjustment to avoid clipping with the surface
        transform.position = contact.point + contact.normal * 0.1f; // Small offset along the normal
        transform.up = contact.normal; // Orient the barrel so it's "up" is away from the surface

        // Disable the collider to prevent further OnCollisionEnter events
        Collider collider = GetComponent<Collider>();
        if (collider != null)
        {
            collider.enabled = false;
        }
        if (stickingEffectPrefab != null)
        {
        Instantiate(stickingEffectPrefab, transform.position, Quaternion.identity);
        }
    }

    public void Explode()
    {
        // Instantiate the explosion effect at the point of collision
        if (explosionEffectPrefab != null)
        {
            Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);
        }

        // Apply area-of-effect damage
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, impactRadius);
        foreach (var hitCollider in hitColliders)
        {
            // Check for damageable and apply damage
            Damageable damageable = hitCollider.GetComponent<Damageable>();
            if (damageable != null)
            {
                float damage = Random.Range(minDamage, maxDamage);
                damageable.ApplyDamage(damage);
            }

            DamageableEnvironment damageableEnvironment = hitCollider.GetComponent<DamageableEnvironment>();
            if (damageableEnvironment != null)
            {
                float damage = Random.Range(minDamage, maxDamage);
                damageableEnvironment.ApplyDamage(damage);
            }

            // Check for MeshDeformer and apply deformation
            MeshDeformer meshDeformer = hitCollider.GetComponent<MeshDeformer>();
            if (meshDeformer != null)
            {
                // Use the barrel's last position for deformation
                meshDeformer.ApplyDeformation(transform.position, deformRadius);
            }
        }

        BreakIntoPieces();

        // Trigger the Cinemachine Impulse if available
        var impulseSource = GetComponent<Cinemachine.CinemachineImpulseSource>();
        if (impulseSource != null)
        {
            impulseSource.GenerateImpulse();
        }

        // Destroy the barrel after explosion
        Destroy(gameObject);
    }

    private void BreakIntoPieces()
    {
        foreach (GameObject piecePrefab in piecePrefabs)
        {
            GameObject piece = Instantiate(piecePrefab, transform.position + explosionOffset, transform.rotation);
            Rigidbody rb = piece.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddExplosionForce(explosionForce, transform.position + explosionOffset, explosionRadius);
            }
        }
    }
}

