using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarketMayhemCollisionHandler : MonoBehaviour
{
    public GameObject bullPrefab; // Assign through the inspector
    public GameObject bearPrefab; // Assign through the inspector
    public GameObject explosionEffectPrefab; // Assign through the inspector
    public float bullMinDamage;
    public float bullMaxDamage;
    public float bullImpactRadius;
    public float deformRadius; // Radius for deforming the mesh
    public Vector3 bearSummonScale; // The scale at which to summon the bear
    public float bearMinDamage;
    public float bearMaxDamage;
    public float bearImpactRadius;
    public GameObject bearCollisionEffectPrefab; // Assign through the inspector
    public float delayBeforeSummoningBear; // Time delay before bear is summoned
    public float summonHeight = 20f; // Adjust this value as needed for your game
    public float crashForce = 1f;    // Adjust this value as needed for your game

    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void Setup(float bullMinDamage, float bullMaxDamage, float bullImpactRadius, float deformRadius, GameObject explosionEffectPrefab, GameObject bearPrefab, Vector3 bearSummonScale, float bearMinDamage, float bearMaxDamage, float bearImpactRadius, GameObject bearCollisionEffectPrefab, float delayBeforeSummoningBear)
    {
        this.bullMinDamage = bullMinDamage;
        this.bullMaxDamage = bullMaxDamage;
        this.bullImpactRadius = bullImpactRadius;
        this.deformRadius = deformRadius;
        this.explosionEffectPrefab = explosionEffectPrefab;
        this.bearPrefab = bearPrefab;
        this.bearSummonScale = bearSummonScale;
        this.bearMinDamage = bearMinDamage;
        this.bearMaxDamage = bearMaxDamage;
        this.bearImpactRadius = bearImpactRadius;
        this.bearCollisionEffectPrefab = bearCollisionEffectPrefab;
        this.delayBeforeSummoningBear = delayBeforeSummoningBear;
    }

    void OnCollisionEnter(Collision collision)
{
    // Immediately explode on impact and apply damage
    Explode();

    // Summon the bear immediately after the bull explodes, without a delay
    SummonBearImmediately();
}

    private void Explode()
    {
        // Instantiate the explosion effect at the point of collision
        if (explosionEffectPrefab != null)
        {
            Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);
        }

        // Apply area-of-effect damage
        ApplyDamage(bullMinDamage, bullMaxDamage, bullImpactRadius);

        // Destroy the bull object after explosion
        Destroy(gameObject);
    }

   private void SummonBearImmediately()
{
    Vector3 summonPosition = transform.position + Vector3.up * summonHeight;
    //Debug.Log("Summoning bear at position: " + summonPosition);

    GameObject bearInstance = Instantiate(bearPrefab, summonPosition, Quaternion.identity);

    if (bearInstance == null)
    {
        //Debug.LogError("Failed to instantiate bear prefab.");
        return;
    }

    bearInstance.transform.localScale = bearSummonScale;

    BearImpactHandler bearImpactHandler = bearInstance.AddComponent<BearImpactHandler>();
    bearImpactHandler.Setup(bearMinDamage, bearMaxDamage, bearImpactRadius, deformRadius, bearCollisionEffectPrefab);

    Rigidbody bearRb = bearInstance.GetComponent<Rigidbody>() ?? bearInstance.AddComponent<Rigidbody>();
    bearRb.isKinematic = false;
    bearRb.useGravity = true;
    bearRb.AddForce(Vector3.down * crashForce, ForceMode.Impulse);
}


    private void ApplyDamage(float minDamage, float maxDamage, float radius)
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, radius);
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

            // Optionally perform mesh deformation
            MeshDeformer meshDeformer = hitCollider.GetComponent<MeshDeformer>();
            if (meshDeformer != null)
            {
                meshDeformer.ApplyDeformation(transform.position, deformRadius);
            }
        }
    }
}