using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : MonoBehaviour
{
    public float minDamage = 40f; // The minimum damage the fireball can do on impact
    public float maxDamage = 60f; // The maximum damage the fireball can do on impact
    public GameObject explosionEffect; // Assign an explosion effect prefab
    public float impactRadius = 1f; // Default impact radius for the fireball

    

    void OnCollisionEnter(Collision collision)
    {
        // Instantiate the explosion effect
        Instantiate(explosionEffect, transform.position, Quaternion.identity);

        // Calculate random damage within the specified range
        float damage = Random.Range(minDamage, maxDamage);

        // Apply damage to the hit object if it has a damageable component
        var damageable = collision.collider.GetComponent<Damageable>();
        if (damageable != null)
        {
            damageable.ApplyDamage(damage);
        }

         // Check if the collision object has a MeshDeformer component
        MeshDeformer meshDeformer = collision.collider.GetComponent<MeshDeformer>();
        if (meshDeformer != null)
        {
            // Use the fireball's impactRadius for the deformation
            meshDeformer.ApplyDeformation(transform.position, impactRadius);
        }
        // Destroy the fireball after impact
        Destroy(gameObject);
    }

    void Start()
    {
        // After instantiating the fireball...
        ApplyWindEffect();
    }

    private void ApplyWindEffect()
    {
        Rigidbody rb = GetComponent<Rigidbody>();

        // Get the WindManager instance from the scene (make sure there is one WindManager object in the scene)
        WindManager windManager = FindObjectOfType<WindManager>();
        
        // Adjust the fireball's velocity based on the wind
        Vector3 windForce = new Vector3(windManager.windDirection.x, windManager.windDirection.y, 0) * windManager.windStrength;
        rb.velocity += windForce; // This adds the wind force to the current velocity
    }
}
