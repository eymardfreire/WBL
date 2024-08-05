using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class FecesBombCollisionHandler : MonoBehaviour
{
    private float minDamage = 40f; // Set your own values
    private float maxDamage = 60f; // Set your own values
    private float impactRadius = 1f; // Set your own values
    public GameObject explosionEffectPrefab; // Assign in the inspector or via the Setup method

    public void Setup(float minDamage, float maxDamage, float impactRadius, GameObject explosionEffectPrefab)
    {
        this.minDamage = minDamage;
        this.maxDamage = maxDamage;
        this.impactRadius = impactRadius;
        this.explosionEffectPrefab = explosionEffectPrefab;
    }

    void OnCollisionEnter(Collision collision)
    {
        // Instantiate the explosion effect at the point of collision
        if (explosionEffectPrefab != null)
        {
            Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);
        }

        // Calculate random damage within the specified range
        float damage = Random.Range(minDamage, maxDamage);

        // Apply damage to the hit object if it has a Damageable component
        Damageable damageable = collision.collider.GetComponent<Damageable>();
        if (damageable != null)
        {
            damageable.ApplyDamage(damage);
        }

        DamageableEnvironment damageableEnvironment = collision.collider.GetComponent<DamageableEnvironment>();
        if (damageableEnvironment != null)
        {
            damageableEnvironment.ApplyDamage(damage);
        }

        // Optional: If you want to deform the mesh on impact, you can use a MeshDeformer component
        MeshDeformer meshDeformer = collision.collider.GetComponent<MeshDeformer>();
        if (meshDeformer != null)
        {
            meshDeformer.ApplyDeformation(transform.position, impactRadius);
        }

        // Trigger the Cinemachine Impulse for camera shake
        CinemachineImpulseSource impulseSource = GetComponent<CinemachineImpulseSource>();
        if (impulseSource != null)
        {
            impulseSource.GenerateImpulse();
        }

        // Destroy the feces bomb after impact
        Destroy(gameObject);
    }
}
