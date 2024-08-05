using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// A specific spell type for Lulu's Aerial Drop
[CreateAssetMenu(fileName = "New LuluSpell", menuName = "Spells/LuluSpell")]
public class LuluSpell : Spell
{
    public GameObject luluPrefab; // Assign Lulu's prefab through the inspector
    public GameObject fecesBombPrefab; // Assign the feces bomb prefab through the inspector
    public float bombReleaseTime = 2f; // Time in seconds after which the bomb will be released
    public GameObject bombReleaseEffectPrefab; // Assign the bomb release particle effect prefab through the inspector
    public GameObject explosionEffectPrefab; // Assign in the inspector
    public float impactRadius = 1f; // Set your own values
    public float minDamage = 80f; // Set your own values
    public float maxDamage = 100f; // Set your own values
    public float upwardForce = 5f; // Public variable for the upward force


    public override void Cast(Transform castingPoint, PlayerMovement playerMovement, MagicalWeapon magicalWeapon, float currentCastingPower)
    {
        WindManager windManager = WindManager.Instance; // Get the singleton instance of WindManager

        // Instantiate Lulu at the casting point without any rotation
        GameObject luluInstance = Instantiate(luluPrefab, castingPoint.position, Quaternion.identity);

        // Calculate the initial velocity based on the casting power and weapon angle
        float angleInRadians = magicalWeapon.CurrentAngle * Mathf.Deg2Rad;
        Vector2 direction = new Vector2(Mathf.Cos(angleInRadians), Mathf.Sin(angleInRadians));

        // Determine the character's current forward direction
        Vector3 characterForward = playerMovement.transform.forward;
        characterForward.z = 0; // Ignore the z-axis as we're working in 2D

        // Apply the initial velocity to Lulu's Rigidbody component
        Rigidbody luluRb = luluInstance.GetComponent<Rigidbody>();
        if (luluRb != null)
        {
            // Adjust the x component of the direction based on the character's forward direction
            float correctXDirection = characterForward.x > 0 ? -direction.x : direction.x;
            luluRb.velocity = new Vector3(correctXDirection * currentCastingPower, direction.y * currentCastingPower, 0f);

            // Apply wind effect with delay if WindManager is available
            if (windManager != null)
            {
                Vector3 windForce = new Vector3(windManager.windDirection.x, windManager.windDirection.y, 0) * windManager.windStrength;
                // Start coroutine to apply wind over time
                playerMovement.StartCoroutine(ApplyWindOverTime(luluRb, windForce));
            }
        }
        //Start the BombRelease coroutine on the MagicalWeapon's MonoBehaviour
        magicalWeapon.StartCoroutine(BombRelease(luluRb, castingPoint, currentCastingPower));
    }

    private IEnumerator BombRelease(Rigidbody luluRb, Transform castingPoint, float currentCastingPower)
    {
        // Wait for the specified time
        yield return new WaitForSeconds(bombReleaseTime);

        // Check if Lulu is still in the game (not destroyed)
        if (luluRb != null)
        {
            // Instantiate the feces bomb at Lulu's current position without any rotation
            GameObject fecesBombInstance = Instantiate(fecesBombPrefab, luluRb.position, Quaternion.identity);

            Rigidbody bombRb = fecesBombInstance.GetComponent<Rigidbody>();
            if (bombRb != null)
            {
                // Instantiate the particle effect at Lulu's current position without any rotation
                if (bombReleaseEffectPrefab != null)
                {
                    Instantiate(bombReleaseEffectPrefab, luluRb.position, Quaternion.identity);
                }

                // Add the collision handler to the feces bomb
                FecesBombCollisionHandler collisionHandler = fecesBombInstance.AddComponent<FecesBombCollisionHandler>();
                collisionHandler.Setup(minDamage, maxDamage, impactRadius, explosionEffectPrefab);

                // Constrain Rigidbody to prevent any movement along the Z-axis
                bombRb.constraints = RigidbodyConstraints.FreezePositionZ;

                // Set the bomb's velocity to be downward in the Y-axis only
                bombRb.velocity = new Vector3(0, -currentCastingPower, 0);
            }

            // Apply a recoil force to Lulu's Rigidbody to simulate the effect of dropping the bomb
            luluRb.AddForce(new Vector3(0, upwardForce, 0), ForceMode.Impulse);
        }
    }

    private IEnumerator ApplyWindOverTime(Rigidbody luluRb, Vector3 windForce)
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
            if (luluRb == null) yield break;

            // Lerp from 0 to 1 over windRampUpTime seconds
            float t = time / windRampUpTime;
            // Apply a fraction of the wind force based on the lerp value
            luluRb.velocity += windForce * t * Time.deltaTime;
            time += Time.deltaTime;
            yield return null;
        }

        // Check again, because the object might have been destroyed during the last loop iteration
        if (luluRb != null)
        {
            luluRb.velocity += windForce * (1 - time / windRampUpTime);
        }
    }

}

