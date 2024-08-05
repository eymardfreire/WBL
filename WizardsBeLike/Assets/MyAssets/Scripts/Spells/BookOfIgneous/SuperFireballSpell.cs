using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// A specific spell type for the super fireball
[CreateAssetMenu(fileName = "New SuperFireballSpell", menuName = "Spells/SuperFireballSpell")]
public class SuperFireballSpell : Spell, ISuperSpell
{
    public float minDamage = 80f;
    public float maxDamage = 120f;
    public float impactRadius = 2f;
    public GameObject explosionEffectPrefab; // This should be assigned with a larger fireball prefab through the inspector
    public float timeScaleDuringCast = 0.5f;
    public float cooldownTime = 60f; // Cooldown time for this specific super spell
    public float CooldownTime => cooldownTime;

    //public Light directionalLight;
    //public Material skyboxMaterial;

    public override void Cast(Transform castingPoint, PlayerMovement playerMovement, MagicalWeapon magicalWeapon, float currentCastingPower)
    {
        SpellCasting spellCastingComponent = playerMovement.GetComponent<SpellCasting>();
        if (spellCastingComponent != null)
        {
            spellCastingComponent.StartCoroutine(CastSequence(castingPoint, playerMovement, magicalWeapon, currentCastingPower));
            //spellCastingComponent.StartSuperSpellCooldown(cooldownTime); // Pass the cooldown time

        }
    }

    private IEnumerator CastSequence(Transform castingPoint, PlayerMovement playerMovement, MagicalWeapon magicalWeapon, float currentCastingPower)
    {
        // Find the active Directional Light in the scene
        Light currentDirectionalLight = FindObjectOfType<Light>();
        float originalLightIntensity = currentDirectionalLight.intensity;
        Material currentSkyboxMaterial = RenderSettings.skybox; // Get the current skybox material
        float originalExposure = currentSkyboxMaterial.GetFloat("_Exposure");
        float originalTimeScale = Time.timeScale;

        // Darken the scene and slow down time
        Time.timeScale = timeScaleDuringCast;
        for (float t = 0; t < 1; t += Time.unscaledDeltaTime)
        {
            currentDirectionalLight.intensity = Mathf.Lerp(originalLightIntensity, 0, t);
            currentSkyboxMaterial.SetFloat("_Exposure", Mathf.Lerp(originalExposure, 0, t));
            yield return null;
        }

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
        }

        // Attach a script to the fireball instance to handle collision events and apply damage
        FireballCollisionHandler collisionHandler = fireballInstance.AddComponent<FireballCollisionHandler>();
        collisionHandler.Setup(minDamage, maxDamage, impactRadius, explosionEffectPrefab);

        // Restore original scene lighting and time scale
        for (float t = 0; t < 1; t += Time.unscaledDeltaTime)
        {
            currentDirectionalLight.intensity = Mathf.Lerp(0, originalLightIntensity, t);
            currentSkyboxMaterial.SetFloat("_Exposure", Mathf.Lerp(0, originalExposure, t));
            Time.timeScale = Mathf.Lerp(timeScaleDuringCast, originalTimeScale, t);
            yield return null;
        }

        // Ensure that the time scale and other settings are set back to normal
        Time.timeScale = originalTimeScale;
        currentDirectionalLight.intensity = originalLightIntensity;
        currentSkyboxMaterial.SetFloat("_Exposure", originalExposure);
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
