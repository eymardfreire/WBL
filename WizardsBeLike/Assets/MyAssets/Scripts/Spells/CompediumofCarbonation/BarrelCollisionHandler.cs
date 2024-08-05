using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrelCollisionHandler : MonoBehaviour
{
    private float minDamage;
    private float maxDamage;
    private float impactRadius;
    private float deformRadius; // Radius for deforming the mesh
    private GameObject explosionEffectPrefab;
    private bool hasBounced = false;
    private float delayBeforeExplosion;
    private Collision lastCollision; // To store the last collision
    private Rigidbody rb; // Rigidbody component for applying physics
    private Collision storedCollision;
    public GameObject[] piecePrefabs;
    public float explosionForce = 1000f; // Adjust the force as needed
    public float explosionRadius = 5f; // Adjust the radius as needed
    public Vector3 explosionOffset = new Vector3(0, 1, 0); // Adjust the offset as needed


    // Updated Setup function to include impactRadius
    void Awake()
{
    rb = GetComponent<Rigidbody>();
}
    public void Setup(float minDamage, float maxDamage, float impactRadius, float deformRadius, GameObject explosionEffectPrefab, float delayBeforeExplosion)
    {
        this.minDamage = minDamage;
        this.maxDamage = maxDamage;
        this.impactRadius = impactRadius;
        this.deformRadius = deformRadius;
        this.explosionEffectPrefab = explosionEffectPrefab;
        this.delayBeforeExplosion = delayBeforeExplosion;
    }

     void OnCollisionEnter(Collision collision)
    {
         // Safety check to ensure rb is not null
        if (rb == null)
        {
            rb = GetComponent<Rigidbody>();
            // If rb is still null, log an error and return to prevent further execution
            if (rb == null)
            {
                return;
            }
        }

        storedCollision = collision;
        
         if (!hasBounced)
        {
            hasBounced = true;

            // Use the magnitude of the velocity and a fixed bounce factor to get a consistent bounce height
            //float bounceForce = rb.velocity.magnitude * bounceIntensity;
        
            // Apply the bounce force upwards, regardless of the collision angle
            //rb.AddForce(Vector3.up * bounceForce, ForceMode.VelocityChange);

            // Start the countdown to explosion
            StartCoroutine(ExplodeAfterDelay());
        }
       
    }

    IEnumerator ExplodeAfterDelay()
    {
        yield return new WaitForSeconds(delayBeforeExplosion);
        Explode();
    }

    void Explode()
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

    // Attempt to deform the mesh of the ground upon explosion
    
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
