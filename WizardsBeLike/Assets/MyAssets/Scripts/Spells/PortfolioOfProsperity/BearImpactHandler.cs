using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BearImpactHandler : MonoBehaviour
{
    public GameObject explosionEffectPrefab;
    public float minDamage;
    public float maxDamage;
    public float impactRadius;
    public float deformRadius;

    public void Setup(float minDamage, float maxDamage, float impactRadius, float deformRadius, GameObject explosionEffectPrefab)
    {
        this.minDamage = minDamage;
        this.maxDamage = maxDamage;
        this.impactRadius = impactRadius;
        this.deformRadius = deformRadius;
        this.explosionEffectPrefab = explosionEffectPrefab;
    }
    
    void OnCollisionEnter(Collision collision)
    {
        Explode();
        // Optionally, you could destroy the bear instance here or in the Explode method
        Destroy(gameObject);
    }

    public void Explode()
    {
        // Instantiate the explosion effect at the bear's position
        if (explosionEffectPrefab != null)
        {
            Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);
        }

        // Apply area-of-effect damage and deformation
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, impactRadius);
        foreach (var hitCollider in hitColliders)
        {
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

            // Perform mesh deformation if the collider has a MeshDeformer component
            MeshDeformer meshDeformer = hitCollider.GetComponent<MeshDeformer>();
            if (meshDeformer != null)
            {
                meshDeformer.ApplyDeformation(transform.position, deformRadius);
            }
        }

        // Generate cinematic impulse if available
        var impulseSource = GetComponent<Cinemachine.CinemachineImpulseSource>();
        if (impulseSource != null)
        {
            impulseSource.GenerateImpulse();
        }
    }
}
