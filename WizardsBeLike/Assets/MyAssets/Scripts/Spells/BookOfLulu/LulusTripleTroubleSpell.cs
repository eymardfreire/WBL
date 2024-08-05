using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New LulusTripleTroubleSpell", menuName = "Spells/LulusTripleTroubleSpell")]
public class LulusTripleTroubleSpell : Spell
{
    public GameObject luluPrefab; // Assign Lulu's prefab through the inspector
    public GameObject fecesBombPrefab; // Assign the feces bomb prefab through the inspector
    public GameObject bombReleaseEffectPrefab; // Assign the bomb release particle effect prefab through the inspector
    public float minDamage = 30f;
    public float maxDamage = 30f;
    public float impactRadius = 1f;
    public GameObject explosionEffectPrefab; // Assign in the inspector
    public float recoilForce = 5f; // Public variable for the recoil force, adjustable from the inspector

    // Time intervals for each bomb release
    public float[] bombReleaseTimes = new float[] { 1.0f, 1.5f, 2.0f }; // Adjust these values as needed for the desired timing

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

        // Start the BombDrop coroutine on the MagicalWeapon's MonoBehaviour
        magicalWeapon.StartCoroutine(BombDropSequence(luluRb, bombReleaseEffectPrefab, fecesBombPrefab, currentCastingPower));
    }

    private IEnumerator BombDropSequence(Rigidbody luluRb, GameObject bombReleaseEffect, GameObject bombPrefab, float currentCastingPower)
    {
        foreach (float releaseTime in bombReleaseTimes)
        {
            // Wait for the specified release time
            yield return new WaitForSeconds(releaseTime);

            // Check if Lulu is still in the game (not destroyed)
            if (luluRb != null)
            {
                // Instantiate the feces bomb at Lulu's current position without any rotation
                GameObject fecesBombInstance = Instantiate(bombPrefab, luluRb.position, Quaternion.identity);

                // Instantiate the particle effect at Lulu's current position
                if (bombReleaseEffect != null)
                {
                    Instantiate(bombReleaseEffect, luluRb.position, Quaternion.identity);
                }

                // Apply forces to the bomb if needed, otherwise it will just drop straight down
                Rigidbody bombRb = fecesBombInstance.GetComponent<Rigidbody>();
                if (bombRb != null)
                {
                    bombRb.constraints = RigidbodyConstraints.FreezePositionZ;
                    bombRb.velocity = new Vector3(0, -currentCastingPower, 0); // Adjust as needed
                }

                // Attach a script to handle collision events of the feces bomb if needed
                FecesBombCollisionHandler collisionHandler = fecesBombInstance.AddComponent<FecesBombCollisionHandler>();
                collisionHandler.Setup(minDamage, maxDamage, impactRadius, explosionEffectPrefab);

                // Apply the recoil force to Lulu's Rigidbody to simulate the effect of dropping the bomb
                luluRb.AddForce(new Vector3(0, recoilForce, 0), ForceMode.Impulse);
            }
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
