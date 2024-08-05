using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// A specific spell type for the scatter fireballs
[CreateAssetMenu(fileName = "New ScatterFireballSpell", menuName = "Spells/ScatterFireballSpell")]
public class ScatterFireballSpell : Spell
{
    public float minDamage = 30f; // Adjusted to match Scatter Fireball damage
    public float maxDamage = 30f; // Assuming fixed damage for simplicity
    public float impactRadius = 1f;
    public GameObject explosionEffectPrefab; // Assign this through the inspector

    public override void Cast(Transform castingPoint, PlayerMovement playerMovement, MagicalWeapon magicalWeapon, float currentCastingPower)
    {
        // Start the coroutine on the MagicalWeapon's MonoBehaviour, as this scriptable object does not have its own MonoBehaviour
        magicalWeapon.StartCoroutine(CastSequence(castingPoint, playerMovement, magicalWeapon, currentCastingPower));
    }

    private IEnumerator CastSequence(Transform castingPoint, PlayerMovement playerMovement, MagicalWeapon magicalWeapon, float currentCastingPower)
    {
        WindManager windManager = WindManager.Instance; // Get the singleton instance of WindManager

        // Define different speed multipliers for each fireball
        float[] speedMultipliers = new float[] { 1.0f, 1.2f, 1.4f }; // Adjust these values as needed for the desired spread

        // Repeat the process 3 times for 3 fireballs
        for (int i = 0; i < 3; i++)
        {
            GameObject fireballInstance = Instantiate(effectPrefab, castingPoint.position, Quaternion.identity);
            Rigidbody fireballRb = fireballInstance.GetComponent<Rigidbody>();

            if (fireballRb != null)
            {
                float angleInRadians = magicalWeapon.CurrentAngle * Mathf.Deg2Rad;
                Vector2 direction = new Vector2(Mathf.Cos(angleInRadians), Mathf.Sin(angleInRadians));
                Vector3 characterForward = playerMovement.transform.forward;
                characterForward.z = 0; // 2D game assumption

                float correctXDirection = characterForward.x > 0 ? -direction.x : direction.x;

                // Apply the speed multiplier to the current casting power for each fireball
                float speedAdjustment = currentCastingPower * speedMultipliers[i];

                fireballRb.velocity = new Vector3(correctXDirection * speedAdjustment, direction.y * speedAdjustment, 0f);

                // Apply wind effect with delay if WindManager is available
                if (windManager != null)
                {
                    Vector3 windForce = new Vector3(windManager.windDirection.x, windManager.windDirection.y, 0) * windManager.windStrength;
                    // Start coroutine to apply wind over time
                    playerMovement.StartCoroutine(ApplyWindOverTime(fireballRb, windForce));
                }
            }

            FireballCollisionHandler collisionHandler = fireballInstance.AddComponent<FireballCollisionHandler>();
            collisionHandler.Setup(minDamage, maxDamage, impactRadius, explosionEffectPrefab);

            // Wait for a very short time before instantiating the next fireball
            yield return new WaitForSeconds(0.5f); // Adjust time as needed
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