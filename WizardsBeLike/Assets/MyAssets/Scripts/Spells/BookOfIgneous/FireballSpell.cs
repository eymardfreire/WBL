using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// A specific spell type for the fireball
[CreateAssetMenu(fileName = "New FireballSpell", menuName = "Spells/FireballSpell")]
public class FireballSpell : Spell
{
    public float minDamage = 40f;
    public float maxDamage = 60f;
    public float impactRadius = 1f;
    public GameObject explosionEffectPrefab; // Assign this through the inspector

    public override void Cast(Transform castingPoint, PlayerMovement playerMovement, MagicalWeapon magicalWeapon, float currentCastingPower)
    {
        WindManager windManager = WindManager.Instance; // Get the singleton instance of WindManager

        // Instantiate the fireball at the casting point without any rotation
        GameObject fireballInstance = UnityEngine.Object.Instantiate(effectPrefab, castingPoint.position, Quaternion.identity);

        // Calculate the initial velocity based on the casting power and weapon angle
        float angleInRadians = magicalWeapon.CurrentAngle * Mathf.Deg2Rad;
        Vector2 direction = new Vector2(Mathf.Cos(angleInRadians), Mathf.Sin(angleInRadians));

        // Determine the character's current forward direction
        Vector3 characterForward = playerMovement.transform.forward;
        characterForward.z = 0; // Ignore the z-axis as we're working in 2D

        // Apply the initial velocity to the fireball's Rigidbody component
        Rigidbody fireballRb = fireballInstance.GetComponent<Rigidbody>();
        if (fireballRb != null)
        {
            // Adjust the x component of the direction based on the character's forward direction
            float correctXDirection = characterForward.x > 0 ? -direction.x : direction.x;
            fireballRb.velocity = new Vector3(correctXDirection * currentCastingPower, direction.y * currentCastingPower, 0f);

            // Apply wind effect with delay if WindManager is available
            if (windManager != null)
            {
                Vector3 windForce = new Vector3(windManager.windDirection.x, windManager.windDirection.y, 0) * windManager.windStrength;
                // Start coroutine to apply wind over time
                playerMovement.StartCoroutine(ApplyWindOverTime(fireballRb, windForce));
            }


            // Attach a script to the fireball instance to handle collision events and apply damage
            FireballCollisionHandler collisionHandler = fireballInstance.AddComponent<FireballCollisionHandler>();
        collisionHandler.Setup(minDamage, maxDamage, impactRadius, explosionEffectPrefab);
        }
    }

    private IEnumerator ApplyWindOverTime(Rigidbody fireballRb, Vector3 windForce)
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
            if (fireballRb == null) yield break;

            // Lerp from 0 to 1 over windRampUpTime seconds
            float t = time / windRampUpTime;
            // Apply a fraction of the wind force based on the lerp value
            fireballRb.velocity += windForce * t * Time.deltaTime;
            time += Time.deltaTime;
            yield return null;
        }

        // Check again, because the object might have been destroyed during the last loop iteration
        if (fireballRb != null)
        {
            fireballRb.velocity += windForce * (1 - time / windRampUpTime);
        }
    }
}