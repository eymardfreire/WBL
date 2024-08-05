using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BearMeteorCollisionHandler : MonoBehaviour
{
    private float minDamage;
    private float maxDamage;
    public float summonHeight = 20f; // The height above the impact point where the bear spawns
    public float crashForce = 1f; // The force with which the bear crashes down
    public GameObject collisionEffectPrefab; // Assign this through the inspector
    private float impactRadius;
    private float deformRadius;
    private GameObject bearPrefab;
    private Vector3 bearSummonScale;
    private GameObject explosionEffectPrefab;
    private Rigidbody rb;

    public void Setup(float minDamage, float maxDamage, float impactRadius, float deformRadius, GameObject bearPrefab, Vector3 bearSummonScale, GameObject explosionEffectPrefab)
    {
        this.minDamage = minDamage;
        this.maxDamage = maxDamage;
        this.impactRadius = impactRadius;
        this.deformRadius = deformRadius;
        this.bearPrefab = bearPrefab;
        this.bearSummonScale = bearSummonScale;
        this.explosionEffectPrefab = explosionEffectPrefab;
    }

    void OnCollisionEnter(Collision collision)
    {
        // Play the collision effect at the point of contact
        if (collisionEffectPrefab != null)
        {
            // Instantiate the effect and optionally destroy it after some time
            GameObject effectInstance = Instantiate(collisionEffectPrefab, collision.contacts[0].point, Quaternion.identity);
            Destroy(effectInstance, 2f); // For example, 2 seconds lifetime
        }

        // This will now summon the bear regardless of what the orb collides with.
        SummonBear(collision.contacts[0].point);
        Destroy(gameObject); // Destroy the orb after summoning the bear
    }

 void SummonBear(Vector3 impactPoint)
{
    Vector3 summonPosition = impactPoint + Vector3.up * summonHeight; // The height above the impact point
    GameObject bearInstance = Instantiate(bearPrefab, summonPosition, Quaternion.identity);
    Rigidbody bearRb = bearInstance.GetComponent<Rigidbody>();

    // Ensure there's a Rigidbody attached to the bear prefab
    if (!bearRb)
    {
        bearRb = bearInstance.AddComponent<Rigidbody>();
    }

    // Set the bear's initial scale and properties
    bearInstance.transform.localScale = bearSummonScale;
    BearImpactHandler bearImpactHandler = bearInstance.AddComponent<BearImpactHandler>();
    bearImpactHandler.explosionEffectPrefab = explosionEffectPrefab;
    bearImpactHandler.minDamage = minDamage;
    bearImpactHandler.maxDamage = maxDamage;
    bearImpactHandler.impactRadius = impactRadius;
    bearImpactHandler.deformRadius = deformRadius;

    // Apply a force to simulate the bear crashing down
    bearRb.isKinematic = false; // Make sure the Rigidbody is not kinematic
    bearRb.useGravity = true; // Ensure gravity is on so it will fall
    bearRb.AddForce(Vector3.down * crashForce, ForceMode.Impulse); // crashForce is a variable you define for how strong the crash should be
}



}

