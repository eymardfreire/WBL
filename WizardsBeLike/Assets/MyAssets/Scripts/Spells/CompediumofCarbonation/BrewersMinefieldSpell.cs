using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New BrewersMinefieldSpell", menuName = "Spells/BrewersMinefieldSpell")]
public class BrewersMinefieldSpell : Spell
{
    public float minDamage = 40f;
    public float maxDamage = 60f;
    public float impactRadius; // Radius for applying damage
    public float deformRadius; // Radius for deforming the mesh
    public GameObject explosionEffectPrefab; // Assign this through the inspector
    public GameObject[] piecePrefabs;
    public float explosionForce = 1000f; // Adjust the force as needed
    public float explosionRadius = 5f; // Adjust the radius as needed
    public Vector3 explosionOffset = new Vector3(0, 1, 0); // Adjust the offset as needed
    public GameObject stickingEffectPrefab;

    // Other properties and methods remain the same as FireballSpell...

    public override void Cast(Transform castingPoint, PlayerMovement playerMovement, MagicalWeapon magicalWeapon, float currentCastingPower)
    {
        WindManager windManager = WindManager.Instance; // Get the singleton instance of WindManager

        // Instantiate the barrel at the casting point without any rotation
        GameObject barrelInstance = UnityEngine.Object.Instantiate(effectPrefab, castingPoint.position, Quaternion.identity);

        // Calculate the initial velocity based on the casting power and weapon angle
        float angleInRadians = magicalWeapon.CurrentAngle * Mathf.Deg2Rad;
        Vector2 direction = new Vector2(Mathf.Cos(angleInRadians), Mathf.Sin(angleInRadians));

        // Determine the character's current forward direction
        Vector3 characterForward = playerMovement.transform.forward;
        characterForward.z = 0; // Ignore the z-axis as we're working in 2D

        // Apply the initial velocity to the barrel's Rigidbody component
        Rigidbody barrelRb = barrelInstance.GetComponent<Rigidbody>();
        if (barrelRb != null)
        {
            // Adjust the x component of the direction based on the character's forward direction
            float correctXDirection = characterForward.x > 0 ? -direction.x : direction.x;
            barrelRb.velocity = new Vector3(correctXDirection * currentCastingPower, direction.y * currentCastingPower, 0f);

            // Apply wind effect with delay if WindManager is available
            if (windManager != null)
            {
                Vector3 windForce = new Vector3(windManager.windDirection.x, windManager.windDirection.y, 0) * windManager.windStrength;
                // Start coroutine to apply wind over time
                playerMovement.StartCoroutine(ApplyWindOverTime(barrelRb, windForce));
            }
        }

        // Add the BrewersMinefieldCollisionHandler component to the barrel instance and pass all required arguments
        BrewersMinefieldCollisionHandler collisionHandler = barrelInstance.AddComponent<BrewersMinefieldCollisionHandler>();
        collisionHandler.Setup(minDamage, maxDamage, impactRadius, deformRadius, explosionEffectPrefab, stickingEffectPrefab);
        collisionHandler.piecePrefabs = this.piecePrefabs;
    }

    private IEnumerator ApplyWindOverTime(Rigidbody barrelRb, Vector3 windForce)
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
            if (barrelRb == null) yield break;

            // Lerp from 0 to 1 over windRampUpTime seconds
            float t = time / windRampUpTime;
            // Apply a fraction of the wind force based on the lerp value
            barrelRb.velocity += windForce * t * Time.deltaTime;
            time += Time.deltaTime;
            yield return null;
        }

        // Check again, because the object might have been destroyed during the last loop iteration
        if (barrelRb != null)
        {
            barrelRb.velocity += windForce * (1 - time / windRampUpTime);
        }
    }
}