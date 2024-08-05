using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New BearMeteorSpell", menuName = "Spells/BearMeteorSpell")]
public class BearMeteorSpell : Spell
{
    public float minDamage = 70f;
    public float maxDamage = 90f;
    public float impactRadius; // Radius for applying damage
    public float deformRadius; // Radius for deforming the mesh
    public GameObject orbPrefab; // Assign the orb prefab through the inspector
    public GameObject bearPrefab; // Assign the bear prefab through the inspector
    public GameObject explosionEffectPrefab; // Assign this through the inspector
    public GameObject collisionEffectPrefab; // Assign this through the inspector
    public Vector3 bearSummonScale = new Vector3(1, 1, 1); // Adjust the bear summon scale as needed

    public override void Cast(Transform castingPoint, PlayerMovement playerMovement, MagicalWeapon magicalWeapon, float currentCastingPower)
    {
        WindManager windManager = WindManager.Instance; // Get the singleton instance of WindManager

        // Instantiate the orb at the casting point without any rotation
        GameObject orbInstance = Instantiate(orbPrefab, castingPoint.position, Quaternion.identity);

        // Calculate the initial velocity based on the casting power and weapon angle
        float angleInRadians = magicalWeapon.CurrentAngle * Mathf.Deg2Rad;
        Vector2 direction = new Vector2(Mathf.Cos(angleInRadians), Mathf.Sin(angleInRadians));

        // Determine the character's current forward direction
        Vector3 characterForward = playerMovement.transform.forward;
        characterForward.z = 0; // Ignore the z-axis as we're working in 2D

        // Apply the initial velocity to the orb's Rigidbody component
        Rigidbody orbRb = orbInstance.GetComponent<Rigidbody>();
        if (orbRb != null)
        {
            // Adjust the x component of the direction based on the character's forward direction
            float correctXDirection = characterForward.x > 0 ? -direction.x : direction.x;
            orbRb.velocity = new Vector3(correctXDirection * currentCastingPower, direction.y * currentCastingPower, 0f);

            if (windManager != null)
            {
                Vector3 windForce = new Vector3(windManager.windDirection.x, windManager.windDirection.y, 0) * windManager.windStrength;
                // Start coroutine to apply wind over time
                playerMovement.StartCoroutine(ApplyWindOverTime(orbRb, windForce));
            }
        }

        // Assign the orb's collision handling script
        BearMeteorCollisionHandler collisionHandler = orbInstance.AddComponent<BearMeteorCollisionHandler>();
        collisionHandler.Setup(minDamage, maxDamage, impactRadius, deformRadius, bearPrefab, bearSummonScale, explosionEffectPrefab);
        collisionHandler.collisionEffectPrefab = collisionEffectPrefab; // Assign the particle effect here

    }
    private IEnumerator ApplyWindOverTime(Rigidbody orbRb, Vector3 windForce)
    {
        // Delay before starting to apply wind
        float windApplicationDelay = 0.5f; // Adjust the delay to your preference
        yield return new WaitForSeconds(windApplicationDelay);

        // Duration over which wind reaches full strength
        float windRampUpTime = 1f; // Adjust the ramp-up time to your preference
        float time = 0;

        while (time < windRampUpTime)
        {
            // Check if the Rigidbody still exists before accessing it
            if (orbRb == null) yield break;

            // Lerp from 0 to 1 over windRampUpTime seconds
            float t = time / windRampUpTime;
            // Apply a fraction of the wind force based on the lerp value
            orbRb.velocity += windForce * t * Time.deltaTime;
            time += Time.deltaTime;
            yield return null;
        }

        // Check again, because the object might have been destroyed during the last loop iteration
        if (orbRb != null)
        {
            orbRb.velocity += windForce * (1 - time / windRampUpTime);
        }
    }

}

