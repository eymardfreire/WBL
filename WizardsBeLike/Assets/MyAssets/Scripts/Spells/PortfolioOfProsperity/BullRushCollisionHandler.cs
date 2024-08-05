using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BullRushCollisionHandler : MonoBehaviour
{
    private float minDamage;
    private float maxDamage;
    private float impactRadius;
    private float deformRadius;
    private GameObject explosionEffectPrefab;
    private bool hasBounced = false;
    private float delayBeforeExplosion;
    private Rigidbody rb;

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
        if (!hasBounced)
        {
            hasBounced = true;
            StartCoroutine(ExplodeAfterDelay());
        }
    }

    private IEnumerator ExplodeAfterDelay()
    {
        yield return new WaitForSeconds(delayBeforeExplosion);
        Explode();
    }

    private void Explode()
    {
        // Instantiate the explosion effect at the point of collision
        if (explosionEffectPrefab != null)
        {
            Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);
        }

        // Apply area-of-effect damage and deformation
        ApplyDamageAndDeformation();

        // Generate cinematic impulse if available
        GenerateImpulse();

        // Destroy the bull object after explosion
        Destroy(gameObject);
    }

    private void ApplyDamageAndDeformation()
    {
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

            MeshDeformer meshDeformer = hitCollider.GetComponent<MeshDeformer>();
            if (meshDeformer != null)
            {
                meshDeformer.ApplyDeformation(transform.position, deformRadius);
            }
        }
    }

    private void GenerateImpulse()
    {
        var impulseSource = GetComponent<Cinemachine.CinemachineImpulseSource>();
        if (impulseSource != null)
        {
            impulseSource.GenerateImpulse();
        }
    }
}