using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
//using UnityEngine.Rendering;
//using UnityEngine.Rendering.Universal;

public class FireballCollisionHandler : MonoBehaviour
{
    private float minDamage;
    private float maxDamage;
    private float impactRadius;
    public GameObject explosionEffectPrefab; // Assign in the inspector or via the Setup method

    //private Volume postProcessVolume;
    //private Coroutine chromaticAberrationCoroutine;


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

        // Check if the collision object has a MeshDeformer component
        MeshDeformer meshDeformer = collision.collider.GetComponent<MeshDeformer>();
        if (meshDeformer != null)
        {
            meshDeformer.ApplyDeformation(transform.position, impactRadius);
        }

        // Trigger the Cinemachine Impulse
        var impulseSource = GetComponent<Cinemachine.CinemachineImpulseSource>();
        if (impulseSource != null)
        {
            impulseSource.GenerateImpulse();
        }

        // Destroy the fireball after impact
        Destroy(gameObject);

    }

}
