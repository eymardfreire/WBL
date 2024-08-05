using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New BullRushSpell", menuName = "Spells/BullRushSpell")]
public class BullRushSpell : Spell
{
    public float minDamage = 40f;
    public float maxDamage = 60f;
    public float impactRadius; // Radius for applying damage
    public float deformRadius; // Radius for deforming the mesh
    public GameObject explosionEffectPrefab; // Assign this through the inspector
    public float delayBeforeExplosion = 2f; // Time before the bull explodes after the first impact

    // Other properties and methods remain the same as FireballSpell...
    
    public override void Cast(Transform castingPoint, PlayerMovement playerMovement, MagicalWeapon magicalWeapon, float currentCastingPower)
    {
        WindManager windManager = WindManager.Instance; // Get the singleton instance of WindManager

        // Instantiate the bull at the casting point without any rotation
        GameObject bullInstance = Instantiate(effectPrefab, castingPoint.position, Quaternion.identity);

        // Calculate the initial velocity based on the casting power and weapon angle
        float angleInRadians = magicalWeapon.CurrentAngle * Mathf.Deg2Rad;
        Vector2 direction = new Vector2(Mathf.Cos(angleInRadians), Mathf.Sin(angleInRadians));

        // Determine the character's current forward direction
        Vector3 characterForward = playerMovement.transform.forward;
        characterForward.z = 0; // Ignore the z-axis as we're working in 2D

        // Apply the initial velocity to the bull's Rigidbody component
        Rigidbody bullRb = bullInstance.GetComponent<Rigidbody>();
        if (bullRb != null)
        {
            // Adjust the x component of the direction based on the character's forward direction
            float correctXDirection = characterForward.x > 0 ? -direction.x : direction.x;
            bullRb.velocity = new Vector3(correctXDirection * currentCastingPower, direction.y * currentCastingPower, 0f);

            // Apply wind effect with delay if WindManager is available
            if (windManager != null)
            {
                Vector3 windForce = new Vector3(windManager.windDirection.x, windManager.windDirection.y, 0) * windManager.windStrength;
                // Start coroutine to apply wind over time
                playerMovement.StartCoroutine(ApplyWindOverTime(bullRb, windForce));
            }
        }

        // Add the BullRushCollisionHandler component to the bull instance and pass all required arguments
        BullRushCollisionHandler collisionHandler = bullInstance.AddComponent<BullRushCollisionHandler>();
        collisionHandler.Setup(minDamage, maxDamage, impactRadius, deformRadius, explosionEffectPrefab, delayBeforeExplosion);
    }

    private IEnumerator ApplyWindOverTime(Rigidbody bullRb, Vector3 windForce)
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
            if (bullRb == null) yield break;

            // Lerp from 0 to 1 over windRampUpTime seconds
            float t = time / windRampUpTime;
            // Apply a fraction of the wind force based on the lerp value
            bullRb.velocity += windForce * t * Time.deltaTime;
            time += Time.deltaTime;
            yield return null;
        }

        // Check again, because the object might have been destroyed during the last loop iteration
        if (bullRb != null)
        {
            bullRb.velocity += windForce * (1 - time / windRampUpTime);
        }
    }
}
